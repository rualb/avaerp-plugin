#line 2


#region BODY
        //BEGIN

        const int VERSION = 11;
        const string FILE = "plugin.sys.event.grid_state.pls";



        #region TEXT


        const string event_GRID2FILE_ = "hadlericom_grid_state_";
        const string event_GRID2FILE_EXCEL = "hadlericom_grid_state_edit";


        #endregion

        #region MAIN




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


                    x.MY_GRID2FILE_USER = s.MY_GRID2FILE_USER;

                    x.GETSYSPRM_USER = PLUGIN.GETSYSPRM_USER();

                    _SETTINGS.BUF = x;

                }


                public string MY_GRID2FILE_USER;


                public short GETSYSPRM_USER;
            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC)
            {

            }


            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active On User")]
            public string MY_GRID2FILE_USER
            {
                get
                {
                    return (_GET("MY_GRID2FILE_USER", ""));
                }
                set
                {
                    _SET("MY_GRID2FILE_USER", value);
                }

            }


            public static bool ISUSEROK(_PLUGIN pPLUGIN)
            {
                return BUF.MY_GRID2FILE_USER == ""
                || Array.IndexOf<string>(
                     EXPLODELIST(BUF.MY_GRID2FILE_USER),
                     FORMAT(BUF.GETSYSPRM_USER)
                     ) >= 0;
            }

        }



        #endregion


        public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
        {

#if  DEBUG
  
           
#else
             //not for super admin in prod
             if (GETSYSPRM_USER() == 1)
                return;
#endif


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
                    MY_SYS_NEWFORM_INTEGRATE(arg1 as Form);
                    break;
                case SysEvent.SYS_USEREVENT:
                    MY_SYS_USEREVENT_HANDLER(EVENTCODE, ARGS);
                    break;

            }



        }



        void MY_SYS_NEWFORM_INTEGRATE(Form FORM)
        {
            if (FORM == null)
                return;

            var fn = GETFORMNAME(FORM);

            if (fn == null)
                return;

            var isTarget = fn.StartsWith("adp.") || fn.StartsWith("ref.");

            if (isTarget)
            {
                //cMenuMainInfo

                FORM.KeyPreview = true;
                FORM.KeyDown += (s, e) =>
                {

                    if (e.Shift && e.Control && e.Alt)
                    {
                        if (e.KeyCode == Keys.D9)
                        {
                            DataGridView grid = FORM.ActiveControl as DataGridView;
                            if (grid == null)
                            {
                                var editing = FORM.ActiveControl as IDataGridViewEditingControl;
                                if (editing != null)
                                    grid = editing.EditingControlDataGridView;

                            }


                            if (grid != null)
                                MY_GRID_STATE(grid);
                        }

                    }


                };

                //var itms = CONTROL_DESTRUCT(FORM);

                //foreach (var ctrl in itms)
                //{
                //    var grid = ctrl as DataGridView;
                //    if (grid != null)
                //    {
                //        //grid.PreviewKeyDown += (s, e) =>
                //        //{

                //        //    if (e.Shift && e.Control && e.Alt)
                //        //    {
                //        //        if (e.KeyCode == Keys.D9)
                //        //        {


                //        //            MY_GRID_STATE(s as DataGridView);
                //        //        }

                //        //    }

                //        //};
                //        //grid.KeyDown += (s,e) => {


                //        //    if (e.Shift && e.Control && e.Alt)
                //        //    {
                //        //        if (e.KeyCode == Keys.D9)
                //        //        {
                //        //            e.Handled = true;

                //        //            MY_GRID_STATE(s as DataGridView);
                //        //        }

                //        //    }
                //        //};
                //    }
                //}

            }

        }



        private void MY_GRID_STATE(DataGridView pGRID)
        {
            try
            {
                var form = new GridSateEditForm(this, pGRID);
                form.ShowDialog();

            }
            catch (Exception exc)
            {
                LOG(exc);
                MSGUSERERROR(exc.Message);
            }
        }


        public void MY_SYS_USEREVENT_HANDLER(string EVENTCODE, object[] ARGS) //adapter start
        {


            //
            try
            {


                object arg1 = ARGS.Length > 0 ? ARGS[0] : null;
                object arg2 = ARGS.Length > 1 ? ARGS[1] : null;
                object arg3 = ARGS.Length > 2 ? ARGS[2] : null;

                string[] list_ = EXPLODELISTPATH(EVENTCODE);
                var cmd = list_.Length > 1 ? list_[1].ToLowerInvariant() : "";

                //switch (cmd)
                //{


                //}


            }
            catch (Exception exc)
            {
                LOG(exc);
                MSGUSERERROR(exc.Message);
            }
        }




        //END



        #region CLAZZ


        class GridSateEditForm : Form
        {
            _PLUGIN PLUGIN;

            DataGridView targetGrid;
            public GridSateEditForm(_PLUGIN pPLUGIN, DataGridView pGRID)
            {


                PLUGIN = pPLUGIN;
                targetGrid = pGRID;

                this.Text = string.Format("{0}", TEXT.text_DESC);


                this.Icon = CTRL_FORM_ICON();
                this.Size = new Size(800, 600);
                this.AutoSize = true;
                //form.BackColor = Color.White;
                this.StartPosition = FormStartPosition.CenterScreen;

                init();

                refreshData();
            }

            private void init()
            {





                var mainPanel = new Panel();
                this.Controls.Add(mainPanel);
                mainPanel.Dock = DockStyle.Fill;
                mainPanel.AutoSize = true;


                var btnClose = new Button()
                {
                    Text = PLUGIN.LANG("T_CLOSE"),
                    Image = RES_IMAGE("close_16x16"),
                    ImageAlign = ContentAlignment.MiddleLeft,
                    Width = 160,
                    Dock = DockStyle.Right
                };
                btnClose.Click += (s, arg) =>
                {
                    this.Close();
                };

                var btnDoUp = new Button()
                {
                    Text = PLUGIN.LANG("T_UP"),
                    Image = RES_IMAGE("up_16x16"),
                    ImageAlign = ContentAlignment.MiddleLeft,
                    Width = 160,
                    Dock = DockStyle.Left
                };
                btnDoUp.Click += (s, arg) =>
                {
                    doMove(true);
                };
                var btnDoDown = new Button()
                {
                    Text = PLUGIN.LANG("T_DOWN"),
                    Image = RES_IMAGE("down_16x16"),
                    ImageAlign = ContentAlignment.MiddleLeft,
                    Width = 160,
                    Dock = DockStyle.Left
                };
                btnDoDown.Click += (s, arg) =>
                {
                    doMove(false);
                };

                var panelData = new Panel() { Dock = DockStyle.Fill };

                var panelBtn = new Panel() { Dock = DockStyle.Bottom, Height = 25 };


                mainPanel.Controls.Add(panelData);


                mainPanel.Controls.Add(new Panel() { Tag = "Dummmy Padding", Height = 10, Dock = DockStyle.Bottom });
                //

                mainPanel.Controls.Add(panelBtn);


                panelBtn.Controls.Add(btnDoDown);
                panelBtn.Controls.Add(btnDoUp);
                //

                panelBtn.Controls.Add(btnClose);
                //

                var grid = createGrid("grid");

                grid.ReadOnly = false;
                grid.Columns.AddRange(
                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_TITLE"), DataPropertyName = "TITLE", Width = 200, ReadOnly = true },
                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_CODE"), DataPropertyName = "CODE", Width = 200, ReadOnly = true },
                                   new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_WIDTH"), DataPropertyName = "WIDTH", Width = 100, ReadOnly = false },
                                   new DataGridViewCheckBoxColumn() { HeaderText = PLUGIN.LANG("T_VISIBLE"), DataPropertyName = "VISIBLE", Width = 60, ReadOnly = false, ThreeState = false, TrueValue = (short)1, FalseValue = (short)0 },
                    //  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_SORT"), DataPropertyName = "SORT", Width = 60, ReadOnly = false }
                    new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_TEXT"), DataPropertyName = "TEXT", Width = 120, ReadOnly = false }
                                   );


                foreach (DataGridViewColumn colObj in grid.Columns)
                {
                    colObj.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    colObj.SortMode = DataGridViewColumnSortMode.Programmatic;

                    switch (colObj.DataPropertyName)
                    {
                        case "TITLE":
                        case "TEXT":
                            colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                            break;
                        // case "SORT":
                        case "WIDTH":
                            colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            break;
                        case "VISIBLE":
                            colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            break;
                        default:

                            colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                            break;
                    }
                }


                {

                    grid.CellParsing += (sender, e) =>
                    {

                    };

                    grid.CellValidating += (sender, e) =>
                    {
                        DataGridView grid_ = sender as DataGridView;
                        if (grid_ == null)
                            return;

                        var colObj_ = e.ColumnIndex >= 0 ? grid_.Columns[e.ColumnIndex] : null;
                        if (colObj_ == null)
                            return;

                        var str = e.FormattedValue as string;


                        if (str != null)
                        {

                            var type = colObj_.ValueType;

                            if (type == typeof(short))
                            {
                                short v = 0;
                                e.Cancel = !short.TryParse(str, out v);
                            }

                            if (type == typeof(int))
                            {
                                int v = 0;
                                e.Cancel = !int.TryParse(str, out v);
                            }

                            if (type == typeof(double))
                            {
                                double v = 0;
                                e.Cancel = !double.TryParse(str, out v);
                            }
                        }

                    };

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



                        }
                    };

                    /////////
                }

                panelData.Controls.Add(grid);


            }

            protected override void OnClosing(CancelEventArgs e)
            {

                try
                {

                    doSaveState();


                }
                catch (Exception exc)
                {
                    PLUGIN.LOG(exc);
                    PLUGIN.MSGUSERERROR(exc.Message);
                }

                base.OnClosing(e);
            }


            void doSaveState()
            {
                var code = "grid";
                var grid = CONTROL_SEARCH(this, code) as DataGridView;
                var stateData = TAB_ASTABLE(grid.DataSource);
                if (stateData == null)
                    throw new Exception("Can not get DataTable from grid");


                var gridName = targetGrid.Name;
                var formName = GETFORMNAME(targetGrid.FindForm());

                var stateCode = formName + "#" + gridName;

                var state = PLUGIN.GETOBJECTSTATE(stateCode) ?? new Dictionary<string, string>();

                var indx = 0;//sort start from 1
                foreach (DataRow row in stateData.Rows)
                {
                    ++indx;

                    var CODE = CASTASSTRING(TAB_GETROW(row, "CODE"));

                    var currGridCol = TOOL_GRID.GET_GRID_COL(targetGrid, CODE);
                    if (currGridCol == null)
                        continue;


                    var SORT = indx; // CASTASSHORT(TAB_GETROW(row, "SORT"));
                    var WIDTH = CASTASSHORT(TAB_GETROW(row, "WIDTH"));
                    var VISIBLE = CASTASSHORT(TAB_GETROW(row, "VISIBLE"));
                    var TEXT = CASTASSTRING(TAB_GETROW(row, "TEXT")).Trim();

                    state[CODE + ".SORT"] = FORMAT(SORT);

                    if (WIDTH > 10)
                        state[CODE + ".WIDTH"] = FORMAT(WIDTH);
                    else
                        state.Remove(CODE + ".WIDTH");

                    state[CODE + ".VISIBLE"] = FORMAT(VISIBLE);

                    if (ISEMPTY(TEXT))
                        state.Remove(CODE + ".TEXT");
                    else
                        state[CODE + ".TEXT"] = (TEXT);

                }

                PLUGIN.SETOBJECTSTATE(stateCode, state, true);


                RUNUIINTEGRATION(targetGrid, "_cmd", "read_state");


            }
            void doMove(bool pUp)
            {
                try
                {
                    var code = "grid";
                    var grid = CONTROL_SEARCH(this, code) as DataGridView;
                    var stateData = TAB_ASTABLE(grid.DataSource);
                    if (stateData == null)
                        throw new Exception("Can not get DataTable from grid");

                    var row = TOOL_GRID.GET_GRID_ROW_DATA(grid);
                    if (row == null)
                        return;

                    var posOld = row.Table.Rows.IndexOf(row);
                    var posNew = posOld + (pUp ? -1 : +1);

                    if (posNew < 0 || posNew >= row.Table.Rows.Count)
                        return;





                    var dataOld = row.Table.Rows[posOld].ItemArray;
                    var dataNew = row.Table.Rows[posNew].ItemArray;

                    row.Table.Rows[posOld].ItemArray = dataNew;
                    row.Table.Rows[posNew].ItemArray = dataOld;

                    TOOL_GRID.SET_GRID_POSITION(grid, row.Table.Rows[posNew], null);

                }
                catch (Exception exc)
                {
                    PLUGIN.LOG(exc);
                    PLUGIN.MSGUSERERROR(exc.Message);
                }
            }


            void refreshData()
            {
                try
                {




                    var code = "grid";
                    var grid = CONTROL_SEARCH(this, code) as DataGridView;
                    //
                    var gridName = targetGrid.Name;
                    var formName = GETFORMNAME(targetGrid.FindForm());
                    var stateCode = formName + "#" + gridName;
                    var state = PLUGIN.GETOBJECTSTATE(stateCode) ?? new Dictionary<string, string>();
                    //

                    var table = new DataTable(code);


                    TAB_ADDCOL(table, "TITLE", typeof(string));
                    TAB_ADDCOL(table, "CODE", typeof(string));
                    //
                    TAB_ADDCOL(table, "WIDTH", typeof(short));
                    TAB_ADDCOL(table, "VISIBLE", typeof(short));
                    //TAB_ADDCOL(table, "SORT", typeof(short));

                    TAB_ADDCOL(table, "TEXT", typeof(string));

                    //





                    var targetTable = TAB_ASTABLE(targetGrid.DataSource);
                    if (targetTable == null)
                        throw new Exception("Can not get DataTable from grid");


                    var listCols = new List<DataGridViewColumn>();
                    foreach (DataGridViewColumn colGrid in targetGrid.Columns)
                    {
                        listCols.Add(colGrid);
                    }

                    listCols.Sort((x, y) =>
                    {

                        var compare = 0;

                        //Visible desc, sortIndex asc,DataPropertyName asc
                        var xArr = new IComparable[] { (y.Visible ? 1 : 0), x.DisplayIndex, x.DataPropertyName };
                        var yArr = new IComparable[] { (x.Visible ? 1 : 0), y.DisplayIndex, y.DataPropertyName };

                        for (var i = 0; i < xArr.Length; ++i)
                        {
                            compare = xArr[i].CompareTo(yArr[i]);

                            if (compare == 0)
                                continue;

                            return compare;

                        }
                        return 0;

                    });


                    foreach (DataGridViewColumn colGrid in listCols)
                    {

                        var CODE = colGrid.DataPropertyName;
                        var propWIDTH = CODE + ".WIDTH";
                        var propTEXT = CODE + ".TEXT";

                        var WIDTH = 0;
                        var TEXT = "";

                        try
                        {
                            WIDTH = CASTASINT(GETDIC(state, propWIDTH, "0"));
                            TEXT = CASTASSTRING(GETDIC(state, propTEXT, ""));
                        }
                        catch { }


                        var row = TAB_ADDROW(table);
                        TAB_FILLNULL(row);


                        TAB_SETROW(row, "TITLE", colGrid.HeaderText);
                        TAB_SETROW(row, "CODE", colGrid.DataPropertyName);
                        TAB_SETROW(row, "WIDTH", WIDTH);
                        TAB_SETROW(row, "VISIBLE", colGrid.Visible ? 1 : 0);
                        TAB_SETROW(row, "TEXT", TEXT);

                        // TAB_SETROW(row, "SORT", Math.Max(1, colGrid.DisplayIndex + 1));

                    }

                    foreach (DataColumn col in targetTable.Columns)
                    {

                    }



                    //table.DefaultView.Sort = "VISIBLE DESC,SORT ASC,CODE ASC";


                    if (grid != null)
                        grid.DataSource = table;




                }
                catch (Exception exc)
                {
                    PLUGIN.LOG(exc);
                    PLUGIN.MSGUSERERROR(exc.Message);
                }
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


        }


        class TEXT
        {

            public const string text_DESC = "Grid State Edit";


        }


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



            public string TEXT = "";



            public void lang_az()
            {

                TEXT = TEXT;

            }

            public void lang_ru()
            {

                TEXT = TEXT;

            }

            public void lang_tr()
            {


                TEXT = TEXT;

            }
        }



        #endregion



        #endregion
        #endregion