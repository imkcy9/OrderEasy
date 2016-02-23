using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroMQ;
using System.Configuration;
using MDFTicker;
using System.IO;
using ProtoBuf;
using System.Threading;
using RcXSpeedMdfPack;
using order_easy;
namespace OrderEasy.common
{
    class ZMQControl
    {
        private MessageType msg_type = new MessageType();
        public static ZmqContext ctx = ZmqContext.Create();
        private ZmqSocket sktSub = null;
        private ZmqSocket sktDealer = null;
        private static ZMQControl subClass = null;
        public Dictionary<string, bool> connectDic = new Dictionary<string, bool>();
        private OrderEasy sim;
        private LoginForm logForm;
        private bool isDealerInit = false;
        private string control_id;
        private string subscript;
        private int testCount = 0;
        private bool isSubAll = false;
        private Dictionary<string, bool> subMap = new Dictionary<string, bool>();
        private Dictionary<string, bool> connMap = new Dictionary<string, bool>();
        ZMQControl()
        { }

        ~ZMQControl()
        { }

        public static ZMQControl Instance()
        {
            if (subClass == null)
            {
                subClass = new ZMQControl();
            }
            return subClass;
        }
        public void Init(SocketType zmqType, string _subscript)
        {
            if (sktSub == null)
            {
                sktSub = ctx.CreateSocket(zmqType);

                sktSub.TcpKeepalive = TcpKeepaliveBehaviour.Enable;
                
                subscript = _subscript;
                //sktSub.SubscribeAll();
                //sktSub.Subscribe(Encoding.UTF8.GetBytes(_subscript));
                //if (_subscript == "")
                //{
                //    sktSub.SubscribeAll();
                //}
                //else
                //{
                //    sktSub.Subscribe(Encoding.UTF8.GetBytes(_subscript));
                //}
            }
        }
        public void SubScript(string _topic,string _tenTopic)
        {
            if (_topic == "")
            {
                if (isSubAll)
                {
                    return;
                }
                sktSub.SubscribeAll();
                isSubAll = true;
            }
            else
            {
                bool isSub;
                if (subMap.TryGetValue(_topic, out isSub))
                {
                    if (isSub)
                        return;
                }
                sktSub.Subscribe(Encoding.UTF8.GetBytes(_topic));
                sktSub.Subscribe(Encoding.UTF8.GetBytes(_tenTopic));
                subscript = _topic;
                subMap[_topic] = true;
                
            }
        }
        public void UnsubScribe(string _topic, string _tenTopic)
        {
            if (_topic == "")
            {
                if (isSubAll)
                    sktSub.UnsubscribeAll();
                isSubAll = false;
            }
            else
            {
                bool isSub;
                if (subMap.TryGetValue(_topic,out isSub))
                {
                    if (isSub)
                    {
                        sktSub.Unsubscribe(Encoding.UTF8.GetBytes(_topic));
                        sktSub.Unsubscribe(Encoding.UTF8.GetBytes(_tenTopic));
                    }

                }
                subMap[_topic] = false;
            }
        }
        public void InitDealer(SocketType zmqType, string addr, string _control_id)
        {
            control_id = _control_id;
            if (sktDealer == null)
            {
                sktDealer = ctx.CreateSocket(zmqType);

                sktDealer.TcpKeepalive = TcpKeepaliveBehaviour.Enable;
                sktDealer.Identity = Encoding.UTF8.GetBytes(control_id);
                sktDealer.Connect(addr);
                isDealerInit = true;

            }
        }
        public Boolean Send2Router(OrderEasy_data data)
        {
            if (!isDealerInit)
            {
                Program.log.Error("the dealer is not init!");
                return false;
            }
            MemoryStream sParam = new MemoryStream();
            sParam.Seek(0, SeekOrigin.Begin);
            Serializer.Serialize<OrderEasy_data>(sParam, data);
            //this.sktDealer.Send("".ToArray(),
            sktDealer.SendMore(control_id, Encoding.UTF8);
            this.sktDealer.Send(sParam.ToArray());

            return true;
        }

        public Boolean Send2Router(MemoryStream data, string msg_type)
        {
            if (!isDealerInit)
            {
                Program.log.Error("the dealer is not init!");
                return false;
            }
            sktDealer.SendMore(control_id, Encoding.UTF8);
            sktDealer.SendMore(msg_type, Encoding.UTF8);
            this.sktDealer.Send(data.ToArray());

            return true;
        }
        public void Connect(string addr)
        {
            bool isCon = false;
            connMap.TryGetValue(addr, out isCon);
            if (isCon)
            {
                return;
            }
            
            sktSub.Connect(addr);
            connMap[addr] = true;
            connectDic[addr] = true;

        }
        public void DisConnect(string addr)
        {
            bool isCon = false;
            connMap.TryGetValue(addr, out isCon);
            if (!isCon)
            {
                return;
            }
            sktSub.Disconnect(addr);
            connMap[addr] = false;
            connectDic[addr] = false;
        }
        public void run(LoginForm logForm, OrderEasy sim101)
        {
            this.logForm = logForm;
            this.sim = sim101;
            //sktSub.ReceiveReady += (s, e) => ReceiverSubHandler(e.Socket, sktSub);
            //sktDealer.ReceiveReady += (s, e) => ReceiverRouterHandler(e.Socket, sktDealer);
            sktSub.ReceiveReady += new EventHandler<SocketEventArgs>(sktSub_ReceiveReady);
            sktDealer.ReceiveReady += new EventHandler<SocketEventArgs>(sktDealer_ReceiveReady);
            var poller = new Poller(new List<ZmqSocket> { sktSub ,sktDealer});

            //  Process messages from both sockets
            while (true)
            {
                //poller.Poll(System.TimeSpan.FromMilliseconds(2000));
                poller.Poll();
            }
        }

