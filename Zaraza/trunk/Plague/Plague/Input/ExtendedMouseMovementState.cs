using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PlagueEngine.Input.Components
{
    struct ExtendedMouseMovementState
    {
        private Vector3 position;
        private Vector3 oldPosition;
        private Vector3 difference;//o ile zmienilo sie polozenie w x,y,z
        private bool moved; //zmiana w x,y,z
        private bool scrolled;//zmiana w z, tj bylo scrollniecie:D

        public ExtendedMouseMovementState(int x,int y,int z,int oldX,int oldY,int oldZ)
        {
            moved = false;
            scrolled = false;

            position = new Vector3(x, y, z);
            oldPosition = new Vector3(oldX, oldY, oldZ);
            difference = position - oldPosition;
            
            if (difference.Length()!=0)
            {
                moved = true;
            }

            if (z != oldZ)
            {
                scrolled = true;
            }
            
        }

        public bool Scrolled
        {
            get { return this.scrolled; }
        }
        public Vector3 Position
        {
            get { return position; }
        }

        public Vector3 OldPosition
        {
            get { return oldPosition; }
        }

        public Vector3 Difference
        {
            get { return difference; }
        }

        public bool Moved
        {
            get { return moved; }
        }

    }
}
