﻿// Copyright (C) 2006-2010 Jim Tilander. See COPYING for and README for more details.
using System.Collections.Generic;

namespace Aurora
{
    public static class Log
    {
        // Internal enumeration. Only used in handlers to identify the type of message
        public enum Level
        {
            Debug,
            Info,
            Warn,
            Error,
        }

        // This needs to be implemented by all clients.
        public interface IHandler
        {
            void OnMessage(Level level, string message, string formattedLine);
        }

        // Helper class to keep the indent levels balanced (with the help of the using statement)

        // Log class implement below
        public static string Prefix { get; set; } = "";

        public static int HandlerCount => mHandlers.Count;

        public static void AddHandler(IHandler handler)
        {
            if (null == handler)
                return;

            lock (mHandlers)
            {
                mHandlers.Add(handler);
            }
        }

        public static void RemoveHandler(IHandler handler)
        {
            lock (mHandlers)
            {
                mHandlers.Remove(handler);
            }
        }

        public static void ClearHandlers()
        {
            lock (mHandlers)
            {
                mHandlers.Clear();
            }
        }

        public static void IncIndent()
        {
            mIndent++;
        }

        public static void DecIndent()
        {
            mIndent--;
        }

        public static void Debug(string message, params object[] args)
        {
#if DEBUG
            OnMessage(Level.Debug, message, args);
#endif
        }

        public static void Info(string message, params object[] args)
        {
            OnMessage(Level.Info, message, args);
        }

        public static void Warning(string message, params object[] args)
        {
            OnMessage(Level.Warn, message, args);
        }

        public static void Error(string message, params object[] args)
        {
            OnMessage(Level.Error, message, args);
        }

        private static void OnMessage(Level level, string format, object[] args)
        {
            string message = args.Length > 0 ? string.Format(format, args) : format;
            string formattedLine;
            string indent = new string(' ', mIndent * 4);
            string levelName = level.ToString().PadLeft(5, ' ');

            if (Prefix.Length > 0)
            {
                formattedLine = Prefix + " (" + levelName + "): " + indent + message + "\n";
            }
            else
            {
                formattedLine = levelName + ": " + indent + message + "\n";
            }

            foreach (IHandler handler in mHandlers)
            {
                handler.OnMessage(level, message, formattedLine);
            }
        }

        private static readonly List<IHandler> mHandlers = new List<IHandler>();
        private static int mIndent = 0;
    }
}
