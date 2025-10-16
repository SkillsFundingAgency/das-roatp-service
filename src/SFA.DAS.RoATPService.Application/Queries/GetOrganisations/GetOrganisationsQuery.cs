﻿using MediatR;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisations;
public record GetOrganisationsQuery() : IRequest<GetOrganisationsQueryResult>;

