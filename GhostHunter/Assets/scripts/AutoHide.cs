using UnityEngine;

public class AutoHide : MonoBehaviour
{
    public float Delay = 5f;//delay in seconds

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void OnEnable()
    {
        Invoke("autoHide", Delay);
    }

    private void autoHide()
    {
        gameObject.SetActive(false);
    }
}
