
using System.Collections.Generic;

namespace SFA.DAS.RoATPService.Api.Client.Models
{
    public class UkprnLookupResponse
    {
        public bool Success { get; set; }
        public List<ProviderDetails> Results { get; set; }
    }
}
