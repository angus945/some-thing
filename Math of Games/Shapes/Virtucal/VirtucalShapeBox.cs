using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPhysicsExercise
{
    public class VirtucalShapeBox : VirtucalShape
    {
        public override Vector3[] ShapeVerticesBasis { get; protected set; } = new Vector3[]
            {
                new Vector3(-1, 1) * 0.5f,
                new Vector3( 1, 1) * 0.5f,
                new Vector3( 1,-1) * 0.5f,
                new Vector3(-1,-1) * 0.5f,
            };

        public VirtucalShapeBox(Vector3 shapePosition, float shapeRotation, Vector3 shapeLocalScale) : base(shapePosition, shapeRotation, shapeLocalScale)
        {
            

        }
    }

}
