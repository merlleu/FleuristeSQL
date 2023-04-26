namespace FleurConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BelleFleurLib.DB.GetConnection();
            
            ModuleConnect moduleConnect = new ModuleConnect();
            moduleConnect.Menu();
        }
    }
}