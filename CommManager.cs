/**************************************************
*文件名：CommManager
*描   述：
*创建者：lrh
*时间：2018-5-16 16:12:43
*
****************************************************/
using GNL.Common.Protocol.Comm;
using GNL.Common.Protocol.Domain;
using GNL.Common.Protocol.Handler;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GNL.Common.Protocol
{
    public class CommManager
    {
        public static CommManager Instance = new CommManager();

        private TcpServer tcpServer;
        private TcpClient tcpClient;
        private Dictionary<string, TcpClient> clientDict = new Dictionary<string, TcpClient>();
        private int port = 9999;
        public ClientManager clientManager = new ClientManager();
        private RespManager responseManager = new RespManager();

        public int Port
        {
            get
            {
                return port;
            }

            set
            {
                port = value;
                tcpServer.Port = value;
            }
        }

        public TcpServer TcpServer
        {
            get
            {
                return tcpServer;
            }
        }

        private CommManager()
        {
            tcpServer = new TcpServer(Port);
            // tcpServer.Log = LogManager.GetLogger("");
            //添加包解码
            tcpServer.AddHandler(new PacketDecoder());
            //添加数据段解码
            tcpServer.AddHandler(new SegmentHandler());
            //添加数据包处理
            var dataSegmentHandler = new DataSegmentHandler();
            dataSegmentHandler.MsgEvent += DataSegmentHandler_MsgEvent;
            dataSegmentHandler.ConnectEvent += DataSegmentHandler_ConnectEvent;
            dataSegmentHandler.ACKEvent += DataSegmentHandler_ACKEvent;
            tcpServer.AddHandler(dataSegmentHandler);

            responseManager.SendAction = clientManager.Send;

            
        }

        public void AddSendAddr(string ip,int port)
        {
            tcpClient = new TcpClient(ip, port);
        }

        private void DataSegmentHandler_ACKEvent(string mn, HandlerContext context, Segment segment)
        {
            responseManager.OnPacket(segment);
        }

        private void DataSegmentHandler_ConnectEvent(HandlerContext context, Domain.ConnectState state)
        {
            if(state== Domain.ConnectState.Disconnect)
            {
                clientManager.Disconnect(context);
            }
        }

        private void DataSegmentHandler_MsgEvent(string mn, HandlerContext context)
        {
            clientManager.UpdateClient(mn, context);
        }



        public void AddHandler(ClientHandler handler)
        {
            tcpServer.AddHandler(handler);
        }

        public void Start()
        {
            tcpServer.Start();
            if (tcpClient != null)
            {
                tcpClient.Connect();
            }
        }

        public void Stop()
        {
            tcpServer.Stop();
        }

        /// <summary>
        /// 发送到设备
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        public Segment Send(string mn, Packet packet)
        {
            Segment res = null;
            //等待响应
            if (packet.Segment.ACK)
            {
                try
                {
                    //clientManager.Send(mn, packet);
                    var retry = 0;
                    while (retry < Config.Config.ReCount)
                    {
                        try
                        {
                            var segment = responseManager.SendAndWait(mn, packet);
                            
                            //接收请求应答
                            if (Config.Config.ReturnQN && segment.CN == CommandCode.INT_REQ_ACK &&
                                segment.DataSegment.GetValue(DataSegCode.QnRtn) == QnCode.PerpareExe)
                            {
                                segment = responseManager.WaitNext(mn, packet);
                                //响应包
                                if(segment.CN == packet.Segment.CN)
                                {
                                    res = segment;
                                    segment = responseManager.WaitNext(mn, packet);
                                    var code = segment.DataSegment.GetValue(DataSegCode.ExeRtn);
                                    if (code != ResultCode.SUCCESS)
                                    {
                                        throw new Exception("执行失败：返回代码：" + code);
                                    }
                                }
                                //执行结果
                                else if(segment.CN == CommandCode.INT_RN)
                                {
                                    var code = segment.DataSegment.GetValue(DataSegCode.ExeRtn);
                                    if (code != ResultCode.SUCCESS)
                                    {
                                        throw new Exception("执行失败：返回代码："+ code);
                                    }
                                }
                                else
                                {
                                    throw new Exception("返回数据包顺序有误,CN:"+segment.CN);
                                }
                            }
                            else if(segment.CN == packet.Segment.CN)
                            {
                                res = segment;
                                segment = responseManager.WaitNext(mn, packet);
                                var code = segment.DataSegment.GetValue(DataSegCode.ExeRtn);
                                if (code != ResultCode.SUCCESS)
                                {
                                    throw new Exception("执行失败：返回代码：" + code);
                                }
                            }
                            else if (segment.CN == CommandCode.INT_RN)
                            {
                                var code = segment.DataSegment.GetValue(DataSegCode.ExeRtn);
                                if (code != ResultCode.SUCCESS)
                                {
                                    throw new Exception("执行失败：返回代码：" + code);
                                }
                            }
                            else
                            {
                                throw new Exception("返回数据包顺序有误,CN:" + segment.CN);
                            }

                            break;
                        }
                        catch (TimeoutException)
                        {
                            retry++;
                        }
                    }
                    if (retry == Config.Config.ReCount)
                    {
                        throw new TimeoutException(string.Format("响应超时，经过{0}次重试后无响应", Config.Config.ReCount));
                    }
                } catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    responseManager.RemoveSession(mn, packet);
                }
            }
            //不等待响应
            else
            {
                clientManager.Send(mn, packet);
            }
            return res;
        }

        public void Send(string mn,string text)
        {
            clientManager.Send(mn, text);
        }

        /// <summary>
        /// 发送到TCPClient
        /// </summary>
        /// <param name="packet"></param>
        public void Send(Packet packet)
        {
            if (tcpClient != null)
            {
                tcpClient.Send(packet.ToBytes());
            }
        }

        public void SendTo(string ip,int port,Packet packet)
        {
            var key = ip + "_" + port;
            lock (clientDict)
            {
                if (!clientDict.ContainsKey(key))
                {
                    var tcpClient = new TcpClient(ip, port);
                    tcpClient.Connect();
                    clientDict.Add(key, tcpClient);
                }
            }
            clientDict[key].Send(packet.ToBytes());
        }

        public void SendCmd(string cn,IDictionary<string,string> param)
        {
        }

        public void Test()
        {
            var param = new Dictionary<string, string>();
            param.Add("PollId", "w01018");
            SendCmd(CommandCode.CTRL_REAL_SAMPLE, param);
        }
    }
}
