using System;
using ArtisanBuddy.EzIpc;
using ArtisanBuddy.Utility;
using ArtisanBuddy.Windows;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Microsoft.Extensions.DependencyInjection;
using ECommons.Automation.NeoTaskManager;

namespace ArtisanBuddy.Services;

public static class ServiceWrapper
{
    public static IServiceProvider ServiceProvider;

    public static void Init(IDalamudPluginInterface pi, Plugin plugin)
    {
        Svc.Init(pi);
        
        var services = new ServiceCollection();

        services.AddSingleton(Svc.PluginInterface);
        services.AddSingleton(Svc.ClientState);
        services.AddSingleton(Svc.Condition);
        services.AddSingleton(Svc.Data);
        services.AddSingleton(Svc.Framework);
        services.AddSingleton(Svc.GameGui);
        services.AddSingleton(Svc.Objects);
        services.AddSingleton(Svc.Chat);
        services.AddSingleton(Svc.Commands);
        services.AddSingleton(Svc.FlyText);
        services.AddSingleton(Svc.PfGui);
        services.AddSingleton(Svc.Gauges);
        services.AddSingleton(Svc.KeyState);
        services.AddSingleton(Svc.Buddies);
        services.AddSingleton(Svc.Fates);
        services.AddSingleton(Svc.Texture);
        services.AddSingleton(Svc.Log);
        
        services.AddSingleton<Chat>();
        
        services.AddSingleton(plugin);
        services.AddSingleton(plugin.Configuration);
        
        services.AddSingleton<ConfigWindow>();
        services.AddSingleton<MainWindow>();
        
        services.AddSingleton<GatherbuddyReborn_IPCSubscriber>();
        services.AddSingleton<TaskManager>();
        services.AddSingleton<CraftingHandler>();
        
        ServiceProvider = services.BuildServiceProvider();
        
        
    }
    
    public static T Get<T>() where T : notnull  => ServiceProvider.GetRequiredService<T>();
    
}
