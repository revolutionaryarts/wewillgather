namespace Gather.Core.Configuration
{
    public interface IConfigurationProvider<T> where T : ISettings, new ()
    {
        T Settings { get; set; }
        void SaveSettings(T settings);
    }
}