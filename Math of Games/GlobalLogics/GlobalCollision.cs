using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPhysicsExercise
{
    public class GlobalCollision : MonoBehaviour
    {

        public delegate void OnShapesCollsisionHandler(ShapeObject shapeA, ShapeObject shapeB, CollisionData collisionData);
        public static OnShapesCollsisionHandler OnShapesCollisionEvent;

        void Awake()
        {
            OnShapesCollisionEvent += (ShapeObject shapeA, ShapeObject shapeB, CollisionData collisionData) =>
            {
                shapeA.SetColor(Color.yellow);
                shapeB.SetColor(Color.yellow);
            };
        }
        void FixedUpdate()
        {
            GlobalCollisionCheck();
        }

        static void GlobalCollisionCheck()
        {
            for (int i = 0; i < ShapeObject.ShapeObjects.Count; i++)
            {
                ShapeObject checkedShape = ShapeObject.ShapeObjects[i];
                Color shapeColor = checkedShape.RigidbodyType == RigidbodyType.Dynamic ? Color.green : checkedShape.RigidbodyType == RigidbodyType.Kinmatic ? Color.blue : Color.red;
                checkedShape.SetColor(shapeColor);

                ShapeObject[] checkedTargets = ShapeQuadTree.OverlapShape(checkedShape);

                for (int ti = 0; ti < checkedTargets.Length; ti++)
                {
                    ShapeObject target = checkedTargets[ti];

                    CollisionData collision = ShapesCollision.IsShapesCollided(checkedShape, target);
                    if (collision.HasCollision)
                    {
                        OnShapesCollisionEvent?.Invoke(checkedShape, target, collision);
                    }
                }

            }
        }
    }

}
