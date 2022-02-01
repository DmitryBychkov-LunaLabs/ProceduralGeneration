using UnityEngine;

public class Main : MonoBehaviour
{
    private GenMeshCircle _genMeshCircle;
    
    private void Awake()
    {
        float startTime = Time.realtimeSinceStartup;
        _genMeshCircle = new GenMeshCircle();

        for (int i = 0; i < 10; i++)
        {
            GenerateCircle();
        }
        
        Debug.Log((Time.realtimeSinceStartup - startTime) * 1000f + "ms");
    }
    
    private void GenerateCircle()
    {
        GameObject circle = _genMeshCircle.Construct(
            Random.Range(0.01f, 1), 
            Random.Range(0.01f, 1), 
            (byte)Random.Range(3,90),
            GenMeshCircle.UvProjection.AngularRadial);

        circle.transform.LookAt(Camera.main.transform);
        circle.transform.position = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
    }
}
