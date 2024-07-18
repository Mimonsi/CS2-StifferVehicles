using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Colossal.IO.AssetDatabase;

namespace StifferVehicles
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(StifferVehicles)}.{nameof(Mod)}")
            .SetShowsErrorsInUI(false);

        private Setting m_Setting;
        public static string path;

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                path = asset.path;

            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));

            AssetDatabase.global.LoadSettings(nameof(StifferVehicles), m_Setting, new Setting(this));
            Setting.Instance = m_Setting;

            updateSystem.UpdateAt<StiffnessSystem>(SystemUpdatePhase.MainLoop);
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
        }
    }
}