using UnityEngine;
using JetBrains.Annotations;
using ShooterGame.Player;
using UnityEngine.UI;

namespace ShooterGame.Managers
{
    public class HUD : MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private BarScript _healthBar, _shieldBar;

        [SerializeField] private Text _wind;

        public static HUD Instance { get; private set; }

        [UsedImplicitly]
        void Awake()
        {
            if (!Instance)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        public void ShowHealth(Stat health)
        {
            ShowStat(_healthBar, health);
        }

        public void ShowShields(Stat shields)
        {
            ShowStat(_shieldBar, shields);
        }

        private void ShowStat(BarScript statusBar, Stat status)
        {
            statusBar.SetFillAmount(status.Value / status.MaxValue);
        }

        public void ShowLoadout(Weapon primary, Weapon secondary)
        {

        }

        public void ShowWind(Vector2 direction)
        {
            float speed = Mathf.Abs(direction.magnitude);
            _wind.text = string.Format("{0:n1} ({1:n2}, {2:n2})", speed, direction.x, direction.y);
        }
    }
}