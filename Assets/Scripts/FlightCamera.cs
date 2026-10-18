using UnityEngine;

public sealed class FlightCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new Vector3(9f, 6f, -15f);
    Camera viewCamera;
    void Awake() { viewCamera = GetComponent<Camera>(); }
    void LateUpdate()
    {
        if (target == null) return;
        float fit = viewCamera == null ? 1f : Mathf.Max(1f, Mathf.Sqrt(1.6f / viewCamera.aspect));
        transform.position = target.position + offset * fit;
        transform.LookAt(target.position + Vector3.forward * 9f + Vector3.up);
    }
}
