using System;
using System.Threading.Tasks;
using NeuToDo.Services;
using NUnit.Framework;

namespace NeuToDo.UnitTest.Services
{
    public class CourseEventStorageTest
    {
        [Test]
        public async Task TestGetAll()
        {
            var courseEventStorage = new CourseEventStorage();
            var eventList = await courseEventStorage.GetAll();
            foreach (var VARIABLE in eventList)
            {
                Console.WriteLine(VARIABLE.ToString());
            }
        }
    }
}