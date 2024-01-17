using Godot;
using Godot.Collections;

public partial class Enemy : CharacterBody2D
{
	[Export] public float Speed = 6.0f;
	[Export] public float AggroSpeed = 8.0f;
	private float speed;

	[Export] public double MaxHealth { set; get; } = 100.00;

	[Export] public float damage = 10f;

	[Export] public float AttackCooldownSeconds = 0.3f;

	private AnimationPlayer animationPlayer;
	private double health;
	private Sprite2D enemy;
	private RayCast2D ledgeDetection;
	private Area2D hitbox;
	private Area2D aggroArea;
	private int level = 1;
	private int direction = 1;

	private Global global;

	private Node2D target;

	private Timer changeDirectionTimer;
	private Timer attackCoolDownTimer;
	private Timer aggroTimer;

	[Signal] public delegate void EnemyPerishedEventHandler();

	public override void _Ready()
	{
		speed = Speed;
		global = GetNode<Global>("/root/Global");
		AddToGroup(Global.Persist);
		AddToGroup(Global.Enemy);

		hitbox = GetNode<Area2D>("AttackBox");
		hitbox.BodyEntered += HandleHitBoxCollision;
		hitbox.BodyExited += HandleHitBoxExited;

		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayer.Play("idle", customSpeed: 2);
		enemy = GetNode<Sprite2D>("Enemy");
		enemy.Modulate = global.PRIMARY_COLOR;
		ledgeDetection = GetNode<RayCast2D>("Enemy/ledgeDetection");
		health = MaxHealth;


		attackCoolDownTimer = GetNode<Timer>("AttackCoolDownTimer");
		attackCoolDownTimer.WaitTime = AttackCooldownSeconds;
		attackCoolDownTimer.Timeout += HandleAttackCooldownTimeout;

		// setting up aggro logic
		aggroTimer = GetNode<Timer>("AggroTimer");
		aggroTimer.Timeout += UnsetAggro;
		aggroArea = GetNode<Area2D>("AggroArea");
		aggroArea.BodyEntered += HandleAggroAreaEntered;
		aggroArea.BodyExited += HandleAggroAreaExited;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (IsOnWall() || !ledgeDetection.IsColliding())
		{
			direction *= -1;
			enemy.Scale *= new Vector2(-1, 1);
		}

		if (target is not null)
		{
			if (target.Position.X > Position.X)
			{
				direction = 1;
			}
			else
			{
				direction = -1;
			}
		}
		if (!IsOnFloor())
		{
			Velocity = new Vector2(
				0,
				Velocity.Y + global.GRAVITA * (float)delta
			);
		}
		else
		{
			Velocity = new Vector2(
				direction * speed * (float)delta * 100,
				0
			);
		}
		MoveAndSlide();
	}

	public void Damage(double damage)
	{
		health -= damage;
		enemy.Modulate = global.SECONDARY_COLOR;
		GetTree().CreateTimer(.05).Timeout += () =>
		{
			enemy.Modulate = global.PRIMARY_COLOR;
		};
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
			{ "Speed", speed },
			{ "MaxHealth", MaxHealth },
			{ "health", health },
			{ "level", level },
			{ "damage", damage },
		};
	}

	private void Die()
	{
		EmitSignal("EnemyPerished");
		speed = 0;
		QueueFree();
	}

	private void HandleHitBoxCollision(Node2D body)
	{
		if (body.IsInGroup(Global.Player))
		{
			attackCoolDownTimer.Start();
		}
	}

	private void HandleHitBoxExited(Node2D body)
	{
		if (body is not LittleFella) return;
		attackCoolDownTimer.Stop();
	}

	private void HandleAggroAreaEntered(Node2D body)
	{
		if (!body.IsInGroup(Global.Player)) return;
		aggroTimer.Stop();
		// changeDirectionTimer.Stop();
		target = body;
		speed = AggroSpeed;
		((LittleFella)target).LittleFellaPerished += () =>
		{
			target = null;
		};
	}

	private void HandleAggroAreaExited(Node2D node)
	{
		if (target == node)
		{
			aggroTimer.Start();
		}
	}

	private void UnsetAggro()
	{
		target = null;
		speed = Speed;
	}

	private void HandleAttackCooldownTimeout()
	{
		((LittleFella)target).Damage(damage);
	}

}
