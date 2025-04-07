using System;
using System.Numerics;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ImGuiNET;
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
        : base("My Amazing Window##With a hidden ID", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
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

        ImGui.TextUnformatted($"The random config bool is {_plugin.Configuration.SomePropertyToBeSavedAndWithADefault}");

        if (ImGui.Button("Show Settings"))
        {
            _plugin.ToggleConfigUI();
        }

        ImGui.Spacing();
        
        using (var child = ImRaii.Child("SomeChildWithAScrollbar", Vector2.Zero, true))
        {
            // Check if this child is drawing
            if (child.Success)
            {
                ImGuiHelpers.ScaledDummy(20.0f);
                

                var localPlayer = _clientState.LocalPlayer;
                if (localPlayer == null)
                {
                    ImGui.TextUnformatted("Our local player is currently not loaded.");
                    return;
                }

                if (!localPlayer.ClassJob.IsValid)
                {
                    ImGui.TextUnformatted("Our current job is currently not valid.");
                    return;
                }
                
                ImGui.TextUnformatted($"Our current job is ({localPlayer.ClassJob.RowId}) \"{localPlayer.ClassJob.Value.Abbreviation.ExtractText()}\"");
                
                var territoryId = _clientState.TerritoryType;
                if (_dataManager.GetExcelSheet<TerritoryType>().TryGetRow(territoryId, out var territoryRow))
                {
                    ImGui.TextUnformatted($"We are currently in ({territoryId}) \"{territoryRow.PlaceName.Value.Name.ExtractText()}\"");
                }
                else
                {
                    ImGui.TextUnformatted("Invalid territory.");
                }
            }
        }
    }
}
