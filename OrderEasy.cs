﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZeroMQ;
using System.Configuration;
using MDFTicker;
using System.IO;
using ProtoBuf;
using System.Threading;
using OrderEasy.common;
using RcXSpeedMdfPack;
using TraderControl;
using order_easy;

namespace OrderEasy
{
    public partial class OrderEasy : Form
    {
        private static ZmqContext context = ZmqContext.Create();
        int local_ref = 0;
        double tickPoint = 0.2;
        private double firstPrice = 0;
        private double endPrice = 0;
        private int firstPriceIndex = 0;
        private int endPriceIndex = 0;
        private int lastIndex = 0;
        private int maxVol = 0;
        private string account = "";
        private string frmPrice = "{0:0.0}";
        private string subTopic = "";
        private string preSymbol = "";
        private SourceGrid.Cells.Views.ColumnHeader headerView = new SourceGrid.Cells.Views.ColumnHeader();
        private SourceGrid.Cells.Views.Cell lastCell = new SourceGrid.Cells.Views.Cell();
        private SourceGrid.Cells.Views.Cell cellView = new SourceGrid.Cells.Views.Cell();
        private SourceGrid.Cells.Views.Cell LmtView = new SourceGrid.Cells.Views.Cell();
        private SourceGrid.Cells.Views.Cell RedView = new SourceGrid.Cells.Views.Cell();
        private SourceGrid.Cells.Views.Cell YellowView = new SourceGrid.Cells.Views.Cell();
        private SourceGrid.Cells.Views.Cell GreenView = new SourceGrid.Cells.Views.Cell();
        private SourceGrid.Cells.Views.Cell delView = new SourceGrid.Cells.Views.Cell();
        private SourceGrid.Cells.Views.Cell priceView = new SourceGrid.Cells.Views.Cell();
        private SourceGrid.Cells.Views.Cell PnLView = new SourceGrid.Cells.Views.Cell();
        private SourceGrid.Cells.Views.Cell GrayView = new SourceGrid.Cells.Views.Cell();

        private SourceGrid.Cells.Views.Cell buttonView = new SourceGrid.Cells.Views.Cell();
        private SourceGrid.Cells.Views.Cell VolView = new SourceGrid.Cells.Views.Cell();

        private MdfTicker curTicker = new MdfTicker();
        private Tick_TenEntrust curTenTicker = new Tick_TenEntrust();
        private bool isLock = false;
        private Common common;
        private OrderEasy_data.Strategy strategy;
        private class Position
        {
            //public int dir;
            //public int vol;
            public int long_pos;
            public int short_pos;
        }

        #region important parameter
        /// <summary>
        /// 未平仓 仓位
        /// </summary>
        private Position notClosePos = new Position();//
        /// <summary>
        /// 23列实时价格表（间隔一个tick）
        /// </summary>
        private Dictionary<string, int> priceMap = new Dictionary<string, int>();
        /// <summary>
        /// 价格对应的button类型
        private Dictionary<string, string> priceBuyButtonMap = new Dictionary<string, string>();
        private Dictionary<string, string> priceSelButtonMap = new Dictionary<string, string>();
        /// 下单的guid与Price的对照表（1对1）
        /// </summary>
        //public BiDictionaryOneToOne<string, double> guidPriceMap = new BiDictionaryOneToOne<string, double>();
        public BiDictionaryOneToOne<double, double> pairPriceMap = new BiDictionaryOneToOne<double, double>();
        public BiDictionaryOneToOne<int, double> refPriceMap = new BiDictionaryOneToOne<int, double>();

        //private Dictionary<int, double> seqNoPriceMap = new Dictionary<int, double>();
        //private Dictionary<double, int> priceSeqNoMap = new Dictionary<double, int>();
        /// <summary>
        /// 已获rep的挂单
        private List<string> repGuidList = new List<string>();
        private List<int> repRefList = new List<int>();
        /// 挂单列表：买
        /// </summary>
        private List<string> buyGuidList = new List<string>();
        //private List<int> buyRefList = new List<int>();
        private Dictionary<int, double> buyRefPriceMap = new Dictionary<int, double>();
        /// <summary>
        /// 挂单列表：卖
        /// </summary>
        private List<string> sellGuidList = new List<string>();
        //private List<int> sellRefList = new List<int>();
        private Dictionary<int, double> sellRefPriceMap = new Dictionary<int, double>();
        /// <summary>
        /// 当前挂单：MARKET的价格（买）
        /// </summary>
        private double marketBuyPrice;
        /// <summary>
        /// 当前挂单：MARKET的价格（卖）
        /// </summary>
        private double marketSellPrice;

        #endregion

        //private int seqNo = 0;
        public OrderEasy()
        {
            InitializeComponent();
        }

        private void InitView()
        {
            DevAge.Drawing.RectangleBorder border = new DevAge.Drawing.RectangleBorder(new DevAge.Drawing.BorderLine
                (Color.Black), new DevAge.Drawing.BorderLine(Color.Black));

            DevAge.Drawing.VisualElements.ColumnHeader flatHeader = new DevAge.Drawing.VisualElements.ColumnHeader();
            flatHeader.Border = border;
            flatHeader.BackColor = Color.DarkGray;
            flatHeader.BackgroundColorStyle = DevAge.Drawing.BackgroundColorStyle.Solid;

            headerView.Font = new Font(grid1.Font, FontStyle.Bold);
            headerView.Background = flatHeader;
            headerView.ForeColor = Color.Black;
            headerView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

            cellView.Background = new DevAge.Drawing.VisualElements.BackgroundLinearGradient(Color.LightGray, Color.LightGray, 45);
            cellView.Border = border;
            cellView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
            cellView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            lastCell.Background = new DevAge.Drawing.VisualElements.BackgroundLinearGradient(Color.Yellow, Color.Yellow, 45);
            lastCell.Border = border;
            lastCell.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
            lastCell.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lastCell.ForeColor = Color.Blue;

            PnLView.Background = new DevAge.Drawing.VisualElements.BackgroundLinearGradient(Color.Black, Color.Black, 45);
            PnLView.Border = border;
            PnLView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
            PnLView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            PnLView.ForeColor = Color.White;


            LmtView.Background = new DevAge.Drawing.VisualElements.BackgroundLinearGradient(Color.DeepSkyBlue, Color.DeepSkyBlue, 45);
            LmtView.Border = border;
            LmtView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
            LmtView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //LmtView.ForeColor = Color.White;


            RedView.Background = new DevAge.Drawing.VisualElements.BackgroundLinearGradient(Color.Red, Color.Red, 45);
            //qtyView.Border = border;
            RedView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
            RedView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));


