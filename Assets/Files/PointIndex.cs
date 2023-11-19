using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointIndex : MonoBehaviour
{
    [SerializeField] int[] i = new int[7];

    //PointIndex(int[] x) { index = x; }
    //~PointIndex() { }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EditIndex(int index, int n) 
    { 
        i[index] = n;
    }

}
