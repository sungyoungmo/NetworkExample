using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySqlConnector;
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace MyProject
{
    public class DatabaseManager : MonoBehaviour
    {
        //private string serverIP = "43.203.218.203";
        private string serverIP = "127.0.0.1";


        private string dbName = "game";
        private string tableName = "users";
        private string rootPasswd = "qwe123"; // 테스트 시에 활용할 수 있지만 보안에 취약하므로 주의

        private MySqlConnection conn; // mysql DB와 연결 상태를 유지하는 객체.

        public static DatabaseManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            DBConnect();
        }

        public void DBConnect()
        {
            // 집에선 3307 , 학원에선 3306 port 변경해야함
            string config = $"server={serverIP};port=3306;database={dbName};" + $"uid=root;pwd={rootPasswd};charset=utf8;";

            conn = new MySqlConnection(config);

            conn.Open();
        }

        // 로그인을 하려고 할 때, 로그인 쿼리를 날린 즉시 데이터가 오지 않을 수 있으므로, 로그인이 완료되었을 때 호출될 함수를 파라미터로 함께 받아주도록 함.
        public void Login(string email, string password, Action<UserData> successCallback, Action failureCallback)
        {
            string pwhash = "";

            SHA256 sha256 = SHA256.Create();
            byte[] hashArray = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            foreach (byte b in hashArray)
            {
                pwhash += $"{b:X2}";
                //pwhash += b.ToString("X2");
                // 둘 중 아무렇게나 상관없음
            }

            sha256.Dispose();

            print(pwhash);



            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"SELECT * FROM {tableName} WHERE email = '{email}' AND pw = '{password}'";

            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);

            DataSet set = new DataSet();

            dataAdapter.Fill(set);

            bool isLoginSuccess = set.Tables.Count > 0 && set.Tables[0].Rows.Count > 0;

            if (isLoginSuccess)
            {
                // 로그인 성공(email과 pw 값이 동시에 일치하는 행이 존재함)
                DataRow row = set.Tables[0].Rows[0];

                //print(row["LEVEL"]);
                //print(row["level"]);

                UserData data = new UserData(row);

                //print(data.email);

                successCallback?.Invoke(data);
            }
            else
            {
                // 로그인 실패
                failureCallback?.Invoke();
            }


        }

        public void Register(string email, string password, CharClass charClass, Action successCallback, Action failureCallback)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;

            cmd.CommandText = $"SELECT email FROM users WHERE email = '{email}'";

            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);

            DataSet set = new DataSet();

            dataAdapter.Fill(set);

            bool isRegisterSuccess = set.Tables.Count > 0 && set.Tables[0].Rows.Count == 0;
            
            if (isRegisterSuccess && ((int)charClass < 4 && (int)charClass >= 0))
            {
                cmd.CommandText = $"INSERT INTO users(email, pw, LEVEL, class) VALUES('{email}', '{password}', 1,{(int)charClass})";
                int queryCount = cmd.ExecuteNonQuery();

                if (queryCount > 0)
                {
                    successCallback?.Invoke();
                }
                else
                {
                    failureCallback?.Invoke();
                }

                
            }
            else
            {
                failureCallback?.Invoke();
            }
        }

    }
}
