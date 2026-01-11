namespace Musicx.Contracts.Dto.Requests.Security;

public sealed class InPermission : BaseInputModel
{
    public string? Name { get; set; } = string.Empty;
}