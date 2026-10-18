using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public sealed class ResponsiveFlightUI : MonoBehaviour
{
    bool? portrait;
    void OnEnable() { ApplyLayout(); }
    void Update()
    {
        if (portrait != (Screen.height > Screen.width)) ApplyLayout();
    }
    void ApplyLayout()
    {
        portrait = Screen.height > Screen.width;
        bool narrow = portrait.Value;
        float width = narrow ? 800f : 1440f;
        GetComponent<CanvasScaler>().referenceResolution = new Vector2(width, 900f);
        Resize("Flight HUD/Header", width - 80f);
        var lesson = transform.Find("Flight HUD/Header/Lesson");
        if (lesson != null) lesson.gameObject.SetActive(!narrow);
        Resize("Flight HUD/Status Card", narrow ? 720f : 900f);
        Label("Flight HUD/Status Card/Flight Status", narrow ? 700f : 870f, narrow ? 20f : 23f);
        Label("Flight HUD/Status Card/Controls", narrow ? 700f : 870f, narrow ? 14f : 17f);
        Resize("Next Chapter/Arrival Card", narrow ? 720f : 840f);
        Resize("Next Chapter/Arrival Card/Accent", narrow ? 720f : 840f);
        Label("Next Chapter/Arrival Card/Welcome", narrow ? 680f : 790f, narrow ? 32f : 40f);
        Label("Next Chapter/Arrival Card/Message", narrow ? 680f : 780f, narrow ? 21f : 24f);
        Label("Next Chapter/Arrival Card/Eyebrow", narrow ? 680f : 780f, 19f);
    }
    void Resize(string path, float width)
    {
        var rect = transform.Find(path) as RectTransform;
        if (rect != null) rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }
    void Label(string path, float width, float size)
    {
        Resize(path, width);
        var target = transform.Find(path);
        if (target != null) target.GetComponent<TMP_Text>().fontSize = size;
    }
}
