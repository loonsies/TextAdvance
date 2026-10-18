using Dalamud.Game.ClientState.Keys;
using ECommons.Configuration;
using FFXIVClientStructs.FFXIV.Client.UI;
using TextAdvance.Gui;

namespace TextAdvance;

internal static unsafe class ModeHotkey
{
    internal static readonly string[] Modes = ["QA", "QC", "RP", "TS", "TSC", "RH", "RF", "CS", "CC", "IN"];
    private static VirtualKey LastKey;
    private static bool WasHeld;
    private static bool IsHeld;

    internal static void Tick()
    {
        var held = IsKeyPressed((int)C.ModeToggleButton);
        if (!held || LastKey != C.ModeToggleButton || !Svc.ClientState.IsLoggedIn || CSFramework.Instance()->WindowInactive
            || Keybind.IsCapturing || ImGui.GetIO().WantTextInput || RaptureAtkModule.Instance()->IsTextInputActive())
        {
            IsHeld = false;
        }
        else if (!WasHeld) IsHeld = true;
        WasHeld = held;
        LastKey = C.ModeToggleButton;
    }

    internal static bool Apply(string mode, bool enabled) => enabled != (IsHeld && C.ModeToggleModes.Contains(mode));

    internal static void Toggle(IEnumerable<string> modes)
    {
        var config = C.MainConfig;
        if (!(C.GlobalOverridesLocal && P.Enabled) && C.TerritoryConditions.TryGetValue(Svc.ClientState.TerritoryType, out var local)) config = local;
        foreach (var mode in modes)
        {
            if (mode.EqualsIgnoreCase("QA")) config.EnableQuestAccept = !config.EnableQuestAccept;
            else if (mode.EqualsIgnoreCase("QC")) config.EnableQuestComplete = !config.EnableQuestComplete;
            else if (mode.EqualsIgnoreCase("RP")) config.EnableRewardPick = !config.EnableRewardPick;
            else if (mode.EqualsIgnoreCase("TS")) config.EnableTalkSkip = !config.EnableTalkSkip;
            else if (mode.EqualsIgnoreCase("TSC")) config.EnableTalkSkipCutscene = !config.EnableTalkSkipCutscene;
            else if (mode.EqualsIgnoreCase("RH")) config.EnableRequestHandin = !config.EnableRequestHandin;
            else if (mode.EqualsIgnoreCase("RF")) config.EnableRequestFill = !config.EnableRequestFill;
            else if (mode.EqualsIgnoreCase("CS")) config.EnableCutsceneEsc = !config.EnableCutsceneEsc;
            else if (mode.EqualsIgnoreCase("CC")) config.EnableCutsceneSkipConfirm = !config.EnableCutsceneSkipConfirm;
            else if (mode.EqualsIgnoreCase("IN")) config.EnableAutoInteract = !config.EnableAutoInteract;
        }
        EzConfig.Save();
    }
}
