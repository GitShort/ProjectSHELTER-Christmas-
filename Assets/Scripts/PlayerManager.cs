using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(InputHandler))]
public class PlayerManager : MonoBehaviour
{
    private InputHandler _input;

    [SerializeField]
    private bool RotateTowardMouse;

    [SerializeField]
    private float MovementSpeed;
    [SerializeField]
    private float RotationSpeed;

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

    Animator anim;

    KnockbackFeedback knockback;

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
        _input = GetComponent<InputHandler>();

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

        if (_input.InputVector != new Vector2(0, 0))
        {
            anim.SetBool("IsWalking", true);
        }
        else
            anim.SetBool("IsWalking", false);

        if (Input.GetMouseButton(0))
        {
            ShootProjectile();
        }

    }

    private void FixedUpdate()
    {
        var targetVector = new Vector3(_input.InputVector.x, 0, _input.InputVector.y);
        var movementVector = MoveTowardTarget(targetVector);

        //if (!RotateTowardMouse)
        //{
        //    RotateTowardMovementVector(movementVector);
        //}
        if (RotateTowardMouse)
        {
            RotateFromMouseVector();
        }
    }

    void ShootProjectile()
    {
        if(!shotFired)
            StartCoroutine(TimeBetweenShots());
    }

    private void RotateFromMouseVector()
    {
        Ray ray = Camera.ScreenPointToRay(_input.MousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance: 300f))
        {
            var target = hitInfo.point;
            target.y = transform.position.y;
            transform.LookAt(target);
        }
    }

    // TODO later: Optimize walking into walls
    private Vector3 MoveTowardTarget(Vector3 targetVector)
    {
        var speed = MovementSpeed * Time.fixedDeltaTime;

        bool rhit = Physics.Raycast(transform.position, targetVector, speed);
        if (!rhit)
        {
            targetVector = Quaternion.Euler(0, Camera.gameObject.transform.rotation.eulerAngles.y, 0) * targetVector;
            var targetPosition = transform.position + targetVector * speed;
            targetVector = Vector3.Normalize(targetVector);
            transform.position = targetPosition;
        }



        return targetVector;
    }

    private void RotateTowardMovementVector(Vector3 movementDirection)
    {
        if (movementDirection.magnitude == 0) { return; }
        var rotation = Quaternion.LookRotation(movementDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, RotationSpeed);
    }

    IEnumerator TimeBetweenShots()
    {
        shotFired = true;
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
            }
            if (currentPlayerHealth <= 0)
            {
                Time.timeScale = 0;
                Debug.Log("YOU LOSE");
            }
        }
        GameManager.Instance.UpdateUI();
    }

    IEnumerator TimeBetweenHits()
    {
        playerHit = true;
        yield return new WaitForSeconds(hitsCooldown);
        playerHit = false;
    }

}
