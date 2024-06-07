using Microsoft.SemanticKernel;
using Specklethron.Speckle;
using System.ComponentModel;
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
        public async Task<Stream> GetSteram([Description("Stram Id")] string id)
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
    }
}
