using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class TimeView : MonoBehaviour
{
    [Header("组件引用")]
    [SerializeField] private Transform _dayOrNight;
    [SerializeField] private Transform _clockParent;
    [SerializeField] private TMP_Text _time;
    [SerializeField] private TMP_Text _date;
    [SerializeField] private Image _seasonImage;

    [Header("事件通道")]
    [SerializeField] private TimeEventChannel _timeEventChannel;
    
    [Header("资源")]
    [SerializeField] private SeasonSO seasonSO;

    void OnEnable()
    {
        _timeEventChannel.OnTimeChanged += HandleTimeChanged;
        _timeEventChannel.OnSeasonChanged += HandleSeasonChanged;
    }

    void OnDisable()
    {
        _timeEventChannel.OnTimeChanged -= HandleTimeChanged;
        _timeEventChannel.OnSeasonChanged -= HandleSeasonChanged;
    }

    private void HandleTimeChanged(GameTime gameTime)
    {
        _time.text = $"{gameTime.Hour:D2}:{gameTime.Minute:D2}";
        _date.text = $"{gameTime.Year:D4}年{gameTime.Month:D2}月{gameTime.Day:D2}日";
        ActiveClock(gameTime.Hour);
        DayOrNightRotation(gameTime.Hour);
    }

    private void HandleSeasonChanged(Season season)
    {
        _seasonImage.sprite = seasonSO.GetSeasonSprite(season);
    }

    private void ActiveClock(int hour)
    {
        int index = hour % 6;
        if (index == 0)
        {
            for (int i = 0;i < 6; i++)
            {
                _clockParent.GetChild(i).gameObject.SetActive(false);
            }
        }
        _clockParent.GetChild(index).gameObject.SetActive(true);
    }

    private void DayOrNightRotation(int hour)
    {
        var target = new Vector3(0, 0, hour * 15 - 90);
        _dayOrNight.DORotate(target, 1f, RotateMode.Fast);
    }

}
