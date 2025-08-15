using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;


namespace ArtisanBuddy.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration _configuration;

    // We give this window a constant ID using ###
    // This allows for labels being dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow(Configuration config) : base("A Wonderful Configuration Window###With a constant ID")
    {
        _configuration = config;
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(500, 150);
        SizeCondition = ImGuiCond.Always;
        
    }

    public void Dispose() { }

    public override void PreDraw()
    {
        if (_configuration.IsConfigWindowMovable)
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
        var configValue = _configuration.ListId;
        ImGui.Text("Artisan List ID:");
        if (ImGui.InputInt("##ArtisanListID", ref configValue, 100))
        {
            _configuration.ListId = configValue;
            _configuration.Save();
        }

        var shouldCraft = _configuration.ShouldCraftOnAutoGatherChanged;
        if (ImGui.Checkbox("Should craft selected list id on disabling Autogather", ref shouldCraft))
        {
            _configuration.ShouldCraftOnAutoGatherChanged = shouldCraft;
            _configuration.Save();
        }
    }
}
