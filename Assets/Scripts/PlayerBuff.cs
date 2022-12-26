using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

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
    [SerializeField] GameObject destroyVFX;

    public BuffType Buff
    {
        get { return buff; }
    }

    public float Value
    {
        get { return value; }
    }

    private void OnDestroy()
    {
        GameObject buffVfx = Instantiate(destroyVFX, transform.position, Quaternion.identity);
        buffVfx.GetComponentInChildren<VisualEffect>().Play();
        Destroy(buffVfx, 3f);
    }

}
