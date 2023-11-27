using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GroundConstructor : MonoBehaviour
{
    [SerializeField] TextAsset cloudFile;
    [SerializeField] GameObject pointPrefab;
    [SerializeField] GameObject trianglePrefab;
    [SerializeField] GameObject pointBasket;
    [SerializeField] GameObject groundParent;
    [SerializeField] float gizmoScale;
    [SerializeField] Vector3 midPoint;
    [SerializeField] int pointAmount;
    [SerializeField] int triangleAmount;

    private List<Vector3> pointCloud;
    private List<Vector3> scaledCloud;
    private List<List<Vector3>> triGrid;

    private Vector3 gizmoSize;

    private List<GameObject> triangles;


    // Start is called before the first frame update
    void Start()
    {
        scaledCloud = new List<Vector3>();
        triGrid = new List<List<Vector3>>();
        gizmoSize = Vector3.up * gizmoScale;

        // To transfer the file to the list
        ReadFile(cloudFile);

        // To get amount of lines on the first line of the file
        SetAmount(cloudFile);

        // Create triangles
        CreateTriangles(scaledCloud);
    }

    // Update is called every frame
    void Update()
    {
        gizmoSize = Vector3.up * gizmoScale;
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
        float maxX = 0.0f;
        float maxZ = 0.0f;
        float minX = 0.0f;
        float minZ = 0.0f;

        int[] resolution = new int[2] { 10, 10 };

        List<Vector2> convex = new List<Vector2>();
        List<List<List<Vector3>>> grid = new List<List<List<Vector3>>>();

        // First I'll find the max x- & z-values & min x- & z-values
        for (int i = 0; i < points.Count(); i++)
        {
            if (points[i].x > maxX) maxX = points[i].x;
            if (points[i].z > maxZ) maxZ = points[i].z;
            if (points[i].x < minX) minX = points[i].x; // Eg skreiv "if (points[i].x < minZ)" Er det mogeleg?! Fy fanden!
            if (points[i].z < minZ) minZ = points[i].z;
        }
        Debug.Log($"Minimum XZ: {minX}, {minZ} & maximum XZ: {maxX}, {maxZ}");

        // Regular triangulation (counterclockwise / right-hand way)
        convex.Add(new Vector2(minX, minZ));
        convex.Add(new Vector2(maxX, minZ));
        convex.Add(new Vector2(maxX, maxZ));
        convex.Add(new Vector2(minX, maxZ));

        // Two-dimensional vectorList grid from minX, minZ to maxX, maxZ, at a resolution of resolutionX, resolutionZ
        // I'll have to figure out how many rows & columns there are, and create that many cells
        int cntX = 0;
        int cntY = 0;
        for (int x = 0; x < maxX - minX; x += resolution[0])
        {
            // Create column (left->right)
            grid.Add(new List<List<Vector3>>());
            Debug.Log("Kolonne laga");
            for (int z = 0; z < maxZ - minZ; z += resolution[1])
            {
                // Create cell in column (bottom->top)
                grid[cntX].Add(new List<Vector3>());
                // Set center point in cell (center)
                grid[cntX][cntY].Add(new Vector3(x + minX + (resolution[0] / 2), 0.0f, z + minZ + (resolution[1] / 2)));
                Debug.Log("Cella laga");
                cntY++;
            }
            cntY = 0;
            cntX++;
        }

        // Then I'll go through the point cloud to put the points in the grid
        for (int i = 0; i < points.Count(); i++)
            for (int j = 0; j < grid.Count(); j++)
                // Find if the current point is inside this 
                if (isPointInsideRow(points[i].x, grid[j][0][0].x, resolution[0]))
                    for (int k = 0; k < grid[j].Count(); k++)
                        if (isPointInsideRow(points[i].z, grid[j][k][0].z, resolution[1]))
                            grid[j][k].Add(new Vector3(points[i].x, points[i].y, points[i].z));

        // Afterwards we will change position of the points placed in the grid to the grid's center value
        for (int i = 0; i < grid.Count(); i++)
            for (int j = 0; j < grid[i].Count(); j++)
                for (int k = 1; k < grid[i][j].Count(); k++)
                    grid[i][j][k].Set(grid[i][j][0].x, grid[i][j][k].y, grid[i][j][0].z);

        // and set the y-value to the average of all the cloud nodes in the area
        float tempYAvg = 0.0f;
        int cnt = 0;
        for (int i = 0; i < grid.Count(); i++)
        {
            for (int j = 0; j < grid[i].Count(); j++)
            {
                tempYAvg = 0.0f;
                cnt = 0;
                for (int k = 1; k < grid[i][j].Count(); k++)
                {
                    tempYAvg += grid[i][j][k].y;
                    cnt++;
                }
                tempYAvg /= cnt;
                grid[i][j][0] += Vector3.up * tempYAvg;
            }
        }

        // for viewing purposes
        for (int i = 0; i < grid.Count(); i++)
        {
            triGrid.Add(new List<Vector3>());
            for (int j = 0; j < grid[i].Count(); j++)
                triGrid[i].Add(grid[i][j][0]);
        }

        //// render triangles
        //// Draw a triangle mesh, square by square
        //for (int i = 0; i < grid.Count() - 1; i++)
        //    for (int j = 0; j < grid[i].Count() - 1; j++)
        //    {
        //        triangles.Add(Instantiate(trianglePrefab, triGrid[i][j], Quaternion.Euler(0.0f, 0.0f, 0.0f)));
        //        triangles.Add(Instantiate(trianglePrefab, triGrid[i][j], Quaternion.Euler(0.0f, 0.0f, 0.0f)));
        //    }
    }

    bool isPointInsideRow(float checkAxis, float reqAxis, float resolution)
    {
        if (checkAxis > reqAxis - (resolution / 2) 
            && checkAxis < reqAxis + (resolution / 2))
            return true;
        else
            return false;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.white;

        //for (int i = 0; i < scaledCloud.Count(); i++)
        //    Gizmos.DrawLine(scaledCloud[i], scaledCloud[i] + gizmoSize);

        Gizmos.color = Color.green;

        for (int i = 0; i < triGrid.Count(); i++)
            for (int j = 0; j < triGrid[i].Count(); j++)
                Gizmos.DrawLine(triGrid[i][j], triGrid[i][j] + gizmoSize);
    }
}
