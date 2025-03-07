using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailEffect : MonoBehaviour
{
    [SerializeField] private Gradient trailColor;
    [SerializeField] private float widthMultiplier = 0.5f;
    
    private TrailRenderer _trail;

    void Start()
    {
        _trail = gameObject.AddComponent<TrailRenderer>();
        ConfigureTrail();
    }

    private void ConfigureTrail()
    {
        _trail.time = 0.3f;
        _trail.minVertexDistance = 0.1f;
        _trail.widthCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        _trail.colorGradient = trailColor;
        _trail.material = new Material(Shader.Find("Sprites/Default"));
    }

    public void ResetTrail()
    {
        _trail.Clear();
    }
}

