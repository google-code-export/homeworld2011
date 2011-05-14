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
    /// LinkedCamera
    /********************************************************************************/
    class LinkedCamera : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private CameraComponent           cameraComponent           = null;
        private KeyboardListenerComponent keyboardListenerComponent = null;
        private MouseListenerComponent    mouselistenerComponent    = null;

        private float movementSpeed = 0;
        private float rotationSpeed = 0;
        private float zoomSpeed     = 0;
 
        private Vector3 position = Vector3.Zero;
        private Vector3 target   = Vector3.Zero;

        private Clock clock = TimeControl.CreateClock();

        private bool    shiftDown = false;

        private GameObjectInstance tracedObject = null;
        private Vector3            moveToPosition;

        private int  rotateCamera  = -1;
        private bool isOnWindow    = false;
        private bool useCommands   = false;
        private bool movingToPoint = false;
        private bool tracing       = false;

        private bool addToSelection      = false;
        private bool removeFromSelection = false;

        private int maxTop, minTop, maxBottom, minBottom, maxLeft, minLeft, maxRight, minRight;

        private float mouseX, mouseY;  
        private String cursor;
        
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
                                                               Keys.LeftShift, Keys.LeftControl, Keys.LeftAlt);

            this.mouselistenerComponent.SubscribeMouseMove(onMouseMove,MouseMoveAction.Move,MouseMoveAction.Scroll);
            this.mouselistenerComponent.SubscribeKeys(OnMouseKey, MouseKeyAction.MiddleClick,MouseKeyAction.RightClick,MouseKeyAction.LeftClick);

            int screenWidth  = cameraComponent.ScreenWidth;
            int screenHeight = cameraComponent.ScreenHeight;

            this.mercenariesManager = mercenariesManager;
                        
            // TODO: jest na pałę, a powinno być z edytora
            maxTop = 0;
            minTop = (int)(screenHeight * 0.05);

            maxBottom = screenHeight;
            minBottom = (int)(screenHeight * 0.95);

            maxLeft = 0;
            minLeft = (int)(screenWidth * 0.05);

            maxRight = screenWidth;
            minRight = (int)(screenWidth * 0.95);
            
            RequiresUpdate = true;
        }
        /****************************************************************************/
     

        /****************************************************************************/
        /// StartMoveToPoint
        /****************************************************************************/
        public void StartMoveToPoint(Vector3 point)
        {
            moveToPosition = point;
            movingToPoint = true;
            stopTracking();
        }
        /****************************************************************************/
        

        /****************************************************************************/
        /// StopMovingToPoint
        /****************************************************************************/
        public void StopMovingToPoint()
        {
            movingToPoint = false;
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
            tracing = true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Stop Tracing 
        /****************************************************************************/
        public void stopTracking()
        {
            tracing = false;      
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
        /// OnMouseKey
        /****************************************************************************/
        private void OnMouseKey(MouseKeyAction mouseKeyAction, ExtendedMouseKeyState mouseKeyState)
        {
          
            if (mouseKeyState.WasPressed() && mouseKeyAction == MouseKeyAction.MiddleClick)
            {
                if (++rotateCamera == 1)
                {
                    mouselistenerComponent.LockCursor();
                }
                else
                {
                    if (isOnWindow)
                    {
                        CollisionSkin skin;
                        Vector3 direction = Physics.PhysicsUlitities.DirectionFromMousePosition(this.cameraComponent.Projection, this.cameraComponent.View, mouseX, mouseY);
                        Vector3 pos, nor;
                        float dist;
                        bool hit = false;

                        hit = Physics.PhysicsUlitities.RayTest(cameraComponent.Position, cameraComponent.Position + direction * 500, out dist, out skin, out pos, out nor);
                        if (skin != null)
                        {
                            //this.Broadcast(new LowLevelGameFlow.GameObjectClicked((uint)((GameObjectInstance)skin.ExternalData).ID));
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
                    }
                }
            }
            else if (mouseKeyState.WasReleased() && mouseKeyAction == MouseKeyAction.MiddleClick)
            {
                //this.Broadcast(new LowLevelGameFlow.GameObjectReleased());
                if (--rotateCamera == 0) mouselistenerComponent.UnlockCursor();
            }


            if (mouseKeyState.WasPressed() && mouseKeyAction == MouseKeyAction.LeftClick)
            {
                CollisionSkin skin;
                Vector3 direction = Physics.PhysicsUlitities.DirectionFromMousePosition(this.cameraComponent.Projection, this.cameraComponent.View, mouseX, mouseY);
                Vector3 pos, nor;
                float dist;
                Physics.PhysicsUlitities.RayTest(cameraComponent.Position, cameraComponent.Position + direction * cameraComponent.ZFar, out dist, out skin, out pos, out nor);
                
                GameObjectInstance goi = null;

                if (skin != null)
                {
                    if (!useCommands)
                    {
                        if ((skin.ExternalData as GameObjectInstance).Status != GameObjectStatus.Nothing)
                            goi = skin.ExternalData as GameObjectInstance;
                    }
                    else
                    {
                        if ((skin.ExternalData as GameObjectInstance).Status == GameObjectStatus.Mercenary)
                            goi = skin.ExternalData as GameObjectInstance;
                    }
                }

                if      (addToSelection)      SendEvent(new AddToSelectionEvent     (goi),      Priority.Normal, mercenariesManager);
                else if (removeFromSelection) SendEvent(new RemoveFromSelectionEvent(goi),      Priority.Normal, mercenariesManager);
                else                          SendEvent(new SelectedObjectEvent     (goi, pos), Priority.Normal, mercenariesManager);
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
            mouseX     = mouseMoveState.Position.X;
            mouseY     = mouseMoveState.Position.Y;            

            Vector3 perpendicular = Vector3.Normalize(Vector3.Transform(direction, Matrix.CreateRotationY(MathHelper.ToRadians(90.0f))));

            float time = (float)(clock.DeltaTime.TotalMilliseconds);

            switch (mouseMoveAction)
            {
                case MouseMoveAction.Scroll:

                    Vector3 testPosition = position + direction * zoomSpeed * time * mouseMoveState.ScrollDifference;
                    float distance = Vector3.Distance(target, testPosition);
                    if (distance>3.0f && testPosition.Y>target.Y)//kamera musi patrzec z gory w dol, nie odwrotnie. wiec lepiej jej nie przyblizac za duzo
                    {                        
                        if (shiftDown)
                        {
                            Vector3 tmp = position;
                            tmp.Y += zoomSpeed * direction.Y * mouseMoveState.ScrollDifference;// *(Vector3.Distance(position, target) / 10.0f);

                            if (CanIMove(position, tmp))
                            {
                                position.Y += zoomSpeed * direction.Y * mouseMoveState.ScrollDifference;// *(Vector3.Distance(position, target) / 10.0f); 
                            }
                        }
                        else
                        {                            
                            Vector3 tmp = position;
                            tmp += zoomSpeed * direction * mouseMoveState.ScrollDifference;// *(Vector3.Distance(position, target) / 10.0f); 
                               
                            if (CanIMove(position, tmp))
                            {
                                position += zoomSpeed * direction * mouseMoveState.ScrollDifference;// *(Vector3.Distance(position, target) / 10.0f);
                            }
                        }                       
                    }

                    break;


                case MouseMoveAction.Move:
                    {
                        if (rotateCamera == 1)
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
            if (key == Keys.LeftShift)   shiftDown           = state.IsDown();
            if (key == Keys.LeftControl) addToSelection      = state.IsDown();
            if (key == Keys.LeftAlt)     removeFromSelection = state.IsDown();  

            if (key == Keys.Q && state.WasPressed())
            {
                if (++rotateCamera == 1) mouselistenerComponent.LockCursor();
            }
            else if (key == Keys.Q && state.WasReleased())
            {
                if (--rotateCamera == 0) mouselistenerComponent.UnlockCursor();
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

            /***************************************/
            /// Check Objects
            /***************************************/
            CollisionSkin skin;
            Vector3 direction1 = Physics.PhysicsUlitities.DirectionFromMousePosition(this.cameraComponent.Projection, this.cameraComponent.View, mouseX, mouseY);
            Vector3 pos, nor;
            float dist;
            Physics.PhysicsUlitities.RayTest(cameraComponent.Position, cameraComponent.Position + direction1 * cameraComponent.ZFar, out dist, out skin, out pos, out nor);
            if (skin != null)
            {
                if (useCommands)
                {
                    switch (((GameObjectInstance)skin.ExternalData).Status)
                    {
                        case GameObjectStatus.Interesting:  cursor = "QuestionMark"; break;
                        case GameObjectStatus.Pickable:     cursor = "Hand";         break;
                        case GameObjectStatus.Targetable:   cursor = "Target";       break;
                        case GameObjectStatus.Walk:         cursor = "Footsteps";    break;
                        case GameObjectStatus.Mercenary:    cursor = "Person";       break;
                        default: cursor = "Default"; break;
                    }
                }
                else
                {
                    switch (((GameObjectInstance)skin.ExternalData).Status)
                    {
                        case GameObjectStatus.Mercenary: cursor = "Person"; break;
                        default: cursor = "Default"; break;                       
                    }
                }               
            }
            /***************************************/
            
            if (movingToPoint) MoveToPoint();
            if (tracing)       traceTarget();
            
            /***************************************/
            /// Check Borders
            /***************************************/
            if (!(mouseX < minRight && mouseX > minLeft && mouseY < minBottom && mouseY > minTop))
            {
                Vector3 direction     = Vector3.Normalize(target - position);
                Vector3 perpendicular = Vector3.Normalize(Vector3.Transform(direction, Matrix.CreateRotationY(MathHelper.ToRadians(90.0f))));
                
                float time = (float)(deltaTime.TotalMilliseconds);

                direction.Y     = 0;
                perpendicular.Y = 0;                

                Vector3 offset = perpendicular * movementSpeed * time;

                /************************/
                /// Left Border
                /************************/
                if (mouseX < minLeft && mouseX > maxLeft)
                {                                        
                    if (CanIMove(position, position + offset))
                    {
                        cursor    = "Left";
                        position += offset;
                        target   += offset;

                        stopTracking();
                        StopMovingToPoint();                                
                    }
                }
                /************************/


                /************************/
                /// Right Border
                /************************/
                if (mouseX > minRight && mouseX < maxRight)
                {
                    if (CanIMove(position, position - offset))
                    {
                        cursor    = "Right";
                        position -= offset;
                        target   -= offset;

                        stopTracking();
                        StopMovingToPoint();
                    }
                }
                /************************/

                offset = direction * movementSpeed * time;

                /************************/
                /// Top Border
                /************************/
                if (mouseY < minTop && mouseY > maxTop)
                {                    
                    if (CanIMove(position, position + offset))
                    {
                        cursor    = "Up";
                        position += offset;
                        target   += offset;

                        stopTracking();
                        StopMovingToPoint();
                    }
                }
                /************************/


                /************************/
                /// Bottom Border
                /************************/
                if (mouseY > minBottom && mouseY < maxBottom)
                {
                    if (CanIMove(position, position - offset))
                    {
                        cursor    = "Down";
                        position -= offset;
                        target   -= offset;

                        stopTracking();
                        StopMovingToPoint();
                    }
                }
                /************************/

            }
            /***************************************/

            mouselistenerComponent.SetCursor(cursor);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Event
        /****************************************************************************/
        public override void OnEvent(EventsSender sender, EventArgs e)
        {
            if (e.GetType().Equals(typeof(ExSwitchEvent)))
            {
                switch ((e as ExSwitchEvent).name)
                {
                    case "UseCommands": useCommands = (e as ExSwitchEvent).value; break;
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