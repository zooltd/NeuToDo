namespace NeuToDo.Services
{
    public class SyncService : ISyncService
    {
        private readonly IDbStorageProvider _localDbStorageProvider;
        private readonly IHttpWebDavService _remoteDbStrDbStorageProvider;

        public SyncService(IDbStorageProvider localDbStorageProvider,
            IHttpWebDavService remoteDbStrDbStorageProvider)
        {
            _localDbStorageProvider = localDbStorageProvider;
            _remoteDbStrDbStorageProvider = remoteDbStrDbStorageProvider;
        }
    }
}