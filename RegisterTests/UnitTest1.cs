using ServiceRegister.DTO;
using ServiceRegister.Models;
using ServiceRegister.Extensions;
using static ServiceRegister.Extensions.Extensions;

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
            List<ServiceInfo> list = new();
            ServiceInfoDTO dto = new()
            {
                Address = "http://1.1.1.1",
                Name = "valid name"
            };

            var addResult = list.Add(dto);
            Assert.That(addResult.Single(), Is.EqualTo(ValidationResult.Success));

            var addedInfo = list.Single();
            Assert.Multiple(() =>
            {
                Assert.That(addedInfo.Name, Is.EqualTo(dto.Name));
                Assert.That(addedInfo.Address?.OriginalString, Is.EqualTo(dto.Address));
            });
        }

        [Test]
        public void ListNull()
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            List<ServiceInfo> list = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            ServiceInfoDTO dto = new()
            {
                Address = "http://1.1.1.1",
                Name = "valid name"
            };

#pragma warning disable CS8604 // Possible null reference argument.
            Assert.Throws(typeof(ArgumentNullException), delegate { list.Add(dto); });
#pragma warning restore CS8604 // Possible null reference argument.
        }

        [Test]
        public void ObjectNull()
        {
            List<ServiceInfo> list = new();
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            ServiceInfoDTO dto = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

#pragma warning disable CS8604 // Possible null reference argument.
            var addResult = list.Add(dto);
#pragma warning restore CS8604 // Possible null reference argument.
            Assert.That(addResult.Single(), Is.EqualTo(ValidationResult.SourceNull));
        }

        [Test]
        public void Name_Empty()
        {
            List<ServiceInfo> list = new();
            ServiceInfoDTO dto = new()
            {
                Address = "http://1.1.1.1",
                Name = ""
            };

            var addResult = list.Add(dto);
            Assert.That(addResult.Single(), Is.EqualTo(ValidationResult.NameEmpty));
        }

        [Test]
        public void Name_Null()
        {
            List<ServiceInfo> list = new();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            ServiceInfoDTO dto = new()
            {
                Address = "http://1.1.1.1",
                Name = null
            };
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            var addResult = list.Add(dto);
            Assert.That(addResult.Single(), Is.EqualTo(ValidationResult.NameEmpty));
        }

        [Test]
        public void Name_TooShort()
        {
            List<ServiceInfo> list = new();
            ServiceInfoDTO dto = new()
            {
                Address = "http://1.1.1.1",
                Name = "n"
            };

            var addResult = list.Add(dto);
            Assert.That(addResult.Single(), Is.EqualTo(ValidationResult.NameTooShort));
        }

        [Test]
        public void Name_AlreadyExist()
        {
            List<ServiceInfo> list = new()
            {
                new ServiceInfo()
                {
                    Address =  new Uri("http://1.1.1.1/"),
                    Name = "first",
                }
            };

            ServiceInfoDTO toAdd = new()
            {
                Address = "http://2.2.2.2/",
                Name = "first",
            };

            var addResult = list.Add(toAdd);
            Assert.That(addResult.Single(), Is.EqualTo(ValidationResult.NameAlreadyExist));
        }

        [Test]
        public void Address_AlreadyExist()
        {
            List<ServiceInfo> list = new()
            {
                new ServiceInfo()
                {
                    Address =  new Uri("http://1.1.1.1/"),
                    Name = "first",
                }
            };

            ServiceInfoDTO toAdd = new()
            {
                Address = "http://1.1.1.1/",
                Name = "second",
            };

            var addResult = list.Add(toAdd);
            Assert.That(addResult.Single(), Is.EqualTo(ValidationResult.AddressAlreadyExist));
        }

        [Test]
        public void Address_Empty()
        {
            List<ServiceInfo> list = new();
            ServiceInfoDTO dto = new()
            {
                Address = "",
                Name = "valid name"
            };

            var addResult = list.Add(dto);
            Assert.That(addResult.Single(), Is.EqualTo(ValidationResult.AddressBadFormat));
        }

        [Test]
        public void Address_Format1()
        {
            List<ServiceInfo> list = new();
            ServiceInfoDTO dto = new()
            {
                Address = "somestring",
                Name = "valid name"
            };

            var addResult = list.Add(dto);
            Assert.That(addResult.Single(), Is.EqualTo(ValidationResult.AddressBadFormat));
        }

        [Test]
        public void Address_Format2()
        {
            List<ServiceInfo> list = new();
            ServiceInfoDTO dto = new()
            {
                Address = "1.1.1.1",
                Name = "valid name"
            };

            var addResult = list.Add(dto);
            Assert.That(addResult.Single(), Is.EqualTo(ValidationResult.AddressBadFormat));
        }
    }
}