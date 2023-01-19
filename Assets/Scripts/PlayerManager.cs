using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


[RequireComponent(typeof(InputHandler))]
public class PlayerManager : MonoBehaviour
{
    private struct FrameInputs
    {
        public float X, Z;
        public int RawX, RawZ;
    }

    private FrameInputs _inputs;
    Vector3 MousePosition;

    [SerializeField]
    private bool RotateTowardMouse;

    [SerializeField]
    private Camera Camera;

    Rigidbody rb;

    [SerializeField] GameObject projectile;
    [SerializeField] Transform[] shootingPos;
    [SerializeField] float projectileSpeed = 15f;
    [SerializeField] float shootingTimer = 0.125f;
    bool shotFired = false;

    [SerializeField] VisualEffect shootVFX;

    [SerializeField] float playerHealth = 100f;
    float currentPlayerHealth;
    [SerializeField] float hitsCooldown = 1.5f;
    bool playerHit = false;
    bool playerFrozen = false;

    Animator anim;

    KnockbackFeedback knockback;

    [SerializeField] AudioClip gunFire;
    [SerializeField] AudioClip playerHitSound;
    [SerializeField] AudioClip playerPickupSound;

    [SerializeField] GameObject playerMesh;
    
    [SerializeField] LayerMask mouseHitLayer;

    [Header("PowerMode")] [SerializeField] float powerHealth = 500f;
    [SerializeField] float powerFireRate = 0.25f;
    [SerializeField] float powerWalkSpeed = 1.5f;
    [SerializeField] GameObject powerModeVFX;
    bool powerModeOn = false;


    public bool PlayerHit
    {
        get { return playerHit; }
    }

    public float CurrentPlayerHealth
    {
        get { return currentPlayerHealth; }  
    }

