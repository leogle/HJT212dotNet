/**************************************************
*文件名：PacketDecoder
*描   述：
*创建者：lrh
*时间：2018-04-10 14:19:03
*
****************************************************/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GNL.Common.Protocol
{
    /// <summary>
    /// 本处理器仅对数据包进行粘包处理
    /// </summary>
    public class PacketDecoder: ClientHandler
    {
        ConcurrentDictionary<string,string> cache=new ConcurrentDictionary<string, string>();
        public override void channelRead(HandlerContext ctx, object msg)
        {
            try
            {
                var ip = (ctx.Socket.RemoteEndPoint as IPEndPoint);
                var key = ip.Address.ToString() + ip.Port;

                byte[] buf = (byte[])msg;
                var str = Encoding.ASCII.GetString(buf);
                LogMsg(string.Format("recv:{0}", str));
                //取出上次未处理完的数据
                var lastData = "";
                if (cache.TryRemove(key, out lastData))
                {
                    str = lastData + str;
                }

                if (str.StartsWith(Packet.HEADER))
                {
                    //多个粘包
                    if (str.Contains(Packet.TAIL + Packet.HEADER))
                    {
                        Log.Info("数据粘包");
                        str = str.Replace(Packet.TAIL + Packet.HEADER, Packet.TAIL + "<##>" + Packet.HEADER);
                        string[] array = Regex.Split(str, "<##>");
                        foreach (var s in array)
                        {
                            if (s.EndsWith(Packet.TAIL))
                            {
                                Log.Info("组包：" + s);
                                Packet p = Packet.FromData(s);
                                Pipe(ctx, p);
                            }
                            else
                            {
                                cache.TryAdd(key, s);
                            }
                        }
                    }
                    //一个包或半个包
                    else
                    {
                        //一个包
                        if (str.EndsWith(Packet.TAIL))
                        {
                            Log.Info("组包：" + str);
                            Packet p = Packet.FromData(str);
                            Pipe(ctx, p);
                        }
                        else//半个包
                        {
                            cache.TryAdd(key, str);
                        }
                    }
                }
                else
                {
                }
            }catch(Exception e)
            {
                if (Log != null)
                {
                    Log.Error("PacketDecoder Error:", e);
                }
            }
        }

        public override void OnDisconnected(HandlerContext ctx)
        {
            var ip = (ctx.Socket.RemoteEndPoint as IPEndPoint);
            var key = ip.Address.ToString() + ip.Port;
            string lastData;
            cache.TryRemove(key, out lastData);
        }

        public override void OnConnected(HandlerContext ctx)
        {

        }
    }
}
