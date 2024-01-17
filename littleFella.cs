using Godot;
using System;

public partial class littleFella : CharacterBody2D
{
	int speed = 20;

	AnimationPlayer animationPlayer = null; 
	Sprite2D player = null;
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		player = GetNode<Sprite2D>("Little-feller");
		animationPlayer.Play("idle", customSpeed: 2);
	}

    public override void _PhysicsProcess(double delta)
    {
		Vector2 position = Position;
		if(Input.IsActionPressed("up")){
			position.Y -= speed * (float)delta;
		}
		if(Input.IsActionPressed("down")){
			position.Y += speed * (float)delta;
		}
		if(Input.IsActionPressed("left")){
			position.X -= speed * (float)delta;
			player.FlipH = true;
		}
		if(Input.IsActionPressed("right")){
			position.X += speed * (float)delta;
			player.FlipH = false;
		}
		Position = position;

		MoveAndSlide();
    }

    public override void _Input(InputEvent @event)
    {
    }


}
