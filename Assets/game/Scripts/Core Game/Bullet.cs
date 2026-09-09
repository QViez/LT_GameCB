using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float lifeTime = 4f;
    [SerializeField] private float timeAfterCollision = 1.5f;
    [SerializeField] private float gravityDelay = 0.5f; 

    private Rigidbody rb;
    private Coroutine returnCoroutine;
    private Coroutine gravityCoroutine;
    private bool hasCollided = false;

    // Delegate để ObjectPool đăng ký lắng nghe
    public Action<GameObject> OnRelease;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        hasCollided = false;
        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        if (gravityCoroutine != null) StopCoroutine(gravityCoroutine);
        gravityCoroutine = StartCoroutine(EnableGravityRoutine(gravityDelay));

        StartReturnTimer(lifeTime);
    }

    private void OnDisable()
    {
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }

        if (gravityCoroutine != null)
        {
            StopCoroutine(gravityCoroutine);
            returnCoroutine = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
            if (hasCollided) return;
        hasCollided = true;


        if (gravityCoroutine != null) StopCoroutine(gravityCoroutine);
        if (rb != null) rb.useGravity = true;

        StartReturnTimer(timeAfterCollision);
    }

    private IEnumerator EnableGravityRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (rb != null && !hasCollided)
        {
            rb.useGravity = true;
        }
    }
    private void StartReturnTimer(float delay)
    {
        if (returnCoroutine != null) StopCoroutine(returnCoroutine);
        returnCoroutine = StartCoroutine(ReturnToPoolRoutine(delay));
    }
    private IEnumerator ReturnToPoolRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Release();
    }
    private void Release()
    {
        if (OnRelease != null)
        {
            OnRelease.Invoke(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}