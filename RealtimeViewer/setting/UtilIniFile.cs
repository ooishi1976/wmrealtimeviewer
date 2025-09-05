using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RealtimeViewer.Setting
{
    /// <summary>
    /// iniファイルアクセスクラス。インスタンス化はせずに使用する
    /// </summary>
    public class UtilIniFile
    {
        // iniファイルのパス
        private static string m_sIniFilePath = null;

        #region // WIN32API

        [DllImport("KERNEL32.DLL")]
        public static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);

        [DllImport("KERNEL32.DLL")]
        public static extern uint GetPrivateProfileInt(string lpAppName, string lpKeyName, int nDefault, string lpFileName);

//        [DllImport("KERNEL32.DLL")]
//        private static extern uint GetPrivateProfileSection(string lpAppName, string lpReturnedString, uint nSize, string lpFileName);
	
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetPrivateProfileSection(string lpAppName, string lpReturnedString, uint nSize, string lpFileName);

        [DllImport("kernel32.dll")]
        private static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpstring, string lpFileName);


        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // 機能		: コンストラクタ
        // 戻り値	: なし
        //--------------------------------------------------------------------------------------------------
        //------------型-----------	名称---------	[I/O]説明文	--------------------------------------------
        // 引数		: string       	sFilePath       [I  ]Iniファイルのフルパス
        //--------------------------------------------------------------------------------------------------
        // 備考     :
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void UtilSetIniFilePath(string sFilePath)
        {
            m_sIniFilePath = sFilePath;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // 機能		: INIファイルパスの取得
        // 戻り値	: string                        コンストラクタで指定したパス
        //--------------------------------------------------------------------------------------------------
        //------------型-----------	名称---------	[I/O]説明文	--------------------------------------------
        // 引数		: 
        //--------------------------------------------------------------------------------------------------
        // 備考     :
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string GetIniFilePath()
        {
            return m_sIniFilePath;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // 機能		: Iniファイル中のセクションのキーを指定して、文字列を返す
        // 戻り値	: string                        指定したセクション・キーに紐ついた文字列
        //--------------------------------------------------------------------------------------------------
        //------------型-----------	名称---------	[I/O]説明文	--------------------------------------------
        // 引数		: string       	sSection	    [I  ]探したいセクション名
        //  		: string      	sKey  		    [I  ]探したいキー名
        //  		: string        sDefault	    [I  ]デフォルト値("")
        //--------------------------------------------------------------------------------------------------
        // 備考     : ※設定しているセクション、キーがない場合はヌル値
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string getValueString(string sSection, string sKey, string sDefault = "")
        {
            StringBuilder sb = new StringBuilder(1024);
            GetPrivateProfileString(sSection, sKey, sDefault, sb, Convert.ToUInt32(sb.Capacity), m_sIniFilePath);
            return Convert.ToString(sb);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // 機能		: Iniファイル中のセクションのキーを指定して、整数値を返す
        // 戻り値	: int                           指定したセクション・キーに紐ついた数値
        //--------------------------------------------------------------------------------------------------
        //------------型-----------	名称---------	[I/O]説明文	--------------------------------------------
        // 引数		: string       	sSection	    [I  ]探したいセクション名
        //  		: string      	sKey  		    [I  ]探したいキー名
        //  		: int           nDefault	    [I  ]デフォルト値(0)
        //--------------------------------------------------------------------------------------------------
        // 備考     : ※設定しているセクション、キーがない場合は０
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static int getValueInt(string sSection, string sKey, int nDefault = 0)
        {
            return (int)GetPrivateProfileInt(sSection, sKey, nDefault, m_sIniFilePath);
        }

        /// <summary>
        /// Iniファイル中のセクションのキーを指定して、得られた文字列をデリミタで分割してリストで返す。
        /// </summary>
        /// <param name="sSection">セクション</param>
        /// <param name="sKey">キー</param>
        /// <param name="delimiter">値を分割するデリミタ</param>
        /// <param name="defaultValue">初期値(iniファイルに格納される形式で指定する)</param>
        /// <returns></returns>
        public static List<int> getValueIntList(string section, string key, char delimiter, string defaultValue = null)
        {
            List<int> result = new List<int>();
            string iniValue = getValueString(section, key);
            if (iniValue == null)
            {
                if (defaultValue == null)
                {
                    iniValue = string.Empty;
                }
                else
                {
                    iniValue = defaultValue;
                }
            } 

            string[] values = iniValue.Split(delimiter);
            foreach (string value in values)
            {
                if (int.TryParse(value, out int target))
                {
                    result.Add(target);
                }
            }
            return result;
        }

        /// <summary>
        /// Iniファイル中のセクション全体を文字列で取得してディクショナリで返す。
        /// </summary>
        /// <param name="section">セクション</param>
        /// <returns>key, valueのペアを格納したディクショナリ</returns>
        public static Dictionary<string, string> getValuesString(string section)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            uint ret = 0;
            uint size = 0;
            int bufferSize = 1024;
            int count = 0;
            string returnedValue;
            do {
                bufferSize = (bufferSize * ++count);
                returnedValue = new string('\0', bufferSize);
                size = (uint)returnedValue.Length;
                ret = GetPrivateProfileSection(section, returnedValue, size, m_sIniFilePath);
            } while (ret == (size - 2));

            returnedValue = returnedValue.TrimEnd('\0');
            foreach (var value in returnedValue.Split('\0')) 
            {
                Match match = Regex.Match(value, @"(\w+)=([\w\W]+)");
                if (match.Success)
                {
                    result[match.Groups[1].Value] = match.Groups[2].Value;
                }
            }
            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // 機能		: Iniファイル中のセクションのキーを指定して、bool値を返す
        // 戻り値	: int                           指定したセクション・キーに紐ついた数値
        //--------------------------------------------------------------------------------------------------
        //------------型-----------	名称---------	[I/O]説明文	--------------------------------------------
        // 引数		: string       	sSection	    [I  ]探したいセクション名
        //  		: string      	sKey  		    [I  ]探したいキー名
        //  		: bool          bDefault	    [I  ]デフォルト値(false)
        //--------------------------------------------------------------------------------------------------
        // 備考     : ※設定しているセクション、キーがない場合は０
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool getValueBoolean(string sSection, string sKey, bool bDefault = false)
        {
            StringBuilder sb = new StringBuilder(1024);
            GetPrivateProfileString(sSection, sKey, bDefault.ToString(), sb, Convert.ToUInt32(sb.Capacity), m_sIniFilePath);
            string sTemp = Convert.ToString(sb);
            return Convert.ToBoolean(sTemp);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // 機能		: Iniファイル中のセクションのキーを指定して、実数を返す
        // 戻り値	: double                         指定したセクション・キーに紐ついた数値
        //--------------------------------------------------------------------------------------------------
        //------------型-----------	名称---------	[I/O]説明文	--------------------------------------------
        // 引数		: string       	sSection	    [I  ]探したいセクション名
        //  		: string      	sKey  		    [I  ]探したいキー名
        //--------------------------------------------------------------------------------------------------
        // 備考     : ※設定しているセクション、キーがない場合は０
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static double getValueDouble(string sSection, string sKey, double dDefault)
        {
            string str = getValueString(sSection, sKey, "");
            if (str == "")
            {
                return dDefault;
            }
            double d;
            if (double.TryParse(str, out d))
            {
                return d;
            }
            else
            {
                return 0.0;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // 機能		: 指定したセクション、キーに数値を書き込む(整数)
        // 戻り値	: なし
        //--------------------------------------------------------------------------------------------------
        //------------型-----------	名称---------	[I/O]説明文	--------------------------------------------
        // 引数		: string       	sSection	    [I  ]Iniファイルへ書き込むセクション名
        //  		: string      	sKey  		    [I  ]Iniファイルへ書き込むキー名
        //  		: int       	iValue    		[I  ]Iniファイルへ書き込むデータ(数値)
        //--------------------------------------------------------------------------------------------------
        // 備考     :
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void setValue(string sSection, string sKey, int iValue)
        {
            setValue(sSection, sKey, iValue.ToString());
        }
        // 符号付き
        public static void setValueM(string sSection, string sKey, int iValue)
        {
            if (iValue >= 0)
            {
                string ss = iValue.ToString();
                ss = "+" + ss;
                setValue(sSection, sKey, ss);
            }
            else
            {
                setValue(sSection, sKey, iValue.ToString());
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // 機能		: 指定したセクション、キーに数値を書き込む(実数)
        // 戻り値	: なし
        //--------------------------------------------------------------------------------------------------
        //------------型-----------	名称---------	[I/O]説明文	--------------------------------------------
        // 引数		: string       	sSection	    [I  ]Iniファイルへ書き込むセクション名
        //  		: string      	sKey  		    [I  ]Iniファイルへ書き込むキー名
        //  		: double       	dValue    		[I  ]Iniファイルへ書き込むデータ(数値)
        //--------------------------------------------------------------------------------------------------
        // 備考     :
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void setValue(string sSection, string sKey, double dValue)
        {
            setValue(sSection, sKey, dValue.ToString());
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // 機能		: 指定したセクション、キーに文字列を書き込む
        // 戻り値	: なし
        //--------------------------------------------------------------------------------------------------
        //------------型-----------	名称---------	[I/O]説明文	--------------------------------------------
        // 引数		: string       	sSection	    [I  ]Iniファイルへ書き込むセクション名
        //  		: string      	sKey  		    [I  ]Iniファイルへ書き込むキー名
        //  		: string       	sValue    		[I  ]Iniファイルへ書き込むデータ(文字列)
        //--------------------------------------------------------------------------------------------------
        // 備考     :
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void setValue(string sSection, string sKey, string sValue)
        {
            WritePrivateProfileString(sSection, sKey, sValue, m_sIniFilePath);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // 機能		: 指定したセクションのキーを削除する
        // 戻り値	: なし
        //--------------------------------------------------------------------------------------------------
        //------------型-----------	名称---------	[I/O]説明文	--------------------------------------------
        // 引数		: string       	sSection	    [I  ]Iniファイルへ書き込むセクション名
        //  		: string      	sKey  		    [I  ]Iniファイルへ書き込むキー名
        //--------------------------------------------------------------------------------------------------
        // 備考     :
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void deleteValue(string sSection, string sKey)
        {
            WritePrivateProfileString(sSection, sKey, null, m_sIniFilePath);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // 機能		: 指定したセクションを削除する
        // 戻り値	: なし
        //--------------------------------------------------------------------------------------------------
        //------------型-----------	名称---------	[I/O]説明文	--------------------------------------------
        // 引数		: string       	sSection	    [I  ]Iniファイルから削除するセクション名
        //--------------------------------------------------------------------------------------------------
        // 備考     :
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void deleteSection(string sSection)
        {
            WritePrivateProfileString(sSection, null, null, m_sIniFilePath);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // 機能		: 指定したセクション、キーに文字列を書き込む
        // 戻り値	: なし
        //--------------------------------------------------------------------------------------------------
        //------------型-----------	名称---------	[I/O]説明文	--------------------------------------------
        // 引数		: string       	sSection	    [I  ]Iniファイルへ書き込むセクション名
        //  		: string      	sKey  		    [I  ]Iniファイルへ書き込むキー名
        //  		: bool       	bValue    		[I  ]Iniファイルへ書き込むデータ(bool)
        //--------------------------------------------------------------------------------------------------
        // 備考     :
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void setValue(string sSection, string sKey, bool bValue)
        {
            if (bValue == true)
            {
                WritePrivateProfileString(sSection, sKey, "true", m_sIniFilePath);
            }
            else
            {
                WritePrivateProfileString(sSection, sKey, "false", m_sIniFilePath);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // 機能		: 指定したセクション、キーに位置情報を書き込む
        // 戻り値	: なし
        //--------------------------------------------------------------------------------------------------
        //------------型-----------	名称---------	[I/O]説明文	--------------------------------------------
        // 引数		: string       	sSection	    [I  ]Iniファイルへ書き込むセクション名
        //  		: string      	sKey  		    [I  ]Iniファイルへ書き込むキー名
        //  		: Point       	pValue    		[I  ]Iniファイルへ書き込むデータ(Point)
        //--------------------------------------------------------------------------------------------------
        // 備考     :
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void setValue(string sSection, string sKey, Point pValue)
        {
            setValue(sSection, sKey, pValue.ToString());
        }

        /// <summary>
        /// リストの値をデリミタで結合した値をIniファイルに書き込む。
        /// </summary>
        /// <param name="section">セクション</param>
        /// <param name="key">キー</param>
        /// <param name="valueList">キー</param>
        /// <param name="delimiter">値を分割するデリミタ</param>
        public static void setValue(string section, string key, List<int> valueList, string delimiter)
        {
            setValue(section, key, string.Join(delimiter, valueList));
        }
    }
}
