using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Tomino;

public class SettingsView : MonoBehaviour
{
    public Text titleText;
    public Toggle musicToggle;
    public Toggle screenButtonsToggle;
    public Text themeText;
    public Toggle neonThemeToggle;
    public Toggle blueThemeToggle;
    public Button closeButton;
    public AudioPlayer audioPlayer;

    private UnityAction onCloseCallback;

    internal void Awake()
    {
        musicToggle.GetComponentInChildren<Text>().text = Constant.Text.Music;
        musicToggle.onValueChanged.AddListener((enabled) =>
        {
            Settings.MusicEnabled = enabled;
            PlayToggleAudioClip(enabled);
        });

        screenButtonsToggle.GetComponentInChildren<Text>().text = Constant.Text.ScreenButtons;
        screenButtonsToggle.onValueChanged.AddListener((enabled) =>
        {
            Settings.ScreenButonsEnabled = enabled;
            PlayToggleAudioClip(enabled);
        });

        neonThemeToggle.GetComponentInChildren<Text>().text = Constant.Text.ThemeNeon;
        neonThemeToggle.onValueChanged.AddListener((enabled) =>
        {
            if (enabled)
            {
                Settings.Theme = Theme.Neon;
                PlayToggleAudioClip(enabled);
            }
        });

        blueThemeToggle.GetComponentInChildren<Text>().text = Constant.Text.ThemeBlue;
        blueThemeToggle.onValueChanged.AddListener((enabled) =>
        {
            if (enabled)
            {
                Settings.Theme = Theme.Blue;
                PlayToggleAudioClip(enabled);
            }
        });

        closeButton.GetComponentInChildren<Text>().text = Constant.Text.Close;
        closeButton.onClick.AddListener(() =>
        {
            Hide();
            onCloseCallback.Invoke();
        });

        closeButton.gameObject.GetComponent<PointerHandler>().onPointerDown.AddListener(() =>
        {
            audioPlayer.PlayResumeClip();
        });
    }

    public void Show(UnityAction onCloseCallback)
    {
        this.onCloseCallback = onCloseCallback;

        titleText.text = Constant.Text.Settings;
        themeText.text = Constant.Text.ThemeLabel;
        musicToggle.isOn = Settings.MusicEnabled;
        screenButtonsToggle.isOn = Settings.ScreenButonsEnabled;
        neonThemeToggle.isOn = Settings.Theme == Theme.Neon;
        blueThemeToggle.isOn = Settings.Theme == Theme.Blue;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void PlayToggleAudioClip(bool enabled)
    {
        if (enabled)
        {
            audioPlayer.PlayToggleOnClip();
        }
        else
        {
            audioPlayer.PlayToggleOffClip();
        }
    }
}
