using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Core.Util
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/4/26 14:54:50 
	* 描述： TimeUtil 
	*************************/
    public class TimeUtil
    {
        private static TimeSpan ts;
        //时分秒
        public static string TimeFormatSecond(int second)
        {
            ts = new TimeSpan(0, 0, second);
            return string.Format("{0}:{1}:{2}", ts.Hours.ToString("00"), ts.Minutes.ToString("00"), ts.Seconds.ToString("00"));
        }

        //分秒毫
        public static string TimeFormatMilliseconds(int milliseconds)
        {
            ts = new TimeSpan(0, 0, 0, 0, milliseconds);
            return string.Format("{0}:{1}:{2}", ts.Minutes.ToString("00"), ts.Seconds.ToString("00"), (ts.Milliseconds / 10).ToString("00"));
        }

        /// <summary>
        /// Unix时间戳格式转换为DateTime时间格式
        /// </summary>
        public static DateTime ConvertTimestampToDateTime(int value)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(value + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        public static int ConvertDateTimeToTimestamp(DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
    }
}
