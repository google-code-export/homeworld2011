﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.ComponentModel;

using PlagueEngine.EventsSystem;

/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow
{

    /********************************************************************************/
    /// Game Object Instance
    /********************************************************************************/
    abstract class GameObjectInstance : EventsSender, IEventsReceiver
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

        private GameObjectInstance owner     = null;        
        
        private delegate Matrix GetWorldDelegate(int bone);
        private GetWorldDelegate getWorld = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Get World
        /****************************************************************************/
        public Matrix GetWorld(int bone)
        {
            return getWorld(bone);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get My World 
        /****************************************************************************/
        protected virtual Matrix GetMyWorld(int bone)
        {
            return World;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Owner World 
        /****************************************************************************/
        private Matrix GetOwnerWorld(int bone)
        {
            return owner.GetWorld(OwnerBone) * GetMyWorld(bone);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public bool Init(uint id, String definition, Matrix world, GameObjectInstance owner = null,int ownerBone = -1)
        {
            if (id == 0) this.id = GenerateID();
            else
            {
                if (PickID(id)) this.id = id;
                else return false;
            }

            this.definition = definition;
            this.World      = world;

            Owner     = owner;
            OwnerBone = ownerBone;

            Broadcast(new CreateEvent());

            return true;            
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
            data.Owner         = (this.Owner == null ? 0 : this.Owner.ID);
            data.OwnerBone     = this.OwnerBone;

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
            data.Owner         = (this.Owner == null ? 0 : this.Owner.ID);
            data.OwnerBone     = this.OwnerBone;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public String Definition { get { return definition; } }        
        public uint   ID         { get { return id; } }
        public int    OwnerBone  { get; set; }
        /****************************************************************************/


        /****************************************************************************/
        /// Owner
        /****************************************************************************/
        public GameObjectInstance Owner
        {
            get { return owner; }
            set
            {
                if (value == null) getWorld = this.GetMyWorld;
                else
                {
                    getWorld = this.GetOwnerWorld;
                    owner = value;
                }
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
            Broadcast(new DestroyEvent());
        }
        /****************************************************************************/


        /****************************************************************************/
        /// IsDisposed
        /****************************************************************************/
        public bool IsDisposed()
        {
            return isDisposed;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Event
        /****************************************************************************/
        public virtual void OnEvent(EventsSender sender, EventArgs e)
        {
            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get String
        /****************************************************************************/
        public override String ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(this.GetType().Name);
            builder.Append(" - ");
            builder.Append(id);

            return builder.ToString();
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

        
        public Vector3 Position
        {
            get { return this.World.Translation; }

            set { this.World.Translation = value; }
        }




        [CategoryAttribute("Rotation")]
        public float Roll
        {
            set
            {
                Rotate(World.Forward, value);               
            }

            get
            {
                return 0;
            }

        }
        [CategoryAttribute("Rotation")]
        public float Yaw
        {
            set
            {
                Rotate(World.Up, value); 
            }

            get
            {
                return 0;
            }

        }
        [CategoryAttribute("Rotation")]
        public float Pitch
        {
            set
            {
                Rotate(World.Right, value);
            }

            get
            {
                return 0;
            }

        }


        [CategoryAttribute("Owner")]
        public uint Owner     { get; set; }
        [CategoryAttribute("Owner")]
        public int  OwnerBone { get; set; }

        /****************************************************************************/
        /// Rotate
        /****************************************************************************/
        private void Rotate(Vector3 vector, float angle)
        {
            angle = MathHelper.ToRadians(angle);
            Quaternion quaternion = Quaternion.CreateFromAxisAngle(vector, angle);
            World.Forward         = Vector3.Transform(World.Forward, quaternion);
            World.Right           = Vector3.Transform(World.Right, quaternion);
            World.Up              = Vector3.Transform(World.Up, quaternion);
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/