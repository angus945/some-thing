using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyPhysicsExercise
{

    public struct CollisionData
    {
        public readonly bool HasCollision;
        public readonly float Immeresion;
        public readonly Vector3 collisionNormal;
        public readonly Vector3 collisionPoint;

        public CollisionData(bool hasCollision, float immeresion,Vector3 normal ,Vector3 point)
        {
            HasCollision = hasCollision;
            Immeresion = immeresion;
            collisionNormal = normal.normalized;
            collisionPoint = point;
        }
        public CollisionData(bool hasCollision, float immeresion, Vector3 normal) : this()
        {
            HasCollision = hasCollision;
            Immeresion = immeresion;

            collisionNormal = normal.normalized;
        }
        public CollisionData(bool hasCollision, float immeresion) : this()
        {
            HasCollision = hasCollision;
            Immeresion = immeresion;
        }
    }
    public static class ShapesCollision
    {

        public static bool DrawOverlap { get; set; } = false;

        //Overlap Check
        public static ShapeObject[] OverlapPoint(Vector3 point)
        {
            return OverlapCheck(point, Vector3.zero, (ShapeObject checkedShape) => IsShapeCollisedWithPoint(checkedShape, point));
        }
        public static ShapeObject[] OverlapRectangle(Vector3 center, float angle, Vector3 size)
        {
            VirtucalShapeBox overlapBox = new VirtucalShapeBox(center, angle, size);
            if (DrawOverlap) overlapBox.DebugDrawShapeTransformation(Color.white, 0f);

            return OverlapCheck(center, size, (ShapeObject checkedShape) => IsShapesCollided(checkedShape, overlapBox));
        }
        static ShapeObject[] OverlapCheck(Vector3 point, Vector3 size, Func<ShapeObject, CollisionData> CollidedCheckHandler)
        {
            ShapeObject[] checking = ShapeQuadTree.OverlapRect(point, size);
            List<ShapeObject> IsOverlapShape = new List<ShapeObject>();

            for (int i = 0; i < checking.Length; i++)
            {
                ShapeObject checkedShape = checking[i];
                if (CollidedCheckHandler.Invoke(checkedShape).HasCollision) IsOverlapShape.Add(checkedShape);
            }

            return IsOverlapShape.ToArray();
        } 

        //Raycast Check
        public static ShapeObject[] RayCast(Vector3 origin, Vector3 dirction)
        {
            dirction = dirction.normalized;

            ShapeObject[] checking = ShapeQuadTree.OverlapLine(origin, dirction);
            List<ShapeObject> IsCastHitShape = new List<ShapeObject>();

            if (DrawOverlap) Debug.DrawRay(origin, dirction * 50);

            for (int i = 0; i < checking.Length; i++)
            {
                ShapeObject checkedShape = checking[i];
                Vector3 rayProjection = Vector2.Perpendicular(dirction);
                if (ShapeDirctionOfRayCast(checkedShape, origin, dirction) && ShapeProjectionOverlapWithPoint(checkedShape, origin, rayProjection).HasCollision)
                    IsCastHitShape.Add(checkedShape);
            }

            return IsCastHitShape.ToArray();
        }

        //Collision
        public static CollisionData IsShapesCollided(IShapeVertices shapeA, IShapeVertices shapeB)
        {
            if (shapeA == null || shapeB == null) return new CollisionData(false, 0);
            if (shapeA == shapeB) return new CollisionData(false, 0);

            Vector3 dirctionProjection = PrjectionShapeDirction(shapeA, shapeB);
            Vector3[] shapeEdgeNormals = shapeA.ShapeEdgeNoraml.Union(shapeB.ShapeEdgeNoraml).ToArray();

            return CollidedCheck(dirctionProjection, shapeEdgeNormals, (Vector3 projectionTo) => ShapesProjectionOverlap(shapeA, shapeB, projectionTo));
        }
        static CollisionData IsShapeCollisedWithPoint(IShapeVertices shape, Vector3 point)
        {
            if (shape == null) return new CollisionData(false, 0); 

            Vector3 dirctionProjection = ShapePosition(shape) - point;
            Vector3[] shapeEdgeNormals = shape.ShapeEdgeNoraml.ToArray();

            return CollidedCheck(dirctionProjection, shapeEdgeNormals, (Vector3 projectionTo) => ShapeProjectionOverlapWithPoint(shape, point, projectionTo));
        }
        static CollisionData CollidedCheck(Vector3 dirctionProjection, Vector3[] projections, Func<Vector3, CollisionData> ShapeProjectionHandler)
        {
            CollisionData dirctionCheck = ShapeProjectionHandler.Invoke(dirctionProjection);
            CollisionData minCollision = dirctionCheck;

            if (!dirctionCheck.HasCollision) return dirctionCheck;
            Vector3 overlapPoint = dirctionCheck.collisionNormal * dirctionCheck.Immeresion;

            foreach (var projectionTo in projections)
            {
                CollisionData collision = ShapeProjectionHandler(projectionTo);
                if (collision.Immeresion < minCollision.Immeresion) minCollision = collision;

                if (!collision.HasCollision) return collision;
                overlapPoint = collision.collisionNormal * collision.Immeresion;
            }

            Vector3 collisionPoint = minCollision.collisionNormal * -(1 - minCollision.Immeresion) + overlapPoint;
            return new CollisionData(true, minCollision.Immeresion, minCollision.collisionNormal, collisionPoint);
        }

        //Projection
        static CollisionData ShapesProjectionOverlap(IShapeVertices shapeA, IShapeVertices shapeB, Vector3 projectionTo)
        {
            shapeA.MinMaxVertexProjection(projectionTo, out float minA, out float maxA);
            shapeB.MinMaxVertexProjection(projectionTo, out float minB, out float maxB);

            bool shapeOrder = shapeA.ShapePositionProjection(projectionTo) > shapeB.ShapePositionProjection(projectionTo);

            if (shapeOrder) return new CollisionData(maxB > minA, maxB - minA, projectionTo);
            else return new CollisionData(maxA > minB, maxA - minB, -projectionTo);
        }
        static CollisionData ShapeProjectionOverlapWithPoint(IShapeVertices shape, Vector3 point, Vector3 projectionTo)
        {
            projectionTo = projectionTo.normalized;
            shape.MinMaxVertexProjection(projectionTo, out float min, out float max);

            float pointProjection = Vector3.Dot(point, projectionTo);

            return new CollisionData(pointProjection > min && pointProjection < max, pointProjection - min, projectionTo);
        }
        static bool ShapeDirctionOfRayCast(IShapeVertices shape, Vector3 rayOrigin, Vector3 rayDirction)
        {
            Vector3 shapeDirction = (ShapePosition(shape) - rayOrigin).normalized;

            return Vector3.Dot(shapeDirction, rayDirction) > 0;
        }

        //Calculate Function
        static Vector3 PrjectionShapeDirction(IShapeVertices shapeA, IShapeVertices shapeB)
        {
            return (ShapePosition(shapeA) - ShapePosition(shapeB)).normalized;
        }
        static Vector3 ShapePosition(IShapeVertices shape)
        {
            return shape.PositioningMatrix.MultiplyPoint3x4(Vector3.zero);
        }


    }


}
