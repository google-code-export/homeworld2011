using System;


/************************************************************************************/
/// PlagueEngine
/************************************************************************************/
namespace PlagueEngine
{
#if WINDOWS || XBOX

    /********************************************************************************/
    /// Program
    /********************************************************************************/
    static class Program
    {

        /****************************************************************************/
        /// Main
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /****************************************************************************/
        static void Main(string[] args)
        {
            using (Game game = new Game("Zaraza"))
            {
                game.Run();
            }
        }
        /****************************************************************************/
    }
    /********************************************************************************/

#endif
}
/************************************************************************************/