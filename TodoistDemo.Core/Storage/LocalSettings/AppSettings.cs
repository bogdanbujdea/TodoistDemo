using System;
using Windows.Storage;

namespace TodoistDemo.Core.Storage.LocalSettings
{
    public class AppSettings : IAppSettings
    {
        public T GetData<T>(SettingsKey key)
        {
            try
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey(key.ToString()))
                    return (T)ApplicationData.Current.LocalSettings.Values[key.ToString()];
                return default(T);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public void SetData(SettingsKey key, object value)
        {
            ApplicationData.Current.LocalSettings.Values[key.ToString()] = value;
        }
    }
}
