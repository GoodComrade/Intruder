using UnityEngine;
using UnityEngine.EventSystems;

public class TouchRepositionableJoystick : TouchDynamicJoystick
{
    protected void UpdateBackgroundPosition()
    {
        BackgroundCanvasGroup.transform.position = _newPosition;
        SetNeutralPosition(_newPosition);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        if (Mathf.Abs(_newTargetPosition.magnitude) > MaxRange)
        {
            MoveBackground(eventData);
        }
    }

    protected virtual void MoveBackground(PointerEventData eventData)
    {
        float overDragMagnitude = _newTargetPosition.magnitude - MaxRange;
        
        float overY = (Mathf.Abs(_newTargetPosition.y) / (Mathf.Abs(_newTargetPosition.x) + Mathf.Abs(_newTargetPosition.y))) 
            * (_newTargetPosition.y > 0 ? overDragMagnitude : -overDragMagnitude);
        
        float overX = (Mathf.Abs(_newTargetPosition.x) / (Mathf.Abs(_newTargetPosition.y) + Mathf.Abs(_newTargetPosition.x)))
            * (_newTargetPosition.x > 0 ? overDragMagnitude : -overDragMagnitude);

        Vector3 overDrag = new Vector3(overX, overY, 0);
        _newPosition += overDrag;
        UpdateBackgroundPosition();
    }

    protected override void ClampKnobPosition()
    {
    }
}