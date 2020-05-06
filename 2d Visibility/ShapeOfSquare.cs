using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeOfSquare : MonoBehaviour
{


    public struct SquareEdge 
    {
       public Vector3 pointA;
       public Vector3 pointB;

        public SquareEdge(Vector3 pointA, Vector3 pointB)
        {
            this.pointA = pointA;
            this.pointB = pointB;
        }
    }


    public Vector3[] Vertex
    {
        get
        {
            return new Vector3[]
            {
                RotateVector(.5f ,.5f ) * Scale + transform.position,
                RotateVector(.5f ,-.5f) * Scale + transform.position,
                RotateVector(-.5f,-.5f) * Scale + transform.position,
                RotateVector(-.5f,.5f ) * Scale + transform.position,
            };
        }
    }
    public SquareEdge[] Edge
    {
        get
        {
            return new SquareEdge[]
            {
                new SquareEdge(Vertex[0], Vertex[1]),
                new SquareEdge(Vertex[1], Vertex[2]),
                new SquareEdge(Vertex[2], Vertex[3]),
                new SquareEdge(Vertex[3], Vertex[0]),
            };
        }
    }
    public Vector2 Position { get => transform.position; }

    float Angle { get => transform.eulerAngles.z; }
    float Scale { get => transform.localScale.z; }
    float RadianRight { get => Angle * Mathf.Deg2Rad; }
    float RadianUp { get => (Angle + 90) * Mathf.Deg2Rad; }

    Vector2 IHat { get => new Vector2(Mathf.Cos(RadianRight), Mathf.Sin(RadianRight)); }
    Vector2 JHat { get => new Vector2(Mathf.Cos(RadianUp), Mathf.Sin(RadianUp)); }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        for (int i = 0; i < Vertex.Length - 1; i++)
        {
            Gizmos.DrawLine(Vertex[i] + Vector3.forward, Vertex[i + 1] + Vector3.forward);
        }
        Gizmos.DrawLine(Vertex[Vertex.Length - 1] + Vector3.forward, Vertex[0] + Vector3.forward);
    }

    Vector3 RotateVector(float x,float y)
    {
        float X = x * IHat.x + y * JHat.x;
        float Y = x * IHat.y + y * JHat.y;

        return new Vector2(X, Y);
    }


}
