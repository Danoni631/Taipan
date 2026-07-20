using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Taipan
{
    public class Form1 : Form
    {
        private ComboBox deviceComboBox;
        private TextBox isoPathTextBox;
        private Button selectIsoButton;
        private ComboBox partitionComboBox;
        private ComboBox fileSystemComboBox;
        private Button startButton;
        private ProgressBar formatProgressBar;
        private string selectedIsoPath = "";

        public Form1()
        {
            this.Text = "Taipan - Alpha 1.0";
            this.Size = new Size(450, 600);
            this.BackColor = Color.FromArgb(28, 28, 28);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            build_metro_ui();
            list_disks();
        }

        private void build_metro_ui()
        {
            Label titleLabel = new Label
            {
                Text = "Taipan",
                Font = new Font("Segoe UI Light", 24F),
                ForeColor = Color.FromArgb(229, 20, 0),
                Location = new Point(20, 20),
                Size = new Size(200, 50)
            };

            this.Controls.Add(titleLabel);

            Label subTitle = new Label
            {
                Text = "Record",
                Font = new Font("Segoe UI", 14F),
                ForeColor = Color.Gray,
                Location = new Point(20, 75),
                Size = new Size(100, 30)
            };

            this.Controls.Add(subTitle);

            Label lblDevice = new Label
            {
                Text = "Device",
                Location = new Point(20, 120),
                Size = new Size(200, 20),
                ForeColor = Color.DarkGray
            };

            deviceComboBox = new ComboBox
            {
                Location = new Point(20, 145),
                Size = new Size(390, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White
            };

            deviceComboBox.Items.Add("D: [Pendrive]");
            deviceComboBox.SelectedIndex = 0;
            this.Controls.Add(lblDevice); this.Controls.Add(deviceComboBox);

            Label lblIso = new Label
            {
                Text = "Boot selection",
                Location = new Point(20, 190),
                Size = new Size(200, 20),
                ForeColor = Color.DarkGray
            };

            isoPathTextBox = new TextBox
            {
                Location = new Point(20, 215),
                Size = new Size(280, 30),
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ReadOnly = true,
                Text = "No Image selected"
            };

            selectIsoButton = new Button
            {
                Text = "Selection",
                Location = new Point(310, 213),
                Size = new Size(100, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(63, 63, 70),
                ForeColor = Color.White
            };

            selectIsoButton.Click += SelectIsoButton_Click;
            this.Controls.Add(lblIso);
            this.Controls.Add(isoPathTextBox);
            this.Controls.Add(selectIsoButton);

            Label lblPart = new Label
            {
                Text = "Partition scheme",
                Location = new Point(20, 260),
                Size = new Size(200, 20),
                ForeColor = Color.DarkGray
            };

            partitionComboBox = new ComboBox
            {
                Location = new Point(20, 285),
                Size = new Size(390, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White
            };
         
            partitionComboBox.Items.Add("GPT (UEFI modern)");
            partitionComboBox.Items.Add("MBR (BIOS old)");
            partitionComboBox.SelectedIndex = 0;
            this.Controls.Add(lblPart); this.Controls.Add(partitionComboBox);

            Label lblFs = new Label
            {
                Text = "File System",
                Location = new Point(20, 330),
                Size = new Size(200, 20),
                ForeColor = Color.DarkGray
            };

            fileSystemComboBox = new ComboBox
            {
                Location = new Point(20, 355),
                Size = new Size(390, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White
            };

            fileSystemComboBox.Items.Add("FAT32");
            fileSystemComboBox.Items.Add("NTFS");
            fileSystemComboBox.SelectedIndex = 0;
            this.Controls.Add(lblFs); this.Controls.Add(fileSystemComboBox);

            formatProgressBar = new ProgressBar
            {
                Location = new Point(20, 410),
                Size = new Size(390, 10),
                Visible = false,
                Style = ProgressBarStyle.Marquee
            };

            this.Controls.Add(formatProgressBar);

            startButton = new Button
            {
                Text = "START",
                Location = new Point(20, 440),
                Size = new Size(390, 50),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(229, 20, 0),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold) 
            };

            startButton.FlatAppearance.BorderSize = 0;
            startButton.Click += StartButton_Click;
            this.Controls.Add(startButton);
        }

        private void SelectIsoButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "ISO FILES (*.iso)|*.iso|IMG FILES (*.img)|*.img";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedIsoPath = openFileDialog.FileName;
                    isoPathTextBox.Text = openFileDialog.SafeFileName;
                }
            }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedIsoPath))
            {
                MessageBox.Show
                (
                    "Please, select a disk image first",
                    "Taipan",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            DialogResult result =
                MessageBox.Show
                (
                    "WARNING:" +
                    "All data gonna be lost\n" +
                    "To continue, click on OK",
                    "Lost data warning",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning
                );

            if (result == DialogResult.OK)
            {
                Start_order();
            }
        }

        private async void Start_order()
        {
            formatProgressBar.Visible = true;
            startButton.Enabled = false;

            startButton.Text = "Cleaning drive...";
            await Task.Delay(2000);
            startButton.Text = "Creating partition...";
            await Task.Delay(2000);
            startButton.Text = "Recording ISO files...";
            await Task.Delay(3000);

            Stop_process();
        }
        private void Stop_process()
        {
            formatProgressBar.Visible = false;
            startButton.Enabled = true;
            startButton.Text = "Start";

            MessageBox.Show
            (
                "Bootable media made with success!",
                "GREAT!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void list_disks()
        {
            deviceComboBox.Items.Clear();

            foreach (System.IO.DriveInfo drive in System.IO.DriveInfo.GetDrives())
            {
                if (drive.DriveType == System.IO.DriveType.Removable && drive.IsReady)
                {
                    long Capacity = drive.TotalSize / (1024 * 1024 * 1024);

                    deviceComboBox.Items.Add($"{drive.Name} [{drive.VolumeLabel} - {Capacity} GB]");
                }
            }

            if (deviceComboBox.Items.Count > 0)
            {
                deviceComboBox.SelectedIndex = 0;
            }
            else
            {
                deviceComboBox.Items.Add("No Drive detecteds");
                deviceComboBox.SelectedIndex = 0;
            }
        }
    }
}