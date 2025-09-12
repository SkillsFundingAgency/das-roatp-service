namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using System;
    using MediatR;

    public class DuplicateUkprnCheckRequest : IRequest<DuplicateCheckResponse>
    {
        public Guid OrganisationId { get; set; }
        public long UKPRN { get; set; }
    }
}
