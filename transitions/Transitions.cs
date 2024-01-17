using System.Runtime;
using Godot;

/**
    reference: https://www.youtube.com/watch?v=yZQStB6nHuI
**/

public partial class Transitions : Node
{

    AnimationPlayer transitionPlayer;

    public override void _Ready()
    {
        transitionPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public void Dissolve(string target)
    {
        {
		    GetTree().CallDeferred("change_scene_to_file", target);
            transitionPlayer.PlayBackwards("dissolve");
        };
    }

}
