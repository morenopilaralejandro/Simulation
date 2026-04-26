using UnityEngine;
using Cinemachine;
using Aremoreno.Enums.Battle;

public class BattleCameraPreview : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [Header("Travel Settings")]
    private Vector3 startPoint = new Vector3(0f, 0f, -11f);
    private Vector3 endPoint = new Vector3(0f, 0f, 3f);
    private float travelDuration = 30f;

    private float travelTimer;
    private bool isTravelling;

    private void Awake() 
    {
        virtualCamera.enabled = false;
    }

    private void OnEnable()
    {
        BattleEvents.OnBattleStart += HandleBattleStart;
        TeamEvents.OnTeamPreviewStarted += HandlePreviewStarted;
        TeamEvents.OnTeamPreviewEnded += HandlePreviewEnded;
    }

    private void OnDisable()
    {
        BattleEvents.OnBattleStart += HandleBattleStart;
        TeamEvents.OnTeamPreviewStarted -= HandlePreviewStarted;
        TeamEvents.OnTeamPreviewEnded -= HandlePreviewEnded;
    }

    private void Update()
    {
        if (!isTravelling) return;

        travelTimer += Time.deltaTime;
        float t = Mathf.Clamp01(travelTimer / travelDuration);

        Vector3 currentPos = virtualCamera.transform.position;
        float newX = Mathf.Lerp(startPoint.x, endPoint.x, t);
        float newZ = Mathf.Lerp(startPoint.z, endPoint.z, t);

        virtualCamera.transform.position = new Vector3(newX, currentPos.y, newZ);

        if (t >= 1f)
        {
            isTravelling = false;
        }
    }

    private void HandleBattleStart(BattleType battleType) 
    {
        if (battleType == BattleType.Mini)
            this.gameObject.SetActive(false);
    }

    private void HandlePreviewStarted()
    {
        virtualCamera.enabled = true;
        StartTravel();
    }

    private void HandlePreviewEnded()
    {
        isTravelling = false;
        virtualCamera.enabled = false;
        this.gameObject.SetActive(false);
    }

    private void StartTravel()
    {
        travelTimer = 0f;
        isTravelling = true;

        Vector3 currentPos = virtualCamera.transform.position;
        virtualCamera.transform.position = new Vector3(startPoint.x, currentPos.y, startPoint.z);
    }
}
