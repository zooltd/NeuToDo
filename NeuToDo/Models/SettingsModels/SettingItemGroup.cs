using System.Collections.Generic;

namespace NeuToDo.Models.SettingsModels
{
    public class SettingItemGroup : List<SettingItem>
    {
        public string Name { get; private set; }

        public SettingItemGroup(string name, List<SettingItem> items) : base(items)
        {
            Name = name;
        }
    }
}