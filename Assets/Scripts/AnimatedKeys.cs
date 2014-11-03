using UnityEngine;
using System.Collections;

public class AnimatedKeys : MonoBehaviour
{
	public string[] keys;
	public GameObject[] keyObjects;
	private Vector3[] keyPositions;

	void Start ()
	{
		keyPositions = new Vector3[keyObjects.Length];
		for (int i = 0; i < keyObjects.Length; i++)
		{
			keyPositions[i] = keyObjects[i].transform.position;
		}
	}

	void Update()
	{
		for (int i = 0; i < keyObjects.Length; i++)
		{
			keyObjects[i].transform.position = Input.GetKey(keys[i]) ? (keyPositions[i] + new Vector3(0.0f, -1.0f / 16.0f, 0.0f)) : keyPositions[i];
		}
	}
}