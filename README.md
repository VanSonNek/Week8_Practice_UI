# Week 8 — Practice UI Design

Unity **6000.5.10f1** · uGUI **2.5.0** · TextMeshPro · Input System **1.20.0**.

## Chạy bài

1. Mở project `Week8_Practice_UI` bằng Unity 6000.5.10f1.
2. Mở `Assets/Scenes/Scene_3_2.unity` và nhấn **Play**.
3. Click Game View để nhận bàn phím. Nhấn **Space** để cất cánh lên cao độ 4.
4. Giữ **W** để bay về cổng màu cam. W chỉ hoạt động sau khi cất cánh.
5. Khi máy bay chạm vùng Trigger của cổng, Popup xuất hiện và máy bay dừng nhận điều khiển.
6. **Continue** mở `Scene_1`; **Restart** tải lại `Scene_3_2`.
7. Trong Scene_1, **Back to Runway** quay về Scene chính.

Popup được lưu ở trạng thái inactive trong Scene và được ẩn lại trong `GameManager.Awake()`.
Không dùng OnGUI. Không thay đổi Time.timeScale khi mở Popup; khóa điều khiển máy bay thay cho dừng toàn bộ game.

## Scene và Build Profiles

- `Assets/Scenes/Scene_3_2.unity`: Scene chính, build index **0**, enabled.
- `Assets/Scenes/Scene_1.unity`: Scene tiếp theo, build index **1**, enabled.
- `Assets/Scenes/SampleScene.unity`: Scene mẫu ban đầu được giữ lại, không nằm trong Build Scene List của bài.

## Scripts

| File | Chức năng |
| --- | --- |
| `Assets/Scripts/AirplaneController.cs` | Đọc Keyboard của Input System; Space cất cánh, giữ W bay; di chuyển Rigidbody trong FixedUpdate; quay cánh quạt. |
| `Assets/Scripts/GameManager.cs` | Khởi tạo chữ, quản lý Popup và HUD, gắn/tháo listener, Continue/Restart. Gắn trên Main Camera luôn active. |
| `Assets/Scripts/EnterGame.cs` | OnTriggerEnter, lọc tag Player và HasTakenOff; chống kích hoạt lặp. |
| `Assets/Scripts/FlightCamera.cs` | Camera bám theo máy bay trong LateUpdate. |
| `Assets/Scripts/ResponsiveFlightUI.cs` | Điều chỉnh bố cục HUD và kích thước chữ theo hướng ngang/dọc. |
| `Assets/Scripts/NextSceneUI.cs` | Nút quay về đường băng từ Scene_1. |
| `Assets/Editor/Week8SceneBuilder.cs` | Công cụ dựng Scene bằng Unity Editor API; chỉ chạy khi gọi menu Week 8 → Build Practice Scenes. |

Các Scene đã được dựng và lưu đầy đủ, **không cần chạy builder để chơi**. Builder dựng lại hai Scene của bài và Build Scene List; thao tác này ghi đè các chỉnh sửa thủ công trong hai Scene đó. Builder từ chối chạy khi đang Play hoặc Scene hiện tại chưa lưu.

## Hierarchy Scene chính

```text
Scene_3_2
├── Directional Light
├── Main Camera [Camera, AudioListener, FlightCamera, GameManager]
├── Airport Environment
│   ├── Landscape / Runway Shoulder / Runway
│   ├── Centerline / Threshold Stripes / Edge Lights
│   └── Terminal / Terminal Windows / Control Tower / Tower Cabin
├── Airplane [tag Player, Rigidbody, BoxCollider, AirplaneController]
│   └── Model
│       ├── Fuselage / Nose / Cockpit Canopy
│       ├── Left Wing / Right Wing / Wing Tips
│       ├── Horizontal Tail / Vertical Tail
│       ├── Landing Struts / Wheels
│       └── Propeller → Blade / Hub
├── EnterWall [BoxCollider Is Trigger, EnterGame]
│   ├── Left Gate Post / Right Gate Post / Gate Header
│   └── Gate Label [TextMeshPro]
├── Canvas [Screen Space Overlay, CanvasScaler, GraphicRaycaster]
│   ├── Flight HUD → Header / Status Card
│   └── Panel [inactive khi bắt đầu]
│       └── Frame
│           ├── Accent / Eyebrow
│           ├── Title [TextMeshProUGUI]
│           ├── Message [TextMeshProUGUI]
│           ├── Divider
│           ├── ContinueButton [Button] → Text [TextMeshProUGUI]
│           ├── RestartButton [Button] → Text [TextMeshProUGUI]
│           └── Footer
└── EventSystem [InputSystemUIInputModule]
```

Rigidbody máy bay là kinematic, tắt gravity và bật interpolation. Collider trên root Player bao phủ máy bay; các primitive con chỉ để hiển thị. EnterWall ở phía trước, z=48, vùng Trigger rộng 19, cao 12, dày 1.5. Máy bay dừng ngay khi collider bắt đầu giao với vùng cổng, trước khi tâm máy bay đến z=48.

Canvas Scaler dùng Scale With Screen Size, reference 1440×900 ở màn hình ngang, 800×900 ở màn hình dọc và ưu tiên chiều rộng để Popup/HUD nằm trong màn hình hẹp. Bố cục dọc ẩn dòng mô tả bài ở header; camera lùi thêm để thấy máy bay. Button dùng Color Tint với màu hover, selected và pressed riêng. Popup chọn sẵn Continue để có thể dùng keyboard navigation qua EventSystem.

## Asset và nguồn

Máy bay và sân bay dùng Unity Primitives. Materials được tạo trong `Assets/Materials/Week8`. Font Liberation Sans SDF và TMP resources được nhập từ **TMP Essential Resources** đi kèm package uGUI của Unity; giấy phép nằm trong `Assets/TextMesh Pro/Fonts`. Không tải hoặc cài thêm package, không dùng asset trả phí.

## Kiểm thử

Xem `TEST_REPORT.md` để biết các phép kiểm tra thực sự đã chạy trong Unity Editor qua MCP, kết quả và giới hạn kiểm chứng. Ảnh Game View được lưu ở `Captures/`.

Trước khi nộp, nên trực tiếp thử bàn phím/chuột trên Game View, kiểm tra độ dễ đọc tại độ phân giải mong muốn và kiểm tra Build Profiles → Scene List. Phiên làm việc này không tạo standalone build và không kiểm thử trên thiết bị Android/iOS.

`.gitignore` bỏ qua Library, Temp, Obj, Logs, UserSettings và cache IDE; vẫn giữ Assets, Packages, ProjectSettings và các file .meta. Không thực hiện Git commit/push hay tạo repository.
