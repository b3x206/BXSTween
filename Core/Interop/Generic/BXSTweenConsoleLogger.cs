using System;
using System.Diagnostics;

namespace BX.Tweening.Interop
{
    /// <summary>
    /// Logger that uses <see cref="Console.WriteLine"/> for it's base.
    /// </summary>
    public sealed class BXSTweenConsoleLogger : IBXSTweenLogger
    {
        private const string InfoPrefix      = "[INFO] ";
        private const string WarnPrefix      = "[WARN] ";
        private const string ErrorPrefix     = "[!ERR] ";
        private const string ExceptionPrefix = "[!!EX] ";

#if DEBUG
        private IBXSTweenLogger.Verbosity m_LogVerbosity = IBXSTweenLogger.Verbosity.Warn;
#else
        private IBXSTweenLogger.Verbosity m_LogVerbosity = IBXSTweenLogger.Verbosity.Error;
#endif
        public IBXSTweenLogger.Verbosity LogVerbosity { get => m_LogVerbosity; set => m_LogVerbosity = value; }

        /// <summary>
        /// Whether to print the <see cref="StackTrace"/> on a failing log method.
        /// <br>This is a redundancy option.</br>
        /// </summary>
        public bool printStackTraceOnFail = true;

        public BXSTweenConsoleLogger()
        { }

        public void Info(object message)
        {
            if (m_LogVerbosity > IBXSTweenLogger.Verbosity.Info)
            {
                return;
            }

            Console.WriteLine($"{InfoPrefix}{message}");
        }
        public void Warn(object message)
        {
            if (m_LogVerbosity > IBXSTweenLogger.Verbosity.Warn)
            {
                return;
            }

            Console.WriteLine($"{WarnPrefix}{message}");
        }
        public void Error(object message)
        {
            if (m_LogVerbosity > IBXSTweenLogger.Verbosity.Error)
            {
                return;
            }

            Console.WriteLine($"{ErrorPrefix}{message}");
        }

        public bool Exception(Exception exception)
        {
            if (m_LogVerbosity > IBXSTweenLogger.Verbosity.Exception)
            {
                return false;
            }

            if (exception == null)
            {
                // This method may be used through https://learn.microsoft.com/en-us/dotnet/api/system.appdomain.unhandledexception?view=net-9.0,
                // so we will not throw. Instead return false if logging fails. We can optionally get stack trace where null was passed.

                if (printStackTraceOnFail)
                {
                    try
                    {
                        Error($"[ConsoleLogger::LogException] Given exception is null. StackTrace {new StackTrace(2, false)}");
                    }
                    catch (Exception innerException)
                    {
                        // Directly do the log behaviour, if this exception is null too we are in bigger problem than logger..
                        Console.WriteLine($"{ExceptionPrefix}{innerException.Message}\n    {innerException.StackTrace}");
                    }
                }
                else
                {
                    Error("[ConsoleLogger::LogException] Given exception is null.");
                }

                return false;
            }

            Console.WriteLine($"{ExceptionPrefix}{exception.Message}\n    {exception.StackTrace}");
            return true;
        }

        public bool Exception(string prependMessage, Exception exception)
        {
            if (m_LogVerbosity > IBXSTweenLogger.Verbosity.Exception)
            {
                return false;
            }

            if (exception == null)
            {
                if (printStackTraceOnFail)
                {
                    try
                    {
                        Error($"[ConsoleLogger::LogException] Given exception is null. StackTrace {new StackTrace(2, false)}");
                    }
                    catch (Exception innerException)
                    {
                        // Directly do the log behaviour, if this exception is null too we are in bigger problem than logger..
                        Console.WriteLine($"{ExceptionPrefix}{innerException.Message}\n    {innerException.StackTrace}");
                    }
                }
                else
                {
                    Error("[ConsoleLogger::LogException] Given exception is null.");
                }

                return false;
            }

            Console.WriteLine($"{ExceptionPrefix}{prependMessage} {exception.Message}\n    {exception.StackTrace}");
            return true;
        }
    }
}