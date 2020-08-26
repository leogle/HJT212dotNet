/**************************************************
*文件名：CommandCode
*描   述：
*创建者：lrh
*时间：2018-04-10 10:54:24
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
    /// 命令代码
    /// </summary>
    public class CommandCode
    {
        /// <summary>
        /// 设置超时时间及重发次数
        /// </summary>
        public const string INIT_SET = "1000";
        #region parameter command
        /// <summary>
        /// 提取现场机时间 
        /// </summary>
        public const string PARAM_FATCH_TIME = "1011";
        public const string PARAM_UP_TIME = "1011";
        /// <summary>
        /// 设置现场机时间 
        /// </summary>
        public const string PARAM_SET_TIME = "1012";
        /// <summary>
        /// 现场机时间校准请求 
        /// </summary>
        public const string PARAM_AJUST_TIME = "1013";
        /// <summary>
        /// 
        /// </summary>
        public const string PARAM_GET_REALTIME_INTERVAL = "1061";
        public const string PARAM_SET_REALTIME_INTERVAL = "1062";
        public const string PARAM_GET_MINUTE_INTERVAL = "1063";
        public const string PARAM_SET_MINUTE_INTERVAL = "1064";
        public const string PARAM_SET_PASSWORD = "1072";
        #endregion

        #region data command
        /// <summary>
        /// 取污染物实时数据
        /// </summary>
        public const string DATA_POL_DATA = "2011";
        /// <summary>
        /// 停止察看污染物实时数据 
        /// </summary>
        public const string DATA_STOP_POL_DATA = "2012";

        public const string DATA_DEVICE_DATA = "2021";
        public const string DATA_STOP_DEVICE_DATA = "2022";

        public const string DATA_DAY_HIS = "2031";
        public const string DATA_DAY_TIME = "2041";
        public const string DATA_MINUTE_HIS = "2051";
        /// <summary>
        /// 取污染物小时历史数据
        /// </summary>
        public const string DATA_HOUR_HIS = "2061";
        public const string DATA_START_TIME = "2081";

        #endregion

        #region control command
        /// <summary>
        /// 零点校准量程校准 
        /// </summary>
        public const string CTRL_ZERO_AJUST = "3011";
        /// <summary>
        /// 即时采样 
        /// </summary>
        public const string CTRL_REAL_SAMPLE = "3012";
        /// <summary>
        /// 启动清洗/反吹 
        /// </summary>
        public const string CTRL_CLEAN = "3013";
        /// <summary>
        /// 比对采样
        /// </summary>
        public const string CTRL_COMPARE_SAMPLE = "3014";
        /// <summary>
        /// 超标留样 
        /// </summary>
        public const string CTRL_OVERFLOW_SAMPLE = "3015";
        /// <summary>
        /// 设置采样时间周期 
        /// </summary>
        public const string CTRL_SAMPLE_INTERAL = "3016";
        /// <summary>
        /// 提取采样时间周期 
        /// </summary>
        public const string CTRL_GET_INTERVAL = "3017";
        /// <summary>
        /// 提取出样时间 
        /// </summary>
        public const string CTRL_GET_SAMPLE_TIME = "3018";
        /// <summary>
        /// 提取设备唯一标识 
        /// </summary>
        public const string CTRL_GET_MN = "3019";
        /// <summary>
        /// 提取现场机信息 
        /// </summary>
        public const string CTRL_GET_INFO = "3020";
        /// <summary>
        /// 设置现场机参数
        /// </summary>
        public const string CTRL_SET_PARAM = "3021";

        /// <summary>
        /// 修改工况参数
        /// </summary>
        public const string CTRL_SET_MICRO_PARAMS= "3033";
        /// <summary>
        /// 查询工况参数
        /// </summary>
        public const string CTRL_GET_MICRO_PARAMS = "3031";

        public const string CTRL_GET_MICRO_PARAMS_RES = "3032";
        #endregion

        #region interactive command
        /// <summary>
        /// 请求应答
        /// </summary>
        public const string INT_REQ_ACK = "9011";
        /// <summary>
        /// 执行结果  
        /// </summary>
        public const string INT_RN = "9012";
        /// <summary>
        /// 通知应答
        /// </summary>
        public const string INT_NOTIFY_ACK = "9013";
        /// <summary>
        /// 数据应答 
        /// </summary>
        public const string INT_DATA_ACK = "9014";
        #endregion
    }
}
