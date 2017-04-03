using System.Diagnostics;
using System.Threading.Tasks;
using BleLab.Services;
using LiteDB;

namespace BleLab.Model
{
    public abstract class InfoObjectBase
    {
        [BsonIgnore]
        protected InfoManager InfoManager;

        public void Initialize(InfoManager infoManager)
        {
            InfoManager = infoManager;
        }

        public void Save()
        {
            Debug.Assert(InfoManager != null);
            DoSave();
        }

        public Task SaveAsync()
        {
            return Task.Run(() => DoSave());
        }

        protected abstract void DoSave();
    }
}
