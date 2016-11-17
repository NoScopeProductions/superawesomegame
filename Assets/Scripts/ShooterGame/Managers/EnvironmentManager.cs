using ShooterGame.UI;
using UnityEngine;

namespace ShooterGame.Managers
{
    public class EnvironmentManager : MonoBehaviour
    {
        public static EnvironmentManager Instance { get; private set; }

        private const float MAX_WIND_FORCE = 15f;

        public Vector2 WindForce { get; private set; }

        private HUD _hud;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
            else if (Instance != this)
                Destroy(this.gameObject);

            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            this._hud = HUD.Instance;
            this.UpdateWind();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                this.UpdateWind();
            }
        }

        private void UpdateWind()
        {
            this.WindForce = Random.insideUnitCircle.normalized;

            this.WindForce *= (int)Random.Range(0, MAX_WIND_FORCE);

            this._hud.ShowWind(this.WindForce);
        }

      
    }
}