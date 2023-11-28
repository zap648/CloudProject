using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        tri = triangle.GetComponent<Triangle>();
        onTri = CheckOnTriangle(tri.points[0], tri.points[1], tri.points[2], position);
        Debug.Log(onTri);

        //logg = GetComponent<KuleLogg>();

        gravityAcc = new Vector3(0.0f, -9.81f, 0.0f);
        if (onTri || FindTri(tri.neighbour)) normal = tri.normal * Vector3.Dot(-gravityAcc, tri.normal);
        else normal = Vector3.zero;
        position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        move();
    }

    void move()
    {
        onTri = CheckOnTriangle(tri.points[0], tri.points[1], tri.points[2], position);
        Debug.Log(onTri);

        if (onTri || FindTri(tri.neighbour)) normal = tri.normal * Vector3.Dot(-gravityAcc, tri.normal);
        else normal = Vector3.zero;

        acceleration = gravityAcc + normal;
        velocity += acceleration * Time.deltaTime;
        position += velocity * Time.deltaTime;
        transform.position = position;
    }

    public static bool CheckOnTriangle(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
    {
        Vector3 u = B - A;
        Vector3 v = C - A;
        Vector3 w = P - A;

        Vector3 vW = Vector3.Cross(v, w);
        Vector3 vU = Vector3.Cross(v, u);

        if (Vector3.Dot(vW, vU) < 0)
        {
            //Debug.Log("vW*vU '" + Vector3.Dot(vW, vU) + "' er mindre enn 0");
            return false;
        }
        //Debug.Log("vW*vU '" + Vector3.Dot(vW, vU) + "' er større enn 0");

        Vector3 uW = Vector3.Cross(u, w);
        Vector3 uV = Vector3.Cross(u, v);

        if (Vector3.Dot(uW, uV) < 0)
        {
            //Debug.Log("uW*uV '" + Vector3.Dot(uW, uV) + "' er mindre enn 0");
            return false;
        }
        //Debug.Log("uW*uV '" + Vector3.Dot(uW, uV) + "' er større enn 0");

        float denom = uV.magnitude;
        float r = vW.magnitude / denom;
        float t = uW.magnitude / denom;

        //if (r <= 1) Debug.Log("r '" + r + "' er mindre enn eller lik 1");
        //else Debug.Log("r '" + r + "' er større enn 1");

        //if (t <= 1) Debug.Log("t '" + t + "' er mindre enn eller lik 1");
        //else Debug.Log("t '" + t + "' er større enn 1");

        //if (r + t <= 1) Debug.Log("r + t '" + (r + t) + "' er mindre enn eller lik 1");
        //else Debug.Log("r + t '" + (r + t) + "' er større enn 1");

        return (r <= 1 && t <= 1 && r + t <= 1);
    }

    bool FindTri(GameObject[] nabo)
    {
        //if (logg.bilde < logg.log.Length)
        //    logg.logKule();

        Triangle triI;
        for (int i = 0; i < nabo.Length; i++)
        {
            if (nabo[i] == null) continue;

            triI = nabo[i].GetComponent<Triangle>();
            onTri = CheckOnTriangle(triI.points[0], triI.points[1], triI.points[2], position);
            if (onTri)
            {
                triangle = nabo[i];
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
