namespace Musicx.Contracts.Dto.Responses;

public sealed class OutTag : BaseOutputModel
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}