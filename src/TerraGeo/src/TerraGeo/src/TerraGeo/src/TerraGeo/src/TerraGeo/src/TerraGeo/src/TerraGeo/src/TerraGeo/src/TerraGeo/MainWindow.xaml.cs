using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Win32;

namespace TerraGeo;

public partial class MainWindow : Window
{
    private ProjectDatabase? _db;
    private readonly ObservableCollection<Point> _points = new();

    public MainWindow()
    {
        InitializeComponent();
        PointsGrid.ItemsSource = _points;
        Title = $"{ProductInfo.Name} — {ProductInfo.VersionLine}";
    }

    private void SetStatus(string msg) => StatusText.Text = msg;

    private void OnNewProject(object sender, RoutedEventArgs e)
    {
        var sfd = new SaveFileDialog
        {
            Filter = "TerraGeo projekat (*.tgp)|*.tgp",
            FileName = "NoviProjekat.tgp"
        };
        if (sfd.ShowDialog() != true) return;
        _db?.Dispose();
        _db = new ProjectDatabase(sfd.FileName);
        _db.SetMeta("created_utc", DateTime.UtcNow.ToString("o"));
        _db.SetMeta("product", ProductInfo.Name);
        _db.SetMeta("designer", ProductInfo.DesignerLine);
        _points.Clear();
        SetStatus($"Novi projekat: {sfd.FileName}");
    }

    private void OnOpenProject(object sender, RoutedEventArgs e)
    {
        var ofd = new OpenFileDialog
        {
            Filter = "TerraGeo projekat (*.tgp)|*.tgp"
        };
        if (ofd.ShowDialog() != true) return;
        try
        {
            _db?.Dispose();
            _db = new ProjectDatabase(ofd.FileName);
            ReloadPoints();
            SetStatus($"Otvoren: {ofd.FileName}");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "Greška pri otvaranju:\n" + ex.Message,
                "TerraGeo", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OnSaveAs(object sender, RoutedEventArgs e)
    {
        if (_db is null) { OnNewProject(sender, e); return; }
        var sfd = new SaveFileDialog
        {
            Filter = "TerraGeo projekat (*.tgp)|*.tgp",
            FileName = System.IO.Path.GetFileName(_db.FilePath)
        };
        if (sfd.ShowDialog() != true) return;
        System.IO.File.Copy(_db.FilePath, sfd.FileName, overwrite: true);
        SetStatus($"Sačuvano kao: {sfd.FileName}");
    }

    private void ReloadPoints()
    {
        _points.Clear();
        if (_db is null) return;
        foreach (var p in _db.GetPoints()) _points.Add(p);
    }

    private void OnAddPoint(object sender, RoutedEventArgs e)
    {
        if (_db is null)
        {
            MessageBox.Show(this, "Prvo otvori ili napravi projekat.",
                "TerraGeo", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var dlg = new PointDialog { Owner = this };
        if (dlg.ShowDialog() != true) return;

        var pt = new Point
        {
            PointId     = dlg.PointId,
            X           = dlg.X,
            Y           = dlg.Y,
            Z           = dlg.Z,
            Description = dlg.Description,
            Code        = dlg.Code
        };
        _db.AddPoint(pt);
        ReloadPoints();
        SetStatus($"Dodata tačka {pt.PointId}.");
    }

    private void OnDeletePoint(object sender, RoutedEventArgs e)
    {
        if (_db is null || PointsGrid.SelectedItem is not Point p) return;
        var r = MessageBox.Show(this,
            $"Obrisati tačku {p.PointId}?", "Potvrda",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (r != MessageBoxResult.Yes) return;
        _db.DeletePoint(p.Id);
        ReloadPoints();
        SetStatus($"Obrisana tačka {p.PointId}.");
    }

    private void OnHelmert(object sender, RoutedEventArgs e)
    {
        var p = new Helmert7Parameters(682, -203, 480, 0, 0, 0, 0);
        var (X, Y, Z) = Helmert7.Apply(p, 4234567.0, 1234567.0, 4567890.0);
        MessageBox.Show(this,
            $"Helmert 7-param (MGI 1901 → WGS84, EPSG:6312)\n\n" +
            $"Ulaz:  4234567.000, 1234567.000, 4567890.000\n" +
            $"Izlaz: {X:F3}, {Y:F3}, {Z:F3}",
            "TerraGeo — Helmert", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void OnAbout(object sender, RoutedEventArgs e)
    {
        new AboutWindow { Owner = this }.ShowDialog();
    }

    private void OnExit(object sender, RoutedEventArgs e) => Close();

    protected override void OnClosed(EventArgs e)
    {
        _db?.Dispose();
        base.OnClosed(e);
    }
}
