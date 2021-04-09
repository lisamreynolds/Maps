using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleCell : MonoBehaviour
{
    internal void SetMesh(Vector3[] vertices)
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = vertices;
        mesh.triangles = new int[] { 0, 1, 2 };
        mesh.RecalculateNormals();
    }
}
