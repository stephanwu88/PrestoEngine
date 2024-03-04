using Engine.Common;
using Engine.Data.DBFAC;
using System;
using System.Data;
using System.Data.OleDb;

namespace Engine.Files
{
    /// <summary>
    /// Excel 输入输出
    /// </summary>
    public class ExcelEIO
    {
        public IDBFactory<LocalSource> _DB;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExcelEIO()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ds"></param>
        public ExcelEIO(LocalSource ds)
        {

        }

        /// <summary>
        /// 查询Excel整张表
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public DataTable GetDataTable(LocalSource ds, string TableName = "Sheet1")
        {
            if (_DB == null)
                _DB = DbCommon.CreateInstance(ds);
            _DB.ConNode = ds;
            return GetDataTable(TableName);
        }

        /// <summary>
        /// 查询Excel整张表
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string TableName = "Sheet1")
        {
            DataTable dt = _DB.ExcuteQuery(string.Format("select * from [{0}$]", TableName)).Result as DataTable;
            return dt;
        }

        public void GetExcelInfo(string excelFile, string connectionString)
        {
            string strConn = string.Empty;
            try
            {
                OleDbConnection conn = new OleDbConnection(strConn);
                conn.Open();
                string strExcel = "";
                DataTable table = conn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);
                string tableName = table.Rows[0]["Table_Name"].ToString();//⾃动读取第⼀个表的表名

                string sheetName;
                sheetName = tableName.Substring(0, tableName.Length - 1);
                string strExcel_Num = string.Format(@"select count(*) from [{0}$]", sheetName);
                OleDbDataAdapter myCommand_Num = new OleDbDataAdapter(strExcel_Num, strConn);
                DataSet ds_CloNum = new DataSet();
                myCommand_Num.Fill(ds_CloNum, sheetName);

                int RowNum = (int)ds_CloNum.Tables[0].Rows[0].ItemArray[0];//    获取总⾏数
                int stat = 0;
                int end = 10000;

                double loopTimes = Math.Ceiling(Convert.ToDouble(RowNum * 1.0 / 10000));//控制循环次数，向上取整
                for (int i = 0; i < loopTimes; i++)
                {

                    //表格中需要加⼀列id是⾃动排序的，⽤来分割数据，若⽤access语句在查询结果上加个序号列也⾏
                    strExcel = string.Format(@"select * from [{0}$] where  id >={1} and id <{2}", sheetName, stat, end);
                    stat = stat + 10000;
                    end = end + 10000;
                    OleDbDataAdapter myCommand = new OleDbDataAdapter(strExcel, strConn);
                    DataSet ds = new DataSet();
                    myCommand.Fill(ds, sheetName);

                    //若⽬标表不存在则创建
                    string strSql = string.Format("if object_id('{0}') is null create table {0}(", sheetName);
                    foreach (System.Data.DataColumn c in ds.Tables[0].Columns)
                    {
                        strSql += string.Format("[{0}] varchar(1000),", c.ColumnName);
                    }
                    strSql = strSql.Trim(',') + ")";
                    using (System.Data.SqlClient.SqlConnection sqlconn = new System.Data.SqlClient.SqlConnection(connectionString))
                    {
                        sqlconn.Open();
                        System.Data.SqlClient.SqlCommand command = sqlconn.CreateCommand();
                        command.CommandText = strSql;
                        command.ExecuteNonQuery();
                        sqlconn.Close();
                    }

                    //⽤bcp导⼊数据
                    using (System.Data.SqlClient.SqlBulkCopy bcp = new System.Data.SqlClient.SqlBulkCopy(connectionString))
                    {
                        bcp.SqlRowsCopied += new System.Data.SqlClient.SqlRowsCopiedEventHandler(bcp_SqlRowsCopied);
                        bcp.BatchSize = 100;//每次传输的⾏数

                        bcp.NotifyAfter = 100;//进度提⽰的⾏数

                        bcp.DestinationTableName = sheetName;//⽬标表
                        bcp.WriteToServer(ds.Tables[0]);
                    }
                }
                sCommon.MyMsgBox("导⼊成功", MsgType.Infomation);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        //进度显示
        void bcp_SqlRowsCopied(object sender, System.Data.SqlClient.SqlRowsCopiedEventArgs e)
        {
            //this.Text = e.RowsCopied.ToString();
            //this.Update();
        }
    }
}
