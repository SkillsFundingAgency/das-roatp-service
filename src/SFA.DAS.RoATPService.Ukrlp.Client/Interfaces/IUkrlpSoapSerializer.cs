using System.Collections.Generic;
using SFA.DAS.RoATPService.Ukrlp.Client.Models;

namespace SFA.DAS.RoATPService.Ukrlp.Client.Interfaces;

public interface IUkrlpSoapSerializer
{
    string BuildUkrlpSoapRequest(long ukprn, string stakeholderId, string queryId);
    List<MatchingProviderRecords> DeserialiseMatchingProviderRecordsResponse(string soapXml);
}
