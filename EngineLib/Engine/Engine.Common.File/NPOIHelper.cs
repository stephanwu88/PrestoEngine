using System;
using System.Data;
using System.IO;
using System.Text;

using System.Windows.Forms;

//using NPOI.HPSF;
//using NPOI.HSSF.UserModel;
//using NPOI.SS.UserModel;
//using NPOI.XSSF.UserModel;

namespace Engine.Files
{
    public class NPOIHelper
    {
        ///// <summary>
        ///// 导出到Excel
        ///// 该方法只能生成一个Tab的Excel
        ///// </summary>
        ///// <param name="obj"></param>
        //public static bool ExportToExcel(DataTable dtSource)
        //{
        //    try
        //    {
        //        SaveFileDialog openTemp = new SaveFileDialog();
        //        openTemp.InitialDirectory = @"D:\";
        //        openTemp.Filter = "Excel (*.xlsx)|*.xlsx|Excel 97-2003 (*.xls)|*.xls|";
        //        openTemp.FileName = DateTime.Now.ToString("yyyy-MM-dd").ToString();
        //        openTemp.FilterIndex = 1;
        //        openTemp.RestoreDirectory = true;
        //        if (openTemp.ShowDialog() == DialogResult.OK)
        //        {
        //            string strTemp = openTemp.FileName;
        //            if (System.IO.File.Exists(strTemp))
        //            {
        //                DialogResult ret = MessageBox.Show("该文件已存在，是否覆盖？", "文件导出", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        //                if (ret == DialogResult.Yes)
        //                    return ExportToExcel(dtSource, "", strTemp);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("数据导出失败，原因:" + ex.ToString(), "文件导出", MessageBoxButtons.OK);
        //    }
        //    return false;
        //}

        ///// <summary>
        ///// 导出到Excel
        ///// </summary>
        ///// <param name="dtSource"></param>
        ///// <param name="strHeaderText"></param>
        ///// <param name="strFileName"></param>
        ///// <returns></returns>
        //public static bool ExportToExcel(DataTable dtSource, string strHeaderText, string strFileName)
        //{
        //    try
        //    {
        //        string strExtension = System.IO.Path.GetExtension(strFileName);
        //        if (strExtension.ToLower() == ".xls")
        //        {
        //            var WorkBook = BuildXlsWorkBook(dtSource, strHeaderText);
        //            using (var fs = System.IO.File.OpenWrite(strFileName))
        //                WorkBook.Write(fs);
        //        }
        //        else if (strExtension.ToLower() == ".xlsx")
        //        {
        //            var WorkBook = BuildXlsxWorkBook(dtSource, strHeaderText);
        //            using (var fs = System.IO.File.OpenWrite(strFileName))
        //                WorkBook.Write(fs);
        //        }
        //        else
        //            throw new Exception("文件格式指定错误!");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.ToString());
        //    }
        //    return true;
        //}

        ///// <summary>
        ///// 从Excel导入
        ///// </summary>
        ///// <param name="strFileName"></param>
        ///// <returns></returns>
        //public static DataTable ImportByExcel(string strFileName)
        //{
        //    try
        //    {
        //        string strExtension = System.IO.Path.GetExtension(strFileName);
        //        if (strExtension.ToLower() == ".xls")
        //            return ImportByXls(strFileName);
        //        else if (strExtension.ToLower() == ".xlsx")
        //            return ImportByXlsx(strFileName);
        //        else
        //            throw new Exception("数据导入失败，原因:文件格式指定错误!");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("数据导出失败，原因:" + ex.ToString());
        //    }
        //}

        #region 创建工作表内部方法
        /// <summary>
        /// 创建Xls工作表
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        //private static HSSFWorkbook BuildXlsWorkBook(DataTable dtSource, string strHeaderText = "")
        //{
        //    //创建工作薄
        //    var workbook = new HSSFWorkbook();
        //    //创建工作表
        //    ISheet sheet = workbook.CreateSheet(string.IsNullOrWhiteSpace(dtSource.TableName) ? "Sheet1" : dtSource.TableName);

        //    #region 右击文件 属性信息
        //    {
        //        DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
        //        dsi.Company = "NPOI";
        //        workbook.DocumentSummaryInformation = dsi;

