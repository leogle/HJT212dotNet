/**************************************************
*文件名：PacketFactory
*描   述：
*创建者：lrh
*时间：2018-5-17 10:19:37
*
****************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GNL.Common.Protocol
{
    public class PacketFactory
    {
        public static Packet CreateCtrlCmd(string cn, string mn,string pw) {
            Segment seg = new Segment(CreateQN(), cn,mn,pw)
            {
                ST = Config.Config.ST,
                Flag = (int.Parse(Config.Config.Version) & 0x01).ToString(),
            };
            return new Protocol.Packet()
            {
                Segment = seg
            };
       
        }

        public static Packet CreateDataPacket(string cn, string mn, string pw)
        {
            Segment seg = new Segment(CreateQN(), cn, mn, pw)
            {
                ST = Config.Config.ST,
                Flag = (int.Parse(Config.Config.Version) & 0x01).ToString(),
            };
            return new Protocol.Packet()
            {
                Segment = seg
            };
        }

        private static string CreateQN()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }

        
    }
}
