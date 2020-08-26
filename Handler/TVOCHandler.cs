/**************************************************
*文件名：TVOCHandler
*描   述：
*创建者：lrh
*时间：2018-5-22 11:13:03
*
****************************************************/
using GNL.Common.Protocol.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GNL.Common.Protocol.Handler
{
    class TVOCHandler : ClientHandler
    {
        public override void channelRead(HandlerContext ctx, object msg)
        {
            var packet = msg as Packet;
            var segment = packet.Segment;
            var data = packet.Segment.DataSegment;
            if (segment.CN == CommandCode.DATA_MINUTE_HIS)
            {
                HandlerMinute(data);
            }
        }

        private void HandlerMinute(DataSegment data)
        {
            foreach (var pollId in data.PollList)
            {

            }
        }

        public override void OnConnected(HandlerContext ctx)
        {

        }

        public override void OnDisconnected(HandlerContext ctx)
        {

        }
    }
}
