using UnityEngine;

public class backgroundParallax : MonoBehaviour
{
    public Transform cam;
    public float length = 79.166f;
    [Range(0,1)]
    public float multiplier;

    float cameraXPos;

    private void Start()
    {
        cameraXPos = cam.position.x;
    }

    private void Update()
    {
        float cameraXMove = cam.position.x - cameraXPos;
        transform.position = new Vector2(transform.position.x + cameraXMove * multiplier,transform.position.y);
        cameraXPos = cam.position.x;
        if(cam.position.x - transform.position.x > length)
        {
            transform.position = new Vector2(transform.position.x + 2*length, transform.position.y);
        }
        else if(transform.position.x - cam.position.x > length)
        {
            transform.position = new Vector2(transform.position.x - 2*length, transform.position.y);
        }

        Vector2 newPos = transform.position;
        newPos.x = Mathf.Round(newPos.x / length) * length;
        transform.position = newPos;
    }
}
