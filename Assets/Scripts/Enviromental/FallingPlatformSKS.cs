using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatformSKS : MonoBehaviour
{
    [SerializeField] private int jumpToFallPlatform = 10;
    public static int jumpToFallPlatformStatic;

    void Update()
    {
        if (jumpToFallPlatformStatic != 0)
        {
            jumpToFallPlatform = jumpToFallPlatformStatic;
            jumpToFallPlatformStatic = jumpToFallPlatform;
        }


    }

    [SerializeField] private float timeToDestroyPlatform = 4f;

    private Rigidbody rb = default;

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.TryGetComponent(out PlayerMovementWallRun player))
        {
            if (jumpToFallPlatform > 0)
            {
                jumpToFallPlatform--;
            }

            if (jumpToFallPlatform == 0)
            {
                FallPlatform();
                StartCoroutine(DestroyObject(timeToDestroyPlatform));
            }
        }
    }

    private void FallPlatform()
    {
        rb = gameObject.AddComponent<Rigidbody>() as Rigidbody;
    }

    private IEnumerator DestroyObject(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}