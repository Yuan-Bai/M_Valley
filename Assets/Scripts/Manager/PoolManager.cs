using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    [Header("预制体")]
    [SerializeField] private List<GameObject> _poolPrefabs;
    
    [Header("事件通道")]
    [SerializeField] private ParticalEventChannel _particalEventChannel;

    private List<ObjectPool<GameObject>> _poolEffectList = new();

    void Start()
    {
        CreatePool();
    }

    void OnEnable()
    {
        _particalEventChannel.OnParticleEffect += HandlePraticleEffect;
    }

    void OnDisable()
    {
        _particalEventChannel.OnParticleEffect -= HandlePraticleEffect;
    }

    private void CreatePool()
    {
        foreach (var poolPrefab in _poolPrefabs)
        {
            Transform parent = new GameObject(name=poolPrefab.name).transform;
            parent.SetParent(transform);

            var newPool = new ObjectPool<GameObject>(
                () => Instantiate(poolPrefab, parent),
                e => e.SetActive(true),
                e => e.SetActive(false),
                e => Destroy(e)
            );
            _poolEffectList.Add(newPool);
        }
    }

    private void HandlePraticleEffect(ParticleEffectTpye particleEffectTpye, Vector3 pos)
    {
        ObjectPool<GameObject> pool = particleEffectTpye switch
        {
            ParticleEffectTpye.LeavesFall01 => _poolEffectList[0],
            ParticleEffectTpye.LeavesFall02 => _poolEffectList[1],
            ParticleEffectTpye.RockParticle01 => _poolEffectList[2],
            ParticleEffectTpye.GrassParticle => _poolEffectList[3],
            _ => null
        };
        if (pool == null) return;
        GameObject gameObject = pool.Get();
        gameObject.transform.position = pos;
        StartCoroutine(ReleaseRoutine(pool, gameObject));
    }

    private IEnumerator ReleaseRoutine(ObjectPool<GameObject> pool, GameObject gameObject)
    {
        yield return new WaitForSeconds(1.5f);
        pool.Release(gameObject);
    }
}
