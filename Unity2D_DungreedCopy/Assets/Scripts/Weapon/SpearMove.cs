using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearMove : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve  returnCurve;
    [SerializeField]
    private float           returnTime;

    private Vector2         start; 
    private Vector2         target;
    private Vector3 startRotation;
    private Vector3 targetRotation;

    private void Start()
    {
        start   = new Vector2(transform.localPosition.x, transform.localPosition.y);
        target  = new Vector2(transform.localPosition.x + 0.3f, transform.localPosition.y);
        startRotation = new Vector3(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z);
        targetRotation = new Vector3(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z + 20.0f);
    }

    public void AttackMove()
    {
        transform.localPosition = target;

        StartCoroutine(ReturnMove());
    }

    public void AttackRotate()
    {
        transform.localRotation = Quaternion.Euler(targetRotation);

        StartCoroutine(ReturnRotate());
    }

    private IEnumerator ReturnMove()
    {
        float time = 0;

        while(time < returnTime)
        {
            time += Time.deltaTime;

            float t = time / returnTime;
            float curve = returnCurve.Evaluate(t);

            transform.localPosition = Vector2.Lerp(target, start, curve);

            yield return null;
        }
        transform.localPosition = start;
    }

    private IEnumerator ReturnRotate()
    {
        float time = 0;

        while (time < returnTime)
        {
            time += Time.deltaTime;

            float t = time / returnTime;
            float curve = returnCurve.Evaluate(t);

            transform.localRotation = Quaternion.Lerp(Quaternion.Euler(targetRotation), Quaternion.Euler(startRotation), curve);

            yield return null;
        }
        transform.localRotation = Quaternion.Euler(startRotation);
    }
}
