/**************************************************
*文件名：ResultCode
*描   述：
*创建者：lrh
*时间：2018-04-10 10:50:28
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
    /// 执行结果定义
    /// </summary>
    public class ResultCode
    {
        /// <summary>
        /// 执行成功 
        /// </summary>
        public static string SUCCESS = "1";
        /// <summary>
        /// 执行失败，但不知道原因 
        /// </summary>
        public static string UNKNOW_FAIL = "2";
        /// <summary>
        /// 命令请求条件错误 
        /// </summary>
        public static string CMD_CONDITION_FAIL = "3";
        /// <summary>
        /// 通讯超时 
        /// </summary>
        public static string COMM_TIMEOUT = "4";
        /// <summary>
        /// 系统繁忙不能执行 
        /// </summary>
        public static string SYS_BUSY = "5";
        /// <summary>
        /// 系统故障 
        /// </summary>
        public static string SYS_FAIL = "6";
        /// <summary>
        /// 没有数据 
        /// </summary>
        public static string NO_DATA = "100";
    }
}
