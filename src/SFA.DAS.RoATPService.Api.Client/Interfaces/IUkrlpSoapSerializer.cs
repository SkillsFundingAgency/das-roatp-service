﻿using System.Collections.Generic;
using SFA.DAS.RoATPService.Api.Client.Models.Ukrlp;

namespace SFA.DAS.RoATPService.Api.Client.Interfaces
{
    public interface IUkrlpSoapSerializer
    {
        string BuildUkrlpSoapRequest(long ukprn, string stakeholderId, string queryId);
        List<MatchingProviderRecords> DeserialiseMatchingProviderRecordsResponse(string soapXml);
        string BuildGetAllUkrlpSoapRequest(List<long> ukprns, string stakeholderId, string queryId);
    }
}
