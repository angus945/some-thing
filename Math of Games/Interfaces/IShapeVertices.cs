using UnityEngine;

namespace MyPhysicsExercise
{
    public interface IShapeVertices
    {
        Vector3[] ShapeVerticesBasis { get; }
        Vector3[] ShapeVertices { get; }

        Vector3[] ShapeEdgeNoraml { get; }

        Matrix4x4 TransformMatrix { get; }
        Matrix4x4 PositioningMatrix { get; }
        Matrix4x4 RotatingMatrix { get; }
        Matrix4x4 ScalingMatrix { get; }
        QuadTreeRect ShapeToRect { get; }

        void MinMaxVertexProjection(Vector3 projectionTo, out float min, out float max);
        void MinMaxVertexProjection(Vector3 projectionTo, out Vector3 min, out Vector3 max);
        float ShapePositionProjection(Vector3 projectionTo);

    }

}
