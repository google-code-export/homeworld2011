using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.ComponentModel;

using PlagueEngine.EventsSystem;
using PlagueLocalizationExtension;

/************************************************************************************/
// PlagueEngine.LowLevelGameFlow
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow
{

    /********************************************************************************/
    // Game Object Instance
    /********************************************************************************/
    abstract class GameObjectInstance : EventsSender, IEventsReceiver
    {

        /****************************************************************************/
        // Fields
        /****************************************************************************/
        internal Matrix World;

        private  String _definition = String.Empty;
        private  bool   _isDisposed;
                
        private static int       _lastId;
        private static readonly List<int> FreeIDs   = new List<int>();
        public static GameObjectsFactory Factory;

        protected GameObjectInstance owner;        
        
        protected delegate Matrix GetWorldDelegate(int bone);
        protected GetWorldDelegate getWorld;
        /****************************************************************************/


        public void SetTranslation(Vector3 translation)
        {
            World.Translation = translation;
        }

        /****************************************************************************/
        // Get World
        /****************************************************************************/
        public Matrix GetWorld(int bone)
        {
            return getWorld(bone);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get World
        /****************************************************************************/
        public Matrix GetWorld()
        {
            return getWorld(-1);
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
        protected Matrix GetOwnerWorld(int bone)
        {
            return GetMyWorld(bone) * owner.GetWorld(OwnerBone);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public bool Init(int id, String definition, Matrix world, GameObjectStatus status,String name,GameObjectInstance owner = null,int ownerBone = -1)
        {
            if (id == 0)
            {
                ID = GenerateID();
            }
            else if (id > 0)
            {
                if (PickID(id)) this.ID = id;
                else return false;
            }
            else
            {
                ID = id;
            }

            _definition = definition;
            World      = world;

            Owner     = owner;
            OwnerBone = ownerBone;
            Status    = status;
            Name      = name;

            return true;            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Generate ID
        /****************************************************************************/
        private static int GenerateID()
        {
            if (FreeIDs.Count == 0)
            {
                return ++_lastId;
            }
            var id = FreeIDs[FreeIDs.Count - 1];
            FreeIDs.RemoveAt(FreeIDs.Count - 1);
            return id;
        }

        /****************************************************************************/


        /****************************************************************************/
        /// Pick ID
        /****************************************************************************/
        private static bool PickID(int id)
        {
            if (FreeIDs.Contains(id))
            {
                FreeIDs.Remove(id);
                return true;
            }
            if (id < _lastId)
            {
                return false;
            }
            for (var i = _lastId + 1; i < id; i++)
            {
                FreeIDs.Add(i);
            }
            _lastId = id;
            return true;
        }

        /****************************************************************************/


        /****************************************************************************/
        /// Release ID
        /****************************************************************************/
        private static void ReleaseID(int id)
        {
            if (FreeIDs.Contains(id)) return;

            if (id > _lastId) return;
            if (id == _lastId)
            {
                --_lastId;
                return;
            }

            FreeIDs.Add(id);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public virtual GameObjectInstanceData GetData()
        {
            var data = new GameObjectInstanceData
                           {
                               ID = ID,
                               Type = GetType(),
                               World = World,
                               Definition = _definition,
                               Owner = (Owner == null ? 0 : Owner.ID),
                               OwnerBone = OwnerBone,
                               Name = Name
                           };
            return data;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public void GetData(GameObjectInstanceData data)
        {
            data.ID            = ID;
            data.Type          = GetType();
            data.World         = World;
            data.Definition    = _definition;
            data.Owner         = (Owner == null ? 0 : Owner.ID);
            data.Status        = StatusToUint(Status);
            data.OwnerBone     = OwnerBone;
            data.Name          = Name;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public String Definition { get { return _definition; } }
        public int ID { get; private set; }
        public String Name       { get; protected set; }
        public int    OwnerBone  { get; set; }
        
        public bool             RequiresUpdate { get; protected set; }
        public GameObjectStatus Status         { get; protected set; }
        /****************************************************************************/


        /****************************************************************************/
        /// Owner
        /****************************************************************************/
        public GameObjectInstance Owner
        {
            get { return owner; }
            set
            {
                OnOwning(value);
                getWorld = value == null ? (GetWorldDelegate) GetMyWorld : GetOwnerWorld;
                owner = value;                
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        // Release Components
        /****************************************************************************/
        public virtual void ReleaseComponents()
        { 
        
        }
        /****************************************************************************/


        /****************************************************************************/
        // Dispose
        /****************************************************************************/
        public void Dispose()
        {
            if (_isDisposed) return;

            ReleaseID(ID);
            ReleaseComponents();
            _isDisposed = true;
            Broadcast(new DestroyEvent());
        }
        /****************************************************************************/


        /****************************************************************************/
        /// IsDisposed
        /****************************************************************************/
        public bool IsDisposed()
        {
            return _isDisposed;
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
        /// Update
        /****************************************************************************/
        public virtual void Update(TimeSpan deltaTime)
        { 
        
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Owning
        /****************************************************************************/
        protected virtual void OnOwning(GameObjectInstance owner)
        { 
        
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get String
        /****************************************************************************/
        public override String ToString()
        {
            var builder = new StringBuilder();
            builder.Append(GetType().Name);
            builder.Append(" - ");
            builder.Append(ID);

            return builder.ToString();
        } 
        /****************************************************************************/

                
        /****************************************************************************/
        /// StatusToUint
        /****************************************************************************/
        public static uint StatusToUint(GameObjectStatus status)
        { 
            switch(status)
            {
                case GameObjectStatus.Nothing:      return 0;
                case GameObjectStatus.Interesting:  return 1;
                case GameObjectStatus.Pickable:     return 2;
                case GameObjectStatus.Mercenary:    return 3;
                case GameObjectStatus.Targetable:   return 4;
                case GameObjectStatus.Passable:     return 5;
                default: return 0;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CreateGameObject
        /****************************************************************************/
        public static GameObjectInstance CreateGameObject(GameObjectInstanceData data)
        {
            return Factory.Create(data);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// UintToStatus
        /****************************************************************************/
        public static GameObjectStatus UintToStatus(uint status)
        {
            switch (status)
            {
                case 0:  return GameObjectStatus.Nothing;
                case 1:  return GameObjectStatus.Interesting;
                case 2:  return GameObjectStatus.Pickable;
                case 3:  return GameObjectStatus.Mercenary;
                case 4:  return GameObjectStatus.Targetable;
                case 5:  return GameObjectStatus.Passable;
                default: return GameObjectStatus.Nothing;
            }
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// GameObjectStatus
    /********************************************************************************/
    enum GameObjectStatus
    { 
        Nothing,
        Interesting,
        Pickable,
        Mercenary,
        Targetable,
        Passable
    }
    /********************************************************************************/


    /********************************************************************************/
    /// Game Object Instance Data
    /********************************************************************************/
    [Serializable]
    public class GameObjectInstanceData
    {
        public int      ID;
        public Type     Type;
        public Matrix   World           = Matrix.Identity;

            [CategoryAttribute("Name")]
        public String Name { get; set; }

        [CategoryAttribute("Definition")]
        public String Definition { get; set; }

        [CategoryAttribute("Position")]        
        public Vector3 Position { get { return World.Translation; } set { World.Translation = value; } }
        
        [CategoryAttribute("Rotation")]
        public float Roll  { set { Rotate(World.Forward, value); } get { return 0; } }
        [CategoryAttribute("Rotation")]
        public float Yaw   { set { Rotate(World.Up,      value); } get { return 0; } }
        [CategoryAttribute("Rotation")]
        public float Pitch { set { Rotate(World.Right,   value); } get { return 0; } }
        
        [CategoryAttribute("Owner")]
        public int Owner     { get; set; }
        [CategoryAttribute("Owner")]
        public int  OwnerBone { get; set; }

        [CategoryAttribute("Status"),
         DescriptionAttribute("0: - GameObjectStatus.Nothing\n"     + 
                              "1: - GameObjectStatus.Interesting\n" +
                              "2: - GameObjectStatus.Pickable\n"    +
                              "3: - GameObjectStatus.Mercenary\n"   +
                              "4: - GameObjectStatus.Targetable\n"  + 
                              "5: - GameObjectStatus.Walk\n")]        
        public uint Status { get; set; }

        /****************************************************************************/
        /// Rotate
        /****************************************************************************/
        private void Rotate(Vector3 vector, float angle)
        {
            angle = MathHelper.ToRadians(angle);
            var quaternion        = Quaternion.CreateFromAxisAngle(vector, angle);
            World.Forward         = Vector3.Transform(World.Forward, quaternion);
            World.Right           = Vector3.Transform(World.Right, quaternion);
            World.Up              = Vector3.Transform(World.Up, quaternion);
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// GlobalGameObjects
    /********************************************************************************/
    class GlobalGameObjects
    {
        public static int GameController        = -1;
        public static int Ammunition            = -2;
        public static LangContent StringManager;
    }
    /********************************************************************************/

}
/************************************************************************************/