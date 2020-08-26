/**************************************************
*文件名：DataSegment
*描   述：
*创建者：lrh
*时间：2018-04-10 11:34:33
*
****************************************************/
using GNL.Common.Protocol.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNL.Common.Protocol
{
    /// <summary>
    /// 数据区
    /// </summary>
    public class DataSegment
    {
        public static string HEADER = "&&";
        public DateTime? SystemTime { get; set; }
        public string QnRtn { get { return GetValue(DataSegCode.QnRtn); } set { SetValue(DataSegCode.QnRtn, value); } }
        public string ExeRtn { get { return GetValue(DataSegCode.ExeRtn); } set { SetValue(DataSegCode.ExeRtn, value); } }
        public int RtdInterval
        {
            get { return GetInt(DataSegCode.RtdInterval); }
            set
            {
                SetValue(DataSegCode.RtdInterval, value.ToString());
            }
        }
        public DateTime? DataTime
        {
            get { return GetDateTime(DataSegCode.DataTime, "yyyyMMddHHmmss"); }
            set { SetValue(DataSegCode.DataTime, value.Value.ToString("yyyyMMddHHmmss")); }
        }
        public DateTime? RestartTime
        {
            get { return GetDateTime("RestartTime", "yyyyMMddHHmmss"); }
            set { SetValue("RestartTime", value.Value.ToString("yyyyMMddHHmmss")); }
        }


        public string PolId { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string NewPW { get; set; }
        public string OverTime { get; set; }
        public string ReCount { get; set; }
        public string VaseNo { get; set; }
        public string CstartTime { get; set; }
        public string Ctime { get; set; }
        public string InfoId { get; set; }

        private Dictionary<string, string> dataDict = new Dictionary<string, string>();
        public Dictionary<string, string> DataDict { get { return dataDict; } set { dataDict = value; } }

        public List<string> PollList = new List<string>();
        public List<string> DataItemList = new List<string>();

        public decimal GetRtd(string polName)
        {
            return GetDecimal(polName+"-Rtd");
        }

        public string GetRtdString(string polName)
        {
            return GetValue(polName + "-Rtd");
        }

        public decimal GetZsRt(string polName)
        {
            return GetDecimal(polName + "-ZsRt");
        }

        public decimal GetAvg(string polName)
        {
            return GetDecimal(polName + "-Avg");
        }

        public string GetFlag(string polName)
        {
            return GetValue(polName + "-Flag");
        }

        public string GetValue(string dataCode)
        {
            if (DataDict.ContainsKey(dataCode))
                return DataDict[dataCode];
            return string.Empty;
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

        public decimal GetDecimal(string dataCode)
        {
            try
            {
                return decimal.Parse(GetValue(dataCode));
            }
            catch
            {
                return -99;
            }
        }

        public DateTime? GetDateTime(string dataCode,string format)
        {
            try
            {
                return DateTime.ParseExact(GetValue(dataCode),format,null);
            }
            catch
            {
                return null;
            }
        }

        public void SetRtd(string pollId,string value)
        {
            SetValue(pollId + "-Rtd", value);
        }

        public void SetFlag(string pollId, string value)
        {
            SetValue(pollId + "-Flag", value);
        }

        public void SetValue(string dataCode, string value)
        {
            if (DataDict.ContainsKey(dataCode))
            {
                DataDict[dataCode] = value;
            }
            else
            {
                DataDict.Add(dataCode, value);
            }
        }

        public static DataSegment FromStr(string dataStr)
        {
            DataSegment segment = new DataSegment();

            if (!string.IsNullOrEmpty(dataStr))
            {
                var datas = dataStr.Split(';');
                List<string> keyValues = new List<string>();
                foreach (var data in datas)
                {
                    if (data.Contains(","))
                    {
                        var items = data.Split(',');
                        if (items[0].Contains('-')){
                            var pollId = items[0].Split('-')[0];

                            if (!segment.PollList.Contains(pollId))
                            {
                                segment.PollList.Add(pollId);
                            }
                            segment.DataItemList.Add(data);
                        }
                        
                        keyValues.AddRange(items);
                    }
                    else
                    {
                        if (data.Contains('-'))
                        {
                            var pollId = data.Split('-')[0];

                            if (!segment.PollList.Contains(pollId))
                            {
                                segment.PollList.Add(pollId);
                            }
                            segment.DataItemList.Add(data);
                        }
                        keyValues.Add(data);
                    }
                }
                keyValues.ForEach(keyvalue => {
                    if (keyvalue.Trim() != string.Empty) {
                        segment.SetValue(keyvalue.Split('=')[0], keyvalue.Split('=')[1]);
                    }});
            }
            return segment;
        }

        public string ToDataStr()
        {
            var needToGroup = DataDict.Keys.Where(k => k.Contains("-")).Select(k => k.Split('-')[0]).Distinct().ToList();
            Dictionary<string, string> groupList = new Dictionary<string, string>();
            needToGroup.ForEach(key => groupList.Add(key, ""));
            string resStr = string.Empty;
            foreach(var kv in this.DataDict)
            {
                if (kv.Key.Contains("-"))
                {
                    groupList[kv.Key.Split('-')[0]] += string.Format(",{0}={1}",kv.Key, kv.Value); 
                }
                else
                {
                    resStr += string.Format("{0}={1};", kv.Key, kv.Value);
                }
            }
            foreach(var kv in groupList)
            {
                resStr += string.Format("{0};", kv.Value.Trim(','));
            }
            return resStr.Trim(';');
        }
    }
}
