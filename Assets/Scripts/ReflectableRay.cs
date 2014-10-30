using UnityEngine;
using System.Collections;

public abstract class ReflectableRay : MonoBehaviour
{
	public float rayLength = 10.0f;

	public Vector2 rayOrigin = new Vector2(0.0f, 0.0f);

	private LineRenderer lineRenderer;
	protected int layerMask;
	protected bool isActive = true;

	private Reflector prevReflector;

	public virtual void Start()
	{
		lineRenderer = GetComponent<LineRenderer>();
	}

	void OnDestroy ()
	{
		ReplaceReflector(null);
	}

	public virtual void Update()
	{
		Vector2 position = new Vector2(transform.position.x, transform.position.y);

		Trace(position, transform.rotation * new Vector2(1.0f, 0.0f));
	}

	private void Trace(Vector2 position, Vector2 direction)
	{
		Vector2 beamVec = direction * rayLength;

		RaycastHit2D ray = Physics2D.Raycast(position + direction * 0.0625f, direction, rayLength, layerMask);

		Vector2 hitPoint = ray.collider == null ? (position + beamVec) : ray.point;

		if (ray.collider != null)
		{
			if (ray.collider.gameObject.GetComponent<Reflector>())
			{
				hitPoint = ray.collider.transform.position;
			}
		}

		RayHit(ray, position, hitPoint);

		lineRenderer.SetPosition(0, position + rayOrigin);
		lineRenderer.SetPosition(1, hitPoint);

		if (ray.collider != null)
		{
			ReplaceReflector(ray.collider.gameObject.GetComponent<Reflector>());
		}
		else
		{
			ReplaceReflector(null);
		}
	}

	private void ReplaceReflector(Reflector reflector)
	{
		if (prevReflector != reflector)
		{
			if (prevReflector != null)
			{
				prevReflector.Remove(this);
			}
			if (reflector != null)
			{
				reflector.Incoming(this);
			}
			prevReflector = reflector;
		}
	}

	public abstract void RayHit(RaycastHit2D ray, Vector2 fromPoint, Vector2 hitPoint);
}