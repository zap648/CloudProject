using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BallManager : MonoBehaviour
{
    [Header("Balls")]
    public GameObject ball;
    public GameObject drop;
    public bool spawnBall;
    [SerializeField] List<GameObject> balls;
    [SerializeField] List<GameObject> pool;
    [SerializeField] GroundConstructor ground;

    private void Start()
    {
        //SpawnBalls(1);
    }

    private void Update()
    {
        if (spawnBall)
        {
            SpawnBall(Vector3.zero);
            spawnBall = false;
        }
    }

    public GameObject SpawnBall(Vector3 position)
    {
        position += this.transform.position;

        if (pool.Count() >= 1)
        {
            GameObject newBall = pool.Last();
            pool.Remove(newBall);
            newBall.transform.position = position;
            newBall.GetComponent<Ball>().position = position;
            newBall.GetComponent<Ball>().velocity = Vector3.zero;
            newBall.SetActive(true);
            return newBall;
        }
        else
        {
            GameObject newBall = Instantiate(ball, position, Quaternion.Euler(Vector3.zero), this.gameObject.transform);
            newBall.GetComponent<Ball>().ground = ground;
            return newBall;
        }
    }

    public void PoolBall(GameObject ball)
    {
        if (!pool.Contains(ball))
            pool.Add(ball);

        ball.SetActive(false);
    }
}
