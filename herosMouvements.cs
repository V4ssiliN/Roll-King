using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class herosMouvements : MonoBehaviour
{
    bool canCollide = true;
    [HideInInspector]
    public bool onWheel = false;
    
    public float jumpForce = 1000;
    public float charge = 100;

    //[HideInInspector]
    public Transform lastWheelOn;

    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public Transform firstWheel;

    public Animator animator;
    public List<AnimationClip> walkAnims;
    public List<AnimationClip> idleAnims;
    public List<AnimationClip> chargeAnims;
    public List<AnimationClip> jumpAnims;
    public int skinCount = 0;
    public int skinIndex = 0;
    public bool unlockAllSkins = false;

    public int[] skinsScoreRequierements;

    public static herosMouvements instance;

    private float vel;
    private bool justOnWheel = false;

    private bool doneWalking = false;

    public bool holding = false;
    public bool charging = false;

    public bool clicDown;
    public bool clic;
    public bool clicUp;

    private bool pointerOnUI = false;

    public Camera cam;

    public Material mat;
    [Range(0, 1)] public float flashIntensity;
    public float flashDuration;
    public int flashCount;

    public float chargingYDiff = .25f;

    public float tapThreshold = 0.3f;
    public float minSwipeDistance = 50f;
    private float pressStartTime;
    private Vector2 pressStartPosition;

    public bool isMovingOnWheel = false;
    public float angleMoving = 180f;
    public float movingSpeed = 20f;
    public float movingDuration = 0.3f;
    private float movingTimer;
    public tourner parentWheelScript;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Il y a plus d'une instance de herosMouvements dans le scène !");
            return;
        }
        instance = this;

        mat = Instantiate(spriteRenderer.material);
        spriteRenderer.material = mat;
    }

    void Start()
    {
        mat.SetFloat("_FlashAmount", 0f);
        Color c = mat.color;
        c.a = 1f;
        mat.color = c;

        ChargeBar.instance.SetCharge(charge);
        onWheel = false;
        StartCoroutine(StartGame());
        transform.localRotation = Quaternion.identity;

        skinCount = walkAnims.Count;
        animator.Play(walkAnims[skinIndex].name);
        animator.speed = 1;

        ChargeBar.instance.slider.minValue = charge;
        ChargeBar.instance.slider.maxValue = jumpForce;

        cam = Camera.main;
    }

    void Update()
    {
        clicDown = Input.GetButtonDown("Jump") || Input.GetMouseButtonDown(0);
        clic = Input.GetButton("Jump") || Input.GetMouseButton(0);
        clicUp = Input.GetButtonUp("Jump") || Input.GetMouseButtonUp(0);

        pointerOnUI = EventSystem.current.IsPointerOverGameObject() || (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId));

        if (isMovingOnWheel)
        {
            if (movingTimer > movingDuration)
            {
                isMovingOnWheel = false;
                parentWheelScript.fakeWheelSpeedOffset = 0f;
                transform.SetParent(transform.parent.parent);
            }
            movingTimer += Time.deltaTime;
        }
        else
        {
            if (onWheel)
            {
                if (!pointerOnUI && (clicDown || (justOnWheel && clic)))
                {
                    holding = true;
                    pressStartPosition = Input.mousePosition;
                }
                else if (clic && holding)
                {
                    charge = Mathf.Min(jumpForce, charge + (jumpForce / 2) * Time.deltaTime);

                    if (charge > Helpers.Map(tapThreshold, 0f, 2f, 0f, jumpForce, true))
                    {
                        if (!charging)
                        {
                            charging = true;

                            AudioManager.instance.PlaySfx("charging");
                            LeanTween.cancel(ChargeBar.instance.gameObject);
                            ChargeBar.instance.Appear();
                            animator.Play(chargeAnims[skinIndex].name);
                            animator.speed = 1;
                        }
                        float downY = Helpers.Map(charge, ChargeBar.instance.slider.minValue,
                            ChargeBar.instance.slider.maxValue, 0f, chargingYDiff, false);

                        spriteRenderer.transform.localPosition = new Vector2(0f, -downY);

                        ChargeBar.instance.SetCharge(charge);

                        if (GameManager.instance.showTrajectory) TrajectoryPreview.instance.ShowTrajectory(transform.TransformDirection(new Vector2(0, charge)));
                    }
                }
                else if (clicUp)
                {
                    if (charging)
                    {
                        spriteRenderer.transform.localPosition = Vector2.zero;
                        if (GameManager.instance.showTrajectory) TrajectoryPreview.instance.HideTrajectory();
                        ChargeBar.instance.SetCharge(charge);
                        LeanTween.cancel(ChargeBar.instance.gameObject);
                        ChargeBar.instance.Fade();

                        if (GameManager.instance.gameScore > 0 || charge > 300)
                        {
                            AudioManager.instance.PlaySfx("swoosh");
                            StartCoroutine(WillCollide());
                            onWheel = false;
                            transform.rotation = Quaternion.identity;
                            rb.velocity = Vector3.zero;
                            rb.AddRelativeForce(new Vector2(0, charge));
                            transform.parent = null;
                            rb.gravityScale = 1f;
                            //animator.SetBool("charging", false);
                            //animator.SetBool("onWheel", false);
                            animator.Play(jumpAnims[skinIndex].name, 0, 0.1f);
                            animator.speed = 0;
                        }


                    }
                    else
                    {
                        DetectSwipe();
                    }
                    charge = 0;
                    charging = false;
                }
            }
            else
            {
                if (doneWalking && rb.velocity.y < 0)
                {
                    animator.Play(jumpAnims[skinIndex].name, 0, 0.6f);
                    animator.speed = 0;
                }
            } 
        }
    }

    private void FixedUpdate()
    {
        vel = rb.velocity.x * rb.velocity.x + rb.velocity.y * rb.velocity.y;
        if (justOnWheel) justOnWheel = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "roue")
        {
            bool visible = Helpers.IsVisibleFromVirtualCam(transform.position, cam.transform.position,
                cam.orthographicSize,
                cam.aspect, .5f);
            if (visible)
            {
                AudioManager.instance.PlaySfx("landing" + Mathf.Clamp((int)vel / 40, 0, 2));
            }
            
            if (transform.position.y > collision.transform.position.y && canCollide)
            {
                if (!visible)
                {
                    AudioManager.instance.PlaySfx("landing" + Mathf.Clamp((int)vel / 40, 0, 2));
                }
                
                PutOnWheel(collision.transform);
                
                if (transform.parent != lastWheelOn)
                {
                    lastWheelOn = transform.parent;
                }
                GameManager.instance.SetScore(transform.parent.GetComponent<tourner>().wheelNumber);

                SuivreJoueur.instance.CalculateTarget();
            }
        }
    }
    public void PutOnWheel(Transform wheel)
    {
        justOnWheel = true;
        onWheel = true;
        canCollide = false;
        rb.gravityScale = 0f;
        rb.velocity = Vector3.zero;
        transform.SetParent(wheel);
        parentWheelScript = wheel.gameObject.GetComponent<tourner>();
        Vector2 direction = transform.position - transform.parent.position;
        transform.up = direction;

        animator.Play(idleAnims[skinIndex].name);
        animator.speed = 1;

        if (clic)
        {
            LeanTween.cancel(ChargeBar.instance.gameObject);
            ChargeBar.instance.Appear();
        }
    }

    public void ActuAnim()
    {
        if (!doneWalking)
        {
            animator.speed = 1;
            animator.Play(walkAnims[skinIndex].name);
            return;
        }

        if (onWheel)
        {
            animator.speed = 1;
            if(Input.GetButton("Jump") || Input.GetMouseButton(0))
            {
                animator.Play(chargeAnims[skinIndex].name);
            }
            else
            {
                animator.Play(idleAnims[skinIndex].name);
            }
        }
        else
        {
            animator.speed = 0;
            animator.Play(jumpAnims[skinIndex].name, 0, rb.velocity.y < 0f ? 0.6f : 0.1f);
        }
    }

    void DetectSwipe()
    {
        Vector2 endPosition = Input.mousePosition;
        float diffX = endPosition.x - pressStartPosition.x;
        if (Mathf.Abs(diffX) > minSwipeDistance)
        {
            if (diffX < 0) Move(1);
            else Move(-1);
            Debug.Log("swipe");
        }
    }

    void Move(int direction)
    {
        isMovingOnWheel = true;
        transform.SetParent(transform.parent.GetChild(1));
        movingSpeed = angleMoving / movingDuration;
        parentWheelScript.fakeWheelSpeedOffset = direction * movingSpeed;
        movingTimer = 0f;
    }

    IEnumerator WillCollide()
    {
        yield return new WaitForSeconds(.1f);
        canCollide = true;
    }

    IEnumerator StartGame()
    {
        while (transform.position.x<-7.5)
        {
            transform.Translate(new Vector2(.2f,0f));
            yield return new WaitForSeconds(0.05f);
        }
        rb.gravityScale = 0;
        rb.velocity = Vector3.zero;
        lastWheelOn = firstWheel;

        Transform butte = firstWheel.GetComponent<tourner>().previousWheel.transform;
        transform.SetParent(butte);

        transform.localPosition = new Vector2(0.75f, 0.92f);
        onWheel = true;
        canCollide = false;
        doneWalking = true;
        Vector2 direction = transform.position - transform.parent.position;
        transform.up = direction;

        //SuivreJoueur.instance.CalculateTarget();

        if (clic && !pointerOnUI)
        {
            LeanTween.cancel(ChargeBar.instance.gameObject);
            ChargeBar.instance.Appear();
        }

        //animator.SetBool("onWheel", true);
        animator.Play(idleAnims[skinIndex].name);

        yield return new WaitUntil(() => charge > 300);

        TitlePanel.instance.RemovePanel();
        HelpText.instance.ChangeText();

        yield return new WaitUntil(() => GameManager.instance.gameScore > 0);
        butte.gameObject.SetActive(false);

        yield return new WaitUntil(() => GameManager.instance.gameScore > 1);
        yield return new WaitForSeconds(.5f);
        Destroy(butte.parent.gameObject);
    }

    public IEnumerator Flash()
    {
        float t;

        for (int i = 0; i < flashCount; i++)
        {
            t = 0f;
            while (t < flashDuration)
            {
                t += Time.deltaTime;
                float normalized = Mathf.Clamp01(t / flashDuration);
                mat.SetFloat("_FlashAmount", normalized * flashIntensity);
                yield return null;
            }

            t = 0f;
            while (t < flashDuration)
            {
                t += Time.deltaTime;
                float normalized = Mathf.Clamp01(1 - (t / flashDuration));
                mat.SetFloat("_FlashAmount", normalized * flashIntensity);
                yield return null;
            } 
        }

        mat.SetFloat("_FlashAmount", 0f);
    }
}
