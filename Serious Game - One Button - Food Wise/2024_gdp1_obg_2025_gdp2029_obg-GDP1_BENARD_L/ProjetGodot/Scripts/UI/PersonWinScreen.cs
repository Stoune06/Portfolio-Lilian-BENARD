using Godot;
using System;

//Author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
	public partial class PersonWinScreen : Control
	{
        private static PackedScene personWinScreen => GD.Load<PackedScene>("Scenes/UI/PersonLabelWinScreen.tscn");

		[Export] private TextureRect sprite;
        [Export] private Shader shader;
		public ShaderMaterial shaderMaterial = new ShaderMaterial();
        public float cutoff = 0;
        private float waveSize = 0.048f;
        private float frequency = 0.2f;
        private float speed = 5.312f;

        const string CUTOFF = "cutoff";

        public override void _Ready()
        {
            PivotOffset = Size / 2f;
            shaderMaterial.Shader = (Shader)shader.Duplicate();
            sprite.Material = (ShaderMaterial)shaderMaterial.Duplicate();
            shaderMaterial = (ShaderMaterial)sprite.Material;
            shaderMaterial.SetShaderParameter("waveSize", waveSize);
            shaderMaterial.SetShaderParameter("frequency",frequency);
            shaderMaterial.SetShaderParameter("speed", speed);
        }

        public void SetCutoff(float pCutoff)
        {
            shaderMaterial = (ShaderMaterial)sprite.Material;
            shaderMaterial.SetShaderParameter(CUTOFF,pCutoff);
            cutoff = (float)shaderMaterial.GetShaderParameter(CUTOFF);
            GD.Print(pCutoff + Name);
        }

        public Tween TweenCutoff(float pStart, float pEnd, float pDuration, bool pParallel = false)
        {
            shaderMaterial = (ShaderMaterial)sprite.Material;
            Tween lTween = GetTree().CreateTween();
            if (pParallel) lTween.SetParallel();
            lTween.TweenMethod(
                Callable.From<float>((v) => shaderMaterial.SetShaderParameter(CUTOFF, v)),
                pStart,
                pEnd,
                pDuration
            );
            cutoff = (float)shaderMaterial.GetShaderParameter(CUTOFF);
            return lTween;
        }

        public static PersonWinScreen CreatePerson(Vector2 pPosition,Node pContainer)
        {
            PersonWinScreen lPerson = personWinScreen.Instantiate<PersonWinScreen>();
            lPerson.GlobalPosition = pPosition;
            pContainer.AddChild(lPerson);
            return lPerson;
        }
    }
}
