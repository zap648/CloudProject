using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle : MonoBehaviour
{
    [Header("Points")]
    public Vector3[] points = new Vector3[3];

    [Header("Normal")]
    public Vector3 normal;

    [Header("UVs")]
    public Vector2 UV;

    [Header("Nabo")]
    public GameObject[] neighbour;
}
