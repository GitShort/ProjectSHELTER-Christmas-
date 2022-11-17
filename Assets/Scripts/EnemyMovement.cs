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

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = PlayerManager.Instance.transform.position;
    }

    public void EnemyHit(float damage)
    {
        Debug.Log("Damage");
        currentEnemyHealth = enemyHealth - damage;
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
}
