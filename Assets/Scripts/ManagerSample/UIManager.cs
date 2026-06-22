using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class UIManager : MonoBehaviour
{
    private Dictionary<string, UI_Panel> _panelDict = new Dictionary<string, UI_Panel>();
    private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();

    private ResourceManager _resourceManager;
    [Inject]
    public void Construct(ResourceManager resourceManager)
    {
        _resourceManager = resourceManager;
    }

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            return root;
        }
    }
    public Transform PanelRoot
    {
        get
        {
            Transform panel = Root.transform.Find("@Panel_Root");
            if (panel == null)
            {
                panel = new GameObject { name = "@Panel_Root" }.transform;
                panel.SetParent(Root.transform);
                panel.SetAsFirstSibling();
            }

            return panel;
        }
    }
    public Transform PopupRoot
    {
        get
        {
            Transform popup = Root.transform.Find("@Popup_Root");
            if (popup == null)
            {
                popup = new GameObject { name = "@Popup_Root" }.transform;
                popup.SetParent(Root.transform);
                popup.SetAsLastSibling();
            }
            return popup;
        }
    }

    public void ShowPanelUIAsync<T>(string key = null, float fadeDuration = 0f, Action<T> completed = null) where T : UI_Panel
    {
        if (string.IsNullOrEmpty(key))
            key = typeof(T).Name;

        key = $"Prefabs/UI/Panel/{key}";
        if (_panelDict.ContainsKey(key))
        {
            UI_Panel panel = _panelDict[key];
            //panel.transform.SetParent(PanelRoot);
            completed?.Invoke(panel as T);
            _panelDict[key].Show(fadeDuration, null);
        }
        else
        {
            StartCoroutine(Co_ShowPanel<T>(key, fadeDuration, completed));
        }
    }
    IEnumerator Co_ShowPanel<T>(string key, float fadeDuration = 0f, Action<T> completed = null) where T : UI_Panel
    {
        var handle = _resourceManager.InstantiateAsync(key, PanelRoot);
        yield return handle;

        GameObject instance = handle.Result;
        T panelUI = instance.GetOrAddComponent<T>();
        _panelDict.Add(key, panelUI);

        while (panelUI.IsLoaded == false)
        {
            yield return null;
        }

        completed?.Invoke(panelUI);
        panelUI.Show(fadeDuration, null);
    }
    public void HidePanelUI(UI_Panel panel, float fadeDuration = 0f, Action<UI_Panel> completed = null)
    {
        panel.Hide(fadeDuration, completed);
    }
    public void HidePanelUI(string key, float fadeDuration = 0f, Action<UI_Panel> completed = null)
    {
        key = $"Prefabs/UI/Panel/{key}";
        if (_panelDict.ContainsKey(key))
        {
            HidePanelUI(_panelDict[key], fadeDuration, completed);
        }
    }

    public void ShowPopupUIAsync<T>(string key = null, float scaleDuration = 0f, Action<T> completed = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(key))
            key = typeof(T).Name;

        key = $"Prefabs/UI/Popup/{key}";
        StartCoroutine(Co_ShowPopup<T>(key, scaleDuration, completed));
    }
    IEnumerator Co_ShowPopup<T>(string key, float scaleDuration = 0f, Action<T> completed = null) where T : UI_Popup
    {
        var handle = _resourceManager.InstantiateAsync(key, PopupRoot);
        yield return handle;

        GameObject instance = handle.Result;
        T popup = instance.GetComponent<T>();
        _popupStack.Push(popup);

        while (popup.IsLoaded == false)
        {
            yield return null;
        }

        completed?.Invoke(popup);
        popup.Show(scaleDuration);
    }

    public void ClosePopupUI(UI_Popup popup, float scaleDuration = 0f, Action completed = null)
    {
        if (_popupStack.Count == 0)
            return;

        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return;
        }

        ClosePopupUI(scaleDuration, completed);
    }

    public void ClosePopupUI(float scaleDuration = 0f, Action completed = null)
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        popup.Hide(scaleDuration, (popup) =>
        {
            _resourceManager.Destroy(popup.gameObject);
            completed?.Invoke();
        });
    }

    public void CloseAllPopupUI(float scaleDuration = 0f)
    {
        while (_popupStack.Count > 0)
            ClosePopupUI(scaleDuration);
    }

    public void CloseAllPanelUI()
    {
        foreach (var kv in _panelDict)
        {
            _resourceManager.Destroy(kv.Value.gameObject);
        }
        _panelDict.Clear();
    }


    public void Clear()
    {
        CloseAllPanelUI();
        CloseAllPopupUI();
    }
}
