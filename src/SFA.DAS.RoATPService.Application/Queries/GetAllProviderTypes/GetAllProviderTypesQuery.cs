using MediatR;

namespace SFA.DAS.RoATPService.Application.Queries.GetAllProviderTypes;

public record GetAllProviderTypesQuery : IRequest<GetAllProviderTypesQueryResult>;
