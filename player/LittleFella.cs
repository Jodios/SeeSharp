using Godot;
using Godot.Collections;

public partial class LittleFella : CharacterBody2D
{
	[Export] public float MaxHealth { set; get; } = 100.00f;
	[Export] public float Speed = 2;
	[Export] public const float JumpAmount = 10;
	private AnimationPlayer animationPlayer;
	private Sprite2D player;
	private PackedScene projectileScene = GD.Load<PackedScene>("res://projectile.tscn");
	private float health;
	private int level = 1;
	[Signal] public delegate void LittleFellaPerishedEventHandler();
	private Global global;
	public bool teleported;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		AddToGroup(Global.Persist);
		AddToGroup(Global.Player);
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		player = GetNode<Sprite2D>("Little-feller");
		animationPlayer.Play("idle", customSpeed: 2);
		health = MaxHealth;
	}

	public override void _PhysicsProcess(double delta)
	{
		HandleMovement(delta);
		HandleShootingAction();
		MoveAndSlide();
	}

	public void Damage(float damage)
	{
		health -= damage;
		if (health <= 0)
		{
			Die();
		}
	}

	public Dictionary<string, Variant> Save()
	{
		return new Godot.Collections.Dictionary<string, Variant>(){
			{ "Filename", SceneFilePath },
			{ "Parent", GetParent().GetPath() },
			{ "Position", Position },
			{ "Speed", Speed },
			{ "MaxHealth", MaxHealth },
			{ "health", health },
			{ "level", level },
		};
	}

	public void Load() { }

	private void Die()
	{
		Speed = 0;
		// would play the dying animation and what not here
		QueueFree();
		// creating custom signals to do stuff
		// when player dies. 
		EmitSignal("LittleFellaPerished");
	}

	private void HandleShootingAction()
	{
		if (Input.IsActionJustPressed("shoot"))
		{
			Projectile projectile = (Projectile)projectileScene.Instantiate();
			int padding = player.FlipH ? -5 : 5;
			float projectileRotation = Mathf.DegToRad(player.FlipH ? 180 : 0);
			projectile.Start(GlobalPosition + new Vector2(padding, 2), projectileRotation, this);
			GetTree().Root.AddChild(projectile);
		}
	}

	private void HandleMovement(double delta)
	{
		float X = Velocity.X;
		float Y = Velocity.Y;
		if (!IsOnFloor())
		{
			Y += global.GRAVITA * (float)delta;
		}
		else if (Input.IsActionJustPressed("jump"))
		{
			Y = -(JumpAmount*(float)delta*500);
		}

		float direction = Input.GetAxis("left", "right");
		X = direction * Speed * (float)delta * 500;
		if (direction == -1)
		{
			player.FlipH = true;
		}
		else if (direction == 1)
		{
			player.FlipH = false;
		}
		// Velocity = direction * (Speed * (float)delta * 800);
		Velocity = new Vector2(X, Y);

	}

}
