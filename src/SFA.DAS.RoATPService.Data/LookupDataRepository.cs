namespace SFA.DAS.RoATPService.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Application.Interfaces;
    using Dapper;
    using SFA.DAS.RoATPService.Domain;
    using System.Linq;
    using SFA.DAS.RoATPService.Data.Helpers;
    using SFA.DAS.RoATPService.Infrastructure.Interfaces;

    public class LookupDataRepository : ILookupDataRepository
    {
        private readonly IDbConnectionHelper _dbConnectionHelper;
        private readonly ICacheHelper _cacheHelper;

        public LookupDataRepository(IDbConnectionHelper dbConnectionHelper, ICacheHelper cacheHelper)
        {
            _dbConnectionHelper = dbConnectionHelper;
            _cacheHelper = cacheHelper;
        }

        public async Task<IEnumerable<ProviderType>> GetProviderTypes()
        {
            var results = _cacheHelper.Get<ProviderType>();

            if (results != null)
            {
                return results.ToList();
            }

            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var sql = $"select Id, ProviderType AS [Type], Description, " +
                          "CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, Status " +
                          "from [ProviderTypes] " +
                          "order by Id";

                var providerTypes = await connection.QueryAsync<ProviderType>(sql);
                _cacheHelper.Cache(providerTypes);

                return providerTypes;
            }
        }

        public async Task<ProviderType> GetProviderType(int providerTypeId)
        {
            var results = _cacheHelper.Get<ProviderType>();
            if (results != null)
            {
                return results.FirstOrDefault(x => x.Id == providerTypeId);
            }

            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var sql = $"select Id, ProviderType AS [Type], Description, " +
                          "CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, Status " +
                          "from [ProviderTypes] " +
                          "where id = @providerTypeId";

                var providerTypes = await connection.QueryAsync<ProviderType>(sql, new {providerTypeId});
              
                return providerTypes.FirstOrDefault();
            }
        }

        public async Task<IEnumerable<OrganisationType>> GetOrganisationTypes()
        {
            var results = _cacheHelper.Get<OrganisationType>();

            if (results != null)
            {
                return results;
            }

            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var sql =
                    $"select ot.Id, ot.Type, ot.Description, ot.CreatedBy, ot.CreatedAt, ot.UpdatedBy, ot.UpdatedAt, ot.Status "
                    + "from [OrganisationTypes] ot " +
                    "order by Id";

                var organisationTypes = await connection.QueryAsync<OrganisationType>(sql);
                _cacheHelper.Cache(organisationTypes);

                return organisationTypes;
            }
        }

        public async Task<OrganisationType> GetOrganisationType(int organisationTypeId)
        {
            var types = await GetOrganisationTypes();
            return types.FirstOrDefault(x => x.Id == organisationTypeId);
        }

        public async Task<IEnumerable<OrganisationStatus>> GetOrganisationStatuses()
        {
            var results = _cacheHelper.Get<OrganisationStatus>();

            if (results != null)
            {
                return results;
            }

            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var sql = "select * FROM [OrganisationStatus] " +
                          "order by Id";

                var organisationStatuses = await connection.QueryAsync<OrganisationStatus>(sql);
                _cacheHelper.Cache(organisationStatuses);

                return organisationStatuses;
            }
        }


        public async Task<OrganisationStatus> GetOrganisationStatus(int statusId)
        {
            var statuses = await GetOrganisationStatuses();
            return statuses.FirstOrDefault(x => x.Id == statusId);
        }


        public async Task<IEnumerable<RemovedReason>> GetRemovedReasons()
        {
            var results = _cacheHelper.Get<RemovedReason>();
            if (results != null)
            {
                return results;
            }
  
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var sql = "select Id, RemovedReason as [Reason], Status, Description, CreatedAt, " +
                          "CreatedBy, UpdatedAt, UpdatedBy FROM [RemovedReasons] " +
                          "ORDER BY Id";

                var removedReasons = await connection.QueryAsync<RemovedReason>(sql);
                _cacheHelper.Cache(removedReasons);

                return removedReasons;
            }
        }

        public async Task<RemovedReason> GetRemovedReason(int removedReasonId)
        {
            var removedReasons = await GetRemovedReasons();
            return removedReasons.FirstOrDefault(x => x.Id == removedReasonId);
        }

        public async Task<IEnumerable<ProviderTypeOrganisationType>> GetProviderTypeOrganisationTypes()
        {
            var results = _cacheHelper.Get<ProviderTypeOrganisationType>();
            if (results != null)
            {
                return results;
            }

            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var sql = "select Id, ProviderTypeId, OrganisationTypeId, CreatedAt, " +
                          "CreatedBy, UpdatedAt, UpdatedBy FROM [ProviderTypeOrganisationTypes] " +
                          "ORDER BY Id";

                var providerTypeOrganisationTypes = await connection.QueryAsync<ProviderTypeOrganisationType>(sql);
                _cacheHelper.Cache(providerTypeOrganisationTypes);

                return providerTypeOrganisationTypes;
            }
        }

        public async Task<IEnumerable<ProviderTypeOrganisationStatus>> GetProviderTypeOrganisationStatuses()
        {
            var results = _cacheHelper.Get<ProviderTypeOrganisationStatus>();
            if (results != null)
            {
                return results;
            }

            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var sql = "select Id, ProviderTypeId, OrganisationStatusId, CreatedAt, " +
                          "CreatedBy, UpdatedAt, UpdatedBy FROM [ProviderTypeOrganisationStatus] " +
                          "ORDER BY Id";

                var providerTypeOrganisationStatuses = await connection.QueryAsync<ProviderTypeOrganisationStatus>(sql);
                _cacheHelper.Cache(providerTypeOrganisationStatuses);

                return providerTypeOrganisationStatuses;
            }
        }

        public async Task<IEnumerable<OrganisationType>> GetOrganisationTypesForProviderTypeId(int? providerTypeId)
        {
            var organisationTypes = await GetOrganisationTypes();
            if (providerTypeId == null)
            {
                return organisationTypes;
            }

            var providerTypeOrganisationTypes = await GetProviderTypeOrganisationTypes();

            var selectedproviderTypeOrganisationTypes =
                providerTypeOrganisationTypes.Where(x => x.ProviderTypeId == providerTypeId);

            return organisationTypes.Where(
                x => selectedproviderTypeOrganisationTypes.Any(z => z.OrganisationTypeId == x.Id));
        }

        public async Task<IEnumerable<OrganisationType>> GetOrganisationTypesForProviderTypeIdCategoryId(
            int providerTypeId, int categoryId)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var sql = $@"select id, Type,Description, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, Status
                                    from organisationTypes where id in
                                    (select organisationTypeId from OrganisationCategoryOrgTypeProviderType 
                                    where ProviderTypeId = @providerTypeId and OrganisationCategoryId = @categoryId)
                                            order by ID";

                var organisationTypes =
                    await connection.QueryAsync<OrganisationType>(sql, new {providerTypeId, categoryId});

                return organisationTypes;
            }
        }

        public async Task<IEnumerable<int>> GetValidOrganisationCategoryIds()
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var sql = $@"select id from OrganisationCategory";

                var validCategoryIds = await connection.QueryAsync<int>(sql);

                return validCategoryIds;
            }
        }

        public async Task<IEnumerable<OrganisationStatus>>
            GetOrganisationStatusesForProviderTypeId(int? providerTypeId)
            {
                var organisationStatuses = await GetOrganisationStatuses();
                if (providerTypeId == null)
                    return organisationStatuses;

                var providerTypeOrganisationStatuses = await GetProviderTypeOrganisationStatuses();
                var selectedProviderTypeOrganisationStatuses =
                    providerTypeOrganisationStatuses.Where(x => x.ProviderTypeId == providerTypeId);

                return organisationStatuses.Where(
                    x => selectedProviderTypeOrganisationStatuses.Any(z => z.OrganisationStatusId == x.Id));
            }

            public async Task<IEnumerable<OrganisationCategory>> GetOrganisationCategories(int providerTypeId)
            {
                using (var connection = _dbConnectionHelper.GetDatabaseConnection())
                {
                    var sql =
                        $@"select id,category, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, Status from organisationCategory where id in
                            (select distinct OrganisationCategoryId from OrganisationCategoryOrgTypeProviderType where ProviderTypeId = @providerTypeId)";

                    var organisationTypes =
                        await connection.QueryAsync<OrganisationCategory>(sql, new {providerTypeId});

                    return organisationTypes;
                }
            }
        }
    }
