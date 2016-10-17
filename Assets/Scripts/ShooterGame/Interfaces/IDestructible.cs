public interface IDestructible
{
    float Health { get; }
    void TakeDamage(float amount, PlayerStats attacker);
}
