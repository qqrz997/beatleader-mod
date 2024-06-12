using BeatLeader.API.RequestDescriptors;
using BeatLeader.API.RequestHandlers;
using BeatLeader.Models;
using BeatLeader.Utils;

namespace BeatLeader.API.Methods {
    internal class PrestigeRequest : PersistentSingletonRequestHandler<PrestigeRequest, Player> {
        private static string Endpoint => BLConstants.BEATLEADER_API_URL + "/player/{0}/prestige";

        public static void SendRequest(string playerId) {
            var url = string.Format(Endpoint, playerId);
            var requestDescriptor = new JsonGetRequestDescriptor<Player>(url);
            Instance.Send(requestDescriptor);
        }
    }
}