using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;

namespace ArtisanBuddy.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly Plugin _plugin;
    private readonly IDataManager _dataManager;
    private readonly IClientState _clientState; 
    
    public MainWindow(Plugin plugin,
                      IDataManager dataManager,
                      IClientState clientState)
        : base("ArtisanBuddy##ArtisanMain", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        _dataManager = dataManager;
        _clientState = clientState;
        
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
        
        _plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        

        if (ImGui.Button("Show Settings"))
        {
            _plugin.ToggleConfigUI();
        }
    }
}
