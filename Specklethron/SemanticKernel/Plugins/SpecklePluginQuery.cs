using Microsoft.SemanticKernel;
using Sentry;
using Speckle.Core.Api;
using Speckle.Core.Api.SubscriptionModels;
using Speckle.Core.Models;
using Specklethron.Speckle;
using System.ComponentModel;
using System.Net;
using Stream = Speckle.Core.Api.Stream;


namespace Specklethron.Plugins
{
    public class SpeklePlugin
    {

        [KernelFunction, Description("Check accounts you can login, give user names")]
        public async Task<List<string>> GetUsers()
        {
            try
            {
                return await Task.Run(() => SpeckleConnector.GetUsers());

            }
            catch
            {
                return new List<string>() { "No user logged in go to youe Spekle manager" };
            }
        }

        [KernelFunction, Description("Login to account with given user name")]
        public async Task<string> Login([Description("User name")] string userName)
        {
            try
            {
                var users = await SpeckleConnector.Login(userName);
                return users;
            }
            catch
            {
                return "Couldn't login";
            }
        }

        [KernelFunction, Description("Get user streams")]
        public async Task<List<Stream>> GetSterams()
        {
            try
            {
                return await SpeckleConnector.GetSterams();
            }
            catch
            {
                throw new ArgumentException("Couldn't fetch streams");
            }
        }

        [KernelFunction, Description("Get user stream")]
        public async Task<Stream> GetSteram([Description("Stream Id")] string id)
        {
            try
            {
                return await SpeckleConnector.GetSteram(id);
            }
            catch
            {
                throw new ArgumentException("Couldn't fetch stream");
            }
        }

        [KernelFunction, Description("Get user stream")]
        public async Task<List<Commit>> GetCommits([Description("Stream Id")] string id)
        {
            try
            {
                return await SpeckleConnector.GetCommits(id);
            }
            catch
            {
                throw new ArgumentException("Couldn't fetch stream");
            }
        }

        [KernelFunction, Description("Get obects form stream and commit")]
        public async Task<Object> GetObject([Description("Stream Id")] string stream, [Description("Commit Id")] string commit)
        {
            try
            {
                return await SpeckleConnector.FetchCommitObjectData(stream, commit);
            }
            catch
            {
                throw new ArgumentException("Couldn't fetch objects from commit");
            }
        }
        [KernelFunction, Description("Get all objects form and commit")]
        public async Task<Object> GetAllObjects([Description("Stream Id")] string streamId, [Description("Commit Id")] string commitId)
        {
            try
            {
                return await SpeckleConnector.FetchAllObjectsInCommit(streamId, commitId);
            }
            catch
            {
                throw new ArgumentException("Couldn't fetch objects from commit");
            }
        }

        [KernelFunction, Description("Get all categories form and commit")]
        public async Task<Dictionary<string, object>> GetAllCategories([Description("Stream Id")] string streamId, [Description("Commit Id")] string commitId)
        {
            try
            {
                return await Task.Run(()=> SpeckleConnector.CalculateCategoryCounts(streamId, commitId));
            }
            catch
            {
                throw new ArgumentException("Couldn't fetch categories from commit");
            }
        }
        /*
         //Object too big to pass throu llm -> need to use fetch in functions
        [KernelFunction, Description("Get commti object can be travesed")]
        public async Task<Base> GetCommitObject([Description("Stream Id")] string streamId, [Description("Commit Id")] string commitId)
        {
            try
            {
                return await Task.Run(() => SpeckleConnector.FetchCommitObject(streamId, commitId));
            }
            catch
            {
                throw new ArgumentException("Couldn't fetch categories from commit");
            }
        }
        */

        [KernelFunction, Description("Generates and sends commit")]
        public async Task SendObject([Description("Stream Id")] string streamId)
        {
            try
            {
                await Task.Run(() => SpeckleConnector.SendGeometry(streamId));
                return;
            }
            catch
            {
                throw new ArgumentException("Couldn't send commit");
            }
        }

        [KernelFunction, Description("Get planed functionalities for Specklethron")]
        public async Task<List<string>> GetPlannedFunctionalities()
        {
            return new List<string>()
            {
                "spekle viewport",
                "lodading pdf",
                "loading dwg",
                "compare commits",
                "add gometry to stream",
                "add data to objects",
                "open app with spekle connector"
            };
        }

        //TODO Get Commit Object

    }
}
