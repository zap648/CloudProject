using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle : MonoBehaviour
{
    public int[] indices = new int[3];
    public int[] neighbours = new int[3];
    public Vector3 normal = Vector3.zero;
}
