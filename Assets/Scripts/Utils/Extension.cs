using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(go);
    }
    public static void BindEvent(this GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_Base.BindEvent(go, action, type);
    }

    public static bool IsValid(this GameObject go)
    {
        return go != null && go.activeSelf;
    }

    public static T GetComponentByName<T>(this GameObject self, string name = null, bool includeInactive = false) where T : Component
    {
        var children = self.GetComponentsInChildren(typeof(T), includeInactive);
        foreach (T c in children)
        {
            if (name == null || (c.name.ToLower() == name.ToLower()))
            {
                return c;
            }
        }
        return null;
    }

    public static T GetComponentByName<T>(this Component self, string name = null, bool includeInactive = false) where T : Component
    {
        var children = self.GetComponentsInChildren(typeof(T), includeInactive);
        foreach (T c in children)
        {
            if (name == null || (c.name.ToLower() == name.ToLower()))
            {
                return c;
            }
        }
        return null;
    }

    public static List<T> GetComponentsByName<T>(this GameObject self, string name = null, bool includeInactive = false) where T : Component
    {
        var listComponents = new List<T>();
        var children = self.GetComponentsInChildren(typeof(T), includeInactive);
        foreach (T c in children)
        {
            if (name == null || (c.name.ToLower() == name.ToLower()))
            {
                listComponents.Add(c);
            }
        }

        return listComponents;
    }

    public static List<T> GetComponentsByName<T>(this Component self, string name = null, bool includeInactive = false) where T : Component
    {
        var listComponents = new List<T>();
        var children = self.GetComponentsInChildren(typeof(T), includeInactive);
        foreach (T c in children)
        {
            if (name == null || (c.name.ToLower() == name.ToLower()))
            {
                listComponents.Add(c);
            }
        }

        return listComponents;
    }
}
