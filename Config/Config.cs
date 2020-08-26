/**************************************************
*文件名：Config
*描   述：
*创建者：lrh
*时间：2018-5-17 10:17:05
*
****************************************************/
using GNL.Common.Protocol.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GNL.Common.Protocol.Config
{
    public class Config
    {
        public static string ST = SystemCode.AirQuality;
        public static string Version = "4";
        public static string[] ContainST = { SystemCode.AirQuality, SystemCode.TVOC };
        public static int Timeout = 10;
        public static int ReCount = 3;
        /// <summary>
        /// 是否返回请求响应
        /// </summary>
        public static bool ReturnQN = true;

        public static string DateTimeFormat = "yyyyMMddHHmmss";
    }
}
