using UnityEngine;
using System.Collections;

public class StasisBeam : ReflectableRay
{
	private ParticleSystem radiation;
	private Grabbable grabbedObject;
	private Vector2 lockPoint;

	public virtual void Start()
	{
		base.Start();
		radiation = transform.FindChild("Radiation").gameObject.GetComponent<ParticleSystem>();
		layerMask = LayerMask.GetMask("Terrain");
	}

	private Vector2 gridify(Vector2 vec)
	{
		return new Vector2(Mathf.Floor(vec.x) + 0.5f, Mathf.Floor(vec.y) + 0.5f);
	}

	public override void RayHit(RaycastHit2D ray, Vector2 fromPoint, Vector2 hitPoint)
	{
		if (ray.collider != null)
		{
			Grabbable grabbable = ray.collider.gameObject.GetComponent<Grabbable>();
			if (grabbable != null)
			{
				if (grabbable != grabbedObject)
				{
					grabbedObject = grabbable;
					lockPoint = gridify(transform.position + transform.rotation * new Vector2((hitPoint - fromPoint).magnitude + 0.5f, 0.0f));
				}
			}
			else
			{
				grabbedObject = null;
			}
		}
		else
		{
			grabbedObject = null;
		}

		radiation.enableEmission = grabbedObject != null;

		if (grabbedObject != null)
		{
			radiation.transform.position = grabbedObject.transform.position;
			radiation.transform.rotation = grabbedObject.transform.rotation;
			radiation.transform.localScale = grabbedObject.transform.localScale;

			if (!grabbedObject.grabbedByPlayer)
			{
				Vector2 otherPos = new Vector2(grabbedObject.transform.position.x, grabbedObject.transform.position.y);
				Vector2 force = (lockPoint - otherPos).normalized * 0.125f;
				if ((lockPoint - otherPos).magnitude < force.magnitude)
				{
					force = lockPoint - otherPos;
				}

				grabbedObject.rigidbody2D.velocity = force * 30.0f;
			}
		}
	}
}