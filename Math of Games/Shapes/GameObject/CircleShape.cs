using UnityEngine;

namespace MyPhysicsExercise
{
    public class CircleShape : ShapeObject
    {
        public override Vector3[] ShapeVerticesBasis { get; set; } = new Vector3[1] { Vector3.zero };
        public override QuadTreeRect ShapeToRect { get => new QuadTreeRect(Position, LocalScale); }

        float CircleRadius { get => LocalScale.x / 2; }

        void OnDrawGizmos()
        {
            GizmoDrawShapeTransformation();
        }

        public override void MinMaxVertexProjection(Vector3 projectionTo, out Vector3 min, out Vector3 max)
        {
            projectionTo = projectionTo.normalized / 2;

            Matrix4x4 projMatrix = PositioningMatrix * ScalingMatrix;

            min = projMatrix.MultiplyPoint3x4(projectionTo);
            max = projMatrix.MultiplyPoint3x4(-projectionTo);
        }
        public override void MinMaxVertexProjection(Vector3 projectionTo, out float min, out float max)
        {
            float posProj = ShapePositionProjection(projectionTo);

            min = posProj - CircleRadius;
            max = posProj + CircleRadius;
        }

        protected override void LockTransform()
        {
            base.LockTransform();
            ShapeTransform.rotation = Quaternion.identity;
            ShapeTransform.localScale = new Vector3(LocalScale.x, LocalScale.x);
        }

        protected override void GizmoDrawShapeBasis()
        {
            Gizmos.color = shapeColor;

            Vector3 lastPoint = DegreeOfPoint(0);
            for (int deg = 1; deg < 360; deg++)
            {
                Vector3 point = DegreeOfPoint(deg);

                Gizmos.DrawLine(lastPoint, point);

                lastPoint = point;
            }

            Gizmos.DrawLine(lastPoint, DegreeOfPoint(0));
        }
        protected override void GizmoDrawShapeTransformation()
        {
            Gizmos.color = shapeColor;

            Vector3 lastPoint = TransformMatrix.MultiplyPoint3x4(DegreeOfPoint(0));
            for (int deg = 1; deg < 360; deg++)
            {
                Vector3 point = TransformMatrix.MultiplyPoint3x4(DegreeOfPoint(deg));

                Gizmos.DrawLine(lastPoint, point);

                lastPoint = point;
            }

            Gizmos.DrawLine(lastPoint, TransformMatrix.MultiplyPoint3x4(DegreeOfPoint(0)));
        }



    }
}