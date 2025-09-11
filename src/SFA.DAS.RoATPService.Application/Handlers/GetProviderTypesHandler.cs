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

    public class GetProviderTypesHandler : IRequestHandler<GetProviderTypesRequest, IEnumerable<ProviderType>>
    {
        private readonly ILookupDataRepository _repository;
        private readonly ILogger<GetProviderTypesHandler> _logger;

        public GetProviderTypesHandler(ILookupDataRepository repository, ILogger<GetProviderTypesHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<ProviderType>> Handle(GetProviderTypesRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return await _repository.GetProviderTypes();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to retrieve list of provider types");
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
