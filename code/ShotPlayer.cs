using Sandbox;
using Sandbox.Citizen;

public sealed class ShotPlayer : Component
{
	[Property][Category("Компонент")] public GameObject Camera {get;set;}
	[Property][Category("Компонент")] public Vector3 EyePosition {get;set;}
	[Property][Category("Компонент")] public CharacterController Controller {get;set;}
	[Property][Category("Компонент")] public CitizenAnimationHelper Animator {get;set;}

	[Property][Category("Показатели")] public float WalkSpeed = 125f;
	[Property][Category("Показатели")] public float RunSpeed = 250f;
	[Property][Category("Показатели")] public float JumpStrength = 400f;

	public Angles EyeAngles {get; set;}
	Transform _initialCameraTransform;

	protected override void DrawGizmos()
	{
		Gizmo.Draw.LineSphere(EyePosition, 10f);
	}
	protected override void OnUpdate()
	{
		EyeAngles += Input.AnalogLook;
		EyeAngles = EyeAngles.WithPitch(MathX.Clamp(EyeAngles.pitch, -60f, 70f));
		Transform.Rotation = Rotation.FromYaw(EyeAngles.yaw);

		if(Camera != null)
			Camera.Transform.Local = _initialCameraTransform.RotateAround(EyePosition,EyeAngles.WithYaw(0f));
	}
	protected override void OnFixedUpdate()
	{
		if(Controller == null) return;

		var wishSpeed = Input.Down("Run") ? RunSpeed : WalkSpeed;
		var wishVelocity = Input.AnalogMove.Normal * wishSpeed * Transform.Rotation;

		Controller.Accelerate(wishVelocity);

		if(Controller.IsOnGround)
		{
			Controller.ApplyFriction(4f);
			Controller.Acceleration = 10f;

			if(Input.Pressed("Jump"))
			{
				Controller.Punch(Vector3.Up * JumpStrength);
				if(Animator != null)
					Animator.TriggerJump();
			}
		}
		else
		{
			Controller.Acceleration = 0.1f;
			Controller.Velocity += Scene.PhysicsWorld.Gravity * Time.Delta;
		}

		Controller.Move();
		
		if(Animator != null)
		{
			Animator.IsGrounded = Controller.IsOnGround;
			Animator.WithVelocity(Controller.Velocity);
		}
	}

	protected override void OnStart()
	{
		if(Camera != null)
			_initialCameraTransform = Camera.Transform.Local;

			if(Components.TryGet<SkinnedModelRenderer>(out var model))
			{
				var clothing = ClothingContainer.CreateFromLocalUser();
				clothing.Apply(model);
			}
	}
}
