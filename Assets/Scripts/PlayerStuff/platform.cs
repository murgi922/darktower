using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class platform : MonoBehaviour
{
    [SerializeField] private Transform loc1;
    [SerializeField] private Transform loc2;
    [SerializeField] private float period;

    private void OnEnable()
    {
        transform.SetPositionAndRotation(loc1.position, loc1.rotation);
        StartCoroutine(PlatformCoroutineCaller());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    IEnumerator PlatformCoroutineCaller()
    {
        while (true)
        {
            yield return PlatformMovement(loc1, loc2);
            yield return PlatformMovement(loc2, loc1);
        }
    }
    IEnumerator PlatformMovement(Transform point1, Transform point2)
    {
        float elapsedTime = 0f;
        while (elapsedTime < period)
        {
            elapsedTime += Time.deltaTime;
            float percentageChange = Mathf.Clamp01(elapsedTime / period);
            transform.position = Vector3.Lerp(point1.position, point2.position, percentageChange);
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(point1.eulerAngles.z, point2.eulerAngles.z, percentageChange));
            yield return null;
        }
        transform.SetPositionAndRotation(point2.position, point2.rotation);
    }
}
