using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;

using PlagueEngine.Rendering.Components;

/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// BackgroundTerrain
    /********************************************************************************/
    class BackgroundTerrain : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private TerrainComponent        terrainComponent     = null;        
        /****************************************************************************/


        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(TerrainComponent      terrainComponent,Matrix world)
        {
            this.terrainComponent     = terrainComponent;            
            this.World                = world;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            terrainComponent.ReleaseMe();            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            BackgroundTerrainData data = new BackgroundTerrainData();
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

            return data;
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// Background Terrain Data
    /********************************************************************************/
    [Serializable]
    public class BackgroundTerrainData : GameObjectInstanceData
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
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/