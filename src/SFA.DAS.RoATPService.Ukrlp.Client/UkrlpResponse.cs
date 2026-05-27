using System.Collections.Generic;

namespace SFA.DAS.RoATPService.Ukrlp.Client;

public record UkrlpResponse(bool Success, IEnumerable<Provider> Providers);
