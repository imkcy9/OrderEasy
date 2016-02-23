using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TraderControl
{
    class TradeType
    {
        public class price_type
        {
            public const int RC_ORDER_LIMIT  = 0;
            public const int RC_ORDER_MARKET = 1;
            public const int RC_ORDER_FAK    = 2;
            public const int RC_ORDER_FOK    = 3;
        }
        public class direction
        {
            public const int RC_ORDER_BUY = '0';
            public const int RC_ORDER_SELL = '1';
        }

        //public const int MAX_PACKAGE_SIZE = 8192;
        //public const string RC_ALGO_HEART_BEAT = "A";

        //public const string RC_LOGIN_TRADE = "L";
        /////////////////交易端连接控制端类型///////////////////

        /////错误回报

        //public const string RC_REP_ERROR = "0";
        /////成交回报
        //public const string RC_REP_TRADE = "1";
        /////撤单回报
        //public const string RC_REP_CANCEL = "2";

        /////撤单请求
        //public const string RC_REQ_CANCEL = "3";

        /////下单请求
        //public const string RC_REQ_ORDER = "4";
        //public const string RC_RTN_ORDER = "a";

        //////////////////交易参数类型///////////////////////

        /////开仓
        //public const string RC_ORDER_OPEN = "0";

        /////平今
        //public const string RC_ORDER_CLOSETODAY = "3";

        /////平仓
        //public const string RC_ORDER_CLOSE = "2";

        /////买
        //public const string RC_ORDER_BUY = "0";

        /////卖
        //public const string RC_ORDER_SELL = "1";

        /////投机
        //public const string RC_ORDER_SPECULATION = "1";

        /////套利
        //public const string RC_ORDER_ARBITRAGE = "2";

        //public const string RC_ORDER_AUTO_CLOSE_POSITON = "auotopenorder";
        
        //public const string RC_ORDER_AUTO_OPEN_POSITON = "auotcloseorder";
       
        ///////////////////控制参数类型///////////////////////

        /////只允许开仓标识
        //public const int RC_CONTROL_OPENONLY = 1;

        /////只允许平仓标识
        //public const int RC_CONTROL_CLOSEONLY = 2;

        /////开平均可标识
        //public const int RC_CONTROL_OPENCLOSE = 3;

        /////强平标志
        //public const int RC_CONTROL_FORCECLOSE = 4;


        /////限价单
        //public const int RC_LEG_ROLE_LIMIT_ORDER = 5;


        /////市价单
        //public const int RC_LEG_ROLE_MARKET_ORDER = 6;

        //public const string RC_REQ_LOGIN = "1001";
        //public const string RC_RESP_LOGIN = "1002";
        //public const string RC_REQ_TRADE_SET = "1003";
        //public const string RC_RESP_TRADE_SET = "1004";
        //public const string RC_REQ_TRADING_ACCOUNT ="1005";
        //public const string RC_RESP_TRADING_ACCOUNT = "1006";
        //public const string RC_REQ_POSITION_DETAIL = "1007";
        //public const string RC_RESP_POSITION_DETAIL = "1008";
        //public const string RC_REQ_SUBMIT_ENTRUST = "2001";
        //public const string RC_REQ_CANCEL_ENTRUST = "2004";    

    }
}
