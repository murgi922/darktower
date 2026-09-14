using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class rootReaction : MonoBehaviour
{
    [SerializeField] private Transform root;
    private Coroutine kinematics;
    private Vector2 velocity;
    private Vector2 acceleration;
    private void OnEnable()
    {
        kinematics = StartCoroutine(Kinematics(root));
    }
    private void OnDisable()
    {
        StopCoroutine(kinematics);
        kinematics = null;
    }
    private void Update()
    {
        
    }

    IEnumerator Kinematics(Transform target)
    {
        Vector2 initialPos;
        Vector2 finalPos;
        Vector2 deltaPos;
        Vector2 initialVel;
        Vector2 finalVel;
        Vector2 deltaVel;
        initialPos = target.position;
        yield return new WaitForFixedUpdate();
        finalPos = target.position;
        deltaPos = finalPos - initialPos;
        velocity = deltaPos / Time.fixedDeltaTime;
        while (true)
        {
            initialPos = target.position;
            initialVel = velocity;
            yield return new WaitForFixedUpdate();
            finalPos = target.position;
            deltaPos = finalPos - initialPos;
            velocity = deltaPos / Time.fixedDeltaTime;
            finalVel = velocity;
            deltaVel = finalVel - initialVel;
            acceleration = deltaVel / Time.fixedDeltaTime;
        }
    }
}
