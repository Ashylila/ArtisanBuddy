using System;
using System.Threading.Tasks;
using ECommons.EzIpcManager;
using ECommons.Reflection;

namespace ArtisanBuddy.EzIpc;
#nullable disable
#pragma warning disable CS0649
public class GatherbuddyReborn_IPCSubscriber : IDisposable
{
    [EzIPC] internal Func<bool>? IsAutoGatherEnabled;
    [EzIPC] internal Action<bool>? SetAutoGatherEnabled;
    [EzIPC] internal Func<bool>? IsAutoGatherWaiting;

    public event Action<bool>? OnAutoGatherStatusChanged;

    private readonly EzIPCDisposalToken[] _disposalTokens;

    public GatherbuddyReborn_IPCSubscriber()
    {
        _disposalTokens = EzIPC.Init(this, "GatherBuddyReborn", SafeWrapper.IPCException);
    }

    public bool GetIsAutoGatherEnabled() => IsAutoGatherEnabled?.Invoke() ?? false;
    public void SetAutoGatherEnabledStatus(bool enabled) => SetAutoGatherEnabled?.Invoke(enabled);
    public bool GetIsAutoGatherWaiting() => IsAutoGatherWaiting?.Invoke() ?? false;

    [EzIPCEvent]
    public void AutoGatherEnabledChanged(bool enabled)
    {
        OnAutoGatherStatusChanged?.Invoke(enabled);
    }

    public void Dispose()
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

