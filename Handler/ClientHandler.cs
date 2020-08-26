/**************************************************
*文件名：IClientHandler
*描   述：
*创建者：lrh
*时间：2018-04-08 14:59:09
*
****************************************************/
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace GNL.Common.Protocol
{
    /// <summary>
    /// 流水线处理器
    /// </summary>
    public abstract class ClientHandler
    { 
        /// <summary>
        /// 处理器在流水线当前位置
        /// </summary>
        internal int PipLineIndex { get; set; }
        
        public ILog Log { get; set; }

        /// <summary>
        /// 流水线
        /// </summary>
        public Pipeline Pipeline { get; set; }
        /// <summary>
        /// 处理流水线中的数据包
        /// </summary>
        /// <param name="ctx">上下文</param>
        /// <param name="msg">数据包</param>
        public abstract void channelRead(HandlerContext ctx, object msg);

        /// <summary>
        /// Socket通道断开
        /// </summary>
        /// <param name="ctx"></param>
        public abstract void OnDisconnected(HandlerContext ctx);

        /// <summary>
        /// Socket已连接
        /// </summary>
        /// <param name="ctx"></param>
        public abstract void OnConnected(HandlerContext ctx);

        /// <summary>
        /// pipline内部调用函数
        /// </summary>
        /// <param name="model"></param>
        internal void OnChannelRead(ClientHandlerModel model)
        {
            model.Ctx.FlowIndex = this.PipLineIndex;
            channelRead(model.Ctx,model.Msg);
        }

        /// <summary>
        /// 推送到下一个处理器进行处理
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="msg"></param>
        protected void Pipe(HandlerContext ctx, object msg)
        {
            Pipeline.Pipe(ctx, msg);
        }


        public class ClientHandlerModel
        {
            public HandlerContext Ctx { get; set; }
            public object Msg { get; set; }

            public string Key { get
                {
                    return Ctx.Socket.GetHashCode().ToString();
                } }
        }

        protected void LogMsg(string msg)
        {
            if (Log != null)
            {
                Log.Info(msg);
            }
        }
    }
}
