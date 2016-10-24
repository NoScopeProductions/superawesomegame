using UnityEngine;
using JetBrains.Annotations;
using ShooterGame.Constants;
using UnityEngine.UI;

namespace ShooterGame.Managers
{
    public class BarScript : MonoBehaviour
    {
        [SerializeField] private HudBarType _barType;
        [SerializeField] private float _fillAmount = 1f;
        [SerializeField] private Image _bar;

        public HudBarType BarType { get { return _barType; } }
        public float FillAmount { get { return _fillAmount; } }

        // Update is called once per frame
        [UsedImplicitly]
        void Update()
        {
            HandleBar();
        }

        /// <summary>
        /// Clamps the fill amount between 0 and 1
        /// </summary>
        public float SetFillAmount(float fillAmount)
        {
            return _fillAmount = Mathf.Clamp(fillAmount, 0, 1);
        }

        private void HandleBar()
        {
            _bar.fillAmount = _fillAmount;
        }
    }
}
