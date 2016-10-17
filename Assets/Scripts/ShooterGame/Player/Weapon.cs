using UnityEngine;
using System.Collections;

namespace ShooterGame.Player
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private float _attackPower;

        private float _cooldown = 0; //# of rounds left on cooldown

        public float AttackPower { get { return _attackPower; } }
        public float Cooldown { get { return _cooldown; } }
    }
}