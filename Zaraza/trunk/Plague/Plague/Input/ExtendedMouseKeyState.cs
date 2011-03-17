using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlagueEngine.Input.Components
{
    struct ExtendedMouseKeyState
    {
        private bool isDown;
        private bool changed;

        public ExtendedMouseKeyState(bool isDown, bool changed)
        {
            this.isDown = isDown;
            this.changed = changed;
        }

        public bool IsDown()
        {
            return isDown;
        }

        public bool IsUp()
        {
            return !isDown;
        }
     
        public bool WasPressed()
        {
            return (isDown && changed ? true : false);
        }
       
       
        public bool WasReleased()
        {
            return (!isDown && changed ? true : false);
        }
    }
}
