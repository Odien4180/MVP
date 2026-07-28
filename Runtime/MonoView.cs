using System;
using UnityEngine;

public class MonoView : MonoBehaviour, IDisposable
{
    private DisposablePoco _presenter;

    private bool _isOnDestroyGuaranteed;

    public virtual T2 Binding<T1, T2>() where T1 : MonoView where T2 : Presenter<T1>, new()
    {
        var presenter = new T2();
        presenter.View = this as T1;

        _presenter?.Dispose();
        _presenter = presenter;

        if (!_isOnDestroyGuaranteed)
        {
            MonoViewObserver.Instance.Observing(this, presenter);
        }

        return presenter;
    }

    /// <summary>
    /// 현재 바인딩된 Presenter가 요청한 타입과 일치하면 반환합니다.
    /// </summary>
    /// <typeparam name="T">가져올 Presenter 타입입니다.</typeparam>
    /// <param name="presenter">타입이 일치하는 Presenter입니다.</param>
    /// <returns>Presenter가 존재하고 타입이 일치하면 true입니다.</returns>
    public bool TryGetPresenter<T>(out T presenter) where T : DisposablePoco
    {
        presenter = _presenter as T;
        return presenter != null;
    }

    protected virtual void Awake()
    {
        if (_isOnDestroyGuaranteed)
            return;

        _isOnDestroyGuaranteed = true;

        if (_presenter != null)
        {
            // Awake 이후에는 OnDestroy가 보장되므로 Observer의 추적을 중단합니다.
            MonoViewObserver.Instance.StopObserving(this);
        }
    }

    public void ClearPresenter()
    {
        _presenter?.Dispose();
        _presenter = null;
    }

    private void OnDestroy()
    {
        Dispose();
    }

    public void Dispose()
    {
        ClearPresenter();
    }
}
