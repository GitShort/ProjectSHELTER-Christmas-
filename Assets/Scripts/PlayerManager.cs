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
    [SerializeField] Transform shootingPos;
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
    [SerializeField] LayerMask mouseHitLayer;

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

        if (Input.GetMouseButton(0))
        {
            ShootProjectile();
        }
        GatherInputs();
        if (!playerFrozen)
        HandleWalking();
    }

    private void FixedUpdate()
    {
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
        var shot = Instantiate(projectile, shootingPos.position, transform.rotation);
        shot.GetComponent<Rigidbody>().AddRelativeForce(projectile.transform.up * projectileSpeed, ForceMode.Impulse);
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
                //Vector3 moveDir = rb.transform.position - objPosition.position;
                //rb.AddForce(moveDir.normalized * 100f, ForceMode.Impulse);
                knockback.PlayFeedback(objPosition);
                Debug.Log("PLAYER HIT for " + value + ", health = " + currentPlayerHealth);
                StartCoroutine(TimeBetweenHits());
                StartCoroutine(FreezeTimeBetweenHits());
            }
            if (currentPlayerHealth <= 0)
            {
                Time.timeScale = 0;
                Debug.Log("YOU LOSE");
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

}
