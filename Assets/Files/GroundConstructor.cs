using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

public class GroundConstructor : MonoBehaviour
{
    public TextAsset cloudFile;
    public GameObject pointPreFab;
    public GameObject pointBasket;
    [SerializeField] Vector3 midPoint;
    [SerializeField] int pointAmount;
    private List<string> pointList;
    private List<Vector3> pointCloud;
    private List<Vector3> scaledCloud;

    // Start is called before the first frame update
    void Start()
    {
        pointList = new List<string>();
        pointCloud = new List<Vector3>();
        scaledCloud = new List<Vector3>();

        // To transfer the file to the unity list
        ReadFile(cloudFile);

        // To get amount of lines on the first line of the file
        SetAmount(cloudFile);

        // To transfer the list of string into a list of points
        ListToPoints(pointList);

        // To create the physical pointCloud
        //PlacePoints(scaledCloud);

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

        midPoint = GetMidPoint(pointList);

        for (int i = 0; i < pointList.Count(); i++)
            scaledCloud.Add(StringToVector(pointList[i]) - midPoint);
    }

    void SetAmount(TextAsset file)
    {
        // Set up variables
        string content = file.text;
        string[] allPoints = content.Split("\n");
        int count = allPoints.Count() - 1;
        int tryInt = 0;
        StreamWriter stream;

        Debug.Log($"Den fyrste lina til fili segjer me hev {allPoints[0]} punkt");
        if (int.TryParse(allPoints[0], out tryInt))
        {
            Debug.Log("Fyrste lina er eit tal!");
            if (count != int.Parse(allPoints[0]))
            {
                stream = new StreamWriter(Application.dataPath + "/files/32-1-490-163-56.txt", false);
                stream.Write((count).ToString() + "\n" + content);
                stream.Close();
                Debug.Log($"No hev me {count} punkt, og fili segjer me hev {allPoints[0]} punkt");
            }
        }
        else
        {
            Debug.Log("Fyrste lina er jo ikkje eit tal!");

            stream = new StreamWriter(Application.dataPath + "/files/32-1-490-163-56.txt", false);
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
            pointCloud.Add(StringToVector(list[i]));

        //midPoint = new Vector3(pointCloud.Average(x => x.x), pointCloud.Average(x => x.y), pointCloud.Average(x => x.z));
    }

    void PlacePoints(List<Vector3> points)
    {
        // Creation of the gameObject points
        for (int i = 0; i < points.Count(); i++)
            // Create GameObject at location
            Instantiate(pointPreFab, points[i], new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)).transform.SetParent(pointBasket.transform);
    }

    Vector3 StringToVector(string str)
    {
        if (string.IsNullOrEmpty(str))
            return Vector3.zero;

        string[] num = str.Split(' ');
        //Debug.Log($"string str {str} has been split into string[] num {num[0]}, {num[1]}, {num[2]}");
       
        CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        ci.NumberFormat.CurrencyDecimalSeparator = ".";

        float x = float.Parse(num[0], NumberStyles.Any, ci);
        float y = float.Parse(num[1], NumberStyles.Any, ci);
        float z = float.Parse(num[2], NumberStyles.Any, ci);
        //Debug.Log($"string[] num {num[0]}, {num[1]}, {num[2]} has been Parsed into Vector3({x}, {y}, {z})");

        return new Vector3(x, z, y);
    }

    List<Vector3> StringToVector(List<string> strList)
    {
        List<Vector3> list = new List<Vector3>();

        foreach (string str in strList)
            list.Add(StringToVector(str));

        return list;
    }

    Vector3 GetMidPoint(List<string> list)
    {
        Vector3 midPoint = new Vector3(StringToVector(list).Average(x => x.x), StringToVector(list).Average(x => x.y), StringToVector(list).Average(x => x.z));

        return midPoint;
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

        for (int i = 0; i < scaledCloud.Count(); i++)
        {
            Gizmos.DrawLine(scaledCloud[i], scaledCloud[i] + Vector3.up);
        }
    }
}
