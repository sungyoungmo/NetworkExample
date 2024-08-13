using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public enum Type
{
	Server=0,
	Client
}

public class ServerChecker : MonoBehaviour
{
	public Type type;
	private NetworkManager networkManager;


	private void Start()
	{
		networkManager=GetComponent<NetworkManager>();

		if(type.Equals(Type.Server)) {
			Start_Server();
		}
        else
        {
            Start_Client();
			FindObjectOfType<DataManager>().data.is_online = true;
        }

    }

	public void Start_Server() {
		if(Application.platform==RuntimePlatform.WebGLPlayer) {
			Debug.Log("WebGL Server ¾ÈµÊ...¤¢¤¹");
		} 
		else {
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

	public void Start_Client() {
		networkManager.StartClient();
		Debug.Log($"{networkManager.networkAddress} startclient");
	}

	private void OnApplicationQuit() 
	{
		if (NetworkClient.isConnected) 
		{
			FindObjectOfType<DataManager>().data.is_online = false;
			networkManager.StopClient();
		}

		if(NetworkServer.active) {
			networkManager.StopServer();
		}

	}
}
