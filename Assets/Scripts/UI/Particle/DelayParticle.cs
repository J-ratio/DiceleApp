using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayParticle : MonoBehaviour
{
    [SerializeField] private GameObject particle;
    [SerializeField] private float delay = 0.4f;
    float nextTime;
    private void OnEnable()
    {
        nextTime = Time.time + delay;
    }

    private void OnDisable()
    {
        particle.SetActive(false);
    }

    private void Update()
    {
        if (Time.time > nextTime)
        {
            particle.SetActive(true);
        }
    }
}
