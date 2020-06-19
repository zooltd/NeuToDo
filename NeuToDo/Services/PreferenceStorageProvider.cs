using System;
using Xamarin.Essentials;

namespace NeuToDo.Services
{
    /// <summary>
    /// 偏好存储。
    /// </summary>
    public class PreferenceStorageProvider : IPreferenceStorageProvider
    {
        /// <param name="key">Preference key.</param>
        /// <param name="value">Preference value.</param>
        /// <summary>Sets a value for a given key.</summary>
        public void Set(string key, string value) =>
            Preferences.Set(key, value);

        /// <param name="key">Preference key.</param>
        /// <param name="defaultValue">Default value to return if the key does not exist.</param>
        /// <summary>Gets the value for a given key, or the default specified if the key does not exist.</summary>
        public string Get(string key, string defaultValue) =>
            Preferences.Get(key, defaultValue);

        /// <param name="key">Preference key.</param>
        /// <param name="value">Preference value.</param>
        /// <summary>Sets a value for a given key.</summary>
        public void Set(string key, int value) =>
            Preferences.Set(key, value);

        /// <param name="key">Preference key.</param>
        /// <param name="defaultValue">Default value to return if the key does not exist.</param>
        /// <summary>Gets the value for a given key, or the default specified if the key does not exist.</summary>
        public int Get(string key, int defaultValue) =>
            Preferences.Get(key, defaultValue);

        /// <param name="key">Preference key.</param>
        /// <param name="value">Preference value.</param>
        /// <summary>Sets a value for a given key.</summary>
        public void Set(string key, DateTime value) =>
            Preferences.Set(key, value);

        /// <param name="key">Preference key.</param>
        /// <param name="defaultValue">Default value to return if the key does not exist.</param>
        /// <summary>Gets the value for a given key, or the default specified if the key does not exist.</summary>
        /// <returns>Value for the given key, or the default if it does not exist.</returns>
        public DateTime Get(string key, DateTime defaultValue) =>
            Preferences.Get(key, defaultValue);
    }
}