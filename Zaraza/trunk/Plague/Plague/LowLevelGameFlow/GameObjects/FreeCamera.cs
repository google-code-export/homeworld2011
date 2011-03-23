using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
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
        private MouseListenerComponent    mouseListenerComponent    = null;

        private float                     movementSpeed             = 0;
        private float                     rotationSpeed             = 0;
        private bool                      rotation                  = false;

        // TODO: Zastanowić się, czy da się rozwiązać to lepiej z tym zegarem
        private Clock                     clock                     = TimeControl.CreateClock();
        /****************************************************************************/


        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(CameraComponent            cameraComponent, 
                         KeyboardListenerComponent  keyboardListenerComponent,
                         MouseListenerComponent     mouseListenerComponent,
                         Matrix                     world, 
                         float                      movementSpeed,
                         float                      rotationSpeed)
        {
            this.cameraComponent            = cameraComponent;
            this.keyboardListenerComponent  = keyboardListenerComponent;
            this.mouseListenerComponent     = mouseListenerComponent;
            this.World                      = world;
            this.movementSpeed              = movementSpeed;
            this.rotationSpeed              = rotationSpeed;

            this.keyboardListenerComponent.SubscibeKeys(OnKey, Keys.W, Keys.S, Keys.A, 
                                                               Keys.D, Keys.Q, Keys.E, 
                                                               Keys.PageUp, Keys.PageDown, 
                                                               Keys.Up,     Keys.Down, 
                                                               Keys.Right,  Keys.Left);

            this.mouseListenerComponent.SubscribeKeys     (OnMouseKey,  MouseKeyAction.RightClick);
            this.mouseListenerComponent.SubscribeMouseMove(OnMouseMove, MouseMoveAction.Move);
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
                    cameraComponent.MoveRight((float)((movementSpeed * clock.DeltaTime.TotalMilliseconds)) * -1);
                    break;

                case Keys.D:
                    cameraComponent.MoveRight((float)((movementSpeed * clock.DeltaTime.TotalMilliseconds)));
                    break;

                case Keys.Q:
                    cameraComponent.MoveUp((float)((movementSpeed * clock.DeltaTime.TotalMilliseconds)) * -1);
                    break;

                case Keys.E:
                    cameraComponent.MoveUp((float)((movementSpeed * clock.DeltaTime.TotalMilliseconds)));
                    break;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Mouse Key
        /****************************************************************************/
        private void OnMouseKey(MouseKeyAction mouseKeyAction, ExtendedMouseKeyState mouseKeyState)
        {
            if (mouseKeyState.WasPressed())
            {
                rotation = true;
                mouseListenerComponent.LockCursor();
            }
            else if (mouseKeyState.WasReleased())
            {
                rotation = false;
                mouseListenerComponent.UnlockCursor();            
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Mouse Move
        /****************************************************************************/
        private void OnMouseMove(MouseMoveAction mouseMoveAction, ExtendedMouseMovementState mouseMovementState)
        {
            if (rotation && mouseMovementState.Moved)
            {
                cameraComponent.RotateY(-rotationSpeed * mouseMovementState.Difference.X);
                cameraComponent.Pitch  (-rotationSpeed * mouseMovementState.Difference.Y);
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

            data.MovementSpeed       = this.movementSpeed;
            data.RotationSpeed       = this.rotationSpeed;
            data.FoV                 = MathHelper.ToDegrees(cameraComponent.FoV);
            data.ZNear               = this.cameraComponent.ZNear;
            data.ZFar                = this.cameraComponent.ZFar;
            data.ActiveKeyListener   = this.keyboardListenerComponent.Active;
            data.ActiveMouseListener = this.mouseListenerComponent.Active;

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
        [CategoryAttribute("Movement")]
        public float MovementSpeed { set; get; }
        [CategoryAttribute("Movement")]
        public float RotationSpeed { set; get; }

        [CategoryAttribute("Perspective")]
        public float FoV { set; get; }
        [CategoryAttribute("Perspective")]
        public float ZNear { set; get; }
        [CategoryAttribute("Perspective")]
        public float ZFar { set; get; }

        [CategoryAttribute("Input")]
        public bool ActiveKeyListener { set; get; }
        [CategoryAttribute("Input")]
        public bool ActiveMouseListener { set; get; }
    }

    /********************************************************************************/

}
/************************************************************************************/