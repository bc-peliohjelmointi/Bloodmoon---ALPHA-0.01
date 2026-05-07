using UnityEngine;

public class SaveMeeee : MonoBehaviour
{
    private PlayerController pc;
    public void Save(ref PlayerData Data)
    {
        Data.Location = transform.position;
        Data.Rotation = transform.rotation;
        Data.health = pc.health;
        Data.food = pc.food;
        Data.water = pc.water;
        Data.stamina = pc.stamina;
        Debug.Log("Player saved:" + Data.Location);
    }
    public void Load(PlayerData Data)
    {
        transform.position = Data.Location;
        transform.rotation = Data.Rotation;
        pc.health = Data.health;
        pc.water = Data.water;
        pc.food = Data.food;
        pc.stamina = Data.stamina;
    }

    void OnEnable() {
        pc = FindAnyObjectByType<PlayerController>();
    }
}
[System.Serializable]
public struct PlayerData
{
    public Vector3 Location;
    public Quaternion Rotation;
    public float health;
    public int food;
    public int water;
    public int stamina;
}
