using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSocketParameter
{
    public class Ping
    {
        public Ping()
        {

        }
    }

    public class Connect
    {
        private string _uid;
        private string _session;

        public string uid => _uid;
        public string session => _session;

        public void SetUID(string _uid)
        {
            this._uid = _uid;
        }

        public void SetSession(string _session)
        {
            this._session = _session;
        }
    }

    public class Stage
    {
        private string _uid;

        public string UID => _uid;

        public void SetUID(string _uid)
        {
            this._uid = _uid;
        }
    }

    public class EnemyDropCoin
    {        
        private string _uid;

        public string UID => _uid;

        /// <summary>
        /// 죽은 캐릭터 UID
        /// </summary>
        /// <param name="_uid"></param>
        public void SetUID(string _uid)
        {
            this._uid = _uid;
        }
    }

    public class StatUp
    {
        private string _type;

        public string TYPE => _type;

        public void SetType(string _type) 
        {
            this._type = _type;
        }
    }

    public class ADGacha
    {
        private string _uid;

        public string UID => _uid;

        /// <summary>
        /// 받을 광고 번호
        /// </summary>
        /// <param name="_uid"></param>
        public void SetUID(string _uid)
        {
            this._uid = _uid;
        }
    }

    public class ConnectReward
    {
        private bool isAdBonus = false;

        public bool BONUS => isAdBonus;

        /// <summary>
        /// 접속 보상 받을때 광고 봤는지 확인
        /// </summary>
        /// <param name="isAdBonus"></param>
        public void SetAdBonus(bool isAdBonus)
        {
            this.isAdBonus = isAdBonus;
        }
    }

    public class Shop
    {
        private string _uid;

        public string UID => _uid;

        public void SetUID(string _uid)
        {
            this._uid = _uid;
        }
    }

    public class PacketHero
    {
        private string _uid;
        private string _position;

        public string UID => _uid;
        public string POSITION => _position;

        public void SetUID(string _uid)
        {
            this._uid = _uid;
        }
        public void SetPosition(string _position)
        {
            this._position = _position;
        }
    }

}

