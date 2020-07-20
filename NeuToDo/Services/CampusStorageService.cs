using System;
using NeuToDo.Models;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public class CampusStorageService : ICampusStorageService
    {
        private readonly IPreferenceStorageProvider _preferenceStorageProvider;

        private readonly IDialogService _dialogService;

        public CampusStorageService(IPreferenceStorageProvider preferenceStorageProvider, IDialogService dialogService)
        {
            _preferenceStorageProvider = preferenceStorageProvider;
            _dialogService = dialogService;
        }

        private Campus? _campus;

        public Campus GetCampus() => (Campus) _preferenceStorageProvider.Get(nameof(Campus), (int) Campus.暂未设置);

        public async Task<Campus> GetOrSelectCampus()
        {
            return _campus ??= await GetCampusFromPreferenceStorage();
        }

        private async Task<Campus> GetCampusFromPreferenceStorage()
        {
            return _preferenceStorageProvider.ContainsKey(nameof(Campus))
                ? (Campus) _preferenceStorageProvider.Get(nameof(Campus), (int) Campus.暂未设置)
                : await GetCampusFromDialog();
        }

        private async Task<Campus> GetCampusFromDialog()
        {
            string res;
            do
            {
                res = await _dialogService.DisplayActionSheet("请选择校区", "Cancel", null, Campus.浑南.ToString(),
                    Campus.南湖.ToString());
            } while (res == null || res == "Cancel");

            var campus = (Campus) Enum.Parse(typeof(Campus), res);
            _preferenceStorageProvider.Set(nameof(Campus), (int) campus);
            return campus;
        }


        public void SaveCampus(Campus campus)
        {
            _preferenceStorageProvider.Set(nameof(Campus), (int) campus);
            _campus = campus;
        }
    }
}