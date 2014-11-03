using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Goal : MonoBehaviour
{
	private static int currrentScene;

	public static void ReloadCurrentScene()
	{
		Application.LoadLevel(currrentScene);
	}

	public static void NextScene()
	{
		Application.LoadLevel(++currrentScene);
	}

	public static void PrevScene()
	{
		Application.LoadLevel(++currrentScene);
	}

	void Start ()
	{
		
	}
	
	void Update ()
	{
		
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
		{
			NextScene();
		}
	}
}