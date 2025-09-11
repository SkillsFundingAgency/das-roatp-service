namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Interfaces;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class DuplicateCharityNumberCheckHandler : IRequestHandler<DuplicateCharityNumberCheckRequest, DuplicateCheckResponse>
    {
        private readonly ILogger<DuplicateCharityNumberCheckHandler> _logger;

        private readonly IDuplicateCheckRepository _repository;

        public DuplicateCharityNumberCheckHandler(ILogger<DuplicateCharityNumberCheckHandler> logger,
            IDuplicateCheckRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<DuplicateCheckResponse> Handle(DuplicateCharityNumberCheckRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return await _repository.DuplicateCharityNumberExists(request.OrganisationId, request.CharityNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to perform charity registration number duplicate check");
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
