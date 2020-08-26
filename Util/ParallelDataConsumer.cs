/*******************************************************************
 * * 文件名： DataConsumer.cs
 * * 文件作用： 并发生产者消费者模式的数据处理类
 * *-------------------------------------------------------------------
 * *修改历史记录：
 * *修改时间      修改人    修改内容概要
 * *2011-10-09    lrh       新增
 * *******************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace GNL.Common.Protocol
{
    /// <summary>
    /// 并发生产者消费者模式的数据处理类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ParallelDataConsumer<T>
    {
        //数据容器 FIFO
        protected BlockingCollection<T> messageCollection = new BlockingCollection<T>(new ConcurrentQueue<T>());
        //任务组
        protected System.Threading.Tasks.Task[] consumers;
        //并发数
        protected int concurrentLevel;
        //停止标志源
        protected CancellationTokenSource tokenSource = new CancellationTokenSource();
        protected CancellationToken token;
        //处理动作
        private Action<T> processAction;
        private Action<List<T>> batchProcessAction;
        private int batchCount;
        private ManualResetEvent pauseSignal = new ManualResetEvent(true);
        protected Timer autoStopTimer;

        private int maxCount = -1;

        public int MaxCount
        {
            get { return maxCount; }
            set { maxCount = value; }
        }
        /// <summary>
        /// 消息数量达到最大值时引发处理函数
        /// </summary>
        public Action<ParallelDataConsumer<T>> OnMaxSizeExcess { private get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="concurrentLevel">并发数</param>
        /// <param name="action">处理委托</param>
        public ParallelDataConsumer(int concurrentLevel,Action<T> action)
        {
            this.processAction = action;
            this.concurrentLevel = concurrentLevel;
            token = tokenSource.Token;
            consumers = new System.Threading.Tasks.Task[concurrentLevel];
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="concurrentLevel">并发数</param>
        /// <param name="action">处理函数</param>
        /// <param name="maxCount">指定最大长度，超过该长度激发OnMaxSizeExcess处理函数</param>
        public ParallelDataConsumer(int concurrentLevel,Action<T> action,int maxCount)
            :this(concurrentLevel,action)
        {
            this.maxCount = maxCount;
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="concurrentLevel">并发数</param>
        /// <param name="action">批量处理函数</param>
        /// <param name="batchCount">批量处理数</param>
        public ParallelDataConsumer(int concurrentLevel, Action<List<T>> action,int batchCount)
        {
            this.batchCount = batchCount;
            this.batchProcessAction = action;
            this.concurrentLevel = concurrentLevel;
            token = tokenSource.Token;
            consumers = new System.Threading.Tasks.Task[concurrentLevel];
        }

        ~ParallelDataConsumer()
        {
            Stop();
        }

        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            for (int i = 0; i < concurrentLevel; i++)
            {
                consumers[i] = new System.Threading.Tasks.Task(() => Consume(), token, TaskCreationOptions.LongRunning);
                consumers[i].Start();
            }
        }

        /// <summary>
        /// 通过TaskScheduler启动
        /// </summary>
        public void Start(TaskScheduler scheduler)
        {
            for (int i = 0; i < concurrentLevel; i++)
            {
                consumers[i] = new System.Threading.Tasks.Task(() => Consume(), token, TaskCreationOptions.LongRunning);
                consumers[i].Start(scheduler);
            }
        }

        public void Pause()
        {
            pauseSignal.Reset();
        }

        public void Resume()
        {
            pauseSignal.Set();
        }

        /// <summary>
        /// 开始
        /// </summary>
        public void Start(int autoStopInterval)
        {
            autoStopTimer = new Timer(new TimerCallback(autoStopTimer_Elapsed),null,0,autoStopInterval);
            Start();
        }

        void autoStopTimer_Elapsed(object state)
        {
            Stop();
            autoStopTimer.Dispose();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            messageCollection.CompleteAdding();
            tokenSource.Cancel();
            pauseSignal.Set();
        }

        /// <summary>
        /// 处理队列中数据
        /// </summary>
        protected virtual void Consume()
        {
            try
            {
                while (!messageCollection.IsAddingCompleted)
                {
                    try
                    {
                        pauseSignal.WaitOne();
                        //设置了最大容量限制
                        if (maxCount > 0 && messageCollection.Count > maxCount)
                        {
                            try
                            {
                                OnMaxSizeExcess(this);
                            }
                            catch (NullReferenceException)
                            {
                                //ErrLog.Instance.Error("not define size excess action");
                            }
                        }
                        if (batchCount > 0)
                        {
                            T t = messageCollection.Take();
                            int count = messageCollection.Count;
                            count=batchCount-1>count?count:batchCount-1;
                            List<T> list = new List<T>();
                            list.Add(t);
                            for (int i = 0; i < count; i++)
                            {
                                list.Add(messageCollection.Take(token));
                            }
                            batchProcessAction(list);
                        }
                        else
                        {
                            //阻塞等待消息
                            T t = messageCollection.Take();
                            processAction(t);
                        }
                    }
                    catch (InvalidOperationException) { }
                }
            }
            catch (TaskCanceledException)
            {
            }
        }

        /// <summary>
        /// 获取n个元素，此方法需在处理函数中调用
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<T> Take(int count)
        {
            if (this.consumers.Length > 1)
                throw new InvalidOperationException();
            var list = new List<T>();
            for (int i = 0; i < count; i++)
            {
                list.Add(this.messageCollection.Take(this.token));
            }
            return list;
        }

        public int Clear()
        {
            int res = 0;
            while (this.messageCollection.Count > 0)
            {
                T t;
                if (!this.messageCollection.TryTake(out t, 1))
                {
                    break;
                }
                res++;
            }
            return res;
        }


        /// <summary>
        /// 获取元素个数，此方法需在处理函数中调用
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            return this.messageCollection.Count;
        }

        /// <summary>
        /// 产生待处理数据
        /// </summary>
        /// <param name="t"></param>
        public void Produce(T t)
        {
            if (!messageCollection.IsAddingCompleted)
            {
                messageCollection.Add(t);
            }
        }

        public void Produce(List<T> list)
        {
            if (!messageCollection.IsAddingCompleted)
            {
                list.ForEach(i =>
                {
                    messageCollection.Add(i, token);
                });
            }
        }

        /// <summary>
        /// 获取并发任务状态
        /// </summary>
        /// <param name="i">并发线程数组ID</param>
        /// <returns></returns>
        public TaskStatus GetTaskStatus(int i)
        {
            return consumers[i].Status;
        }
    }
}
