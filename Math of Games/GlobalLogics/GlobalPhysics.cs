using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPhysicsExercise
{
    public class GlobalPhysics : MonoBehaviour
    {
        [SerializeField] bool drawVelocity = true;
        [SerializeField] bool drawForce = true;
        [SerializeField] bool drawElasticCollision = true;

        struct ElasticCollisionData
        {
            public readonly ShapeObject onCollision;
            public readonly ShapeObject target;

            public readonly CollisionData collisionData;

            public ElasticCollisionData(ShapeObject collision, ShapeObject target, CollisionData collisionData)
            {
                this.onCollision = collision;
                this.target = target;
                this.collisionData = collisionData;
            }
        }
        List<ElasticCollisionData> OnCollisionsShapes = new List<ElasticCollisionData>();

        float TimeStep { get => Time.fixedDeltaTime; }

        float minImmeresion = 0.1f;
        float paushLerp = 20;

        void Start()
        {
            GlobalCollision.OnShapesCollisionEvent += AddElasticCollision;
            GlobalCollision.OnShapesCollisionEvent += ShapesPush;
        }
        void FixedUpdate()
        {
            for (int i = 0; i < OnCollisionsShapes.Count; i++)
            {
                CalculateElasticCollision(OnCollisionsShapes[i]);
            }
            OnCollisionsShapes.Clear();

            for (int i = 0; i < ShapeObject.ShapeObjects.Count; i++)
            {
                ShapeObject shape = ShapeObject.ShapeObjects[i];

                ShapePositionLimit(shape);
                ShapeMotion(shape);
            }
        }

        //Compute Physics
        void ShapePositionLimit(ShapeObject limitShape)
        {
            Vector3 treeRegion = ShapeQuadTree.TreeRegion;

            Vector3 clamp = new Vector3(Mathf.Clamp(limitShape.Position.x, 0.1f, treeRegion.x - 0.1f), Mathf.Clamp(limitShape.Position.y, 0.1f, treeRegion.y - 0.1f));
            limitShape.Position = Vector3.Lerp(limitShape.Position, clamp, 20 * Time.fixedDeltaTime);
        }
        void ShapeMotion(IShapeRigidbody shape)
        {
            shape.ApplyForces();
            shape.RigidBodyMotion(TimeStep);

            if (drawVelocity) Debug.DrawRay(shape.BodyPosition, shape.Velocity, Color.red);
            if (drawForce) Debug.DrawRay(shape.BodyPosition, shape.Velocity, Color.red);
        }
        void CalculateElasticCollision(ElasticCollisionData calculateData)
        {
            IShapeRigidbody onCollision = calculateData.onCollision;
            IShapeRigidbody target = calculateData.target;

            if (onCollision.RigidbodyType != RigidbodyType.Dynamic) return;
            if (onCollision.Velocity == Vector3.zero) return;

            if (target.RigidbodyType == RigidbodyType.Dynamic)
            {
                Vector3 collisionDirction = (target.BodyPosition - onCollision.BodyPosition).normalized;
                Vector3 collisionNormal = Vector2.Perpendicular(collisionDirction).normalized;

                Vector3 originForce = onCollision.CurrentForce;
                Vector3 transferForce = collisionDirction * Vector3.Dot(originForce, collisionDirction);

                onCollision.AddForce(-transferForce);
                target.AddForce(transferForce);

                if (drawElasticCollision)
                {
                    float drawDuration = 1.5f;
                    Vector3 shapeAForce = collisionNormal * Vector3.Dot(originForce, collisionNormal);
                    Debug.DrawRay(onCollision.BodyPosition, originForce, Color.red, drawDuration);

                    Debug.DrawRay(onCollision.BodyPosition, collisionDirction, Color.white, drawDuration);
                    Debug.DrawRay(onCollision.BodyPosition + collisionDirction / 2, collisionNormal, Color.white, drawDuration);

                    Debug.DrawRay(onCollision.BodyPosition, collisionNormal, Color.blue, drawDuration);
                    Debug.DrawRay(target.BodyPosition, collisionDirction, Color.blue, drawDuration);

                    Debug.DrawRay(onCollision.BodyPosition, shapeAForce, Color.yellow, drawDuration);
                    Debug.DrawRay(target.BodyPosition, transferForce, Color.yellow, drawDuration);
                }
            }
            else
            {

            }
        }

        //Event Physics
        void AddElasticCollision(ShapeObject collision, ShapeObject target, CollisionData collisionData)
        {
            if (collisionData.Immeresion < minImmeresion) return;

            OnCollisionsShapes.Add(new ElasticCollisionData(collision, target, collisionData));
        }
        void ShapesPush(ShapeObject onCollision, ShapeObject collisionTarget, CollisionData collisionData)
        {
            if (collisionData.Immeresion < minImmeresion) return;

            Vector3 paushdir = collisionData.collisionNormal;
            float pushValue = collisionData.Immeresion - minImmeresion;

            if (onCollision.RigidbodyType == RigidbodyType.Dynamic)
            {
                Vector3 target = onCollision.Position + paushdir * pushValue;
                onCollision.Position = Vector3.Lerp(onCollision.Position, target, paushLerp * TimeStep);

                Debug.DrawRay(onCollision.Position, collisionData.collisionPoint);
            }
            if (collisionTarget.RigidbodyType == RigidbodyType.Dynamic)
            {
                Vector3 target = collisionTarget.Position - paushdir * pushValue;
                collisionTarget.Position = Vector3.Lerp(collisionTarget.Position, target, paushLerp * TimeStep);
            }
        }



    }

}
