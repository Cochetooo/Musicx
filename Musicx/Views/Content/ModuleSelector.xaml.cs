using System.Windows.Media.Media3D;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace Musicx.Views.Content;

public partial class ModuleSelector
{
    public ModuleSelector()
    {
        InitializeComponent();
        this.MouseMove += ModuleSelector_MouseMove;
    }

    private void ModuleSelector_MouseMove(object sender, MouseEventArgs e)
    {
        var mousePos = e.GetPosition(this);
        var centerX = this.ActualWidth / 2;
        var centerY = this.ActualHeight / 2;

        var offsetX = (mousePos.X - centerX) / centerX;
        var offsetY = (mousePos.Y - centerY) / centerY;

        AxisAngleRotation.Angle = offsetX * 10;
        AxisAngleRotation.Axis = new Vector3D(0, 1, 0);
        RotateTransform.Rotation = AxisAngleRotation;
        
        AxisAngleRotation.Angle = offsetY * 10;
        AxisAngleRotation.Axis = new Vector3D(1, 0, 0);
        RotateTransform.Rotation = AxisAngleRotation;
    }
}