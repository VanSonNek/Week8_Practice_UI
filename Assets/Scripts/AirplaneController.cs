using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public sealed class AirplaneController : MonoBehaviour
{
    [SerializeField] float forwardSpeed = 11f;
    [SerializeField] float cruiseHeight = 4f;
    [SerializeField] float climbSpeed = 2.5f;
    [SerializeField] Transform propeller;
    public bool HasTakenOff { get; private set; }
    public bool ControlsLocked { get; set; }
    public bool IsMoving { get; private set; }
    Rigidbody body;
    bool forwardHeld;

    void Awake() { body = GetComponent<Rigidbody>(); }
    void Update()
    {
        var keyboard = Keyboard.current;
        if (ControlsLocked || keyboard == null) { forwardHeld = false; return; }
        if (keyboard.spaceKey.wasPressedThisFrame) HasTakenOff = true;
        forwardHeld = keyboard.wKey.isPressed;
        if (propeller != null) propeller.Rotate(0f, 0f, (HasTakenOff ? 1500f : 250f) * Time.deltaTime, Space.Self);
    }
    void FixedUpdate()
    {
        IsMoving = !ControlsLocked && HasTakenOff && forwardHeld;
        if (ControlsLocked || !HasTakenOff) return;
        var position = body.position;
        position.y = Mathf.MoveTowards(position.y, cruiseHeight, climbSpeed * Time.fixedDeltaTime);
        if (IsMoving) position += transform.forward * (forwardSpeed * Time.fixedDeltaTime);
        body.MovePosition(position);
    }
}
