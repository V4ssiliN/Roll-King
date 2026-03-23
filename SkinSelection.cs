using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class SkinSelection : MonoBehaviour
{
    int skinIndex => herosMouvements.instance.skinIndex;
    int skinCount => herosMouvements.instance.skinCount;

    private int skinDisplaysCount;

    public List<Image> skinDisplays;

    public List<Sprite> displaySprites;

    public int selectedSprite = 0;

    private Image unseenSprite;

    public float spaceBetweenSprites = 75f;
    [Range(0f,1f)]
    public float fadedAlpha = .5f;

    public float moveDuration = .25f;
    float timer = 0f;

    public static SkinSelection instance;

    public Image[] lockImages;
    public TextMeshProUGUI[] lockTexts;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Il y a plus d'une instance de SkinSelection dans la scène !");
            return;
        }

        instance = this;
    }

    private void Start()
    {
        skinDisplaysCount = skinDisplays.Count;
        unseenSprite = skinDisplays[skinDisplaysCount - 1];

        lockImages = new Image[skinDisplaysCount];
        lockTexts = new TextMeshProUGUI[skinDisplaysCount];

        for (int i = 0; i < skinDisplaysCount; i++)
        {
            lockImages[i] = skinDisplays[i].transform.GetChild(0).gameObject.GetComponent<Image>();
            lockImages[i].color = new Color(1f, 1f, 1f, fadedAlpha);

            lockTexts[i] = lockImages[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        }
    }

    private void Update()
    {
        if (timer > -1f)
        {
            timer -= Time.unscaledDeltaTime;
        }
    }

    public void ScrollSprite(int direction)
    {
        if (timer < 0f)
        {
            timer = moveDuration;
            StopAllCoroutines();

            unseenSprite.rectTransform.anchoredPosition = new Vector2(direction * 2 * spaceBetweenSprites, unseenSprite.rectTransform.anchoredPosition.y);

            for (int i = 0; i < skinDisplaysCount; i++)
            {
                Image skin = skinDisplays[i];

                if (direction * skin.rectTransform.anchoredPosition.x < - 0.5f * spaceBetweenSprites && direction * skin.rectTransform.anchoredPosition.x > - 1.5f * spaceBetweenSprites)
                {
                    int index = modulo(selectedSprite + direction * 2, skinCount);
                    int unseenSpriteIndex = skinDisplays.IndexOf(unseenSprite);
                    
                    unseenSprite.sprite = displaySprites[index];

                    unseenSprite.color = new Color(1f, 1f, 1f, fadedAlpha);
                    lockImages[unseenSpriteIndex].color = new Color(1f, 1f, 1f, fadedAlpha);
                    
                    if (GameManager.instance.bestScore < herosMouvements.instance.skinsScoreRequierements[index] && !herosMouvements.instance.unlockAllSkins)
                    {
                        lockImages[unseenSpriteIndex].gameObject.SetActive(true);
                        lockTexts[unseenSpriteIndex].text = herosMouvements.instance.skinsScoreRequierements[index].ToString();
                    }
                    else
                    {
                        lockImages[unseenSpriteIndex].gameObject.SetActive(false);
                    }
                    unseenSprite = skin;
                }

                if (direction * skin.rectTransform.anchoredPosition.x > 0.5f * spaceBetweenSprites &&
                    direction * skin.rectTransform.anchoredPosition.x < 1.5f * spaceBetweenSprites)
                {
                    int index = modulo(selectedSprite + direction, skinCount);
                    
                    bool isLocked = GameManager.instance.bestScore <
                                    herosMouvements.instance.skinsScoreRequierements[index] && !herosMouvements.instance.unlockAllSkins;
                    Image imageToLean = isLocked ? lockImages[i] : skin;
                    StartCoroutine(LeanAlpha(imageToLean, 1f));
                }

                if (skin.rectTransform.anchoredPosition.x > -0.5f * spaceBetweenSprites && skin.rectTransform.anchoredPosition.x < 0.5f * spaceBetweenSprites)
                {
                    int index = modulo(selectedSprite + direction, skinCount);

                    bool isLocked = GameManager.instance.bestScore <
                                    herosMouvements.instance.skinsScoreRequierements[index] && !herosMouvements.instance.unlockAllSkins;
                    Image imageToLean = isLocked ? lockImages[i] : skin;
                    StartCoroutine(LeanAlpha(imageToLean, fadedAlpha));
                }

                LeanTween.moveLocalX(skin.gameObject, skin.rectTransform.anchoredPosition.x - direction * spaceBetweenSprites, moveDuration).setIgnoreTimeScale(true);
            }
            selectedSprite = modulo(selectedSprite + direction, skinCount);
        }
    }

    public void ActuSkinsDisplays()
    {
        Start();
        
        selectedSprite = skinIndex;

        for (int i = 0; i < skinDisplaysCount; i++)
        {
            int skinIndex = modulo(selectedSprite + i - 1, skinCount);

            skinDisplays[i].sprite = displaySprites[skinIndex];
            skinDisplays[i].rectTransform.anchoredPosition =
                new Vector2(spaceBetweenSprites * (i - 1), skinDisplays[i].rectTransform.anchoredPosition.y);

            bool isLocked = GameManager.instance.bestScore <
                            herosMouvements.instance.skinsScoreRequierements[skinIndex] && !herosMouvements.instance.unlockAllSkins;
            if (isLocked)
            {
                lockImages[i].gameObject.SetActive(true);
                lockTexts[i].text = herosMouvements.instance.skinsScoreRequierements[skinIndex].ToString();
            }
            else
            {
                lockImages[i].gameObject.SetActive(false);
            }

            if (i == 1)
            {
                skinDisplays[i].color = isLocked ? new Color(1f, 1f, 1f, fadedAlpha) : Color.white;
                lockImages[i].color = Color.white;
            }
            else
            {
                lockImages[i].color = new Color(1f, 1f, 1f, fadedAlpha);
                lockImages[i].color = new Color(1f, 1f, 1f, fadedAlpha);
            }
        }

        unseenSprite = skinDisplays[3];
    }

    public void ActuChosenSkin()
    {
        if (GameManager.instance.bestScore >=
            herosMouvements.instance.skinsScoreRequierements[selectedSprite] || herosMouvements.instance.unlockAllSkins)
        {
            herosMouvements.instance.skinIndex = selectedSprite;
            herosMouvements.instance.ActuAnim(); 
        }
    }

    IEnumerator LeanAlpha(Image skin, float alpha)
    {
        float initialAlpha = skin.color.a;
        Color newColor;
        while (timer > 0f)
        {
            newColor = new Color(1f, 1f, 1f, 1f);
            newColor.a = Helpers.Map(timer, moveDuration, 0f, initialAlpha, alpha, false);
            skin.color = newColor;

            yield return null;
        }
        newColor = new Color(1f, 1f, 1f, 1f);
        newColor.a = alpha;
        skin.color = newColor;
    }

    public int modulo(int n, int mod)
    {
        int res = n % mod;
        if (res < 0)
        {
            res += mod;
        }

        return res;
    }
}
