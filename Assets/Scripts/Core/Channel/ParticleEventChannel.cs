using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ParticalEventChannel", menuName = "Events/ParticalEventChannel")]
public class ParticalEventChannel : ScriptableObject
{
    public event UnityAction<ParticleEffectTpye, Vector3> OnParticleEffect;

    public void RaiseParticleEffect(ParticleEffectTpye particleEffectTpye, Vector3 pos)
    {
        OnParticleEffect?.Invoke(particleEffectTpye, pos);
    }
}
