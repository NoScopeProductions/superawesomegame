using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    private int _cooldown = 0; //# of rounds left on cooldown
    [SerializeField] private int _attackPower;

    private PlayerStats _wielder;

    public int AttackPower { get { return _attackPower; } }
    public int Cooldown { get { return _cooldown; } }
    
    public void Attack(PlayerStats target)
    {
        target.TakeDamage(_attackPower, _wielder);
    }

    public void Equip(PlayerStats wielder)
    {
        _wielder = wielder;
    }
}
