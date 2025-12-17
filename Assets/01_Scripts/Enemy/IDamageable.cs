public interface IDamageable
{
    void TakeDamage(int amount);
    void ApplySlow(float percent, float duration);
    void ApplyStun(float duration);
    void ApplyBurn(float duration, float dps);
    void ApplyPoison(float duration, float dps);
}
