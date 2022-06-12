using UnityEngine;

namespace Tomino
{
    public enum Theme
    {
        Neon, Blue
    }

    public static class Settings
    {
        public delegate void SettingsDelegate();
        public static SettingsDelegate ChangedEvent = delegate { };

        private static readonly string musicEnabledKey = "tomino.settings.musicEnabled";
        private static readonly string screenButtonsEnabledKey = "tomino.settings.screenButtonsEnabled";
        private static readonly string themeKey = "tomino.settings.theme";

        public static bool MusicEnabled
        {
            get => PlayerPrefs.GetInt(musicEnabledKey, 1).BoolValue();

            set
            {
                PlayerPrefs.SetInt(musicEnabledKey, value.IntValue());
                PlayerPrefs.Save();
                ChangedEvent.Invoke();
            }
        }

        public static bool ScreenButonsEnabled
        {
            get => PlayerPrefs.GetInt(screenButtonsEnabledKey, 0).BoolValue();

            set
            {
                PlayerPrefs.SetInt(screenButtonsEnabledKey, value.IntValue());
                PlayerPrefs.Save();
                ChangedEvent.Invoke();
            }
        }

        public static Theme Theme
        {
            get => (Theme)PlayerPrefs.GetInt(themeKey, 0);

            set
            {
                PlayerPrefs.SetInt(themeKey, (int)value);
                PlayerPrefs.Save();
                ChangedEvent.Invoke();
            }
        }
    }
}
