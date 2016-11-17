using ShooterGame.UI;
using UnityEngine;

namespace ShooterGame.Managers
{
    public class EnvironmentManager : MonoBehaviour
    {
        public static EnvironmentManager Instance { get; private set; }

        private const float MAX_WIND_FORCE = 15f;

        public Vector2 WindForce { get; private set; }

        private HudManager _hudManager;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            this._hudManager = HudManager.Instance;
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

            this._hudManager.ShowWind(this.WindForce);
        }

      
    }
}