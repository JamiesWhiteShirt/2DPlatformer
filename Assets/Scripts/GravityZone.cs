using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(BoxCollider2D))]
public class GravityZone : MonoBehaviour
{
	void Start ()
	{
		Bounds bounds = GetComponent<BoxCollider2D>().bounds;
		Vector3 min = bounds.center - transform.position - bounds.size / 2.0f;
		Vector3 max = min + bounds.size;

		Mesh mesh = new Mesh();

		mesh.vertices = new Vector3[]
		{
			new Vector3(min.x, min.y, 0.0f),
			new Vector3(min.x, max.y, 0.0f),
			new Vector3(max.x, max.y, 0.0f),
			new Vector3(max.x, min.y, 0.0f)
		};
		mesh.uv = new Vector2[]
		{
			new Vector2(min.x, min.y),
			new Vector2(min.x, max.y),
			new Vector2(max.x, max.y),
			new Vector2(max.x, min.y)
		};
		mesh.triangles = new int[]
		{
			0, 1, 2,
			2, 3, 0
		};

		MeshFilter filter = GetComponent<MeshFilter>();
		filter.mesh = mesh;
	}
	
	void Update ()
	{
		
	}
}