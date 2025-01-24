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
    [SerializeField] private float boostSpeed = 10.0f;
    [SerializeField] private float boostDuration = 0.5f;
    [SerializeField] private float minSpeed = 0.1f;

    [Header("Visuals")]
    [SerializeField] private GameObject model;
    [SerializeField] private ParticleSystem boostVFX;
    [SerializeField] private float rotationSmoothTime = 0.1f;
    private float rotationVelocity = 0.0f;

    private Vector2 velocity = Vector2.zero;
    private Rigidbody rb;
    private bool isBoosting = false;

    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }
    public void HandleInput()
    {
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

        // Model rotation
        model.transform.localEulerAngles = new Vector3(0.0f, Mathf.SmoothDamp(model.transform.localEulerAngles.y, -movement.x * 90.0f + 90.0f, ref rotationVelocity, 0.1f), 0.0f);

        if (isBoosting)
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
        isBoosting = true;
        rb.velocity = _boostDir.normalized * boostSpeed;

        boostVFX.Play();
        boostVFX.transform.forward = _boostDir;

        yield return new WaitForSeconds(boostDuration);
        isBoosting = false;
    }
}
