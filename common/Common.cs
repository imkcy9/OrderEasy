using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
namespace OrderEasy.common
{
    public class tb_Version
    {
        public int ID;
        public string Name;
        public string Version;
        public string Path;
    }
    struct sqlCommand
    {
        //public const string mySqlCon = "Database='rctech';Data Source='192.168.0.10';User Id='test';Password='test';charset='utf8'";
        public const string selectStock = "select t.stock_code,t.tick,t.active,ifnull(t.stock_name,'') stock_name from sb_trading_control.sb_stock t";
        public const string selectFutureInfo = "select Product,Tip,Scale from tb_Product where IsActiva=1 and IsComb=0 ;";
        public const string selectFutureInstrument = "select Product,Pair from tb_Product a,tb_Pair b where a.id=b.ProductID and a.IsActiva=1 and a.IsComb=0";
        public const string selectAccountPair = "select SubAccount,Account,Name,(SELECT IP as ip  from tb_Server where tb_Server.ID= tb_Account.ServerID) as sererip,(SELECT  `Port`  from tb_Server where tb_Server.ID= tb_Account.ServerID) as ipPort from tb_Account  ;";
    }
    class Future
    {
        public string product = "";
        public bool isActive = false;
        public double tick = 0;
        public int point = 0;
        public string symbols = "";
        public Dictionary<string, Symbol> symbolDic = new Dictionary<string, Symbol>();
        public List<string> instrument = new List<string>();
    }
    class Symbol
    {
        public string code = "";
        public string name = "";
        public bool isActive = false;

    }
    class Stock
    {
        public string code = "";
        public string name = "";
        public double tick = 0;
        public int point = 0;
        public bool isActive = false;
    }

    class AccountInfo
    {
        public string subaccount = "";
        public string motherAccount = "";
        public string name = "";
        public string sertverIp = "";
        public string ipPort = "";
    }
    class Common
    {
        public string currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + "\n";

        private static Common subClass = null;
        public Configuration config;
        public Dictionary<string, Future> futureDic = new Dictionary<string, Future>();
        public Dictionary<string, Future> stockDic = new Dictionary<string, Future>();
        public Dictionary<string, bool> addrDic = new Dictionary<string, bool>();
        public Dictionary<string, AccountInfo> AccountDic = new Dictionary<string, AccountInfo>();
        public string routerAddr = "";
        public string subaccount = "";
        public string control_id = "";
        private string sqlServerCon = "";
        private string mysqlCon = "";
        private string sliptKey = "";
        public string category = "";
        public string instrument = "";
        public static Common Instance()
        {
            if (subClass == null)
            {
                subClass = new Common();
            }
            return subClass;
        }
        public void Init()
        {
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            SetDic(addrDic, "subAddr");
            //mysqlCon = sqlCommand.mySqlCon;
            mysqlCon = ConfigurationManager.AppSettings["mySqlCon"];
            //routerAddr = ConfigurationManager.AppSettings["routerAddr"];
            subaccount = ConfigurationManager.AppSettings["subaccount"];
            //traderAccount = ConfigurationManager.AppSettings["traderAccount"];
            sliptKey = "_";
            category = ConfigurationManager.AppSettings["category"];
            string strat = ConfigurationManager.AppSettings["strat"];
            instrument = ConfigurationManager.AppSettings["instrument"];
            control_id = "easy_" + subaccount + "_" + category + "_" + strat;
            SetFutureDic(sqlCommand.selectFutureInfo, sqlCommand.selectFutureInstrument);
            SetStockDic(sqlCommand.selectStock);
            SetAccount(sqlCommand.selectAccountPair);

        }

