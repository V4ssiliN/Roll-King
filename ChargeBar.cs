using System;
using UnityEngine.UI;
using UnityEngine;

public class ChargeBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public CanvasGroup cGroup;

    public static ChargeBar instance;

    public float magnitude = 5f;
    private Vector3 originalPos;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Il y a plus d'une instance de ChargeBar dans la scène !");
            return;
        }
        instance = this;
    }

    private void Start()
    {
        originalPos = transform.localPosition;
    }

    private void Update()
    {
        if (slider.value > .99f * slider.maxValue)
        {
            float offsetX = UnityEngine.Random.Range(-magnitude, magnitude);
            float offsetY = UnityEngine.Random.Range(-magnitude, magnitude);
            transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);
        }
    }

    public void SetCharge(float charge)
    {
        slider.value = charge;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void Fade()
    {
        cGroup.LeanAlpha(0, 1f);
    }

    public void Appear()
    {
        cGroup.LeanAlpha(1, .2f);
    }
}
