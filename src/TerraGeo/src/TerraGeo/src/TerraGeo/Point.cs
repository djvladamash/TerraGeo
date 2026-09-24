namespace TerraGeo;

public sealed class Point
{
    public long   Id          { get; set; }
    public string PointId     { get; set; } = "";
    public double X           { get; set; }
    public double Y           { get; set; }
    public double Z           { get; set; } = double.NaN;
    public string? Description { get; set; }
    public string? Code       { get; set; }

    public string ZDisplay => double.IsNaN(Z) ? "" : Z.ToString("F3");
}
