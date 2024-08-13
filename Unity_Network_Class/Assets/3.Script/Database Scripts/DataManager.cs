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
        // 서버에 보내질 않았음
        // DatabaseManager를 DonDestroy했을 때 
        // 얘는 있어야 하나

        // 없어도 될 것 같음
        this.data = data;
    }
}
