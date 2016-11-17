using UnityEngine;
using JetBrains.Annotations;
using ShooterGame.Constants;
using ShooterGame.Player;
using ShooterGame.Player.Stats;
using UnityEngine.UI;

namespace ShooterGame.UI
{
    public class BarScript : MonoBehaviour
    {
        [SerializeField] private HudBarType _barType;
        [SerializeField] private float _fillAmount = 1f;
        [SerializeField] private Image _bar;
        
        private float _parentWidth;

        public HudBarType BarType { get { return this._barType; } }

        public float FillAmount
        {
            get { return this._fillAmount; }
            set { this._fillAmount = Mathf.Clamp(value, 0, 1); }
        }

        private void Awake()
        {
            this._parentWidth = this._bar.GetComponentInParent<RectTransform>().rect.width;
        }

        private void Update()
        {
            this._bar.rectTransform.sizeDelta = new Vector2(this._fillAmount *this._parentWidth, this._bar.rectTransform.sizeDelta.y);
        }

        public void ShowStat(Stat status)
        {
            this.FillAmount = status.Value / status.MaxValue;
        }
    }
}
