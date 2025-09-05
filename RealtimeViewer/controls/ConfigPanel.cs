using RealtimeViewer.Model;
using RealtimeViewer.Setting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealtimeViewer.Controls
{
    public partial class ConfigPanel : UserControl
    {
        public ConfigPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ConfigPanelModelを設定する。
        /// コントロールやデータソースにバインドする
        /// </summary>
        private ConfigPanelModel dataModel = null;
        public ConfigPanelModel DataModel {
            get 
            {
                return dataModel;
            }
            set
            {
                // モデル設定時、並びに、モデルがバインドされて、
                // コントロール間の通知が行われるとき、ライブラリから例外が発生する。(ただ、プログラムは止まらない)
                // 処理は続行できるため、解決方法を見つけるまではこのままとする。
                // パネルのコンボボックスにDataSourceを設定すると例外が発生する
                dataModel = value;
                numericUpDownSessionWait.Minimum = value.StreamingSessionWaitMin;
                officeInfoDataSourceBindingSource.DataSource = dataModel.OfficeInfoDataSource;
                configPanelModelBindingSource.DataSource = dataModel;

            }
        }

        /// <summary>
        /// コントロールの内容をIniファイル情報のものに戻す
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonConfigCancel_Click(object sender, EventArgs e)
        {
            if (DataModel != null)
            {
                DataModel.LoadConfigData();
            }
        }

        /// <summary>
        /// コントロールの内容をIniファイル情報に設定し、
        /// このパネルのOwnerをCloseする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonConfigApply_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show(
                    this,
                    "設定を反映する為にプログラムを終了します。",
                    "確認",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
            {
                if (DataModel != null)
                {
                    DataModel.ApplyConfigData();
                }
                DataModel.Owner.Close();
            }
        }

        /// <summary>
        /// 営業所DataGridのParsingイベント。
        /// 緯度経度を編集したときに、入力値が妥当であるか判断する。
        /// ・(緯度, 経度)の場合は、文字列から(int, int)タプルに変換して設定する。
        /// ・住所文字列の場合は、Validatingで検索されていると判断して、
        ///   DataModelを通じて検索結果の位置情報を設定する。
        /// ・空白の場合は、空白を設定する。
        /// 上記以外は入力不可と判断する。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridViewOffice_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                if (string.IsNullOrEmpty((string)e.Value))
                {
                    e.Value = null;
                    e.ParsingApplied = true;
                }
                else
                {
                    var match = Regex.Match((string)e.Value, @"\((\d+),\s*(\d+)\)");
                    if (match.Success)
                    {
                        e.Value = (int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
                        e.ParsingApplied = true;
                    }
                    else
                    {
                        if (DataModel.FindedLocation == null)
                        {
                            e.ParsingApplied = false;
                        }
                        else 
                        {
                            e.Value = DataModel.FindedLocation;
                            e.ParsingApplied = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 営業所DataGridのValidateイベント。
        /// 緯度経度を編集したときに、入力値が妥当であるか判断する。
        /// ・(緯度, 経度)形式
        /// ・住所検索を行い、該当がある
        /// ・空白
        /// 上記以外は入力不可と判断する。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridViewOffice_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (!DataModel.IsApplyConfig)
            {
                if (e.ColumnIndex == 0)  // 表示/非表示
                {
                    if (!(bool)e.FormattedValue)
                    {
                        var cuurent = officeInfoDataSourceBindingSource.Current as OfficeInfo;
                        if (DataModel.GetVisibleOfficeCount() == 1 && cuurent.Visible)
                        {
                            e.Cancel = true;
                        }
                    }
                }
                else if (e.ColumnIndex == 2)  // 所在地
                {
                    string target = e.FormattedValue.ToString();
                    if (!string.IsNullOrEmpty(target))
                    {
                        if (!Regex.IsMatch(target, @"\((\d+),\s*(\d+)\)"))
                        {
                            if (DataModel.FindLocation(target) == null)
                            {
                                e.Cancel = true;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 営業所DataGridの編集開始イベント
        /// セル編集中は営業所DataGrid以外は操作できない
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridViewOffice_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataModel.IsApplyConfig = false;
        }

        /// <summary>
        /// 営業所DataGridの編集終了イベント
        /// 営業所DataGrid以外も操作可能とする。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridViewOffice_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataModel.IsApplyConfig = true;
        }

        /// <summary>
        /// 営業所DataGridの変更中イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridViewOffice_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            if (grid.IsCurrentCellDirty)
            {
                // チェックボックスのセルだけ入力即、確定する。
                // ただし、最後の1件のチェックを外す場合は、確定できない
                if (grid.CurrentCell.ColumnIndex == 0)
                {
                    var cuurent = officeInfoDataSourceBindingSource.Current as OfficeInfo;
                    if (DataModel.GetVisibleOfficeCount() == 1 && cuurent.Visible)
                    {
                        grid.CancelEdit();
                    }
                    else
                    {
                        grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
                    }
                    grid.EndEdit();
                }
            }
        }

        private void ButtonOpenLogDirectory_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (DialogResult.OK == result)
            {
                dataModel.LogFileDirectory = folderBrowserDialog1.SelectedPath;
            }
        }

        private void ButtonDirectoryClear_Click(object sender, EventArgs e)
        {
            dataModel.LogFileDirectory = string.Empty;
        }

        private void serverInfoDataSourceBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void serverInfoDataSourceBindingSource_BindingComplete(object sender, BindingCompleteEventArgs e)
        {
            Debug.WriteLine("aaaaa-----------------");
        }
    }
}
