using Microsoft.Xrm.Tooling.Connector;
using System.Configuration;

namespace AwaraIT.BCS.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.Run();
        }

        public static CrmServiceClient GetCrmClient() => new CrmServiceClient(ConfigurationManager.ConnectionStrings["Crm"].ConnectionString);
    }
}
