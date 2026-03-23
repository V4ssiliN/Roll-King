using UnityEngine.UI;
using UnityEngine;

public class score : MonoBehaviour
{
    public Text text;
    public RectTransform rt;

    public void SetText(string score)
    {
        text.text = "Score : " + score;
        
        rt.sizeDelta = new Vector2(250 + 45 * score.Length, rt.sizeDelta.y);
        rt.anchoredPosition = new Vector2((float)(-180 - 22.5 * score.Length), rt.anchoredPosition.y);
    }
}