        private int getPointCount(string point)
        {
            if (!point.Contains('.'))
            {
                return 0;
            }
            return point.Split('.')[1].Length;
        }
        private void SetAccount(string sql)
        {
            List<string> list = SelectData(sql);
            foreach (string value in list)
            {
                AccountInfo accIn = new AccountInfo();
                
                string[] data = value.Split(sliptKey[0]);
                accIn.subaccount = data[0];
                accIn.motherAccount = data[1];
                accIn.name = data[2];
                accIn.sertverIp = data[3];
                accIn.ipPort = data[4];
                AccountDic.Add(data[0], accIn);
            }
            //AccountDic.Add(subaccount, traderAccount);
        }
        private void SetStockDic(string sql)
        {
            List<string> list = SelectData(sql);
            foreach (string value in list)
            {
                Future f = new Future();
                string[] data = value.Split(sliptKey[0]);
                f.product = data[0];
                double.TryParse(data[1], out f.tick);
                f.isActive = data[2] == "1" ? true : false;
                f.point = getPointCount(data[1]);
                stockDic.Add(f.product, f);
            }
        }
        class Info
        {
            public string Product { get; set; }
            public double Tick { get; set; }
            public bool Active { get; set; }

        }
        private void SetFutureDic(string sql, string sql2)
        {
            List<string> list = SelectData(sql);
            List<Future> listF = new List<Future>();
            foreach (string value in list)
            {
                Future f = new Future();
                string[] data = value.Split(sliptKey[0]);
                f.product = data[0];
                double.TryParse(data[1], out f.tick);
                f.point = getPointCount(data[1]);
                f.isActive = true;// data[2] == "1" ? true : false;
                //f.symbols = data[3] + this.sliptKey+data[4];
                listF.Add(f);
            }

            foreach (Future f in listF)
            {
                List<string> list2 = SelectData(sql2 + " and Product=" + "\"" + f.product + "\"");
                foreach (string lis in list2)
                {
                    string[] strLis = lis.Split(sliptKey[0]);
                    if (f.product == strLis[0])
                    {
                        string[] InstruLis = strLis[1].Split('-');
                        if (!f.instrument.Contains(InstruLis[0]))
                            f.instrument.Add(InstruLis[0]);
                        if (!f.instrument.Contains(InstruLis[1]))
                            f.instrument.Add(InstruLis[1]);
                    }
                }
                if(list2.Count != 0)
                    Common.Instance().futureDic.Add(f.product, f);
            }
            

            /*var results = (from fut in listF.AsEnumerable()
                           select fut.product).Distinct();
            foreach (string p in results)
            {
                
                var symbols = from fut in listF.AsEnumerable()
                              where fut.product == p
                              select fut;
                
                Future future = new Future();
                future.product = p;
                foreach (Future s in symbols)
                {
                    string str = s.symbols;
                    Symbol symbol = new Symbol();
                    symbol.code = str.Split(this.sliptKey[0])[0];
                    symbol.isActive = true;// str.Split(this.sliptKey[0])[1] == "1" ? true : false;
                    future.isActive = s.isActive;
                    future.point = s.point;
                    future.tick = s.tick;
                    future.symbols += symbol.code + sliptKey;
                    future.symbolDic.Add(symbol.code, symbol);
                }
                Common.Instance().futureDic.Add(future.product, future);
            } */

           /* DataTable dt = SelectData(sql);
            DataView view = new DataView(dt);
            DataTable distinctValues = view.ToTable(true, "product", "tick", "pActive");

            foreach(DataRow dr in distinctValues.Rows){
                Future f = new Future();
                f.product = Convert.ToString(dr["product"]);
                double.TryParse(Convert.ToString(dr["tick"]), out f.tick);
                f.isActive = Convert.ToString(dr["pActive"]) == "1" ? true : false;

                var results = from myRow in dt.AsEnumerable()
                              where myRow.Field<string>("product") == f.product
                              select myRow;
                foreach (DataRow sDr in results)
                {
                    Symbol symbol = new Symbol();
                    symbol.code = Convert.ToString(sDr["symbol_code"]);
                    symbol.name = Convert.ToString(sDr["symbol_name"]);
                    symbol.isActive = Convert.ToString(sDr["sActive"]) == "1" ? true : false;
                    f.symbolDic.Add(symbol.code, symbol);
                    string s = symbol.isActive ? "?1" : "?0";
                    f.symbols += symbol.code + s + "|";
                }
                futureDic.Add(f.product, f);
            }*/
        }
        public void SaveConfig()
        {
            //1001?0_1002?1_1003?1_1004?1_1005?1_1006?1
            //SaveOneConfigKey("account", futureDic);
            //IF?0_RU?1_CU?1_T1?1_T2?1_T3?1_T4?1_T5?1_T6?1_T7?1_T8?1
            //SaveOneConfigKey("product", productDic);
            //tcp://192.168.0.10:6006?1
            SaveOneConfigKey("subAddr", addrDic);
            config.Save(ConfigurationSaveMode.Modified);

            
        }

        public int GetFutureDicCount()
        {
            int count = 0;
            foreach (KeyValuePair<string, Future> pair in futureDic)
            {
                if (pair.Value.isActive)
                    count++;
            }
            return count;
        }

        public int GetStockCount()
        {
            int count = 0;
            foreach (KeyValuePair<string, Future> pair in stockDic)
            {
                if (pair.Value.isActive)
                    count++;
            }
            return count;
        }
        public string InsertData(string insertSql)
        {

            using (SqlConnection connection = new SqlConnection(sqlServerCon))
            {
                //connection.Open();
                //SqlCommand command = new SqlCommand("Insert_TB_Profit", connection);
                //command.CommandType = CommandType.Text;//.StoredProcedure;
                ////command.Parameters.Add(new SqlParameter("@EmployeeID", employeeID));
                //command.CommandTimeout = 5;

                //command.ExecuteNonQuery(); 
                using (SqlDataAdapter dapt = new SqlDataAdapter(insertSql, connection))
                {
                    try
                    {
                        dapt.InsertCommand = new SqlCommand(insertSql);
                        DataTable table = new DataTable();
                        dapt.Fill(table);
                    }
                    catch (System.Exception ex)
                    {
                        Program.log.Error(ex);
                        return ex.Message;
                    }
                }

            }
            return "";
        }

