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
    public class OrganisationValidatorIsValidUpdateOrganisationTests
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
        public void Organisation_Validation_checks()
        {
            var command = new UpdateOrganisationCommand();
            command.LegalName = "legal name";
            command.TradingName = null;
            command.ProviderTypeId = 1;
            command.ApplicationDeterminedDate = DateTime.Today;
            command.OrganisationTypeId = 1;
            command.CompanyNumber = "12345678";
            command.CharityNumber = null;
            var result = _validator.IsValidUpdateOrganisation(command);
            Assert.AreEqual(result, true);
        }
    }
}
