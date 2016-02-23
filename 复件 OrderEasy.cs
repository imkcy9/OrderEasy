using System;
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

namespace OrderEasy
{
    public partial class OrderEasy : Form
    {
        private static ZmqContext context = ZmqContext.Create();
        double tickPoint = 0.2;
        private double firstPrice = 0;
        private double endPrice = 0;
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

        private SourceGrid.Cells.Views.Cell buttonView = new SourceGrid.Cells.Views.Cell();
        private SourceGrid.Cells.Views.Cell VolView = new SourceGrid.Cells.Views.Cell();

        private MdfTicker curTicker = new MdfTicker();
        private Tick_TenEntrust curTenTicker = new Tick_TenEntrust();
        private bool isLock = false;
        private Common common;
        private OrderEasy_data.Strategy strategy;
        private class Position
        {
            public OrderEasy_data.PosStatus pos;
            public int vol;
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
        /// 下单的guid与Price的对照表（1对1）
        /// </summary>
        public BiDictionaryOneToOne<string, double> guidPriceMap = new BiDictionaryOneToOne<string, double>();
        public BiDictionaryOneToOne<double, double> pairPriceMap = new BiDictionaryOneToOne<double, double>();
        //private Dictionary<int, double> seqNoPriceMap = new Dictionary<int, double>();
        //private Dictionary<double, int> priceSeqNoMap = new Dictionary<double, int>();
        /// <summary>
        /// 挂单列表：买
        /// </summary>
        private List<string> buyGuidList = new List<string>();
        /// <summary>
        /// 挂单列表：卖
        /// </summary>
        private List<string> sellGuidList = new List<string>();
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
                grid1[r, 2].View = cellView;
                grid1[r, 2].AddController(new AllClickEvent());

                grid1[r, 3] = new SourceGrid.Cells.Cell("", typeof(string));
                grid1[r, 3].View = priceView;
                grid1[r, 3].AddController(new AllClickEvent());

                grid1[r, 4] = new SourceGrid.Cells.Cell("", typeof(string));
                grid1[r, 4].View = cellView;
                grid1[r, 4].AddController(new AllClickEvent());


                grid1[r, 5] = null;
                grid1[r, 6] = null;

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
            grid1[24, 2] = new SourceGrid.Cells.Cell("MARKET", typeof(string));
            grid1[24, 2].View = cellView;

            grid1[24, 02].AddController(new AllClickEvent());


            grid1[24, 3] = new SourceGrid.Cells.Cell("PnL", typeof(string));
            grid1[24, 3].View = PnLView;

            grid1[24, 4] = new SourceGrid.Cells.Cell("MARKET", typeof(string));
            grid1[24, 4].View = cellView;
            grid1[24, 4].AddController(new AllClickEvent());

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

            grid1[26, 2] = new SourceGrid.Cells.Cell("REV", typeof(string));
            grid1[26, 2].View = buttonView;

            grid1[26, 2].AddController(new RevClickEvent());


            grid1[26, 3] = new SourceGrid.Cells.Cell("FLAT", typeof(string));
            grid1[26, 3].View = buttonView;
            grid1[26, 3].AddController(new AllClickEvent());

            grid1[26, 4] = new SourceGrid.Cells.Cell("CLOSE", typeof(string));
            grid1[26, 4].View = buttonView;
            grid1[26, 4].AddController(new CloseClickEvent());

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
            notClosePos.vol = 0;
            this.radBtn1.Checked = true;
            comboBox_strat.SelectedText = "<none>";
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

        private List<string> GetGuidListByPrice(double price)
        {

            List<string> guidList = new List<string>();
         
            string guid = "";
            bool ret = guidPriceMap.TryGetBySecond(price, out guid);
            if (!ret)
            {

                double maxPrice = 0;
                ret = pairPriceMap.TryGetBySecond(price, out maxPrice);
                if (ret)
                    return GetGuidListByPrice(maxPrice);//
                showErrorMsg("can't get guid by price:" + price);
                return guidList;
            }
            guidList.Add(guid);
            return guidList;

        }

