namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Domain;
    using Interfaces;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class GetOrganisationStatusesHandler : IRequestHandler<GetOrganisationStatusesRequest, IEnumerable<OrganisationStatus>>
    {
        private readonly ILookupDataRepository _repository;
        private readonly ILogger<GetOrganisationStatusesHandler> _logger;

        public GetOrganisationStatusesHandler(ILookupDataRepository repository,
            ILogger<GetOrganisationStatusesHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<OrganisationStatus>> Handle(GetOrganisationStatusesRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling Organisation Statuses lookup");

            try
            {
                return await _repository.GetOrganisationStatusesForProviderTypeId(request.ProviderTypeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to retrieve Organisation Statuses");
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
