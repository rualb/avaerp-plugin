 #line 2
 
 
 
  #region BODY
        //BEGIN

        const int VERSION = 11;
        const string FILE = "plugin.sys.event.grid2file.pls";



        #region TEXT


        const string event_GRID2FILE_ = "hadlericom_grid2file_";
        const string event_GRID2FILE_EXCEL = "hadlericom_grid2file_excel";


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

            var isList = fn.StartsWith("adp.");

            if (isList)
            {
                //cMenuMainInfo

                var gridName = GRID_NAME_BY_FORMNAME(fn);

                var grid = CONTROL_SEARCH(FORM, gridName);
                if (grid != null)
                {

                    var cPanelBtnSub = CONTROL_SEARCH(FORM, "cPanelBtnSub");

                    if (cPanelBtnSub == null)
                        return;
                    _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_GRID2FILE_EXCEL, LANG("T_EXIM (Excel)"), "excel_16x16");
                }

            }

        }


        static string GRID_NAME_BY_FORMNAME(string pFormName)
        {

            if (pFormName == "adp.mm.rec.mat")
                return "cGridMatSub";

            return "cGrid";
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
                object arg3 = ARGS.Length > 2 ? ARGS[2] : null;

                string[] list_ = EXPLODELISTPATH(EVENTCODE);
                var cmd = list_.Length > 1 ? list_[1].ToLowerInvariant() : "";

                switch (cmd)
                {


                    case event_GRID2FILE_EXCEL:
                        {

                            var FORM = arg1 as Form;
                            if (FORM != null && ISADAPTERFORM(FORM))
                            {
                                var fn = GETFORMNAME(FORM);
                                var gridName = GRID_NAME_BY_FORMNAME(fn);

                                var grid = CONTROL_SEARCH(FORM, gridName) as DataGridView;
                                var ds = GETDATASETFROMADPFORM(FORM);
                                MY_GRID2FILE(FORM, grid, ds);

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



        void MY_GRID2FILE(Form pForm, DataGridView pGrid, DataSet pDs)
        {
            if (pDs == null || pGrid == null || pForm == null)
                return;

            var tool = MY_ASK_TOOL(this);

            if (ISEMPTY(tool))
                return;

            switch (tool)
            {
                case tool_grid2file_export_template:
                    MY_GRID2FILE(pForm, pGrid, true);
                    break;
                case tool_grid2file_export:
                    MY_GRID2FILE(pForm, pGrid, false);
                    break;
                case tool_grid2file_import:
                    MY_GRID2FILE_IMPORT(pForm, pGrid, pDs);
                    break;
                case tool_grid2file_import_by_col:
                    MY_GRID2FILE_IMPORT_BY_COL(pForm, pGrid, pDs);
                    break;
            }


        }

        //END
        #region IMPORT


        DataRow[] MY_ADD_ROWS(DataGridView pGrid, DataSet pDs, int pCount)
        {
            var targetTable = TAB_ASTABLE(pGrid.DataSource);
            if (targetTable == null)
                throw new Exception("Can not get DataTable from grid");



            var list = new List<DataRow>();



            var lastPos = -1;
            foreach (DataRow rec in targetTable.Rows)
            {
                var stop = false;

                ++lastPos;
                //
                if (TAB_ROWADDED(rec))
                {
                    var ok = false;

                    switch (targetTable.TableName)
                    {
                        case "STLINE":
                            {
                                var trcode = CASTASSHORT(TAB_GETROW(rec, "TRCODE"));
                                var linetype_const = 0;

                                if (trcode == 9 || trcode == 4)
                                    linetype_const = 4;

                                var linetype_ok = CASTASSHORT(TAB_GETROW(rec, "LINETYPE")) == linetype_const;
                                var local = CASTASSHORT(TAB_GETROW(rec, "GLOBTRANS")) == 0;

                                if (linetype_ok && local)
                                    ok = true;

                                if (!local)
                                    stop = true;
                            }
                            break;
                        default:
                            ok = true;
                            break;


                    }




                    if (ok)
                    {
                        list.Add(rec);
                    }

                }
                else
                {
                    break;
                }


                if (stop)
                    break;

            }

            if (lastPos < 0)
                lastPos = 0;

            while (list.Count < pCount)
            {

                var newRow = TAB_ADDROW(targetTable, lastPos);
                list.Add(newRow);
                //var gridRow = TOOL_GRID.GET_GRID_ROW(pGrid, newRow);

                ++lastPos;
            }


            //var lastPos = -1;
            //foreach (DataRow rec in targetTable.Rows)
            //{
            //    ++lastPos;
            //    //
            //    if (!TAB_ROWDELETED(rec))
            //    {
            //        var ok = false;

            //        switch (targetTable.TableName)
            //        {
            //            case "STLINE":
            //                {
            //                    var linetype = CASTASSHORT(TAB_GETROW(rec, "LINETYPE"))  ;
            //                    var local = CASTASSHORT(TAB_GETROW(rec, "GLOBTRANS")) == 0;
            //                    var trcode = CASTASSHORT(TAB_GETROW(rec, "TRCODE"))  ;


            //                    if (linetype == 0 && local)
            //                        ok = true;

            //                    if (!local)
            //                        break;
            //                }
            //                break;
            //            default:
            //                ok = true;
            //                break;


            //        }


            //        if (ok)
            //        {
            //            list.Add(rec);
            //        }

            //    }

            //}

            //if (lastPos < 0)
            //    lastPos = 0;

            //while (list.Count < pCount)
            //{

            //    var newRow = TAB_ADDROW(targetTable, lastPos);
            //    list.Add(newRow);
            //    //var gridRow = TOOL_GRID.GET_GRID_ROW(pGrid, newRow);

            //    ++lastPos;
            //}




            return list.ToArray();
        }

        private void MY_GRID2FILE_IMPORT_BY_COL(Form pForm, DataGridView pGrid, DataSet pDs)
        {

            if (pGrid.ReadOnly)
                throw new Exception("Grid is read-only");

            var targetTable = TAB_ASTABLE(pGrid.DataSource);
            if (targetTable == null)
                throw new Exception("Can not get DataTable from grid");

            var bannedColumns = new List<string>(new string[] { 
            "LOGICALREF",
            "TRCODE"
            });
            var bannedColumnsByTab = new Dictionary<string, List<string>>() { 
            {"STLINE",new List<string>(new string[]{
            "LINETYPE"
            })},
            };



            var gridCellObject = TOOL_GRID.GET_GRID_CELL(pGrid);

            if (gridCellObject == null)
                return;

            var grid_col_obj = gridCellObject.OwningColumn;

            var data_name = grid_col_obj.DataPropertyName;
            var data_title = grid_col_obj.HeaderText;
            var data_col_obj = targetTable.Columns[data_name];

            if (data_col_obj == null)
                throw new Exception("Column [" + data_name + "] not exists in data table");


            if (grid_col_obj.ReadOnly)
                throw new Exception("Column [" + data_name + "] [" + data_title + "] is read-only");


            {

                {
                    foreach (var colName in bannedColumns)
                        if (data_name == (colName))
                            throw new Exception("Importing column [" + data_name + "] [" + data_title + "] not allowed");
                }

                if (bannedColumnsByTab.ContainsKey(targetTable.TableName))
                {
                    var bannedList = bannedColumnsByTab[targetTable.TableName];
                    foreach (var colName in bannedList)
                        if (data_name == (colName))
                            throw new Exception("Importing column [" + data_name + "] [" + data_title + "] not allowed");
                }
            }


            var text = MY_GET_TEXT(this, "T_COLUMN - " + data_title);

            if (ISEMPTY(text))
                return;

            var lines = text.Split('\n');

            for (var i = 0; i < lines.Length; ++i)
            {
                lines[i] = lines[i].Split('\t')[0].Trim();
            }

            


            var data_rows = MY_ADD_ROWS(pGrid, pDs, lines.Length);


            var is_grid_column_string = (data_col_obj.DataType == typeof(string));
            var is_grid_column_double = (data_col_obj.DataType == typeof(double));


            for (int i = 0; i < lines.Length; ++i)
            {



                var newRow = data_rows[i];
                var gridRow = TOOL_GRID.GET_GRID_ROW(pGrid, newRow);



                try
                {



                    var cell = gridRow.Cells[grid_col_obj.Index];

                    if (cell.ReadOnly)
                        continue;

                    var val = lines[i];

                    if (ISNULL(val) || ISEMPTY(val))
                        continue;

                    pGrid.CurrentCell = cell;

                    //if (is_grid_column_double)
                    //{
                    //    if (val == "0" || val == "0.0")
                    //        continue;
                    //}


                    if (is_grid_column_string)
                    {
                        if (val == cell.Value as string)
                            continue;
                    }
                

                    cell.Value = val;


                    if (is_grid_column_string)
                    {
                        if (val != cell.Value as string)
                        {
                            throw new Exception("Can not insert [" + val + "] at [" + (i+1) + "]. New value rejected.");
                        }
                    }
                }
                catch (Exception exc)
                {
                    throw new Exception("Error on import column by code:" + grid_col_obj.HeaderText + ".\n" + exc.Message);
                }



            }


            //var dicPropToGridColumn = new Dictionary<string, DataGridViewColumn>();

            //foreach (DataColumn colObjData in dataTable_.Columns)
            //    foreach (DataGridViewColumn colObjGrid in pGrid.Columns)
            //    {
            //        if (
            //            (colObjData.ColumnName == colObjGrid.DataPropertyName) &&
            //            (!colObjGrid.ReadOnly)
            //            )
            //        {
            //            dicPropToGridColumn[colObjData.ColumnName] = colObjGrid;
            //        }
            //    }

            //{
            //    foreach (var colName in bannedColumns)
            //        dicParametres_.Remove(colName);
            //}

            //if (bannedColumnsByTab.ContainsKey(targetTable.TableName))
            //{
            //    var bannedList = bannedColumnsByTab[targetTable.TableName];
            //    foreach (var colName in bannedList)
            //        dicParametres_.Remove(colName);
            //}


            //if (dicPropToGridColumn.Count <= 0)
            //    throw new Exception("No any column for import");

            //var rowNr = 0;
            //var insertPos = -1;
            //foreach (DataRow rowImp in dataTable_.Rows)
            //{
            //    ++rowNr;
            //    ++insertPos;

            //    try
            //    {
            //        var newRow = TAB_ADDROW(targetTable, insertPos);
            //        var gridRow = TOOL_GRID.GET_GRID_ROW(pGrid, newRow);

            //        foreach (DataColumn colObjData in dataTable_.Columns)
            //        {
            //            try
            //            {

            //                var gridCol = dicPropToGridColumn.ContainsKey(colObjData.ColumnName) ? dicPropToGridColumn[colObjData.ColumnName] : null; ;
            //                if (gridCol == null)
            //                    continue;



            //                var cell = gridRow.Cells[gridCol.Index];

            //                if (cell.ReadOnly)
            //                    continue;

            //                var val = ISNULL(TAB_GETROW(rowImp, colObjData.ColumnName), "") as string;

            //                if (ISNULL(val) || ISEMPTY(val))
            //                    continue;

            //                if (colObjData.DataType == typeof(double))
            //                {
            //                    if (val == "0" || val == "0.0")
            //                        continue;
            //                }


            //                cell.Value = val;


            //            }
            //            catch (Exception exc)
            //            {
            //                throw new Exception("Error on import column by code:" + colObjData.ColumnName + ".\n" + exc.Message);
            //            }
            //        }


            //    }
            //    catch (Exception exc)
            //    {
            //        throw new Exception("Error on import row by Nr:" + rowNr + ".\n" + exc.Message);
            //    }

            //}



            MSGUSERINFO("T_MSG_OPERATION_FINISHED (T_IMPORT)");

        }


        private void MY_GRID2FILE_IMPORT(Form pForm, DataGridView pGrid, DataSet pDs)
        {

            if (pGrid.ReadOnly)
                throw new Exception("Grid is read-only");

            var targetTable = TAB_ASTABLE(pGrid.DataSource);
            if (targetTable == null)
                throw new Exception("Can not get DataTable from grid");

            var bannedColumns = new List<string>(new string[] { 
            "LOGICALREF",
            "TRCODE"
            });
            var bannedColumnsByTab = new Dictionary<string, List<string>>() { 
            {"INVOICE",new List<string>(new string[]{
            "LINETYPE"
            })},
            {"STFICHE",new List<string>(new string[]{
            "LINETYPE"
            })},
            };


            const string dataTableData = "DATA";

            string xlsFile_ = ASKFILE("Excel|*.xls;*.xlsx");

            if (xlsFile_ == null || xlsFile_ == string.Empty)
                return;

            DataSet ds_ = XLSREAD(xlsFile_);
            if (!ds_.Tables.Contains(dataTableData))
                throw new Exception("T_MSG_OPERATION_STOPPING" + " (T_MSG_DATA_NO, " + dataTableData + ")");


            MY_CLEAN(ds_.Tables[dataTableData]);
            Dictionary<string, string> dicParametres_ = MY_GET_PARAMETERS(ds_.Tables[dataTableData], false); //get header info
            //Dictionary<string, string> dicParametresLine_ = null;
            DataTable dataTable_ = MY_GET_TABLE(ds_.Tables[dataTableData], dicParametres_, false);
            //DataTable dataTableLine_ = null;


            var dicPropToGridColumn = new Dictionary<string, DataGridViewColumn>();

            foreach (DataColumn colObjData in dataTable_.Columns)
                foreach (DataGridViewColumn colObjGrid in pGrid.Columns)
                {
                    if (
                        (colObjData.ColumnName == colObjGrid.DataPropertyName) &&
                        (!colObjGrid.ReadOnly)
                        )
                    {
                        dicPropToGridColumn[colObjData.ColumnName] = colObjGrid;
                    }
                }

            {
                foreach (var colName in bannedColumns)
                    dicPropToGridColumn.Remove(colName);
            }

            if (bannedColumnsByTab.ContainsKey(targetTable.TableName))
            {
                var bannedList = bannedColumnsByTab[targetTable.TableName];
                foreach (var colName in bannedList)
                    dicPropToGridColumn.Remove(colName);
            }


            if (dicPropToGridColumn.Count <= 0)
                throw new Exception("No any column for import");

            var rowNr = 0;
            var insertPos = -1;
            foreach (DataRow rowImp in dataTable_.Rows)
            {
                ++rowNr;
                ++insertPos;

                try
                {
                    var newRow = TAB_ADDROW(targetTable, insertPos);
                    var gridRow = TOOL_GRID.GET_GRID_ROW(pGrid, newRow);

                    foreach (DataColumn colObjData in dataTable_.Columns)
                    {
                        try
                        {
                            var is_grid_column_string = (colObjData.DataType == typeof(string));
                            var is_grid_column_double = (colObjData.DataType == typeof(double));


                            var gridCol = dicPropToGridColumn.ContainsKey(colObjData.ColumnName) ? dicPropToGridColumn[colObjData.ColumnName] : null; ;
                            if (gridCol == null)
                                continue;



                            var cell = gridRow.Cells[gridCol.Index];

                            if (cell.ReadOnly)
                                continue;

                            var val = (ISNULL(TAB_GETROW(rowImp, colObjData.ColumnName), "") as string)??"";

                            val = val.Trim();

                            if (ISNULL(val) || ISEMPTY(val))
                                continue;

                            if (is_grid_column_double)
                            {
                                if (val == "0" || val == "0.0")
                                    continue;
                            }


                            if (is_grid_column_string)
                            {
                                if (val == cell.Value as string)
                                    continue;
                            }


                            cell.Value = val;


                            if (is_grid_column_string)
                            {
                                if (val != cell.Value as string)
                                {
                                    throw new Exception("Can not insert [" + val + "] at [" + (rowNr) + "]. New value rejected.");
                                }
                            }


                        }
                        catch (Exception exc)
                        {
                            throw new Exception("Error on import column by code:" + colObjData.ColumnName + ".\n" + exc.Message);
                        }
                    }


                }
                catch (Exception exc)
                {
                    throw new Exception("Error on import row by Nr:" + rowNr + ".\n" + exc.Message);
                }

            }

            //  MY_IMPORT(dicParametres_, dataTable_, dicParametresLine_, dataTableLine_);


            MSGUSERINFO("T_MSG_OPERATION_FINISHED (T_IMPORT)");

        }

        DataTable MY_GET_TABLE(DataTable pData, Dictionary<string, string> pParameters, bool pIsLine)
        {
            const string parmTable = "TABLE";

            DataTable table_ = pData.Copy();
            if (table_.Rows.Count > 0) //parameters
                table_.Rows.RemoveAt(0);
            if (table_.Rows.Count > 0) //col desc
                table_.Rows.RemoveAt(0);

            //  string tableName_ = pParameters[parmTable];

            //List<string> listDelCols_ = new List<string>();
            //foreach (DataColumn col_ in table_.Columns)
            //{
            //    if (MY_IS_DUMMY(col_.ColumnName))
            //        listDelCols_.Add(col_.ColumnName);
            //}
            //foreach (string col_ in listDelCols_)
            //    table_.Columns.Remove(col_);
            ////
            //foreach (DataColumn col_ in table_.Columns)
            //{
            //    if (!MY_IS_VALID_DS_COL(tableName_, col_.ColumnName))
            //        throw new Exception("T_MSG_INVALID_PARAMETER, T_TABLE/T_COLUMN, " + tableName_ + "/" + col_.ColumnName);
            //}


            MY_CHECK_TABLE(table_, pParameters, pIsLine);
            //
            return table_;
        }


        void MY_CHECK_TABLE(DataTable pTable, Dictionary<string, string> pParameters, bool pIsLine)
        {
            const string parmTable = "TABLE";
            //
            //    string tableName_ = pParameters[parmTable];

            foreach (DataColumn col_ in pTable.Columns)
                foreach (DataRow row_ in pTable.Rows)
                    row_[col_] = row_[col_].ToString().Trim();

        }
        Dictionary<string, string> MY_GET_PARAMETERS(DataTable pData, bool pIsLine)
        {

            Dictionary<string, string> dic_ = new Dictionary<string, string>();


            return dic_;
        }

        void MY_CLEAN(DataTable pData)
        {

            if (pData == null)
                return;

            const int specialTopRowsCount = 2;

            List<DataRow> listRow_ = new List<DataRow>();
            List<DataColumn> listCol_ = new List<DataColumn>();

            foreach (DataRow row_ in pData.Rows)
                if (pData.Rows.IndexOf(row_) >= specialTopRowsCount)
                    if (MY_IS_EMPTY_ROW(row_))
                        listRow_.Add(row_);

            foreach (DataRow row_ in listRow_)
                pData.Rows.Remove(row_);

            foreach (DataColumn col_ in pData.Columns)
                if (MY_IS_EMPTY_COL(col_))
                    listCol_.Add(col_);

            foreach (DataColumn col_ in listCol_)
                pData.Columns.Remove(col_);

        }

        bool MY_IS_EMPTY_ROW(DataRow pRow)
        {
            foreach (DataColumn col_ in pRow.Table.Columns)
                if (ISNULL(pRow[col_], "").ToString().Trim() != "")
                    return false;
            return true;
        }
        bool MY_IS_EMPTY_COL(DataColumn pCol)
        {
            foreach (DataRow row_ in pCol.Table.Rows)
                if (ISNULL(row_[pCol], "").ToString().Trim() != "")
                    return false;
            return true;
        }

        #endregion

        #region EXPORT


        private void MY_GRID2FILE(Form pForm, DataGridView pGrid, bool pTemplate)
        {

            var first = true;

            var sb = new StringBuilder();

            sb.AppendLine(@"<?xml version='1.0'?>
<?mso-application progid='Excel.Sheet'?>
<Workbook xmlns='urn:schemas-microsoft-com:office:spreadsheet'
 xmlns:o='urn:schemas-microsoft-com:office:office'
 xmlns:x='urn:schemas-microsoft-com:office:excel'
 xmlns:ss='urn:schemas-microsoft-com:office:spreadsheet'
 xmlns:html='http://www.w3.org/TR/REC-html40'>
 <Styles>
  <Style ss:ID='Bold'>
   <Font ss:Bold='1'/>
  </Style>
 </Styles>
<Worksheet ss:Name='DATA'>
<Table>");

            if (pTemplate)
                APPEND_DATA(pGrid, sb);
            if (pTemplate)
                APPEND_PRM(pForm, pGrid, sb);

            APPEND_HEADER(pGrid, sb);

            var dicSum = new Dictionary<int, double>();

            APPEND_ROWS(pGrid, 0, sb, dicSum);

            if (!pTemplate)
                APPEND_SUM(pGrid, sb, dicSum);


            sb.AppendLine(@"
</Table>
</Worksheet>
</Workbook>");

            var name = MAKENAME(pForm.Text, "");

            var file = MY_DIR.SAVE(sb, name, ".xls");

            PROCESS(file, null);
        }
        void APPEND_DATA(DataGridView pGrid, StringBuilder pSb)
        {
            pSb.AppendLine("<Row ss:StyleID='Bold'>");

            for (var i = 0; i < pGrid.ColumnCount; ++i)
            {
                var val = HTMLESC(pGrid.Columns[i].DataPropertyName);
                pSb.Append("<Cell><Data ss:Type='" + "String" + "'>").Append(val).Append("</Data></Cell>").AppendLine();
            }

            pSb.AppendLine("</Row>");
        }
        void APPEND_PRM(Form pForm, DataGridView pGrid, StringBuilder pSb)
        {
            pSb.AppendLine("<Row ss:StyleID='Bold'>");

            var dic = new Dictionary<string, string>();

            //dic["TITLE"] = pForm.Text;

            foreach (var itm in dic)
            {
                foreach (var _val in new string[] { HTMLESC(itm.Key), HTMLESC(itm.Value) })
                {
                    var val = HTMLESC(_val);
                    pSb.Append("<Cell><Data ss:Type='" + "String" + "'>").Append(val).Append("</Data></Cell>").AppendLine();
                }
            }

            pSb.AppendLine("</Row>");
        }
        void APPEND_HEADER(DataGridView pGrid, StringBuilder pSb)
        {
            pSb.AppendLine("<Row ss:StyleID='Bold'>");

            for (var i = 0; i < pGrid.ColumnCount; ++i)
            {
                var val = HTMLESC(pGrid.Columns[i].HeaderText);
                pSb.Append("<Cell><Data ss:Type='" + "String" + "'>").Append(val).Append("</Data></Cell>").AppendLine();
            }

            pSb.AppendLine("</Row>");
        }
        void APPEND_SUM(DataGridView pGrid, StringBuilder pSb, Dictionary<int, double> pSum)
        {
            pSb.AppendLine("<Row ss:StyleID='Bold'>");

            for (var i = 0; i < pGrid.ColumnCount; ++i)
            {
                var type = "String";// 
                var str = "";// 
                if (pSum.ContainsKey(i))
                {
                    type = "Number";
                    str = FORMAT(ROUND(pSum[i], 2));
                }

                pSb.Append("<Cell><Data ss:Type='" + type + "'>").Append(str).Append("</Data></Cell>").AppendLine();
            }

            pSb.AppendLine("</Row>");
        }

        void APPEND_ROWS(DataGridView pGrid, int pPos, StringBuilder pSb, Dictionary<int, double> pSum)
        {

            TOOL_GRID.SET_GRID_POSITION(pGrid, pPos, null);

            var pos = TOOL_GRID.GET_GRID_POS(pGrid);

            for (; pos >= 0 && pos < pGrid.Rows.Count; ++pos)
            {
                TOOL_GRID.SET_GRID_POSITION(pGrid, pos, null);
                var gridRow = TOOL_GRID.GET_GRID_ROW(pGrid);

                pSb.AppendLine("<Row>");

                for (var i = 0; i < pGrid.ColumnCount; ++i)
                {
                    var cell = gridRow.Cells[i];
                    var val = "";

                    var type = "String";//Number

                    if (cell.Value != null && (
                        cell.Value.GetType() == typeof(double) ||
                        cell.Value.GetType() == typeof(float)
                        ))
                    {
                        var num = ROUND(CASTASDOUBLE(cell.Value), 2);

                        if (pSum != null)
                        {
                            if (!pSum.ContainsKey(i))
                                pSum[i] = 0;

                            pSum[i] += num;
                        }

                        val = FORMAT(num);

                        type = "Number";
                    }
                    else
                    {
                        val = (cell.FormattedValue as string) ?? "";
                        val = HTMLESC(val);
                    }


                    pSb.Append("<Cell><Data ss:Type='" + type + "'>").Append(val).Append("</Data></Cell>").AppendLine();


                }

                pSb.AppendLine("</Row>");



            }

        }



        #endregion


        #region CLAZZ

        const string tool_grid2file_export = "tool_grid2file_export";
        const string tool_grid2file_export_template = "tool_grid2file_export_template";
        const string tool_grid2file_import = "tool_grid2file_import";
        const string tool_grid2file_import_by_col = "tool_grid2file_import_by_col";

        static string MY_ASK_TOOL(_PLUGIN pPLUGIN)
        {
            var list = new List<string>();

            //
            //distribute
            //sum debit
            //sum credit

            list.AddRange(new string[] { tool_grid2file_import, pPLUGIN.LANG("T_IMPORT") });
            list.AddRange(new string[] { tool_grid2file_import_by_col, pPLUGIN.LANG("T_IMPORT (T_COLUMN)") });
            list.AddRange(new string[] { tool_grid2file_export, pPLUGIN.LANG("T_EXPORT") });
            list.AddRange(new string[] { tool_grid2file_export_template, pPLUGIN.LANG("T_EXPORT (T_TEMPLATE)") });


            var res_ = pPLUGIN.REF("ref.gen.definedlist [obj::" + JOINLIST(list.ToArray()) + "] [desc::T_TOOL] type::string");

            string exportCode_ = (res_ != null && res_.Length > 0 ? CASTASSTRING(res_[0]["VALUE"]) : null);

            return exportCode_;

        }




        static string MY_GET_TEXT(_PLUGIN pPLUGIN, string pTitle)
        {

            DataRow[] refData_ = pPLUGIN.REF("ref.gen.text desc::" + STRENCODE(pTitle)); //
            if (refData_ == null || refData_.Length <= 0)
                return null;

            return CASTASSTRING(refData_[0]["VALUE"]);

        }

        class TEXT
        {

            public const string text_DESC = "Grid 2 File";

            static TEXT _L = null;

            public static TEXT L
            {
                get
                {
                    if (_L == null)
                    {
                        _L = new TEXT();


                    }

                    return _L;
                }
            }


            public TEXT()
            {

                lang_en();

                var m = this.GetType().GetMethod("lang_" + LANG_ACTIVE());
                if (m != null)
                    m.Invoke(this, null);

            }



            public void lang_tr()
            {

                A = "A";

            }


            public void lang_en()
            {

                A = "A";


            }

            public void lang_az()
            {

                A = "A";



            }



            public void lang_ru()
            {

                A = "A";


            }

            public string A;



        }


        class MY_DIR
        {
            public static string PRM_DIR_ROOT = PATHCOMBINE(
     Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "GRID2FILE");

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



        #endregion
        #endregion