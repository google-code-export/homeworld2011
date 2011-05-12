using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;

using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;

/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// Terrain
    /********************************************************************************/
    class Terrain : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private TerrainComponent        terrainComponent     = null;
        private TerrainSkinComponent    terrainSkinComponent = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(TerrainComponent      terrainComponent,
                         TerrainSkinComponent terrainSkinComponent)
        {
            this.terrainComponent     = terrainComponent;
            this.terrainSkinComponent = terrainSkinComponent;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            terrainComponent.ReleaseMe();
            terrainSkinComponent.ReleaseMe();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            TerrainData data = new TerrainData();
            GetData(data);

            data.Width          = terrainComponent.Width;
            data.Length         = terrainComponent.Length;
            data.Height         = terrainComponent.Height;
            data.CellSize       = terrainComponent.CellSize;
           
            data.HeightMap      = terrainComponent.HeightMap;
            data.BaseTexture    = terrainComponent.BaseTexture;
            data.RTexture       = terrainComponent.RTexture;
            data.GTexture       = terrainComponent.GTexture;
            data.BTexture       = terrainComponent.BTexture;
            data.WeightMap      = terrainComponent.WeightMap;
            data.TextureTiling  = terrainComponent.TextureTiling;

            data.Elasticity = terrainSkinComponent.Elasticity;
            data.StaticRoughness = terrainSkinComponent.StaticRoughness;
            data.DynamicRoughness = terrainSkinComponent.DynamicRoughness;

            return data;
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// Terrain Data
    /********************************************************************************/
    [Serializable]
    public class TerrainData : GameObjectInstanceData
    {

        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        [CategoryAttribute("Sizes")]
        public int   Width          { get; set; }
        [CategoryAttribute("Sizes")]
        public int   Length         { get; set; }
        [CategoryAttribute("Sizes")]
        public float Height         { get; set; }
        [CategoryAttribute("Sizes")]
        public float CellSize       { get; set; }        

        [CategoryAttribute("Textures")]
        public String HeightMap     { get; set; }
        [CategoryAttribute("Textures")]
        public String BaseTexture   { get; set; }
        [CategoryAttribute("Textures")]
        public String RTexture      { get; set; }
        [CategoryAttribute("Textures")]
        public String GTexture      { get; set; }
        [CategoryAttribute("Textures")]
        public String BTexture      { get; set; }
        [CategoryAttribute("Textures")]
        public String WeightMap     { get; set; }
        [CategoryAttribute("Textures")]
        public float TextureTiling  { get; set; }

        [CategoryAttribute("Physics")]
        public float Elasticity { get; set; }
        [CategoryAttribute("Physics")]
        public float StaticRoughness { get; set; }
        [CategoryAttribute("Physics")]
        public float DynamicRoughness { get; set; }

        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/