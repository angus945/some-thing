using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;

public class LightPoint : MonoBehaviour
{

    [System.Serializable]
    class LightRayInfo
    {
        LightPoint lightPoint;

        public Vector3 rayDirction;
        public Vector3 vertiex;
        public Vector3 vertiexOfCollinear;
        public Vector3 RayEndPoint { get => rayDirction + lightPoint.LightPosition; }

        public bool useVertiex = false;
        public bool isFailure = false;
        public bool isBlocking = false;

        public float angle;
        public float Angle
        {
            get
            {
                float angle = Mathf.Atan2(rayDirction.y, rayDirction.x);

                this.angle = angle;
                return (angle > 0) ? angle : 360 - Mathf.Abs(angle);
            }
        }

        public LightRayInfo(Vector3 rayDirction, Vector3 vertiex, LightPoint lightPoint)
        {
            this.rayDirction = rayDirction;
            this.vertiex = vertiex;
            this.lightPoint = lightPoint;
        }
    }

    ShapeOfSquare[] Squares { get => FindObjectsOfType<ShapeOfSquare>(); }

    [SerializeField] float lightRadius = 5f;
    public Vector3 LightPosition { get => transform.position; }

    [SerializeField][Range(10,100)] int circleEdge = 50;

    [SerializeField] MeshFilter meshFinter;

    [Space]
    [SerializeField] bool showResult = true;
    [SerializeField] bool showMesh = true;

    [Header("Debug")]
    [SerializeField] bool showDebugA = true;
    [SerializeField] bool showDebugB = true;
    [SerializeField] bool showDebugC = true;

    List<LightRayInfo> lightCheckRay = new List<LightRayInfo>();
    List<Vector3> meshVertex = new List<Vector3>();
    List<int> meshTri = new List<int>();

