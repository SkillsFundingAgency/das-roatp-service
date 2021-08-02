namespace SFA.DAS.RoATPService.Data
{
    using System;
    using System.Threading.Tasks;
    using Dapper;
    using SFA.DAS.RoATPService.Application.Interfaces;
    using SFA.DAS.RoATPService.Infrastructure.Interfaces;

    public class EventsRepository : IEventsRepository
    {
        private readonly IDbConnectionHelper _dbConnectionHelper;

        public EventsRepository(IDbConnectionHelper dbConnectionHelper)
        {
            _dbConnectionHelper = dbConnectionHelper;
        }

        public async Task<bool> AddOrganisationStatusEvents(long ukprn, int organisationStatusId, DateTime createdOn)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var sql = $@"INSERT INTO [dbo].[OrganisationStatusEvent]
                                    ([OrganisationStatusId]
                                    ,[CreatedOn]
                                    ,[ProviderId]) " +
                          "VALUES " +
                          "(@organisationStatusId, @createdOn, @ukprn)";

                var eventsCreated = await connection.ExecuteAsync(sql,
                    new
                    {
                        organisationStatusId,
                        createdOn,
                        ukprn
                    });

                return eventsCreated > 0;
            }
        }

        public async Task<bool> AddOrganisationStatusEventsFromOrganisationId(Guid organisationId, int organisationStatusId, DateTime createdOn)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var sql = $@"INSERT INTO [dbo].[OrganisationStatusEvent]
                                    ([OrganisationStatusId]
                                    ,[CreatedOn]
                                    ,[ProviderId]) " +
                          "VALUES " +
                          "(@organisationStatusId, @createdOn, (select top 1 ukprn from organisations where  id=@organisationId))";

                var eventsCreated = await connection.ExecuteAsync(sql,
                    new
                    {
                        organisationStatusId,
                        createdOn,
                        organisationId
                    });

                return eventsCreated > 0;
            }
        }
    }
}
