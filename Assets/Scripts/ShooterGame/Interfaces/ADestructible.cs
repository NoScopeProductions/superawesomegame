using ShooterGame.Player;
using UnityEngine;

namespace ShooterGame.Interfaces
{
    public abstract class ADestructible : MonoBehaviour, IDestructible
    {
        [SerializeField] protected Stat _health;

        public Stat Health { get { return _health; } }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public abstract void TakeDamage(float amount, PlayerStats attacker, Vector2 pointOfContact);
    }
}