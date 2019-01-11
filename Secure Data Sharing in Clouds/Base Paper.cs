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
    public partial class Base_Paper : Form
    {
        public Base_Paper()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            axAcroPDF1.src = "C:\\Users\\akash\\source\\repos\\Secure Data Sharing in Clouds\\Base paper SEDASC.pdf";
        }
    }
}
