#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace RoomGenerator
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
            Rooms game = null;
            try
            {
                game = new Rooms();
                game.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
            finally
            {
                if (game is IDisposable)
                {
                    ((IDisposable)game).Dispose();
                }
            }
        }
    }
#endif
}
