using Com.IsartDigital.ProjectName;
using Godot;
using System;

public partial class InputManager : Node2D
{
	public static bool isTouching = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
		if(!GameManager.GetInstance().isPlaying) return;
		if(@event is InputEventScreenTouch lInputEventTouch)
		{
			if(lInputEventTouch.Pressed)
			{
				isTouching = true;
			}
		}
        if (@event is InputEventMouseButton lInputMouse)
        {
            if (lInputMouse.ButtonIndex == MouseButton.Left)
            {
				if(lInputMouse.Pressed) isTouching = true;
				else isTouching = false;
            }
        }
    }
}
