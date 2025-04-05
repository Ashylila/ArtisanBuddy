using System;
using System.Linq;
using System.Threading.Tasks;
using Lumina.Excel.Sheets;
using ArtisanBuddy;
using ArtisanBuddy.EzIpc;
using ArtisanBuddy.Utility;
using Dalamud.Game.ClientState.Conditions;
using ECommons;
using ECommons.Automation.NeoTaskManager;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.UI;
using CollectablesShop = FFXIVClientStructs.FFXIV.Client.Game.UI.CollectablesShop;

namespace ArtisanBuddy;

public class CraftingHandler : IDisposable
{
    
    private TaskManager _taskManager;
    private Configuration configuration;
    public CraftingHandler(Plugin plugin)
    {
        _taskManager = new TaskManager();
        this.configuration = plugin.Configuration;
        GatherbuddyReborn_IPCSubscriber.OnAutoGatherStatusChanged += OnAutoGatherStatusChanged;
    }
    public void Dispose()
    {
        GatherbuddyReborn_IPCSubscriber.OnAutoGatherStatusChanged -= OnAutoGatherStatusChanged;
        _taskManager.Dispose();
    }
    private void OnAutoGatherStatusChanged(bool isEnabled)
    {
        if (isEnabled) return;
        if (configuration.ShouldCraftOnAutoGatherChanged)
        {
            ShouldStartCrafting();
        }
        
    }

    public void ShouldStartCrafting()
    {
        if(configuration.ShouldCraftOnAutoGatherChanged && !GatherbuddyReborn_IPCSubscriber.IsAutoGatherEnabled())
        {
             _taskManager.Enqueue(StartCrafting);
        }
    }
        
    
    
    private void StartCrafting()
    {

            if (Player.TerritoryIntendedUse == TerritoryIntendedUseEnum.Open_World &&
                Player.Available)
            {
                _taskManager.Enqueue(TeleportToSafeArea);
                _taskManager.EnqueueDelay(7000);
                _taskManager.Enqueue(()=>Variables.CanAct);
                _taskManager.Enqueue(Invoke);
            }
            else if(Player.Available)
            {
                _taskManager.Enqueue(Invoke);
            }else
            {
                Svc.Log.Debug("Player is not available for crafting.");
            }
        

    }

    public void Invoke()
    {
        Chat.Instance.ExecuteCommand($"/artisan lists {configuration.ListId} start");
        Svc.Log.Debug("Artisan.Invoke");
    }
    
    private void TeleportToSafeArea()
    {
        var nearestAetheryte = Svc.Data.GetExcelSheet<Aetheryte>().FirstOrDefault(a => a.Territory.RowId == Player.Territory).PlaceName.Value.Name.ExtractText();
            if (TeleportHelper.TryFindAetheryteByName(nearestAetheryte, out var info, out var aetherName))
            {
                TeleportHelper.Teleport(info.AetheryteId, info.SubIndex);
                Svc.Log.Debug($"Teleporting to {aetherName}...");
            }
            else
            {
                Svc.Log.Error("Failed to find teleport location.");
            }
    }

}
