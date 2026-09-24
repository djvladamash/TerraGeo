namespace TerraGeo;

public sealed record Helmert7Parameters(
    double Dx, double Dy, double Dz,
    double Rx, double Ry, double Rz,
    double ScalePpm)
{
    public double RxRad => Rx * Math.PI / (180.0 * 3600.0);
    public double RyRad => Ry * Math.PI / (180.0 * 3600.0);
    public double RzRad => Rz * Math.PI / (180.0 * 3600.0);
    public double Scale => 1.0 + ScalePpm * 1e-6;
}

public static class Helmert7
{
    public static (double X, double Y, double Z) Apply(
        Helmert7Parameters p, double x, double y, double z)
    {
        double s = p.Scale;
        return (
            p.Dx + s * (x - p.RzRad * y + p.RyRad * z),
            p.Dy + s * (p.RzRad * x + y - p.RxRad * z),
            p.Dz + s * (-p.RyRad * x + p.RxRad * y + z)
        );
    }
}
