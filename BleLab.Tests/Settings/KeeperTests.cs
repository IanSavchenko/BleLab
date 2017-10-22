using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BleLab.Settings;
using HyperMock;
using Xunit;

namespace BleLab.Tests.Settings
{
    public class KeeperTests
    {
        private TestKeeper _instance;
        private Mock<IKeeperStorage> _storageMock = Mock.Create<IKeeperStorage>();

        public KeeperTests()
        {
            _instance = new TestKeeper(_storageMock.Object);
        }

        [Fact]
        public void ReadingSimpleProperty_IfNotDefined_ShouldReturnDefault()
        {
            Assert.True(_instance.JustProperty == default(bool));

            _storageMock.Verify(s => s.HasKey(nameof(TestKeeper.JustProperty)), Occurred.Once());
            _storageMock.Verify(s => s.GetValue(nameof(TestKeeper.JustProperty)), Occurred.Never());
        }

    }

    public class TestKeeper : Keeper
    {
        public TestKeeper(IKeeperStorage storage) : base(storage)
        {
        }

        public bool JustProperty
        {
            get => Get<bool>();
            set => Set(value);
        }

        [KeeperProperty]
        public bool PropertyWithAttribute
        {
            get => Get<bool>();
            set => Set(value);
        }

        [KeeperProperty(PersistentKey = "MyKey")]
        public bool PropertyWithPersistentKey
        {
            get => Get<bool>();
            set => Set(value);
        }

        [KeeperProperty(DefaultValue = true)]
        public bool PropertyWithDefaultValue
        {
            get => Get<bool>();
            set => Set(value);
        }
    }
}
