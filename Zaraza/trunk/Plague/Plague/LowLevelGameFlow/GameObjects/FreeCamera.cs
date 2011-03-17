using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Input.Components;
using PlagueEngine.TimeControlSystem;


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
        private CameraComponent           cameraComponent           = null;
        private KeyboardListenerComponent keyboardListenerComponent = null;
        private double                    movementSpeed             = 0;
        private double                    rotationSpeed             = 0;

        // TODO: Zastanowić się, czy da się rozwiązać to lepiej z tym zegarem
        private Clock                     clock                     = TimeControl.CreateClock();
        /****************************************************************************/


        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(CameraComponent            cameraComponent, 
                         KeyboardListenerComponent  keyboardListenerComponent, 
                         Matrix                     world, 
                         double                     movementSpeed, 
                         double                     rotationSpeed)
        {
            this.cameraComponent            = cameraComponent;
            this.keyboardListenerComponent  = keyboardListenerComponent;
            this.World                      = world;
            this.movementSpeed              = movementSpeed;
            this.rotationSpeed              = rotationSpeed;

            this.keyboardListenerComponent.SubscibeKeys(OnKey, Keys.W, Keys.S, Keys.A, 
                                                               Keys.D, Keys.Q, Keys.E, 
                                                               Keys.PageUp, Keys.PageDown, 
                                                               Keys.Up,     Keys.Down, 
                                                               Keys.Right,  Keys.Left);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Key
        /****************************************************************************/
        private void OnKey(Keys key, ExtendedKeyState state)
        {
            if (!state.IsDown()) return;


            switch (key)
            { 
                case Keys.W:
                    cameraComponent.MoveForward((float)(movementSpeed * clock.DeltaTime.TotalMilliseconds));
                    break;

                case Keys.S:
                    cameraComponent.MoveForward((float)(movementSpeed * clock.DeltaTime.TotalMilliseconds) * -1);
                    break;

                case Keys.A:
                    cameraComponent.MoveRight((float)((movementSpeed * clock.DeltaTime.TotalMilliseconds)));
                    break;

                case Keys.D:
                    cameraComponent.MoveRight((float)((movementSpeed * clock.DeltaTime.TotalMilliseconds) * -1));
                    break;

                case Keys.Q:
                    cameraComponent.MoveUp((float)((movementSpeed * clock.DeltaTime.TotalMilliseconds)));
                    break;

                case Keys.E:
                    cameraComponent.MoveUp((float)((movementSpeed * clock.DeltaTime.TotalMilliseconds) * -1));
                    break;

                case Keys.PageUp:
                    cameraComponent.RotateZ((float)(rotationSpeed * clock.DeltaTime.TotalMilliseconds));
                    break;

                case Keys.PageDown:
                    cameraComponent.RotateZ((float)(rotationSpeed * clock.DeltaTime.TotalMilliseconds) * -1);
                    break;

                case Keys.Up:
                    cameraComponent.RotateX((float)(rotationSpeed * clock.DeltaTime.TotalMilliseconds) * -1);
                    break;

                case Keys.Down:
                    cameraComponent.RotateX((float)(rotationSpeed * clock.DeltaTime.TotalMilliseconds));
                    break;

                case Keys.Right:
                    cameraComponent.Yaw((float)(rotationSpeed * clock.DeltaTime.TotalMilliseconds));
                    break;

                case Keys.Left:
                    cameraComponent.Yaw((float)(rotationSpeed * clock.DeltaTime.TotalMilliseconds) * -1);
                    break;

            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            FreeCameraData data = new FreeCameraData();
            GetData(data);

            data.MovementSpeed      = this.movementSpeed;
            data.RotationSpeed      = this.rotationSpeed;
            data.FoV                = this.cameraComponent.FoV;
            data.ZNear              = this.cameraComponent.ZNear;
            data.ZFar               = this.cameraComponent.ZFar;
            data.ActiveKeyListener  = this.keyboardListenerComponent.Active;

            return data;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            cameraComponent.ReleaseMe();
            keyboardListenerComponent.ReleaseMe();        
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
        public double MovementSpeed     = 0;
        public double RotationSpeed     = 0;
        public float  FoV               = 0;
        public float  ZNear             = 0;
        public float  ZFar              = 0;
        public bool   ActiveKeyListener = false;
    }
    /********************************************************************************/

}
/************************************************************************************/