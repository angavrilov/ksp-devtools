using System;
using UnityEngine;

using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Text;

[KSPAddon(KSPAddon.Startup.Instantly, false)]
public class MonoProfilerToggle : MonoBehaviour
{
    private static TcpClient link = null;
    private static NetworkStream stream = null;
    private static StreamReader reader = null;
    private static StreamWriter writer = null;

    private static int port = -1;
    private static bool profiling = true;
    private static bool error = false;

    private Rect pos;
    private GUIStyle style_r, style_g, style_b;

    private float last_command = -1;

    public MonoProfilerToggle()
    {
        if (link != null)
            return;

        var profargs = System.Environment.GetEnvironmentVariable("MONO_PROFILE");
        if (profargs == null || profargs == "")
            return;

        if (Connect(profargs))
            DontDestroyOnLoad(this);
    }

    private bool Connect(string profargs)
    {
        Regex port_re = new Regex(@"command-port=(?<port>\d+)");
        MatchCollection matches = port_re.Matches(profargs);

        foreach (Match match in matches)
        {
            if (match.Groups["port"].Success)
                port = int.Parse(match.Groups["port"].Value);
        }

        if (port < 0)
            return false;

        if (profargs.Contains("start-disabled"))
            profiling = false;

        try
        {
            link = new TcpClient("127.0.0.1", port);
            link.ReceiveTimeout = 5000;

            stream = link.GetStream();
            reader = new StreamReader(stream, Encoding.ASCII);
            writer = new StreamWriter(stream, Encoding.ASCII);

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Cannot connect to profiler: "+e);
            link = null;
            error = true;
            return false;
        }
    }

    private void SendEnable(bool on)
    {
        try
        {
            error = false;

            try
            {
                while (stream.DataAvailable)
                    Debug.Log("profiler: "+reader.ReadLine());
            }
            catch (SocketException e)
            {
                Debug.LogError("profiler read: "+e);
            }

            reader.DiscardBufferedData();

            writer.WriteLine(on ? "enable" : "disable");
            writer.Flush();

            var reply = reader.ReadLine();

            if (reply.StartsWith("DONE"))
                profiling = on;
            else
            {
                Debug.LogError("Profiler reply: "+reply);
                error = true;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Profiler control: "+e);
            error = true;
        }
    }

    public void Update()
    {
        if (link != null && link.Connected)
        {
            if (Input.GetKeyDown(KeyCode.F10) && Input.GetKey(KeyCode.RightAlt))
            {
                // Don't send commands too often
                if (Time.realtimeSinceStartup < last_command+1)
                    return;

                last_command = Time.realtimeSinceStartup;
                SendEnable(!profiling);
            }
        }
    }

    public void OnGUI()
    {
        if (link == null && !error)
            return;

        if (style_r == null)
        {
            pos = new Rect(Screen.width - 60, Screen.height - 60, 50, 60);

            style_r = new GUIStyle(GUI.skin.label);
            style_r.alignment = TextAnchor.LowerRight;
            style_r.normal.textColor = new Color(1.0f, 0.0f, 0.0f, 0.7f);

            style_g = new GUIStyle(style_r);
            style_g.normal.textColor = new Color(0.0f, 1.0f, 0.0f, 0.7f);

            style_b = new GUIStyle(style_r);
            style_b.normal.textColor = new Color(0.0f, 0.0f, 1.0f, 0.7f);
        }

        if (!error && !link.Connected)
            error = true;

        if (error)
            GUI.Label(pos, "ERR", style_r);
        else if (profiling)
            GUI.Label(pos, "On", style_g);
        else
            GUI.Label(pos, "Off", style_b);
    }
}

