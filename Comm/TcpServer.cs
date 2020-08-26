/**************************************************
*文件名：TcpServer
*描   述：
*创建者：lrh
*时间：2018-04-04 11:24:18
*
****************************************************/
using GNL.Common.Protocol.Event;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GNL.Common.Protocol
{
    /// <summary>
    /// Tcp服务器
    /// </summary>
    public class TcpServer
    {
        private int port;
        private string ip;
        private Socket listenSocket;
        private int backlog = 100;
        private SocketAsyncEventArgsPool _readWritePool = new SocketAsyncEventArgsPool(1000);
        private int numConnections = 1000;
        private BufferManager _bufferManager;
        private int receiveBufferSize = 1024 * 100;
        private const int opsToPreAlloc = 2;
        private Pipeline pipeline = new Pipeline();
        public ILog Log { get; set; }
        public bool IsStarted { get; private set; }

        public int Port
        {
            get
            {
                return port;
            }

            set
            {
                port = value;
            }
        }

        private ParallelDataConsumer<LogEventArg> logMessageQueue;
        public event LogEventHandle logEvent;

        public TcpServer(int port)
        {
            this.Port = port;

            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _bufferManager = new BufferManager(receiveBufferSize * numConnections * opsToPreAlloc,
            receiveBufferSize);
            logMessageQueue = new ParallelDataConsumer<LogEventArg>(1, LogRecv);
            for (int i = 0; i < numConnections; i++)
            {
                var readWriteEventArg = new SocketAsyncEventArgs();
                readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                _bufferManager.SetBuffer(readWriteEventArg);
                _readWritePool.Push(readWriteEventArg);
            }
        }

        private void LogRecv(LogEventArg msg)
        {
            try
            {
                if (logEvent != null)
                {
                    logEvent(this, msg);
                }
            }catch
            {

            }
        }

        /// <summary>
        /// 添加处理器，处理器对数据进行处理，并推送到下一个处理器
        /// </summary>
        /// <param name="hander"></param>
        public void AddHandler(ClientHandler hander)
        {
            hander.Log = Log;
            pipeline.AddHandler(hander);
        }

        public TcpServer(string ip, int port) : this(port)
        {
            this.ip = ip;
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            pipeline.Start();
            logMessageQueue.Start();
            IPEndPoint localEndPoint = new System.Net.IPEndPoint(IPAddress.Any, Port);
            listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            listenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            listenSocket.Bind(localEndPoint);
            listenSocket.Listen(backlog);
            StartAccept(null);
            IsStarted = true;
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            listenSocket.Shutdown(SocketShutdown.Both);
            pipeline.Stop();
            IsStarted = false;
        }

        /// <summary>
        /// IOCP处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Receive:
                        ProcessReceive(e);
                        break;
                    case SocketAsyncOperation.Send:
                        ProcessSend(e);
                        break;
                    case SocketAsyncOperation.Accept:
                        if (e.SocketError == SocketError.OperationAborted)
                        {
                            Log.Debug("接收端关闭");
                        }
                        else
                        {
                            ProcessAccept(e);
                        }
                        break;
                    case SocketAsyncOperation.Disconnect:
                        ProcessDisconnect(e);
                        break;
                    case SocketAsyncOperation.Connect:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                this.Log.Error("io_complete error", ex);
            }
        }

        /// <summary>
        /// 开始异步Accept
        /// </summary>
        /// <param name="e"></param>
        public void StartAccept(SocketAsyncEventArgs e)
        {
            if (e == null)
            {
                e = new SocketAsyncEventArgs();
                e.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
            }
            else
            {
                e.AcceptSocket = null;
            }
            bool willRaiseEvent = true;
            willRaiseEvent = listenSocket.AcceptAsync(e);
            if (!willRaiseEvent)
            {
                ProcessAccept(e);
            }
        }

        /// <summary>
        /// 处理接入
        /// </summary>
        /// <param name="e"></param>
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.BytesTransferred >= 0 && e.SocketError == SocketError.Success)
                {
                    var _socket = e.AcceptSocket;
                    IPEndPoint remoteEndpoint = null;
                    IPEndPoint localEndpoint = null;
                    try
                    {
                        remoteEndpoint = _socket.RemoteEndPoint as IPEndPoint;
                        localEndpoint = _socket.LocalEndPoint as IPEndPoint;
                    }
                    catch (SocketException)
                    { }
                    if (remoteEndpoint != null && localEndpoint != null)
                    {
                        Log.Info("远程客户端连接:"+remoteEndpoint.Address.ToString());
                        SocketAsyncEventArgs readEventArgs = _readWritePool.Pop();
                        readEventArgs.AcceptSocket = e.AcceptSocket;
                        bool willRaiseEvent = true;
                        willRaiseEvent = e.AcceptSocket.ReceiveAsync(readEventArgs);
                        if (!willRaiseEvent)
                        {
                            ProcessReceive(readEventArgs);
                        }
                        pipeline.Connect(new HandlerContext() { Socket = e.AcceptSocket,Log = this.Log });
                    }
                }
            }
            catch
            {
                //Logger.Error("ProcessAccept Error{0}", ex);
            }
            StartAccept(e);
        }

        private void ProcessDisconnect(SocketAsyncEventArgs e)
        {
            Log.Info("远程客户端中断:" + e.ConnectSocket);
            pipeline.Disconnect(new HandlerContext() { Socket = e.AcceptSocket,Log = this.Log });
            CloseClientSocket(e.AcceptSocket);
            this.ReleaseSAEA(e);
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Socket token = e.AcceptSocket;

            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                byte[] buf = new byte[e.BytesTransferred];
                Array.Copy(e.Buffer,e.Offset,buf,0,e.BytesTransferred);
                pipeline.Spout(new HandlerContext() {Socket = e.AcceptSocket,Log = this.Log},buf);
                //Console.WriteLine(Encoding.ASCII.GetString(buf));
                var str = Encoding.ASCII.GetString(buf);
                Log.Info("[recv]"+str);
                logMessageQueue.Produce(new LogEventArg() { Msg = str, socket = token });
                var willRaiseEvent = e.AcceptSocket.ReceiveAsync(e);
                if (!willRaiseEvent)
                {
                    ProcessReceive(e);
                }
            }
            else
            {
                Log.Info("远程客户端中断:" + (e.AcceptSocket.RemoteEndPoint as IPEndPoint).Address);
                pipeline.Disconnect(new HandlerContext() {Socket = e.AcceptSocket,Log = this.Log});
                CloseClientSocket(e.AcceptSocket);
                this.ReleaseSAEA(e);
            }
            sw.Stop();
            Console.WriteLine("recv elapsed" + sw.Elapsed);
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                
            }
            //try
            //{
            //    CloseClientSocket(e.AcceptSocket);
            //}
            //catch { }
            //ReleaseSAEA(e);
        }

        private void CloseClientSocket(Socket token)
        {
            if (token != null)
            {
                //Interlocked.Decrement(ref m_numConnectedSockets);
                //if (OnClientDisConnect != null)
                //{
                //    OnClientDisConnect(token);
                //}
                try
                {
                    token.Shutdown(SocketShutdown.Both);
                }
                catch (Exception ex)
                {
                    //Logger.Error("CloseClientSocket Error", ex);
                    Log.Error(ex.Message);
                }
                token.Dispose();
            }
        }

        private void ReleaseSAEA(SocketAsyncEventArgs e)
        {
            //e.SetBuffer(e.Offset, _receiveBufferSize);
            //e.AcceptSocket = null;
            //e.UserToken = null;
            //m_maxNumberAcceptedClients.Release();
            _readWritePool.Push(e);
        }

        //public void Log(string format, params object[] obj)
        //{
        //    Log(string.Format(format, obj));
        //}

        //public void Log(string message)
        //{
        //}
    }
}
