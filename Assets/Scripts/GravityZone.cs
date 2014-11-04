using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(BoxCollider2D))]
public class GravityZone : MonoBehaviour
{
	void Start ()
	{
		Vector3 size = GetComponent<BoxCollider2D>().bounds.size;

		Mesh mesh = new Mesh();

		mesh.vertices = new Vector3[]
		{
			new Vector3(-0.5f, -0.5f, 0.0f),
			new Vector3(-0.5f, 0.5f, 0.0f),
			new Vector3(0.5f, 0.5f, 0.0f),
			new Vector3(0.5f, -0.5f, 0.0f)
		};
		mesh.uv = new Vector2[]
		{
			new Vector2(0.0f, 0.0f),
			new Vector2(0.0f, size.y),
			new Vector2(size.x, size.y),
			new Vector2(size.x, 0.0f)
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