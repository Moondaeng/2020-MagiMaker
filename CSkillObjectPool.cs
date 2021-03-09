using System.Collections.Generic;
using UnityEngine;

public class CSkillObjectPool : MonoBehaviour
{
    public static CSkillObjectPool instance;
    public Dictionary<GameObject, List<GameObject>> skillObjectDict = new Dictionary<GameObject, List<GameObject>>();

    #region 초기 설정
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        LoadAllSkillObject();
    }

    private void LoadAllSkillObject()
    {
        Debug.Log("LoadAllSkillObj");
        GameObject[] skills = Resources.LoadAll<GameObject>("SkillPrefab");
        foreach (var skill in skills)
        {
            AddSkillObject(skill);
        }
    }

    private GameObject AddSkillObject(GameObject skill)
    {
        if (!skillObjectDict.ContainsKey(skill))
        {
            skillObjectDict.Add(skill, new List<GameObject>());
        }

        if (skillObjectDict.TryGetValue(skill, out var skillObjects))
        {
            var skillObject = Instantiate(skill);
            skillObject.transform.SetParent(transform);
            // 구현 필요 : 생성한 Object에 대한 초기 설정
            skillObjects.Add(skillObject);
            skillObject.SetActive(false);
            return skillObject;
        }

        // Error Case
        Debug.Log($"{skill.name} object add error");
        return null;
    }
    #endregion

    #region 스킬 사용 관련
    public GameObject GetAvailableSkillObject(GameObject skill)
    {
        GameObject skillObject;
        if (skillObjectDict.TryGetValue(skill, out var skillObjectList)
            && (skillObject = skillObjectList.Find(obj => obj.activeSelf == false)) != null)
        {
            return skillObject;
        }
        return AddSkillObject(skill);
    }
    #endregion

    /// <summary>
    /// 맵에 있는 스킬 오브젝트를 전부 비활성화한다
    /// </summary>
    public void CleanAllSkillObject()
    {
        foreach (var objects in skillObjectDict.Values)
        {
            objects.ForEach(skillObject => skillObject.SetActive(false));
        }
    }
}
