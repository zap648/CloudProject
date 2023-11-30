using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Radius")]
    public float radius;

    [Header("Physics")]
    public Vector3 Force;
    public Vector3 acceleration;
    public Vector3 velocity;
    public Vector3 position;
    public Vector3 gravityAcc;
    public Vector3 normal;

    [Header("Information")]
    //[SerializeField] KuleLogg logg;
    public GameObject triangle;
    [SerializeField] Triangle tri;
    [SerializeField] bool onTri = false;
    public GroundConstructor ground;

    // Start is called before the first frame update
    void Start()
    {
        //tri = triangle.GetComponent<Triangle>();
        //if (tri != null) onTri = CheckOnTriangle(tri.points[0], tri.points[1], tri.points[2], position);
        //else onTri = false;
        //Debug.Log(onTri);

        //logg = GetComponent<KuleLogg>();

        gravityAcc = new Vector3(0.0f, -9.81f, 0.0f);
        //if (onTri || FindTri(tri)) normal = tri.normal * Vector3.Dot(-gravityAcc, tri.normal);
        //else 
        normal = Vector3.zero;
        position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        if (ground.IsInsideGrid(this.gameObject))
        {
            triangle = ground.FindTriangle(this.gameObject);
            if (triangle != null)
                tri = triangle.GetComponent<Triangle>();
    }

        if (tri == null)
            onTri = false;
        else
            onTri = true;

        Debug.Log($"onTri is {onTri}");
        Debug.Log($"FindTri is {FindTri(tri)}");
        Debug.Log($"ground.IsInsideGrid is {ground.IsInsideGrid(this.gameObject)}");

        normal = Vector3.zero;
        if (ground.IsInsideGrid(this.gameObject))
            if (onTri || FindTri(tri)) 
                normal = tri.normal * Vector3.Dot(-gravityAcc, tri.normal);

        acceleration = gravityAcc + normal;
        velocity += acceleration * Time.deltaTime;
        position += velocity * Time.deltaTime;
        transform.position = position;
    }

    bool FindTri(Triangle triI)
    {
        //if (logg.bilde < logg.log.Length)
        //    logg.logKule();

        if (triI == null || !ground.IsInsideGrid(this.gameObject))
            return false;
        
        else
            for (int i = 0; i < triI.neighbour.Count(); i++)
            {
                if (triI.neighbour[i] == null) 
                    continue;

                triI = triI.neighbour[i].GetComponent<Triangle>();
                //if (ground.IsInsideGrid(this.gameObject))
                //    onTri = ground.CheckOnTriangle(triI.points[0], triI.points[1], triI.points[2], position);
                if (onTri)
                {
                    triangle = triI.neighbour[i];
                    tri = triI;
                    velocity = Vector3.ProjectOnPlane(velocity, tri.normal);
                    Debug.Log("On triangle '" + triangle.name + "'");
                    return true;
                }
            }

        Debug.Log("Not on a triangle");
        return false;
    }
}
