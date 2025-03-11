using System.Windows;
using System.Windows.Controls;

namespace Musicx.Ui.Converters;

public class ViewModelToViewConverter : DataTemplateSelector
{
    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        if (null == item)
        {
            return null;
        }

        var viewModelType = item.GetType();
        var viewTypeName = viewModelType.FullName?.Replace("ViewModel", "View");
        
        if (null == viewTypeName)
        {
            return null;
        }
        
        var viewType = viewModelType.Assembly.GetType(viewTypeName);

        if (null == viewType)
        {
            return null;
        }

        return new DataTemplate { VisualTree = new FrameworkElementFactory(viewType) };
    }
}