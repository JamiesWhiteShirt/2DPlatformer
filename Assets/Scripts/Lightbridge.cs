using UnityEngine;
using System.Collections;

public class Lightbridge : ReflectableRay
{
	private ParticleSystem sparkle;
	private BoxCollider2D boxCollider;

	public virtual void Start()
	{
		base.Start();
		sparkle = transform.FindChild("Sparkle").gameObject.GetComponent<ParticleSystem>();
		boxCollider = GetComponent<BoxCollider2D>();
		layerMask = LayerMask.GetMask("Terrain");
	}

	public override void RayHit(RaycastHit2D ray, Vector2 fromPoint, Vector2 hitPoint)
	{
		Vector2 bridge = hitPoint - fromPoint;

		sparkle.transform.position = transform.position + transform.rotation * new Vector2(bridge.magnitude / 2.0f, 0.0f);
		sparkle.transform.localScale = new Vector2(bridge.magnitude, 2.0f / 16.0f);
		sparkle.transform.rotation = transform.rotation;

		//sparkle.emissionRate = (hitPoint - fromPoint).magnitude * 5.0f;

		boxCollider.center = new Vector2(bridge.magnitude / 2.0f, 0.0f);
		boxCollider.size = new Vector2(bridge.magnitude, 2.0f / 16.0f);
	}
}