    void OnDrawGizmos()
    {

        DrawCircle();
        lightCheckRay.Clear();
        meshVertex.Clear();
        meshTri.Clear();

        foreach (var square in Squares)
        {
            if (!AllVertexInRange(square.Vertex)) continue;

            for (int i = 0; i < square.Vertex.Length; i++)
            {
                Vector3 vertex = square.Vertex[i];
                Vector3 dirction = (vertex - LightPosition).normalized * lightRadius;

                Vector3 firstInteresection = LightPosition + dirction;
                bool haveInterest = false;

                foreach (var edge in square.Edge)
                {
                    if (edge.pointA == vertex || edge.pointB == vertex) continue;
                    if (!FindInsterestPoint(LightPosition, dirction, edge.pointA, edge.pointB, out Vector3 interesection)) continue;

                    if ((interesection - LightPosition).sqrMagnitude > (firstInteresection - LightPosition).sqrMagnitude) continue;

                    firstInteresection = interesection;
                    haveInterest = true;

                }

                bool useVertiex = false;
                if (!haveInterest) lightCheckRay.Add(new LightRayInfo(dirction, vertex,this));
                else if ((LightPosition - vertex).sqrMagnitude < (LightPosition - firstInteresection).sqrMagnitude)
                {
                    useVertiex = true;
                    lightCheckRay.Add(new LightRayInfo(vertex - LightPosition, vertex,this) { useVertiex = true });
                }
                else
                {
                    lightCheckRay.Add(new LightRayInfo(vertex - LightPosition, vertex, this) { isFailure = true });
                }

                if (showDebugA)
                { 
                    if (!haveInterest)
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawLine(LightPosition, firstInteresection);
                        Gizmos.DrawWireSphere(firstInteresection, 0.1f);
                    }
                    else if (useVertiex)
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawRay(LightPosition, vertex- LightPosition );
                    }
                    else
                    {
                        Gizmos.color = Color.gray;
                        Gizmos.DrawRay(LightPosition, dirction);
                        Gizmos.DrawWireSphere(firstInteresection, 0.1f);
                    }

                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(vertex, 0.1f);
                }
            }
        }

        foreach (var checkRay in lightCheckRay)
        {
            foreach (var square in Squares)
            {
                if (!AllVertexInRange(square.Vertex)) continue;

                Vector3 firstInteresection = LightPosition + checkRay.rayDirction;

                bool isRayBlocking = false;
                foreach (var edge in square.Edge)
                {
                    if (edge.pointA == checkRay.vertiex || edge.pointB == checkRay.vertiex) continue;
                    if (!FindInsterestPoint(LightPosition, checkRay.rayDirction, edge.pointA, edge.pointB, out Vector3 interesection)) continue;

                    if ((interesection - LightPosition).sqrMagnitude > (firstInteresection - LightPosition).sqrMagnitude) continue;

                    firstInteresection = interesection;
                    isRayBlocking = true;
                }

                if (isRayBlocking)
                {
                    checkRay.isFailure = (firstInteresection - LightPosition).sqrMagnitude < (checkRay.vertiex - LightPosition).sqrMagnitude;
                    checkRay.rayDirction = firstInteresection-  LightPosition;
                    checkRay.isBlocking = true;
                }

                if (showDebugB && isRayBlocking)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(firstInteresection, 0.1f);
                }
            }

            if (showDebugB)
            {
                Gizmos.color = checkRay.isFailure ? Color.red : Color.gray;
                Gizmos.DrawRay(LightPosition, checkRay.rayDirction);
            }
        }

        lightCheckRay.RemoveAll(n => n.isFailure);
        for (int i = 0; i < lightCheckRay.Count; i++)
        {
            if (showDebugC)
            {
                if (lightCheckRay[i].isFailure) Gizmos.color = Color.gray;
                else Gizmos.color = Color.yellow;

                Gizmos.DrawRay(LightPosition, lightCheckRay[i].rayDirction);
            }
        }

        bool resort = false;
        for (int i = 0; i < 1000; i++)
        {
            for (int j = 0; j < lightCheckRay.Count - 1; j++)
            {
                if (lightCheckRay[j].Angle > lightCheckRay[j + 1].Angle)
                {
                    (lightCheckRay[j], lightCheckRay[j + 1]) = (lightCheckRay[j + 1], lightCheckRay[j]);
                    resort = true;
                }                
            }

            if (!resort) break;
        }

        bool haveFailure = false;
        int tri = 0;
        for (int i = 0; i < lightCheckRay.Count - 1; i++)
        {
            if (!showResult) return;
            if (lightCheckRay[i].isFailure) continue;

            LightRayInfo rayinfoA = lightCheckRay[i];
            LightRayInfo rayinfoB = null;
            for (int j = 1; j < lightCheckRay.Count; j++)
            {
                if (i + j >= lightCheckRay.Count) break;
                if (lightCheckRay[i + j].isFailure)
                {
                    haveFailure = true;
                    continue;
                }
                else
                {
                    rayinfoB = lightCheckRay[i + j];
                    break;
                }
            }
            if (rayinfoB == null) break;

            Gizmos.color = Color.yellow;

            meshVertex.Add(LightPosition);



            if(haveFailure)
            {
                if (!rayinfoA.isBlocking )
                {
                    Gizmos.DrawLine(rayinfoA.vertiex, rayinfoB.vertiex);
                }
                else if (rayinfoA.isBlocking && rayinfoB.useVertiex)
                {
                    Gizmos.DrawLine(rayinfoA.RayEndPoint, rayinfoB.vertiex);
                }
                else if(rayinfoB.isBlocking &&  rayinfoA.useVertiex)
                {
                    Gizmos.DrawLine(rayinfoA.vertiex, rayinfoB.RayEndPoint);
                }
                else if (rayinfoA.useVertiex || rayinfoB.useVertiex)
                {
                    Gizmos.DrawLine(rayinfoA.vertiex, rayinfoB.vertiex);
                }
                else
                {
                    Gizmos.DrawLine(rayinfoA.vertiex, rayinfoB.vertiex);
                }
            }
            else
            {
                if (rayinfoA.useVertiex || rayinfoB.useVertiex)
                {
                    if (rayinfoA.isBlocking)
                    {
                        Gizmos.DrawLine(rayinfoA.RayEndPoint, rayinfoB.vertiex);
                    }
                    else if (rayinfoB.isBlocking)
                    {
                        Gizmos.DrawLine(rayinfoA.vertiex, rayinfoB.RayEndPoint);
                    }
                    else
                    {
                        Gizmos.DrawLine(rayinfoA.vertiex, rayinfoB.vertiex);
                    }                    
                }
                else if (rayinfoA.isBlocking)
                {
                    Gizmos.DrawLine(rayinfoA.RayEndPoint, rayinfoB.vertiex);
                }
                else if (rayinfoB.isBlocking)
                {
                    Gizmos.DrawLine(rayinfoA.vertiex, rayinfoB.RayEndPoint);
                }
                else
                {
                    Gizmos.DrawLine(rayinfoA.RayEndPoint, rayinfoB.RayEndPoint);
                }
            }


    

            meshTri.Add(tri + 0);
            meshTri.Add(tri + 1);
            meshTri.Add(tri + 2);
            tri += 3;

            haveFailure = false;
        }

        if (showMesh)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = meshVertex.ToArray();
            mesh.triangles = meshTri.ToArray();
            meshFinter.mesh = mesh;
        }
        else
        {
            meshFinter.mesh.Clear();
        }


    }
    void DrawCircle()
    {

        float radian = 360 * ((float)circleEdge) * Mathf.Deg2Rad;
        float radianNext = 360 * ((float)(circleEdge - 1) / (float)circleEdge) * Mathf.Deg2Rad;

        Vector3 draw = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian)) * lightRadius + transform.position;
        Vector3 draw2 = new Vector3(Mathf.Cos(radianNext), Mathf.Sin(radianNext)) * lightRadius + transform.position;
        Gizmos.DrawLine(draw, draw2);

        for (int i = 0; i < circleEdge - 1; i++)
        {
            radian = 360 * ((float)i / (float)circleEdge) * Mathf.Deg2Rad;
            radianNext = 360 * ((float)(i + 1) / (float)circleEdge) * Mathf.Deg2Rad;

            draw = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian)) * lightRadius + transform.position;
            draw2 = new Vector3(Mathf.Cos(radianNext), Mathf.Sin(radianNext)) * lightRadius + transform.position;

            Gizmos.DrawLine(draw, draw2);
        }


    }

    bool AllVertexInRange(Vector3[] vertex)
    {
        foreach (var point in vertex)
        {
            if ((point - LightPosition).sqrMagnitude < lightRadius * lightRadius) return true;
        }
        return false;
    }

    bool PointInRange(Vector3 point,Vector3 rangeA,Vector3 rangeB)
    {
        return
            point.x >= Mathf.Min(rangeA.x, rangeB.x) - 0.001f && point.x <= Mathf.Max(rangeA.x, rangeB.x) + 0.001f &&
            point.y >= Mathf.Min(rangeA.y, rangeB.y) - 0.001f && point.y <= Mathf.Max(rangeA.y, rangeB.y) + 0.001f;
    }
    bool FindInsterestPoint(Vector3 basisPoint, Vector3 rayDir, Vector3 linePointA, Vector3 LinePointB,out Vector3 intersection)
    {
        Vector3 line = LinePointB - linePointA;
        Vector3 lineNormal = new Vector3(-line.y, line.x).normalized;

        intersection = Vector3.zero;

        //float dotOfRayAndLineNormal = rayDir.x * lineNormal.x + rayDir.y * lineNormal.y;
        float dotOfRayAndLineNormal = Vector3.Dot(rayDir, lineNormal);

        if (dotOfRayAndLineNormal != 0)
        {
            float dotOfVerticalDistance = Vector3.Dot(linePointA - basisPoint, lineNormal);
            float distanceScale = dotOfVerticalDistance / dotOfRayAndLineNormal;

            intersection = (rayDir * distanceScale) + basisPoint;
        }

        return PointInRange(intersection, linePointA, LinePointB);
    }

}
