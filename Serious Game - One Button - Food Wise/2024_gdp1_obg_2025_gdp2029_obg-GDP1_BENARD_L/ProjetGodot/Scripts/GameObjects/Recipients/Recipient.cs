using Godot;
using System;
using System.Threading;

//Author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
    public partial class Recipient : Node2D
    {
        [Export] public int nIngredients = 1;
        protected ColorRect currentContent;


        public static Recipient LoadRecipient(PackedScene pScene, Node pContainer, Vector2 pPos)
        {
            Recipient lRecipient = pScene.Instantiate<Recipient>();
            lRecipient.GlobalPosition = pPos;
            pContainer.AddChild(lRecipient);
            return lRecipient;
        }


        public virtual void AddNewContent(Color pColor)
        {
            ColorRect lContent = new ColorRect();
            lContent.Color = pColor;
            currentContent = lContent;
            
            AddChild(lContent);
            
        }
    }
}
