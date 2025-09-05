using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using RealtimeViewer.Network.Mqtt;
using System;
using System.Diagnostics;
using System.IO;

namespace RealtimeViewer
{
    /// <summary>
    /// 車載器ごとにリモート設定で送信した内容を保存しておく。
    /// </summary>
    public class AppPermanentData : IDisposable
    {
        public const int MOVIE_CHANNEL_COUNT = 8;
        public const string REMOTE_CONFIG_TABLE_NAME = "RemoteConfigV2";

        private string dataStorePath;
        private string sqliteFile;
        private SqliteConnectionStringBuilder dbConnectionSb = null;
        private SqliteConnection dbConnection = null;
        private MqttJsonRemoteConfig defaultRemoteConfig = new MqttJsonRemoteConfig()
        {
            cameras = new MqttJsonRemoteConfigCamera[] {
                // ch 1-4.
                // TODO: resolution についてはTBD。
                new MqttJsonRemoteConfigCamera() { is_enabled = 1, bitrate = "5M", framerate = 15, resolution = string.Empty, },
                new MqttJsonRemoteConfigCamera() { is_enabled = 1, bitrate = "384K", framerate = 10, resolution = string.Empty, },
                new MqttJsonRemoteConfigCamera() { is_enabled = 1, bitrate = "384K", framerate = 10, resolution = string.Empty, },
                new MqttJsonRemoteConfigCamera() { is_enabled = 1, bitrate = "384K", framerate = 10, resolution = string.Empty, },

                // ch 5-8.
                // TODO: resolution についてはTBD。
                new MqttJsonRemoteConfigCamera() { is_enabled = 1, bitrate = "384K", framerate = 10, resolution = string.Empty, },
                new MqttJsonRemoteConfigCamera() { is_enabled = 1, bitrate = "384K", framerate = 10, resolution = string.Empty, },
                new MqttJsonRemoteConfigCamera() { is_enabled = 0, bitrate = "384K", framerate = 10, resolution = string.Empty, },
                new MqttJsonRemoteConfigCamera() { is_enabled = 0, bitrate = "384K", framerate = 10, resolution = string.Empty, },
            },
            event_pre_sec = 10,
            event_post_sec = 5,
            gsensor_forward = 0.8,
            gsensor_backward = 0.8,
            gsensor_horizontal = 0.8,
            gsensor_vertical = 1.0,
            //gsensor_direction = 5, // TODO: TBD. 現在は仮の値。
            speed_level = 0,
            speed_num_of_gear = 8,
            speed_threshold = 30,
            tach_level = 0,
            tach_num_of_gear = 1500,
            tach_threshold = 30,
            streaming_bitrate = "256K",
            streaming_framerate = 15,
            speaker_volume = 95,
            odometer_km = 0,
            io_signal_polarities = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, }, // TODO: TBD. 現在は仮の値。
            has_serial_devices = new int [] { 0, 0, 0, 0, }, // TODO: TBD. 現在は仮の値。
        };

        public AppPermanentData()
        {
            MakeStorePath();
            ConnectDatabase();
        }

        public void Dispose()
        {
            if (dbConnection != null)
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        public string StoreFolder
        {
            get
            {
                return dataStorePath;
            }
        }

        public string SqliteFile
        {
            get
            {
                return sqliteFile;
            }
        }

        /// <summary>
        /// Get a deep copy that is default config.
        /// </summary>
        /// <returns></returns>
        public MqttJsonRemoteConfig GetDefaultConfig()
        {
            return JsonConvert.DeserializeObject<MqttJsonRemoteConfig>(JsonConvert.SerializeObject(defaultRemoteConfig));
        }

        public bool MakeStorePath()
        {
            bool ret = false;
            var info = new Microsoft.VisualBasic.ApplicationServices.AssemblyInfo(System.Reflection.Assembly.GetExecutingAssembly());

            dataStorePath = Path.Combine(new string[] {
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                info.CompanyName,
                info.AssemblyName,
            });
            Debug.WriteLine($"storeFolder: {dataStorePath}");

            try
            {
                Directory.CreateDirectory(dataStorePath);
                ret = true;

                sqliteFile = Path.Combine(dataStorePath, @"appdata.sqlite");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Error: CreateDirectory({dataStorePath}), {ex}");
            }

            return ret;
        }

        /// <summary>
        /// テーブルを作成する。
        /// </summary>
        /// <param name="connection"></param>
        private void CreateTables(SqliteConnection connection)
        {
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = String.Format(@"
CREATE TABLE IF NOT EXISTS {0} (
    device_id TEXT UNIQUE,
    update_at DATETIME,
    content TEXT
);", REMOTE_CONFIG_TABLE_NAME);
                cmd.ExecuteNonQuery();
            }
        }

        public bool ConnectDatabase()
        {
            bool ret = false;
            try
            {
                dbConnectionSb = new SqliteConnectionStringBuilder { DataSource = sqliteFile };
                dbConnection = new SqliteConnection(dbConnectionSb.ToString());
                dbConnection.Open();
                ret = true;

                CreateTables(dbConnection);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Error: could not open database. {ex}");
            }

            return ret;
        }

        public MqttJsonRemoteConfig ReadRemoteConfig(string deviceId)
        {
            MqttJsonRemoteConfig result = GetDefaultConfig();
            result.device_id = deviceId;
            bool hasRecord = false;

            try
            {
                using (SqliteCommand cmd = dbConnection.CreateCommand())
                {

                    cmd.CommandText = String.Format(@"SELECT device_id, update_at, content FROM {0} WHERE device_id == $did", REMOTE_CONFIG_TABLE_NAME);
                    cmd.Parameters.AddWithValue("$did", deviceId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result = JsonConvert.DeserializeObject<MqttJsonRemoteConfig>(reader.GetString(2));
                            hasRecord = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Error: {ex}");
            }

            if (!hasRecord)
            {
                AddDefaultRemoteConfig(result);
            }

            return result;
        }

        public bool AddDefaultRemoteConfig(MqttJsonRemoteConfig config)
        {
            bool ret = false;

            try
            {
                using (SqliteCommand cmd = dbConnection.CreateCommand())
                {
                    cmd.CommandText = String.Format(@"
INSERT INTO {0} (
    device_id, update_at, content
    )
VALUES (
    $device_id, $update_at, $content
    );", REMOTE_CONFIG_TABLE_NAME);
                    cmd.Parameters.AddWithValue("$device_id", config.device_id);
                    cmd.Parameters.AddWithValue("$update_at", DateTime.Now);
                    cmd.Parameters.AddWithValue("$content", JsonConvert.SerializeObject(config));

                    cmd.ExecuteNonQuery();

                    ret = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Error: {ex}");
            }

            return ret;
        }

        public bool UpdateRemoteConfig(MqttJsonRemoteConfig config)
        {
            bool ret = false;

            try
            {
                using (SqliteCommand cmd = dbConnection.CreateCommand())
                {
                    cmd.CommandText = String.Format(@"
UPDATE {0} SET
    update_at = $update_at, content = $content
WHERE
    device_id = $device_id;", REMOTE_CONFIG_TABLE_NAME);
                    cmd.Parameters.AddWithValue("$device_id", config.device_id);
                    cmd.Parameters.AddWithValue("$update_at", DateTime.Now);
                    cmd.Parameters.AddWithValue("$content", JsonConvert.SerializeObject(config));

                    cmd.ExecuteNonQuery();

                    ret = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Error: {ex}");
            }

            return ret;
        }
    }
}
