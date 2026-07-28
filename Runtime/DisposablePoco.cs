using System;
using System.Collections.Generic;

/// <summary>
/// 등록된 리소스의 수명을 함께 관리하는 Presenter용 기본 클래스입니다.
/// </summary>
public class DisposablePoco : IDisposable
{
    private List<IDisposable> _ownedResources = new List<IDisposable>();

    public bool IsDisposed { get; private set; }

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

    public virtual void Dispose()
    {
        if (IsDisposed)
            return;

        IsDisposed = true;

        List<IDisposable> resources = _ownedResources;
        _ownedResources = null;

        // 나중에 등록한 리소스부터 정리해 의존 관계를 역순으로 해제합니다.
        for (int i = resources.Count - 1; i >= 0; i--)
        {
            resources[i]?.Dispose();
        }
    }
}
