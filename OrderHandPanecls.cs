using System;
using System.Collections;
using System.Windows.Forms;
using Common;
using DAL;
using LSExtensionExplorer;
using LSSERVICEPROVIDERLib;
using OrderV2;

namespace OrderHandPane
{

    public partial class OrderHandPanecls : UserControl, ILSXplVisualControl
    {

        #region members

        private INautilusDBConnection _ntlCon;

        private NautilusServiceProvider _serviceProvider;

        private IDataLayer _dal;

        private Sdg _sdg;

        private Order_cls _order;

        #endregion


        #region Ctor
        public OrderHandPanecls()
        {
            InitializeComponent();
        }
        #endregion


        #region ILSXplVisualControl methods
        public void PreDisplay()
        {
            ExceptionThrown += LSExtensionExpl_ExceptionThrown;


            //Cretae connection first time
            Utils.CreateConstring(_ntlCon);

        }

        public void ChangeDataExplorerView(DataExplorerViewStyles style)
        {
            //throw new NotImplementedException();
        }

        public string GetObjectsStaticItemText()
        {
            //throw new NotImplementedException();
            if (_sdg != null) return _sdg.Name;
            return "";
        }

        public void BeforeFocusedNodeChange(string keyData)
        {
            this.Controls.Clear();
            if (_order != null)
            {
                _order.CloseSamplesColumnChooser();
                _order = null;
            }
            if (_dal == null) return;
            _dal.Close();
            _dal = null;

        }

        void InitData(string id)
        {
            _dal = new DataLayer();
            _dal.Connect();
            _order = new Order_cls();
            _order.RunFromWindow = true;
            _order.SetServiceProvider(_serviceProvider);
            _order.Internationalise();
            _order.PreDisplay();
            _order.VisibleExitButton = false;
            _order.GetButtons();
            _order.Setup();
            _order.Show();
            _sdg = _dal.GetSdgById(long.Parse(id));
            _order.Dock = DockStyle.Fill;
            _order.InputData = _sdg.Name;
            this.Controls.Clear();
            this.Controls.Add(_order);
            _order.GetData(_sdg.Name);
        }
        public void FocusedNodeChanged(string keyData)
        {
            var sdgId = keyData.Split('/')[0];
            InitData(sdgId);


        }

        public void NeedRefresh(string keyData, params string[] parameters)
        {
            if (_dal != null)
            {
                _dal.Close();
                _dal = null;
            }
            var sdgId = keyData.Split('/')[0];
            InitData(sdgId);
        }

        public void ProcessToolbarItemClick(ToolbarItem item)
        {
        }

        public void DataExplorerToolbarButtonClicked(ToolbarItem item)
        {
        }

        public void SetServiceProvider(object sp)
        {

            if (sp != null)
            {
                // Cast the object to the correct type
                _serviceProvider = (NautilusServiceProvider)sp;

                // Using the service provider object get the XML Processor interface
                // We will use this object to get information from the database
                // var nautilusProcessXML = Utils.GetXmlProcessor(serviceProvider);


                _ntlCon = Utils.GetNtlsCon(_serviceProvider);
            }

        }

        public void InitializeToolbarItemsStatus(ref Hashtable toolbarItems)
        {
        }

        public event ExceptionThrownEventHandler ExceptionThrown;
        #endregion

        void LSExtensionExpl_ExceptionThrown(object sender, Exception e)
        {
            MessageBox.Show("Error");
        }
    }
}