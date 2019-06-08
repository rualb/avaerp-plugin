#line 2

      #region BODY
        //BEGIN

        const int VERSION = 9;
        const string FILE = "plugin.sys.event.stockdoclineinfo.pls";
 
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

                    x.MY_STOCKLINESINFO_USER = s.MY_STOCKLINESINFO_USER;

                    //

                    _SETTINGS.BUF = x;

                }

                public string MY_STOCKLINESINFO_USER;



            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC) //, "ava.production.config")
            {

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active On User")]
            public string MY_STOCKLINESINFO_USER
            {
                get
                {
                    return (_GET("MY_STOCKLINESINFO_USER", "1,2"));
                }
                set
                {
                    _SET("MY_STOCKLINESINFO_USER", value);
                }

            }



            //

            public static bool ISUSEROK(_PLUGIN pPLUGIN)
            {
                return Array.IndexOf<string>(
                     EXPLODELIST(BUF.MY_STOCKLINESINFO_USER),
                     FORMAT(pPLUGIN.GETSYSPRM_USER())
                     ) >= 0;
            }

        }

        #endregion

        #region TEXT

        public class TEXT
        {
            public const string text_DESC = "Stock Lines Info";


            public class L
            {

            }
        }

        const string event_MAT_LINE_INFO_SLS = "hadlericom_mat_line_info_sls";
        const string event_MAT_LINE_INFO_PRCH = "hadlericom_mat_line_info_prch";
        const string event_MAT_LINE_INFO_LIST = "hadlericom_mat_line_info_list";
        const string event_MAT_LINE_INFO_WH_ONHAND = "hadlericom_mat_line_info_wh_onhand";


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
            try
            {
                _MY_SYS_NEWFORM_INTEGRATE_STOCKADP(FORM);
            }
            catch (Exception exc)
            {
                MSGUSERERROR("Cant add button: " + exc.Message);
            }

        }


        void _MY_SYS_NEWFORM_INTEGRATE_STOCKADP(Form FORM)
        {


            var fn = GETFORMNAME(FORM);



            var isPrch = fn.StartsWith("adp.prch.doc.inv");
            var isSls = fn.StartsWith("adp.sls.doc.inv");
            var isList = fn.StartsWith("ref.mm.rec.mat");
            var isOrder = fn.StartsWith("adp.sls.doc.order");



            if (isPrch || isSls || isList || isOrder)
            {

                var cPanelBtnSub = CONTROL_SEARCH(FORM, "cPanelBtnSub");

                if (cPanelBtnSub == null)
                    return;


                if (isPrch)
                    _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_MAT_LINE_INFO_PRCH, LANG("T_TRANS"));

                if (isSls)
                    _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_MAT_LINE_INFO_SLS, LANG("T_TRANS"));

                if (isOrder)
                    _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_MAT_LINE_INFO_SLS, LANG("T_TRANS"));

                if (isList)
                {
                    _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_MAT_LINE_INFO_LIST, LANG("T_TRANS"));
                    _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_MAT_LINE_INFO_WH_ONHAND, LANG("T_WH"), "basket_16x16");

                }
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


                    case event_MAT_LINE_INFO_PRCH:
                    case event_MAT_LINE_INFO_SLS:
                    case event_MAT_LINE_INFO_LIST:
                    case event_MAT_LINE_INFO_WH_ONHAND:
                        {
                            var f = arg1 as Form;
                            if (f != null) // if (ISADAPTERFORM(f))
                            {

                                var grid_ = CONTROL_SEARCH(f, "cGrid") as DataGridView;
                                if (grid_ != null)
                                {
                                    var data = TOOL_GRID.GET_GRID_ROW_DATA(grid_);
                                    if (data != null)
                                    {
                                        if (cmd == event_MAT_LINE_INFO_WH_ONHAND)
                                            MY_LINE_INFO_WIN_HTML(data, cmd);
                                        else
                                            MY_LINE_INFO_WIN(data);
                                    }

                                }


                            }


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


        void MY_LINE_INFO_WIN_HTML(DataRow pRow, string pCmd)
        {
            if (pRow == null)
                return;

            var isMat = (pRow.Table.TableName == "ITEMS");
            if (!isMat)
                return;

            var matref = (object)null;
            if (isMat)
            {

                matref = TAB_GETROW(pRow, "LOGICALREF");

            }


            if (ISEMPTYLREF(matref))
                return;

            var res = new StringBuilder();

            res.AppendLine("<html>");
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
                font-weight:bold
            }

</style>
 <body style=''>
");


            switch (pCmd)
            {
                case event_MAT_LINE_INFO_WH_ONHAND:
                    {

                        var date = CASTASDATE(SQLSCALAR(
                            MY_CHOOSE_SQL(
                            "select getdate()",
                            "select NOW()::TIMESTAMP(0)")
                            ));


                        var matDesc = CASTASSTRING(SQLSCALAR(
                            MY_CHOOSE_SQL(
@"		
SELECT I.CODE + '/' + I.NAME
FROM LG_$FIRM$_ITEMS I WITH(NOLOCK)
WHERE I.LOGICALREF = @P1
",
 @"		
SELECT I.CODE || '/' || I.NAME
FROM LG_$FIRM$_ITEMS I 
WHERE I.LOGICALREF = @P1
")
, new object[] { matref }));
                        var data = (SQL(
                            MY_CHOOSE_SQL(
@"

SELECT  
T.ONHAND,
T.INVENNO,
(SELECT W.NAME FROM L_CAPIWHOUSE W WHERE W.NR= T.INVENNO) WHOUSE_NAME
		
		FROM LG_$FIRM$_$PERIOD$_GNTOTST T WITH(NOLOCK)
		WHERE T.STOCKREF = @P1  AND T.ONHAND>0.01 AND T.INVENNO >= 0
		ORDER BY T.INVENNO ASC


",
@"

SELECT  
T.ONHAND,
T.INVENNO,
(SELECT W.NAME FROM L_CAPIWHOUSE W WHERE W.NR= T.INVENNO) WHOUSE_NAME
		
		FROM LG_$FIRM$_$PERIOD$_GNTOTST T 
		WHERE T.STOCKREF = @P1  AND T.ONHAND>0.01 AND T.INVENNO >= 0
		ORDER BY T.INVENNO ASC


")
, new object[] { matref }));





                        //name
                        res.AppendLine(string.Format("<h2 style='width:100%;text-align:center'>{0}</h2>",
                           HTMLENCODE(LANG("T_ONHAND"))
                              ));


                        //filter
                        {

                            var lines = new List<string[]>();

                            lines.Add(new string[] { LANG("T_DATE"), FORMAT(date, "yyyy-MM-dd hh:mm") });
                            lines.Add(new string[] { LANG("T_MATERIAL"), FORMAT(matDesc) });
                            res.AppendLine("<table>");

                            foreach (var row in lines)
                            {

                                res.AppendLine("<tr>");
                                foreach (var cell in row)
                                {
                                    res.AppendLine(string.Format(
                                    "<td>{0}</td>",
                                     cell
                                   ));
                                }
                                res.AppendLine("</tr>");

                            }


                            res.AppendLine("</table>");
                        }


                        res.AppendLine("<br/>");

                        //body
                        {
                            res.AppendLine("<table style='width:100%'>");

                            res.AppendLine("<tr>");
                            foreach (var cell in new string[] { "", LANG("T_WH - T_NR"), LANG("T_WH - T_DESC"), LANG("T_QUANTITY") })
                            {
                                res.AppendLine(string.Format(
                                "<th>{0}</th>",
                                 cell
                               ));
                            }
                            res.AppendLine("</tr>");


                            var tot = 0.0;
                            //

                            for (int i = 0; i <= data.Rows.Count; ++i)
                            {
                                var indx = i + 1;
                                var row = i < data.Rows.Count ? data.Rows[i] : null;
                                //
                                var isDark = false;
                                var isBold = false;

                                string[] arrCell = null;

                                if (row != null)
                                {
                                    var wh = CASTASSHORT(TAB_GETROW(row, "INVENNO"));
                                    var whDesc = CASTASSTRING(TAB_GETROW(row, "WHOUSE_NAME"));
                                    var amnt = ROUND(CASTASDOUBLE(TAB_GETROW(row, "ONHAND")), 2);
                                    tot += amnt;

                                    arrCell = new string[] { 
                             FORMAT(indx),
                                FORMAT(wh<0?"***":FORMAT(wh)), //0
                                FORMAT(wh<0?LANG("T_ALL"):whDesc),//1
                                FORMAT(amnt, "0.#"),//4

                            };

                                }
                                else
                                {
                                    isDark = true;
                                    isBold = true;

                                    arrCell = new string[] { 
                             "",
                                "", //0
                                FORMAT( LANG("T_TOTAL") ),//1
                                FORMAT(tot, "0.#"),//4

                            };

                                }


                                var backColor = "#FFFFFF";

                                if (indx % 2 == 1)
                                    backColor = "#F2F2F2";

                                if (isDark)
                                    backColor = "#B0B0B0";


                                var fontWeight = isBold ? "bold" : "normal";

                                res.AppendLine("<tr style='background-color:" + backColor + ";font-weight:" + fontWeight + "'>");
                                foreach (var cellVal in arrCell)
                                {
                                    res.AppendLine(string.Format(
                                    "<td>{0}</td>",
                                     cellVal
                                   ));
                                }
                                res.AppendLine("</tr>");




                            }

                            res.AppendLine("</table>");
                        }
                    }
                    break;
            }

            res.AppendLine("</body></html>");

            MSGUSERINFO(res.ToString());





        }

        void MY_LINE_INFO_WIN(DataRow pRow)
        {
            try
            {

                if (pRow == null)
                    return;

                var isMat = (pRow.Table.TableName == "ITEMS");
                var isLine = (pRow.Table.TableName == "STLINE" && pRow.Table.Columns["ORDTRANSREF"] != null);
                var isOrder = (pRow.Table.TableName == "STLINE" && pRow.Table.Columns["ORDTRANSREF"] == null);

                if (!isMat && !isLine && !isOrder)
                    return;

                var matref = (object)null;
                //var lref = TAB_GETROW(pRow, "LOGICALREF");
                var clref = isMat ? 0 : TAB_GETROW(pRow, "CLIENTREF");
                var date = new DateTime(2999, 1, 1);//             // isMat ? DateTime.Now.Date : CASTASDATE(TAB_GETROW(pRow, "DATE_"));
                var trcode = isMat ? (short)0 : CASTASSHORT(TAB_GETROW(pRow, "TRCODE"));
                var price = isMat ? 0.0 : CASTASDOUBLE(TAB_GETROW(pRow, "PRICE"));
                var linetype = isMat ? (short)0 : CASTASSHORT(TAB_GETROW(pRow, "LINETYPE"));




                ;

                if (isMat)
                {

                    matref = TAB_GETROW(pRow, "LOGICALREF");
                    //lref = TAB_GETROW(pRow, "LOGICALREF");
                    //clref = TAB_GETROW(pRow, "CLIENTREF");
                    //date = CASTASDATE(TAB_GETROW(pRow, "DATE_"));
                    //trcode = CASTASSHORT(TAB_GETROW(pRow, "TRCODE"));
                    //price = CASTASDOUBLE(TAB_GETROW(pRow, "PRICE"));
                    //linetype = CASTASSHORT(TAB_GETROW(pRow, "LINETYPE"));

                }


                if (isLine || isOrder)
                {

                    matref = TAB_GETROW(pRow, "STOCKREF");
                    //lref = TAB_GETROW(pRow, "LOGICALREF");
                    clref = TAB_GETROW(pRow, "CLIENTREF");
                    date = CASTASDATE(TAB_GETROW(pRow, "DATE_"));
                    trcode = CASTASSHORT(TAB_GETROW(pRow, "TRCODE"));
                    price = CASTASDOUBLE(TAB_GETROW(pRow, "PRICE"));
                    linetype = CASTASSHORT(TAB_GETROW(pRow, "LINETYPE"));

                }





                if (ISEMPTYLREF(matref))
                    return;

                if (linetype != 0)
                    return;

                var matDesc = CASTASSTRING(SQLSCALAR(
                    MY_CHOOSE_SQL(
@" SELECT  CODE+'/'+NAME CODE FROM LG_$FIRM$_ITEMS WHERE LOGICALREF = @P1",
@" SELECT  CODE||'/'||NAME CODE FROM LG_$FIRM$_ITEMS WHERE LOGICALREF = @P1"
),
                    new object[] { matref }));


                var dataLists = new List<DataTable>();
                var dataTypes = new List<DataType>();



                var clDesc = ISEMPTYLREF(clref) ? "" : SQLSCALAR(
                    MY_CHOOSE_SQL(
                    "select top(1) DEFINITION_ from LG_$FIRM$_CLCARD WITH(NOLOCK) where LOGICALREF = @P1",
                    "select DEFINITION_ from LG_$FIRM$_CLCARD where LOGICALREF = @P1 LIMIT 1"),
                    new object[] { clref });

                if (isMat)
                {

                    dataTypes.Add(DataType.sale);
                    dataTypes.Add(DataType.prchs);

                }

                if (isLine)
                {

                    if (trcode == 2 || trcode == 3 || trcode == 7 || trcode == 8)
                    {

                        if (!ISEMPTYLREF(clref))
                            dataTypes.Add(DataType.saleByCl);
                        dataTypes.Add(DataType.sale);


                        if (!ISEMPTYLREF(clref))
                            dataTypes.Add(DataType.prchsByCl);
                        dataTypes.Add(DataType.prchs);

                    }
                    else
                        if (trcode == 1 || trcode == 6)
                        {

                            if (!ISEMPTYLREF(clref))
                                dataTypes.Add(DataType.prchsByCl);
                            dataTypes.Add(DataType.prchs);


                            if (!ISEMPTYLREF(clref))
                                dataTypes.Add(DataType.saleByCl);
                            dataTypes.Add(DataType.sale);

                        }
                }

                if (isOrder)
                {

                    if (trcode == 2) //prch
                    {


                        if (!ISEMPTYLREF(clref))
                            dataTypes.Add(DataType.prchsByCl);
                        dataTypes.Add(DataType.prchs);


                        if (!ISEMPTYLREF(clref))
                            dataTypes.Add(DataType.saleByCl);
                        dataTypes.Add(DataType.sale);


                    }
                    else
                        if (trcode == 1) //prch
                        {

                            if (!ISEMPTYLREF(clref))
                                dataTypes.Add(DataType.saleByCl);
                            dataTypes.Add(DataType.sale);


                            if (!ISEMPTYLREF(clref))
                                dataTypes.Add(DataType.prchsByCl);
                            dataTypes.Add(DataType.prchs);
                        }

                }



                date = new DateTime(2999, 1, 1);

                foreach (var _dataType in dataTypes)
                {



                    switch (_dataType)
                    {
                        case DataType.prchs:
                            if (EXECMDTEXTALLOWED("ref.prch.doc.inv"))
                            {
                                var tmpData_ = MY_LAST_TRANS(matref, DateTime.Now.Date, 1);
                                tmpData_.TableName = LANG("T_PURCHASE : T_ALL");
                                dataLists.Add(tmpData_);
                            }
                            break;
                        case DataType.prchsByCl:
                            if (EXECMDTEXTALLOWED("ref.prch.doc.inv"))
                            {
                                var tmpData_ = MY_LAST_TRANS_CL(ISEMPTYLREF(clref) ? -1 : clref, matref, date, 1);
                                tmpData_.TableName = LANG("T_PURCHASE : ") + clDesc;
                                dataLists.Add(tmpData_);
                            }
                            break;
                        case DataType.sale:
                            if (EXECMDTEXTALLOWED("ref.sls.doc.inv"))
                            {
                                var tmpData_ = MY_LAST_TRANS(matref, DateTime.Now.Date, 8);
                                tmpData_.TableName = LANG("T_SALE : T_ALL");
                                dataLists.Add(tmpData_);

                            }
                            break;
                        case DataType.saleByCl:
                            if (EXECMDTEXTALLOWED("ref.sls.doc.inv"))
                            {

                                var tmpData_ = MY_LAST_TRANS_CL(ISEMPTYLREF(clref) ? -1 : clref, matref, date, 8);
                                tmpData_.TableName = LANG("T_SALE : ") + clDesc;
                                dataLists.Add(tmpData_);
                            }
                            break;


                    }

                }

                //
                dataLists.Reverse();

                var gridHeight = 110;

                Form form = new Form();
                form.Text = LANG("T_TRANS (T_INFO)") + " " + matDesc;
                form.Icon = CTRL_FORM_ICON();
                form.Size = new Size(700, gridHeight );
                form.AutoSize = true;
                //form.BackColor = Color.White;
                form.StartPosition = FormStartPosition.CenterScreen;
                var mainPanel = new Panel();
                form.Controls.Add(mainPanel);
                mainPanel.Dock = DockStyle.Fill;
                mainPanel.AutoSize = true;


                var btnClose = new Button() { Text = LANG("T_CLOSE"), Image = RES_IMAGE("close_16x16"), ImageAlign = ContentAlignment.MiddleLeft, Width = 160, Dock = DockStyle.Right };
                btnClose.Click += (s, arg) => { form.Close(); };


                var priceBox = new NumericUpDown() { Minimum = 0, Maximum = 99999999, DecimalPlaces = 2, Value = (decimal)price };


                var btnSetPrice = new Button() { Width = 80, Text = LANG("T_SAVE"), Image = RES_IMAGE("checked_16x16"), ImageAlign = ContentAlignment.MiddleLeft };
                btnSetPrice.Click += (s, arg) => { TAB_SETROW(pRow, "PRICE", (double)priceBox.Value); form.Close(); };


                var panelBtn = new Panel() { Dock = DockStyle.Bottom, Height = 25 };
                mainPanel.Controls.Add(panelBtn);
                panelBtn.Controls.Add(btnClose);
                /*
                var panelSetPrice = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight };


                mainPanel.Controls.Add(panelSetPrice);
                mainPanel.Controls.Add(btnClose);

                btnClose.Dock = DockStyle.Bottom;

                panelSetPrice.Controls.Add(btnSetPrice);
                panelSetPrice.Controls.Add(priceBox);
                panelSetPrice.Dock = DockStyle.Bottom;
                panelSetPrice.AutoSize = true;

                if (isMat)
                    panelSetPrice.Visible = false;
                */

                foreach (DataTable tab in dataLists) //reverse
                    if (tab != null)
                    {
                        var grid = new DataGridView();



                        mainPanel.Controls.Add(grid);
                        {
                            grid.Dock = DockStyle.Top;
                            grid.MultiSelect = false;
                            grid.ReadOnly = true;
                            grid.AllowUserToAddRows = false;
                            grid.AllowUserToDeleteRows = false;
                            grid.AllowUserToResizeColumns = false;
                            grid.AllowUserToResizeRows = false;
                            grid.Columns.AddRange(
                                new DataGridViewTextBoxColumn() { HeaderText = LANG("T_NET (T_SYS_CURR1)"), DataPropertyName = "PRICENET" },
                                new DataGridViewTextBoxColumn() { HeaderText = LANG("+T_VAT (T_SYS_CURR1)"), DataPropertyName = "PRICEVAT" },
                                new DataGridViewTextBoxColumn() { HeaderText = LANG("T_NET (T_SYS_CURR2)"), DataPropertyName = "PRICENETREP" },
                                new DataGridViewTextBoxColumn() { HeaderText = LANG("+T_VAT (T_SYS_CURR2)"), DataPropertyName = "PRICEVATREP" },
                                new DataGridViewTextBoxColumn() { HeaderText = LANG("T_QUANTITY"), DataPropertyName = "AMOUNT" },
                                new DataGridViewTextBoxColumn() { HeaderText = LANG("T_DATE"), DataPropertyName = "DATE_" },
                                new DataGridViewTextBoxColumn() { HeaderText = LANG("T_PERSONAL"), DataPropertyName = "CLCARD_DESC" }
                                );
                            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                            grid.AutoGenerateColumns = false;
                            grid.BackgroundColor = Color.White;
                            grid.RowHeadersVisible = false;
                            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                            grid.Height = gridHeight;
                            // grid.AutoSize = true;
                            grid.BorderStyle = BorderStyle.None;
                            grid.CellDoubleClick += (s, arg) =>
                            {
                                DataRow ROW = TOOL_GRID.GET_GRID_ROW_DATA(grid);
                                if (ROW == null)
                                    return;

                                //if(COMPARE(lref,TAB_GETROW(ROW,"INVOICEREF")))
                                //	return;
                                var type_ = CASTASSHORT(TAB_GETROW(ROW, "TRCODE"));
                                var suff = "";
                                if (type_ == 1 || type_ == 6)
                                    suff = "prch";
                                if (type_ == 2 || type_ == 3 || type_ == 7 || type_ == 8)
                                    suff = "sls";

                                if (suff == "")
                                    return;

                                var cmd = string.Format(
                                "adp.{3}.doc.inv.{2} cmd::view lref::{0} targetrec::{1}/STLINE",
                                TAB_GETROW(ROW, "INVOICEREF"),
                                TAB_GETROW(ROW, "LOGICALREF"),
                                type_,
                                suff
                                );

                                if (!EXEADPCMDALLOWED(cmd, null))
                                    return;

                                EXECMDTEXT(cmd);


                            };

                            foreach (DataGridViewColumn colObj in grid.Columns)
                                switch (colObj.DataPropertyName)
                                {
                                    case "PRICENET":
                                    case "PRICENETREP":
                                    case "PRICEVAT":
                                    case "PRICEGROSS":
                                    case "PRICEVATREP":
                                    case "PRICEGROSSREP":
                                        colObj.DefaultCellStyle.Format = "0.00;-0.00;''";
                                        colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                                        if (colObj.DataPropertyName == "PRICENET")
                                            colObj.DefaultCellStyle.ForeColor = Color.Green;

                                        if (colObj.DataPropertyName == "PRICEVAT")
                                            colObj.DefaultCellStyle.ForeColor = Color.Blue;

                                        break;
                                    case "AMOUNT":
                                        colObj.DefaultCellStyle.Format = "0.##;-0.##;''";
                                        colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                                        colObj.DefaultCellStyle.ForeColor = Color.SkyBlue;
                                        break;
                                    case "DATE_":
                                        colObj.DefaultCellStyle.Format = "d";
                                        colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                                        break;
                                }
                        }
                        grid.DataSource = tab;
                        //
                        var _l = new Label()
                        {
                            Text = tab.TableName,
                            Dock = DockStyle.Top,
                            TextAlign = ContentAlignment.MiddleCenter,
                            ForeColor = Color.DarkBlue,
                            BackColor = Color.White
                        };
                        _l.Font = new Font(_l.Font, FontStyle.Bold);
                        mainPanel.Controls.Add(_l);
                    }

                form.ShowDialog();

            }
            catch (Exception exc)
            {
                LOG(exc);
                MSGUSERERROR(exc.Message);
            }
        }

        DataTable MY_LAST_TRANS(object STOCKREF, DateTime DATE_, short TRCODE)
        {

            var sqlMat =
MY_CHOOSE_SQL(
@"

SELECT TOP (3) 
		 
		(case when AMOUNT > 0 then (VATMATRAH )/AMOUNT else 0 end) PRICENET,
		(case when AMOUNT > 0 then (VATMATRAH+VATAMNT )/AMOUNT else 0 end) PRICEVAT,
		(case when AMOUNT > 0 and REPORTRATE > 0 then (VATMATRAH )/AMOUNT/REPORTRATE else 0 end) PRICENETREP,
		(case when AMOUNT > 0 and REPORTRATE > 0 then (VATMATRAH+VATAMNT )/AMOUNT/REPORTRATE else 0 end) PRICEVATREP,
		--(case when AMOUNT > 0 then (VATMATRAH+VATAMNT+DISTEXP)/AMOUNT else 0 end) PRICEGROSS,
		AMOUNT, 
		DATE_,
		
		ISNULL((SELECT DEFINITION_ FROM LG_$FIRM$_CLCARD WHERE LOGICALREF = CLIENTREF),'') CLCARD_DESC,
		INVOICEREF,LOGICALREF,TRCODE
		FROM LG_$FIRM$_$PERIOD$_STLINE WITH (
				NOLOCK,
				INDEX = I$FIRM$_$PERIOD$_STLINE_I2
				)
		WHERE (
				STOCKREF = @P1 AND DATE_ <= @P2
				) AND (
				CANCELLED = 0 AND TRCODE = @P3
				)
		ORDER BY STOCKREF DESC,
			DATE_ DESC,
			FTIME DESC
 
			",
@"

SELECT  
		 
		(case when AMOUNT > 0 then (VATMATRAH )/AMOUNT else 0 end) PRICENET,
		(case when AMOUNT > 0 then (VATMATRAH+VATAMNT )/AMOUNT else 0 end) PRICEVAT,
		(case when AMOUNT > 0 and REPORTRATE > 0 then (VATMATRAH )/AMOUNT/REPORTRATE else 0 end) PRICENETREP,
		(case when AMOUNT > 0 and REPORTRATE > 0 then (VATMATRAH+VATAMNT )/AMOUNT/REPORTRATE else 0 end) PRICEVATREP,
		--(case when AMOUNT > 0 then (VATMATRAH+VATAMNT+DISTEXP)/AMOUNT else 0 end) PRICEGROSS,
		AMOUNT, 
		DATE_,
		
		COALESCE((SELECT DEFINITION_ FROM LG_$FIRM$_CLCARD WHERE LOGICALREF = CLIENTREF),'') CLCARD_DESC,
		INVOICEREF,LOGICALREF,TRCODE
		FROM LG_$FIRM$_$PERIOD$_STLINE 
		WHERE (
				STOCKREF = @P1 AND DATE_ <= @P2
				) AND (
				CANCELLED = 0 AND TRCODE = @P3
				)
		ORDER BY STOCKREF DESC,
			DATE_ DESC,
			FTIME DESC
 LIMIT 3
			");



            return SQL(sqlMat,
            new object[] { STOCKREF, DATE_, TRCODE });


        }

        DataTable MY_LAST_TRANS_CL(object CLIENTREF, object STOCKREF, DateTime DATE_, short TRCODE)
        {


            var sqlCl =
                MY_CHOOSE_SQL(
@"

SELECT TOP (3) 


		(case when AMOUNT > 0 then (VATMATRAH )/AMOUNT else 0 end) PRICENET,
		(case when AMOUNT > 0 then (VATMATRAH+VATAMNT )/AMOUNT else 0 end) PRICEVAT,
		(case when AMOUNT > 0 and REPORTRATE > 0 then (VATMATRAH )/AMOUNT/REPORTRATE else 0 end) PRICENETREP,
		(case when AMOUNT > 0 and REPORTRATE > 0 then (VATMATRAH+VATAMNT )/AMOUNT/REPORTRATE else 0 end) PRICEVATREP,
		--(case when AMOUNT > 0 then (VATMATRAH+VATAMNT+DISTEXP)/AMOUNT else 0 end) PRICEGROSS,
		AMOUNT, 
		DATE_,
		
		ISNULL((SELECT DEFINITION_ FROM LG_$FIRM$_CLCARD WHERE LOGICALREF = CLIENTREF),'') CLCARD_DESC,
		INVOICEREF,LOGICALREF,TRCODE
		FROM LG_$FIRM$_$PERIOD$_STLINE WITH (
				NOLOCK,
				INDEX = I$FIRM$_$PERIOD$_STLINE_I8
				)
		WHERE (
				CLIENTREF = @P4 AND STOCKREF = @P1 AND DATE_ <= @P2
				) AND (
				CANCELLED = 0 AND TRCODE = @P3
				)
		ORDER BY CLIENTREF DESC,
			STOCKREF DESC,
			DATE_ DESC,
			FTIME DESC
 
			",

@"

SELECT  


		(case when AMOUNT > 0 then (VATMATRAH )/AMOUNT else 0 end) PRICENET,
		(case when AMOUNT > 0 then (VATMATRAH+VATAMNT )/AMOUNT else 0 end) PRICEVAT,
		(case when AMOUNT > 0 and REPORTRATE > 0 then (VATMATRAH )/AMOUNT/REPORTRATE else 0 end) PRICENETREP,
		(case when AMOUNT > 0 and REPORTRATE > 0 then (VATMATRAH+VATAMNT )/AMOUNT/REPORTRATE else 0 end) PRICEVATREP,
		--(case when AMOUNT > 0 then (VATMATRAH+VATAMNT+DISTEXP)/AMOUNT else 0 end) PRICEGROSS,
		AMOUNT, 
		DATE_,
		
		COALESCE((SELECT DEFINITION_ FROM LG_$FIRM$_CLCARD WHERE LOGICALREF = CLIENTREF),'') CLCARD_DESC,
		INVOICEREF,LOGICALREF,TRCODE
		FROM LG_$FIRM$_$PERIOD$_STLINE 
		WHERE (
				CLIENTREF = @P4 AND STOCKREF = @P1 AND DATE_ <= @P2
				) AND (
				CANCELLED = 0 AND TRCODE = @P3
				)
		ORDER BY CLIENTREF DESC,
			STOCKREF DESC,
			DATE_ DESC,
			FTIME DESC
 LIMIT 3
			");
            return SQL(sqlCl,
            new object[] { STOCKREF, DATE_, TRCODE, CLIENTREF });


        }

        //END


        enum DataType
        {
            sale,
            prchs,
            saleByCl,
            prchsByCl



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
