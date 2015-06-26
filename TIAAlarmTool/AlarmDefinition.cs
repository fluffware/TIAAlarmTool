using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIAAlarmTool
{
    class AlarmDefinition
    {
        [FlagsAttribute] 
        public enum Option : int
        {
            None = 0,
            Silent = 1,
            AutoAck = 2
        };

     
        public string Name {get;set;}
        public string Text { get; set; }
       
        public int ID { get; set; }

        public Option Options { get; set; }
        public AlarmDefinition(string name, string text)
        {
            Name = name;
            Text = text;
            ID = -1;
            Options = Option.None;
        }


    }
}
