using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class EnemyMovement : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField] float enemyHealth = 2f;
    float currentEnemyHealth;

    [SerializeField] GameObject deathVFX;
    [SerializeField] float damageToPlayer = 20f;

    float defaultSpeed;
    [SerializeField] float staggerTime = 0.125f;
    [SerializeField] float staggerSpeed = 0.25f;
    bool wasHit = false;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentEnemyHealth = enemyHealth;
        defaultSpeed = agent.speed;
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = PlayerManager.Instance.transform.position;
    }

    public void EnemyHit(float damage)
    {
        Debug.Log("Damage");
        currentEnemyHealth = currentEnemyHealth - damage;
        if (!wasHit)
            StartCoroutine(GetHit());
        if (currentEnemyHealth <= 0)
        {
            GameManager.Instance.CurrentEnemyCount--;
            GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation);
            explosion.GetComponentInChildren<VisualEffect>().Play();
            //AudioManager.instance.PlaySound("ProjectileExplode", explosion);
            Destroy(explosion, 4f);
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            PlayerManager.Instance.AdjustPlayerHealth(-damageToPlayer, this.transform);
        }
    }

    public float GetRemainingDistanceToPlayer()
    {
        return agent.remainingDistance;
    }

    public float GetStoppingDistance()
    {
        return agent.stoppingDistance;
    }

    public Vector3 GetCurrentVelocity()
    {
        return agent.velocity;
    }

    IEnumerator GetHit()
    {
        wasHit = true;
        agent.speed = staggerSpeed;
        yield return new WaitForSeconds(staggerTime);
        agent.speed = defaultSpeed;
        wasHit = false;
    }
}
