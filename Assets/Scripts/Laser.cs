using UnityEngine;
using System.Collections;

public class Laser : ReflectableRay
{
	private ParticleSystem fizzle;

	public virtual void Start ()
	{
		base.Start();
		fizzle = transform.FindChild("Fizzle").gameObject.GetComponent<ParticleSystem>();
		layerMask = LayerMask.GetMask("Terrain", "Player");
	}

	public override void RayHit(RaycastHit2D ray, Vector2 fromPoint, Vector2 hitPoint)
	{
		if (ray.collider != null && PlayerController.IsMe(ray.collider.gameObject))
		{
			PlayerController.Kill();
		}

		fizzle.transform.position = hitPoint;

		if (ray.collider == null)
		{
			fizzle.enableEmission = false;
		}
		else if (ray.collider.gameObject.GetComponent<Reflector>() != null)
		{
			fizzle.enableEmission = false;
		}
		else
		{
			fizzle.enableEmission = true;
		}
	}
}