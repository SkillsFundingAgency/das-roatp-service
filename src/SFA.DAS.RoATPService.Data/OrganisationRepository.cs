﻿namespace SFA.DAS.RoATPService.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Application.Interfaces;
    using Dapper;
    using SFA.DAS.RoATPService.Domain;
    using RoatpService.Data.DapperTypeHandlers;
    using Newtonsoft.Json;
    using SFA.DAS.RoATPService.Api.Types.Models;
    using SFA.DAS.RoATPService.Infrastructure.Interfaces;

    public class OrganisationRepository : IOrganisationRepository
    {
        private readonly IDbConnectionHelper _dbConnectionHelper;

        public OrganisationRepository(IDbConnectionHelper dbConnectionHelper)
        {
            _dbConnectionHelper = dbConnectionHelper;
            SqlMapper.AddTypeHandler(typeof(OrganisationData), new OrganisationDataHandler());
        }

        public async Task<Organisation> GetOrganisation(Guid organisationId)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                const string sql = "select * from [Organisations] o " +
                                   "inner join ProviderTypes pt on o.ProviderTypeId = pt.Id  " +
                                   "inner join OrganisationTypes ot on o.OrganisationTypeId = ot.Id " +
                                   "inner join OrganisationStatus os on o.StatusId = os.Id " +
                                   "where o.Id = @organisationId";

                var organisations = await connection.QueryAsync<Organisation, ProviderType, OrganisationType, 
                                                                OrganisationStatus, Organisation>
                    (sql, (org, providerType, type, status) => {
                        org.OrganisationType = type;
                        org.ProviderType = providerType;
                        org.OrganisationStatus = status;
                        return org;
                    },
                    new { organisationId });

                return organisations.FirstOrDefault();
            }
        }

        public async Task<string> GetLegalName(Guid organisationId)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                const string sql = "select LegalName FROM [Organisations] WHERE Id = @organisationId";
                return await connection.ExecuteScalarAsync<string>(sql, new { organisationId });
            }
        }

        public async Task<string> GetTradingName(Guid organisationId)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                const string sql = "select TradingName FROM [Organisations] WHERE Id = @organisationId";
                return await connection.ExecuteScalarAsync<string>(sql, new { organisationId });
            }
        }

        public async Task<int> GetOrganisationStatus(Guid organisationId)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                const string sql = "select StatusId FROM [Organisations] WHERE Id = @organisationId";
                return await connection.ExecuteScalarAsync<int>(sql, new { organisationId });
            }
        }

        public async Task<RemovedReason> GetRemovedReason(Guid organisationId)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                const string sql = "select JSON_QUERY(OrganisationData, '$.RemovedReason') FROM [Organisations] " +
                                   "WHERE Id = @organisationId";
                var results = await connection.QueryAsync<string>(sql, new { organisationId });
                var resultJson = results.FirstOrDefault();
                if (resultJson == null)
                {
                    return null;
                }

                return JsonConvert.DeserializeObject<RemovedReason>(resultJson);
            }
        }

        public async Task<string> GetCompanyNumber(Guid organisationId)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                const string sql = "SELECT Json_value(organisationData,'$.CompanyNumber') FROM [Organisations] WHERE Id = @organisationId";
                return await connection.ExecuteScalarAsync<string>(sql, new { organisationId });
            }
        }

        public async Task<bool> GetParentCompanyGuarantee(Guid organisationId)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                const string sql = @"select CASE WHEN isnull(JSON_Value(OrganisationData,'$.ParentCompanyGuarantee'),'false') = 'false'
                                    THEN 0
                                    ELSE 1
                                    END
                                    FROM[Organisations] 
                                    WHERE Id = @organisationId";
                return await connection.ExecuteScalarAsync<bool>(sql, new { organisationId });
            }
        }

        public async Task<bool> GetFinancialTrackRecord(Guid organisationId)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                const string sql = @"select CASE WHEN isnull(JSON_Value(OrganisationData,'$.FinancialTrackRecord'),'false') = 'false'
                                     THEN 0
                                     ELSE 1
                                     END
                                     FROM[Organisations] 
                                     WHERE Id = @organisationId";
                return await connection.ExecuteScalarAsync<bool>(sql, new { organisationId });
            }
        }

        public async Task<int> GetProviderType(Guid organisationId)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                const string sql = "SELECT ProviderTypeId FROM [Organisations] WHERE Id = @organisationId";
                return await connection.ExecuteScalarAsync<int>(sql, new { organisationId });
            }
        }

        public async Task<DateTime?> GetStartDate(Guid organisationId)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                const string sql = "SELECT Json_value(organisationData,'$.StartDate') FROM [Organisations] WHERE Id = @organisationId";
                return await connection.ExecuteScalarAsync<DateTime?>(sql, new { organisationId });
            }
        }

        public async Task<int> GetOrganisationType(Guid organisationId)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                const string sql = "SELECT OrganisationTypeId FROM [Organisations] WHERE Id = @organisationId";
                return await connection.ExecuteScalarAsync<int>(sql, new { organisationId });
            }
        }

        public async Task<long> GetUkprn(Guid organisationId)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                const string sql = "select ukprn FROM [Organisations] WHERE Id = @organisationId";

                return await connection.ExecuteScalarAsync<long>(sql, new { organisationId });
            }
        }

        public async Task<string> GetCharityNumber(Guid organisationId)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                const string sql = "SELECT Json_value(organisationData,'$.CharityNumber') FROM [Organisations] WHERE Id = @organisationId";
                return await connection.ExecuteScalarAsync<string>(sql, new { organisationId });
            }
        }

        public async Task<OrganisationRegisterStatus> GetOrganisationRegisterStatus(string ukprn)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var sql = "SELECT 1 FROM Organisations WHERE UKPRN = @ukprn";

                var ukPrnOnRegister = await connection.QueryAsync<bool>(sql, new {ukprn});

                if (!ukPrnOnRegister.FirstOrDefault())
                {
                    return new OrganisationRegisterStatus
                    {
                        UkprnOnRegister = false
                    };
                }

                sql = "SELECT 1 AS UkprnOnRegister, Id AS [OrganisationId], ProviderTypeId, StatusId, StatusDate, "
                    + "JSON_VALUE(OrganisationData, '$.RemovedReason.Id') AS RemovedReasonId FROM Organisations " +
                      "WHERE UKPRN = @ukprn";

                var registerStatus = await connection.QueryAsync<OrganisationRegisterStatus>(sql, new {ukprn});

                return registerStatus.FirstOrDefault();

             }
        }
                
        public async Task<DateTime?> GetApplicationDeterminedDate(Guid organisationId)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                const string sql = "SELECT Json_value(organisationData,'$.ApplicationDeterminedDate') FROM [Organisations] WHERE Id = @organisationId";
                return await connection.ExecuteScalarAsync<DateTime?>(sql, new { organisationId });

            }
        }

        public async Task<IEnumerable<Engagement>> GetEngagements(GetEngagementsRequest request)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var sqlIfPaginated = request.SinceEventId.Equals(0) ? string.Empty : $@" where ose.Id >= @SinceEventId
                                            order by ose.Id
                                            OFFSET ((@PageNumber - 1) * @PageSize) rows
                                            fetch next @PageSize rows ONLY";

                var sql = $@"select ose.ID, ose.providerId, ose.CreatedOn, isnull(os.EventDescription, 'UNKNOWN') as Event
	                                        from OrganisationStatusEvent ose left join organisationStatus os
	                                        on os.Id = ose.OrganisationStatusId" + sqlIfPaginated;
                return await connection.QueryAsync<Engagement>(sql, new { request.SinceEventId, request.PageSize, request.PageNumber });
            }
        }
    }
}
