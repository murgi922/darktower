using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class proceduralMovement : MonoBehaviour
{
    #region Variables
    [Header("Assign IK")]
    [SerializeField] private Transform rHandIk;
    [SerializeField] private Transform lHandIk;
    [SerializeField] private Transform rLegIk;
    [SerializeField] private Transform lLegIk;
    [SerializeField] private Transform lFoot;
    [SerializeField] private Transform rFoot;

    [Header("Leg IK")]
    [SerializeField] private float legRayLength;
    [SerializeField] private playerMovement playerMovement;
    [SerializeField] private Transform root;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float legMoveThreshold;
    [SerializeField] private float legVertMag;
    [SerializeField] private float timePeriod;
    [SerializeField] private Transform orientation;
    [SerializeField] private float rayCastOffset;
    [SerializeField] private AnimationCurve footCurve;
    public class LegInfo
    {
        public Transform ik;
        public Transform defaultTransform;
        public Coroutine lerpCoroutine;
        public float stepProgress = 0f;
    }
    LegInfo lLegInfo;
    LegInfo rLegInfo;

    private RaycastHit2D hit;
    #endregion
    #region Built in functions
    private void OnEnable()
    {
        lLegInfo = new LegInfo();
        rLegInfo = new LegInfo();
        lLegInfo.ik = lLegIk;
        lLegInfo.defaultTransform = lFoot;
        rLegInfo.ik = rLegIk;
        rLegInfo.defaultTransform = rFoot;
    }
    private void Update()
    {
        hit = Physics2D.Raycast(orientation.position + (orientation.right * rayCastOffset), -orientation.up, 0.8f, whatIsGround);
        Walking(lLegInfo, rLegInfo);
        Walking(rLegInfo, lLegInfo);
    }
    private void Walking(LegInfo legInfo, LegInfo otherLeg)
    {
        if (!playerMovement.IsGrounded)
        {
            legInfo.ik.position = legInfo.defaultTransform.position;
        }
        else
        {
            if (hit)
            {
                if (Vector2.Distance(hit.point, legInfo.ik.position) > legMoveThreshold)
                {
                    if (legInfo.lerpCoroutine == null && (otherLeg.stepProgress > 0.9f) || otherLeg.lerpCoroutine == null)
                    {
                        legInfo.lerpCoroutine = StartCoroutine(Lerp(legInfo));
                    }
                }
            }
            else
            {
                if (legInfo.lerpCoroutine != null)
                {
                    StopCoroutine(legInfo.lerpCoroutine);
                    legInfo.lerpCoroutine = null;
                }
                if (otherLeg.lerpCoroutine != null)
                {
                    StopCoroutine(otherLeg.lerpCoroutine);
                    otherLeg.lerpCoroutine = null;
                }
                legInfo.ik.position = legInfo.defaultTransform.position;
                otherLeg.ik.position = otherLeg.defaultTransform.position;
            }
        }
    }
    IEnumerator Lerp(LegInfo legInfo)
    {
        legInfo.stepProgress = 0f;
        float elapsedTime = 0f;
        Vector2 oldPos = root.position;
        yield return null;
        Vector2 newPos = root.position;
        Vector2 offset = newPos - oldPos;
        while (elapsedTime < timePeriod)
        {
            legInfo.stepProgress = elapsedTime / timePeriod;
            newPos = root.position;
            offset = newPos - oldPos;
            oldPos = newPos;
            elapsedTime += Time.deltaTime; 
            Vector2 temp = new Vector2(legInfo.ik.position.x, legInfo.ik.position.y) + offset;
            if (!hit) break;
            temp = Vector2.Lerp(temp, hit.point, elapsedTime / timePeriod);
            temp.y += footCurve.Evaluate(elapsedTime / timePeriod) * legVertMag;
            //temp.y += Mathf.Sin(Mathf.Lerp(0f, Mathf.PI, elapsedTime / timePeriod)) * legVertMag;
            legInfo.ik.position = temp;
            yield return null;
        }
        legInfo.lerpCoroutine = null;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(orientation.position + (orientation.right * 0.5f), (-orientation.up * 0.8f) + (orientation.position + (orientation.right * 0.5f)));
        Gizmos.DrawSphere(hit.point, 0.1f);

    }
    #endregion

}
