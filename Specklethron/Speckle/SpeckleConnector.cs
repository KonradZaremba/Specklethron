using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Models;
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
        public async static Task<List<Commit>> GetCommits(string id)
        {

            if (_client is null) throw new ArgumentException("you need to login first");
            return await _client.StreamGetCommits(id);
        }

        public static async Task<SpeckleObject> FetchCommitData(string streamId, string commitId)
        {
            var commit = await _client.CommitGet(streamId, commitId);
            var data = await _client.ObjectGet(streamId, commit.referencedObject);
            return data;
        }

        static Dictionary<string, int> CalculateCategoryCounts(Base data)
        {
            var categoryCounts = new Dictionary<string, int>();

            void Traverse(Base obj)
            {
                if (obj == null) return;

                foreach (var prop in obj.GetDynamicMembers())
                {
                    var value = obj[prop];
                    if (value is Base nestedObj)
                    {
                        Traverse(nestedObj);
                    }
                    else if (value is IEnumerable<Base> baseCollection)
                    {
                        foreach (var item in baseCollection)
                        {
                            Traverse(item);
                        }
                    }

                    if (obj["category"] != null)
                    {
                        var category = obj["category"].ToString();
                        if (categoryCounts.ContainsKey(category))
                        {
                            categoryCounts[category]++;
                        }
                        else
                        {
                            categoryCounts[category] = 1;
                        }
                    }
                }
            }
            Traverse(data);
            return categoryCounts;
        }

    }
}
