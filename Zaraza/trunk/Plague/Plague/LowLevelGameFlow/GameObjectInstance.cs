using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.ComponentModel;

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
                for (uint i = lastID + 1; i < id; i++)
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


        private Vector3 position = Vector3.Zero;
        private Vector3 rotation = Vector3.Zero;
        private Vector3 scale = Vector3.One;

        public String definition
        {
            get { return this.Definition; }
            set { this.Definition = value; }
        }

       
        public Vector3 Position
        {
            get { return this.position; }

            set {
                this.position = value;
                World = Matrix.Identity;
                World *= Matrix.CreateScale(Scale);
                World *= Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(Yaw),MathHelper.ToRadians( Pitch),MathHelper.ToRadians( Roll));
                World *= Matrix.CreateTranslation(value);
                
                }
        }

        [CategoryAttribute("Rotation")]
        public float Roll
        {
            set
            { 
                this.rotation.X = value;
             

                World = Matrix.Identity;
                World *= Matrix.CreateScale(Scale);
                World *= Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(Yaw), MathHelper.ToRadians(Pitch), MathHelper.ToRadians(value));
                World *= Matrix.CreateTranslation(position);
                
            }

            get
            {
                return this.rotation.X;
            }

        }
        [CategoryAttribute("Rotation")]
        public float Yaw
        {
            set
            {
                this.rotation.Y = value;
            
                World = Matrix.Identity;
                World *= Matrix.CreateScale(Scale);
                World *= Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(value), MathHelper.ToRadians(Pitch), MathHelper.ToRadians(Roll));
                World *= Matrix.CreateTranslation(position);
                
            }

            get
            {
                return this.rotation.Y;
            }

        }
        [CategoryAttribute("Rotation")]
        public float Pitch
        {
            set
            {
                this.rotation.X = value;

                World = Matrix.Identity;
                World *= Matrix.CreateScale(Scale);
                World *= Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(Yaw), MathHelper.ToRadians(value), MathHelper.ToRadians(Roll));
                World *= Matrix.CreateTranslation(position);
                
            }

            get
            {
                return this.rotation.Z;

            }

        }

        public Vector3 Scale
        {
            set
            {
                this.scale = value;
            

                World = Matrix.Identity;
                World *= Matrix.CreateScale(value);
                World *= Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(Yaw), MathHelper.ToRadians(Pitch), MathHelper.ToRadians(Roll));
                World *= Matrix.CreateTranslation(position);
                
            }
            get
            {
                return this.scale;
            }
        }

    }
    /********************************************************************************/

}
/************************************************************************************/