﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Rendering.Components;


/************************************************************************************/
/// Plague.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// StaticMesh
    /********************************************************************************/
    class StaticMesh : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        BasicMeshComponent basicMeshComponent = null;
        /****************************************************************************/
       

        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(BasicMeshComponent basicMeshComponent, Matrix world)
        {
            this.basicMeshComponent = basicMeshComponent;
            this.World              = world;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            basicMeshComponent.ReleaseMe();
        }
        /****************************************************************************/
        

        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            StaticMeshData data = new StaticMeshData();
            GetData(data);
            data.Model = basicMeshComponent.Model;
            return data;
        }
        /****************************************************************************/


    }
    /********************************************************************************/



    /********************************************************************************/
    /// StaticMeshData
    /********************************************************************************/
    [Serializable]
    public class StaticMeshData : GameObjectInstanceData
    {
        public String Model { get; set; }        
    }
    /********************************************************************************/

}
/************************************************************************************/