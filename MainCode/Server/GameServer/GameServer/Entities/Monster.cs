using GameServer.Core;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    class Monster : CharacterBase
    {
        public Monster(int configId, int level, Vector3Int pos, Vector3Int dir) : base(CharacterType.Monster, configId, level, pos, dir)
        {
            this.Define = DataManager.Instance.Characters[configId];
            this.Info = new NCharacterInfo();
            this.Info.Type = CharacterType.Monster;
            this.Info.Level = level;
            this.Info.configId = configId;
            this.Info.Entity = this.EntityData;
            this.Info.Class = this.Define.Class;
            this.Position = pos;
            this.Direction = dir;
        }
    }
}
