using System.Collections.Generic;
using UnityEngine;

public class GenMeshCircle
{
    private UvProjection _uvProjection;

    private float _thickness;
    private float _innerRadius;
    private byte _angularSegmentsCount;

    public enum UvProjection { AngularRadial, ProjectionZ }

    private int VertexCount => _angularSegmentsCount * 2;
    private float RadiusOuter => _innerRadius + _thickness;

    private const float Tau = Mathf.PI + Mathf.PI;

    public GameObject Construct(float innerRadius, float thickness, byte segmentsCount, UvProjection projection)
    {
        _angularSegmentsCount = segmentsCount;
        _uvProjection = projection;
        _thickness = thickness;
        _innerRadius = innerRadius;

        ValidateConstructData();
        return GenerateMesh();
    }

    private GameObject GenerateMesh()
    {
        GameObject target = CreatePrimitive();

        Mesh mesh = new Mesh {name = "QuadRing"};

        mesh.Clear();

        var uvs = new List<Vector2>();
        var normals = new List<Vector3>();
        var vertices = new List<Vector3>();
        var triangleIndices = new List<int>();

        for (int i = 0; i < _angularSegmentsCount + 1; i++)
        {
            float t = i / (float) _angularSegmentsCount;
            float angRad = t * Tau;

            Vector2 dir = GetVectorByAngle(angRad);

            MakeVertices(dir, ref vertices);
            MakeNormals(ref normals);
            MakeUvs(ref uvs, t, dir);
        }

        MakeTriangleIndices(ref triangleIndices);

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangleIndices, 0);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);

        target.GetComponent<MeshFilter>().sharedMesh = mesh;
        target.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
        
        return target;
    }

    private GameObject CreatePrimitive()
    {
        GameObject primitive = new GameObject
        {
            name = $"QuadRing-Inner:{_innerRadius:F1}-thickness:{_thickness:F1}-Segments:{_angularSegmentsCount}"
        };

        primitive.AddComponent<MeshFilter>();
        primitive.AddComponent<MeshRenderer>();

        return primitive;
    }

    private void ValidateConstructData()
    {
        _innerRadius = _innerRadius switch
        {
            < 0f => 0.01f,
            > 1f => 1f,
            _ => _innerRadius
        };

        _thickness = _thickness switch
        {
            < 0f => 0.01f,
            > 1f => 1f,
            _ => _thickness
        };

        _angularSegmentsCount = _angularSegmentsCount switch
        {
            < 3 => 3,
            > 90 => 90,
            _ => _angularSegmentsCount
        };
    }

    private void MakeUvs(ref List<Vector2> uvs, float t, Vector2 dir)
    {
        switch (_uvProjection)
        {
            case UvProjection.AngularRadial:
                uvs.Add(new Vector2(t, 1));
                uvs.Add(new Vector2(t, 0));
                break;
            case UvProjection.ProjectionZ:
                uvs.Add(dir * 0.5f + Vector2.one * 0.5f);
                uvs.Add(dir * (_innerRadius / RadiusOuter) * 0.5f + Vector2.one * 0.5f);
                break;
        }
    }

    private void MakeNormals(ref List<Vector3> normals)
    {
        normals.Add(Vector3.forward);
        normals.Add(Vector3.forward);
    }

    private void MakeVertices(Vector2 dir, ref List<Vector3> vertices)
    {
        vertices.Add(dir * RadiusOuter);
        vertices.Add((dir * _innerRadius));
    }

    private void MakeTriangleIndices(ref List<int> triangleIndices)
    {
        for (int index = 0; index < _angularSegmentsCount; index++)
        {
            int indexRoot = index * 2;
            int indexInnerRoot = indexRoot + 1;
            int indexOuterNext = (indexRoot + 2); // % VertexCount;
            int indexInnerNext = (indexRoot + 3); // % VertexCount;

            triangleIndices.Add(indexRoot);
            triangleIndices.Add(indexOuterNext);
            triangleIndices.Add(indexInnerNext);

            triangleIndices.Add(indexRoot);
            triangleIndices.Add(indexInnerNext);
            triangleIndices.Add(indexInnerRoot);
        }
    }

    private Vector2 GetVectorByAngle(float angRad) => new Vector2(Mathf.Cos(angRad), Mathf.Sin(angRad));
}