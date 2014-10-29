using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	public float topSpeed = 10.0f;
	public float jumpSpeed = 10.0f;
	public float wallJumpPower = 1.0f;
	public float wallSlideSpeed = 1.0f;
	public float deadlyMomentum = 5.0f;

	private Vector2 spawnPos;
	private bool facingRight = true;
	private bool gravityDown = true;
	private Vector3 desiredScale = new Vector3(1.0f, 1.0f, 1.0f);

	private Transform upward;
	private Transform downward;
	private Transform forward;
	private Transform backward;
	private int terrainLayerMask;

	private GameObject animatedChild;
	private Animator animator;

	public static PlayerController me;

	public static bool IsMe(GameObject obj)
	{
		return me.gameObject == obj;
	}

	public static void Kill()
	{
		me.transform.position = me.spawnPos;
		me.rigidbody2D.velocity = new Vector2(0.0f, 0.0f);
	}

	void Start()
	{
		spawnPos = transform.position;
		me = this;

		animatedChild = transform.FindChild("Teslaboy_Anim").gameObject;
		animator = animatedChild.GetComponent<Animator>();

		upward = transform.FindChild("Upward");
		downward = transform.FindChild("Downward");
		forward = transform.FindChild("Forward");
		backward = transform.FindChild("Backward");
		terrainLayerMask = LayerMask.GetMask("Terrain");
	}

	void Update()
	{
		bool isStanding = touchingGround();
		bool facingWall = touchingWall();

		if (isStanding) animator.SetBool("JumpingFromWall", false);

		if (Input.GetButtonDown("Jump"))
		{
			Vector3 vel;
			if (facingWall && !isStanding)
			{
				vel = new Vector2(facingRight ? -wallJumpPower : wallJumpPower, gravityDown ? 1.0f : -1.0f);
				vel = vel.normalized * jumpSpeed;

				updateFacing(!facingRight);
				animator.SetBool("JumpingFromWall", true);
			}
			else if (isStanding)
			{
				vel = new Vector2(rigidbody2D.velocity.x, gravityDown ? jumpSpeed : -jumpSpeed);
				animator.SetBool("OnWall", !isStanding && facingWall);
			}
			else
			{
				vel = rigidbody2D.velocity;
			}
			rigidbody2D.velocity = vel;
		}

		animator.SetBool("Jumping", !isStanding);
		animator.SetFloat("Speed", Mathf.Abs(rigidbody2D.velocity.x));
		animator.SetBool("OnWall", !isStanding && facingWall);

		if (animator.GetCurrentAnimatorStateInfo(0).nameHash == Animator.StringToHash("Base Layer.Running"))
		{
			animator.speed = Mathf.Abs(rigidbody2D.velocity.x / 4.0f);
		}
	}

	private bool checkSideForCollision(Transform side)
	{
		Vector2 v = side.position - transform.position;
		return Physics2D.Raycast(transform.position, v.normalized, v.magnitude, terrainLayerMask);
	}

	private void updateFacing(bool facingRight)
	{
		this.facingRight = facingRight;
		desiredScale.x = facingRight ? 1.0f : -1.0f;
	}

	private bool touchingGround()
	{
		return Physics2D.OverlapArea(transform.position + new Vector3(-0.375f, gravityDown ? -0.5f : 0.5f, 0.0f), transform.position + new Vector3(0.375f, gravityDown ? -0.625f : 0.625f, 0.0f), terrainLayerMask) != null;
	}

	private bool touchingWall()
	{
		return facingRight ? checkSideForCollision(forward) : checkSideForCollision(backward);
	}
	
	void FixedUpdate()
	{
		float speed = Input.GetAxis("Horizontal") * topSpeed;
		float gravity = Input.GetAxis("Vertical");

		bool isStanding = touchingGround();
		bool facingWall = touchingWall();

		if (isStanding)
		{
			if (speed != 0.0f)
			{
				updateFacing(speed > 0.0f);
			}
		}
		else
		{
			float vx = rigidbody2D.velocity.x;
			if (Mathf.Abs(vx) > 0.05f)
			{
				updateFacing(vx > 0.0f);
			}
		}
		
		if (gravity != 0.0f)
		{
			gravityDown = gravity < 0.0f;
			desiredScale.y = gravityDown ? 1.0f : -1.0f;
			Physics2D.gravity = new Vector2(0.0f, gravityDown ? -30.0f : 30.0f);
		}

		float x = rigidbody2D.velocity.x + speed * Time.deltaTime * 3.0f;
		float y = rigidbody2D.velocity.y;

		if (speed != 0.0f && (speed < 0.0 ^ facingRight) && facingWall)
		{
			if (y < -wallSlideSpeed) y = (-wallSlideSpeed + y * 3) / 4.0f;
		}

		if (x > topSpeed) x = topSpeed;
		else if (x < -topSpeed) x = -topSpeed;

		rigidbody2D.velocity = new Vector2(x, y);

		animatedChild.transform.localScale = (animatedChild.transform.localScale * 2.0f + desiredScale) / 3.0f;
	}

	void OnCollisionStay2D(Collision2D collision)
	{
		Rigidbody2D other = collision.gameObject.rigidbody2D;
		
		if (other)
		{
			float f = (other.velocity.y - rigidbody2D.velocity.y) * other.mass;
			if (Mathf.Abs(f) > deadlyMomentum)
			{
				if (f > 0.0f ^ animatedChild.transform.localScale.y > 0.0f)
				{
					if (touchingGround())
					{
						Kill();
					}
				}
			}
		}
	}
}