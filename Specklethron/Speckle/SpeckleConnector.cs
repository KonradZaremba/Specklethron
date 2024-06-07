using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Stream = Speckle.Core.Api.Stream;

namespace Specklethron.Speckle
{
    public class SpeckleConnector
    {
        public static Client _client;

        public async static Task<string> Login(string username)
        {
            var account = AccountManager.GetAccounts().Where(x => x.userInfo.name == username).SingleOrDefault();

            if (account is null) return "fail to login";
            _client = new Client(account);

            return ("Logged in:" + _client.Account.userInfo.name);
        }

        public static List<string> GetUsers()
        {
            var accounts = AccountManager.GetAccounts();
            return accounts.Select(x => x.userInfo.name).ToList();
        }

        public async static Task<List<Stream>> GetSterams()
        {
            if (_client is null) throw new ArgumentException("you need to login first");
            return await _client.StreamsGet();
        }

        public async static Task<Stream> GetSteram(string id)
        {
            var cts = new CancellationTokenSource();

            if (_client is null) throw new ArgumentException("you need to login first");
            return await _client.StreamGet(id);
        }
    }
}
