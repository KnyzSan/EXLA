using Sandbox;

public sealed class CamereMovement : Component
{
	[Property] public PlayerMovement Player {get; set;}
	[Property] public GameObject Head {get; set;}
	[Property] public GameObject Body {get; set;}
	[Property] public float Distance {get; set;} = 0f;

	public bool IsFirstPerson => Distance == 0f;

	private Vector3 CurrentOffset = Vector3.Zero;
	private CameraComponent Camera;
	private ModelRenderer BodyRenderer;

	protected override void OnAwake()
	{
		Camera = Components.Get<CameraComponent>();
		BodyRenderer = Body.Components.Get<ModelRenderer>();
	}
	
	protected override void OnUpdate()
	{
		var eyeAngles = Head.Transform.Rotation.Angles();
		eyeAngles.pitch += Input.MouseDelta.y * 0.1f;
		eyeAngles.yaw -= Input.MouseDelta.x * 0.1f;
		eyeAngles.roll = 0f;
		eyeAngles.pitch = eyeAngles.pitch.Clamp(-89.9f, 89.9f);
		Head.Transform.Rotation = eyeAngles.ToRotation();

		var targetOffset = Vector3.Zero;
		if(Player.IsCrouching) targetOffset += Vector3.Down *32f;
		CurrentOffset = Vector3.Lerp(CurrentOffset, targetOffset, Time.Delta *10f);

		if(Camera is not null)
		{
			var camPos = Head.Transform.Position + CurrentOffset;
			if(!IsFirstPerson)
			{
				var camForward = eyeAngles.ToRotation().Forward;
				var camTrace = Scene.Trace.Ray(camPos,camPos - (camForward * Distance))
				.WithoutTags("player").Run();

				if (camTrace.Hit)
				{
					camPos = camTrace.HitPosition + camTrace.Normal;
				}
				else
				{
					camPos = camTrace.EndPosition;
				}
				BodyRenderer.RenderType = ModelRenderer.ShadowRenderType.On;
			}
			else
			{
				BodyRenderer.RenderType = ModelRenderer.ShadowRenderType.ShadowsOnly;
			}

			Camera.Transform.Position = camPos;
			Camera.Transform.Rotation = eyeAngles.ToRotation();
		}
	}
}
