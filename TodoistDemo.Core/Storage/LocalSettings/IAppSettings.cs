namespace TodoistDemo.Core.Storage.LocalSettings
{
    public interface IAppSettings
    {
        T GetData<T>(SettingsKey key);
        void SetData(SettingsKey key, object value);
    }
}