namespace EyeYul.Infraestructura.Interoperabilidad;

internal static class ProcesosConocidos
{
    public static readonly HashSet<string> Meetings = new(StringComparer.OrdinalIgnoreCase)
    {
        "Teams", "ms-teams", "Zoom", "Zoom.exe", "CptHost", "Slack", "Discord", "Webex",
        "GoToMeeting", "BlueJeans", "skype"
    };

    public static readonly HashSet<string> ScreenRecorders = new(StringComparer.OrdinalIgnoreCase)
    {
        "obs64", "obs32", "obs", "ShareX", "SnagitEditor", "Snagit32", "GameBar",
        "GameBarFTServer", "Camtasia", "bdcam", "Bandicam"
    };

    public static readonly HashSet<string> MediaPlayers = new(StringComparer.OrdinalIgnoreCase)
    {
        "vlc", "mpc-hc64", "mpc-hc", "wmplayer", "PotPlayerMini64", "Netflix"
    };
}
