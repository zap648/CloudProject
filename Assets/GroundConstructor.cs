using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEngine;

public class GroundConstructor : MonoBehaviour
{
    public TextAsset cloudFile;
    public GameObject point;
    public GameObject pointBasket;
    [SerializeField] int pointAmount;
    [SerializeField] List<string> wordList;
    [SerializeField] List<Vector3> pointCloud;
    [SerializeField] List<GameObject> pointObjects;

    // Start is called before the first frame update
    void Start()
    {
        // To transfer the file to the unity list
        ReadFile(cloudFile);

        // To get amount of lines in the file
        pointAmount = GetAmount(cloudFile);

        // To transfer the list of string into a list of points
        ListToPoints(wordList);

        // To create the physical pointCloud
        PlacePoints(pointCloud);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ReadFile(TextAsset cloud)
    {
        // Set up variables
        var content = cloud.text;
        var AllPoints = content.Split("\n");

        //// The full list
        //wordList = new List<string>(AllPoints);

        // To test a certain number of lines
        // Add only every xth line (for performance's sake, KEEP IT)
        int xth = 40;
        for (int i = 0; i < AllPoints.Length; i++)
        {
            if (AllPoints[i] != null)
                if (i % xth == 0)
                    wordList.Add(AllPoints[i]);
        }

        // Remove the first line, count line
        wordList.Remove(wordList[0]);
    }

    int GetAmount(TextAsset file)
    {
        int count = File.ReadLines(AssetDatabase.GetAssetPath(file)).Count();

        return count - 1;
    }

    void ListToPoints(List<string> list)
    {
        string temp = "";
        int xzyCount = 0;
        double tempX = 0;
        double tempZ = 0;
        double tempY = 0;

        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < list[i].Length; j++)
            {
                if (list[i][j] == ' ')
                {
                    // Set current temp as x, z, or y value
                    if (xzyCount == 0)
                    {
                        // Set current temp as temp X value
                        tempX = float.Parse(temp, CultureInfo.InvariantCulture.NumberFormat);
                        Debug.Log($"Converting temp to tempX");
                        xzyCount = 1;
                    }
                    else if (xzyCount == 1)
                    {
                        // Set current temp as temp Z value
                        tempZ = float.Parse(temp, CultureInfo.InvariantCulture.NumberFormat);
                        Debug.Log($"Converting temp to tempZ");
                        xzyCount = 2;
                    }

                    // Clear temp
                    temp = "";
                }
                else if (j == list[i].Length - 1)
                {
                    if (xzyCount == 2)
                    {
                        // Set current temp as temp Y value
                        tempY = float.Parse(temp, CultureInfo.InvariantCulture.NumberFormat);
                        Debug.Log($"Converting temp to tempY");

                        // Due to being start of a new line, set current values in the pointCloud list
                        pointCloud.Add(new Vector3((float)tempX, (float)tempY, (float)tempZ));
                        xzyCount = 0;
                    }

                    // Clear temp
                    temp = "";

                    Debug.Log("New Line");
                }
                else
                {
                    temp += list[i][j];
                }
            }
        }
    }

    void PlacePoints(List<Vector3> points)
    {
        // Getting the average so that the points come closer to the somewhat stable orego
        Vector3 average = new Vector3(points.Average(x=>x.x), points.Average(x=>x.y), points.Average(x=>x.z));

        
        //for (int i = 0; i < points.Count; i++)
        //{
        //    average += points[i];
        //}
        //average /= points.Count;
        Debug.Log($"Center point is {average}");

        // Creation of the gameObject points
        for (int i = 0; i < points.Count; i++) 
        {
            // Create GameObject at location
            pointObjects.Add(Instantiate(point, points[i] - average, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)));
            pointObjects[i].transform.SetParent(pointBasket.transform);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        for (int i = 0; i < pointCloud.Count; i++)
        {
            Gizmos.DrawLine(pointObjects[i].transform.position, pointObjects[i].transform.position + Vector3.up);
        }
    }
}
