using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEngine;

public class GroundConstructor : MonoBehaviour
{
    public TextAsset cloudFile;
    public GameObject point;
    public GameObject pointBasket;
    [SerializeField] int pointAmount;
    private List<string> pointList;
    private List<Vector3> pointCloud;
    private List<GameObject> pointObjects;

    // Start is called before the first frame update
    void Start()
    {
        pointList = new List<string>();
        pointCloud = new List<Vector3>();
        pointObjects = new List<GameObject>();

        // To transfer the file to the unity list
        ReadFile(cloudFile);

        // To get amount of lines on the first line of the file
        SetAmount(cloudFile);

        // To transfer the list of string into a list of points
        ListToPoints(pointList);

        // To create the physical pointCloud
        PlacePoints(pointCloud);

        // Create triangles
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ReadFile(TextAsset cloud)
    {
        // Set up variables
        string content = cloud.text;
        string[] allPoints = content.Split("\n");

        pointList = new List<string>(allPoints);

        // Remove the first, count line
        pointList.Remove(pointList[0]);
    }

    void SetAmount(TextAsset file)
    {
        // Set up variables
        string content = file.text;
        string[] allPoints = content.Split("\n");
        int count = allPoints.Count() - 1;
        int tryInt = 0;

        Debug.Log($"Den fyrste lina til fili segjer me hev {allPoints[0]} punkt");
        if (int.TryParse(allPoints[0], out tryInt))
        {
            Debug.Log("Fyrste lina er eit tal!");
            if (count != int.Parse(allPoints[0]))
            {
                StreamWriter stream = new StreamWriter(Application.dataPath + "/files/32-1-490-163-56.txt", false);
                stream.Write((count).ToString() + "\n" + content);
                stream.Close();
                Debug.Log($"No hev me {count} punkt, og fili segjer me hev {allPoints[0]} punkt");
            }
        }
        else
        {
            Debug.Log("Fyrste lina er jo ikkje eit tal!");

            StreamWriter stream = new StreamWriter(Application.dataPath + "/files/32-1-490-163-56.txt", false);
            stream.Write((count + 1).ToString() + "\n" + content);
            stream.Close();
            Debug.Log($"No hev me {count + 1} punkt, og fili segjer me hev {allPoints[0]} punkt");
        }
        pointAmount = count;
        Debug.Log("Fili er no riktig");
    }

    void ListToPoints(List<string> list)
    {
        for (int i = 0; i < list.Count(); i++)
        {
            pointCloud.Add(StringToVector(list[i]));
        }
    }

    void PlacePoints(List<Vector3> points)
    {
        // Getting the average so that the points come closer to an orego
        Vector3 average = new Vector3(points.Average(x => x.x), points.Average(x => x.y), points.Average(x => x.z));
        Debug.Log($"Midpunktet er {average}");

        // Creation of the gameObject points
        for (int i = 0; i < points.Count(); i++) 
        {
            // Create GameObject at location
            pointObjects.Add(Instantiate(point, points[i] - average, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)));
            pointObjects[i].transform.SetParent(pointBasket.transform);
        }
    }
    
    Vector3 StringToVector(string str)
    {
        if (string.IsNullOrEmpty(str))
            return Vector3.zero;

        float x = 0;
        float y = 0;
        float z = 0;

        string[] num = str.Split(' ');
        //Debug.Log($"string str {str} has been split into string[] num {num[0]}, {num[1]}, {num[2]}");
       
        CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        ci.NumberFormat.CurrencyDecimalSeparator = ".";

        x = float.Parse(num[0], NumberStyles.Any, ci);
        y = float.Parse(num[1], NumberStyles.Any, ci);
        z = float.Parse(num[2], NumberStyles.Any, ci);
        //Debug.Log($"string[] num {num[0]}, {num[1]}, {num[2]} has been Parsed into Vector3({x}, {y}, {z})");

        return new Vector3(x, z, y);
    }

    void CreateTriangles(List<Vector3> points)
    {
        // Finn trekantkoplingane frå punktlista

        // Skriv indeksfili med trekantindeksi

        // Rendre trekantane frå koplingane/punkti
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        for (int i = 0; i < pointCloud.Count(); i++)
        {
            Gizmos.DrawLine(pointObjects[i].transform.position, pointObjects[i].transform.position + Vector3.up);
        }
    }
}
