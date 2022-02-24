using UnityEngine;

public class Main : MonoBehaviour
{
    private void Start()
    {
        GenerateCircle();
    }
    
    private void GenerateCircle()
    {
        GenMeshCircle genMeshCircle = new();
        GameObject circle = genMeshCircle.Construct(
            Random.Range(0.01f, 1), 
            Random.Range(0.01f, 1), 
            (byte)Random.Range(3,90),
            GenMeshCircle.UvProjection.AngularRadial);

        circle.transform.LookAt(Camera.main.transform);
        circle.transform.position = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
    }
}
