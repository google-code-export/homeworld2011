using System;
using System.Collections.Generic;
using System.Text;


/************************************************************************************/
/// PlagueEngine.TimeControlSystem
/************************************************************************************/
namespace PlagueEngine.TimeControlSystem
{
  
    /********************************************************************************/
    /// Time Control    
    /// <summary>
    /// Statyczna klasa do zarządzania zegarami i timerami. Pozwala na tworzenie ich 
    /// w kontekście czasu działania aplikacji. 
    /// </summary>
    /********************************************************************************/
    static class TimeControl
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private static Dictionary<uint,Timer> Timers       = new Dictionary<uint,Timer>();
        private static List<Timer>            WastedTimers = new List<Timer>();
        private static List<Clock>            Clocks       = new List<Clock>();
        /****************************************************************************/


        /****************************************************************************/
        /// Update
        /****************************************************************************/
        public static void Update(TimeSpan deltaTime)
        {
            foreach (Timer timer in Timers.Values)
            {
                if (timer.Wasted) WastedTimers.Add(timer);               
                else timer.Update(deltaTime);
            }

            foreach (Clock clock in Clocks)
            {
                clock.Update(deltaTime);
            }

            foreach (Timer timer in WastedTimers) Timers.Remove(timer.ID);
            WastedTimers.Clear();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Timer
        /// <summary>
        /// Tworzy nowy timer.
        /// </summary>
        /// <param name="alarm">Czas w sekundach, po ilu zostanie uruchomiony alarm.</param>
        /// <param name="repeat">Powtarzalność alarmu. -1 = nieskończoność.</param>
        /// <param name="callback">Metoda która zostanie wywołana jako alarm</param>
        /// <returns>ID Timera.</returns>         
        /****************************************************************************/
        public static uint CreateTimer(TimeSpan alarm, int repeat, Timer.CallbackDelegate1 callback)
        {
            Timer timer = new Timer(alarm, repeat, callback);
            Timers.Add(timer.ID, timer);
            return timer.ID;            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Timer (2)
        /// <summary>
        /// Tworzy nowy timer.
        /// </summary>
        /// <param name="alarm">Czas w sekundach, po ilu zostanie uruchomiony alarm.</param>
        /// <param name="repeat">Powtarzalność alarmu. -1 = nieskończoność.</param>
        /// <param name="callback">Metoda która zostanie wywołana jako alarm</param>
        /// <returns>ID Timera.</returns>         
        /****************************************************************************/
        public static uint CreateTimer(TimeSpan alarm, int repeat, Timer.CallbackDelegate2 callback)
        {
            Timer timer = new Timer(alarm, repeat, callback);
            Timers.Add(timer.ID, timer);
            return timer.ID;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Reset Timer
        /****************************************************************************/
        public static void ResetTimer(uint id, TimeSpan alarm, int repeat)
        {
            Timers[id].Reset(alarm, repeat);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Timer
        /****************************************************************************/
        public static void ReleaseTimer(uint id)
        {
            Timers.Remove(id);
        }
        /****************************************************************************/

                
        /****************************************************************************/
        /// Create Clock
        /****************************************************************************/
        public static Clock CreateClock()
        {
            Clock clock = new Clock();
            Clocks.Add(clock);
            return clock;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Clock (2)
        /****************************************************************************/
        public static Clock CreateClock(TimeSpan start,double ratio = 1)
        {
            Clock clock = new Clock(start,ratio);
            Clocks.Add(clock);
            return clock;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Clock (3)
        /****************************************************************************/
        public static Clock CreateClock(double ratio)
        {
            Clock clock = new Clock(TimeSpan.Zero, ratio);
            Clocks.Add(clock);
            return clock;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Clock
        /****************************************************************************/
        public static void ReleaseClock(Clock clock)
        {
            Clocks.Remove(clock);                
        }
        /****************************************************************************/

    }
    /********************************************************************************/  

}
/************************************************************************************/