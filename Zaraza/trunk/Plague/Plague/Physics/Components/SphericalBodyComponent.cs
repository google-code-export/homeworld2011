﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;

using JigLibX.Geometry;
using JigLibX.Collision;
using JigLibX.Physics;
using JigLibX.Math;
using PlagueEngine.LowLevelGameFlow;

namespace PlagueEngine.Physics.Components
{
    class SphericalBodyComponent:RigidBodyComponent
    {
        private float radius;

        public SphericalBodyComponent(GameObjectInstance gameObject,
                            float mass,
                            float radius,
                            MaterialProperties material,
                            bool immovable,
                            Matrix world) :base(gameObject,mass,immovable,material)
    {
            this.radius = radius;
            Sphere sphere = new Sphere(Vector3.Zero, radius);
            Skin.AddPrimitive(sphere, material);
            Vector3 com = SetMass();
            MoveTo(world);
            Skin.ApplyLocalTransform(new Transform(-com, Matrix.Identity));
            Enable();
    }


        public float Radius { get; set; }

    }
}
