﻿namespace PlagueEngine.Input.Components
{
    /****************************************************************************/
    /// Extended MouseKey State
    /****************************************************************************/
    struct ExtendedMouseKeyState
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private readonly bool _isDown;
        private readonly bool _changed;
        /****************************************************************************/



        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public ExtendedMouseKeyState(bool isDown, bool changed) : this()
        {
            _isDown = isDown;
            _changed = changed;
            Propagate = true;
        }
        /****************************************************************************/

        public bool Propagate { get; set; }

        /// Is Down
        /****************************************************************************/
        public bool IsDown()
        {
            return _isDown;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Is Up
        /****************************************************************************/
        public bool IsUp()
        {
            return !_isDown;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Was Pressed
        /****************************************************************************/
        public bool WasPressed()
        {
            return (_isDown && _changed ? true : false);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Was Released
        /****************************************************************************/
        public bool WasReleased()
        {
            return (!_isDown && _changed ? true : false);
        }
        /****************************************************************************/

    }
    /****************************************************************************/
}
/****************************************************************************/