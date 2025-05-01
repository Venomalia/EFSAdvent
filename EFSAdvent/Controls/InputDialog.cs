using System.Windows.Forms;

namespace EFSAdvent
{
    public static class InputDialog
    {
        public static string? Show(string title, string prompt, string defaultValue = "")
        {
            using var form = new Form()
            {
                Width = 300,
                Height = 150,
                Text = title,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                ShowInTaskbar = false,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var label = new Label() { Left = 10, Top = 10, Text = prompt, Width = 260 };
            var textBox = new TextBox() { Left = 10, Top = 35, Width = 260, Text = defaultValue };
            var okButton = new Button() { Text = "OK", Left = 200, Width = 70, Top = 70, DialogResult = DialogResult.OK };
            var cancelButton = new Button() { Text = "Cancel", Left = 120, Width = 70, Top = 70, DialogResult = DialogResult.Cancel };

            form.Controls.Add(label);
            form.Controls.Add(textBox);
            form.Controls.Add(okButton);
            form.Controls.Add(cancelButton);

            form.AcceptButton = okButton;
            form.CancelButton = cancelButton;

            return form.ShowDialog() == DialogResult.OK ? textBox.Text : null;
        }
    }
}
