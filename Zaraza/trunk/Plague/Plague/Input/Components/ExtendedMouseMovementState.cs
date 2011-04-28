﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
/****************************************************************************/
/// PlagueEngine.Input.Components
/****************************************************************************/
namespace PlagueEngine.Input.Components
{
    /****************************************************************************/
    /// Extended MouseMovement State
    /****************************************************************************/
    struct ExtendedMouseMovementState
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private Vector3 position;
        private Vector3 oldPosition;
        private Vector3 difference;//o ile zmienilo sie polozenie w x,y,z
        private bool moved; //zmiana w x,y,z
        private bool scrolled;//zmiana w z, tj bylo scrollniecie:D
        private float scrollDifference;//zmiana wartosci scrolla
        internal static DisplayMode display = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public ExtendedMouseMovementState(int x,int y,int z,int oldX,int oldY,int oldZ)
        {
            moved = false;
            scrolled = false;

            scrollDifference = oldZ - z;

            position = new Vector3(x, y, z);
            oldPosition = new Vector3(oldX, oldY, oldZ);

            difference = position - oldPosition;

            if (x!=oldX || y!=oldY)
            {
                moved = true;
            }

            if (z != oldZ)
            {
                scrolled = true;
            }
            
        }
        /****************************************************************************/



        /****************************************************************************/
        /// ScrollDifference
        /****************************************************************************/
        public float ScrollDifference
        {
            get { return scrollDifference; }
        }
        /****************************************************************************/



        /****************************************************************************/
        /// Scrolled
        /****************************************************************************/
        public bool Scrolled
        {
            get { return this.scrolled; }
        }
        /****************************************************************************/




        /****************************************************************************/
        /// is on window
        /****************************************************************************/
        public bool IsOnWindow
        {
            get 
            {
                if (position.X < 0 || position.Y < 0 || position.X > display.Width || position.Y > display.Height)
                {
                    return false;
                }
                return true;
            }
        }
        /****************************************************************************/




        /****************************************************************************/
        /// Position
        /****************************************************************************/
        public Vector3 Position
        {
            get { return position; }
        }
        /****************************************************************************/




        /****************************************************************************/
        /// OldPosition
        /****************************************************************************/
        public Vector3 OldPosition
        {
            get { return oldPosition; }
        }
        /****************************************************************************/



        /****************************************************************************/
        /// Difference
        /****************************************************************************/
        public Vector3 Difference
        {
            get { return difference; }
        }
        /****************************************************************************/



        /****************************************************************************/
        /// Moved
        /****************************************************************************/
        public bool Moved
        {
            get { return moved; }
        }
        /****************************************************************************/


    }
    /****************************************************************************/


}
/****************************************************************************/
