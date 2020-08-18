using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPhysicsExercise
{
    public class RegularShape : ShapeObject
    {
        [SerializeField] [Range(3, 10)] int shapeEdge = 3;

        public override Vector3[] ShapeVerticesBasis { get; set; }

        void OnValidate()
        {
            SetRegularVertices();
        }

        void SetRegularVertices()
        {
            ShapeVerticesBasis = new Vector3[shapeEdge];

            for (int i = 0; i < shapeEdge; i++)
            {
                float degree = 360 / shapeEdge * i;
                ShapeVerticesBasis[i] = DegreeOfPoint(degree);
            }
        }

        void OnDrawGizmos()
        {
            GizmoDrawShapeTransformation();
        }




    }
}