        //        SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
        //        si.Author = "Elvis.Wu";               //填加文件作者信息
        //        si.ApplicationName = "自动导出程序";  //填加文件创建程序信息
        //        si.LastAuthor = "Elvis.Wu";           //填加文件最后保存者信息
        //        si.Comments = "Elvis.Wu";             //填加文件作者信息
        //        si.Title = "归档导出文件";            //填加文件标题信息

        //        si.Subject = "导出文件";//填加文件主题信息
        //        si.CreateDateTime = DateTime.Now;
        //        workbook.SummaryInformation = si;
        //    }
        //    #endregion

        //    ICellStyle dateStyle = workbook.CreateCellStyle();
        //    IDataFormat format = workbook.CreateDataFormat();
        //    dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

        //    //取得列宽
        //    int[] arrColWidth = new int[dtSource.Columns.Count];
        //    foreach (DataColumn item in dtSource.Columns)
        //    {
        //        arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
        //    }
        //    for (int i = 0; i < dtSource.Rows.Count; i++)
        //    {
        //        for (int j = 0; j < dtSource.Columns.Count; j++)
        //        {
        //            int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
        //            if (intTemp > arrColWidth[j])
        //            {
        //                arrColWidth[j] = intTemp;
        //            }
        //        }
        //    }
        //    int rowIndex = 0;
        //    foreach (DataRow row in dtSource.Rows)
        //    {
        //        #region 新建表，填充表头，填充列头，样式
        //        if (rowIndex == 65535 || rowIndex == 0)
        //        {
        //            if (rowIndex != 0)
        //            {
        //                sheet = workbook.CreateSheet();
        //            }

        //            #region 表头及样式
        //            {
        //                var headerRow = sheet.CreateRow(0);
        //                headerRow.HeightInPoints = 25;
        //                headerRow.CreateCell(0).SetCellValue(strHeaderText);
        //                //CellStyle
        //                ICellStyle headStyle = workbook.CreateCellStyle();
        //                headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; //左右居中
        //                headStyle.VerticalAlignment = VerticalAlignment.Center;// 上下居中 
        //                // 设置单元格的背景颜色（单元格的样式会覆盖列或行的样式）    
        //                headStyle.FillForegroundColor = (short)11;
        //                //定义字体
        //                IFont font = workbook.CreateFont();
        //                font.FontHeightInPoints = 20;
        //                //font.Boldweight = 700;
        //                font.IsBold = true;
        //                headStyle.SetFont(font);
        //                headerRow.GetCell(0).CellStyle = headStyle;
        //                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));
        //            }
        //            #endregion

        //            #region 列头及样式
        //            {
        //                var headerRow = sheet.CreateRow(1);
        //                ICellStyle headStyle = workbook.CreateCellStyle();
        //                //左右居中 
        //                headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
        //                // 上下居中
        //                headStyle.VerticalAlignment = VerticalAlignment.Center;
        //                //定义字体
        //                IFont font = workbook.CreateFont();
        //                font.FontHeightInPoints = 10;
        //                //font.Boldweight = 700;
        //                font.IsBold = true;
        //                headStyle.SetFont(font);
        //                foreach (DataColumn column in dtSource.Columns)
        //                {
        //                    headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
        //                    headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
        //                    //设置列宽
        //                    sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
        //                }
        //            }
        //            #endregion
        //            //添加默认表头文本
        //            if (strHeaderText != "")
        //            {
        //                IRow row0 = sheet.CreateRow(0);
        //                for (int i = 0; i < dtSource.Columns.Count; i++)
        //                {
        //                    ICell cell = row0.CreateCell(i, CellType.String);
        //                    cell.SetCellValue(dtSource.Columns[i].ColumnName);
        //                }
        //            }
        //            rowIndex = 2;
        //        }
        //        #endregion

        //        #region 填充内容
        //        var dataRow = sheet.CreateRow(rowIndex);
        //        foreach (DataColumn column in dtSource.Columns)
        //        {
        //            var newCell = dataRow.CreateCell(column.Ordinal);
        //            string drValue = row[column].ToString();
        //            switch (column.DataType.ToString())
        //            {
        //                case "System.String"://字符串类型
        //                    newCell.SetCellValue(drValue);
        //                    break;
        //                case "System.DateTime"://日期类型
        //                    DateTime dateV;
        //                    DateTime.TryParse(drValue, out dateV);
        //                    newCell.SetCellValue(dateV);

