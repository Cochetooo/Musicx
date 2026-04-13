namespace Musicx.Contracts.Dto.Requests.User;

public sealed class InReviewSource : BaseInputModel
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Color { get; set; } = "#777777";
}