using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Contracts.Enums;

namespace Musicx.Presentation.Web.Client.Components.Ratings;

public partial class DynamicRating
{
    [Parameter] public RatingMode RatingMode { get; set; }
    [Parameter] public EventCallback<int?> ValueChanged { get; set; }
    [Parameter] public int? Value { get; set; }

    private bool _isOpen;

    private IEnumerable<TextualRating> TextualShortOptions => new[]
    {
        TextualRating.Meh,
        TextualRating.Neutral,
        TextualRating.Good,
        TextualRating.Favourite
    };

    private void SetTextual(TextualRating rating)
    {
        Value = MapTextual(rating);
        ValueChanged.InvokeAsync(Value);
    }

    private int? IntValue
    {
        get => Value;
        set
        {
            Value = value;
            ValueChanged.InvokeAsync(Value);
        }
    }
    
    private int StarValue
    {
        get => Value / 20 ?? 0; // 0-5 stars mapped to 0-100
        set
        {
            Value = (short?)value * 20;
            ValueChanged.InvokeAsync(Value);
        }
    }

    private RenderFragment NumericField(int max) => __builder =>
    {
        int? current = Value * max / 100;

        __builder.OpenComponent(0, typeof(MudNumericField<int?>));
        __builder.AddAttribute(1, "Min", 0);
        __builder.AddAttribute(2, "Max", max);
        __builder.AddAttribute(3, "Immediate", false);
        __builder.AddAttribute(4, "Value", current);
        __builder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create<int?>(this, v =>
        {
            Value = v * 100 / max;
            ValueChanged.InvokeAsync(Value);
        }));
        __builder.CloseComponent();
        __builder.AddContent(6, $" / {max}");
    };
    
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