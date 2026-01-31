using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

public class TapToShoot : MonoBehaviour
{
    [SerializeField] private Bow bow;
    [SerializeField] private float topBlockedHeightPx = 160f;
    [SerializeField] private bool blockWhenOverUI = true;
    [SerializeField] private bool requireBottomHalf = true;
    [SerializeField] private Camera cam;

    private void Update()
    {
        if (bow == null) return;
        if (cam == null) cam = Camera.main;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        var touch = Touchscreen.current?.primaryTouch;
        if (touch != null && touch.press.wasPressedThisFrame)
        {
            Vector2 pos = touch.position.ReadValue();
            int id = touch.touchId.ReadValue();
            if (IsInBottomHalf(pos)) bow.Shoot();
            return;
        }

        var mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            Vector2 pos = mouse.position.ReadValue();
            if (IsInBottomHalf(pos)) bow.Shoot();
        }
#else
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase != TouchPhase.Began) return;
            if (!IsInBottomHalf(t.position)) return;
            bow.Shoot();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Input.mousePosition;
            if (!IsInBottomHalf(pos)) return;
            bow.Shoot();
        }
#endif
    }

    private bool IsInBottomHalf(Vector2 screenPos)
    {
        if (!requireBottomHalf) return true;
        if (cam == null) return true;

        Vector3 world = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
        return world.y <= cam.transform.position.y;
    }
}
