using Sandbox;
using Sandbox.Internal;

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

	public Vector3 Velocity = Vector3.Zero;
	public bool IsCrounching = false;
	public bool IsSprinting = false;

	
	protected override void OnUpdate()
	{

	}
}
