namespace Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Shared
{
    public interface ITarget
    {
        public float targetHealth { get; }
        public float targetMaxHealth { get; }

        public void TakeDamage(float pDamage);
    }
}
