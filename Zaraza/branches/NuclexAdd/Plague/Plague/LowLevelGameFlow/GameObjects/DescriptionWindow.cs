using System;
using PlagueEngine.GUI.Components;

namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// DescriptionWindow
    /********************************************************************************/
    class DescriptionWindow : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private WindowComponent _window;
        private ButtonComponent _button;
        private LabelComponent  _label;
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(WindowComponent window,ButtonComponent button,LabelComponent label)
        {
            _window = window;
            _button = button;
            _label  = label;
  
            button.SetDelegate(OnButtonClick);
            window.AddControl(button.Control);
            window.AddControl(label.Control);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            _label.ReleaseMe();
            _label = null;
            _button.ReleaseMe();
            _button = null;
            _window.ReleaseMe();
            _window = null;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Button Click
        /****************************************************************************/
        public void OnButtonClick(object sender, EventArgs e)
        {
            _window.Control.Close();
            SendEvent(new DestroyObjectEvent(ID), EventsSystem.Priority.High, GlobalGameObjects.GameController);
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// DescriptionWindowData
    /********************************************************************************/
    [Serializable]
    public class DescriptionWindowData : GameObjectInstanceData
    {
        public DescriptionWindowData()
        {
            Type = typeof(DescriptionWindow);
        }

        private string _title;
        public String Title
        {
            get { return _title; }
            set { _title = string.IsNullOrEmpty(value) ? "" : value; }
        }

        private string _text;
        public String Text
        {
            get { return _text; }
            set { _text = string.IsNullOrEmpty(value) ? "" : value; }
        }

        private int _width;
        public int    Width
        {
            get { return _width; }
            set { _width = value < 100 ? 100 : value; }
        }

        private int _height;
        public int    Height
        {
            get { return _height; }
            set { _height = value < 100 ? 100 : value; }
        }
    }
    /********************************************************************************/

}
/************************************************************************************/