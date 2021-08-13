namespace SFA.DAS.RoATPService.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Application.Interfaces;
    using Dapper;
    using SFA.DAS.RoATPService.Api.Types.Models;
    using SFA.DAS.RoATPService.Infrastructure.Interfaces;

    public class DuplicateCheckRepository : IDuplicateCheckRepository
    {
        private readonly IDbConnectionHelper _dbConnectionHelper;

        public DuplicateCheckRepository(IDbConnectionHelper dbConnectionHelper)
        {
            _dbConnectionHelper = dbConnectionHelper;
        }

        public async Task<DuplicateCheckResponse> DuplicateUKPRNExists(Guid organisationId, long ukprn)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var sql = "select LegalName AS DuplicateOrganisationName, " +
                          "CASE WHEN LegalName IS NOT NULL THEN 1 ELSE 0 END AS DuplicateFound, " +
                          "Id AS DuplicateOrganisationId " +
                          "FROM [Organisations] " +
                          "WHERE UKPRN = @ukprn " +
                          "AND Id != @organisationId";
                var results = await connection.QueryAsync<DuplicateCheckResponse>(sql, new { organisationId, ukprn });

                var duplicate = results.FirstOrDefault();
                if (duplicate == null)
                {
                    return new DuplicateCheckResponse {DuplicateFound = false};
                }

                return duplicate;
            }
        }

        public async Task<DuplicateCheckResponse> DuplicateCompanyNumberExists(Guid organisationId, string companyNumber)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var sql = "select LegalName FROM [Organisations] " +
                          "WHERE JSON_VALUE(OrganisationData, '$.CompanyNumber') = @companyNumber " +
                          "AND Id != @organisationId";
                string duplicateLegalName = await connection.ExecuteScalarAsync<string>(sql, new { organisationId, companyNumber });

                DuplicateCheckResponse response = new DuplicateCheckResponse
                {
                    DuplicateFound = !string.IsNullOrWhiteSpace(duplicateLegalName),
                    DuplicateOrganisationName = duplicateLegalName
                };
                return response;
            }
        }

        public async Task<DuplicateCheckResponse> DuplicateCharityNumberExists(Guid organisationId, string charityNumber)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var sql = "select LegalName FROM [Organisations] " +
                          "WHERE JSON_VALUE(OrganisationData, '$.CharityNumber') = @charityNumber " +
                          "AND Id != @organisationId";
                string duplicateLegalName = await connection.ExecuteScalarAsync<string>(sql, new { organisationId, charityNumber });

                DuplicateCheckResponse response = new DuplicateCheckResponse
                {
                    DuplicateFound = !string.IsNullOrWhiteSpace(duplicateLegalName),
                    DuplicateOrganisationName = duplicateLegalName
                };
                return response;
            }
        }
    }
}
