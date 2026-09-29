using Godot;
using System;
using System.Collections.Generic;

// Author : Louis Robin

namespace Com.IsartDigital.ProjectName
{
	public partial class GameObject : Node2D
	{
        [Export] protected AnimatedSprite2D outRenderer;
        [Export] public Marker2D marker;

        protected static GameManager gameManager = GameManager.GetInstance();
        protected static float yOffset = -200;
        public Vector2 originPos = default;
        public Vector2 endPos = default;

        

        protected string currentState;
        public static bool isWalk;
        public static string LAST_STATE = "";
        public static string DEFAULT_STATE = "default";
        public static string IDLE_UP_STATE = "idle_up";
        public static string IDLE_DOWN_STATE = "idle_down";
        public static string IDLE_LEFT_STATE = "idle_left";
        public static string IDLE_RIGHT_STATE = "idle_right";
        public static string WALK_UP_STATE = "walk_up";
        public static string WALK_DOWN_STATE = "walk_down";
        public static string WALK_LEFT_STATE = "walk_left";
        public static string WALK_RIGHT_STATE = "walk_right";
        public static string PUSH_UP_STATE = "push_up";
        public static string PUSH_DOWN_STATE = "push_down";
        public static string PUSH_LEFT_STATE = "push_left";
        public static string PUSH_RIGHT_STATE = "push_right";
        public static string SF_UP_STATE = "sf_up";
        public static string SF_DOWN_STATE = "sf_down";
        public static string SF_LEFT_STATE = "sf_left";
        public static string SF_RIGHT_STATE = "sf_right";
        public static string STUNNED_UP_STATE = "stun_up";
        public static string STUNNED_DOWN_STATE = "stun_down";
        public static string STUNNED_LEFT_STATE = "stun_left";
        public static string STUNNED_RIGHT_STATE = "stun_right";
        public static string FALL_STATE = "fall";
        public override void _Ready()
        {
            base._Ready();
            originPos = Position;
        }

        public virtual void SetAnimation(string pState)
        {
            outRenderer.Stop();
            outRenderer.Play(pState);
            currentState = pState;
            
        }

        public virtual void Resize(Vector2 pSize)
        {
            Vector2 lRatio = pSize / outRenderer.SpriteFrames.GetFrameTexture(DEFAULT_STATE,0).GetWidth();
            outRenderer.Scale *= lRatio;
        }
    }
}
