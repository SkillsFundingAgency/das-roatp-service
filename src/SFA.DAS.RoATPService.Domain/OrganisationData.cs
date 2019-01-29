namespace SFA.DAS.RoATPService.Domain
{
    using System;

    public class OrganisationData
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Postcode { get; set; }
        public string CompanyNumber { get; set; }
        public string CharityNumber { get; set; }
        public bool AcceptNewApprentices { get; set; }
        public DateTime? AcceptStartDate { get; set; }
        public DateTime? AcceptEndDate { get; set; }
        public int EndReasonId { get; set; }
        public string EndReasonDescription { get; set; }
        public bool ParentCompanyGuarantee { get; set; }
        public bool HasNoFinancialTrackRecord { get; set; }
    }
}
