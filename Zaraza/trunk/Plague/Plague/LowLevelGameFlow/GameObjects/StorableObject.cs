using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.ComponentModel;


/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// StorableObject
    /********************************************************************************/
    class StorableObject : GameObjectInstance, IActiveGameObject
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public Rectangle Icon      { get; private set; }
        public Rectangle SlotsIcon { get; private set; }

        public String Description  { get; private set; }
        
        private int DescriptionWindowWidth  = 0;
        private int DescriptionWindowHeight = 0;
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(Rectangle icon, 
                         Rectangle slotsIcon,
                         String    description,
                         int       descriptionWindowWidth,
                         int       descriptionWindowHeight)
        {
            Icon        = icon;
            SlotsIcon   = slotsIcon;
            Description = description;
            Status      = GameObjectStatus.Pickable;

            DescriptionWindowHeight = descriptionWindowHeight;
            DescriptionWindowWidth  = descriptionWindowWidth;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// GetActions
        /****************************************************************************/
        public string[] GetActions()
        {
            return new String[] { };
        }
        /****************************************************************************/


        /****************************************************************************/
        /// GetActions
        /****************************************************************************/
        public string[] GetActions(Mercenary mercenary)
        {
            return new String[] { "Grab" , "Examine" };
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Storing
        /****************************************************************************/
        public virtual void OnStoring()
        {
            owner = null;
            OwnerBone = -1;
            getWorld = GetMyWorld;            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public void GetData(StorableObjectData data)
        {            
            base.GetData(data);

            data.Icon                    = Icon;
            data.SlotsIcon               = SlotsIcon;
            data.Description             = Description;
            data.DescriptionWindowWidth  = DescriptionWindowWidth;
            data.DescriptionWindowHeight = DescriptionWindowHeight;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Event
        /****************************************************************************/
        public override void OnEvent(EventsSystem.EventsSender sender, EventArgs e)
        {
            if (e.GetType().Equals(typeof(ExamineEvent)))
            {
                DescriptionWindowData data = new DescriptionWindowData();
                
                data.Title  = Name;
                data.Text   = Description;
                data.Width  = DescriptionWindowWidth;
                data.Height = DescriptionWindowHeight;

                SendEvent(new CreateObjectEvent(data), EventsSystem.Priority.Normal, GlobalGameObjects.GameController);
            }
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// StorableObjectData
    /********************************************************************************/
    [Serializable]
    public class StorableObjectData : GameObjectInstanceData
    {
        public StorableObjectData()
        {
            Type = typeof(StorableObject);
        }

        [CategoryAttribute("Icons")]
        public Rectangle Icon      { get; set; }
        [CategoryAttribute("Icons")]
        public Rectangle SlotsIcon { get; set; }

        [CategoryAttribute("Description")]
        public String Description          { get; set; }
        [CategoryAttribute("Description")]
        public int DescriptionWindowWidth  { get; set; }
        [CategoryAttribute("Description")]
        public int DescriptionWindowHeight { get; set; }                
    }
    /********************************************************************************/

}
/************************************************************************************/
