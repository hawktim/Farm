namespace FarmConsoleApp
{
    public interface IController: IControlConsole
    {
        FEventArgs GetActions(FEventArgs arg);
    }

    public interface IControlConsole
    {
        string Code { get; }
        string Caption { get; }
    }
}
