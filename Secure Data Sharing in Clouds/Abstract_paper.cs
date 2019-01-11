using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Secure_Data_Sharing_in_Clouds
{
    public partial class Abstract_paper : Form
    {
        public Abstract_paper()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            axAcroPDF1.src = "C:\\Users\\akash\\source\\repos\\Secure Data Sharing in Clouds\\119015038insurance abstract 1.pdf";

        }
    }
}
