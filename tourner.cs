using System.Collections.Generic;
using UnityEngine;

public class tourner : MonoBehaviour
{
    public int vitesse;
    public float radius;

    public Transform cam;
    //[HideInInspector]
    public GameObject previousWheel;
    //[HideInInspector]
    public GameObject nextWheel;
    public int wheelNumber;
    public AudioSource audioSource;

    public enum WheelType {Normal, Traveling}
    public WheelType type;
    public bool movingX;
    public Vector2 nextPos;
    public float travelingSpeed;
    public Transform center;

    public int sens = 1;
    private Vector2 target;

    private bool isCurrent;

    public CircleCollider2D trajectoryCollider;
    public Transform fakeWheel;

    public float fakeWheelSpeedOffset = 0f;

    private void Start()
    {
        radius = gameObject.GetComponent<CircleCollider2D>().radius;
       
        trajectoryCollider = transform.GetChild(0).gameObject.GetComponent<CircleCollider2D>();
        trajectoryCollider.radius = radius + 0.7f;

        fakeWheel = transform.GetChild(1);

        cam = Camera.main.transform;

        float distance = Vector2.Distance(transform.parent.position, previousWheel.transform.parent.position);

        float sumRadius = previousWheel.GetComponent<CircleCollider2D>().radius +
                          gameObject.GetComponent<CircleCollider2D>().radius;

        bool isUp = transform.parent.position.y > previousWheel.transform.parent.position.y;
       
        float maxDistance = 12;

        if (wheelNumber > 1 && type == WheelType.Traveling && movingX && distance - sumRadius > 8)
        {
            Debug.Log("correction incertaine");
            transform.parent.position = new Vector2(transform.parent.position.x - 6f, transform.parent.position.y);
            nextPos.x -= 6f;
        }
        else if (wheelNumber > 1 && isUp && distance - sumRadius > maxDistance)
        {
            float newRandomY = -3f + UnityEngine.Random.value * (previousWheel.transform.parent.position.y + 3f);

            transform.parent.position = new Vector2(transform.parent.position.x, newRandomY);
            Debug.Log("Un rapprochement de roue a été effectué (par y).");

            //if (transform.parent.position.x - previousWheel.transform.parent.position.x > 10)
            //{
            //    transform.parent.position = new Vector2(transform.parent.position.x - 3, transform.parent.position.y);
            //    Debug.Log("Un rapprochement de roue a été effectué (par x).");
            //}
        }

        if(Vector3.Distance(previousWheel.transform.parent.position, transform.parent.position)
            - previousWheel.GetComponent<CircleCollider2D>().radius
            - gameObject.GetComponent<CircleCollider2D>().radius < 2)
        {
            //Debug.Log("Une suppression de roue a été effectuée.");

            //GameManager.instance.currentWheels.Remove(wheelNumber);
            //GameManager.instance.lastWheel = previousWheel.GetComponent<tourner>();
            //Destroy(transform.parent.gameObject);
            return;
        }

        AudioManager.instance.sfxSources.Add(audioSource);
        target = nextPos;
    }

    void Update()
    {
        isCurrent = transform.childCount > 1;

        float omegaDeg = (vitesse / radius) * Mathf.Rad2Deg;
        transform.Rotate(new Vector3(0, 0, vitesse*-20*Time.deltaTime));
        fakeWheel.Rotate(new Vector3(0, 0, fakeWheelSpeedOffset * Time.deltaTime / radius));

        if (type == WheelType.Traveling)
        {
            Vector2 direction = (target - (Vector2)transform.position).normalized;
            //bool depasse;
            //if (movingX)
            //{
            //    depasse = (target.x - transform.position.x) < 0;
            //}
            //else
            //{
            //    depasse = (target.y - transform.position.y) < 0;
            //}

            if (Vector2.Distance(transform.position, target) < 0.05f)
            {
                sens *= -1;
                target = sens == 1 ? nextPos : transform.parent.position;
            }
            transform.Translate(travelingSpeed * Time.deltaTime * direction, Space.World);
            center.Translate(travelingSpeed * Time.deltaTime * direction, Space.World);
        }

        Vector2 lastPos = (type == WheelType.Traveling && movingX)
            ? nextPos
            : transform.parent.position;

        if (cam.position.x - lastPos.x > 15)
        {
            //GameManager.instance.CreateNewWheel(cam.position.x + 15);

            GameManager.instance.currentWheels.Remove(wheelNumber);
            AudioManager.instance.sfxSources.Remove(audioSource);
            Destroy(transform.parent.gameObject);
        }
    }
}
