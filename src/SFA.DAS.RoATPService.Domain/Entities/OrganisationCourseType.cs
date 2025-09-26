using System;

namespace SFA.DAS.RoATPService.Domain.Entities;

public class OrganisationCourseType
{
    public Guid Id { get; set; }
    public Guid OrganisationId { get; set; }
    public int CourseTypeId { get; set; }
    public virtual Organisation Organisation { get; set; }
    public virtual CourseType CourseType { get; set; }
}
