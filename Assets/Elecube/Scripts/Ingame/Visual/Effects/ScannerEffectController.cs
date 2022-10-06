using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScannerEffectController : MonoBehaviour
{
    [Header("speed")] public float speed;

    [Header("destory time")] public float delay_destory_time;

    void Start()
    {
        Invoke(nameof(DestroySelf), this.delay_destory_time);
    }

    void Update()
    {
        Vector3 v3 = this.transform.localScale;
        float temp = this.speed * Time.deltaTime;
        this.transform.localScale = new Vector3(v3.x + temp, v3.y + temp, v3.z + temp);
    }

    private void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}