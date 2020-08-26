/**************************************************
*文件名：SegmentHandler
*描   述：
*创建者：lrh
*时间：2018-04-10 15:51:15
*
****************************************************/
using GNL.Common.Protocol.Domain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GNL.Common.Protocol.Handler
{
    /// <summary>
    /// 本处理器仅对数据段分包时进行组包
    /// </summary>
    public class SegmentHandler: ClientHandler
    {
        private ConcurrentDictionary<string,List<Segment>> segmentCache 
            = new ConcurrentDictionary<string, List<Segment>>();

        public override void channelRead(HandlerContext ctx, object msg)
        {
            try
            {
                var ip = (ctx.Socket.RemoteEndPoint as IPEndPoint);
                var key = ip.Address.ToString() + ip.Port;
                Packet packet = (Packet)msg;

                var segment = packet.Segment;
                //处理分包
                if (segment.Divide)
                {
                    segmentCache.TryAdd(key, new List<Segment>());
                    segmentCache[key].Add(segment);

                    if (segmentCache[key].Count == segment.PNUM)
                    {
                        List<Segment> segments;
                        if (segmentCache.TryRemove(key, out segments))
                        {
                            var orderList = segments.OrderBy(o => o.PNO);
                            var data = string.Join("", orderList.Select(o => o.DataSegStr));
                            DataSegment dataSegment = DataSegment.FromStr(data);
                            segment.DataSegment = dataSegment;
                            Pipeline.Pipe(ctx, packet);
                        }
                    }
                }
                else
                {
                    Pipeline.Pipe(ctx, packet);
                }
            }catch(Exception e)
            {
                if (Log != null)
                {
                    Log.Error("SegmentHandle Error:", e);
                }
            }
        }

        public override void OnDisconnected(HandlerContext ctx)
        {
            var ip = (ctx.Socket.RemoteEndPoint as IPEndPoint);
            var key = ip.Address.ToString() + ip.Port;
            List<Segment> data;
            segmentCache.TryRemove(key, out data);
        }

        public override void OnConnected(HandlerContext ctx)
        {
            
        }
    }
}
