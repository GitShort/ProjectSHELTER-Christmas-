using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Projectile : MonoBehaviour
{
    [SerializeField] GameObject projectileExplosion;
    [SerializeField] float VFXOffset = 0.05f;
    [SerializeField] float projectileDamage = 2f;

    [SerializeField] string projectileType;

    VisualEffect projectileVFX;

    [SerializeField] AudioClip projectileSound;

    // Start is called before the first frame update
    void Start()
    {
        projectileVFX = GetComponentInChildren<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Enemy") && projectileType.Equals("PlayerProjectile"))
        {
            other.gameObject.GetComponent<EnemyMovement>().EnemyHit(projectileDamage);
        }

        if (other.gameObject.tag.Equals("Player") && projectileType.Equals("EnemyProjectile"))
        {
            PlayerManager.Instance.AdjustPlayerHealth(-projectileDamage * 10f, this.transform);
        }
        //AudioSource.PlayClipAtPoint(projectileSound, this.transform.position, 1f);

        GameObject explosion = Instantiate(projectileExplosion, transform.position, transform.rotation);
        explosion.GetComponentInChildren<VisualEffect>().Play();
        //AudioManager.instance.PlaySound("ProjectileExplode", explosion);
        Destroy(explosion, 1.5f);
        Destroy(this.gameObject );
    }
}
