#line 2


  #region BODY
        //BEGIN

        const int VERSION = 21;
        const string FILE = "plugin.sys.event.clfintots.pls";


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

                    x.MY_CLFINTOTS_USER = s.MY_CLFINTOTS_USER;

                    //

                    _SETTINGS.BUF = x;

                }

                public string MY_CLFINTOTS_USER;



            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC) //, "ava.production.config")
            {

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active On User")]
            public string MY_CLFINTOTS_USER
            {
                get
                {
                    return (_GET("MY_CLFINTOTS_USER", "1,2"));
                }
                set
                {
                    _SET("MY_CLFINTOTS_USER", value);
                }

            }



            //

            public static bool ISUSEROK(_PLUGIN pPLUGIN)
            {
                return Array.IndexOf<string>(
                     EXPLODELIST(BUF.MY_CLFINTOTS_USER),
                     FORMAT(pPLUGIN.GETSYSPRM_USER())
                     ) >= 0;
            }

        }

        #endregion

        #region TEXT

        public class TEXT
        {
            public const string text_DESC = "Client Finance Tots";


            public class L
            {

            }
        }

        const string event_CLFINTOTS_ = "hadlericom_clfintots_";
        const string event_CLFINTOTS_MONTH = "hadlericom_clfintots_month";
        const string event_CLFINTOTS_RANGE = "hadlericom_clfintots_range";
        const string event_CLFINTOTS_LASTTRAN = "hadlericom_clfintots_lasttran";
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



            public string TOTS_BY_MONTH = "By Month Account Finance Tots";
            public string TOTS_BY_RANGE = "By Date Range Account Finance Tots";
            public string TOTS_LAST = "Operations";
            public string TOTS_LAST_SHORT = "Last Opr.";
            public string SUFIX_DEBIT = "(D)";
            public string SUFIX_CREDIT = "(C)";

            public string COUNT_DOC = "Docs Count";


            public void lang_az()
            {

                TOTS_BY_MONTH = "Ayliq Cari Hesab Toplamları";
                TOTS_BY_RANGE = "Tarih Aralığı Cari Hesab Toplamları";
                TOTS_LAST = "Cari Hesab Son Hərəkətləri";
                TOTS_LAST_SHORT = "Hərəkətlər";

                SUFIX_DEBIT = "(B)";
                SUFIX_CREDIT = "(A)";

                COUNT_DOC = "Sənəd Sayı";
            }

            public void lang_ru()
            {

                TOTS_BY_MONTH = "Финфнсовые Итоги Счетов По Месяцам";
                TOTS_BY_RANGE = "Финфнсовые Итоги Счетов За Интервал";
                TOTS_LAST = "Последние Операции со Счетом";
                TOTS_LAST_SHORT = "Операции";
                SUFIX_DEBIT = "(Д)";
                SUFIX_CREDIT = "(К)";

                COUNT_DOC = "Кол. Док.";
            }

            public void lang_tr()
            {


                TOTS_BY_MONTH = "Aylik Cari Hesap Toplamlari";
                TOTS_BY_RANGE = "Tarih Aralığı Cari Hesap Toplamlari";
                TOTS_LAST = "Cari Hesap Ekstrası";
                TOTS_LAST_SHORT = "C.H. Ekstra";
                SUFIX_DEBIT = "(B)";
                SUFIX_CREDIT = "(A)";
                COUNT_DOC = "Fiş Sayısı";
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




                var isListClient = fn.StartsWith("ref.fin.rec.client");


                if (isListClient)
                {
                    foreach (var ctrl in CONTROL_DESTRUCT(FORM))
                    {
                        {
                            var menuItem = ctrl as ToolStripItem;
                            if (menuItem != null && menuItem.Name == "cMenuGridInfoPlugin")
                            {
                                {
                                    var args = new Dictionary<string, object>() { 
 
            { "_cmd" ,"add"},
            { "_type" ,"item"},
            { "_name" , event_CLFINTOTS_MONTH},

            { "_infoLocation" ,"event"},
            { "_infoArg" ,event_CLFINTOTS_MONTH},

            { "Text" ,_LANG.L.TOTS_BY_MONTH},
            { "ImageName" ,"sum_16x16"},
             { "Name" ,event_CLFINTOTS_MONTH},
            };

                                    RUNUIINTEGRATION(menuItem, args);

                                }


                                {
                                    var args = new Dictionary<string, object>() { 
 
            { "_cmd" ,"add"},
            { "_type" ,"item"},
            { "_name" , event_CLFINTOTS_RANGE},

            { "_infoLocation" ,"event"},
            { "_infoArg" ,event_CLFINTOTS_RANGE},

            { "Text" ,_LANG.L.TOTS_BY_RANGE},
            { "ImageName" ,"calendar_16x16"},
             { "Name" ,event_CLFINTOTS_RANGE},
            };

                                    RUNUIINTEGRATION(menuItem, args);

                                }




                                {
                                    var args = new Dictionary<string, object>() { 
 
            { "_cmd" ,"add"},
            { "_type" ,"item"},
            { "_name" , event_CLFINTOTS_LASTTRAN},

            { "_infoLocation" ,"event"},
            { "_infoArg" ,event_CLFINTOTS_LASTTRAN},

            { "Text" ,_LANG.L.TOTS_LAST},
            { "ImageName" ,"account_balances_16x16"},
             { "Name" ,event_CLFINTOTS_LASTTRAN},
            };

                                    RUNUIINTEGRATION(menuItem, args);

                                }



                            }
                        }


                        {

                            var cPanelBtnSub = ctrl as Control;
                            if (cPanelBtnSub != null && cPanelBtnSub.Name == "cPanelBtnSub")
                            {
                                _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_CLFINTOTS_LASTTRAN, _LANG.L.TOTS_LAST_SHORT, "account_balances_16x16");
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


                    case event_CLFINTOTS_MONTH:
                        {
                            CLFINTOTS_MONTH(arg1 as DataRow);
                        }
                        break;

                    case event_CLFINTOTS_RANGE:
                        {
                            CLFINTOTS_RANGE(arg1 as DataRow);
                        }
                        break;

                    case event_CLFINTOTS_LASTTRAN:
                        {
                            DataRow row = arg1 as DataRow;

                            if (row == null)
                            {
                                var grid_ = CONTROL_SEARCH(arg1 as Form, "cGrid") as DataGridView;
                                if (grid_ != null)
                                    row = TOOL_GRID.GET_GRID_ROW_DATA(grid_);
                            }


                            CLFINTOTS_LASTTRAN(row);
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

        private void CLFINTOTS_MONTH(DataRow pRecord)
        {

            try
            {


                var args = new FormMonthTots.Filter()
                {

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


        private void CLFINTOTS_LASTTRAN(DataRow pRecord)
        {
            if (pRecord == null)
                return;

            try
            {


                var args = new FormLastTran.Filter()
                {

                    client = CASTASINT(MY_GET_CLIENT_LREF(pRecord)),
                    dateFrom = GETSYSPRM_PERIODBEG().Date,
                    dateTo = DateTime.Now.Date
                };


                Form form = new FormLastTran(this, args);

                form.Show();

            }
            catch (Exception exc)
            {
                LOG(exc);
                MSGUSERERROR(exc.Message);
            }

        }


        private void CLFINTOTS_RANGE(DataRow pRecord)
        {

            try
            {


                var args = new FormDateRangeTots.Filter()
                {

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

                var panelFilterCl = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Bottom, AutoSize = true };
                var panelBtn = new Panel() { Dock = DockStyle.Bottom, Height = 25 };


                //
                mainPanel.Controls.Add(panelData);

                mainPanel.Controls.Add(panelFilterYear);

                mainPanel.Controls.Add(panelFilterCl);






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



                panelFilterCl.Controls.AddRange(
                 new Control[]{
                    new Label(){Text = PLUGIN.LANG("T_PERSONAL"), Width = 80, TextAlign = ContentAlignment.MiddleLeft,Dock = DockStyle.Left},
                    new NumericUpDown() { Minimum = 0, Maximum = int.MaxValue, DecimalPlaces = 0, Value = filter.client, Name = "filter_client",Width=80},
                   new Button(){Text="",Image = RES_IMAGE("find_16x16"),ImageAlign = ContentAlignment.MiddleCenter,Name = "do_filter_client",Width=30 },
                   new TextBox(){Width=500,ReadOnly=true,Name="desc_filter_client"}
                    }
                 );




                //        var grid = new DataGridView();


                var tabs = new TabControl() { Dock = DockStyle.Fill };



                TabPage page_grid_prch_gross = null;
                TabPage page_grid_prch_ret = null;
                TabPage page_grid_sale_gross = null;
                TabPage page_grid_sale_ret = null;
                //

                TabPage page_grid_cash = null;
                TabPage page_grid_bank = null;

                tabs.TabPages.Add(page_grid_sale_gross = new TabPage() { Text = PLUGIN.LANG("T_SALE - T_GROSS") });
                tabs.TabPages.Add(page_grid_sale_ret = new TabPage() { Text = PLUGIN.LANG("T_SALE - T_RETURN") });
                tabs.TabPages.Add(page_grid_prch_gross = new TabPage() { Text = PLUGIN.LANG("T_PURCHASE - T_GROSS") });
                tabs.TabPages.Add(page_grid_prch_ret = new TabPage() { Text = PLUGIN.LANG("T_PURCHASE - T_RETURN") });

                tabs.TabPages.Add(page_grid_cash = new TabPage() { Text = PLUGIN.LANG("T_CASH") });
                tabs.TabPages.Add(page_grid_bank = new TabPage() { Text = PLUGIN.LANG("T_BANK") });


                DataGridView grid_prch_gross = null;
                DataGridView grid_prch_ret = null;
                DataGridView grid_sale_gross = null;
                DataGridView grid_sale_ret = null;

                DataGridView grid_cash = null;
                DataGridView grid_bank = null;

                page_grid_prch_gross.Controls.Add(grid_prch_gross = createCreate("grid_prch_gross"));
                page_grid_prch_ret.Controls.Add(grid_prch_ret = createCreate("grid_prch_ret"));
                page_grid_sale_gross.Controls.Add(grid_sale_gross = createCreate("grid_sale_gross"));
                page_grid_sale_ret.Controls.Add(grid_sale_ret = createCreate("grid_sale_ret"));

                page_grid_cash.Controls.Add(grid_cash = createCreate("grid_cash"));
                page_grid_bank.Controls.Add(grid_bank = createCreate("grid_bank"));

                foreach (var grid in new DataGridView[] { grid_prch_gross, grid_prch_ret, grid_sale_gross, grid_sale_ret })
                    grid.Columns.AddRange(
                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_MONTH"), DataPropertyName = "TITLE", Width = 120 },

                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_VATEXC (-)\n(T_SYS_CURR1)"), DataPropertyName = "TOT", Width = 120 },
                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_VATINC (+)\n(T_SYS_CURR1)"), DataPropertyName = "TOTVAT", Width = 120 },

                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_VATEXC (-)\n(T_SYS_CURR2)"), DataPropertyName = "TOTTREP", Width = 120 },
                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_VATINC (+)\n(T_SYS_CURR2)"), DataPropertyName = "TOTVATREP", Width = 120 }
                                  );


                foreach (var grid in new DataGridView[] { grid_cash })
                {
                    grid.Columns.AddRange(
                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_MONTH"), DataPropertyName = "TITLE", Width = 120 },

                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_DOC_FIN_CASH_11\n(T_SYS_CURR1)"), DataPropertyName = "DEBIT", Width = 120 },
                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_DOC_FIN_CASH_12\n(T_SYS_CURR1)"), DataPropertyName = "CREDIT", Width = 120 },

                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_DOC_FIN_CASH_11\n(T_SYS_CURR2)"), DataPropertyName = "DEBITREP", Width = 120 },
                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_DOC_FIN_CASH_12\n(T_SYS_CURR2)"), DataPropertyName = "CREDITREP", Width = 120 }
                                  );
                }


                foreach (var grid in new DataGridView[] { grid_bank })
                {
                    grid.Columns.AddRange(
                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_MONTH"), DataPropertyName = "TITLE", Width = 120 },

                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_DOC_FIN_BANK_3\n(T_SYS_CURR1)"), DataPropertyName = "DEBIT", Width = 120 },
                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_DOC_FIN_BANK_4\n(T_SYS_CURR1)"), DataPropertyName = "CREDIT", Width = 120 },

                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_DOC_FIN_BANK_3\n(T_SYS_CURR2)"), DataPropertyName = "DEBITREP", Width = 120 },
                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_DOC_FIN_BANK_4\n(T_SYS_CURR2)"), DataPropertyName = "CREDITREP", Width = 120 }
                                  );
                }

                foreach (var grid in new DataGridView[] {
                    grid_prch_gross, grid_prch_ret, grid_sale_gross, grid_sale_ret,
                grid_cash, grid_bank
                })
                    foreach (DataGridViewColumn colObj in grid.Columns)
                    {
                        colObj.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        colObj.SortMode = DataGridViewColumnSortMode.Programmatic;

                        switch (colObj.DataPropertyName)
                        {
                            case "DEBIT":
                            case "CREDIT":
                            case "DEBITREP":
                            case "CREDITREP":
                            //
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
                            case "TITLE":
                                //  colObj.DefaultCellStyle.Format = "d";
                                colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                                break;
                        }
                    }

                foreach (var grid in new DataGridView[] {
                    grid_prch_gross, grid_prch_ret, grid_sale_gross, grid_sale_ret,
                   grid_cash, grid_bank
                })
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
                        case "do_filter_client":
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

                        case "filter_client":
                            filter.client = (int)asNum.Value;
                            break;

                    }



                }
            }

            void refreshData()
            {
                try
                {

                    readFilter();



                    var clDesc = GET_TITLE_CLIENT(filter.client);


                    var ctrlDescCl = CONTROL_SEARCH(this, "desc_filter_client") as TextBox;

                    if (ctrlDescCl != null)
                        ctrlDescCl.Text = clDesc;

                    this.Text = string.Format("{0}", _LANG.L.TOTS_BY_MONTH);


                    var codes = new string[] { 
                        "grid_sale_gross", "grid_sale_ret", "grid_prch_gross", "grid_prch_ret",
                     "grid_cash", "grid_bank"
                    };
                    var trcodes = new short[][] { 
                        new short[] { 8, 7, }, new short[] { 3, 2 }, new short[] { 1 }, new short[] { 6 },
                        new short[] { 1001,1002 },new short[] { 720,721 }
                    };

                    for (int c = 0; c < codes.Length; ++c)
                    {
                        var code = codes[c];
                        switch (code)
                        {
                            case "grid_cash":
                            case "grid_bank":
                                {
                                    var table = new DataTable(code);

                                    TAB_ADDCOL(table, "INDX", typeof(short));
                                    TAB_ADDCOL(table, "TITLE", typeof(string));
                                    //
                                    TAB_ADDCOL(table, "DEBIT", typeof(double));
                                    TAB_ADDCOL(table, "CREDIT", typeof(double));
                                    TAB_ADDCOL(table, "DEBITREP", typeof(double));
                                    TAB_ADDCOL(table, "CREDITREP", typeof(double));


                                    var sumValues = new ValuesDebitCredit() { title = PLUGIN.LANG("T_TOTAL"), indx = 13 };


                                    for (int m = 1; m <= 13; ++m)
                                    {
                                        int indx = m;

                                        if (m < 13)
                                        {

                                            var newValues = new ValuesDebitCredit() { title = "" + filter.year + " - " + PLUGIN.LANG("T_MONTH_" + m), indx = (short)m };

                                            var df = new DateTime(filter.year, m, 1);
                                            var dt = df.AddMonths(+1).AddDays(-1);

                                            var arrTrcode = trcodes[c];

                                            var debit = new ValuesSimple(MY_TOTS_CLFLINE(df, dt, arrTrcode[0], filter.client));
                                            var credit = new ValuesSimple(MY_TOTS_CLFLINE(df, dt, arrTrcode[1], filter.client));

                                            newValues.debit += debit.val;
                                            newValues.debitRep += debit.valRep;

                                            newValues.credit += credit.val;
                                            newValues.creditRep += credit.valRep;

                                            newValues.write(table);

                                            sumValues.add(newValues);
                                        }
                                        else
                                        {
                                            if (m == 13)
                                                sumValues.write(table);

                                        }

                                    }


                                    var grid = CONTROL_SEARCH(this, code) as DataGridView;
                                    if (grid != null)
                                        grid.DataSource = table;

                                }
                                break;
                            case "grid_sale_gross":
                            case "grid_sale_ret":
                            case "grid_prch_gross":
                            case "grid_prch_ret":
                                {


                                    var table = new DataTable(code);

                                    TAB_ADDCOL(table, "INDX", typeof(int));
                                    TAB_ADDCOL(table, "TITLE", typeof(string));
                                    //
                                    TAB_ADDCOL(table, "TOT", typeof(double));
                                    TAB_ADDCOL(table, "TOTVAT", typeof(double));
                                    TAB_ADDCOL(table, "TOTREP", typeof(double));
                                    TAB_ADDCOL(table, "TOTVATREP", typeof(double));

                                    var sumValues = new Values() { title = PLUGIN.LANG("T_TOTAL"), indx = 13 };

                                    for (int m = 1; m <= 13; ++m)
                                    {
                                        int indx = m;




                                        if (m < 13)
                                        {

                                            var newValues = new Values() { title = "" + filter.year + " - " + PLUGIN.LANG("T_MONTH_" + m), indx = (short)m };

                                            var df = new DateTime(filter.year, m, 1);
                                            var dt = df.AddMonths(+1).AddDays(-1);

                                            var arrTrcode = trcodes[c];



                                            for (int t = 0; t < arrTrcode.Length; ++t)
                                            {
                                                var trcode = arrTrcode[t];


                                                var rec = MY_TOTS_INVOICE(df, dt, trcode, filter.client);

                                                newValues.add(rec);

                                            }


                                            newValues.write(table);

                                            sumValues.add(newValues);
                                        }
                                        else
                                        {
                                            if (m == 13)
                                                sumValues.write(table);


                                        }

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

            private DataRow MY_TOTS_INVOICE(DateTime df, DateTime dt, short trcode, int client)
            {


                //   TAB_GETLASTROW(PLUGIN.SQL("SELECT 1.0 QTY,2.0 TOT,3.0 TOTVAT,4.0 TOTREP,5.0 TOTVATREP", new object[]{}));


                var listWhere = new List<string>();
                var listArgs = new List<object>();


                var sqlText =

    @"

SELECT  

		sum(NETTOTAL - TOTALVAT) TOT,
		sum(NETTOTAL ) TOTVAT,
		sum(case when REPORTRATE > 0 then (NETTOTAL - TOTALVAT )/REPORTRATE else 0 end) TOTREP,
		sum(case when REPORTRATE > 0 then (NETTOTAL )/REPORTRATE else 0 end) TOTVATREP
 
		FROM LG_$FIRM$_$PERIOD$_INVOICE WITH(NOLOCK)
		WHERE 
        ( DATE_ BETWEEN @P1 AND @P2 AND TRCODE = @P3 AND CANCELLED = 0 ) 
   
			";

                listArgs.Add(df);
                listArgs.Add(dt);
                listArgs.Add(trcode);


                if (client > 0)//!
                { listArgs.Add(client); listWhere.Add("CLIENTREF = @P" + listArgs.Count); }

                if (listWhere.Count > 0)
                {

                    sqlText += (" AND " + string.Join(" AND ", listWhere.ToArray()));

                }


                return TAB_GETLASTROW(PLUGIN.SQL(sqlText, listArgs.ToArray()));




            }



            private DataRow MY_TOTS_CLFLINE(DateTime df, DateTime dt, short trcode, int client)
            {


                //   TAB_GETLASTROW(PLUGIN.SQL("SELECT 1.0 QTY,2.0 TOT,3.0 TOTVAT,4.0 TOTREP,5.0 TOTVATREP", new object[]{}));


                var listWhere = new List<string>();
                var listArgs = new List<object>();

                short _modnr = (short)(trcode / (short)100);
                short _trcode = (short)(trcode % (short)100);

                var sqlText =

    @"

SELECT  

		sum(AMOUNT) TOT,
		sum(REPORTNET ) TOTREP 

		FROM LG_$FIRM$_$PERIOD$_CLFLINE WITH(NOLOCK)
		WHERE 
        ( DATE_ BETWEEN @P1 AND @P2 AND MODULENR = @P3 AND TRCODE = @P4 AND CANCELLED = 0 ) 
   
			";

                listArgs.Add(df);
                listArgs.Add(dt);
                listArgs.Add(_modnr);
                listArgs.Add(_trcode);

                if (client > 0)//!
                { listArgs.Add(client); listWhere.Add("CLIENTREF = @P" + listArgs.Count); }

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


                public int client = 0;

            }


            public class Values
            {
                public short indx = 0;
                public string title = "";



                public double tot = 0.0;
                public double totVat = 0.0;
                public double totRep = 0.0;
                public double totVatRep = 0.0;



                public void read(DataRow rec)
                {




                    tot = CASTASDOUBLE(TAB_GETROW(rec, "TOT"));
                    totVat = CASTASDOUBLE(TAB_GETROW(rec, "TOTVAT"));
                    totRep = CASTASDOUBLE(TAB_GETROW(rec, "TOTREP"));
                    totVatRep = CASTASDOUBLE(TAB_GETROW(rec, "TOTVATREP"));


                }



                public void write(DataRow rec)
                {

                    TAB_SETROW(rec, "INDX", indx);
                    TAB_SETROW(rec, "TITLE", title);

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

                    tot += (pSign * pSrc.tot);
                    totVat += (pSign * pSrc.totVat);
                    totRep += (pSign * pSrc.totRep);
                    totVatRep += (pSign * pSrc.totVatRep);
                }

                public void add(DataRow pSrcRec, int pSign = 1)
                {
                    var v = new Values();
                    v.read(pSrcRec);
                    add(v, pSign);

                }
            }


            public class ValuesDebitCredit
            {
                public short indx = 0;
                public string title = "";



                public double debit = 0.0;
                public double credit = 0.0;
                public double debitRep = 0.0;
                public double creditRep = 0.0;



                public void read(DataRow rec)
                {




                    debit = CASTASDOUBLE(TAB_GETROW(rec, "DEBIT"));
                    credit = CASTASDOUBLE(TAB_GETROW(rec, "CREDIT"));
                    debitRep = CASTASDOUBLE(TAB_GETROW(rec, "DEBITREP"));
                    creditRep = CASTASDOUBLE(TAB_GETROW(rec, "CREDITREP"));


                }



                public void write(DataRow rec)
                {

                    TAB_SETROW(rec, "INDX", indx);
                    TAB_SETROW(rec, "TITLE", title);

                    TAB_SETROW(rec, "DEBIT", debit);
                    TAB_SETROW(rec, "CREDIT", credit);
                    TAB_SETROW(rec, "DEBITREP", debitRep);
                    TAB_SETROW(rec, "CREDITREP", creditRep);

                }



                public DataRow write(DataTable tab)
                {
                    var rec = tab.NewRow();

                    write(rec);

                    tab.Rows.Add(rec);

                    return rec;

                }
                public void add(ValuesDebitCredit pSrc, int pSign = 1)
                {

                    debit += (pSign * pSrc.debit);
                    credit += (pSign * pSrc.credit);
                    debitRep += (pSign * pSrc.debitRep);
                    creditRep += (pSign * pSrc.creditRep);
                }

                public void add(DataRow pSrcRec, int pSign = 1)
                {
                    var v = new ValuesDebitCredit();
                    v.read(pSrcRec);
                    add(v, pSign);

                }
            }



            public class ValuesSimple
            {

                public double val = 0.0;
                public double valRep = 0.0;

                public ValuesSimple(DataRow rec)
                {

                    val = CASTASDOUBLE(TAB_GETROW(rec, "TOT"));
                    valRep = CASTASDOUBLE(TAB_GETROW(rec, "TOTREP"));

                }


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

                var panelFilterCl = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Bottom, AutoSize = true };
                var panelBtn = new Panel() { Dock = DockStyle.Bottom, Height = 25 };


                //
                mainPanel.Controls.Add(panelData);

                mainPanel.Controls.Add(panelFilterDateRange);

                mainPanel.Controls.Add(panelFilterCl);






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



                panelFilterCl.Controls.AddRange(
                 new Control[]{
                    new Label(){Text = PLUGIN.LANG("T_PERSONAL"), Width = 100, TextAlign = ContentAlignment.MiddleLeft,Dock = DockStyle.Left},
                    new NumericUpDown() { Minimum = 0, Maximum = int.MaxValue, DecimalPlaces = 0, Value = filter.client, Name = "filter_client",Width=80},
                   new Button(){Text="",Image = RES_IMAGE("find_16x16"),ImageAlign = ContentAlignment.MiddleCenter,Name = "do_filter_client",Width=30 },
                   new TextBox(){Width=500,ReadOnly=true,Name="desc_filter_client"}
                    }
                 );




                //        var grid = new DataGridView();


                var grid = createGrid("grid_tots");


                grid.Columns.AddRange(
                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_TITLE"), DataPropertyName = "TITLE", Width = 300, Frozen = true },

                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_VATINC (+)\n(T_SYS_CURR1)"), DataPropertyName = "TOT", Width = 120 },
                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_VATINC (+)\n(T_SYS_CURR2)"), DataPropertyName = "TOTREP", Width = 120 },

                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_VATEXC (-)\n(T_SYS_CURR1)"), DataPropertyName = "TOTNOVAT", Width = 120 },
                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_VATEXC (-)\n(T_SYS_CURR2)"), DataPropertyName = "TOTNOVATREP", Width = 120 }

                                   );

                foreach (DataGridViewColumn colObj in grid.Columns)
                {
                    colObj.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    colObj.SortMode = DataGridViewColumnSortMode.Programmatic;

                    switch (colObj.DataPropertyName)
                    {
                        case "TOT":
                        case "TOTREP":
                        case "TOTNOVAT":
                        case "TOTNOVATREP":
                            colObj.DefaultCellStyle.Format = "#,0.00;-#,0.00;''";
                            colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


                            if (colObj.DataPropertyName.EndsWith("REP"))
                                colObj.DefaultCellStyle.ForeColor = Color.SkyBlue;
                            else
                                colObj.DefaultCellStyle.ForeColor = Color.Blue;

                            break;
                        case "TITLE":
                            //  colObj.DefaultCellStyle.Format = "d";
                            colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                            break;
                    }
                }


                {

                    ///////////

                    grid.CellFormatting += (sender, e) =>
                    {

                        DataGridView grid_ = sender as DataGridView;
                        if (grid_ == null)
                            return;

                        var rowObj_ = TOOL_GRID.GET_GRID_ROW(grid_, e.RowIndex);
                        if (rowObj_ == null)
                            return;

                        var colObj_ = e.ColumnIndex >= 0 ? grid_.Columns[e.ColumnIndex] : null;
                        if (colObj_ == null)
                            return;


                        var rowData_ = TOOL_GRID.GET_GRID_ROW_DATA(rowObj_);
                        if (rowData_ == null)
                            return;

                        var indx = CASTASSHORT(TAB_GETROW(rowData_, "INDX"));


                        if (indx == 9903 || indx == 9906)
                        {
                            if (colObj_.DataPropertyName == "TOT" || colObj_.DataPropertyName == "TOTREP")
                            {
                                //balance
                                var val = CASTASDOUBLE(e.Value);

                                if (!ISNUMZERO(val))
                                {
                                    var valStr = FORMAT(ABS(val), e.CellStyle.Format) + " " + (val > 0 ? _LANG.L.SUFIX_DEBIT : _LANG.L.SUFIX_CREDIT);
                                    e.Value = valStr;
                                }
                            }
                        }
                    };

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

                            var isSum = CASTASSHORT(TAB_GETROW(rowData_, "INDX")) > 9900;
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



            DataGridView createGrid(string pName)
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
                        case "do_filter_client":
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

                        case "filter_client":
                            filter.client = (int)asNum.Value;
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


                    var clDesc = GET_TITLE_CLIENT(filter.client);


                    var ctrlDescCl = CONTROL_SEARCH(this, "desc_filter_client") as TextBox;

                    if (ctrlDescCl != null)
                        ctrlDescCl.Text = clDesc;






                    var code = "grid_tots";

                    var table = new DataTable(code);

                    TAB_ADDCOL(table, "INDX", typeof(int));
                    TAB_ADDCOL(table, "TITLE", typeof(string));

                    //

                    TAB_ADDCOL(table, "TOT", typeof(double));
                    TAB_ADDCOL(table, "TOTREP", typeof(double));
                    TAB_ADDCOL(table, "TOTNOVAT", typeof(double));
                    TAB_ADDCOL(table, "TOTNOVATREP", typeof(double));

                    //




                    var arrTrcodes = new short[] {
                 
                    431,
                    -436,
                    9901,
                    -438,
                    -437,
                    432,
                    433,
                    9902,

                     1001,
                     -1002,
                     720,
                     -721,


                      501,
                     -502,
                      503,
                     -504,

                     9903,//beg balance
                     9904,//  debit
                     9905,//  credit
                     9906,//end balance
 
                    };


                    var prchValues = new Values() { title = PLUGIN.LANG("T_PURCHASE - T_NET"), indx = 9901 };
                    var saleValues = new Values() { title = PLUGIN.LANG("T_SALE - T_NET"), indx = 9902 };


                    var balance = new ValuesFinBalance();
                    if (!ISEMPTYLREF(filter.client))
                        balance.read(MY_BALANCE(PLUGIN, filter.dateFrom, filter.dateTo, filter.client));

                    var balanceBeg = new Values() { title = PLUGIN.LANG("T_BALANCEBEG"), indx = 9903, tot = (balance.beg), totRep = (balance.begRep) };
                    var balanceDebit = new Values() { title = PLUGIN.LANG("T_DEBIT"), indx = 9904, tot = balance.debit, totRep = balance.debitRep };
                    var balanceCredit = new Values() { title = PLUGIN.LANG("T_CREDIT"), indx = 9905, tot = balance.credit, totRep = balance.creditRep };
                    var balanceEnd = new Values() { title = PLUGIN.LANG("T_BALANCEEND"), indx = 9906, tot = (balance.end), totRep = (balance.endRep) };


                    for (int t = 0; t < arrTrcodes.Length; ++t)
                    {
                        short sign = (short)(arrTrcodes[t] > 0 ? +1 : -1);
                        short indx = ABS(arrTrcodes[t]);
                        short _trcode = (short)(indx % 100);
                        short _modnr = (short)(indx / 100);

                        string title = "";

                        Values newValues = null;

                        if (_modnr < 99)
                        {
                            switch (indx)
                            {
                                case 1001:
                                    title = "T_DOC_FIN_CASH_11";
                                    break;
                                case 1002:
                                    title = "T_DOC_FIN_CASH_12";
                                    break;
                                case 720:
                                    title = "T_DOC_FIN_BANK_3";
                                    break;
                                case 721:
                                    title = "T_DOC_FIN_BANK_4";
                                    break;
                                default:
                                    title = PLUGIN.RESOLVESTR("[list::LIST_FIN_PERSONAL_TRANS_TYPE/" + _trcode + "]");
                                    break;
                            }



                            title = PLUGIN.LANG(title);

                            newValues = new Values() { title = title, indx = indx };

                            DataRow rec = null;

                            if (_modnr == 4)
                                rec = MY_TOTS_INVOICE(PLUGIN, filter.dateFrom, filter.dateTo, (short)(_trcode - 30), filter.client);
                            else
                                rec = MY_TOTS_CLFLINE(PLUGIN, filter.dateFrom, filter.dateTo, indx, filter.client);

                            newValues.read(rec);

                            if (_modnr == 4)
                            {
                                //mod 4 
                                if (_trcode == 38 || _trcode == 37 || _trcode == 32 || _trcode == 33)
                                    saleValues.add(newValues);
                                //mod 4
                                if (_trcode == 31 || _trcode == 36)
                                    prchValues.add(newValues);
                            }

                            newValues.write(table);



                            continue;
                        }


                        switch (indx)
                        {

                            case 9901:
                                prchValues.write(table);
                                break;
                            case 9902:
                                saleValues.write(table);
                                break;
                            case 9903:
                                balanceBeg.write(table);
                                break;
                            case 9904:
                                balanceDebit.write(table);
                                break;
                            case 9905:
                                balanceCredit.write(table);
                                break;
                            case 9906:
                                balanceEnd.write(table);
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

            public static DataRow GET_MAT_TOTS(_PLUGIN pPLUGIN, DateTime df, DateTime dt, short trcode, int client)
            {


                //   TAB_GETLASTROW(PLUGIN.SQL("SELECT 1.0 QTY,2.0 TOT,3.0 TOTVAT,4.0 TOTREP,5.0 TOTVATREP", new object[]{}));


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



                if (client > 0)//!
                { listArgs.Add(client); listWhere.Add("CLIENTREF = @P" + listArgs.Count); }


                if (listWhere.Count > 0)
                {

                    sqlText += (" AND " + string.Join(" AND ", listWhere.ToArray()));

                }


                return TAB_GETLASTROW(pPLUGIN.SQL(sqlText, listArgs.ToArray()));




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


            static DataRow MY_TOTS_INVOICE(_PLUGIN pPLUGIN, DateTime df, DateTime dt, short trcode, int client)
            {


                //   TAB_GETLASTROW(PLUGIN.SQL("SELECT 1.0 QTY,2.0 TOT,3.0 TOTVAT,4.0 TOTREP,5.0 TOTVATREP", new object[]{}));


                var listWhere = new List<string>();
                var listArgs = new List<object>();


                var sqlText =

    @"

SELECT  

		sum(NETTOTAL - TOTALVAT) TOTNOVAT,
		sum(NETTOTAL ) TOT,
        sum(case when REPORTRATE > 0 then (NETTOTAL )/REPORTRATE else 0 end) TOTREP,
		sum(case when REPORTRATE > 0 then (NETTOTAL - TOTALVAT )/REPORTRATE else 0 end) TOTNOVATREP
	 
		FROM LG_$FIRM$_$PERIOD$_INVOICE WITH(NOLOCK)
		WHERE 
        ( DATE_ BETWEEN @P1 AND @P2 AND TRCODE = @P3 AND CANCELLED = 0 ) 
   
			";

                listArgs.Add(df);
                listArgs.Add(dt);
                listArgs.Add(trcode);


                if (client > 0)//!
                { listArgs.Add(client); listWhere.Add("CLIENTREF = @P" + listArgs.Count); }

                if (listWhere.Count > 0)
                {

                    sqlText += (" AND " + string.Join(" AND ", listWhere.ToArray()));

                }


                return TAB_GETLASTROW(pPLUGIN.SQL(sqlText, listArgs.ToArray()));




            }



            static DataRow MY_TOTS_CLFLINE(_PLUGIN pPLUGIN, DateTime df, DateTime dt, short trcode, int client)
            {

                var listWhere = new List<string>();
                var listArgs = new List<object>();

                short _modnr = (short)(trcode / (short)100);
                short _trcode = (short)(trcode % (short)100);

                var sqlText =
    @"

SELECT  
		sum(AMOUNT) TOT,
		sum(REPORTNET ) TOTREP ,
	    0.0 TOTNOVAT,
		0.0 TOTNOVATREP 
		FROM LG_$FIRM$_$PERIOD$_CLFLINE WITH(NOLOCK)
		WHERE 
        ( DATE_ BETWEEN @P1 AND @P2 AND MODULENR = @P3 AND TRCODE = @P4 AND CANCELLED = 0 ) 
			";

                listArgs.Add(df);
                listArgs.Add(dt);
                listArgs.Add(_modnr);
                listArgs.Add(_trcode);

                if (client > 0)//!
                { listArgs.Add(client); listWhere.Add("CLIENTREF = @P" + listArgs.Count); }

                if (listWhere.Count > 0)
                {

                    sqlText += (" AND " + string.Join(" AND ", listWhere.ToArray()));

                }


                return TAB_GETLASTROW(pPLUGIN.SQL(sqlText, listArgs.ToArray()));



            }


            static DataRow MY_BALANCE(_PLUGIN pPLUGIN, DateTime df, DateTime dt, int client)
            {

                var listWhere = new List<string>();
                var listArgs = new List<object>();


                var sqlText =
    @"

SELECT  
		sum((case when DATE_ < @P1 then 1.0 else 0.0 end)*(case when SIGN = 0 then +1.0 else -1.0 end)*AMOUNT) as BEG,
        sum((case when DATE_ between @P1 and @P2 then 1.0 else 0.0 end)*(case when SIGN = 0 then +1.0 else 0.0 end)*AMOUNT) as DEBIT,
        sum((case when DATE_ between @P1 and @P2 then 1.0 else 0.0 end)*(case when SIGN = 1 then +1.0 else 0.0 end)*AMOUNT) as CREDIT,
        sum((case when DATE_ <= @P2 then 1.0 else 0.0 end)*(case when SIGN = 0 then +1.0 else -1.0 end)*AMOUNT) as END,

		sum((case when DATE_ < @P1 then 1.0 else 0.0 end)*(case when SIGN = 0 then +1.0 else -1.0 end)*REPORTNET) as BEGREP,
        sum((case when DATE_ between @P1 and @P2 then 1.0 else 0.0 end)*(case when SIGN = 0 then +1.0 else 0.0 end)*REPORTNET) as DEBITREP,
        sum((case when DATE_ between @P1 and @P2 then 1.0 else 0.0 end)*(case when SIGN = 1 then +1.0 else 0.0 end)*REPORTNET) as CREDITREP,
        sum((case when DATE_ <= @P2 then 1.0 else 0.0 end)*(case when SIGN = 0 then +1.0 else -1.0 end)*REPORTNET) as ENDREP 
  
 
		FROM LG_$FIRM$_$PERIOD$_CLFLINE WITH(NOLOCK)
		WHERE 
        ( DATE_ <= @P2 AND CANCELLED = 0 AND CLIENTREF = @P3) 
			";

                listArgs.Add(df);
                listArgs.Add(dt);
                listArgs.Add(client);


                return TAB_GETLASTROW(pPLUGIN.SQL(sqlText, listArgs.ToArray()));



            }

            public class Filter
            {
                public DateTime dateFrom = DateTime.Now.Date;
                public DateTime dateTo = DateTime.Now.Date;


                public int client = 0;

            }


            public class Values
            {
                public short indx = 0;
                public string title = "";




                public double tot = 0.0;
                public double totRep = 0.0;

                public double totNoVat = 0.0;
                public double totNoVatRep = 0.0;



                public void read(DataRow rec)
                {

                    tot = CASTASDOUBLE(TAB_GETROW(rec, "TOT"));
                    totRep = CASTASDOUBLE(TAB_GETROW(rec, "TOTREP"));

                    totNoVat = CASTASDOUBLE(TAB_GETROW(rec, "TOTNOVAT"));
                    totNoVatRep = CASTASDOUBLE(TAB_GETROW(rec, "TOTNOVATREP"));


                }



                public void write(DataRow rec)
                {

                    TAB_SETROW(rec, "INDX", indx);
                    TAB_SETROW(rec, "TITLE", title);


                    TAB_SETROW(rec, "TOT", tot);
                    TAB_SETROW(rec, "TOTREP", totRep);

                    TAB_SETROW(rec, "TOTNOVAT", totNoVat);
                    TAB_SETROW(rec, "TOTNOVATREP", totNoVatRep);

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

                    tot += (pSign * pSrc.tot);
                    totNoVat += (pSign * pSrc.totNoVat);
                    totRep += (pSign * pSrc.totRep);
                    totNoVatRep += (pSign * pSrc.totNoVatRep);
                }
            }

            public class ValuesFinBalance
            {


                public double beg = 0.0;
                public double debit = 0.0;
                public double credit = 0.0;
                public double end = 0.0;

                public double begRep = 0.0;
                public double debitRep = 0.0;
                public double creditRep = 0.0;
                public double endRep = 0.0;

                public void read(DataRow rec)
                {

                    beg = CASTASDOUBLE(TAB_GETROW(rec, "BEG"));
                    debit = CASTASDOUBLE(TAB_GETROW(rec, "DEBIT"));
                    credit = CASTASDOUBLE(TAB_GETROW(rec, "CREDIT"));
                    end = CASTASDOUBLE(TAB_GETROW(rec, "END"));
                    //
                    begRep = CASTASDOUBLE(TAB_GETROW(rec, "BEGREP"));
                    debitRep = CASTASDOUBLE(TAB_GETROW(rec, "DEBITREP"));
                    creditRep = CASTASDOUBLE(TAB_GETROW(rec, "CREDITREP"));
                    endRep = CASTASDOUBLE(TAB_GETROW(rec, "ENDREP"));

                }


            }


        }


        class FormLastTran : Form
        {
            _PLUGIN PLUGIN;
            Args initInfo;
            Filter filter = new Filter();

            public FormLastTran(_PLUGIN pPLUGIN, Filter pFilter)
            {
                PLUGIN = pPLUGIN;
                filter = pFilter;

                this.Text = string.Format("{0}", _LANG.L.TOTS_LAST);

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

                var btnFile = new Button() { Text = PLUGIN.LANG("T_FILE"), Image = RES_IMAGE("file_16x16"), ImageAlign = ContentAlignment.MiddleLeft, Width = 160, Dock = DockStyle.Left };
                btnFile.Click += (s, arg) => { this.toFile(); };


                //var btnSetPrice = new Button() { Width = 80, Text = PLUGIN.LANG("T_SAVE"), Image = RES_IMAGE("checked_16x16"), ImageAlign = ContentAlignment.MiddleLeft };
                //  btnSetPrice.Click += (s, arg) => { TAB_SETROW(pRow, "PRICE", (double)priceBox.Value); this.Close(); };


                var panelData = new Panel() { Dock = DockStyle.Fill };
                var panelFilterDateRange = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Bottom, AutoSize = true };

                var panelFilterCl = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Bottom, AutoSize = true };
                var panelFilterInfoBalance = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Bottom, AutoSize = true };
                var panelBtn = new Panel() { Dock = DockStyle.Bottom, Height = 25 };


                //
                mainPanel.Controls.Add(panelData);


                mainPanel.Controls.Add(panelFilterDateRange);
                mainPanel.Controls.Add(panelFilterCl);
                mainPanel.Controls.Add(panelFilterInfoBalance);





                mainPanel.Controls.Add(new Panel() { Tag = "Dummmy Padding", Height = 10, Dock = DockStyle.Bottom });
                //

                mainPanel.Controls.Add(panelBtn);

                panelBtn.Controls.Add(btnFile);
                panelBtn.Controls.Add(btnRefresh);

                panelBtn.Controls.Add(btnClose);
                //

                panelFilterDateRange.Controls.AddRange(
new Control[]{
                    new Label(){Text = PLUGIN.LANG("T_DATE_RANGE"), Width = 100, TextAlign = ContentAlignment.MiddleLeft,Dock = DockStyle.Left},
                    new DateTimePicker() { Value = filter.dateFrom, Name = "filter_date_from",Width=160 },
                    new DateTimePicker() { Value = filter.dateTo, Name = "filter_date_to",Width=160 },

                    new Button(){Text=PLUGIN.LANG("T_TODAY"),Image = RES_IMAGE("run_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_filter_date_today",Width=100 }, 
                    new Button(){Text=PLUGIN.LANG("-1 T_MONTH"),Image = RES_IMAGE("run_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_filter_date_month_dec",Width=100 },
                    new Button(){Text=PLUGIN.LANG("+1 T_MONTH"),Image = RES_IMAGE("run_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_filter_date_month_inc",Width=100 },
                     new Button(){Text=PLUGIN.LANG("T_YEAR"),Image = RES_IMAGE("run_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_filter_date_year",Width=100 },
                    }
);

                panelFilterCl.Controls.AddRange(
                 new Control[]{
                    new Label(){Text = PLUGIN.LANG("T_PERSONAL"), Width = 100, TextAlign = ContentAlignment.MiddleLeft,Dock = DockStyle.Left},
                    new NumericUpDown() { Minimum = 0, Maximum = int.MaxValue, DecimalPlaces = 0, Value = filter.client, Name = "filter_client",Width=80},
                   new Button(){Text="",Image = RES_IMAGE("find_16x16"),ImageAlign = ContentAlignment.MiddleCenter,Name = "do_filter_client",Width=30 },
                   new TextBox(){Width=700,ReadOnly=true,Name="desc_filter_client"}
                    }
                 );

                panelFilterInfoBalance.Controls.AddRange(
               new Control[]{
                        new Label(){Text = PLUGIN.LANG("T_BALANCE"), Width = 100, TextAlign = ContentAlignment.MiddleLeft,Dock = DockStyle.Left},
                         new TextBox(){Width=160,ReadOnly=true,Name="desc_info_balance_1",TextAlign = HorizontalAlignment.Right},
                          new TextBox(){Width=160,ReadOnly=true,Name="desc_info_balance_2",TextAlign = HorizontalAlignment.Right}
                        }
           );


                //        var grid = new DataGridView();


                var grid = createGrid("grid_oper");
                var gridAgr = createGrid("grid_agr");

                grid.Columns.AddRange(
                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_TITLE"), DataPropertyName = "TITLE", Width = 200, Frozen = true },
                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_DATE"), DataPropertyName = "DATE_", Width = 100, Frozen = true },

                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_TOTAL\nT_SYS_CURR1"), DataPropertyName = "TOT", Width = 120 },
                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_BALANCE\nT_SYS_CURR1"), DataPropertyName = "BALANCE", Width = 120 },
                                    new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_VATEXC (-)\n(T_SYS_CURR1)"), DataPropertyName = "TOTNOVAT", Width = 120 },


                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_TOTAL\nT_SYS_CURR2"), DataPropertyName = "TOTREP", Width = 120 },
                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_BALANCE\nT_SYS_CURR2"), DataPropertyName = "BALANCEREP", Width = 120 },
                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_VATEXC (-)\n(T_SYS_CURR2)"), DataPropertyName = "TOTNOVATREP", Width = 120 },

                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_DOCNO"), DataPropertyName = "DOCNR", Width = 120 },
                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_CARD"), DataPropertyName = "COMPCARDTITLE", Width = 120 },
                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_INFO\nT_ROW"), DataPropertyName = "LINEEXP", Width = 160 },
                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_SPECODE\nT_ROW"), DataPropertyName = "LSPECODE", Width = 160 },

                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_INFO 1\nT_DOC"), DataPropertyName = "GENEXP1", Width = 160 },
                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_INFO 2\nT_DOC"), DataPropertyName = "GENEXP2", Width = 160 },
                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_INFO 3\nT_DOC"), DataPropertyName = "GENEXP3", Width = 160 },
                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_INFO 4\nT_DOC"), DataPropertyName = "GENEXP4", Width = 160 },

                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_SPECODE 1\nT_DOC"), DataPropertyName = "SPECODE", Width = 160 },
                                    new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_SPECODE 2\nT_DOC"), DataPropertyName = "SPECODE2", Width = 160 },
                                    new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_SPECODE 3\nT_DOC"), DataPropertyName = "SPECODE3", Width = 160 }

                                   );


                gridAgr.Columns.AddRange(

                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_TITLE"), DataPropertyName = "TITLE", Width = 200, Frozen = true },
                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG(_LANG.L.COUNT_DOC), DataPropertyName = "COUNT", Width = 120 },

                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_TOTAL\nT_SYS_CURR1"), DataPropertyName = "TOT", Width = 120 },
                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_VATEXC (-)\n(T_SYS_CURR1)"), DataPropertyName = "TOTNOVAT", Width = 120 },


                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_TOTAL\nT_SYS_CURR2"), DataPropertyName = "TOTREP", Width = 120 },
                                  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_VATEXC (-)\n(T_SYS_CURR2)"), DataPropertyName = "TOTNOVATREP", Width = 120 }

                                  );

                foreach (var x in new DataGridView[] { grid, gridAgr })
                    foreach (DataGridViewColumn colObj in x.Columns)
                    {
                        colObj.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        colObj.SortMode = DataGridViewColumnSortMode.Programmatic;

                        switch (colObj.DataPropertyName)
                        {
                            case "TOT":
                            case "TOTREP":
                            case "TOTNOVAT":
                            case "TOTNOVATREP":
                            case "BALANCE":
                            case "BALANCEREP":
                                colObj.DefaultCellStyle.Format = "#,0.00;-#,0.00;''";
                                colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


                                if (colObj.DataPropertyName.EndsWith("REP"))
                                    colObj.DefaultCellStyle.ForeColor = Color.DarkGray;
                                //else
                                //    colObj.DefaultCellStyle.ForeColor = Color.Blue;

                                break;
                            case "COUNT":
                                colObj.DefaultCellStyle.Format = "#,0.##;-#,0.##;''";
                                colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                                break;
                            case "TITLE":
                                //  colObj.DefaultCellStyle.Format = "d";
                                colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                                break;
                            case "DATE_":
                                colObj.DefaultCellStyle.Format = "d";
                                colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                                break;
                            case "DOCNR":
                                colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                                break;
                        }
                    }


                {
                    grid.CellClick += (sender, e) =>
                    {

                        try
                        {
                            if (e.RowIndex >= 0)
                                return;


                            DataGridView grid_ = sender as DataGridView;
                            if (grid_ == null)
                                return;

                            var colObj_ = e.ColumnIndex >= 0 ? grid_.Columns[e.ColumnIndex] : null;
                            if (colObj_ == null)
                                return;


                            if (colObj_.SortMode != DataGridViewColumnSortMode.Programmatic)
                                return;

                            var tab = grid_.DataSource as DataTable;

                            if (tab == null)
                                return;

                            var sortStr = "";

                            //switch (colObj_.DataPropertyName)
                            //{
                            //    case "TITLE":
                            //        sortStr = "TITLE ASC,DATE_ ASC,LOGICALREF ASC";
                            //        break;

                            //    case "DATE_":
                            //        sortStr = "DATE_ ASC,SIGN ASC,LOGICALREF ASC";
                            //        break;

                            //    default:
                            //        return;


                            //}

                            if (!ISEMPTY(sortStr))
                                if (tab.DefaultView.Sort != sortStr)
                                    tab.DefaultView.Sort = sortStr;

                        }
                        catch (Exception exc)
                        {
                            PLUGIN.MSGUSERERROR(exc.Message);
                            PLUGIN.LOG(exc);
                        }


                    };


                    ///////////
                    grid.CellMouseDoubleClick += (sender, e) =>
                    {
                        try
                        {
                            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                                return;


                            DataGridView grid_ = sender as DataGridView;
                            if (grid_ == null)
                                return;

                            DataRow ROW = TOOL_GRID.GET_GRID_ROW_DATA(grid);
                            if (ROW == null)
                                return;

                            try
                            {
                                var cmd = "event name::replink/adp/edit/CLFLINE/" + TAB_GETROW(ROW, "LOGICALREF");
                                PLUGIN.EXECMDTEXT(cmd);
                            }
                            catch (Exception exc)
                            {
                                PLUGIN.LOG(exc);
                            }

                        }
                        catch (Exception exc)
                        {
                            PLUGIN.MSGUSERERROR(exc.Message);
                            PLUGIN.LOG(exc);
                        }

                    };


                    grid.CellFormatting += (sender, e) =>
                    {

                        DataGridView grid_ = sender as DataGridView;
                        if (grid_ == null)
                            return;

                        var rowObj_ = TOOL_GRID.GET_GRID_ROW(grid_, e.RowIndex);
                        if (rowObj_ == null)
                            return;

                        var colObj_ = e.ColumnIndex >= 0 ? grid_.Columns[e.ColumnIndex] : null;
                        if (colObj_ == null)
                            return;


                        var rowData_ = TOOL_GRID.GET_GRID_ROW_DATA(rowObj_);
                        if (rowData_ == null)
                            return;

                        // var indx = CASTASSHORT(TAB_GETROW(rowData_, "INDX"));


                        {
                            if (colObj_.DataPropertyName.StartsWith("BALANCE") || colObj_.DataPropertyName.StartsWith("TOT"))
                            {
                                //balance
                                var val = CASTASDOUBLE(e.Value);

                                if (!ISNUMZERO(val))
                                {
                                    var valStr = FORMAT(ABS(val), e.CellStyle.Format) + " " + (val > 0 ? _LANG.L.SUFIX_DEBIT : _LANG.L.SUFIX_CREDIT);
                                    e.Value = valStr;
                                }
                            }
                        }
                    };


                    gridAgr.CellFormatting += (sender, e) =>
                    {

                        DataGridView grid_ = sender as DataGridView;
                        if (grid_ == null)
                            return;

                        var rowObj_ = TOOL_GRID.GET_GRID_ROW(grid_, e.RowIndex);
                        if (rowObj_ == null)
                            return;

                        var colObj_ = e.ColumnIndex >= 0 ? grid_.Columns[e.ColumnIndex] : null;
                        if (colObj_ == null)
                            return;


                        var rowData_ = TOOL_GRID.GET_GRID_ROW_DATA(rowObj_);
                        if (rowData_ == null)
                            return;

                        //var indx = CASTASSHORT(TAB_GETROW(rowData_, "INDX"));


                        {
                            if (colObj_.DataPropertyName.StartsWith("BALANCE") || colObj_.DataPropertyName.StartsWith("TOT"))
                            {
                                //balance
                                var val = CASTASDOUBLE(e.Value);

                                if (!ISNUMZERO(val))
                                {
                                    var valStr = FORMAT(ABS(val), e.CellStyle.Format) + " " + (val > 0 ? _LANG.L.SUFIX_DEBIT : _LANG.L.SUFIX_CREDIT);
                                    e.Value = valStr;
                                }
                            }
                        }
                    };


                    grid.CellPainting += (sender, e) =>
                    {
                        // if (stripeRowBackColor)
                        {

                            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                                return;

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

                            var isBalance = (colObj_.DataPropertyName.StartsWith("BALANCE"));
                            var isBalanceDebit = (isBalance && CASTASDOUBLE(e.Value) > 0.01);
                            var isSum = CASTASSHORT(TAB_GETROW(rowData_, "INDX")) > 9900;
                            var isBold = (isSum || colObj_.DataPropertyName == "TITLE");

                            Color foreColor = colObj_.DefaultCellStyle.ForeColor;

                            if (isBalanceDebit)
                                isBold = true;

                            var font = e.CellStyle.Font;
                            if (font == null)
                                font = colObj_.InheritedStyle.Font;

                            if (isBold != font.Bold)
                            {

                                e.CellStyle.Font = new Font(font, FontStyle.Bold | font.Style);

                            }

                            //if (isBalanceDebit)
                            //   e.CellStyle.ForeColor = e.CellStyle.SelectionForeColor = Color.Red;

                            if (isSum)
                                bgColor = Color.WhiteSmoke;

                            if (bgColor != e.CellStyle.BackColor)
                                e.CellStyle.BackColor = bgColor;

                        }
                    };

                    /////////
                }

                panelData.Controls.Add(grid);

                gridAgr.Dock = DockStyle.Bottom;
                gridAgr.Height = 150;

                panelData.Controls.Add(gridAgr);


            }



            DataGridView createGrid(string pName)
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
                        case "do_filter_client":
                        //
                        case "do_filter_date_today":
                        case "do_filter_date_month":
                        case "do_filter_date_year":
                        case "do_filter_date_month_dec":
                        case "do_filter_date_month_inc":
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
                                case "do_filter_date_month_inc":
                                    {
                                        var tmp = ctrlDateTo.Value.Date;
                                        tmp = new DateTime(tmp.Year, tmp.Month, 1).AddMonths(+1);
                                        ctrlDateFrom.Value = tmp;
                                        ctrlDateTo.Value = tmp.AddMonths(+1).AddDays(-1);
                                    }
                                    break;
                                case "do_filter_date_month_dec":
                                    {
                                        var tmp = ctrlDateTo.Value.Date;
                                        tmp = new DateTime(tmp.Year, tmp.Month, 1).AddMonths(-1);
                                        ctrlDateFrom.Value = tmp;
                                        ctrlDateTo.Value = tmp.AddMonths(+1).AddDays(-1);
                                    }
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

                    if (asNum != null)
                    {

                        switch (ctrl.Name)
                        {

                            case "filter_client":
                                filter.client = (int)asNum.Value;
                                break;

                        }

                    }

                    var asDate = ctrl as DateTimePicker;

                    if (asDate != null)
                    {
                        switch (ctrl.Name)
                        {

                            case "filter_date_from":
                                filter.dateFrom = asDate.Value.Date;
                                break;
                            case "filter_date_to":
                                filter.dateTo = asDate.Value.Date;
                                break;
                        }

                    }






                }



            }

            void refreshData()
            {
                try
                {

                    readFilter();



                    var clDesc = GET_TITLE_CLIENT(filter.client);


                    var ctrlDescCl = CONTROL_SEARCH(this, "desc_filter_client") as TextBox;
                    var ctrlDescInfoBalance1 = CONTROL_SEARCH(this, "desc_info_balance_1") as TextBox;
                    var ctrlDescInfoBalance2 = CONTROL_SEARCH(this, "desc_info_balance_2") as TextBox;

                    if (ctrlDescCl != null)
                        ctrlDescCl.Text = clDesc;






                    var gridCode = "grid_oper";
                    var gridAgrCode = "grid_agr";

                    var table = new DataTable(gridCode);



                    TAB_ADDCOL(table, "LOGICALREF", typeof(int));
                    TAB_ADDCOL(table, "INDX", typeof(int));
                    TAB_ADDCOL(table, "TITLE", typeof(string));
                    TAB_ADDCOL(table, "DATE_", typeof(DateTime));
                    TAB_ADDCOL(table, "SIGN", typeof(short));
                    //

                    TAB_ADDCOL(table, "TOT", typeof(double));
                    TAB_ADDCOL(table, "TOTREP", typeof(double));
                    TAB_ADDCOL(table, "TOTNOVAT", typeof(double));
                    TAB_ADDCOL(table, "TOTNOVATREP", typeof(double));
                    TAB_ADDCOL(table, "BALANCE", typeof(double));
                    TAB_ADDCOL(table, "BALANCEREP", typeof(double));
                    //
                    TAB_ADDCOL(table, "DOCNR", typeof(string));

                    TAB_ADDCOL(table, "GENEXP1", typeof(string));
                    TAB_ADDCOL(table, "GENEXP2", typeof(string));
                    TAB_ADDCOL(table, "GENEXP3", typeof(string));
                    TAB_ADDCOL(table, "GENEXP4", typeof(string));
                    TAB_ADDCOL(table, "LINEEXP", typeof(string));
                    TAB_ADDCOL(table, "SPECODE", typeof(string));
                    TAB_ADDCOL(table, "SPECODE2", typeof(string));
                    TAB_ADDCOL(table, "SPECODE3", typeof(string));
                    TAB_ADDCOL(table, "LSPECODE", typeof(string));
                     
                    TAB_ADDCOL(table, "COMPCARDTITLE", typeof(string));


                    var balance = (GET_BALANCE(PLUGIN, filter.client, filter.dateTo));



                    if (ctrlDescInfoBalance1 != null)
                    {
                        var val = balance.balance;
                        var box = ctrlDescInfoBalance1;
                        box.Text = ISNUMZERO(val) ? "" : string.Format("{0} {1} {2}", FORMAT(ABS(val), "#,0.##"), (val > 0 ? _LANG.L.SUFIX_DEBIT : _LANG.L.SUFIX_CREDIT), PLUGIN.LANG("T_SYS_CURR1"));

                    }

                    if (ctrlDescInfoBalance2 != null)
                    {
                        var val = balance.balanceRep;
                        var box = ctrlDescInfoBalance2;
                        box.Text = ISNUMZERO(val) ? "" : string.Format("{0} {1} {2}", FORMAT(ABS(val), "#,0.##"), (val > 0 ? _LANG.L.SUFIX_DEBIT : _LANG.L.SUFIX_CREDIT), PLUGIN.LANG("T_SYS_CURR2"));

                    }
                    var data = GET_CLFLINES(PLUGIN, filter.client, filter.dateFrom, filter.dateTo);



                    var balanceCurr = new ValuesFinBalance();

                    balanceCurr.balance = balance.balance;
                    balanceCurr.balanceRep = balance.balanceRep;

                    for (var r = data.Rows.Count - 1; r >= 0; --r)
                    {

                        var recObj = new ValuesClTran(data.Rows[r]);
                        var sign = (recObj.SIGN == 0 ? +1 : -1);

                        var newRec = new Values();

                        newRec.indx = 0;

                        newRec.title = "";


                        switch (recObj.MODULENR * 100 + recObj.TRCODE)
                        {
                            case 1001:
                                newRec.title = PLUGIN.LANG("T_DOC_FIN_CASH_11");
                                break;
                            case 1002:
                                newRec.title = PLUGIN.LANG("T_DOC_FIN_CASH_12");
                                break;
                            case 720:
                                newRec.title = PLUGIN.LANG("T_DOC_FIN_BANK_3");
                                break;
                            case 721:
                                newRec.title = PLUGIN.LANG("T_DOC_FIN_BANK_4");
                                break;
                            default:
                                newRec.title = PLUGIN.RESOLVESTR("[list::LIST_FIN_PERSONAL_TRANS_TYPE/" + recObj.TRCODE + "]");
                                break;
                        }

                        newRec.date = recObj.DATE_;
                        //sign change
                        newRec.tot = sign * recObj.AMOUNT;
                        newRec.totRep = sign * recObj.REPORTNET;

                        if (recObj.MODULENR == 4)
                        {
                            var inv = MY_INVOICE(PLUGIN, recObj.SOURCEFREF);
                            if (inv != null)
                            {
                                newRec.totNoVat = sign * CASTASDOUBLE(TAB_GETROW(inv, "TOTNOVAT"));
                                newRec.totNoVatRep = sign * CASTASDOUBLE(TAB_GETROW(inv, "TOTNOVATREP"));
                                //  newRec.docnr = CASTASSTRING(TAB_GETROW(inv, "DOCNR"));
                            }

                        }

                        newRec.DOCNR = recObj.DOCNR;// MY_DOCNRBYCLTRAN(PLUGIN, recObj.LOGICALREF);

                        newRec.COMPCARDTITLE = recObj.COMPCARDTITLE;
                        newRec.GENEXP1 = recObj.GENEXP1;
                        newRec.GENEXP2 = recObj.GENEXP2;
                        newRec.GENEXP3 = recObj.GENEXP3;
                        newRec.GENEXP4 = recObj.GENEXP4;
                        newRec.LINEEXP = recObj.LINEEXP;
                        newRec.SPECODE = recObj.SPECODE;
                        newRec.SPECODE2 = recObj.SPECODE2;
                        newRec.SPECODE3 = recObj.SPECODE3;
                        newRec.LSPECODE = recObj.LSPECODE;

                        newRec.balance = balanceCurr.balance;
                        newRec.balanceRep = balanceCurr.balanceRep;

                        var row = newRec.write(table, 0);//pos 0

                        //sign changed
                        balanceCurr.balance = balanceCurr.balance - newRec.tot;
                        balanceCurr.balanceRep = balanceCurr.balanceRep - newRec.totRep;

                        TAB_SETROW(row, "LOGICALREF", recObj.LOGICALREF);
                        //TAB_SETROW(row, "SIGN", recObj.SIGN);


                    }

                    //  table.DefaultView.Sort = "DATE_ ASC,SIGN ASC,LOGICALREF ASC";

                    var grid = CONTROL_SEARCH(this, gridCode) as DataGridView;
                    if (grid != null)
                        grid.DataSource = table;

                    TOOL_GRID.SET_GRID_POSITION(grid, int.MaxValue, null);

                    ///////////////////////////////////////////////////
                    //Agr
                    ///////////////////////////////////////////////////



                    var tableAgr = new DataTable(gridAgrCode);




                    TAB_ADDCOL(tableAgr, "TITLE", typeof(string));
                    TAB_ADDCOL(tableAgr, "COUNT", typeof(int));

                    TAB_ADDCOL(tableAgr, "TOT", typeof(double));
                    TAB_ADDCOL(tableAgr, "TOTNOVAT", typeof(double));

                    TAB_ADDCOL(tableAgr, "TOTREP", typeof(double));
                    TAB_ADDCOL(tableAgr, "TOTNOVATREP", typeof(double));

                    foreach (DataRow row in table.Rows)
                    {

                        var title = CASTASSTRING(TAB_GETROW(row, "TITLE"));


                        var trg = TAB_SEARCH(tableAgr, "TITLE", title);
                        if (trg == null)
                        {
                            trg = tableAgr.NewRow();
                            TAB_FILLNULL(trg);
                            tableAgr.Rows.Add(trg);
                            TAB_SETROW(trg, "TITLE", title);
                        }

                        TAB_SETROW(trg, "COUNT", SUM(TAB_GETROW(trg, "COUNT"), 1));
                        //
                        foreach (var c in new string[] { "TOT", "TOTREP", "TOTNOVAT", "TOTNOVATREP" })
                            TAB_SETROW(trg, c, SUM(TAB_GETROW(trg, c), TAB_GETROW(row, c)));


                    }

                    var gridAgr = CONTROL_SEARCH(this, gridAgrCode) as DataGridView;
                    if (gridAgr != null)
                        gridAgr.DataSource = tableAgr;

                    TOOL_GRID.SET_GRID_POSITION(gridAgr, int.MaxValue, null);

                }
                catch (Exception exc)
                {
                    PLUGIN.LOG(exc);
                    PLUGIN.MSGUSERERROR(exc.Message);
                }
            }



            void toFile()
            {
                try
                {

                    var html = toHtml();

                    //var path = PATHCOMBINE(
                    //    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    //    (_LANG.L.TOTS_LAST + ".xls").Replace(' ', '_').ToLowerInvariant());





                    var file = MY_DIR.SAVE(new StringBuilder(html), (_LANG.L.TOTS_LAST).Replace(' ', '_').ToLowerInvariant(), ".xls");

                    PROCESS(file, null);

                    // FILEWRITE(path, html);

                }
                catch (Exception exc)
                {
                    PLUGIN.LOG(exc);
                    PLUGIN.MSGUSERERROR(exc.Message);
                }
            }
            string toHtml()
            {


                DataTable dataRaw = null;
                {
                    var code = "grid_oper";
                    var grid = CONTROL_SEARCH(this, code) as DataGridView;
                    if (grid != null)
                        dataRaw = grid.DataSource as DataTable;


                }

                if (dataRaw == null)
                    throw new Exception("No data");


                DataTable dataAgr = null;
                {
                    var code = "grid_agr";
                    var grid = CONTROL_SEARCH(this, code) as DataGridView;
                    if (grid != null)
                        dataAgr = grid.DataSource as DataTable;


                }

                if (dataAgr == null)
                    throw new Exception("No agr data");


                var data = new DataTable();


                TAB_ADDCOL(data, "TITLE", typeof(string));
                TAB_ADDCOL(data, "DOCNR", typeof(string));
                TAB_ADDCOL(data, "DATE_", typeof(DateTime));

                TAB_ADDCOL(data, "DEBIT", typeof(double));
                TAB_ADDCOL(data, "CREDIT", typeof(double));

                TAB_ADDCOL(data, "DEBITREP", typeof(double));
                TAB_ADDCOL(data, "CREDITREP", typeof(double));

                TAB_ADDCOL(data, "BALANCEDEBIT", typeof(double));
                TAB_ADDCOL(data, "BALANCECREDIT", typeof(double));
                //


                TAB_ADDCOL(data, "COMPCARDTITLE", typeof(string));
                TAB_ADDCOL(data, "LINEEXP", typeof(string));
                TAB_ADDCOL(data, "LSPECODE", typeof(string));

                TAB_ADDCOL(data, "GENEXP1", typeof(string));
                TAB_ADDCOL(data, "GENEXP2", typeof(string));
                TAB_ADDCOL(data, "GENEXP3", typeof(string));
                TAB_ADDCOL(data, "GENEXP4", typeof(string));

                TAB_ADDCOL(data, "SPECODE", typeof(string));
                //TAB_ADDCOL(data, "SPECODE2", typeof(string));
                //  TAB_ADDCOL(data, "SPECODE3", typeof(string));


                var dataView = dataRaw.DefaultView;

                double valSumDebit = 0;
                double valSumCredit = 0;
                double valSumDebitRep = 0;
                double valSumCreditRep = 0;


                for (int i = 0; i < dataView.Count; ++i)
                {
                    var rec = dataView[i].Row;
                    var title = CASTASSTRING(TAB_GETROW(rec, "TITLE"));
                    var docnr = CASTASSTRING(TAB_GETROW(rec, "DOCNR"));
                    var date = CASTASDATE(TAB_GETROW(rec, "DATE_"));
                    var val = CASTASDOUBLE(TAB_GETROW(rec, "TOT"));
                    var valRep = CASTASDOUBLE(TAB_GETROW(rec, "TOTREP"));
                    var balanceRec = CASTASDOUBLE(TAB_GETROW(rec, "BALANCE"));
                    var balanceRecRep = CASTASDOUBLE(TAB_GETROW(rec, "BALANCEREP"));

                    valSumDebit += (val > 0 ? val : 0);
                    valSumCredit += (val < 0 ? ABS(val) : 0);
                    valSumDebitRep += (valRep > 0 ? valRep : 0);
                    valSumCreditRep += (valRep < 0 ? ABS(valRep) : 0);

                    var newRec = data.NewRow();
                    TAB_FILLNULL(newRec);
                    data.Rows.Add(newRec);

                    TAB_SETROW(newRec, "TITLE", title);
                    TAB_SETROW(newRec, "DOCNR", docnr);
                    TAB_SETROW(newRec, "DATE_", date);

                    TAB_SETROW(newRec, "DEBIT", val > 0 ? val : 0);
                    TAB_SETROW(newRec, "CREDIT", val < 0 ? ABS(val) : 0);
                    TAB_SETROW(newRec, "DEBITREP", valRep > 0 ? valRep : 0);
                    TAB_SETROW(newRec, "CREDITREP", valRep < 0 ? ABS(valRep) : 0);

                    TAB_SETROW(newRec, "BALANCEDEBIT", balanceRec > 0 ? balanceRec : 0);
                    TAB_SETROW(newRec, "BALANCECREDIT", balanceRec < 0 ? ABS(balanceRec) : 0);

                    TAB_SETROW(newRec, "COMPCARDTITLE", TAB_GETROW(rec, "COMPCARDTITLE"));
                    TAB_SETROW(newRec, "LINEEXP", TAB_GETROW(rec, "LINEEXP"));
                    TAB_SETROW(newRec, "LSPECODE", TAB_GETROW(rec, "LSPECODE"));
                    TAB_SETROW(newRec, "GENEXP1", TAB_GETROW(rec, "GENEXP1"));
                    TAB_SETROW(newRec, "GENEXP2", TAB_GETROW(rec, "GENEXP2"));
                    TAB_SETROW(newRec, "GENEXP3", TAB_GETROW(rec, "GENEXP3"));
                    TAB_SETROW(newRec, "GENEXP4", TAB_GETROW(rec, "GENEXP4"));
                    TAB_SETROW(newRec, "SPECODE", TAB_GETROW(rec, "SPECODE"));
                }


                data.Rows.Add(PLUGIN.LANG("T_TOTAL"), "", new DateTime(1900, 1, 1),
                      valSumDebit,
                      valSumCredit,
                      valSumDebitRep,
                      valSumCreditRep

                    //
                    //balance > 0 ? balance : 0, 
                    //balance < 0 ? ABS(balance) : 0,
                    ////
                    //balanceRep > 0 ? balanceRep : 0, 
                    //balanceRep < 0 ? ABS(balanceRep) : 0
                    ////

                       );

                var clDesc = GET_TITLE_CLIENT(filter.client);
                var balanceLast = (GET_BALANCE(PLUGIN, filter.client, filter.dateTo));


                var res = new StringBuilder();

                res.AppendLine("<html>");
                res.AppendLine("<head>");
                res.AppendLine("<meta charset='utf-8'>");
                res.AppendLine("<title>" + HTMLENCODE(_LANG.L.TOTS_LAST + " - " + clDesc) + "</title>");

                res.AppendLine(@" 

<style>
            table, th, td {
                border: 1px solid #dbdbdb;
                border-collapse: collapse;
                white-space: nowrap;
                padding: 3px;
 
            }
            table, h1, h2, h3, h4, h5, h6 {
                font-family: Segoe UI, Verdana, Aral;
            }
            th {
                font-weight:bold;
                text-align: center;
            }
      

        .num {
          mso-number-format:General;
        }
        .text{
          mso-number-format:""\@"";
        }


</style>
 
");

                res.AppendLine("</head>");
                res.AppendLine("<body>");
                res.AppendLine(string.Format("<h2 style='width:100%;text-align:center'>{0}</h2>",
                    HTMLENCODE(_LANG.L.TOTS_LAST)
                       ));

                res.AppendLine(string.Format("<h3 style='width:100%;text-align:center'>{0}</h2>",
                  HTMLENCODE(clDesc)
                    ));


                //filter
                {

                    var lines = new List<string[]>();

                    //lines.Add(new string[] { PLUGIN.LANG("T_DATE"), FORMAT(DateTime.Now.Date, "yyyy-MM-dd") });
                    lines.Add(new string[] { PLUGIN.LANG("T_DATE_FROM"), FORMAT(filter.dateFrom, "yyyy-MM-dd") });
                    lines.Add(new string[] { PLUGIN.LANG("T_DATE_TO"), FORMAT(filter.dateTo, "yyyy-MM-dd") });
                    //  lines.Add(new string[] { PLUGIN.LANG("T_PERSONAL"), HTMLENCODE(clDesc) });

                    {
                        var val = balanceLast.balance;
                        var str = (ISNUMZERO(val) ? "" : string.Format("{0} {1} {2}", FORMAT(ABS(val), "#,0.##"), (val > 0 ? _LANG.L.SUFIX_DEBIT : _LANG.L.SUFIX_CREDIT), PLUGIN.LANG("T_SYS_CURR1")));
                        lines.Add(new string[] { PLUGIN.LANG("T_BALANCE (T_SYS_CURR1)"), HTMLENCODE(str) });
                    }
                    {
                        var val = balanceLast.balanceRep;
                        var str = (ISNUMZERO(val) ? "" : string.Format("{0} {1} {2}", FORMAT(ABS(val), "#,0.##"), (val > 0 ? _LANG.L.SUFIX_DEBIT : _LANG.L.SUFIX_CREDIT), PLUGIN.LANG("T_SYS_CURR2")));
                        lines.Add(new string[] { PLUGIN.LANG("T_BALANCE (T_SYS_CURR2)"), HTMLENCODE(str) });
                    }

                    res.AppendLine("<table>");

                    foreach (var row in lines)
                    {

                        res.AppendLine(
                          string.Format("<tr class='text' ><td style='text-align:left;font-weight:bold;'>{0}</td><td style='text-align:right;'>{1}</td></tr>", row[0], row[1]));


                    }


                    res.AppendLine("</table>");
                }


                res.AppendLine("<br/>");

                //agr

                {

                    res.AppendLine("<table>");
                    {
                        res.AppendLine("<tr>");
                        foreach (DataColumn col in dataAgr.Columns)
                        {

                            string objStr = null;
                            var style = "";
                            style += "text-align:center;background-color:#B0B0B0;";
                            switch (col.ColumnName)
                            {
                                case "TITLE":
                                    objStr = PLUGIN.LANG("T_DESC");
                                    break;
                                case "COUNT":
                                    objStr = PLUGIN.LANG(_LANG.L.COUNT_DOC);
                                    break;
                                case "TOT":
                                    objStr = PLUGIN.LANG("T_TOTAL|T_SYS_CURR1");
                                    break;
                                case "TOTREP":
                                    objStr = PLUGIN.LANG("T_TOTAL|T_SYS_CURR2");
                                    break;
                                case "TOTNOVAT":
                                    objStr = PLUGIN.LANG("T_VATEXC (-)|(T_SYS_CURR1)");
                                    break;
                                case "TOTNOVATREP":
                                    objStr = PLUGIN.LANG("T_VATEXC (-)|(T_SYS_CURR2)");
                                    break;

                            }

                            if (objStr == null)
                                continue;

                            res.AppendLine(string.Format(
                            "<th class='text' style=" + style + ">{0}</th>",
                            HTMLENCODE(objStr).Replace("|", "<br/>")
                           ));
                        }
                        res.AppendLine("</tr>");

                    }

                    for (int i = 0; i < dataAgr.Rows.Count; ++i)
                    {
                        DataRow row = dataAgr.Rows[i];
                        // var isLast = (dataAgr.Rows.Count - 1 == i);

                        var rowStyle = "";
                        //  if (isLast)
                        //     rowStyle += "background-color:#B0B0B0;";

                        res.AppendLine("<tr style='" + rowStyle + "'>");
                        foreach (DataColumn col in dataAgr.Columns)
                        {
                            var obj = row[col];
                            string objStr = null;
                            var style = "";
                            var clazz = "";
                            switch (col.ColumnName)
                            {
                                case "TITLE":
                                    objStr = CASTASSTRING(obj);
                                    style += "text-align:left;";
                                    style += "font-weight:bold;";

                                    break;
                                case "COUNT":
                                    objStr = FORMAT(obj, "#,0.##;-#,0.##;''");
                                    style += "text-align:right;";
                                    clazz += "text";
                                    break;

                                case "TOT":
                                case "TOTREP":
                                case "TOTNOVAT":
                                case "TOTNOVATREP":
                                    {
                                        var val = CASTASDOUBLE(obj);
                                        objStr = ISNUMZERO(val) ? "" : string.Format("{0} {1}", FORMAT(ABS(val), "#,0.00"), (val > 0 ? _LANG.L.SUFIX_DEBIT : _LANG.L.SUFIX_CREDIT));


                                        style += "text-align:right;";
                                        clazz += "text";
                                    }
                                    break;



                            }
                            if (objStr == null)
                                continue;

                            res.AppendLine(string.Format(
                            "<td class='" + clazz + "' style='" + style + "'>{0}</td>",
                            HTMLENCODE(objStr)
                           ));
                        }
                        res.AppendLine("</tr>");

                    }


                    res.AppendLine("</table>");


                }


                res.AppendLine("<br/>");


                //oper
                {

                    res.AppendLine("<table>");
                    {
                        res.AppendLine("<tr>");
                        foreach (DataColumn col in data.Columns)
                        {

                            string objStr = null;
                            var style = "";
                            style += "text-align:center;background-color:#B0B0B0;";
                            switch (col.ColumnName)
                            {
                                case "TITLE":
                                    objStr = PLUGIN.LANG("T_DESC");
                                    break;
                                case "DOCNR":
                                    objStr = PLUGIN.LANG("T_DOCNO");
                                    break;
                                case "DATE_":
                                    objStr = PLUGIN.LANG("T_DATE");
                                    break;
                                case "DEBIT":
                                    objStr = PLUGIN.LANG("T_TRANS|T_SYS_CURR1|T_DEBIT");
                                    break;
                                case "CREDIT":
                                    objStr = PLUGIN.LANG("T_TRANS|T_SYS_CURR1|T_CREDIT");
                                    break;
                                case "DEBITREP":
                                    objStr = PLUGIN.LANG("T_TRANS|T_SYS_CURR2|T_DEBIT");
                                    break;
                                case "CREDITREP":
                                    objStr = PLUGIN.LANG("T_TRANS|T_SYS_CURR2|T_CREDIT");
                                    break;
                                case "BALANCEDEBIT":
                                    objStr = PLUGIN.LANG("T_BALANCE|T_SYS_CURR1|T_DEBIT");
                                    break;
                                case "BALANCECREDIT":
                                    objStr = PLUGIN.LANG("T_BALANCE|T_SYS_CURR1|T_CREDIT");
                                    break;
                                case "COMPCARDTITLE":
                                    objStr = PLUGIN.LANG("T_CARD");
                                    break;
                                case "LINEEXP":
                                    objStr = PLUGIN.LANG("T_INFO|T_ROW");
                                    break;
                                case "LSPECODE":
                                    objStr = PLUGIN.LANG("T_SPECODE|T_ROW");
                                    break;
                                case "GENEXP1":
                                    objStr = PLUGIN.LANG("T_INFO 1|T_DOC");
                                    break;
                                case "GENEXP2":
                                    objStr = PLUGIN.LANG("T_INFO 2|T_DOC");
                                    break;
                                case "GENEXP3":
                                    objStr = PLUGIN.LANG("T_INFO 3|T_DOC");
                                    break;
                                case "GENEXP4":
                                    objStr = PLUGIN.LANG("T_INFO 4|T_DOC");
                                    break;
                                case "SPECODE":
                                    objStr = PLUGIN.LANG("T_SPECODE|T_DOC");
                                    break;
                            }
                            if (objStr == null)
                                continue;

                            res.AppendLine(string.Format(
                            "<th class='text' style=" + style + ">{0}</th>",
                            HTMLENCODE(objStr).Replace("|", "<br/>")
                           ));
                        }
                        res.AppendLine("</tr>");

                    }

                    for (int i = 0; i < data.Rows.Count; ++i)
                    {
                        DataRow row = data.Rows[i];
                        var isLast = (data.Rows.Count - 1 == i);

                        var rowStyle = "";
                        if (isLast)
                            rowStyle += "background-color:#B0B0B0;";

                        res.AppendLine("<tr style='" + rowStyle + "'>");
                        foreach (DataColumn col in data.Columns)
                        {
                            var obj = row[col];
                            string objStr = null;
                            var style = "";
                            var clazz = "";
                            switch (col.ColumnName)
                            {
                                case "TITLE":
                                    objStr = CASTASSTRING(obj);
                                    style += "text-align:left;";
                                    style += "font-weight:bold;";

                                    break;
                                case "DOCNR":
                                    objStr = CASTASSTRING(obj);
                                    style += "text-align:right;";
                                    clazz += "text";

                                    break;
                                case "DATE_":
                                    {
                                        var date = CASTASDATE(obj);
                                        objStr = date.Year == 1900 ? "" : FORMAT(date, "yyyy-MM-dd");
                                        style += "text-align:left;";
                                        clazz += "text";
                                    }
                                    break;
                                case "DEBIT":
                                case "CREDIT":
                                    objStr = FORMAT(obj, "#,0.00;-#,0.00;''");
                                    style += "text-align:right;";
                                    clazz += "text";
                                    break;
                                case "DEBITREP":
                                case "CREDITREP":
                                    objStr = FORMAT(obj, "#,0.00;-#,0.00;''");
                                    style += "text-align:right;";
                                    clazz += "text";
                                    break;

                                case "BALANCEDEBIT":
                                case "BALANCECREDIT":
                                    objStr = FORMAT(obj, "#,0.00;-#,0.00;''");
                                    style += "text-align:right;";
                                    style += "font-weight:bold;";
                                    break;

                                case "COMPCARDTITLE":
                                case "LINEEXP":
                                case "LSPECODE":
                                case "GENEXP1":
                                case "GENEXP2":
                                case "GENEXP3":
                                case "GENEXP4":
                                case "SPECODE":
                                    objStr = CASTASSTRING(obj);
                                    style += "text-align:right;";

                                    break;
                            }
                            if (objStr == null)
                                continue;
                            res.AppendLine(string.Format(
                            "<td class='" + clazz + "' style='" + style + "'>{0}</td>",
                            HTMLENCODE(objStr)
                           ));
                        }
                        res.AppendLine("</tr>");

                    }


                    res.AppendLine("</table>");


                }


                res.AppendLine("</body></html>");


                return res.ToString();




            }

            public static DataRow GET_MAT_TOTS(_PLUGIN pPLUGIN, DateTime df, DateTime dt, short trcode, int client)
            {


                //   TAB_GETLASTROW(PLUGIN.SQL("SELECT 1.0 QTY,2.0 TOT,3.0 TOTVAT,4.0 TOTREP,5.0 TOTVATREP", new object[]{}));


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



                if (client > 0)//!
                { listArgs.Add(client); listWhere.Add("CLIENTREF = @P" + listArgs.Count); }


                if (listWhere.Count > 0)
                {

                    sqlText += (" AND " + string.Join(" AND ", listWhere.ToArray()));

                }


                return TAB_GETLASTROW(pPLUGIN.SQL(sqlText, listArgs.ToArray()));




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




            static DataRow MY_INVOICE(_PLUGIN pPLUGIN, object pInvRef)
            {

                var listWhere = new List<string>();
                var listArgs = new List<object>();


                var sqlText =

    @"

SELECT  
        FICHENO AS DOCNR,
		 (NETTOTAL - TOTALVAT) TOTNOVAT,
		 (NETTOTAL ) TOT,
         (case when REPORTRATE > 0 then (NETTOTAL )/REPORTRATE else 0 end) TOTREP,
		 (case when REPORTRATE > 0 then (NETTOTAL - TOTALVAT )/REPORTRATE else 0 end) TOTNOVATREP
	 
		FROM LG_$FIRM$_$PERIOD$_INVOICE WITH(NOLOCK)
		WHERE LOGICALREF = @P1 
";

                listArgs.Add(pInvRef);

                return TAB_GETLASTROW(pPLUGIN.SQL(sqlText, listArgs.ToArray()));




            }



            static DataTable GET_CLFLINES(_PLUGIN pPLUGIN, int client, DateTime pDateFrom, DateTime pDateTo)
            {

                var listWhere = new List<string>();
                var listArgs = new List<object>();


                var sqlText =
    @"

WITH DATA AS (
SELECT
--$MS$--TOP 10000
		LOGICALREF,AMOUNT,REPORTNET,DATE_,MODULENR,TRCODE,SOURCEFREF,SIGN
		FROM LG_$FIRM$_$PERIOD$_CLFLINE WITH(NOLOCK)
		WHERE 
        (CLIENTREF = @P3 AND (DATE_ BETWEEN @P1 AND @P2) AND CANCELLED = 0) 
        ORDER BY
	    CLIENTREF DESC,
	    DATE_ DESC,
	    MODULENR DESC,
	    TRCODE DESC,
	    LOGICALREF DESC
--$PG$--LIMIT 10000
--$SL$--LIMIT 10000

)
SELECT 
OPR.* ,
COALESCE(KSCARD.NAME,BANKACC.DEFINITION_,'') AS COMPCARDTITLE,
COALESCE(KSLINES.FICHENO,BNFICHE.FICHENO,INVOICE.FICHENO,CLFICHE.FICHENO,'') AS DOCNR,
COALESCE(KSLINES.LINEEXP,BNFICHE.GENEXP1,INVOICE.GENEXP1,CLFICHE.GENEXP1,'') AS GENEXP1,
COALESCE(KSLINES.CUSTTITLE,BNFICHE.GENEXP2,INVOICE.GENEXP2,CLFICHE.GENEXP2,'') AS GENEXP2,
COALESCE(NULL,BNFICHE.GENEXP3,INVOICE.GENEXP3,CLFICHE.GENEXP3,'') AS GENEXP3,
COALESCE(NULL,BNFICHE.GENEXP4,INVOICE.GENEXP4,CLFICHE.GENEXP4,'') AS GENEXP4,
COALESCE(NULL,BNFLINE.LINEEXP,NULL,CLFLINE.LINEEXP,'') AS LINEEXP,
COALESCE(NULL,BNFLINE.SPECODE,NULL,CLFLINE.SPECODE,'') AS LSPECODE,
COALESCE(KSLINES.SPECODE,BNFICHE.SPECODE,INVOICE.SPECODE,CLFICHE.SPECCODE,'') AS SPECODE,
COALESCE(KSLINES.SPECODE2,BNFICHE.SPECODE2,INVOICE.SPECODE2,CLFICHE.SPECODE2,'') AS SPECODE2,
COALESCE(KSLINES.SPECODE3,BNFICHE.SPECODE3,INVOICE.SPECODE3,CLFICHE.SPECODE3,'') AS SPECODE3


FROM  DATA AS OPR 
LEFT JOIN LG_$FIRM$_$PERIOD$_KSLINES AS KSLINES ON (OPR.MODULENR IN (10) AND OPR.TRCODE IN (1, 2)) AND KSLINES.LOGICALREF = OPR.SOURCEFREF
LEFT JOIN LG_$FIRM$_$PERIOD$_BNFLINE AS BNFLINE ON (OPR.MODULENR IN (7) AND OPR.TRCODE IN (20, 21)) AND BNFLINE.LOGICALREF = OPR.SOURCEFREF
LEFT JOIN LG_$FIRM$_$PERIOD$_BNFICHE AS BNFICHE ON (BNFLINE.SOURCEFREF = BNFICHE.LOGICALREF)
LEFT JOIN LG_$FIRM$_$PERIOD$_INVOICE AS INVOICE ON (OPR.MODULENR IN (4) AND OPR.TRCODE IN (31, 32, 33, 34, 36, 37, 38, 39)) AND INVOICE.LOGICALREF = OPR.SOURCEFREF
LEFT JOIN LG_$FIRM$_$PERIOD$_CLFLINE AS CLFLINE ON CLFLINE.LOGICALREF = OPR.LOGICALREF
LEFT JOIN LG_$FIRM$_$PERIOD$_CLFICHE AS CLFICHE ON (OPR.MODULENR IN (5) AND OPR.TRCODE IN (3, 4, 5, 6, 12, 14)) AND CLFICHE.LOGICALREF = OPR.SOURCEFREF
--------------------------
LEFT JOIN LG_$FIRM$_KSCARD AS KSCARD ON (KSLINES.CARDREF = KSCARD.LOGICALREF)
LEFT JOIN LG_$FIRM$_BANKACC AS BANKACC ON (BNFLINE.BNACCREF = BANKACC.LOGICALREF)
--------------------------
ORDER BY 
OPR.DATE_ ASC,OPR.SIGN ASC/*0 debit 1 credit*/,OPR.LOGICALREF ASC
";
                //1//0 sale
                //2//1 payment

                listArgs.Add(pDateFrom.Date);
                listArgs.Add(pDateTo.Date);
                listArgs.Add(client);


                return (pPLUGIN.SQL(sqlText, listArgs.ToArray()));



            }


            static ValuesFinBalance GET_BALANCE(_PLUGIN pPLUGIN, int client, DateTime pDate)
            {
                var balance = new ValuesFinBalance();
                if (ISEMPTYLREF(client))
                    return balance;


                //                var sqlText =
                //    @"
                // SELECT 
                //(SELECT (DEBIT-CREDIT) FROM LG_$FIRM$_$PERIOD$_GNTOTCL WHERE CARDREF = @P1 AND TOTTYP = 1) TOT,
                //(SELECT (DEBIT-CREDIT) FROM LG_$FIRM$_$PERIOD$_GNTOTCL WHERE CARDREF = @P1 AND TOTTYP = 2) TOTREP
                //			";



                var sqlText = @"
 
SELECT
SUM((case when SIGN = 0 then +1 else -1 end)*AMOUNT) TOT,
SUM((case when SIGN = 0 then +1 else -1 end)*REPORTNET) TOTREP

FROM 
LG_$FIRM$_$PERIOD$_CLFLINE 
WHERE CLIENTREF= @P2 AND DATE_ <= @P1 AND CANCELLED = 0


";



                var info = TAB_GETLASTROW(pPLUGIN.SQL(sqlText, new object[] { pDate.Date, client }));

                balance.read(info);

                return balance;
            }

            public class Filter
            {

                public int client = 0;
                public DateTime dateFrom;
                public DateTime dateTo;
            }


            public class Values
            {
                public short indx = 0;
                public string title = "";
                //public string docnr = "";
                public DateTime date;

                public double tot = 0.0;
                public double totRep = 0.0;

                public double totNoVat = 0.0;
                public double totNoVatRep = 0.0;

                public double balance = 0.0;
                public double balanceRep = 0.0;

                public short sign = 0;


                public string DOCNR;
                public string COMPCARDTITLE;
                public string GENEXP1;
                public string GENEXP2;
                public string GENEXP3;
                public string GENEXP4;
                public string LINEEXP;
                public string SPECODE;
                public string SPECODE2;
                public string SPECODE3;
                public string LSPECODE;



                public void write(DataRow rec)
                {

                    TAB_SETROW(rec, "INDX", indx);
                    TAB_SETROW(rec, "TITLE", title);
                    TAB_SETROW(rec, "DATE_", date);
                    TAB_SETROW(rec, "SIGN", sign);

                    TAB_SETROW(rec, "TOT", tot);
                    TAB_SETROW(rec, "TOTREP", totRep);

                    TAB_SETROW(rec, "TOTNOVAT", totNoVat);
                    TAB_SETROW(rec, "TOTNOVATREP", totNoVatRep);

                    TAB_SETROW(rec, "BALANCE", balance);
                    TAB_SETROW(rec, "BALANCEREP", balanceRep);

                    TAB_SETROW(rec, "DOCNR", DOCNR);
                    TAB_SETROW(rec, "COMPCARDTITLE", COMPCARDTITLE);
                    TAB_SETROW(rec, "GENEXP1", GENEXP1);
                    TAB_SETROW(rec, "GENEXP2", GENEXP2);
                    TAB_SETROW(rec, "GENEXP3", GENEXP3);
                    TAB_SETROW(rec, "GENEXP4", GENEXP4);
                    TAB_SETROW(rec, "LINEEXP", LINEEXP);
                    TAB_SETROW(rec, "SPECODE", SPECODE);
                    TAB_SETROW(rec, "SPECODE2", SPECODE2);
                    TAB_SETROW(rec, "SPECODE3", SPECODE3);
                    TAB_SETROW(rec, "LSPECODE", LSPECODE);

                }



                public DataRow write(DataTable tab, int pos = -1)
                {
                    var rec = tab.NewRow();

                    write(rec);

                    if (pos < 0)
                        tab.Rows.Add(rec);
                    else
                        tab.Rows.InsertAt(rec, pos);

                    return rec;

                }

            }

            public class ValuesFinBalance
            {

                public double balance = 0.0;
                public double balanceRep = 0.0;


                public void read(DataRow rec)
                {

                    balance = CASTASDOUBLE(TAB_GETROW(rec, "TOT"));
                    balanceRep = CASTASDOUBLE(TAB_GETROW(rec, "TOTREP"));


                }


            }

            public class ValuesClTran
            {

                public object LOGICALREF;
                public double AMOUNT;
                public double REPORTNET;
                public DateTime DATE_;
                public short MODULENR;
                public short TRCODE;
                public object SOURCEFREF;
                public short SIGN;

                public string DOCNR;
                public string COMPCARDTITLE;
                public string GENEXP1;
                public string GENEXP2;
                public string GENEXP3;
                public string GENEXP4;
                public string LINEEXP;
                public string SPECODE;
                public string SPECODE2;
                public string SPECODE3;
                public string LSPECODE;

                public ValuesClTran(DataRow rec)
                {
                    LOGICALREF = (TAB_GETROW(rec, "LOGICALREF"));
                    AMOUNT = CASTASDOUBLE(TAB_GETROW(rec, "AMOUNT"));
                    REPORTNET = CASTASDOUBLE(TAB_GETROW(rec, "REPORTNET"));
                    DATE_ = CASTASDATE(TAB_GETROW(rec, "DATE_"));
                    MODULENR = CASTASSHORT(TAB_GETROW(rec, "MODULENR"));
                    TRCODE = CASTASSHORT(TAB_GETROW(rec, "TRCODE"));
                    SOURCEFREF = (TAB_GETROW(rec, "SOURCEFREF"));
                    SIGN = CASTASSHORT(TAB_GETROW(rec, "SIGN"));

                    DOCNR = CASTASSTRING(TAB_GETROW(rec, "DOCNR"));
                    COMPCARDTITLE = CASTASSTRING(TAB_GETROW(rec, "COMPCARDTITLE"));
                    GENEXP1 = CASTASSTRING(TAB_GETROW(rec, "GENEXP1"));
                    GENEXP2 = CASTASSTRING(TAB_GETROW(rec, "GENEXP2"));
                    GENEXP3 = CASTASSTRING(TAB_GETROW(rec, "GENEXP3"));
                    GENEXP4 = CASTASSTRING(TAB_GETROW(rec, "GENEXP4"));
                    LINEEXP = CASTASSTRING(TAB_GETROW(rec, "LINEEXP"));
                    SPECODE = CASTASSTRING(TAB_GETROW(rec, "SPECODE"));
                    SPECODE2 = CASTASSTRING(TAB_GETROW(rec, "SPECODE2"));
                    SPECODE3 = CASTASSTRING(TAB_GETROW(rec, "SPECODE3"));
                    LSPECODE = CASTASSTRING(TAB_GETROW(rec, "LSPECODE"));



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



        class MY_DIR
        {
            public static string PRM_DIR_ROOT = PATHCOMBINE(
     Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "EXPORT");

            static string filePrefix = "exp";

            public static void CHECK_DIR()
            {

                if (!System.IO.Directory.Exists(PRM_DIR_ROOT))
                    System.IO.Directory.CreateDirectory(PRM_DIR_ROOT);


            }




            public static string SAVE(StringBuilder pSb, string pSufix, string pExt = ".xls")
            {
                CHECK_DIR();


                var data = Encoding.UTF8.GetBytes(pSb.ToString());

                var fileName = filePrefix + "." + pSufix + "." + (FORMAT(DateTime.Now).Replace(" ", "-").Replace(":", "-")) + pExt;

                var fileNameFull = PRM_DIR_ROOT + "/" + fileName;

                FILEWRITE(fileNameFull, data);

                return fileNameFull;
            }
        }

        #endregion