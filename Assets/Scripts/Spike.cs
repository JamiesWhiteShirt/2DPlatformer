using UnityEngine;
using System.Collections;

public class Spike : MonoBehaviour
{
	void OnCollisionEnter2D(Collision2D collision)
	{
		if (PlayerController.IsMe(collision.gameObject))
		{
			PlayerController.Kill();
		}
	}
}