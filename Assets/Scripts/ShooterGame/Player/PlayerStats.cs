using UnityEngine;
using JetBrains.Annotations;

public class PlayerStats : MonoBehaviour, IDestructible
{
    [SerializeField] private float _health = 100f;
    [SerializeField] private float _maxHealth = 100f;

    private Weapon _primary, _secondary;
    private HUD _hud;

    public float Health { get { return _health; } }
    public float MaxHealth { get { return _maxHealth; } }
    public Weapon PrimaryWeapon { get { return _primary; } }
    public Weapon SecondaryWeapon { get { return _secondary; } }

    [UsedImplicitly]
    void Start()
    {
        _hud = HUD.Instance;

        _hud.ShowHealth(_health, _maxHealth);
        _hud.ShowLoadout(_primary, _secondary);
    }

    [UsedImplicitly]
    void Update()
    {
        if(Input.GetButtonDown(Inputs.Temp_DecreaseHealth))
            TakeDamage(10, this);

        if(Input.GetButtonUp(Inputs.Temp_IncreaseHealth))
            Heal(10, this);
    }

    public void Equip(Weapon weapon)
    {
        _primary = weapon;
    }

    public void EquipSecondary(Weapon weapon)
    {
        _secondary = weapon;
    }

    #region IDestructible implementation
    public void TakeDamage(float amount, PlayerStats attacker)
    {
        _health -= amount;
        _hud.ShowHealth(_health, _maxHealth);

        if (_health <= 0)
            Die();
    }
    #endregion

    public void Heal(float amount, PlayerStats healer)
    {
        TakeDamage(-amount, healer);
    }

    private void Die()
    {
        Debug.Log("He's dead, Jim.");
        Destroy(gameObject);
    }
}
