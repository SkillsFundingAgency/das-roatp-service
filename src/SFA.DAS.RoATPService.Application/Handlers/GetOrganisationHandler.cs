using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Application.Exceptions;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Application.Validators;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Application.Handlers;

public class GetOrganisationHandler : IRequestHandler<GetOrganisationRequest, Organisation>
{
    private readonly IOrganisationRepository _organisationRepository;
    private readonly ILogger<GetOrganisationHandler> _logger;
    private readonly IOrganisationValidator _organisationValidator;

    public GetOrganisationHandler(IOrganisationRepository repository, ILogger<GetOrganisationHandler> logger,
        IOrganisationValidator organisationValidator)
    {
        _organisationRepository = repository;
        _logger = logger;
        _organisationValidator = organisationValidator;
    }

    public Task<Organisation> Handle(GetOrganisationRequest request, CancellationToken cancellationToken)
    {
        if (!_organisationValidator.IsValidOrganisationId(request.OrganisationId))
        {
            string invalidOrganisationError = $@"Invalid Organisation Id [{request.OrganisationId}]";
            _logger.LogInformation(invalidOrganisationError);
            throw new BadRequestException(invalidOrganisationError);
        }

        _logger.LogInformation("Handling Organisation lookup for Id [{OrganisationId}]", request.OrganisationId);

        return _organisationRepository.GetOrganisation(request.OrganisationId);
    }
}
