using UnityEngine;
using System.Collections;
using System.Linq;

public enum HUDBarType
{
    Health
}

public class HUD : MonoBehaviour {
    [SerializeField] private BarScript _healthBar;
    
    public static HUD Instance { get; private set; }

    void Awake()
    {
        if (!Instance)
            Instance = this;
        else if(Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        _healthBar = GetComponentsInChildren<BarScript>().First(bar => bar.BarType == HUDBarType.Health);
    }

    public void ShowHealth(float health, float maxHealth)
    {
        _healthBar.SetFillAmount(health/maxHealth);
    }

    public void ShowLoadout(Weapon primary, Weapon secondary)
    {
        
    }
}
