namespace Musicx.Contracts.Dto.Requests;

public sealed class InPermission : BaseInputModel
{
    public string? Name { get; set; } = string.Empty;
}