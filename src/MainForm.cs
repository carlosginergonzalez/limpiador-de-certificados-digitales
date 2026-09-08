using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace AsesoriaColon.LimpiarCertificados
{
    /// <summary>
    /// Ventana principal al estilo Administrador de certificados de Windows
    /// (menú, barra de herramientas, árbol de almacenes, lista y barra de estado).
    /// </summary>
    internal sealed class MainForm : Form
    {
        readonly TreeView _tree;
        readonly ListView _list;
        readonly PropertyGrid _props;
        readonly StatusStrip _statusBar;
        readonly ToolStripStatusLabel _statusText;
        readonly ToolStripStatusLabel _statusCount;
        readonly ToolStripButton _btnRefresh;
        readonly ToolStripButton _btnMarkExpired;
        readonly ToolStripButton _btnClear;
        readonly Button _btnEliminar;
        readonly ToolStripMenuItem _mnuOnlyExpired;
        readonly ContextMenuStrip _ctx;

        StoreLocation _currentLocation = StoreLocation.CurrentUser;
        string _currentStore = "My";
        bool _suppressTree;

        public MainForm()
        {
            Text = "Limpiador de certificados digitales";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(860, 560);
            Size = new Size(1024, 680);
            Font = SystemFonts.MessageBoxFont;
            try
            {
                var ico = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                if (ico != null) Icon = ico;
            }
            catch { }

            // --- Menú ---
            var menu = new MenuStrip();
            var mArchivo = new ToolStripMenuItem("&Archivo");
            var mActualizar = new ToolStripMenuItem("&Actualizar", null, (s, e) => RefreshList()) { ShortcutKeys = Keys.F5 };
            var mSalir = new ToolStripMenuItem("&Salir", null, (s, e) => Close());
            mArchivo.DropDownItems.Add(mActualizar);
            mArchivo.DropDownItems.Add(new ToolStripSeparator());
            mArchivo.DropDownItems.Add(mSalir);

            var mEdicion = new ToolStripMenuItem("&Edicion");
            var mMarcarCad = new ToolStripMenuItem("Marcar &caducados para eliminar", null, (s, e) => MarkExpired());
            var mQuitar = new ToolStripMenuItem("&Quitar todas las marcas", null, (s, e) => ClearMarks());
            var mEliminar = new ToolStripMenuItem("&Eliminar...", null, (s, e) => DeleteMarked()) { ShortcutKeys = Keys.Control | Keys.D };
            mEdicion.DropDownItems.Add(mMarcarCad);
            mEdicion.DropDownItems.Add(mQuitar);
            mEdicion.DropDownItems.Add(new ToolStripSeparator());
            mEdicion.DropDownItems.Add(mEliminar);

            var mVer = new ToolStripMenuItem("&Ver");
            _mnuOnlyExpired = new ToolStripMenuItem("Mostrar solo &caducados", null, (s, e) =>
            {
                _mnuOnlyExpired.Checked = !_mnuOnlyExpired.Checked;
                RefreshList();
            }) { CheckOnClick = false };
            mVer.DropDownItems.Add(_mnuOnlyExpired);

            var mAyuda = new ToolStripMenuItem("A&yuda");
            mAyuda.DropDownItems.Add(new ToolStripMenuItem("&Acerca de...", null, (s, e) => ShowAbout()));

            menu.Items.Add(mArchivo);
            menu.Items.Add(mEdicion);
            menu.Items.Add(mVer);
            menu.Items.Add(mAyuda);
            MainMenuStrip = menu;

            // --- Barra de herramientas ---
            var tools = new ToolStrip { GripStyle = ToolStripGripStyle.Hidden, RenderMode = ToolStripRenderMode.System };
            _btnRefresh = new ToolStripButton("Actualizar", null, (s, e) => RefreshList()) { DisplayStyle = ToolStripItemDisplayStyle.Text };
            _btnMarkExpired = new ToolStripButton("Marcar caducados", null, (s, e) => MarkExpired()) { DisplayStyle = ToolStripItemDisplayStyle.Text };
            _btnClear = new ToolStripButton("Quitar marcas", null, (s, e) => ClearMarks()) { DisplayStyle = ToolStripItemDisplayStyle.Text };
            tools.Items.Add(_btnRefresh);
            tools.Items.Add(new ToolStripSeparator());
            tools.Items.Add(_btnMarkExpired);
            tools.Items.Add(_btnClear);

            // --- Arbol estrecho a la izquierda (Panel fijo, sin SplitContainer problematico) ---
            _tree = new TreeView
            {
                Dock = DockStyle.Fill,
                HideSelection = false,
                ShowLines = true,
                ShowPlusMinus = true,
                ShowRootLines = true
            };
            BuildStoreTree();
            _tree.AfterSelect += (s, e) => OnStoreSelected(e.Node);

            var treePanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 210,
                Padding = new Padding(0),
                BorderStyle = BorderStyle.FixedSingle
            };
            treePanel.Controls.Add(_tree);

            _list = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                CheckBoxes = true,
                GridLines = false,
                HideSelection = false,
                MultiSelect = true,
                UseCompatibleStateImageBehavior = false
            };
            _list.Columns.Add("Emitido para", 320);
            _list.Columns.Add("Emitido por", 280);
            _list.Columns.Add("Fecha de expiracion", 140);
            _list.SelectedIndexChanged += (s, e) => ShowSelectedProps();

            _ctx = new ContextMenuStrip();
            _ctx.Items.Add("Marcar para eliminar", null, (s, e) => SetCheckedOnSelected(true));
            _ctx.Items.Add("Marcar para conservar", null, (s, e) => SetCheckedOnSelected(false));
            _ctx.Items.Add(new ToolStripSeparator());
            _ctx.Items.Add("Propiedades", null, (s, e) => ShowSelectedProps());
            _list.ContextMenuStrip = _ctx;

            _props = new PropertyGrid
            {
                Dock = DockStyle.Fill,
                ToolbarVisible = false,
                HelpVisible = false,
                PropertySort = PropertySort.NoSort
            };

            var propsPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 160,
                BorderStyle = BorderStyle.FixedSingle
            };
            propsPanel.Controls.Add(_props);

            var rightPanel = new Panel { Dock = DockStyle.Fill };
            rightPanel.Controls.Add(_list);
            rightPanel.Controls.Add(propsPanel);

            var body = new Panel { Dock = DockStyle.Fill };
            body.Controls.Add(rightPanel);
            body.Controls.Add(treePanel);

            // --- Boton grande Eliminar (abajo, bien visible) ---
            var actionBar = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 72,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(16, 10, 16, 10)
            };
            _btnEliminar = new Button
            {
                Text = "Eliminar",
                Size = new Size(280, 48),
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(180, 35, 35),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            _btnEliminar.FlatAppearance.BorderSize = 0;
            _btnEliminar.Click += (s, e) => DeleteMarked();
            actionBar.Controls.Add(_btnEliminar);
            Action centerEliminar = () =>
            {
                _btnEliminar.Left = Math.Max(8, (actionBar.ClientSize.Width - _btnEliminar.Width) / 2);
                _btnEliminar.Top = Math.Max(4, (actionBar.ClientSize.Height - _btnEliminar.Height) / 2);
            };
            actionBar.Resize += (s, e) => centerEliminar();
            actionBar.HandleCreated += (s, e) => centerEliminar();

            // --- Barra de estado ---
            _statusBar = new StatusStrip();
            _statusText = new ToolStripStatusLabel("Seleccione un almacen a la izquierda.") { Spring = true, TextAlign = ContentAlignment.MiddleLeft };
            _statusCount = new ToolStripStatusLabel("0 certificados") { BorderSides = ToolStripStatusLabelBorderSides.Left };
            _statusBar.Items.Add(_statusText);
            _statusBar.Items.Add(_statusCount);

            // Orden Dock: estado (abajo), boton Eliminar, cuerpo, tools, menu
            Controls.Add(body);
            Controls.Add(actionBar);
            Controls.Add(tools);
            Controls.Add(_statusBar);
            Controls.Add(menu);

            Shown += (s, e) =>
            {
                SelectDefaultStore();
                UpdateStatusHint();
            };
        }

        void BuildStoreTree()
        {
            _suppressTree = true;
            _tree.Nodes.Clear();

            var user = _tree.Nodes.Add("user", "Certificados - usuario actual");
            user.Tag = StoreLocation.CurrentUser;
            AddStoreChildren(user, StoreLocation.CurrentUser);

            user.Expand();
            _suppressTree = false;
        }

        void AddStoreChildren(TreeNode parent, StoreLocation location)
        {
            AddStoreNode(parent, location, "My", "Personal");
            AddStoreNode(parent, location, "TrustedPeople", "Personas de confianza");
            AddStoreNode(parent, location, "Root", "Entidades de certificacion raiz de confianza");
            AddStoreNode(parent, location, "CA", "Entidades de certificacion intermedias");
            AddStoreNode(parent, location, "AddressBook", "Otras personas");
        }

        void AddStoreNode(TreeNode parent, StoreLocation location, string storeName, string caption)
        {
            var n = parent.Nodes.Add(location + "\\" + storeName, caption);
            n.Tag = new StoreRef { Location = location, StoreName = storeName };
        }

        void SelectDefaultStore()
        {
            foreach (TreeNode root in _tree.Nodes)
            {
                foreach (TreeNode child in root.Nodes)
                {
                    var r = child.Tag as StoreRef;
                    if (r != null && r.Location == StoreLocation.CurrentUser && r.StoreName == "My")
                    {
                        _tree.SelectedNode = child;
                        return;
                    }
                }
            }
        }

        void OnStoreSelected(TreeNode node)
        {
            if (_suppressTree || node == null) return;
            var r = node.Tag as StoreRef;
            if (r == null) return;
            _currentLocation = r.Location;
            _currentStore = r.StoreName;
            RefreshList();
        }

        void RefreshList()
        {
            _list.BeginUpdate();
            _list.Items.Clear();
            _props.SelectedObject = null;
            try
            {
                var certs = CertificateService.List(_currentLocation, _currentStore);
                int shown = 0;
                foreach (var c in certs)
                {
                    if (_mnuOnlyExpired.Checked && !c.IsExpired) continue;
                    var item = new ListViewItem(DisplayName(c));
                    item.SubItems.Add(c.Issuer);
                    item.SubItems.Add(c.NotAfter.ToString("dd/MM/yyyy"));
                    item.Tag = c;
                    if (c.IsExpired) item.ForeColor = SystemColors.GrayText;
                    _list.Items.Add(item);
                    shown++;
                }
                _statusCount.Text = shown + " certificado(s)";
                _statusText.Text = CaptionForCurrentStore() + " — marque los que desea eliminar; el resto se conserva.";
            }
            catch (Exception ex)
            {
                _statusCount.Text = "Error";
                _statusText.Text = ex.Message;
                MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                _list.EndUpdate();
            }
        }

        static string DisplayName(CertInfo c)
        {
            if (!string.IsNullOrEmpty(c.FriendlyName)) return c.FriendlyName;
            return c.Subject;
        }

        string CaptionForCurrentStore()
        {
            return "Usuario actual > " + _currentStore;
        }

        void ShowSelectedProps()
        {
            if (_list.SelectedItems.Count == 0)
            {
                _props.SelectedObject = null;
                return;
            }
            var c = _list.SelectedItems[0].Tag as CertInfo;
            if (c == null) return;
            _props.SelectedObject = new CertView(c);
        }

        void MarkExpired()
        {
            int n = 0;
            foreach (ListViewItem item in _list.Items)
            {
                var c = item.Tag as CertInfo;
                if (c != null && c.IsExpired)
                {
                    item.Checked = true;
                    n++;
                }
            }
            _statusText.Text = n + " caducado(s) marcados para eliminar. Revise y use Eliminar marcados.";
        }

        void ClearMarks()
        {
            foreach (ListViewItem item in _list.Items)
                item.Checked = false;
            _statusText.Text = "Marcas quitadas. Todos se conservan.";
        }

        void SetCheckedOnSelected(bool check)
        {
            foreach (ListViewItem item in _list.SelectedItems)
                item.Checked = check;
        }

        void UpdateStatusHint()
        {
            _statusText.Text = CaptionForCurrentStore() + " — Marque los certificados a eliminar y pulse Eliminar.";
        }

        void DeleteMarked()
        {
            var targets = new List<CertInfo>();
            foreach (ListViewItem item in _list.Items)
            {
                if (!item.Checked) continue;
                var c = item.Tag as CertInfo;
                if (c != null) targets.Add(c);
            }

            if (targets.Count == 0)
            {
                MessageBox.Show(this,
                    "No hay certificados marcados para eliminar.\n\nMarque la casilla de los que no necesite; los no marcados se conservan.",
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show(this,
                "Se eliminaran de forma permanente " + targets.Count +
                " certificado(s).\n\nLos no marcados se conservan.\n\n¿Eliminar ahora?",
                Text,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes) return;

            try
            {
                int n = CertificateService.Delete(targets);
                MessageBox.Show(this, "Se eliminaron " + n + " certificado(s).", Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "No se pudo completar la operacion:\n\n" + ex.Message, Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void ShowAbout()
        {
            MessageBox.Show(this,
                "Limpiador de certificados digitales\n\n" +
                AppConfig.Description + "\n\n" +
                "Carlos Giner Gonzalez\nVersion 1.0",
                "Acerca de",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        sealed class StoreRef
        {
            public StoreLocation Location;
            public string StoreName;
        }

        sealed class CertView
        {
            readonly CertInfo _c;
            public CertView(CertInfo c) { _c = c; }

            public string Emitido_para { get { return _c.Subject; } }
            public string Emitido_por { get { return _c.Issuer; } }
            public string Nombre_descriptivo { get { return string.IsNullOrEmpty(_c.FriendlyName) ? "(ninguno)" : _c.FriendlyName; } }
            public string Valido_desde { get { return _c.NotBefore.ToString("dd/MM/yyyy HH:mm"); } }
            public string Valido_hasta { get { return _c.NotAfter.ToString("dd/MM/yyyy HH:mm"); } }
            public string Estado { get { return _c.IsExpired ? "Caducado" : "Correcto"; } }
            public string Tiene_clave_privada { get { return _c.HasPrivateKey ? "Si" : "No"; } }
            public string Almacen { get { return _c.StoreLocation + " / " + _c.StoreName; } }
            public string Huella_digital { get { return _c.Thumbprint; } }
        }
    }
}
