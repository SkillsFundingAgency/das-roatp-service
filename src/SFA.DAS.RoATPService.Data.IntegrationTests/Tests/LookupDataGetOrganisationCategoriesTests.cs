﻿using System;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.RoatpService.Data.IntegrationTests.Handlers;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;
using SFA.DAS.RoATPService.Data;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Tests
{    public class LookupDataGetOrganisationCategoriesTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private LookupDataRepository _lookupRepository;

        private int _providerTypeId2;
        private int _providerTypeId1;
        private int _organisationTypeId1WithProviderTypeId1CategoryId1;
        private int _organisationTypeId4WithProviderTypeId2;
        private int _organisationTypeId2WithProviderTypeId1CategoryId1;
        private int _organisationTypeId3WithProviderTypeId1CategoryId1;
        private int _organisationTypeId3WithProviderTypeId2CategoryId1;

        private int _categoryId1;
        private int _categoryId2;
         private int _categoryId3;

        [OneTimeSetUp]
        public void Before_the_tests()
        {
            _lookupRepository = new LookupDataRepository(_databaseService.DbConnectionHelper, null);

            _providerTypeId1 = 1;
            _providerTypeId2 = 2;
            _organisationTypeId1WithProviderTypeId1CategoryId1 = 10;
            _organisationTypeId2WithProviderTypeId1CategoryId1 = 20;
            _organisationTypeId3WithProviderTypeId1CategoryId1 = 30;
            _organisationTypeId3WithProviderTypeId2CategoryId1 = 40;
            _organisationTypeId4WithProviderTypeId2 = 100;
            _categoryId1 = 1;
            _categoryId2 = 2;
            _categoryId3 = 3;


            OrganisationTypeHandler.InsertRecord(new OrganisationTypeModel { Id = _organisationTypeId1WithProviderTypeId1CategoryId1, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", Type = "a" });
            OrganisationTypeHandler.InsertRecord(new OrganisationTypeModel { Id = _organisationTypeId2WithProviderTypeId1CategoryId1, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", Type = "b" });
            OrganisationTypeHandler.InsertRecord(new OrganisationTypeModel { Id = _organisationTypeId3WithProviderTypeId1CategoryId1, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", Type = "c" });
            OrganisationTypeHandler.InsertRecord(new OrganisationTypeModel { Id = _organisationTypeId3WithProviderTypeId2CategoryId1, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", Type = "c" });
            OrganisationTypeHandler.InsertRecord(new OrganisationTypeModel { Id = _organisationTypeId4WithProviderTypeId2, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", Type = "d" });
            ProviderTypeHandler.InsertRecord(new ProviderTypeModel { Id = _providerTypeId1, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", ProviderType = "a" });
            ProviderTypeHandler.InsertRecord(new ProviderTypeModel { Id = _providerTypeId2, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", ProviderType = "b" });
            OrganisationCategoryHandler.InsertRecord(new OrganisationCategoryModel { Id = _categoryId1, Category = "category 1", CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x" });
            OrganisationCategoryHandler.InsertRecord(new OrganisationCategoryModel { Id = _categoryId2, Category = "category 2", CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x" });
            OrganisationCategoryHandler.InsertRecord(new OrganisationCategoryModel { Id = _categoryId3, Category = "category 3", CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x" });
            OrganisationCategoryOrgTypeProviderTypeHandler.InsertRecord(new OrganisationCategoryOrgTypeProviderTypeModel { Id = 1, OrganisationTypeId = _organisationTypeId1WithProviderTypeId1CategoryId1, ProviderTypeId = _providerTypeId1, OrganisationCategoryId = _categoryId1, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x" });
            OrganisationCategoryOrgTypeProviderTypeHandler.InsertRecord(new OrganisationCategoryOrgTypeProviderTypeModel { Id = 2, OrganisationTypeId = _organisationTypeId2WithProviderTypeId1CategoryId1, ProviderTypeId = _providerTypeId1, OrganisationCategoryId = _categoryId1, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x" });
            OrganisationCategoryOrgTypeProviderTypeHandler.InsertRecord(new OrganisationCategoryOrgTypeProviderTypeModel { Id = 3, OrganisationTypeId = _organisationTypeId3WithProviderTypeId1CategoryId1, ProviderTypeId = _providerTypeId1, OrganisationCategoryId = _categoryId1, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x" });
            OrganisationCategoryOrgTypeProviderTypeHandler.InsertRecord(new OrganisationCategoryOrgTypeProviderTypeModel { Id = 4, OrganisationTypeId = _organisationTypeId3WithProviderTypeId2CategoryId1, ProviderTypeId = _providerTypeId2, OrganisationCategoryId = _categoryId1, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x" });
            OrganisationCategoryOrgTypeProviderTypeHandler.InsertRecord(new OrganisationCategoryOrgTypeProviderTypeModel { Id = 5, OrganisationTypeId = _organisationTypeId3WithProviderTypeId2CategoryId1, ProviderTypeId = _providerTypeId1, OrganisationCategoryId = _categoryId2, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x" });
            OrganisationCategoryOrgTypeProviderTypeHandler.InsertRecord(new OrganisationCategoryOrgTypeProviderTypeModel { Id = 6, OrganisationTypeId = _organisationTypeId3WithProviderTypeId2CategoryId1, ProviderTypeId = _providerTypeId1, OrganisationCategoryId = _categoryId3, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x" });


        }

        [TestCase(1,3)]
        [TestCase(2,1)]
        [TestCase(3,0)]
        public void Get_organisation_categories_by_providerId(int providerTypeId, int numberOfExpectedResults)
        {
            var result = _lookupRepository.GetOrganisationCategories(providerTypeId).Result;
            Assert.AreEqual(numberOfExpectedResults, result.Count());
        }

        [OneTimeTearDown]
        public void Tear_down()
        {
            OrganisationCategoryOrgTypeProviderTypeHandler.DeleteAllRecords();
            OrganisationCategoryHandler.DeleteAllRecords();
            ProviderTypeHandler.DeleteAllRecords();
            OrganisationTypeHandler.DeleteAllRecords();
        }
    }
}
