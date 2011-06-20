using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PlagueEngine.EventsSystem;
using PlagueEngine.HighLevelGameFlow;

/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// GameController
    /********************************************************************************/
    class GameController : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private Game game = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(Game game)
        {
            this.game = game;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Event
        /****************************************************************************/
        public override void OnEvent(EventsSender sender, EventArgs e)
        {
            /*************************************/
            /// ChangeLevelEvent
            /*************************************/
            if (e.GetType().Equals(typeof(ChangeLevelEvent)))
            {
                ChangeLevelEvent ChangeLevelEvent = e as ChangeLevelEvent;
                game.Level.LoadLevel(ChangeLevelEvent.Level);
            }
            /*************************************/
            /// CreateObjectEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(CreateObjectEvent)))
            {
                CreateObjectEvent CreateObjectEvent = e as CreateObjectEvent;

                GameObjectInstance goi = game.Level.GameObjectsFactory.Create(CreateObjectEvent.Data);

                if (goi != null)
                {
                    SendEvent(new ObjectCreatedEvent(goi), Priority.High, sender as IEventsReceiver);
                }
            }
            /*************************************/
            /// DestroyObjectEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(DestroyObjectEvent)))
            {
                DestroyObjectEvent DestroyObjectEvent = e as DestroyObjectEvent;

                game.Level.GameObjectsFactory.RemoveGameObject(DestroyObjectEvent.ID);
            }
            /*************************************/
            /// ChangeSpeedEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(ChangeSpeedEvent)))
            {
                ChangeSpeedEvent ChangeSpeedEvent = e as ChangeSpeedEvent;

                game.PhysicsClock.Ratio *= ChangeSpeedEvent.Amount;
                game.RendererClock.Ratio *= ChangeSpeedEvent.Amount;
            }
            /*************************************/
            /// ExitGameEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(ExitGameEvent)))
            {
                game.Exit();
            }
            /*************************************/
            /// SetBloomEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(SetBloomEvent)))
            { 
                SetBloomEvent SetBloomEvent = e as SetBloomEvent;
                game.Renderer.BaseIntensity = SetBloomEvent.BaseIntensity;
                game.Renderer.BaseSaturation = SetBloomEvent.BaseSaturation;
                game.Renderer.BloomIntensity = SetBloomEvent.BloomIntensity;
                game.Renderer.BloomSaturation = SetBloomEvent.BloomSaturation;
                game.Renderer.BloomThreshold = SetBloomEvent.BloomThreshold;
            }
            /*************************************/        

        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            GameControllerData data = new GameControllerData();
            GetData(data);
            return data;
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// GameControllerData
    /********************************************************************************/
    [Serializable]
    public class GameControllerData : GameObjectInstanceData
    {
        public GameControllerData()
        {
            Type = typeof(GameController);
        }
    }
    /********************************************************************************/

}
/************************************************************************************/