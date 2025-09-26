namespace SFA.DAS.RoATPService.Application.Types;
public record IdDescriptionPair(int Id, string Description)
{
    public static implicit operator IdDescriptionPair(Domain.Entities.OrganisationType source)
         => source == null ? null : new(source.Id, source.Type);
}
