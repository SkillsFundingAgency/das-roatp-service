using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.RoATPService.Data;
using SFA.DAS.RoatpService.Data.IntegrationTests.Handlers;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Tests
{
    public class FatDataExportTests : TestBase
    {
        private FatDataExportRepository _repository;

        [OneTimeSetUp]
        public void setup()
        {
            OrganisationStatusHandler.InsertRecords(
                new List<OrganisationStatusModel>
                {
                    new OrganisationStatusModel { Id = OrganisationStatusHandler.Active, Status = "Active", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" },
                    new OrganisationStatusModel { Id = OrganisationStatusHandler.Removed, Status = "Removed", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" },
                    new OrganisationStatusModel { Id = OrganisationStatusHandler.ActiveNotTakingOnApprentices, Status = "Active - but not taking on apprentices", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" },
                    new OrganisationStatusModel { Id = OrganisationStatusHandler.Onboarding, Status = "On-boarding", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" }
                });
            OrganisationTypeHandler.InsertRecords(
                new List<OrganisationTypeModel>
                {
                    new OrganisationTypeModel { Id = 1, Status = "1", Type = "1", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" },
                    new OrganisationTypeModel { Id = 2, Status = "2", Type = "2", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" },
                    new OrganisationTypeModel { Id = 3, Status = "3", Type = "3", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" },
                    new OrganisationTypeModel { Id = 4, Status = "4", Type = "4", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" }
                });
            ProviderTypeHandler.InsertRecords(
                new List<ProviderTypeModel>
                {
                    new ProviderTypeModel {Id = 1, Status = "1", ProviderType = "1", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" },
                    new ProviderTypeModel {Id = 2, Status = "2", ProviderType = "2", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" },
                    new ProviderTypeModel {Id = 3, Status = "3", ProviderType = "3", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" },
                    new ProviderTypeModel {Id = 4, Status = "4", ProviderType = "4", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" }
                });

            _repository = new FatDataExportRepository(new DatabaseService().WebConfiguration);
        }

        [Test]
        public void ExpectedLatestDateIsReturnedWithUpdatedTakingPrecedence()
        {
            var organisations = new List<OrganisationModel>
            {
                new OrganisationModel
                {
                    UKPRN = 23423,
                    ProviderTypeId = 1,
                    OrganisationTypeId = 1,
                    StatusId = OrganisationStatusHandler.Removed,
                    StatusDate = DateTime.Today.AddDays(-10),
                    LegalName = "Provider 1",
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    CreatedBy = "Test"
                },
                new OrganisationModel
                {
                    UKPRN = 54675,
                    ProviderTypeId = 2,
                    OrganisationTypeId = 2,
                    StatusId = OrganisationStatusHandler.Active,
                    StatusDate = DateTime.Today.AddDays(-10),
                    LegalName = "Provider 2",
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    CreatedBy = "Test"
                },
                new OrganisationModel
                {
                    UKPRN = 79878,
                    ProviderTypeId = 3,
                    OrganisationTypeId = 3,
                    StatusId = OrganisationStatusHandler.ActiveNotTakingOnApprentices,
                    StatusDate = DateTime.Today.AddDays(-10),
                    LegalName = "Provider 3",
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    CreatedBy = "Test"
                },
                new OrganisationModel
                {
                    UKPRN = 3784,
                    ProviderTypeId = 4,
                    OrganisationTypeId = 4,
                    StatusId = OrganisationStatusHandler.Onboarding,
                    StatusDate = DateTime.Today.AddDays(-10),
                    LegalName = "Provider 4",
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    CreatedBy = "Test"
                }
            };
            OrganisationHandler.InsertRecords(organisations);

            var dataExportResult = _repository.GetFatDataExport().Result.ToList();
            dataExportResult.Count.Should().Be(organisations.Count);
            foreach (var organisation in organisations)
            {
                dataExportResult.Should().Contain(obj =>
                    obj.ToString().Contains(
                        $"UKPRN = '{organisation.UKPRN}', " +
                        $"StatusDate = '{organisation.StatusDate}', " +
                        $"StatusId = '{organisation.StatusId}', " +
                        $"OrganisationTypeId = '{organisation.OrganisationTypeId}', " +
                        $"ProviderTypeId = '{organisation.ProviderTypeId}'"));
            }
        }

        [OneTimeTearDown]
        public void tear_down()
        {
            OrganisationHandler.DeleteAllRecords();
            OrganisationStatusHandler.DeleteAllRecords();
            OrganisationTypeHandler.DeleteAllRecords();
            ProviderTypeHandler.DeleteAllRecords();
        }
    }
}
