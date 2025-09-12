namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Interfaces;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class DuplicateCompanyNumberCheckHandler : IRequestHandler<DuplicateCompanyNumberCheckRequest, DuplicateCheckResponse>
    {
        private readonly ILogger<DuplicateCompanyNumberCheckHandler> _logger;

        private readonly IDuplicateCheckRepository _repository;

        public DuplicateCompanyNumberCheckHandler(ILogger<DuplicateCompanyNumberCheckHandler> logger,
            IDuplicateCheckRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<DuplicateCheckResponse> Handle(DuplicateCompanyNumberCheckRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return await _repository.DuplicateCompanyNumberExists(request.OrganisationId, request.CompanyNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to perform company number duplicate check");
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
