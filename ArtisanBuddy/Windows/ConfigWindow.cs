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
    public ConfigWindow(Configuration config) : base("Artisan Config###ArtisanConfig")
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
        ImGui.TextUnformatted("This plugin is depricated. Please use TheCollector instead.");
    }
}
