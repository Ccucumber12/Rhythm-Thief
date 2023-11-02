using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
public class FieldOfView : MonoBehaviour
{
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float viewDistance = 20;
    [SerializeField] int rayCount = 2;
    [SerializeField] LayerMask layerMask;
    [SerializeField] Material normalVisionMaterial;
    [SerializeField] Material alertVisionMaterial;

    [HideInInspector] float startingAngle;
    private Vector3[] vertices;
    private Vector2[] uv;
    private int[] triangles;

    private Mesh mesh;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        meshRenderer.sortingOrder = -5;
        meshRenderer.material = normalVisionMaterial;
    }

    private void Start()
    {
        vertices = new Vector3[rayCount + 2];
        triangles = new int[rayCount * 3];
        uv = new Vector2[vertices.Length];
    }

    private void Update()
    {
        float angle = startingAngle;
        float angleIncrease = viewAngle / rayCount;

        vertices[0] = Vector3.zero; // The first vertex is origin
        
        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, MathUtils.GetVectorFromAngle(angle), viewDistance, layerMask);

            if (raycastHit.collider == null)
                vertex = MathUtils.GetVectorFromAngle(angle) * viewDistance;
            else
                vertex = (MathUtils.Vector2ToVector3(raycastHit.point) - transform.position);

            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0; // origin
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
    }

    public void SetAngle(float angle)
    {
        startingAngle = angle / 2;
    }

    public void SetBlind()
    {
        meshRenderer.enabled = false;
        meshCollider.enabled = false;
    }

    public void SetSighted()
    {
        meshRenderer.enabled = true;
        meshCollider.enabled = true;
    }

    public void SetAlert()
    {
        meshRenderer.material = alertVisionMaterial;
    }

    public void SetNormal()
    {
        meshRenderer.material = normalVisionMaterial;
    }
}
