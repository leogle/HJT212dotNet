/**************************************************
*文件名：DataFlag
*描   述：
*创建者：lrh
*时间：2018-04-12 12:02:30
*
****************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNL.Common.Protocol.Domain
{
    /// <summary>
    /// 据标记表
    /// </summary>
    public class DataFlag
    {
        /// <summary>
        /// 在线监控（监测）仪器仪表工作正常
        /// </summary>
        public static string Normal = "N";
        /// <summary>
        /// 在线监控（监测）仪器仪表停运 
        /// </summary>
        public static string Fail = "F";
        /// <summary>
        /// 在线监控（监测）仪器仪表处于维护期间产生的数据 
        /// </summary>
        public static string Mantain = "M";
        /// <summary>
        /// 手工输入的设定值 
        /// </summary>
        public static string Set = "S";
        /// <summary>
        /// 在线监控（监测）仪器仪表故障 
        /// </summary>
        public static string Down = "D";
        /// <summary>
        /// 在线监控（监测）仪器仪表处于校准状态 
        /// </summary>
        public static string Ajust = "C";
        /// <summary>
        /// 在线监控（监测）仪器仪表采样数值超过测量上限
        /// </summary>
        public static string Top = "T";
        /// <summary>
        /// 在线监控（监测）仪器仪表与数采仪通讯异常
        /// </summary>
        public static string CommFail = "B";
    }
}
