using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using PlagueEngine.LowLevelGameFlow;

using JigLibX.Geometry;
using JigLibX.Collision;
using JigLibX.Physics;


namespace PlagueEngine.AItest
{
    

    static class AI
    {

        static class GOcomparator
        {
            public static GameObjectInstance GO;


            public static int compareGO(GameObjectInstance go1, GameObjectInstance go2)
            {
                float d1 = Vector3.Distance(go1.World.Translation, GO.World.Translation);
                float d2 = Vector3.Distance(go2.World.Translation, GO.World.Translation);
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



        public static GameObjectInstance FindClosestVisible(List<GameObjectInstance> targets, GameObjectInstance owner,Vector3 ownerForward, float angle, float maxDistance,float rayForwardTranslation=0.75f)
        {

            //ownerForward.Y = 0;
            ownerForward=Vector3.Normalize(ownerForward);
            Vector3 ownerForwardCopy = ownerForward;


            List<GameObjectInstance> testedTargets = new List<GameObjectInstance>();

            //test odleglosci
            foreach (GameObjectInstance go in targets)
            {
                if (Vector3.Distance(go.World.Translation, owner.World.Translation) <= maxDistance)
                {
                    testedTargets.Add(go);
                }
                GOcomparator.GO=owner;
                
            }


            //sortowanie
            testedTargets.Sort(GOcomparator.compareGO);


            List<GameObjectInstance> testedTargets2 = new List<GameObjectInstance>();

            //testowanie kata widocznosci
            foreach (GameObjectInstance go in testedTargets)
            {
                Vector3 dir = Vector3.Normalize(go.World.Translation - owner.World.Translation);
                dir.Y = 0;
                ownerForward.Y = 0;
                float dot = Vector3.Dot(ownerForward, dir);
               
                float angleBetweenGO = MathHelper.ToDegrees((float)Math.Abs(Math.Acos(dot  )));

                if ( angleBetweenGO <= angle)
                {
                    testedTargets2.Add(go);
                }
            }


            //testowanie promieniem

            foreach(GameObjectInstance go in testedTargets2)
            {


            float distance;
            CollisionSkin skin;
            Vector3 skinPosition;
            Vector3 normal;
            Vector3 modifiedOwnerPosition = owner.World.Translation + rayForwardTranslation * ownerForwardCopy;
            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.SegmentIntersect(out distance, out skin, out skinPosition, out normal, new Segment(modifiedOwnerPosition, go.World.Translation - modifiedOwnerPosition), new ImmovableSkinPredicate());

            if (skin != null)
            {
                if (skin.ExternalData!=null && ((GameObjectInstance)(skin.ExternalData)).ID == go.ID)
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
