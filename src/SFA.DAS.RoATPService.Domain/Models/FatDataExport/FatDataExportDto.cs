using System;

namespace SFA.DAS.RoATPService.Domain.Models.FatDataExport
{
    public class FatDataExportDto
    {
        public long UkPrn { get; set; }
        public DateTime StatusDate { get; set; }
        public int StatusId { get; set; }
        public int OrganisationTypeId { get; set; }
        public int ProviderTypeId { get; set; }
    }
}