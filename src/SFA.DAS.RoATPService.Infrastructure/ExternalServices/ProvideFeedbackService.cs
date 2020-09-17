using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.RoATPService.Domain.Models.ProvideFeedback;
using SFA.DAS.RoATPService.Infrastructure.Interfaces;
using SFA.DAS.RoATPService.Settings;

namespace SFA.DAS.RoATPService.Infrastructure.ExternalServices
{
    public class ProvideFeedbackService
    {
        private readonly IWebConfiguration _configuration;
        private readonly HttpClient _client;
        private readonly IAzureClientCredentialHelper _azureClientCredentialHelper;

        public ProvideFeedbackService(IWebConfiguration configuration, HttpClient client, IAzureClientCredentialHelper azureClientCredentialHelper)
        {
            _configuration = configuration;
            _client = client;
            _client.BaseAddress = new Uri(_configuration.ProvideFeedbackApiConfiguration.Url);
            _azureClientCredentialHelper = azureClientCredentialHelper;
        }

        public async Task<IEnumerable<EmployerFeedbackSourceDto>> GetProviderFeedBack()
        {
            await AddAuthHeader();

            var response = await _client.GetAsync("/api/feedback").ConfigureAwait(false);
            
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<IEnumerable<EmployerFeedbackSourceDto>>(json);
        }

        private async Task AddAuthHeader()
        {
            if (_configuration.ProvideFeedbackApiConfiguration.Url.StartsWith("https://localhost"))
            {
                return;
            }
            
            var accessToken =
                await _azureClientCredentialHelper.GetAccessTokenAsync(
                    _configuration.ProvideFeedbackApiConfiguration.Identifier);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
}