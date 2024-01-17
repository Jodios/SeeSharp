using Godot;

public partial class Global : Node
{
    public Color PRIMARY_COLOR { get; set; } = new Color("#1a1914");
    public Color SECONDARY_COLOR { get; set; } = new Color("#879188");

    public float GRAVITA = (float)ProjectSettings.GetSetting("physics/2d/default_gravity") / 4;
    public const string Persist = "persist";
    public const string Door = "door";
    public const string Enemy = "enemy";
    public const string Player = "player";

    public string TargetPathToSpawnIn;

    public Node CurrentScene { get; set; }

    public override void _Ready()
    {
        Viewport root = GetTree().Root;
        CurrentScene = root.GetChild(root.GetChildCount() - 1);
    }


}