using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class NextSceneUI : MonoBehaviour
{
    [SerializeField] Button backButton;
    void Awake() { Time.timeScale = 1f; backButton.onClick.AddListener(ReturnToRunway); }
    public void ReturnToRunway() { Time.timeScale = 1f; SceneManager.LoadScene("Scene_3_2"); }
    void OnDestroy() { if (backButton != null) backButton.onClick.RemoveListener(ReturnToRunway); }
}
