using System;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShooterGame.Managers
{
    public enum WeatherState : int
    {
        Clear     = 50, //50% chance
        LightRain = 67, //~16%
        HeavyRain = 84, //~16%
        Snow      = 100 //~16%
    }

    public enum TimeOfDay
    {
        Day = 75,
        Night = 100
    }

    public class EnvironmentManager : MonoBehaviour
    {
        private Vector2 _windDirection;
        private HUD _hud;
        private GameManager _gm;

        private WeatherState _weather;
        private TimeOfDay _timeOfDay;

        [UsedImplicitly]
        void Awake()
        {
            _weather = GenerateWeather();
        }

        // Use this for initialization
        [UsedImplicitly]
        void Start()
        {
            _hud = HUD.Instance;
            _gm = GameManager.Instance;
            _gm.OnTurnUpdate += TurnUpdate;
        }

        void TurnUpdate()
        {
            UpdateWind();
        }

        void UpdateWind()
        {
            float x = Random.Range(-1f, 1f);
            float y = Random.Range(-1f, 1f);

            _windDirection = new Vector2(x, y);

            Debug.Log(_windDirection);
            _hud.ShowWind(_windDirection);
        }

        WeatherState GenerateWeather()
        {
            //there has to be a fancier way to do this
            int state = Random.Range(0, 100);

            if (state <= (int) WeatherState.Clear)
                return WeatherState.Clear;
            else if (state <= (int) WeatherState.LightRain)
                return WeatherState.LightRain;
            else if (state <= (int) WeatherState.HeavyRain)
                return WeatherState.HeavyRain;
            else
                return WeatherState.Snow;
        }
        
        
    }
}