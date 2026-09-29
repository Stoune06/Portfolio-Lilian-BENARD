using UnityEngine;
using UnityEngine.VFX;

namespace Platformer
{
	public class VFX_Manager : MonoBehaviour
	{
		[SerializeField] private VisualEffect MovementParticles;

        public void PlayWalkParticles() => MovementParticles.SendEvent("PlayWalkParticles");
        public void PlayJumpParticles() => MovementParticles.SendEvent("PlayJumpParticles");
    }
}