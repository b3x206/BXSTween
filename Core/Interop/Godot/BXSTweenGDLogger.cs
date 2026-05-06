#if GODOT
using Godot;
using System;

namespace BX.Tweening.Interop
{
    /// <summary>
    /// BXSTween Logger for godot engine.
    /// </summary>
    public sealed class BXSTweenGDLogger : IBXSTweenLogger
    {
#if DEBUG
        public IBXSTweenLogger.Verbosity LogVerbosity { get; set; } = IBXSTweenLogger.Verbosity.Warn;
#else
        public IBXSTweenLogger.Verbosity LogVerbosity { get; set; } = IBXSTweenLogger.Verbosity.Error;
#endif

        public void Info(object message)
        {
            if (LogVerbosity > IBXSTweenLogger.Verbosity.Info)
            {
                return;
            }

            GD.Print(message);
        }
        public void Warn(object message)
        {
            if (LogVerbosity > IBXSTweenLogger.Verbosity.Warn)
            {
                return;
            }

            GD.PushWarning(message);
        }
        public void Error(object message)
        {
            if (LogVerbosity > IBXSTweenLogger.Verbosity.Error)
            {
                return;
            }

            GD.PushError(message);
        }

        public bool Exception(Exception exception)
        {
            if (LogVerbosity > IBXSTweenLogger.Verbosity.Exception)
            {
                return false;
            }

            if (exception is null)
            {
                Error("Given exception is null.");
                return false;
            }

            GD.PrintErr(exception);
            return true;
        }
        public bool Exception(string prependMessage, Exception exception)
        {
            if (LogVerbosity > IBXSTweenLogger.Verbosity.Exception)
            {
                return false;
            }

            if (exception is null)
            {
                Error("Given exception is null.");
                return false;
            }

            GD.PrintErr(prependMessage, exception);
            return true;
        }
    }
}
#endif
