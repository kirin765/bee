using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameLanguage { Korean = 0, English = 1 }

/// <summary>
/// Lightweight runtime localization for the bee game.
/// - Default language follows the device language (Korean devices -> Korean,
///   everything else -> English).
/// - The player can switch at any time with the small KR/EN button added to
///   the main canvas; the choice is remembered in PlayerPrefs.
/// </summary>
public static class Localization
{
    private const string PrefKey = "bee_language";
    private const string ToggleName = "LanguageToggle";

    // Korean source string -> English translation.
    private static readonly Dictionary<string, string> KoToEn = new Dictionary<string, string>
    {
        { "시작", "Start" },
        { "다시 하기", "Retry" },
        { "게임 오버", "Game Over" },
        { "비 슈터", "Bee Shooter" },
        { "빨라짐!", "Faster!" },
        { "화살 공격력: +20%", "Arrow damage: +20%" },
        { "화살 쿨다운 감소", "Reduce cooldown" },
        { "궁극기: +1 (최대 2)", "Ultimate: +1 (max 2)" },
        { "하트: +1 (최대 3)", "Heart: +1 (max 3)" },
        { "벌 감속: 벌 속도 감소", "Bee slow: reduce speed" },
        { "지연 샷: 화살 추가(0.1초 지연, 최대 5)", "Delayed shot: extra arrow (0.1s delay, max 5)" },
    };

    private static GameLanguage _current = GameLanguage.Korean;
    private static readonly Dictionary<TMP_Text, string> _originals = new Dictionary<TMP_Text, string>();

    public static event System.Action OnLanguageChanged;

    public static GameLanguage Current
    {
        get => _current;
        set
        {
            if (_current == value) return;
            _current = value;
            PlayerPrefs.SetInt(PrefKey, (int)_current);
            PlayerPrefs.Save();
            ApplyToScene();
            RefreshToggleLabel();
            OnLanguageChanged?.Invoke();
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (PlayerPrefs.HasKey(PrefKey))
            _current = (GameLanguage)PlayerPrefs.GetInt(PrefKey);
        else
            _current = Application.systemLanguage == SystemLanguage.Korean
                ? GameLanguage.Korean
                : GameLanguage.English;

        SceneManager.sceneLoaded += (scene, mode) =>
        {
            _originals.Clear();
            ApplyToScene();
            EnsureToggleButton();
        };

        ApplyToScene();
        EnsureToggleButton();
    }

    /// <summary>Translate a Korean source string for the current language.</summary>
    public static string T(string korean)
    {
        if (string.IsNullOrEmpty(korean)) return korean;
        if (_current == GameLanguage.Korean) return korean;
        return KoToEn.TryGetValue(korean, out string en) ? en : korean;
    }

    /// <summary>Apply the current language to every TMP text in the scene.</summary>
    public static void ApplyToScene()
    {
        var texts = Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include);
        foreach (TMP_Text text in texts)
        {
            if (text == null) continue;
            if (!_originals.TryGetValue(text, out string original))
            {
                // Cache the authored (Korean) string the first time we see it so
                // we can switch back and forth.
                original = text.text;
                _originals[text] = original;
            }
            text.text = T(original);
        }
    }

    private static GameObject _toggle;

    private static void EnsureToggleButton()
    {
        if (_toggle != null) return;

        Canvas canvas = FindOverlayCanvas();
        if (canvas == null) return;

        // Parent into a safe-area container so the toggle never sits under the
        // notch / home indicator (or the Toss close button) and stays tappable.
        Transform parent = EnsureSafeAreaParent(canvas);

        _toggle = new GameObject(ToggleName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        _toggle.transform.SetParent(parent, false);

        RectTransform rt = _toggle.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(0f, 0f);
        rt.pivot = new Vector2(0f, 0f);
        rt.anchoredPosition = new Vector2(16f, 16f);
        rt.sizeDelta = new Vector2(130f, 64f);

        Image bg = _toggle.GetComponent<Image>();
        bg.color = new Color(1f, 1f, 1f, 0.65f);

        Button button = _toggle.GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            Current = Current == GameLanguage.Korean ? GameLanguage.English : GameLanguage.Korean;
        });

        GameObject labelGo = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer));
        labelGo.transform.SetParent(_toggle.transform, false);
        RectTransform lrt = labelGo.GetComponent<RectTransform>();
        lrt.anchorMin = Vector2.zero;
        lrt.anchorMax = Vector2.one;
        lrt.offsetMin = Vector2.zero;
        lrt.offsetMax = Vector2.zero;

        TextMeshProUGUI label = labelGo.AddComponent<TextMeshProUGUI>();
        label.name = "Label";
        label.alignment = TextAlignmentOptions.Center;
        label.fontSize = 30f;
        label.color = Color.black;
        label.text = LabelText();
        label.raycastTarget = false;

        RefreshToggleLabel();
    }

    private static Canvas FindOverlayCanvas()
    {
        foreach (Canvas c in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Exclude))
            if (c.renderMode == RenderMode.ScreenSpaceOverlay) return c;
        return null;
    }

    private static Transform EnsureSafeAreaParent(Canvas canvas)
    {
        Transform existing = canvas.transform.Find(ToggleName + "SafeArea");
        if (existing != null) return existing;

        var go = new GameObject(ToggleName + "SafeArea", typeof(RectTransform), typeof(SafeAreaFitter));
        go.transform.SetParent(canvas.transform, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return go.transform;
    }

    private static string LabelText() =>
        _current == GameLanguage.Korean ? "KR" : "EN";

    private static void RefreshToggleLabel()
    {
        if (_toggle == null) return;
        TextMeshProUGUI label = _toggle.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null) label.text = LabelText();
    }
}
