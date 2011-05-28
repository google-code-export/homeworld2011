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
using PlagueEngine.Tools;
using PlagueEngine.Rendering;

using JigLibX.Geometry;
using JigLibX.Collision;
using JigLibX.Physics;
using PlagueEngine.Physics;

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
        public CameraComponent cameraComponent = null;
        public KeyboardListenerComponent keyboardListenerComponent = null;
        public MouseListenerComponent mouseListenerComponent = null;

        private float                     movementSpeed             = 0;
        private float                     rotationSpeed             = 0;
        private bool                      rotation                  = false;

        private Clock                     clock                     = TimeControl.CreateClock();

        private float                     mouseX, mouseY;
        private float                     mouseXclicked, mouseYclicked;
        private bool                      clicked = false;
        private bool                      isOnWindow = false;
        ConstraintWorldPoint              objectController = new ConstraintWorldPoint();
        ConstraintVelocity                damperController = new ConstraintVelocity();
        bool                              middleButton = false;
        float                             camPickDistance = 0;

        internal static Renderer renderer = null;
        internal static GameObjectEditorWindow editor = null;

        //poruszanie obiektami
        bool moveObjectEnabled = true;
        bool local = false;
        float distance=0.0f;
        float a = 0.11f;
        bool pressed = false;
        GameObjectInstance selectedGameObject = null;
        CollisionSkin skin = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(CameraComponent            cameraComponent, 
                         KeyboardListenerComponent  keyboardListenerComponent,
                         MouseListenerComponent     mouseListenerComponent,
                         float                      movementSpeed,
                         float                      rotationSpeed,
                         bool                       current)
        {
            this.cameraComponent            = cameraComponent;
            this.keyboardListenerComponent  = keyboardListenerComponent;
            this.mouseListenerComponent     = mouseListenerComponent;
            this.movementSpeed              = movementSpeed;
            this.rotationSpeed              = rotationSpeed;

            this.keyboardListenerComponent.SubscibeKeys(OnKey, Keys.W, Keys.S, Keys.A, 
                                                               Keys.D, Keys.Q, Keys.E, 
                                                               Keys.R,
                                                               Keys.PageUp, Keys.PageDown, 
                                                               Keys.Up,     Keys.Down, 
                                                               Keys.Right,  Keys.Left,
                                                               Keys.F11,Keys.F12,
                                                               Keys.PageDown,Keys.PageDown,
                                                               Keys.OemPlus,Keys.OemMinus);

            this.mouseListenerComponent.SubscribeKeys     (OnMouseKey,  MouseKeyAction.RightClick,MouseKeyAction.LeftClick);
            this.mouseListenerComponent.SubscribeMouseMove(OnMouseMove, MouseMoveAction.Move);

            if (current) cameraComponent.SetAsCurrent();

            cameraComponent.ForceUpdate();
        }
        /****************************************************************************/



        private void disablePhysics()
        {
            object field = selectedGameObject.GetType().GetField("body");
            if (field != null)
            {
                field = selectedGameObject.GetType().GetField("body").GetValue(selectedGameObject);
                if (typeof(RigidBodyComponent).IsAssignableFrom(field.GetType()))
                {
                    RigidBodyComponent body = (RigidBodyComponent)field;
                    body.DisableBody();

                }
                else if (typeof(CollisionSkinComponent).IsAssignableFrom(field.GetType()))
                {
                    CollisionSkinComponent body = (CollisionSkinComponent)field;
                    body.Disable();

                }
            }
        }

        private void enablePhysics()
        {
            object field = selectedGameObject.GetType().GetField("body");
            if (field != null)
            {
                field = selectedGameObject.GetType().GetField("body").GetValue(selectedGameObject);
                if (typeof(RigidBodyComponent).IsAssignableFrom(field.GetType()))
                {
                    RigidBodyComponent body = (RigidBodyComponent)field;
                    body.EnableBody();

                }
                else if (typeof(CollisionSkinComponent).IsAssignableFrom(field.GetType()))
                {
                    CollisionSkinComponent body = (CollisionSkinComponent)field;
                    body.Enable();

                }
            }
        }

        /****************************************************************************/
        /// On Key
        /****************************************************************************/
        private void OnKey(Keys key, ExtendedKeyState state)
        {


            if (state.WasPressed())
            {
                if (key == Keys.Left || key == Keys.Right || key == Keys.Up || key == Keys.Down || key == Keys.PageDown || key == Keys.PageUp)
                {
                    pressed = true;
                    disablePhysics();
                }
                if (key == Keys.F11)
                {
                    editor.moveObject = !editor.moveObject;
                    moveObjectEnabled = !moveObjectEnabled;
                   
                }
                if (key == Keys.F12)
                {

                    local = !local;
                }
            }

            if (state.WasReleased())
            {
                if (key == Keys.Left || key == Keys.Right || key == Keys.Up || key == Keys.Down || key == Keys.PageDown || key == Keys.PageUp)
                {
                    pressed = false;
                    enablePhysics();
                }
            }

            if (pressed && state.IsDown() && (key == Keys.Left || key == Keys.Right || key == Keys.Up || key == Keys.Down || key == Keys.PageDown || key == Keys.PageUp))
            {
                distance = a;
            }


            if (state.IsUp()) return;

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


                case Keys.Left:
                    if (selectedGameObject != null)
                    {
                        if (moveObjectEnabled)//czy poruszanie
                        {
                            if (!local)
                            {
                                selectedGameObject.World *= Matrix.CreateTranslation(new Vector3(distance, 0, 0));
                            }
                            else
                            {
                                selectedGameObject.World *= Matrix.CreateTranslation(selectedGameObject.World.Left * distance); 
                            }
                        }
                        else//obrot
                        {
                            Rotate(Vector3.Right, distance/20);
                        }
                    }
                    break;
                case Keys.Right:
                    if (selectedGameObject != null)
                    {
                        if (moveObjectEnabled)//czy poruszanie
                        {
                            if (!local)
                            {
                                selectedGameObject.World *= Matrix.CreateTranslation(new Vector3(-distance, 0, 0));
                            }
                            else
                            {
                                selectedGameObject.World *= Matrix.CreateTranslation(selectedGameObject.World.Right * distance); 

                            }
                        }
                        else//obrot
                        {
                            Rotate(Vector3.Right, -distance / 20);
                        }
                    }
                    break;
                case Keys.Up:
                    if (selectedGameObject != null)
                    {
                        if (moveObjectEnabled)//czy poruszanie
                        {
                            if (!local)
                            {
                                selectedGameObject.World *= Matrix.CreateTranslation(new Vector3(0, 0, distance));
                            }
                            else
                            {
                                selectedGameObject.World *= Matrix.CreateTranslation(selectedGameObject.World.Forward * distance); 

                            }
                        }
                        else//obrot
                        {
                            Rotate(Vector3.Forward, distance / 20);
                        }
                    }
                    break;
                case Keys.Down:
                    if (selectedGameObject != null)
                    {
                        if (moveObjectEnabled)//czy poruszanie
                        {
                            if (!local)
                            {
                                selectedGameObject.World *= Matrix.CreateTranslation(new Vector3(0, 0, -distance));
                            }
                            else
                            {
                                selectedGameObject.World *= Matrix.CreateTranslation(selectedGameObject.World.Backward * distance); 

                            }
                        }
                        else//obrot
                        {
                            Rotate(Vector3.Forward, -distance / 20);
                        }
                    }
                    break;
                case Keys.PageUp:
                    if (selectedGameObject != null)
                    {
                        if (moveObjectEnabled)//czy poruszanie
                        {
                            if (!local)
                            {
                                selectedGameObject.World *= Matrix.CreateTranslation(new Vector3(0,distance,0));
                            }
                            else
                            {
                                selectedGameObject.World *= Matrix.CreateTranslation(selectedGameObject.World.Up * distance); 

                            }
                        }
                        else//obrot
                        {
                            Rotate(Vector3.Up, distance / 20);
                        }
                    }
                    break;
                case Keys.PageDown:
                    if (selectedGameObject != null)
                    {
                        if (moveObjectEnabled)//czy poruszanie
                        {
                            if (!local)
                            {
                                selectedGameObject.World *= Matrix.CreateTranslation(new Vector3(0, -distance,0));
                            }
                            else
                            {
                                selectedGameObject.World *= Matrix.CreateTranslation(selectedGameObject.World.Down * distance); 

                            }
                        }
                        else//obrot
                        {
                            Rotate(Vector3.Up, -distance / 20);
                        }
                    }
                    break;


                case Keys.OemPlus:
                    a = a * 1.05f;
                    Diagnostics.PushLog(a.ToString());
                    break;

                case Keys.OemMinus:
                    a = a * 0.95f;
                    break;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        private void Rotate(Vector3 vector, float angle)
        {
            Quaternion quaternion = Quaternion.CreateFromAxisAngle(vector, angle);
            selectedGameObject.World.Forward = Vector3.Transform(selectedGameObject.World.Forward, quaternion);
            selectedGameObject.World.Right = Vector3.Transform(selectedGameObject.World.Right, quaternion);
            selectedGameObject.World.Up = Vector3.Transform(selectedGameObject.World.Up, quaternion);
        }


        /****************************************************************************/
        /// On Mouse Key
        /****************************************************************************/
        private void OnMouseKey(MouseKeyAction mouseKeyAction,ref ExtendedMouseKeyState mouseKeyState)
        {
            

            Dictionary<int, float> PickedObjects = new Dictionary<int, float>();

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
                
                if (middleButton == false && isOnWindow)
                {

                    if (!editor.jiglibxSelection)
                    {
                        //selecting
                        foreach (iconInfo info in editor.iconInfo)
                        {
                            if (mouseX > info.pos.X && mouseY > info.pos.Y && mouseX < (info.pos.X + info.width) && mouseY < (info.pos.Y + info.height))
                            {



                                PickedObjects.Add(info.goID, Vector3.Distance(this.World.Translation, editor.level.GameObjects[info.goID].World.Translation));

                            }
                        }

                        Microsoft.Xna.Framework.Ray ray = cameraComponent.GetMouseRay(new Vector2(mouseX, mouseY));



                        foreach (MeshComponent mesh in renderer.meshes)
                        {
                            if (ray.Intersects(mesh.BoundingBox) != null)
                            {
                                if (!PickedObjects.Keys.Contains(mesh.GameObject.ID))
                                {
                                    PickedObjects.Add(mesh.GameObject.ID, Vector3.Distance(this.World.Translation, mesh.GameObject.World.Translation));
                                }

                            }
                        }
                        foreach (SkinnedMeshComponent mesh2 in renderer.skinnedMeshes)
                        {
                            if (ray.Intersects(mesh2.BoundingBox) != null)
                            {
                                if (!PickedObjects.Keys.Contains(mesh2.GameObject.ID))
                                {
                                    PickedObjects.Add(mesh2.GameObject.ID, Vector3.Distance(this.World.Translation, mesh2.GameObject.World.Translation));
                                }
                            }
                        }

                        if (PickedObjects.Count != 0)
                        {
                            var sortedDict = (from entry in PickedObjects orderby entry.Value ascending select entry);
                            this.Broadcast(new LowLevelGameFlow.GameObjectClicked(sortedDict.ElementAt(0).Key));
                            selectedGameObject = editor.level.GameObjects[sortedDict.ElementAt(0).Key];
                        }
                    }
                    //draging
                    
                    Vector3 direction = Physics.PhysicsUlitities.DirectionFromMousePosition(this.cameraComponent.Projection, this.cameraComponent.View, mouseX, mouseY);
                    Vector3 pos, nor;
                    float dist;
                    bool hit = false;
                    hit = Physics.PhysicsUlitities.RayTest(cameraComponent.Position, cameraComponent.Position + direction * 500, out dist, out skin, out pos, out nor);
                    if (skin != null)
                    {
                        if (editor.jiglibxSelection)
                        {
                            this.Broadcast(new LowLevelGameFlow.GameObjectClicked((int)((GameObjectInstance)skin.ExternalData).ID));
                            selectedGameObject = editor.level.GameObjects[(int)((GameObjectInstance)skin.ExternalData).ID];
                        }
                    }

                    if (hit)
                    {


                        Vector3 delta = pos - skin.Owner.Position;
                        delta = Vector3.Transform(delta, Matrix.Transpose(skin.Owner.Orientation));

                        camPickDistance = (cameraComponent.Position - pos).Length();

                        skin.Owner.Immovable = false;
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
                if (skin != null)
                {
                    if (skin.Owner != null)
                    {
                        skin.Owner.Immovable = true;
                    }
                    skin = null;
                }
            }
         
            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Mouse Move
        /****************************************************************************/
        private void OnMouseMove(MouseMoveAction mouseMoveAction,ref ExtendedMouseMovementState mouseMovementState)
        {
            mouseX = mouseMovementState.Position.X;
            mouseY = mouseMovementState.Position.Y;
            isOnWindow = mouseMovementState.IsOnWindow;
            if (rotation && mouseMovementState.Moved)
            {
                cameraComponent.RotateY(-rotationSpeed * mouseMovementState.Difference.X);
                cameraComponent.Pitch  (-rotationSpeed * mouseMovementState.Difference.Y);
            }

            if (clicked)
            {
                mouseXclicked = mouseMovementState.Position.X;
                mouseYclicked = mouseMovementState.Position.Y;

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

            if (cameraComponent.Renderer.CurrentCamera != null)
            {
                if (this.cameraComponent.Renderer.CurrentCamera.Equals(this.cameraComponent))
                {

                    data.CurrentCamera = true;
                }
                else
                {
                    data.CurrentCamera = false;
                }
            }
            else
            {
                data.CurrentCamera = false;
            }
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
        [CategoryAttribute("Misc")]
        public bool CurrentCamera { set; get; }
    }

    /********************************************************************************/

}
/************************************************************************************/