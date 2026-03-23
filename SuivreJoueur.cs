using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SuivreJoueur : MonoBehaviour
{
    public Transform player;
    public herosMouvements playerMove;
    public Vector3 normalOffset;
    Vector3 realOffset;
    [Range(1,10)]
    public float smoothFactor;

    public Camera cam;

    public static SuivreJoueur instance;

    private Vector3 targetPosition;

    public bool onIt = false;
    public bool isTravelingX = false;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Il y a plus d'une instance de SuivreJoueur dans la scène");
            return;
        }

        instance = this;
    }

    void Start()
    {
        cam = Camera.main;
        realOffset = normalOffset;

        transform.position = new Vector3(-4, -1, normalOffset.z);
        targetPosition = transform.position;
    }

    void LateUpdate()
    {
        if (playerMove.onWheel)
        {
            Follow();
        }
    }

    public void CalculateTarget()
    {
        onIt = false;
        tourner wheelScript = herosMouvements.instance.lastWheelOn.gameObject.GetComponent<tourner>();
        isTravelingX = (wheelScript.type == tourner.WheelType.Traveling && wheelScript.movingX);

        if (GameManager.instance.gameScore == 0)
        {
            targetPosition = new Vector3(-4, -1, normalOffset.z);
        }
        else
        {
            Vector3 pos = (isTravelingX && wheelScript.sens == 1) ? wheelScript.nextPos : player.parent.parent.position;

            targetPosition = pos + realOffset;

            if (!GameManager.instance.currentWheels.ContainsKey(GameManager.instance.gameScore + 1))
            {
                GameManager.instance.CreateNewWheel();
            }

            if (GameManager.instance.gameScore > 2 && !isTravelingX &&
                !Helpers.IsVisibleFromVirtualCam(GameManager.instance.currentWheels[GameManager.instance.gameScore + 1].position, targetPosition, cam.orthographicSize, cam.aspect, 0f))
            {
                Debug.Log("invisible");
                Vector3 firstPos = (isTravelingX && wheelScript.sens == 1) ? wheelScript.nextPos : wheelScript.transform.parent.position;

                targetPosition = 0.5f * firstPos + 0.5f * GameManager.instance.currentWheels[GameManager.instance.gameScore + 1].position;
                targetPosition.z = realOffset.z;
            }
        }
    }

    public void Follow()
    {
        //Debug.Log("diff x : " + (transform.position.x - (player.parent.position.x + realOffset.x))
        //    + "diff y : " + (transform.position.y - (player.parent.position.y + realOffset.y)) 
        //    + "diff z : " + (transform.position.z - (player.parent.position.z + realOffset.z)));
        //if (isTravelingX && (onIt || Vector3.Distance(transform.position, player.parent.position + realOffset) < 0.2f))
        //{
        //    onIt = true;
        //    transform.position = player.parent.position + realOffset;
        //}
        //else if (Vector3.Distance(transform.position, targetPosition) > 0.05f)
        //{
        if (isTravelingX)
        {
            targetPosition = player.parent.position + realOffset;
        }
        
        Vector3 smoothPosition = Vector3.Lerp(transform.position, targetPosition, smoothFactor * Time.fixedDeltaTime);
        transform.position = smoothPosition;
        //}
    }
}
