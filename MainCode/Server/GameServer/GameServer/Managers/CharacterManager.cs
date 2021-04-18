using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Entities;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class CharacterManager:Singleton<CharacterManager>
    {
        public Dictionary<int, Character> Chars = new Dictionary<int, Character>();
        public Character AddChar(TCharacter Tchar)
        {

            Character character = new Character(CharacterType.Player, Tchar);

            //创建到EntityManager，并给其添加唯一的EntityId
            EntityManager.Instance.AddEntity(Tchar.MapID, character);
            //---

            //将由EntityManager生成的entityId赋值给网络中的NChar.entityId
            character.Info.entityId = character.entityId;
            //---

            this.Chars[character.Id] = character;

            return character;

        }
        public void RemoveChar(int chaId)
        {

            EntityManager.Instance.RemoveEntiy(Chars[chaId].Info.mapId, Chars[chaId]);//Character.Data就是TChar，Character.Info.mapId 与TChar.MapID相等
            Chars.Remove(chaId);
            Log.Info("key of CharManager:[" + chaId + "] has Deleted!");


        }

        public Character GetCharacter(int characterId)
        {
            Character character = null;
            this.Chars.TryGetValue(characterId, out character);
            return character;
        }
    }
}