        protected void SetFocus()
        {

            grid1.Selection.Focus(new SourceGrid.Position(24, 3), true);
        }

        private void Sim101_Load(object sender, EventArgs e)
        {
            initCommon();


            initZMQ();
            runZMQ();
            maxVol = Convert.ToInt32(ConfigurationManager.AppSettings["maxVol"]);
            tickPoint = Convert.ToDouble(ConfigurationManager.AppSettings["tickPoint"]);
            frmPrice = ConfigurationManager.AppSettings["FormatPrice"];

            //comb_Instrument.SelectedIndex = comb_Instrument.Items.IndexOf("");
            //comb_product.SelectedIndex = 0;
            //comb_Instrument.SelectedIndex = 0;
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
            comb_account.Enabled = false;


        }

        private void initCommon()
        {

            common = Common.Instance();
            common.Init();

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

            Thread testThread = new Thread((ThreadStart)delegate { ZMQControl.Instance().run(this); });
            testThread.IsBackground = true;
            testThread.Start();
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

        private bool StratHandle(ref OrderEasy_data data)
        {
            return true;
        }
        private OrderEasy_data SetStratData(OrderEasy_data data)
        {

            //data.orderReq.orderType = OrderEasy_data.OrderType.strat;
            OrderEasy_data.Strategy strate = new OrderEasy_data.Strategy();
            strate.vol = Convert.ToInt32(numericQty1.Value);
            if (data.orderReq.dir == OrderEasy_data.Direction.buy)
            {

                strate.maxPrice = data.orderReq.price + Convert.ToDouble(numProfitStop1.Value) * tickPoint;
                strate.minPrice = data.orderReq.price - Convert.ToDouble(numStopLoss1.Value) * tickPoint;
            }
            else
            {
                strate.maxPrice = data.orderReq.price + Convert.ToDouble(numStopLoss1.Value) * tickPoint;
                strate.minPrice = data.orderReq.price - Convert.ToDouble(numProfitStop1.Value) * tickPoint;
            }
            this.strategy = strate;
            data.orderReq.strat.Add(strate);

            return data;
        }
        private void SetStratCancelBtns(OrderEasy_data data)
        {

            int r = 0;
            if (priceMap.TryGetValue(data.orderReq.strat[0].minPrice + "", out r))
            {
                if (r > 0)
                    SetStratCancelBtn(r, data.orderReq.dir, true);
            }

            if (priceMap.TryGetValue(data.orderReq.strat[0].maxPrice + "", out r))
            {
                if (r > 0)
                    SetStratCancelBtn(r, data.orderReq.dir, true);
            }
        }
        private bool  OrderHandle(OrderEasy_data.Direction dir, double price, OrderEasy_data.OrderType orderType)
        {
            //seqNo++;
            OrderEasy_data data = new OrderEasy_data();
            data.reqType = OrderEasy_data.REQType.order;
            data.orderReq = new OrderEasy_data.OrderREQ();
            data.orderReq.symbol = subTopic;
            data.orderReq.orderType = orderType;
            data.orderReq.guid = Convert.ToString(Guid.NewGuid());
            data.orderReq.dir = dir;
      
            data.orderReq.qty = Convert.ToInt32(txtOrderQty.Value);
            data.orderReq.price = price;
            if ("<Custom>".Equals(this.comboBox_strat.SelectedItem))
            {
                if (!CheckStrat())
                {
                    return false;
                }
                if (orderType == OrderEasy_data.OrderType.strat)
                {
                    data.orderReq.strat.Add(strategy);
                    SetStratCancelBtns(data);
                    int stratVol = data.orderReq.strat[0].vol;
                    data.orderReq.qty = notClosePos.vol > stratVol ? stratVol : notClosePos.vol;
                    data.orderResp = new OrderEasy_data.OrderRESP();
                    data.orderResp.e_x = OrderEasy_data.E_x.Exit;
                }
                else
                {
                    data = SetStratData(data);
                }

            }
            if (!AddGuidPriceMap(data.orderReq.guid, price))
                return false;

            ZMQControl.Instance().Send2Router(data);

            //priceSeqNoMap.Add(price, seqNo);
            if (dir == OrderEasy_data.Direction.buy)
                buyGuidList.Add(data.orderReq.guid);
            else
                sellGuidList.Add(data.orderReq.guid);
            return true;

        }

        private bool CheckStrat()
        {
            if (numStopLoss1.Value <= 0)
            {
                MessageBox.Show("止损参数错误设置：" + numStopLoss1.Value);
                return false;
            }
            if (numProfitStop1.Value <= 0)
            {
                MessageBox.Show("止赢参数错误设置：" + numProfitStop1.Value);
                return false;
            }


            return true;
        }
        private void CacelHandle(List<string> guids, OrderEasy_data.CancelType cType, OrderEasy_data.Direction dir)
        {

            OrderEasy_data data = new OrderEasy_data();
            data.reqType = OrderEasy_data.REQType.cancel;
            data.cancelReq = new OrderEasy_data.CancelREQ();
            data.cancelReq.cancelType = cType;

            foreach (string guid in guids)
            {
                data.cancelReq.guid.Add(guid);
            }

            data.cancelReq.symbol = subTopic;
            data.cancelReq.dir = dir;
            ZMQControl.Instance().Send2Router(data);
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

            CacelHandle(guidList, OrderEasy_data.CancelType.cancelAll, OrderEasy_data.Direction.buy);

            if (notClosePos.vol > 0)
            {
                if (notClosePos.pos == OrderEasy_data.PosStatus.Long)
                {
                    SetCancelButton(24, OrderEasy_data.Direction.sell, true);
                    OrderHandle(OrderEasy_data.Direction.sell, curTicker.bid, OrderEasy_data.OrderType.market);
                }
                else
                {
                    SetCancelButton(24, OrderEasy_data.Direction.buy, true);
                    OrderHandle(OrderEasy_data.Direction.buy, curTicker.ask, OrderEasy_data.OrderType.market);
                }
            }
        }

        private void RevHandle()
        {

            if (notClosePos.vol > 0)
            {
                if (notClosePos.pos == OrderEasy_data.PosStatus.Long)
                {
                    OrderHandle(OrderEasy_data.Direction.sell, curTicker.bid, OrderEasy_data.OrderType.rev);
                }
                else
                {
                    OrderHandle(OrderEasy_data.Direction.buy, curTicker.ask, OrderEasy_data.OrderType.rev);
                }
            }
        }


        #region ServiceRtnHandle


        public void InitRtnHandle(OrderEasy_data data)
        {
            Invoke((MethodInvoker)delegate
            {
                InitRtn(data);
            });
        }
        public void OrderRtnHandle(OrderEasy_data data)
        {
            Invoke((MethodInvoker)delegate
            {
                OrderRtn(data);


            });
        }
        public void CancelRtnHandle(OrderEasy_data data)
        {
            Invoke((MethodInvoker)delegate
            {
                CancelRtn(data);
            });
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
        private void OrderRtnImp(OrderEasy_data data, int r, double price)
        {

            if (!priceMap.TryGetValue(FormatPrice(price), out r))
            {
                return;
            }
            if (data.orderReq.orderType == OrderEasy_data.OrderType.market)
            {
                r = 24;
            }
            if (data.orderReq.dir == OrderEasy_data.Direction.buy)
            {
                //
                grid1[r, 1].View = GreenView;
                if (data.orderReq.orderType == OrderEasy_data.OrderType.strat
                    && data.orderResp.e_x == OrderEasy_data.E_x.Exit)
                {//自动平仓策略开起
                    OrderEasy_data.Strategy strat = data.orderReq.strat[0];
                    SetStopView(RedView, 1, strat.maxPrice);
                    SetStopView(GreenView, 1, strat.minPrice);
                }

            }
            else
            {
                grid1[r, 5].View = GreenView;
                if (data.orderReq.orderType == OrderEasy_data.OrderType.strat
                    && data.orderResp.e_x == OrderEasy_data.E_x.Exit)
                {//自动平仓策略开起
                    OrderEasy_data.Strategy strat = data.orderReq.strat[0];
                    SetStopView(GreenView, 2, strat.maxPrice);
                    SetStopView(RedView, 2, strat.minPrice);
                }

            }
            grid1.Refresh();
            return;
        }
        private void TradeRtnHandle(OrderEasy_data data, int r, double price, OrderEasy_data.Direction dir)
        {

            if (!priceMap.TryGetValue(FormatPrice(price), out r))
            {
                buyGuidList.Remove(data.orderReq.guid);
                return;
            }
            if (data.orderReq.orderType == OrderEasy_data.OrderType.market)
            {
                r = 24;
                if (dir == OrderEasy_data.Direction.buy)
                    marketBuyPrice = 0;
                else
                    marketSellPrice = 0;
            }
            if (data.orderReq.strat.Count > 0 && data.orderResp.ErrorID == 0)
            {
                TradeStratHandle(data);
            }
            if (data.orderReq.qty > 0)//部分成交
            {
                //SetLeftVol(data);
                int c = dir == OrderEasy_data.Direction.buy ? 1 : 5;
                grid1[r, c].Value = data.orderReq.qty;
                grid1.Refresh();
                if (data.orderResp.ErrorID == 0)
                    return;
            }
            if (dir == OrderEasy_data.Direction.buy)
                buyGuidList.Remove(data.orderReq.guid);
            else
                sellGuidList.Remove(data.orderReq.guid);

            SetCancelButton(r, dir, false);
            RemoveGuidPriceMap(data.orderReq.guid);
        }
        /// <summary>
        /// 处理自动平仓策略
        /// </summary>
        /// <param name="data"></param>
        private void TradeStratHandle(OrderEasy_data data)
        {

            if (data.orderReq.qty > 0)//部分成交
            {
                //if (data.orderResp.e_x == OrderEasy_data.E_x.Entry)
                //return;

            }
            if (data.orderResp.e_x == OrderEasy_data.E_x.Entry)
            {
                SendStratOrder(data);//通知后台开起自动平仓策略
            }
            else if (data.orderResp.e_x == OrderEasy_data.E_x.Exit)
            {
                //自动平仓完成
                CancelStratBtn(strategy.minPrice, data.orderReq.dir);
                CancelStratBtn(strategy.maxPrice, data.orderReq.dir);
            }
        }
        private void CancelStratBtn(double price, OrderEasy_data.Direction dir)
        {
            int r = 0;
            if (priceMap.TryGetValue(price + "", out r))
            {
                if (r > 0)
                {
                    SetStratCancelBtn(r, dir, false);
                }
            }
        }
        private void TradeRtn(OrderEasy_data data, int r, double price)
        {

            notClosePos.vol = data.orderResp.notCloseVol;
            notClosePos.pos = data.orderResp.pos;
            SetFLAT();
            TradeRtnHandle(data, r, price, data.orderReq.dir);

        }
        private void OrderRtn(OrderEasy_data data)
        {
            if (data.orderResp.ErrorID != 0)//return error
            {

                ErrorHanle(data);
                //return;
            }
            double price = 0;
            if (!guidPriceMap.TryGetByFirst(data.orderReq.guid, out price))
            {
                showErrorMsg("there havn't fund price by rev seqno:" + data.orderReq.guid);
                return;
            }
            int r = -1;
            if (data.orderResp.ErrorMsg == "")//return Immediately
            {
                OrderRtnImp(data, r, price);
            }
            else//成交回报
            {
                TradeRtn(data, r, price);
            }
            grid1.Refresh();
        }
        private void SetStratCancelBtn(int r, OrderEasy_data.Direction dir, bool visable)
        {
            if (r <= 0)
            {
                return;
            }
            if (dir == OrderEasy_data.Direction.buy)
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
        private void SetCancelButton(int r, OrderEasy_data.Direction dir, bool visable)
        {

            if (r <= 0)
            {
                return;
            }
            if (dir == OrderEasy_data.Direction.buy)
            {
                if (visable)
                {
                    grid1[r, 0] = new SourceGrid.Cells.Cell("×", typeof(string));
                    grid1[r, 0].View = delView;
                    grid1[r, 0].AddController(new CancelClickEvent());

                    grid1[r, 1] = new SourceGrid.Cells.Cell("", typeof(string));
                    grid1[r, 1].Value = this.txtOrderQty.Value;
                    grid1[r, 1].View = YellowView;
                    if (r < 24)
                    {
                        grid1[r, 2].View = LmtView;

                        grid1[r, 2].Value = "LMT " + getLimitVol(r, OrderEasy_data.Direction.buy);
                    }
                }
                else
                {
                    grid1[r, 0] = null;
                    grid1[r, 1] = null;
                    if (r <= 23)
                    {
                        grid1[r, 2].Value = getLimitVol(r, OrderEasy_data.Direction.buy);
                        grid1[r, 2].View = cellView;
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
                    grid1[r, 5].Value = this.txtOrderQty.Value;
                    grid1[r, 5].View = YellowView;
                    if (r < 24)
                    {
                        grid1[r, 4].Value = "LMT " + getLimitVol(r, OrderEasy_data.Direction.sell);
                        grid1[r, 4].View = LmtView;
                    }
                }
                else
                {
                    grid1[r, 6] = null;
                    grid1[r, 5] = null;
                    if (r <= 23)
                    {
                        grid1[r, 4].Value = getLimitVol(r, OrderEasy_data.Direction.buy);
                        grid1[r, 4].View = cellView;
                    }
                }
            }
            SetCancelAllButton(dir);
            grid1.Refresh();
        }
        private void SetCancelAllButton(OrderEasy_data.Direction dir)
        {
            int c = dir == OrderEasy_data.Direction.buy ? 0 : 6;
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
            if (notClosePos.vol == 0)
            {
                grid1[26, 3].Value = "FLAT";
                grid1[26, 3].View = buttonView;
            }
            else
            {
                string posStr = notClosePos.pos == OrderEasy_data.PosStatus.Long ? "L" : "S";
                grid1[26, 3].Value = notClosePos.vol + posStr;
                grid1[26, 3].View = notClosePos.pos == OrderEasy_data.PosStatus.Long ? GreenView : RedView;
            }
            grid1.Refresh();
        }
        private void CancelRtn(OrderEasy_data data)
        {
            double price = 0;
            if (data.cancelResp.ErrorID != 0)//return error
            {

                showErrorMsg("ErrorID:" + data.cancelResp.ErrorID + "|msg:" + data.cancelResp.ErrorMsg);

                return;
            }
            if (!guidPriceMap.TryGetByFirst(data.cancelResp.guid, out price))
            {
                showErrorMsg("there havn't fund price by rev seqno:" + data.cancelResp.guid);
                return;
            }
            int r = -1;
            if (data.cancelReq.cancelType == OrderEasy_data.CancelType.cancelMarket)
            {
                r = 24;
            }
            else if (!priceMap.TryGetValue(FormatPrice(price), out r))
            {
                return;
            }

            if (data.cancelResp.ErrorMsg == "")//return Immediately
            {

                return;
            }
            else//撤单回报
            {

                notClosePos.vol = data.cancelResp.notCloseVol;
                notClosePos.pos = data.cancelResp.pos;
                SetFLAT();

                if (data.cancelReq.dir == OrderEasy_data.Direction.buy)
                {
                    grid1[r, 0] = null;
                    grid1[r, 1] = null;
                    if (data.cancelReq.cancelType == OrderEasy_data.CancelType.cancelMarket)
                    {
                        marketBuyPrice = 0;
                    }
                    buyGuidList.Remove(data.orderReq.guid);
                    SetCancelButton(r, OrderEasy_data.Direction.buy, false);
                    grid1.Refresh();

                }
                else
                {
                    grid1[r, 5] = null;
                    grid1[r, 6] = null;
                    if (data.orderReq.orderType == OrderEasy_data.OrderType.market)
                    {
                        marketSellPrice = 0;
                    }
                    sellGuidList.Remove(data.orderReq.guid);
                    SetCancelButton(r, OrderEasy_data.Direction.sell, false);
                    grid1.Refresh();

                }
                RemoveGuidPriceMap(data.orderReq.guid);
            }
            grid1.Refresh();
        }

        private void RemoveGuidPriceMap(string guid)
        {
            double maxPrice = 0;
            guidPriceMap.TryGetByFirst(guid, out maxPrice);
            if (!guidPriceMap.TryRemoveByFirst(guid))
            {
                showErrorMsg("guidPriceMap remove fail!,guid=" + guid);
            }
            pairPriceMap.TryRemoveByFirst(maxPrice);
        }

        private string getLimitVol(int r, OrderEasy_data.Direction dir)
        {
            string vol = dir == OrderEasy_data.Direction.buy ? grid1[r, 2].Value + "" : grid1[r, 4].Value + "";
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
            Tick_TenEntrust tenTick = curTenTicker;
            grid1[lastIndex, 3].View = lastCell;
            grid1[0, 9].Value = tenTick.best_ask_qty1 + tenTick.best_ask_qty2+
                tenTick.best_ask_qty3 + tenTick.best_ask_qty4+
                tenTick.best_ask_qty5 + tenTick.best_ask_qty6+
                tenTick.best_ask_qty7 + tenTick.best_ask_qty8+
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
                string curPrice = FormatPrice(firstPrice - (i - 1) * tickPoint);
                grid1[i, 3].Value = curPrice;
                if (priceMap.Count == 0 && !priceMap.ContainsKey(curPrice))
                {
                    priceMap.Add(curPrice, i);
                }
                else
                {
                    priceMap[curPrice] = i;
                }
                ////////////////////last////////////////////////
                if (Convert.ToDouble(curTicker.last) == Convert.ToDouble(grid1[i, 3].Value))
                {
                    grid1[i, 3].Value = "(" + curTicker.vol + ")" + grid1[i, 3].Value;
                    lastIndex = i;
                    grid1[lastIndex, 3].View = lastCell;
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
        
            for (int i = 1; i < 24; i++)
            {
                double p = firstPrice - (i - 1) * tickPoint;
                string curPrice = FormatPrice(p);
                grid1[i, 3].Value = curPrice;
                if (priceMap.Count == 0 && !priceMap.ContainsKey(curPrice))
                {
                    priceMap.Add(curPrice, i);
                }
                else
                {
                    priceMap[curPrice] = i;
                }
                ////////////////////last////////////////////////
                if (curTicker.last == Convert.ToDouble(grid1[i, 3].Value))
                {
                    String value = Convert.ToString(grid1[lastIndex, 3].Value);
                    
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
                    grid1[i, 2].Value = "LMT ";
                }
                else
                {
                    grid1[i, 2].Value = "";
                }

                if (Convert.ToString(grid1[i, 4].Value).Contains("LMT"))
                {
                    grid1[i, 4].Value = "LMT ";
                }
                else
                {
                    grid1[i, 4].Value = "";
                }

            }
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
            }
            else
            {
                grid1[priceMap[FormatPrice(price)], index].Value = vol;
            }

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

                SourceGrid.Grid grid = (SourceGrid.Grid)sender.Grid;
                OrderEasy f = (OrderEasy)grid.Parent.Parent;
                f.groupBox2.Visible = !f.groupBox2.Visible;
                if (f.groupBox2.Visible)
                {
                    f.grid1[26, 1].Value = ">";
                    f.Height = 735;
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
                f.RevHandle();

            }
        }
        private void InitVolHandle()
        {
            string showMsg = "清空所有的交易信息 并且初始化后台? \r\n\n ";
            DialogResult result = MessageBox.Show(showMsg, "初始化 确认", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                return;
            }

            OrderEasy_data data = new OrderEasy_data();
            data.reqType = OrderEasy_data.REQType.init;
            data.initReq = new OrderEasy_data.InitREQ();
            data.initReq.maxVol = maxVol;
            data.initReq.account = account;
            data.initReq.symbol = subTopic;

            wirteDebugMsg("send Init req");
            ZMQControl.Instance().Send2Router(data);

            ClearData();
            SetCancelButton2NULL(OrderEasy_data.Direction.buy);
            SetCancelButton2NULL(OrderEasy_data.Direction.sell);

        }

        private void BuyClinkHandle(int r)
        {

            string showMsg = "Do you want to place this order? \r\n\n Buy limit "
                + txtOrderQty.Value + " contract(s) " + subTopic + "@ " + grid1[r, 2].Value;
            DialogResult result = MessageBox.Show(showMsg, "Confirm order placement", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                return;
            }       

            Double price = GetPriceByIndex(r);
            if (OrderHandle(OrderEasy_data.Direction.buy, price, OrderEasy_data.OrderType.limit))
            {
                SetCancelButton(r, OrderEasy_data.Direction.buy, true);
            }
        }

        private void SellClinkHandle(int r)
        {

            string showMsg = "Do you want to place this order? \r\n\n Sell limit "
                + txtOrderQty.Value + " contract(s) " + subTopic + "@ " + getLimitVol(r, OrderEasy_data.Direction.sell);
            DialogResult result = MessageBox.Show(showMsg, "Confirm order placement", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                return;
            }

           

            Double price = GetPriceByIndex(r);
            if (OrderHandle(OrderEasy_data.Direction.sell, price, OrderEasy_data.OrderType.limit))
            {
                SetCancelButton(r, OrderEasy_data.Direction.sell, true);
            
            }
        }

        private void MarketBuyClinkHandle(int r)
        {

            string showMsg = "          Do you want to place this order? \r\n\r\n\n          Buy market "
                + txtOrderQty.Value + " contract(s) " + subTopic + "\r\n\r\n";
            DialogResult result = MessageBox.Show(showMsg, "Confirm order placement", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                return;
            }
         

            marketBuyPrice = curTicker.ask;
            if(OrderHandle(OrderEasy_data.Direction.buy, marketBuyPrice, OrderEasy_data.OrderType.market))
                SetCancelButton(r, OrderEasy_data.Direction.buy, true);
        }

        private void MarketSellClinkHandle(int r)
        {

            string showMsg = "          Do you want to place this order? \r\n\r\n\n          Sell market "
                + txtOrderQty.Value + " contract(s) " + subTopic + "\r\n\r\n";
            DialogResult result = MessageBox.Show(showMsg, "Confirm order placement", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                return;
            }
            

            marketSellPrice = curTicker.bid;
            if(OrderHandle(OrderEasy_data.Direction.sell, marketSellPrice, OrderEasy_data.OrderType.market))
                SetCancelButton(r, OrderEasy_data.Direction.sell, true);
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
            /// <summary>
            /// 视图单元格点击事件
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
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
                    if (c == 2)
                        f.MarketBuyClinkHandle(r);
                    else if (c == 4)
                        f.MarketSellClinkHandle(r);
                }

            }
        }
        private void SetCancelButton2NULL(OrderEasy_data.Direction dir)
        {
            for (int i = 1; i < 25; i++)
            {
                //grid1[i, c] = null;
                SetCancelButton(i, dir, false);
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
                OrderEasy_data.Direction dir = c == 0 ? OrderEasy_data.Direction.buy : OrderEasy_data.Direction.sell;
                double price = c == 0 ? f.marketBuyPrice : f.marketSellPrice;
                if (r == 24)
                {
                    List<string> guids = f.GetGuidListByPrice(price);
                    if (guids.Count <= 0)
                    {
                        f.SetCancelButton(24, dir, false);
                        return;
                    }
                    f.CacelHandle(guids, OrderEasy_data.CancelType.cancelMarket, dir);
                }
                else if (r == 26)
                {
                    List<string> guids = c == 0 ? f.buyGuidList : f.sellGuidList;
                    f.CacelHandle(guids, OrderEasy_data.CancelType.cancelAll, dir);

                   

                }
                else
                {
                    f.CacelHandle(f.GetGuidListByPrice(f.GetPriceByIndex(r)), OrderEasy_data.CancelType.cancelOne, dir);
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
                f.SetFocus();
                f.CloseHandle();
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
            notClosePos.vol = 0;         
            guidPriceMap.Clear();
            pairPriceMap.Clear();
            buyGuidList.Clear();
            sellGuidList.Clear();
            marketBuyPrice = 0;
            marketSellPrice = 0;
            SetFLAT();
        }

        private void SetTarget2(bool visible)
        {
            numericQty2.Visible = visible;
            numStopLoss2.Visible = visible;
            numProfitStop2.Visible = visible;
        }

        private void SetTarget3(bool visible)
        {
            numericQty3.Visible = visible;
            numStopLoss3.Visible = visible;
            numProfitStop3.Visible = visible;
        }

        private void radBtn1_CheckedChanged(object sender, EventArgs e)
        {
            numericQty1.Value = txtOrderQty.Value;
            numericQty1.Enabled = false;
            SetTarget2(false);
            SetTarget3(false);

        }

        private void radBtn2_CheckedChanged(object sender, EventArgs e)
        {
            numericQty1.Enabled = true;
            SetTarget2(true);
            SetTarget3(false);
        }

        private void radBtn3_CheckedChanged(object sender, EventArgs e)
        {
            numericQty1.Enabled = true;
            SetTarget2(true);
            SetTarget3(true);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ("<Custom>".Equals(Convert.ToString(this.comboBox_strat.SelectedItem)))
            {
                this.groupBox2.Enabled = true;
            }
            else
            {
                this.groupBox2.Enabled = false;
            }
        }

        private void txtOrderQty_ValueChanged(object sender, EventArgs e)
        {

            if (txtOrderQty.Value == 0)
            {
                txtOrderQty.Value = 1;
            }
            if (this.radBtn1.Checked)
            {
                numericQty1.Value = txtOrderQty.Value;
            }
        }

        private void comb_Instrument_SelectedIndexChanged(object sender, EventArgs e)
        {
            string product = Convert.ToString(comb_product.SelectedItem);
            string symbol = Convert.ToString(comb_Instrument.SelectedItem);
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
                    }
                }
            }



        }

        private bool AddGuidPriceMap(string guid, double price)
        {

            if (!guidPriceMap.TryAdd(guid, price))
            {
                RemoveGuidPriceMap(guid);
                showErrorMsg("AddGuidPriceMap fail seqNo=" + guid + "price=" + price);
                return false;
            }
            return true;
        }

        private void SendStratOrder(OrderEasy_data oldData)
        {
            OrderEasy_data.Direction dir;

            if (oldData.orderReq.dir == OrderEasy_data.Direction.buy)
                dir = OrderEasy_data.Direction.sell;
            else
                dir = OrderEasy_data.Direction.buy;

            OrderHandle(dir, oldData.orderReq.strat[0].maxPrice, OrderEasy_data.OrderType.strat);             

        }


    }
}
