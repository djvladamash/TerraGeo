namespace TerraGeo;

public static class ProductInfo
{
    public const string Name         = "TerraGeo Inženjering";
    public const string ShortName    = "TerraGeo";
    public const string Designer     = "Vladimir Djokich";
    public const string DesignerLine = "Designed by Vladimir Djokich";
    public const string Copyright    = "© 2026 Vladimir Djokich — All Rights Reserved";
    public const string Version      = "0.1.0";
    public const string Build        = "0001";

    public static string VersionLine => $"v{Version} (build {Build})";
}
