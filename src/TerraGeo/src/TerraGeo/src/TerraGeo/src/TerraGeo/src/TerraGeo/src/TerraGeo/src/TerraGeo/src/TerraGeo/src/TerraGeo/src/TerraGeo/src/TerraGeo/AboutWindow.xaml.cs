using System.Windows;

namespace TerraGeo;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        VersionText.Text   = "Verzija: " + ProductInfo.VersionLine;
        CopyrightText.Text = ProductInfo.Copyright;
    }

    private void OnClose(object sender, RoutedEventArgs e) => Close();
}
