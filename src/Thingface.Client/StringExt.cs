#if (NETMF43 || NETMF44)
namespace Thingface.Client
{
    public static class StringExt
    {
        public static bool IsNullOrWhiteSpace(string input)
        {
            if (input == null)
            {
                return true;
            }
            if (input.Trim().Length == 0)
            {
                return true;
            }
            return false;
        }
    }
}
#endif