using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuff : MonoBehaviour
{
    public enum BuffType
    {
        Power,
        FireRate,
        Movement,
        Health
    }

    [SerializeField] BuffType buff;

    [SerializeField] float value;

    public BuffType Buff
    {
        get { return buff; }
    }

    public float Value
    {
        get { return value; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Player"))
        {
            
        }
    }

}
