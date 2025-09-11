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

    public class GetRemovedReasonsHandler : IRequestHandler<GetRemovedReasonsRequest, IEnumerable<RemovedReason>>
    {
        private readonly ILookupDataRepository _repository;
        private readonly ILogger<GetRemovedReasonsHandler> _logger;

        public GetRemovedReasonsHandler(ILookupDataRepository repository,
            ILogger<GetRemovedReasonsHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<RemovedReason>> Handle(GetRemovedReasonsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling Removed Reasons lookup");

            try
            {
                return await _repository.GetRemovedReasons();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to retrieve Removed Reasons");
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
