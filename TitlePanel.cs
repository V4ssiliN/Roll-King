using System.Collections;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;

public class TitlePanel : MonoBehaviour
{
    public static TitlePanel instance;
    public RectTransform rectTransform;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Il y a plus d'une instance de TitlePanel dans la scène !");
            return;
        }
        instance = this;

        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        Restart();
    }

    public void Restart()
    {
        Invoke(nameof(PlayChainSound), .3f);
        LeanTween.value(gameObject, rectTransform.anchoredPosition.y, -650f, 1f)
            .setOnUpdate((float y) =>
            {
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, y);
            }).setEaseInOutBack();
    }

    void PlayChainSound()
    {
        AudioManager.instance.PlaySfx("chains");
    }

    public void RemovePanel()
    {
        PlayChainSound();
        GameManager.instance.MoveTopRightPanelY();
        LeanTween.value(gameObject, rectTransform.anchoredPosition.y, 0f, 1f)
            .setOnUpdate((float y) =>
            {
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, y);
            }).setEaseInOutBack();
    }
}
