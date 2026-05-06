using Com.IsartDigital.ProjectName;
using Godot;
using System;
using System.Security.Permissions;

//Author Lilian Benard
namespace Com.IsartDigital.ProjectName
{
    public partial class Pan : Recipient
    {
        [Export] private ColorRect bottomRect;
        [Export] private Marker2D contentPos;
      
        private bool isBoiling = false;
        public override void _Process(double delta)
        {
            ChangeSize(GameManager.GetInstance().count);
        }

        public void ChangeSize(int pSize)
        {
            currentContent.Size = new Vector2(currentContent.Size.X, pSize);
        }

        public override void AddNewContent(Color pColor)
        {
            base.AddNewContent(pColor);
            currentContent.Scale = new Vector2(1, -1);
            currentContent.Size = new Vector2(bottomRect.Size.X, 0);
            currentContent.Position = contentPos.Position;
        }
    }
}

