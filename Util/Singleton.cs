using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�� ������� ��� ���ٴϴ� �̱���
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(T)) as T;

                if (instance == null)
                {
                    GameObject go = AddressableManager.Instance.Instantiate(typeof(T).Name, null);

                    if (go == null)
                    {
                        go = Instantiate(Resources.Load<GameObject>($"Prefab/Popup/{typeof(T).Name}"));
                    }

                    if (go == null)
                    {
                        Debug.LogError(string.Format("this {0} Singleton Error", typeof(T).Name));

                        return null;
                    }

                    instance = go.GetComponent<T>();

                    if (instance == null)
                    {
                        Debug.LogError(string.Format("this {0} Singleton Error", typeof(T).Name));
                    }
                }
            }
            return instance;
        }
    }
    private void OnEnable()
    {
        var instance = FindObjectsOfType<MonoSingleton<T>>();

        if (instance.Length != 1)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this);
    }

    private void OnApplicationQuit()
    {
        Destroy(this.gameObject);
    }
}

//�������� ����ϴ� �̱���
public class MonoSingletonInScene<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(T)) as T;
            }

            return instance;
        }
    }
    private void OnApplicationQuit()
    {
        Destroy(this.gameObject);
    }
}

//������Ʈ�� ������� �ʴ� �̱���
public class Singleton<T> where T : Singleton<T>, new()
{
    private static T instance;
    public static T Instance 
    {
        get
        {
            if(instance == null)
            {
                instance = new T();
            }

            return instance;
        }
    }
}
