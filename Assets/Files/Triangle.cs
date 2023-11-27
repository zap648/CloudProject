using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle : MonoBehaviour
{
    [Header("Points")]
    public Vector3[] points = new Vector3[3];

    [Header("Normal")]
    public Vector3 normal;

    [Header("Nabo")]
    public GameObject[] t;
}
