using ShooterGame.Player;

namespace ShooterGame.Interfaces
{
    public interface IDestructible
    {
        float Health { get; }
        float MaxHealth { get; }
        void TakeDamage(float amount, PlayerStats attacker);
    }
}