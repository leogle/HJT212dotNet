/**************************************************
*文件名：Pipeline
*描   述：
*创建者：lrh
*时间：2018-04-04 17:14:23
*
****************************************************/
using System.Collections.Generic;

namespace GNL.Common.Protocol
{
    /// <summary>
    /// 流水线
    /// </summary>
    public class Pipeline
    {
        /// <summary>
        /// 处理器队列
        /// </summary>
        private List<ClientHandler> workline = new List<ClientHandler>();

        /// <summary>
        /// 处理线程队列
        /// </summary>
        private List<ParallelDataConsumer<ClientHandler.ClientHandlerModel>> workerList =
            new List<ParallelDataConsumer<ClientHandler.ClientHandlerModel>>();
         
        /// <summary>
        /// 添加处理器
        /// </summary>
        /// <param name="handler">处理器</param>
        public void AddHandler(ClientHandler handler)
        {
            AddHandler(handler, 1);
        }

        /// <summary>
        /// 添加处理器
        /// </summary>
        /// <param name="handler">处理器</param>
        /// <param name="concurrentLevel">该处理器同时启动线程数</param>
        public void AddHandler(ClientHandler handler,int concurrentLevel)
        {
            handler.Pipeline = this;
            handler.PipLineIndex = workline.Count;
            workline.Add(handler);
            workerList.Add(new ParallelDataConsumer<ClientHandler.ClientHandlerModel>
                (concurrentLevel, handler.OnChannelRead));
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            foreach (var worker in workerList)
            {
                worker.Start();
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            foreach (var worker in workerList)
            {
                worker.Stop();
            }
        }

        /// <summary>
        /// 流水线入口
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="msg"></param>
        public void Spout(HandlerContext ctx, object msg)
        {
            var model = new ClientHandler.ClientHandlerModel()
            {
                Ctx = ctx,
                Msg = msg,
            };
            workerList[0].Produce(model);
        }

        /// <summary>
        /// 交给下一步处理
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="msg"></param>
        public void Pipe(HandlerContext ctx, object msg)
        {
            //ctx.NextStep();
            if (ctx.FlowIndex+1 < workerList.Count)
            {
                workerList[ctx.FlowIndex+1].Produce(new ClientHandler.ClientHandlerModel()
                {
                    Ctx = (HandlerContext)ctx.Clone(),
                    Msg = msg,
                });
            }
        }

        /// <summary>
        /// 通信中断
        /// </summary>
        /// <param name="handlerContext"></param>
        public void Disconnect(HandlerContext handlerContext)
        {
            workline.ForEach(handler => { handler.OnDisconnected(handlerContext); });
        }

        /// <summary>
        /// 通信中断
        /// </summary>
        /// <param name="handlerContext"></param>
        public void Connect(HandlerContext handlerContext)
        {
            workline.ForEach(handler => { handler.OnConnected(handlerContext); });
        }
    }
}
