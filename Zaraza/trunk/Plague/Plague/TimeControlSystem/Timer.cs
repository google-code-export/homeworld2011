using System;
using System.Collections.Generic;
using System.Text;


/************************************************************************************/
/// PlagueEngine.TimeControlSystem
/************************************************************************************/
namespace PlagueEngine.TimeControlSystem
{

    /********************************************************************************/
    /// Timer
    /********************************************************************************/
    /// <summary>
    /// Timer pozwala ustawiac alarm po upływie danego czasu gry. Może działać w czasie
    /// względem systemu (gry), bądź względem czasu zegara w kontekście którego został
    /// utworzony.
    /// </summary>
    class Timer
    {

        /****************************************************************************/
        /// Delegates
        /****************************************************************************/
        public delegate void CallbackDelegate1(uint id);
        public delegate void CallbackDelegate2();
        
        private CallbackDelegate1 callback1 = null;
        private CallbackDelegate2 callback2 = null;
        /****************************************************************************/

        
        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private TimeSpan elapsedTime = TimeSpan.Zero;
        private TimeSpan alarm       = TimeSpan.Zero;
        private int      repeat      = 0;
        private bool     wasted      = false;
        private uint     id          = 0;
        private static uint totalID  = 0;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public Timer(TimeSpan alarm, int repeat, CallbackDelegate1 callback)
        {
            this.id         = ++Timer.totalID;
            this.alarm      = alarm;
            this.repeat     = repeat;
            this.callback1  = callback;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor (2)
        /****************************************************************************/
        public Timer(TimeSpan alarm, int repeat, CallbackDelegate2 callback)
        {
            this.id         = ++Timer.totalID;
            this.alarm      = alarm;
            this.repeat     = repeat;
            this.callback2  = callback;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update
        /****************************************************************************/
        public void Update(TimeSpan deltaTime)
        {
            if(wasted) return;

            elapsedTime += deltaTime;
            if (elapsedTime >= alarm)
            {
                elapsedTime -= alarm;
                if (repeat >  0) --repeat;
                if (repeat == 0) wasted = true;

                if      (callback1 != null) callback1(id);
                else if (callback2 != null) callback2();
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Wasted
        /****************************************************************************/
        public bool Wasted
        {
            get
            {
                return wasted;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// ID
        /****************************************************************************/
        public uint ID
        {
            get
            {
                return id;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Reset
        /****************************************************************************/
        public void Reset(TimeSpan alarm, int repeat)
        {
            this.elapsedTime    = TimeSpan.Zero;
            this.alarm          = alarm;
            this.repeat         = repeat;
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/