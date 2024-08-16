using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.IO;

// Table 데이터 자료형 만들기
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

    public MySqlConnection connection;  // DB에 연결하기 위한 클래스
    public MySqlDataReader reader;      // 데이터를 직접적으로 읽어오는 녀석

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
                값을 예외처리 -> 어떠한 조건을 검사하기 위한 예외처리

            try - catch
                프로그램의 흐름에다가 어떠한 에러가 나타났을 때 예외처리 
                -> 프로그램의 흐름에다가 예외처리

        */

        try
        {
            if (Serverinfo.Equals(string.Empty))
            {
                Debug.Log("serverInfo가 없어... 제대로 해");
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

        JsonData data = JsonMapper.ToJson(jData_DB);    // Json으로 컨벌팅

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
        // 직접적으로 DB에서 데이터를 가지고 오는 메소드

        /*
            만약에 조회되는 데이터가 없다면 반환값 false
            조회가 되는 데이터가 있다면 true 


           쿼리문에 직접적으로 데이터를 넣는다.
           그 다음 위에 캐싱해놓은 info에다 넣는다.

            1. 커넥션 상태확인 -> 메소드화
            2. Reader의 상태가 현재 읽고 있는 상황인지 확인
            3. 데이터를 다 읽었다면 -> reader의 상태를 Close
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
                // reader가 읽은 데이터가 1개 이상입니까? 라는 뜻
                // 읽은 데이터를 하나씩 나열해서 데이터를 자료형으로 변형

                while (reader.Read())
                {
                    // 읽은 데이터가 남아 있다면
                    string name = (reader.IsDBNull(0)) ? string.Empty : (string)reader["User_Name"];
                    string pass = (reader.IsDBNull(1)) ? string.Empty : (string)reader["User_Password"];
                    string phone = (reader.IsDBNull(2)) ? string.Empty : (string)reader["User_PhoneNum"];

                    if (!name.Equals(string.Empty) || !pass.Equals(string.Empty))
                    {
                        // 로그인 성공
                        info = new UserInfo(name, pass, phone);

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }

                        return true;
                    }
                    else
                    {
                        // 로그인 실패
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