            YellowView.Background = new DevAge.Drawing.VisualElements.BackgroundLinearGradient(Color.Yellow, Color.Yellow, 45);
            //qtyView.Border = border;
            YellowView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
            YellowView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));


            GreenView.Background = new DevAge.Drawing.VisualElements.BackgroundLinearGradient(Color.Green, Color.Green, 45);
            //qtyView.Border = border;
            GreenView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
            GreenView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            GrayView.Background = new DevAge.Drawing.VisualElements.BackgroundLinearGradient(Color.LightGray, Color.LightGray, 45);
            GrayView.Border = border;
            GrayView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
            GrayView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            GrayView.ForeColor = Color.Gray;

            //delView
            //qtyView.Background = new DevAge.Drawing.VisualElements.BackgroundLinearGradient(Color.Green, Color.Green, 45);
            //qtyView.Border = border;
            delView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
            delView.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            delView.ForeColor = Color.Red;

            priceView.Background = new DevAge.Drawing.VisualElements.BackgroundLinearGradient(Color.White, Color.White, 45);
            priceView.Border = border;
            priceView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
            priceView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            VolView.Background = new DevAge.Drawing.VisualElements.BackgroundLinearGradient(Color.LightGoldenrodYellow, Color.LightGoldenrodYellow, 45);
            VolView.Border = border;
            VolView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
            VolView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //VolView.ForeColor = Color.Purple;

            buttonView.Background = new DevAge.Drawing.VisualElements.BackgroundLinearGradient(Color.BlanchedAlmond, Color.BlanchedAlmond, 45);
            buttonView.Border = border;
            buttonView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            buttonView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

        }

        #region SetGrid


        private void SetGrid1()
        {

            grid1.BorderStyle = BorderStyle.FixedSingle;
            //grid1.scr
            //grid1.ColumnsCount = 3;
            //grid1.FixedRows = 1;
            //grid1.Rows.Insert(0);
            //Rectangle r = new Rectangle();

            this.grid1.RectangleToScreen(new Rectangle());

            this.panel2.Width = Width;// this.Width;
            this.grid1.Width = Width;// this.Width;
            //priceView
            //grid1.Columns[0].Visible = false;
            grid1.Redim(27, 10);
            grid1.Columns[0].Width = 31;
            grid1.Columns[1].Width = 31;

            grid1.Columns[2].Width = 70;
            grid1.Columns[3].Width = 110;
            grid1.Columns[4].Width = 70;

            grid1.Columns[5].Width = 31;
            grid1.Columns[6].Width = 31;

            grid1.Columns[7].Width = 80;
            grid1.Columns[8].Width = 70;
            grid1.Columns[9].Width = 80;
            //grid1.Columns[10].Width = 31;
            //grid1.Columns[11].Width = 31;

            /*this.panel2.Width = 617;// this.Width;
            this.grid1.Width = 618;// this.Width;*/
            SourceGrid.Cells.ColumnHeader header = new SourceGrid.Cells.ColumnHeader("BUY");
            header.AutomaticSortEnabled = false;


            grid1[0, 0] = new SourceGrid.Cells.ColumnHeader("锁定");
            grid1[0, 0].ColumnSpan = 2;
            grid1[0, 0].AddController(new AllClickEvent());
            grid1[0, 0].View = headerView;

            grid1[0, 2] = header;// new SourceGrid.Cells.ColumnHeader("BUY");
            grid1[0, 2].View = headerView;

            grid1[0, 3] = new SourceGrid.Cells.ColumnHeader("PRICE");
            grid1[0, 3].View = headerView;
            grid1[0, 4] = new SourceGrid.Cells.ColumnHeader("SELL");
            grid1[0, 4].View = headerView;
            //grid1.Location = new Point(80, 28);
            SourceGrid.Cells.Cell priceCell = new SourceGrid.Cells.Cell("", typeof(string));


            grid1[0, 5] = new SourceGrid.Cells.ColumnHeader("初始化");
            grid1[0, 5].ColumnSpan = 2;
            grid1[0, 5].AddController(new AllClickEvent());
            grid1[0, 5].View = headerView;

            grid1[0, 7] = new SourceGrid.Cells.ColumnHeader("卖十笔委托");
            grid1[0, 7].View = headerView;
            grid1[0, 8] = new SourceGrid.Cells.ColumnHeader("价位");
            grid1[0, 8].View = headerView;
            grid1[0, 9] = new SourceGrid.Cells.ColumnHeader("买十笔委托");
            grid1[0, 9].View = headerView;
            //grid1[0, 10] = new SourceGrid.Cells.ColumnHeader("增仓");
            //grid1[0, 10].View = headerView;
            //grid1[0, 11] = new SourceGrid.Cells.ColumnHeader("开平");
            //grid1[0, 11].View = headerView;
            /////////////////show data init
            for (int r = 1; r < 24; r++)
            {
                //grid1.Rows.Insert(r);
                grid1[r, 0] = null;
                grid1[r, 1] = null;

                grid1[r, 2] = new SourceGrid.Cells.Cell("", typeof(string));
                grid1[r, 2].View =  cellView;
                grid1[r, 2].AddController(new AllClickEvent());

                grid1[r, 3] = new SourceGrid.Cells.Cell("", typeof(string));
                grid1[r, 3].View = priceView;
                grid1[r, 3].AddController(new AllClickEvent());

                grid1[r, 4] = new SourceGrid.Cells.Cell("", typeof(string));
                grid1[r, 4].View = cellView;
                grid1[r, 4].AddController(new AllClickEvent());


                grid1[r, 5] = null;
                grid1[r, 6] = null;

                //grid1[r, 5] = new SourceGrid.Cells.Cell("", typeof(string)); ;
                //grid1[r, 6] = new SourceGrid.Cells.Cell("", typeof(string)); ;

                grid1[r, 7] = new SourceGrid.Cells.Cell("", typeof(string));
                grid1[r, 7].View = cellView;
                grid1[r, 8] = new SourceGrid.Cells.Cell("", typeof(string));
                grid1[r, 8].View = cellView;
                grid1[r, 9] = new SourceGrid.Cells.Cell("", typeof(string));
                grid1[r, 9].View = cellView;
                //grid1[r, 10] = new SourceGrid.Cells.Cell("", typeof(string));
                //grid1[r, 10].View = cellView;
                //grid1[r, 11] = new SourceGrid.Cells.Cell("", typeof(string));
                //grid1[r, 11].View = cellView;
                grid1.Rows.SetHeight(r, 17);
            }
            SetBotton();
            SetFocus();
            this.grid1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.grid1_OnMouseWheel);
        }
        private void SetBotton()
        {
            grid1[24, 0] = null;
            grid1[24, 1] = null;
            grid1[24, 2] = new SourceGrid.Cells.Cell("市场价", typeof(string));
            grid1[24, 2].View = cellView;

            //grid1[24, 2].AddController(new AllClickEvent());


            grid1[24, 3] = new SourceGrid.Cells.Cell("PnL", typeof(string));
            grid1[24, 3].View = PnLView;

            grid1[24, 4] = new SourceGrid.Cells.Cell("市场价", typeof(string));
            grid1[24, 4].View = cellView;
            //grid1[24, 4].AddController(new AllClickEvent());

            grid1[24, 5] = null;
            grid1[24, 6] = null;

            for (int i = 0; i < 7; i++)
            {
                grid1[25, i] = null;
            }
            grid1.Rows.SetHeight(25, 6);

            grid1[26, 0] = null;

            grid1[26, 1] = new SourceGrid.Cells.Cell("<", typeof(string));
            grid1[26, 1].View = buttonView;
            grid1[26, 1].AddController(new CollapseClickEvent());

            grid1[26, 2] = new SourceGrid.Cells.Cell("L", typeof(string)); ;
            grid1[26, 2].View = buttonView;

            //grid1[26, 2].AddController(new RevClickEvent());


            grid1[26, 3] = new SourceGrid.Cells.Cell();
            grid1[26, 3].View = buttonView;
            //grid1[26, 3].AddController(new AllClickEvent());

            grid1[26, 4] = new SourceGrid.Cells.Cell("S", typeof(string));
            grid1[26, 4].View = buttonView;
            //grid1[26, 4].AddController(new CloseClickEvent());

            grid1[26, 5] = new SourceGrid.Cells.Cell(" C ", typeof(string));
            grid1[26, 5].View = buttonView;
            grid1[26, 5].AddController(new CenterClickEvent());

            grid1[26, 6] = null;
            grid1.Rows.SetHeight(26, 20);
        }


        #endregion

        protected override void OnLoad(EventArgs e)
        {

            base.OnLoad(e);

            InitView();
            SetGrid1();
            notClosePos.long_pos = notClosePos.short_pos = 0;
            //this.radBtn1.Checked = true;
            //comboBox_strat.SelectedText = "<none>";
            this.Height = 600;

        }



        private double GetPriceByIndex(int r)
        {
            double price = 0;
            string p = grid1[r, 3].Value + "";
            if (p.Contains(")"))
            {
                p = p.Split(')')[1];
            }
            price = Convert.ToDouble(p);

            return price;
        }

        protected void SetFocus()
        {

            grid1.Selection.Focus(new SourceGrid.Position(24, 3), true);
        }

        private void Sim101_Load(object sender, EventArgs e)
        {
            initCommon();


            //initZMQ();
            //runZMQ();
            maxVol = Convert.ToInt32(ConfigurationManager.AppSettings["maxVol"]);
            tickPoint = Convert.ToDouble(ConfigurationManager.AppSettings["tickPoint"]);
            frmPrice = ConfigurationManager.AppSettings["FormatPrice"];

            //comb_Instrument.SelectedIndex = comb_Instrument.Items.IndexOf("");
            //comb_product.SelectedIndex = 0;
            //comb_Instrument.SelectedIndex = 0;

            this.comb_account.Items.Add(common.account);
            comb_account.SelectedIndex = comb_account.Items.IndexOf(common.account);
            comb_account.Enabled = false;


        }

        private void initCommon()
        {

            common = Common.Instance();
            //common.Init();

            //this.comb_Instrument.Items.Add(subTopic);

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

            //Thread testThread = new Thread((ThreadStart)delegate { ZMQControl.Instance().run(this); });
            //testThread.IsBackground = true;
            //testThread.Start();
        }
        public void updateGridHandle()
        {
            Invoke((MethodInvoker)delegate
            {
                //SetGrid();
                //updateData();

            });
        }
        private void setToolStripMenuItem_Click(object sender, EventArgs e)
        {

            SetForm set = new SetForm(this);
            set.ShowDialog();
        }

        private bool checkOrder(double price)
        {

            if (price <= 0)
                return false;
            return true;
        }
        private void wirteDebugMsg(string msg)
        {
            Program.log.Debug(msg);
        }
        private void wirteInfoMsg(string msg)
        {
            Program.log.Info(msg);
        }
        private void showErrorMsg(string msg)
        {
            Program.log.Error(msg);
            MessageBox.Show(msg);

        }


        private bool OrderHandle(int direction, double price, int price_type)
        {
            order_req data = new order_req();
            data.local_ref = ++this.local_ref;
            data.symbol = subTopic;
            data.price_type = price_type;
            data.price = price;
            data.vol = Convert.ToInt32(txtOrderQty.Value);
            data.dir = direction;

            MemoryStream sParam = new MemoryStream();
            sParam.Seek(0, SeekOrigin.Begin);
            Serializer.Serialize<order_req>(sParam, data);

            //if(!AddRefPriceMap(data.local_ref,price))
            //    return false;
            ZMQControl.Instance().Send2Router(sParam, MessageType.OE_ORDER_REQ);
            if (direction == TradeType.direction.RC_ORDER_BUY)
                //buyRefList.Add(data.local_ref);
                buyRefPriceMap.Add(data.local_ref, price);
            else
                //sellRefList.Add(data.local_ref);
                sellRefPriceMap.Add(data.local_ref, price);
            return true;
        }

        private void CacelHandle(int local_ref)
        {
            cancel_req data = new cancel_req();
            data.local_ref = local_ref;

            MemoryStream sParam = new MemoryStream();
            sParam.Seek(0, SeekOrigin.Begin);
            Serializer.Serialize<cancel_req>(sParam, data);
            ZMQControl.Instance().Send2Router(sParam, MessageType.OE_CANCEL_REQ);
        }
        private void CloseHandle()
        {
            List<string> guidList = new List<string>();

            foreach (string guid in buyGuidList)
            {
                guidList.Add(guid);
            }
            foreach (string guid in sellGuidList)
            {
                guidList.Add(guid);
            }
            //DealForeCloseHandle(guidList, OrderEasy_data.CancelType.cancelAll, OrderEasy_data.Direction.buy);

            //if (notClosePos.vol > 0)
            //{
            //    if (notClosePos.pos == OrderEasy_data.PosStatus.Long)
            //    {
            //        SetCancelButton(24, OrderEasy_data.Direction.sell, true);
            //        OrderHandle(OrderEasy_data.Direction.sell, curTicker.bid, OrderEasy_data.OrderType.market);
            //    }
            //    else
            //    {
            //        SetCancelButton(24, OrderEasy_data.Direction.buy, true);
            //        OrderHandle(OrderEasy_data.Direction.buy, curTicker.ask, OrderEasy_data.OrderType.market);
            //    }
            //}
        }
        //private void RevHandle()
        //{

        //    if (notClosePos.vol > 0)
        //    {
        //        if (notClosePos.pos == OrderEasy_data.PosStatus.Long)
        //        {
        //            //OrderHandle(OrderEasy_data.Direction.sell, curTicker.bid, OrderEasy_data.OrderType.rev);
        //        }
        //        else
        //        {
        //            //OrderHandle(OrderEasy_data.Direction.buy, curTicker.ask, OrderEasy_data.OrderType.rev);
        //        }
        //    }
        //}


        #region ServiceRtnHandle
        public void on_order_resp_handle(order_resp data)
        {
            Invoke((MethodInvoker)delegate
            {
                on_order_resp(data);
            });
        }
        public void on_order_resp_err_handle(order_resp_err data)
        {
            Invoke((MethodInvoker)delegate
            {
                showErrorMsg("错误返回信息：" + data.error_msg);
                delete_rtn tempData = new delete_rtn();
                tempData.local_ref = data.local_ref;
                on_delete_rtn_handle(tempData);
            });
        }
        public void on_cancel_resp_err_handle(cancel_resp_err data)
        {
            Invoke((MethodInvoker)delegate
            {
                showErrorMsg(data.error_msg + " local_ref:" + data.local_ref);
                delete_rtn tempData = new delete_rtn();
                tempData.local_ref = data.local_ref;
                on_delete_rtn_handle(tempData);
            });

        }
        public void on_pos_rtn_handle(pos_rtn data)
        {
            Invoke((MethodInvoker)delegate
            {
                if (data.symbol == subTopic)
                {
                    if (data.dir == TradeType.direction.RC_ORDER_BUY)
                        notClosePos.long_pos = data.vol;
                    else if (data.dir == TradeType.direction.RC_ORDER_SELL)
                        notClosePos.short_pos = data.vol;
                    SetFLAT();
                }
            });
        }

        public void on_message_rtn_handle(message_rtn data)
        {
            Invoke((MethodInvoker)delegate
            {
                Program.log.Info(data.msg);
                string msg = data.msg + "\n" + richTextBox1.Text;
                richTextBox1.Text = msg;
            });
        }
        public void on_delete_rtn_handle(delete_rtn data)
        {
            Invoke((MethodInvoker)delegate
            {

                int r = -1;
                double price = -1;

                foreach (KeyValuePair<int, double> pair in buyRefPriceMap)
                {
                    if (pair.Key == data.local_ref)
                    {
                        price = pair.Value;
                    }
                }
                foreach (KeyValuePair<int, double> pair in sellRefPriceMap)
                {
                    if (pair.Key == data.local_ref)
                    {
                        price = pair.Value;
                    }
                }

                if (!priceMap.TryGetValue(FormatPrice(price), out r))
                {
                    if (buyRefPriceMap.ContainsKey(data.local_ref))
                        buyRefPriceMap.Remove(data.local_ref);
                    if (sellRefPriceMap.ContainsKey(data.local_ref))
                        sellRefPriceMap.Remove(data.local_ref);
                    repRefList.Remove(data.local_ref);
                    return;
                }
                if (buyRefPriceMap.ContainsKey(data.local_ref))
                {
                    buyRefPriceMap.Remove(data.local_ref);
                    repRefList.Remove(data.local_ref);
                    SetCancelButton(r, TradeType.direction.RC_ORDER_BUY, false, "0");
                }
                else
                {
                    sellRefPriceMap.Remove(data.local_ref);
                    repRefList.Remove(data.local_ref);
                    SetCancelButton(r, TradeType.direction.RC_ORDER_SELL, false, "0");
                }
            });
           
        }

        private void on_order_resp(order_resp data)
        {
            if (!buyRefPriceMap.ContainsKey(data.local_ref) && !sellRefPriceMap.ContainsKey(data.local_ref))
            {
                showErrorMsg("there havn't fund price by rev seqno:" + data.local_ref);
                return;
            }
            //if (!refPriceMap.TryGetByFirst(data.local_ref, out price))
            //{
            //    showErrorMsg("there havn't fund price by rev seqno:" + data.local_ref);
            //    return;
            //}
            int r = -1;
            double price = -1;
            foreach (KeyValuePair<int, double> pair in buyRefPriceMap)
            {
                if (pair.Key == data.local_ref)
                {
                    price = pair.Value;
                    break;
                }
            }
            foreach (KeyValuePair<int, double> pair in sellRefPriceMap)
            {
                if (pair.Key == data.local_ref)
                {
                    price = pair.Value;
                    break;
                }
            }

            string sPrice = FormatPrice(price);
            if (!priceMap.TryGetValue(sPrice, out r))
                return;
            //if (buyRefList.Contains(data.local_ref))
            if(buyRefPriceMap.ContainsKey(data.local_ref))
            {
                grid1[r, 1].View = GreenView;
                repRefList.Add(data.local_ref);
            }
            //else if (sellRefList.Contains(data.local_ref))
            else if(sellRefPriceMap.ContainsKey(data.local_ref))
            {
                if (grid1[r, 5] == null)
                {
                    grid1[r, 5] = new SourceGrid.Cells.Cell("×", typeof(string));
                    grid1[r, 5].View = delView;
                    grid1[r, 5].AddController(new CancelClickEvent());
                }
                //repRefList.Add(data.local_ref);
            }
            else
            {
                MessageBox.Show("找不到对应的挂单：local_ref = " + data.local_ref);
            }
            grid1.Refresh();
        }

        public void TickerRevHandle(MdfTicker ticker)
        {
            Invoke((MethodInvoker)delegate
            {
                //this.textBox1.Text = Convert.ToString(ticker.last);
                UpdateListView(ticker);
            });
        }

        public void TenTickerRevHandle(Tick_TenEntrust ticker)
        {
            Invoke((MethodInvoker)delegate
            {
                //this.textBox1.Text = Convert.ToString(ticker.last);
                UpdateListView(ticker);
            });
        }


        #endregion

        #region Evnent handle's functioan
        private void ErrorHanle(OrderEasy_data data)
        {
            showErrorMsg("ErrorID:" + data.orderResp.ErrorID + "|msg:" + data.orderResp.ErrorMsg);
        }
        private void SetLeftVol(OrderEasy_data data)
        {

            //data.orderReq.qty

        }
        private void SetStopView(SourceGrid.Cells.Views.Cell view, int start, double price)
        {
            int stopR = 0;
            string p = GetPrice(price.ToString());
            bool isShow = priceMap.TryGetValue(p, out stopR);
            if (!isShow)
            {
                return;
            }
            for (int i = 0; i < 4; i++)
            {
                grid1[stopR, start++].View = view;
            }

            //grid1[stopR, 1].View = GreenView;
            //grid1[stopR, 2].View = GreenView;
            //grid1[stopR, 3].View = GreenView;
            //grid1[stopR, 4].View = GreenView;
        }

        private void SetStratCancelBtn(int r, int dir, bool visable)
        {
            if (r <= 0)
            {
                return;
            }
            if (dir == TradeType.direction.RC_ORDER_BUY)
            {
                if (visable)
                {
                    grid1[r, 0] = new SourceGrid.Cells.Cell("×", typeof(string));
                    grid1[r, 0].View = delView;
                    grid1[r, 0].AddController(new CancelClickEvent());
                   
                    

                    grid1[r, 1] = new SourceGrid.Cells.Cell("", typeof(string));
                    for (int i = 1; i < 5; i++)
                    {
                        grid1[r, i].View = YellowView;
                    }
                    grid1[r, 1].Value = this.txtOrderQty.Value;
                }
                else
                {

                    grid1[r, 0] = null;
                    grid1[r, 1] = null;
                    grid1[r, 2].View = cellView;
                    grid1[r, 3].View = priceView;
                    grid1[r, 4].View = cellView;
                }

            }
            else
            {
                if (visable)
                {
                    grid1[r, 6] = new SourceGrid.Cells.Cell("×", typeof(string));
                    grid1[r, 6].View = delView;
                    grid1[r, 6].AddController(new CancelClickEvent());

                    grid1[r, 5] = new SourceGrid.Cells.Cell("", typeof(string));
                    for (int i = 2; i < 6; i++)
                    {
                        //grid1[r, i] = new SourceGrid.Cells.Cell("", typeof(string));
                        grid1[r, i].View = YellowView;
                    }
                    grid1[r, 5].Value = this.txtOrderQty.Value;
                }
                else
                {
                    grid1[r, 2].View = cellView;
                    grid1[r, 3].View = priceView;
                    grid1[r, 4].View = cellView;
                    grid1[r, 5] = null;
                    grid1[r, 6] = null;
                }
            }
            SetCancelAllButton(dir);
            grid1.Refresh();
        }
        private void SetCancelButton(int r, int dir, bool visable, string orderVol)
        {
            double price = GetPriceByIndex(r);
            if (r <= 0)
            {
                return;
            }
            if (dir == TradeType.direction.RC_ORDER_BUY)
            {
                if (visable)
                {
                    grid1[r, 0] = new SourceGrid.Cells.Cell("×", typeof(string));
                    grid1[r, 0].View = delView;
                    grid1[r, 0].AddController(new CancelClickEvent());

                    grid1[r, 1] = new SourceGrid.Cells.Cell("", typeof(string));
                    grid1[r, 1].Value = orderVol;
                    grid1[r, 1].View = YellowView;
                    if (r < 24)
                    {
                        grid1[r, 2].View = LmtView;

                        grid1[r, 2].Value = "LMT " + getLimitVol(r, TradeType.direction.RC_ORDER_BUY);
                    }
                    if (!priceBuyButtonMap.ContainsKey(FormatPrice(price)))
                    {
                        priceBuyButtonMap.Add(FormatPrice(price), orderVol);
                    }
                }
                else
                {
                    grid1[r, 0] = null;
                    grid1[r, 1] = null;
                    if (r <= 23)
                    {
                        grid1[r, 2].Value = getLimitVol(r, TradeType.direction.RC_ORDER_BUY);
                        grid1[r, 2].View = GrayView;
                        if (priceBuyButtonMap.ContainsKey(FormatPrice(price)))
                            priceBuyButtonMap.Remove(FormatPrice(price));
                    }
                }

            }
            else
            {
                if (visable)
                {
                    grid1[r, 6] = new SourceGrid.Cells.Cell("×", typeof(string));
                    grid1[r, 6].View = delView;
                    grid1[r, 6].AddController(new CancelClickEvent());

                    grid1[r, 5] = new SourceGrid.Cells.Cell("", typeof(string));
                    grid1[r, 5].Value = orderVol;
                    grid1[r, 5].View = YellowView;
                    if (r < 24)
                    {
                        grid1[r, 4].Value = "LMT " + getLimitVol(r, TradeType.direction.RC_ORDER_SELL);
                        grid1[r, 4].View = LmtView;
                    }
                    if (!priceSelButtonMap.ContainsKey(FormatPrice(price)))
                    {
                        priceSelButtonMap.Add(FormatPrice(price), orderVol);
                    }
                }
                else
                {
                    grid1[r, 6] = null;
                    grid1[r, 5] = null;
                    if (r <= 23)
                    {
                        grid1[r, 4].Value = getLimitVol(r, TradeType.direction.RC_ORDER_SELL);
                        grid1[r, 4].View = GrayView;
                        if (priceSelButtonMap.ContainsKey(FormatPrice(price)))
                            priceSelButtonMap.Remove(FormatPrice(price));
                    }
                }
            }
            SetCancelAllButton(dir);
            grid1.Refresh();
        }
        private void SetCancelAllButton(int dir)
        {
            int c = dir == TradeType.direction.RC_ORDER_BUY ? 0 : 6;
            bool hasOneCancelButton = false;
            for (int i = 1; i < 25; i++)
            {
                if (grid1[i, c] != null)
                {
                    hasOneCancelButton = true;
                    break;
                }
            }
            if (hasOneCancelButton)
            {

                grid1[26, c] = new SourceGrid.Cells.Cell("×", typeof(string));
                grid1[26, c].View = delView;
                grid1[26, c].AddController(new CancelClickEvent());
            }
            else
            {
                grid1[26, c] = null;
            }
            grid1.Refresh();
        }
        private void SetFLAT()
        {
            if (notClosePos.long_pos > 0)
            {
                grid1[26, 2].Value = notClosePos.long_pos + "L"; ;
                grid1[26, 2].View = GreenView;
            }
            else
            {
                grid1[26, 2].Value = "L";
                grid1[26, 2].View = buttonView;
            }

            if (notClosePos.short_pos > 0)
            {
                grid1[26, 4].Value = notClosePos.short_pos + "S"; 
                grid1[26, 4].View = RedView;
            }
            else
            {
                grid1[26, 4].Value = "S";
                grid1[26, 4].View = buttonView;
            }
            grid1.Refresh();
        }

        private void RemoveRefPriceMap(int local_ref)
        {
            double maxPrice = 0;
            refPriceMap.TryGetByFirst(local_ref, out maxPrice);
            if (!refPriceMap.TryRemoveByFirst(local_ref))
            {
                showErrorMsg("guidPriceMap remove fail!,guid=" + local_ref);
            }
            pairPriceMap.TryRemoveByFirst(maxPrice);
        }
        private string getLimitVol(int r, int dir)
        {
            string vol = dir == TradeType.direction.RC_ORDER_BUY ? grid1[r, 2].Value + "" : grid1[r, 4].Value + "";
            if (vol.Contains("LMT "))
            {
                return vol.Split(' ')[1];
            }
            else
            {
                return vol;
            }
        }
        private void InitRtn(OrderEasy_data data)
        {
            ClearData();
            this.SetCancelButton2NULL(OrderEasy_data.Direction.buy);
            this.SetCancelButton2NULL(OrderEasy_data.Direction.sell);
        }
        #endregion

        #region updateListView
        public void UpdateListView(MdfTicker ticker)
        {

            this.curTicker = ticker;
            if (isLock)
            {
                SetHead();
            }
            else
            {

                SetPrice();
                SetVol();
            }
        }

        public void UpdateListView(Tick_TenEntrust ticker)
        {
            this.curTenTicker = ticker;
            EntrustData();
        }

        private void SetHead()
        {
            grid1[0, 2].Value = curTicker.bid;
            grid1[0, 3].Value = "(" + curTicker.vol + ")" + FormatPrice(curTicker.last);
            grid1[0, 4].Value = curTicker.ask;
        }

        /// <summary>
        /// *__________________________*
        /// |BUY |     PRICE   | SELL  |
        /// |----|-------------|-------|
        /// |    |     7564.8  |       |1    --firstPrice
        /// |    |     7564.6  |  68   |2    --bid5
        /// |    |     7564.4  |  20   |3    --bid4
        /// |    |     7564.2  |  85   |4    --bid3
        /// |    |     7564.0  |  6    |5    --bid2
        /// |    | (29)7563.8  |  50   |6    --last(bid)
        /// |22  |     7563.6  |       |7    --ask  
        /// |88  |     7563.4  |       |8    --ask2
        /// |59  |     7563.2  |       |9    --ask3 
        /// |63  |     7563.0  |       |10   --ask4  
        /// |99  |     7562.8  |       |11   --ask5   
        /// |    |     7562.6  |       |12 
        /// |    |     7562.4  |       |13
        /// |    |     7562.2  |       |14
        /// </summary>
        private void SetPrice()
        {
            MdfTicker ticker = this.curTicker;
            string ask5 = FormatPrice(ticker.ask5);//sell
            string bid5 = FormatPrice(ticker.bid5);//buy

            if (String.IsNullOrEmpty(grid1[1, 3].DisplayText))//初次赋值
            {
                ResetPrice();
            }
            //else if(!String.IsNullOrEmpty(grid1[1, 2].DisplayText))//SELL的最顶端
            else if (ticker.ask > 0 && ticker.ask > firstPrice)//SELL的最顶端
            {
                ResetPrice();
            }
            else if (ticker.bid > 0 && ticker.bid < endPrice)
            {//BUY最底端
                ResetPrice();
            }
            else
            {
                String value = Convert.ToString(grid1[lastIndex, 3].Value);
                ///double pre = Convert.ToDouble(value)-tick;
                grid1[lastIndex, 3].Value = GetPrice(value);
                grid1[lastIndex, 3].View = priceView;

                lastIndex = Convert.ToInt32((firstPrice - ticker.last) / tickPoint) + 1;
                grid1[lastIndex, 3].Value = "(" + ticker.vol + ")" + FormatPrice(ticker.last);
                grid1[lastIndex, 3].View = lastCell;
            }
        }
        private void EntrustData()
        {
            try
            {
                Tick_TenEntrust tenTick = curTenTicker;
                grid1[lastIndex, 3].View = lastCell;
                grid1[0, 9].Value = tenTick.best_ask_qty1 + tenTick.best_ask_qty2 +
                    tenTick.best_ask_qty3 + tenTick.best_ask_qty4 +
                    tenTick.best_ask_qty5 + tenTick.best_ask_qty6 +
                    tenTick.best_ask_qty7 + tenTick.best_ask_qty8 +
                    tenTick.best_ask_qty9 + tenTick.best_ask_qty10;
                grid1[1, 9].Value = tenTick.best_ask_qty10;
                grid1[2, 9].Value = tenTick.best_ask_qty9;
                grid1[3, 9].Value = tenTick.best_ask_qty8;
                grid1[4, 9].Value = tenTick.best_ask_qty7;
                grid1[5, 9].Value = tenTick.best_ask_qty6;
                grid1[6, 9].Value = tenTick.best_ask_qty5;
                grid1[7, 9].Value = tenTick.best_ask_qty4;
                grid1[8, 9].Value = tenTick.best_ask_qty3;
                grid1[9, 9].Value = tenTick.best_ask_qty2;
                grid1[10, 9].Value = tenTick.best_ask_qty1;

                grid1[10, 8].Value = tenTick.best_ask;
                grid1[11, 8].Value = tenTick.best_bid;

                grid1[11, 7].Value = tenTick.best_bid_qty1;
                grid1[12, 7].Value = tenTick.best_bid_qty2;
                grid1[13, 7].Value = tenTick.best_bid_qty3;
                grid1[14, 7].Value = tenTick.best_bid_qty4;
                grid1[15, 7].Value = tenTick.best_bid_qty5;
                grid1[16, 7].Value = tenTick.best_bid_qty6;
                grid1[17, 7].Value = tenTick.best_bid_qty7;
                grid1[18, 7].Value = tenTick.best_bid_qty8;
                grid1[19, 7].Value = tenTick.best_bid_qty9;
                grid1[20, 7].Value = tenTick.best_bid_qty10;

                grid1[0, 7].Value = tenTick.best_bid_qty1 + tenTick.best_bid_qty2 +
                   tenTick.best_bid_qty3 + tenTick.best_bid_qty4 +
                   tenTick.best_bid_qty5 + tenTick.best_bid_qty6 +
                   tenTick.best_bid_qty7 + tenTick.best_bid_qty8 +
                   tenTick.best_bid_qty9 + tenTick.best_bid_qty10;
            }
            catch (Exception ex)
            {
                Program.log.Error(ex);
            }

        }
        private string GetPrice(string _value)
        {
            if (_value.Contains(')'))
            {
                return _value.Split(')')[1];
            }
            return _value;

        }
        private string FormatPrice(double price)
        {
            return String.Format(frmPrice, price);
        }
        private void ResetPrice()
        {
            firstPrice = curTicker.last + 11 * tickPoint;
            endPrice = curTicker.last - 11 * tickPoint;
            if (lastIndex > 0)
            {
                grid1[lastIndex, 3].View = priceView;
            }

            for (int i = 1; i < 24; i++)
            {
                double dcurPrice = firstPrice - (i - 1) * tickPoint;
                string curPrice = FormatPrice(dcurPrice);
                grid1[i, 3].Value = curPrice;
                //if (priceMap.Count == 0 && !priceMap.ContainsKey(curPrice))
                if (!priceMap.ContainsKey(curPrice))
                {
                    priceMap.Add(curPrice, i);
                }
                else
                {
                    priceMap[curPrice] = i;
                }
                buttonMoveCheck(i, curPrice);

                ////////////////////last////////////////////////
                if (Convert.ToDouble(curTicker.last) == Convert.ToDouble(grid1[i, 3].Value))
                {
                    grid1[i, 3].Value = "(" + curTicker.vol + ")" + grid1[i, 3].Value;
                    lastIndex = i;
                    grid1[lastIndex, 3].View = lastCell;
                }
            }
            for (int i = 1; i < 24; i++)
            {
                if (grid1[i, 2].View != LmtView && grid1[i, 2].View != VolView)
                    grid1[i, 2].Value = "";
                if (grid1[i, 4].View != LmtView && grid1[i, 4].View != VolView)
                    grid1[i, 4].Value = "";
                if (Convert.ToString(grid1[i, 2].Value).Contains("LMT"))
                    grid1[i, 2].Value = "LMT ";
                if (Convert.ToString(grid1[i, 4].Value).Contains("LMT"))
                    grid1[i, 4].Value = "LMT ";
            }
            SetVol();
            grid1.Refresh();
        }
        private void buttonMoveCheck(int rows, string price)
        {
            if (priceBuyButtonMap.Count == 0)
            {
                grid1[0, 2].View = headerView;
                grid1[24, 2].View = cellView;
            }
            if (priceSelButtonMap.Count == 0)
            {
                grid1[0, 4].View = headerView;
                grid1[24, 4].View = cellView;
            }
                
            foreach(KeyValuePair<string,string> pair in priceBuyButtonMap)
            {
                double int_price = Convert.ToDouble( pair.Key);
                if (int_price > firstPrice)
                {
                    grid1[0, 2].View = LmtView;
                    break;
                }
                    grid1[0, 2].View = headerView;
            }
            foreach (KeyValuePair<string, string> pair in priceBuyButtonMap)
            {
                double int_price = Convert.ToDouble(pair.Key);
                if (int_price < endPrice)
                {
                    grid1[24, 2].View = LmtView;
                    break;
                }
                    grid1[24, 2].View = cellView;
            }
            foreach (KeyValuePair<string, string> pair in priceSelButtonMap)
            {
                double int_price = Convert.ToDouble(pair.Key);
                if (int_price > firstPrice)
                {
                    grid1[0, 4].View = LmtView;
                    break;
                }
                grid1[0, 4].View = headerView;
            }
            foreach (KeyValuePair<string, string> pair in priceSelButtonMap)
            {
                double int_price = Convert.ToDouble(pair.Key);
                if (int_price < endPrice)
                {
                    grid1[24, 4].View = LmtView;
                    break;
                }
                grid1[24, 4].View = cellView;
            }
            int local_ref = -1;
            bool ret = false; //= refPriceMap.TryGetBySecond(Convert.ToDouble(price), out local_ref);//guidPriceMap.TryGetBySecond(Convert.ToDouble(price), out guid);

            foreach (KeyValuePair<int, double> pair in buyRefPriceMap)
            {
                if (pair.Value.ToString() == price)
                {
                    local_ref = pair.Key;
                    ret = true;
                }
            }
            foreach (KeyValuePair<int, double> pair in sellRefPriceMap)
            {
                if (pair.Value.ToString() == price)
                {
                    local_ref = pair.Key;
                    ret = true;
                }
            }
            bool retBuy = repRefList.Contains(local_ref); 

              if (priceBuyButtonMap.ContainsKey(price))
                {
                    //setbutton
                    grid1[rows, 0] = new SourceGrid.Cells.Cell("×", typeof(string));
                    grid1[rows, 0].View = delView;
                    grid1[rows, 0].AddController(new CancelClickEvent());

                    grid1[rows, 1] = new SourceGrid.Cells.Cell("", typeof(string));
                    grid1[rows, 1].Value = priceBuyButtonMap[price];
                    if (ret&&retBuy)
                    {
                        grid1[rows, 1].View = GreenView;
                    }
                    else
                    {
                        grid1[rows, 1].View = YellowView;
                    }
                    
                    if (rows < 24)
                    {
                        grid1[rows, 2].View = LmtView;

                        grid1[rows, 2].Value = "LMT " + getLimitVol(rows, TradeType.direction.RC_ORDER_BUY);
                    }
                }
                else
                {
                    //delbutton
                    grid1[rows, 0] = null;
                    grid1[rows, 1] = null;
                    if (rows <= 23)
                    {
                        grid1[rows, 2].Value = getLimitVol(rows, TradeType.direction.RC_ORDER_BUY);
                        grid1[rows, 2].View = cellView;
                    }
                }
                if (priceSelButtonMap.ContainsKey(price))
                {
                    grid1[rows, 6] = new SourceGrid.Cells.Cell("×", typeof(string));
                    grid1[rows, 6].View = delView;
                    grid1[rows, 6].AddController(new CancelClickEvent());

                    grid1[rows, 5] = new SourceGrid.Cells.Cell("", typeof(string));
                    grid1[rows, 5].Value = priceSelButtonMap[price];
                    grid1[rows, 5].View = YellowView;

                    
                    if (rows < 24)
                    {
                        grid1[rows, 4].Value = "LMT " + getLimitVol(rows, TradeType.direction.RC_ORDER_SELL);
                        grid1[rows, 4].View = LmtView;
                    }
                }
                else
                {
                    grid1[rows, 6] = null;
                    grid1[rows, 5] = null;
                    if (rows <= 23)
                    {
                        grid1[rows, 4].Value = getLimitVol(rows, TradeType.direction.RC_ORDER_SELL);
                        grid1[rows, 4].View = cellView;
                    }
                }

        }
        private int isIn5SpeedPrice(double p)
        {
            if (p == curTicker.ask || p == curTicker.ask2 || p == curTicker.ask3 || p == curTicker.ask4 || p == curTicker.ask5)
            {

                return 1;
            }
            else if (p == curTicker.bid || p == curTicker.bid2 || p == curTicker.bid3 || p == curTicker.bid4
                || p == curTicker.bid5)
            {
                return -1;
            }
            return 0;
        }
        private void ResetPriceByMouseWheel()
        {
            //MdfTicker ticker = curTicker;
            //firstPrice = curTicker.last + 11 * tickPoint;
            //endPrice = curTicker.last - 11 * tickPoint;
            //grid1[lastIndex, 3].View = priceView;
            for (int i = 1; i < 24; i++)
            {
                double p = firstPrice - (i - 1) * tickPoint;
                string curPrice = FormatPrice(p);
                grid1[i, 3].Value = curPrice;
                //if (priceMap.Count == 0 && !priceMap.ContainsKey(curPrice))
                if (!priceMap.ContainsKey(curPrice))
                {
                    priceMap.Add(curPrice, i);
                }
                else
                {
                    priceMap[curPrice] = i;
                }

                buttonMoveCheck(i, curPrice);

                ////////////////////last////////////////////////
                if (curTicker.last == Convert.ToDouble(grid1[i, 3].Value))
                {
                    String value = Convert.ToString(grid1[lastIndex, 3].Value);
                    ///double pre = Convert.ToDouble(value)-tick;
                    value = value.Contains(")") ? value.Split(')')[1] : value;
                    grid1[lastIndex, 3].Value = value;
                    grid1[lastIndex, 3].View = priceView;
                     
                    grid1[i, 3].Value = "(" + curTicker.vol + ")" + grid1[i, 3].Value;
                    lastIndex = i;
                    grid1[lastIndex, 3].View = lastCell;
                }
            }


            cleanVol();
            SetVol();
        }
        private void cleanVol()
        {
            for (int i = 1; i < 24; i++)
            {
                if (Convert.ToString(grid1[i, 2].Value).Contains("LMT"))
                {
                    grid1[i, 2].Value = "LMT " + getLimitVol(i, TradeType.direction.RC_ORDER_BUY); 
                    //grid1[i, 2].View.ForeColor = Color.Gray;
                }
                else if(isLock)
                {
                    grid1[i, 2].Value = "";
                }
                if(grid1[i, 2].View != LmtView)
                    grid1[i, 2].View = GrayView;

                if (Convert.ToString(grid1[i, 4].Value).Contains("LMT"))
                {
                    grid1[i, 4].Value = "LMT " + getLimitVol(i, TradeType.direction.RC_ORDER_SELL);
                    //grid1[i, 4].View.ForeColor = Color.Gray;
                }
                else if (isLock)
                {
                    grid1[i, 4].Value = "";
                }
                if (grid1[i, 4].View != LmtView)
                    grid1[i, 4].View = GrayView;
            }
            //grid1.Refresh();
        }
        private void SetVol()
        {
            MdfTicker ticker = curTicker;
            cleanVol();
            //////////ask/////////
            SetOneVol(ticker.ask, ticker.askvol, true);
            if (ticker.ask2 > 0)
                SetOneVol(ticker.ask2, ticker.askVol2, true);
            if (ticker.ask3 > 0)
                SetOneVol(ticker.ask3, ticker.askVol3, true);
            if (ticker.ask4 > 0)
                SetOneVol(ticker.ask4, ticker.askVol4, true);
            if (ticker.ask5 > 0)
                SetOneVol(ticker.ask5, ticker.askVol5, true);
            //////////bid/////////
            SetOneVol(ticker.bid, ticker.bidvol, false);
            if (ticker.bid2 > 0)
                SetOneVol(ticker.bid2, ticker.bidVol2, false);
            if (ticker.bid3 > 0)
                SetOneVol(ticker.bid3, ticker.bidVol3, false);
            if (ticker.bid4 > 0)
                SetOneVol(ticker.bid4, ticker.bidVol4, false);
            if (ticker.bid5 > 0)
                SetOneVol(ticker.bid5, ticker.bidVol5, false);
            grid1[0, 2].Value = ticker.bidvol + ticker.bidVol2 + ticker.bidVol3 + ticker.bidVol4 + ticker.bidVol5;
            grid1[0, 4].Value = ticker.askvol + ticker.askVol2 + ticker.askVol3 + ticker.askVol4 + ticker.askVol5;
        }
        private void SetOneVol(double price, int vol, bool isAsk)
        {
            if (price <= 0) return;
            int r = 0;
            if (price > this.firstPrice || price < this.endPrice)
                return;
            if (!priceMap.TryGetValue(FormatPrice(price), out r))
                return;
            int index = isAsk ? 4 : 2;
            string value = Convert.ToString(grid1[priceMap[FormatPrice(price)], index].Value);
            if (value.Contains("LMT"))
            {
                grid1[priceMap[FormatPrice(price)], index].Value = "LMT " + vol;
                grid1[priceMap[FormatPrice(price)], index].View = LmtView;
            }
            else
            {
                grid1[priceMap[FormatPrice(price)], index].Value = vol;
                grid1[priceMap[FormatPrice(price)], index].View = VolView;
            }

            //买卖只能出现一边的数据
            string oppositeValue = Convert.ToString(grid1[priceMap[FormatPrice(price)], index == 4 ? 2 : 4].Value);
            if (oppositeValue.Contains("LMT "))
            {
                //grid1[priceMap[FormatPrice(price)], index == 4 ? 2 : 4].View = LmtView;
                grid1[priceMap[FormatPrice(price)], index == 4 ? 2 : 4].Value = "LMT ";
            }
            else
                grid1[priceMap[FormatPrice(price)], index == 4 ? 2 : 4].Value = "";

            grid1.Refresh();
        }
        #endregion

        #region ClickEvent

        /// <summary>
        /// _____  _____
        /// | < |  | > |
        /// -----  -----
        /// </summary>
        public class CollapseClickEvent : SourceGrid.Cells.Controllers.ControllerBase
        {

            public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
            {
                base.OnClick(sender, e);

                //自动平仓策略，暂时没用到
                SourceGrid.Grid grid = (SourceGrid.Grid)sender.Grid;
                OrderEasy f = (OrderEasy)grid.Parent.Parent;
                f.groupBox2.Visible = !f.groupBox2.Visible;
                if (f.groupBox2.Visible)
                {
                    f.grid1[26, 1].Value = ">";
                    f.Height = 711;
                }
                else
                {
                    f.grid1[26, 1].Value = "<";
                    f.Height = 600;
                }
                f.SetFocus();

            }
        }
        public class RevClickEvent : SourceGrid.Cells.Controllers.ControllerBase
        {
            public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
            {
                base.OnClick(sender, e);

                SourceGrid.Grid grid = (SourceGrid.Grid)sender.Grid;
                OrderEasy f = (OrderEasy)grid.Parent.Parent;
                f.SetFocus();
                //f.RevHandle();

            }
        }
        private void InitVolHandle()
        {
            showErrorMsg("未实现的功能【初始化】");
            //string showMsg = "清空所有的交易信息 并且初始化后台? \r\n\n ";
            //DialogResult result = MessageBox.Show(showMsg, "初始化 确认", MessageBoxButtons.YesNo);
            //if (result == DialogResult.No)
            //{
            //    return;
            //}
            //wirteDebugMsg("send Init req");
            //ClearData();
            //SetCancelButton2NULL(OrderEasy_data.Direction.buy);
            //SetCancelButton2NULL(OrderEasy_data.Direction.sell);
            //ResetPrice();
        }
        private void BuyClinkHandle(int r)
        {

            string showMsg = " Buy limit "
                + txtOrderQty.Value + " contract(s) " + subTopic + "@ " + grid1[r, 3].Value;
            Double price = GetPriceByIndex(r);
            DialogResult result = MessageBox.Show(showMsg, "确定订单", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                return;
            }
            if (priceMap.ContainsKey(FormatPrice(price)))
                r = priceMap[FormatPrice(price)];
            if (OrderHandle(TradeType.direction.RC_ORDER_BUY, price, TradeType.price_type.RC_ORDER_LIMIT))
                SetCancelButton(r, TradeType.direction.RC_ORDER_BUY, true, txtOrderQty.Value.ToString());
        }
        private void SellClinkHandle(int r)
        {

            string showMsg = " Sell limit "
                + txtOrderQty.Value + " contract(s) " + subTopic + "@ " + grid1[r, 3].Value + getLimitVol(r, TradeType.direction.RC_ORDER_SELL);
            Double price = GetPriceByIndex(r);
            DialogResult result = MessageBox.Show(showMsg, "确定订单", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                return;
            }

            if (priceMap.ContainsKey(FormatPrice(price)))
                r = priceMap[FormatPrice(price)];
            if (OrderHandle(TradeType.direction.RC_ORDER_SELL, price, TradeType.price_type.RC_ORDER_LIMIT))
                SetCancelButton(r, TradeType.direction.RC_ORDER_SELL, true, txtOrderQty.Value.ToString());
        }

        private void LockButtonHandle()
        {
            isLock = !isLock;
            if (!isLock)
            {
                grid1[0, 1].Value = "锁定";
                grid1[0, 2].Value = "BUY";
                grid1[0, 3].Value = "PRICE";
                grid1[0, 3].View = this.headerView;
                grid1[0, 4].Value = "SELL";
                this.ResetPrice();

            }
            else
            {
                grid1[0, 1].Value = "释放";
            }
        }
        public class AllClickEvent : SourceGrid.Cells.Controllers.ControllerBase
        {

            public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
            {
                base.OnClick(sender, e);
                SourceGrid.Grid grid = (SourceGrid.Grid)sender.Grid;
                OrderEasy f = (OrderEasy)grid.Parent.Parent;
                f.SetFocus();

                if (!f.checkOrder(f.curTicker.ask))
                    return;
                int r = sender.Position.Row;
                int c = sender.Position.Column;
                if (r == 0 && c == 0)
                {
                    f.LockButtonHandle();

                }
                else if (r == 0 && c == 5)
                {
                    f.InitVolHandle();
                }
                else if (r <= 23 && r > 0)
                {
                    if (c == 2)
                        f.BuyClinkHandle(r);
                    else if (c == 4)
                        f.SellClinkHandle(r);
                }
                else if (r == 24)//MARKET
                {
                    //if (c == 2)
                        //f.MarketBuyClinkHandle(r);
                    //else if (c == 4)
                        //f.MarketSellClinkHandle(r);
                }

                //Thread.Sleep(10000);
                //order_resp data2 = new order_resp();
                //data2.local_ref = 1;
                //f.on_order_resp(data2);
            }
        }
        private void SetCancelButton2NULL(OrderEasy_data.Direction dir)
        {
            for (int i = 1; i < 24; i++)
            {
                //grid1[i, c] = null;
                //SetCancelButton(i, dir, false, "0");
            }
            int c = dir == OrderEasy_data.Direction.buy ? 0 : 6;
            grid1[24, c] = null;
            grid1[26, c] = null;
            grid1.Refresh();
            return;
        }
        public class CancelClickEvent : SourceGrid.Cells.Controllers.ControllerBase
        {

            public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
            {
                base.OnClick(sender, e);

                SourceGrid.Grid grid = (SourceGrid.Grid)sender.Grid;
                OrderEasy f = (OrderEasy)grid.Parent.Parent;
                f.SetFocus();

                int r = sender.Position.Row;
                int c = sender.Position.Column;
                int dir = c == 0 ? TradeType.direction.RC_ORDER_BUY : TradeType.direction.RC_ORDER_SELL;
                double price = c == 0 ? f.marketBuyPrice : f.marketSellPrice;
                if (r == 24)
                {//cancel MARKET

                }
                else if (r == 26)
                {//cancel ALL


                }
                else
                {//cancel ONE
                    int local_ref = -1;
                    foreach (KeyValuePair<int, double> pair in f.buyRefPriceMap)
                    {
                        if (pair.Value == Convert.ToDouble(f.GetPriceByIndex(r)) && dir == TradeType.direction.RC_ORDER_BUY)
                        {
                            local_ref = pair.Key;
                            f.CacelHandle(local_ref);
                            return;
                        }
                    }
                    foreach (KeyValuePair<int, double> pair in f.sellRefPriceMap)
                    {
                        if (pair.Value == Convert.ToDouble(f.GetPriceByIndex(r)) && dir == TradeType.direction.RC_ORDER_SELL)
                        {
                            local_ref = pair.Key;
                            f.CacelHandle(local_ref);
                            return;
                        }
                    }

                    if(local_ref == -1)
                        f.showErrorMsg("未找到对应local_ref");
                }


            }
        }

        public class CloseClickEvent : SourceGrid.Cells.Controllers.ControllerBase
        {

            public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
            {
                base.OnClick(sender, e);
                SourceGrid.Grid grid = (SourceGrid.Grid)sender.Grid;
                OrderEasy f = (OrderEasy)grid.Parent.Parent;
                f.showErrorMsg("未实现的功能【强平】");
                //SourceGrid.Grid grid = (SourceGrid.Grid)sender.Grid;
                //OrderEasy f = (OrderEasy)grid.Parent.Parent;
                //f.SetFocus();
                //f.CloseHandle();
            }
        }
        public class CenterClickEvent : SourceGrid.Cells.Controllers.ControllerBase
        {

            public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
            {
                base.OnClick(sender, e);

                SourceGrid.Grid grid = (SourceGrid.Grid)sender.Grid;
                OrderEasy f = (OrderEasy)grid.Parent.Parent;
                f.SetFocus();
                f.ResetPrice();
            }
        }

        public class CellMouseUpEvent : SourceGrid.Cells.Controllers.ControllerBase
        {
            public override void OnMouseUp(SourceGrid.CellContext sender, MouseEventArgs e)
            {
                base.OnMouseUp(sender, e);

            }
        }

        #endregion


        private void grid1_MouseHover(object sender, EventArgs e)
        {
            //this.OnMouseWheel = 
        }
        private void grid1_OnMouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int maxWheel = 400;
            if (!isLock || e.Delta == 0)
            {
                return;
            }
            int index = e.Delta / 120;
            this.firstPrice += index * tickPoint;
            this.endPrice = firstPrice - 22 * tickPoint;
            if ((this.firstPrice - curTicker.last) / tickPoint > maxWheel)
                return;
            if ((curTicker.last - this.endPrice) / tickPoint > maxWheel)
                return;
            this.ResetPriceByMouseWheel();
            grid1.Refresh();


        }

        private void OrderEasy_FormClosed(object sender, FormClosedEventArgs e)
        {

            System.Environment.Exit(0);
        }

        private void ClearData()
        {
            notClosePos.long_pos = 0;
            notClosePos.short_pos = 0;
            //priceMap.Clear();
            //guidPriceMap.Clear();
            refPriceMap.Clear();
            //buyRefList.Clear();
            //sellRefList.Clear();
            buyRefPriceMap.Clear();
            sellRefPriceMap.Clear();
            pairPriceMap.Clear();
            //buyGuidList.Clear();
            //sellGuidList.Clear();
            marketBuyPrice = 0;
            marketSellPrice = 0;
            priceBuyButtonMap.Clear();
            priceSelButtonMap.Clear();
            SetFLAT();
        }





        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtOrderQty_ValueChanged(object sender, EventArgs e)
        {

            if (txtOrderQty.Value == 0)
            {
                txtOrderQty.Value = 1;
            }
        }

        public void start_sub_recv(string _product, string _symbol)
        {
            Invoke((MethodInvoker)delegate
            {
                string product = _product;
                string symbol = _symbol;
                if (String.IsNullOrEmpty(symbol))
                {
                    ZMQControl.Instance().UnsubScribe("", "");
                    return;
                }
                ZMQControl.Instance().UnsubScribe(preSymbol, "TE_" + preSymbol);
                preSymbol = symbol;
                if (product == "ST")
                {
                    Future f = new Future();
                    if (common.stockDic.TryGetValue(symbol, out f))
                    {
                        this.tickPoint = f.tick;
                        this.frmPrice = GetFrmPrice(f.point);
                    }
                    else
                    {
                        MessageBox.Show("无此合约");
                        return;
                    }

                }
                else
                {
                    Future f = common.futureDic[product];
                    this.tickPoint = f.tick;
                    this.frmPrice = GetFrmPrice(f.point);

                }
                ResetPrice();
                this.subTopic = symbol;
                ZMQControl.Instance().SubScript(symbol, "TE_" + symbol);
                
            });

        }
        public void start_data_init(login_resp data)
        {
            this.local_ref = data.last_local_ref;

            if(data.long_pos > 0)
            {
                notClosePos.long_pos = data.long_pos;
                //notClosePos.dir = TradeType.direction.RC_ORDER_BUY;
                //notClosePos.vol = data.long_pos;
            }
            if(data.short_pos > 0)
            {
                notClosePos.short_pos = data.short_pos;
                //this.notClosePos.dir = TradeType.direction.RC_ORDER_SELL;
                //notClosePos.vol = data.short_pos;
            }
            SetFLAT();
            int Count = data.order_list.Count;
            foreach (order_record rec in data.order_list )
            {
                //AddRefPriceMap(rec.local_ref, rec.price);
                if (rec.symbol != subTopic)
                {
                    showErrorMsg("后台数据错误：返回的挂单SYMBOL与客户端所选不一至");
                }
                if (rec.dir == TradeType.direction.RC_ORDER_BUY)
                {
                    repRefList.Add(rec.local_ref);
                    //buyRefList.Add(rec.local_ref);
                    buyRefPriceMap.Add(rec.local_ref, rec.price);
                    if (!priceBuyButtonMap.ContainsKey(FormatPrice(rec.price)))
                    {
                        priceBuyButtonMap.Add(FormatPrice(rec.price), rec.vol.ToString());
                    }
                }
                else
                {
                    //sellRefList.Add(rec.local_ref);
                    sellRefPriceMap.Add(rec.local_ref, rec.price);
                    if (!priceSelButtonMap.ContainsKey(FormatPrice(rec.price)))
                    {
                        priceSelButtonMap.Add(FormatPrice(rec.price), rec.vol.ToString());
                    }
                }
            }
            
        }
        private void comb_Instrument_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// //"{0:0.0}"
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private string GetFrmPrice(int p)
        {
            string ret = "{0:0";
            for (int i = 0; i < p; i++)
            {
                if (i == 0)
                {
                    ret += ".";
                }
                ret += "0";

            }
            ret += "}";
            return ret;

        }

        private void comb_product_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private bool AddRefPriceMap(int local_ref, double price)
        {
            if (!refPriceMap.TryAdd(local_ref, price))
            {
                RemoveRefPriceMap(local_ref);
                showErrorMsg("AddOrderRefMap fail seqNo=" + local_ref + "price=" + price);
                return false;
            }
            return true;
        }



    }
}
