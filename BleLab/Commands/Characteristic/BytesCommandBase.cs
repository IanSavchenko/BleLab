using BleLab.Model;

namespace BleLab.Commands.Characteristic
{
    public abstract class BytesCommandBase : CharacteristicCommandBase
    {
        public BytesCommandBase(CharacteristicInfo characteristicInfo) : base(characteristicInfo)
        {
        }

        public byte[] Bytes { get; protected set; }
    }
}
