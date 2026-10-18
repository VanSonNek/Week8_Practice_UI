using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public sealed class EnterGame : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    public bool HasTriggered { get; private set; }
    void OnTriggerEnter(Collider other)
    {
        if (HasTriggered || !other.CompareTag("Player")) return;
        var airplane = other.GetComponentInParent<AirplaneController>();
        if (airplane == null || !airplane.HasTakenOff) return;
        HasTriggered = true;
        gameManager.ShowPopup();
    }
}
