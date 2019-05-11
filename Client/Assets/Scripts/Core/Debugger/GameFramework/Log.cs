using System;
using System.Diagnostics;

namespace GameFramework
{
    public static class Log
    {
        public interface ILogHelper
        {
            void Log(LogLevel level, object message);
        }

        private static Log.ILogHelper s_LogHelper;

        public static void SetLogHelper(Log.ILogHelper logHelper)
        {
            Log.s_LogHelper = logHelper;
        }

        [Conditional("DEBUG")]
        public static void Debug(object message)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Debug, message);
        }

        [Conditional("DEBUG")]
        public static void Debug(string message)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Debug, message);
        }

        [Conditional("DEBUG")]
        public static void Debug(string format, object arg0)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Debug, string.Format(format, arg0));
        }

        [Conditional("DEBUG")]
        public static void Debug(string format, object arg0, object arg1)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Debug, string.Format(format, arg0, arg1));
        }

        [Conditional("DEBUG")]
        public static void Debug(string format, object arg0, object arg1, object arg2)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Debug, string.Format(format, arg0, arg1, arg2));
        }

        [Conditional("DEBUG")]
        public static void Debug(string format, params object[] args)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Debug, string.Format(format, args));
        }

        public static void Info(object message)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Info, message);
        }

        public static void Info(string message)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Info, message);
        }

        public static void Info(string format, object arg0)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Info, string.Format(format, arg0));
        }

        public static void Info(string format, object arg0, object arg1)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Info, string.Format(format, arg0, arg1));
        }

        public static void Info(string format, object arg0, object arg1, object arg2)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Info, string.Format(format, arg0, arg1, arg2));
        }

        public static void Info(string format, params object[] args)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Info, string.Format(format, args));
        }

        public static void Warning(object message)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Warning, message);
        }

        public static void Warning(string message)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Warning, message);
        }

        public static void Warning(string format, object arg0)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Warning, string.Format(format, arg0));
        }

        public static void Warning(string format, object arg0, object arg1)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Warning, string.Format(format, arg0, arg1));
        }

        public static void Warning(string format, object arg0, object arg1, object arg2)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Warning, string.Format(format, arg0, arg1, arg2));
        }

        public static void Warning(string format, params object[] args)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Warning, string.Format(format, args));
        }

        public static void Error(object message)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Error, message);
        }

        public static void Error(string message)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Error, message);
        }

        public static void Error(string format, object arg0)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Error, string.Format(format, arg0));
        }

        public static void Error(string format, object arg0, object arg1)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Error, string.Format(format, arg0, arg1));
        }

        public static void Error(string format, object arg0, object arg1, object arg2)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Error, string.Format(format, arg0, arg1, arg2));
        }

        public static void Error(string format, params object[] args)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Error, string.Format(format, args));
        }

        public static void Fatal(object message)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Fatal, message);
        }

        public static void Fatal(string message)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Fatal, message);
        }

        public static void Fatal(string format, object arg0)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Fatal, string.Format(format, arg0));
        }

        public static void Fatal(string format, object arg0, object arg1)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Fatal, string.Format(format, arg0, arg1));
        }

        public static void Fatal(string format, object arg0, object arg1, object arg2)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Fatal, string.Format(format, arg0, arg1, arg2));
        }

        public static void Fatal(string format, params object[] args)
        {
            if (Log.s_LogHelper == null)
            {
                return;
            }
            Log.s_LogHelper.Log(LogLevel.Fatal, string.Format(format, args));
        }
    }
}
