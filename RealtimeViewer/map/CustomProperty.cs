using System.Drawing;

namespace RealtimeViewer.Map
{
    /// <summary>
    /// カスタム情報プロパティの値を管理するクラスを定義する。
    /// </summary>
    public class CustomProperty
    {
        // 塗り潰し色
        Color m_fillColor = Color.Orange;
        // 線色
        Color m_lineColor = Color.Brown;
        // 線幅
        int m_lineWidth = 2;
        // 文字色
        Color m_textColor = Color.Yellow;
        // アイコン画像
        Bitmap m_icon;

        /// <summary>
        /// 塗り潰し色
        /// </summary>
        public Color FillColor
        {
            set { m_fillColor = value; }
            get { return m_fillColor; }
        }

        /// <summary>
        /// 線色
        /// </summary>
        public Color LineColor
        {
            set { m_lineColor = value; }
            get { return m_lineColor; }
        }

        /// <summary>
        /// 線幅
        /// </summary>
        public int LineWidth
        {
            set { m_lineWidth = value; }
            get { return m_lineWidth; }
        }

        /// <summary>
        /// 文字色
        /// </summary>
        public Color TextColor
        {
            set { m_textColor = value; }
            get { return m_textColor; }
        }

        /// <summary>
        /// アイコン画像
        /// </summary>
        public Bitmap Icon
        {
            set { m_icon = value; }
            get { return m_icon; }
        }
    }
}
