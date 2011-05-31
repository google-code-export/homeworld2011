using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using JigLibX.Geometry;
using JigLibX.Collision;
using JigLibX.Physics;
using JigLibX.Math;
using JigLibX.Utils;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Resources;


/****************************************************************************/
/// PlagueEngine.Physics.Components
/****************************************************************************/
namespace PlagueEngine.Physics.Components
{


    /****************************************************************************/
    /// TerrainSkinComponent
    /****************************************************************************/
    class TerrainSkinComponent : CollisionSkinComponent
    {

        public string HeightmapSkin { get; set; }
        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public TerrainSkinComponent(GameObjectInstance gameObject,
                                    Texture2D heightMap,
                                    //Model model,
                                    string HeightmapSkin,
                                    int width,
                                    int length,
                                    float height,
                                    float scale,
                                    MaterialProperties material)
            : base(false,gameObject, material,Vector3.Zero,0,0,0)
        {
            this.HeightmapSkin = HeightmapSkin;


            Color[] heightMapData = new Color[heightMap.Width * heightMap.Height];
            heightMap.GetData<Color>(heightMapData);


            Array2D field = new Array2D(width, length);
            float vertexHeight;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < length; y++)
                {
                    int t1 = (heightMap.Height > length ? y * (heightMap.Height / length) : y / (length / heightMap.Height));
                    int t2 = (heightMap.Width > width ? x * (heightMap.Width / width) : x / (width / heightMap.Width));
                    vertexHeight = heightMapData[(t1 * heightMap.Width) + t2].R;
                    vertexHeight /= 255;
                    vertexHeight *= height;
                    field.SetAt(x, y, vertexHeight);
                }
            }

            skin.AddPrimitive(new Heightmap(field, scale * width / 2 - scale / 2, scale * length / 2 - scale / 2, scale, scale), material);

            //HeightMapInfo heightMapInfo = model.Tag as HeightMapInfo;
            //Array2D field = new Array2D(heightMapInfo.heights.GetLength(0), heightMapInfo.heights.GetLength(1));

            //for (int x = 0; x < heightMapInfo.heights.GetLength(0); x++)
            //{
            //    for (int z = 0; z < heightMapInfo.heights.GetLength(1); z++)
            //    {
            //        field.SetAt(x, z, heightMapInfo.heights[x, z] * height);
            //    }
            //}

            //skin.AddPrimitive(new Heightmap(field, 0, 0, scale, scale), new MaterialProperties(0.7f, 0.7f, 0.6f));

            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(skin);

        }
        /****************************************************************************/


        protected override void SetSkin(Matrix world)
        {
            base.SetSkin(world);
        }

    }
    /****************************************************************************/




}
/****************************************************************************/