using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private TextMeshProUGUI fuelText;
    public AudioSource src;
    public AudioClip shooting_sfx;

    [Header("Movement Settings")]
    public float _normalSpeed = 10f;
    public float _thrusterSpeed = 15f;
    private Vector2 moveInput;
    private bool _isMoving = false;

    [Header("Follow Mouse Settings")]
    public bool followMouse = true;          // Implicitly switched based on input
    public float followDelay = 0.2f;           // Delay (smooth time) for following the mouse
    private Vector2 currentVelocity = Vector2.zero;
    private Vector2 lastMousePosition;         // Track last frame's mouse position
    public float mouseMoveThreshold = 5f;      // Pixels moved to trigger mouse mode

    [Header("Fuel Settings")]
    public float maxFuel = 100f;
    public float fuelConsumptionRate = 15f;
    public float fuelRechargeRate = 30f;
    public float fuelRechargeDelay = 2f;       // Wait time before fuel starts recharging
    private float currentFuel;
    private bool _isThrustActive = false;
    private bool isRecharging = false;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.25f;
    private float fireCooldown;
    private bool _isShooting = false;

    // Reference to the Rigidbody2D component
    private Rigidbody2D rb;

    public float CurrentMovingSpeed
    {
        get
        {
            if (_isMoving)
            {
                return _isThrustActive ? _thrusterSpeed : _normalSpeed;
            }
            else
            {
                return 0;
            }
        }
    }

    public bool IsMoving
    {
        get { return _isMoving; }
        private set { _isMoving = value; }
    }

    public bool IsThrustActive
    {
        get { return _isThrustActive; }
        private set
        {
            _isThrustActive = value;
            animator.SetBool("IsThrustActive", value);
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Ensure your GameObject has an Animator component
    }

    void Start()
    {
        currentFuel = maxFuel;
        lastMousePosition = Input.mousePosition;
    }

    void Update()
    {
        if (fireCooldown > 0)
            fireCooldown -= Time.deltaTime;

        // Implicitly switch to mouse-follow mode if mouse movement is significant
        Vector2 currentMousePos = Input.mousePosition;
        if (Vector2.Distance(currentMousePos, lastMousePosition) > mouseMoveThreshold)
        {
            followMouse = true;
        }
        lastMousePosition = currentMousePos;

        // Handle fuel consumption and recharging
        HandleFuel();
        fuelText.text = "Fuel: %" + currentFuel.ToString("F2");
        
    }

    void FixedUpdate()
    {
        if (fireCooldown > 0)
            fireCooldown -= Time.deltaTime;

        if (followMouse)
        {
            // Convert mouse position from screen space to world space and follow with smoothing
            Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 newPos = Vector2.SmoothDamp(rb.position, targetPos, ref currentVelocity, followDelay);
            rb.MovePosition(newPos);
        }
        else
        {
            // Move using keyboard input
            rb.linearVelocity = new Vector2(moveInput.x * CurrentMovingSpeed, moveInput.y * CurrentMovingSpeed);
        }

        // Calculate a turn value based on input method
        float turnValue = 0f;
        if (!followMouse)
        {
            // Use keyboard horizontal input for turning
            turnValue = moveInput.x;
        }
        else
        {
            // For mouse mode, calculate difference between mouse's x and the ship's x
            Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            turnValue = targetPos.x - rb.position.x;
            // Optionally clamp or scale turnValue if needed
        }

        // Define a dead zone threshold
        float deadZone = 0.65f;
        bool turnRight = turnValue > deadZone;
        bool turnLeft = turnValue < -deadZone;

        // If the absolute turn value is within the dead zone, don't play any turn animation
        if (Mathf.Abs(turnValue) < deadZone)
        {
            turnRight = false;
            turnLeft = false;
        }

        // Set the animator parameters
        animator.SetBool("TurnRight", turnRight);
        animator.SetBool("TurnLeft", turnLeft);
        // Shooting logic: fire while the shooting button is held and cooldown permits
        if (_isShooting && fireCooldown <= 0)
        {
            Fire();
            fireCooldown = fireRate;
        }
    }

    // Called by the PlayerInput component for movement (keyboard mode)
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        IsMoving = (moveInput != Vector2.zero);
        if (moveInput != Vector2.zero)
            followMouse = false; // Switch to keyboard mode if keyboard movement is detected
    }

    // Called by the PlayerInput component for thrust
    public void OnThrust(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsThrustActive = true;
            isRecharging = false;
            StopCoroutine("FuelRechargeDelay");
        }
        else if (context.canceled)
        {
            IsThrustActive = false;
            StartCoroutine("FuelRechargeDelay");
        }
    }

    // Called by the PlayerInput component for shooting
    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isShooting = true;
        }
        else if (context.canceled)
        {
            _isShooting = false;
        }
    }

    // Coroutine to delay fuel recharge after thrusting stops
    private IEnumerator FuelRechargeDelay()
    {
        yield return new WaitForSeconds(fuelRechargeDelay);
        isRecharging = true;
    }

    // Handle fuel consumption when thrusting and recharging when not
    private void HandleFuel()
    {
        if (IsThrustActive && currentFuel > 0)
        {
            currentFuel -= fuelConsumptionRate * Time.deltaTime;
            if (currentFuel <= 0)
            {
                currentFuel = 0;
                IsThrustActive = false;
                StartCoroutine(FuelRechargeDelay());
            }
        }
        else if (!IsThrustActive && isRecharging && currentFuel < maxFuel)
        {
            currentFuel += fuelRechargeRate * Time.deltaTime;
            if (currentFuel > maxFuel)
                currentFuel = maxFuel;
        }
    }

    void Fire()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            src.clip = shooting_sfx;
            src.Play();
            animator.SetTrigger("Shoot");
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }

    public void TakeDamage()
    {
        GameManager.instance.DecreaseLives();
        if (GameManager.instance.Lives <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.instance.LoadScene(2); // 2 for game over
    }
}
