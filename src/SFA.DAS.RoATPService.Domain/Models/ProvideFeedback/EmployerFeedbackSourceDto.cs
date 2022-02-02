using System;
using System.Collections.Generic;

namespace SFA.DAS.RoATPService.Domain.Models.ProvideFeedback
{
    public class EmployerFeedbackSourceDto
    {
        public long Ukprn { get; set; }

        public DateTime DateTimeCompleted { get; set; }

        public List<ProviderAttributeSourceDto> ProviderAttributes { get; set; }

        public string ProviderRating { get; set; }
    }
}