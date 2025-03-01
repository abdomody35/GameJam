using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private TextMeshProUGUI fuelText;
    public AudioSource src;
    public AudioClip shooting_sfx;
    public AudioClip hurting_sfx;

    [Header("Movement Settings")]
    public float _normalSpeed = 10f;
    public float _thrusterSpeed = 15f;
    private readonly float followDelay = 0.2f;
    private Vector2 currentVelocity = Vector2.zero;

    private Rigidbody2D rb;

    [Header("Fuel Settings")]
    public float maxFuel = 50f;
    public float fuelConsumptionRate = 25f;
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

    public float speedUpgradeIncrement = 0.75f;
    public float damageUpgradeIncrement = 0.5f;

    private float _currentMovingSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        GameState.instance.CurrentFuel = maxFuel;
        _currentMovingSpeed = _normalSpeed;
    }

    void Update()
    {
        HandleFuel();
        fuelText.text = "Fuel: " + (GameState.instance.CurrentFuel * 2).ToString("F2");
        if (GameState.instance.CurrentFuel * 2 > 65f)
            fuelText.color = Color.green;
        else if (GameState.instance.CurrentFuel * 2 > 30f)
            fuelText.color = Color.yellow;
        else
            fuelText.color = Color.red;
    }

    void FixedUpdate()
    {
        Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 smoothedPos = Vector2.SmoothDamp(rb.position, targetPos, ref currentVelocity, followDelay);
        Vector2 displacement = smoothedPos - rb.position;
        float maxDisplacement = _currentMovingSpeed * Time.fixedDeltaTime;
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
            _currentMovingSpeed = _thrusterSpeed;
            _isThrustActive = true;
            isRecharging = false;
            animator.SetBool("IsThrustActive", true);
        }
        else if (context.canceled)
        {
            _currentMovingSpeed = _normalSpeed;
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
        if (_isThrustActive && GameState.instance.CurrentFuel > 0)
        {
            GameState.instance.CurrentFuel -= fuelConsumptionRate * Time.deltaTime;
            if (GameState.instance.CurrentFuel <= 0)
            {
                GameState.instance.CurrentFuel = 0;
                _isThrustActive = false;
                isRecharging = true;
            }
        }
        else if (!_isThrustActive && isRecharging && GameState.instance.CurrentFuel < maxFuel)
        {
            GameState.instance.CurrentFuel += fuelRechargeRate * Time.deltaTime;
            if (GameState.instance.CurrentFuel > maxFuel)
                GameState.instance.CurrentFuel = maxFuel;
        }
    }

    void Fire()
    {
        if (bulletPrefabs != null && firePoint != null)
        {
            src.clip = shooting_sfx;
            src.Play();
            animator.SetTrigger("Shoot");
            Instantiate(bulletPrefabs[GameState.instance.BulletLevel], firePoint.position, firePoint.rotation);
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
        SceneManager.LoadScene(2); // 2 for game over
        GameState.instance.Reset();
    }

    // --- Powerup / Upgrade System ---
    public void ApplyPowerup()
    {
        damage += damageUpgradeIncrement;
        GameState.instance.PowerupCount++;

        if (GameState.instance.PowerupCount % 3 == 0)
        {
            if (GameState.instance.BulletLevel < GameState.instance.MaxBulletLevel)
            {
                GameState.instance.BulletLevel++;
            }
        }
        else if (GameState.instance.PowerupCount % 5 == 0)
        {
            GameManager.instance.IncreaseLives();
        }
    }
}
