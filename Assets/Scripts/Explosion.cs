using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] float damageToEnemies;
    [SerializeField] float damageToPlayer;
    [SerializeField] float explosionTime;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyObject());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Enemy"))
        {
            other.gameObject.GetComponent<EnemyMovement>().EnemyHit(damageToEnemies);
        }
        if (other.gameObject.tag.Equals("Player"))
        {
            PlayerManager.Instance.AdjustPlayerHealth(-damageToPlayer, this.transform);
        }
    }

    IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(explosionTime);
        Destroy(this.gameObject);
    }

}