        //                    newCell.CellStyle = dateStyle;//格式化显示
        //                    break;
        //                case "System.Boolean"://布尔型
        //                    bool boolV = false;
        //                    bool.TryParse(drValue, out boolV);
        //                    newCell.SetCellValue(boolV);
        //                    break;
        //                case "System.Int16"://整型
        //                case "System.Int32":
        //                case "System.Int64":
        //                case "System.Byte":
        //                    int intV = 0;
        //                    int.TryParse(drValue, out intV);
        //                    newCell.SetCellValue(intV);
        //                    break;
        //                case "System.Decimal"://浮点型
        //                case "System.Double":
        //                    double doubV = 0;
        //                    double.TryParse(drValue, out doubV);
        //                    newCell.SetCellValue(doubV);
        //                    break;
        //                case "System.DBNull"://空值处理
        //                    newCell.SetCellValue("");
        //                    break;
        //                default:
        //                    newCell.SetCellValue("");
        //                    break;
        //            }
        //        }
        //        #endregion

        //        rowIndex++;
        //    }
        //    //自动列宽
        //    for (int i = 0; i <= dtSource.Columns.Count; i++)
        //        sheet.AutoSizeColumn(i, true);

        //    return workbook;
        //}

        /// <summary>
        /// 创建Xlsx工作表
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        /// <returns></returns>
        //private static XSSFWorkbook BuildXlsxWorkBook(DataTable dtSource, string strHeaderText = "")
        //{

        //    return new XSSFWorkbook();
        //}
        #endregion

        #region 从Excel导入内部方法
        ///// <summary>
        ///// 读取xls版本
        ///// 默认第一行为标头
        ///// </summary>
        ///// <param name="strFileName">excel文件路径</param>
        ///// <returns></returns>
        //private static DataTable ImportByXls(string strFileName)
        //{
        //    DataTable dt = new DataTable();

        //    HSSFWorkbook workbook;
        //    using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
        //    {
        //        workbook = new HSSFWorkbook(file);
        //    }
        //    ISheet sheet = workbook.GetSheetAt(0);
        //    System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

        //    IRow headerRow = sheet.GetRow(0);
        //    int cellCount = headerRow.LastCellNum;

        //    for (int j = 0; j < cellCount; j++)
        //    {
        //        ICell cell = headerRow.GetCell(j);
        //        dt.Columns.Add(cell.ToString());
        //    }

        //    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
        //    {
        //        IRow row = sheet.GetRow(i);
        //        DataRow dataRow = dt.NewRow();

        //        for (int j = row.FirstCellNum; j < cellCount; j++)
        //        {
        //            if (row.GetCell(j) != null)
        //                dataRow[j] = row.GetCell(j).ToString();
        //        }

        //        dt.Rows.Add(dataRow);
        //    }
        //    return dt;
        //}

        ///// <summary>
        ///// 读取xlsx版本
        ///// 默认第一行为标头
        ///// </summary>
        ///// <param name="strFileName">excel文件路径</param>
        ///// <returns></returns>
        //private static DataTable ImportByXlsx(string strFileName)
        //{
        //    DataTable dt = new DataTable();

        //    XSSFWorkbook workbook;
        //    using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
        //    {
        //        workbook = new XSSFWorkbook(file);
        //    }
        //    ISheet sheet = workbook.GetSheetAt(0);
        //    System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

        //    IRow headerRow = sheet.GetRow(0);
        //    int cellCount = headerRow.LastCellNum;

        //    for (int j = 0; j < cellCount; j++)
        //    {
        //        ICell cell = headerRow.GetCell(j);
        //        dt.Columns.Add(cell.ToString());
        //    }

        //    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
        //    {
        //        IRow row = sheet.GetRow(i);
        //        DataRow dataRow = dt.NewRow();

        //        for (int j = row.FirstCellNum; j < cellCount; j++)
        //        {
        //            if (row.GetCell(j) != null)
        //                dataRow[j] = row.GetCell(j).ToString();
        //        }

        //        dt.Rows.Add(dataRow);
        //    }
        //    return dt;
        //}
        #endregion
    }
}