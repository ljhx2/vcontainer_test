using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager
{
    private Dictionary<string, AsyncOperationHandle> _loadedResourceHandle = new Dictionary<string, AsyncOperationHandle>();

    public AsyncOperationHandle<T> LoadAsync<T>(string key, Action<T> completed = null) where T : UnityEngine.Object
    {
        if (_loadedResourceHandle.ContainsKey(key))
        {
            AsyncOperationHandle<T> handle = _loadedResourceHandle[key].Convert<T>();
            completed?.Invoke(handle.Result);
            return handle;
        }
        else
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
            handle.Completed += (result) =>
            {
                if (result.Status == AsyncOperationStatus.Succeeded)
                {
                    _loadedResourceHandle.Add(key, handle);
                    completed?.Invoke(result.Result);
                }
                else
                {
                    Debug.LogError($"Load Resource key:{key} result:{result.Status.ToString()}");
                }
            };
            return handle;
        }
    }

    public AsyncOperationHandle<GameObject> InstantiateAsync(string key, Transform parent = null, Action<GameObject> completed = null)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(key, parent);
        handle.Completed += (result) =>
        {
            if (result.Status == AsyncOperationStatus.Succeeded)
            {
                completed?.Invoke(result.Result);
            }
            else
            {
                Debug.LogError($"InstantiateAsync key:{key} result:{result.Status.ToString()}");
            }
        };
        return handle;
    }
    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        Addressables.ReleaseInstance(go);
    }
    public void Clear()
    {
        foreach (var kv in _loadedResourceHandle)
        {
            Addressables.Release(kv.Value);
        }
        _loadedResourceHandle.Clear();
    }
}
