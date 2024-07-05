using UnityEngine;

public class BoxController : MonoBehaviour
{
    private Animator anim;
    private bool isDestroyed = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void DestroyBox()
    {
        if (!isDestroyed)
        {
            isDestroyed = true;
            anim.SetTrigger("Destroy");
            Destroy(gameObject, 0.5f);
        }
    }


    // Xử lý khi chân nhân vật chạm vào đỉnh của Box
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Character"))
        {
            DestroyBox(); // Phá hủy Box
        }
    }

}
