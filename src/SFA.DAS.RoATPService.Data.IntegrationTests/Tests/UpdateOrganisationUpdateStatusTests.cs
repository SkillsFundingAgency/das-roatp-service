﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.RoatpService.Data.IntegrationTests.Handlers;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;
using SFA.DAS.RoATPService.Data;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Tests
{
    public class UpdateOrganisationUpdateStatusTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private UpdateOrganisationRepository _updateOrganisationRepository;
        private OrganisationRepository _repository;
        private OrganisationStatusModel _status1;
        private OrganisationStatusModel _status2;
        private int _organisationStatusId1;
        private int _organisationStatusId2;
        private int _newOrganisationStatusId;
        private ProviderTypeModel _providerType1;
        private int _providerTypeId1;
        private OrganisationTypeModel _organisationTypeModel1;
        private int _organisationTypeId1;
        private OrganisationModel _organisation;
        private long _organisationUkprn;
        private string _legalName;
        private Guid _organisationId;
        private string _changedBy;
        private bool _successfulUpdate;
    
        [OneTimeSetUp]
        public void Set_up_and_run_update()
        {
            _organisationStatusId1 = 1;
            _organisationStatusId2 = 2;
            _providerTypeId1 = 10;
            _organisationTypeId1 = 100;
            _organisationUkprn = 11114433;
            _legalName = "Legal name 1";
            _organisationId = Guid.NewGuid();
            _updateOrganisationRepository = new UpdateOrganisationRepository(_databaseService.DbConnectionHelper);
            _repository = new OrganisationRepository(_databaseService.DbConnectionHelper);
            _status1 = new OrganisationStatusModel { Id = _organisationStatusId1, Status = "Live", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" };
            OrganisationStatusHandler.InsertRecord(_status1);
            _status2 = new OrganisationStatusModel { Id = _organisationStatusId2, Status = "Live", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" };
            OrganisationStatusHandler.InsertRecord(_status2);
            _providerType1 = new ProviderTypeModel { Id = _providerTypeId1, ProviderType = "provider type 10", Description = "provider type description", CreatedAt = DateTime.Now, CreatedBy = "TestSystem", Status = "Live" };
            ProviderTypeHandler.InsertRecord(_providerType1);
            _organisationTypeModel1 = new OrganisationTypeModel { Id = _organisationTypeId1, Type = "organisation type 10", Description = "organisation type description", CreatedAt = DateTime.Now, CreatedBy = "TestSystem", Status = "Live" };
            OrganisationTypeHandler.InsertRecord(_organisationTypeModel1);
            _organisation = new OrganisationModel
            {
                UKPRN = _organisationUkprn,
                OrganisationTypeId = _organisationTypeId1,
                ProviderTypeId = _providerTypeId1,
                StatusId = _organisationStatusId1,
                StatusDate = DateTime.Today.AddDays(5),
                LegalName = _legalName,
                Id = _organisationId,
                CreatedAt = DateTime.Now,
                CreatedBy = "Test"
            };
            OrganisationHandler.InsertRecord(_organisation);
            _changedBy = "SystemChange";

            _successfulUpdate = _updateOrganisationRepository.UpdateOrganisationStatus(_organisationId, _organisationStatusId2, _changedBy).Result;
            _newOrganisationStatusId = _repository.GetOrganisationStatus(_organisationId).Result;
        }

        [Test]
        public void Update_was_marked_successful()
        {
            Assert.AreEqual(true, _successfulUpdate);
        }

        [Test]
        public void Organisation_has_new_status()
        {
            Assert.AreEqual(_organisationStatusId2, _newOrganisationStatusId);
        }

        [Test]
        public void Updated_by_is_correct()
        {
            var changedOrganisation = OrganisationHandler.GetOrganisationFromId(_organisationId);
            Assert.AreEqual(_changedBy, changedOrganisation.UpdatedBy);
        }

        [OneTimeTearDown]
        public void Tear_down()
        {
            OrganisationHandler.DeleteAllRecords();
            OrganisationTypeHandler.DeleteRecord(_organisationTypeId1);
            ProviderTypeHandler.DeleteAllRecords();
            OrganisationStatusHandler.DeleteRecords(new List<int> { _status1.Id });
        }
    }
}
