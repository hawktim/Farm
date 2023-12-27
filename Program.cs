namespace FarmConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IController defaultController = new DefaultControler();
            defaultController.GetActions(new FEventArgs { TypeAction = TypeAction.List });
        }
    }
}
