using System;
using System.Threading.Tasks;
using ECommons.EzIpcManager;
using ECommons.Reflection;

namespace ArtisanBuddy.EzIpc;
#nullable disable
#pragma warning disable CS0649
public static class GatherbuddyReborn_IPCSubscriber
{
    public static event Action<bool> OnAutoGatherStatusChanged;
    private static readonly EzIPCDisposalToken[] _disposalTokens;
    static GatherbuddyReborn_IPCSubscriber()
    {
        _disposalTokens = EzIPC.Init(typeof(GatherbuddyReborn_IPCSubscriber),"GatherBuddyReborn");
    }


    [EzIPC]
    internal static readonly Func<bool> IsAutoGatherEnabled;
    [EzIPC]
    internal static readonly Action<bool> SetAutoGatherEnabled;
    [EzIPC]
    internal static readonly Func<bool> IsAutoGatherWaiting;

    [EzIPCEvent]
    public static void AutoGatherEnabledChanged(bool enabled)
    {
        OnAutoGatherStatusChanged.Invoke(enabled);
    }

    internal static bool IsEnabled => IPCSubscriber_Common.IsReady("GatherbuddyReborn");

    internal static void Dispose()
    {
        IPCSubscriber_Common.DisposeAll(_disposalTokens);
    }
}
internal class IPCSubscriber_Common
{
    internal static bool IsReady(string pluginName)
    {
        return DalamudReflector.TryGetDalamudPlugin(pluginName, out _, false, true);
    }

    internal static Version Version(string pluginName)
    {
        return DalamudReflector.TryGetDalamudPlugin(pluginName, out var dalamudPlugin, false, true)
                   ? dalamudPlugin.GetType().Assembly.GetName().Version
                   : new Version(0, 0, 0, 0);
    }

    internal static void DisposeAll(EzIPCDisposalToken[] disposalTokens)
    {
        foreach (var disposalToken in disposalTokens)
            try
            {
                disposalToken.Dispose();
            }
            catch (Exception ex)
            {
                Svc.Log.Debug($"Error while unregistering IPC: {ex}");
            }
    }
}

