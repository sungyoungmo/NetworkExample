using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using LitJson;
using System.IO;


public enum Type
{
    Server = 0,
    Client
}

public class jdata
{
    public string ServerIP { get; set; }
    public jdata(string ip)
    {
        ServerIP = ip;
    }
}

public class ServerChecker : MonoBehaviour
{
    public Type type;
    private NetworkManager networkManager;

    [SerializeField]
    private string path;

    private void Awake()
    {
        path = Application.dataPath + "/Server";
    }
    private void Start()
    {
        networkManager = GetComponent<NetworkManager>();
        networkManager.networkAddress = NetworkIP_Set();


        if (type.Equals(Type.Server))
        {
            Start_Server();
        }
        else
        {
            Start_Client();
        }

    }
    private string JsonCreate()
    {
        List<jdata> JDatas = new List<jdata>();
        JDatas.Add(new jdata("127.0.0.0"));

        JsonData data = JsonMapper.ToJson(JDatas);
        return data.ToString();
    }
    private string NetworkIP_Set()
    {
        string JsonPath = path + "/Server.json";
        if (!File.Exists(path))
        {
            Directory.CreateDirectory(path);
            if (!File.Exists(JsonPath))
            {
                File.WriteAllText(JsonPath, JsonCreate());
            }
        }

        string JsonString = File.ReadAllText(JsonPath);
        JsonData item = JsonMapper.ToObject(JsonString);
        string ip = item[0]["ServerIP"].ToString();
        return ip;
    }

    public void Start_Server()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Debug.Log("WebGL Server ¾ÈµÊ...¤¢¤¹");
        }
        else
        {
            networkManager.StartServer();
            Debug.Log($"{networkManager.networkAddress} start SErver");
            NetworkServer.OnConnectedEvent +=
                (NetworkConnectionToClient) =>
                {
                    Debug.Log($"new client connect : {NetworkConnectionToClient.address}");
                };
            NetworkServer.OnDisconnectedEvent += (NetworkConnectionToClient) =>
            {
                Debug.Log($"client disconnect : {NetworkConnectionToClient.address}");
            };
        }
    }

    public void Start_Client()
    {
        networkManager.StartClient();
        Debug.Log($"{networkManager.networkAddress} startclient");
    }

    private void OnApplicationQuit()
    {
        if (NetworkClient.isConnected)
        {
            networkManager.StopClient();
        }

        if (NetworkServer.active)
        {
            networkManager.StopServer();
        }

    }
}
