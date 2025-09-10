
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.RoATPService.Ukrlp.Models;

namespace SFA.DAS.RoATPService.Api.Client.Models.Ukrlp;

public class UkprnLookupResponse
{
    public bool Success { get; set; }
    public IEnumerable<ProviderDetails> Results { get; set; } = Enumerable.Empty<ProviderDetails>();
}
