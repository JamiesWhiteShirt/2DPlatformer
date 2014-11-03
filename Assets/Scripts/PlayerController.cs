using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	public float topSpeed = 10.0f;
	public float jumpSpeed = 10.0f;
	public float wallJumpPower = 1.0f;
	public float wallSlideSpeed = 1.0f;

	private Vector2 spawnPos;
	private bool facingRight = true;
	private bool gravityDown = true;
	private int gravityZoneLayerMask;

	private Transform upward;
	private Transform downward;
	private Transform forward;
	private Transform backward;
	private int terrainLayerMask;

	private Grabbable grabbedObject;
	private bool grabbedOnRight;
	private int grabLayerMask;

	private GameObject animatedChild;
	private Animator animator;

	public static PlayerController me;

	public static bool IsMe(GameObject obj)
	{
		return me.gameObject == obj;
	}

	public static void Kill()
	{
		if (me == null) return;
		me.setGrabbedObject(null);
		me.transform.position = me.spawnPos;
		me.rigidbody2D.velocity = new Vector2(0.0f, 0.0f);
	}

	void Start()
	{
		spawnPos = transform.position;
		me = this;

		gravityZoneLayerMask = LayerMask.GetMask("GravityZone");

		animatedChild = transform.FindChild("Teslaboy_Anim").gameObject;
		animator = animatedChild.GetComponent<Animator>();

		upward = transform.FindChild("Upward");
		downward = transform.FindChild("Downward");
		forward = transform.FindChild("Forward");
		backward = transform.FindChild("Backward");
		terrainLayerMask = LayerMask.GetMask("Terrain", "Lightbridge", "Reflector");

		grabLayerMask = LayerMask.GetMask("Terrain");
	}

	private void setGrabbedObject(Grabbable grabbable)
	{
		if (grabbedObject != null) grabbedObject.grabbedByPlayer = false;
		if (grabbable != null)
		{
			grabbable.grabbedByPlayer = true;
			grabbedOnRight = grabbable.transform.position.x > transform.position.x;
		}
		grabbedObject = grabbable;
	}

	void Update()
	{
		bool isStanding = touchingGround();
		bool facingWall = touchingWall();

		if (isStanding) animator.SetBool("JumpingFromWall", false);

		if (grabbedObject == null && Input.GetButtonDown("Jump"))
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

		if (Input.GetButtonDown("Grab"))
		{
			RaycastHit2D hit = simpleRay(getForwardTransform(), grabLayerMask);

			if (hit.collider != null)
			{
				Grabbable grabbable = hit.collider.gameObject.GetComponent<Grabbable>();
				if (grabbable.grabbableByPlayer)
				{
					setGrabbedObject(grabbable);
				}
			}
		}
		if (Input.GetButtonUp("Grab"))
		{
			setGrabbedObject(null);
		}

		if (grabbedObject != null)
		{
			Vector3 posDiff = grabbedObject.transform.position - transform.position;
			if (posDiff.magnitude > 1.0f)
			{
				setGrabbedObject(null);
			}
		}

		if (grabbedObject != null)
		{
			Vector2 otherPos = new Vector2(grabbedObject.transform.position.x, grabbedObject.transform.position.y);
			Vector2 thisPos = new Vector2(transform.position.x, transform.position.y);
			Vector2 desiredPosition = thisPos + rigidbody2D.velocity / 50.0f + new Vector2(grabbedOnRight ? 0.9375f : -0.9375f, (gravityDown ? 1.0f : -1.0f) / 64.0f);

			Vector2 force = (desiredPosition - otherPos).normalized * 5.0f;
			if ((desiredPosition - otherPos).magnitude < force.magnitude)
			{
				force = desiredPosition - otherPos;
			}

			grabbedObject.rigidbody2D.velocity = force * 50.0f;
		}

		animator.SetBool("Jumping", !isStanding);
		animator.SetFloat("Speed", Mathf.Abs(rigidbody2D.velocity.x));
		animator.SetBool("OnWall", !isStanding && facingWall);
		animator.SetBool("Grabbing", grabbedObject != null);

		int nameHash = animator.GetCurrentAnimatorStateInfo(0).nameHash;

		if (nameHash == Animator.StringToHash("Base Layer.Running"))
		{
			animator.speed = Mathf.Abs(rigidbody2D.velocity.x / 4.0f);
		}
		else if (nameHash == Animator.StringToHash("Base Layer.GrabbingRunning"))
		{
			animator.speed = Mathf.Abs(rigidbody2D.velocity.x / 3.0f);
		}
		else
		{
			animator.speed = 1.0f;
		}
	}

	private Vector3 getDesiredScale()
	{
		return new Vector3((grabbedObject == null ? facingRight : grabbedOnRight) ? 1.0f : -1.0f, gravityDown ? 1.0f : -1.0f, 1.0f);
	}

	private RaycastHit2D simpleRay(Transform side, int layerMask)
	{
		Vector2 v = side.position - transform.position;
		return Physics2D.Raycast(transform.position, v.normalized, v.magnitude, layerMask);
	}

	private bool checkSideForCollision(Transform side)
	{
		return simpleRay(side, terrainLayerMask).collider != null;
	}

	private void updateFacing(bool facingRight)
	{
		this.facingRight = facingRight;
	}

	private bool touchingGround()
	{
		return Physics2D.OverlapArea(transform.position + new Vector3(-0.375f, gravityDown ? -0.5f : 0.5f, 0.0f), transform.position + new Vector3(0.375f, gravityDown ? -0.625f : 0.625f, 0.0f), terrainLayerMask) != null;
	}

	private Transform getForwardTransform()
	{
		return facingRight ? forward : backward;
	}

	private bool touchingWall()
	{
		return checkSideForCollision(getForwardTransform());
	}

	private bool isInGravityZone()
	{
		Bounds bounds = GetComponent<BoxCollider2D>().bounds;

		return Physics2D.OverlapArea(bounds.min, bounds.max, gravityZoneLayerMask) != null;
	}
	
	void FixedUpdate()
	{
		float currentTopSpeed = topSpeed * (grabbedObject == null ? 1.0f : 0.5f);
		float speed = Input.GetAxis("Horizontal") * currentTopSpeed;
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
		
		if (gravity != 0.0f && isInGravityZone())
		{
			gravityDown = gravity < 0.0f;
			Physics2D.gravity = new Vector2(0.0f, gravityDown ? -30.0f : 30.0f);
		}

		float x = rigidbody2D.velocity.x + speed * Time.deltaTime * 3.0f;
		float y = rigidbody2D.velocity.y;

		if (grabbedObject == null && speed != 0.0f && (speed < 0.0 ^ facingRight) && facingWall)
		{
			if (y < -wallSlideSpeed) y = (-wallSlideSpeed + y * 3) / 4.0f;
		}

		if (x > currentTopSpeed) x = currentTopSpeed;
		else if (x < -currentTopSpeed) x = -currentTopSpeed;

		rigidbody2D.velocity = new Vector2(x, y);

		animatedChild.transform.localScale = (animatedChild.transform.localScale * 2.0f + getDesiredScale()) / 3.0f;
	}

	void OnCollisionStay2D(Collision2D collision)
	{
		Rigidbody2D other = collision.gameObject.rigidbody2D;
		
		if (other != null)
		{
			if (touchingGround())
			{
				Bounds bounds1 = GetComponent<BoxCollider2D>().bounds;
				Bounds bounds2 = collision.gameObject.GetComponent<BoxCollider2D>().bounds;

				if (bounds1.min.x < bounds2.max.x && bounds1.max.x > bounds2.min.x && (gravityDown ^ transform.position.y > other.transform.position.y))
				{
					Kill();
				}
			}
		}
	}
}