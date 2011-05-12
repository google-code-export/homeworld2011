using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;

using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;


/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// GlowStick
    /********************************************************************************/
    class GlowStick : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        MeshComponent            mesh   = null;
        CylindricalBodyComponent body   = null;
        PointLightComponent      light1 = null;
        PointLightComponent      light2 = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(MeshComponent mesh,
                         CylindricalBodyComponent body,
                         PointLightComponent light1,
                         PointLightComponent light2)
        {
            this.mesh   = mesh;
            this.body   = body;
            this.light1 = light1;
            this.light2 = light2;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            mesh.ReleaseMe();
            mesh = null;
            body.ReleaseMe();
            body = null;
            light1.ReleaseMe();
            light1 = null;
            light2.ReleaseMe();
            light2 = null;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            GlowStickData data = new GlowStickData();

            GetData(data);
            
            data.Texture = mesh.Textures.Diffuse.Name;
            data.InstancingMode = Renderer.InstancingModeToUInt(mesh.InstancingMode);

            data.Mass = body.Mass;
            data.Elasticity = body.Elasticity;
            data.StaticRoughness = body.StaticRoughness;
            data.DynamicRoughness = body.DynamicRoughness;
            data.Immovable = body.Immovable;

            data.Enabled              = light1.Enabled;
            data.Color                = light1.Color;

            return data;
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// GlowStickData
    /********************************************************************************/
    [Serializable]
    public class GlowStickData : GameObjectInstanceData
    {
        [CategoryAttribute("Textures")]
        public String Texture { get; set; }

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

        [CategoryAttribute("Light")]
        public bool Enabled { get; set; }

        [CategoryAttribute("Light")]
        public Vector3 Color { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/