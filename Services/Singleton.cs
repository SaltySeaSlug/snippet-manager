using IniFile;

public sealed class Singleton
{
    private static readonly Lazy<Singleton> lazy =
        new(() => new Singleton());

    public static Singleton Instance { get { return lazy.Value; } }

    public Ini Config { get; set; }

    private Singleton()
    {
        Config = new Ini(Path.Combine(Application.StartupPath, "config.ini"));
    }
}