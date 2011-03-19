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


    /************************************************************************************/
    /// Regions
    /************************************************************************************/
    public struct regions
    {
        public region left, right, top, bottom;
    }
    /************************************************************************************/




    /************************************************************************************/
    /// Region
    /************************************************************************************/
    public struct region
    {
        public int x, y, width, height;
        public region(int X, int Y, int Width, int Height)
        {
            x = X;
            y = Y;
            width = Width;
            height = Height;
        }
    }
    /************************************************************************************/



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

        private float movementSpeed = 0;
        private float rotationSpeed = 0;
        private float zoomSpeed = 0;
 
        private Vector3 position = Vector3.Zero;
        private Vector3 target = Vector3.Zero;

        private Clock clock = TimeControl.CreateClock();

        private bool shiftDown = false;
        private regions mouseRegions;
        /****************************************************************************/


        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(CameraComponent cameraComponent,
                         KeyboardListenerComponent keyboardListenerComponent,
                         MouseListenerComponent mouseListenerComponent,
                         float movementSpeed,
                         float rotationSpeed,
                         float zoomSpeed,
                         Vector3 position,
                         Vector3 target)
        {
            this.cameraComponent            = cameraComponent;
            this.keyboardListenerComponent  = keyboardListenerComponent;
            this.mouselistenerComponent     = mouseListenerComponent;

            this.movementSpeed              = movementSpeed;
            this.rotationSpeed              = rotationSpeed;
            this.zoomSpeed                  = zoomSpeed;
            this.position                   = position;
            this.target                     = target;

            this.World = Matrix.CreateLookAt(this.position, this.target, Vector3.Up);

            this.keyboardListenerComponent.SubscibeKeys(OnKey, Keys.W, Keys.S, Keys.A,
                                                   Keys.D, Keys.Q, Keys.E, Keys.LeftShift);

            this.mouselistenerComponent.SubscribeMouseMove(onMouseMove,MouseMoveAction.Move,MouseMoveAction.Scroll);


            int screenWidth = cameraComponent.ScreenWidth;
            int screenHeight = cameraComponent.ScreenHeight;

            ///domyslne miejsca na ekranie do przesuwania mysza kamery
            mouseRegions.left = new region(0, 0, (int)(screenWidth * 0.05), screenHeight);
            mouseRegions.right = new region((int)(screenWidth * 0.95), 0, (int)(screenWidth * 0.05), screenHeight);
            mouseRegions.top = new region(0, 0, screenWidth, (int)(screenHeight * 0.05));
            mouseRegions.bottom = new region(0, (int)(screenHeight * 0.95), screenWidth, (int)(screenHeight * 0.05));
       
        }
        /****************************************************************************/



        /****************************************************************************/
        /// Regions
        /****************************************************************************/
        public regions Regions
        {
            get { return mouseRegions; }
            set { this.mouseRegions = value; }
        }
        /****************************************************************************/


        /************************************************************************************/
        /// Is Mouse In Region
        /************************************************************************************/
        private bool isMouseInRegion(region testedRegion, ExtendedMouseMovementState mouseState)
        {

            if ((mouseState.Position.X >= testedRegion.x) && (mouseState.Position.Y >= testedRegion.y) && (mouseState.Position.X <= (testedRegion.x + testedRegion.width)) && (mouseState.Position.Y <= (testedRegion.y + testedRegion.height)))
            {
                return true;
            }

            return false;
        }

        /************************************************************************************/




        /****************************************************************************/
        /// On Mouse Move 
        /****************************************************************************/
        private void onMouseMove(MouseMoveAction mouseMoveAction, ExtendedMouseMovementState mouseMoveState)
        {
            Vector3 direction = Vector3.Normalize(target - position);
            

            Vector3 perpendicular = Vector3.Transform(direction, Matrix.CreateRotationY(MathHelper.ToRadians(90.0f)));
            perpendicular = Vector3.Normalize(perpendicular);

            float time = (float)(clock.DeltaTime.TotalMilliseconds);


            switch (mouseMoveAction)
            {
                case MouseMoveAction.Scroll:

                    Vector3 testPosition = position + direction * zoomSpeed * time * mouseMoveState.ScrollDifference;
                    float distance = Vector3.Distance(target, testPosition);

                    if (distance>100 && testPosition.Y>target.Y)//kamera musi patrzec z gory w dol, nie odwrotnie. wiec lepiej jej nie przyblizac za duzo
                    {

                        if (shiftDown)
                        {
                            position.Y += zoomSpeed * direction.Y *mouseMoveState.ScrollDifference ;
                        }
                        else
                        {
                            position += zoomSpeed * direction * mouseMoveState.ScrollDifference;
                        }

                    }

                    break;


                case MouseMoveAction.Move:

                    direction.Y = 0;
                    perpendicular.Y = 0;
                    if (isMouseInRegion(mouseRegions.left, mouseMoveState))
                    {
                        position += perpendicular * movementSpeed * time;
                        target += perpendicular * movementSpeed * time;
                    }

                    if (isMouseInRegion(mouseRegions.right, mouseMoveState))
                    {
                        position -= perpendicular * movementSpeed * time;
                        target -= perpendicular * movementSpeed * time;

                    }

                    if (isMouseInRegion(mouseRegions.top, mouseMoveState))
                    {
                        position += direction * movementSpeed * time;
                        target += direction * movementSpeed * time;

                    }

                    if (isMouseInRegion(mouseRegions.bottom, mouseMoveState))
                    {
                        position -= direction * movementSpeed * time;
                        target -= direction * movementSpeed * time;

                    }

                    break;
            }


            cameraComponent.LookAt(position, target, Vector3.Up);
        }
        /****************************************************************************/




        /****************************************************************************/
        /// On Key
        /****************************************************************************/
        private void OnKey(Keys key, ExtendedKeyState state)
        {
            if (key == Keys.LeftShift)
            {
                shiftDown = state.IsDown();
            }

            if (!state.IsDown()) return;

            Vector3 direction = Vector3.Normalize(target - position);
            direction.Y = 0;

            Vector3 perpendicular = Vector3.Transform(direction, Matrix.CreateRotationY(MathHelper.ToRadians(90.0f)));
            perpendicular = Vector3.Normalize(perpendicular);
            perpendicular.Y = 0;

            float time = (float)(clock.DeltaTime.TotalMilliseconds);

            switch (key)
            {
                case Keys.W:
                            position += direction * time * movementSpeed ;
                            target += direction * time * movementSpeed ;
                            
                            
                            break;

                case Keys.S:
                            position -= direction * time * movementSpeed ;
                            target -= direction * time * movementSpeed ;
                            
                            
                            break;

                case Keys.A:
                            position += perpendicular * time * movementSpeed ;
                            target += perpendicular * time * movementSpeed ;
                            
                            
                            break;

                case Keys.D:
                            position -= perpendicular * time * movementSpeed ;
                            target -= perpendicular * time * movementSpeed ;
                            
                            
                            break;

                case Keys.Q:
                    position -= target;
                    position = Vector3.Transform(position, Matrix.CreateRotationY(time* rotationSpeed ));
                    position += target;
                    
                    
                    break;

                case Keys.E:
                    position -= target;
                    position = Vector3.Transform(position, Matrix.CreateRotationY(time* rotationSpeed *-1));
                    position += target;
                    
                    
                    break;
                
            



            }
            cameraComponent.LookAt(position,target, Vector3.Up);
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
            data.ZoomSpeed = this.zoomSpeed;
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
        public float MovementSpeed = 0;
        public float RotationSpeed = 0;
        public float ZoomSpeed = 0;
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