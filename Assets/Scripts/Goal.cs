using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Goal : MonoBehaviour
{
	private static int currrentScene;
	public static Goal currentGoal;

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
		currentGoal = this;
	}
	
	void Update ()
	{
		
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (PlayerController.IsMe(other.gameObject))
		{
			PlayerController.Complete();
		}
	}
}