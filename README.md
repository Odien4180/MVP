# MVP for Unity

Unity UI를 위한 가벼운 MVP(Model-View-Presenter) 기반 코드입니다. View와 Presenter의 책임을 분리하고, Presenter가 소유한 리소스를 `IDisposable`로 정리할 수 있습니다.

## 요구 사항

- Unity 6.0(6000.0) 이상
- Git 클라이언트

## 설치

Unity 메뉴에서 **Window > Package Manager**를 연 뒤 **+ > Add package from git URL...**을 선택하고 다음 URL을 입력합니다.

```text
https://github.com/Odien4180/MVP.git
```

특정 릴리스로 고정하려면 Git 태그를 URL 뒤에 붙입니다.

```text
https://github.com/Odien4180/MVP.git#v1.0.0
```

또는 프로젝트의 `Packages/manifest.json`에 직접 추가할 수 있습니다.

```json
{
  "dependencies": {
    "com.odien.mvp": "https://github.com/Odien4180/MVP.git#v1.0.0"
  }
}
```

## 사용 예시

```csharp
using UnityEngine;
using UnityEngine.UI;

public class SampleView : MonoView
{
    [SerializeField] private Button button;
    public Button Button => button;
}

public class SamplePresenter : Presenter<SampleView>
{
    public void Initialize()
    {
        View.Button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        Debug.Log("Button clicked");
    }

    public override void Dispose()
    {
        View.Button.onClick.RemoveListener(OnClick);
        base.Dispose();
    }
}

public class RootUI : MonoBehaviour
{
    [SerializeField] private SampleView sampleView;

    private void Start()
    {
        sampleView.Binding<SampleView, SamplePresenter>().Initialize();
    }
}
```

`MonoView`가 파괴되거나 `ClearPresenter()`가 호출되면 연결된 Presenter의 `Dispose()`가 호출됩니다. Presenter에서 추가로 소유하는 `IDisposable` 객체는 상속받은 `DisposablePoco.Add()`로 등록할 수 있습니다.

`Awake()` 전에 View가 파괴되는 예외적인 수명주기에서도 Presenter가 남지 않도록 `MonoViewObserver`가 임시로 추적합니다. 현재 Presenter가 필요하면 다음처럼 안전하게 조회할 수 있습니다.

```csharp
if (sampleView.TryGetPresenter<SamplePresenter>(out var presenter))
{
    // presenter 사용
}
```

Editor에서는 `MonoView` Inspector에 현재 바인딩된 Presenter 정보와 정리 버튼이 표시됩니다. Editor 전용 코드는 Runtime 어셈블리와 분리되어 플레이어 빌드에 포함되지 않습니다.

## Third-party notices

현재 `DisposablePoco`는 패키지에 필요한 소유 리소스 등록과 일괄 정리 기능만 독립적으로 구현합니다. 과거 저장소 리비전의 UniRx 관련 고지는 [Third Party Notices.md](Third%20Party%20Notices.md)를 확인하세요.
