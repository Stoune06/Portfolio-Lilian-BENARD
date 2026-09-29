using Godot;
using System;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;

// Author : Guillaume Julia

namespace Com.IsartDigital.ProjectName
{
	public partial class ScreenManager : Manager
	{
        #region Singleton
        static private ScreenManager instance;
        private ScreenManager() { }

        static public ScreenManager GetInstance()
        {
            if (instance == null) instance = new ScreenManager();
            return instance;
        }

        #endregion
        public Vector2 screenSize { get; private set; }
		public Vector2 anchorPoint { get; private set; }

        private const string ANDROID_VERSION = "Android";
        private const string IOS_VERSION = "IOS";
        public override void _Ready()
        {
            base._Ready();
            #region Singleton
            if (instance != null)
            {
                QueueFree();
                GD.Print(nameof(ScreenManager) + "instance already exists, destroying the last added");
                return;
            }

            instance = this;
            #endregion
        }

        public override void Init()
		{
			screenSize = GetScreenSize();
			anchorPoint = Vector2.Zero;
        }

        private Vector2 GetScreenSize()
		{
			return DisplayServer.WindowGetSize();
		}
        
        private bool DetectTablet()
		{
			return OS.GetName() == ANDROID_VERSION || OS.GetName() == IOS_VERSION;
		}

        protected override void Dispose(bool pDisposing)
        {
            instance = null;
            base.Dispose(pDisposing);
        }
    }
}
