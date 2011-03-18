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
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// LinkedCamera
    /********************************************************************************/
    class LinkedCamera : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private CameraComponent cameraComponent= null;
        private KeyboardListenerComponent keyboardListenerComponent = null;
        private MouseListenerComponent mouselistenerComponent = null;

        private double movementSpeed = 0;
        private double rotationSpeed = 0;

        private Vector3 position = Vector3.Zero;
        private Vector3 target = Vector3.Zero;

        private Clock clock = TimeControl.CreateClock();
        /****************************************************************************/


        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(CameraComponent cameraComponent,
                         KeyboardListenerComponent keyboardListenerComponent,
                         MouseListenerComponent mouseListenerComponent,
                         double movementSpeed,
                         double rotationSpeed,
                         Vector3 position,
                         Vector3 target)
        {
            this.cameraComponent            = cameraComponent;
            this.keyboardListenerComponent  = keyboardListenerComponent;
            this.mouselistenerComponent     = mouseListenerComponent;

            this.movementSpeed              = movementSpeed;
            this.rotationSpeed              = rotationSpeed;

            this.position = position;
            this.target = target;
            this.World = Matrix.CreateLookAt(this.position, this.target, Vector3.Up);

            this.keyboardListenerComponent.SubscibeKeys(OnKey, Keys.W, Keys.S, Keys.A,
                                                   Keys.D, Keys.Q, Keys.E);


            this.mouselistenerComponent.SubscribeKeys(OnMouseKey, MouseKeyAction.LeftClick);

        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Mouse Key
        /****************************************************************************/
        private void OnMouseKey(MouseKeyAction mouseKeyAction,ExtendedMouseKeyState mouseKeyState)
        {
            Vector3 up = Vector3.Up;
            switch (mouseKeyAction)
            {
                case MouseKeyAction.LeftClick:
                    if (mouseKeyState.IsDown())
                    {
                        position.X += 10;
                        cameraComponent.LookAt(ref position, ref target, ref up);
                    }
                    break;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Key
        /****************************************************************************/
        private void OnKey(Keys key, ExtendedKeyState state)
        {
            if (!state.IsDown()) return;

            Vector3 Up = Vector3.Up;

            Vector3 direction = Vector3.Normalize(target - position);
            direction.Y = 0;

            Vector3 perpendicular = Vector3.Transform(direction, Matrix.CreateRotationY(MathHelper.ToRadians(90.0f)));
            perpendicular = Vector3.Normalize(perpendicular);
            perpendicular.Y = 0;

            switch (key)
            {
                case Keys.W:
                            position += direction * (float)(movementSpeed * clock.DeltaTime.TotalMilliseconds);
                            target += direction * (float)(movementSpeed * clock.DeltaTime.TotalMilliseconds);
                            
                            
                            break;

                case Keys.S:
                            position -= direction * (float)(movementSpeed * clock.DeltaTime.TotalMilliseconds);
                            target -= direction * (float)(movementSpeed * clock.DeltaTime.TotalMilliseconds);
                            
                            
                            break;

                case Keys.A:
                            position += perpendicular * (float)(movementSpeed * clock.DeltaTime.TotalMilliseconds);
                            target += perpendicular * (float)(movementSpeed * clock.DeltaTime.TotalMilliseconds);
                            
                            
                            break;

                case Keys.D:
                            position -= perpendicular * (float)(movementSpeed * clock.DeltaTime.TotalMilliseconds);
                            target -= perpendicular * (float)(movementSpeed * clock.DeltaTime.TotalMilliseconds);
                            
                            
                            break;

                case Keys.Q:
                    position -= target;
                    position = Vector3.Transform(position, Matrix.CreateRotationY((float)(rotationSpeed * clock.DeltaTime.TotalMilliseconds)));
                    position += target;
                    
                    
                    break;

                case Keys.E:
                    position -= target;
                    position = Vector3.Transform(position, Matrix.CreateRotationY((float)(rotationSpeed * clock.DeltaTime.TotalMilliseconds)*-1));
                    position += target;
                    
                    
                    break;


            }
            cameraComponent.LookAt(ref position, ref target, ref Up);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            cameraComponent.ReleaseMe();
            keyboardListenerComponent.ReleaseMe();
            mouselistenerComponent.ReleaseMe();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            LinkedCameraData data = new LinkedCameraData();
            GetData(data);

            data.MovementSpeed = this.movementSpeed;
            data.RotationSpeed = this.rotationSpeed;
            data.FoV = this.cameraComponent.FoV;
            data.ZNear = this.cameraComponent.ZNear;
            data.ZFar = this.cameraComponent.ZFar;
            data.ActiveKeyListener = this.keyboardListenerComponent.Active;
            data.ActiveMouseListener = this.mouselistenerComponent.Active;
            data.position = this.position;
            data.target = this.target;

            return data;
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// LinkedCameraData
    /********************************************************************************/
    [Serializable]
    public class LinkedCameraData : GameObjectInstanceData
    {
        public double MovementSpeed = 0;
        public double RotationSpeed = 0;
        public float FoV = 0;
        public float ZNear = 0;
        public float ZFar = 0;
        public bool ActiveKeyListener = false;
        public bool ActiveMouseListener = false;
        public Vector3 position;
        public Vector3 target;
    }
    /********************************************************************************/

}
/************************************************************************************/