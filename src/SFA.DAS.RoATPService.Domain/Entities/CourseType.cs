using System.Collections.Generic;

namespace SFA.DAS.RoATPService.Domain.Entities;

public class CourseType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public LearningType LearningType { get; set; }
    public bool IsActive { get; set; }

    public virtual ICollection<OrganisationCourseType> OrganisationCourseTypes { get; set; } = [];
}
