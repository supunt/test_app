using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Peercore.Email.Common
{
    public class RegexParser
    {
        //---------------------------------------------------------------------------------
        public static DateTime? ParseDateString(string regexStr, string matchThisString)
        {
            Regex rOD = new Regex(regexStr);
            Match match = rOD.Match(matchThisString);
            
            if (match.Groups.Count == 0)
            {
               return null;
            }
            else
            {
                DateTime dt = new DateTime();
                if (DateTime.TryParseExact(match.Groups[1].Value, "dd/MM/yy", null, DateTimeStyles.None, out dt) == false)
                    return null;

                return dt;
            }
        }
        //---------------------------------------------------------------------------------
        public static string ParseString(string regexStr, string matchThisString)
        {
            Regex rOD = new Regex(regexStr);
            Match match = rOD.Match(matchThisString);

            if (match.Groups.Count == 0)
            {
               return "";
            }
            else
            {
                return match.Groups[1].Value;
            }
        }
        //---------------------------------------------------------------------------------
    }
}
