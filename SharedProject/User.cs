using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UserBio
{
    public enum Role
    {
        SuperUser = 1,
        OfficeEditor,
        Generic,
    }

    [Flags]
    public enum Permission
    {
        None = 0,
        LocalView = 0x0001, // ローカルストレージのビューアを使用可能
        StreamingView = 0x0002, // リアルタイム配信、プリポストデータの閲覧可能
        MasterEdit = 0x0010,    // 運転手、車両など、営業所内マスターデータの編集可能
        OfficeEdit = 0x0020,    // PCアプリで『営業所』の変更可能
        SettingEdit = 0x0040,   // PCアプリで営業所以外の設定変更可能
        UserEdit = 0x0100,      // ユーザー情報の編集可能
    }

    public class PermissionToText
    {
        private Dictionary<Permission, string> dicShortJapanese;
        private Dictionary<Permission, string> dicLongJapanese;

        public PermissionToText()
        {
            // リソースから引っ張り出すのが、より良い作法だがここではハードコードする。
            // 短いテキスト
            dicShortJapanese = new Dictionary<Permission, string>();
            dicShortJapanese.Add(Permission.None, string.Empty);
            dicShortJapanese.Add(Permission.LocalView, @"ビューア");
            dicShortJapanese.Add(Permission.StreamingView, @"配信");
            dicShortJapanese.Add(Permission.MasterEdit, @"編集");
            dicShortJapanese.Add(Permission.OfficeEdit, @"営業所");
            dicShortJapanese.Add(Permission.SettingEdit, @"設定");
            dicShortJapanese.Add(Permission.UserEdit, @"ユ編");

            // 長いテキスト
            dicLongJapanese = new Dictionary<Permission, string>();
            dicLongJapanese.Add(Permission.None, string.Empty);
            dicLongJapanese.Add(Permission.LocalView, @"ビューア使用");
            dicLongJapanese.Add(Permission.StreamingView, @"リアルタイム配信");
            dicLongJapanese.Add(Permission.MasterEdit, @"車両/運転手編集");
            dicLongJapanese.Add(Permission.OfficeEdit, @"営業所変更");
            dicLongJapanese.Add(Permission.SettingEdit, @"設定変更");
            dicLongJapanese.Add(Permission.UserEdit, @"ユーザー編集");
        }

        public Dictionary<Permission, string> DicShortJapanese
        {
            get
            {
                return dicShortJapanese;
            }

            set
            {
                dicShortJapanese = value;
            }
        }

        public Dictionary<Permission, string> DicLongJapanese
        {
            get
            {
                return dicLongJapanese;
            }

            set
            {
                dicLongJapanese = value;
            }
        }
    }

    public class DefaultRoleToPermissonConverter
    {
        public static Permission ToPermission(Role role)
        {
            Permission permission = Permission.None;
            switch (role)
            {
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
    }

    public class RoleItem
    {
        public string Name { get; set; }
        public Permission Permission { get; set; }
    }

    public class DefaultRoleList
    {
        private List<RoleItem> list = new List<RoleItem>()
        {
            new RoleItem() { Name = @"特権", Permission = DefaultRoleToPermissonConverter.ToPermission(Role.SuperUser) },
            new RoleItem() { Name = @"営業所", Permission = DefaultRoleToPermissonConverter.ToPermission(Role.OfficeEditor) },
            new RoleItem() { Name = @"一般", Permission = DefaultRoleToPermissonConverter.ToPermission(Role.Generic) },
        };

        public List<RoleItem> Roles
        {
            get
            {
                return list;
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

        private byte[] _biometrics;
        public byte[] Biometrics
        {
            get { return _biometrics; }
            set
            {
                _biometrics = value;
                // Always Notifies.
                NotifyPropertyChanged();
            }
        }

        public User()
        {
            Name = string.Empty;
            Memo = string.Empty;
        }

        public bool Can(Permission permission)
        {
            return ((int)this.Permission & (int)permission) != 0;
        }

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
