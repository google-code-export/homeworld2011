using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;
using PlagueEngine.Particles.Components;
using PlagueEngine.Physics;
using PlagueEngine.Rendering;

/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// AmmoBox
    /********************************************************************************/
    class AmmoBox : StorableObject
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public MeshComponent       mesh = null;
        public SquareBodyComponent body = null;

        public String AmmunitionName  { get; private set; }
        public uint   AmmunitionGenre { get; private set; }
        public bool   PPP             { get; private set; }

        public AmmunitionVersionInfo AmmunitionVersionInfo { get; private set; }
        private int amount = 0;

        public int Amount { get; set; }
        public int Capacity { get; private set; }
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(MeshComponent mesh,
                         SquareBodyComponent body,
                         ParticleEmitterComponent particle,
                         String ammunitionName,
                         uint ammunitionGenre,
                         bool ppp,
                         AmmunitionVersionInfo ammunitionVersionInfo,
                         int amount,
                         String description,
                         int descriptionWindowWidth,
                         int descriptionWindowHeight)
        {
            this.mesh = mesh;
            this.body = body;

            AmmunitionName = ammunitionName;
            AmmunitionGenre = ammunitionGenre;
            PPP = ppp;

            switch (ammunitionGenre)
            {
                case 1: Capacity = 60; break;
                case 2: Capacity = 45; break;
                case 3: Capacity = 30; break;
                case 4: Capacity = 12; break;
            }            

            AmmunitionVersionInfo = ammunitionVersionInfo;

            Amount = amount > Capacity ? Capacity : amount;
            
            StringBuilder sb = new StringBuilder();
            sb.Append(AmmunitionName);
            sb.Append(" (");
            sb.Append(AmmunitionVersionInfo.VersionToString(AmmunitionVersionInfo.Version));
            sb.Append(")");
            Name = sb.ToString();

            Rectangle icon      = new Rectangle(0,0,50,50);
            Rectangle slotsIcon = new Rectangle(0,0,64,64);

            icon.Y      = 964  + (114 * ((int)AmmunitionGenre - 1));
            slotsIcon.Y = 1014 + (114 * ((int)AmmunitionGenre - 1));

            icon.X      = (int)ammunitionVersionInfo.Version * 50;
            slotsIcon.X = (int)ammunitionVersionInfo.Version * 64;
                    
            if (!body.Immovable)
            {
                body.SubscribeCollisionEvent(typeof(Terrain));
            }

            Init(icon, slotsIcon, description, descriptionWindowWidth, descriptionWindowHeight, particle);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Owning
        /****************************************************************************/
        protected override void OnOwning(GameObjectInstance owner)
        {
            if (owner != null)
            {
                World = Matrix.Identity;
                if (mesh != null) mesh.Enabled = true;
                if (body != null) body.DisableBody();
            }
            else
            {
                if (getWorld != null) World = GetWorld();
                if (body != null) body.EnableBody();
                if (mesh != null) mesh.Enabled = true;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Event
        /****************************************************************************/
        public override void OnEvent(EventsSystem.EventsSender sender, EventArgs e)
        {
            if (e.GetType().Equals(typeof(CollisionEvent)))
            {
                CollisionEvent CollisionEvent = e as CollisionEvent;
                if (CollisionEvent.gameObject.GetType().Equals(typeof(Terrain)))
                {
                    body.Immovable = true;
                    body.CancelCollisionWithGameObjectsType(typeof(Terrain));
                }
            }
            else
            {
                base.OnEvent(sender, e);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// GetData
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            AmmoBoxData data = new AmmoBoxData();
            GetData(data);

            data.Immovable = body.Immovable;
            data.EnabledPhysics = body.Enabled;
            data.EnabledMesh = mesh.Enabled;

            data.AmmunitionName = AmmunitionName;
            data.Amount = Amount;
            data.Version = AmmunitionVersionInfo.Version;
            data.PPP = PPP;

            return data;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            if (mesh != null)
            {
                mesh.ReleaseMe();
                mesh = null;
            }

            if (body != null)
            {
                body.ReleaseMe();
                body = null;
            }

            base.ReleaseComponents();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// OnStoring
        /****************************************************************************/
        public override void OnStoring()
        {
            body.DisableBody();
            mesh.Enabled = false;

            base.OnStoring();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Dropping
        /****************************************************************************/
        public override void OnDropping()
        {
            body.EnableBody();
            mesh.Enabled = true;

            body.Immovable = false;
            body.SubscribeCollisionEvent(typeof(Terrain));

            base.OnDropping();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Picking
        /****************************************************************************/
        public override void OnPicking()
        {
            mesh.Enabled = true;
            body.DisableBody();

            base.OnPicking();
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// AmmoBoxData
    /********************************************************************************/
    [Serializable]
    class AmmoBoxData : StorableObjectData
    {
        public AmmoBoxData()
        {
            Type = typeof(AmmoBoxData);            
        }

        [CategoryAttribute("Ammunition")]
        public String   AmmunitionName  { get; set; }
        [CategoryAttribute("Ammunition")]
        public uint     Version         { get; set; }
        [CategoryAttribute("Ammunition")]
        public bool     PPP             { get; set; }
        [CategoryAttribute("Ammunition"),
        DescriptionAttribute("Pistol: 60, Intermediate: 45, Rifle: 30, Shotgun: 12")]
        public int      Amount          { get; set; }





        [CategoryAttribute("Model")]
        public bool EnabledMesh { get; set; }

        [CategoryAttribute("Physics")]
        public bool Immovable { get; set; }
        [CategoryAttribute("Physics")]
        public bool EnabledPhysics { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/