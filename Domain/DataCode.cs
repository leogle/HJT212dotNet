/**************************************************
*文件名：DataCode
*描   述：
*创建者：lrh
*时间：2018-04-10 10:43:33
*
****************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNL.Common.Protocol.Domain
{
    internal class DataCode
    {
        /// <summary>
        /// 请求编码 QN
        /// </summary>
        public static string QN = "QN";
        /// <summary>
        /// 系统编码 ST
        /// </summary>
        public static string ST = "ST";
        /// <summary>
        /// 命令编码 CN
        /// </summary>
        public static string CN = "CN";
        /// <summary>
        /// 访问密码
        /// </summary>
        public static string PW = "PW";
        /// <summary>
        /// 设备唯一标识 MN 
        /// </summary>
        public static string MN = "MN";
        /// <summary>
        /// 拆分包及应答标志
        /// </summary>
        public static string FLAG = "Flag";
        /// <summary>
        /// 总包数PNUM 
        /// </summary>
        public static string PNUM = "PNUM";
        /// <summary>
        /// 包号PNO
        /// </summary>
        public static string PNO = "PNO";
        /// <summary>
        /// 指令参数 CP 
        /// </summary>
        public static string CP = "CP";
    }
}
