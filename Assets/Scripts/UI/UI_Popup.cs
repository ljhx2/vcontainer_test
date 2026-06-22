using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

public abstract class UI_Popup : UI_Base
{
    protected List<Transform> _childTransformList = new List<Transform>();
    protected List<Vector3> _childOriginScaleList = new List<Vector3>();

    protected virtual void OnEnable()
    {

    }

    public override void Init()
    {
        base.Init();

        Transform canvasTransform = GetComponent<Transform>();
        for (int i = 0; i < canvasTransform.childCount; i++)
        {
            Transform child = canvasTransform.GetChild(i);
            _childTransformList.Add(child);
            _childOriginScaleList.Add(child.localScale);
            child.localScale = Vector3.zero;
        }
    }

    public virtual void Show(float duration = 0f, Action<UI_Popup> completed = null)
    {
        gameObject.SetActive(true);
        StartCoroutine(Co_ScaleShow(duration, completed));
    }

    public virtual void Hide(float duration = 0f, Action<UI_Popup> completed = null)
    {
        StartCoroutine(Co_ScaleHide(duration, completed));
    }

    IEnumerator Co_ScaleShow(float duration, Action<UI_Popup> completed = null)
    {
        yield return null;

        if (duration > 0f)
        {
            for (int i = 0; i < _childTransformList.Count; i++)
            {
                _childTransformList[i].localScale = Vector3.zero;
            }

            RatioTimer rt = new RatioTimer(duration);
            while (rt.Ratio < 1f)
            {
                rt.Update(Time.deltaTime);
                float scale = rt.Ratio;
                for (int i = 0; i < _childTransformList.Count; i++)
                {
                    _childTransformList[i].localScale = _childOriginScaleList[i] * scale;
                }
                yield return null;
            }
        }

        for (int i = 0; i < _childTransformList.Count; i++)
        {
            _childTransformList[i].localScale = _childOriginScaleList[i];
        }
        completed?.Invoke(this);
    }
    IEnumerator Co_ScaleHide(float duration, Action<UI_Popup> completed = null)
    {
        yield return null;

        if (duration > 0f)
        {
            RatioTimer rt = new RatioTimer(duration);
            while (rt.Ratio < 1f)
            {
                rt.Update(Time.deltaTime);
                float scale = 1f - rt.Ratio;
                for (int i = 0; i < _childTransformList.Count; i++)
                {
                    _childTransformList[i].localScale = _childOriginScaleList[i] * scale;
                }
                yield return null;
            }
        }
        for (int i = 0; i < _childTransformList.Count; i++)
        {
            _childTransformList[i].localScale = Vector3.zero;
        }
        completed?.Invoke(this);
        gameObject.SetActive(false);
    }
}