        public List<string> SelectData(string selectSql)
        {
            List<string> retList = new List<string>();
            MySqlConnection conn = null;
            //MySqlDataReader rdr = null;
            try
            {
                conn = new MySqlConnection(mysqlCon);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(selectSql, conn);

                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        StringBuilder oneData = new StringBuilder();
                        for (int i = 0; i < rdr.FieldCount;i++ )
                        {
                            oneData.Append(rdr.GetValue(i).ToString() + sliptKey);
                        }
                        retList.Add(oneData.ToString());
                    }
                }
                
            }
            catch (Exception e)
            {
                Program.log.Error(e, e);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
            return retList;
        }

        /// <summary>
        /// return format "yyyyMMdd"
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string FormartTime(DateTime dt)
        {

            return dt.ToString("yyyyMMdd");
        }


        #region private


        /// <summary>
        /// 
        /// </summary>
        /// <param name="str">1001?1</param>
        /// <returns>1001</returns>
        private string getKey(string str)
        {
            if (str.Contains("?"))
                return str.Split('?')[0];
            return "";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str">1001?1</param>
        /// <returns>0:false 1:true</returns>
        private bool getValue(string str)
        {

            if (str.Contains("?"))
                return str.Split('?')[1] == "1" ? true : false;
            return false;

        }

        private void SetCapitalDic(Dictionary<string, double> dic, string key)
        {
            foreach (string data in ConfigurationManager.AppSettings[key].Split('_'))
            {
                double capital = 0;
                if (!double.TryParse(data.Split('?')[1], out capital))
                {
                    Program.log.Error("Capital error ! '" + data + "' is not a double data ");
                }
                dic.Add(getKey(data), capital);
            }
        }
        private void SetDic(Dictionary<string, bool> dic, string key)
        {
            foreach (string data in ConfigurationManager.AppSettings[key].Split('_'))
            {
                dic.Add(getKey(data), getValue(data));
            }
        }
        private void SaveOneConfigKey(string key, Dictionary<string, bool> dic)
        {

            //1001?0_1002?1_1003?1_1004?1_1005?1_1006?1
            string value = "";
            foreach (KeyValuePair<string, bool> pair in dic)
            {
                int isCheck = pair.Value ? 1 : 0;
                value += pair.Key + "?" + isCheck + "_";

            }
            value = value.Remove(value.Length - 1);

            config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(key, value);

        }
        public void set_control_id(string _control_id)
        {
            string strat = ConfigurationManager.AppSettings["strat"];
            control_id = _control_id + "_" + strat;
        }
        public bool CompareVersion()
        {
            try
            {
                string versionName;
                versionName = "order_easy";
                //tb_Version version = common.SelectVersion(versionName);
                tb_Version version = new tb_Version();
                version.ID = 1;
                version.Name = "order_easy";
                version.Version = "1.0.0.2";
                if (version != null)
                {
                    if (currentVersion != version.Version)
                    {
                        if (MessageBox.Show("发现新版本" + version.Version + "，是否更新？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            string updatePath = Application.StartupPath + "\\Update\\Update.exe";
                            System.Diagnostics.Process.Start(updatePath, version.Path);
                            System.Environment.Exit(System.Environment.ExitCode);
                            Program.log.Info("更新为版本：" + version.Version);
                            return true;
                        }
                    }
                }
                else
                {
                    string msg = "错误：获取版本信息失败！";
                    Program.log.Error(msg);
                    MessageBox.Show(msg);
                }
            }
            catch (Exception ex)
            {
                string msg = "错误：获取版本信息失败！";
                Program.log.Error(msg + ex.Message);
                MessageBox.Show(msg);
            }
            return false;
        }
        //public tb_Version SelectVersion(string Name)
        //{
        //    try
        //    {
        //        DataTable table = new DataTable();
        //        string strVersion = "select * from tb_Version where Name='" + Name + "'";
        //        using (MySqlConnection sqlConn = new MySqlConnection(strRcConn))
        //        {
        //            using (MySqlDataAdapter dapt = new MySqlDataAdapter(strVersion, sqlConn))
        //            {
        //                dapt.Fill(table);
        //            }
        //        }

        //        tb_Version version = new tb_Version();
        //        if (table.Rows.Count > 0)
        //        {
        //            version.ID = Convert.ToInt32(table.Rows[0]["ID"]);
        //            version.Name = table.Rows[0]["Name"].ToString();
        //            version.Version = table.Rows[0]["Version"].ToString();
        //            version.Path = table.Rows[0]["Path"].ToString();
        //        }
        //        return version;
        //    }
        //    catch (Exception ex)
        //    {
        //        string msg = "错误：获取最新版本信息失败！";
        //        Program.log.Error(msg + ex.Message);
        //        MessageBox.Show(msg);
        //        return null;
        //    }
        //}
        #endregion

    }
}
