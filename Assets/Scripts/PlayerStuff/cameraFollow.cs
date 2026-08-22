using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class cameraFollow : MonoBehaviour
{
    [SerializeField] private Transform cameraPos;
    [SerializeField] private float camFollowSpeed;
    private void Update()
    {
        transform.position = Vector2.LerpUnclamped(transform.position, cameraPos.position, Time.deltaTime * camFollowSpeed);
    }
}
