using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading.Tasks;

namespace UserBio
{
    /// <summary>
    /// ユーザー認証データベースのアクセサ。
    /// 大抵、トランザクションしていないため、競合状態には注意されたい。
    /// というのも、書き込むのは登録アプリだけのはずで、コイツが多重起動しない限り、読み取りは複数プロセスから可能になる(予定)。
    /// </summary>
    public class UserDatabase : IDisposable
    {
        // Properties.
        public string DatabaseFile { get; set; }

        // Public constants.
        public const int DatabaseVersion = 1;

        public bool HasChangePermission { get; set; } = true;

        public bool IsReadOnly { get; set; } = false;

        private SqliteConnection connection = null;

        public void Dispose()
        {
            try
            {
                connection.Dispose();
            }
            catch
            {
            }
        }

        public UserDatabase()
        {
            var dir = GetStorePath();
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var filePath = Path.Combine(dir, @"userEnrolldata.sqlite");
            CreateDatabaseFile(filePath);

            try 
            { 
                SetEveryoneFullControl(filePath);
            }
            catch (Exception)
            {
                // Permission 変更権限がない
                HasChangePermission = false;
            }

            DatabaseFile = filePath;
        }

        /// <summary>
        /// テスト用にデータベースファイルを指定するコンストラクタ。
        /// ファイルの存在はチェックしないし、途中のディレクトリは作らない。
        /// </summary>
        /// <param name="file"></param>
        public UserDatabase(string file)
        {
            DatabaseFile = file;
        }

        public string GetStorePath()
        {
            return Path.Combine(new string[] {
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "ISSUI",
                "RealtimeViewer",
                "Common",
            });
        }

        public void Connect()
        {
            var connectString = new SqliteConnectionStringBuilder { DataSource = DatabaseFile };
            connection = new SqliteConnection(connectString.ToString());
        }

