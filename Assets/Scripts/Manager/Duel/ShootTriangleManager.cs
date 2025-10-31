using UnityEngine;
using Simulation.Enums.Character;

public class ShootTriangleManager : MonoBehaviour
{
    public static ShootTriangleManager Instance { get; private set; }

    private ShootTriangle shootTriangle;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {

    }

    public void RegisterTriangle(ShootTriangle shootTriangle)
    {
        this.shootTriangle = shootTriangle;
    }

    public void UnregisterTriangle(ShootTriangle shootTriangle)
    {
        this.shootTriangle = null;
    }

    public void Show()
    {
        shootTriangle.Show();
    }

    public void Hide()
    {
        shootTriangle.Hide();
    }

    public void SetTriangleFromCharacter(
        Character character)
    {
        shootTriangle.SetTriangleFromCharacter(
            character, 
            GoalManager.Instance.GetOpponentGoal(character).transform.position);
        Show();
    }

    public Vector3 GetRandomPoint()
    {
        return shootTriangle.GetRandomPoint();
    }
}
