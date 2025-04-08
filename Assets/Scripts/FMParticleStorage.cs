using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMParticleStorage : MonoBehaviour
{
    [SerializeField] private Transform root;
    private static FMParticleStorage singleton;
    private Dictionary<string, List<FMParticle>> particleStorages;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
    }

    public static FMParticleStorage Get()
    {
        return singleton;
    }

    public FMParticle Load(string name)
    {
        FMParticle particle = null;
        if (particleStorages != null)
        {
            bool particleExist = particleStorages.ContainsKey(name);
            if (particleExist)
            {
                List<FMParticle> particles = particleStorages[name];
                if (particles.Count > 0)
                {
                    particle = particles[0];
                }
            }
        }
        return particle;
    }

    public void Save(FMParticle particle)
    {
        if (particleStorages == null)
        {
            particleStorages = new Dictionary<string, List<FMParticle>>();
        }

        if (particle.transform != null)
        {
            particle.transform.SetParent(root);

            string particleName = particle.gameObject.name;
            if (particleStorages.ContainsKey(particleName))
            {
                List<FMParticle> listParticle = new List<FMParticle>();
                listParticle.Add(particle);
                particleStorages.Add(particleName, listParticle);
            }
        }
    }
}
