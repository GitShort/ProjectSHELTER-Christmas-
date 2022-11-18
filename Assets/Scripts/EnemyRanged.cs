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

    bool shotFired = true;

    // Start is called before the first frame update
    void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        StartCoroutine(StopInitialShot());
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyMovement.GetRemainingDistanceToPlayer() <= enemyMovement.GetStoppingDistance() * shootingDistanceMultiplier && !shotFired)
        {
            StartCoroutine(FireProjectile());
        }
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
