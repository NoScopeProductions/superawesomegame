using UnityEngine;
using JetBrains.Annotations;
using ShooterGame.Constants;
using UnityEngine.UI;

namespace ShooterGame.UI
{
    public class BarScript : MonoBehaviour
    {
        [SerializeField] private HudBarType _barType;
        [SerializeField] private float _fillAmount = 1f;
        [SerializeField] private Image _bar;
        
        private float _parentWidth;

        public HudBarType BarType { get { return _barType; } }
        public float FillAmount { get { return _fillAmount; } }

        [UsedImplicitly]
        void Awake()
        {
            _parentWidth = _bar.GetComponentInParent<RectTransform>().rect.width;
        }

        // Update is called once per frame
        [UsedImplicitly]
        void Update()
        {
            _bar.rectTransform.sizeDelta = new Vector2(_fillAmount * _parentWidth,
                                                       _bar.rectTransform.sizeDelta.y);
        }

        /// <summary>
        /// Clamps the fill amount between 0 and 1
        /// </summary>
        public float SetFillAmount(float fillAmount)
        {
            return _fillAmount = Mathf.Clamp(fillAmount, 0, 1);
        }
    }
}
