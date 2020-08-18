using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyPhysicsExercise
{
    public struct QuadTreeRect
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }

        public float XWidth { get => X + Width; }
        public float YHeight { get => Y + Height; }
        public Vector3 Center { get => new Vector3(X + Width / 2, Y + Height / 2); }

        public QuadTreeRect(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }
        public QuadTreeRect(Vector3 center, Vector3 size)
        {
            X = center.x - (size.x / 2);
            Y = center.y - (size.y / 2);

            Width = size.x;
            Height = size.y;
        }
        //public QuadTreeRect(ShapeObject objectShape)
        //{
        //    X = objectShape.ShapeVertices[0].x;
        //    Y = objectShape.ShapeVertices[0].y;
        //    float maxX = X;
        //    float maxY = Y;

        //    for (int i = 0; i < objectShape.ShapeVertices.Length; i++)
        //    {
        //        Vector3 vertices = objectShape.ShapeVertices[i];

        //        X = Mathf.Min(X, vertices.x);
        //        Y = Mathf.Min(Y, vertices.y);
        //        maxX = Mathf.Max(maxX, vertices.x);
        //        maxY = Mathf.Max(maxY, vertices.y);
        //    }
        //    Width = maxX - X;
        //    Height = maxY - Y;
        //}

        public static bool PointOverlap(QuadTreeRect rect, Vector3 point)
        {
            bool xOverlap = (point.x > rect.X) && (point.x < rect.XWidth);
            bool yOverlap = (point.y > rect.Y) && (point.y < rect.YHeight);

            return xOverlap && yOverlap;
        }
        public static bool RectOverlap(QuadTreeRect rectA, QuadTreeRect rectB)
        {
            if (rectA.X > rectB.XWidth || rectB.X > rectA.XWidth) return false;
            if (rectA.Y > rectB.YHeight || rectB.Y > rectA.YHeight) return false;

            return true;
        }
        public static bool LineOverlap(QuadTreeRect rect, Vector3 lineOrigin, Vector3 lineDir)
        {
            lineDir = lineDir.normalized;
            if (PointOverlap(rect, lineOrigin)) return true;

            if (Vector3.Dot((rect.Center - lineOrigin).normalized, lineDir) < -0.01f) return false;

            Vector3 projectionTo = Vector2.Perpendicular(lineDir);
            float originProj = Vector3.Dot(lineOrigin, projectionTo.normalized);
            RectProjection(rect, projectionTo, out float min, out float max);

            if (originProj > min && originProj < max) return true;

            return false;
        }
        static void RectProjection(QuadTreeRect rect, Vector3 projectionTo, out float min, out float max)
        {
            projectionTo = projectionTo.normalized;

            float LDDot = Vector3.Dot(new Vector3(rect.X, rect.Y), projectionTo);
            float RDDot = Vector3.Dot(new Vector3(rect.XWidth, rect.Y), projectionTo);

            float LTDot = Vector3.Dot(new Vector3(rect.X, rect.YHeight), projectionTo);
            float RTDot = Vector3.Dot(new Vector3(rect.XWidth, rect.YHeight), projectionTo);

            min = Mathf.Min(new float[] { LDDot, RDDot, LTDot, RTDot });
            max = Mathf.Max(new float[] { LDDot, RDDot, LTDot, RTDot });
        }

        public static void DebugDrawRect(QuadTreeRect rect, Color color, float duration, float patting)
        {
            Vector3 LD = new Vector3(rect.X + patting, rect.Y + patting);
            Vector3 RD = new Vector3(rect.XWidth - patting, rect.Y + patting);
            Vector3 RT = new Vector3(rect.XWidth - patting, rect.YHeight - patting);
            Vector3 LT = new Vector3(rect.X + patting, rect.YHeight - patting);

            Debug.DrawLine(LD, RD, color, duration);
            Debug.DrawLine(RD, RT, color, duration);
            Debug.DrawLine(RT, LT, color, duration);
            Debug.DrawLine(LT, LD, color, duration);
        }

        public override string ToString()
        {
            return "X: " + X + ", Y: " + Y + ", Width: " + Width + ", Hieght: " + Height;
        }
    }
    public class ShapeQuadTree : MonoBehaviour
    {
        public static ShapeQuadTree instance;

        [System.Serializable]
        class QuadTree
        {
            int maxDepth;
            public int TreeDepth { get; set; }
            public QuadTreeRect TreeRect { get; set; }

            int maxAmount;
            public List<ShapeObject> chiledShapes { get; set; }

            public QuadTree[] chiledTrees { get; set; }

            public QuadTree(int treeDepth, QuadTreeRect treeRect)
            {
                maxDepth = 5;
                maxAmount = 4;

                chiledShapes = new List<ShapeObject>();

                TreeDepth = treeDepth;
                TreeRect = treeRect;
            }

            //Add
            public void AddShape(ShapeObject checkShape)
            {
                if (!IsShapeOverlapTreeRect(checkShape)) return;

                if (chiledShapes.Count < maxAmount || TreeDepth >= maxDepth)
                {
                    chiledShapes.Add(checkShape);
                    Color color = (TreeDepth == 0) ? Color.blue : (TreeDepth == 1) ? Color.red : (TreeDepth == 2) ? Color.green : (TreeDepth == 3) ? Color.yellow : Color.black;
                    if (ShapeQuadTree.DrawTreeDebug) checkShape.SetColor(color);
                }
                else
                {
                    if (TreeDepth >= maxDepth) return;
                    if (chiledTrees == null) CreateChiledTree();

                    for (int i = 0; i < chiledTrees.Length; i++)
                    {
                        chiledTrees[i].AddShape(checkShape);
                    }
                }
            }
            void CreateChiledTree()
            {
                chiledTrees = new QuadTree[4];

                int chiledDepth = TreeDepth + 1;
                float chiledSize = TreeRect.Width / 2;

                QuadTreeRect chiledRect = new QuadTreeRect(TreeRect.X, TreeRect.Y, chiledSize, chiledSize);
                chiledTrees[0] = new QuadTree(chiledDepth, chiledRect);

                chiledRect = new QuadTreeRect(TreeRect.X + chiledSize, TreeRect.Y, chiledSize, chiledSize);
                chiledTrees[1] = new QuadTree(chiledDepth, chiledRect);

                chiledRect = new QuadTreeRect(TreeRect.X, TreeRect.Y + chiledSize, chiledSize, chiledSize);
                chiledTrees[2] = new QuadTree(chiledDepth, chiledRect);

                chiledRect = new QuadTreeRect(TreeRect.X + chiledSize, TreeRect.Y + chiledSize, chiledSize, chiledSize);
                chiledTrees[3] = new QuadTree(chiledDepth, chiledRect);
            }

            //Take
            public List<ShapeObject> TakeShapesInRect(QuadTreeRect rect)
            {
                List<ShapeObject> takes = new List<ShapeObject>();

                if (!QuadTreeRect.RectOverlap(TreeRect, rect)) return takes;

                takes.AddRange(chiledShapes);

                if (chiledTrees == null) return takes;

                for (int i = 0; i < chiledTrees.Length; i++)
                {
                    takes.AddRange(chiledTrees[i].TakeShapesInRect(rect));
                }

                return takes;
            }
            public List<ShapeObject> TakeShapesOfLine(Vector3 origin, Vector3 dirction)
            {
                List<ShapeObject> takes = new List<ShapeObject>();

                if (!QuadTreeRect.LineOverlap(TreeRect, origin, dirction)) return takes;

                takes.AddRange(chiledShapes);

                if (chiledTrees == null) return takes;

                for (int i = 0; i < chiledTrees.Length; i++)
                {
                    takes.AddRange(chiledTrees[i].TakeShapesOfLine(origin, dirction));
                }

                return takes;

            }

            public bool IsShapeOverlapTreeRect(ShapeObject shape)
            {
                return QuadTreeRect.RectOverlap(TreeRect, shape.ShapeToRect);
            }
        }

        [SerializeField] Vector3 treeRegion = Vector3.one * 10;
        public static Vector3 TreeRegion { get => instance.treeRegion; set => instance.treeRegion = value; }

        [Header("Debug")]
        [SerializeField] bool drawTreeDebug = true;
        public static bool DrawTreeDebug { get => instance.drawTreeDebug; set => instance.drawTreeDebug = value; }

        QuadTree FirstTree;

        void Awake()
        {
            instance = this;
        }
        void FixedUpdate()
        {
            FirstTree = new QuadTree(0, new QuadTreeRect(0, 0, treeRegion.x, treeRegion.y));

            for (int i = 0; i < ShapeObject.ShapeObjects.Count; i++)
            {
                ShapeObject checkShape = ShapeObject.ShapeObjects[i];

                FirstTree.AddShape(checkShape);
            }
        }
        void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(treeRegion / 2, treeRegion);

            if (drawTreeDebug) DrawTreeSelited(FirstTree);
        }

        //Overlap
        public static ShapeObject[] OverlapShape(ShapeObject shape)
        {
            QuadTreeRect rect = shape.ShapeToRect;

            return instance.FirstTree.TakeShapesInRect(rect).Distinct().ToArray();
        }
        public static ShapeObject[] OverlapRect(Vector3 center, Vector3 size)
        {
            QuadTreeRect rect = new QuadTreeRect(center, size);

            return instance.FirstTree.TakeShapesInRect(rect).Distinct().ToArray();
        }
        public static ShapeObject[] OverlapLine(Vector3 origin, Vector3 dirction)
        {
            return instance.FirstTree.TakeShapesOfLine(origin, dirction).Distinct().ToArray();
        }
        //TODO Pptimization - Hash Table


        //Gizmo
        void DrawTreeSelited(QuadTree draw)
        {
            if (draw == null || draw.chiledTrees == null) return;

            Gizmos.color = (draw.TreeDepth == 0) ? Color.red : (draw.TreeDepth == 1) ? Color.green : (draw.TreeDepth == 2) ? Color.yellow : Color.black;

            Vector3 start = new Vector3(draw.TreeRect.X + draw.TreeRect.Width / 2, draw.TreeRect.Y);
            Vector3 end = new Vector3(draw.TreeRect.X + draw.TreeRect.Width / 2, draw.TreeRect.Y + draw.TreeRect.Height);
            Gizmos.DrawLine(start, end);

            start = new Vector3(draw.TreeRect.X, draw.TreeRect.Y + draw.TreeRect.Width / 2);
            end = new Vector3(draw.TreeRect.X + draw.TreeRect.Height, draw.TreeRect.Y + draw.TreeRect.Width / 2);
            Gizmos.DrawLine(start, end);

            for (int i = 0; i < draw.chiledTrees.Length; i++)
            {
                DrawTreeSelited(draw.chiledTrees[i]);
            }
        }


    }
}

