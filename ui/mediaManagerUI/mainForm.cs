
using System.Threading;

namespace tomtiv.myMediaManager.ui.mediaManagerUI
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using tomtiv.myMediaManager.core.mediaManagerLib;

    public partial class mainForm : Form
    {
        private MediaItems mediaItems;

        public mainForm()
        {
            InitializeComponent();
        }

        private void startButton_Click(object sender, EventArgs e)
        {       
            Thread th = new Thread(ProcessItems);
            th.Start();
            //dataGridView.DataSource = mediaItems;
            //dataGridView.Visible = true;
        }

        private void folderButton_Click(object sender, EventArgs e)
        {
            folderBrowser.ShowDialog();
            //folderText.Text = folderBrowser.SelectedPath;
            startButton.Enabled = true;
        }

        private void ProcessItems()
        {
            try
            {
                /*
                mediaItems = new MediaItems { PathToProcess = folderText.Text };
                mediaItems.Load();
                mediaItems.ProcessItems();
                */
            }
            catch (MediaItemException mediaItemException)
            {
            }
            catch (Exception ex)
            {
            }
        }
    }
}
