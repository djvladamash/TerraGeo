using Microsoft.Data.Sqlite;

namespace TerraGeo;

public sealed class ProjectDatabase : IDisposable
{
    private readonly SqliteConnection _conn;

    public string FilePath { get; }

    public ProjectDatabase(string filePath)
    {
        FilePath = filePath;
        var csb = new SqliteConnectionStringBuilder
        {
            DataSource = filePath,
            Mode = File.Exists(filePath)
                ? SqliteOpenMode.ReadWrite
                : SqliteOpenMode.ReadWriteCreate,
        };
        _conn = new SqliteConnection(csb.ToString());
        _conn.Open();
        Init();
    }

    private void Init()
    {
        using var cmd = _conn.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS meta (
                key TEXT PRIMARY KEY,
                value TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS points (
                id          INTEGER PRIMARY KEY AUTOINCREMENT,
                point_id    TEXT NOT NULL,
                x           REAL NOT NULL,
                y           REAL NOT NULL,
                z           REAL,
                description TEXT,
                code        TEXT
            );
            """;
        cmd.ExecuteNonQuery();
    }

    public void SetMeta(string key, string value)
    {
        using var cmd = _conn.CreateCommand();
        cmd.CommandText = "INSERT INTO meta(key,value) VALUES($k,$v) " +
                          "ON CONFLICT(key) DO UPDATE SET value=excluded.value;";
        cmd.Parameters.AddWithValue("$k", key);
        cmd.Parameters.AddWithValue("$v", value);
        cmd.ExecuteNonQuery();
    }

    public long AddPoint(Point p)
    {
        using var cmd = _conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO points(point_id,x,y,z,description,code)
            VALUES($pid,$x,$y,$z,$d,$c);
            SELECT last_insert_rowid();
            """;
        cmd.Parameters.AddWithValue("$pid", p.PointId);
        cmd.Parameters.AddWithValue("$x", p.X);
        cmd.Parameters.AddWithValue("$y", p.Y);
        cmd.Parameters.AddWithValue("$z", double.IsNaN(p.Z) ? DBNull.Value : p.Z);
        cmd.Parameters.AddWithValue("$d", (object?)p.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$c", (object?)p.Code ?? DBNull.Value);
        return (long)cmd.ExecuteScalar()!;
    }

    public List<Point> GetPoints()
    {
        var list = new List<Point>();
        using var cmd = _conn.CreateCommand();
        cmd.CommandText = "SELECT id,point_id,x,y,z,description,code FROM points ORDER BY id;";
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Point
            {
                Id          = r.GetInt64(0),
                PointId     = r.GetString(1),
                X           = r.GetDouble(2),
                Y           = r.GetDouble(3),
                Z           = r.IsDBNull(4) ? double.NaN : r.GetDouble(4),
                Description = r.IsDBNull(5) ? null : r.GetString(5),
                Code        = r.IsDBNull(6) ? null : r.GetString(6),
            });
        }
        return list;
    }

    public void DeletePoint(long id)
    {
        using var cmd = _conn.CreateCommand();
        cmd.CommandText = "DELETE FROM points WHERE id=$id;";
        cmd.Parameters.AddWithValue("$id", id);
        cmd.ExecuteNonQuery();
    }

    public void Dispose() => _conn.Dispose();
}
