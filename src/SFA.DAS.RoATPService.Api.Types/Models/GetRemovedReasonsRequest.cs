﻿namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using System.Collections.Generic;
    using Domain;
    using MediatR;
 
    public class GetRemovedReasonsRequest : IRequest<IEnumerable<RemovedReason>>
    {
    }
}
