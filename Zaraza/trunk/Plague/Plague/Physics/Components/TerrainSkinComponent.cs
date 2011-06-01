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

        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public TerrainSkinComponent(GameObjectInstance gameObject,
                                    Matrix world,
                                    Texture2D heightMap,
<<<<<<< .mine
                                    //Model model,
                                    string HeightmapSkin,
=======
>>>>>>> .r436
                                    int width,
                                    int length,
                                    float height,
                                    float scale,
                                    MaterialProperties material)
            : base(false,gameObject, material,Vector3.Zero,0,0,0)
        {


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
                    field.SetAt(x, y, vertexHeight+world.Translation.Y);
                }
            }


   
            skin.AddPrimitive(new Heightmap(field,
                scale * width / 2.0f - scale / 2.0f+world.Translation.X,
                scale * length / 2.0f - scale / 2.0f + world.Translation.Z,
                scale, scale),
                material);


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