using System.Windows.Forms;

namespace EFSAdvent
{
    public class Logger
    {
        private readonly TextBox _output;

        public Logger(TextBox textBox)
        {
            _output = textBox;
        }

        public void Clear()
        {
            _output.Clear();
        }

        public void AppendText(string text)
        {
            _output.AppendText(text);
        }

        public void AppendLine(string text)
        {
            _output.AppendText("\r\n" + text);
        }
    }
}
