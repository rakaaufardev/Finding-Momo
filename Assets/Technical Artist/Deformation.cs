using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deformation : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    private Vector3[] originalVertices;
    private Vector3[] modifiedVertices;

    [SerializeField] private float amplitude = 1f;
    private float frequency = 1f;
    private float newAmplitude;

    [SerializeField] private Rigidbody rb;

    /*private float targetRotationX;*/

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        originalVertices = meshFilter.mesh.vertices;
        modifiedVertices = new Vector3[originalVertices.Length];
        originalVertices.CopyTo(modifiedVertices, 0);
    }

    void Update()
    {
        DeformMesh();
    }

    void DeformMesh()
    {
        float targetAmplitude = amplitude;

        if (rb != null)
        {
            float velocity = rb.velocity.y;
            targetAmplitude = amplitude * Mathf.Clamp(velocity, -10f, 10f);
            /*targetRotationX = amplitude * Mathf.Clamp(velocity, 0f, 10f);*/
        }

        newAmplitude = Mathf.Lerp(newAmplitude, targetAmplitude, Time.deltaTime * 15f);

        /*Vector3 targetRotation = new Vector3(targetRotationX * 4.5f, 180, 0);
        Quaternion target = Quaternion.Euler(targetRotation);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 30f);*/

        int count = originalVertices.Length;

        //for (int i = 0; i < count; i++)
        //{
        //    int index = i;

        //    Vector3 vertex = originalVertices[index];
            
        //    vertex.y += Mathf.Sin((vertex.x) * frequency) * newAmplitude * 0.1f;
            
        //    modifiedVertices[index] = vertex;
        //}
        
        //meshFilter.mesh.vertices = modifiedVertices;
        //meshFilter.mesh.RecalculateNormals();
        //meshFilter.mesh.RecalculateBounds();
    }
}
