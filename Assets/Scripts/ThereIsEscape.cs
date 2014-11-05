using UnityEngine;
using System.Collections;

public class ThereIsEscape : MonoBehaviour
{
	public KeyCode key;
	
	void Update ()
	{
		if (Input.GetKeyDown(key))
		{
			Application.Quit();
		}
	}
}