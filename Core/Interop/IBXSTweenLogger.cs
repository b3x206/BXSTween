using System;

namespace BX.Tweening.Interop
{
    /// <summary>
    /// Defines a basic generic logger implementation, just with the extra Exception added.
    /// </summary>
    public interface IBXSTweenLogger
    {
        /// <summary>
        /// Logging verbosity.
        /// <br>Being set to <see cref="None"/> means nothing is output.
        /// Default should be <see cref="Error"/> or on <see cref="Warn"/>/<see cref="Info"/> 
        /// in devtime, which outputs Error and above.
        /// </br>
        /// </summary>
        public enum Verbosity
        {
            /// <summary>
            /// Output everything. Pretty much same as <see cref="Info"/>.
            /// </summary>
            All = 0,
            /// <summary>
            /// Output <see cref="IBXSTweenLogger.Info"/> and above priority logs.
            /// </summary>
            Info = 1,
            /// <summary>
            /// Output <see cref="IBXSTweenLogger.Warn"/> and above priority logs.
            /// </summary>
            Warn = 2,
            /// <summary>
            /// Output <see cref="IBXSTweenLogger.Error"/> and above priority logs.
            /// </summary>
            Error = 3,
            /// <summary>
            /// Output <see cref="IBXSTweenLogger.Exception"/> and above priority logs.
            /// </summary>
            Exception = 4,
            /// <summary>
            /// Output nothing.
            /// </summary>
            None = 5
        }

        /// <summary>
        /// Output verbosity. This can be internally handled on the logger as well and the logger 
        /// calls can be passed as usually the logger receives static boxed content, unless for an exception.
        /// </summary>
        public Verbosity LogVerbosity { get; set; }

        /// <summary>
        /// Prints out a standard log.
        /// </summary>
        public void Info(object message);
        /// <summary>
        /// Prints out a warning priority log.
        /// </summary>
        public void Warn(object message);
        /// <summary>
        /// Prints out an error priority log.
        /// </summary>
        public void Error(object message);

        /// <summary>
        /// Prints out an exception.
        /// <br>A special unity method, use <see cref="Error"/> if there's no method like <c>Debug.LogException()</c> in logger.</br>
        /// </summary>
        public bool Exception(Exception exception);
        /// <summary>
        /// Prints out an exception.
        /// <br>A special unity method, should use <see cref="Error"/> internally if there's no method like <c>Debug.LogException()</c> in logger.</br>
        /// </summary>
        /// <param name="prependMessage">
        /// Message to prepend into the exception. This is necessary for more comprehensive and controlled logging of exceptions.
        /// <br>If you are unsure how, you can copy paste this following code for the code:</br><br/><br/>
        /// <c>
        /// <![CDATA[
        /// using System;
        /// using UnityEngine;
        /// using System.Reflection;
        /// 
        /// public sealed class CustomLogger : ILogger
        /// {
        ///     // ... rest of the interface
        ///     // For Unity:
        ///     public bool Exception(string prependMessage, Exception exception)
        ///     {
        ///         if (exception == null)
        ///         {
        ///             Error("[CustomLogger::LogException] Given exception is null.");
        ///             return false;
        ///         }
        /// 
        ///         const string FieldName = "_message";
        ///         FieldInfo messageField = exception.GetType().GetField(FieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        ///         if (messageField != null)
        ///         {
        ///             messageField.SetValue(exception, exception.Message.Insert(0, exceptionPrependMessage));
        ///         }
        ///         else
        ///         {
        ///             // Or handle it however you like. But trust me, there is indeed `_message` inside `System.Exception`.
        ///             LogError($"[Logger::LogException] There's no such field as '{FieldName}' inside exception '{exception}'.\n{prependMessage}");
        ///         }
        ///         
        ///         // Unity's default logger is somewhat of a blackbox, requiring this workaround. If you know something else more elegant open PR/issue.
        ///         Debug.LogException(exception);
        ///         return true;
        ///     }
        ///     // For anything sane:
        ///     public bool Exception(string prependMessage, Exception exception)
        ///     {
        ///         if (exception == null)
        ///         {
        ///             LogError("[CustomLogger::LogException] Given exception is null."); // Add System.Diagnostics.StackTrace at your own discretion.
        ///             return false;
        ///         }
        /// 
        ///         Console.Write(prependMessage);
        ///         Console.WriteLine($"{exception.Message}\n    {exception.StackTrace}");
        ///         return true;
        ///     }
        /// }
        /// ]]>
        /// </c>
        /// </param>
        public bool Exception(string prependMessage, Exception exception);
    }
}
