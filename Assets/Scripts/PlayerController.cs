using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	public float topSpeed = 10.0f;
	public float jumpSpeed = 10.0f;
	public float wallJumpPower = 1.0f;
	public int maxJumps = 2;
	public int maxJumpsFromWall = 1;
	
	private bool facingRight = true;
	private bool gravityDown = true;
	private Vector3 desiredScale = new Vector3(1.0f, 1.0f, 1.0f);
	private int jump = 0;

	private Transform upward;
	private Transform downward;
	private Transform forward;
	private Transform backward;
	private int terrainLayerMask;
	private bool jumpedThisTick = false;

	private Animator animator;

	void Start()
	{
		upward = transform.FindChild("Upward");
		downward = transform.FindChild("Downward");
		forward = transform.FindChild("Forward");
		backward = transform.FindChild("Backward");
		terrainLayerMask = LayerMask.GetMask("Terrain");

		animator = GetComponent<Animator>();
	}

	void Update()
	{
		bool isStanding = checkSideForCollision(downward);

		jumpedThisTick = false;
		if (jump > 0 && Input.GetButtonDown("Jump"))
		{
			jumpedThisTick = true;

			Vector3 vel;
			if (checkSideForCollision(forward) && !isStanding)
			{
				vel = new Vector2(facingRight ? -wallJumpPower : wallJumpPower, gravityDown ? 1.0f : -1.0f);
				vel = vel.normalized * jumpSpeed;

				updateFacing(!facingRight);
			}
			else
			{
				vel = new Vector2(rigidbody2D.velocity.x, gravityDown ? jumpSpeed : -jumpSpeed);
			}
			rigidbody2D.velocity = vel;

			jump--;
		}

		animator.SetBool("Jumping", !isStanding);
		animator.SetFloat("Speed", rigidbody2D.velocity.x);

		Debug.Log(animator.GetCurrentAnimatorStateInfo(0).nameHash + " " + Animator.StringToHash("Base Layer.Running"));

		if (animator.GetCurrentAnimatorStateInfo(0).nameHash == Animator.StringToHash("Base Layer.Running"))
		{
			animator.speed = Mathf.Abs(rigidbody2D.velocity.x / 5.0f);
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
	
	void FixedUpdate()
	{
		float speed = Input.GetAxis("Horizontal") * topSpeed;
		float gravity = Input.GetAxis("Vertical");

		if (checkSideForCollision(downward))
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

		if (x > topSpeed) x = topSpeed;
		else if (x < -topSpeed) x = -topSpeed;

		rigidbody2D.velocity = new Vector2(x, rigidbody2D.velocity.y);

		transform.localScale = (transform.localScale * 2.0f + desiredScale) / 3.0f;

		if (!jumpedThisTick)
		{
			if (checkSideForCollision(downward))
			{
				jump = maxJumps;
			}
			else if (checkSideForCollision(forward))
			{
				jump = maxJumpsFromWall;
			}
		}
	}
}