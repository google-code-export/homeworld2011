﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.ComponentModel;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Rendering;
using PlagueEngine.Physics;
using PlagueEngine.Physics.Components;
using PlagueEngine.Input.Components;
using PlagueEngine.Input;

using Microsoft.Xna.Framework.Input;

/********************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/********************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// CylindricalBodyMesh
    /********************************************************************************/
    class CylindricalBodyMesh : GameObjectInstance
    {

        /********************************************************************************/
        /// Fields
        /********************************************************************************/
        MeshComponent meshComponent = null;
        CylindricalBodyComponent physicsComponent = null;

        PhysicsController controller = null;
        KeyboardListenerComponent keyboardListener= null;
        /********************************************************************************/




        /********************************************************************************/
        /// Init
        /********************************************************************************/
        public void Init(MeshComponent meshComponent, CylindricalBodyComponent physcisComponent, Matrix world)
        {
            this.meshComponent = meshComponent;
            this.physicsComponent = physcisComponent;
            this.World = world;
            controller = new PhysicsController(physcisComponent.Body);
            keyboardListener = new KeyboardListenerComponent(this, true);
            keyboardListener.SubscibeKeys(OnKey, Keys.Y, Keys.H);
            physcisComponent.SubscribeCollisionEvent(typeof(CylindricalBodyMesh));
            physcisComponent.DontCollideWithGameObjectsType(typeof(CylindricalBodyMesh));
        }
        /********************************************************************************/




        /****************************************************************************/
        /// On Key
        /****************************************************************************/
        private void OnKey(Keys key, ExtendedKeyState state)
        {
            if (state.WasPressed())
            {
                switch (key)
                {
                    case Keys.Y:
                        controller.MoveToPoint( Vector3.Up*100, true);
                        break;
                    case Keys.H:

                        controller.MoveToPoint(World.Translation - Vector3.Forward, true);
                        break;
                }
            }
        }
        /****************************************************************************/



        /****************************************************************************/
        /// On Event
        /****************************************************************************/
        public override void OnEvent(EventsSystem.EventsSender sender, EventArgs e)
        {

            

        }
        /****************************************************************************/

        /********************************************************************************/
        /// Release Components
        /********************************************************************************/
        public override void ReleaseComponents()
        {
            meshComponent.ReleaseMe();
            physicsComponent.ReleaseMe();
        }
        /********************************************************************************/




        /********************************************************************************/
        /// GetData
        /********************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            CylindricalBodyMeshData data = new CylindricalBodyMeshData();
            GetData(data);
            data.Model = meshComponent.Model.Name;
            data.Diffuse = (meshComponent.Textures.Diffuse == null ? String.Empty : meshComponent.Textures.Diffuse.Name);
            data.Specular = (meshComponent.Textures.Specular == null ? String.Empty : meshComponent.Textures.Specular.Name);
            data.Normals = (meshComponent.Textures.Normals == null ? String.Empty : meshComponent.Textures.Normals.Name);

            data.InstancingMode = Renderer.InstancingModeToUInt(meshComponent.InstancingMode);

            data.Mass = physicsComponent.Mass;
            data.Elasticity = physicsComponent.Elasticity;
            data.StaticRoughness = physicsComponent.StaticRoughness;
            data.DynamicRoughness = physicsComponent.DynamicRoughness;
            data.Lenght = physicsComponent.Length;
            data.Radius = physicsComponent.Radius;
            data.Immovable = physicsComponent.Immovable;
            data.Translation = physicsComponent.SkinTranslation;
            data.SkinPitch = physicsComponent.Pitch;
            data.SkinRoll = physicsComponent.Roll;
            data.SkinYaw = physicsComponent.Yaw;

            return data;
        }
        /********************************************************************************/



    }
    /********************************************************************************/


    /********************************************************************************/
    /// CylindricalBodyMeshData
    /********************************************************************************/
    [Serializable]
    public class CylindricalBodyMeshData : GameObjectInstanceData
    {
        [CategoryAttribute("Model")]
        public String Model { get; set; }

        [CategoryAttribute("Textures")]
        public String Diffuse { get; set; }

        [CategoryAttribute("Textures")]
        public String Specular { get; set; }

        [CategoryAttribute("Textures")]
        public String Normals { get; set; }

        [CategoryAttribute("Instancing"),
        DescriptionAttribute("1 - No Instancing, 2 - Static Instancing, 3 - Dynamic Instancing.")]
        public uint InstancingMode { get; set; }

        [CategoryAttribute("Physics")]
        public float Mass { get; set; }

        [CategoryAttribute("Physics")]
        public float Elasticity { get; set; }

        [CategoryAttribute("Physics")]
        public float StaticRoughness { get; set; }

        [CategoryAttribute("Physics")]
        public float DynamicRoughness { get; set; }

        [CategoryAttribute("Physics")]
        public bool Immovable { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float Radius { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float Lenght { get; set; }

        [CategoryAttribute("Collision Skin")]
        public Vector3 Translation { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float SkinYaw { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float SkinPitch { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float SkinRoll { get; set; }

    }
    /********************************************************************************/



}
/********************************************************************************/