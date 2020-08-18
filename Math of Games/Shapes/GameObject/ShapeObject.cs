using System.Collections.Generic;
using UnityEngine;

namespace MyPhysicsExercise
{
    [ExecuteInEditMode]
    public abstract class ShapeObject : MonoBehaviour, IShapeVertices, IShapeRigidbody
    {

        public static List<ShapeObject> ShapeObjects { get; set; } = new List<ShapeObject>();

        [SerializeField] protected Color shapeColor = Color.white;

        public abstract Vector3[] ShapeVerticesBasis { get; set; }
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

        //Component
        Transform shapeTransform;
        public Transform ShapeTransform
        {
            get
            {
                if (shapeTransform == null) Reset();
                return shapeTransform;
            }
        }
        public Vector3 Position { get => ShapeTransform.position; set => shapeTransform.position = value; }
        public Vector3 Rotation { get => ShapeTransform.eulerAngles; set => shapeTransform.eulerAngles = value; }
        public Vector3 LocalScale { get => ShapeTransform.localScale; set => shapeTransform.localScale = value; }

        //Transform
        public Matrix4x4 TransformMatrix { get; private set; }
        public Matrix4x4 PositioningMatrix { get; private set; }
        public Matrix4x4 RotatingMatrix { get; private set; }
        public Matrix4x4 ScalingMatrix { get; private set; }
        Matrix4x4 PositioningMatrixCalculate
        {
            get
            {
                Matrix4x4 matrix = new Matrix4x4();
                matrix.SetRow(0, new Vector4(1, 0, 0, Position.x));
                matrix.SetRow(1, new Vector4(0, 1, 0, Position.y));
                matrix.SetRow(2, new Vector4(0, 0, 1, 0));
                matrix.SetRow(3, new Vector4(0, 0, 0, 1));

                return matrix;
            }
        }
        Matrix4x4 RotatingMatrixCalculate
        {
            get
            {
                float radian = Rotation.z * Mathf.Deg2Rad;

                Matrix4x4 matrix = new Matrix4x4();
                matrix.SetRow(0, new Vector4(Mathf.Cos(radian), -Mathf.Sin(radian), 0, 0));
                matrix.SetRow(1, new Vector4(Mathf.Sin(radian), Mathf.Cos(radian), 0, 0));
                matrix.SetRow(2, new Vector4(0, 0, 1, 0));
                matrix.SetRow(3, new Vector4(0, 0, 0, 1));

                return matrix;
            }
        }
        Matrix4x4 ScalingMatrixCalculate
        {
            get
            {
                Matrix4x4 matrix = new Matrix4x4();
                matrix.SetRow(0, new Vector4(LocalScale.x, 0, 0, 0));
                matrix.SetRow(1, new Vector4(0, LocalScale.y, 0, 0));
                matrix.SetRow(2, new Vector4(0, 0, 1, 0));
                matrix.SetRow(3, new Vector4(0, 0, 0, 1));

                return matrix;
            }
        }

        public virtual QuadTreeRect ShapeToRect 
        {
            get
            {
                float x = ShapeVertices[0].x;
                float y = ShapeVertices[0].y;
                float maxX = x;
                float maxY = y;

                for (int i = 0; i < ShapeVertices.Length; i++)
                {
                    Vector3 vertices = ShapeVertices[i];

                    x = Mathf.Min(x, vertices.x);
                    y = Mathf.Min(y, vertices.y);
                    maxX = Mathf.Max(maxX, vertices.x);
                    maxY = Mathf.Max(maxY, vertices.y);
                }
                float width = maxX - x;
                float height = maxY - y;

                return new QuadTreeRect(x, y, width, height);
            }
        }


        //Physicstic
        [SerializeField] RigidbodyType bodyType = RigidbodyType.Dynamic;
        [SerializeField] float shapeMass = 1;
        [SerializeField] float linearDrag = 1;
        public RigidbodyType RigidbodyType { get => bodyType; set => bodyType = value; }
        public float BodyMass { get => shapeMass; set => shapeMass = value; }
        public float LinearDrag { get => linearDrag; set => linearDrag = value; }
        public Vector3 BodyPosition { get => Position; }
        public Vector3 BodyRotation { get => Rotation; }
        public Vector3 Velocity { get; set; }
        public Vector3 CurrentForce { get => Velocity * BodyMass;  }
        Vector3 addForce;

        void Reset()
        {
            shapeTransform = transform;
        }
        void OnEnable()
        {
            ShapeObjects.Add(this);
            UpdateTransformMatrix();
        }
        void Update()
        {
            UpdateTransformMatrix();
        }
        void OnDisable()
        {
            ShapeObjects.Remove(this);
        }

        //Transform
        void UpdateTransformMatrix()
        {
            if (!ShapeTransform.hasChanged) return;
            LockTransform();

            PositioningMatrix = PositioningMatrixCalculate;
            RotatingMatrix = RotatingMatrixCalculate;
            ScalingMatrix = ScalingMatrixCalculate;

            TransformMatrix = PositioningMatrix * RotatingMatrix * ScalingMatrix;
        }
        protected virtual void LockTransform()
        {
            ShapeTransform.position = new Vector3(Position.x, Position.y, 0);
            ShapeTransform.rotation = Quaternion.Euler(0, 0, Rotation.z);
            ShapeTransform.localScale = new Vector3(LocalScale.x, LocalScale.y, 0);
        }

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

            minValue = Vector3.Dot(minVert, projectionTo);
            maxValue = Vector3.Dot(maxVert, projectionTo);

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
            return Vector3.Dot(Position, projectionTo);
        }

        protected Vector3 DegreeOfPoint(float degree)
        {
            float radian = degree * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(radian), Mathf.Sin(radian)) * 0.5f;
        }

        //Physicstic
        public void RigidBodyMotion(float timeStep)
        {
            Velocity = Velocity * (1 - timeStep * LinearDrag);

            Position += Velocity * timeStep;
        }
        public void ApplyForces()
        {
            Velocity += ForceToVelocity(addForce);

            addForce = Vector3.zero;
        }
        public void AddGravity(Vector3 gravity)
        {

        }
        public void AddForce(Vector3 force)
        {
            addForce += force;
        }
        Vector3 ForceToVelocity(Vector3 force)
        {
            return force / BodyMass;
        }

        //Visual
        public void SetColor(Color setColor)
        {
            shapeColor = setColor;
        }
        protected virtual void GizmoDrawShapeBasis()
        {
            Gizmos.color = shapeColor;

            for (int i = 0; i < ShapeVerticesBasis.Length - 1; i++)
            {
                Gizmos.DrawLine(ShapeVerticesBasis[i], ShapeVerticesBasis[i + 1]);
            }
            Gizmos.DrawLine(ShapeVerticesBasis[ShapeVerticesBasis.Length - 1], ShapeVerticesBasis[0]);
        }
        protected virtual void GizmoDrawShapeTransformation()
        {
            Gizmos.color = shapeColor;

            for (int i = 0; i < ShapeVertices.Length - 1; i++)
            {
                Gizmos.DrawLine(ShapeVertices[i], ShapeVertices[i + 1]);
            }
            Gizmos.DrawLine(ShapeVertices[ShapeVertices.Length - 1], ShapeVertices[0]);
        }


    }
}