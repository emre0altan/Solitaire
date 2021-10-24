using UnityEngine;
using UnityEngine.Events;

public struct TouchInput
{
    public enum TouchPhase
    {
        Started,
        Moved,
        Ended,
        NotActive
    }

    public Vector2 FirstWorldPosition;
    public Vector2 WorldPosition;
    public Vector2 FirstScreenPosition;
    public Vector2 ScreenPosition;
    public Vector2 DeltaScreenPosition;
    public Vector2 DeltaWorldPosition;
    public TouchPhase Phase;
}

public delegate void TouchEvent(TouchInput touch);

public class TouchManager : MonoBehaviour
{
    public static TouchManager Instance;

    private TouchInput _touch;

    public bool isActive = true;

    public TouchEvent onTouchBegan = null, onTouchMoved = null, onTouchEnded = null;

    public UnityAction OnTouch;

    private void Awake()
    {

        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }


    private void Update()
    {
        if (!isActive) return;

        ApplyTouchForCurrentPlatform();

        switch (_touch.Phase)
        {
            case TouchInput.TouchPhase.NotActive:
                return;
            case TouchInput.TouchPhase.Started:
                onTouchBegan?.Invoke(_touch);
                if (OnTouch != null)
                    OnTouch();
                break;
            case TouchInput.TouchPhase.Moved:
                onTouchMoved?.Invoke(_touch);
                break;
            case TouchInput.TouchPhase.Ended:
                onTouchEnded?.Invoke(_touch);
                break;
        }
    }

    private void ApplyTouchForCurrentPlatform()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN

        if (!Input.GetMouseButton(0) && !Input.GetMouseButtonUp(0))
        {
            _touch.Phase = TouchInput.TouchPhase.NotActive;
        }
        else
        {
            Vector2 screenPosition = Input.mousePosition;
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            if (Input.GetMouseButtonDown(0))
            {
                _touch.Phase = TouchInput.TouchPhase.Started;
                _touch.FirstScreenPosition = screenPosition;
                _touch.FirstWorldPosition = worldPosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _touch.Phase = TouchInput.TouchPhase.Ended;
            }
            else
            {
                _touch.Phase = TouchInput.TouchPhase.Moved;
                _touch.DeltaScreenPosition = screenPosition - _touch.ScreenPosition;
                _touch.DeltaWorldPosition = worldPosition - _touch.WorldPosition;
            }

            _touch.ScreenPosition = screenPosition;
            _touch.WorldPosition = worldPosition;
        }

#else

		if (Input.touchCount > 0)
		{
			var inputTouch = Input.GetTouch(0);
			
			var screenPosition = inputTouch.position;
			Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

			if (inputTouch.phase == TouchPhase.Began)
			{
				_touch.Phase = TouchInput.TouchPhase.Started;
				_touch.FirstScreenPosition = screenPosition;
				_touch.FirstWorldPosition = worldPosition;
	
				_touch.ScreenPosition = screenPosition;
				_touch.WorldPosition = worldPosition;
			}
			else if (inputTouch.phase == TouchPhase.Moved || inputTouch.phase == TouchPhase.Stationary)
			{
				_touch.Phase = TouchInput.TouchPhase.Moved;
				_touch.DeltaScreenPosition = screenPosition - _touch.ScreenPosition;
				_touch.DeltaWorldPosition = worldPosition - _touch.WorldPosition;
	
				_touch.ScreenPosition = screenPosition;
				_touch.WorldPosition = worldPosition;
			}
			else
			{
				_touch.Phase = TouchInput.TouchPhase.Ended;
			}
		}
		else
		{
			_touch.Phase = TouchInput.TouchPhase.NotActive;
		}
		
#endif
    }
}