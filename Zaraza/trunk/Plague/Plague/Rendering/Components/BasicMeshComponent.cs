﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using PlagueEngine.LowLevelGameFlow;


/************************************************************************************/
/// PlagueEngine.Rendering.Components
/************************************************************************************/
namespace PlagueEngine.Rendering.Components
{

    /********************************************************************************/
    /// Basic Mesh Component
    /********************************************************************************/
    class BasicMeshComponent : GameObjectComponent
    {
        
        /****************************************************************************/
        /// Delegates
        /****************************************************************************/
        public delegate void ReleaseMeDelegate(BasicMeshComponent component);
        /****************************************************************************/


        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private Model                 model     = null;
        private ReleaseMeDelegate     releaseMe = null;
        /****************************************************************************/


        /****************************************************************************/
        /// BasicMeshComponent
        /****************************************************************************/
        public BasicMeshComponent(GameObjectInstance gameObject,Model model,ReleaseMeDelegate releaseMe) : base(gameObject)
        {
            this.model      = model;
            this.releaseMe  = releaseMe;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Model
        /****************************************************************************/
        public Model Model
        {
            get
            {
                return model;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            releaseMe(this);
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/