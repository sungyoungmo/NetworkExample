using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyProject;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public UserData data;

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this);

    }


    public void LoadData(UserData data)
    {
        // ������ ������ �ʾ���
        // DatabaseManager�� DonDestroy���� �� 
        // ��� �־�� �ϳ�

        // ��� �� �� ����
        this.data = data;
    }
}
