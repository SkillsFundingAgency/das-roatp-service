using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace SFA.DAS.RoATPService.Application.Commands.PatchOrganisation;

public static class PatchOrganisationCommandValidatorHelper
{
    public const string StatusPath = "/status";
    public const string ProviderTypePath = "/providertype";
    public const string OrganisationTypeIdPath = "/organisationtypeid";
    public const string RemovedReasonIdPath = "/removedreasonid";

    public static readonly List<string> PatchFields =
    [
        StatusPath,
        ProviderTypePath,
        OrganisationTypeIdPath,
        RemovedReasonIdPath
    ];

    public static Operation<PatchOrganisationModel> FindOperationWithPath(this List<Operation<PatchOrganisationModel>> operations, string path)
    {
        return operations.Find(o => o.path.ToLowerInvariant() == path);
    }
}
