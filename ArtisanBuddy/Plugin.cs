using System;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ECommons;
using ArtisanBuddy;
using ArtisanBuddy.EzIpc;
using ArtisanBuddy.Windows;
using ArtisanBuddy.Services;

namespace ArtisanBuddy;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;

    private const string CommandName = "/artisanbuddy";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("ArtisanBuddy");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    public Plugin()
    {
        ECommonsMain.Init(PluginInterface, this, Module.DalamudReflector);
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        ServiceWrapper.Init(PluginInterface, this);
        
        ConfigWindow = ServiceWrapper.Get<ConfigWindow>();
        MainWindow = ServiceWrapper.Get<MainWindow>();

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);
        

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Artisan Buddy command. Use /artisanbuddy to toggle the config window."
        });

        PluginInterface.UiBuilder.Draw += DrawUI;
        
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;
        
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
        
        ServiceWrapper.Get<CraftingHandler>().Init();
    }

    public void Dispose()
    {
        if (ServiceWrapper.ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
        ECommonsMain.Dispose();
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        switch (args)
        {
            case "toggle":
                Configuration.ShouldCraftOnAutoGatherChanged = !Configuration.ShouldCraftOnAutoGatherChanged;
                break;
            default:
                ToggleConfigUI();
                break;
        }
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();
}
