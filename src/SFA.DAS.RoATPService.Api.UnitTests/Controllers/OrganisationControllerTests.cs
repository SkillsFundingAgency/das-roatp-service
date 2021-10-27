using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Application.Api.Controllers;

namespace SFA.DAS.RoATPService.Api.UnitTests.Controllers
{
    [TestFixture]
    public class OrganisationControllerTests
    {
        private Mock<IMediator> _mediator;
        private Mock<ILogger<OrganisationController>> _logger;
        private OrganisationController _controller;
        [SetUp]
        public void SetUp()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<OrganisationController>>();
            _controller = new OrganisationController(_logger.Object,_mediator.Object);
        }

        [Test]
        public async Task When_Calling_update_mediator_invokes_handler()
        {
            _mediator.Setup(x => x.Send(It.IsAny<UpdateOrganisationRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            await _controller.Update(new UpdateOrganisationRequest());
            _mediator.Verify(x=>x.Send(It.IsAny<UpdateOrganisationRequest>(),It.IsAny<CancellationToken>()),Times.Once);

        }
    }
}
