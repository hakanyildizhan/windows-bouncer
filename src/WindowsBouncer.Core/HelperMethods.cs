using System.Net;

namespace WindowsBouncer.Core
{
    public static class HelperMethods
    {
        public static long IpStringToLong(string address)
        {
            return (long)(uint)IPAddress.NetworkToHostOrder(
                 (int)IPAddress.Parse(address).Address);
        }
    }
}
