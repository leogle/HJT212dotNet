/**************************************************
*文件名：HandlerContext
*描   述：
*创建者：lrh
*时间：2018-04-08 14:59:43
*
****************************************************/
using log4net;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GNL.Common.Protocol
{
    /// <summary>
    /// 流水线上下文
    /// </summary>
    public class HandlerContext:ICloneable,IComparable
    {
        public Socket Socket { get; set; }
        public ILog Log { get; set; }
        private int flowIndex = 0;
        /// <summary>
        /// 指示当前正在执行流水线的步骤
        /// </summary>
        public  int FlowIndex  { get { return flowIndex; } set { flowIndex = value; } }

        /// <summary>
        /// 将数据写回发送端
        /// </summary>
        /// <param name="data"></param>
        public void WriteAndFlush(byte[] data)
        {
            Socket.Send(data);
            Log.Info("[send]" + Encoding.ASCII.GetString(data));
        }

        public object Clone()
        {
            return new HandlerContext() { Socket = this.Socket,Log=this.Log };
        }

        public int CompareTo(object obj)
        {
            var ep = (IPEndPoint)Socket.RemoteEndPoint;
            var objEp = (IPEndPoint)(obj as HandlerContext).Socket.RemoteEndPoint;
            return (ep.Address.ToString()+ep.Port).CompareTo(objEp.Address.ToString()+ep.Port);
        }
    }
}
