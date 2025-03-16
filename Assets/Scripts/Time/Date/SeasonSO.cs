using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SeasonSO", menuName = "SO/SeasonSO")]
public class SeasonSO : ScriptableObject
{
    public List<Sprite> seasonSprite;

    public Sprite GetSeasonSprite(Season season)
    {
        return seasonSprite[(int)season];
    }
}
