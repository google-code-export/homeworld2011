﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PlagueEngine.LowLevelGameFlow;

/************************************************************************************/
/// PlagueEngine.Rendering.Components
/************************************************************************************/
namespace PlagueEngine.Rendering.Components
{

    /********************************************************************************/
    /// Camera Component
    /********************************************************************************/
    class CameraComponent : GameObjectComponent
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private Matrix   projection = Matrix.Identity;
        private float    fov        = 0;
        private float    aspect     = 0;        
        private float    zNear      = 0;
        private float    zFar       = 0;
        private Renderer renderer   = null;
        /****************************************************************************/



        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public CameraComponent( GameObjectInstance gameObject,
                                Renderer renderer,
                                float fov,
                                float zNear,
                                float zFar) : base(gameObject)
        {
            this.renderer   = renderer;
            this.fov        = fov;            
            this.zNear      = zNear;
            this.zFar       = zFar;
            this.aspect     = renderer.Device.Viewport.AspectRatio;

            ComputeProjectionMatrix();

        }
        /****************************************************************************/


        /****************************************************************************/
        /// Scren Width
        /****************************************************************************/
        public int ScreenWidth
        {
            get {return renderer.Device.Viewport.Width;}
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Scren Height
        /****************************************************************************/
        public int ScreenHeight
        {
            get { return renderer.Device.Viewport.Height; }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// View
        /****************************************************************************/
        public Matrix View
        {
            get
            {
                return gameObject.World;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Projection
        /****************************************************************************/
        public Matrix Projection
        {
            get
            {
                return projection;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// FoV
        /****************************************************************************/
        public float FoV
        {
            get
            {
                return fov;
            }

            set
            {
                fov = value;
                ComputeProjectionMatrix();
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Aspect
        /****************************************************************************/
        public float Aspect
        {
            get
            {
                return aspect;
            }

            set
            {
                aspect = value;
                ComputeProjectionMatrix();
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// ZNear
        /****************************************************************************/
        public float ZNear
        {
            get
            {
                return zNear;
            }

            set
            {
                zNear = value;
                ComputeProjectionMatrix();
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// ZFar
        /****************************************************************************/
        public float ZFar
        {
            get
            {
                return zFar;
            }

            set
            {
                zFar = value;
                ComputeProjectionMatrix();
            }  
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Compute Projection Matrix
        /****************************************************************************/
        private void ComputeProjectionMatrix()
        { 
            projection = Matrix.CreatePerspectiveFieldOfView(fov, aspect, zNear, zFar);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Rotate X
        /****************************************************************************/
        public void RotateX(float angle)
        {
            gameObject.World *= Matrix.CreateRotationX(angle);            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Rotate Y
        /****************************************************************************/
        public void RotateY(float angle)
        {
            gameObject.World *= Matrix.CreateRotationY(angle);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Rotate Z
        /****************************************************************************/
        public void RotateZ(float angle)
        {
            gameObject.World *= Matrix.CreateRotationZ(angle);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Yaw
        /****************************************************************************/
        public void Yaw(float angle)
        {
            gameObject.World *= Matrix.CreateFromAxisAngle(gameObject.World.Up, angle);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Pitch
        /****************************************************************************/
        public void Pitch(float angle)
        {
            gameObject.World *= Matrix.CreateFromAxisAngle(gameObject.World.Right, angle);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Roll
        /****************************************************************************/
        public void Roll(float angle)
        {
            gameObject.World *= Matrix.CreateFromAxisAngle(gameObject.World.Forward, angle);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Move Foreward
        /****************************************************************************/
        public void MoveForward(float step)
        {
            gameObject.World *= Matrix.CreateTranslation(0, 0, step);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Move Up
        /****************************************************************************/
        public void MoveUp(float step)
        {
            gameObject.World *= Matrix.CreateTranslation(0, step * -1, 0);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Move Right
        /****************************************************************************/
        public void MoveRight(float step)
        {
            gameObject.World *= Matrix.CreateTranslation(step, 0, 0);            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Move X
        /****************************************************************************/
        public void MoveX(float step)
        {
            gameObject.World *= Matrix.CreateTranslation(gameObject.World.Right * step);  
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Move Y
        /****************************************************************************/
        public void MoveY(float step)
        {
            gameObject.World *= Matrix.CreateTranslation(gameObject.World.Up * step * -1);            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Move Z
        /****************************************************************************/
        public void MoveZ(float step)
        {
            gameObject.World *= Matrix.CreateTranslation(gameObject.World.Forward * step);            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Look At
        /****************************************************************************/
        public void LookAt(ref Vector3 position, ref Vector3 target, ref Vector3 up)
        {
            gameObject.World = Matrix.CreateLookAt(position, target, up);
        }

        public void LookAt(Vector3 position,Vector3 target,Vector3 up)
        {
            gameObject.World = Matrix.CreateLookAt(position, target, up);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Set As Current
        /****************************************************************************/
        public void SetAsCurrent()
        {
            renderer.CurrentCamera = this;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            if (renderer.CurrentCamera == this) renderer.CurrentCamera = null;
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/