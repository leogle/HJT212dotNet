/**************************************************
*文件名：SenderHandler
*描   述：
*创建者：lrh
*时间：2018-5-21 11:08:14
*
****************************************************/
using GNL.Common.Protocol.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GNL.Common.Protocol.Handler
{
    class SenderHandler : ClientHandler
    {
        public override void channelRead(HandlerContext ctx, object msg)
        {
            var packet = msg as Packet;
            var segment = packet.Segment;
            switch (segment.CN) {
                case CommandCode.INIT_SET:
                    HandleInitSet(ctx, packet);
                    break;
                case CommandCode.PARAM_FATCH_TIME:
                    HandleFatchTime(ctx, packet);
                    break;
            }
            
        }

        /// <summary>
        /// 提取现场机时间
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="packet"></param>
        private void HandleFatchTime(HandlerContext ctx, Packet packet)
        {
            var pollId = packet.Segment.DataSegment.GetValue(DataSegCode.PollId);
            var res = packet.CreateResponsePacket();
            res.Segment.DataSegment.SetValue(DataSegCode.PollId, pollId);
            //在此修改
            res.Segment.DataSegment.SetValue(DataSegCode.SystemTime, DateTime.Now.ToString("yyyyMMddHHmmss"));
            ctx.WriteAndFlush(res.ToBytes());

            var exeRtn = packet.CreateExeACKPacket(ResultCode.SUCCESS);
            ctx.WriteAndFlush(exeRtn.ToBytes());
        }

        /// <summary>
        /// 设置超时时间及重发次数 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="packet"></param>
        private void HandleInitSet(HandlerContext ctx, Packet packet)
        {
            var data = packet.Segment.DataSegment;
            
            var overTime = data.GetInt(DataSegCode.OverTime);
            var reCount = data.GetInt(DataSegCode.ReCount);
            Config.Config.Timeout = overTime;
            Config.Config.ReCount = reCount;
            var res = packet.CreateExeACKPacket(ResultCode.SUCCESS);
            ctx.WriteAndFlush(res.ToBytes());
        }

        public override void OnConnected(HandlerContext ctx)
        {
            throw new NotImplementedException();
        }

        public override void OnDisconnected(HandlerContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
