using FSALib;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace EFSAdvent
{
    public class TileStampFlowLayoutPanel : FlowLayoutPanel
    {
        private int _selectedIndex = -1;

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (_selectedIndex == value)
                    return;

                if (value >= Controls.Count || value < -1)
                    throw new ArgumentOutOfRangeException(nameof(value));


                if (_selectedIndex != -1)
                    Controls[_selectedIndex].BackColor = SystemColors.Control;

                if (value != -1)
                    Controls[value].BackColor = Color.LightBlue;

                _selectedIndex = value;
                OnSelectionChanged(EventArgs.Empty);
            }
        }

        public event EventHandler SelectionChanged
        {
            add
            {
                base.Events.AddHandler(nameof(SelectionChanged), value);
            }
            remove
            {
                base.Events.RemoveHandler(nameof(SelectionChanged), value);
            }
        }

        protected virtual void OnSelectionChanged(EventArgs e)
        {
            var handler = (EventHandler)base.Events[nameof(SelectionChanged)];
            handler?.Invoke(this, e);
        }

        public void Load(Stamp stamp, int index)
        {
            string stampFilePath = (string)Controls[index].Tag;
            using FileStream stampRaw = new FileStream(stampFilePath, FileMode.Open);
            stamp.BinaryDeserialize(stampRaw);
        }

        public void Add(string stampFilePath)
        {
            if (!File.Exists(stampFilePath))
                throw new FileNotFoundException("Stamp file not found.", stampFilePath);

            var panel = new Panel
            {
                Width = 78,
                Height = 78,
                BackColor = SystemColors.Control,
                BorderStyle = BorderStyle.FixedSingle,
                Tag = stampFilePath
            };

            string iconFilePath = Path.ChangeExtension(stampFilePath, ".png");
            if (File.Exists(iconFilePath))
            {
                var pictureBox = new PictureBox
                {
                    Image = Image.FromFile(iconFilePath),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Dock = DockStyle.Fill,
                    Width = panel.Width,
                };
                panel.Controls.Add(pictureBox);
            }

            var label = new Label
            {
                Text = Path.GetFileNameWithoutExtension(stampFilePath),
                Dock = DockStyle.Bottom,
                TextAlign = ContentAlignment.MiddleCenter,
                Width = panel.Width,
            };
            panel.Controls.Add(label);

            ToolTip tooltip = new ToolTip();
            foreach (Control child in panel.Controls)
            {
                child.Click += ClickHandler;
                tooltip.SetToolTip(child, label.Text);
            }

            Controls.Add(panel);

            void ClickHandler(object? sender, EventArgs e)
            {
                SelectedIndex = Controls.IndexOf(panel);
            }
        }

        public void Delete(int index)
        {
            if (index >= 0 && index < Controls.Count)
            {
                string stampFilePath = (string)Controls[index].Tag;

                if (_selectedIndex == index)
                    SelectedIndex = -1;
                else if (_selectedIndex > index)
                    _selectedIndex--;

                if (Controls[index].Controls[0] is PictureBox picture)
                {
                    picture.Image?.Dispose();
                    picture.Dispose();
                }
                Controls.RemoveAt(index);

                File.Delete(Path.ChangeExtension(stampFilePath, ".png"));
                File.Delete(stampFilePath);
            }
        }
    }
}
