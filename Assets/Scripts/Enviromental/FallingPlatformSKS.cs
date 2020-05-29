using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatformSKS : MonoBehaviour
{
    [SerializeField] private int jumpToFallPlatform = 10;

    [SerializeField] private float timeToDestroyPlatform = 4f;

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
        
    }

    private IEnumerator DestroyObject(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