        /// <summary>
        /// 空のデータベースファイルを作成する
        /// </summary>
        /// <param name="filePath"></param>
        private void CreateDatabaseFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                using (var stream = File.Create(filePath))
                {
                    ;
                }
            }
        }

        /// <summary>
        /// 指定のファイルにEveryone Fullcontrol権を与える
        /// </summary>
        /// <param name="filePath"></param>
        private void SetEveryoneFullControl(string filePath)
        {
            var accessRule = new FileSystemAccessRule(
                    new NTAccount("everyone"), FileSystemRights.Modify, AccessControlType.Allow);
            if (File.Exists(filePath))
            {
                var security = File.GetAccessControl(filePath);
                security.AddAccessRule(accessRule);
                File.SetAccessControl(filePath, security);
            }
        }

        /// <summary>
        /// 初期状態のデータベースを作る。
        /// テーブルやデフォルト値など。
        /// 想定としては、登録アプリからのみ呼び出される。
        /// </summary>
        public void CreateDefault()
        {
            try
            {
                // データベース情報テーブル。現状では使わないが、将来アップデート用として。
                CreateDatabaseInfoTable();
                CreateDatabaseInfoRecord();

                // ロール。
                CreateRoleTable();
                CreateRoleRecord();

                // ユーザー。
                CreateUserTable();
            }
            catch (Microsoft.Data.Sqlite.SqliteException se)
            {
                // Readonly database
                if (se.SqliteExtendedErrorCode == 8)
                {
                    // through
                    IsReadOnly = true;
                }
                else
                {
                    throw;
                }
            }
        }

        public void CreateDatabaseInfoTable()
        {
            connection.Open();

            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS dbinfo (
    version TEXT UNIQUE,
    update_at DATETIME
);
";
                cmd.ExecuteNonQuery();
            }

            connection.Close();
        }

        public void CreateDatabaseInfoRecord()
        {
            connection.Open();

            // DatabaseVersion と同値のレコードが存在しない時、レコードを追加する。
            // なお、将来migrationを考えるなら、この方式は不適切である。問答無用で追加してしまうため。
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"
INSERT INTO dbinfo (version, update_at) 
SELECT $version, $update_at
WHERE NOT EXISTS (SELECT 1 FROM dbinfo WHERE version = $version);
";
                cmd.Parameters.AddWithValue("$version", DatabaseVersion);
                cmd.Parameters.AddWithValue("$update_at", DateTime.Now);

                cmd.ExecuteNonQuery();
            }

            connection.Close();
        }

        public void CreateRoleTable()
        {
            connection.Open();

            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS roles (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT,
    permission INTEGER,
    memo TEXT
);
";
                cmd.ExecuteNonQuery();
            }

            connection.Close();
        }

        /// <summary>
        /// roles テーブルに1行もデータが無いときだけ、デフォルトのロールレコードを追加する。
        /// </summary>
        public void CreateRoleRecord()
        {
            connection.Open();

            int recordCount = 0;
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT COUNT(id) FROM roles";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        recordCount = reader.GetInt32(0);
                        break;
                    }
                }
            }

            if (recordCount == 0)
            {
                using (SqliteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO roles (name, permission) VALUES ($name, $permission)";
                    cmd.Parameters.Add(@"$name", SqliteType.Text);
                    cmd.Parameters.Add(@"$permission", SqliteType.Integer);

                    var roles = new DefaultRoleList();
                    foreach (var r in roles.Roles)
                    {
                        cmd.Parameters[@"$name"].Value = r.Name;
                        cmd.Parameters[@"$permission"].Value = (int)r.Permission;
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            connection.Close();
        }

        /// <summary>
        /// ユーザー管理テーブルを作る。
        /// せいぜい数十レコードを想定している。
        /// </summary>
        public void CreateUserTable()
        {
            connection.Open();

            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS users (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT,
    update_at DATETIME,
    permission INTEGER,
    memo TEXT,
    biometric BLOB
);
";
                cmd.ExecuteNonQuery();
            }

            connection.Close();
        }

        public User GetUser(long id)
        {
            User result = null;
            connection.Open();

            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT id, name, update_at, permission, memo, biometric FROM users WHERE id = $id";
                cmd.Parameters.AddWithValue("$id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result = new User()
                        {
                            Id = reader.GetInt64(0),
                            Name = reader.GetString(1),
                            UpdateAt = reader.GetDateTime(2),
                            Permission = reader.GetInt32(3),
                            Memo = reader.GetString(4),
                        };
                        result.Biometrics = new byte[User.BiometricsLength];
                        reader.GetStream(5).Read(result.Biometrics, 0, User.BiometricsLength);
                    }
                }
            }

            connection.Close();
            return result;
        }

        public List<User> GetAllUsers()
        {
            var list = new List<User>();

            connection.Open();

            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT id, name, update_at, permission, memo, biometric FROM users ORDER BY id";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var u = new User()
                        {
                            Id = reader.GetInt64(0),
                            Name = reader.GetString(1),
                            UpdateAt = reader.GetDateTime(2),
                            Permission = reader.GetInt32(3),
                            Memo = reader.GetString(4),
                        };
                        u.Biometrics = new byte[User.BiometricsLength];
                        reader.GetStream(5).Read(u.Biometrics, 0, User.BiometricsLength);
                        list.Add(u);
                    }
                }
            }

            connection.Close();

            return list;
        }

        /// <summary>
        /// ユーザーを追加する。
        /// IdはDBが勝手に割り振るため、入力としては使わない。
        /// </summary>
        /// <param name="user"></param>
        public long AddUser(User user)
        {
            connection.Open();

            var memstream = new MemoryStream(user.Biometrics);
            long rowid = -1;

            using (SqliteCommand cmd = connection.CreateCommand())
            {
                // see
                // [BLOB の I/O](https://docs.microsoft.com/ja-jp/dotnet/standard/data/sqlite/blob-io)
                // [お兄ちゃん！そこは MemoryStream の出番だよ！](https://yuzutan-hnk.hatenablog.com/entry/2017/05/29/020348)
                //
                // 要約としては、rowid が分かるなら、その行のBLOB列へStreamとして書き込める。
                //
                cmd.CommandText = @"
INSERT INTO users (name, update_at, permission, memo, biometric)
VALUES ($name, $update_at, $permission, $memo, zeroblob($length));
SELECT last_insert_rowid();
";
                cmd.Parameters.AddWithValue(@"$name", user.Name);
                cmd.Parameters.AddWithValue(@"$update_at", user.UpdateAt);
                cmd.Parameters.AddWithValue(@"$permission", user.Permission);
                cmd.Parameters.AddWithValue(@"$memo", user.Memo);
                cmd.Parameters.AddWithValue(@"$length", memstream.Length);

                rowid = (long)cmd.ExecuteScalar();
            }

            if (rowid >= 0)
            {
                user.Id = rowid;
                using (var writeStream = new SqliteBlob(connection, @"users", @"biometric", rowid))
                {
                    memstream.CopyTo(writeStream);
                }
            }

            connection.Close();

            // Autoincrement のカラム(id)とrowidは一致するらしい。
            return rowid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>削除した行数</returns>
        public int DeleteUser(long id)
        {
            int count = 0;
            connection.Open();

            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"DELETE FROM users WHERE id = $id;";
                cmd.Parameters.AddWithValue(@"$id", id);

                count = cmd.ExecuteNonQuery();
                Debug.WriteLine($"Deleted Rows: {count}");
            }

            connection.Close();

            return count;
        }

        public void UpdateUser(User user)
        {
            UpdateUser(user.Id, user);
        }
        
        public void UpdateUser(long id, User user)
        {
            connection.Open();

            var memstream = new MemoryStream(user.Biometrics);

            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"
UPDATE
    users
SET
    name = $name, update_at = $update_at, permission = $permission, memo = $memo, biometric = zeroblob($length)
WHERE
    id = $id;
";
                cmd.Parameters.AddWithValue(@"$name", user.Name);
                cmd.Parameters.AddWithValue(@"$update_at", user.UpdateAt);
                cmd.Parameters.AddWithValue(@"$permission", user.Permission);
                cmd.Parameters.AddWithValue(@"$memo", user.Memo);
                cmd.Parameters.AddWithValue(@"$length", memstream.Length);
                cmd.Parameters.AddWithValue(@"$id", id);

                cmd.ExecuteNonQuery();
            }

            using (var writeStream = new SqliteBlob(connection, @"users", @"biometric", id))
            {
                memstream.CopyTo(writeStream);
            }

            connection.Close();
        }

        public List<RoleItem> GetRoleItems()
        {
            var list = new List<RoleItem>();

            connection.Open();

            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT name, permission FROM roles ORDER BY id";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var r = new RoleItem()
                        {
                            Name = reader.GetString(0),
                            Permission = (UserBio.Permission)reader.GetInt32(1),
                        };
                        list.Add(r);
                    }
                }
            }

            connection.Close();

            return list;
        }
    }
}
