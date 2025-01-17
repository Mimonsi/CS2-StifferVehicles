using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace StifferVehicles
{
    [FileLocation(nameof(StifferVehicles))]
    public class Setting : ModSetting
    {
        public static Setting Instance;
        public Setting(IMod mod) : base(mod)
        {

        }


        private float _stiffnessModifier = 3f;
        [SettingsUISlider(min = 0.00f, max = 10f, step = 0.25f, unit = Unit.kFloatTwoFractions, scalarMultiplier = 1f)]
        public float StiffnessModifier
        {
            get => _stiffnessModifier;
            set
            {
                _stiffnessModifier = value;
                if (StiffnessSystem.Instance != null)
                    StiffnessSystem.Instance.UpdateEntities();
            }
        }

        private float _dampingModifier = 2f;
        [SettingsUISlider(min = 0.00f, max = 10f, step = 0.25f, unit = Unit.kFloatTwoFractions, scalarMultiplier = 1f)]
        public float DampingModifier
        {
            get => _dampingModifier;
            set
            {
                _dampingModifier = value;
                if (StiffnessSystem.Instance != null)
                    StiffnessSystem.Instance.UpdateEntities();
            }
        }

        [SettingsUIButton]
        public bool ResetToDefault
        {
            set
            {
                SetDefaults();
                if (StiffnessSystem.Instance != null)
                    StiffnessSystem.Instance.UpdateEntities();
            }
        }


        public override void SetDefaults()
        {
            StiffnessModifier = 3f;
            DampingModifier = 1f;
        }
    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Stiffer Vehicles" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessModifier)), "Stiffness Modifier" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessModifier)),
                    $"The Max Position modifier for all vehicles. Vanilla is 1.0, Default for Mod is 3.0 Higher values makes vehicles lean less, but it might look harsher."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingModifier)), "Damping Modifier" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingModifier)),
                    $"The Damping modifier for all vehicles. Vanilla is 1.0, Default for Mod is 2.0 Higher values makes vehicles bounce more. Low values are stiffer."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToDefault)), "Reset to Default" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToDefault)),
                    "Reset the modifiers to the default values."
                },
            };
        }

        public void Unload()
        {
        }
    }
}
