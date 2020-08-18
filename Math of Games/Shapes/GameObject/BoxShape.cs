using UnityEngine;

namespace MyPhysicsExercise
{
    public class BoxShape : ShapeObject
    {
        public override Vector3[] ShapeVerticesBasis { get; set; } = new Vector3[]
            {
                new Vector3(-1, 1) * 0.5f,
                new Vector3( 1, 1) * 0.5f,
                new Vector3( 1,-1) * 0.5f,
                new Vector3(-1,-1) * 0.5f,
            };

        void OnDrawGizmos()
        {
            GizmoDrawShapeTransformation();
        }



    }
}