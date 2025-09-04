namespace SFA.DAS.RoATPService.Application.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using FluentAssertions;
    using Handlers;
    using Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using SFA.DAS.RoATPService.Domain;

    [TestFixture]
    public class GetProviderTypesHandlerTests
    {
        private GetProviderTypesHandler _handler;
        private Mock<ILookupDataRepository> _repository;
        private Mock<ILogger<GetProviderTypesHandler>> _logger;

        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<GetProviderTypesHandler>>();
            _repository = new Mock<ILookupDataRepository>();
            var providerTypes = new List<ProviderType>
            {
                new ProviderType {Id = 1, Type = "Main provider"},
                new ProviderType {Id = 2, Type = "Employer provider"},
                new ProviderType {Id = 3, Type = "Supporting provider"}
            };
            _repository.Setup(x => x.GetProviderTypes()).ReturnsAsync(providerTypes);
            _handler = new GetProviderTypesHandler(_repository.Object, _logger.Object);
        }

        [Test]
        public void Handler_returns_list_of_provider_types()
        {
            var providerTypes = _handler.Handle(new GetProviderTypesRequest(), new CancellationToken()).Result;

            providerTypes.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task Handler_returns_exception_from_repository()
        {
            _repository.Setup(x => x.GetProviderTypes())
                .Throws(new Exception("Unit test exception"));

            Func<Task> result = () => _handler.Handle(new GetProviderTypesRequest(), new CancellationToken());
            await result.Should().ThrowAsync<ApplicationException>();
        }
    }
}
