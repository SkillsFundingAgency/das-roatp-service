using System.Collections.Generic;

namespace SFA.DAS.RoATPService.Domain.Models.FatDataExport
{
    public class FatProviderFeedbackData
    {
        public int Total { get; set; }
        public List<KeyValuePair<string, int>> FeedbackRating { get; set; }
        
        public List<ProviderAttribute> ProviderAttributes { get; set; }
    }
}