using Godot;
using System;

//Author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
	public partial class LevelManager : Node2D
	{
        #region Singleton
        static private LevelManager instance;
        private LevelManager() { }

        static public LevelManager GetInstance()
        {
            if (instance == null) instance = new LevelManager();
            return instance;
        }

        #endregion
        [Export] public PackedScene[] allLevels;
		[Export] public Level currentLevel;
        public int currentLevelIndex = 0;


        public override void _Ready()
        {
            #region Singleton
            if (instance != null)
            {
                QueueFree();
                GD.Print(nameof(LevelManager) + "instance already exists, destroying the last added");
                return;
            }

            instance = this;
            #endregion
            base._Ready();
        }

        public void SwitchLevel(int pLevelIndex, Node pContainer)
		{
            if(pLevelIndex >= allLevels.Length)
            {
                HUDManager.GetInstance().endScreen.Visible = true;
                return;
            }
			currentLevel.QueueFree();
			LoadLevel(pLevelIndex, pContainer);
            currentLevelIndex = pLevelIndex;
            HUDManager.GetInstance().gameHUD.UpdatePersonCount(currentLevel.personNumber);
            HUDManager.GetInstance().gameHUD.UpdateGCount(0);

        }

		public Level LoadLevel(int pLevelIndex, Node pContainer)
		{
			Level lLevel = allLevels[pLevelIndex].Instantiate<Level>();
			pContainer.AddChild(lLevel);
            currentLevel = lLevel;
			return lLevel;
		}

        protected override void Dispose(bool pDisposing)
        {
            instance = null;
            base.Dispose(pDisposing);
        }
    }
}
