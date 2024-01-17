using Godot;


public partial class Level : Node2D
{

	private int enemyCount = 0;
	private bool doorsOpened = false;

	public override void _Ready()
	{
		GetNode<SaveGameUtils>("/root/SaveGameUtils").LoadGame();
		enemyCount = GetTree().GetNodesInGroup(Global.Enemy).Count;
		foreach (var enemy in GetTree().GetNodesInGroup(Global.Enemy))
		{
			((Enemy)enemy).EnemyPerished += () =>
			{
				enemyCount--;
			};
		}
	}

	public override void _Process(double delta)
	{
		if (enemyCount == 0 && !doorsOpened)
		{
			foreach(var door in GetTree().GetNodesInGroup(Global.Door)){
				((Door)door).OpenDoor();
			}
			doorsOpened = true;
		}
	}

}
