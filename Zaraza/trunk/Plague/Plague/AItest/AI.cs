using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using PlagueEngine.LowLevelGameFlow;

using JigLibX.Geometry;
using JigLibX.Collision;
using JigLibX.Physics;
using PlagueEngine.ArtificialInteligence.Controllers;
using PlagueEngine.LowLevelGameFlow.GameObjects;


namespace PlagueEngine.AItest
{
    

    static class AI
    {

        static class GOcomparator
        {
            public static AbstractAIController GO;


            public static int compareGO(AbstractAIController go1, AbstractAIController go2)
            {
                float d1 = Vector3.Distance(go1.controlledObject.World.Translation, GO.controlledObject.World.Translation);
                float d2 = Vector3.Distance(go2.controlledObject.World.Translation, GO.controlledObject.World.Translation);
                return d1.CompareTo(d2);
            }
        }


        class ImmovableSkinPredicate : CollisionSkinPredicate1
        {
            public override bool ConsiderSkin(CollisionSkin skin0)
            {
                if (skin0 != null)
                    return true;
                else
                    return false;
            }
        }



        public static AbstractAIController FindClosestVisible(List<AbstractAIController> targets, AbstractAIController owner, Vector3 ownerForward, float angle, float maxDistance, float rayForwardTranslation = 0.75f)
        {

            //ownerForward.Y = 0;
            ownerForward=Vector3.Normalize(ownerForward);
            Vector3 ownerForwardCopy = ownerForward;


            List<AbstractAIController> testedTargets = new List<AbstractAIController>();

            //test odleglosci
            foreach (AbstractAIController go in targets)
            {
                if (Vector3.Distance(go.controlledObject.World.Translation, owner.controlledObject.World.Translation) <= maxDistance)
                {
                    testedTargets.Add(go);
                }
                GOcomparator.GO=owner;
                
            }


            //sortowanie
            testedTargets.Sort(GOcomparator.compareGO);


            List<AbstractAIController> testedTargets2 = new List<AbstractAIController>();

            
            //testowanie kata widocznosci
            foreach (AbstractAIController go in testedTargets)
            {
                
                
                //if (Math.Abs(angle) > AnglePrecision) controlledObject.Controller.Rotate(MathHelper.ToDegrees(angle) * RotationSpeed * (float)deltaTime.TotalSeconds);


                Vector3 dir = (go.controlledObject.World.Translation - owner.controlledObject.World.Translation);
                Vector2 v1 = Vector2.Normalize(new Vector2(dir.X, dir.Y));
                Vector2 v2 = Vector2.Normalize(new Vector2(owner.controlledObject.World.Forward.X, 
                                                owner.controlledObject.World.Forward.Z));
                //float det = v1.X * v2.Y - v1.Y * v2.X;
                float angleBetweenGO = (float)Math.Acos((double)Vector2.Dot(v1, v2));
                //if (det < 0) angleBetweenGO = -angleBetweenGO;

                //dir.Y = 0;
                //ownerForward.Y = 0;
                //float dot = Vector3.Dot(ownerForward, dir);
               
                //float angleBetweenGO = MathHelper.ToDegrees((float)Math.Acos(dot));

                if ( Math.Abs(angleBetweenGO) <= angle)
                {
                    testedTargets2.Add(go);
                }
            }


            //testowanie promieniem

            foreach (AbstractAIController go in testedTargets2)
            {


            float distance;
            CollisionSkin skin;
            Vector3 skinPosition;
            Vector3 normal;
            Vector3 modifiedOwnerPosition = owner.controlledObject.World.Translation + rayForwardTranslation * ownerForwardCopy;
            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.SegmentIntersect(out distance, out skin, out skinPosition, out normal, new Segment(modifiedOwnerPosition, go.controlledObject.World.Translation - modifiedOwnerPosition), new ImmovableSkinPredicate());

            if (skin != null)
            {
                if (skin.ExternalData!=null && ((GameObjectInstance)(skin.ExternalData)).ID == go.controlledObject.ID)
                {
                    //znaleziony!!!
                    return go;
                }
            }

            }


            return null;
        }






    }
}
