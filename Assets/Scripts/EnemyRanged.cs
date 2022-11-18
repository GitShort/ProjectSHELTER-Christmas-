using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRanged : MonoBehaviour
{
    [SerializeField] GameObject projectile;
    [SerializeField] Transform shootingPos;
    [SerializeField] float projectileSpeed;
    [SerializeField] float shootingTimer;
    [SerializeField] float shootingDistanceMultiplier = 1.1f;

    EnemyMovement enemyMovement;

    Animator anim;

    bool shotFired = true;

    // Start is called before the first frame update
    void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        StartCoroutine(StopInitialShot());
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(PlayerManager.Instance.transform);

        if (enemyMovement.GetRemainingDistanceToPlayer() <= enemyMovement.GetStoppingDistance() * shootingDistanceMultiplier && !shotFired)
        {
            StartCoroutine(FireProjectile());
        }

        if (enemyMovement.GetCurrentVelocity() != new Vector3(0, 0, 0))
        {
            anim.SetBool("IsRunning", true);
        }
        else
            anim.SetBool("IsRunning", false);
    }

    IEnumerator FireProjectile()
    {
        shotFired = true;
        var shot = Instantiate(projectile, shootingPos.position, transform.rotation);
        shot.GetComponent<Rigidbody>().AddRelativeForce(projectile.transform.up * projectileSpeed, ForceMode.Impulse);



        yield return new WaitForSeconds(shootingTimer);
        shotFired = false;
    }

    IEnumerator StopInitialShot()
    {
        yield return new WaitForSeconds(0.05f);
        shotFired = false;
    }
}
