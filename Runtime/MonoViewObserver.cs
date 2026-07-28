using System;
using System.Collections.Generic;
using UnityEngine;

public class MonoViewObserver : MonoBehaviour
{
    private struct ObserverEntry
    {
        public MonoView View;
        public IDisposable Presenter;
    }

    private static MonoViewObserver _instance;
    public static MonoViewObserver Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GameObject(nameof(MonoViewObserver));
                _instance = obj.AddComponent<MonoViewObserver>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    private readonly Dictionary<EntityId, ObserverEntry> _observingMap = new Dictionary<EntityId, ObserverEntry>();
    private readonly List<EntityId> _removeBuffer = new List<EntityId>(16);

    private void Update()
    {
        if (_observingMap.Count == 0)
            return;

        _removeBuffer.Clear();

        foreach (var pair in _observingMap)
        {
            // Unity Object는 Destroy된 뒤 null 비교가 true가 됩니다.
            if (pair.Value.View == null)
            {
                pair.Value.Presenter?.Dispose();
                _removeBuffer.Add(pair.Key);
            }
        }

        for (int i = 0; i < _removeBuffer.Count; i++)
        {
            _observingMap.Remove(_removeBuffer[i]);
        }
    }

    public void Observing(MonoView view, IDisposable presenter)
    {
        if (presenter == null)
            return;

        // 이미 파괴된 View라면 Presenter를 즉시 정리합니다.
        if (view == null)
        {
            presenter.Dispose();
            return;
        }

        EntityId viewId = view.GetEntityId();
        _observingMap[viewId] = new ObserverEntry
        {
            View = view,
            Presenter = presenter
        };
    }

    public void StopObserving(MonoView view)
    {
        if (view == null)
            return;

        _observingMap.Remove(view.GetEntityId());
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
