using System;

namespace RoomRunner
{
#if WINDOWS || XBOX
    public static class Program
    {
        public static Game1 Game = new Game1();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Game.Run();
        }
    }
#endif
}

