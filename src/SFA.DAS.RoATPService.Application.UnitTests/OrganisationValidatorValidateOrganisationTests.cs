using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Application.Validators;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Application.UnitTests
{
    [TestFixture]
    public class OrganisationValidatorValidateOrganisationTests
    {
        private OrganisationValidator _validator;
        private Mock<ILookupDataRepository> _lookupRepository;

        [SetUp]
        public void Before_each_test()
        {
            _lookupRepository = new Mock<ILookupDataRepository>();
            _lookupRepository.Setup(x=>x.GetProviderTypes()).ReturnsAsync(new List<ProviderType> {new ProviderType {Id=1,Description = "Main"}});
            _lookupRepository.Setup(x => x.GetOrganisationTypes()).ReturnsAsync(new List<OrganisationType> { new OrganisationType { Id = 1, Description = "Something" } });
            _validator = new OrganisationValidator(null, _lookupRepository.Object, null);
        }

        [Test]
        public void Organisation_Validation_checks_passes_validation()
        {
            var command = new UpdateOrganisationCommand
            {
                OrganisationId = Guid.NewGuid(),
                LegalName = "legal name",
                TradingName = null,
                ProviderTypeId = 1,
                ApplicationDeterminedDate = DateTime.Today,
                OrganisationTypeId = 1,
                CompanyNumber = "12345678",
                CharityNumber = null
            };
            var result = _validator.ValidateOrganisation(command);
            Assert.IsNull(result.Message);
        }

        [Test]
        public void Organisation_Validation_checks_fails_invalid_legal_name()
        {
            var command = new UpdateOrganisationCommand
            {
                OrganisationId = Guid.NewGuid(),
                LegalName = null,
                TradingName = null,
                ProviderTypeId = 1,
                ApplicationDeterminedDate = DateTime.Today,
                OrganisationTypeId = 1,
                CompanyNumber = "12345678",
                CharityNumber = null
            };
            var result = _validator.ValidateOrganisation(command);
            Assert.IsNotNull(result.Message);
            Assert.IsTrue(result.Message.ToLower().Contains("legal name"));
        }

        [Test]
        public void Organisation_Validation_checks_fails_invalid_trading_name()
        {
            var command = new UpdateOrganisationCommand
            {
                OrganisationId = Guid.NewGuid(),
                LegalName = "name",
                TradingName = new string('x',201),
                ProviderTypeId = 1,
                ApplicationDeterminedDate = DateTime.Today,
                OrganisationTypeId = 1,
                CompanyNumber = "12345678",
                CharityNumber = null
            };
            var result = _validator.ValidateOrganisation(command);
            Assert.IsNotNull(result.Message);
            Assert.IsTrue(result.Message.ToLower().Contains("trading name"));
        }

        [Test]
        public void Organisation_Validation_checks_fails_invalid_provider_type()
        {
            var command = new UpdateOrganisationCommand
            {
                OrganisationId = Guid.NewGuid(),
                LegalName = "name",
                TradingName = null,
                ProviderTypeId = 2,
                ApplicationDeterminedDate = DateTime.Today,
                OrganisationTypeId = 1,
                CompanyNumber = "12345678",
                CharityNumber = null
            };
            var result = _validator.ValidateOrganisation(command);
            Assert.IsNotNull(result.Message);
            Assert.IsTrue(result.Message.ToLower().Contains("provider type"));
        }

        [Test]
        public void Organisation_Validation_checks_fails_invalid_application_determined_date()
        {
            var command = new UpdateOrganisationCommand
            {
                OrganisationId = Guid.NewGuid(),
                LegalName = "name",
                TradingName = null,
                ApplicationDeterminedDate = DateTime.Today.AddDays(1),
                ProviderTypeId = 1,
                OrganisationTypeId = 1,
                CompanyNumber = "12345678",
                CharityNumber = null
            };
            var result = _validator.ValidateOrganisation(command);
            Assert.IsNotNull(result.Message);
            Assert.IsTrue(result.Message.ToLower().Contains("application determined date"));
        }

        [Test]
        public void Organisation_Validation_checks_fails_invalid_organisation_type()
        {
            var command = new UpdateOrganisationCommand
            {
                OrganisationId = Guid.NewGuid(),
                LegalName = "name",
                TradingName = null,
                ApplicationDeterminedDate = DateTime.Today,
                ProviderTypeId = 1,
                OrganisationTypeId = 2,
                CompanyNumber = "12345678",
                CharityNumber = null
            };
            var result = _validator.ValidateOrganisation(command);
            Assert.IsNotNull(result.Message);
            Assert.IsTrue(result.Message.ToLower().Contains("organisation type"));
        }

        [Test]
        public void Organisation_Validation_checks_fails_invalid_company_number()
        {
            var command = new UpdateOrganisationCommand
            {
                OrganisationId = Guid.NewGuid(),
                LegalName = "name",
                TradingName = null,
                ApplicationDeterminedDate = DateTime.Today,
                ProviderTypeId = 1,
                OrganisationTypeId = 1,
                CompanyNumber = "1234567",
                CharityNumber = null
            };
            var result = _validator.ValidateOrganisation(command);
            Assert.IsNotNull(result.Message);
            Assert.IsTrue(result.Message.ToLower().Contains("company number"));
        }

        [Test]
        public void Organisation_Validation_checks_fails_invalid_charity_number()
        {
            var command = new UpdateOrganisationCommand
            {
                OrganisationId = Guid.NewGuid(),
                LegalName = "name",
                TradingName = null,
                ApplicationDeterminedDate = DateTime.Today,
                ProviderTypeId = 1,
                OrganisationTypeId = 1,
                CompanyNumber = "12345678",
                CharityNumber = "1234"
            };
            var result = _validator.ValidateOrganisation(command);
            Assert.IsNotNull(result.Message);
            Assert.IsTrue(result.Message.ToLower().Contains("charity registration number"));
        }
    }
}
