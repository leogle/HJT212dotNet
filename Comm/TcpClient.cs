/**************************************************
*文件名：TcpClient
*描   述：
*创建者：lrh
*时间：2018-04-04 11:24:41
*
****************************************************/
using GNL.Common.Protocol.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GNL.Common.Protocol
{
    public delegate void MsgRecvEvent(TcpClient sender, string msg);

    public class TcpClient
    {
        private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private string ip;
        private int port;
        private byte[] buffer = new byte[1024 * 10];
        public event MsgRecvEvent msgEvent;
        private Pipeline pipeline = new Pipeline();
        private Task reconnectTask;
        private bool isStop = false;


        public TcpClient(string ip,int port)
        {
            this.ip = ip;
            this.port = port;
            pipeline.AddHandler(new PacketDecoder());
            pipeline.AddHandler(new SegmentHandler());
            pipeline.AddHandler(new DataSegmentHandler());
            pipeline.AddHandler(new SenderHandler());
        }

        public void Connect()
        {
            
            try
            {
                pipeline.Start();
                socket.Connect(ip, port);
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, RecvCallback, socket);
                reconnectTask = new Task(Reconnect);
                reconnectTask.Start();
             
            }
            catch(Exception e)
            {

            }
        }

        private void Reconnect()
        {
            while (!isStop) {
                if (socket == null || socket.Connected == false)
                {
                    try
                    {
                        try
                        {
                            socket.Shutdown(SocketShutdown.Both);
                            socket.Dispose();
                        }
                        catch { }

                        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        socket.Connect(ip, port);
                        socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, RecvCallback, socket);
                    }
                    catch (Exception e)
                    {

                    }
                }
                Thread.Sleep(3000);
            }
        }

        private void RecvCallback(IAsyncResult ar)
        {
            try
            {
                socket = ar.AsyncState as Socket;
                var length = socket.EndReceive(ar);
                if (length > 0)
                {
                    var msg = Encoding.ASCII.GetString(buffer, 0, length);
                    if (msgEvent != null)
                    {
                        msgEvent(this, msg);
                    }
                    byte[] buf = new byte[length];
                    Array.Copy(buffer, 0, buf, 0, length);
                    pipeline.Spout(new HandlerContext() { Socket = socket }, buf);
                    socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, RecvCallback, socket);
                }
                else
                {
                    try
                    {
                        socket.Shutdown(SocketShutdown.Both);
                        socket = null;
                    }
                    catch { }
                }
            }catch(Exception e)
            {

            }
        }

        public void Send(byte[] buf)
        {
            
            socket.Send(buf);
        }
    }
}
