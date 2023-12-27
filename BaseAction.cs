using System;

namespace FarmConsoleApp
{
    public class ActionHelper: IControlConsole
    {
        private readonly Action<EventArgs> _func;
        public string Caption { get; }
        public TypeAction TypeAction { get; }

        public string Code { get; }
        
        public ActionHelper(TypeAction typeAction, string caption, Action<EventArgs> func, string code="") 
        {
            Code = (string.IsNullOrEmpty(code) ? getActionCode(typeAction) : code).ToLower();
            TypeAction = typeAction;
            Caption = caption;
            _func = func;
        }
        private string getActionCode(TypeAction typeAction)
        {
            switch (typeAction)
            {
                case TypeAction.Add: return "+";
                case TypeAction.Remove: return "-";
                case TypeAction.Select: return "*";
                default:
                    return "l";
            }
        }
        public void Execute(EventArgs value)
        {
            _func.Invoke(value);
        }
    }

}
