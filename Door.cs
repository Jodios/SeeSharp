using Godot;
using Godot.Collections;

public partial class Door : AnimatableBody2D
{

	private AnimationPlayer animationPlayer;
	private CollisionShape2D collisionShape;
	private bool open = false;

	public override void _Ready()
	{
		AddToGroup(Global.Door);
		AddToGroup(Global.Persist);
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		if (open)
		{
			collisionShape.QueueFree();
			animationPlayer.Play("RESET_OPEN");
		}
	}

    public void OpenDoor()
	{
		open = true;
		animationPlayer.Play("open");
		animationPlayer.AnimationFinished += (name) =>
		{
			collisionShape.QueueFree();
		};
	}

	public Dictionary<string, Variant> Save()
	{
		return new Dictionary<string, Variant>(){
			{ "Filename", SceneFilePath },
			{ "Parent", GetParent().GetPath() },
			{ "Position", Position },
			{ "open", open },
			{ "rotation", Rotation },
		};
	}

}
