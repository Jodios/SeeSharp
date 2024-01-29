using Godot;
using System;

public partial class Teleporter : Node2D
{
	[Export] public bool flip = false;
	[Export] public Teleporter target;
	private Area2D teleportationArea;
	private AnimationPlayer animationPlayer;
	private Sprite2D sprite;

	public override void _Ready()
	{
		// TODO: will add more animations for the teleporter 
		// and select a random one here so it looks more natural.
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayer.Play("teleporter");
		teleportationArea = GetNode<Area2D>("TeleportationArea");
		teleportationArea.BodyEntered += handleTeleportation;
		teleportationArea.BodyExited += handleTeleportationExit;
		sprite = GetNode<Sprite2D>("TeleportationAnimation");
		sprite.FlipH = flip;
	}

	private void handleTeleportation(Node2D body)
	{
		if (!body.IsInGroup(Global.Player) || target == null) return;
		LittleFella littleFella = (LittleFella)body;
		if (littleFella.teleported) return;
		littleFella.teleported = true;
		littleFella.Position = target.Position;
	}

	private void handleTeleportationExit(Node2D body)
	{
		if (!body.IsInGroup(Global.Player)) return;
		LittleFella littleFella = (LittleFella)body;
		littleFella.teleported = false;
	}

}
