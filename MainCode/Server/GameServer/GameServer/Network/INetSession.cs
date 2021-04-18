using SkillBridge.Message;
using System;


namespace Network
{
    public interface INetSession
    {
        byte[] GetResponse();
    }
}