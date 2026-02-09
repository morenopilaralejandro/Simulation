using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Net;
using System.Text;
using System.Threading.Tasks;

public class DataTransferManualManager : MonoBehaviour
{
    public static DataTransferManualManager Instance;

    private PersistanceManager persistanceManager;
    private HttpListener listener;
    private string currentPin;

    private const int PORT = 8080;
    private const int PROTOCOL_VERSION = 1;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        persistanceManager = PersistanceManager.Instance;
    }

    // =========================
    // SERVER MODE
    // =========================
    public void StartAsServer()
    {
        currentPin = Random.Range(100000, 999999).ToString();

        listener = new HttpListener();
        listener.Prefixes.Add($"http://*:{PORT}/");
        listener.Start();

        Task.Run(ListenLoop);

        LogManager.Trace($"[DataTransferManager] Server started. PIN: {currentPin}");
        LogManager.Trace($"[DataTransferManager] IP: {GetLocalIp()}");
    }

    async Task ListenLoop()
    {
        while (listener.IsListening)
        {
            HttpListenerContext context = await listener.GetContextAsync();
            HandleRequest(context);
        }
    }

    void HandleRequest(HttpListenerContext context)
    {
        if (context.Request.Headers["X-PIN"] != currentPin)
        {
            context.Response.StatusCode = 403;
            context.Response.Close();
            return;
        }

        if (context.Request.HttpMethod == "GET")
        {
            SendSave(context);
        }
        else if (context.Request.HttpMethod == "POST")
        {
            ReceiveSave(context);
        }
    }

    void SendSave(HttpListenerContext context)
    {
        string json = ExportSavePacket();
        byte[] buffer = Encoding.UTF8.GetBytes(json);

        context.Response.ContentType = "application/json";
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        context.Response.Close();
    }

    async void ReceiveSave(HttpListenerContext context)
    {
        using var reader = new System.IO.StreamReader(context.Request.InputStream);
        string json = await reader.ReadToEndAsync();

        ImportSavePacket(json);
        context.Response.StatusCode = 200;
        context.Response.Close();
    }

    // =========================
    // CLIENT MODE
    // =========================
    public IEnumerator SendSave(string targetIp, string pin)
    {
        string url = $"http://{targetIp}:{PORT}/";
        string json = ExportSavePacket();

        UnityWebRequest request =
            UnityWebRequest.Post(url, json, "application/json");

        request.SetRequestHeader("X-PIN", pin);
        yield return request.SendWebRequest();

        LogManager.Trace("[DataTransferManager] Save sent.");
    }

    public IEnumerator RequestSave(string targetIp, string pin)
    {
        string url = $"http://{targetIp}:{PORT}/";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("X-PIN", pin);

        yield return request.SendWebRequest();

        ImportSavePacket(request.downloadHandler.text);
        LogManager.Trace("[DataTransferManager] Save received.");
    }

    // =========================
    // SHARED LOGIC
    // =========================
    string ExportSavePacket()
    {
        SaveData save = persistanceManager.GetLastSaveData();
        DataTransferPacket packet = new()
        {
            protocolVersion = PROTOCOL_VERSION,
            deviceId = SystemInfo.deviceUniqueIdentifier,
            timestamp = System.DateTime.UtcNow.ToString("o"),
            saveData = save
        };

        return JsonUtility.ToJson(packet, true);
    }

    void ImportSavePacket(string json)
    {
        DataTransferPacket packet =
            JsonUtility.FromJson<DataTransferPacket>(json);

        if (packet.protocolVersion != PROTOCOL_VERSION)
        {
            LogManager.Error("[DataTransferManager] Protocol mismatch");
            return;
        }

        persistanceManager.Save(packet.saveData);
    }

    public void StopServer()
    {
        listener?.Stop();
        listener = null;
    }

    static string GetLocalIp()
    {
        using var socket = new System.Net.Sockets.Socket(
            System.Net.Sockets.AddressFamily.InterNetwork,
            System.Net.Sockets.SocketType.Dgram, 0);
        socket.Connect("8.8.8.8", 65530);
        return ((System.Net.IPEndPoint)socket.LocalEndPoint).Address.ToString();
    }
}
