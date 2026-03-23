using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public GameObject[] wheelPrefabs;

    public score scoreJeu;

    public Sprite[] sprites;

    public Sprite[] structureSprites;
    public Sprite[] structureSpritesTravelingY;

    public Sprite bigCenterSprite;
    public Sprite smallCenterSprite;

    public Transform cam;
    public tourner lastWheel;
    public static GameManager instance;

    public Transform allWheels;

    public int gameScore = 0;

    public float initialMaxDistance = 8f;
    public float maxDistance;

    public int minRadius = 3;
    public int maxRadius = 5;

    public int minTravelingRadius;

    public int initialMinSpeed = 1;
    public int initialMaxSpeed = 5;

    public int minSpeed = 1;
    public int maxSpeed = 5;

    public int initialMinTravelingSpeed;
    public int initialMaxTravelingSpeed;

    public int minTravelingSpeed;
    public int maxTravelingSpeed;

    [Range(0, 1)] public float initialTravelingWheelProba;
    public float travelingWheelProba;

    [Range(0, 1)] public float initialTravelingYProba = 0.5f;
    public float travelingYProba = 0.5f;

    public bool showTrajectory;

    public int bestScore;
    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI bestScoreTitleText;

    public RectTransform topRightPanel;

    public RectTransform menuPanel;
    public Image menuBlackPanel;
    private float initialMenuPosY;

    public Image blackPanel;

    public Dictionary<int, Transform> currentWheels = new Dictionary<int, Transform>();
    public List<int> test;

    public float minTravelingDist = 3f;
    public float maxTravelingDistY = 5f;
    public float maxTravelingDistX = 10f;

    private int initialNbTrajectoryPoints;

    public GameObject xTravel;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Random.InitState(System.Environment.TickCount);

        if (instance != null)
        {
            Debug.LogError("Il y a plus d'une instance de GameManager dans la scène");
            return;
        }

        instance = this;

        bestScore = PlayerPrefs.GetInt(nameof(bestScore), 0);
    }

    private void Start()
    {
        scoreJeu.SetText("0");
        minSpeed = initialMinSpeed;
        maxSpeed = initialMaxSpeed;
        minTravelingSpeed = initialMinTravelingSpeed;
        maxTravelingSpeed = initialMaxTravelingSpeed;
        travelingWheelProba = 0f;
        travelingYProba = initialTravelingYProba;
        minTravelingRadius = 5;
        maxDistance = initialMaxDistance;
        showTrajectory = true;
        initialNbTrajectoryPoints = TrajectoryPreview.instance.numberOfPoints;

        herosMouvements.instance.unlockAllSkins =
            PlayerPrefs.GetInt(nameof(herosMouvements.instance.unlockAllSkins), 0) == 1;

        herosMouvements.instance.skinIndex = PlayerPrefs.GetInt(nameof(herosMouvements.instance.skinIndex), 0);
        if (bestScore < herosMouvements.instance.skinsScoreRequierements[herosMouvements.instance.skinIndex] && !herosMouvements.instance.unlockAllSkins)
        {
            herosMouvements.instance.skinIndex = 0;
        }

        AudioManager.instance.ChangeMusicVolume(PlayerPrefs.GetFloat("musicVolume", 1f));
        AudioManager.instance.ChangeSFXVolume(PlayerPrefs.GetFloat("sfxVolume", 1f));
        AudioManager.instance.PlayMusic("music1");
        bestScoreTitleText.text = bestScore.ToString();

        initialMenuPosY = menuPanel.anchoredPosition.y;

        menuBlackPanel = menuPanel.transform.parent.gameObject.GetComponent<Image>();

        blackPanel.color = Color.black;
        LeanTween.value(blackPanel.gameObject, 1f, 0f, .5f).setOnUpdate((float val) =>
        {
            Color c = blackPanel.color;
            c.a = val;
            blackPanel.color = c;
        });

        CreateNewWheel();
        CreateNewWheel();

    }

    private void Update()
    {
        test.Clear();
        foreach (KeyValuePair<int, Transform> pair in currentWheels)
        {
            test.Add(pair.Key);
        }

        if (gameScore > 0)
        {
            Vector2 lastPos = (lastWheel.type == tourner.WheelType.Traveling && lastWheel.movingX)
                ? lastWheel.nextPos
                : lastWheel.transform.parent.position;
            if (lastPos.x - cam.position.x < 15)
            {
                CreateNewWheel();
            }  
        }
    }

    void SavePrefs()
    {
        PlayerPrefs.SetInt(nameof(bestScore), bestScore);
        PlayerPrefs.SetInt(nameof(herosMouvements.instance.skinIndex), herosMouvements.instance.skinIndex);

        PlayerPrefs.SetFloat("musicVolume", AudioManager.instance.musicSource.volume);
        PlayerPrefs.SetFloat("sfxVolume", AudioManager.instance.sfxSources[0].volume);

        PlayerPrefs.Save();
    }

    void OnDestroy()
    {
        PlayerPrefs.SetInt(nameof(herosMouvements.instance.unlockAllSkins), herosMouvements.instance.unlockAllSkins ? 1 : 0);
        SavePrefs();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(nameof(herosMouvements.instance.unlockAllSkins), 0);
        SavePrefs();
    }

    public void CreateNewWheel()
    {
        bool lastWheelTravelingX = lastWheel.type == tourner.WheelType.Traveling && lastWheel.movingX;

        Vector2 lastPos = lastWheelTravelingX
            ? lastWheel.nextPos
            : lastWheel.transform.parent.position;

        bool isTraveling = lastWheel.type == tourner.WheelType.Normal && UnityEngine.Random.value < travelingWheelProba;

        float maxXPos = lastWheelTravelingX ? Mathf.Min(14f, maxDistance) : maxDistance;

        float mean = (maxXPos + 6f) / 2f;
        float spreadMax = maxXPos - mean;
        
        float spread = spreadMax * .75f;

        float locatedMean = lastPos.x + mean;

        float x = RandomUtils.SampleClamped(locatedMean - spreadMax, locatedMean + spreadMax, locatedMean, spread, .5f);

        int nbRandom;
        if (isTraveling)
        {
            nbRandom = Random.Range(minTravelingRadius, Mathf.Min(5, maxRadius + 2));
        }
        else
        {
            nbRandom = Random.Range(minRadius, maxRadius + 1);
        }

        float newYPos = Random.Range(-3f, 3f);


        Vector2 wheelPos = new Vector2(x, newYPos);
        GameObject newWheel = Instantiate(wheelPrefabs[0], wheelPos, Quaternion.identity);
        newWheel.transform.parent = allWheels;

        GameObject rotatingPart = newWheel.transform.GetChild(0).gameObject;
        GameObject center = newWheel.transform.GetChild(1).gameObject;
        GameObject structure = newWheel.transform.GetChild(2).gameObject;

        Sprite spriteRotatingPart = sprites[5 - nbRandom];
        rotatingPart.GetComponent<SpriteRenderer>().sprite = spriteRotatingPart;

        Sprite spriteStructure = structureSprites[5 - nbRandom];
        structure.GetComponent<SpriteRenderer>().sprite = spriteStructure;

        center.GetComponent<SpriteRenderer>().sprite = nbRandom == 1 ? smallCenterSprite : bigCenterSprite;

        rotatingPart.GetComponent<CircleCollider2D>().radius = (float)((nbRandom) / 2.0);
        tourner wheelScript = rotatingPart.GetComponent<tourner>();
        wheelScript.vitesse = Random.Range(minSpeed, maxSpeed + 1) * (int)Mathf.Pow(-1, Random.Range(1,3));
        wheelScript.previousWheel = lastWheel.gameObject;
        wheelScript.wheelNumber = lastWheel.wheelNumber + 1;
        wheelScript.center = center.transform;
        currentWheels.Add(wheelScript.wheelNumber, newWheel.transform);

        wheelScript.type = tourner.WheelType.Normal;
        if (isTraveling)
        {
            Debug.Log("traveling");
            wheelScript.type = tourner.WheelType.Traveling;
            int randomSpeed = Random.Range(minTravelingSpeed, maxTravelingSpeed + 1);

            if (UnityEngine.Random.value < travelingYProba)
            {
                wheelScript.travelingSpeed = randomSpeed;
                wheelScript.movingX = false;

                float travelingDistance =
                    minTravelingDist + UnityEngine.Random.value * (maxTravelingDistY - minTravelingDist);

                float maxHeight = 3f - travelingDistance;
                float yPos = -3f + UnityEngine.Random.value * (maxHeight + 3);
                newWheel.transform.position = new Vector2(newWheel.transform.position.x, yPos);

                wheelScript.nextPos.x = newWheel.transform.position.x;
                wheelScript.nextPos.y = yPos + travelingDistance;

                spriteStructure = structureSpritesTravelingY[5 - nbRandom];
                structure.GetComponent<SpriteRenderer>().sprite = spriteStructure;

                structure.transform.localPosition = new Vector2(0, travelingDistance);
            }
            else
            {
                wheelScript.travelingSpeed = randomSpeed;
                wheelScript.movingX = true;

                float travelingDistance =
                    minTravelingDist + UnityEngine.Random.value * (maxTravelingDistX - minTravelingDist);

                wheelScript.nextPos.x = newWheel.transform.position.x + travelingDistance;
                wheelScript.nextPos.y = newWheel.transform.position.y;

                spriteStructure = structureSpritesTravelingY[3];
                structure.GetComponent<SpriteRenderer>().sprite = spriteStructure;

                Transform newStruct = Instantiate(xTravel, wheelPos, Quaternion.identity).transform;
                newStruct.SetParent(newWheel.transform);
                newStruct.localPosition = new Vector2(travelingDistance, 0f);

                Transform horizontalBar = newStruct.GetChild(0);
                horizontalBar.localPosition = new Vector2(- travelingDistance / 2f, 0f);
                horizontalBar.localScale = new Vector2(travelingDistance, 1f);
            }
        }

        lastWheel.nextWheel = rotatingPart;
        lastWheel = wheelScript;
    }

    public void SetScore(int score)
    {
        gameScore = score;
        scoreJeu.SetText(gameScore.ToString());
        HandleDifficulty();

        if (gameScore > bestScore)
        {
            bestScore = gameScore;
        }
    }

    public void HandleDifficulty()
    {
        TrajectoryPreview.instance.numberOfPoints = Mathf.Max(0, initialNbTrajectoryPoints - gameScore);
        if (TrajectoryPreview.instance.numberOfPoints == 0)
        {
            showTrajectory = false;
        }

        if (minRadius > 2 && gameScore > 4)
        {
            minRadius = 2;
        }
        else if (maxRadius > 4 && gameScore > 9)
        {
            maxRadius = 4;
        }
        else if (minRadius > 1 && gameScore > 14)
        {
            minRadius = 1;
        }
            
        minSpeed = initialMinSpeed + gameScore / 15;
        maxSpeed = initialMaxSpeed + gameScore / 7;

        maxDistance = Mathf.Min(16f, initialMaxDistance + (gameScore / 5) * .5f);

        int tmpScore = Mathf.Max(0, gameScore - 15);
        travelingYProba = gameScore >= 15 ? initialTravelingYProba : 1f;
        tmpScore = Mathf.Max(0, gameScore - 10);
        travelingWheelProba = gameScore >= 10 ? Mathf.Min(initialTravelingWheelProba + (tmpScore / 5) * 0.05f, 0.8f) : 0f;

        minTravelingSpeed = initialMinTravelingSpeed + tmpScore / 15;
        maxTravelingSpeed = initialMaxTravelingSpeed + tmpScore / 7;

        minTravelingRadius = Mathf.Max(1, 5 - (tmpScore / 10));
    }


    public void ManageMenu(bool quit)
    {
        if (!quit)
        {
            menuBlackPanel.gameObject.SetActive(true);
        }
        
        Time.timeScale = quit ? 1 : 0;
        herosMouvements.instance.enabled = quit;

        AudioManager.instance.SetSliders();
        AudioManager.instance.PlaySfx("chains");

        if (quit)
        {
            SkinSelection.instance.ActuChosenSkin(); 
        }
        else
        {
            SkinSelection.instance.ActuSkinsDisplays();
        }

        UpdateBestScore();

        LeanTween.cancel(menuPanel);
        float newYPos = quit ? initialMenuPosY : 0f;
        LeanTween.value(gameObject, menuPanel.anchoredPosition.y, newYPos, .5f)
            .setOnUpdate((float y) =>
            {
                menuPanel.anchoredPosition = new Vector2(menuPanel.anchoredPosition.x, y);
            }).setEaseInOutBack().setIgnoreTimeScale(true);
        
        StopAllCoroutines();
        float newAlpha = quit ? 0f : .3f;
        StartCoroutine(LeanAlpha(menuBlackPanel, newAlpha, .25f));

        if (quit)
        {
            Invoke(nameof(DisableMenuPanel), .5f);
        }
    }

    void DisableMenuPanel()
    {
        menuBlackPanel.gameObject.SetActive(false);
    }

    public void MoveTopRightPanelY()
    {
        LeanTween.value(topRightPanel.gameObject, topRightPanel.anchoredPosition.y, 0f, 0.5f)
            .setOnUpdate((float y) =>
            {
                topRightPanel.anchoredPosition = new Vector2(topRightPanel.anchoredPosition.x, y);
            }).setDelay(1f);
    }

    public void UpdateBestScore()
    {
        bestScoreText.text = bestScore.ToString();
    }

    IEnumerator LeanAlpha(Image skin, float alpha, float time)
    {
        float initialAlpha = skin.color.a;
        Color newColor;
        float timer = time;
        while (timer > 0f)
        {
            newColor = skin.color;
            newColor.a = Helpers.Map(timer, time, 0f, initialAlpha, alpha, false);
            skin.color = newColor;
            timer -= Time.unscaledDeltaTime;

            yield return null;
        }
        newColor = skin.color;
        newColor.a = alpha;
        skin.color = newColor;
    }

    public void Die()
    {
        herosMouvements.instance.gameObject.SetActive(false);
        
        LeanTween.value(blackPanel.gameObject, 0f, 1f, .5f).setOnUpdate((float val) =>
        {
            Color c = blackPanel.color;
            c.a = val;
            blackPanel.color = c;
        });
        Invoke(nameof(LoadCurrentScene), .5f);
    }
    public void LoadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
