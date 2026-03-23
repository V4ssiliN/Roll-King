using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public GameObject heart;
    public RectTransform rt;
    public int health;
    public int maxHealth;

    public static HealthBar instance;

    private void Start()
    {
        if (instance != null)
        {
            Debug.LogError("Il y a plus d'une instance de HealthBar dans la scne !");
            return;
        }

        instance = this;
        SetHealth(maxHealth);
    }

    public void SetHealth(int healthToSet) 
    {
        health = healthToSet;
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < healthToSet; i++)
        {
            GameObject currentHeart = Instantiate(heart, Vector3.zero, Quaternion.identity);
            currentHeart.transform.SetParent(transform, worldPositionStays:false);
            RectTransform heartRT = currentHeart.GetComponent<RectTransform>();
            heartRT.anchoredPosition = new Vector2(62 * i, 0); //(rt.anchoredPosition.x + 62*i, rt.anchoredPosition.y);
            currentHeart.LeanMoveLocalY(currentHeart.transform.localPosition.y + Screen.height / 100, .5f).setEaseOutSine().setLoopPingPong().delay = .1f;
        }
    }

    public void AddHealth(int healthToAdd)
    {
        
    }

    public void TakeDamage()
    {
        health--;
        if (health < 1)
        {
            GameManager.instance.Die();
        }
        else
        {
            herosMouvements.instance.rb.velocity = Vector3.zero;

            tourner lastWheelScript = herosMouvements.instance.lastWheelOn.gameObject.GetComponent<tourner>();

            Vector2 newPlayerPos = lastWheelScript.transform.position;
            newPlayerPos.y += lastWheelScript.radius;

            herosMouvements.instance.transform.position = newPlayerPos;
            herosMouvements.instance.PutOnWheel(lastWheelScript.transform);

            herosMouvements.instance.StartCoroutine(herosMouvements.instance.Flash());

            if (GameManager.instance.gameScore == 0)
            {
                herosMouvements.instance.lastWheelOn = lastWheelScript.transform;
                GameManager.instance.SetScore(lastWheelScript.wheelNumber);
                Debug.Log("aha");
                SuivreJoueur.instance.CalculateTarget();
            }
        }

        transform.GetChild(transform.childCount - 1).gameObject.LeanScale(Vector2.zero, .5f).setEaseInOutBack();
        StartCoroutine(DestroyHeart(.5f));
    }

    IEnumerator DestroyHeart(float wantedTime)
    {
        yield return new WaitForSeconds(wantedTime);
        Destroy(transform.GetChild(transform.childCount - 1).gameObject);
    }
}
