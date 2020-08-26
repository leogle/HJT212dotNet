/**************************************************
*文件名：DataSegmentHandler
*描   述：
*创建者：lrh
*时间：2018-04-12 16:19:57
*
****************************************************/
using GNL.Common.Protocol.Domain;
using GNL.Common.Protocol.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNL.Common.Protocol.Handler
{

    /// <summary>
    /// 本处理器返回需响应的数据包和请求包
    /// </summary>
    internal class DataSegmentHandler : ClientHandler
    {
        public event MsgEventHandler MsgEvent;
        public event ConnectEventHandler ConnectEvent;
        public event ACKEventHandler ACKEvent;

        public override void channelRead(HandlerContext ctx, object msg)
        {
            try
            {
                var packet = msg as Packet;

                if (MsgEvent != null)
                {
                    MsgEvent(packet.Segment.MN, ctx);
                }
                if (packet.Segment.ACK && Config.Config.ReturnQN)
                {
                    //返回应答
                    if (packet.Segment.CN.StartsWith("20"))
                    {
                        var ack = packet.CreateDataACKPacket();
                        ctx.WriteAndFlush(ack.ToBytes());
                    }
                    else if (packet.Segment.CN.StartsWith("30") || packet.Segment.CN.StartsWith("10"))
                    {
                        //判断数据段格式是否正确
                        var check = packet.Segment.CheckSegment();
                        if (!packet.IsCrcCorrect)
                        {
                            check = QnCode.CRCError;
                        }
                        var ack = packet.CreateReqAck(check);
                        ctx.WriteAndFlush(ack.ToBytes());
                    }
                }

                //数据包为应答包
                //if (packet.Segment.CN.StartsWith("90"))
                //{
                    if (ACKEvent != null)
                    {
                        ACKEvent(packet.Segment.MN, ctx, packet.Segment);
                    }
                //}
                Pipeline.Pipe(ctx, msg);
            }catch(Exception e)
            {
                if (Log != null)
                {
                    Log.Error("DataSegmentHandle Error:", e);
                }
            }
        }

        public override void OnConnected(HandlerContext ctx)
        {
            if (ConnectEvent != null)
            {
                ConnectEvent(ctx, ConnectState.Connected);
            }
        }

        public override void OnDisconnected(HandlerContext ctx)
        {
            if (ConnectEvent != null)
            {
                ConnectEvent(ctx, ConnectState.Disconnect);
            }
        }
    }
}
