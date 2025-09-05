using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UserBioDP
{
    public enum Role
    {
        None = 0,   // ダミー。プログラム上でのみ使う。表に出すな！
        Engineer,
        SuperUser,
        OfficeEditor,
        Generic,
    }

    /// <summary>
    /// 権限とロールがごちゃ混ぜのひとがformsアプリを作ってしまったため、ぐちゃぐちゃである。
    /// 表示上、あるいは、社長の述べる「権限(Permission)」は役割(Role)である。注意されたい。
    /// </summary>
    [Flags]
    public enum Permission
    {
        None = 0,
        LocalView = 0x0001, // ローカルストレージのビューアを使用可能
        StreamingView = 0x0002, // リアルタイム配信、プリポストデータの閲覧可能
        MasterEdit = 0x0010,    // 運転手、車両など、営業所内マスターデータの編集可能
        OfficeEdit = 0x0020,    // PCアプリで『営業所』の変更可能
        SettingEdit = 0x0040,   // PCアプリで営業所以外の設定変更可能 → もはや使わない
        UserEdit = 0x0100,      // ユーザー情報の編集可能
        Engineer = 0x1000,      // 開発者モードが利用可能
    }

    public class RolePermissonConverter
    {
        public static Permission ToPermission(Role role)
        {
            Permission permission = Permission.None;
            switch (role)
            {
                case Role.Engineer:
                    permission = Permission.LocalView | Permission.StreamingView | Permission.MasterEdit | Permission.OfficeEdit | Permission.SettingEdit | Permission.UserEdit | Permission.Engineer;
                    break;

                case Role.SuperUser:
                    permission = Permission.LocalView | Permission.StreamingView | Permission.MasterEdit | Permission.OfficeEdit | Permission.SettingEdit | Permission.UserEdit;
                    break;

                case Role.OfficeEditor:
                    permission = Permission.LocalView | Permission.StreamingView | Permission.MasterEdit | Permission.OfficeEdit | Permission.SettingEdit;
                    break;

                case Role.Generic:
                    permission = Permission.LocalView | Permission.StreamingView | Permission.MasterEdit | Permission.SettingEdit;
                    break;
            }
            return permission;
        }

        /// <summary>
        /// 指定された権限から、ロールを推測する。
        /// 特定の権限のみ調べているため、完全に一致するかは分からない。
        /// </summary>
        /// <param name="perm"></param>
        /// <returns></returns>
        public static Role GuessRole(Permission perm)
        {
            var role = Role.None;

            if ((perm & Permission.Engineer) != 0)
                role = Role.Engineer;
            else if ((perm & Permission.UserEdit) != 0)
                role = Role.SuperUser;
            else if ((perm & Permission.OfficeEdit) != 0)
                role = Role.OfficeEditor;
            else if ((int)perm != 0)
                role = Role.Generic;

            return role;
        }

        public static string GetRoleName(Role role)
        {
            string roleName = string.Empty;
            switch (role)
            {
                case Role.None:
                    break;

                case Role.Engineer:
                    roleName = @"エンジニア";
                    break;

                case Role.SuperUser:
                    roleName = @"特権";
                    break;

                case Role.OfficeEditor:
                    roleName = @"全営業所";
                    break;

                case Role.Generic:
                    roleName = @"所属営業所";
                    break;
            }

            return roleName;
        }
    }

    public class RoleItem
    {
        public RoleItem(Role role)
        {
            Role = role;
            Name = RolePermissonConverter.GetRoleName(role);
        }

        public string Name { get; set; }
        public Role Role { get; set; }
    }

    public class DefaultRoleList
    {
        public DefaultRoleList()
        {
            // 並び順は、権限の弱い→強い。
            _roles = new BindingList<RoleItem>()
            {
                new RoleItem(Role.Generic),
                new RoleItem(Role.OfficeEditor),
                new RoleItem(Role.SuperUser),
            };

            _exRoles = new BindingList<RoleItem>();
            foreach (var item in _roles)
            {
                _exRoles.Add(item);
            }
            _exRoles.Add(new RoleItem(Role.Engineer));
        }

        private BindingList<RoleItem> _roles;
        public BindingList<RoleItem> Roles
        {
            get
            {
                return _roles;
            }

            private set
            {

            }
        }

        private BindingList<RoleItem> _exRoles;
        public BindingList<RoleItem> ExRoles
        {
            get
            {
                return _roles;
            }

            private set
            {

            }
        }
    }

    /// <summary>
    /// PCアプリのユーザー情報。
    /// ここではユーザーは一般論としてのオペレーターを指す。運転手ではない。
    /// </summary>
    public class User : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public const int BiometricsLength = 1048 + 42004;
        public const int BiometricsFmdLength = 1024;

        private long _id;
        public long Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private DateTime _updateAt;
        public DateTime UpdateAt
        {
            get { return _updateAt; }
            set
            {
                if (_updateAt != value)
                {
                    _updateAt = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _permission;
        public int Permission
        {
            get { return _permission; }
            set
            {
                if (_permission != value)
                {
                    _permission = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _roleName;
        public string RoleName
        {
            get
            {
                return _roleName;
            }
            set
            {
                if (_roleName != value)
                {
                    _roleName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _memo;
        public string Memo
        {
            get { return _memo; }
            set
            {
                if (_memo != value)
                {
                    _memo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _biometricsFmd;
        public string BiometricsFmd
        {
            get { return _biometricsFmd; }
            set
            {
                _biometricsFmd = value;
                // Always Notifies.
                NotifyPropertyChanged();
            }
        }

        public User()
        {
            Name = string.Empty;
            Memo = string.Empty;
        }

        /// <summary>
        /// 特定権限を使用可能か調べる。
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool Can(Permission permission)
        {
            return Can((int)permission);
        }

        /// <summary>
        /// 特定権限を使用可能か調べる。
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool Can(int permission)
        {
            return ((int)this.Permission & permission) != 0;
        }

        public void AddPermission(Permission permission)
        {
            this.Permission |= (int)permission;
        }

        public void RemovePermission(Permission permission)
        {
            this.Permission &= ~(int)permission;
        }
    }
}
