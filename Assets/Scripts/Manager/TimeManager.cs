using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("事件通道")]
    [SerializeField] private TimeEventChannel _timeEventChannel;
    [SerializeField] private float _realSecondsPerGameSecond;
    
    private bool _isPaused;
    private GameTime _currentTime;
    private Season _previousSeason = Season.None;
    private Coroutine _timeCoroutine;

    public void SetPause(bool pause) => _isPaused = pause;

    void OnEnable()
    {
        StartTimeSystem();
    }

    void OnDisable()
    {
        StopTimeSystem();
    }

    private void StartTimeSystem()
    {
        _currentTime = new GameTime();
        _timeCoroutine = StartCoroutine(TimeUpdateCoroutine());
    }

    private void StopTimeSystem()
    {
        if (_timeCoroutine != null)
            StopCoroutine(_timeCoroutine);
    }

    private IEnumerator TimeUpdateCoroutine()
    {
        while (true)
        {
            if (!_isPaused)
            {
                yield return new WaitForSeconds(_realSecondsPerGameSecond);
                _currentTime.AddSecond(1);
                _timeEventChannel.RaiseTimeChanged(_currentTime);
                if (_currentTime.Season != _previousSeason)
                {
                    _timeEventChannel.RaiseSeasonChanged(_currentTime.Season);
                    _previousSeason = _currentTime.Season;
                }
            }
            else
            {
                yield return null; // 暂停时保持低消耗
            }
        }
    }
}

public class GameTime
{
    public int totalTime;

    // 游戏世界10s为1分钟，6分钟为一小时，10天为1季度，1月即一个季节
    public int Second => (totalTime % 10) *10;
    public int Minute => (totalTime / 10 % 6) * 10;
    public int Hour => totalTime / 60 % 24;
    public int Day => totalTime / 1440 % 10 + 1;
    public int Month => totalTime / 14400 % 4 + 1;
    public Season Season => (Season)Month;
    public int Year => totalTime / 57600 % 9999 + 2025;

    public void AddSecond(int detalSecond)
    {
        totalTime += detalSecond;
    }
}

public enum Season
{
    Spring = 0,
    Summer = 1,
    Autumn = 2,
    Winter = 3,
    None,
}