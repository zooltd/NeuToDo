using System.Threading.Tasks;
using NeuToDo.Models;

namespace NeuToDo.Services
{
    public class CampusStorageService : ICampusStorageService
    {
        private readonly IPreferenceStorageProvider _preferenceStorageProvider;

        private readonly IAlertService _alertService;

        public CampusStorageService(IPreferenceStorageProvider preferenceStorageProvider, IAlertService alertService)
        {
            _preferenceStorageProvider = preferenceStorageProvider;
            _alertService = alertService;
        }

        private Campus? _campus;

        public async Task<Campus> GetCampus()
        {
            return _campus ??= await GetCampusFromPreferenceStorage();
        }

        private async Task<Campus> GetCampusFromPreferenceStorage()
        {
            return _preferenceStorageProvider.ContainsKey(nameof(Campus))
                ? (Campus) _preferenceStorageProvider.Get(nameof(Campus), (int) Campus.Hunnan)
                : await GetCampusFromDialog();
        }

        private async Task<Campus> GetCampusFromDialog()
        {
            var res = await _alertService.DisplayActionSheet("请选择校区", "Cancel", null, "浑南", "南湖");
            var campus = res == "浑南" ? Campus.Hunnan : Campus.Nanhu;
            _preferenceStorageProvider.Set(nameof(Campus), (int) campus);
            return campus;
        }

        public async Task UpdateCampus()
        {
            _campus = null;
            _preferenceStorageProvider.Remove(nameof(Campus));
            await GetCampusFromDialog();
        }

        public void SaveCampus(Campus campus)
        {
            _preferenceStorageProvider.Set(nameof(Campus), (int) campus);
            _campus = campus;
        }
    }
}