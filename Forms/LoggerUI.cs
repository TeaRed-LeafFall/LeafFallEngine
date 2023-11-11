using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LeafFallEngine.Forms
{
    public partial class LoggerUI : Form
    {
        public LoggerUI()
        {
            InitializeComponent();
        }

        private void UpdateLoggerUIList()
        {
            List<LogData> logdata_temp = Logger.LogRegMsg;
            dataGridView1.Rows.Clear();
            int index = 0;
            foreach (LogData logdata in logdata_temp)
            {
                index++;
                dataGridView1.Rows.Add(index, logdata.Header, logdata.Type, logdata.Context);
            }
        }

        private void 刷新列表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateLoggerUIList();
        }

        private void LoggerUI_Load(object sender, EventArgs e)
        {
            UpdateLoggerUIList();
        }
    }
}
