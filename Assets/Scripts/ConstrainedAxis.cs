using UnityEngine;
using System.Collections;

public class ConstrainedAxis : MonoBehaviour
{
	public bool constrainX;
	public bool constrainY;

	private Vector2 initialPosition;

	void Start ()
	{
		initialPosition = transform.position;
	}

	void Update()
	{
		Vector3 pos = transform.position;
		Vector3 vel = rigidbody2D.velocity;

		if (constrainX)
		{
			pos.x = initialPosition.x;
			vel.x = 0.0f;
		}
		if (constrainY)
		{
			pos.y = initialPosition.y;
			vel.y = 0.0f;
		}

		transform.position = pos;
		rigidbody2D.velocity = vel;
	}
}