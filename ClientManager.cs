/**************************************************
*文件名：ClientManager
*描   述：
*创建者：lrh
*时间：2018-5-16 16:18:09
*
****************************************************/
using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GNL.Common.Protocol
{
    public class ClientManager
    {
        public ConcurrentDictionary<string, HandlerContext> clientDict = new ConcurrentDictionary<string, HandlerContext>();
        private static ILog Logger = LogManager.GetLogger("");

        public void UpdateClient(string mn,HandlerContext ctx)
        {
            Logger.Debug(string.Format("更新客户端连接,mn:{0},ip:{1}", mn, ctx.Socket.RemoteEndPoint));
            HandlerContext oldCtx;
            if (clientDict.TryGetValue(mn, out oldCtx))
            {
                clientDict.TryUpdate(mn, ctx, oldCtx);
            }
            else
            {
                clientDict.TryAdd(mn, ctx);
            }
        }
        
        public void Disconnect(HandlerContext ctx)
        {
            try
            {
                var client = clientDict.Where(o => o.Value == ctx).FirstOrDefault();
                HandlerContext clientSocket;
                clientDict.TryRemove(client.Key, out clientSocket);
            }
            catch { }
        } 

        public void Send(string mn,Packet packet)
        {
            if(clientDict.ContainsKey(mn))
            {
                var socket = clientDict[mn].Socket;
                //if (socket.Connected)
                //{
                    Logger.Info("[Send] TO:" + clientDict[mn].Socket.RemoteEndPoint);
                    Logger.Info(packet.ToDataStr());
                    clientDict[mn].Socket.Send(packet.ToBytes());
                //}
                //else
                //{
                //    throw new Exception("设备连接已断开");
                //}
            }
            else
            {
                throw new Exception("设备未连接");
            }
        }

        public HandlerContext GetContext(string mn)
        {
            return clientDict[mn];
        }

        public void Send(string mn, string text)
        {
            if (clientDict.ContainsKey(mn))
            {
                var socket = clientDict[mn].Socket;
                //if (socket.Connected)
                //{
                    Logger.Info("[Send] TO:" + clientDict[mn].Socket.RemoteEndPoint);
                    Logger.Info(text);
                    clientDict[mn].Socket.Send(Encoding.ASCII.GetBytes(text));
                //}
            }
            else
            {
                throw new Exception("设备未连接");
            }
        }
    }
}
