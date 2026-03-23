using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class HelpText : MonoBehaviour
{
    public TextMeshProUGUI helpText;
    public CanvasGroup cg;
    public static HelpText instance;

    private void Awake()
    {
        if(instance!= null)
        {
            Debug.LogError("Il y a plus d'une instance de HelpText dans la scène !");
            return;
        }
        instance = this;
    }

    void Start()
    {
        Restart();
    }

    public void Restart()
    {
        gameObject.SetActive(true);
        //helpText.text = "- - - MAINTENEZ ESPACE - - -";
        cg.LeanAlpha(0f, 1f).setEaseInOutBack().setLoopPingPong();
    }

    public void ChangeText()
    {
        StartCoroutine(ChangeTextWhenSpace());
    }

    IEnumerator ChangeTextWhenSpace()
    {
        yield return new WaitForSeconds(1);
        if(!Input.GetButton("Jump") && !Input.GetMouseButton(0))
        {
            LeanTween.cancel(gameObject);
            cg.LeanAlpha(0, cg.alpha);
            yield return new WaitForSeconds(cg.alpha);
            gameObject.SetActive(false);
        }
        else
        {
        LeanTween.cancel(gameObject);
        cg.LeanAlpha(0, cg.alpha);
        yield return new WaitForSeconds(cg.alpha);
        helpText.text = "- - - RELACHEZ - - -";
        cg.LeanAlpha(1f, 1f).setEaseInOutBack().setLoopPingPong();
        yield return new WaitUntil(() => !Input.GetButton("Jump") || !Input.GetMouseButton(0));
        LeanTween.cancel(gameObject);
        cg.LeanAlpha(0, cg.alpha);
        yield return new WaitForSeconds(cg.alpha);
        gameObject.SetActive(false);
        }
    }
}
