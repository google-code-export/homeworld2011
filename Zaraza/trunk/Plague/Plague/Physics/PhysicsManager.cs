using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JigLibX.Physics;
using JigLibX.Collision;
using PlagueEngine.Physics;
using PlagueEngine.Physics.Components;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.LowLevelGameFlow.GameObjects;

/****************************************************************************/
///  PlagueEngine.Physics
/****************************************************************************/
namespace PlagueEngine.Physics
{
    /****************************************************************************/
    ///  PlagueEngine.Physics
    /****************************************************************************/
    class PhysicsManager
    {


        /****************************************************************************/
        ///  Fields
        /****************************************************************************/
        private PhysicsSystem physicsSystem = null;
        private List<PhysicsComponent> physicsComponents = new List<PhysicsComponent>();
        /****************************************************************************/




        /****************************************************************************/
        ///  Constructor
        /****************************************************************************/
        public PhysicsManager()
        {
            physicsSystem=new PhysicsSystem();
            physicsSystem.CollisionSystem=new CollisionSystemSAP();
            
        }
        /****************************************************************************/





        /****************************************************************************/
        ///  Update
        /****************************************************************************/
        public void Update(float timeStep)
        {
            PhysicsSystem.CurrentPhysicsSystem.Integrate(timeStep);
            
            foreach (PhysicsComponent component in physicsComponents)
            {
       
                component.UpdateWorldMatrix();
            }
        }

        /****************************************************************************/



        /****************************************************************************/
        ///  Release Component
        /****************************************************************************/
        public void ReleaseComponent(PhysicsComponent physicsComponent)
        {

            physicsComponents.Remove(physicsComponent);

        }
        /****************************************************************************/




        /****************************************************************************/
        ///  Create Physics Component
        /****************************************************************************/
        public PhysicsComponent CreatePhysicsComponent(GameObjectInstanceData data,GameObjectInstance gameObject)
        {
            PhysicsComponent result = null;

            if (data.Type.Name == "StaticMesh")
            {
                StaticMeshData smdata = (StaticMeshData)data;

                if(smdata.physicsComponentData.type.Name=="BoxPhysicsComponent")
                {
                    BoxPhysicsComponentData bpdata = (BoxPhysicsComponentData)smdata.physicsComponentData;
                    result =  new BoxPhysicsComponent(gameObject, this, bpdata.mass, bpdata.boxSize, bpdata.elasicity, bpdata.staticRoughness, bpdata.dynamicRoughness, bpdata.immovable,data.World);

                }
            }

            physicsComponents.Add(result);

            return result;
        }


        /****************************************************************************/


    }
    /****************************************************************************/



}
/****************************************************************************/