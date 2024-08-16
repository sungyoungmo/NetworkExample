using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.IO;

// Table ������ �ڷ��� �����
public class UserInfo
{
    public string UserName { get; private set; }
    public string User_Password { get; private set; }
    public string User_PhoneNum { get; private set; }

    public UserInfo(string name, string password, string phoneNum)
    {
        UserName = name;
        User_Password = password;
        User_PhoneNum = phoneNum;
    }

}

public class SQLManager : MonoBehaviour
{
    public UserInfo info;

    public MySqlConnection connection;  // DB�� �����ϱ� ���� Ŭ����
    public MySqlDataReader reader;      // �����͸� ���������� �о���� �༮

    [SerializeField]
    private string DB_Path = string.Empty;

    public static SQLManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DB_Path = Application.dataPath + "/Database";
        string Serverinfo = Server_set();


        /*
            if
                ���� ����ó�� -> ��� ������ �˻��ϱ� ���� ����ó��

            try - catch
                ���α׷��� �帧���ٰ� ��� ������ ��Ÿ���� �� ����ó�� 
                -> ���α׷��� �帧���ٰ� ����ó��

        */

        try
        {
            if (Serverinfo.Equals(string.Empty))
            {
                Debug.Log("serverInfo�� ����... ����� ��");
                return;
            }
            connection = new MySqlConnection(Serverinfo);

            connection.Open();

            Debug.Log("DB Open Complete");

        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            
        }


    }

    private string Json_Create()
    {
        List<Jdata_DB> jData_DB = new List<Jdata_DB>();
        jData_DB.Add(new Jdata_DB());

        JsonData data = JsonMapper.ToJson(jData_DB);    // Json���� ������

        return data.ToString();
    }

    public string Server_set()
    {
        string filePath = DB_Path + "/config.json";

        if (!File.Exists(DB_Path))
        {
            Directory.CreateDirectory(DB_Path);
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, Json_Create());
            }
        }

        string JsonString = File.ReadAllText(filePath);

        JsonData data = JsonMapper.ToObject(JsonString);
        string serverInfo =
            $"Server = {data[0]["IP"]};" +
            $"Database = {data[0]["TableName"]};" +
            $"Uid = {data[0]["ID"]};" +
            $"PWd = {data[0]["PW"]};" +
            $"Port = {data[0]["Port"]};" +
            "CharSet = utf8;";

        return serverInfo;
    }


    private bool Connection_check(MySqlConnection c)
    {
        if (c.State != System.Data.ConnectionState.Open)
        {
            c.Open();
            if (c.State != System.Data.ConnectionState.Open)
            {
                return false;
            }
        }
        return true;
    }

    public bool Login(string id, string password)
    {
        // ���������� DB���� �����͸� ������ ���� �޼ҵ�

        /*
            ���࿡ ��ȸ�Ǵ� �����Ͱ� ���ٸ� ��ȯ�� false
            ��ȸ�� �Ǵ� �����Ͱ� �ִٸ� true 


           �������� ���������� �����͸� �ִ´�.
           �� ���� ���� ĳ���س��� info���� �ִ´�.

            1. Ŀ�ؼ� ����Ȯ�� -> �޼ҵ�ȭ
            2. Reader�� ���°� ���� �а� �ִ� ��Ȳ���� Ȯ��
            3. �����͸� �� �о��ٸ� -> reader�� ���¸� Close
        */


        try
        {
            if (!Connection_check(connection))
            {
                return false;
            }

            string sql_command = string.Format($@"SELECT User_Name, User_Password, User_PhoneNum FROM user_info WHERE User_Name = '{id}' AND User_Password = '{password}';");

            MySqlCommand cmd = new MySqlCommand(sql_command, connection);
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                // reader�� ���� �����Ͱ� 1�� �̻��Դϱ�? ��� ��
                // ���� �����͸� �ϳ��� �����ؼ� �����͸� �ڷ������� ����

                while (reader.Read())
                {
                    // ���� �����Ͱ� ���� �ִٸ�
                    string name = (reader.IsDBNull(0)) ? string.Empty : (string)reader["User_Name"];
                    string pass = (reader.IsDBNull(1)) ? string.Empty : (string)reader["User_Password"];
                    string phone = (reader.IsDBNull(2)) ? string.Empty : (string)reader["User_PhoneNum"];

                    if (!name.Equals(string.Empty) || !pass.Equals(string.Empty))
                    {
                        // �α��� ����
                        info = new UserInfo(name, pass, phone);

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }

                        return true;
                    }
                    else
                    {
                        // �α��� ����
                        break;

                    }
                }
            }

            if (!reader.IsClosed)
            {
                reader.Close();
            }
            return false;

        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);

            if (!reader.IsClosed)
            {
                reader.Close();
            }
            return false;
        }
    }

}

public class Jdata_DB
{
    public string IP { get; set; }
    public string TableName { get; set; }

    public string ID { get; set; }

    public string PW { get; set; }
    public string Port { get; set; }
    
    public Jdata_DB()
    {
        IP = "127.0.0.1";
        TableName = "Programming";
        ID = "root";
        PW = "qwe123";
        Port = "3306";

    }
}