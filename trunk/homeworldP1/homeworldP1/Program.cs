using System;

namespace homeworldP1
{
    static class Program
    {
        /// <summary>
        /// </summary>
        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
}

