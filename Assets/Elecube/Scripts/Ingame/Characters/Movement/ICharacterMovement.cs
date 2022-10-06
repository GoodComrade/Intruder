
public interface ICharacterMovement
{
    bool IsMoving();
    void SetMovementDisabled(bool disabled);
    void AddSpeedModifier(float modifier, float duration);
}
