//-------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
//-------------------------------------------------------------------------------

namespace CelestialEngine.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// Provides a mechanism to log different levels of information to multiple destinations.
    /// </summary>
    public static class Logger
    {
        #region Members
        /// <summary>
        /// Maintains a list of <see cref="LogReceiver"/>s to which log messages are
        /// sent.
        /// </summary>
        private static List<LogReceiver> logReceivers = new List<LogReceiver>();
        #endregion

        #region LogReceiver Delegate
        /// <summary>
        /// Defines a method capable of receiving log notifications from the
        /// <see cref="Logger"/>.
        /// </summary>
        /// <param name="logLevel">The log level of the message.</param>
        /// <param name="message">The log message.</param>
        public delegate void LogReceiver(Level logLevel, string message);
        #endregion

        #region Logger Level Enum
        /// <summary>
        /// Defines the level of the log message.
        /// </summary>
        public enum Level : byte
        {
            /// <summary>
            /// No logger level set.
            /// </summary>
            None = 0x0,

            /// <summary>
            /// Log message is for debug purposes only; super very stupidly verbose.
            /// </summary>
            VerboseDebug = 0x1,

            /// <summary>
            /// Log message is for debug purposes only; very verbose.
            /// </summary>
            Debug = 0x2,

            /// <summary>
            /// Log message is for informational purposes only; somewhat verbose.
            /// </summary>
            Info = 0x4,

            /// <summary>
            /// Log message is a warning message.
            /// </summary>
            Warning = 0x8,

            /// <summary>
            /// Log message is a non-terminating error.
            /// </summary>
            Error = 0x10,

            /// <summary>
            /// Log message is a terminating error and typically precedes the termination
            /// of the application.
            /// </summary>
            Fatal = 0x20
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a <see cref="LogReceiver"/> that logs all output to the specified file.
        /// </summary>
        /// <param name="filePath">String containing the output file.</param>
        /// <param name="listenLevel">Max Log Level that we're interested in.</param>
        /// <param name="append">If true (default) the file will be appended to if it exists, otherwise the file will be cleared.</param>
        /// <param name="forceFlush">If true (default) the file stream will be flushed following each write.</param>
        /// <returns>A <see cref="LogReceiver"/> instance that was added to the receiver list.</returns>
        public static LogReceiver AddFileReceiver(string filePath, Level listenLevel, bool append = true, bool forceFlush = true)
        {
            StreamWriter outStream = new StreamWriter(filePath, append, System.Text.Encoding.Unicode);

            LogReceiver fileReceiver = new LogReceiver((Level logLevel, string message) =>
            {
                // Is this within our log level?
                if (listenLevel <= logLevel)
                {
                    outStream.WriteLine(message);

                    if (forceFlush)
                    {
                        outStream.Flush();
                    }
                }
            });

            AddReceiver(fileReceiver);

            return fileReceiver;
        }

        /// <summary>
        /// Creates a <see cref="LogReceiver"/> that logs all output to the console.
        /// </summary>
        /// <param name="listenLevel">Max Log Level that we're interested in.</param>
        /// <returns>A <see cref="LogReceiver"/> instance that was added to the receiver list.</returns>
        public static LogReceiver AddConsoleReceiver(Level listenLevel)
        {
            LogReceiver consoleReceiver = new LogReceiver((Level logLevel, string message) =>
            {
                // Is this within our log level?
                if (listenLevel <= logLevel)
                {
                    Console.WriteLine(message); // Write the mesage
                }
            });

            AddReceiver(consoleReceiver);

            return consoleReceiver;
        }

        /// <summary>
        /// Creates a <see cref="LogReceiver"/> that logs all output to the immediate window.
        /// </summary>
        /// <param name="listenLevel">Max Log Level that we're interested in.</param>
        /// <returns>A <see cref="LogReceiver"/> instance that was added to the receiver list.</returns>
        public static LogReceiver AddImmediateReceiver(Level listenLevel)
        {
            LogReceiver immediateReceiver = new LogReceiver((Level logLevel, string message) =>
            {
                // Is this within our log level?
                if (listenLevel <= logLevel)
                {
                    System.Diagnostics.Debug.WriteLine(message); // Write the mesage
                }
            });

            AddReceiver(immediateReceiver);

            return immediateReceiver;
        }

        /// <summary>
        /// Adds the given <see cref="LogReceiver"/> to the list of log message
        /// receivers.
        /// </summary>
        /// <param name="receiver">A <see cref="LogReceiver"/> pointing to a method interested in receiving
        /// log messages.</param>
        public static void AddReceiver(LogReceiver receiver)
        {
            logReceivers.Add(receiver);
            Log(Level.Debug, "Added log receiver: {0} in {1}.", receiver.Method.Name, receiver.Method.DeclaringType.FullName);
        }

        /// <summary>
        /// Removes the given <see cref="LogReceiver"/> to the list of log message
        /// receivers.
        /// </summary>
        /// <param name="receiver">The <see cref="LogReceiver"/> that is no longer interested in receiving
        /// log messages.</param>
        public static void RemoveReceiver(LogReceiver receiver)
        {
            Log(Level.Debug, "Removing log receiver: {0} in {1}.", receiver.Method.Name, receiver.Method.DeclaringType.FullName);
            logReceivers.Remove(receiver);
        }

        /// <summary>
        /// Logs the specified message and log level; the log message is distributed to the
        /// registered log receivers.
        /// </summary>
        /// <param name="logLevel">The <see cref="Level"/> of the log message.</param>
        /// <param name="message">The log message. The message can contain formatter symbols such as <c>{0} {1}</c>.</param>
        /// <param name="args">The arguments to be used in the call to <c>String.Format</c>.</param>
        public static void Log(Level logLevel, string message, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                message = string.Format(CultureInfo.InvariantCulture, message, args);
            }

            message = string.Format("[{0} @ {1} - {2}] {3}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), logLevel.ToString(), message);

            foreach (LogReceiver receiver in logReceivers)
            {
                receiver.Invoke(logLevel, message);
            }
        }

        /// <summary>
        /// Logs and then throws the specified exception.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="exception">The exception to log and throw.</param>
        public static void Throw(Level logLevel, Exception exception)
        {
            Log(logLevel, exception.ToString());
            throw exception;
        }
        #endregion
    }
}