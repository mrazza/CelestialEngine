// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="">
// Copyright (C) Matt Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.TechDemo
{
    using System.Windows.Forms;

#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            CelestialEngine.Core.Logger.AddImmediateReceiver(Core.Logger.Level.VerboseDebug);
            Application.Run(new LaunchOptions());
        }
    }
#endif
}

