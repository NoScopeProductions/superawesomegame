using UnityEngine;
using JetBrains.Annotations;
using UnityEngine.UI;

public class BarScript : MonoBehaviour
{
    [SerializeField] private HUDBarType _barType;

    [SerializeField] private float _fillAmount = 1f;
    [SerializeField] private Image _bar;

    public HUDBarType BarType { get { return _barType; } }

    public float FillAmount { get { return _fillAmount; } }

    // Use this for initialization
    [UsedImplicitly]
    void Start()
    {

    }

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
