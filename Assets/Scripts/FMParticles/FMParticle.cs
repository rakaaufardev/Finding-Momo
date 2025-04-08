using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FMParticle : MonoBehaviour
{
    [SerializeField] protected Transform root;
    [SerializeField] protected Transform rootParticle;
    [SerializeField] protected ParticleSystem particle;

    public abstract void SetPosition(Vector3 position);
    public void PlayParticle()
    {
        rootParticle.gameObject.SetActive(true);
        if (!IsParticlePlaying())
        {
            StopParticle();
            particle.Play(true);
        }
    }

    public void StopParticle()
    {
        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public bool IsParticlePlaying()
    {
        bool result = particle.isPlaying;
        return result;
    }

    void DoUpdate()
    {
        if (particle.isPaused)
        {
            //particle.Stop();
        }

        bool isPlaying = IsParticlePlaying();
        if (!isPlaying)
        {
            //rootParticle.gameObject.SetActive(false);
        }
    }

    protected void Update()
    {
        DoUpdate();
    }
}
