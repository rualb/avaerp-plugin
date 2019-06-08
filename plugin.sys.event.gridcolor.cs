#line 2

  #region BODY
        //BEGIN

        const int VERSION = 7;


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

                    x.MY_GRIDCOLOR_REC_CANCELLED = s.MY_GRIDCOLOR_REC_CANCELLED;
                    x.MY_GRIDCOLOR_REC_READONLY = s.MY_GRIDCOLOR_REC_READONLY;
                    x.MY_GRIDCOLOR_REC_NEGATIVE_LEVEL = s.MY_GRIDCOLOR_REC_NEGATIVE_LEVEL;
                    //

                    _SETTINGS.BUF = x;

                }

                public Color MY_GRIDCOLOR_REC_CANCELLED;
                public Color MY_GRIDCOLOR_REC_READONLY;
                public Color MY_GRIDCOLOR_REC_NEGATIVE_LEVEL;



            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC) //, "ava.production.config")
            {

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Grid Record Color If Cancelled")]
            public Color MY_GRIDCOLOR_REC_CANCELLED
            {
                get
                {
                    try
                    {
                        var clr = (_GET("MY_GRIDCOLOR_REC_CANCELLED", "blue"));
                        return System.Drawing.ColorTranslator.FromHtml(clr);
                    }
                    catch (Exception exc)
                    {

                    }
                    //warning
                    return Color.Orange;
                }
                set
                {
                    _SET("MY_BARCODETERM_LABELMAT_USER", System.Drawing.ColorTranslator.ToHtml(value));
                }

            }


            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Grid Record Color If Readonly")]
            public Color MY_GRIDCOLOR_REC_READONLY
            {
                get
                {
                    try
                    {
                        var clr = (_GET("MY_GRIDCOLOR_REC_READONLY", "darkgreen"));
                        return System.Drawing.ColorTranslator.FromHtml(clr);
                    }
                    catch (Exception exc)
                    {

                    }
                    //warning
                    return Color.Orange;
                }
                set
                {
                    _SET("MY_GRIDCOLOR_REC_READONLY", System.Drawing.ColorTranslator.ToHtml(value));
                }

            }


            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Grid Record Color If Negative Level")]
            public Color MY_GRIDCOLOR_REC_NEGATIVE_LEVEL
            {
                get
                {
                    try
                    {
                        var clr = (_GET("MY_GRIDCOLOR_REC_NEGATIVE_LEVEL", "#B3679B"));
                        return System.Drawing.ColorTranslator.FromHtml(clr);
                    }
                    catch (Exception exc)
                    {

                    }
                    //warning
                    return Color.Orange;
                }
                set
                {
                    _SET("MY_GRIDCOLOR_REC_NEGATIVE_LEVEL", System.Drawing.ColorTranslator.ToHtml(value));
                }

            }

        }

        #endregion



        #region TEXT

        public class TEXT
        {
            public const string text_DESC = "Grid Color";


            public class L
            {

            }
        }

        #endregion
        public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
        {


            if (ISWEB())
                return;


            _SETTINGS._BUF.LOAD_SETTINGS(this);


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
                    MY_SYS_NEWFORM(arg1 as Form);
                    break;

            }



        }

        public void MY_SYS_NEWFORM(Form FORM)
        {
            if (FORM == null) return;

            var refForm_ = FORM as Ava_Ext.DataRefernce.IFormForDataReferenceBase;
            if (refForm_ == null)
                return;

            var grid_ = CONTROL_SEARCH(FORM, "cGrid") as DataGridView;
            if (grid_ == null)
                return;

            grid_.RowPrePaint += event_grid_color;
        }




        void event_grid_color(object s, DataGridViewRowPrePaintEventArgs a)
        {
            try
            {
                DataRow dataRow = null;
                DataGridViewRow rowControl = null;
                DataGridView grid = null;
                try
                {
                    grid = s as DataGridView;
                    if (a.RowIndex >= 0)
                    {
                        rowControl = grid.Rows[a.RowIndex];
                        var v = rowControl.DataBoundItem as DataRowView;
                        if (v != null)
                            dataRow = v.Row;
                    }

                }
                catch { }

                if (
                grid == null ||
                rowControl == null ||
                dataRow == null ||
                TAB_ROWDELETED(dataRow)
                )
                    return;


                var _handled = false;

                var isCancelled = false;
                var isReadonly = false;
                var isNegLevel = false;



                if (!_handled)
                {
                    var isItems_ = (dataRow.Table.TableName == "ITEMS");
                    if (isItems_)
                    {
                        var col_ = dataRow.Table.Columns["GNTOTST_____ONHAND"];
                        if (col_ != null)
                        {
                            isNegLevel = (CASTASDOUBLE(ISNULL(dataRow[col_], (double)0)) < -0.01);
                            if (isNegLevel)
                            {
                                _handled = true;
                            }
                        }

                    }
                }

                if (!_handled)
                {
                    var isOrder_ = (dataRow.Table.TableName == "ORFICHE");
                    if (isOrder_)
                    {

                        var col_ = dataRow.Table.Columns["STATUS"];
                        if (col_ != null)
                        {
                            var status = CASTASSHORT(dataRow[col_]);
                            if (status == 1)
                            {
                                isCancelled = true;
                                _handled = true;
                            }
                            else
                                if (status == 2)
                                {
                                    isReadonly = true;
                                    _handled = true;
                                }

                        }

                    }
                }


                if (!_handled)
                {
                    var col_ = dataRow.Table.Columns["READONLY"];

                    if (col_ != null)
                    {

                        isReadonly = (col_ != null) ? (CASTASSHORT(ISNULL(dataRow[col_], (short)0)) == 1) : false;

                        if (isReadonly)
                            _handled = true;
                    }

                }


                if (!_handled)
                    foreach (var name in new string[] { "CANCELLED", "ACTIVE" })
                    {
                        var col_ = dataRow.Table.Columns[name];

                        if (col_ != null)
                        {

                            isCancelled = (col_ != null) ? (CASTASSHORT(ISNULL(dataRow[col_], (short)0)) == 1) : false;
                            if (isCancelled)
                            {
                                _handled = true;
                                break;
                            }
                        }
                    }




                Color newColor = grid.RowTemplate.DefaultCellStyle.ForeColor;


                if (isReadonly)
                    newColor = _SETTINGS.BUF.MY_GRIDCOLOR_REC_READONLY;
                else
                    if (isCancelled)
                        newColor = _SETTINGS.BUF.MY_GRIDCOLOR_REC_CANCELLED;
                    else
                        if (isNegLevel)
                            newColor = _SETTINGS.BUF.MY_GRIDCOLOR_REC_NEGATIVE_LEVEL;


                if (rowControl.DefaultCellStyle.ForeColor != newColor)
                    rowControl.DefaultCellStyle.ForeColor = newColor;


            }
            catch { }
        }

        #endregion