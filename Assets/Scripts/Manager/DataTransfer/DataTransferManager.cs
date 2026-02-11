using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Net;
using System.Text;
using System.Threading.Tasks;

public class DataTransferManager : MonoBehaviour
{
    public static DataTransferManager Instance;

    [SerializeField] private DataTransferDiscoveryBroadcaster broadcaster;
    [SerializeField] private DataTransferDiscoveryListener discoveryListener;

    private PersistenceManager persistenceManager;
    private HttpListener listener;
    private int activePort;
    private bool isClientConnected;

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
        persistenceManager = PersistenceManager.Instance;
        discoveryListener.OnServerFound += OnServerDiscovered;
    }

    // =========================
    // SERVER MODE
    // =========================
    public void StartAsServer()
    {
        activePort = StartServerWithFallback();
        if (activePort < 0) return;

        broadcaster.StartBroadcast(activePort);

        LogManager.Trace("[DataTransferManager] Server started (auto-discovery)");
    }

    int StartServerWithFallback()
    {
        int[] ports = { 8080, 8081, 8082, 8083, 8090 };

        foreach (int port in ports)
        {
            try
            {
                listener = new HttpListener();
                listener.Prefixes.Add($"http://*:{port}/");
                listener.Start();
                Task.Run(ListenLoop);
                return port;
            }
            catch { }
        }

        LogManager.Error("[DataTransferManager] No free port available");
        return -1;
    }

    async Task ListenLoop()
    {
        while (listener != null && listener.IsListening)
        {
            HttpListenerContext context = await listener.GetContextAsync();
            HandleRequest(context);
        }
    }

    void HandleRequest(HttpListenerContext context)
    {
        if (context.Request.HttpMethod == "GET")
            SendSave(context);
        else if (context.Request.HttpMethod == "POST")
            ReceiveSave(context);
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
    // CLIENT MODE (AUTO)
    // =========================
    public void StartLookingForServer()
    {
        isClientConnected = false;
        discoveryListener.StartListening();

        LogManager.Trace("[DataTransferManager] Auto-discovery started");
    }

    void OnServerDiscovered(DataTransferDiscoveryPacket packet)
    {
        if (isClientConnected)
            return;

        isClientConnected = true;
        discoveryListener.StopListening();

        StartCoroutine(RequestSave(packet));
    }

    IEnumerator RequestSave(DataTransferDiscoveryPacket packet)
    {
        string url = $"http://{packet.ip}:{packet.port}/";

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        ImportSavePacket(request.downloadHandler.text);
        LogManager.Trace("[DataTransferManager] Save received automatically");
    }

    // =========================
    // SHARED LOGIC
    // =========================
    string ExportSavePacket()
    {
        SaveData save = persistenceManager.GetLastSaveData();

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

        persistenceManager.Save(packet.saveData);
    }

    public void StopServer()
    {
        broadcaster.StopBroadcast();
        listener?.Stop();
        listener = null;
    }
}
