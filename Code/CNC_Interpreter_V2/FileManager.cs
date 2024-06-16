using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNC_Interpreter_V2
{
    internal class FileManager
    {
        private int lineCounter;
        private int lines;
        private string file = "";

        public FileManager()
        {
        }

        public int LineCounter { get { return lineCounter; } }
        public int TotalLines { get { return lines; } }
        public string FileName { get { return file;} }
        public string SetFile {  set { file = value; lines = File.ReadLines(value).Count(); } }

        public string GetLine(int lineNumber)
        {
            return File.ReadLines(file).Skip(lineNumber).Take(1).First();
        }

        public string GetNext()
        {
            if(lineCounter >= lines)
            {
                Console.WriteLine("All lines done");
                return "";
            }

            string line = "";
            try
            {
                line = File.ReadLines(file).Skip(lineCounter).Take(1).First();
                lineCounter++;
            } catch(Exception e) {
                Console.WriteLine(e);
            }
            return line;
        }
    }
}
