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

        [SettingsUIHidden]
        public bool HiddenSetting { get; set; }

        [SettingsUISlider(min=0.25f, max=10f, step=0.25f, unit=Unit.kFloatTwoFractions, scalarMultiplier = 1f)]
        public float SpringModifier { get; set; }= 1.75f;

        [SettingsUISlider(min=0.25f, max=10f, step=0.25f, unit=Unit.kFloatTwoFractions, scalarMultiplier = 1f)]
        public float DampingModifier { get; set; }= 1.75f;

        [SettingsUISlider(min=0.25f, max=10f, step=0.25f, unit=Unit.kFloatTwoFractions, scalarMultiplier = 1f)]
        public float MaxPosition { get; set; }= 1.75f;

        [SettingsUIButton]
        public bool ApplyModifier
        {
            set => StiffnessSystem.Instance.UpdateEntities();
        }

        [SettingsUIButton]
        public bool ResetToDefault
        {
            set
            {
                SetDefaults();
                StiffnessSystem.Instance.UpdateEntities();
            }

        }


        public override void SetDefaults()
        {
            HiddenSetting = true;
            SpringModifier = 1.75f;
            DampingModifier = 1.75f;
            MaxPosition = 1.75f;
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

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpringModifier)), "Spring Modifier" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.SpringModifier)),
                    $"The Spring modifier for all vehicles. Vanilla is 1.0, Default for Mod is 1.75 Higher values make vehicles stiffer, lower values make vehicles more bouncy and leaning into the curves."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingModifier)), "Damping Modifier" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingModifier)),
                    $"The damping modifier for all vehicles. Vanilla is 1.0, Default for Mod is 2.0 Higher values make vehicles stiffer, lower values make vehicles more bouncy and leaning into the curves."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.MaxPosition)), "Max Position Modifier" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.MaxPosition)),
                    $"The Max Position modifier for all vehicles. Vanilla is 1.0, Default for Mod is 1.75 Higher values make vehicles lean less, but it might look harsher."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ApplyModifier)), "Apply Modifier" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ApplyModifier)),
                    "Apply the modifiers to all vehicles."
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
