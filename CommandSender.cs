/**************************************************
*文件名：CommandSender
*描   述：
*创建者：lrh
*时间：2018-5-18 16:59:38
*
****************************************************/
using GNL.Common.Protocol.Domain;
using GNL.Common.Protocol.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GNL.Common.Protocol
{
    /// <summary>
    /// 发送命令与交互信息在此处理
    /// </summary>
    public class CommandSender
    {
        public static CommandSender Instance = new CommandSender();

        private Segment SendPacket(string mn, Packet packet)
        {
            return CommManager.Instance.Send(mn, packet);
        }

        /// <summary>
        /// 设置超时时间及重发次数
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="overtime"></param>
        /// <param name="recount"></param>
        public void SetTimeout(string mn,string pw,int overtime,int recount)
        {
            var packet = PacketFactory.CreateCtrlCmd(CommandCode.INIT_SET, mn,pw);
            packet.Segment.DataSegment.SetValue(DataSegCode.OverTime, overtime.ToString());
            packet.Segment.DataSegment.SetValue(DataSegCode.ReCount, recount.ToString());
            SendPacket(mn, packet);
        }

        /// <summary>
        /// 提取现场机时间
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="pw"></param>
        /// <param name="pollId"></param>
        public DateTime GetTime(string mn,string pw,string pollId)
        {
            var packet = PacketFactory.CreateCtrlCmd(CommandCode.PARAM_FATCH_TIME, mn, pw);
            packet.Segment.DataSegment.SetValue(DataSegCode.PollId, pollId);
            var res = SendPacket(mn, packet);
            return res.GetValue(DataSegCode.SystemTime).ToDateTime(Config.Config.DateTimeFormat);
        }

        public DateTime SetTime(string mn, string pw, string pollId,DateTime time)
        {
            var packet = PacketFactory.CreateCtrlCmd(CommandCode.PARAM_SET_TIME, mn, pw);
            if (!string.IsNullOrEmpty(pollId))
            {
                packet.Segment.DataSegment.SetValue(DataSegCode.PollId, pollId);
            }
            packet.Segment.DataSegment.SetValue(DataSegCode.SystemTime, 
                time.ToString(Config.Config.DateTimeFormat));
            var res = SendPacket(mn, packet);
            return res.GetValue(DataSegCode.SystemTime).ToDateTime(Config.Config.DateTimeFormat);
        }

        /// <summary>
        /// 提取实时数据间隔
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="pw"></param>
        /// <returns></returns>
        public int GetRtdInterval(string mn,string pw)
        {
            var packet = PacketFactory.CreateCtrlCmd(
                CommandCode.PARAM_GET_REALTIME_INTERVAL, mn, pw);
            var res = SendPacket(mn, packet);
            return res.DataSegment.RtdInterval;
        }

        /// <summary>
        /// 设置实时数据间隔
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="pw"></param>
        public void SetRtdInterval(string mn,string pw)
        {
            var packet = PacketFactory.CreateCtrlCmd(
                CommandCode.PARAM_SET_REALTIME_INTERVAL, mn, pw);
            SendPacket(mn, packet);
        }

        /// <summary>
        /// 提取分钟数据间隔
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="pw"></param>
        public int GetMinInterval(string mn,string pw)
        {
            var packet = PacketFactory.CreateCtrlCmd(
                CommandCode.PARAM_GET_MINUTE_INTERVAL, mn, pw);
            var res = SendPacket(mn, packet);
            return res.DataSegment.GetInt(DataSegCode.MinInterval);
        }

        /// <summary>
        /// 设置分钟数据间隔
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="pw"></param>
        public void SetMinInterval(string mn, string pw)
        {
            var packet = PacketFactory.CreateCtrlCmd(
                CommandCode.PARAM_SET_MINUTE_INTERVAL, mn, pw);
            SendPacket(mn, packet);
        }

        /// <summary>
        /// 设置现场机访问密码
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="oldPw"></param>
        /// <param name="newPw"></param>
        public void SetPassword(string mn,string oldPw,string newPw)
        {
            var packet = PacketFactory.CreateCtrlCmd(
                CommandCode.PARAM_SET_PASSWORD, mn, oldPw);
            packet.Segment.DataSegment.SetValue(DataSegCode.NewPw, newPw);
            SendPacket(mn, packet);
        }

        /// <summary>
        /// 取污染物实时数据
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="pw"></param>
        public void GetRtdData(string mn,string pw)
        {
            var packet = PacketFactory.CreateCtrlCmd(
                CommandCode.DATA_POL_DATA, mn, pw);
            SendPacket(mn, packet);
        }

        /// <summary>
        /// 停止察看污染物实时数据
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="pw"></param>
        public void StopRtdData(string mn, string pw)
        {
            var packet = PacketFactory.CreateCtrlCmd(
                CommandCode.DATA_STOP_POL_DATA, mn, pw);
            SendPacket(mn, packet);
        }

        /// <summary>
        ///  取设备运行状态数据
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="pw"></param>
        public void GetDeviceStatus(string mn, string pw)
        {
            var packet = PacketFactory.CreateCtrlCmd(
                CommandCode.DATA_DEVICE_DATA, mn, pw);
            SendPacket(mn, packet);
        }

        /// <summary>
        ///  停止察看设备运行状态
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="pw"></param>
        public void StopDeviceStatus(string mn, string pw)
        {
            var packet = PacketFactory.CreateCtrlCmd(
                CommandCode.DATA_STOP_DEVICE_DATA, mn, pw);
            SendPacket(mn, packet);
        }

        /// <summary>
        /// 取污染物分钟历史数据 
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="pw"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public void GetMinHis(string mn,string pw,DateTime startTime,DateTime endTime)
        {
            var packet = PacketFactory.CreateCtrlCmd(
               CommandCode.DATA_MINUTE_HIS, mn, pw);
            packet.Segment.DataSegment.SetValue(DataSegCode.BeginTime, startTime.ToString(Config.Config.DateTimeFormat));
            packet.Segment.DataSegment.SetValue(DataSegCode.EndTime, endTime.ToString(Config.Config.DateTimeFormat));
            var seg = SendPacket(mn, packet);
            
        }

        /// <summary>
        /// 取污染物小时历史数据 
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="pw"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public void GetHourHis(string mn, string pw, DateTime startTime, DateTime endTime)
        {
            var packet = PacketFactory.CreateCtrlCmd(
               CommandCode.DATA_HOUR_HIS, mn, pw);
            packet.Segment.DataSegment.SetValue(DataSegCode.BeginTime, 
                startTime.ToString(Config.Config.DateTimeFormat));
            packet.Segment.DataSegment.SetValue(DataSegCode.EndTime, 
                endTime.ToString(Config.Config.DateTimeFormat));
            var seg = SendPacket(mn, packet);

        }

        /// <summary>
        /// 即时采样 
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="pw"></param>
        /// <param name="pollId"></param>
        public void ZeroAjust(string mn, string pw,string pollId)
        {
            var packet = PacketFactory.CreateCtrlCmd(
               CommandCode.CTRL_ZERO_AJUST, mn, pw);
            packet.Segment.DataSegment.PolId = pollId;
            SendPacket(mn, packet);
        }

        /// <summary>
        /// 即时采样 
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="pw"></param>
        /// <param name="pollId"></param>
        public void RtdSample(string mn, string pw, string pollId)
        {
            var packet = PacketFactory.CreateCtrlCmd(
               CommandCode.CTRL_REAL_SAMPLE, mn, pw);
            packet.Segment.DataSegment.PolId = pollId;
            SendPacket(mn, packet);
        }

        /// <summary>
        /// 启动清洗/反吹
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="pw"></param>
        /// <param name="pollId"></param>
        public void Clean(string mn, string pw, string pollId)
        {
            var packet = PacketFactory.CreateCtrlCmd(
               CommandCode.CTRL_CLEAN, mn, pw);
            packet.Segment.DataSegment.PolId = pollId;
            SendPacket(mn, packet);
        }

        /// <summary>
        /// 比对采样 
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="pw"></param>
        /// <param name="pollId"></param>
        public void CompareSample(string mn, string pw, string pollId)
        {
            var packet = PacketFactory.CreateCtrlCmd(
               CommandCode.CTRL_COMPARE_SAMPLE, mn, pw);
            packet.Segment.DataSegment.PolId = pollId;
            SendPacket(mn, packet);
        }

        /// <summary>
        /// 超标留样 
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="pw"></param>
        /// <param name="pollId"></param>
        public void OverSample(string mn, string pw)
        {
            var packet = PacketFactory.CreateCtrlCmd(
               CommandCode.CTRL_OVERFLOW_SAMPLE, mn, pw);
            var segment = SendPacket(mn, packet);
        }

        /// <summary>
        /// 设置采样时间周期 
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="pw"></param>
        public void SetSampleInterval(string mn, string pw,string pollId,DateTime startTime,int interval)
        {
            var packet = PacketFactory.CreateCtrlCmd(
               CommandCode.CTRL_SAMPLE_INTERAL, mn, pw);
            packet.Segment.DataSegment.PolId = pollId;
            packet.Segment.DataSegment.SetValue(DataSegCode.CstartTime, startTime.ToString("HHmmss"));
            packet.Segment.DataSegment.SetValue(DataSegCode.CTime, interval.ToString());
            SendPacket(mn, packet);
        }

        /// <summary>
        /// 提取采样时间周期
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="pw"></param>
        /// <param name="pollId"></param>
        /// <param name="startTime"></param>
        /// <param name="interval"></param>
        public dynamic GetSampleInterval(string mn, string pw, string pollId)
        {
            var packet = PacketFactory.CreateCtrlCmd(
               CommandCode.CTRL_GET_INTERVAL, mn, pw);
            packet.Segment.DataSegment.PolId = pollId;
            var res = SendPacket(mn, packet);

            return new { time = res.DataSegment.GetValue(DataSegCode.CstartTime) ,interval = res.DataSegment.GetValue(DataSegCode.CTime)};
        }

        /// <summary>
        /// 提取出样时间 
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="pw"></param>
        /// <param name="pollId"></param>
        /// <returns></returns>
        public int GetSampleTime(string mn, string pw, string pollId)
        {
            var packet = PacketFactory.CreateCtrlCmd(
               CommandCode.CTRL_GET_SAMPLE_TIME, mn, pw);
            packet.Segment.DataSegment.PolId = pollId;
            var res = SendPacket(mn, packet);
            return res.DataSegment.GetInt(DataSegCode.STime);
        }

        /// <summary>
        /// 提取设备唯一标识
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="pw"></param>
        /// <param name="pollId"></param>
        /// <returns></returns>
        public string GetSN(string mn, string pw, string pollId)
        {
            var packet = PacketFactory.CreateCtrlCmd(
               CommandCode.CTRL_GET_MN, mn, pw);
            packet.Segment.DataSegment.PolId = pollId;
            var res = SendPacket(mn, packet);
            return res.DataSegment.GetValue(pollId+"-SN");
        }
    }
}
