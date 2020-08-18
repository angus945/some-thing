using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MyPhysicsExercise
{
    public class VirtucalShape : IShapeVertices
    {

        public virtual Vector3[] ShapeVerticesBasis { get; protected set; }
        public Vector3[] ShapeVertices
        {
            get
            {
                Vector3[] vertices = new Vector3[ShapeVerticesBasis.Length];

                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] = TransformMatrix.MultiplyPoint3x4(ShapeVerticesBasis[i]);
                }

                return vertices;
            }
        }
        public Vector3[] ShapeEdgeNoraml
        {
            get
            {
                Vector3[] vertices = ShapeVertices;
                Vector3[] normal = new Vector3[vertices.Length];

                for (int i = 0; i < normal.Length - 1; i++)
                {
                    normal[i] = Vector2.Perpendicular(vertices[i] - vertices[i + 1]);
                }
                normal[normal.Length - 1] = Vector2.Perpendicular(vertices[normal.Length - 1] - vertices[0]);

                return normal;
            }
        }

        public Matrix4x4 TransformMatrix { get; private set; }
        public Matrix4x4 PositioningMatrix { get; private set; }
        public Matrix4x4 RotatingMatrix { get; private set; }
        public Matrix4x4 ScalingMatrix { get; private set; }

        public QuadTreeRect ShapeToRect => throw new System.NotImplementedException();

        public virtual void MinMaxVertexProjection(Vector3 projectionTo, out float min, out float max)
        {
            MinMaxVertexProjection(projectionTo, out Vector3 minVert, out min, out Vector3 maxVert, out max);
        }
        public virtual void MinMaxVertexProjection(Vector3 projectionTo, out Vector3 min, out Vector3 max)
        {
            MinMaxVertexProjection(projectionTo, out min, out float minValue, out max, out float maxValue);
        }
        public virtual void MinMaxVertexProjection(Vector3 projectionTo, out Vector3 minVert, out float minValue, out Vector3 maxVert, out float maxValue)
        {
            projectionTo = projectionTo.normalized;

            minVert = ShapeVertices[0];
            maxVert = ShapeVertices[0];

            minValue = (Vector3.Dot(ShapeVertices[0], projectionTo));
            maxValue = (Vector3.Dot(ShapeVertices[0], projectionTo));

            foreach (var vertex in ShapeVertices)
            {
                float vertexDot = (Vector3.Dot(vertex, projectionTo));
                float minDot = (Vector3.Dot(minVert, projectionTo));
                float maxDot = (Vector3.Dot(maxVert, projectionTo));

                if (vertexDot < minDot)
                {
                    minVert = vertex;
                    minValue = vertexDot;
                }
                if (vertexDot > maxDot)
                {
                    maxVert = vertex;
                    maxValue = vertexDot;
                }
            }
        }
        public float ShapePositionProjection(Vector3 projectionTo)
        {
            projectionTo = projectionTo.normalized;
            return Vector3.Dot(PositioningMatrix.MultiplyPoint3x4(Vector3.zero), projectionTo);
        }

        public VirtucalShape(Vector3 shapePosition, float shapeRotation, Vector3 shapeLocalScale)
        {
            Matrix4x4 PositioningMatrix = new Matrix4x4();
            PositioningMatrix.SetRow(0, new Vector4(1, 0, 0, shapePosition.x));
            PositioningMatrix.SetRow(1, new Vector4(0, 1, 0, shapePosition.y));
            PositioningMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
            PositioningMatrix.SetRow(3, new Vector4(0, 0, 0, 1));
            this.PositioningMatrix = PositioningMatrix;

            float radian = shapeRotation * Mathf.Deg2Rad;
            Matrix4x4 RotatingMatrix = new Matrix4x4();
            RotatingMatrix.SetRow(0, new Vector4(Mathf.Cos(radian), -Mathf.Sin(radian), 0, 0));
            RotatingMatrix.SetRow(1, new Vector4(Mathf.Sin(radian),  Mathf.Cos(radian), 0, 0));
            RotatingMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
            RotatingMatrix.SetRow(3, new Vector4(0, 0, 0, 1));
            this.RotatingMatrix = RotatingMatrix;

            Matrix4x4 ScalingMatrix = new Matrix4x4();
            ScalingMatrix.SetRow(0, new Vector4(shapeLocalScale.x, 0, 0, 0));
            ScalingMatrix.SetRow(1, new Vector4(0, shapeLocalScale.y, 0, 0));
            ScalingMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
            ScalingMatrix.SetRow(3, new Vector4(0, 0, 0, 1));
            this.ScalingMatrix = ScalingMatrix;

            TransformMatrix = PositioningMatrix * RotatingMatrix * ScalingMatrix;
        }

        public virtual void DebugDrawShapeBasis(Color color, float duration)
        {
            for (int i = 0; i < ShapeVerticesBasis.Length - 1; i++)
            {
                Debug.DrawLine(ShapeVerticesBasis[i], ShapeVerticesBasis[i + 1], color, duration);
            }
            Debug.DrawLine(ShapeVerticesBasis[ShapeVerticesBasis.Length - 1], ShapeVerticesBasis[0], color, duration);
        }
        public virtual void DebugDrawShapeTransformation(Color color, float duration)
        {
            for (int i = 0; i < ShapeVertices.Length - 1; i++)
            {
                Debug.DrawLine(ShapeVertices[i], ShapeVertices[i + 1], color, duration);
            }
            Debug.DrawLine(ShapeVertices[ShapeVertices.Length - 1], ShapeVertices[0], color, duration);
        }

    }

}
