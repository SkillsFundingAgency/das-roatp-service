using System.Collections.Generic;

namespace SFA.DAS.RoATPService.Ukrlp.Client;

public record UkrlpQueryResult(bool Success, IEnumerable<Provider> Providers);
