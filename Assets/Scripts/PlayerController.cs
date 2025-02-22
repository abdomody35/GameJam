using System;
using System.Runtime.CompilerServices;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fuelText;
    public AudioSource src;
    public AudioClip shooting_sfx;
    public static float playerLevel = -1f;

    [Header("Movement Settings")]
    public float _normalSpeed = 10f;
    public float _thrusterSpeed = 15f;
    private Vector2 moveInput;
    private bool _isMoving = false;

    [Header("Fuel Settings")]
    public float maxFuel = 100f;
    public float fuelConsumptionRate = 15f;
    public float fuelRechargeRate = 30f;
    public float fuelRechargeDelay = 2f;
    private float currentFuel;
    public Slider fuelSlider;
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

    // Is the thrust button currently held down?

    public float CurrentMovingSpeed
    {
        get
        {
            if (_isMoving)
            {
                _isThrustActive? _thrusterSpeed : _normalSpeed;
            }
            else
            {
                return 0;
            }
        }
    }

    public bool IsMoving
    {
        get
        {
            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            //animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    public bool IsThrustActive
    {
        get
        {
            return _isThrustActive;
        }
        private set
        {
            _isThrustActive = value;
            //animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Initialize fuel and update the slider, if available
        currentFuel = maxFuel;
    }

    void Update()
    {
        // Update the fire cooldown timer
        if (fireCooldown > 0)
            fireCooldown -= Time.deltaTime;

        // Handle fuel consumption and recharge
        HandleFuel();

        // Update the fuel text
        fuelText.text = "Fuel: %" + currentFuel.ToString("F2");

    }

    void FixedUpdate()
    {
        if (fireCooldown > 0)
            fireCooldown -= Time.deltaTime;
        // Determine speed based on whether thruster is active and fuel is available
        rb.linearVelocity = new Vector2(moveInput.x * CurrentMovingSpeed, moveInput.y * CurrentMovingSpeed);

        if (_isShooting && fireCooldown <= 0)
        {
            Fire();
            fireCooldown = fireRate;
        }
    }

    // This method is called by the PlayerInput component when the Move action is triggered.
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        IsMoving = (moveInput != Vector2.zero);
    }

    // This method is called by the PlayerInput component when the Thrust action is triggered.
    public void OnThrust(InputAction.CallbackContext context)
    {
        // If the thrust button is pressed and there's fuel, enable thruster mode.
        if (context.started)
        {
            _isThrustActive = true;
            isRecharging = false; // Stop recharging when thrusting
            StopCoroutine(FuelRechargeDelay());
        }
        else if (context.canceled)
        {
            _isThrustActive = false;
            StartCoroutine(FuelRechargeDelay());
        }
    }

    private IEnumerator FuelRechargeDelay()



    {


        yield return new WaitForSeconds(fuelRechargeDelay);


        isRecharging = true;


    }





    private void HandleFuel()
    {
        if (_isThrustActive && currentFuel > 0)
        {
            currentFuel -= fuelConsumptionRate * Time.deltaTime;
            if (currentFuel <= 0)
            {
                currentFuel = 0;
                _isThrustActive = false;
                StartCoroutine(FuelRechargeDelay());
            }
        }
        else if (!_isThrustActive && isRecharging && currentFuel < maxFuel)
        {
            currentFuel += fuelRechargeRate * Time.deltaTime;
            if (currentFuel > maxFuel)
                currentFuel = maxFuel;
        }

        // Update the fuel text
        fuelText.text = "Fuel: %" + currentFuel.ToString("F2");
    }

    // This method is called by the PlayerInput component when the Attack action is triggered.
    public void OnShoot(InputAction.CallbackContext context)
    {
        // Fire a bullet if the button is pressed and the cooldown has elapsed.
        //if (value.isPressed && fireCooldown <= 0)
        if (context.started)
        {
            _isShooting = true;
        }
        else if (context.canceled)
        {
            _isShooting = false;
        }
    }

    void Fire()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            src.clip = shooting_sfx;
            src.Play();
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }

    public void TakeDamage(int damage)
    {
        // Reduce player health and check for death
        GameManager.instance.DecreaseLives();
        //Debug.Log("Player hit! Lives remaining: " + _lives);

        if (GameManager.instance.Lives <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player is dead!");
        // Trigger game over logic, play explosion, etc.
        Destroy(gameObject);
    }
}
