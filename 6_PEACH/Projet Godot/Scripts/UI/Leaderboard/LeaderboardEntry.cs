using Godot;
using System;

// Author : Hector Rosier

namespace Com.IsartDigital.ProjectName {
	
	public partial class LeaderboardEntry : HBoxContainer
	{
		[Export] private Label name;
		[Export] private Label score;

		public string playerName = "";
		public string playerScore = "";

		public override void _Ready()
		{
			UpdateEntry(); //Because HttpLeaderboardRequestCompleted() in LeaderBoard
        }

		private void UpdateEntry()
		{
			name.Text = playerName;
			score.Text = playerScore;
		}
	}
}
