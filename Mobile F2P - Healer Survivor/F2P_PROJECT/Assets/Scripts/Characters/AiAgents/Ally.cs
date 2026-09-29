namespace Com.IsartDigital.HealerSurvivor.Gameplay
{
    public class Ally : AIAgent
    {
        protected override void Start()
        {
            base.Start();
            InvokeAllySpawn(transform);
        }

        // OnDestroy fires whether the ally dies via Kill() (Destroy(gameObject)) or via direct Destroy
        // (e.g. GameManager.CleanupRun on leave). Announcing here covers both paths — no double-fire since
        // Kill's Destroy(gameObject) leads to this OnDestroy and nowhere else.
        protected override void OnDestroy()
        {
            base.OnDestroy();
            InvokeAllyDied(transform);
        }
    }
}
