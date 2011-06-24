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
        public int[] activationRecievers;
        public bool activated;


        public String Description { get; private set; }

        private int _descriptionWindowWidth;
        private int _descriptionWindowHeight;

        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(int[] activationReciever,
                         String description,
                         int descriptionWindowWidth,
                         int descriptionWindowHeight)
        {
            activationRecievers = activationRecievers;

            Description = description;

            _descriptionWindowHeight = descriptionWindowHeight;
            _descriptionWindowWidth = descriptionWindowWidth;
        }
        /****************************************************************************/



        /****************************************************************************/
        /// GetActions
        /****************************************************************************/
        public virtual string[] GetActions()
        {
            return new[] { "Examine" };
        }
        /****************************************************************************/


        /****************************************************************************/
        /// GetActions
        /****************************************************************************/
        public virtual string[] GetActions(Mercenary mercenary)
        {
            return new[] { "Examine", "Activate" };
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Event
        /****************************************************************************/
        public override void OnEvent(EventsSystem.EventsSender sender, EventArgs e)
        {


            if (e.GetType().Equals(typeof(ExamineEvent)))
            {
                var data = new DescriptionWindowData
                               {
                                   Title = Name,
                                   Text = Description,
                                   Width = _descriptionWindowWidth,
                                   Height = _descriptionWindowHeight
                               };

                SendEvent(new CreateObjectEvent(data), EventsSystem.Priority.Normal, GlobalGameObjects.GameController);
            }


            if (e.GetType().Equals(typeof(ObjectActivatedEvent)))
            {
                if (CheckActivation())
                {
                    if (activationRecievers != null)
                    {
                        SendEvent(new ObjectActivatedEvent(this), EventsSystem.Priority.Low, activationRecievers);
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
            data.DescriptionWindowWidth = _descriptionWindowWidth;
            data.DescriptionWindowHeight = _descriptionWindowHeight;
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
