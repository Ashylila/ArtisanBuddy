using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace ArtisanBuddy.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;

    // We give this window a constant ID using ###
    // This allows for labels being dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow(Plugin plugin) : base("A Wonderful Configuration Window###With a constant ID")
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(500, 150);
        SizeCondition = ImGuiCond.Always;

        Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
        if (Configuration.IsConfigWindowMovable)
        {
            Flags &= ~ImGuiWindowFlags.NoMove;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoMove;
        }
    }

    public override void Draw()
    {
        var configValue = Configuration.ListId;
        ImGui.Text("Artisan List ID:");
        if (ImGui.InputInt("##ArtisanListID", ref configValue, 100))
        {
            Configuration.ListId = configValue;
            Configuration.Save();
        }

        var shouldCraft = Configuration.ShouldCraftOnAutoGatherChanged;
        if (ImGui.Checkbox("Should craft selected list id on disabling Autogather", ref shouldCraft))
        {
            Configuration.ShouldCraftOnAutoGatherChanged = shouldCraft;
            Configuration.Save();
        }
    }
}
