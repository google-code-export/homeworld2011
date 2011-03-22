using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow
{

    /********************************************************************************/
    /// Game Object Instance
    /********************************************************************************/
    abstract class GameObjectInstance : IDisposable
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        internal Matrix World;

        private  uint   id         = 0;        
        private  String definition = String.Empty;
        private  bool   isDisposed = false;
                
        private static uint       lastID    = 0;
        private static List<uint> freeIDs   = new List<uint>();
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public bool Init(uint id,String definition)
        {
            if (id == 0) this.id = GenerateID();
            else
            {
                if (PickID(id)) this.id = id;
                else return false;
            }

            this.definition = definition;
            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// ID
        /****************************************************************************/
        public uint ID
        {
            get
            {
                return id;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Generate ID
        /****************************************************************************/
        private static uint GenerateID()
        {

       return ++lastID;
           

            if (freeIDs.Count != 0)
            {
                uint id = freeIDs[freeIDs.Count - 1];
                freeIDs.RemoveAt(freeIDs.Count - 1);
                return id;
            }
            else
            {
                return ++lastID;
            }

        }
        /****************************************************************************/


        /****************************************************************************/
        /// Pick ID
        /****************************************************************************/
        private static bool PickID(uint id)
        {
            if (freeIDs.Contains(id))
            {
                freeIDs.Remove(id);
                return true;
            }
            else if (id < lastID)
            {
                return false;
            }
            else
            {
                for (uint i = lastID; i < id; i++)
                {
                    freeIDs.Add(i);
                }
                lastID = id;
                return true;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release ID
        /****************************************************************************/
        private static void ReleaseID(uint id)
        {
            if (freeIDs.Contains(id)) return;

            if (id > lastID) return;
            else if (id == lastID)
            {
                --lastID;
                return;
            }

            freeIDs.Add(id);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public virtual GameObjectInstanceData GetData()
        { 
            GameObjectInstanceData data = new GameObjectInstanceData();
            
            data.ID            = this.id;
            data.Type          = this.GetType();
            data.World         = this.World;
            data.Definition    = this.definition;
            
            return data;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public void GetData(GameObjectInstanceData data)
        {
            data.ID            = this.id;
            data.Type          = this.GetType();
            data.World         = this.World;
            data.Definition    = this.definition;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Definition
        /****************************************************************************/
        public String Definition
        {
            get
            {
                return definition;
            }
        }        
        /****************************************************************************/
        
        
        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public abstract void ReleaseComponents();
        /****************************************************************************/


        /****************************************************************************/
        /// Dispose
        /****************************************************************************/
        public void Dispose()
        {         
            ReleaseID(this.id);
            ReleaseComponents();
            isDisposed = true;
        }
        /****************************************************************************/


    }
    /********************************************************************************/


    /********************************************************************************/
    /// Game Object Instance Data
    /********************************************************************************/
    [Serializable]
    public class GameObjectInstanceData
    {
        public uint     ID              = 0;
        public Type     Type            = null;
        public String   Definition      = String.Empty;
        public Matrix   World           = Matrix.Identity;

        public String definition
        {
            get { return this.Definition; }
            set { this.Definition = value; }
        }

       
        public Vector3 position
        {
            get { return World.Translation; }
            set { World = Matrix.CreateWorld(value, Vector3.Forward, Vector3.Up); }
        }



        public Vector3 rotation
        {
            get
            {
                float roll = MathHelper.ToDegrees((float)Math.Atan2(World.M12, World.M11));
                float pitch = MathHelper.ToDegrees((float)(Math.Acos(World.M13) * -1));
                float yaw = MathHelper.ToDegrees((float)Math.Atan2(World.M23, World.M33));

                return new Vector3(yaw, pitch, roll);
            }

            set
            {
                Vector3 Rotation = rotation;
                World = Matrix.CreateWorld(position, Vector3.Forward, Vector3.Up);
                World *= Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(value.Y), MathHelper.ToRadians(value.X), MathHelper.ToRadians(value.Z));
                
                        
            }
        }

        public Vector3 scale
        {
            set
            {
                World = Matrix.CreateWorld(position, Vector3.Forward, Vector3.Up);
                World *= Matrix.CreateScale(value);
            }
            get
            {
                float x = new Vector3(World.M11, World.M12, World.M13).Length();
                float y = new Vector3(World.M21, World.M22, World.M23).Length();
                float z = new Vector3(World.M31, World.M32, World.M33).Length();

                return new Vector3(x, y, z);
            }
        }

    }
    /********************************************************************************/

}
/************************************************************************************/