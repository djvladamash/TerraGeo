using System.Globalization;
using System.Windows;

namespace TerraGeo;

public partial class PointDialog : Window
{
    public string PointId     { get; private set; } = "";
    public double X           { get; private set; }
    public double Y           { get; private set; }
    public double Z           { get; private set; } = double.NaN;
    public string? Code       { get; private set; }
    public string? Description { get; private set; }

    public PointDialog() => InitializeComponent();

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void OnOk(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TbPointId.Text))
        {
            MessageBox.Show(this, "Point ID je obavezan.", "TerraGeo",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (!double.TryParse(TbX.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var x) ||
            !double.TryParse(TbY.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var y))
        {
            MessageBox.Show(this, "X i Y moraju biti brojevi (decimalna tačka).",
                "TerraGeo", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        double z = double.NaN;
        if (!string.IsNullOrWhiteSpace(TbZ.Text))
        {
            if (!double.TryParse(TbZ.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out z))
            {
                MessageBox.Show(this, "Z mora biti broj ili prazno.", "TerraGeo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        PointId     = TbPointId.Text.Trim();
        X           = x;
        Y           = y;
        Z           = z;
        Code        = string.IsNullOrWhiteSpace(TbCode.Text) ? null : TbCode.Text.Trim();
        Description = string.IsNullOrWhiteSpace(TbDesc.Text) ? null : TbDesc.Text.Trim();

        DialogResult = true;
        Close();
    }
}
