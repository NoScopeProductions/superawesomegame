using UnityEngine;

namespace ShooterGame.Managers
{
    public class EnvironmentManager : MonoBehaviour
    {
        private Vector2 _windDirection;
        private HUD _hud;
        // Use this for initialization
        void Start()
        {
            _hud = HUD.Instance;

        }
        // Update is called once per frame
        void Update()
        {
            if(Time.frameCount % 144 == 0)
                UpdateWind(); //will eventually be called once per turn
        }

        void UpdateWind()
        {
            float x = Random.Range(-1f, 1f);
            float y = Random.Range(-1f, 1f);

            _windDirection = new Vector2(x, y);

            Debug.Log(_windDirection);
            _hud.ShowWind(_windDirection);
        }
    }
}