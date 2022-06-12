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
            List<ServiceInfo> list = null;
            ServiceInfoDTO dto = new()
            {
                Address = "http://1.1.1.1",
                Name = "valid name"
            };

            Assert.Throws(typeof(ArgumentNullException), delegate { list.Add(dto); });
        }

        [Test]
        public void ObjectNull()
        {
            List<ServiceInfo> list = new();
            ServiceInfoDTO dto = null;

            var addResult = list.Add(dto);
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
            ServiceInfoDTO dto = new()
            {
                Address = "http://1.1.1.1",
                Name = null
            };

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