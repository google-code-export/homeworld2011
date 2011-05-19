using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PlagueEngine.GUI.Components;

/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
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
        private WindowComponent window = null;
        private ButtonComponent button = null;
        private LabelComponent  label  = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(WindowComponent window,ButtonComponent button,LabelComponent label)
        {
            this.window = window;
            this.button = button;
            this.label  = label;

            button.setDelegate(OnButtonClick);
            window.AddControl(button.Control);
            window.AddControl(label.Control);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            label.ReleaseMe();
            label = null;
            button.ReleaseMe();
            button = null;
            window.ReleaseMe();
            window = null;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Button Click
        /****************************************************************************/
        public void OnButtonClick(object sender, EventArgs e)
        {
            window.Control.Close();
            SendEvent(new DestroyObjectEvent(this.ID), EventsSystem.Priority.High, GlobalGameObjects.GameController);
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

        public String Title  { get; set; }
        public String Text   { get; set; }
        public int    Width  { get; set; }
        public int    Height { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/