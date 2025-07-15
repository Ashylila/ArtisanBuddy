using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace ArtisanBuddy;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool IsConfigWindowMovable { get; set; } = true;
    public bool SomePropertyToBeSavedAndWithADefault { get; set; } = true;
    public int ListId { get; set; } = 0;
    public bool ShouldCraftOnAutoGatherChanged { get; set; } = false;
    
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
