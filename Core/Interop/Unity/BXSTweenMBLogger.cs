#if UNITY_5_6_OR_NEWER
using System;
using System.Reflection;
using Debug = UnityEngine.Debug;

namespace BX.Tweening.Interop
{
    /// <summary>
    /// Logger that uses <see cref="Console.WriteLine"/> for it's base.
    /// </summary>
    public sealed class BXSTweenMBLogger : IBXSTweenLogger
    {
        public BXSTweenMBLogger()
        { }

#if UNITY_EDITOR || DEBUG
        private IBXSTweenLogger.Verbosity m_LogVerbosity = IBXSTweenLogger.Verbosity.Warn;
#else
        private IBXSTweenLogger.Verbosity m_LogVerbosity = IBXSTweenLogger.Verbosity.Error;
#endif
        public IBXSTweenLogger.Verbosity LogVerbosity { get => m_LogVerbosity; set => m_LogVerbosity = value; }

        public void Info(object message)
        {
            if (m_LogVerbosity > IBXSTweenLogger.Verbosity.Info)
            {
                return;
            }

            Debug.Log(message);
        }
        public void Warn(object message)
        {
            if (m_LogVerbosity > IBXSTweenLogger.Verbosity.Warn)
            {
                return;
            }

            Debug.LogWarning(message);
        }
        public void Error(object message)
        {
            if (m_LogVerbosity > IBXSTweenLogger.Verbosity.Error)
            {
                return;
            }

            Debug.LogError(message);
        }

        public bool Exception(Exception exception)
        {
            if (m_LogVerbosity > IBXSTweenLogger.Verbosity.Exception)
            {
                return false;
            }

            if (exception == null)
            {
                Error("[BXSTweenUnityLogger::LogException] Given exception is null.");
                return false;
            }

            Debug.LogException(exception);
            return true;
        }

        public bool Exception(string prependMessage, Exception exception)
        {
            if (m_LogVerbosity > IBXSTweenLogger.Verbosity.Exception)
            {
                return false;
            }

            const string FieldName = "_message";
            FieldInfo messageField = exception.GetType().GetField(FieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (messageField != null)
            {
                messageField.SetValue(exception, exception.Message.Insert(0, prependMessage));
            }
            else
            {
                // Or handle it however you like. But trust me, there is indeed `_message` inside `System.Exception`.
                Error($"[BXSTweenUnityLogger::LogException] There's no such field as '{FieldName}' inside exception '{prependMessage}{exception}'.\n");
            }

            // Unity's default logger is somewhat of a blackbox, requiring this workaround. If you know something else more elegant open PR/issue.
            Debug.LogException(exception);
            return true;
        }
    }
}
#endif
