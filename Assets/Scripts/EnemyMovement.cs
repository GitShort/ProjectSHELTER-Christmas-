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
    [SerializeField] AudioClip[] splatSounds;

    //enum enemyType { Generic, Ranged, Fast, Tank};
    //[SerializeField] enemyType type;

    [SerializeField] GameObject[] buffDrops;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentEnemyHealth = enemyHealth;
        defaultSpeed = agent.speed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        agent.destination = PlayerManager.Instance.transform.position;
    }

    public void EnemyHit(float damage)
    {
        currentEnemyHealth = currentEnemyHealth - damage;
        if (!wasHit)
            StartCoroutine(GetHit());
        if (currentEnemyHealth <= 0)
        {
            AudioSource.PlayClipAtPoint(splatSounds[Random.Range(0, splatSounds.Length)], this.transform.position, 1f);
            GameManager.Instance.CurrentEnemyCount--;
            GameManager.Instance.KilledEnemyCount++;
            GameManager.Instance.UpdateUI(true);

            GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation);
            explosion.GetComponentInChildren<VisualEffect>().Play();
            //AudioManager.instance.PlaySound("ProjectileExplode", explosion);

            var buffDropChance = Random.Range(0, 100);
            if (buffDropChance < 2)
            {
                GameObject buff = Instantiate(buffDrops[3], transform.position, Quaternion.identity);
            }
            else if (buffDropChance < 10 && buffDropChance > 2)
            {
                GameObject buff = Instantiate(buffDrops[Random.Range(0, 3)], transform.position, Quaternion.identity);
            }

            Destroy(explosion, 4f);
            Destroy(this.gameObject);
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{

    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
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
