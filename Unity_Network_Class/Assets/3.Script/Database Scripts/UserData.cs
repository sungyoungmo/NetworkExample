using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;


namespace MyProject
{

	public enum CharClass
    {
		none = 0,
		Warrior = 1,
		Wizard = 2,
		Rogue = 3
    }

    public class UserData
	{
		
		public string email;
		public string name;
		public CharClass charClass;
		public int level;
		public string profileText;
        public bool is_online;

        // 읽기 전용
        // public int uid { get; private set; } 과 같음
        public int UID => uid;


		private int uid;
		private string passwd;


        public UserData(DataRow row) : this
            (
                row["email"].ToString(),
                row["name"].ToString(),
                (CharClass)int.Parse(row["class"].ToString()),
                int.Parse(row["LEVEL"].ToString()),
                row["profile_text"].ToString(),
                int.Parse(row["uid"].ToString()),
                row["pw"].ToString(),
                Convert.ToBoolean(int.Parse(row["is_online"].ToString()))
            ){}

        public UserData(string email, string name, CharClass charClass, int level, string profileText, int uid, string passwd, bool is_online)
        {
            this.email = email;
            this.name = name;
            this.charClass = charClass;
            this.level = level;
            this.profileText = profileText;
            this.uid = uid;
            this.passwd = passwd;
            this.is_online = is_online;
        }

        public bool ComparePasswd(string passwd)
        {
			return this.passwd.Equals(passwd);
        }


	}
}
