using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPhysicsExercise
{
    public enum RigidbodyType
    {
        Dynamic,
        Kinmatic,
        Static,
    }
    public interface IShapeRigidbody
    {
        RigidbodyType RigidbodyType { get; set; }

        float BodyMass { get; set; }
        float LinearDrag { get; set; }

        Vector3 BodyPosition { get; }
        Vector3 BodyRotation { get; }

        Vector3 Velocity { get; set; }
        Vector3 CurrentForce { get; }

        void RigidBodyMotion(float timeStep);
        void ApplyForces();
        void AddGravity(Vector3 gravity);
        void AddForce(Vector3 force);

    }

}
