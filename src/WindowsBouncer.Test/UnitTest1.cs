using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WindowsBouncer.Core;
using WindowsBouncer.Persistence;

namespace WindowsBouncer.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //var dbb = new LiteDBDataService();
            //var a = dbb.AddLoginAttempt(new LoginAttempt { Ip = 3232235819, Login = "asd" });
            //var b = dbb.GetLoginAttempt(3232235819);

            var eventReader = new EventReader();
            var aa = eventReader.GetFailedLoginAttempts(DateTime.Now.Subtract(TimeSpan.FromMinutes(180)));
        }
    }
}