using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 등록된 리소스의 수명을 함께 관리하는 Presenter용 기본 클래스입니다.
/// </summary>
public class DisposablePoco : IDisposable, ICollection<IDisposable>
{
    private readonly List<IDisposable> _ownedResources = new List<IDisposable>();

    public bool IsDisposed { get; private set; }
    public int Count => _ownedResources.Count;
    public bool IsReadOnly => false;

    /// <summary>
    /// 이 객체가 소유할 리소스를 등록합니다.
    /// 이미 정리된 뒤 등록된 리소스는 즉시 정리합니다.
    /// </summary>
    public void Add(IDisposable resource)
    {
        if (resource == null)
            throw new ArgumentNullException(nameof(resource));

        if (IsDisposed)
        {
            resource.Dispose();
            return;
        }

        _ownedResources.Add(resource);
    }

    /// <summary>
    /// 등록된 리소스를 모두 정리하되 컨테이너는 계속 사용할 수 있게 유지합니다.
    /// </summary>
    public void Clear()
    {
        DisposeOwnedResources();
    }

    public bool Contains(IDisposable resource)
    {
        return _ownedResources.Contains(resource);
    }

    public void CopyTo(IDisposable[] array, int arrayIndex)
    {
        _ownedResources.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// 리소스의 소유권을 해제합니다. 제거된 리소스는 호출자가 직접 정리해야 합니다.
    /// </summary>
    public bool Remove(IDisposable resource)
    {
        return _ownedResources.Remove(resource);
    }

    public IEnumerator<IDisposable> GetEnumerator()
    {
        return _ownedResources.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public virtual void Dispose()
    {
        if (IsDisposed)
            return;

        IsDisposed = true;

        DisposeOwnedResources();
    }

    private void DisposeOwnedResources()
    {
        // 나중에 등록한 리소스부터 정리해 의존 관계를 역순으로 해제합니다.
        for (int i = _ownedResources.Count - 1; i >= 0; i--)
        {
            _ownedResources[i]?.Dispose();
        }

        _ownedResources.Clear();
    }
}
