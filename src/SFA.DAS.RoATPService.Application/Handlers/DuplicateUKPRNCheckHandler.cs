namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Interfaces;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class DuplicateUkprnCheckHandler : IRequestHandler<DuplicateUkprnCheckRequest, DuplicateCheckResponse>
    {
        private readonly ILogger<DuplicateUkprnCheckHandler> _logger;

        private readonly IDuplicateCheckRepository _repository;

        public DuplicateUkprnCheckHandler(ILogger<DuplicateUkprnCheckHandler> logger,
            IDuplicateCheckRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<DuplicateCheckResponse> Handle(DuplicateUkprnCheckRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return await _repository.DuplicateUKPRNExists(request.OrganisationId, request.UKPRN);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to perform UKPRN duplicate check");
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