    public static PlayerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        knockback = GetComponent<KnockbackFeedback>();
        currentPlayerHealth = playerHealth;

    }

    // Update is called once per frame
    void Update()
    {
        if (currentPlayerHealth > 0)
        {
            if (Input.GetMouseButton(0))
            {
                ShootProjectile();
            }
            GatherInputs();
            if (!playerFrozen)
                HandleWalking();
        }
    }

    private void FixedUpdate()
    {
        if (currentPlayerHealth > 0)
            RotateFromMouseVector();
    }

    void ShootProjectile()
    {
        if(!shotFired)
            StartCoroutine(TimeBetweenShots());
    }

    private void RotateFromMouseVector()
    {
        Ray ray = Camera.ScreenPointToRay(MousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance: 300f, mouseHitLayer))
        {
            var target = hitInfo.point;
            target.y = transform.position.y;
            transform.LookAt(target);
        }
    }

    #region New Movement

    #region Inputs

    private void GatherInputs()
    {
        _inputs.RawX = (int)Input.GetAxisRaw("Horizontal");
        _inputs.RawZ = (int)Input.GetAxisRaw("Vertical");
        _inputs.X = Input.GetAxis("Horizontal");
        _inputs.Z = Input.GetAxis("Vertical");

        _dir = new Vector3(_inputs.X, 0, _inputs.Z);
        MousePosition = Input.mousePosition;

        if (_dir != new Vector3(0, 0, 0))
        {
            anim.SetBool("IsWalking", true);
        }
        else
            anim.SetBool("IsWalking", false);

    }

    #endregion

    [Header("Walking")][SerializeField] private float _walkSpeed = 8;
    [SerializeField] private float _acceleration = 2;
    [SerializeField] private float _maxWalkingPenalty = 0.5f;
    [SerializeField] private float _currentMovementLerpSpeed = 100;
    private float _currentWalkingPenalty;

    private Vector3 _dir;

    /// <summary>
    /// I'm sure this section could use a big refactor
    /// </summary>
    private void HandleWalking()
    {

        // Slowly increase max speed
        if (_dir != Vector3.zero) _currentWalkingPenalty += _acceleration * Time.deltaTime;
        else _currentWalkingPenalty -= _acceleration * Time.deltaTime;
        _currentWalkingPenalty = Mathf.Clamp(_currentWalkingPenalty, _maxWalkingPenalty, 1);

        // Set current y vel and add walking penalty
        var targetVel = new Vector3(_dir.x, rb.velocity.y, _dir.z) * _currentWalkingPenalty;

        rb.velocity = targetVel * _walkSpeed;
    }

    #endregion


    IEnumerator TimeBetweenShots()
    {
        shotFired = true;
        AudioSource.PlayClipAtPoint(gunFire, this.transform.position, 2f);
        shootVFX.Play();
        var shot = Instantiate(projectile, shootingPos[0].position, transform.rotation);
        shot.GetComponent<Rigidbody>().AddRelativeForce(projectile.transform.up * projectileSpeed, ForceMode.Impulse);
        if (powerModeOn)
        {
            var shot2 = Instantiate(projectile, shootingPos[1].position, shootingPos[1].rotation);
            shot2.GetComponent<Rigidbody>().AddRelativeForce(projectile.transform.up * projectileSpeed, ForceMode.Impulse);
            var shot3 = Instantiate(projectile, shootingPos[2].position, shootingPos[2].rotation);
            shot3.GetComponent<Rigidbody>().AddRelativeForce(projectile.transform.up * projectileSpeed, ForceMode.Impulse);
        }
        yield return new WaitForSeconds(shootingTimer);
        shotFired = false;
    }

    public void AdjustPlayerHealth(float value, Transform objPosition)
    {
        if (!playerHit)
        {
            currentPlayerHealth = currentPlayerHealth + value;
            if (value < 0)
            {
                AudioSource.PlayClipAtPoint(playerHitSound, this.transform.position, 2f);
                //Vector3 moveDir = rb.transform.position - objPosition.position;
                //rb.AddForce(moveDir.normalized * 100f, ForceMode.Impulse);
                if (!powerModeOn)
                    knockback.PlayFeedback(objPosition);
                Debug.Log("PLAYER HIT for " + value + ", health = " + currentPlayerHealth);
                StartCoroutine(TimeBetweenHits());
                StartCoroutine(FreezeTimeBetweenHits());
            }
        }
        GameManager.Instance.UpdateUI(false);
    }

    IEnumerator TimeBetweenHits()
    {
        playerHit = true;
        yield return new WaitForSeconds(hitsCooldown);
        playerHit = false;
    }

    IEnumerator FreezeTimeBetweenHits()
    {
        playerFrozen = true;
        yield return new WaitForSeconds(.5f);
        playerFrozen = false;
    }

    #region Buffs

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Buff"))
        {
            PlayerBuff pBuff = other.gameObject.GetComponent<PlayerBuff>();
            switch (pBuff.Buff)
            {
                case PlayerBuff.BuffType.FireRate:
                    shootingTimer -= pBuff.Value;
                    Debug.Log("Fire Rate increased");
                    break;
                case PlayerBuff.BuffType.Movement:
                    _walkSpeed += pBuff.Value;
                    Debug.Log("Movement increased");
                    break;
                case PlayerBuff.BuffType.Health:
                    Debug.Log("Fire Rate health");
                    if(!powerModeOn)
                        if (currentPlayerHealth <= 80)
                        {
                            AdjustPlayerHealth(pBuff.Value, null);
                        }
                        else if (currentPlayerHealth < 100)
                        {
                            currentPlayerHealth = 100f;
                            GameManager.Instance.UpdateUI(false);
                        }
                    break;
                case PlayerBuff.BuffType.Power:
                    Debug.Log("POWAAAH");
                    if (!powerModeOn)
                        StartCoroutine(PowerMode(pBuff.Value));
                    break;
                default:
                    break;
            }
            AudioSource.PlayClipAtPoint(playerPickupSound, this.transform.position, 1f);
            Destroy(other.gameObject);
        }
    }

    IEnumerator PowerMode(float duration)
    {
        float tempHealth = currentPlayerHealth;
        float tempFireRate = shootingTimer;
        float tempWalkSpeed = _walkSpeed;

        AdjustPlayerHealth(powerHealth, null);
        shootingTimer = powerFireRate;
        _walkSpeed = powerWalkSpeed;
        powerModeOn = true;
        GameObject powerModeVFXGo = Instantiate(powerModeVFX, transform.position, Quaternion.identity);
        powerModeVFXGo.GetComponentInChildren<VisualEffect>().Play();
        Destroy(powerModeVFXGo, 1.5f);
        yield return new WaitForSeconds(duration);
        powerModeOn = false;
        currentPlayerHealth = tempHealth;
        shootingTimer = tempFireRate;
        _walkSpeed = tempWalkSpeed;
        GameManager.Instance.UpdateUI(false);
    }

    #endregion

}
