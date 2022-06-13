using ServiceRegister.DTO;
using ServiceRegister.Models;
using ServiceRegister.Extensions;
using static ServiceRegister.Extensions.Extensions;
using ServiceRegister.Helpers;
using static ServiceRegister.Helpers.ServiceCollector;

namespace RegisterTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AddInfo()
        {
            ServiceCollector collector = new();
            ServiceInfoDTO dto = new()
            {
                Address = "http://1.1.1.1",
                Name = "valid name"
            };

            var addResult = collector.Add(dto);
            Assert.That(addResult.Single(), Is.EqualTo(ValidationResult.Success));

            var addedInfo = collector.Services().Single();
            Assert.Multiple(() =>
            {
                Assert.That(addedInfo.Name, Is.EqualTo(dto.Name));
                Assert.That(addedInfo.Address?.OriginalString, Is.EqualTo(dto.Address));
            });
        }

        [Test]
        public void ObjectNull()
        {
            ServiceCollector collector = new();
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            ServiceInfoDTO dto = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

#pragma warning disable CS8604 // Possible null reference argument.
            var addResult = collector.Add(dto);
#pragma warning restore CS8604 // Possible null reference argument.
            Assert.That(addResult.Single(), Is.EqualTo(ValidationResult.SourceNull));
        }

        [Test]
        public void Name_Empty()
        {
            ServiceCollector collector = new();
            ServiceInfoDTO dto = new()
            {
                Address = "http://1.1.1.1",
                Name = ""
            };

            var addResult = collector.Add(dto);
            Assert.That(addResult.Single(), Is.EqualTo(ValidationResult.NameEmpty));
        }

        [Test]
        public void Name_Null()
        {
            ServiceCollector collector = new();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            ServiceInfoDTO dto = new()
            {
                Address = "http://1.1.1.1",
                Name = null
            };
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            var addResult = collector.Add(dto);
            Assert.That(addResult.Single(), Is.EqualTo(ValidationResult.NameEmpty));
        }

        [Test]
        public void Name_TooShort()
        {
            ServiceCollector collector = new();
            ServiceInfoDTO dto = new()
            {
                Address = "http://1.1.1.1",
                Name = "n"
            };

            var addResult = collector.Add(dto);
            Assert.That(addResult.Single(), Is.EqualTo(ValidationResult.NameTooShort));
        }

        [Test]
        public void Name_AlreadyExist()
        {
            ServiceCollector collector = new();
            collector.Add(new ServiceInfoDTO()
            {
                Address = "http://1.1.1.1/",
                Name = "first",
            });

            ServiceInfoDTO toAdd = new()
            {
                Address = "http://2.2.2.2/",
                Name = "first",
            };

            var addResult = collector.Add(toAdd);
            Assert.That(addResult.Single(), Is.EqualTo(ValidationResult.NameAlreadyExist));
        }

        [Test]
        public void Address_AlreadyExist()
        {
            ServiceCollector collector = new();
            collector.Add(new ServiceInfoDTO()
            {
                Address = "http://1.1.1.1/",
                Name = "first",
            });

            ServiceInfoDTO toAdd = new()
            {
                Address = "http://1.1.1.1/",
                Name = "second",
            };

            var addResult = collector.Add(toAdd);
            Assert.That(addResult.Single(), Is.EqualTo(ValidationResult.AddressAlreadyExist));
        }

        [Test]
        public void Address_Empty()
        {
            ServiceCollector collector = new();
            ServiceInfoDTO dto = new()
            {
                Address = "",
                Name = "valid name"
            };

            var addResult = collector.Add(dto);
            Assert.That(addResult.Single(), Is.EqualTo(ValidationResult.AddressBadFormat));
        }

        [Test]
        public void Address_Format1()
        {
            ServiceCollector collector = new();
            ServiceInfoDTO dto = new()
            {
                Address = "somestring",
                Name = "valid name"
            };

            var addResult = collector.Add(dto);
            Assert.That(addResult.Single(), Is.EqualTo(ValidationResult.AddressBadFormat));
        }

        [Test]
        public void Address_Format2()
        {
            ServiceCollector collector = new();
            ServiceInfoDTO dto = new()
            {
                Address = "1.1.1.1",
                Name = "valid name"
            };

            var addResult = collector.Add(dto);
            Assert.That(addResult.Single(), Is.EqualTo(ValidationResult.AddressBadFormat));
        }
    }
}