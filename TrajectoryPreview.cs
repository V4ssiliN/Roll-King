using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryPreview : MonoBehaviour
{
    public Rigidbody2D rb;                 // Le Rigidbody du perso
    public Transform pointPrefab;        // Prefab d'un point
    public int numberOfPoints = 20;      // Nombre de points dans la trajectoire
    public float pointSpacing = 0.1f;    // Temps entre deux points
    public LayerMask obstacleLayerMask;

    public Vector2 offset;

    private List<Transform> points = new List<Transform>();

    public static TrajectoryPreview instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Il y a plus d'une instance de TrajectoryPreview dans la scène");
            return;
        }

        instance = this;
    }

    void Start()
    {
        rb = herosMouvements.instance.rb;
        
        // Crée les points au début
        for (int i = 0; i < numberOfPoints; i++)
        {
            Transform p = Instantiate(pointPrefab, transform.position, Quaternion.identity);
            p.gameObject.SetActive(false);
            points.Add(p);
        }
    }

    public void ShowTrajectory(Vector3 force)
    {
        bool hideNextPoints = false;
        
        Vector2 currentPosition = rb.position + (Vector2)transform.TransformDirection(offset);
        Vector2 velocity = force / rb.mass * Time.fixedDeltaTime; // force = vitesse initiale

        for (int i = 0; i < numberOfPoints; i++)
        {
            Vector2 nextPosition = currentPosition + velocity * pointSpacing;
            velocity += Physics2D.gravity * pointSpacing;

            RaycastHit2D hit = Physics2D.Raycast(currentPosition, nextPosition - currentPosition,
                Vector2.Distance(currentPosition, nextPosition),
                obstacleLayerMask);

            bool dropping;
            if (rb.transform.up.y <= 0.1f)
            {
                dropping = false;
            }
            else
            {
                dropping = velocity.y <= 0 && (herosMouvements.instance.charge > 200 || i > numberOfPoints/3);
            }

            if (hit.collider != null && (hit.transform.parent != transform.parent || dropping))
            {
                // on a touché un obstacle donc on s'arrête ici
                points[i].position = hit.point;
                points[i].gameObject.SetActive(true); // dernier point à l’impact
                for (int j = i + 1; j < numberOfPoints; j++)
                {
                    points[j].gameObject.SetActive(false);
                }
                break;
            }

            points[i].position = currentPosition;
            points[i].gameObject.SetActive(true);
            currentPosition = nextPosition;
        }
    }
    public void HideTrajectory()
    {
        foreach (var p in points)
            p.gameObject.SetActive(false);
    }
}
