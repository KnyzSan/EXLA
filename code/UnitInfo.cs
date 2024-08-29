using Sandbox;
using System;

public enum UnitType
{
	None,
	Player,
	Shot
}

public sealed class UnitInfo : Component
{
	[Property] 
	UnitType Team {get; set;}
	[Property]
	public float HealthRegen { get; set; } = 0.5f;
	[Property]
	public float HealthRegenTimer { get; set; } = 3f;
	[Property]
	public float HealthRegenAmount { get; set; }
	[Property]
	public float MaxHealth {get; set;} = 5f;
	public	float Health { get; private set;}

	public bool Alive { get; private set; } = true;

	TimeSince _lastDamage;
	TimeUntil _nextHeal;

	protected override void OnUpdate()
	{
		if(_lastDamage >= HealthRegenTimer && Health != MaxHealth && Alive)
		{
			if(_nextHeal)
			{
				Damage( -HealthRegenAmount );
				_nextHeal = 1f;
			}
		}
	}
	protected override void OnStart()
	{
		Health = MaxHealth;
	}
	public void Damage (float damage)
	{
		if (!Alive) return;

		Health = Math.Clamp(Health - damage, 0f, MaxHealth);

		if ( damage > 0 )
			_lastDamage = 0f;

		if ( Health <= 0 )
			Krill();
	}
	public void Krill()
	{
		Health = 0f;
		Alive = false;
		GameObject.Destroy();
	}
}
