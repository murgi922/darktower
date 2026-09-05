using UnityEngine;

public class rootMove : MonoBehaviour
{
    [SerializeField] private Transform rootTarget;
    void Update()
    {
        transform.position = rootTarget.position;
    }
}
