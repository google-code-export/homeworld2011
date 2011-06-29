using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Input.Components;
using PlagueEngine.TimeControlSystem;
using PlagueEngine.EventsSystem;
using JigLibX.Collision;
using PlagueEngine.Audio.Components;
using PlagueEngine.Audio;

/************************************************************************************/
// PlagueEngine.LowLevelGameFlow.GameObjects
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
        public CameraComponent CameraComponent;
        public KeyboardListenerComponent KeyboardListenerComponent;
        public MouseListenerComponent MouseListenerComponent;
        public BackgroundMusicComponent backgroundMusic;

        public EventsSnifferComponent sniffer;
        private float _movementSpeed;
        private float _rotationSpeed;
        private float _zoomSpeed;

        private float Xmin,Xmax,Ymin,Ymax,Zmin,Zmax;
        private float angleMin;

        private Vector3 _position = Vector3.Zero;
        private Vector3 _target = Vector3.Zero;

        private readonly Clock _clock = TimeControl.CreateClock();

        private GameObjectInstance _tracedObject;
        private Vector3 _moveToPosition;

        private int  _rotateCamera   = -1;
        private bool _isOnWindow;
        private bool _useCommands;        
        private bool _movingToPoint;
        private bool _tracing;
        
        private bool _selectionRect;
        private Rectangle _rect;
        
        private bool _addToSelection;
        private bool _removeFromSelection;
        private bool _zoom;

        private int _maxTop, _minTop, _maxBottom, _minBottom, _maxLeft, _minLeft, _maxRight, _minRight;

        private float _mouseX, _mouseY;
        private bool lookAt = false;

        private Vector2 HeightRange;

        public MercenariesManager MercenariesManager;

        public bool StopScrolling   { get; set; }
        public bool FireMode        { get; set; }
        public bool Run             { get; set; }

        int escPressCounter = 0;
        /****************************************************************************/


        /****************************************************************************/
        // Initialization
        /****************************************************************************/
        public void Init(CameraComponent cameraComponent,
                         KeyboardListenerComponent keyboardListenerComponent,
                         MouseListenerComponent mouseListenerComponent,
                         float movementSpeed,
                         float rotationSpeed,
                         float zoomSpeed,
                         Vector3 position,
                         Vector3 target,
                         GameObjectInstance mercenariesManager,
                         bool current,
                         Vector2 heightRange,
                         float xmax,
                         float xmin,
                         float ymax,
                         float ymin,
                         float zmax,
                         float zmin,
                         float anglemin)
        {

            backgroundMusic = new BackgroundMusicComponent();
            AudioManager.GetInstance.BackgroundMusicComponent = backgroundMusic;
            backgroundMusic.LoadFolder("Ambients", 0.1f);
            backgroundMusic.AutomaticMode = false;
            //backgroundMusic.PlaySong("default", "Begining", false);

            CameraComponent = cameraComponent;
            KeyboardListenerComponent = keyboardListenerComponent;
            MouseListenerComponent = mouseListenerComponent;

            sniffer = new EventsSnifferComponent();
            sniffer.SetOnSniffedEvent(OnSniffedEvent);
            sniffer.SubscribeEvents(typeof(InGameMenuClose));

            HeightRange = heightRange;
            _movementSpeed = movementSpeed;
            _rotationSpeed = rotationSpeed;
            _zoomSpeed = zoomSpeed;
            _position = position;
            _target = target;            

            World = Matrix.CreateLookAt(_position, _target, Vector3.Up);

            KeyboardListenerComponent.SubscibeKeys(OnKey, Keys.W, Keys.S, Keys.A,Keys.D,
                                                          Keys.LeftControl,Keys.Space, 
                                                          Keys.LeftAlt,Keys.Escape);

            MouseListenerComponent.SubscribeMouseMove(OnMouseMove, MouseMoveAction.Move, MouseMoveAction.Scroll);
            MouseListenerComponent.SubscribeKeys(OnMouseKey, MouseKeyAction.MiddleClick, MouseKeyAction.RightClick, MouseKeyAction.LeftClick);

            var screenWidth = cameraComponent.ScreenWidth;
            var screenHeight = cameraComponent.ScreenHeight;

         
            MercenariesManager = mercenariesManager as MercenariesManager;
          
            // TODO: Wartości wpisane ręcznie, a nie podawane z edytora.
            _maxTop = 0;
            _minTop = (int)(screenHeight * 0.025);

            _maxBottom = screenHeight;
            _minBottom = (int)(screenHeight * 0.975);

            _maxLeft = 0;
            _minLeft = (int)(screenWidth * 0.025);

            _maxRight = screenWidth;
            _minRight = (int)(screenWidth * 0.975);

            RequiresUpdate = true;

            if (current) cameraComponent.SetAsCurrent();


            Xmax = xmax;
            Xmin = xmin;
            Zmax = zmax;
            Zmin = zmin;
            Ymax = ymax;
            Ymin = ymin;
            angleMin = anglemin;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Sniffed Event
        /****************************************************************************/
        private void OnSniffedEvent(EventsSender sender, IEventsReceiver receiver, EventArgs e)
        {
            if (e.GetType().Equals(typeof(InGameMenuClose)) )
            {
               
                escPressCounter++;
                
            }
        }
        /****************************************************************************/



        /****************************************************************************/
        // StartMoveToPoint
        /****************************************************************************/
        public void StartMoveToPoint(Vector3 point)
        {
            _moveToPosition = point;
            _movingToPoint = true;
            StopTracking();
        }
        /****************************************************************************/


        /****************************************************************************/
        // StopMovingToPoint
        /****************************************************************************/
        public void StopMovingToPoint()
        {
            _movingToPoint = false;
        }
        /****************************************************************************/


        /****************************************************************************/
        // MoveToPoint
        /****************************************************************************/
        private void MoveToPoint()
        {
            if (_moveToPosition == _target) return;
            if (Vector3.Distance(_target, _moveToPosition) < 0.01f)
            {
                _target = _moveToPosition;
            }
            else
            {
                var distanceVec = _moveToPosition - _target;
                _target += distanceVec / 50.0f;
                _position += distanceVec / 50.0f;
            }
        }

        /****************************************************************************/


        /****************************************************************************/
        // SetTarget
        /****************************************************************************/
        public void SetTarget(GameObjectInstance target)
        {
            _tracedObject = target;
        }
        /****************************************************************************/


        /****************************************************************************/
        // Start Tracing
        /****************************************************************************/
        public void StartTracing()
        {
            StopMovingToPoint();
            _tracing = true;
        }
        /****************************************************************************/


        /****************************************************************************/
        // Stop Tracing 
        /****************************************************************************/
        public void StopTracking()
        {
            _tracing = false;
        }
        /****************************************************************************/


        /****************************************************************************/
        // RealeseTarget
        /****************************************************************************/
        public void RealeseTarget()
        {
            _tracedObject = null;
        }
        /****************************************************************************/


        /****************************************************************************/
        // Trace Target
        /****************************************************************************/
        private void TraceTarget()
        {
            var targetPosition = _tracedObject.World.Translation;

            if (targetPosition == _target) return;
            if (Vector3.Distance(_target, targetPosition) < 0.01f)
            {
                _target = targetPosition;
            }
            else
            {
                var distanceVec = targetPosition - _target;
                _target += distanceVec / 50.0f;
                _position += distanceVec / 50.0f;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        // OnMouseKey
        /****************************************************************************/
        private void OnMouseKey(MouseKeyAction mouseKeyAction, ref ExtendedMouseKeyState mouseKeyState)
        {


            /****************************/
            // Middle Button
            /****************************/
            if (mouseKeyState.WasPressed() && mouseKeyAction == MouseKeyAction.MiddleClick)
            {
                if (++_rotateCamera == 1)
                {
                    MouseListenerComponent.LockCursor();
                }
                else
                {
                    if (_isOnWindow)
                    {
                        CollisionSkin skin;
                        var direction = Physics.PhysicsUlitities.DirectionFromMousePosition(CameraComponent.Projection, CameraComponent.View, _mouseX, _mouseY);
                        Vector3 pos, nor;
                        float dist;
                        var hit = Physics.PhysicsUlitities.RayTest(CameraComponent.Position, CameraComponent.Position + direction * 500, out dist, out skin, out pos, out nor);
                        if (skin != null)
                        {
                            if (skin.ExternalData.GetType().Equals(typeof(Terrain)))
                            {
                                StartMoveToPoint(pos);
                            }
                            else
                            {
                                SetTarget((GameObjectInstance)skin.ExternalData);
                                StartTracing();
                            }
                        }
                    }
                }
            }
            else if (mouseKeyState.WasReleased() && mouseKeyAction == MouseKeyAction.MiddleClick)
            {                
                if (--_rotateCamera == 0) MouseListenerComponent.UnlockCursor();
            }
            /****************************/


            /****************************/
            // Left Button
            /****************************/
            if (mouseKeyState.WasPressed() && mouseKeyAction == MouseKeyAction.LeftClick)
            {
                _rect.X = (int)_mouseX;
                _rect.Y = (int)_mouseY;

                CollisionSkin skin;
                var direction = Physics.PhysicsUlitities.DirectionFromMousePosition(CameraComponent.Projection, CameraComponent.View, _mouseX, _mouseY);
                Vector3 pos, nor;
                float dist;
                Physics.PhysicsUlitities.RayTest(CameraComponent.Position, CameraComponent.Position + direction * CameraComponent.ZFar, out dist, out skin, out pos, out nor);

                GameObjectInstance goi = null;

                if (skin != null)
                {
                    if (((GameObjectInstance) skin.ExternalData).Status == GameObjectStatus.Mercenary) goi = skin.ExternalData as GameObjectInstance;
                }

                if      (_addToSelection)      SendEvent(new AddToSelectionEvent     (goi), Priority.Normal, MercenariesManager);
                else if (_removeFromSelection) SendEvent(new RemoveFromSelectionEvent(goi), Priority.Normal, MercenariesManager);
                else                          SendEvent(new SelectedObjectEvent(goi, pos), Priority.Normal, MercenariesManager);
            }
            else if (mouseKeyState.IsDown() && mouseKeyAction == MouseKeyAction.LeftClick)
            {
                _rect.Width    = (int)_mouseX - _rect.X;
                _rect.Height   = (int)_mouseY - _rect.Y;
                _selectionRect = true;
            }
            else if (mouseKeyState.WasReleased() && mouseKeyAction == MouseKeyAction.LeftClick)
            {
                if (!_selectionRect) return;

                if (_rect.Width != 0 && _rect.Height != 0)
                {
                    //TODO: zamienić na FastFrustrum
                    var frustrum = CameraComponent.GetFrustumFromRect(_rect);

                    var i = 0;
                    foreach (var mercenary in MercenariesManager.Mercenaries.Keys)
                    {
                        if (!frustrum.Intersects(mercenary.Mesh.BoundingBox)) continue;
                        ++i;

                        if      (_removeFromSelection)      SendEvent(new RemoveFromSelectionEvent(mercenary), Priority.Normal, MercenariesManager);
                        else if (i == 1 && _addToSelection) SendEvent(new AddToSelectionEvent(mercenary), Priority.Normal, MercenariesManager);
                        else if (_addToSelection)           SendEvent(new AddToSelectionEvent(mercenary), Priority.Normal, MercenariesManager);
                        else if (i == 1)                   SendEvent(new SelectedObjectEvent(mercenary, mercenary.World.Translation), Priority.Normal, MercenariesManager);
                        else                               SendEvent(new AddToSelectionEvent(mercenary), Priority.Normal, MercenariesManager);
                    }
                }
                _selectionRect = false;
            }
            /****************************/


            /****************************/
            // Right Button
            /****************************/
            if (mouseKeyState.WasPressed() && mouseKeyAction == MouseKeyAction.RightClick && _useCommands)
            {
                CollisionSkin skin;
                var direction = Physics.PhysicsUlitities.DirectionFromMousePosition(CameraComponent.Projection, CameraComponent.View, _mouseX, _mouseY);
                Vector3 pos, nor;
                float dist;
                Physics.PhysicsUlitities.RayTest(CameraComponent.Position, CameraComponent.Position + direction * CameraComponent.ZFar, out dist, out skin, out pos, out nor);

                if (skin != null && skin.ExternalData!=null)
                {
                    if (((GameObjectInstance) skin.ExternalData).Status != GameObjectStatus.Nothing)
                    {
                        SendEvent(new CommandOnObjectEvent(skin.ExternalData as GameObjectInstance, pos), Priority.Normal, MercenariesManager);
                    }
                }
            }
            /****************************/

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

            Physics.PhysicsUlitities.RayTest(position, desiredPosition + (desiredPosition - position) * 10, out dist, out skin, out pos, out nor);
            if (skin != null)
            {
                return false;
            }


            if (desiredPosition.X < Xmin)
            {
                return false;
            }
            if (desiredPosition.X > Xmax)
            {
                return false;
            }
            if (desiredPosition.Z < Zmin)
            {
                return false;
            }
            if (desiredPosition.Z > Zmax)
            {
                return false;
            }
            if (desiredPosition.Y < Ymin)
            {
                return false;
            }
            if (desiredPosition.Y > Ymax)
            {
                return false;
            }

            Vector3 v1 = _target - desiredPosition;
            Vector3 v2 = _target - desiredPosition;
            v2.Y = 0;

            v1.Normalize();
            v2.Normalize();

            float angle= (float)Math.Acos(Vector3.Dot(v1, v2));
            float angle2 = (float)(angle * 180.0 / Math.PI);

            if (angle2 < angleMin)
            {
                return false;
            }

            return true;

        }
        /****************************************************************************/




        /****************************************************************************/
        // On Mouse Move 
        /****************************************************************************/
        private void OnMouseMove(MouseMoveAction mouseMoveAction, ref ExtendedMouseMovementState mouseMoveState)
        {
            var direction = Vector3.Normalize(_target - _position);

            _isOnWindow = mouseMoveState.IsOnWindow;
            _mouseX = mouseMoveState.Position.X;
            _mouseY = mouseMoveState.Position.Y;

            var perpendicular = Vector3.Normalize(Vector3.Transform(direction, Matrix.CreateRotationY(MathHelper.ToRadians(90.0f))));

            var time = (float)(_clock.DeltaTime.TotalMilliseconds);

            switch (mouseMoveAction)
            {
                case MouseMoveAction.Scroll:
                    if (!StopScrolling)
                    {
                        //if (_position.Y < HeightRange.X)
                        //{
                        //    _position.Y = HeightRange.X;
                        //    return;
                        //}
                        //else if (_position.Y > HeightRange.Y)
                        //{
                        //    _position.Y = HeightRange.Y;
                        //    return;
                        //}

                        var testPosition = _position + direction * _zoomSpeed * mouseMoveState.ScrollDifference;

                        var distance = Vector3.Distance(_target, testPosition);
                        if (distance > 3.0f && testPosition.Y > _target.Y)//kamera musi patrzec z gory w dol, nie odwrotnie. wiec lepiej jej nie przyblizac za duzo
                        {
                            if (_zoom)
                            {
                                var tmp = _position;
                                tmp.Y += _zoomSpeed * direction.Y * mouseMoveState.ScrollDifference * (Vector3.Distance(_position, _target) / 20);

                                if (CanIMove(_position, tmp))
                                {
                                    _position.Y += _zoomSpeed * direction.Y * mouseMoveState.ScrollDifference * (Vector3.Distance(_position, _target) / 20);
                                }
                            }
                            else
                            {
                                var tmp = _position;
                                tmp += _zoomSpeed * direction * mouseMoveState.ScrollDifference * (Vector3.Distance(_position, _target) / 20);

                                if (CanIMove(_position, tmp))
                                {
                                    _position += _zoomSpeed * direction * mouseMoveState.ScrollDifference * (Vector3.Distance(_position, _target) / 20);
                                }
                            }
                        }
                    }
                    break;


                case MouseMoveAction.Move:
                    {
                        if (_rotateCamera == 1)
                        {
                            var tmp = _position;
                            tmp -= _target;
                            tmp = Vector3.Transform(tmp, Matrix.CreateRotationY(_rotationSpeed * (mouseMoveState.Position.X - mouseMoveState.OldPosition.X)));
                            tmp += _target;

                            if (CanIMove(_position, tmp))
                            {
                                _position -= _target;
                                _position = Vector3.Transform(_position, Matrix.CreateRotationY(_rotationSpeed * (mouseMoveState.Position.X - mouseMoveState.OldPosition.X)));
                                _position += _target;
                            }
                        }
                        var cursor = "Default";
                        if (MercenariesManager.isSomeoneSelected())
                        {
                            CollisionSkin skin;
                            var direction1 = Physics.PhysicsUlitities.DirectionFromMousePosition(CameraComponent.Projection, CameraComponent.View, _mouseX, _mouseY);
                            Vector3 pos, nor;
                            float dist;
                            Physics.PhysicsUlitities.RayTest(CameraComponent.Position, CameraComponent.Position + direction1 * CameraComponent.ZFar, out dist, out skin, out pos, out nor);
                            if (skin != null)
                            {
                                if (skin.ExternalData != null)
                                {
                                    if (lookAt)
                                    {
                                        SendEvent(new LookAtPointEvent(pos), Priority.Normal, MercenariesManager);
                                        cursor = "Default";
                                    }
                                    if (FireMode)
                                    {
                                        cursor = "Targeting";
                                    }
                                    else if (_useCommands)
                                    {

                                        switch (((GameObjectInstance)skin.ExternalData).Status)
                                        {
                                            case GameObjectStatus.Interesting: cursor = "QuestionMark"; break;
                                            case GameObjectStatus.Pickable: cursor = "Hand"; break;
                                            case GameObjectStatus.Targetable: cursor = "Target"; break;
                                            case GameObjectStatus.Passable: cursor = Run ? "Run" : "Footsteps"; break;
                                            case GameObjectStatus.Mercenary: cursor = "Person"; break;
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
                            }
                        }
                        MouseListenerComponent.SetCursor(cursor);
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
            if (key == Keys.Escape && state.WasPressed())
            {
                Diagnostics.PushLog(escPressCounter.ToString());
             
                    InGameMenuData data = new InGameMenuData();
                   
                    this.SendEvent(new CreateObjectEvent(data), Priority.Normal, GlobalGameObjects.GameController);

            }

            if (key == Keys.LeftControl) _addToSelection = state.IsDown();

            if (key == Keys.Space) lookAt = state.IsDown();

            if (key == Keys.LeftAlt)
            {
                _removeFromSelection = state.IsDown();
                _zoom = state.IsDown();

                if (key == Keys.LeftAlt && state.WasPressed())
                {
                    if (++_rotateCamera == 1) MouseListenerComponent.LockCursor();
                }
                else if (key == Keys.LeftAlt && state.WasReleased())
                {
                    if (--_rotateCamera == 0) MouseListenerComponent.UnlockCursor();
                }
            }

            if (!state.IsDown()) return;

            
            
            var direction      = Vector3.Normalize(_target - _position); direction.Y = 0;                        
            var distanceFactor = Vector3.Distance(_target, _position) / 50;
            var perpendicular  = Vector3.Transform(direction, Matrix.CreateRotationY(MathHelper.ToRadians(90.0f))); perpendicular.Y = 0;            
            var time           = (float)(_clock.DeltaTime.TotalMilliseconds);
            var offset         = direction * time * _movementSpeed * distanceFactor;

            switch (key)
            {
                case Keys.W:

                    if (CanIMove(_position, _position + offset))
                    {
                        _position += offset;
                        _target   += offset;
                        StopTracking();
                        StopMovingToPoint();
                    }
                    break;

                case Keys.S:

                    if (CanIMove(_position, _position - offset))
                    {
                        _position -= offset;
                        _target   -= offset;
                        StopTracking();
                        StopMovingToPoint();
                    }
                    break;

                case Keys.A:

                    offset = perpendicular * time * _movementSpeed * distanceFactor;

                    if (CanIMove(_position, _position + offset))
                    {
                        _position += offset;
                        _target   += offset;
                        StopTracking();
                        StopMovingToPoint();
                    }
                    break;

                case Keys.D:

                    offset = perpendicular * time * _movementSpeed * distanceFactor;

                    if (CanIMove(_position, _position - offset))
                    {
                        _position -= offset;
                        _target   -= offset;
                        StopTracking();
                        StopMovingToPoint();
                    }
                    break;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        // Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            CameraComponent.ReleaseMe();
            KeyboardListenerComponent.ReleaseMe();
            MouseListenerComponent.ReleaseMe();
            sniffer.ReleaseMe();            
        }
        /****************************************************************************/


        /****************************************************************************/
        // Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            var data = new LinkedCameraData();
            GetData(data);

            data.MovementSpeed = _movementSpeed;
            data.RotationSpeed = _rotationSpeed;
            data.ZoomSpeed = _zoomSpeed;
            data.FoV =MathHelper.ToDegrees( CameraComponent.FoV);
            data.ZNear = CameraComponent.ZNear;
            data.ZFar = CameraComponent.ZFar;
            data.ActiveKeyListener = KeyboardListenerComponent.Active;
            data.ActiveMouseListener = MouseListenerComponent.Active;
            data.position = _position;
            data.Target = _target;
            
            data.MercenariesManager = MercenariesManager.ID;

            data.CurrentCamera = CameraComponent.Renderer.CurrentCamera != null && CameraComponent.Renderer.CurrentCamera.Equals(CameraComponent);
            
            data.Xmax = Xmax;
            data.Xmin = Xmin;
            data.Ymax = Ymax;
            data.Ymin = Ymin;
            data.Zmax = Zmax;
            data.Zmin = Zmin;
            data.AngleMin = angleMin;
            return data;
        }
        /****************************************************************************/


        /****************************************************************************/
        // Update
        /****************************************************************************/
        public override void Update(TimeSpan deltaTime)
        {         
            if (_movingToPoint)  MoveToPoint();
            if (_tracing)        TraceTarget();
            if (_selectionRect) CameraComponent.Renderer.DrawSelectionRect(_rect);
            else
            {
                /***************************************/
                // Check Borders
                /***************************************/

                if (!(_mouseX < _minRight && _mouseX > _minLeft && _mouseY < _minBottom && _mouseY > _minTop))
                {
                    var direction = Vector3.Normalize(_target - _position);
                    var distanceFactor = Vector3.Distance(_target, _position) / 50;
                    var perpendicular = Vector3.Normalize(Vector3.Transform(direction, Matrix.CreateRotationY(MathHelper.ToRadians(90.0f))));

                    var time = (float)(deltaTime.TotalMilliseconds);

                    direction.Y = 0;
                    perpendicular.Y = 0;

                    var offset = perpendicular * _movementSpeed * time * distanceFactor;
                    /************************/
                    // Left Border
                    /************************/
                    if (_mouseX < _minLeft && _mouseX > _maxLeft)
                    {
                        if (CanIMove(_position, _position + offset))
                        {
                            MouseListenerComponent.SetCursor("Left");
                            _position += offset;
                            _target += offset;

                            StopTracking();
                            StopMovingToPoint();
                        }
                    }
                    /************************/


                    /************************/
                    // Right Border
                    /************************/
                    if (_mouseX > _minRight && _mouseX < _maxRight)
                    {
                        if (CanIMove(_position, _position - offset))
                        {
                            MouseListenerComponent.SetCursor("Right");
                            _position -= offset;
                            _target -= offset;

                            StopTracking();
                            StopMovingToPoint();
                        }
                    }
                    /************************/

                    offset = direction * _movementSpeed * time * distanceFactor;

                    /************************/
                    // Top Border
                    /************************/
                    if (_mouseY < _minTop && _mouseY > _maxTop)
                    {
                        if (CanIMove(_position, _position + offset))
                        {
                            MouseListenerComponent.SetCursor("Up");
                            _position += offset;
                            _target += offset;

                            StopTracking();
                            StopMovingToPoint();
                        }
                    }
                    /************************/


                    /************************/
                    // Bottom Border
                    /************************/
                    if (_mouseY > _minBottom && _mouseY < _maxBottom)
                    {
                        if (CanIMove(_position, _position - offset))
                        {
                            MouseListenerComponent.SetCursor("Down");
                            _position -= offset;
                            _target -= offset;

                            StopTracking();
                            StopMovingToPoint();
                        }
                    }
                    /************************/

                }
                /***************************************/
            }

  

            CameraComponent.LookAt(_position, _target, Vector3.Up);
        }
        /****************************************************************************/



        /****************************************************************************/
        /// On Event
        /****************************************************************************/
        public override void OnEvent(EventsSender sender, EventArgs e)
        {
            if (e.GetType().Equals(typeof(ExSwitchEvent)))
            {
                switch (((ExSwitchEvent) e).name)
                {
                    case "UseCommands": _useCommands = (e as ExSwitchEvent).value; break;
                }
            }
            else if (e.GetType().Equals(typeof(MoveToObjectCommandEvent)))
            {
                SetTarget(((MoveToObjectCommandEvent) e).gameObject);
                StartTracing();
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
        public float MovementSpeed;
        public float RotationSpeed;
        public float ZoomSpeed;
        public float FoV;
        public float ZNear;
        public float ZFar;
        public bool ActiveKeyListener;
        public bool ActiveMouseListener;
        public Vector3 position;
        public Vector3 Target { get; set; }


        [CategoryAttribute("Camera restriction")]
        public float Xmin { get; set; }
        [CategoryAttribute("Camera restriction")]
        public float Xmax { get; set; }
        [CategoryAttribute("Camera restriction")]
        public float Ymin { get; set; }
        [CategoryAttribute("Camera restriction")]
        public float Ymax { get; set; }
        [CategoryAttribute("Camera restriction")]
        public float Zmin { get; set; }
        [CategoryAttribute("Camera restriction")]
        public float Zmax { get; set; }
        [CategoryAttribute("Camera restriction")]
        public float AngleMin { get; set; }

        public float movementSpeed
        {
            
            set { MovementSpeed = value; }
            get { return MovementSpeed; }
        }

        public float rotationSpeed
        {
            set { RotationSpeed = value; }
            get { return RotationSpeed; }
        }
        public float zoomSpeed
        {
            set { ZoomSpeed = value; }
            get { return ZoomSpeed; }
        }
        public float FOV
        {
            set { FoV = value; }
            get { return FoV; }
        }
        public float zNear
        {
            set { ZNear = value; }
            get { return ZNear; }
        }
        public float zFar
        {
            set { ZFar = value; }
            get { return ZFar; }
        }
        public bool activeKeyListener
        {
            set { ActiveKeyListener = value; }
            get { return ActiveKeyListener; }
        }
        public bool activeMouseListener
        {
            set { ActiveMouseListener = value; }
            get { return ActiveMouseListener; }
        }

        public int MercenariesManager { get; set; }
        [CategoryAttribute("Misc")]
        public bool CurrentCamera { set; get; }

        public Vector2 HeightRange { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/