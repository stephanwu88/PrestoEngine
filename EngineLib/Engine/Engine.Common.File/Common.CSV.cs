using Engine.Common;
using Engine.WpfControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Engine.Files
{
    /// <summary>
    /// 文件导入导出
    /// </summary>
    public static class FileEIO
    {
        public static void ExportCSV(this DataGridEx dataGrid)
        {
            try
            {
                string strFile = sCommon.ShowSaveFileDialog(DateTime.Now.ToString("yyyy-MM-dd"), "(Excel文件)|*.xls|(CSV文件)|*.csv", 1, "导出表格文件");
                if (File.Exists(strFile))
                    File.Delete(strFile);
                bool ret = ExportToCSV(dataGrid, strFile);
                if (ret)
                    sCommon.MyMsgBox("导出成功!");
            }
            catch (Exception ex)
            {
                sCommon.MyMsgBox("导出失败！\r\n" + ex.Message);
            }
        }

        public static void ExportCSV(this DataGrid dataGrid)
        {
            try
            {
                string strFile = sCommon.ShowSaveFileDialog(DateTime.Now.ToString("yyyy-MM-dd"), "(Excel文件)|*.xls", 1, "导出表格文件");
                if (File.Exists(strFile))
                    File.Delete(strFile);
                bool ret = ExportToCSV(dataGrid, strFile);
                if (ret)
                    sCommon.MyMsgBox("导出成功!");
            }
            catch (Exception ex)
            {
                sCommon.MyMsgBox("导出失败！\r\n" + ex.Message);
            }
        }

        public static void ExportCSV(this DataTable Table)
        {
            try
            {
                string strFile = sCommon.ShowSaveFileDialog(DateTime.Now.ToString("yyyy-MM-dd"), "(Excel文件)|*.xls", 1, "导出表格文件");
                if (File.Exists(strFile))
                    File.Delete(strFile);
                bool ret = ExportToCSV(Table, strFile);
                if (ret)
                    sCommon.MyMsgBox("导出成功!");
            }
            catch (Exception ex)
            {
                sCommon.MyMsgBox("导出失败！\r\n" + ex.Message);
            }
        }

        /// <summary>
        /// DataTable导出到CSV
        /// </summary>
        private static bool ExportToCSV(DataTable TableSource, string fileName = "")
        {
            string strSplitSign = ",";
            string TextData = string.Empty;
            if (Directory.Exists(fileName)) throw new Exception("导出文件的目录不存在");
            DataTable dt = TableSource.ToMyDataTable();
            if (dt.Rows.Count == 0) throw new Exception("导出表的数据为空");
            if (dt.Rows.Count >= 20000) throw new Exception("最多可导出数据20000条");
           
            //添加表头
            List<string> LstTableHeader = new List<string>();
            foreach (DataColumn col in dt.Columns)
            {
                string ColName = string.Format("\"{0}\"", col.ColumnName.Replace("\"", "\"\""));
                LstTableHeader.Add(ColName);
            }
            TextData = string.Join(strSplitSign, LstTableHeader) + "\r\n";
            //添加行数据
            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    string ColName = col.ColumnName;
                    object RowValue = row[col];
                    string strRowValue = string.Empty;
                    if (RowValue != null && RowValue != DBNull.Value)
                    {
                        // 根据列类型进行格式化
                        switch (col.DataType.Name)
                        {
                            case "DateTime":
                                DateTime datetime = RowValue.ToMyDateTime();
                                strRowValue = datetime.ToString("yyyy-MM-dd HH:mm:ss");
                                break;
                            case "Boolean":
                                bool b = (bool)RowValue;
                                strRowValue = b ? "true" : "false";
                                break;
                            default:
                                strRowValue = RowValue.ToMyString();
                                break;
                        }
                    }
                    //string RowValue = row[ColName].ToMyString().Replace(strSplitSign, "-");
                    strRowValue = strRowValue.Replace("\"", "\"\"").Replace(strSplitSign, "_");
                    strRowValue = string.Format("\" {0}\"",strRowValue);
                    TextData += string.IsNullOrEmpty(strRowValue) ? strSplitSign : RowValue + strSplitSign;
                    TextData.Remove(TextData.Length - 1);
                }
                TextData += "\r\n";
            }
            //输出文件
            File.WriteAllText(fileName, TextData, Encoding.UTF8);
            return true;
        }

        /// <summary>
        /// DataTable导出到CSV 
        /// 从DataGrid取列
        /// </summary>
        private static bool ExportToCSV(DataGrid dataGrid, string fileName = "")
        {
            string strSplitSign = ",";
            string TextData = string.Empty;
            if (Directory.Exists(fileName)) throw new Exception("导出文件的目录不存在");
            DataTable dt = dataGrid.ItemsSource.ToMyDataTable();
            if (dt.Rows.Count == 0) throw new Exception("导出表的数据为空");
            if (dt.Rows.Count >= 20000) throw new Exception("最多可导出数据20000条");
            //添加表头
            List<string> LstTableHeader = new List<string>();
            foreach (DataGridColumn col in dataGrid.Columns)
            {
                if (col.Visibility != Visibility.Visible)
                    continue;
                if (col is DataGridTextColumn txCol)
                {
                    //string strHeader = txCol.Header.ToMyString();
                    string strHeader = string.Format("\"\t{0}\"", txCol.Header.ToMyString().Replace("\"", "\"\""));
                    LstTableHeader.Add(strHeader);
                }
            }
            TextData = string.Join(strSplitSign, LstTableHeader) + "\r\n";
            //添加行数据
            foreach (DataRow row in dt.Rows)
            {
                foreach (DataGridColumn col in dataGrid.Columns)
                {
                    if (col.Visibility != Visibility.Visible)
                        continue;
                    DataGridTextColumn txCol = col as DataGridTextColumn;
                    string strHeader = txCol.Header.ToMyString();
                    string path = ((System.Windows.Data.Binding)txCol.Binding).Path.Path;
                    if (!dt.Columns.Contains(path))
                        continue;
                    object RowValue = row[path];
                    string strRowValue = string.Empty;
                    if (RowValue != null && RowValue != DBNull.Value)
                    {
                        // 根据列类型进行格式化
                        switch (col.GetType().Name)
                        {
                            case "DateTime":
                                DateTime datetime = RowValue.ToMyDateTime();
                                strRowValue = datetime.ToString("yyyy-MM-dd HH:mm:ss");
                                break;
                            case "Boolean":
                                bool b = (bool)RowValue;
                                strRowValue = b ? "true" : "false";
                                break;
                            default:
                                strRowValue = RowValue.ToMyString();
                                break;
                        }
                    }
                    //string strRowValue = row[path].ToMyString().Replace(",", "-");
                    strRowValue = strRowValue.Replace("\"", "\"\"").Replace(",", "_"); 
                    strRowValue = string.Format("\"\t{0}\"", strRowValue);
                    TextData += string.IsNullOrEmpty(strRowValue) ? "," : strRowValue + ",";
                    TextData.Remove(TextData.Length - 1);
                }
                TextData += "\r\n";
            }
            //输出文件
            File.WriteAllText(fileName, TextData, Encoding.UTF8);
            return true;
        }

        /// <summary>
        /// 结束进程 
        /// </summary>
        public static void KillSpecialExcel()
        {
            foreach (System.Diagnostics.Process theProc in System.Diagnostics.Process.GetProcessesByName("EXCEL"))
            {
                if (!theProc.HasExited)
                {
                    bool b = theProc.CloseMainWindow();
                    if (b == false)
                    {
                        theProc.Kill();
                    }
                    theProc.Close();
                }
            }
        }

        /// <summary>
        /// 字符串写入文件
        /// </summary>
        /// <param name="str"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool FileWrite(string str,string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(fileName)))
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fileName));
                FileStream fs = new FileStream(fileName, FileMode.Create);
                byte[] buf = Encoding.UTF8.GetBytes(str);
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
                fs.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 读文件字符串
        /// </summary>
        /// <param name="str">读文件内容</param>
        /// <returns></returns>
        public static bool FileRead(out string str,string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    FileInfo fi = new FileInfo(fileName);
                    long len = fi.Length;
                    FileStream fs = new FileStream(fileName, FileMode.Open);
                    byte[] buffer = new byte[len];
                    fs.Read(buffer, 0, (int)len);
                    fs.Close();
                    str = Encoding.UTF8.GetString(buffer);
                    return true;
                }
            }
            catch (Exception )
            {

            }
            str = string.Empty;
            return false;
        }
    }
}
