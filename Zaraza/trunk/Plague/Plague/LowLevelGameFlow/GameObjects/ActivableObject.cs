using System;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{


    abstract class ActivableObject : GameObjectInstance, IActiveGameObject
    {



        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public int[] activationRecievers = null;
        public bool activated = false;


        public String Description { get; private set; }

        private int DescriptionWindowWidth = 0;
        private int DescriptionWindowHeight = 0;

        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(int[] activationReciever,
                         String description,
                         int descriptionWindowWidth,
                         int descriptionWindowHeight)
        {
            this.activationRecievers = activationRecievers;

            Description = description;

            DescriptionWindowHeight = descriptionWindowHeight;
            DescriptionWindowWidth = descriptionWindowWidth;
        }
        /****************************************************************************/



        /****************************************************************************/
        /// GetActions
        /****************************************************************************/
        public virtual string[] GetActions()
        {
            return new String[] { "Examine" };
        }
        /****************************************************************************/


        /****************************************************************************/
        /// GetActions
        /****************************************************************************/
        public virtual string[] GetActions(Mercenary mercenary)
        {
            return new String[] { "Examine", "Activate" };
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

                data.Title = Name;
                data.Text = Description;
                data.Width = DescriptionWindowWidth;
                data.Height = DescriptionWindowHeight;

                SendEvent(new CreateObjectEvent(data), EventsSystem.Priority.Normal, GlobalGameObjects.GameController);
            }


            if (e.GetType().Equals(typeof(ObjectActivatedEvent)))
            {
                if (CheckActivation())
                {
                    if (activationRecievers != null)
                    {
                        this.SendEvent(new ObjectActivatedEvent(this), EventsSystem.Priority.Low, activationRecievers);
                    }
                    OnActivation();
                    
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Action
        /****************************************************************************/
        protected virtual void OnActivation()
        {
            
        }
        /****************************************************************************/

        /****************************************************************************/
        /// CheckActivation
        /****************************************************************************/
        protected virtual bool CheckActivation()
        {
            return true;
        }
        /****************************************************************************/





        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public void GetData(ActivableObjectData data)
        {
            base.GetData(data);

            data.Description = Description;
            data.DescriptionWindowWidth = DescriptionWindowWidth;
            data.DescriptionWindowHeight = DescriptionWindowHeight;
        }
        /****************************************************************************/


    }



    /********************************************************************************/
    /// ActivableObjectData
    /********************************************************************************/
    [Serializable]
    public class ActivableObjectData : GameObjectInstanceData
    {
        public ActivableObjectData()
        {
            Type = typeof(ActivableObject);
        }

        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [CategoryAttribute("Description")]
        public String Description { get; set; }
        [CategoryAttribute("Description")]
        public int DescriptionWindowWidth { get; set; }
        [CategoryAttribute("Description")]
        public int DescriptionWindowHeight { get; set; }
    }
    /********************************************************************************/




}
