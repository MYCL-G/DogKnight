using System;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class mMouse : MonoBehaviour
{
    static mMouse inst;
    public static mMouse Inst => inst;
    RaycastHit hit;
    public Action<Vector3> OnMouseClick;
    public Action<GameObject> OnEnemyClick;
    public Texture2D point, doorway, attack, target, arrow;
    private void Awake()
    {
        if (Inst == null) inst = this;
        else Destroy(this);
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {

    }
    void Update()
    {
        SetCursorTexture();
        MouseControl();
    }
    void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            switch (hit.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Attackable":
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Portal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                    break;
                default:
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }
    }
    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hit.collider != null)
        {
            switch (hit.collider.gameObject.tag)
            {
                case "Portal":
                case "Ground":
                    OnMouseClick?.Invoke(hit.point);
                    break;
                case "Attackable":
                case "Enemy":
                    OnEnemyClick?.Invoke(hit.collider.gameObject);
                    break;
            }
        }
    }
}
