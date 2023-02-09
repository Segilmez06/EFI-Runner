using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EFI_Runner
{
    public partial class VNCViewer : Form
    {
        public VNCViewer()
        {
            InitializeComponent();
            Viewer.Size = this.Size;
        }

        private void VNCViewer_Load(object sender, EventArgs e)
        {
            Viewer.Connect("localhost");
        }
    }
}
