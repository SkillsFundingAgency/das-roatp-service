using MediatR;
using SFA.DAS.RoATPService.Application.Common;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisation;

public record GetOrganisationQuery(int Ukprn) : IRequest<OrganisationModel>;
