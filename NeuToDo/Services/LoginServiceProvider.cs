using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeuToDo.Models;

namespace NeuToDo.Services
{
    public class LoginServiceProvider : ILoginServiceProvider
    {
        private ILoginService _loginService;

        private readonly IEventModelStorageProvider _eventModelStorageProvider;

        public LoginServiceProvider(IEventModelStorageProvider eventModelStorageProvider)
        {
            _eventModelStorageProvider = eventModelStorageProvider;
        }


        public Dictionary<ServerType, ILoginService> KeyServiceDic = new Dictionary<ServerType, ILoginService>();

        public async Task<ILoginService> GetLoginService(ServerType serverType)
        {
            if (KeyServiceDic.ContainsKey(serverType)) return KeyServiceDic[serverType];
            switch (serverType)
            {
                case ServerType.Neu:
                    var neuStorage = await _eventModelStorageProvider.GetEventModelStorage<NeuEvent>();
                    var neuLoginService = new NeuLoginService(neuStorage);
                    KeyServiceDic.Add(serverType, neuLoginService);
                    return neuLoginService;
                case ServerType.Mooc:
                    var moocStorage = await _eventModelStorageProvider.GetEventModelStorage<MoocEvent>();
                    var moocLoginService = new MoocLoginService(moocStorage);
                    KeyServiceDic.Add(serverType, moocLoginService);
                    return moocLoginService;
                case ServerType.Bb:
                    break;
                case ServerType.WebDav:
                    break;
                case ServerType.Github:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(serverType), serverType, null);
            }

            return null;
        }
    }
}