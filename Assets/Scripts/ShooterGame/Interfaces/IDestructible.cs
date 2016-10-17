using ShooterGame.Player;

namespace ShooterGame.Interfaces
{
    /// <summary>
    /// Interface for both players and destructible objects
    /// </summary>
    public interface IDestructible
    {
        /// <summary>
        /// return true if health is dropped to 0
        /// </summary>
        void TakeDamage(float amount, PlayerStats attacker);
    }
}