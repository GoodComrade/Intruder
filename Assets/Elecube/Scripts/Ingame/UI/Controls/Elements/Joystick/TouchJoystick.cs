using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;


[RequireComponent(typeof(Rect))]
[RequireComponent(typeof(CanvasGroup))]
public class TouchJoystick : OnScreenControl, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public CanvasGroup KnobCanvasGroup;
    public CanvasGroup BackgroundCanvasGroup;

    public float MaxRange = 1.5f;

    [Header("Binding")] [InputControl(layout = "Vector2")] [SerializeField]
    private string _controlPath;

    /// Store neutral position of the stick
    protected Vector2 _neutralPosition;
    protected Vector3 _initialPosition;

    /// working vector
    protected Vector2 _newTargetPosition;

    protected Vector3 _newJoystickPosition;
    protected float _initialZPosition;
    
    
    public float PressedKnobOpacity = 0.5f;
    protected float _initialKnobOpacity;

    protected virtual void Start()
    {
        Initialize();
    }

    public virtual void Initialize()
    {
        SetNeutralPosition();
        _initialPosition = BackgroundCanvasGroup.transform.position;
        _initialZPosition = KnobCanvasGroup.transform.position.z;
        _initialKnobOpacity = KnobCanvasGroup.alpha;
    }
    
    /// <summary>
    /// Sets the neutral position of the joystick
    /// </summary>
    public virtual void SetNeutralPosition()
    {
        _neutralPosition = KnobCanvasGroup.transform.position;
    }

    public virtual void SetNeutralPosition(Vector3 newPosition)
    {
        _neutralPosition = newPosition;
    }

    /// <summary>
    /// Handles dragging of the joystick
    /// </summary>
    public virtual void OnDrag(PointerEventData eventData)
    {
        _newTargetPosition = eventData.position - _neutralPosition;

        ClampKnobPosition();

        _newJoystickPosition = _neutralPosition + _newTargetPosition;
        _newJoystickPosition.z = _initialZPosition;

        // We move the joystick to its dragged position
        KnobCanvasGroup.transform.position = _newJoystickPosition;

        SendValueToControl(new Vector2(EvaluateInputValue(_newTargetPosition.x),
            EvaluateInputValue(_newTargetPosition.y)));
    }

    protected virtual void ClampKnobPosition()
    {
        // We clamp the stick's position to let it move only inside its defined max range
        _newTargetPosition = Vector2.ClampMagnitude(_newTargetPosition, MaxRange);
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        SendValueToControl(Vector2.zero);
    }

    protected void SetKnobPressed()
    {
        KnobCanvasGroup.alpha = PressedKnobOpacity;
    }
    
    protected void SetKnobUnpressed()
    {
        KnobCanvasGroup.alpha = _initialKnobOpacity;
    }

    protected virtual float EvaluateInputValue(float vectorPosition)
    {
        return Mathf.InverseLerp(0, MaxRange, Mathf.Abs(vectorPosition)) * Mathf.Sign(vectorPosition);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Initialize();
    }

    protected override string controlPathInternal
    {
        get => _controlPath;
        set => _controlPath = value;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        SetKnobPressed();
        OnDrag(eventData);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        SetKnobUnpressed();
        KnobCanvasGroup.transform.position = _initialPosition;
    }
}