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
        private WaterSurfaceComponent   waterComponent       = null;
        private TerrainSkinComponent    terrainSkinComponent = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(TerrainComponent      terrainComponent,
                         WaterSurfaceComponent waterComponent,
                         TerrainSkinComponent  terrainSkinComponent,
                         Matrix world)
        {
            this.terrainComponent     = terrainComponent;
            this.waterComponent       = waterComponent;
            this.terrainSkinComponent = terrainSkinComponent;
            this.World                = world;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            terrainComponent.ReleaseMe();
            waterComponent.ReleaseMe();
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

            data.Level          = waterComponent.Level;
            data.Color          = waterComponent.WaterColor;
            data.ColorAmount    = waterComponent.ColorAmount;

            data.WaveHeight          = waterComponent.WaveHeight;
            data.WaveLength          = waterComponent.WaveLength;
            data.WaveSpeed           = waterComponent.WaveSpeed;
            data.NormalMap           = waterComponent.NormalMap;
            data.Bias                = waterComponent.Bias;
            data.WTextureTiling      = waterComponent.TextureTiling;
            data.ClipPlaneAdjustment = waterComponent.ClipPlaneAdjustment;
            data.SpecularStength     = waterComponent.SpecularStrength;

            data.Elasticity          = terrainSkinComponent.Elasticity;
            data.StaticRoughness     = terrainSkinComponent.StaticRoughness;
            data.DynamicRoughness    = terrainSkinComponent.DynamicRoughness;

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

        [CategoryAttribute("Water Surface")]
        public float Level          { get; set; }
        [CategoryAttribute("Water Surface")]
        public Vector3  Color       { get; set; }
        [CategoryAttribute("Water Surface")]
        public float ColorAmount    { get; set; }
        [CategoryAttribute("Water Surface")]
        public float WaveLength     { get; set; }
        [CategoryAttribute("Water Surface")]
        public float WaveHeight     { get; set; }
        [CategoryAttribute("Water Surface")]
        public float WaveSpeed      { get; set; }
        [CategoryAttribute("Water Surface")]
        public String NormalMap     { get; set; }
        [CategoryAttribute("Water Surface")]
        public float Bias           { get; set; }
        [CategoryAttribute("Water Surface")]
        public float WTextureTiling { get; set; }
        [CategoryAttribute("Water Surface")]
        public float ClipPlaneAdjustment { get; set; }
        [CategoryAttribute("Water Surface")]
        public float SpecularStength { get; set; }

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