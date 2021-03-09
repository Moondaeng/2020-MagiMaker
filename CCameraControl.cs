using UnityEngine;

public class CCameraControl : MonoBehaviour
{
    public GameObject _player;

    private Vector3 offset;

    // Start is called before the first frame update
    private void Start()
    {
        offset = transform.position - _player.transform.position;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        transform.position = _player.transform.position + offset;
    }
}