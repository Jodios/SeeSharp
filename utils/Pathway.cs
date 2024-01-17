using System.Collections.Generic;
using Godot;

public partial class Pathway : Area2D
{
	[Export] public CollisionShape2D PathwayCollision;
	[Export] public Polygon2D SpawnPoint;
	[Export] public string File;
	[Export] public string TargetSpawnPath;

	// This signal is probably not needed but i'm leaving it
	// in case it comes in handy :) 
	[Signal] public delegate void PathwayEnteredEventHandler(string to);
	private Global global;
	private SaveGameUtils saveGameUtils;
	private Transitions transitions;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		saveGameUtils = GetNode<SaveGameUtils>("/root/SaveGameUtils");
		transitions = GetNode<Transitions>("/root/Transitions");
		SpawnPoint.Visible = false;
		// Adding a little bit of a cooldown for when player spawns 
		// into a collision shape.
		GetTree().CreateTimer(.1).Timeout += () => {
			this.BodyEntered += HandlePathwayEntered;
		};
	}

	public void HandlePathwayEntered(Node2D body)
	{
		if (body is not LittleFella) return;
		global.TargetPathToSpawnIn = TargetSpawnPath;
		EmitSignal("PathwayEntered", File);
		saveGameUtils.SaveGame();
		transitions.Dissolve(File);
	}

}
