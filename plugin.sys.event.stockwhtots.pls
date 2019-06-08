#line 2

   #region BODY
        //BEGIN

        const int VERSION = 9;
        const string FILE = "plugin.sys.event.stockwhtots.pls";


        #region SETTINGS

        class _SETTINGS : TOOL_SETTINGS
        {

            public static _BUF BUF = null;
            public class _BUF
            {
                public static void LOAD_SETTINGS(_PLUGIN PLUGIN)
                {
                    if (_SETTINGS.BUF != null)
                        return;

                    var x = new _SETTINGS._BUF();

                    var s = new _SETTINGS(PLUGIN);

                    x.MY_STOCKWHTOTS_USER = s.MY_STOCKWHTOTS_USER;

                    //

                    _SETTINGS.BUF = x;

                }

                public string MY_STOCKWHTOTS_USER;



            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC) //, "ava.production.config")
            {

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active On User")]
            public string MY_STOCKWHTOTS_USER
            {
                get
                {
                    return (_GET("MY_STOCKWHTOTS_USER", "1,2"));
                }
                set
                {
                    _SET("MY_STOCKWHTOTS_USER", value);
                }

            }



            //

            public static bool ISUSEROK(_PLUGIN pPLUGIN)
            {
                return Array.IndexOf<string>(
                     EXPLODELIST(BUF.MY_STOCKWHTOTS_USER),
                     FORMAT(pPLUGIN.GETSYSPRM_USER())
                     ) >= 0;
            }

        }

        #endregion

        #region TEXT

        public class TEXT
        {
            public const string text_DESC = "Stock Warehouse Tots";


            public class L
            {

            }
        }

        const string event_STOCKWHTOTS_ = "hadlericom_stockwhtots_";
        const string event_STOCKWHTOTS_MONTH = "hadlericom_stockwhtots_month";
        const string event_STOCKWHTOTS_RANGE = "hadlericom_stockwhtots_range";

        public class _LANG
        {


            static _LANG _L = null;

            public static _LANG L
            {
                get
                {
                    if (_L == null)
                        _L = new _LANG();
                    return _L;
                }
            }


            public _LANG()
            {

                var m = this.GetType().GetMethod("lang_" + LANG_ACTIVE());
                if (m != null)
                    m.Invoke(this, null);

            }



            public string TOTS_BY_MONTH = "By Month Material Warehouse Tots";
            public string TOTS_BY_RANGE = "By Date Range Material Warehouse Tots";

            public void lang_az()
            {

                TOTS_BY_MONTH = "Ayliq Material Ambar Toplamları";
                TOTS_BY_RANGE = "Tarih Aralığı Material Ambar Toplamları";
            }

            public void lang_ru()
            {

                TOTS_BY_MONTH = "Складские Итоги По Месяцам";
                TOTS_BY_RANGE = "Складские Итоги За Интервал";


            }

            public void lang_tr()
            {


                TOTS_BY_MONTH = "Aylik Malzeme Ambar Toplamlari";
                TOTS_BY_RANGE = "Tarih Aralığı Malzeme Ambar Toplamlari";


            }
        }



        #endregion



        public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
        {


            _SETTINGS._BUF.LOAD_SETTINGS(this);

            if (!_SETTINGS.ISUSEROK(this))
                return;


            object arg1 = ARGS.Length > 0 ? ARGS[0] : null;
            object arg2 = ARGS.Length > 1 ? ARGS[1] : null;
            object arg3 = ARGS.Length > 2 ? ARGS[2] : null;

            string[] list_ = EXPLODELISTPATH(EVENTCODE);

            switch (list_.Length > 0 ? list_[0] : "")
            {
                case SysEvent.SYS_PLUGINSETTINGS:
                    (arg1 as List<object>).Add(new _SETTINGS(this));
                    break;
                case SysEvent.SYS_NEWFORM:
                    MY_SYS_NEWFORM_INTEGRATE_REF(arg1 as Form);

                    break;
                case SysEvent.SYS_USEREVENT:
                    MY_SYS_USEREVENT_HANDLER(EVENTCODE, ARGS);
                    break;

            }



        }



        void MY_SYS_NEWFORM_INTEGRATE_REF(Form FORM)
        {
            if (FORM == null)
                return;
            try
            {

                var fn = GETFORMNAME(FORM);


                var isPrch = fn.StartsWith("adp.prch.doc.inv");
                var isSls = fn.StartsWith("adp.sls.doc.inv");
                var isListMat = fn.StartsWith("ref.mm.rec.mat");
                var isListClient = fn.StartsWith("ref.fin.rec.client");
                var isAdpOrder = fn.StartsWith("adp.sls.doc.order") || fn.StartsWith("adp.prch.doc.order");

                if (isListMat || isListClient || isPrch || isSls || isAdpOrder)
                {
                    foreach (var ctrl in CONTROL_DESTRUCT(FORM))
                    {
                        var menuItem = ctrl as ToolStripItem;
                        if (menuItem != null && menuItem.Name == "cMenuGridInfoPlugin")
                        {
                            {
                                var args = new Dictionary<string, object>() { 
 
            { "_cmd" ,"add"},
            { "_type" ,"item"},
            { "_name" , event_STOCKWHTOTS_MONTH},

            { "_infoLocation" ,"event"},
            { "_infoArg" ,event_STOCKWHTOTS_MONTH},

            { "Text" ,_LANG.L.TOTS_BY_MONTH},
            { "ImageName" ,"sum_16x16"},
             { "Name" ,event_STOCKWHTOTS_MONTH},
            };

                                RUNUIINTEGRATION(menuItem, args);

                            }


                            {
                                var args = new Dictionary<string, object>() { 
 
            { "_cmd" ,"add"},
            { "_type" ,"item"},
            { "_name" , event_STOCKWHTOTS_RANGE},

            { "_infoLocation" ,"event"},
            { "_infoArg" ,event_STOCKWHTOTS_RANGE},

            { "Text" ,_LANG.L.TOTS_BY_RANGE},
            { "ImageName" ,"calendar_16x16"},
             { "Name" ,event_STOCKWHTOTS_RANGE},
            };

                                RUNUIINTEGRATION(menuItem, args);

                            }

                        }

                    }



                }

            }
            catch (Exception exc)
            {
                MSGUSERERROR("Cant add button: " + exc.Message);
            }

        }





        void _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(Control pCtrl, string pEvent, string pText, string pImg = null)
        {
            if (pCtrl == null)
                return;
            pImg = pImg ?? "info_16x16";
            try
            {


                var args = new Dictionary<string, object>() { 
            
			{ "_cmd" ,""},
            { "_type" ,""},
			{ "CmdText" ,"event name::"+pEvent},
			{ "Text" ,pText},
			{ "ImageName" ,pImg},
			{"AutoSize", true},
			//{ "Width" ,100},
            };

                var b = RUNUIINTEGRATION(pCtrl, args) as Button;
                if (b != null)
                {

                    var w = (Math.Max(100, b.Width + 32) / 20) * 20;
                    b.AutoSize = false;
                    b.Width = w;

                }

            }
            catch (Exception exc)
            {
                MSGUSERERROR("Cant add button: " + exc.Message);
            }

        }

        public void MY_SYS_USEREVENT_HANDLER(string EVENTCODE, object[] ARGS) //adapter start
        {


            //
            try
            {


                object arg1 = ARGS.Length > 0 ? ARGS[0] : null;
                object arg2 = ARGS.Length > 1 ? ARGS[1] : null;

                string[] list_ = EXPLODELISTPATH(EVENTCODE);
                var cmd = list_.Length > 1 ? list_[1].ToLowerInvariant() : "";

                switch (cmd)
                {


                    case event_STOCKWHTOTS_MONTH:
                        {
                            STOCKWHTOTS_MONTH(arg1 as DataRow);
                        }
                        break;

                    case event_STOCKWHTOTS_RANGE:
                        {
                            STOCKWHTOTS_RANGE(arg1 as DataRow);
                        }
                        break;




                }


            }
            catch (Exception exc)
            {
                LOG(exc);
                MSGUSERERROR(exc.Message);
            }
        }



        object MY_GET_STOCK_LREF(DataRow pMatRec)
        {
            if (pMatRec == null)
                return null;

            object lref = null;
            switch (pMatRec.Table.TableName)
            {

                case "STLINE":
                    var ltype = CASTASSHORT(TAB_GETROW(pMatRec, "LINETYPE"));
                    if (ltype == 0 || ltype == 1)
                        lref = TAB_GETROW(pMatRec, "STOCKREF");
                    break;
                case "ITEMS":
                    lref = TAB_GETROW(pMatRec, "LOGICALREF");
                    break;

            }

            return lref;
        }

        object MY_GET_CLIENT_LREF(DataRow pMatRec)
        {
            if (pMatRec == null)
                return null;

            object lref = null;
            switch (pMatRec.Table.TableName)
            {
                case "INVOICE":
                case "STLINE":
                    lref = TAB_GETROW(pMatRec, "CLIENTREF");
                    break;
                case "CLCARD":
                    lref = TAB_GETROW(pMatRec, "LOGICALREF");
                    break;

            }

            return lref;
        }

        private void STOCKWHTOTS_MONTH(DataRow pRecord)
        {

            try
            {


                var args = new FormMonthTots.Filter()
                {
                    material = CASTASINT(MY_GET_STOCK_LREF(pRecord)),
                    client = CASTASINT(MY_GET_CLIENT_LREF(pRecord)),
                };


                Form form = new FormMonthTots(this, args);

                form.Show();

            }
            catch (Exception exc)
            {
                LOG(exc);
                MSGUSERERROR(exc.Message);
            }

        }




        private void STOCKWHTOTS_RANGE(DataRow pRecord)
        {

            try
            {


                var args = new FormDateRangeTots.Filter()
                {
                    material = CASTASINT(MY_GET_STOCK_LREF(pRecord)),
                    client = CASTASINT(MY_GET_CLIENT_LREF(pRecord)),
                };


                Form form = new FormDateRangeTots(this, args);

                form.Show();

            }
            catch (Exception exc)
            {
                LOG(exc);
                MSGUSERERROR(exc.Message);
            }

        }


        class FormMonthTots : Form
        {
            _PLUGIN PLUGIN;
            Args initInfo;
            Filter filter = new Filter();

            public FormMonthTots(_PLUGIN pPLUGIN, Filter pFilter)
            {
                this.Text = string.Format("{0}", _LANG.L.TOTS_BY_MONTH);

                PLUGIN = pPLUGIN;
                filter = pFilter;


                init();

                bindFilterDo();

                refreshData();
            }


            public void init()
            {

                filter.year = DateTime.Now.Year;

                this.Text = "";

                this.Icon = CTRL_FORM_ICON();
                this.Size = new Size(760, 600);
                this.AutoSize = true;
                //form.BackColor = Color.White;
                this.StartPosition = FormStartPosition.CenterScreen;
                var mainPanel = new Panel();
                this.Controls.Add(mainPanel);
                mainPanel.Dock = DockStyle.Fill;
                mainPanel.AutoSize = true;


                var btnClose = new Button() { Text = PLUGIN.LANG("T_CLOSE"), Image = RES_IMAGE("close_16x16"), ImageAlign = ContentAlignment.MiddleLeft, Width = 160, Dock = DockStyle.Right };
                btnClose.Click += (s, arg) => { this.Close(); };

                var btnRefresh = new Button() { Text = PLUGIN.LANG("T_REFRESH"), Image = RES_IMAGE("refresh_16x16"), ImageAlign = ContentAlignment.MiddleLeft, Width = 160, Dock = DockStyle.Left };
                btnRefresh.Click += (s, arg) => { this.refreshData(); };




                //var btnSetPrice = new Button() { Width = 80, Text = PLUGIN.LANG("T_SAVE"), Image = RES_IMAGE("checked_16x16"), ImageAlign = ContentAlignment.MiddleLeft };
                //  btnSetPrice.Click += (s, arg) => { TAB_SETROW(pRow, "PRICE", (double)priceBox.Value); this.Close(); };


                var panelData = new Panel() { Dock = DockStyle.Fill };
                var panelFilterYear = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Bottom, AutoSize = true };
                var panelFilterWh = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Bottom, AutoSize = true };

                var panelFilterCl = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Bottom, AutoSize = true };
                var panelFilterMat = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Bottom, AutoSize = true };
                var panelBtn = new Panel() { Dock = DockStyle.Bottom, Height = 25 };


                //
                mainPanel.Controls.Add(panelData);

                mainPanel.Controls.Add(panelFilterYear);
                mainPanel.Controls.Add(panelFilterMat);
                mainPanel.Controls.Add(panelFilterCl);
                mainPanel.Controls.Add(panelFilterWh);





                mainPanel.Controls.Add(new Panel() { Tag = "Dummmy Padding", Height = 10, Dock = DockStyle.Bottom });
                //

                mainPanel.Controls.Add(panelBtn);


                panelBtn.Controls.Add(btnRefresh);
                panelBtn.Controls.Add(btnClose);
                //
                panelFilterYear.Controls.AddRange(
                    new Control[]{
                    new Label(){Text = PLUGIN.LANG("T_YEAR"), Width = 80, TextAlign = ContentAlignment.MiddleLeft,Dock = DockStyle.Left},
                    new NumericUpDown() { Minimum = 1970, Maximum = 9999, DecimalPlaces = 0, Value = filter.year, Name = "filter_year",Width=80 },
             
                    }
                    );

                panelFilterMat.Controls.AddRange(
             new Control[]{
                    new Label(){Text = PLUGIN.LANG("T_MATERIAL"), Width = 80, TextAlign = ContentAlignment.MiddleLeft,Dock = DockStyle.Left},
                    new NumericUpDown() { Minimum = 0, Maximum = int.MaxValue, DecimalPlaces = 0, Value = filter.material, Name = "filter_material",Width=80 },
                   new Button(){Text="",Image = RES_IMAGE("find_16x16"),ImageAlign = ContentAlignment.MiddleCenter,Name = "do_filter_material",Width=30 },
                   new TextBox(){Width=500,ReadOnly=true,Name="desc_filter_material"}
                    }
             );

                panelFilterCl.Controls.AddRange(
                 new Control[]{
                    new Label(){Text = PLUGIN.LANG("T_PERSONAL"), Width = 80, TextAlign = ContentAlignment.MiddleLeft,Dock = DockStyle.Left},
                    new NumericUpDown() { Minimum = 0, Maximum = int.MaxValue, DecimalPlaces = 0, Value = filter.client, Name = "filter_client",Width=80},
                   new Button(){Text="",Image = RES_IMAGE("find_16x16"),ImageAlign = ContentAlignment.MiddleCenter,Name = "do_filter_client",Width=30 },
                   new TextBox(){Width=500,ReadOnly=true,Name="desc_filter_client"}
                    }
                 );

                panelFilterWh.Controls.AddRange(
             new Control[]{
                    new Label(){Text = PLUGIN.LANG("T_WH"), Width = 80, TextAlign = ContentAlignment.MiddleLeft,Dock = DockStyle.Left},
                    new NumericUpDown() { Minimum = -1, Maximum = short.MaxValue, DecimalPlaces = 0, Value = filter.wh, Name = "filter_wh",Width=80 },
                    new Button(){Text="",Image = RES_IMAGE("find_16x16"),ImageAlign = ContentAlignment.MiddleCenter,Name = "do_filter_wh",Width=30 }
                    }
             );


                //        var grid = new DataGridView();


                var tabs = new TabControl() { Dock = DockStyle.Fill };

                TabPage pageSale = null;
                TabPage pagePurch = null;

                tabs.TabPages.Add(pageSale = new TabPage() { Text = PLUGIN.LANG("T_SALE - T_NET") });
                tabs.TabPages.Add(pagePurch = new TabPage() { Text = PLUGIN.LANG("T_PURCHASE - T_NET") });

                DataGridView gridSale = null;
                DataGridView gridPurch = null;

                pageSale.Controls.Add(gridSale = createCreate("grid_sale"));
                pagePurch.Controls.Add(gridPurch = createCreate("grid_purchase"));


                foreach (var grid in new DataGridView[] { gridSale, gridPurch })
                    grid.Columns.AddRange(
                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_MONTH"), DataPropertyName = "TITLE", Width = 120 },
                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_QUANTITY"), DataPropertyName = "QTY", Width = 100 },

                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_VATEXC (-)\n(T_SYS_CURR1)"), DataPropertyName = "TOTNET", Width = 120 },
                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_VATINC (+)\n(T_SYS_CURR1)"), DataPropertyName = "TOTNETVAT", Width = 120 },

                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_VATEXC (-)\n(T_SYS_CURR2)"), DataPropertyName = "TOTNETREP", Width = 120 },
                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_VATINC (+)\n(T_SYS_CURR2)"), DataPropertyName = "TOTNETVATREP", Width = 120 }
                                  );


                foreach (var grid in new DataGridView[] { gridSale, gridPurch })
                    foreach (DataGridViewColumn colObj in grid.Columns)
                    {
                        colObj.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        colObj.SortMode = DataGridViewColumnSortMode.Programmatic;

                        switch (colObj.DataPropertyName)
                        {
                            case "TOTNET":
                            case "TOTNETVAT":
                            case "TOTNETREP":
                            case "TOTNETVATREP":
                                colObj.DefaultCellStyle.Format = "#,0.00;-#,0.00;''";
                                colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


                                if (colObj.DataPropertyName.EndsWith("REP"))
                                    colObj.DefaultCellStyle.ForeColor = Color.SkyBlue;
                                else
                                    colObj.DefaultCellStyle.ForeColor = Color.Blue;

                                break;
                            case "QTY":
                                colObj.DefaultCellStyle.Format = "#,0.##;-#,0.##;''";
                                colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                                colObj.DefaultCellStyle.ForeColor = Color.Green;
                                break;
                            case "TITLE":
                                //  colObj.DefaultCellStyle.Format = "d";
                                colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                                break;
                        }
                    }

                foreach (var grid in new DataGridView[] { gridSale, gridPurch })
                {

                    ///////////
                    grid.CellPainting += (sender, e) =>
                {
                    // if (stripeRowBackColor)
                    {
                        DataGridView grid_ = sender as DataGridView;
                        if (grid_ == null)
                            return;

                        var bgColor = grid_.DefaultCellStyle.BackColor;

                        var rowObj_ = TOOL_GRID.GET_GRID_ROW(grid_, e.RowIndex);
                        if (rowObj_ == null)
                            return;

                        var colObj_ = e.ColumnIndex >= 0 ? grid_.Columns[e.ColumnIndex] : null;
                        if (colObj_ == null)
                            return;



                        var rowData_ = TOOL_GRID.GET_GRID_ROW_DATA(rowObj_);
                        if (rowData_ == null)
                            return;

                        var monthIndx = CASTASSHORT(TAB_GETROW(rowData_, "INDX"));

                        var isSum = (monthIndx >= 13 || monthIndx == DateTime.Now.Month);

                        var isBold = (isSum || colObj_.DataPropertyName == "TITLE");

                        var font = e.CellStyle.Font;
                        if (font == null)
                            font = colObj_.InheritedStyle.Font;

                        if (isBold != font.Bold)
                        {

                            e.CellStyle.Font = new Font(font, FontStyle.Bold | font.Style);

                        }

                        if (isSum)
                            bgColor = Color.WhiteSmoke;

                        if (bgColor != e.CellStyle.BackColor)
                            e.CellStyle.BackColor = bgColor;

                    }
                };

                    /////////
                }

                panelData.Controls.Add(tabs);



            }



            DataGridView createCreate(string pName)
            {

                var grid = new DataGridView();
                grid.Name = pName;
                grid.Dock = DockStyle.Fill;
                grid.MultiSelect = false;
                grid.ReadOnly = true;
                grid.AllowUserToAddRows = false;
                grid.AllowUserToDeleteRows = false;
                grid.AllowUserToResizeColumns = false;
                grid.AllowUserToResizeRows = false;

                grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                grid.AutoGenerateColumns = false;
                grid.BackgroundColor = Color.White;
                grid.RowHeadersVisible = false;
                grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                //grid.Height = 110;
                // grid.AutoSize = true;
                grid.BorderStyle = BorderStyle.None;



                grid.ColumnHeadersHeight *= 2;
                grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                return grid;
            }

            void bindFilterDo()
            {
                foreach (var o in CONTROL_DESTRUCT(this))
                {

                    var ctrl = o as Control;

                    if (ctrl == null)
                        continue;

                    switch (ctrl.Name)
                    {

                        case "do_filter_wh":
                        case "do_filter_client":
                        case "do_filter_material":
                            {
                                var btn = ctrl as Button;
                                btn.Click += filterDoBtnClick;

                            }
                            break;
                    }


                }
            }

            void filterDoBtnClick(object sender, EventArgs e)
            {
                try
                {

                    var btn = sender as Button;

                    if (btn == null || btn.Name == null)
                        return;

                    readFilter();


                    if (btn.Name == "do_filter_wh")
                    {

                        var valCtrl = CONTROL_SEARCH(this, "filter_wh") as NumericUpDown;
                        if (valCtrl != null)
                        {
                            DataRow[] res_ = PLUGIN.REF("ref.gen.rec.wh", "NR", CASTASSHORT(valCtrl.Value));
                            if (res_ != null && res_.Length > 0)
                            {
                                valCtrl.Value = CASTASSHORT(TAB_GETROW(res_[0], "NR"));
                            }
                        }

                        return;
                    }

                    if (btn.Name == "do_filter_client")
                    {

                        var valCtrl = CONTROL_SEARCH(this, "filter_client") as NumericUpDown;
                        if (valCtrl != null)
                        {
                            DataRow[] res_ = PLUGIN.REF("ref.fin.rec.client", "LOGICALREF", CASTASINT(valCtrl.Value));
                            if (res_ != null && res_.Length > 0)
                            {
                                valCtrl.Value = CASTASSHORT(TAB_GETROW(res_[0], "LOGICALREF"));
                            }
                        }

                        return;
                    }

                    if (btn.Name == "do_filter_material")
                    {

                        var valCtrl = CONTROL_SEARCH(this, "filter_material") as NumericUpDown;
                        if (valCtrl != null)
                        {
                            DataRow[] res_ = PLUGIN.REF("ref.mm.rec.mat", "LOGICALREF", CASTASINT(valCtrl.Value));
                            if (res_ != null && res_.Length > 0)
                            {
                                valCtrl.Value = CASTASSHORT(TAB_GETROW(res_[0], "LOGICALREF"));
                            }
                        }

                        return;
                    }
                }
                catch (Exception exc)
                {
                    PLUGIN.LOG(exc);
                    PLUGIN.MSGUSERERROR(exc.Message);
                }
            }

            void readFilter()
            {
                foreach (var o in CONTROL_DESTRUCT(this))
                {

                    var ctrl = o as Control;

                    if (ctrl == null)
                        continue;


                    var asNum = ctrl as NumericUpDown;

                    if (asNum != null && asNum.Text == "")
                    {
                        //!!! num has problem on delete text, Text empty but Value hidden
                        asNum.Value = 0;
                    }



                    switch (ctrl.Name)
                    {
                        case "filter_year":
                            filter.year = (int)asNum.Value;
                            break;
                        case "filter_wh":
                            filter.wh = (short)asNum.Value;
                            break;
                        case "filter_client":
                            filter.client = (int)asNum.Value;
                            break;
                        case "filter_material":
                            filter.material = (int)asNum.Value;
                            break;
                    }



                }
            }

            void refreshData()
            {
                try
                {

                    readFilter();


                    var matDesc = GET_TITLE_MAT(filter.material);
                    var clDesc = GET_TITLE_CLIENT(filter.client);

                    var ctrlDescMat = CONTROL_SEARCH(this, "desc_filter_material") as TextBox;
                    var ctrlDescCl = CONTROL_SEARCH(this, "desc_filter_client") as TextBox;
                    if (ctrlDescMat != null)
                        ctrlDescMat.Text = matDesc;
                    if (ctrlDescCl != null)
                        ctrlDescCl.Text = clDesc;

                    this.Text = string.Format("{0}", _LANG.L.TOTS_BY_MONTH);


                    var codes = new string[] { "grid_sale", "grid_purchase" };
                    var trcodes = new short[][] { new short[] { 8, 7, 3, 2 }, new short[] { 1, 6 } };
                    var trcodesSign = new short[][] { new short[] { 1, 1, -1, -1 }, new short[] { 1, -1 } };

                    for (int c = 0; c < codes.Length; ++c)
                    {
                        var code = codes[c];
                        switch (code)
                        {
                            case "grid_sale":
                            case "grid_purchase":
                                {


                                    var table = new DataTable(code);

                                    TAB_ADDCOL(table, "INDX", typeof(int));
                                    TAB_ADDCOL(table, "TITLE", typeof(string));
                                    //
                                    TAB_ADDCOL(table, "QTY", typeof(double));
                                    //
                                    TAB_ADDCOL(table, "TOTNET", typeof(double));
                                    TAB_ADDCOL(table, "TOTNETVAT", typeof(double));
                                    TAB_ADDCOL(table, "TOTNETREP", typeof(double));
                                    TAB_ADDCOL(table, "TOTNETVATREP", typeof(double));

                                    double sum_qty = 0.0;
                                    double sum_totNet = 0.0;
                                    double sum_totNetVat = 0.0;
                                    double sum_totNetRep = 0.0;
                                    double sum_totNetVatRep = 0.0;

                                    for (int m = 1; m <= 13; ++m)
                                    {
                                        int indx = m;
                                        string title = PLUGIN.LANG(m == 13 ? "T_TOTAL" : "" + filter.year + " - " + "T_MONTH_" + m);

                                        double qty = 0.0;
                                        double totNet = 0.0;
                                        double totNetVat = 0.0;
                                        double totNetRep = 0.0;
                                        double totNetVatRep = 0.0;

                                        if (m < 13)
                                        {
                                            var df = new DateTime(filter.year, m, 1);
                                            var dt = df.AddMonths(+1).AddDays(-1);

                                            var arrTrcode = trcodes[c];
                                            var arrTrcodeSign = trcodesSign[c];

                                            qty = 0;
                                            totNet = 0;
                                            totNetVat = 0;
                                            totNetRep = 0;
                                            totNetVatRep = 0;


                                            for (int t = 0; t < arrTrcode.Length; ++t)
                                            {
                                                var trcode = arrTrcode[t];
                                                var sign = arrTrcodeSign[t];

                                                var rec = MY_TOTS_SALES(df, dt, trcode, filter.material, filter.client, filter.wh);

                                                qty += (sign * CASTASDOUBLE(TAB_GETROW(rec, "QTY")));
                                                totNet += (sign * CASTASDOUBLE(TAB_GETROW(rec, "TOTNET")));
                                                totNetVat += (sign * CASTASDOUBLE(TAB_GETROW(rec, "TOTNETVAT")));
                                                totNetRep += (sign * CASTASDOUBLE(TAB_GETROW(rec, "TOTNETREP")));
                                                totNetVatRep += (sign * CASTASDOUBLE(TAB_GETROW(rec, "TOTNETVATREP")));

                                            }



                                        }


                                        if (m < 13)
                                        {

                                            sum_qty += qty;
                                            sum_totNet += totNet;
                                            sum_totNetVat += totNetVat;
                                            sum_totNetRep += totNetRep;
                                            sum_totNetVatRep += totNetVatRep;

                                        }
                                        else
                                        {

                                            qty = sum_qty;
                                            totNet = sum_totNet;
                                            totNetVat = sum_totNetVat;
                                            totNetRep = sum_totNetRep;
                                            totNetVatRep = sum_totNetVatRep;
                                        }

                                        table.Rows.Add(

                                            indx,
                                            title,
                                            qty,
                                            totNet,
                                            totNetVat,
                                            totNetRep,
                                            totNetVatRep

                                            );





                                    }


                                    var grid = CONTROL_SEARCH(this, code) as DataGridView;
                                    if (grid != null)
                                        grid.DataSource = table;

                                }

                                break;




                        }



                    }


                }
                catch (Exception exc)
                {
                    PLUGIN.LOG(exc);
                    PLUGIN.MSGUSERERROR(exc.Message);
                }
            }

            private DataRow MY_TOTS_SALES(DateTime df, DateTime dt, short trcode, int material, int client, short wh)
            {


                //   TAB_GETLASTROW(PLUGIN.SQL("SELECT 1.0 QTY,2.0 TOTNET,3.0 TOTNETVAT,4.0 TOTNETREP,5.0 TOTNETVATREP", new object[]{}));


                var listWhere = new List<string>();
                var listArgs = new List<object>();






                var sqlText =

    @"

SELECT  

		sum(AMOUNT) QTY, 
		sum(VATMATRAH) TOTNET,
		sum(VATMATRAH+VATAMNT ) TOTNETVAT,
		sum(case when REPORTRATE > 0 then (VATMATRAH )/REPORTRATE else 0 end) TOTNETREP,
		sum(case when REPORTRATE > 0 then (VATMATRAH+VATAMNT )/REPORTRATE else 0 end) TOTNETVATREP
 
		FROM LG_$FIRM$_$PERIOD$_STLINE WITH(NOLOCK)
		WHERE 
        ( DATE_ BETWEEN @P1 AND @P2 AND TRCODE = @P3 AND LINETYPE = 0 AND CANCELLED = 0 ) 
   
			";

                listArgs.Add(df);
                listArgs.Add(dt);
                listArgs.Add(trcode);

                if (material > 0)//!
                { listArgs.Add(material); listWhere.Add("STOCKREF = @P" + listArgs.Count); }
                if (client > 0)//!
                { listArgs.Add(client); listWhere.Add("CLIENTREF = @P" + listArgs.Count); }

                if (wh >= 0)
                { listArgs.Add(wh); listWhere.Add("SOURCEINDEX = @P" + listArgs.Count); }


                if (listWhere.Count > 0)
                {

                    sqlText += (" AND " + string.Join(" AND ", listWhere.ToArray()));

                }


                return TAB_GETLASTROW(PLUGIN.SQL(sqlText, listArgs.ToArray()));




            }


            string GET_TITLE_CLIENT(object pLRef)
            {


                var tab = PLUGIN.SQL(" SELECT  C.CODE AS CODE,C.DEFINITION_ AS NAME FROM LG_$FIRM$_CLCARD C WHERE LOGICALREF = @P1",
new object[] { pLRef });

                var rec = TAB_GETLASTROW(tab);

                if (rec != null)
                {
                    var code = CASTASSTRING(TAB_GETROW(rec, "CODE"));
                    var name = CASTASSTRING(TAB_GETROW(rec, "NAME"));
                    return code + "/" + name;
                }

                return "";

            }

            string GET_TITLE_MAT(object pLRef)
            {

                var tab = PLUGIN.SQL(" SELECT  C.CODE AS CODE,C.NAME AS NAME FROM LG_$FIRM$_ITEMS C WHERE LOGICALREF = @P1",
new object[] { pLRef });

                var rec = TAB_GETLASTROW(tab);

                if (rec != null)
                {
                    var code = CASTASSTRING(TAB_GETROW(rec, "CODE"));
                    var name = CASTASSTRING(TAB_GETROW(rec, "NAME"));
                    return code + "/" + name;
                }

                return "";

            }
            public class Filter
            {
                public int year = DateTime.Now.Year;
                public short wh = -1;

                public int client = 0;
                public int material = 0;
            }


        }





        class FormDateRangeTots : Form
        {
            _PLUGIN PLUGIN;
            Args initInfo;
            Filter filter = new Filter();

            public FormDateRangeTots(_PLUGIN pPLUGIN, Filter pFilter)
            {
                PLUGIN = pPLUGIN;
                filter = pFilter;

                this.Text = string.Format("{0}", _LANG.L.TOTS_BY_RANGE);

                init();

                bindFilterDo();

                refreshData();
            }


            public void init()
            {



                this.Icon = CTRL_FORM_ICON();
                this.Size = new Size(1000, 700);
                this.AutoSize = true;
                //form.BackColor = Color.White;
                this.StartPosition = FormStartPosition.CenterScreen;
                var mainPanel = new Panel();
                this.Controls.Add(mainPanel);
                mainPanel.Dock = DockStyle.Fill;
                mainPanel.AutoSize = true;


                var btnClose = new Button() { Text = PLUGIN.LANG("T_CLOSE"), Image = RES_IMAGE("close_16x16"), ImageAlign = ContentAlignment.MiddleLeft, Width = 160, Dock = DockStyle.Right };
                btnClose.Click += (s, arg) => { this.Close(); };

                var btnRefresh = new Button() { Text = PLUGIN.LANG("T_REFRESH"), Image = RES_IMAGE("refresh_16x16"), ImageAlign = ContentAlignment.MiddleLeft, Width = 160, Dock = DockStyle.Left };
                btnRefresh.Click += (s, arg) => { this.refreshData(); };




                //var btnSetPrice = new Button() { Width = 80, Text = PLUGIN.LANG("T_SAVE"), Image = RES_IMAGE("checked_16x16"), ImageAlign = ContentAlignment.MiddleLeft };
                //  btnSetPrice.Click += (s, arg) => { TAB_SETROW(pRow, "PRICE", (double)priceBox.Value); this.Close(); };


                var panelData = new Panel() { Dock = DockStyle.Fill };
                var panelFilterDateRange = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Bottom, AutoSize = true };
                var panelFilterWh = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Bottom, AutoSize = true };

                var panelFilterCl = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Bottom, AutoSize = true };
                var panelFilterMat = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Bottom, AutoSize = true };
                var panelBtn = new Panel() { Dock = DockStyle.Bottom, Height = 25 };


                //
                mainPanel.Controls.Add(panelData);

                mainPanel.Controls.Add(panelFilterDateRange);
                mainPanel.Controls.Add(panelFilterMat);
                mainPanel.Controls.Add(panelFilterCl);
                mainPanel.Controls.Add(panelFilterWh);





                mainPanel.Controls.Add(new Panel() { Tag = "Dummmy Padding", Height = 10, Dock = DockStyle.Bottom });
                //

                mainPanel.Controls.Add(panelBtn);


                panelBtn.Controls.Add(btnRefresh);
                panelBtn.Controls.Add(btnClose);
                //
                panelFilterDateRange.Controls.AddRange(
                    new Control[]{
                    new Label(){Text = PLUGIN.LANG("T_DATE_RANGE"), Width = 100, TextAlign = ContentAlignment.MiddleLeft,Dock = DockStyle.Left},
                    new DateTimePicker() { Value = filter.dateFrom, Name = "filter_date_from",Width=160 },
                    new DateTimePicker() { Value = filter.dateTo, Name = "filter_date_to",Width=160 },

                    new Button(){Text=PLUGIN.LANG("T_TODAY"),Image = RES_IMAGE("run_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_filter_date_today",Width=100 }, 
                    new Button(){Text=PLUGIN.LANG("T_MONTH"),Image = RES_IMAGE("run_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_filter_date_month",Width=100 },
                     new Button(){Text=PLUGIN.LANG("T_YEAR"),Image = RES_IMAGE("run_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_filter_date_year",Width=100 },
                    }
                    );

                panelFilterMat.Controls.AddRange(
             new Control[]{
                    new Label(){Text = PLUGIN.LANG("T_MATERIAL"), Width = 100, TextAlign = ContentAlignment.MiddleLeft,Dock = DockStyle.Left},
                    new NumericUpDown() { Minimum = 0, Maximum = int.MaxValue, DecimalPlaces = 0, Value = filter.material, Name = "filter_material",Width=80 },
                   new Button(){Text="",Image = RES_IMAGE("find_16x16"),ImageAlign = ContentAlignment.MiddleCenter,Name = "do_filter_material",Width=30 },
                   new TextBox(){Width=500,ReadOnly=true,Name="desc_filter_material"},

                    }
             );

                panelFilterCl.Controls.AddRange(
                 new Control[]{
                    new Label(){Text = PLUGIN.LANG("T_PERSONAL"), Width = 100, TextAlign = ContentAlignment.MiddleLeft,Dock = DockStyle.Left},
                    new NumericUpDown() { Minimum = 0, Maximum = int.MaxValue, DecimalPlaces = 0, Value = filter.client, Name = "filter_client",Width=80},
                   new Button(){Text="",Image = RES_IMAGE("find_16x16"),ImageAlign = ContentAlignment.MiddleCenter,Name = "do_filter_client",Width=30 },
                   new TextBox(){Width=500,ReadOnly=true,Name="desc_filter_client"}
                    }
                 );

                panelFilterWh.Controls.AddRange(
             new Control[]{
                    new Label(){Text = PLUGIN.LANG("T_WH"), Width = 100, TextAlign = ContentAlignment.MiddleLeft,Dock = DockStyle.Left},
                    new NumericUpDown() { Minimum = -1, Maximum = short.MaxValue, DecimalPlaces = 0, Value = filter.wh, Name = "filter_wh",Width=80 },
                    new Button(){Text="",Image = RES_IMAGE("find_16x16"),ImageAlign = ContentAlignment.MiddleCenter,Name = "do_filter_wh",Width=30 }
                    }
             );


                //        var grid = new DataGridView();


                var grid = createCreate("grid_tots");


                grid.Columns.AddRange(
                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_TITLE"), DataPropertyName = "TITLE", Width = 240, Frozen = true },
                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_QUANTITY"), DataPropertyName = "QTY", Width = 100 },
                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_VATEXC (-)\n(T_SYS_CURR1)"), DataPropertyName = "TOT", Width = 120 },
                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_VATINC (+)\n(T_SYS_CURR1)"), DataPropertyName = "TOTVAT", Width = 120 },
                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_VATEXC (-)\n(T_SYS_CURR2)"), DataPropertyName = "TOTREP", Width = 120 },
                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_VATINC (+)\n(T_SYS_CURR2)"), DataPropertyName = "TOTVATREP", Width = 120 }
                                   );

                foreach (DataGridViewColumn colObj in grid.Columns)
                {
                    colObj.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    colObj.SortMode = DataGridViewColumnSortMode.Programmatic;

                    switch (colObj.DataPropertyName)
                    {
                        case "TOT":
                        case "TOTVAT":
                        case "TOTREP":
                        case "TOTVATREP":

                            colObj.DefaultCellStyle.Format = "#,0.00;-#,0.00;''";
                            colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


                            if (colObj.DataPropertyName.EndsWith("REP"))
                                colObj.DefaultCellStyle.ForeColor = Color.SkyBlue;
                            else
                                colObj.DefaultCellStyle.ForeColor = Color.Blue;

                            break;
                        case "QTY":

                            colObj.DefaultCellStyle.Format = "#,0.##;-#,0.##;''";
                            colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            colObj.DefaultCellStyle.ForeColor = Color.Green;
                            break;
                        case "TITLE":
                            //  colObj.DefaultCellStyle.Format = "d";
                            colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                            break;
                    }
                }


                {

                    ///////////
                    grid.CellPainting += (sender, e) =>
                    {
                        // if (stripeRowBackColor)
                        {
                            DataGridView grid_ = sender as DataGridView;
                            if (grid_ == null)
                                return;

                            var bgColor = grid_.DefaultCellStyle.BackColor;

                            var rowObj_ = TOOL_GRID.GET_GRID_ROW(grid_, e.RowIndex);
                            if (rowObj_ == null)
                                return;

                            var colObj_ = e.ColumnIndex >= 0 ? grid_.Columns[e.ColumnIndex] : null;
                            if (colObj_ == null)
                                return;



                            var rowData_ = TOOL_GRID.GET_GRID_ROW_DATA(rowObj_);
                            if (rowData_ == null)
                                return;

                            var isSum = CASTASSHORT(TAB_GETROW(rowData_, "INDX")) > 100;
                            var isBold = (isSum || colObj_.DataPropertyName == "TITLE");



                            var font = e.CellStyle.Font;
                            if (font == null)
                                font = colObj_.InheritedStyle.Font;

                            if (isBold != font.Bold)
                            {

                                e.CellStyle.Font = new Font(font, FontStyle.Bold | font.Style);

                            }

                            if (isSum)
                                bgColor = Color.WhiteSmoke;

                            if (bgColor != e.CellStyle.BackColor)
                                e.CellStyle.BackColor = bgColor;

                        }
                    };

                    /////////
                }

                panelData.Controls.Add(grid);



            }



            DataGridView createCreate(string pName)
            {

                var grid = new DataGridView();
                grid.Name = pName;
                grid.Dock = DockStyle.Fill;
                grid.MultiSelect = false;
                grid.ReadOnly = true;
                grid.AllowUserToAddRows = false;
                grid.AllowUserToDeleteRows = false;
                grid.AllowUserToResizeColumns = false;
                grid.AllowUserToResizeRows = false;

                grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                grid.AutoGenerateColumns = false;
                grid.BackgroundColor = Color.White;
                grid.RowHeadersVisible = false;
                grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                //grid.Height = 110;
                // grid.AutoSize = true;
                grid.BorderStyle = BorderStyle.None;


                // grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                grid.ColumnHeadersDefaultCellStyle.Font = new Font(grid.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);


                grid.ColumnHeadersHeight *= 2;
                grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                return grid;
            }

            void bindFilterDo()
            {
                foreach (var o in CONTROL_DESTRUCT(this))
                {

                    var ctrl = o as Control;

                    if (ctrl == null)
                        continue;

                    switch (ctrl.Name)
                    {

                        case "do_filter_wh":
                        case "do_filter_client":
                        case "do_filter_material":
                        //
                        case "do_filter_date_today":
                        case "do_filter_date_month":
                        case "do_filter_date_year":
                            {
                                var btn = ctrl as Button;
                                btn.Click += filterDoBtnClick;

                            }
                            break;
                    }


                }
            }

            void filterDoBtnClick(object sender, EventArgs e)
            {
                try
                {

                    var btn = sender as Button;

                    if (btn == null || btn.Name == null)
                        return;

                    readFilter();


                    if (btn.Name == "do_filter_wh")
                    {

                        var valCtrl = CONTROL_SEARCH(this, "filter_wh") as NumericUpDown;
                        if (valCtrl != null)
                        {
                            DataRow[] res_ = PLUGIN.REF("ref.gen.rec.wh", "NR", CASTASSHORT(valCtrl.Value));
                            if (res_ != null && res_.Length > 0)
                            {
                                valCtrl.Value = CASTASSHORT(TAB_GETROW(res_[0], "NR"));
                            }
                        }

                        return;
                    }

                    if (btn.Name == "do_filter_client")
                    {

                        var valCtrl = CONTROL_SEARCH(this, "filter_client") as NumericUpDown;
                        if (valCtrl != null)
                        {
                            DataRow[] res_ = PLUGIN.REF("ref.fin.rec.client", "LOGICALREF", CASTASINT(valCtrl.Value));
                            if (res_ != null && res_.Length > 0)
                            {
                                valCtrl.Value = CASTASSHORT(TAB_GETROW(res_[0], "LOGICALREF"));
                            }
                        }

                        return;
                    }

                    if (btn.Name == "do_filter_material")
                    {

                        var valCtrl = CONTROL_SEARCH(this, "filter_material") as NumericUpDown;
                        if (valCtrl != null)
                        {
                            DataRow[] res_ = PLUGIN.REF("ref.mm.rec.mat", "LOGICALREF", CASTASINT(valCtrl.Value));
                            if (res_ != null && res_.Length > 0)
                            {
                                valCtrl.Value = CASTASSHORT(TAB_GETROW(res_[0], "LOGICALREF"));
                            }
                        }

                        return;
                    }


                    if (btn.Name.StartsWith("do_filter_date_"))
                    {

                        var ctrlDateFrom = CONTROL_SEARCH(this, "filter_date_from") as DateTimePicker;
                        var ctrlDateTo = CONTROL_SEARCH(this, "filter_date_to") as DateTimePicker;
                        if (ctrlDateFrom != null && ctrlDateTo != null)
                        {
                            var now = DateTime.Now.Date;

                            switch (btn.Name)
                            {
                                case "do_filter_date_today":
                                    ctrlDateFrom.Value = now;
                                    ctrlDateTo.Value = now;
                                    break;
                                case "do_filter_date_month":
                                    ctrlDateFrom.Value = new DateTime(now.Year, now.Month, 1);
                                    ctrlDateTo.Value = now;
                                    break;
                                case "do_filter_date_year":
                                    ctrlDateFrom.Value = new DateTime(now.Year, 1, 1);
                                    ctrlDateTo.Value = now;
                                    break;
                            }
                        }

                        return;
                    }
                }
                catch (Exception exc)
                {
                    PLUGIN.LOG(exc);
                    PLUGIN.MSGUSERERROR(exc.Message);
                }
            }

            void readFilter()
            {
                foreach (var o in CONTROL_DESTRUCT(this))
                {

                    var ctrl = o as Control;

                    if (ctrl == null)
                        continue;


                    var asNum = ctrl as NumericUpDown;

                    if (asNum != null && asNum.Text == "")
                    {
                        //!!! num has problem on delete text, Text empty but Value hidden
                        asNum.Value = 0;
                    }

                    var asDate = ctrl as DateTimePicker;

                    if (asDate != null)
                    {


                    }

                    switch (ctrl.Name)
                    {
                        case "filter_date_from":
                            filter.dateFrom = asDate.Value.Date;
                            break;
                        case "filter_date_to":
                            filter.dateTo = asDate.Value.Date;
                            break;
                        case "filter_wh":
                            filter.wh = (short)asNum.Value;
                            break;
                        case "filter_client":
                            filter.client = (int)asNum.Value;
                            break;
                        case "filter_material":
                            filter.material = (int)asNum.Value;
                            break;
                    }



                }



            }

            void refreshData()
            {
                try
                {

                    readFilter();


                    if (filter.dateTo < filter.dateFrom)
                        throw new Exception("T_MSG_INVALID_DATETIME");

                    var matDesc = GET_TITLE_MAT(filter.material);
                    var clDesc = GET_TITLE_CLIENT(filter.client);

                    var ctrlDescMat = CONTROL_SEARCH(this, "desc_filter_material") as TextBox;
                    var ctrlDescCl = CONTROL_SEARCH(this, "desc_filter_client") as TextBox;
                    if (ctrlDescMat != null)
                        ctrlDescMat.Text = matDesc;
                    if (ctrlDescCl != null)
                        ctrlDescCl.Text = clDesc;






                    var code = "grid_tots";

                    var table = new DataTable(code);

                    TAB_ADDCOL(table, "INDX", typeof(int));
                    TAB_ADDCOL(table, "TITLE", typeof(string));
                    //
                    TAB_ADDCOL(table, "QTY", typeof(double));
                    TAB_ADDCOL(table, "TOT", typeof(double));
                    TAB_ADDCOL(table, "TOTVAT", typeof(double));
                    TAB_ADDCOL(table, "TOTREP", typeof(double));
                    TAB_ADDCOL(table, "TOTVATREP", typeof(double));
                    //




                    var arrTrcodes = new short[] {
                 
                     
                     +08,+07,+02,+03,//sale
                     208,//net sale
                     +01,+06,//prch
                     201,//net prch
                     +14,//opn
                     +25,-25,+50,+51,
                     +11,+12,+13,//prod
                     303,//sum
                     //
                     401,//beg onhand
                     301,//sum
                     302,//sum
                     402 //end onhand

                    };
                    var arrTrcodesSign = new short[] {
                       
                     -01,-01,+01,+01,
                     0,//net sale
                     +01,-01,
                     0,//net prch
                     +01,
                     -01,+01,+01,-01,
                     -01,-01,+01,//prod
                      0,
                     0,//sum
                    0,//sum
                     0,//sum
                     0,
                    };





                    var saleValues = new Values() { title = PLUGIN.LANG("T_SALE - T_NET"), indx = 208 };
                    var prchValues = new Values() { title = PLUGIN.LANG("T_PURCHASE - T_NET"), indx = 201 };
                    var inValues = new Values() { title = PLUGIN.LANG("T_INPUT"), indx = 301 };
                    var outValues = new Values() { title = PLUGIN.LANG("T_OUTPUT"), indx = 302 };
                    var sumValues = new Values() { title = PLUGIN.LANG("T_TOTAL"), indx = 303 };

                    var onhandBegValues = new Values() { title = PLUGIN.LANG("T_ONHAND - T_BEGIN"), indx = 401 };
                    var onhandEndValues = new Values() { title = PLUGIN.LANG("T_ONHAND - T_END"), indx = 402 };


                    for (int t = 0; t < arrTrcodes.Length; ++t)
                    {
                        var trcode = arrTrcodes[t];
                        var sign = arrTrcodesSign[t];

                        short indx = ABS(trcode);

                        string title = "";

                        Values newValues = null;

                        if (trcode < 100)
                        {
                            switch (trcode)
                            {
                                case 25:
                                    title = "T_DOC_STOCK_SLIP_25 - T_OUTPUT";
                                    break;
                                case -25:
                                    title = "T_DOC_STOCK_SLIP_25 - T_INPUT";
                                    break;
                                default:
                                    title = "T_DOC_STOCK_SLIP_" + indx;
                                    break;
                            }

                            title = PLUGIN.LANG(title);

                            newValues = new Values() { title = title, indx = indx };

                            var rec = GET_MAT_TOTS(PLUGIN, filter.dateFrom, filter.dateTo, trcode, filter.material, filter.client, filter.wh);

                            newValues.read(rec);

                            if (trcode == 8 || trcode == 7 || trcode == 2 || trcode == 3)
                                saleValues.add(newValues, -1 * sign);



                            if (trcode == 1 || trcode == 6)
                                prchValues.add(newValues, +1 * sign);



                            if (sign > 0)
                                inValues.add(newValues);
                            else
                                outValues.add(newValues);

                            sumValues.add(newValues, sign);

                            newValues.write(table);



                            continue;
                        }

                        switch (trcode)
                        {
                            case 208:
                                saleValues.write(table);
                                break;
                            case 201:
                                prchValues.write(table);
                                break;
                            case 301:
                                inValues.write(table);
                                break;
                            case 302:
                                outValues.write(table);
                                break;
                            case 303:
                                sumValues.write(table);
                                break;
                            case 401://beg
                                onhandBegValues.qty = GET_MAT_ONHAND(PLUGIN, filter.dateFrom.AddDays(-1), filter.material, filter.wh);
                                onhandBegValues.write(table);
                                break;
                            case 402://end
                                onhandEndValues.qty = GET_MAT_ONHAND(PLUGIN, filter.dateTo, filter.material, filter.wh);
                                onhandEndValues.write(table);
                                break;

                        }


                    }



                    var grid = CONTROL_SEARCH(this, code) as DataGridView;
                    if (grid != null)
                        grid.DataSource = table;




                }
                catch (Exception exc)
                {
                    PLUGIN.LOG(exc);
                    PLUGIN.MSGUSERERROR(exc.Message);
                }
            }

            public static DataRow GET_MAT_TOTS(_PLUGIN pPLUGIN, DateTime df, DateTime dt, short trcode, int material, int client, short wh)
            {


                //   TAB_GETLASTROW(PLUGIN.SQL("SELECT 1.0 QTY,2.0 TOTNET,3.0 TOTNETVAT,4.0 TOTNETREP,5.0 TOTNETVATREP", new object[]{}));


                var listWhere = new List<string>();
                var listArgs = new List<object>();






                var sqlText =

    @"

SELECT  

		sum(AMOUNT) QTY, 
		sum(VATMATRAH) TOT,
		sum(VATMATRAH+VATAMNT ) TOTVAT,
		sum(case when REPORTRATE > 0 then (VATMATRAH )/REPORTRATE else 0 end) TOTREP,
		sum(case when REPORTRATE > 0 then (VATMATRAH+VATAMNT )/REPORTRATE else 0 end) TOTVATREP
 
		FROM LG_$FIRM$_$PERIOD$_STLINE WITH(NOLOCK)
		WHERE 
        ( DATE_ BETWEEN @P1 AND @P2 AND TRCODE = @P3 AND LINETYPE = 0 AND CANCELLED = 0 ) 
   
			";

                listArgs.Add(df);
                listArgs.Add(dt);
                listArgs.Add(ABS(trcode));

                if (trcode == 25)
                {
                    sqlText += " AND (IOCODE IN (3,4)) ";
                }
                else
                    if (trcode == -25)
                    {
                        sqlText += " AND (IOCODE IN (1,2)) ";
                    }


                if (material > 0)//!
                { listArgs.Add(material); listWhere.Add("STOCKREF = @P" + listArgs.Count); }
                if (client > 0)//!
                { listArgs.Add(client); listWhere.Add("CLIENTREF = @P" + listArgs.Count); }

                if (wh >= 0)
                { listArgs.Add(wh); listWhere.Add("SOURCEINDEX = @P" + listArgs.Count); }


                if (listWhere.Count > 0)
                {

                    sqlText += (" AND " + string.Join(" AND ", listWhere.ToArray()));

                }


                return TAB_GETLASTROW(pPLUGIN.SQL(sqlText, listArgs.ToArray()));




            }


            public static double GET_MAT_ONHAND(_PLUGIN pPLUGIN, DateTime pDate, object pMatRef, short pWhNr)
            {
                return CASTASDOUBLE(pPLUGIN.SQLSCALAR(
     @"
	SELECT  
    SUM(ONHAND) 
	FROM LG_$FIRM$_$PERIOD$_STINVTOT T 
--$MS$--WITH(NOLOCK)
	WHERE 	 
        T.STOCKREF = @P1 AND 
        T.DATE_ <= @P2 AND
        T.INVENNO = @P3 
		
 
", new object[] { pMatRef, pDate.Date, pWhNr }));

            }



            string GET_TITLE_CLIENT(object pLRef)
            {


                var tab = PLUGIN.SQL(" SELECT  C.CODE AS CODE,C.DEFINITION_ AS NAME FROM LG_$FIRM$_CLCARD C WHERE LOGICALREF = @P1",
new object[] { pLRef });

                var rec = TAB_GETLASTROW(tab);

                if (rec != null)
                {
                    var code = CASTASSTRING(TAB_GETROW(rec, "CODE"));
                    var name = CASTASSTRING(TAB_GETROW(rec, "NAME"));
                    return code + "/" + name;
                }

                return "";

            }

            string GET_TITLE_MAT(object pLRef)
            {

                var tab = PLUGIN.SQL(" SELECT  C.CODE AS CODE,C.NAME AS NAME FROM LG_$FIRM$_ITEMS C WHERE LOGICALREF = @P1",
new object[] { pLRef });

                var rec = TAB_GETLASTROW(tab);

                if (rec != null)
                {
                    var code = CASTASSTRING(TAB_GETROW(rec, "CODE"));
                    var name = CASTASSTRING(TAB_GETROW(rec, "NAME"));
                    return code + "/" + name;
                }

                return "";

            }
            public class Filter
            {
                public DateTime dateFrom = DateTime.Now.Date;
                public DateTime dateTo = DateTime.Now.Date;
                public short wh = -1;

                public int client = 0;
                public int material = 0;
            }


            public class Values
            {
                public short indx = 0;
                public string title = "";



                public double qty = 0.0;
                public double tot = 0.0;
                public double totVat = 0.0;
                public double totRep = 0.0;
                public double totVatRep = 0.0;



                public void read(DataRow rec)
                {



                    qty = CASTASDOUBLE(TAB_GETROW(rec, "QTY"));
                    tot = CASTASDOUBLE(TAB_GETROW(rec, "TOT"));
                    totVat = CASTASDOUBLE(TAB_GETROW(rec, "TOTVAT"));
                    totRep = CASTASDOUBLE(TAB_GETROW(rec, "TOTREP"));
                    totVatRep = CASTASDOUBLE(TAB_GETROW(rec, "TOTVATREP"));


                }



                public void write(DataRow rec)
                {

                    TAB_SETROW(rec, "INDX", indx);
                    TAB_SETROW(rec, "TITLE", title);
                    TAB_SETROW(rec, "QTY", qty);
                    TAB_SETROW(rec, "TOT", tot);
                    TAB_SETROW(rec, "TOTVAT", totVat);
                    TAB_SETROW(rec, "TOTREP", totRep);
                    TAB_SETROW(rec, "TOTVATREP", totVatRep);

                }



                public DataRow write(DataTable tab)
                {
                    var rec = tab.NewRow();

                    write(rec);

                    tab.Rows.Add(rec);

                    return rec;

                }
                public void add(Values pSrc, int pSign = 1)
                {
                    qty += (pSign * pSrc.qty);
                    tot += (pSign * pSrc.tot);
                    totVat += (pSign * pSrc.totVat);
                    totRep += (pSign * pSrc.totRep);
                    totVatRep += (pSign * pSrc.totVatRep);
                }
            }


        }





        public static string MY_CHOOSE_SQL(string pSqlMs, string pSqlPg)
        {

            if (ISMSSQL())
                return pSqlMs;

            if (ISPOSTGRESQL())
                return pSqlPg;


            throw new Exception("Undefined datasource");
        }


        #endregion