        void sktSub_ReceiveReady(object sender, SocketEventArgs e)
        {
            string more = e.Socket.Receive(Encoding.UTF8);
            byte[] revBytes = new byte[1024];
            int revSize;
            if (more != subscript)
            {
                revSize = e.Socket.Receive(revBytes);
                testCount++;
                Tick_TenEntrust ticker = new Tick_TenEntrust();
                MemoryStream revParam = new MemoryStream(revBytes, 0, revSize);
                revParam.Seek(0, SeekOrigin.Begin);
                try
                {
                    ticker = Serializer.Deserialize<Tick_TenEntrust>(revParam);

                    Program.log.Info("price=" + ticker.best_ask + " count=" + testCount);
                    this.sim.TenTickerRevHandle(ticker);
                }
                catch (Exception ex)
                {
                    Program.log.Error(ex, ex);
                }
                return;
            }
            else
            {
                revSize = e.Socket.Receive(revBytes);
                testCount++;
                MdfTicker ticker = new MdfTicker();
                MemoryStream revParam = new MemoryStream(revBytes, 0, revSize);
                revParam.Seek(0, SeekOrigin.Begin);
                try
                {
                    ticker = Serializer.Deserialize<MdfTicker>(revParam);

                    Program.log.Info("price=" + ticker.last + " count=" + testCount);
                    this.sim.TickerRevHandle(ticker);
                }
                catch (Exception ex)
                {
                    Program.log.Error(ex, ex);
                }
            }
        }

        void sktDealer_ReceiveReady(object sender, SocketEventArgs e)
        {
            string more = e.Socket.Receive(Encoding.UTF8);
            string msgtype = e.Socket.Receive(Encoding.UTF8);

            byte[] revBytes = new byte[1024 * 256];
            int revSize = e.Socket.Receive(revBytes);

            MemoryStream revParam = new MemoryStream(revBytes, 0, revSize);
            revParam.Seek(0, SeekOrigin.Begin);

            if (msgtype == MessageType.OE_LOGIN_RESP)
            {
                login_resp data = new login_resp();
                data = Serializer.Deserialize<login_resp>(revParam);
                this.logForm.on_login_resp(data);
                return;
            }
            if (msgtype == MessageType.OE_ORDER_RESP)
            {
                order_resp data = new order_resp();
                data = Serializer.Deserialize<order_resp>(revParam);
                this.sim.on_order_resp_handle(data);
                return;
            }
            if (msgtype == MessageType.OE_ORDER_RESP_ERR)
            {
                order_resp_err data = new order_resp_err();
                data = Serializer.Deserialize<order_resp_err>(revParam);
                this.sim.on_order_resp_err_handle(data);
                return;
            }
            if (msgtype == MessageType.OE_CANCEL_RESP)
            {
                cancel_resp data = new cancel_resp();
                data = Serializer.Deserialize<cancel_resp>(revParam);
                Program.log.Info("OE_CANCEL_RESP,local_ref;" + data.local_ref + " order_ref:" + data.order_ref);
                return;
            }
            if (msgtype == MessageType.OE_CANCEL_RESP_ERR)
            {
                cancel_resp_err data = new cancel_resp_err();
                data = Serializer.Deserialize<cancel_resp_err>(revParam);
                this.sim.on_cancel_resp_err_handle(data);
                return;
            }
            if (msgtype == MessageType.OE_DELET_RTN)
            {
                delete_rtn data = new delete_rtn();
                data = Serializer.Deserialize<delete_rtn>(revParam);
                this.sim.on_delete_rtn_handle(data);
                return;
            }
            if (msgtype == MessageType.OE_POS_RTN)
            {
                pos_rtn data = new pos_rtn();
                data = Serializer.Deserialize<pos_rtn>(revParam);
                this.sim.on_pos_rtn_handle(data);
                return;
            }
            if (msgtype == MessageType.OE_MESSAGE_RTN)
            {

                return;
            }




            OrderEasy_data respData = new OrderEasy_data();

            try
            {
                respData = Serializer.Deserialize<OrderEasy_data>(revParam);
                switch (respData.reqType)
                {
                    case OrderEasy_data.REQType.cancel:
                    case OrderEasy_data.REQType.close:
                        {

                            this.sim.CancelRtnHandle(respData);
                            break;
                        }
                    case OrderEasy_data.REQType.order:
                        {
                            this.sim.OrderRtnHandle(respData);
                            break;
                        }
                    case OrderEasy_data.REQType.init:
                        {
                            this.sim.InitRtnHandle(respData);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Program.log.Error(ex, ex);
            }
        }
    }
}
