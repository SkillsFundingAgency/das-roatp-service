using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.Queries.GetCourseTypes;
public class GetCourseTypesQueryHandler(ICourseTypesRepository _courseTypesRepository, ILogger<GetCourseTypesQueryHandler> _logger) : IRequestHandler<GetCourseTypesQuery, GetCourseTypesResult>
{
    public async Task<GetCourseTypesResult> Handle(GetCourseTypesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling Get All Course Types");

        List<CourseType> courseType = await _courseTypesRepository.GetAllCourseTypes(cancellationToken);
        if (courseType == null)
        {
            return null;
        }

        GetCourseTypesResult result = new()
        {
            CourseTypes = courseType.Select(c => (CourseTypes)c)
        };

        return result;
    }
}