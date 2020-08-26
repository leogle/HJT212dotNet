/**************************************************
*文件名：LogEvent
*描   述：
*创建者：lrh
*时间：2018-5-22 9:32:52
*
****************************************************/
using GNL.Common.Protocol.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace GNL.Common.Protocol.Event
{
    public delegate void LogEventHandle(object sender, LogEventArg arg);
    public delegate void MsgEventHandler(string mn, HandlerContext context);
    public delegate void ConnectEventHandler(HandlerContext context, ConnectState state);
    public delegate void ACKEventHandler(string mn, HandlerContext context, Segment segment);

    public class LogEventArg : EventArgs
    {
        public Socket socket { get; set; }
        public object Msg { get; set; }
    }
}
