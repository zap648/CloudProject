using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

public class GroundConstructor : MonoBehaviour
{
    [SerializeField] TextAsset cloudFile;
    [SerializeField] GameObject pointPreFab;
    [SerializeField] GameObject pointBasket;
    [SerializeField] float gizmoScale;
    [SerializeField] Vector3 midPoint;
    [SerializeField] int pointAmount;

    private List<Vector3> pointCloud;
    private List<Vector3> scaledCloud;

    private Vector3 gizmoSize;

    private List<Triangle> triangles;


    // Start is called before the first frame update
    void Start()
    {
        scaledCloud = new List<Vector3>();
        gizmoSize = Vector3.up * gizmoScale;

        // To transfer the file to the list
        ReadFile(cloudFile);

        // To get amount of lines on the first line of the file
        SetAmount(cloudFile);

        // Create triangles
        CreateTriangles(scaledCloud);
    }

    void ReadFile(TextAsset cloud)
    {
        // Set up variables
        string content = cloud.text;
        string[] allPoints = content.Split("\n");
        List<string> pointList = new List<string>(allPoints);

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

        if (int.TryParse(allPoints[0], out tryInt))
        {
            if (count != int.Parse(allPoints[0]))
            {
                stream = new StreamWriter(Application.dataPath + "/files/points.txt", false);
                stream.Write((count).ToString() + "\n" + content);
                stream.Close();
            }
        }
        else
        {
            stream = new StreamWriter(Application.dataPath + "/files/32-1-490-163-56.txt", false);
            stream.Write((count + 1).ToString() + "\n" + content);
            stream.Close();
        }
        pointAmount = count;
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
        // Det kan hende at eg må få dette i same funksjon som den yver... fanden...

        // Trekantene har punktindeksar i dei tri fyrste verdiane og nabotrekantindeksar i dei tri andre verdiane


        // Finn trekantkoplingane frå punktlista og skriv dei i indeksfili med trekantindeksi
        // Holy fucking shit. Dette kjem i ein eigen funksjon


        // Rendre trekantane frå koplingane/punkti
        // Dette kan gjerast i ein eigen funksjon

    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.white;

    //    for (int i = 0; i < scaledCloud.Count(); i++)
    //        Gizmos.DrawLine(scaledCloud[i], scaledCloud[i] + gizmoSize);
    //}
}
