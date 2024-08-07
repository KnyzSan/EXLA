using Sandbox;
using Sandbox.Citizen;

public sealed class PlayerMovement : Component
{
	[Property] public float GroundControl = 4.0f;
	[Property] public float AirControl = 0.1f;
	[Property] public float MaxForce = 50f;
	[Property] public float Speed = 150f;
	[Property] public float RunSpeed = 310f;
	[Property] public float CrouchSpeed = 110f;
	[Property] public float JumpForce = 450f;

	[Property] public GameObject Head{get; set;}	//getter и setter
	[Property] public GameObject Body{get; set;}	//обязательны БЛЯТЬ!!!

	public Vector3 WishVelocity = Vector3.Zero;
	public bool IsCrouching = false;
	public bool IsSprinting = false;

	private CharacterController characterController;
	private CitizenAnimationHelper animationHelper;
	
	protected override void OnAwake()
	{
		characterController = Components.Get<CharacterController>();
		animationHelper = Components.Get<CitizenAnimationHelper>();
	}

	protected override void OnUpdate()
	{
		UpdateCrouching();
		IsSprinting = Input.Down("Run");
		if(Input.Pressed("Jump")) Jump();

		UpdateAnimations();
	}

	protected override void OnFixedUpdate()
	{
		BuildWishVelocity();
		RotateBody();
		Move();
	}

	void BuildWishVelocity()
	{
		WishVelocity = 0;

		var rot = Head.Transform.Rotation;
		if(Input.Down("Forward")) WishVelocity += rot.Forward;
		if(Input.Down("Backward")) WishVelocity += rot.Backward;
		if(Input.Down("Left")) WishVelocity += rot.Left;
		if(Input.Down("Right")) WishVelocity += rot.Right;

		WishVelocity = WishVelocity.WithZ(0);
		if(!WishVelocity.IsNearZeroLength) WishVelocity = WishVelocity.Normal;

		if (IsCrouching) WishVelocity *= CrouchSpeed;
		else if (IsSprinting) WishVelocity *= RunSpeed;
		else WishVelocity *= Speed;
	}

	void Move()
	{
		var gravity = Scene.PhysicsWorld.Gravity;

		if(characterController.IsOnGround)
		{
			characterController.Velocity = characterController.Velocity.WithZ(0);
			characterController.Accelerate(WishVelocity);
			characterController.ApplyFriction(GroundControl);
		}
		else
		{
			characterController.Velocity += gravity * Time.Delta * 0.5f;
			characterController.Accelerate(WishVelocity.ClampLength(MaxForce));
			characterController.ApplyFriction(AirControl);
		}

		characterController.Move(); 

		if(!characterController.IsOnGround)
		{
			characterController.Velocity += gravity * Time.Delta * 0.5f;
		}
		else
		{
			characterController.Velocity = characterController.Velocity.WithZ(0);
		}
	}

	void RotateBody()
	{
		if(Body is null) return;

		var targetAngle = new Angles(0, Head.Transform.Rotation.Yaw(), 0).ToRotation();
		float rotateDifference = Body.Transform.Rotation.Distance(targetAngle);
		
		if(rotateDifference > 50f || characterController.Velocity.Length > 10f)
		{
			Body.Transform.Rotation = Rotation.Lerp(Body.Transform.Rotation, targetAngle, Time.Delta * 2f);
		}
	}

	void Jump()
	{
		if(!characterController.IsOnGround) return;

		characterController.Punch( Vector3.Up * JumpForce);
		animationHelper?.TriggerJump();
	}

	void UpdateAnimations()
	{
		if(animationHelper is null) return;

		animationHelper.WithWishVelocity(WishVelocity);
		animationHelper.WithVelocity(characterController.Velocity);
		animationHelper.AimAngle = Head.Transform.Rotation;
		animationHelper.IsGrounded = characterController.IsOnGround;
		animationHelper.WithLook(Head.Transform.Rotation.Forward, 1f, 0.75f, 0.5f);
		animationHelper.MoveStyle = CitizenAnimationHelper.MoveStyles.Run;
		animationHelper.DuckLevel = IsCrouching? 1f : 0f;
	}

	void UpdateCrouching()
	{
		if(characterController is null) return;

		if(Input.Pressed("Crouch") && !IsCrouching)
		{
			IsCrouching = true;
			characterController.Height /= 2f;
		}
		if(Input.Released("Crouch") && IsCrouching)
		{
			IsCrouching = false;
			characterController.Height *= 2f;
		}
	}
}
