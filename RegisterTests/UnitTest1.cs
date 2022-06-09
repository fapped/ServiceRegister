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

            Assert.That(addResult.Count(), Is.EqualTo(1));
            Assert.That(list.Count, Is.EqualTo(1));

            var addedInfo = list.Single();

            Assert.That(addedInfo.Name, Is.EqualTo(dto.Name));
            Assert.That(addedInfo.Address?.OriginalString, Is.EqualTo(dto.Address));
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

            Assert.That(addResult.Count(), Is.EqualTo(1));
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

            Assert.That(addResult.Count(), Is.EqualTo(1));
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

            Assert.That(addResult.Count(), Is.EqualTo(1));
            Assert.That(addResult.Single(), Is.EqualTo(ValidationResult.AddressBadFormat));
        }

        //[Test]
        //public void Name_Empty()
        //{
        //    List<ServiceInfo> list = new();
        //    ServiceInfoDTO dto = new()
        //    {
        //        Address = "http://1.1.1.1",
        //        Name = ""
        //    };

        //    var addResult = list.Add(dto);

        //    Assert.That(addResult.Count(), Is.EqualTo(1));
        //    Assert.That(addResult.Single(), Is.EqualTo(ValidationResult.NameBadFormat));
        //}
    }
}