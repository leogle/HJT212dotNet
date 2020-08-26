/**************************************************
*文件名：QnCode
*描   述：
*创建者：lrh
*时间：2018-04-12 11:59:17
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
    /// 请求返回代码
    /// </summary>
    public class QnCode
    {
        public static string PerpareExe = "1";
        public static string ReqDeny = "2";
        public static string PWError = "3";
        public static string MNError = "4";
        public static string STError = "5";
        public static string FlagError = "6";
        public static string QNError = "7";
        public static string CNError = "8";
        public static string CRCError = "9";
        public static string UnkonwError = "100";
    }
}
