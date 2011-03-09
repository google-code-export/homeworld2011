using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/************************************************************************************/
/// PlagueEngine.TimeControlSystem
/************************************************************************************/
namespace PlagueEngine.TimeControlSystem
{

    /********************************************************************************/
    /// Clock
    /********************************************************************************/
    /// <summary>
    /// Zegar pozwala na podstawową kontrolę czasu. Obiekty tej klasy mają za zadanie
    /// umożliwić osobne kontrolowanie czasów różnych systemów w silniku/grze. Jak np
    /// zatrzymywanie/spowalnianie samej fizyki. Możemy tworzyć w jego kontekście
    /// timery i (UWAGA!!) kolejne zegary xD. Nie wiem czy to się przyda, ale jest 
    /// śmieszne.
    /// </summary>
    class Clock
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private TimeSpan    time      = TimeSpan.Zero;
        private TimeSpan    delta     = TimeSpan.Zero;            
        private double      ratio     = 1;
        private uint        paused    = 0;

        private Dictionary<uint, Timer> Timers   = new Dictionary<uint, Timer>();
        private List<Timer> WastedTimers         = new List<Timer>();
        private List<Clock> Clocks               = new List<Clock>();
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public Clock() { }
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor (2)
        /****************************************************************************/
        public Clock(TimeSpan start,double ratio)
        {
            time        = start;
            this.ratio  = Math.Abs(ratio);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update
        /****************************************************************************/
        public void Update(TimeSpan deltaTime)
        {
            if (paused > 0) return;

            delta = TimeSpan.FromTicks((ratio >= 1 ? deltaTime.Ticks * (long)ratio : deltaTime.Ticks / (long)(1/ratio)));
            time += delta;

            foreach (Timer timer in Timers.Values)
            {
                if (timer.Wasted) WastedTimers.Add(timer);
                else timer.Update(delta);
            }

            foreach (Clock clock in Clocks)
            {
                clock.Update(delta);
            }

            foreach (Timer timer in WastedTimers) Timers.Remove(timer.ID);
            WastedTimers.Clear();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Time
        /****************************************************************************/
        public TimeSpan Time
        {
            set
            {
                time = value;
            }

            get
            {
                return time;    
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Delta Time
        /****************************************************************************/
        public TimeSpan DeltaTime
        {
            get
            {
                return delta;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Pause
        /****************************************************************************/
        public void Pause()
        {
            ++paused;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Resume
        /****************************************************************************/
        public void Resume()
        {
            if(paused > 0) --paused;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Stop
        /****************************************************************************/
        public void Stop()
        {
            paused  = 1;
            time    = TimeSpan.Zero;
            delta   = TimeSpan.Zero;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Reset
        /****************************************************************************/
        public void Reset()
        {
            paused  = 0;
            time    = TimeSpan.Zero;
            delta   = TimeSpan.Zero;
            ratio   = 1;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Ratio
        /****************************************************************************/
        public double Ratio
        {
            set
            {
                ratio = Math.Abs(value);
            }

            get
            {
                return ratio;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Timer
        /// <summary>
        /// Tworzy nowy timer.
        /// </summary>
        /// <param name="alarm">Czas w sekundach, po ilu zostanie uruchomiony alarm.</param>
        /// <param name="repeat">Powtarzalność alarmu. -1 : nieskończoność, 0 : brak, > 0 : krotność.</param>
        /// <param name="callback">Metoda która zostanie wywołana jako alarm</param>
        /// <returns>ID Timera.</returns>         
        /****************************************************************************/
        public uint CreateTimer(TimeSpan alarm, int repeat, Timer.CallbackDelegate callback)
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
        /// <param name="repeat">Powtarzalność alarmu. -1 : nieskończoność, 0 : brak, > 0 : krotność.</param>
        /// <param name="callback">Metoda która zostanie wywołana jako alarm</param>
        /// <returns>ID Timera.</returns>
        /****************************************************************************/
        public uint CreateTimer(uint alarm, int repeat, Timer.CallbackDelegate callback)
        {
            Timer timer = new Timer(TimeSpan.FromSeconds(alarm), repeat, callback);
            Timers.Add(timer.ID, timer);
            return timer.ID;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Timer
        /****************************************************************************/
        public void ReleaseTimer(uint id)
        {
            Timers.Remove(id);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Clock
        /****************************************************************************/
        public Clock CreateClock()
        {
            Clock clock = new Clock();
            Clocks.Add(clock);
            return clock;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Clock (2)
        /****************************************************************************/
        public Clock CreateClock(TimeSpan start, double ratio = 1)
        {
            Clock clock = new Clock(start, ratio);
            Clocks.Add(clock);
            return clock;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Clock (3)
        /****************************************************************************/
        public Clock CreateClock(double ratio)
        {
            Clock clock = new Clock(TimeSpan.Zero, ratio);
            Clocks.Add(clock);
            return clock;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Clock
        /****************************************************************************/
        public void ReleaseClock(Clock clock)
        {
            Clocks.Remove(clock);
        }
        /****************************************************************************/


    }
    /********************************************************************************/

}
/************************************************************************************/