using System;

namespace VoxerGame
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new VoxerGame())
            {
                game.IsFixedTimeStep = false;
                game.GraphicsDeviceManager.PreferredBackBufferWidth = 1920;
                game.GraphicsDeviceManager.PreferredBackBufferHeight = 1080;
                game.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
                game.GraphicsDeviceManager.ApplyChanges();
                game.Run();
            }
        }
    }
#endif
}
