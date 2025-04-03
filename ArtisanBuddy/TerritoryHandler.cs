using System;
using System.Linq;
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
    private void OnTerritoryChanged(ushort newTerritoryId)
    {
        var territory = Svc.Data.GetExcelSheet<TerritoryType>().FirstOrDefault(x => x.RowId == newTerritoryId);
        if(ShouldStartCrafting(territory))
        {
            StartCrafting();
        }
    }
    private bool ShouldStartCrafting(TerritoryType territory)
    {
        //if (!GatherbuddyReborn_IPCSubscriber.IsEnabled) return false;
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
