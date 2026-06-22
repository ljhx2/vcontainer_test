
//비율(1f) 단위로 계산 타이머
public class RatioTimer
{
    //플레이 시간(s)
    float _playTime = 1f;
    //진행 시간(s)
    float _time = 0f;
    //진행 비율(0~1)
    float _ratio = 0f;

    public RatioTimer(float playTime = 1f)
    {
        _playTime = playTime;
    }

    //진행 시간
    public float Time
    {
        get { return _time; }
        set
        {
            _time = value;

            if (_time > _playTime)
                _time = _playTime;

            if (_time < 0f)
                _time = 0f;
        }
    }

    public float Ratio
    {
        get { return _ratio; }
    }

    public float PlayTime
    {
        get { return _playTime; }
    }

    public void SetTimer(float playTime)
    {
        _playTime = playTime;
        _time = 0f;
        _ratio = 0f;
    }

    public void Reset()
    {
        _time = 0f;
        _ratio = 0f;
    }

    //타이머 업데이트
    public float Update(float deltaTIme)
    {
        _time += deltaTIme;

        _ratio = _time / _playTime;
        return _ratio;
    }
}
