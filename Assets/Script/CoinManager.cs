using System.Collections;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public float speed = 1f;
    public Transform targetTransform;
    public int value = 1;  // Giá trị của vàng này

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Character"))
        {
            CharacterController characterController = collision.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.collecCoin(value);
                characterController.UpdateCoinUI();
            }
            StartCoroutine(MoveToTarget());
        }
    }

    private IEnumerator MoveToTarget()
    {
        while (Vector3.Distance(transform.position, targetTransform.position) > 3f)
        {
            transform.position = Vector3.Lerp(transform.position, targetTransform.position, speed * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }
}
