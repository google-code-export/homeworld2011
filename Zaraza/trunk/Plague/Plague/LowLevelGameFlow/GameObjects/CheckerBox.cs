using System;
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
using PlagueEngine.Pathfinder;

/********************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/********************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{


    /********************************************************************************/
    /// CheckerBox
    /********************************************************************************/
    class CheckerBox : GameObjectInstance
    {


        /********************************************************************************/
        /// Fields
        /********************************************************************************/
        public SquareBodyComponent body = null;
        public bool isCollision;
        public NodeType nodeType = NodeType.Blocked;
        int posX, posY;
        /********************************************************************************/



        /********************************************************************************/
        /// Init
        /********************************************************************************/
        public void Init(SquareBodyComponent physcisComponent,int posX,int posY)
        {
            this.body = physcisComponent;
            this.posX = posX;
            this.posY = posY;

            this.body.SubscribeAnyCollisionEvent();
        }
        /********************************************************************************/

        /****************************************************************************/
        /// On Event
        /****************************************************************************/
        public override void OnEvent(EventsSystem.EventsSender sender, EventArgs e)
        {
            if (e.GetType().Equals(typeof(AnyCollisionEvent)))
            {
                isCollision = true;
            }
        }



        /********************************************************************************/
        /// ReleaseComponents
        /********************************************************************************/
        public override void ReleaseComponents()
        {
            body.ReleaseMe();
        }
        /********************************************************************************/





        /********************************************************************************/
        /// GetData
        /********************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            CheckerBoxData data = new CheckerBoxData();
            GetData(data);

            data.Mass = body.Mass;
            data.Elasticity = body.Elasticity;
            data.StaticRoughness = body.StaticRoughness;
            data.DynamicRoughness = body.DynamicRoughness;
            data.Width = body.Width;
            data.Height = body.Height;
            data.Immovable = body.Immovable;
            data.Translation = body.SkinTranslation;
            data.SkinPitch = body.Pitch;
            data.SkinRoll = body.Roll;
            data.SkinYaw = body.Yaw;
            data.EnabledPhysics = body.Enabled;

            data.posX = posX;
            data.posY = posY;

            return data;
        }
        /********************************************************************************/



    }
    /********************************************************************************/




    /********************************************************************************/
    /// SquareBodyMeshData
    /********************************************************************************/
    [Serializable]
    public class CheckerBoxData : GameObjectInstanceData
    {


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

        [CategoryAttribute("Physics")]
        public bool EnabledPhysics { get; set; }


        [CategoryAttribute("Collision Skin")]
        public float Width { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float Height { get; set; }

        [CategoryAttribute("Collision Skin")]
        public Vector3 Translation { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float SkinYaw { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float SkinPitch { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float SkinRoll { get; set; }

        public int posX { get; set; }
        public int posY { get; set; }

    }
    /********************************************************************************/



}
/********************************************************************************/