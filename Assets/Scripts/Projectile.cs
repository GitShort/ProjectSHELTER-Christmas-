using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Projectile : MonoBehaviour
{
    [SerializeField] GameObject projectileExplosion;
    [SerializeField] float VFXOffset = 0.05f;

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
        GameObject explosion = Instantiate(projectileExplosion, transform.position, transform.rotation);
        explosion.GetComponentInChildren<VisualEffect>().Play();
        //AudioManager.instance.PlaySound("ProjectileExplode", explosion);
        Destroy(explosion, 3f);
        Destroy(this.gameObject);
    }
}
