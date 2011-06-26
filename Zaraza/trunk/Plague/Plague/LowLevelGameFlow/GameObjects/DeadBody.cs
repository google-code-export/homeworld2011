using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;

namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class DeadBody : GameObjectInstance, IActiveGameObject
    {  /****************************************************************************/
        /// Fields
        /****************************************************************************/
        protected SkinnedMeshComponent mesh = null;
        protected CapsuleBodyComponent body = null;

        private bool open = false;

        private int DescriptionWindowWidth = 0;
        private int DescriptionWindowHeight = 0;
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public uint Slots { get; private set; }
        public Dictionary<StorableObject, ItemPosition> Items { get; private set; }
        public String Description { get; private set; }
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(SkinnedMeshComponent mesh,
                         CapsuleBodyComponent body,
                         String description,
                         int descriptionWindowWidth,
                         int descriptionWindowHeight,
                         uint slots,
                         Dictionary<StorableObject, ItemPosition> items)
        {
            this.mesh = mesh;
            this.body = body;

            Description = description;
            Status = GameObjectStatus.Pickable;

            mesh.Stop();

            Slots = slots;
            Items = items;

            DescriptionWindowHeight = descriptionWindowHeight;
            DescriptionWindowWidth = descriptionWindowWidth;

            mesh.BlendTo("Dying", TimeSpan.FromSeconds(0.3f));
        }
        /****************************************************************************/


        /****************************************************************************/
        /// GetActions
        /****************************************************************************/
        public string[] GetActions()
        {
            return new string[0];
        }
        /****************************************************************************/


        /****************************************************************************/
        /// GetActions
        /****************************************************************************/
        public string[] GetActions(Mercenary mercenary)
        {
            return new string[] { "Get Items", "Examine" };
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Event
        /****************************************************************************/
        public override void OnEvent(EventsSystem.EventsSender sender, EventArgs e)
        {
            if (e.GetType().Equals(typeof(ExamineEvent)))
            {
                DescriptionWindowData data = new DescriptionWindowData();

                data.Title = Name;
                data.Text = Description;
                data.Width = DescriptionWindowWidth;
                data.Height = DescriptionWindowHeight;
                SendEvent(new CreateObjectEvent(data), EventsSystem.Priority.Normal, GlobalGameObjects.GameController);
            }
            else if (e.GetType().Equals(typeof(OpenEvent)) && !open)
            {
                OpenEvent OpenEvent = e as OpenEvent;

                var data = new InventoryData
                {
                    Mercenary = OpenEvent.mercenary.ID,
                    Container = ID
                };

                SendEvent(new CreateObjectEvent(data), EventsSystem.Priority.High, GlobalGameObjects.GameController);

                open = true;

            }
            else if (e.GetType().Equals(typeof(CloseEvent)) && open)
            {
                open = false;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// GetData
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            DeadBodyData data = new DeadBodyData();
            GetData(data);
            data.Model = mesh.Model.Name;
            data.Diffuse = (mesh.Textures.Diffuse == null ? String.Empty : mesh.Textures.Diffuse.Name);
            data.Specular = (mesh.Textures.Specular == null ? String.Empty : mesh.Textures.Specular.Name);
            data.Normals = (mesh.Textures.Normals == null ? String.Empty : mesh.Textures.Normals.Name);

            data.Mass = body.Mass;
            data.Elasticity = body.Elasticity;
            data.StaticRoughness = body.StaticRoughness;
            data.DynamicRoughness = body.DynamicRoughness;

            data.Length = body.Length;
            data.Radius = body.Radius;
            data.Immovable = body.Immovable;
            data.Translation = body.SkinTranslation;
            data.SkinPitch = body.Pitch;
            data.SkinRoll = body.Roll;
            data.SkinYaw = body.Yaw;
            data.EnabledPhysics = body.Enabled;

            data.DescriptionWindowWidth = DescriptionWindowWidth;
            data.DescriptionWindowHeight = DescriptionWindowHeight;
            data.Description = Description;

            data.Slots = Slots;

            data.Items = new List<int[]>();

            foreach (var item in Items)
            {
                data.Items.Add(new[] { item.Key.ID, item.Value.Slot, item.Value.Orientation });
            }

            return data;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            mesh.ReleaseMe();
            mesh = null;
            body.ReleaseMe();
            body = null;
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// ContainerData
    /********************************************************************************/
    [Serializable]
    public class DeadBodyData : GameObjectInstanceData
    {
        public DeadBodyData()
        {
            Type = typeof(Container);
        }


        [CategoryAttribute("Model")]
        public String Model { get; set; }

        [CategoryAttribute("Textures")]
        public String Diffuse { get; set; }
        [CategoryAttribute("Textures")]
        public String Specular { get; set; }
        [CategoryAttribute("Textures")]
        public String Normals { get; set; }

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
        public float Length { get; set; }
        [CategoryAttribute("Collision Skin")]
        public float Radius { get; set; }
        [CategoryAttribute("Collision Skin")]
        public Vector3 Translation { get; set; }
        [CategoryAttribute("Collision Skin")]
        public float SkinYaw { get; set; }
        [CategoryAttribute("Collision Skin")]
        public float SkinPitch { get; set; }
        [CategoryAttribute("Collision Skin")]
        public float SkinRoll { get; set; }

        [CategoryAttribute("Description")]
        public String Description { get; set; }
        [CategoryAttribute("Description")]
        public int DescriptionWindowWidth { get; set; }
        [CategoryAttribute("Description")]
        public int DescriptionWindowHeight { get; set; }

        [CategoryAttribute("Inventory")]
        public uint Slots { get; set; }
        [CategoryAttribute("Inventory")]
        public List<int[]> Items { get; set; }

        

        
    }
    /********************************************************************************/
}
