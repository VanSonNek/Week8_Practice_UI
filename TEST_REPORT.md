# Kết quả kiểm thử Unity MCP — 18/10/2026

## Môi trường và cách kiểm thử

- Unity MCP xác nhận project `Week8_Practice_UI`, đường dẫn `/home/hovanson-vku/HocTap/DiDong/Project/Week8_Practice_UI`, Unity `6000.5.10f1`.
- Kiểm thử thực sự trong Editor Play Mode. Sự kiện bàn phím được đưa vào Input System bằng virtual Keyboard; sự kiện chuột down/up được đưa vào virtual Mouse và xử lý bởi InputSystemUIInputModule/EventSystem.
- Không gọi trực tiếp `Button.onClick.Invoke()` để thay thế các phép kiểm tra click. Không gọi `ShowPopup()` trực tiếp để thay thế kiểm tra đường bay/Trigger.
- Các virtual input device đã được gỡ sau kiểm thử. Game View đã được trả về Free Aspect. Editor cuối cùng ở Edit Mode, mở `Scene_3_2`, Scene sạch và đã lưu.

## Kết quả

| Kiểm tra | Kết quả và bằng chứng |
| --- | --- |
| Compile | PASS. `EditorUtility.scriptCompilationFailed == false`; Editor không còn compiling/domain reload pending ở kiểm tra cuối. |
| Popup lúc bắt đầu | PASS. `PopupVisible == false`, `HasTakenOff == false`, vị trí `(0, 1.25, 0)`. Panel cũng được lưu inactive trong Scene. |
| W trước Space | PASS. Gửi W qua Input System; máy bay vẫn ở z=0, chưa cất cánh. |
| Space | PASS. Gửi Space; `HasTakenOff == true`, máy bay lên `(0, 4, 0)`, Popup vẫn ẩn. |
| W sau cất cánh | PASS. Máy bay di chuyển bằng Rigidbody trong FixedUpdate tới `(0, 4, 44.22)`. |
| Trigger sau cất cánh | PASS. Collider máy bay giao cổng ở z=48; `EnterGame.HasTriggered == true`, `PopupVisible == true`, `ControlsLocked == true`. Tâm máy bay chưa cần đến z=48 vì collider có chiều dài. |
| Player chưa cất cánh trong Trigger | PASS. Đưa Rigidbody của Player chưa cất cánh vào vùng cổng; Popup vẫn ẩn, HasTriggered vẫn false. |
| Lọc tag | PASS. Cho máy bay đã cất cánh mang tag Untagged bay xuyên qua cổng; Popup vẫn ẩn. Khôi phục Player, đưa về trước cổng rồi bay lại; Popup xuất hiện. Mọi thay đổi thử nghiệm chỉ diễn ra trong Play Mode. |
| Chống lặp | Cờ HasTriggered được xác nhận true khi chạm cổng, máy bay bị khóa, Popup giữ trạng thái mở. Code bỏ qua OnTriggerEnter tiếp theo nếu HasTriggered đã true; chưa kiểm thử riêng nhiều lần rời/vào Trigger trong cùng lượt. |
| Nội dung và font | PASS. Đọc đúng `Welcome to Unity!`, `Do you want to continue?`, `Continue`, `Restart`; mọi TMP_Text trong cả hai Scene đều có font. Ảnh Game View không có ô vuông thay ký tự. |
| Click Restart | PASS. Raycast tại tâm nút trả về RestartButton. Sự kiện Mouse down/up tải lại Scene_3_2; trạng thái bay, vị trí và Popup trở về ban đầu. |
| Click Continue | PASS. Sự kiện Mouse down/up mở Scene_1; hiện `Welcome to the Next Scene!`, Time.timeScale=1. Kiểm tra lại click Continue sau khi bổ sung bố cục dọc cũng thành công. |
| Back to Runway | PASS. Sự kiện chuột tại BackButton đưa về Scene_3_2. |
| EventSystem | PASS. InputSystemUIInputModule có actionsAsset, raycast và click hoạt động trong Play Mode. |
| Bố cục | PASS tại Full HD 1920×1080 và portrait 281×500 trong Game View. Popup portrait nằm trong khoảng x=21.08..259.93, y=170.97..329.03. Arrival Card và Accent cuối cùng cùng rộng 720 UI units trên reference width 800. |
| Build Scene List | PASS. Scene_3_2 enabled index 0; Scene_1 enabled index 1. SceneManager.LoadScene hoạt động với tên Scene thực tế. |
| Missing Script / Reference | PASS. Quét mọi GameObject, MonoBehaviour và serialized object reference của hai Scene: không có missing script/reference; các trường tham chiếu bắt buộc của script bài đều được gán. |

## Quét Scene đã lưu

| Scene | GameObject | Missing Script | Missing Reference | TMP thiếu font | Tham chiếu bắt buộc chưa gán | Chưa lưu |
| --- | ---: | ---: | ---: | ---: | ---: | --- |
| Scene_3_2 | 111 | 0 | 0 | 0 | 0 | Không |
| Scene_1 | 12 | 0 | 0 | 0 | 0 | Không |

## Console cuối cùng

- **0 Error**.
- **1 Warning của plugin MCP for Unity**: `[WebSocket] Unexpected receive error: WebSocket is not initialised` tại `Editor/Helpers/McpLog.cs:45` trong package MCP. Cảnh báo này xuất hiện quanh các lần domain reload; kết nối vẫn hoạt động và các phép kiểm tra tiếp tục thành công.
- Không có Error/Warning từ các script gameplay/UI của bài trong lần đọc Console cuối. Không gọi Clear Console để che thông báo.
- Trong quá trình phát triển có lỗi biên dịch tạm thời khi Editor cập nhật script mới; đã khắc phục, biên dịch lại và kiểm tra trạng thái compile thành công trước bàn giao.

## Ảnh

- `Captures/01-ready.png`: máy bay, sân bay và HUD ban đầu ở màn hình ngang.
- `Captures/02-checkpoint.png`: Popup thực sự mở bởi Trigger trong Play Mode.
- `Captures/03-next-scene.png`: Scene_1 sau click Continue.
- `Captures/05-portrait-final.png`: Popup với bố cục dọc cuối cùng.
- `Captures/06-portrait-next.png`: Scene_1 với bố cục dọc cuối cùng.

## Giới hạn và kiểm tra thủ công

- Chưa kiểm thử bàn phím/chuột vật lý của người dùng; các kiểm tra tự động dùng Input System và EventSystem với thiết bị mô phỏng. Hãy focus Game View, nhấn Space, giữ W và click hai nút để kiểm tra cảm giác điều khiển.
- Màu hover/pressed được cấu hình bằng Button Color Tint; chưa có ảnh so sánh riêng từng trạng thái hover/pressed.
- Chưa kiểm thử riêng keyboard navigation, nhiều lần rời/vào Trigger trong cùng lượt, standalone build hoặc thiết bị Android/iOS.
- Kiểm tra độ dễ đọc và bố cục trên màn hình/độ phân giải dự định sử dụng. Không khẳng định đã kiểm thử mọi tỉ lệ màn hình.
- Không cài thêm package, không thay đổi project khác, không Git commit/push và không tạo repository.
