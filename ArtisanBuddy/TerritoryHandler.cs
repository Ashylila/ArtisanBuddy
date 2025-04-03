using System;
using System.Linq;
using System.Threading.Tasks;
using Lumina.Excel.Sheets;
using ArtisanBuddy;
using ArtisanBuddy.EzIpc;
using ArtisanBuddy.Utility;

namespace ArtisanBuddy;

public class TerritoryHandler
{
    private Configuration configuration;
    public TerritoryHandler(Plugin plugin)
    {
        this.configuration = plugin.Configuration;
        Svc.ClientState.TerritoryChanged += OnTerritoryChanged;
    }
    private async void OnTerritoryChanged(ushort newTerritoryId)
    {
        var territory = Svc.Data.GetExcelSheet<TerritoryType>().FirstOrDefault(x => x.RowId == newTerritoryId);
        try
        {
            if (await ShouldStartCrafting(territory))
            {
                StartCrafting();
            }
        }
        catch (Exception e)
        {
            Svc.Log.Error(e.Message, "Error in OnTerritoryChanged");
        }
    }
        
    
    private async Task<bool> ShouldStartCrafting(TerritoryType territory)
    {
        //if (!GatherbuddyReborn_IPCSubscriber.IsEnabled) return false;
        await Task.Delay(2000);
        if (configuration.ShouldCraftOnEnter && territory.PlaceName.Value.Name.ExtractText().Contains("private", StringComparison.OrdinalIgnoreCase) && !GatherbuddyReborn_IPCSubscriber.IsAutoGatherEnabled() && !GatherbuddyReborn_IPCSubscriber.IsAutoGatherWaiting())
        {
            return true;
        }
        return false;
    }
    private void StartCrafting()
    {
            Chat.Instance.ExecuteCommand($"/artisan lists {configuration.ListId} start");
            Svc.Log.Debug("Artisan.Invoke");
    }
}
