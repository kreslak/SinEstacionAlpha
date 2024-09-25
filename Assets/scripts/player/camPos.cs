using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class camPos : NetworkBehaviour
{
    public Transform cameraPosition;

    public void Init()
    {
        cameraPosition = FindObjectOfType<xdxd>().transform;
        Debug.Log("camera position init");
    }

    private void Update()
    {
        if (!cameraPosition) return;
        transform.position = cameraPosition.position;
    }

    IEnumerator Waitforcam()
    {
        Debug.Log("camera co");
        cameraPosition = FindObjectOfType<xdxd>().gameObject.transform;
        yield return new WaitForSecondsRealtime(1);
        StopCoroutine(Waitforcam());
        if (!cameraPosition)
            StartCoroutine(Waitforcam());
    }
}
