using SFA.DAS.RoATPService.Application.Commands;

namespace SFA.DAS.RoATPService.Data
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using Dapper;
    using Application.Interfaces;
    using System.Threading.Tasks;
    using SFA.DAS.RoatpService.Data.DapperTypeHandlers;
    using SFA.DAS.RoATPService.Domain;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using SFA.DAS.RoATPService.Infrastructure.Interfaces;

    public class UpdateOrganisationRepository : IUpdateOrganisationRepository
    {
        private const string RoatpDateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        private readonly IDbConnectionHelper _dbConnectionHelper;

        public UpdateOrganisationRepository(IDbConnectionHelper dbConnectionHelper)
        {
            _dbConnectionHelper = dbConnectionHelper;
            SqlMapper.AddTypeHandler(typeof(OrganisationData), new OrganisationDataHandler());
        }



        public async Task<bool> UpdateOrganisation(UpdateOrganisationCommand command)
        {
            var organisationId = command.OrganisationId;
            var providerTypeId = command.ProviderTypeId;
            var organisationTypeId = command.OrganisationTypeId;
            var legalName = command.LegalName;
            var tradingName = command.TradingName;
            var companyNumber = command.CompanyNumber;
            var charityNumber = command.CharityNumber;
            var updatedBy = command.Username;
            var applicationDeterminedDateValue = command.ApplicationDeterminedDate.ToString(RoatpDateTimeFormat);

            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var updatedAt = DateTime.Now;

                const string sql = "update [Organisations] SET LegalName = @legalName,organisationTypeId=@organisationTypeId, providerTypeId=@providerTypeId, tradingName= @tradingName," +
                                   " OrganisationData = JSON_MODIFY(JSON_MODIFY(JSON_MODIFY(OrganisationData, '$.ApplicationDeterminedDate', @applicationDeterminedDateValue),'$.CompanyNumber',@companyNumber),'$.CharityNumber',@CharityNumber), " +
                                   " UpdatedBy = @updatedBy, UpdatedAt = @updatedAt " +
                                   " WHERE Id = @organisationId";
                int recordsAffected = await connection.ExecuteAsync(sql, new { legalName, organisationTypeId,providerTypeId,tradingName,companyNumber,
                                                            charityNumber, applicationDeterminedDateValue, updatedBy, updatedAt, organisationId });

                return recordsAffected > 0;
            }
        }

        public async Task<bool> UpdateLegalName(Guid organisationId, string legalName, string updatedBy)
        {            
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var updatedAt = DateTime.Now;

                const string sql = "update [Organisations] SET LegalName = @legalName, UpdatedBy = @updatedBy, UpdatedAt = @updatedAt " +
                                   "WHERE Id = @organisationId";
                int recordsAffected = await connection.ExecuteAsync(sql, new { legalName, updatedBy, updatedAt, organisationId });

                return recordsAffected > 0;
            }
        }

        public async Task<bool> UpdateTradingName(Guid organisationId, string tradingName, string updatedBy)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var updatedAt = DateTime.Now;

                const string sql = "update [Organisations] SET TradingName = @tradingName, UpdatedBy = @updatedBy, UpdatedAt = @updatedAt " +
                                   "WHERE Id = @organisationId";
                var recordsAffected = await connection.ExecuteAsync(sql, new { tradingName, updatedBy, updatedAt, organisationId });

                return recordsAffected > 0;
            }
        }

        public async Task<bool> UpdateStartDate(Guid organisationId, DateTime startDate, string updatedBy)
        {
            var startDateValue = startDate.ToString(RoatpDateTimeFormat);

            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var updatedAt = DateTime.Now;

                const string updateSql =
                    "update [Organisations] set OrganisationData = JSON_MODIFY(OrganisationData, '$.StartDate', @startDateValue), UpdatedBy = @updatedBy, UpdatedAt = @updatedAt " +
                    "WHERE Id = @organisationId";

                int recordsAffected = await connection.ExecuteAsync(updateSql, new { startDateValue, organisationId, updatedBy, updatedAt });

                return recordsAffected > 0;
            }
        }

        public async Task<bool> UpdateOrganisationStatus(Guid organisationId, int organisationStatusId, string updatedBy)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var updatedAt = DateTime.Now;

                const string sql = "update [Organisations] SET StatusId = @organisationStatusId, " +    
                                   "UpdatedBy = @updatedBy, UpdatedAt = @updatedAt, StatusDate = @updatedAt " +
                                   "WHERE Id = @organisationId";
                int recordsAffected = await connection.ExecuteAsync(sql, new { organisationStatusId, updatedBy, updatedAt, organisationId });

                return recordsAffected > 0;
            }
        }

        public async Task<bool> UpdateProviderType(Guid organisationId, int providerTypeId, string updatedBy)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var updatedAt = DateTime.Now;

                const string sql = "update [Organisations] SET ProviderTypeId = @providerTypeId, " +
                                   "UpdatedBy = @updatedBy, UpdatedAt = @updatedAt, StatusDate = @updatedAt " +
                                   "WHERE Id = @organisationId";
                int recordsAffected = await connection.ExecuteAsync(sql, new { providerTypeId, updatedBy, updatedAt, organisationId });

                return recordsAffected > 0;
            }
        }

        public async Task<bool> UpdateOrganisationType(Guid organisationId, int organisationTypeId, string updatedBy)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var updatedAt = DateTime.Now;

                const string sql = "update [Organisations] SET OrganisationTypeId = @organisationTypeId, " +
                                   "UpdatedBy = @updatedBy, UpdatedAt = @updatedAt, StatusDate = @updatedAt " +
                                   "WHERE Id = @organisationId";
                int recordsAffected = await connection.ExecuteAsync(sql, new { organisationTypeId, updatedBy, updatedAt, organisationId });

                return recordsAffected > 0;
            }
        }
        public async Task<RemovedReason> UpdateStatusWithRemovedReason(Guid organisationId, int organisationStatusId, int removedReasonId, string updatedBy)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var sql = "select Id, RemovedReason as [Reason], Status, Description, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy FROM [RemovedReasons] " +
                          "WHERE Id = @removedReasonId";
                var reason = await connection.QueryAsync<RemovedReason>(sql, new { removedReasonId });
                var removedReason = reason.FirstOrDefault();

                var reasonJson = JsonConvert.SerializeObject(removedReason,
                    new IsoDateTimeConverter() { DateTimeFormat = RoatpDateTimeFormat });

                var updatedAt = DateTime.Now;

                const string updateSql =
                    "update [Organisations] set OrganisationData = JSON_MODIFY(OrganisationData, '$.RemovedReason', JSON_QUERY(@reasonJson)), " +
                    "StatusId = @organisationStatusId, UpdatedBy = @updatedBy, UpdatedAt = @updatedAt, StatusDate = @updatedAt " +
                    "WHERE Id = @organisationId";

                var recordsAffected = await connection.ExecuteAsync(updateSql,
                    new { reasonJson, organisationStatusId, updatedBy, updatedAt, organisationId });

                if (recordsAffected > 0)
                {
                    sql = $@"INSERT INTO [dbo].[OrganisationStatusEvent]
                                    ([OrganisationStatusId]
                                    ,[CreatedOn]
                                    ,[ProviderId]) 
                                    VALUES (@organisationStatusId, @updatedAt, (select top 1 ukprn from organisations where  id=@organisationId))";

                    await connection.ExecuteAsync(sql,
                        new
                        {
                            organisationStatusId,
                            updatedAt,
                            organisationId
                        });
                }

                return removedReason;
            }
        }

        public async Task<bool> UpdateRemovedReason(Guid organisationId, int? removedReasonId, string updatedBy)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var sql = "select Id, RemovedReason as [Reason], Status, Description, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy FROM [RemovedReasons] " +
                          "WHERE Id = @removedReasonId";
                var reason = await connection.QueryAsync<RemovedReason>(sql, new { removedReasonId });
                var removedReason = reason.FirstOrDefault();

                var reasonJson = JsonConvert.SerializeObject(removedReason,
                    new IsoDateTimeConverter() { DateTimeFormat = RoatpDateTimeFormat });

                if (removedReason == null)
                    reasonJson = null;
                var updatedAt = DateTime.Now;

                const string updateSql =
                    "update [Organisations] set OrganisationData = JSON_MODIFY(OrganisationData, '$.RemovedReason', JSON_QUERY(@reasonJson)), " +
                    " UpdatedBy = @updatedBy, UpdatedAt = @updatedAt, StatusDate = @updatedAt " +
                    "WHERE Id = @organisationId";

                var recordsAffected = await connection.ExecuteAsync(updateSql,
                    new { reasonJson, updatedBy, updatedAt, organisationId });

                return recordsAffected > 0;
            }
        }

        public async Task<bool> UpdateCompanyNumber(Guid organisationId, string companyNumber, string updatedBy)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var updatedAt = DateTime.Now;

                const string sql = "update [Organisations] SET OrganisationData = JSON_MODIFY(OrganisationData,'$.CompanyNumber',@companyNumber), UpdatedBy = @updatedBy, UpdatedAt = @updatedAt " +
                                   "WHERE Id = @organisationId";
                int recordsAffected = await connection.ExecuteAsync(sql, new { companyNumber, updatedBy, updatedAt, organisationId });

                return recordsAffected > 0;
            }
        }

        public async Task<bool> UpdateParentCompanyGuarantee(Guid organisationId, bool parentCompanyGuarantee, string updatedBy)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var updatedAt = DateTime.Now;

                const string sql = "update [Organisations] SET OrganisationData = JSON_MODIFY(OrganisationData,'$.ParentCompanyGuarantee',@parentCompanyGuarantee), UpdatedBy = @updatedBy, UpdatedAt = @updatedAt " +
                                   "WHERE Id = @organisationId";
                int recordsAffected = await connection.ExecuteAsync(sql, new { parentCompanyGuarantee, updatedBy, updatedAt, organisationId });

                return recordsAffected > 0;
            }
        }

        public async Task<bool> UpdateCharityNumber(Guid organisationId, string charityNumber, string updatedBy)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var updatedAt = DateTime.Now;

                const string sql = "update [Organisations] SET OrganisationData = JSON_MODIFY(OrganisationData,'$.CharityNumber',@CharityNumber), UpdatedBy = @updatedBy, UpdatedAt = @updatedAt " +
                                   "WHERE Id = @organisationId";
                int recordsAffected = await connection.ExecuteAsync(sql, new { charityNumber, updatedBy, updatedAt, organisationId });

                return recordsAffected > 0;
            }
        }

        public async Task<bool> UpdateApplicationDeterminedDate(Guid organisationId, DateTime applicationDeterminedDate, string updatedBy)
        {
            var applicationDeterminedDateValue = applicationDeterminedDate.ToString(RoatpDateTimeFormat);

            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var updatedAt = DateTime.Now;

                const string updateSql =
                    "update [Organisations] set OrganisationData = JSON_MODIFY(OrganisationData, '$.ApplicationDeterminedDate', @applicationDeterminedDateValue), UpdatedBy = @updatedBy, UpdatedAt = @updatedAt " +
                    "WHERE Id = @organisationId";

                int recordsAffected = await connection.ExecuteAsync(updateSql, new { applicationDeterminedDateValue, organisationId, updatedBy, updatedAt });

                return recordsAffected > 0;
            }
        }

        public async Task<bool> UpdateFinancialTrackRecord(Guid organisationId, bool financialTrackRecord, string updatedBy)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var updatedAt = DateTime.Now;

                const string sql = "update [Organisations] SET OrganisationData = JSON_MODIFY(OrganisationData,'$.FinancialTrackRecord',@financialTrackRecord), UpdatedBy = @updatedBy, UpdatedAt = @updatedAt " +
                                   "WHERE Id = @organisationId";
                var recordsAffected = await connection.ExecuteAsync(sql, new { financialTrackRecord, updatedBy, updatedAt, organisationId });

                return recordsAffected > 0;
            }
        }

        public async Task<bool> UpdateUkprn(Guid organisationId, long ukprn, string updatedBy) 
		{
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
				var updatedAt = DateTime.Now;

                const string sql = "update [Organisations] SET UKPRN = @ukprn, UpdatedBy = @updatedBy, UpdatedAt = @updatedAt " +
                                   "WHERE Id = @organisationId";
                int recordsAffected = await connection.ExecuteAsync(sql, new { ukprn, updatedBy, updatedAt, organisationId });

                return recordsAffected > 0;
			}
		}

 
        public async Task<bool> UpdateProviderTypeAndOrganisationType(Guid organisationId, int providerTypeId, int organisationTypeId, string updatedBy)
		{
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var updatedAt = DateTime.Now;

                const string sql = "update [Organisations] SET ProviderTypeId = @providerTypeId, " +
                          "OrganisationTypeId = @organisationTypeId, " +
                          "UpdatedBy = @updatedBy, UpdatedAt = @updatedAt " +
                          "WHERE Id = @organisationId";
                int recordsAffected = await connection.ExecuteAsync(sql, new { providerTypeId, organisationTypeId, updatedBy, updatedAt, organisationId });

                return recordsAffected > 0;
            }
        }

        public async Task<bool> WriteFieldChangesToAuditLog(AuditData auditFieldChanges)
        {
            if (!auditFieldChanges.FieldChanges.Any())
            {
                return false;
            }

            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                const string sql = "INSERT INTO Audit " +
                                   "([OrganisationId], [UpdatedBy], [UpdatedAt], [AuditData]) " +
                                   "VALUES(@organisationId, @updatedBy, @updatedAt, @auditData)";

                var updatedAt = DateTime.Now;
                var auditData = JsonConvert.SerializeObject(auditFieldChanges);
                var recordsAffected = await connection.ExecuteAsync(sql,
                    new
                    {
                        auditFieldChanges.OrganisationId,
                        auditFieldChanges.UpdatedBy,
                        updatedAt,
                        auditData
                    });

                return recordsAffected > 0;
            }
        }
    }
}
