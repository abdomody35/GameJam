using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public int _lives = 3;
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
    private float currentFuel;
    public Slider fuelSlider;
    private bool _isThrustActive = false;

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
                if (_isThrustActive)
                {
                    return _thrusterSpeed;
                }
                else
                {
                    return _normalSpeed;
                }
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
        if (fuelSlider != null)
        {
            fuelSlider.maxValue = maxFuel;
            fuelSlider.value = currentFuel;
        }
    }

    //void Update()
    //{
    //    // Update the fire cooldown timer
    //    if (fireCooldown > 0)
    //    {
    //        fireCooldown -= Time.deltaTime;
    //    }

    //    // Recharge fuel when not thrusting
    //    if (!isThrustActive && currentFuel < maxFuel)
    //    {
    //        currentFuel += fuelRechargeRate * Time.deltaTime;
    //        currentFuel = Mathf.Clamp(currentFuel, 0, maxFuel);
    //        if (fuelSlider != null)
    //            fuelSlider.value = currentFuel;
    //    }
    //}

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
            IsThrustActive = true;
        }
        else if (context.canceled)
        {
            IsThrustActive = false;
        }
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
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }

    public void TakeDamage(int damage)
    {
        // Reduce player health and check for death
        _lives -= damage;
        Debug.Log("Player hit! Lives remaining: " + _lives);

        if (_lives <= 0)
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
