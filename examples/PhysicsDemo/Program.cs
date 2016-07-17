using System;

namespace PhysicsDemo
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
            using (var game = new PhysicsGame())
            {
                game.GraphicsDeviceManager.PreferredBackBufferWidth = 1440;
                game.GraphicsDeviceManager.PreferredBackBufferHeight = 900;
                game.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
                game.GraphicsDeviceManager.ApplyChanges();
                game.Run();
            }
        }
    }
#endif
}
