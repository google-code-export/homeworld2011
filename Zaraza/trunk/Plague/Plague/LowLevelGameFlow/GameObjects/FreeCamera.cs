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
using PlagueEngine.EventsSystem;

using JigLibX.Geometry;
using JigLibX.Collision;
using JigLibX.Physics;

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

        private float                     mouseX, mouseY;
        ConstraintWorldPoint              objectController = new ConstraintWorldPoint();
        ConstraintVelocity                damperController = new ConstraintVelocity();
        bool                              middleButton = false;
        float                             camPickDistance = 0;
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

            this.mouseListenerComponent.SubscribeKeys     (OnMouseKey,  MouseKeyAction.RightClick,MouseKeyAction.LeftClick);
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
            if (mouseKeyState.WasPressed() && mouseKeyAction==MouseKeyAction.RightClick)
            {
                rotation = true;
                mouseListenerComponent.LockCursor();
            }
            else if (mouseKeyState.WasReleased() && mouseKeyAction == MouseKeyAction.RightClick)
            {
                rotation = false;
                mouseListenerComponent.UnlockCursor();            
            }
            
            
            
            if (mouseKeyState.IsDown() && mouseKeyAction == MouseKeyAction.LeftClick)
            {
                
                if (middleButton == false)
                {
                    CollisionSkin skin;
                    Vector3 direction = Physics.PhysicsUlitities.DirectionFromMousePosition(this.cameraComponent.Projection, this.cameraComponent.View, mouseX, mouseY);
                    Vector3 pos, nor;
                    float dist;
                    bool hit = false;
                    hit = Physics.PhysicsUlitities.RayTest(cameraComponent.Position, cameraComponent.Position + direction * 500, out dist, out skin, out pos, out nor);
                    if (skin != null)
                    {

                        this.Broadcast(new LowLevelGameFlow.GameObjectClicked((uint)((GameObjectInstance)skin.ExternalData).ID));
                    }
                    if (hit)
                    {


                        Vector3 delta = pos - skin.Owner.Position;
                        delta = Vector3.Transform(delta, Matrix.Transpose(skin.Owner.Orientation));

                        camPickDistance = (cameraComponent.Position - pos).Length();


                        skin.Owner.SetActive();
                        objectController.Destroy();
                        damperController.Destroy();
                        objectController.Initialise(skin.Owner, delta, pos);
                        damperController.Initialise(skin.Owner, ConstraintVelocity.ReferenceFrame.Body, Vector3.Zero, Vector3.Zero);
                        objectController.EnableConstraint();
                        damperController.EnableConstraint();

                    }


                    middleButton = true;
                }

                if (objectController.IsConstraintEnabled && (objectController.Body != null))
                {
                    Vector3 delta = objectController.Body.Position - cameraComponent.Position;
                    Vector3 ray = Physics.PhysicsUlitities.DirectionFromMousePosition(cameraComponent.Projection, cameraComponent.View, mouseX, mouseY);

                    Vector3 result = cameraComponent.Position + camPickDistance * ray;


                    objectController.WorldPosition = result;
                    objectController.Body.SetActive();
                }
            }
            else if(mouseKeyState.WasReleased() && mouseKeyAction == MouseKeyAction.LeftClick)
            {

                objectController.DisableConstraint();
                damperController.DisableConstraint();
                middleButton = false;
                this.Broadcast(new LowLevelGameFlow.GameObjectReleased());
            }
         
            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Mouse Move
        /****************************************************************************/
        private void OnMouseMove(MouseMoveAction mouseMoveAction, ExtendedMouseMovementState mouseMovementState)
        {
            mouseX = mouseMovementState.Position.X;
            mouseY = mouseMovementState.Position.Y;

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
            mouseListenerComponent.ReleaseMe();
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

        new public Vector3 Position
        {
            get { return Matrix.Invert(this.World).Translation; }
            set { this.World = Matrix.CreateTranslation(value); }
        }


        [CategoryAttribute("Input")]
        public bool ActiveKeyListener { set; get; }
        [CategoryAttribute("Input")]
        public bool ActiveMouseListener { set; get; }

    }

    /********************************************************************************/

}
/************************************************************************************/