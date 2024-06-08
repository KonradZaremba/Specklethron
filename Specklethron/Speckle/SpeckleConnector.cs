using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Models;
using Speckle.Core.Models.Extensions;
using Speckle.Core.Models.GraphTraversal;
using Speckle.Core.Transports;
using System;
using System.Threading;
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
        public async static Task<List<Commit>> GetCommit(string id, string commitId)
        {

            if (_client is null) throw new ArgumentException("you need to login first");
            return await _client.StreamGetCommits(id);
        }

        public static async Task<SpeckleObject> FetchCommitObjectData(string streamId, string commitId)
        {
            var commit = await _client.CommitGet(streamId, commitId);
            var data = await _client.ObjectGet(streamId, commit.referencedObject);
            return data;
        }


        private static async Task<Base> FetchCommitObject(string streamId, string commitId)
        {
            var commit = await _client.CommitGet(streamId, commitId).ConfigureAwait(false);
            using ServerTransport transport = new(_client.Account, streamId);

            Base commitObject = await Operations
              .Receive(
                commit.referencedObject,
                transport
              )
              .ConfigureAwait(false);
            return commitObject;
        }

        public static async Task<List<Base>> FetchAllObjectsInCommit(string streamId, string commitId)
        {
            var commitData = await FetchCommitObject(streamId, commitId);
            var traversalFunc = DefaultTraversal.CreateTraversalFunc();

           return await Task.Run(() => traversalFunc.Traverse(commitData)
                .Select(c => c.Current)
                .ToList());
        }

        public static async Task<SpeckleObject> GetObjectCountInCommit(string streamId, string commitId)
        {

            var commit = await _client.CommitGet(streamId, commitId);
            var data = await _client.ObjectCountGet(streamId, commit.referencedObject);
            return data;
        }

        public async static Task<Dictionary<string, object>> CalculateCategoryCounts(string streamId, string commitId)
        {
            var categoryCounts = new Dictionary<string, object>();

            var commitObject = await FetchCommitObject(streamId, commitId);

            var members = commitObject.GetMembers();

            foreach(var m in members)
            {
                var val = m.Value as List<object>;
                if (val != null)
                {
                    categoryCounts.Add(m.Key, val.Count());
                }
            }
            return categoryCounts;
        }
    }
}
