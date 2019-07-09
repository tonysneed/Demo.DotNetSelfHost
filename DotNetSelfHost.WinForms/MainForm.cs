using System.Threading;
using System.Windows.Forms;

namespace DotNetSelfHost.WinForms
{
    public partial class MainForm : Form
    {
        private SynchronizationContext _syncRoot;

        public MainForm()
        {
            InitializeComponent();
            _syncRoot = SynchronizationContext.Current;
        }

        public string NameText
        {
            get { return nameTextBox.Text; }
            set { _syncRoot.Post(SetName, value); }
        }

        private void SetName(object arg)
        {
            string name = arg as string;
            if (name != null)
                nameTextBox.Text = name;
        }
    }
}
