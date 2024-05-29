using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClassLibrary
{
    public class Dialogservice
    {
        public void ShowMessage (string message, string title)
        {
            _ = MessageBox.Show(message, title);

        }
    }
}
