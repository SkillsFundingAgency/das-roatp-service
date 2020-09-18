using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.RoATPService.Domain.Models.FatDataExport;

namespace SFA.DAS.RoATPService.Domain.UnitTests.Models.FataDataExportTests
{
    public class WhenMappingFataDataExportDtoToFatDataExport
    {
        [Test, AutoData]
        public void Then_The_Fields_Are_Mapped(FatDataExportDto source)
        {
            var actual = (FatDataExport) source;

            actual.Should().BeEquivalentTo(source);
        }
    }
}