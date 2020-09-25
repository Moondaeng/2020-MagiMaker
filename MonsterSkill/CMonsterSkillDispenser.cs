using UnityEngine;
using System.Collections;

public class CMonsterSkillDispenser : MonoBehaviour
{
    public GameObject[] _prefabs;
    private GameObject _prefabObject;
    private CParitcleSkillBase _prefabScript;
    public void BeginEffect(int index)
    {
        Vector3 pos;
        float yRot = transform.rotation.eulerAngles.y;
        Vector3 forwardY = Quaternion.Euler(0.0f, yRot, 0.0f) * Vector3.forward;
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 up = transform.up;
        Quaternion rotation = Quaternion.identity;
        _prefabObject = GameObject.Instantiate(_prefabs[index]);
        _prefabScript = _prefabObject.GetComponent<CParticleConstant>();

        if (_prefabScript == null)
        {
            _prefabScript = _prefabObject.GetComponent<CParitcleSkillBase>();
            if (_prefabScript._isProjectile)
            {
                rotation = transform.rotation;
                pos = transform.position + forward + right + up;
            }
            else
            {
                pos = transform.position + (forwardY * 10.0f);
            }
        }
        else
        {
            pos = transform.position + (forwardY * 5.0f);
            rotation = transform.rotation;
            pos.y = 0.0f;
        }
        _prefabObject.transform.position = pos;
        _prefabObject.transform.rotation = rotation;

        _prefabScript._attackPower = gameObject.GetComponent<CharacterPara>().GetRandomAttack();
        _prefabScript._skillUsingUser = gameObject;
    }
}
