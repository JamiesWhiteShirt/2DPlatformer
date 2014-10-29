using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour
{
	public GameObject obj;

	private Vector3 offset;

	void Start ()
	{
		offset = transform.position;
	}
	
	void Update ()
	{
		offset.y += (Physics2D.gravity.y > 0.0f ? -4.0f : 4.0f) * Time.deltaTime;

		if (offset.y > 1.0f) offset.y = 1.0f;
		if (offset.y < -1.0f) offset.y = -1.0f;

		transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, 0.0f) + offset;
	}
}