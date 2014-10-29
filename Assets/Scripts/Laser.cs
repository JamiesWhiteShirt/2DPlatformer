using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour
{
	public float beamLength = 10.0f;

	private LineRenderer lineRenderer;

	void Start ()
	{
		lineRenderer = GetComponent<LineRenderer>();
	}
	
	void Update ()
	{
		Vector2 beamVec = transform.rotation * new Vector2(beamLength, 0.0f);

		Vector2 pos = new Vector2(transform.position.x, transform.position.y);
		RaycastHit2D ray = Physics2D.Raycast(pos + beamVec.normalized * 0.125f, beamVec.normalized, beamLength);

		if (ray.collider != null && PlayerController.IsMe(ray.collider.gameObject))
		{
			PlayerController.Kill();
		}

		Vector2 laser = ray.collider == null ? beamVec : (ray.point - pos);

		lineRenderer.SetPosition(0, new Vector2(0.0f, 0.0f));
		lineRenderer.SetPosition(1, new Vector2(laser.magnitude, 0.0f));
	}
}