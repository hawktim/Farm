using System;

namespace FarmConsoleApp
{
    public class FEventArgs : EventArgs
    {
        public string Code { get; set; } = "";
        public string Select { get; set; } = "";
        public TypeAction TypeAction {  get; set; }
    }

}
