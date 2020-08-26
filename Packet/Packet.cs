/**************************************************
*文件名：Packet
*描   述：
*创建者：lrh
*时间：2018-04-08 15:11:41
*
****************************************************/
using GNL.Common.Protocol.Domain;
using GNL.Common.Protocol.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNL.Common.Protocol
{
    /// <summary>
    /// 通讯包
    /// </summary>
    public class Packet
    {
        public static string HEADER = "##";
        public static string TAIL = "\r\n";
        public static int HEADER_LENGTH = 12;

        public int PacketLength { get; private set; }
        public Segment Segment { get; set; }
        public string Crc { get; set; }
        public bool IsCrcCorrect { get; set; }

        public Packet()
        {

        }
        public Packet(string segmentStr)
        {
            Segment = Segment.FromData(segmentStr);
        }

        public static Packet FromData(byte[] buf,int index,int length)
        {
            var packet = new Packet();
            var header = Encoding.ASCII.GetString(buf, index, 2);
            packet.PacketLength = int.Parse(Encoding.ASCII.GetString(buf, index + 2,4));
            packet.Segment = Protocol.Segment.FromData(buf, index + 6, packet.PacketLength-12);
            packet.Crc = Encoding.ASCII.GetString(buf, index + packet.PacketLength - 6, 4);
            return packet;
        }

        public static Packet FromData(string str)
        {
            var packet = new Packet();
            packet.PacketLength = int.Parse(str.Substring(2,4));
            packet.Segment = Protocol.Segment.FromData(str.Substring(6,packet.PacketLength));
            var bytes = Encoding.ASCII.GetBytes(str.Substring(6, packet.PacketLength));
            var crc = CRC.ToCRC16(bytes);
            packet.Crc = str.Substring(packet.PacketLength + 6, 4);
            packet.IsCrcCorrect = (packet.Crc == crc);
            return packet;
        }

        //public static byte[] ToData(Packet packet)
        //{
        //    var dataBytes = packet.Segment.ToData();
        //    string crc = CRC.ToCRC16(dataBytes);
        //    var datalength = dataBytes.Length;
        //    byte[] buf = new byte[datalength+ HEADER_LENGTH];
        //    Encoding.ASCII.GetBytes(HEADER).CopyTo(buf,0);
        //    BitConverter.GetBytes(datalength).CopyTo(buf,2);
        //    dataBytes.CopyTo(buf, 6);
        //    Encoding.ASCII.GetBytes(crc).CopyTo(buf, datalength+6);
        //    Encoding.ASCII.GetBytes(TAIL).CopyTo(buf, 0);
        //    return buf;
        //}

        public string ToDataStr()
        {
            var dataStr = Segment.ToDataStr();
            var len = Encoding.ASCII.GetBytes(dataStr).Length;
            var crc = CRC.ToCRC16(dataStr,false);

            var resStr = string.Empty;
            resStr += Packet.HEADER;
            resStr += len.ToString("D4");
            resStr += dataStr;
            resStr += crc;
            resStr += Packet.TAIL;

            return resStr;
        }

        public byte[] ToBytes()
        {
            return Encoding.ASCII.GetBytes(this.ToDataStr());
        }

        /// <summary>
        /// 创建请求应答
        /// </summary>
        /// <param name="exeReq"></param>
        /// <returns></returns>
        public Packet CreateExeACKPacket(string exeReq)
        {
            var packet = new Packet();
            packet.Segment = new Segment(this.Segment.QN,
                CommandCode.INT_RN,this.Segment.MN,this.Segment.PW)
            {
                DataSegment = new DataSegment()
                {
                    ExeRtn = exeReq,
                }
            };
            return packet;
        }

        public Packet CreateResponsePacket()
        {
            var packet = new Packet();
            packet.Segment = new Segment(this.Segment.QN,
                this.Segment.CN, this.Segment.MN, this.Segment.PW);
            return packet;
        }

        /// <summary>
        /// 创建数据应答
        /// </summary>
        /// <param name="exeReq"></param>
        /// <returns></returns>
        public Packet CreateDataACKPacket()
        {
            var packet = new Packet();
            packet.Segment = new Segment(this.Segment.QN,
                CommandCode.INT_DATA_ACK, this.Segment.MN, this.Segment.PW)
            {
                DataSegment = new DataSegment()
                {
                }
            };
            return packet;
        }

        /// <summary>
        /// 创建请求应答
        /// </summary>
        /// <param name="qnRtn"></param>
        /// <returns></returns>
        public Packet CreateReqAck(string qnRtn)
        {
            var packet = new Packet();
            packet.Segment = new Segment(this.Segment.QN,
                CommandCode.INT_REQ_ACK, this.Segment.MN, this.Segment.PW)
            {
                DataSegment = new DataSegment()
                {
                    QnRtn = qnRtn,
                }
            };
            return packet;
        }

        public Packet CreateNotifyACK()
        {
            var packet = new Packet();
            packet.Segment = new Segment(this.Segment.QN,
                CommandCode.INT_NOTIFY_ACK, this.Segment.MN, this.Segment.PW)
            {
                DataSegment = new DataSegment()
                {
                }
            };
            return packet;
        }
    }
}
