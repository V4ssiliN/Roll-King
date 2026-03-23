using UnityEngine;

public class deathZone : MonoBehaviour
{
    public Transform player;
    public float cooldown = 0.25f;
    private float timer = 0f;
    
    private void Update()
    {
        transform.position = new Vector2(player.position.x, transform.position.y);
        timer += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && timer > cooldown)
        {
            HealthBar.instance.TakeDamage();
            timer = 0f;
        }
    }
}
