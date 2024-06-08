using Objects.Geometry;
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
    //TODO Make as service and add to container
    //TODO Separate Query and Create


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


        public static async Task SendGeometry(string streamId)
        {
            // Create geometry for "SPEKLETHRON"
            var lines = CreateSpeklethronLines();

            // Create a base object to hold the geometry
            var baseObject = new Base();
            for (int i = 0; i < lines.Count; i++)
            {
                baseObject[$"@line_{i}"] = lines[i];
            };


            // Create a base object (Speckle object)
            baseObject["@example"] = "Hello Human, I am Specklethron!";

            // Use the server transport to send the object
            var transport = new ServerTransport(_client.Account, streamId);

            // Send the object to the server
            string objectId = await Operations.Send(baseObject, new List<ITransport> { transport });

            // Create commit
            var commitCreateInput = new CommitCreateInput
            {
                streamId = streamId,
                objectId = objectId,
                branchName = "main",
                message = "Hello Human, I am Specklethron!",
                sourceApplication = "Specklethron"
            };

            await _client.CommitCreate(commitCreateInput);

        }


        private static List<Line> CreateSpeklethronLines()
        {
            var lines = new List<Line>();

            // S
            lines.Add(new Line(new Point(0, 10, 0), new Point(10, 10, 0)));
            lines.Add(new Line(new Point(10, 10, 0), new Point(10, 20, 0)));
            lines.Add(new Line(new Point(0, 20, 0), new Point(0, 30, 0)));
            lines.Add(new Line(new Point(0, 30, 0), new Point(10, 20, 0)));
            lines.Add(new Line(new Point(0, 30, 0), new Point(10, 30, 0)));

            // P
            lines.Add(new Line(new Point(20, 10, 0), new Point(20, 30, 0)));
            lines.Add(new Line(new Point(20, 30, 0), new Point(30, 30, 0)));
            lines.Add(new Line(new Point(30, 30, 0), new Point(30, 20, 0)));
            lines.Add(new Line(new Point(30, 20, 0), new Point(20, 20, 0)));

            // E
            lines.Add(new Line(new Point(40, 10, 0), new Point(40, 30, 0)));
            lines.Add(new Line(new Point(40, 30, 0), new Point(50, 30, 0)));
            lines.Add(new Line(new Point(40, 20, 0), new Point(50, 20, 0)));
            lines.Add(new Line(new Point(40, 10, 0), new Point(50, 10, 0)));

            // C
            lines.Add(new Line(new Point(60, 10, 0), new Point(60, 30, 0)));
            lines.Add(new Line(new Point(60, 30, 0), new Point(70, 30, 0)));
            lines.Add(new Line(new Point(60, 10, 0), new Point(70, 10, 0)));

            // K
            lines.Add(new Line(new Point(80, 10, 0), new Point(80, 30, 0)));
            lines.Add(new Line(new Point(80, 20, 0), new Point(90, 30, 0)));
            lines.Add(new Line(new Point(80, 20, 0), new Point(90, 10, 0)));

            // L
            lines.Add(new Line(new Point(100, 10, 0), new Point(100, 30, 0)));
            lines.Add(new Line(new Point(100, 10, 0), new Point(110, 10, 0)));

            // E
            lines.Add(new Line(new Point(120, 10, 0), new Point(120, 30, 0)));
            lines.Add(new Line(new Point(120, 30, 0), new Point(130, 30, 0)));
            lines.Add(new Line(new Point(120, 20, 0), new Point(130, 20, 0)));
            lines.Add(new Line(new Point(120, 10, 0), new Point(130, 10, 0)));

            // T
            lines.Add(new Line(new Point(140, 30, 0), new Point(150, 30, 0)));
            lines.Add(new Line(new Point(145, 10, 0), new Point(155, 30, 0)));

            // H
            lines.Add(new Line(new Point(160, 10, 0), new Point(160, 30, 0)));
            lines.Add(new Line(new Point(170, 10, 0), new Point(170, 30, 0)));
            lines.Add(new Line(new Point(160, 20, 0), new Point(170, 20, 0)));

            // R
            lines.Add(new Line(new Point(180, 10, 0), new Point(180, 30, 0)));
            lines.Add(new Line(new Point(180, 30, 0), new Point(190, 30, 0)));
            lines.Add(new Line(new Point(190, 30, 0), new Point(190, 20, 0)));
            lines.Add(new Line(new Point(190, 20, 0), new Point(180, 20, 0)));
            lines.Add(new Line(new Point(170, 20, 0), new Point(190, 10, 0)));

            // O
            lines.Add(new Line(new Point(200, 10, 0), new Point(200, 30, 0)));
            lines.Add(new Line(new Point(200, 30, 0), new Point(210, 30, 0)));
            lines.Add(new Line(new Point(210, 30, 0), new Point(210, 10, 0)));
            lines.Add(new Line(new Point(210, 10, 0), new Point(200, 10, 0)));

            // N
            lines.Add(new Line(new Point(220, 10, 0), new Point(220, 30, 0)));
            lines.Add(new Line(new Point(220, 30, 0), new Point(230, 10, 0)));
            lines.Add(new Line(new Point(230, 10, 0), new Point(230, 30, 0)));

            return lines;

        }
    }
}

