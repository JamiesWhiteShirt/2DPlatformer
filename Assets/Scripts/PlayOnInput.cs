using UnityEngine;
using System.Collections;

public class PlayOnInput : MonoBehaviour
{
	public string input;
	public AudioClip clip;
	
	void Update ()
	{
		if (Input.GetButtonDown(input))
		{
			AudioSource.PlayClipAtPoint(clip, transform.position);
		}
	}
}