/**************************************************
*文件名：Segment
*描   述：
*创建者：lrh
*时间：2018-04-08 15:13:21
*
****************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GNL.Common.Protocol.Domain;

namespace GNL.Common.Protocol
{
    /// <summary>
    /// 数据段
    /// </summary>
    public class Segment
    {
        /// <summary>
        /// 请求编码
        /// </summary>
        public string QN { get { return GetValue(DataCode.QN); } set {SetValue(DataCode.QN,value);} }
        /// <summary>
        /// 系统编码
        /// </summary>
        public string ST { get { return GetValue(DataCode.ST); } set { SetValue(DataCode.ST, value); } }
        /// <summary>
        /// 命令编码
        /// </summary>
        public string CN { get { return GetValue(DataCode.CN); } set { SetValue(DataCode.CN, value); } }
        /// <summary>
        /// 访问密码
        /// </summary>
        public string PW { get { return GetValue(DataCode.PW); } set { SetValue(DataCode.PW, value); } }
        /// <summary>
        /// 设备唯一标志
        /// </summary>
        public string MN { get { return GetValue(DataCode.MN); } set { SetValue(DataCode.MN, value); } }
        public string Flag { get { return GetValue(DataCode.FLAG); } set { SetValue(DataCode.FLAG, value); } }

        public int PNUM { get { return GetInt(DataCode.PNUM); } set { SetValue(DataCode.PNUM, value.ToString("D9")); } }

        public int PNO { get { return GetInt(DataCode.PNO); } set { SetValue(DataCode.PNO, value.ToString("D8")); } }
        public bool ACK
        {
            set
            {
                var flag = int.Parse(Flag);
                flag &= 0x1;
                Flag = flag.ToString();
            }
            get
            {
                if (!string.IsNullOrEmpty(Flag))
                {
                    int flag = int.Parse(Flag);
                    return (flag & 0x1) == 0x1;
                }
                return false;
            }
        }

        public bool Divide
        {
            set
            {
                var flag = int.Parse(Flag);
                flag &= 0x2;
                Flag = flag.ToString();
            }
            get
            {
                if (!string.IsNullOrEmpty(Flag))
                {
                    int flag = int.Parse(Flag);
                    return (flag & 0x2) == 0x2;
                }
                return false;
            }
        }

        private Dictionary<string, string> headerDict = new Dictionary<string, string>();
        public Dictionary<string, string> HeaderDict { get { return headerDict; } set { headerDict = value; } }
        public string DataSegStr { get; set; }

        public DataSegment DataSegment { get; set; }

        /// <summary>
        /// 初始化函数
        /// </summary>
        public Segment()
        {
            ST = Config.Config.ST;
            Flag = Config.Config.Version;
            DataSegment = new DataSegment();
        }

        public Segment(string qn,string cn,string mn,string pw)
        {
            //QN = qn;
            ST = Config.Config.ST;
            CN = cn;
            PW = pw;
            MN = mn;
            Flag = Config.Config.Version;
        }

        /// <summary>
        /// 序列化数据转成数据包
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Segment FromData(byte[] buf,int offset,int length)
        {
            string str = Encoding.ASCII.GetString(buf,offset,length);

            return FromData(str);
        }

        public static Segment FromData(string str)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            var dataStr = string.Empty;
            if (!string.IsNullOrEmpty(str))
            {
                //剪切参数
                if (str.IndexOf("&&") > 0)
                {
                    dataStr = str.Substring(str.IndexOf("&&"), str.LastIndexOf("&&") + 2 - str.IndexOf("&&"));
                }
                str = str.Replace(dataStr, "");
                str = str.Replace("CP=", "");
                dataStr = dataStr.Trim('&');

                var kvs = str.Split(';');

                foreach (var kv in kvs)
                {
                    if (kv.Trim() != string.Empty)
                    {
                        var keyvalue = kv.Split('=');
                        data.Add(keyvalue[0], keyvalue[1]);
                    }
                }
            }
            Segment segment = new Segment()
            {
                HeaderDict = data,
                DataSegStr = dataStr,
            };
            segment.DataSegment = DataSegment.FromStr(dataStr);
            return segment;
        }

        public string GetValue(string dataCode)
        {
            if(HeaderDict.ContainsKey(dataCode))
                return HeaderDict[dataCode];
            return string.Empty;
        }

        public decimal? GetDecimal(string dataCode)
        {
            try
            {
                return decimal.Parse(dataCode);
            }catch{
                return null;
            }
        }

        public int GetInt(string dataCode)
        {
            try
            {
                return int.Parse(GetValue(dataCode));
            }
            catch
            {
                return 0;
            }
        }

        public void SetValue(string dataCode, string value)
        {
            if (HeaderDict.ContainsKey(dataCode))
            {
                HeaderDict[dataCode] = value;
            }
            else
            {
                HeaderDict.Add(dataCode,value);
            }
        }

        public static byte[] ToData(Segment segment)
        {
            return ToArray(segment.HeaderDict);
        }

        public byte[] ToData()
        {
            var header = ToArray(HeaderDict);
            //var data = ToArray(DataDict);
            return header;
        }
        public string ToDataStr()
        {
            var reqString = string.Empty;
            foreach (var d in this.HeaderDict)
            {
                var str = string.Format("{0}={1};", d.Key, d.Value);
                reqString += str;
            }
            if (DataSegment != null)
            {
                reqString = string.Format("{0};CP=&&{1}&&", reqString.Trim(';'), DataSegment.ToDataStr());
            }
            else
            {
                reqString += "&&";
            }
            return reqString;
        }

        public static byte[] ToArray(Dictionary<string, string> data)
        {
            var reqString = string.Empty;
            foreach (var d in data)
            {
                var str = string.Format("{0}={1};", d.Key, d.Value);
                reqString += str;
            }
            return Encoding.ASCII.GetBytes(reqString);
        }

        public string CheckSegment()
        {
            DateTime qn;
            if (string.IsNullOrEmpty(QN)
                || !DateTime.TryParseExact(QN, "yyyyMMddHHmmssfff", null, System.Globalization.DateTimeStyles.None, out qn))
            {
                return QnCode.QNError;
            }
            else if (string.IsNullOrEmpty(ST))
            {
                return QnCode.STError;
            }
            else if (string.IsNullOrEmpty(MN))
            {
                return QnCode.MNError;
            }
            else if (string.IsNullOrEmpty(PW))
            {
                return QnCode.PWError;
            }
            else if (string.IsNullOrEmpty(Flag))
            {
                return QnCode.FlagError;
            }
            else
            {
                return QnCode.PerpareExe;
            }
        }
    }
}
