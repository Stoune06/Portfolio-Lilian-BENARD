using Com.IsartDigital.ProjectName;
using Godot;
using System;

public partial class Water : HoldType
{
    private bool isPlaying = false;
    public override void _Process(double delta)
    {
        base._Process(delta);
        if (InputManager.isTouching && canPlay && !isPlaying)
        {
            AudioManager.PlaySound(SoundsList.BOILING, true);
            isPlaying = true;
        }
    }
}
