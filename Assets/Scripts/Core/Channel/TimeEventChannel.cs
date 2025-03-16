using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "TimeEventChannel", menuName = "Events/TimeEventChannel")]
public class TimeEventChannel : ScriptableObject
{
    public event UnityAction<GameTime> OnTimeChanged;
    public event UnityAction<Season> OnSeasonChanged;

    public void RaiseTimeChanged(GameTime currentTime)
    {
        OnTimeChanged?.Invoke(currentTime);
    }

    public void RaiseSeasonChanged(Season season)
    {
        OnSeasonChanged?.Invoke(season);
    }
}
