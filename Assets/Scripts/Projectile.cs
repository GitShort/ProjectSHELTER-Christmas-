using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Projectile : MonoBehaviour
{
    [SerializeField] GameObject projectileExplosion;
    [SerializeField] float VFXOffset = 0.05f;
    [SerializeField] float projectileDamage = 2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Enemy"))
        {
            other.gameObject.GetComponent<EnemyMovement>().EnemyHit(projectileDamage);
        }

        GameObject explosion = Instantiate(projectileExplosion, transform.position, transform.rotation);
        explosion.GetComponentInChildren<VisualEffect>().Play();
        //AudioManager.instance.PlaySound("ProjectileExplode", explosion);
        Destroy(explosion, 1.5f);
        Destroy(this.gameObject);
    }
}
