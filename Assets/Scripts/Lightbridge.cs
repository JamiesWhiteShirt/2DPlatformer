using UnityEngine;
using System.Collections;

public class Lightbridge : MonoBehaviour
{
	public float beamLength = 10.0f;

	private LineRenderer lineRenderer;
	private BoxCollider2D boxCollider;
	private int layerMask;

	void Start()
	{
		lineRenderer = GetComponent<LineRenderer>();
		boxCollider = GetComponent<BoxCollider2D>();
		layerMask = LayerMask.GetMask("Terrain");
	}

	void Update()
	{
		Vector2 beamVec = transform.rotation * new Vector2(beamLength, 0.0f);

		Vector2 pos = new Vector2(transform.position.x, transform.position.y);
		RaycastHit2D ray = Physics2D.Raycast(pos + beamVec.normalized * 0.125f, beamVec.normalized, beamLength, layerMask);

		if (ray.collider != null && PlayerController.IsMe(ray.collider.gameObject))
		{
			//PlayerController.Kill();
		}

		Vector2 laser = ray.collider == null ? beamVec : (ray.point - pos);

		lineRenderer.SetPosition(0, new Vector2(0.0f, 0.0f));
		lineRenderer.SetPosition(1, new Vector2(laser.magnitude, 0.0f));

		boxCollider.center = new Vector2(laser.magnitude / 2.0f, 0.0f);
		boxCollider.size = new Vector2(laser.magnitude, 2.0f / 16.0f);
	}
}