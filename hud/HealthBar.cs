using Godot;

public partial class HealthBar : Node2D
{
	Timer visibilityTimer;
	TextureProgressBar healthBar;
	Node2D parent;
	float previousHealth;
	float previousMaxHealth;

	public override void _Ready()
	{
		Visible = false;
		healthBar = GetNode<TextureProgressBar>("TextureProgressBar");
		visibilityTimer = GetNode<Timer>("VisibilityTimer");
		visibilityTimer.OneShot = true;
		visibilityTimer.Timeout += () =>
		{
			Visible = false;
		};
		parent = (Node2D)GetParent();
	}

	public override void _Process(double delta)
	{
		float currentMaxHealth = (float)parent.Get("MaxHealth");
		float currentHealth = (float)parent.Get("health");
		if (currentHealth != previousHealth || currentMaxHealth != previousMaxHealth)
		{
			healthBar.MaxValue = currentMaxHealth;
			healthBar.Value = currentHealth;
			Visible = true;
		}
		else
		{
			if(visibilityTimer.IsStopped() && Visible == true){
				visibilityTimer.Start();
			}
		}
		previousHealth = currentHealth;
		previousMaxHealth = currentMaxHealth;
	}

}
