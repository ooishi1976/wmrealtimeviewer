using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace SampleApplication
{
    public partial class CustomPropertyForm : Form
    {
        // カスタム情報プロパティ
        CustomProperty m_property = new CustomProperty();

        public CustomPropertyForm(CustomProperty property)
        {
            InitializeComponent();
            // カスタム情報プロパティを設定する。
            m_property = property;
            // コントロールの初期値を設定する。
            pictFillColor.BackColor = property.FillColor;
            pictLineColor.BackColor = property.LineColor;
            txtLineWidth.Text = property.LineWidth.ToString();
            pictTextColor.BackColor = property.TextColor;
            if (property.Icon != null)
            {
                pictPreview.Image = property.Icon;
            }
        }

        /// <summary>
        /// カスタム情報プロパティを取得する。
        /// </summary>
        /// <returns>カスタム情報プロパティを返す。</returns>
        public CustomProperty GetCustomProperty()
        {
            return m_property;
        }

        /// <summary>
        /// 線幅テキストボックスは数値のみ入力を許可する。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtLineWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b')
            {
                e.Handled = true;
            }

        }

        /// <summary>
        /// カラーダイアログからColorオブジェクトを取得する。
        /// </summary>
        /// <param name="def">デフォルト色を指定する。</param>
        /// <returns>選択されたColorを返す。(キャンセルされた場合は引数を返す。)</returns>
        private Color getColorToColorDialog(Color def)
        {
            // ColorDialog の新しいインスタンスを生成する。
            using (ColorDialog cDialog = new ColorDialog())
            {
                // 初期選択する色を設定する。
                cDialog.Color = def;
                // カスタム カラーを表示した状態にする。
                cDialog.FullOpen = true;
                // 使用可能なすべての色を基本セットに表示する。
                cDialog.AnyColor = true;
                // [ヘルプ] ボタンを表示する。
                cDialog.ShowHelp = true;
                if (cDialog.ShowDialog() == DialogResult.OK)
                {
                    // 選択した色を返す。
                    return cDialog.Color;
                }
                else
                {
                    // デフォルト色を返す。
                    return def;
                }
            }
        }

        /// <summary>
        /// ファイル選択ダイアログからイメージを取得する。
        /// </summary>
        /// <returns>取得したイメージオブジェクトを返す。</returns>
        private Image GetImageFromDialog()
        {
            // OpenFileDialogクラスのインスタンスを作成する。
            using (OpenFileDialog ofDialog = new OpenFileDialog())
            {
                // 起動時に表示されるファイル名を空で指定する
                ofDialog.FileName = "";
                // デフォルトで選択されるドライブを指定する。
                ofDialog.InitialDirectory = @"C:\";
                // [ファイルの種類]に表示される選択肢を指定する。
                ofDialog.Filter = "画像ファイル(*.gif,*.png,*.jpg,*.bmp)|*.gif;*.png;*.jpg;*.bmp|すべてのファイル(*.*)|*.*";
                // [ファイルの種類]のデフォルトを画像ファイルにする。
                ofDialog.FilterIndex = 0;
                // タイトルを設定する。
                ofDialog.Title = "画像ファイルを選択してください";
                // ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする。
                ofDialog.RestoreDirectory = true;

                // ダイアログを表示する。
                if (ofDialog.ShowDialog() == DialogResult.OK)
                {
                    // 取得したイメージを表示する。
                    return Image.FromFile(ofDialog.FileName);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 塗り潰し色を設定する。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFillColor_Click(object sender, EventArgs e)
        {
            pictFillColor.BackColor = getColorToColorDialog(pictFillColor.BackColor);
        }

        /// <summary>
        /// 線色を設定する。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLineColor_Click(object sender, EventArgs e)
        {
            pictLineColor.BackColor = getColorToColorDialog(pictLineColor.BackColor);
        }

        /// <summary>
        /// 文字色を設定する。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTextColor_Click(object sender, EventArgs e)
        {
            pictTextColor.BackColor = getColorToColorDialog(pictTextColor.BackColor);
        }

        /// <summary>
        /// アイコン画像を設定する。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnIconRef_Click(object sender, EventArgs e)
        {
            pictPreview.Image = GetImageFromDialog();
        }

        /// <summary>
        /// このウィンドウを閉じる。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // DialogResultにCancelを指定し、ウィンドウを閉じる。
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// 設定されたプロパティ値を保存する。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            // 入力チェックを行う。
            Regex reg = new Regex("^[0-9]+$");
            if (!reg.Match(txtLineWidth.Text).Success)
            {
                MessageBox.Show("線幅には整数値を入力してください。");
                return;
            }
            // 設定内容を保存する。
            m_property.FillColor = pictFillColor.BackColor;
            m_property.LineColor = pictLineColor.BackColor;
            m_property.LineWidth = int.Parse(txtLineWidth.Text);
            m_property.TextColor = pictTextColor.BackColor;
            if (pictPreview.Image != null)
            {
                m_property.Icon = (Bitmap)pictPreview.Image;
            }
            // DialogResultにOKを指定し、ウィンドウを閉じる。
            this.DialogResult = DialogResult.OK;
        }
    }
}
