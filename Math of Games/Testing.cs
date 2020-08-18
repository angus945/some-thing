using MyPhysicsExercise;
using System;
using UnityEngine;

public class Testing : MonoBehaviour
{

    enum TestType
    {
        Instantiate,
        Draging,
        Overlap,
        Raycast,
        PushForce,
        PushVelocity,
    }

    [SerializeField] TestType type = TestType.Instantiate;
    [SerializeField] ShapeObject addShape = null;

    [Space]
    [SerializeField] Vector3 checkRegion = Vector3.one * 3;
    [SerializeField] TextMesh text = null;
    float checkAngle = 0;

    Camera mainCamera;
    Vector3 mousePoint;
    Vector3 lineOrigin;

    ShapeObject select;
    RigidbodyType selectType;
    Vector3 mouseOffset;

    void Start()
    {
        mainCamera = Camera.main;
    }
    void Update()
    {
        mousePoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePoint.z = 0;

        if (Input.GetMouseButtonDown(1))

            ShapesCollision.DrawOverlap = true;
        switch (type)
        {
            case TestType.Instantiate:
                if (Input.GetMouseButtonDown(0)) Instantiate(addShape, mousePoint, Quaternion.identity);
                break;

            case TestType.Overlap:
                OverlapCheck();
                break;

            case TestType.Draging:
                if (Input.GetMouseButton(0)) DragShape();
                else if (select != null)
                {
                    select.RigidbodyType = selectType;
                    select = null;
                }
                break;

            case TestType.Raycast:
                RayCastCheck();
                break;

            case TestType.PushForce:
                PushForceShape();
                break;

            case TestType.PushVelocity:
                PushVecocityShape();
                break;
        }
    }

    void OverlapCheck()
    {
        //Overlap Check
        ShapeObject[] shapes = ShapeQuadTree.OverlapRect(mousePoint, checkRegion);

        foreach (var shape in shapes)
        {
            shape.SetColor(Color.black * 0.8f);
        }

        checkAngle += Input.mouseScrollDelta.y * 5;
        ShapeObject[] overlap = ShapesCollision.OverlapRectangle(mousePoint, checkAngle, checkRegion);
        foreach (var shape in overlap)
        {
            shape.SetColor(Color.white);
        }
        text.text = overlap.Length.ToString();
        text.transform.position = new Vector3(mousePoint.x, mousePoint.y);
    }
    void DragShape()
    {
        if (select != null)
        {
            select.Position = mouseOffset + mousePoint;
            select.SetColor(Color.white);
            select.RigidbodyType = RigidbodyType.Kinmatic;
        }
        else
        {
            ShapeObject[] shapes = ShapesCollision.OverlapPoint(mousePoint);
            if (shapes.Length <= 0) return;
            if (shapes[0].RigidbodyType == RigidbodyType.Static) return;

            select = shapes[0];
            selectType = select.RigidbodyType;
            mouseOffset = select.Position - mousePoint;
        }
    }
    void RayCastCheck()
    {
        if (Input.GetMouseButtonDown(1)) lineOrigin = mousePoint;

        Vector3 dirction = mousePoint - lineOrigin;
        ShapeObject[] shapes = ShapeQuadTree.OverlapLine(lineOrigin, dirction);

        foreach (var shape in shapes)
        {
            shape.SetColor(Color.black * 0.8f);
        }

        shapes = ShapesCollision.RayCast(lineOrigin, dirction);
        foreach (var shape in shapes)
        {
            shape.SetColor(Color.white * 0.8f);
        }
        text.text = shapes.Length.ToString();
        text.transform.position = lineOrigin;
    }
    void PushForceShape()
    {
        if (select == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ShapeObject[] shapes = ShapesCollision.OverlapPoint(mousePoint);
                if (shapes.Length <= 0) return;

                select = shapes[0];
            }
        }
        else
        {
            mouseOffset = mousePoint - select.Position;

            text.text = mouseOffset.magnitude.ToString();
            text.transform.position = mousePoint;
            Debug.DrawLine(select.Position, mousePoint);

            if (Input.GetMouseButtonUp(0))
            {
                select.AddForce(-mouseOffset);
                select = null;
            }
        }
    }
    void PushVecocityShape()
    {
        if (select == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ShapeObject[] shapes = ShapesCollision.OverlapPoint(mousePoint);
                if (shapes.Length <= 0) return;

                select = shapes[0];
            }
        }
        else
        {
            mouseOffset = mousePoint - select.Position;

            text.text = mouseOffset.magnitude.ToString();
            text.transform.position = mousePoint;
            Debug.DrawLine(select.Position, mousePoint);

            if (Input.GetMouseButtonUp(0))
            {
                select.Velocity = (-mouseOffset);
                select = null;
            }
        }
    }

}
