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
using PlagueEngine.EventsSystem;

using JigLibX.Geometry;
using JigLibX.Collision;
using JigLibX.Physics;

/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// Regions
    /********************************************************************************/
    public struct regions
    {
        public region left, right, top, bottom;
    }
    /********************************************************************************/


    /********************************************************************************/
    /// Region
    /********************************************************************************/
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
    /********************************************************************************/
    

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
        private float zoomSpeed     = 0;
 
        private Vector3 position = Vector3.Zero;
        private Vector3 target   = Vector3.Zero;

        private Clock clock = TimeControl.CreateClock();

        private bool    shiftDown = false;
        private regions mouseRegions;

        private GameObjectInstance tracedObject = null;
        private uint               frameCounterID = 0;
        private bool               middleButton = false;
        private bool               rmb = false;
        private bool               isOnWindow = false;
        
        private float mouseX, mouseY;
        
        private Vector3 moveToPosition;

        private GameObjectInstance mercenariesManager = null;
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
                         Vector3 target,
                         GameObjectInstance mercenariesManager)
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
                                                               Keys.D, Keys.Q, Keys.E, 
                                                               Keys.LeftShift, Keys.F1, Keys.F2);

            this.mouselistenerComponent.SubscribeMouseMove(onMouseMove,MouseMoveAction.Move,MouseMoveAction.Scroll);
            this.mouselistenerComponent.SubscribeKeys(OnMouseKey, MouseKeyAction.MiddleClick,MouseKeyAction.RightClick);

            int screenWidth  = cameraComponent.ScreenWidth;
            int screenHeight = cameraComponent.ScreenHeight;

            this.mercenariesManager = mercenariesManager;

            ///domyslne miejsca na ekranie do przesuwania mysza kamery
            mouseRegions.left   = new region(0, 0, (int)(screenWidth * 0.05), screenHeight);
            mouseRegions.right  = new region((int)(screenWidth * 0.95), 0, (int)(screenWidth * 0.05), screenHeight);
            mouseRegions.top    = new region(0, 0, screenWidth, (int)(screenHeight * 0.05));
            mouseRegions.bottom = new region(0, (int)(screenHeight * 0.95), screenWidth, (int)(screenHeight * 0.05));       

            RequiresUpdate = true;
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
        

        /****************************************************************************/
        /// StartMoveToPoint
        /****************************************************************************/
        public void StartMoveToPoint(Vector3 point)
        {
            moveToPosition = point;
            stopTracking();
            if (frameCounterID == 0)
            {
                frameCounterID = TimeControl.CreateFrameCounter(1, -1, MoveToPoint);
            }
        }
        /****************************************************************************/
        

        /****************************************************************************/
        /// StopMovingToPoint
        /****************************************************************************/
        public void StopMovingToPoint()
        {
            TimeControl.ReleaseFrameCounter(frameCounterID);
            frameCounterID = 0;
        }
        /****************************************************************************/
        

        /****************************************************************************/
        /// MoveToPoint
        /****************************************************************************/
        private void MoveToPoint()
        {

            if (moveToPosition != target)
            {
                if (Vector3.Distance(target, moveToPosition) < 0.01f)
                {
                    target = moveToPosition;
                }
                else
                {
                    Vector3 distanceVec = moveToPosition - target;
                    target += distanceVec / 50.0f;
                    position += distanceVec / 50.0f;
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// SetTarget
        /****************************************************************************/
        public void setTarget(GameObjectInstance target)
        {
            this.tracedObject = target;            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Start Tracing
        /****************************************************************************/
        public void startTracing()
        {
            StopMovingToPoint();
            if (tracedObject != null && frameCounterID==0 )
            {
                frameCounterID = TimeControl.CreateFrameCounter(1, -1, traceTarget);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Stop Tracing 
        /****************************************************************************/
        public void stopTracking()
        {                        
            TimeControl.ReleaseFrameCounter(frameCounterID);
            frameCounterID = 0;            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// RealeseTarget
        /****************************************************************************/
        public void RealeseTarget()
        {
            tracedObject = null;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Trace Target
        /****************************************************************************/
        private void traceTarget()
        {           
            Vector3 targetPosition = tracedObject.World.Translation;
           
            if (targetPosition != target)
            {
                if (Vector3.Distance(target, targetPosition) < 0.01f)
                {
                    target = targetPosition;
                }
                else
                {
                    Vector3 distanceVec = targetPosition - target;
                    target += distanceVec / 50.0f;
                    position += distanceVec / 50.0f;
                }
            }
   
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Is Mouse In Region
        /****************************************************************************/
        private bool isMouseInRegion(region testedRegion, ExtendedMouseMovementState mouseState)
        {
            if ((mouseState.Position.X >= testedRegion.x) && 
                (mouseState.Position.Y >= testedRegion.y) && 
                (mouseState.Position.X <= (testedRegion.x + testedRegion.width)) && 
                (mouseState.Position.Y <= (testedRegion.y + testedRegion.height)))
            {
                return true;
            }
            
            return false;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// OnMouseKey
        /****************************************************************************/
        private void OnMouseKey(MouseKeyAction mouseKeyAction, ExtendedMouseKeyState mouseKeyState)
        {

            if (mouseKeyState.WasPressed() && mouseKeyAction == MouseKeyAction.RightClick)
            {
                rmb = true;
                mouselistenerComponent.LockCursor();
            }
            if (mouseKeyState.WasReleased() && mouseKeyAction == MouseKeyAction.RightClick)
            {
                rmb = false;
                mouselistenerComponent.UnlockCursor();
            }
            

            if (mouseKeyState.IsDown() && mouseKeyAction == MouseKeyAction.MiddleClick)
            {
                if (middleButton == false && isOnWindow)
                {
                    CollisionSkin skin;
                    Vector3 direction = Physics.PhysicsUlitities.DirectionFromMousePosition(this.cameraComponent.Projection, this.cameraComponent.View, mouseX, mouseY);
                    Vector3 pos, nor;
                    float dist;
                    bool hit = false;
#if DEBUG
                    Diagnostics.PushLog(direction.ToString());
#endif
                    hit = Physics.PhysicsUlitities.RayTest(cameraComponent.Position, cameraComponent.Position + direction * 500, out dist, out skin, out pos, out nor);
                    if (skin != null)
                    {
#if DEBUG
                        Diagnostics.PushLog(pos.ToString());
#endif
                        this.Broadcast(new LowLevelGameFlow.GameObjectClicked((uint)((GameObjectInstance)skin.ExternalData).ID));

                        if (skin.ExternalData.GetType().Equals(typeof(Terrain)))
                        {
                            StartMoveToPoint(pos);
                        }
                        else
                        {
                            setTarget((GameObjectInstance)skin.ExternalData);
                            startTracing();
                        }
                        
                    }

                    middleButton = true;
                }

            }
            else if (mouseKeyState.WasReleased() && mouseKeyAction == MouseKeyAction.MiddleClick)
            {

                middleButton = false;
                this.Broadcast(new LowLevelGameFlow.GameObjectReleased());
            }

            if (mouseKeyState.WasPressed() && mouseKeyAction == MouseKeyAction.MiddleClick)
            {
                CollisionSkin skin;
                Vector3 direction = Physics.PhysicsUlitities.DirectionFromMousePosition(this.cameraComponent.Projection, this.cameraComponent.View, mouseX, mouseY);
                Vector3 pos, nor;
                float dist;
                Physics.PhysicsUlitities.RayTest(cameraComponent.Position, cameraComponent.Position + direction * cameraComponent.ZFar, out dist, out skin, out pos, out nor);
                if (skin != null)
                {
                    SendEvent(new SelectedObjectEvent((GameObjectInstance)skin.ExternalData, pos), Priority.Normal, mercenariesManager);
                }
            }
         
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CanIMove
        /****************************************************************************/
        private bool CanIMove(Vector3 position, Vector3 desiredPosition)
        {
            CollisionSkin skin;
            Vector3 pos, nor;  
            float dist;

            Physics.PhysicsUlitities.RayTest(position, desiredPosition+(desiredPosition-position)*10, out dist, out skin, out pos, out nor);

            if (skin == null)
            {
                return true;
            }
            return false;
        }
        /****************************************************************************/         


        /****************************************************************************/
        /// On Mouse Move 
        /****************************************************************************/
        private void onMouseMove(MouseMoveAction mouseMoveAction, ExtendedMouseMovementState mouseMoveState)
        {
            Vector3 direction = Vector3.Normalize(target - position);

            isOnWindow = mouseMoveState.IsOnWindow;
            mouseX = mouseMoveState.Position.X;
            mouseY = mouseMoveState.Position.Y;
            Vector3 perpendicular = Vector3.Transform(direction, Matrix.CreateRotationY(MathHelper.ToRadians(90.0f)));
            perpendicular = Vector3.Normalize(perpendicular);

            float time = (float)(clock.DeltaTime.TotalMilliseconds);

            switch (mouseMoveAction)
            {
                case MouseMoveAction.Scroll:

                    Vector3 testPosition = position + direction * zoomSpeed/10 * time * mouseMoveState.ScrollDifference;
                    float distance = Vector3.Distance(target, testPosition);
                    if (distance>3.0f && testPosition.Y>target.Y)//kamera musi patrzec z gory w dol, nie odwrotnie. wiec lepiej jej nie przyblizac za duzo
                    {

                        if (shiftDown)
                        {
                            Vector3 tmp = position;
                            tmp.Y += zoomSpeed * direction.Y * mouseMoveState.ScrollDifference;

                            if (CanIMove(position, tmp))
                            {
                                position.Y += zoomSpeed * direction.Y * mouseMoveState.ScrollDifference;
                            }
                        }
                        else
                        {
                            
                                Vector3 tmp = position;
                                tmp += zoomSpeed * direction * mouseMoveState.ScrollDifference;
                               
                                if (CanIMove(position, tmp))
                                {
                                    position += zoomSpeed * direction * mouseMoveState.ScrollDifference;
                                }
                            }
                       

                    }

                    break;


                case MouseMoveAction.Move:
                    {
                        bool flag = true;

                        if (rmb)
                        {
                            Vector3 tmp = position;
                            tmp -= target;
                            tmp = Vector3.Transform(tmp, Matrix.CreateRotationY(rotationSpeed * (mouseMoveState.Position.X - mouseMoveState.OldPosition.X)));
                            tmp += target;

                            if (CanIMove(position, tmp))
                            {
                                position -= target;
                                position = Vector3.Transform(position, Matrix.CreateRotationY(rotationSpeed * (mouseMoveState.Position.X - mouseMoveState.OldPosition.X)));
                                position += target;
                            }
                        }

                        direction.Y = 0;
                        perpendicular.Y = 0;
                        if (isMouseInRegion(mouseRegions.left, mouseMoveState))
                        {
                            Vector3 tmp = position;
                            tmp += perpendicular * movementSpeed * time;
                            if (CanIMove(position, tmp))
                            {
                                mouselistenerComponent.SetCursor("Left");

                                position += perpendicular * movementSpeed * time;
                                target += perpendicular * movementSpeed * time;

                                stopTracking();
                                StopMovingToPoint();
                                flag = false;
                            }

                        }

                        if (isMouseInRegion(mouseRegions.right, mouseMoveState))
                        {
                            Vector3 tmp = position;
                            tmp -= perpendicular * movementSpeed * time;
                            if (CanIMove(position, tmp))
                            {
                                mouselistenerComponent.SetCursor("Right");

                                position -= perpendicular * movementSpeed * time;
                                target -= perpendicular * movementSpeed * time;

                                stopTracking();
                                StopMovingToPoint();
                                flag = false;
                            }

                        }

                        if (isMouseInRegion(mouseRegions.top, mouseMoveState))
                        {
                            Vector3 tmp = position;
                            tmp += direction * movementSpeed * time;
                            if (CanIMove(position, tmp))
                            {
                                mouselistenerComponent.SetCursor("Up");

                                position += direction * movementSpeed * time;
                                target += direction * movementSpeed * time;

                                stopTracking();
                                StopMovingToPoint();
                                flag = false;
                            }

                        }

                        if (isMouseInRegion(mouseRegions.bottom, mouseMoveState))
                        {
                            Vector3 tmp = position;
                            tmp -= direction * movementSpeed * time;
                            if (CanIMove(position, tmp))
                            {
                                mouselistenerComponent.SetCursor("Down");

                                position -= direction * movementSpeed * time;
                                target -= direction * movementSpeed * time;

                                stopTracking();
                                StopMovingToPoint();
                                flag = false;
                            }

                        }

                        if (flag) mouselistenerComponent.SetCursor("Default");
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
            //perpendicular = Vector3.Normalize(perpendicular);
            perpendicular.Y = 0;

            float time = (float)(clock.DeltaTime.TotalMilliseconds);
            Vector3 tmp = position;

            switch (key)
            {
                case Keys.W:

                            tmp += direction * time * movementSpeed;

                            if (CanIMove(position, tmp))
                            {
                                position += direction * time * movementSpeed;
                                target += direction * time * movementSpeed;
                                stopTracking();
                            }
                            break;

                case Keys.S:

                            tmp -= direction * time * movementSpeed ;
                            if (CanIMove(position, tmp))
                            {
                                position -= direction * time * movementSpeed;
                                target -= direction * time * movementSpeed;

                                stopTracking();
                            }
                            break;

                case Keys.A:

                            tmp += perpendicular * time * movementSpeed;
                            if (CanIMove(position, tmp))
                            {
                                position += perpendicular * time * movementSpeed;
                                target += perpendicular * time * movementSpeed;
                                stopTracking();
                            }
                            break;

                case Keys.D:

                            tmp -= perpendicular * time * movementSpeed;
                            if (CanIMove(position, tmp))
                            {
                                position -= perpendicular * time * movementSpeed;
                                target -= perpendicular * time * movementSpeed;

                                stopTracking();
                            }
                            break;

                case Keys.Q:

                    tmp -= target;
                    tmp = Vector3.Transform(tmp, Matrix.CreateRotationY(time * rotationSpeed));
                    tmp += target;

                    if (CanIMove(position, tmp))
                    {
                        position -= target;
                        position = Vector3.Transform(position, Matrix.CreateRotationY(time * rotationSpeed));
                        position += target;
                    }
                    break;

                case Keys.E:

                    tmp -= target;
                    tmp = Vector3.Transform(tmp, Matrix.CreateRotationY(time * rotationSpeed *-1));
                    tmp += target;
                    if (CanIMove(position, tmp))
                    {
                        position -= target;
                        position = Vector3.Transform(position, Matrix.CreateRotationY(time * rotationSpeed * -1));
                        position += target;
                    }
                    
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
            data.Position = this.position;
            data.Target = this.target;
            data.MercenariesManager = this.mercenariesManager.ID;

            return data;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update
        /****************************************************************************/
        public override void Update(TimeSpan deltaTime)
        {
            CollisionSkin skin;
            Vector3 direction = Physics.PhysicsUlitities.DirectionFromMousePosition(this.cameraComponent.Projection, this.cameraComponent.View, mouseX, mouseY);
            Vector3 pos, nor;
            float dist;
            Physics.PhysicsUlitities.RayTest(cameraComponent.Position, cameraComponent.Position + direction * cameraComponent.ZFar, out dist, out skin, out pos, out nor);
            if (skin != null)
            {
                switch (((GameObjectInstance)skin.ExternalData).Status)
                {
                    case GameObjectStatus.Interesting: mouselistenerComponent.SetCursor("QuestionMark"); break;
                    case GameObjectStatus.Pickable:    mouselistenerComponent.SetCursor("Hand");         break;
                    case GameObjectStatus.Targetable:  mouselistenerComponent.SetCursor("Target");       break;
                    case GameObjectStatus.Mercenary:   mouselistenerComponent.SetCursor("Person");       break;
                }
            }
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
        public Vector3 Target { get; set; }

        public float movementSpeed
        {            
            set { this.MovementSpeed = value; }
            get { return MovementSpeed; }
        }

        public float rotationSpeed
        {
            set { this.RotationSpeed = value; }
            get { return RotationSpeed; }
        }
        public float zoomSpeed
        {
            set { this.ZoomSpeed = value; }
            get { return ZoomSpeed; }
        }
        public float FOV
        {
            set { this.FoV = value; }
            get { return FoV; }
        }
        public float zNear
        {
            set { this.ZNear = value; }
            get { return ZNear; }
        }
        public float zFar
        {
            set { this.ZFar = value; }
            get { return ZFar; }
        }
        public bool activeKeyListener
        {
            set { this.ActiveKeyListener = value; }
            get { return ActiveKeyListener; }
        }
        public bool activeMouseListener
        {
            set { this.ActiveMouseListener = value; }
            get { return ActiveMouseListener; }
        }

        public uint MercenariesManager { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/