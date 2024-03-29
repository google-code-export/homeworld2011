using System;
using System.Threading;


/************************************************************************************/
// PlagueEngine
/************************************************************************************/
namespace PlagueEngine
{
#if WINDOWS || XBOX

    /********************************************************************************/
    // Program
    /********************************************************************************/
    static class Program
    {
        public static ApartmentState ApartmentState = ApartmentState.STA;
        /****************************************************************************/
        // Main
        // <summary>
        // The main entry point for the application.
        // </summary>
        /****************************************************************************/
        [STAThread]
        static void Main()
        {
            using (var game = new Game("Pyramiden"))
            {
                #if RELEASE
                try
                {
                #endif

                    game.Run();
                
                #if RELEASE                
                }
                catch
                {
                    game.FlushEventsHistory();
                    throw;
                }
                #endif
            }
        }
        /****************************************************************************/
    }
    /********************************************************************************/

#endif
}
/************************************************************************************/
