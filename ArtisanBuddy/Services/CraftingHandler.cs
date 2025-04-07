﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Lumina.Excel.Sheets;
using ArtisanBuddy;
using ArtisanBuddy.EzIpc;
using ArtisanBuddy.Services;
using ArtisanBuddy.Utility;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using ECommons;
using ECommons.Automation.NeoTaskManager;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.UI;
using CollectablesShop = FFXIVClientStructs.FFXIV.Client.Game.UI.CollectablesShop;

namespace ArtisanBuddy;

public class CraftingHandler : IDisposable
{

    private readonly TaskManager _taskManager;
    private readonly Configuration _configuration;
    private readonly GatherBuddyService _gatherbuddyService;
    private readonly IChatGui _chat;
    private readonly IPluginLog _log;
    private readonly IDataManager _data;
    private readonly Chat _chatSender;
    private bool isOn = false;
    
    
    
    public CraftingHandler(Configuration configuration,
                           IChatGui chat,
                           IPluginLog log,
                           IDataManager data,
                           TaskManager taskManager,
                           Chat chatSender,
                           GatherBuddyService gatherBuddyService)
    {
        _taskManager = taskManager;
        _configuration = configuration;
        _chat = chat;
        _log = log;
        _data = data;
        _chatSender = chatSender;
        _gatherbuddyService = gatherBuddyService;
    }

    public void Init()
    {
        isOn = _gatherbuddyService.IsAutoGatherEnabled;
        _gatherbuddyService.OnAutoGatherStatusChanged += OnAutoGatherStatusChanged;
    }
    public void Dispose()
    {
        _gatherbuddyService.OnAutoGatherStatusChanged -= OnAutoGatherStatusChanged;
        _taskManager.Dispose();
    }
    private void OnAutoGatherStatusChanged(bool isEnabled)
    {
        if (isEnabled) return;
        if (_configuration.ShouldCraftOnAutoGatherChanged)
        {
            ShouldStartCrafting();
        }
        
    }

    public void ShouldStartCrafting()
    {
        if(_configuration.ShouldCraftOnAutoGatherChanged && !GatherbuddyReborn_IPCSubscriber.IsAutoGatherEnabled())
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
                _log.Debug("Player is not available for crafting.");
            }
        

    }

    public void Invoke()
    {
        _chatSender.ExecuteCommand($"/artisan lists {_configuration.ListId} start");
        _log.Debug("Artisan.Invoke");
    }
    
    private void TeleportToSafeArea()
    {
        var nearestAetheryte = _data.GetExcelSheet<Aetheryte>().FirstOrDefault(a => a.Territory.RowId == Player.Territory).PlaceName.Value.Name.ExtractText();
            if (TeleportHelper.TryFindAetheryteByName(nearestAetheryte, out var info, out var aetherName))
            {
                TeleportHelper.Teleport(info.AetheryteId, info.SubIndex);
                _log.Debug($"Teleporting to {aetherName}...");
            }
            else
            {
                _log.Error("Failed to find teleport location.");
            }
    }

}
