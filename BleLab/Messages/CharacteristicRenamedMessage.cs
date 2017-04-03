using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BleLab.Model;

namespace BleLab.Messages
{
    public class CharacteristicRenamedMessage
    {
        public CharacteristicRenamedMessage(CharacteristicInfo characteristicInfo)
        {
            CharacteristicInfo = characteristicInfo;
        }

        public CharacteristicInfo CharacteristicInfo { get; }
    }
}
