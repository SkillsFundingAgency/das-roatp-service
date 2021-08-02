namespace SFA.DAS.RoATPService.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Application.Interfaces;
    using Dapper;
    using SFA.DAS.RoATPService.Domain;
    using Microsoft.Extensions.Configuration;
    using SFA.DAS.RoatpService.Data.DapperTypeHandlers;
    using SFA.DAS.RoATPService.Api.Types.Models;
    using SFA.DAS.RoATPService.Infrastructure.Interfaces;

    public class OrganisationSearchRepository : IOrganisationSearchRepository
    {
        private readonly IDbConnectionHelper _dbConnectionHelper;
        private readonly IConfiguration _appConfiguration;

        public OrganisationSearchRepository(IDbConnectionHelper dbConnectionHelper, IConfiguration appConfiguration)
        {
            _dbConnectionHelper = dbConnectionHelper;
            _appConfiguration = appConfiguration;
            SqlMapper.AddTypeHandler(typeof(OrganisationData), new OrganisationDataHandler());
        }

        public async Task<OrganisationSearchResults> OrganisationSearchByUkPrn(string ukPrn)
        {
            long ukPrnValue = Convert.ToInt64(ukPrn);

            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                const string sql = "select * from [Organisations] o " +
                                   "inner join ProviderTypes pt on o.ProviderTypeId = pt.Id  " +
                                   "inner join OrganisationTypes ot on o.OrganisationTypeId = ot.Id " +
                                   "inner join OrganisationStatus os on o.StatusId = os.Id " +
                                   "where UKPRN = @ukPrnValue";

                var organisations = await connection.QueryAsync<Organisation, ProviderType, OrganisationType,
                    OrganisationStatus, Organisation>
                     (sql, (org, providerType, type, status) => {
                        org.OrganisationType = type;
                        org.ProviderType = providerType;
                        org.OrganisationStatus = status;
                        return org;
                    },
                    new {ukPrnValue});

                var searchResults = new OrganisationSearchResults
                {
                    SearchResults = organisations,
                    TotalCount = organisations.Count()
                };

                return searchResults;
            }
         }

        public async Task<OrganisationSearchResults> OrganisationSearchByName(string organisationName)
        {
            int rowLimit = 5;
            int.TryParse(_appConfiguration["OrganisationSearchResultsLimit"], out rowLimit);

            var organisationNameFilter = $"%{organisationName}%";
       
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                const string sql = "select top (@rowLimit) * from [Organisations] o " +
                                   "inner join ProviderTypes pt on o.ProviderTypeId = pt.Id " +
                                   "inner join OrganisationTypes ot on o.OrganisationTypeId = ot.Id " +
                                   "inner join OrganisationStatus os on o.StatusId = os.Id " +
                                   "where o.LegalName LIKE @organisationNameFilter " +
                                   "order by legalname asc; " +
                                   "select count(*) from[Organisations] " +
                                   "where LegalName like @organisationNameFilter";
                var searchQuery = await connection.QueryMultipleAsync
                    (sql, new { rowLimit, organisationNameFilter });

                var results =
                    searchQuery.Read<Organisation, ProviderType, OrganisationType, OrganisationStatus, Organisation>(
                        (org, providerType, type, status) =>
                        {
                            org.OrganisationType = type;
                            org.ProviderType = providerType;
                            org.OrganisationStatus = status;
                            return org;
                        });

                var resultCount = searchQuery.ReadFirst<int>();

                var searchResult = new OrganisationSearchResults
                {
                    SearchResults = results,
                    TotalCount = resultCount
                };

                return searchResult;
            }
        }
    }
}
