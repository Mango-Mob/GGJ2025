using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;
    [Header("Move Stats")]
    [SerializeField] private float maxSpeed = 4.0f;
    [SerializeField] private float moveAcceleration = 4.0f;
    [SerializeField] private float smoothTime = 0.3f;
    [SerializeField] private float minSpeed = 0.1f;
    [SerializeField] private float bounceMult = 2.0f;
    [SerializeField] private float startHeight = 10.0f;
    [SerializeField] private float resetSpeed = 12.0f;
    [SerializeField] private float resetRotateSpeed = 360.0f;
    [SerializeField] private float enemyShoveSpeed = 10.0f;
    private int reset_count = 0;

    [Header("Visuals")]
    [SerializeField] private GameObject model;
    [SerializeField] private ParticleSystem boostVFX;
    [SerializeField] private ParticleSystem boostVFX2;
    [SerializeField] private ParticleSystem scoreVFX;
    [SerializeField] private float rotationSmoothTime = 0.1f;
    [SerializeField] private GameObject biscuit;
    private float rotationVelocity = 0.0f;
    private Animator animator;

    private Vector2 velocity = Vector2.zero;
    private Rigidbody rb;

    // Boosting
    [Header("Boosting")]
    [SerializeField] private float boostSpeed = 10.0f;
    [SerializeField] private float boostDuration = 0.5f;
    [SerializeField] private float boostCooldown = 0.5f;
    [SerializeField] private float boostRechargeSpeed = 0.2f;
    [SerializeField] private float boostIFrameLingerTime = 0.5f;
    public bool isBoosting = false;
    public bool isBoostMove = false;
    private float boostValue;
    private float timeOfLastBoost;

    [Header("Stun Stats")]
    [SerializeField] private float healthDrain = 0.15f;
    [SerializeField] private float breakSpeed = 0.1f;
    private float health = 1.0f;
    private bool isStunned = false;
    public bool isDead = false;
    private bool isResetting = false;
    private float timeOfReset;

    [SerializeField] private MultiAudioAgent audioAgent;

    // Other scripts
    private BoostUI boostUI;
    private BreakoutUI breakoutUI;


    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();
        boostUI = GetComponentInChildren<BoostUI>();
        breakoutUI = GetComponentInChildren<BreakoutUI>();
        animator = model.GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        boostValue = boostUI.maxValue;
        CameraController.instance.SetCameraState(CameraController.CameraState.FOLLOW);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    StartCoroutine(Stun(null));
        //}
        //if (Input.GetKeyDown(KeyCode.R) && !isResetting)
        //{
        //    Debug.Log("Resetting");
        //    StartCoroutine(ResetToStart());
        //}

        if (!isResetting)
            CameraController.instance.SetCameraState(isDead ? CameraController.CameraState.DEAD : (isStunned ? CameraController.CameraState.ZOOMED : CameraController.CameraState.FOLLOW));
        
        boostUI.SetVisible(!isStunned && !isDead && !isResetting);
        breakoutUI.SetVisible(isStunned && !isDead && !isResetting);
        breakoutUI.SetHealth(health);
        biscuit.SetActive(isResetting);

        animator.SetBool("IsStunned", isStunned);

        HandleInput();
    }
    public void HandleInput()
    {
        if (isStunned || isDead || isResetting)
            return;

        bool usingGamepad = InputManager.Instance.isInGamepadMode;
        Vector2 movement = Vector2.zero;
        if (usingGamepad)
        {
            movement = InputManager.Instance.GetGamepadStick(StickType.LEFT, 0);
        }
        else
        {
            movement.x += InputManager.Instance.GetKey(KeyType.D).isPressed ? 1.0f : 0.0f;
            movement.x -= InputManager.Instance.GetKey(KeyType.A).isPressed ? 1.0f : 0.0f;
            movement.y += InputManager.Instance.GetKey(KeyType.W).isPressed ? 1.0f : 0.0f;
            movement.y -= InputManager.Instance.GetKey(KeyType.S).isPressed ? 1.0f : 0.0f;
            movement = movement.normalized;
        }

        bool boostAvailable = movement.magnitude > 0.0f;
        if (boostAvailable && InputManager.Instance.IsBindDown("Roll"))
        {
            StartCoroutine(Boost(movement));
        }

        // Boost value
        boostValue += Time.deltaTime * boostRechargeSpeed;
        boostValue = Mathf.Clamp(boostValue, 0.0f, boostUI.maxValue);
        boostUI.currentValue = boostValue;

        // Model rotation
        model.transform.localEulerAngles = new Vector3(0.0f, Mathf.SmoothDamp(model.transform.localEulerAngles.y, -movement.x * 90.0f + 90.0f, ref rotationVelocity, 0.1f), 0.0f);

        if (isBoostMove)
            return;

        // Funky drifting clamp
        if (movement.magnitude == 0.0f)
        {
            if (rb.velocity.magnitude == 0.0f)
            {
                rb.velocity = Vector2.right * minSpeed;
            }

            movement = rb.velocity.normalized * minSpeed;
            movement.y = Mathf.Min(movement.y, 0.0f);
        }
        else if (movement.magnitude < minSpeed)
        {
            movement = movement.normalized * minSpeed;
            movement.y = Mathf.Min(movement.y, 0.0f);
        }

        Move(movement);
    }
    public void Move(Vector2 _move)
    {
        rb.velocity = Vector2.SmoothDamp(rb.velocity, _move * maxSpeed, ref velocity, smoothTime);
    }
    IEnumerator Boost(Vector2 _boostDir)
    {
        if (boostValue < 1.0f || Time.time < timeOfLastBoost + boostCooldown)
        {
            // Can not dash - either on cooldown or not enough resource
            audioAgent.Play("DashNotReady", false, Random.Range(0.75f, 1.25f));

            yield break;
        }

        audioAgent.Play("Dash", false, Random.Range(0.75f, 1.25f));

        timeOfLastBoost = Time.time;
        boostValue -= 1.0f;

        isBoosting = true;
        isBoostMove = true;
        rb.velocity = _boostDir.normalized * boostSpeed;

        boostVFX.Play();
        boostVFX2.Play();
        boostVFX.transform.forward = _boostDir;

        yield return new WaitForSeconds(boostDuration);
        isBoostMove = false;

        yield return new WaitForSeconds(boostIFrameLingerTime);

        isBoosting = false;

    }
    public void HitPlayer(Bubble _bubble)
    {
        if (!isStunned && !isResetting && timeOfReset < Time.time + 0.5f) 
        {
            StartCoroutine(Stun(_bubble));
        }
    }
    IEnumerator Stun(Bubble _bubble)
    {
        isStunned = true;
        Debug.Log("Stunned");

        float breakHealth = 1.0f;
        rb.velocity = Vector3.zero;

        if (GameManager.Instance)
            GameManager.Instance.TogglePause();

        // Teleport bubble
        if (_bubble)
            _bubble.transform.position = transform.position;

        int lastDirection = 0;
        breakoutUI.SetNextInput(0);

        audioAgent.Play("OnHitSound", false, Random.Range(0.75f, 1.25f));

        audioAgent.Play("Struggle", false, Random.Range(0.75f, 1.25f));

        while (breakHealth > 0.0f)
        {
            bool breaking = false;
            if (lastDirection >= 0 && (InputManager.Instance.GetGamepadStick(StickType.LEFT, 0).x < 0.0f || InputManager.Instance.GetKey(KeyType.A).wasPressedThisFrame))
            {
                lastDirection = -1;
                breaking = true;
                breakoutUI.SetNextInput(-1);
            }
            else if (lastDirection <= 0 && (InputManager.Instance.GetGamepadStick(StickType.LEFT, 0).x > 0.0f || InputManager.Instance.GetKey(KeyType.D).wasPressedThisFrame))
            {
                lastDirection = 1;
                breaking = true;
                breakoutUI.SetNextInput(1);
            }

            if (breaking)
            {
                breakHealth -= breakSpeed;
            }
            health -= healthDrain * Time.deltaTime;

            if (health <= 0.0f)
            {
                // End sound
                audioAgent.StopAudio("Struggle");

                // Kill player
                isDead = true;
                animator.SetBool("IsDead", true);

                if (GameManager.Instance)
                    GameManager.Instance.TogglePause();

                float timeOfDeath = Time.time;
                bool loadedLevel = false;

                while (true)
                {
                    if (_bubble != null)
                    {
                        transform.position = _bubble.transform.position;
                    }
                    else
                    {
                        rb.velocity = new Vector3(0.0f, rb.velocity.y, 0.0f);
                        rb.useGravity = true;
                    }

                    if (Time.time > timeOfDeath + 1.0f)
                    {
                        if (_bubble)
                            _bubble?.Pop(false);
                    }

                    if (Time.time > timeOfDeath + 5.0f && !loadedLevel)
                    {
                        LevelManager.Instance.LoadNewLevel("EndScene");
                        loadedLevel = true;
                    }

                    yield return new WaitForEndOfFrame();
                }
            }
            yield return new WaitForEndOfFrame();
        }

        audioAgent.StopAudio("Struggle");

        if (GameManager.Instance)
            GameManager.Instance.TogglePause();

        if (_bubble)
            _bubble?.Pop(false);

        isStunned = false;
    }
    IEnumerator ResetToStart()
    {
        isResetting = true;

        if (GameManager.Instance)
            GameManager.Instance.TogglePause();

        CameraController.instance.SetCameraState(CameraController.CameraState.OBJECTIVE);

        // Get direction to start
        Vector3 startPos = new Vector3(0.0f, startHeight, transform.position.z);

        //animator.SetBool("IsResetting", true);

        audioAgent.Play("Score", false, Random.Range(0.75f, 1.25f));

        scoreVFX.Play();

        if (GameManager.Instance)
            GameManager.Instance.score += 500;

        while (transform.position.y < startPos.y)
        {
            //transform.position = Vector3.MoveTowards(transform.position, startPos, resetSpeed * Time.deltaTime);
            model.transform.localEulerAngles = new Vector3(0.0f, model.transform.localEulerAngles.y - resetRotateSpeed * Time.deltaTime, 0.0f);

            rb.velocity = (startPos - transform.position).normalized * resetSpeed * (reset_count <= 0 ? 1.0f : 1.5f);

            yield return new WaitForEndOfFrame();
        }

        rb.velocity = Vector3.zero;

        // Wait for dog animation
        yield return new WaitForEndOfFrame();

        if (GameManager.Instance)
            GameManager.Instance.TogglePause();

        model.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        timeOfReset = Time.time;
        isResetting = false;
        reset_count++;

        CameraController.instance.SetCameraState(CameraController.CameraState.FOLLOW);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Entered");
        if (other.gameObject.layer == LayerMask.NameToLayer("Objective") && !isResetting && !isDead)
        {
            Debug.Log("Objective Entered");
            GameManager.Instance.WakeDogs();
            StartCoroutine(ResetToStart());
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided");
        if (collision.transform.tag == "Enemy" && !isResetting)
        {
            audioAgent.Play("MosquitoHit", false, Random.Range(0.75f, 1.25f));
            Vector3 direction = transform.position - collision.transform.position;
            rb.velocity = direction.normalized * enemyShoveSpeed;

            boostValue -= 1.0f;
            boostValue = Mathf.Clamp(boostValue, 0.0f, 2.0f);
        }

        return;
    }
}
