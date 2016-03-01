using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZeroMQ;
using OrderEasy.common;
using System.Threading;
using order_easy;
using ProtoBuf;
using System.IO;
using TraderControl;
using System.Configuration;

namespace OrderEasy
{

    public partial class LoginForm : Form
    {
        private string routerAddr = "";
        public Thread testThread = null;
        private Common common = Common.Instance();
        private OrderEasy order_easy = new OrderEasy();
        private double current_tick = -1.0;
        public LoginForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            foreach (KeyValuePair<string, Future> pair in common.futureDic)
            {
                Future f = pair.Value;
                if (f.isActive)
                {
                    comb_product.Items.Add(f.product);
                }
            }
            //foreach (KeyValuePair<string, string> pair in common.AccountDic)
            //{
            //    this.comb_account.Items.Add(pair.Key);
            //}
            comb_product.Items.Add("ST");
            comb_account.Text = common.subaccount;
            comb_product.SelectedIndex = comb_product.Items.IndexOf(common.category);
            comb_Instrument.SelectedIndex = comb_Instrument.Items.IndexOf(common.instrument);


            //comb_account.Enabled = false;
        }

        private void initCommon()
        {

            //common = Common.Instance();
            common.Init();

        }
        private void initMdfSub()
        {
            ZMQControl.Instance().Init(ZeroMQ.SocketType.SUB, "");
            foreach (KeyValuePair<string, bool> pair in common.addrDic)
            {
                if (pair.Value)
                {
                    ZMQControl.Instance().Connect(pair.Key);
                }
            }
        }
        private void initZmqDealer()
        {

            if (!common.AccountDic.ContainsKey(comb_account.Text))
            {
                MessageBox.Show("初始化失败，数据库未找到帐号信息");
                return;
            }
            string temAddr = "tcp://" + common.AccountDic[comb_account.Text].sertverIp + ":" + common.AccountDic[comb_account.Text].ipPort;
            if (routerAddr != "" && temAddr != routerAddr)
                ZMQControl.Instance().dealerDisConnect(routerAddr);
            routerAddr = temAddr;
           
            ZMQControl.Instance().InitDealer(SocketType.DEALER, routerAddr, common.control_id);
        }

        private void runZMQ()
        {
            if (testThread == null)
            {
                testThread = new Thread((ThreadStart)delegate { ZMQControl.Instance().run(this, order_easy); });
                testThread.IsBackground = true;
                testThread.Start();
                //testThread.Join();
            }
        }
        private void LoginForm_Load(object sender, EventArgs e)
        {
            initCommon();
            initMdfSub();

        }

        private void button_login_Click(object sender, EventArgs e)
        {

            if (!accountCheck(comb_account.Text))
            {
                MessageBox.Show("未找到数据库对应的母帐号信息");
                return;
            }
            string control_id = "easy_" + comb_account.Text + "_" + comb_product.Text;
            common.set_control_id(control_id);
            ZMQControl.Instance().setControlId(common.control_id);
            //zmqTerm();
            initZmqDealer();
            runZMQ();
            login();
        }

        public void on_login_resp(login_resp data)
        {
             
            Invoke((MethodInvoker)delegate
            {
                if (data.success != 0)
                {
                    MessageBox.Show("登陆失败：" + data.ErrorMsg);
                    return;
                }
                if (data.symbol != comb_Instrument.Text)
                {
                    MessageBox.Show("登陆失败：服务器返回symbol与前端symbol不一致");
                    return;
                }
                common.CompareVersion();

                order_easy.Show();
                ZMQControl.Instance().heart_beat_timer_start();
                order_easy.start_sub_recv(comb_product.Text, comb_Instrument.Text);
                order_easy.start_data_init(data);
                saveConfig();
                this.Hide();
            });
        }
        private void comb_Instrument_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("hello");
            
        }

        private void comb_product_SelectedIndexChanged(object sender, EventArgs e)
        {
            string product = Convert.ToString(comb_product.SelectedItem);

            if (String.IsNullOrEmpty(product))
            {
                return;
            }

            comb_Instrument.Items.Clear();

            if (product == "ST")
            {

                foreach (KeyValuePair<string, Future> pair in common.stockDic)
                {
                    if (pair.Value.isActive)
                    {
                        this.comb_Instrument.Items.Add(pair.Value.product);
                        this.current_tick = pair.Value.tick;
                    }
                }
                return;
            }
            Future f = new Future();
            if (common.futureDic.TryGetValue(product, out f))
            {
                foreach (string instrument in f.instrument)
                {
                    this.comb_Instrument.Items.Add(instrument);
                    this.current_tick = f.tick;
                }
                //foreach (KeyValuePair<string, Symbol> pair in f.symbolDic)
                //{
                //    if (pair.Value.isActive)
                //    {
                //        this.comb_Instrument.Items.Add(pair.Value.code);
                //        this.current_tick = f.tick;
                //    }
                //}
            }
            comb_Instrument.SelectedIndex = 0;
        }

        private void login()
        {
            login_req data = new login_req();
            data.version = common.currentVersion;
            data.account = common.AccountDic[comb_account.Text].motherAccount;
            data.symbol = comb_Instrument.Text;
            data.symbol_tip = this.current_tick;

            MemoryStream sParam = new MemoryStream();
            sParam.Seek(0, SeekOrigin.Begin);
            Serializer.Serialize<login_req>(sParam, data);
            ZMQControl.Instance().Send2Router(sParam, MessageType.OE_LOGIN_REQ);
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            zmqTerm();
        }

        private void zmqTerm()
        {
            ZMQControl.Instance().zmqTerm();
            if (testThread != null && testThread.ThreadState == ThreadState.Running)
                testThread.Join();
        }

        private bool accountCheck(string subAccount)
        {
            if (common.AccountDic.ContainsKey(subAccount))
                return true;
            else
                return false;
        }

        private void saveConfig()
        {
            common.config.AppSettings.Settings.Remove("subaccount");
            common.config.AppSettings.Settings.Add("subaccount", comb_account.Text);
            common.config.AppSettings.Settings.Remove("category");
            common.config.AppSettings.Settings.Add("category", comb_product.Text);
            common.config.AppSettings.Settings.Remove("instrument");
            common.config.AppSettings.Settings.Add("instrument", comb_Instrument.Text);
            common.config.Save(ConfigurationSaveMode.Modified);
        }

    }
}
