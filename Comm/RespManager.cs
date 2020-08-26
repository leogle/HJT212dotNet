/**************************************************
*文件名：RespManager
*描   述：
*创建者：lrh
*时间：2018-5-21 10:40:19
*
****************************************************/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GNL.Common.Protocol.Comm
{
    public class RespManager
    {
        private ConcurrentDictionary<string, RespContext> sessionMap = new ConcurrentDictionary<string, RespContext>();

        public Action<string, Packet> SendAction { get; internal set; }

        /// <summary>
        /// 发送并等待响应
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        public Segment SendAndWait(string mn,Packet packet)
        {
            var qn = packet.Segment.QN;
            RespContext context = null;
            var key = mn + qn;
            if (!sessionMap.TryGetValue(key, out context))
            {
                context = new RespContext(qn);
                sessionMap.TryAdd(key, context);
            }
            context.Event.Reset();
            SendAction(mn, packet);
            if (context.Event.WaitOne(Config.Config.Timeout * 1000))
            {
                return context.Queue.Dequeue();
            }
            else
            {
                throw new TimeoutException("等待响应超时");
            }
        }


        public void RemoveSession(string mn, Packet packet)
        {
            var key = mn + packet.Segment.QN;
            RespContext context;
            sessionMap.TryRemove(key, out context);
        }

        /// <summary>
        /// 获取下个通信包
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        public Segment WaitNext(string mn, Packet packet)
        {
            var qn = packet.Segment.QN;
            var key = mn + qn;
            if (sessionMap.ContainsKey(key))
            {
                var context = sessionMap[key];
                context.Event.Reset();
                if (context.Queue.Count > 0)
                {
                    return context.Queue.Dequeue();
                }
                if (context.Event.WaitOne(Config.Config.Timeout * 1000))
                {
                    return context.Queue.Dequeue();
                }
                else
                {
                    throw new TimeoutException("等待响应超时");
                }
            }
            else
            {
                throw new InvalidOperationException("未能关联发送信息");
            }
        }

        public void OnPacket(Segment segment)
        {
            var qn = segment.QN;
            var key = segment.MN + qn;
            if (sessionMap.ContainsKey(key))
            {
                var context = sessionMap[key];
                context.Queue.Enqueue(segment);
                context.Event.Set();
            }
        }

        public class RespContext
        {
            public string Qn { get; private set; }
            public Queue<Segment> Queue { get; private set; }
            public ManualResetEvent Event { get; private set; }

            public RespContext(string qn)
            {
                Qn = qn;
                Queue = new Queue<Segment>();
                Event = new ManualResetEvent(false);
            }
        }
    }
}
