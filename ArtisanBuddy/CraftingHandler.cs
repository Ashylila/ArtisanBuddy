using System;
using System.Linq;
using System.Threading.Tasks;
using Lumina.Excel.Sheets;
using ArtisanBuddy;
using ArtisanBuddy.EzIpc;
using ArtisanBuddy.Utility;
using Dalamud.Game.ClientState.Conditions;
using ECommons.ExcelServices;
using ECommons.GameHelpers;

namespace ArtisanBuddy;

public class CraftingHandler
{
    private Configuration configuration;
    public CraftingHandler(Plugin plugin)
    {
        this.configuration = plugin.Configuration;
        GatherbuddyReborn_IPCSubscriber.OnAutoGatherStatusChanged += OnAutoGatherStatusChanged;
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
             StartCrafting();
        }
    }
        
    
    
    private void StartCrafting()
    {

            if (Player.TerritoryIntendedUse == TerritoryIntendedUseEnum.Open_World &&
                Player.Available)
            {
                _ = TeleportToSafeArea();
            }
            else if(Player.Available)
            {
                Invoke();
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
    
    private async Task TeleportToSafeArea()
    {
        var nearestAetheryte = Svc.Data.GetExcelSheet<Aetheryte>().FirstOrDefault(a => a.Territory.RowId == Player.Territory).PlaceName.Value.Name.ExtractText();
            if (TeleportHelper.TryFindAetheryteByName(nearestAetheryte, out var info, out var aetherName))
            {
                TeleportHelper.Teleport(info.AetheryteId, info.SubIndex);
                Svc.Log.Debug($"Teleporting to {aetherName}...");

                // Wait until teleport is complete
                await WaitForTeleportCompletion();
                Invoke();
                Svc.Log.Debug("Teleport completed!");
            }
            else
            {
                Svc.Log.Error("Failed to find teleport location.");
            }
    }

    private async Task WaitForTeleportCompletion()
    {
        int timeoutMs = 20000;
        int elapsedTime = 0;
        int checkInterval = 500;
        var currentLocation = Svc.Framework.Run( ()=> Svc.ClientState.LocalPlayer.Position);
        while (elapsedTime < timeoutMs)
        {
            await Task.Delay(checkInterval);
            elapsedTime += checkInterval;
            Svc.Log.Verbose($"Waiting for teleport to complete.");
            if (Svc.Framework.Run(()=> Svc.ClientState.LocalPlayer.Position).Result != currentLocation.Result)
            {
                Svc.Log.Verbose("teleportation completed.");
                return;
            }

        }

        Svc.Log.Error("Teleportation timeout reached.");
    }
}
