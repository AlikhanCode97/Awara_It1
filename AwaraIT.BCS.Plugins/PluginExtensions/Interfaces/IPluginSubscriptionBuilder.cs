namespace AwaraIT.BCS.Plugins.PluginExtensions.Interfaces
{
    public interface IPluginSubscriptionBuilder
    {
        IPluginSubscribeToMessage ToMessage(string message);
    }
}
