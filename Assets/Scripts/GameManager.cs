using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class GameManager : MonoBehaviour
{
    [SerializeField] GameObject popup;
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text message;
    [SerializeField] Button continueButton;
    [SerializeField] Button restartButton;
    [SerializeField] AirplaneController airplane;
    [SerializeField] TMP_Text flightStatus;
    public bool PopupVisible => popup != null && popup.activeSelf;

    void Awake()
    {
        Time.timeScale = 1f;
        popup.SetActive(false);
        title.text = "Welcome to Unity!";
        message.text = "Do you want to continue?";
        continueButton.GetComponentInChildren<TMP_Text>(true).text = "Continue";
        restartButton.GetComponentInChildren<TMP_Text>(true).text = "Restart";
        continueButton.onClick.AddListener(NextScene);
        restartButton.onClick.AddListener(RestartScene);
    }
    void Update()
    {
        if (flightStatus == null || PopupVisible) return;
        flightStatus.text = airplane.HasTakenOff ? "AIRBORNE  /  Hold W to reach the orange gate" : "READY FOR TAKEOFF  /  Press SPACE";
    }
    public void ShowPopup()
    {
        if (PopupVisible) return;
        airplane.ControlsLocked = true;
        popup.SetActive(true);
        EventSystem.current?.SetSelectedGameObject(continueButton.gameObject);
    }
    public void NextScene() { Time.timeScale = 1f; SceneManager.LoadScene("Scene_1"); }
    public void RestartScene() { Time.timeScale = 1f; SceneManager.LoadScene("Scene_3_2"); }
    void OnDestroy()
    {
        if (continueButton != null) continueButton.onClick.RemoveListener(NextScene);
        if (restartButton != null) restartButton.onClick.RemoveListener(RestartScene);
    }
}
