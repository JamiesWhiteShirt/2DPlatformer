using UnityEngine;
using System.Collections;

public class Reflector : MonoBehaviour
{
	public int[] reflectTo = new int[4] { 1, 2, 3, 0 };

	private ReflectableRay[] emission = new ReflectableRay[4];

	public int getIntAngle(float degrees)
	{
		return ((int)Mathf.Floor((degrees - 45.0f) / 90.0f) + 2) & 3;
	}

	public Vector2 getDirection(int angle)
	{
		return new Vector2(angle == 0 ? 1.0f : (angle == 2 ? -1.0f : 0.0f), angle == 1 ? 1.0f : (angle == 3 ? -1.0f : 0.0f));
	}

	public float getDegreeAngle(int angle)
	{
		return angle * 90.0f;
	}

	public void Incoming(ReflectableRay ray)
	{
		int angle = getIntAngle(ray.transform.rotation.eulerAngles.z);
		int reflectionAngle = reflectTo[angle];

		GameObject newObject = Instantiate(ray.gameObject) as GameObject;

		Vector3 direction = getDirection(reflectionAngle);

		newObject.transform.position = transform.position + direction / 2.0f;
		newObject.transform.rotation = Quaternion.AngleAxis(getDegreeAngle(reflectionAngle), Vector3.forward);

		if (emission[reflectTo[angle]] != null)
		{
			Destroy(emission[reflectionAngle].gameObject);
		}
		emission[reflectionAngle] = newObject.GetComponent<ReflectableRay>();
		emission[reflectionAngle].rayOrigin = transform.position - newObject.transform.position;
	}

	public void Remove(ReflectableRay ray)
	{
		int angle = getIntAngle(ray.transform.rotation.eulerAngles.z);
		int reflectionAngle = reflectTo[angle];

		if (emission[reflectionAngle] != null)
		{
			Destroy(emission[reflectionAngle].gameObject);
			emission[reflectionAngle] = null;
		}
	}
}