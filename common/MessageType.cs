using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrderEasy.common
{
    class MessageType
    {
        public const string OE_LOGIN_REQ = "OE_1" ;
        public const string OE_LOGIN_RESP = "OE_2";
        public const string OE_ORDER_REQ = "OE_3";
        public const string OE_ORDER_RESP = "OE_4";
        public const string OE_ORDER_RESP_ERR = "OE_5";
        public const string OE_CANCEL_REQ = "OE_6";
        public const string OE_CANCEL_RESP = "OE_7";
        public const string OE_CANCEL_RESP_ERR = "OE_8";
        public const string OE_TRADE_RTN = "OE_9";
        public const string OE_CANCEL_RTN = "OE_10";
        public const string OE_POS_RTN = "OE_11";
        public const string OE_MESSAGE_RTN = "OE_12";
        public const string OE_DELET_RTN = "OE_13";
    }

}
