using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Rendering.Components;

using Microsoft.Xna.Framework.Input;

/************************************************************************************/
/// Plague.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// FreeCamera
    /********************************************************************************/
    class FreeCamera : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private CameraComponent cameraComponent = null;
        private double          movementSpeed   = 0;
        private double          rotationSpeed   = 0;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public FreeCamera(uint id, String definition) : base(id, definition)
        {
            requireUpdate = true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(CameraComponent cameraComponent, Matrix world, double movementSpeed, double rotationSpeed)
        {
            this.cameraComponent = cameraComponent;
            this.World           = world;
            this.movementSpeed   = movementSpeed;
            this.rotationSpeed   = rotationSpeed;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update
        /****************************************************************************/
        public override void Update(TimeSpan deltaTime)
        { 

            /************************************************************************/
            // To jest brzydkie, i nie bedzie tak działać         
            // TODO: Zamienić to na coś porządnego! Ot co!
            KeyboardState currentState = Keyboard.GetState(PlayerIndex.One);
            
            double movement = movementSpeed * deltaTime.TotalMilliseconds;
            double rotation = rotationSpeed * deltaTime.TotalMilliseconds;

            if (currentState.IsKeyDown(Keys.W))
            {
                cameraComponent.MoveForward((float)movement);
            }

            if (currentState.IsKeyDown(Keys.S))
            {
                cameraComponent.MoveForward((float)(movement * -1));
            }

            if (currentState.IsKeyDown(Keys.A))
            {
                cameraComponent.MoveRight((float)(movement));
            }

            if (currentState.IsKeyDown(Keys.D))
            {
                cameraComponent.MoveRight((float)(movement * -1));
            }

            if (currentState.IsKeyDown(Keys.Q))
            {
                cameraComponent.MoveUp((float)(movement));
            }

            if (currentState.IsKeyDown(Keys.E))
            {
                cameraComponent.MoveUp((float)(movement * -1));
            }



            if (currentState.IsKeyDown(Keys.PageUp))
            {
                cameraComponent.RotateZ((float)rotation);
            }

            if (currentState.IsKeyDown(Keys.PageDown))
            {
                cameraComponent.RotateZ((float)rotation * -1);
            }
            
            if (currentState.IsKeyDown(Keys.Up))
            {
                cameraComponent.RotateX((float)rotation);
            }

            if (currentState.IsKeyDown(Keys.Down))
            {
                cameraComponent.RotateX((float)rotation * -1);
            }

            if (currentState.IsKeyDown(Keys.Right))
            {
                cameraComponent.Yaw((float)rotation);
            }

            if (currentState.IsKeyDown(Keys.Left))
            {
                cameraComponent.Yaw((float)rotation * -1);
            }
            /************************************************************************/

        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            FreeCameraData data = new FreeCameraData();
            GetData(data);

            data.MovementSpeed = this.movementSpeed;
            data.RotationSpeed = this.rotationSpeed;
            data.FoV           = this.cameraComponent.FoV;
            data.ZNear         = this.cameraComponent.ZNear;
            data.ZFar          = this.cameraComponent.ZFar;

            return data;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            cameraComponent.ReleaseMe();                    
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// Free Camera Data
    /********************************************************************************/
    [Serializable]
    public class FreeCameraData : GameObjectInstanceData
    {
        public double MovementSpeed = 0;
        public double RotationSpeed = 0;
        public float  FoV           = 0;
        public float  ZNear         = 0;
        public float  ZFar          = 0;
    }
    /********************************************************************************/

}
/************************************************************************************/