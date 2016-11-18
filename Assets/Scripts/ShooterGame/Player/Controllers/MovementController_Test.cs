using UnityEngine;

public class MovementController_Test : MonoBehaviour
{
	private Vector2 _velocity = Vector2.zero;

	private int xInput = 0f;

	private const float GRAVITY = 0.98f;
	private const float GROUND_SPEED  = 5f;

	private const Vector3 SCALE_LEFT  = new Vector3(1, 1, 1);
	private const Vector3 SCALE_RIGHT = new Vector3(-1, 1, 1);

	public void Update() 
	{
		this.GetInput();

		this.ApplyGravity();
		
		this.ApplyMovement();
		
		this.AlignToGround();
	}

	private void GetInput()
	{
		xInput = Input.GetAxisRaw("Horizontal");
	}
	private void ApplyGravity()
	{
		float deltaY = GRAVITY * Time.deltaTime;

		Ray middleRay = new Ray(position: this.transform.middle, direction: Vector2.down, length: deltaY);
		RaycastHit2D middleHit = Physics2D.Raycast(middleRay);

		if(middleHit != null) 
		{
			this.transform.position = middleHit.point;
			this.IsGrounded = true;
		}
		else
		{
			this.IsGrounded = false;
			this.transform.Translate(new Vector2(0f, deltaY));
		}

		
	}

	private void ApplyMovement() 
	{
		if(xInput != 0)
		{
			this.transform.Translate(this.transform.forward * this.xInput * GROUND_SPEED);
			this.transform.scale = xInput > 0 ? SCALE_RIGHT : SCALE_LEFT;
		}
	}

	private void AlignToGround() 
	{
		Ray leftRay = new Ray(position: this.transform.left, direction: Vector2.down, length: Mathf.Infinity);
		Ray middleRay = new Ray(position: this.transform.middle, direction: Vector2.down, length: Mathf.Infinity);
		Ray rightRay = new Ray(position: this.transform.right, direction: Vector2.down, length: Mathf.Infinity);

		RaycastHit2D leftHit = Physics2D.Raycast(leftRay);
		RaycastHit2D middleHit = Physics2D.Raycast(middleRay);
		RaycastHit2D rightHit = Physics2D.Raycast(rightRay);

		if(leftHit != null || rightHit != null) 
		{
			Vector3 hitDirection = rightHit.point - leftHit.point;
        	Vector3 groundNormal = Vector3.Cross(hitDirection, -Vector3.forward).normalized;

        	transform.up = groundNormal;
		}
		else if(middleHit != null) 
		{
			this.transform.up = middleHit.normal;
		}
		else 
		{
			this.transform.up = Vector2.up;
		}
	}
}
