using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GNL.Common.Protocol.Extension
{
    public static class StringExtension
    {
        public static string Substring(this string str, int length, string endOf)
        {
            if (!string.IsNullOrEmpty(str) && ((length > 0) && (length < str.Length)))
            {
                return (str.Substring(0, length) + endOf);
            }
            return str;
        }

        public static string FormatTo(this string str, params object[] args)
        {
            return string.Format(str, args);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string StripHTML(this string strHtml)
        {
            string[] aryReg =
        {
          @"<script[^>]*?>.*?</script>",@"<(\/\s*)?!?((\w+:)?\w+)(\w+(\s*=?\s*(([""'])(\\[""'tbnr]|[^\7])*?\7|\w+)|.{0})|\s)*?(\/\s*)?>", @"([\r\n])[\s]+", @"&(quot|#34);", @"&(amp|#38);", @"&(lt|#60);", @"&(gt|#62);", @"&(nbsp|#160);", @"&(iexcl|#161);", @"&(cent|#162);", @"&(pound|#163);",@"&(copy|#169);", @"&#(\d+);", @"-->", @"<!--.*\n"
        };
            string[] aryRep =
        {
          "", "", "", "\"", "&", "<", ">", "   ", "\xa1",  //chr(161),
          "\xa2",  //chr(162),
          "\xa3",  //chr(163),
          "\xa9",  //chr(169),
          "", "\r\n", ""
        };
            string newReg = aryReg[0];
            string strOutput = strHtml;
            for (int i = 0; i < aryReg.Length; i++)
            {
                Regex regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
                strOutput = regex.Replace(strOutput, aryRep[i]);
            }
            strOutput.Replace("<", "");
            strOutput.Replace(">", "");
            strOutput.Replace("\r\n", "");
            return strOutput;
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 从字符串结尾处起向左获取LEN个字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len">要获取的字符长度</param>
        /// <returns></returns>
        public static string Right(this string str, int len)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (str.Length < len)
            {
                throw new ArgumentException("len argument can not be bigger than given string's length!");
            }

            return str.Substring(str.Length - len, len);
        }

        public static string CharAt(this string s, int index)
        {
            if ((index >= s.Length) || (index < 0))
                return "";
            return s.Substring(index, 1);
        }

        public static T ToEnum<T>(this string str) where T : struct
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            return (T)Enum.Parse(typeof(T), str);
        }

        public static DateTime ToDateTime(this string str,string format)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }
            DateTime datetime;
            if( DateTime.TryParseExact(str,format,null, System.Globalization.DateTimeStyles.None,out datetime))
            {
                return datetime;
            }else
            {
                throw new FormatException("str");
            }

        }

        public static T ToEnum<T>(this string str, bool ignoreCase) where T : struct
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            return (T)Enum.Parse(typeof(T), str, ignoreCase);
        }
    }
}
