using Dalamud.Game.ClientState.Keys;

namespace TextAdvance.Gui;

internal static class Keybind
{
    private static readonly VirtualKey[] Keys = Enum.GetValues<VirtualKey>().Where(x => x >= VirtualKey.BACK && x != VirtualKey.SHIFT && x != VirtualKey.CONTROL && x != VirtualKey.MENU).ToArray();
    private static HashSet<VirtualKey> HeldKeys = [];
    private static string ActiveId;
    private static long Deadline;
    internal static bool IsCapturing => ActiveId != null && Environment.TickCount64 < Deadline;

    internal static void Draw(string id, ref VirtualKey key)
    {
        if (Environment.TickCount64 >= Deadline || !ImGui.IsWindowFocused(ImGuiFocusedFlags.RootAndChildWindows)) ActiveId = null;
        ImGui.PushID(id);
        var label = ActiveId == id ? $"Press any key... ({(Deadline - Environment.TickCount64 + 999) / 1000}s)" : key == VirtualKey.NO_KEY ? "Not bound" : key.GetFancyName();
        if (ImGui.Button($"{label}###Bind", new Vector2(200f, 0)))
        {
            ActiveId = ActiveId == id ? null : id;
            Deadline = Environment.TickCount64 + 5000;
            HeldKeys = Keys.Where(x => IsKeyPressed((int)x)).ToHashSet();
        }
        ImGui.SameLine();
        if (ImGui.SmallButton("Clear"))
        {
            key = VirtualKey.NO_KEY;
            ActiveId = null;
        }
        if (ActiveId == id)
        {
            var pressed = Keys.Where(x => IsKeyPressed((int)x)).ToHashSet();
            foreach (var candidate in pressed)
            {
                if (HeldKeys.Contains(candidate)) continue;
                key = candidate;
                ActiveId = null;
                break;
            }
            HeldKeys = pressed;
        }
        ImGui.PopID();
    }
}