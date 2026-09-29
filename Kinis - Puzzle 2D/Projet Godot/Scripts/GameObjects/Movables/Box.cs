using Com.IsartDigital.Kinisi;
using Godot;
using System;

// Author : Louis Robin

namespace Com.IsartDigital.ProjectName
{

    public partial class Box : Movable
    {
        [Export] Sprite2D flame;

        public bool isBoxOnTarget = false;
        private const string ACTIVATED_STATE = "activated";

        public override void _Ready()
        {
            base._Ready();
            MovementFinished += CheckActivation;
        }

        public void CheckActivation() //sets the isOnTarget variable and set the associated animation to this box
        {
            if (toDelete) return;
            Vector2I lIndex = GridManager.GetInstance().GetIndexOnGrid(this);
            bool lIsBoxOnTarget = GridManager.GetInstance().targetList[lIndex.X][lIndex.Y] is Target;

            if (lIsBoxOnTarget)
            {
                ActivateBox();
            } 
            else DeactivateBox();
            
        }


        public void ActivateBox()
        {
            if (!isBoxOnTarget)
            {
                isBoxOnTarget = true;
                GameManager.activatedTargetsNum++;
                gameManager.CheckTargetsActivation();
                SetAnimation(ACTIVATED_STATE);
                //changement d'état graphique pour montrer que la boîte est sur le bon emplacement
            }
        }

        public void DeactivateBox()
        {
            if (isBoxOnTarget)
            {
                isBoxOnTarget = false;
                SetAnimation(DEFAULT_STATE);
                //changement d'état graphique pour montrer que la boîte n'est pas sur le bon emplacement
            }

        }

        public override void MoveObject(Vector2 pEndPosition, float pDuration = 0.2F)
        {
            SlideSFX();
            if (isBoxOnTarget) GameManager.activatedTargetsNum--;
            base.MoveObject(pEndPosition, pDuration);
        }

        public override void SetAnimation(string pState)
        {
            flame.Visible = true;
            Vector2 lStartScale;
            Vector2 lEndScale;
            Tween lTween = CreateTween();
            if (isBoxOnTarget)
            {
                lStartScale = Vector2.Zero;
                lEndScale = Vector2.One * 0.8f;
                lTween.Finished += () =>
                {
                    base.SetAnimation(pState);
                    flame.Visible = false;
                    return;
                };
            }
            else
            {
                lStartScale = Vector2.One;
                lEndScale = Vector2.Zero;
                base.SetAnimation(pState);
            }
            lTween.TweenProperty(flame, TweenProp.SCALE, lEndScale, 0.2f).From(lStartScale);
        }

        private void SlideSFX()
        {
            switch (GD.RandRange(0, 2))
            {
                case 0:
                    AudioManager.PlaySound("BoxSlide", true);
                    break;
                case 1:
                    AudioManager.PlaySound("BoxSlide2", true);
                    break;
                case 2:
                    AudioManager.PlaySound("BoxSlide3", true);
                    break;
                
            }
        }

        protected override void Dispose(bool disposing)
        {
            MovementFinished -= CheckActivation;
            base.Dispose(disposing);
        }
    }
}
