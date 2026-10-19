using UnityEngine;

/// <summary>
/// Insets a full-screen RectTransform by the device safe-area (notch / Dynamic
/// Island / home indicator) so HUD content stays inside the tappable region.
/// In WebGL (Apps in Toss WebView) Screen.safeArea is full-screen, so the insets
/// are read from the browser via BeeSafeArea.jslib.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    [SerializeField] private float marginTop;
    [SerializeField] private float marginBottom;
    [SerializeField] private float marginLeft;
    [SerializeField] private float marginRight;

    [Tooltip("Top inset (fraction of screen) used when the platform reports none, " +
             "so top-anchored HUD never hugs a notch.")]
    [SerializeField] private float minTopInset = 0.06f;

    private RectTransform _rt;
    private int _w;
    private int _h;

    private void Awake() { _rt = (RectTransform)transform; }

    private void OnEnable() { Apply(); }

    private void Update()
    {
        if (Screen.width != _w || Screen.height != _h) Apply();
    }

    public void Apply()
    {
        if (_rt == null) _rt = (RectTransform)transform;

        int sw = Screen.width, sh = Screen.height;
        if (sw <= 0 || sh <= 0) return;
        _w = sw;
        _h = sh;

        float left = 0f, bottom = 0f, right = 0f, top = 0f;
        Rect sa = Screen.safeArea;
        if (sa.width > 0f && sa.height > 0f)
        {
            left = sa.xMin / sw;
            bottom = sa.yMin / sh;
            right = 1f - sa.xMax / sw;
            top = 1f - sa.yMax / sh;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        left = Mathf.Max(left, __BeeSafeAreaInset(2));
        bottom = Mathf.Max(bottom, __BeeSafeAreaInset(1));
        right = Mathf.Max(right, __BeeSafeAreaInset(3));
        top = Mathf.Max(top, __BeeSafeAreaInset(0));
#endif

        if (top <= 0f) top = minTopInset;

        _rt.anchorMin = new Vector2(left + marginLeft, bottom + marginBottom);
        _rt.anchorMax = new Vector2(1f - right - marginRight, 1f - top - marginTop);
        _rt.offsetMin = Vector2.zero;
        _rt.offsetMax = Vector2.zero;
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern float __BeeSafeAreaInset(int side);
#endif
}
