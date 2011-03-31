using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JigLibX.Physics;
using JigLibX.Collision;
using PlagueEngine.Physics;


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
        ///  Update
        /****************************************************************************/
        public void ReleaseComponent(PhysicsComponent physicsComponent)
        {

            foreach (PhysicsComponent component in physicsComponents)
            {
                if (component == physicsComponent)
                {
                    physicsSystem.RemoveBody(component.Body);

                    physicsComponents.Remove(component);
                }
            }

        }
        /****************************************************************************/



    }
    /****************************************************************************/



}
/****************************************************************************/