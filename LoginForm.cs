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

namespace OrderEasy
{
    public partial class LoginForm : Form
    {
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
            comb_product.Items.Add("ST");

            this.comb_account.Items.Add(common.account);
            comb_account.SelectedIndex = comb_account.Items.IndexOf(common.account);
            comb_product.SelectedIndex = 0;
            comb_Instrument.SelectedIndex = 0;

            //comb_account.Enabled = false;
        }

        private void initCommon()
        {

            //common = Common.Instance();
            common.Init();
        }
        private void initZMQ()
        {

            ZMQControl.Instance().Init(ZeroMQ.SocketType.SUB, "");
            ZMQControl.Instance().InitDealer(SocketType.DEALER, common.routerAddr, common.control_id);
            foreach (KeyValuePair<string, bool> pair in common.addrDic)
            {
                if (pair.Value)
                {
                    ZMQControl.Instance().Connect(pair.Key);
                }
            }

        }

        private void runZMQ()
        {

            Thread testThread = new Thread((ThreadStart)delegate { ZMQControl.Instance().run(this,order_easy); });
            testThread.IsBackground = true;
            testThread.Start();
        }
        private void LoginForm_Load(object sender, EventArgs e)
        {
            initCommon();
            initZMQ();
            runZMQ();
        }

        private void button_login_Click(object sender, EventArgs e)
        {
            string control_id = "easy_" + comb_account.Text + "_" + comb_product.Text;
            common.set_control_id(control_id);
            initZMQ();
            login();

            login_resp d = new login_resp();
            d.success = 0;
            d.symbol = comb_Instrument.Text;
            on_login_resp(d);

            //pos_rtn d2 = new pos_rtn();
            //d2.dir = 48;
            //d2.symbol = comb_Instrument.Text;
            //d2.vol = 10;
            //order_easy.on_pos_rtn_handle(d2);
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
                //if (data.symbol != )
                //{
                //    MessageBox.Show("登陆失败：服务器返回symbol与前端symbol不一致");
                //    return;
                //}
                order_easy.Show();
                order_easy.start_sub_recv(comb_product.Text, comb_Instrument.Text);
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
                foreach (KeyValuePair<string, Symbol> pair in f.symbolDic)
                {
                    if (pair.Value.isActive)
                    {
                        this.comb_Instrument.Items.Add(pair.Value.code);
                        this.current_tick = f.tick;
                    }
                }
            }

 

        }

        private void login()
        {
            login_req data = new login_req();
            data.version = "beta";
            data.account = comb_account.Text;
            data.symbol = comb_Instrument.Text;
            data.symbol_tip = this.current_tick;

            MemoryStream sParam = new MemoryStream();
            sParam.Seek(0, SeekOrigin.Begin);
            Serializer.Serialize<login_req>(sParam, data);
            ZMQControl.Instance().Send2Router(sParam, MessageType.OE_LOGIN_REQ);
        }
    }
}
