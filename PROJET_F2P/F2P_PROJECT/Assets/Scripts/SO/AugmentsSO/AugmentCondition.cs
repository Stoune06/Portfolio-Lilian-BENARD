namespace Com.IsartDigital.HealerSurvivor.SO
{
    public abstract class AugmentCondition
    {
        public abstract bool IsMet(IAugmentable pPlayer);

        protected abstract string GetDescription();
    }
}
