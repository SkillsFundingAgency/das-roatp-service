using System;
using System.Collections.Generic;

namespace SFA.DAS.RoATPService.Ukrlp.Client;

public record UkrlpRequest(DateTime? UpdatedSinceDate, IEnumerable<int> Ukprns);
