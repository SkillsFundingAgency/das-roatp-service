using System;

namespace SFA.DAS.RoATPService.Domain.Models.FatDataExport
{
    public class FatDataExport
    {
        public int UkPrn { get; set; }
        public DateTime StatusDate { get; set; }
        public int StatusId { get; set; }
        public int OrganisationTypeId { get; set; }
        public int ProviderTypeId { get; set; }
        public FatProviderFeedbackData Feedback { get ; set ; }

        public static implicit operator FatDataExport(FatDataExportDto source)
        {
            return new FatDataExport
            {
                StatusDate = source.StatusDate,
                StatusId = source.StatusId,
                UkPrn = source.UkPrn,
                OrganisationTypeId = source.OrganisationTypeId,
                ProviderTypeId = source.ProviderTypeId
            };
        }
    }
}