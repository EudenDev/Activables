// ANDRES

/// <summary>
/// Gives functions that can be controlled by other scripts. <para></para>
/// NOTE: Prefer inheriting from <see cref="Activable"/> class instead.
/// </summary>
public interface IActivable
{
    /// <summary>
    /// Returns true if is activated or activating
    /// </summary>
    bool IsActivated { set; get; } 

    void Activate();
    void Deactivate();
    void ActivateInmediate();
    void DeactivateInmediate();
}

//public enum ObjectState { Activation, Action, Deactivation }

