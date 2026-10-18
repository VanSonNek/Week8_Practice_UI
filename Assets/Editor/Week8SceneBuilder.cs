using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Explicit menu command only: never rebuilds automatically or changes unrelated scenes.
public static class Week8SceneBuilder
{
    static readonly Color Navy = new Color32(12, 25, 48, 255);
    static readonly Color Blue = new Color32(30, 106, 238, 255);
    static readonly Color Orange = new Color32(255, 155, 66, 255);
    static readonly Color Muted = new Color32(171, 193, 221, 255);
    static TMP_FontAsset font;
    static Material white, blue, orange, dark, grass, concrete, glass;

    [MenuItem("Week 8/Build Practice Scenes")]
    public static void Build()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play Mode before building.");
        if (Application.productName != "Week8_Practice_UI" && !Application.dataPath.EndsWith("Week8_Practice_UI/Assets"))
            throw new InvalidOperationException("Wrong project.");
        var current = SceneManager.GetActiveScene();
        if (current.isDirty) throw new InvalidOperationException("Save current edits before building.");
        System.IO.Directory.CreateDirectory("Assets/Materials/Week8");
        System.IO.Directory.CreateDirectory("Assets/Scenes");
        AssetDatabase.Refresh();
        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        if (font == null) throw new InvalidOperationException("Import TMP Essential Resources first.");
        white = Mat("Ivory", new Color32(233,241,250,255));
        blue = Mat("AviationBlue", Blue); orange = Mat("SignalOrange", Orange);
        dark = Mat("Runway", new Color32(35,48,65,255));
        grass = Mat("Landscape", new Color32(63,113,103,255));
        concrete = Mat("Concrete", new Color32(108,133,144,255));
        glass = Mat("Cockpit", new Color32(20,51,83,255));
        BuildMain(); BuildNext();
        EditorBuildSettings.scenes = new[] {
            new EditorBuildSettingsScene("Assets/Scenes/Scene_3_2.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Scene_1.unity", true)
        };
        AssetDatabase.SaveAssets();
        EditorSceneManager.OpenScene("Assets/Scenes/Scene_3_2.unity");
        Debug.Log("Week 8: both scenes built and saved. Ready to fly.");
    }

    static Material Mat(string name, Color color)
    {
        string path = "Assets/Materials/Week8/" + name + ".mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null) { material = new Material(Shader.Find("Universal Render Pipeline/Lit")); AssetDatabase.CreateAsset(material, path); }
        material.color = color; EditorUtility.SetDirty(material); return material;
    }
    static GameObject Group(string name, Transform parent = null)
    {
        var go = new GameObject(name); if (parent != null) go.transform.SetParent(parent, false); return go;
    }
    static GameObject Shape(string name, PrimitiveType type, Transform parent, Vector3 position, Vector3 scale, Material material, bool collision = false)
    {
        var go = GameObject.CreatePrimitive(type); go.name = name;
        if (parent != null) go.transform.SetParent(parent, false);
        go.transform.localPosition = position; go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = material;
        if (!collision) UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
        return go;
    }
    static void Set(UnityEngine.Object target, string field, UnityEngine.Object value)
    {
        var data = new SerializedObject(target); var property = data.FindProperty(field);
        if (property == null) throw new InvalidOperationException("Missing field: " + field);
        property.objectReferenceValue = value; data.ApplyModifiedPropertiesWithoutUndo();
    }
    static Camera Environment()
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(.65f,.74f,.85f);
        RenderSettings.fog = true; RenderSettings.fogColor = new Color(.56f,.74f,.83f);
        RenderSettings.fogMode = FogMode.ExponentialSquared; RenderSettings.fogDensity = .005f;
        var sun = Group("Directional Light").AddComponent<Light>(); sun.type = LightType.Directional;
        sun.intensity = 2f; sun.shadows = LightShadows.Soft; sun.transform.rotation = Quaternion.Euler(45,-35,0);
        var camera = Group("Main Camera").AddComponent<Camera>(); camera.tag = "MainCamera";
        camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(.56f,.74f,.83f);
        camera.fieldOfView = 55; camera.farClipPlane = 600; camera.gameObject.AddComponent<AudioListener>();
        return camera;
    }
    static Canvas Canvas()
    {
        var go = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = go.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = go.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1440,900); scaler.matchWidthOrHeight = 0f;
        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        return canvas;
    }
    static RectTransform Rect(string name, Transform parent, Vector2 anchor, Vector2 position, Vector2 size)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false); rect.anchorMin = rect.anchorMax = anchor;
        rect.anchoredPosition = position; rect.sizeDelta = size; return rect;
    }
    static Image Image(string name, Transform parent, Vector2 anchor, Vector2 position, Vector2 size, Color color)
    {
        var image = Rect(name,parent,anchor,position,size).gameObject.AddComponent<Image>(); image.color = color; return image;
    }
    static TMP_Text Text(string name, Transform parent, string text, Vector2 anchor, Vector2 position, Vector2 size, float fontSize, Color color, TextAlignmentOptions align = TextAlignmentOptions.Center)
    {
        var label = Rect(name,parent,anchor,position,size).gameObject.AddComponent<TextMeshProUGUI>();
        label.font = font; label.text = text; label.fontSize = fontSize; label.color = color;
        label.alignment = align; label.raycastTarget = false; return label;
    }
    static Button Button(string name, Transform parent, string label, Vector2 position, Vector2 size, bool primary)
    {
        var image = Image(name,parent,new Vector2(.5f,.5f),position,size,primary ? Blue : new Color(.13f,.22f,.35f));
        var button = image.gameObject.AddComponent<Button>(); button.targetGraphic = image;
        var colors = button.colors; colors.normalColor = Color.white; colors.highlightedColor = new Color(.78f,.87f,1f);
        colors.pressedColor = new Color(.52f,.66f,.85f); colors.selectedColor = new Color(.88f,.93f,1f); colors.fadeDuration = .12f;
        button.colors = colors;
        Text("Text", image.transform,label,new Vector2(.5f,.5f),Vector2.zero,size,23,Color.white).fontStyle = FontStyles.Bold;
        return button;
    }
    static void BuildMain()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var camera = Environment();
        var environment = Group("Airport Environment").transform;
        Shape("Landscape",PrimitiveType.Cube,environment,new Vector3(0,-.4f,35),new Vector3(200,.5f,220),grass,true);
        Shape("Runway Shoulder",PrimitiveType.Cube,environment,new Vector3(0,-.12f,29),new Vector3(20,.2f,105),concrete);
        Shape("Runway",PrimitiveType.Cube,environment,new Vector3(0,0,29),new Vector3(14,.12f,105),dark,true);
        for (int i=0;i<13;i++) Shape("Centerline " + i,PrimitiveType.Cube,environment,new Vector3(0,.08f,-18+i*8),new Vector3(.18f,.025f,3),white);
        for (int i=0;i<19;i++) foreach (int side in new[] {-1,1})
        {
            Shape("Edge Light",PrimitiveType.Cube,environment,new Vector3(side*7.7f,.22f,-20+i*6),new Vector3(.18f,.3f,.55f),orange);
        }
        for (int i=0;i<6;i++) Shape("Threshold Stripe",PrimitiveType.Cube,environment,new Vector3(-5+i*2,.085f,-9),new Vector3(.7f,.03f,5),white);
        Shape("Terminal",PrimitiveType.Cube,environment,new Vector3(-28,3,38),new Vector3(18,6,30),concrete);
        Shape("Terminal Windows",PrimitiveType.Cube,environment,new Vector3(-18.9f,3.7f,38),new Vector3(.15f,2,26),glass);
        Shape("Control Tower",PrimitiveType.Cylinder,environment,new Vector3(-25,7,10),new Vector3(3,7,3),concrete);
        Shape("Tower Cabin",PrimitiveType.Cube,environment,new Vector3(-25,14,10),new Vector3(6,3,6),glass);
        var plane = Group("Airplane"); plane.tag = "Player"; plane.transform.position = new Vector3(0,1.25f,0);
        var body = plane.AddComponent<Rigidbody>(); body.useGravity = false; body.isKinematic = true;
        body.interpolation = RigidbodyInterpolation.Interpolate; body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        var collider = plane.AddComponent<BoxCollider>(); collider.center = new Vector3(0,.1f,0); collider.size = new Vector3(7.2f,1.7f,6.7f);
        var controller = plane.AddComponent<AirplaneController>(); var model = Group("Model",plane.transform).transform;
        Shape("Fuselage",PrimitiveType.Sphere,model,Vector3.zero,new Vector3(1.2f,1.2f,6.2f),white);
        Shape("Nose",PrimitiveType.Sphere,model,new Vector3(0,0,2.5f),new Vector3(1.1f,1.1f,1.5f),blue);
        Shape("Cockpit Canopy",PrimitiveType.Sphere,model,new Vector3(0,.58f,.6f),new Vector3(.94f,.75f,1.8f),glass);
        var left = Shape("Left Wing",PrimitiveType.Cube,model,new Vector3(-2,.05f,.2f),new Vector3(4,.16f,1.6f),blue);
        left.transform.localRotation = Quaternion.Euler(0,-10,-3);
        var right = Shape("Right Wing",PrimitiveType.Cube,model,new Vector3(2,.05f,.2f),new Vector3(4,.16f,1.6f),blue);
        right.transform.localRotation = Quaternion.Euler(0,10,3);
        Shape("Left Wing Tip",PrimitiveType.Cube,model,new Vector3(-3.8f,.13f,-.1f),new Vector3(.4f,.19f,1.6f),orange);
        Shape("Right Wing Tip",PrimitiveType.Cube,model,new Vector3(3.8f,.13f,-.1f),new Vector3(.4f,.19f,1.6f),orange);
        Shape("Horizontal Tail",PrimitiveType.Cube,model,new Vector3(0,.3f,-2.3f),new Vector3(2.8f,.14f,1f),blue);
        var tail = Shape("Vertical Tail",PrimitiveType.Cube,model,new Vector3(0,.85f,-2.4f),new Vector3(.15f,1.5f,1.1f),orange);
        tail.transform.localRotation = Quaternion.Euler(-16,0,0);
        foreach (float x in new[] {-.65f,.65f})
        {
            Shape("Landing Strut",PrimitiveType.Cube,model,new Vector3(x,-.65f,.6f),new Vector3(.12f,.7f,.12f),concrete);
            var wheel = Shape("Wheel",PrimitiveType.Cylinder,model,new Vector3(x,-.97f,.6f),new Vector3(.46f,.12f,.46f),dark);
            wheel.transform.localRotation = Quaternion.Euler(0,0,90);
        }
        var propeller = Group("Propeller",model); propeller.transform.localPosition = new Vector3(0,0,3.35f);
        Shape("Blade",PrimitiveType.Cube,propeller.transform,Vector3.zero,new Vector3(.16f,2.5f,.1f),dark);
        Shape("Hub",PrimitiveType.Sphere,propeller.transform,Vector3.zero,new Vector3(.32f,.32f,.32f),orange);
        Set(controller,"propeller",propeller.transform);
        var follow = camera.gameObject.AddComponent<FlightCamera>(); Set(follow,"target",plane.transform);
        camera.transform.position = plane.transform.position + new Vector3(9,6,-15);
        camera.transform.LookAt(plane.transform.position + new Vector3(0,1,9));
        var wall = Group("EnterWall"); wall.transform.position = new Vector3(0,4,48);
        var trigger = wall.AddComponent<BoxCollider>(); trigger.isTrigger = true; trigger.size = new Vector3(19,12,1.5f);
        var enter = wall.AddComponent<EnterGame>();
        Shape("Left Gate Post",PrimitiveType.Cube,wall.transform,new Vector3(-9,0,0),new Vector3(.5f,12,.5f),orange);
        Shape("Right Gate Post",PrimitiveType.Cube,wall.transform,new Vector3(9,0,0),new Vector3(.5f,12,.5f),orange);
        Shape("Gate Header",PrimitiveType.Cube,wall.transform,new Vector3(0,6,0),new Vector3(18.5f,.5f,.5f),orange);
        var gateLabel = Group("Gate Label",wall.transform).AddComponent<TextMeshPro>(); gateLabel.font = font;
        gateLabel.text = "ENTER  /  NEXT CHAPTER"; gateLabel.fontSize = 7; gateLabel.alignment = TextAlignmentOptions.Center;
        gateLabel.color = Color.white; gateLabel.transform.localPosition = new Vector3(0,6.9f,-.3f); gateLabel.transform.localRotation = Quaternion.identity;
        gateLabel.rectTransform.sizeDelta = new Vector2(19,2);
        var canvas = Canvas(); var hud = Rect("Flight HUD",canvas.transform,new Vector2(.5f,.5f),Vector2.zero,Vector2.zero);
        hud.anchorMin = Vector2.zero; hud.anchorMax = Vector2.one; hud.sizeDelta = Vector2.zero;
        var top = Image("Header",hud,new Vector2(.5f,1),new Vector2(0,-58),new Vector2(1360,76),Navy);
        Text("Brand",top.transform,"FLIGHT LAB  <color=#FF9B42>/</color>  08",new Vector2(0,.5f),new Vector2(245,0),new Vector2(440,50),28,Color.white,TextAlignmentOptions.Left).fontStyle = FontStyles.Bold;
        Text("Lesson",top.transform,"UNITY UI DESIGN     /     PRACTICE 3.3",new Vector2(1,.5f),new Vector2(-280,0),new Vector2(510,50),18,Muted,TextAlignmentOptions.Right);
        var status = Image("Status Card",hud,new Vector2(.5f,0),new Vector2(0,88),new Vector2(900,102),Navy);
        var flightStatus = Text("Flight Status",status.transform,"READY FOR TAKEOFF  /  Press SPACE",new Vector2(.5f,.5f),new Vector2(0,17),new Vector2(870,35),23,Color.white);
        Text("Controls",status.transform,"SPACE  Take off        W  Fly forward        TARGET  Orange gate",new Vector2(.5f,.5f),new Vector2(0,-21),new Vector2(870,30),17,Muted);
        var panel = Image("Panel",canvas.transform,new Vector2(.5f,.5f),Vector2.zero,Vector2.zero,new Color(.02f,.05f,.1f,.78f));
        panel.rectTransform.anchorMin = Vector2.zero; panel.rectTransform.anchorMax = Vector2.one; panel.rectTransform.sizeDelta = Vector2.zero;
        var frame = Image("Frame",panel.transform,new Vector2(.5f,.5f),Vector2.zero,new Vector2(680,450),Navy);
        var shadow = frame.gameObject.AddComponent<Shadow>(); shadow.effectColor = new Color(0,0,0,.45f); shadow.effectDistance = new Vector2(0,-12);
        Image("Accent",frame.transform,new Vector2(.5f,1),new Vector2(0,-3),new Vector2(680,6),Orange);
        Text("Eyebrow",frame.transform,"CHECKPOINT REACHED  /  01",new Vector2(.5f,.5f),new Vector2(0,155),new Vector2(590,30),17,Orange).fontStyle = FontStyles.Bold;
        var title = Text("Title",frame.transform,"Welcome to Unity!",new Vector2(.5f,.5f),new Vector2(0,85),new Vector2(620,72),44,Color.white); title.fontStyle = FontStyles.Bold;
        var message = Text("Message",frame.transform,"Do you want to continue?",new Vector2(.5f,.5f),new Vector2(0,20),new Vector2(600,52),25,Muted);
        Image("Divider",frame.transform,new Vector2(.5f,.5f),new Vector2(0,-35),new Vector2(560,1),new Color(.23f,.32f,.45f));
        var next = Button("ContinueButton",frame.transform,"Continue",new Vector2(-145,-102),new Vector2(270,64),true);
        var restart = Button("RestartButton",frame.transform,"Restart",new Vector2(145,-102),new Vector2(270,64),false);
        Text("Footer",frame.transform,"Your next chapter is ready.",new Vector2(.5f,.5f),new Vector2(0,-177),new Vector2(580,30),17,Muted);
        var gm = camera.gameObject.AddComponent<GameManager>();
        Set(gm,"popup",panel.gameObject); Set(gm,"title",title); Set(gm,"message",message); Set(gm,"continueButton",next); Set(gm,"restartButton",restart);
        Set(gm,"airplane",controller); Set(gm,"flightStatus",flightStatus); Set(enter,"gameManager",gm);
        canvas.gameObject.AddComponent<ResponsiveFlightUI>();
        panel.gameObject.SetActive(false);
        EditorSceneManager.SaveScene(scene,"Assets/Scenes/Scene_3_2.unity");
    }
    static void BuildNext()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        var camera = Environment(); camera.transform.position = new Vector3(0,4,-10); camera.transform.LookAt(Vector3.zero);
        var canvas = Canvas();
        var background = Image("Next Chapter",canvas.transform,new Vector2(.5f,.5f),Vector2.zero,Vector2.zero,Navy);
        background.rectTransform.anchorMin = Vector2.zero; background.rectTransform.anchorMax = Vector2.one;
        var frame = Image("Arrival Card",background.transform,new Vector2(.5f,.5f),Vector2.zero,new Vector2(840,440),new Color(.07f,.14f,.25f));
        Image("Accent",frame.transform,new Vector2(.5f,1),new Vector2(0,-3),new Vector2(840,6),Orange);
        Text("Eyebrow",frame.transform,"FLIGHT LAB  /  ARRIVAL CONFIRMED",new Vector2(.5f,.5f),new Vector2(0,135),new Vector2(780,40),19,Orange);
        Text("Welcome",frame.transform,"Welcome to the Next Scene!",new Vector2(.5f,.5f),new Vector2(0,55),new Vector2(790,80),40,Color.white).fontStyle = FontStyles.Bold;
        Text("Message",frame.transform,"Takeoff complete. A new chapter begins.",new Vector2(.5f,.5f),new Vector2(0,-15),new Vector2(780,50),24,Muted);
        var back = Button("BackButton",frame.transform,"Back to Runway",new Vector2(0,-112),new Vector2(310,64),true);
        var ui = camera.gameObject.AddComponent<NextSceneUI>(); Set(ui,"backButton",back);
        canvas.gameObject.AddComponent<ResponsiveFlightUI>();
        EditorSceneManager.SaveScene(scene,"Assets/Scenes/Scene_1.unity");
    }
}
