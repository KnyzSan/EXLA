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
	public bool IsCrounching = false;
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
	}

	void BildWishVelocity()
	{
		WishVelocity = 0;

		var rot = Head.Transform.Rotation;
		if(Input.Down("Forward")) WishVelocity += rot.Forward;
		if(Input.Down("Backward")) WishVelocity += rot.Backward;
		if(Input.Down("Left")) WishVelocity += rot.Left;
		if(Input.Down("Right")) WishVelocity += rot.Right;

		WishVelocity = WishVelocity.WithZ(0);
		if(!WishVelocity.IsNearZeroLength) WishVelocity = WishVelocity.Normal;

		if (IsCrounching) WishVelocity *= CrouchSpeed;
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
			characterController.Velocity += gravity *Time.Delta * 0.5f;
			characterController.Accelerate(WishVelocity.ClampLength(MaxForce));
			characterController.ApplyFriction(AirControl);
		}

		characterController.Move(); //https://youtu.be/5hDENSPlCts?si=ljScqgiM7QYGrj1K&t=875
	}
}
