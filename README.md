# UI 구현에 사용할 MVP패턴 코드 베이스
UNITY에서 효율적인 UI 구현을 위한 MVP패턴의 View 및 Presenter 코드 베이스입니다.
>Last update : 24/10/13
<br>
목차
<br>

1.[View](#View)<br>
2.[Presenter](#Presenter)<br>
3.[Example](#Example)<br>
<br>

## View
UI를 어떻게 보여줄 지 담당하는 부분입니다.<br>
해당 클래스를 상속받아 필요한 View를 작성하면 됩니다.<br>
그 어떠한 데이터의 변경도 View에서는 발생시켜선 안되는 것이 원칙입니다.

```C#
public class MonoView : MonoBehaviour, IDisposable
{
    ...
    //View에 Presenter를 바인딩하는 메서드 입니다.
    //만약 이미 Presenter가 바인딩 되어 있다면, View와 Presenter의 1:1 대응을 위해 해당 Presenter는 자동으로 Dispose처리 됩니다.
    public virtual T2 Binding<T1, T2>() where T1 : MonoView where T2 : Presenter<T1>, new()
    {
        var presenter = new T2();
        presenter.View = this as T1;

        _presenter?.Dispose();
        _presenter = presenter;

        return presenter;
    }
}
```

## Presenter
UI 조작을 위해 외부 Model로 부터 데이터를 받아 View로 전달하는 역할을 하는 Presenter입니다.<br>
최대한 단순하게 작성되어 View의 레퍼런스 하나만 가지고 있습니다.<br>
해당 클래스를 상속받아 필요한 Presenter를 작성하면 됩니다.

Presenter의 베이스 클래스인 DisposablePoco는 UniRx 사용 시 구독 관리를 간편하게 하기 위해 UniRx의 CompositeDisposable 클래스의 코드를 가져와 사용하였습니다.
```C#
public class Presenter<T> : DisposablePoco where T : MonoView
{
    private T _view;
    public T View
    {
        get { return _view; }
        set { _view = value; }
    }
}
```

## Example
```C#
public class RootUI : MonoBehaviour
{
    [SerializeField]
    private SampleView _sView;

    private void Start()
    {
        _sView.Binding<SampleView, SamplePresenter>()
            .Initialize();
    }
}
```
```C#
public class SampleView : MonoView
{
    [SerializeField]
    private Button _testBtn;
    public Button TestBtn => _testBtn;
}

public class SamplePresenter : Presenter<SampleView>
{
    public void Initialize()
    {
        View.TestBtn.onClick.AddListener(OnClickBtn);
    }

    //예시 코드기에 static한 문자열을 사용했으나 본래 외부 Model에서 문자열 받아와 출력
    private void OnClickBtn()
    {
        Debug.Log("Button Clicked");
    }

    public override void Dispose()
    {
        View.TestBtn.onClick.RemoveListener(OnClickBtn);
    }
}
```
