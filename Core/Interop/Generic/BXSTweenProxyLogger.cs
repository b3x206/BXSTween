using System;

namespace BX.Tweening.Interop
{
    /// <summary>
    /// Proxy based event logger.
    /// <br>Logging handler "should" be a method that doesn't throw exceptions.</br>
    /// </summary>
    public sealed class BXSTweenProxyLogger : IBXSTweenLogger
    {
        public enum RedirectBehaviour
        {
            /// <summary>
            /// Do nothing.
            /// </summary>
            None,
            /// <summary>
            /// Use other logger callbacks if the target log level is unavailable.
            /// </summary>
            UseOther,
            /// <summary>
            /// Let the usage throw a <see cref="NullReferenceException"/>
            /// </summary>
            Throw
        }

#if DEBUG
        private IBXSTweenLogger.Verbosity m_LogVerbosity = IBXSTweenLogger.Verbosity.Warn;
#else
        private IBXSTweenLogger.Verbosity m_LogVerbosity = IBXSTweenLogger.Verbosity.Error;
#endif
        public IBXSTweenLogger.Verbosity LogVerbosity { get => m_LogVerbosity; set => m_LogVerbosity = value; }
        /// <summary>
        /// The redirection behaviour. Note that all redirects descend to <see cref="OnLog"/>, if that value throws then everything throws.
        /// </summary>
        public RedirectBehaviour redirectBehaviour = RedirectBehaviour.UseOther;

        private Action<object> _onLog;
        public Action<object> OnLog
        {
            get => _onLog;
            set => _onLog = value ?? throw new ArgumentNullException(nameof(value));
        }
        private Action<object> _onWarn;
        public Action<object> OnWarn
        {
            get => _onWarn;
            set => _onWarn = value ?? throw new ArgumentNullException(nameof(value));
        }
        private Action<object> _onError;
        public Action<object> OnError
        {
            get => _onError;
            set => _onError = value ?? throw new ArgumentNullException(nameof(value));
        }
        private Func<string, Exception, bool> _onException;
        public Func<string, Exception, bool> OnException
        {
            get => _onException;
            set => _onException = value ?? throw new ArgumentNullException(nameof(value));
        }

        public BXSTweenProxyLogger(Action<object> info)
        {
            OnLog = info;
        }
        public BXSTweenProxyLogger(Action<object> info, Action<object> warn, Action<object> error)
        {
            OnLog = info;
            OnWarn = warn;
            OnError = error;
        }
        public BXSTweenProxyLogger(Action<object> info, Action<object> warn, Action<object> error, Func<string, Exception, bool> exception)
        {
            OnLog = info;
            OnWarn = warn;
            OnError = error;
            OnException = exception;
        }

        void IBXSTweenLogger.Info(object message)
        {
            OnLog(message);
        }
        void IBXSTweenLogger.Warn(object message)
        {
            switch (redirectBehaviour)
            {
                case RedirectBehaviour.None:
                    OnWarn?.Invoke(message);
                    break;
                case RedirectBehaviour.UseOther:
                    if (OnWarn != null)
                    {
                        OnWarn(message);
                        return;
                    }

                    OnLog(message);
                    return;

                default:
                case RedirectBehaviour.Throw:
                    OnWarn(message);
                    break;
            }
        }
        void IBXSTweenLogger.Error(object message)
        {
            switch (redirectBehaviour)
            {
                case RedirectBehaviour.None:
                    OnError?.Invoke(message);
                    break;
                case RedirectBehaviour.UseOther:
                    if (OnError != null)
                    {
                        OnError(message);
                        return;
                    }
                    if (OnWarn != null)
                    {
                        OnWarn(message);
                        return;
                    }

                    OnLog(message);
                    return;

                default:
                case RedirectBehaviour.Throw:
                    OnError(message);
                    break;
            }
        }
        bool IBXSTweenLogger.Exception(Exception exception) { return ((IBXSTweenLogger)this).Exception(null, exception); }
        bool IBXSTweenLogger.Exception(string prependMessage, Exception exception)
        {
            switch (redirectBehaviour)
            {
                case RedirectBehaviour.None:
                    return OnException?.Invoke(prependMessage, exception) ?? false;
                case RedirectBehaviour.UseOther:
                    if (OnException != null)
                    {
                        return OnException(prependMessage, exception);
                    }
                    if (OnError != null)
                    {
                        OnError(prependMessage);
                        OnError(exception);
                        return true;
                    }
                    if (OnWarn != null)
                    {
                        OnWarn(prependMessage);
                        OnWarn(exception);
                        return true;
                    }

                    OnError(prependMessage);
                    OnLog(exception);
                    return true;

                default:
                case RedirectBehaviour.Throw:
                    return OnException(prependMessage, exception);
            }
        }
    }
}
