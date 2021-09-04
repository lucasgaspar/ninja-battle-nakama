using UnityEngine;

public class Hazard : MonoBehaviour
{
    private RollbackVar<bool> wasCreated = new RollbackVar<bool>();
    private SpriteRenderer spriteRenderer = null;
    private Map map = null;

    public Vector2Int Coordinates { get; private set; } = new Vector2Int();

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(int tick, Vector2Int coordinates, Color color, Map map)
    {
        this.map = map;
        color.a = 0.75f;
        spriteRenderer.color = color;
        Coordinates = coordinates;
        wasCreated[default(int)] = false;
        wasCreated[tick] = true;
        GameManager.Instance.onRewind += Rewind;
    }

    private void OnDestroy()
    {
        GameManager.Instance.onRewind -= Rewind;
    }

    private void Rewind(int tick)
    {
        tick--;
        if (wasCreated.GetLastValue(tick) == true)
            return;

        map.RemoveHazard(this);
        Destroy(this.gameObject);
    }
}
