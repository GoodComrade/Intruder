using UnityEngine;
using UnityEngine.EventSystems;

public class TouchDynamicJoystick : TouchJoystick
{
    protected Vector3 _newPosition;

    public override void Initialize()
    {
        base.Initialize();
    }

    /// <summary>
    /// When the zone is pressed, we move our joystick accordingly
    /// </summary>
    /// <param name="data">Data.</param>
    public override void OnPointerDown(PointerEventData data)
    {
        _newPosition = data.position;
        _newPosition.z = this.transform.position.z;

        // we define a new neutral position
        BackgroundCanvasGroup.transform.position = _newPosition;
        SetNeutralPosition(_newPosition);
        base.OnPointerDown(data);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        BackgroundCanvasGroup.transform.position = _initialPosition;
    }
}