using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KnockbackFeedback : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] float strength = 16f;
    [SerializeField] float delay = .15f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void PlayFeedback(Transform sender)
    {
        Debug.Log("KNOCKBACK");
        Vector3 dir = (transform.position - sender.position).normalized;
        rb.AddForce(dir * strength, ForceMode.Impulse);

        //StartCoroutine(Reset(rb.velocity));
    }

    IEnumerator Reset(Vector3 startVelocity)
    {
        yield return new WaitForSeconds(delay);
        rb.velocity = Vector3.zero;
    }

}
