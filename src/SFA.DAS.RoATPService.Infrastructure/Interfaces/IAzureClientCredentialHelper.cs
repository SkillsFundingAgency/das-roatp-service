using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Infrastructure.Interfaces
{
    public interface IAzureClientCredentialHelper
    {
        Task<string> GetAccessTokenAsync(string identifier);
    }
}