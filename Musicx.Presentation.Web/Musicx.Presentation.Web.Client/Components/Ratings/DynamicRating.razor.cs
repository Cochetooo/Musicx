using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Musicx.Contracts.Enums;

namespace Musicx.Presentation.Web.Client.Components.Ratings;

public partial class DynamicRating
{
    [Parameter] public RatingMode RatingMode { get; set; }
    [Parameter] public EventCallback<int?> ValueChanged { get; set; }
    [Parameter] public int? Value { get; set; }

    private bool _isOpen;
    private bool _isEditing;
    private int? _editingValue;

    private bool IsTextualMode =>
        RatingMode == RatingMode.TextualShort || RatingMode == RatingMode.TextualDetailed;
    
    private bool IsNumericMode =>
        RatingMode == RatingMode.OutOfFive || RatingMode == RatingMode.OutOfTen
            || RatingMode == RatingMode.OutOfFifty || RatingMode == RatingMode.Percentage
            || RatingMode == RatingMode.OutOfThousand || RatingMode == RatingMode.OutOfTwenty;

    private int MaxValue => RatingMode switch
    {
        RatingMode.OutOfFive => 5,
        RatingMode.OutOfTen => 10,
        RatingMode.OutOfTwenty => 20,
        RatingMode.OutOfFifty => 50,
        RatingMode.Percentage => 100,
        RatingMode.OutOfThousand => 1000,
        _ => 100,
    };
    
    private void ToggleCollapse() => _isOpen = !_isOpen;

    private void SetTextual(TextualRating rating)
    {
        Value = MapTextual(rating);
        ValueChanged.InvokeAsync(Value);
        _isOpen = false;
    }

    private void CancelEdit(FocusEventArgs e)
    {
        _isEditing = false;
        _editingValue = null;
    }

    private void HandleNumericKey(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && _editingValue.HasValue)
        {
            Value = _editingValue.Value * 100 / MaxValue;
            _isEditing = false;
            ValueChanged.InvokeAsync(Value);
        } 
        else if (e.Key == "Escape")
        {
            _isEditing = false;
            _editingValue = null;
        }
    }
    
    private int StarValue
    {
        get => Value / 20 ?? 0; // 0-5 stars mapped to 0-100
        set
        {
            Value = value * 20;
            ValueChanged.InvokeAsync(Value);
        }
    }
    
    private IEnumerable<TextualRating> GetTextualOptions() =>
        RatingMode == RatingMode.TextualShort
            ? new[] { TextualRating.Meh, TextualRating.Neutral, TextualRating.Good, TextualRating.Favourite }
            : Enum.GetValues<TextualRating>();
    
    private static int MapTextual(TextualRating r) => r switch
    {
        TextualRating.Hate => 15,
        TextualRating.Meh => 35,
        TextualRating.Neutral => 50,
        TextualRating.Ok => 60,
        TextualRating.Good => 70,
        TextualRating.VeryGood => 80,
        TextualRating.Excellent => 90,
        TextualRating.Favourite => 100,
        _ => 0
    };
    
    private static string SplitCamelCase(string input) =>
        System.Text.RegularExpressions.Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");
}