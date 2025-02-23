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
    public AudioClip hurting_sfx;

    [Header("Movement Settings")]
    public float _normalSpeed = 9f;
    public float _thrusterSpeed = 12f;
    private float followDelay = 0.2f;
    private Vector2 currentVelocity = Vector2.zero;

    private Rigidbody2D rb;

    [Header("Fuel Settings")]
    public float maxFuel = 100f;
    private float currentFuel;
    public float fuelConsumptionRate = 40f;
    public float fuelRechargeRate = 5f;
    private bool _isThrustActive = false;
    private bool isRecharging = true;

    [Header("Shooting Settings")]
    public GameObject[] bulletPrefabs;
    public Transform firePoint;
    public float damage = 1f;
    public float shotInterval = 4.2f;
    private float _lastShootTime = 0f;
    private Coroutine shootingCoroutine;

    // --- Upgrade & Stats ---
    public int bulletLevel = 0;
    public int maxBulletLevel = 4;
    private int powerupCount = 0;

    public float speedUpgradeIncrement = 0.75f;
    public float damageUpgradeIncrement = 0.5f;

    public float CurrentMovingSpeed => _isThrustActive ? _thrusterSpeed : _normalSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentFuel = maxFuel;
    }

    void Update()
    {
        HandleFuel();
        fuelText.text = "Fuel: " + currentFuel.ToString("F2");
        if (currentFuel > 65f)
            fuelText.color = Color.green;
        else if (currentFuel > 30f)
            fuelText.color = Color.yellow;
        else
            fuelText.color = Color.red;
    }

    void FixedUpdate()
    {
        Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 smoothedPos = Vector2.SmoothDamp(rb.position, targetPos, ref currentVelocity, followDelay);
        Vector2 displacement = smoothedPos - rb.position;
        float maxDisplacement = CurrentMovingSpeed * Time.fixedDeltaTime;
        if (displacement.magnitude > maxDisplacement)
        {
            displacement = displacement.normalized * maxDisplacement;
        }
        rb.MovePosition(rb.position + displacement);

        float turnValue = targetPos.x - rb.position.x;
        float deadZone = 1.65f;
        bool turnRight = turnValue > deadZone;
        bool turnLeft = turnValue < -deadZone;
        if (Mathf.Abs(turnValue) < deadZone)
        {
            turnRight = false;
            turnLeft = false;
        }
        animator.SetBool("TurnRight", turnRight);
        animator.SetBool("TurnLeft", turnLeft);
    }



    public void OnMove(InputAction.CallbackContext context)
    {
        // keyboard controls removed
    }

    public void OnThrust(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isThrustActive = true;
            isRecharging = false;
            animator.SetBool("IsThrustActive", true);
        }
        else if (context.canceled)
        {
            _isThrustActive = false;
            isRecharging = true;
            animator.SetBool("IsThrustActive", false);
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (Time.time - _lastShootTime >= shotInterval)
            {
                Fire();
                _lastShootTime = Time.time;
            }
            if (shootingCoroutine == null)
                shootingCoroutine = StartCoroutine(ContinuousShooting());
        }
        else if (context.canceled)
        {
            if (shootingCoroutine != null)
            {
                StopCoroutine(shootingCoroutine);
                shootingCoroutine = null;
            }
        }
    }

    private IEnumerator ContinuousShooting()
    {
        while (true)
        {
            yield return new WaitForSeconds(shotInterval);
            if (Time.time - _lastShootTime >= shotInterval)
            {
                Fire();
                _lastShootTime = Time.time;
            }
        }
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
                isRecharging = true;
            }
        }
        else if (!_isThrustActive && isRecharging && currentFuel < maxFuel)
        {
            currentFuel += fuelRechargeRate * Time.deltaTime;
            if (currentFuel > maxFuel)
                currentFuel = maxFuel;
        }
    }

    void Fire()
    {
        if (bulletPrefabs != null && firePoint != null)
        {
            src.clip = shooting_sfx;
            src.Play();
            animator.SetTrigger("Shoot");
            Instantiate(bulletPrefabs[bulletLevel], firePoint.position, firePoint.rotation);
        }
    }

    public void TakeDamage()
    {
        src.clip = hurting_sfx;
        src.Play();
        GameManager.instance.DecreaseLives();
        if (GameState.instance.Lives <= 0)
            Die();
    }

    private void Die()
    {
        GameManager.instance.LoadScene(2); // 2 for game over
        GameState.instance.Score = 0;
        GameState.instance.Lives = 3;
    }

    // --- Powerup / Upgrade System ---
    public void ApplyPowerup()
    {
        damage += damageUpgradeIncrement;
        powerupCount++;

        if (powerupCount % 3 == 0)
        {
            if (bulletLevel < maxBulletLevel)
            {
                bulletLevel++;
            }
        }
        else if (powerupCount % 5 == 0)
        {
            GameManager.instance.IncreaseLives();
        }
    }
}
