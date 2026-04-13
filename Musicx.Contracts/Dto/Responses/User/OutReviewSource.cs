namespace Musicx.Contracts.Dto.Responses.User;

public sealed class OutReviewSource : BaseOutputModel
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Color { get; set; } = "#7E57C2";
}