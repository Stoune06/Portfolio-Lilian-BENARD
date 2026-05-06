using Com.IsartDigital.ProjectName;
using Godot;
using System;


namespace Com.IsartDigital.ProjectName
{
    public partial class Stove : Recipient
    {
        [Export] public Marker2D[] bakingPosition;
        public override void _Ready()
        {
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        }
    }
}

