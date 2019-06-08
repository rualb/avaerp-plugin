#line 2


  #region PLUGIN_BODY
        const int VERSION = 21;



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


                    //

                    x.MY_IMPORT_EXCEL_ONUSERNR = s.MY_IMPORT_EXCEL_ONUSERNR;

                    //
                    x._USER = PLUGIN.GETSYSPRM_USER();
                    x._FIRM = PLUGIN.GETSYSPRM_FIRM();
                    x._FIRMNAME = PLUGIN.GETSYSPRM_FIRMNAME();
                    x._PERIOD = PLUGIN.GETSYSPRM_PERIOD();


                    x._ISUSEROK =
                      new List<string>(EXPLODELIST(x.MY_IMPORT_EXCEL_ONUSERNR)).Contains(FORMAT(x._USER));


                    _SETTINGS.BUF = x;


                }


                public string MY_IMPORT_EXCEL_ONUSERNR;







                public bool _ISUSEROK;
                public short _FIRM;
                public string _FIRMNAME;
                public short _PERIOD;
                public short _USER;
            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC) //, "ava.email.config")
            {

            }


            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active If User Nr")]
            public string MY_IMPORT_EXCEL_ONUSERNR
            {
                get
                {
                    return (_GET("MY_IMPORT_EXCEL_ONUSERNR", "1,2"));
                }
                set
                {
                    _SET("MY_IMPORT_EXCEL_ONUSERNR", value);
                }

            }







        }

        #endregion
        #region TEXT

        public class TEXT
        {
            public const string text_DESC = "Import Excel";

        }


        const string event_IMPORT_EXCEL_ = "_import_excel_";
        const string event_IMPORT_EXCEL_MAT = "_import_excel_mat";
        const string event_IMPORT_EXCEL_CLIENT = "_import_excel_client";
        const string event_IMPORT_EXCEL_ACCOUNT = "_import_excel_account";
        const string event_IMPORT_EXCEL_GENERAL = "_import_excel_general";

        #endregion

        #region MAIN



        public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
        {

            if (ISWEB())
                return;


            object arg0 = ARGS.Length > 0 ? ARGS[0] : null;
            object arg1 = ARGS.Length > 1 ? ARGS[1] : null;
            object arg2 = ARGS.Length > 2 ? ARGS[2] : null;


            _SETTINGS._BUF.LOAD_SETTINGS(this);


            string[] list_ = EXPLODELISTPATH(EVENTCODE);

            var cmdType = list_.Length > 0 ? list_[0] : "";
            var cmdExt = list_.Length > 1 ? list_[1] : "";




            switch (cmdType)
            {
                case SysEvent.SYS_PLUGINSETTINGS:
                    (arg0 as List<object>).Add(new _SETTINGS(this));
                    break;

                case SysEvent.SYS_USEREVENT:
                    if (_SETTINGS.BUF._ISUSEROK)
                        MY_SYS_USEREVENT_HANDLER(this, EVENTCODE, ARGS);
                    break;


                case SysEvent.SYS_NEWFORM:
                    if (_SETTINGS.BUF._ISUSEROK)
                        MY_SYS_NEWFORM_INTEGRATE(this, arg0 as Form);
                    break;



            }



        }


        static void MY_SYS_NEWFORM_INTEGRATE(_PLUGIN pPLUGIN, Form FORM)
        {


            var fn = GETFORMNAME(FORM);

            if (fn == null)
                return;

            if (fn == "form.app")
            {
                var tree = CONTROL_SEARCH(FORM, "cTreeTools");
                if (tree != null)
                {

                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
   
 
			{ "Text" ,"T_IMPORT (Excel)"},
			{ "ImageName" ,"excel_32x32"},
			 { "Name" ,event_IMPORT_EXCEL_},
            };

                        RUNUIINTEGRATION(tree, args);

                    }

                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
            { "_root" ,event_IMPORT_EXCEL_},
			{ "CmdText" ,"event name::"+event_IMPORT_EXCEL_MAT},
			{ "Text" ,"T_IMPORT (Excel) T_MATERIAL"},
			{ "ImageName" ,"import_32x32"},
			//{ "Name" ,""},
            };

                        RUNUIINTEGRATION(tree, args);

                    }

                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
                    { "_root" ,event_IMPORT_EXCEL_},
			{ "CmdText" ,"event name::"+event_IMPORT_EXCEL_CLIENT},
			{ "Text" ,"T_IMPORT (Excel) T_PERSONAL"},
			{ "ImageName" ,"import_32x32"},
			//{ "Name" ,""},
            };

                        RUNUIINTEGRATION(tree, args);

                    }
                    if (pPLUGIN.EXEADPCMDALLOWED("adp.gl.rec.acc", null))
                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
                  { "_root" ,event_IMPORT_EXCEL_},
			{ "CmdText" ,"event name::"+event_IMPORT_EXCEL_ACCOUNT},
			{ "Text" ,"T_IMPORT (Excel) T_GL - T_ACCOUNT"},
			{ "ImageName" ,"import_32x32"},
			//{ "Name" ,""},
            };

                        RUNUIINTEGRATION(tree, args);

                    }

                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
                     { "_root" ,event_IMPORT_EXCEL_},
			{ "CmdText" ,"event name::"+event_IMPORT_EXCEL_GENERAL},
			{ "Text" ,"T_IMPORT (Excel) T_GENERAL"},
			{ "ImageName" ,"import_32x32"},
			//{ "Name" ,""},
            };

                        RUNUIINTEGRATION(tree, args);

                    }







                }
                return;

            }









        }


        static void MY_SYS_USEREVENT_HANDLER(_PLUGIN pPLUGIN, string EVENTCODE, object[] ARGS) //adapter start
        {


            //
            try
            {


                object arg1 = ARGS.Length > 0 ? ARGS[0] : null;
                object arg2 = ARGS.Length > 1 ? ARGS[1] : null;

                string[] list_ = EXPLODELISTPATH(EVENTCODE);

                switch (list_.Length > 1 ? list_[1].ToLowerInvariant() : "")
                {


                    case event_IMPORT_EXCEL_MAT:

                        new IMPORT_MAT(pPLUGIN).IMPORT();

                        break;
                    case event_IMPORT_EXCEL_CLIENT:

                        new IMPORT_CLIENT(pPLUGIN).IMPORT();

                        break;
                    case event_IMPORT_EXCEL_ACCOUNT:

                        new IMPORT_ACCOUNT(pPLUGIN).IMPORT();

                        break;
                    case event_IMPORT_EXCEL_GENERAL:

                        new IMPORT_GENERAL(pPLUGIN).IMPORT();

                        break;


                }


            }
            catch (Exception exc)
            {
                RUNTIMELOG(exc.ToString());
                pPLUGIN.MSGUSERERROR(exc.Message);
            }
        }








        #endregion

        #region MAT


        class IMPORT_MAT
        {


            //text 70-89
            //date 90-99
            //float 1-69
            //dummy >=500
            //

            //vers 5


            const string parmAdp = "ADP";
            const string parmTable = "TABLE";
            const string dataTableData = "IMPORT_MATERIAL";

            const string mainTable = "ITEMS";
            const string mainAdp = "adp.mm.rec.mat/1";
            string docAdp = "adp.prch.doc.inv.1";
            //if empty, doc price will be used 


            bool PRM_FORCE_UPDATE_CARD = false;

            const int specialTopRowsCount = 2;


            _PLUGIN PLUGIN;

            public IMPORT_MAT(_PLUGIN pPLUGIN)
            {
                PLUGIN = pPLUGIN;
            }

            public void IMPORT()
            {


                if (!PLUGIN.MSGUSERASK("T_MSG_COMMIT_BEGIN (T_MATERIAL, T_IMPORT)"))
                    return;

                string xlsFile_ = ASKFILE("Excel|*.xls;*.xlsx");

                if (xlsFile_ == null || xlsFile_ == string.Empty)
                    return;

                DataSet ds_ = XLSREAD(xlsFile_);
                if (!ds_.Tables.Contains(dataTableData))
                    throw new Exception("T_MSG_OPERATION_STOPPING" + " (T_MSG_DATA_NO, " + dataTableData + ")");

                MY_CLEAN(ds_.Tables[dataTableData]);


                Dictionary<string, string> dicParametres_ = MY_GET_PARAMETERS(ds_.Tables[dataTableData]); //get header info



                if (dicParametres_.ContainsKey("ADPDOC"))
                    docAdp = dicParametres_["ADPDOC"];

                DataTable dataTable_ = MY_GET_TABLE(ds_.Tables[dataTableData], dicParametres_);


                MY_IMPORT(dicParametres_, dataTable_);

                PLUGIN.MSGUSERINFO("T_MSG_OPERATION_FINISHED (T_MATERIAL, T_IMPORT)");
            }
            void MY_CLEAN(DataTable pData)
            {
                if (pData == null)
                    return;
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
            string CHECKDATE(string pDate)
            {

                string ptrn_ = "1900-01-01 00-00-00";
                string res_ = pDate;
                if (pDate.Length < ptrn_.Length)
                    res_ = pDate + ptrn_.Substring(pDate.Length, ptrn_.Length - pDate.Length);
                return res_;

            }




            class IMPORT_ITEM
            {

                public DataTable first = null;

                public Dictionary<string, DataTable> tables = new Dictionary<string, DataTable>();

                public Dictionary<string, string> parameters;

                public string getAdapterCmd()
                {
                    return mainAdp;//  parameters[parmAdp];
                }

            }
            IMPORT_ITEM[] MY_CONVERTTOITEMS(Dictionary<string, string> pParameters, DataTable pTable)
            {
                List<IMPORT_ITEM> list_ = new List<IMPORT_ITEM>();


                foreach (DataRow row_ in pTable.Rows)
                {

                    IMPORT_ITEM item_ = new IMPORT_ITEM();

                    item_.parameters = pParameters;


                    DataTable tab_ = pTable.Clone();
                    DataRow newParentRow_ = tab_.Rows.Add(row_.ItemArray);
                    item_.first = item_.tables[tab_.TableName] = (tab_);




                    //split TABLE_____COLUMN to one-to-many

                    List<DataTable> listTabs_ = new List<DataTable>();

                    for (int c = 0; c < tab_.Columns.Count; ++c)
                    {

                        DataColumn col_ = tab_.Columns[c];
                        //
                        // if column like PRCLIST/PTYPE  INVDEF/MINLEVEL/INVENNO/0 INVDEF/MINLEVEL/1
                        //
                        string[] arrCol_ = EXPLODELISTPATH(col_.ColumnName);
                        if (arrCol_.Length >= 2) //TAB_ISCOLFULLNAME(col_.ColumnName))
                        {

                            string tabName_ = arrCol_[0];// TAB_GETTABFROMFULLNAME(col_.ColumnName);
                            string colName_ = arrCol_[1];//TAB_GETCOLFROMFULLNAME(col_.ColumnName);


                            DataTable newChild_ = null;
                            DataRow newChildLastRow_ = null;

                            //search tab
                            foreach (DataTable t in listTabs_)
                                if (t.TableName == tabName_)
                                    newChild_ = t;

                            if (newChild_ == null) newChild_ = new DataTable(tabName_);

                            //if has col then add new row
                            if (newChild_.Columns[colName_] == null)
                                newChild_.Columns.Add(colName_, typeof(string)); //add col
                            //


                            string colFilterName_ = arrCol_.Length >= 4 ? arrCol_[2] : null;
                            string colFilterVal_ = arrCol_.Length >= 3 ? arrCol_[3] : null; ;


                            var filterByColVal = (colFilterVal_ != null && colFilterName_ != null);
                            var filterByIndex = (colFilterVal_ != null && colFilterName_ == null);

                            int rowIndx_ = filterByIndex ? (PARSEINT(arrCol_[2]) - 1) : 0;

                            if (rowIndx_ < 0)
                                throw new Exception("Row index of [" + col_.ColumnName + "] is incorrect");

                            //

                            if (filterByIndex)
                            {

                                while (newChild_.Rows.Count - 1 < rowIndx_)
                                    newChild_.Rows.Add(newChild_.NewRow()); //new row


                                newChildLastRow_ = newChild_.Rows[rowIndx_];
                            }
                            else
                                if (filterByColVal)
                                {
                                    var type_ = MY_GET_COL_TYPE(tabName_, colFilterName_);
                                    //  var colObj = newChild_.Columns[colFilterName_];

                                    if (type_ == null)
                                        throw new Exception("Table [" + tabName_ + "] hasnt column [" + colFilterName_ + "]");

                                    //if has col then add new row
                                    if (newChild_.Columns[colFilterName_] == null)
                                        newChild_.Columns.Add(colFilterName_, typeof(string)); //add col
                                    //


                                    newChildLastRow_ = TAB_SEARCH(newChild_, colFilterName_, colFilterVal_);
                                    if (newChildLastRow_ == null)
                                    {
                                        newChild_.Rows.Add(newChildLastRow_ = newChild_.NewRow());
                                        TAB_SETROW(newChildLastRow_, colFilterName_, colFilterVal_);
                                    }
                                }
                                else
                                {

                                    //first record
                                    if (newChild_.Rows.Count == 0)
                                        newChild_.Rows.Add(newChild_.NewRow()); //new row
                                    newChildLastRow_ = newChild_.Rows[0];

                                }

                            newChildLastRow_[colName_] = newParentRow_[col_];

                            //remove row
                            tab_.Columns.Remove(col_);
                            --c;
                            //
                            listTabs_.Add(newChild_);
                        }



                    }

                    foreach (DataTable t in listTabs_)
                        TAB_FILLNULL(t);

                    foreach (var t in listTabs_)
                        item_.tables[t.TableName] = (t);

                    list_.Add(item_);

                }

                return list_.ToArray();
            }
            void MY_IMPORT(Dictionary<string, string> pParameters, DataTable pTable)
            {
                MY_CHECK_REPEAT(pTable, pParameters);


                //change to real names
                pTable.TableName = pParameters[parmTable];
                //

                //explode tables by TABNAME_____COLUMN
                //

                IMPORT_ITEM[] items_ = MY_CONVERTTOITEMS(pParameters, pTable);

                PLUGIN.INVOKEINBATCH(MY_IMPORT_EXT, new object[] { items_, pParameters });


            }



            string GET_CMD_SUFIX(IMPORT_ITEM pItem)
            {

                if (pItem.first == null)
                    return "";


                var code_ = CASTASSTRING(TAB_GETROW(pItem.first, "CODE"));

                var ref_ = ((PLUGIN.SQLSCALAR("SELECT LOGICALREF FROM LG_$FIRM$_ITEMS WITH(NOLOCK) WHERE CODE = @P1", new object[] { code_ })));

                if (!ISEMPTYLREF(ref_))
                    return " cmd::edit lref::" + FORMAT(ref_);


                return "";
            }

            void MY_IMPORT_EXT(object t, DoWorkEventArgs vars)
            {

                try
                {
                    object[] objs_ = (object[])vars.Argument;
                    var items_ = ACTIVE_JOB_ITEMS = objs_[0] as IMPORT_ITEM[];
                    var prmts_ = PARAMETERS = objs_[1] as Dictionary<string, string>;


                    var indx = 0;

                    foreach (IMPORT_ITEM item_ in items_)
                    {
                        ++indx;
                        var code = FORMAT(indx);

                        var tab = item_.tables["ITEMS"];
                        if (tab != null)
                        {
                            code = CASTASSTRING(TAB_GETROW(tab, "CODE"));

                        }


                        ACTIVE_JOB_ITEM = item_;

                        string sufix_ = GET_CMD_SUFIX(item_);
                        if (sufix_ == "") //add
                            UPDATE_CARD = true;
                        else
                            UPDATE_CARD = false;


                        if (PRM_FORCE_UPDATE_CARD)
                            UPDATE_CARD = true;

                        if (MY_IS_UPDATE())
                            UPDATE_CARD = true;

                        var cmd = item_.getAdapterCmd() + sufix_;

                        try
                        {
                            PLUGIN.EXEADPCMD(new string[] { cmd }, new DoWorkEventHandler[] { MY_IMPORT_ITEM }, true);

                        }
                        catch (Exception exc)
                        {

                            throw new Exception("Import error: T_CODE: [" + code + "] T_NR:[" + indx + "]", exc);
                        }
                    }

                    PLUGIN.EXEADPCMD(new string[] { docAdp }, new DoWorkEventHandler[] { MY_IMPORT_DOC }, true);


                }
                finally
                {
                    ACTIVE_JOB_ITEM = null;
                    ACTIVE_JOB_ITEMS = null;
                    PARAMETERS = null;
                }
            }

            IMPORT_ITEM ACTIVE_JOB_ITEM;
            bool UPDATE_CARD = true;
            IMPORT_ITEM[] ACTIVE_JOB_ITEMS;
            Dictionary<string, string> PARAMETERS;


            DateTime MY_GET_DATE()
            {
                if (PARAMETERS.ContainsKey("DATE_") && PARAMETERS["DATE_"] != "")
                    return PARSEDATETIME(CHECKDATE(PARAMETERS["DATE_"])).Date;
                return DateTime.Now;
            }


            bool MY_IS_UPDATE()
            {
                if (PARAMETERS.ContainsKey("UPDATE") && PARAMETERS["UPDATE"] != "")
                    return PARAMETERS["UPDATE"] == "1";
                return false;
            }

            public void MY_IMPORT_DOC(object t, DoWorkEventArgs args)
            {

                args.Result = false;


                var DATE_ = MY_GET_DATE();

                DataSet doc_ = ((DataSet)args.Argument);

                DataTable tabHeaderInv_ = TAB_GETTAB(doc_, "INVOICE");
                DataTable tabHeaderSlip_ = TAB_GETTAB(doc_, "STFICHE");
                DataTable tabLine_ = TAB_GETTAB(doc_, "STLINE");


                var tabHeader_ = tabHeaderInv_ != null ? tabHeaderInv_ : tabHeaderSlip_;

                TAB_SETROW(tabHeader_, "SPECODE", "MATIMPORT");
                TAB_SETROW(tabHeader_, "DUMMY_____DATETIME", DATE_);

                foreach (IMPORT_ITEM item_ in ACTIVE_JOB_ITEMS)
                {
                    DataTable MAIN = item_.tables.ContainsKey(mainTable) ? item_.tables[mainTable] : null;
                    DataTable STLINE = item_.tables.ContainsKey("STLINE") ? item_.tables["STLINE"] : null;
                    DataTable PRCLIST = item_.tables.ContainsKey("PRCLIST") ? item_.tables["PRCLIST"] : null;
                    if (MAIN != null && STLINE != null)
                    {

                        var code_ = CASTASSTRING(TAB_GETROW(MAIN, "CODE"));
                        var amount_ = CASTASDOUBLE(TAB_GETROW(STLINE, "AMOUNT"));

                        var price = -1.0;// (docPriceCol != "" ? 0.0 : -1.0);
                        //
                        if (PRCLIST != null)
                        {

                            var colPrice_ = PRCLIST.Columns["PRICE"];
                            var colType_ = PRCLIST.Columns["PTYPE"];

                            if (colPrice_ != null && colType_ != null)
                            {
                                var rec = TAB_SEARCH(PRCLIST, colType_.ColumnName, "1");//PTYPE = 1 Purchase, string value
                                if (rec != null)
                                    price = CASTASDOUBLE(TAB_GETROW(rec, colPrice_.ColumnName));
                            }

                        }

                        if (!ISNUMZERO(amount_))
                        {

                            var mref_ = ((PLUGIN.SQLSCALAR("SELECT LOGICALREF FROM LG_$FIRM$_ITEMS WITH(NOLOCK) WHERE CODE = @P1", new object[] { code_ })));

                            if (ISEMPTYLREF(mref_))
                                throw new Exception("Undefined material ID [" + code_ + "]");

                            DataRow rowLine_ = TAB_ADDROW(tabLine_);
                            //TAB_FILLNULL(tabLine_);
                            TAB_SETROW(rowLine_, "STOCKREF", mref_);
                            TAB_SETROW(rowLine_, "AMOUNT", amount_);

                            if (price >= 0)
                                TAB_SETROW(rowLine_, "PRICE", price);



                        }

                    }
                }



                args.Result = (tabLine_.Rows.Count > 0); //save if has data
            }
            public void MY_IMPORT_ITEM(object t, DoWorkEventArgs args)
            {
                args.Result = false;

                DataSet dataSet = ((DataSet)args.Argument);

                var DATE_ = MY_GET_DATE();

                DataTable MAIN = ACTIVE_JOB_ITEM.tables.ContainsKey(mainTable) ? ACTIVE_JOB_ITEM.tables[mainTable] : null;
                DataTable PRCLIST = ACTIVE_JOB_ITEM.tables.ContainsKey("PRCLIST") ? ACTIVE_JOB_ITEM.tables["PRCLIST"] : null;
                DataTable UNITBARCODE = ACTIVE_JOB_ITEM.tables.ContainsKey("UNITBARCODE") ? ACTIVE_JOB_ITEM.tables["UNITBARCODE"] : null;
                DataTable INVDEF = ACTIVE_JOB_ITEM.tables.ContainsKey("INVDEF") ? ACTIVE_JOB_ITEM.tables["INVDEF"] : null;

                if (MAIN == null)
                    return;

                DataTable MAIND_ = dataSet.Tables[MAIN.TableName];
                if (MAIND_ == null)
                    throw new Exception("Cant find table [" + MAIN.TableName + "]");

                if (UPDATE_CARD)
                    for (int rIndx_ = 0; rIndx_ < MAIN.Rows.Count; ++rIndx_)
                    {
                        DataRow rowS_ = MAIN.Rows[rIndx_];

                        DataRow rowD_ = null;

                        if (rIndx_ < MAIND_.Rows.Count)
                            rowD_ = MAIND_.Rows[rIndx_];
                        else
                            MAIND_.Rows.Add(rowD_ = MAIND_.NewRow());


                        foreach (DataColumn colS_ in MAIN.Columns)
                        {
                            DataColumn colD_ = MAIND_.Columns[colS_.ColumnName];
                            if (colD_ == null)
                                throw new Exception("Cant find column in target[" + colS_.ColumnName + "]");


                            object valObj = null;
                            var rawVal = ISNULL(rowS_[colS_], "0").ToString();
                            try
                            {

                                valObj = PARSE(rawVal, colD_.DataType);
                            }
                            catch (Exception exc)
                            {
                                throw new Exception("Table [" + rowD_.Table.TableName + "]  column [" + colD_.ColumnName + "] value parse error [" + rawVal + "]", exc);
                            }

                            TAB_SETROW(rowD_, colD_.ColumnName, valObj);


                        }
                    }

                if (PRCLIST != null)
                {

                    DataTable PRCLISTD_ = dataSet.Tables[PRCLIST.TableName];
                    if (PRCLISTD_ == null)
                        throw new Exception("Cant find table [" + PRCLIST.TableName + "]");



                    if (PRCLIST.Columns["PTYPE"] == null)
                        throw new Exception("Cant find table [" + PRCLIST.TableName + "] column [PTYPE]");

                    foreach (DataRow rowS_ in PRCLIST.Rows)
                    {

                        var type = CASTASSHORT(TAB_GETROW(rowS_, "PTYPE"));

                        {


                            var beg_ = MY_GET_DATE().Date;//DateTime.Now.Date;
                            var end_ = new DateTime(9999, 1, 1);


                            var rowD_ = PRCLISTD_.NewRow();

                            double val_ = 0;
                            TAB_SETROW(rowD_, "PRICE", val_ = PARSEDOUBLE(ISNULL(rowS_["PRICE"], "0").ToString()));


                            if (!ISNUMZERO(val_))
                            {
                                List<DataRow> lDel = new List<DataRow>();
                                foreach (DataRow row in PRCLISTD_.Rows)
                                    if (!TAB_ROWDELETED(row) && COMPARE(type, TAB_GETROW(row, "PTYPE")))
                                    {
                                        var df_ = CASTASDATE(TAB_GETROW(row, "BEGDATE"));
                                        var dt_ = CASTASDATE(TAB_GETROW(row, "ENDDATE"));

                                        if (df_ <= beg_ && dt_ >= beg_)
                                        {

                                            if (df_ == beg_)
                                                lDel.Add(row);
                                            else
                                                TAB_SETROW(row, "ENDDATE", beg_.AddDays(-1));

                                            end_ = dt_;

                                        }
                                        else
                                            if (beg_ < df_ && end_.Year == 9999)
                                                end_ = df_.AddDays(-1);

                                    }
                                foreach (DataRow row in lDel)
                                    row.Delete();
                            }

                            PRCLISTD_.Rows.Add(rowD_);

                            rowD_["PTYPE"] = type;
                            rowD_["BEGDATE"] = beg_;
                            rowD_["ENDDATE"] = end_;
                        }

                    }









                }

                if (UNITBARCODE != null)
                {

                    DataTable UNITBARCODED_ = dataSet.Tables[UNITBARCODE.TableName];
                    if (UNITBARCODE == null)
                        throw new Exception("Cant find table [" + UNITBARCODE.TableName + "]");

                    var rowS_ = TAB_GETLASTROW(UNITBARCODE);
                    if (rowS_ == null)
                        return;

                    var c1 = UNITBARCODE.Columns["BARCODE"];
                    if (c1 != null)
                    {
                        var val_ = ISNULL(rowS_[c1], "").ToString();
                        if (val_ != "")
                        {
                            var rowD_ = UNITBARCODED_.NewRow();
                            TAB_SETROW(rowD_, "BARCODE", val_); ;
                            UNITBARCODED_.Rows.Add(rowD_);
                        }
                    }
                }



                if (INVDEF != null)
                {

                    DataTable INVDEFD_ = dataSet.Tables[INVDEF.TableName];
                    if (INVDEFD_ == null)
                        throw new Exception("Cant find table [" + INVDEF.TableName + "]");



                    if (INVDEF.Columns["INVENNO"] == null)
                        throw new Exception("Cant find table [" + INVDEF.TableName + "] column [INVENNO]");

                    foreach (DataRow rowS_ in INVDEF.Rows)
                    {

                        var wh = CASTASSHORT(TAB_GETROW(rowS_, "INVENNO"));

                        var rowD_ = TAB_SEARCH(INVDEFD_, "INVENNO", wh);
                        if (rowD_ == null)
                        {
                            rowD_ = INVDEFD_.NewRow();
                            INVDEFD_.Rows.Add(rowD_);

                        }



                        foreach (DataColumn colS_ in INVDEF.Columns)
                        {
                            DataColumn colD_ = INVDEFD_.Columns[colS_.ColumnName];
                            if (colD_ == null)
                                throw new Exception("Cant find column in target [" + colS_.ColumnName + "]");


                            object valObj = null;
                            var rawVal = ISNULL(rowS_[colS_], "0").ToString();
                            try
                            {

                                valObj = PARSE(rawVal, colD_.DataType);
                            }
                            catch (Exception exc)
                            {
                                throw new Exception("Table [" + rowD_.Table.TableName + "]  column [" + colD_.ColumnName + "] value parse error [" + rawVal + "]", exc);
                            }

                            TAB_SETROW(rowD_, colD_.ColumnName, valObj);


                        }






                    }









                }

                args.Result = true;


            }

            Dictionary<string, string> MY_GET_PARAMETERS(DataTable pData)
            {

                Dictionary<string, string> dic_ = new Dictionary<string, string>();

                if (pData == null)
                    return dic_;

                if (pData == null || pData.Rows.Count == 0)
                    throw new Exception("T_MSG_OPERATION_STOPPING" + " (T_MSG_DATA_NO, T_HEADER)");

                object[] arr_ = pData.Rows[0].ItemArray;

                for (int indx_ = 0; indx_ + 1 < arr_.Length; indx_ += 2)
                {
                    string key_ = ISNULL(arr_[indx_], "").ToString().Trim();
                    string val_ = ISNULL(arr_[indx_ + 1], "").ToString().Trim();

                    if (key_ != "")
                    {
                        if (dic_.ContainsKey(key_))
                            throw new Exception("T_MSG_ERROR_DUPLICATE_RECORD (" + key_ + ")");
                        else
                            dic_.Add(key_, val_);
                    }
                }

                MY_CHECK_PRM(dic_);
                return dic_;
            }

            DataTable MY_GET_TABLE(DataTable pData, Dictionary<string, string> pParameters)
            {

                if (pData == null)
                    return null;

                DataTable table_ = pData.Copy();
                if (table_.Rows.Count > 0) //parameters
                    table_.Rows.RemoveAt(0);
                if (table_.Rows.Count > 0) //col desc
                    table_.Rows.RemoveAt(0);

                string tableName_ = pParameters[parmTable];

                List<string> listDelCols_ = new List<string>();
                foreach (DataColumn col_ in table_.Columns)
                {
                    if (MY_IS_DUMMY(col_.ColumnName))
                        listDelCols_.Add(col_.ColumnName);
                }
                foreach (string col_ in listDelCols_)
                    table_.Columns.Remove(col_);
                //
                foreach (DataColumn col_ in table_.Columns)
                {
                    if (!MY_IS_VALID_DS_COL(tableName_, col_.ColumnName))
                        throw new Exception("T_MSG_INVALID_PARAMETER, T_TABLE/T_COLUMN, " + tableName_ + "/" + col_.ColumnName);
                }


                MY_CHECK_TABLE(table_, pParameters);
                //
                return table_;
            }
            void MY_CHECK_PRM(Dictionary<string, string> pPrm)
            {

                pPrm[parmTable] = mainTable;

            }
            void MY_CHECK_TABLE(DataTable pTable, Dictionary<string, string> pParameters)
            {
                string tableName_ = pParameters[parmTable];


                foreach (DataColumn col_ in pTable.Columns)
                    foreach (DataRow row_ in pTable.Rows)
                    {
                        var str = row_[col_] as string; //if string col
                        if (str != null)
                            row_[col_] = str.Trim();
                    }


                foreach (DataColumn col_ in pTable.Columns)
                    if (MY_IS_FLOAT(tableName_, col_.ColumnName))
                        foreach (DataRow row_ in pTable.Rows)
                        {
                            string value_ = row_[col_].ToString();
                            if (value_ == string.Empty)
                                value_ = "0";

                            value_ = value_.Replace(" ", "").Replace(",", ".");

                            try
                            {
                                PARSEDOUBLE(value_);
                            }
                            catch
                            {
                                throw new Exception(
                                string.Format("T_MSG_INVALID_RECODR, T_TABLE/T_COLUMN/T_VALUE, {0}/{1},{2}", tableName_, col_.ColumnName, value_)
                                );
                            }

                            row_[col_] = value_;
                        }

                foreach (DataColumn col_ in pTable.Columns)
                    if (MY_IS_DATE(tableName_, col_.ColumnName))
                        foreach (DataRow row_ in pTable.Rows)
                        {
                            string value_ = row_[col_].ToString();
                            try
                            {
                                if (value_.Length < 10) throw new Exception();
                                value_ = CHECKDATE(value_);
                                PARSEDATETIME(value_);
                            }
                            catch
                            {
                                throw new Exception(
                                string.Format("T_MSG_INVALID_RECODR, T_TABLE/T_COLUMN/T_VALUE, {0}/{1},{2}", tableName_, col_.ColumnName, value_)
                                );
                            }

                            row_[col_] = value_;
                        }

                MY_CHECK_DATA_VALID(pTable, pParameters);

                MY_CHECK_DS_HAS_REQUIRED_DATA(pTable, pParameters);


            }
            void MY_CHECK_DATA_VALID(DataTable pTable, Dictionary<string, string> pParameters)
            {
                foreach (DataColumn col in pTable.Columns)
                {
                    if (col.ColumnName == "CODE")
                    {

                        int indx = 0;
                        foreach (DataRow row in pTable.Rows)
                        {
                            ++indx;
                            var val_ = row[col].ToString();

                            if (val_ == "")
                                throw new Exception("Qeyd ücun kod buraxılıb [Nr=" + indx + "]");
                        }


                    }


                }




            }
            void MY_CHECK_DS_HAS_REQUIRED_DATA(DataTable pTable, Dictionary<string, string> pParameters)
            {


            }
            void MY_CHECK_REPEAT(DataTable pTable, Dictionary<string, string> pParameters)
            {
                /*
         if(pTable == null)
         return;

 
 
         foreach(DataColumn col in pTable.Columns){
         if(col.ColumnName == "CODE" || col.ColumnName == "FICHENO"){
 
        List<string> list = new List<string>();
        foreach(DataRow row in pTable.Rows){
        var val_ = row[col].ToString();

        if(list.Contains(val_))
        throw new Exception("Qeyd təkrarlanması ["+val_+"]");

        list.Add(val_);

        }

 
         }

 
         }  */
            }



            bool MY_IS_DUMMY(string pCol)
            {
                try
                {
                    return (int.Parse(pCol) >= 500);
                }
                catch { }
                return false;
            }

            bool MY_IS_FLOAT(string pTab, string pCol)
            {
                Type t_ = MY_GET_COL_TYPE(pTab, pCol);
                return ((t_ == typeof(double)) || (t_ == typeof(int)) || (t_ == typeof(short)) || (t_ == typeof(decimal)) || (t_ == typeof(float)));
            }

            bool MY_IS_DATE(string pTab, string pCol)
            {
                Type t_ = MY_GET_COL_TYPE(pTab, pCol);
                return (t_ == typeof(DateTime));

            }
            bool MY_IS_STRING(string pTab, string pCol)
            {
                Type t_ = MY_GET_COL_TYPE(pTab, pCol);
                return (t_ == typeof(string));
            }
            Type MY_GET_COL_TYPE(string pTab, string pCol)
            {
                string[] arrCol_ = EXPLODELISTPATH(pCol);
                if (arrCol_.Length > 1)
                    pCol = arrCol_[0] + "_____" + arrCol_[1];

                return PLUGIN.GETCOLUMNTYPE(pTab, pCol);
            }
            bool MY_IS_VALID_DS_COL(string pTab, string pCol)
            {



                try
                {
                    return MY_GET_COL_TYPE(pTab, pCol) != null;
                }
                catch { }

                return false;
            }
            bool MY_IS_EMPTY(object pVal)
            {
                if (pVal == null)
                    return true;

                if (pVal.ToString().Trim() == "")
                    return true;

                if (pVal.ToString().Trim() == "0")
                    return true;

                return false;
            }



        }


        class IMPORT_CLIENT
        {

            //text 70-89
            //date 90-99
            //float 1-69
            //dummy >=500
            //

            //vers 5


            const string parmAdp = "ADP";
            const string parmTable = "TABLE";
            const string dataTableData = "IMPORT_PERSONAL";

            const string mainTable = "CLCARD";
            const string mainAdp = "adp.fin.rec.client";
            string docAdp = "adp.fin.doc.client.14";
            //if empty, doc price will be used 


            bool PRM_FORCE_UPDATE_CARD = true;

            const int specialTopRowsCount = 2;


            _PLUGIN PLUGIN;

            public IMPORT_CLIENT(_PLUGIN pPLUGIN)
            {
                PLUGIN = pPLUGIN;
            }

            public void IMPORT()
            {



                if (!PLUGIN.MSGUSERASK("T_MSG_COMMIT_BEGIN (T_PERSONAL, T_IMPORT)"))
                    return;

                string xlsFile_ = ASKFILE("Excel|*.xls;*.xlsx");

                if (xlsFile_ == null || xlsFile_ == string.Empty)
                    return;

                DataSet ds_ = XLSREAD(xlsFile_);
                if (!ds_.Tables.Contains(dataTableData))
                    throw new Exception("T_MSG_OPERATION_STOPPING" + " (T_MSG_DATA_NO, " + dataTableData + ")");

                MY_CLEAN(ds_.Tables[dataTableData]);


                Dictionary<string, string> dicParametres_ = MY_GET_PARAMETERS(ds_.Tables[dataTableData]); //get header info



                if (dicParametres_.ContainsKey("ADPDOC"))
                    docAdp = dicParametres_["ADPDOC"];


                DataTable dataTable_ = MY_GET_TABLE(ds_.Tables[dataTableData], dicParametres_);


                MY_IMPORT(dicParametres_, dataTable_);



                PLUGIN.MSGUSERINFO("T_MSG_OPERATION_FINISHED (T_PERSONAL, T_IMPORT)");
            }
            void MY_CLEAN(DataTable pData)
            {
                if (pData == null)
                    return;
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
            string CHECKDATE(string pDate)
            {

                string ptrn_ = "1900-01-01 00-00-00";
                string res_ = pDate;
                if (pDate.Length < ptrn_.Length)
                    res_ = pDate + ptrn_.Substring(pDate.Length, ptrn_.Length - pDate.Length);
                return res_;

            }




            class IMPORT_ITEM
            {

                public DataTable first = null;

                public Dictionary<string, DataTable> tables = new Dictionary<string, DataTable>();

                public Dictionary<string, string> parameters;

                public string getAdapterCmd()
                {

                    var type_ = "2";
                    if (parameters.ContainsKey("CARDTYPE") && parameters["CARDTYPE"] != "")
                        type_ = parameters["CARDTYPE"];


                    return mainAdp + "/" + type_;//  parameters[parmAdp];
                }

            }
            IMPORT_ITEM[] MY_CONVERTTOITEMS(Dictionary<string, string> pParameters, DataTable pTable)
            {
                List<IMPORT_ITEM> list_ = new List<IMPORT_ITEM>();


                foreach (DataRow row_ in pTable.Rows)
                {

                    IMPORT_ITEM item_ = new IMPORT_ITEM();

                    item_.parameters = pParameters;


                    DataTable tab_ = pTable.Clone();
                    DataRow newParentRow_ = tab_.Rows.Add(row_.ItemArray);
                    item_.first = item_.tables[tab_.TableName] = (tab_);




                    //split TABLE_____COLUMN to one-to-many

                    List<DataTable> listTabs_ = new List<DataTable>();

                    for (int c = 0; c < tab_.Columns.Count; ++c)
                    {

                        DataColumn col_ = tab_.Columns[c];
                        //
                        // if column like PRCLIST/PTYPE
                        //
                        string[] arrCol_ = EXPLODELISTPATH(col_.ColumnName);
                        if (arrCol_.Length >= 2) //TAB_ISCOLFULLNAME(col_.ColumnName))
                        {

                            string tabName_ = arrCol_[0];// TAB_GETTABFROMFULLNAME(col_.ColumnName);
                            string colName_ = arrCol_[1];//TAB_GETCOLFROMFULLNAME(col_.ColumnName);


                            DataTable newChild_ = null;
                            DataRow newChildLastRow_ = null;

                            //search tab
                            foreach (DataTable t in listTabs_)
                                if (t.TableName == tabName_)
                                    newChild_ = t;

                            if (newChild_ == null) newChild_ = new DataTable(tabName_);

                            //if has col then add new row
                            if (newChild_.Columns[colName_] == null)
                                newChild_.Columns.Add(colName_, typeof(string)); //add col
                            //


                            string colFilterName_ = arrCol_.Length >= 4 ? arrCol_[2] : null;
                            string colFilterVal_ = arrCol_.Length >= 3 ? arrCol_[3] : null; ;


                            var filterByColVal = (colFilterVal_ != null && colFilterName_ != null);
                            var filterByIndex = (colFilterVal_ != null && colFilterName_ == null);

                            int rowIndx_ = filterByIndex ? (PARSEINT(arrCol_[2]) - 1) : 0;

                            if (rowIndx_ < 0)
                                throw new Exception("Row index of [" + col_.ColumnName + "] is incorrect");

                            //

                            if (filterByIndex)
                            {

                                while (newChild_.Rows.Count - 1 < rowIndx_)
                                    newChild_.Rows.Add(newChild_.NewRow()); //new row


                                newChildLastRow_ = newChild_.Rows[rowIndx_];
                            }
                            else
                                if (filterByColVal)
                                {
                                    var type_ = MY_GET_COL_TYPE(tabName_, colFilterName_);
                                    //  var colObj = newChild_.Columns[colFilterName_];

                                    if (type_ == null)
                                        throw new Exception("Table [" + tabName_ + "] hasnt column [" + colFilterName_ + "]");

                                    //if has col then add new row
                                    if (newChild_.Columns[colFilterName_] == null)
                                        newChild_.Columns.Add(colFilterName_, typeof(string)); //add col
                                    //


                                    newChildLastRow_ = TAB_SEARCH(newChild_, colFilterName_, colFilterVal_);
                                    if (newChildLastRow_ == null)
                                    {
                                        newChild_.Rows.Add(newChildLastRow_ = newChild_.NewRow());
                                        TAB_SETROW(newChildLastRow_, colFilterName_, colFilterVal_);
                                    }
                                }
                                else
                                {

                                    //first record
                                    if (newChild_.Rows.Count == 0)
                                        newChild_.Rows.Add(newChild_.NewRow()); //new row
                                    newChildLastRow_ = newChild_.Rows[0];

                                }

                            newChildLastRow_[colName_] = newParentRow_[col_];

                            //remove row
                            tab_.Columns.Remove(col_);
                            --c;
                            //
                            listTabs_.Add(newChild_);
                        }



                    }

                    foreach (DataTable t in listTabs_)
                        TAB_FILLNULL(t);

                    foreach (var t in listTabs_)
                        item_.tables[t.TableName] = (t);

                    list_.Add(item_);

                }

                return list_.ToArray();
            }
            void MY_IMPORT(Dictionary<string, string> pParameters, DataTable pTable)
            {
                MY_CHECK_REPEAT(pTable, pParameters);


                //change to real names
                pTable.TableName = pParameters[parmTable];
                //

                //explode tables by TABNAME_____COLUMN
                //

                IMPORT_ITEM[] items_ = MY_CONVERTTOITEMS(pParameters, pTable);

                PLUGIN.INVOKEINBATCH(MY_IMPORT_EXT, new object[] { items_, pParameters });


            }



            string GET_CMD_SUFIX(IMPORT_ITEM pItem)
            {

                if (pItem.first == null)
                    return "";


                var code_ = CASTASSTRING(TAB_GETROW(pItem.first, "CODE"));

                var ref_ = ((PLUGIN.SQLSCALAR("SELECT LOGICALREF FROM LG_$FIRM$_CLCARD WITH(NOLOCK) WHERE CODE = @P1", new object[] { code_ })));

                if (!ISEMPTYLREF(ref_))
                    return " cmd::edit lref::" + FORMAT(ref_);


                return "";
            }

            void MY_IMPORT_EXT(object t, DoWorkEventArgs vars)
            {

                try
                {
                    object[] objs_ = (object[])vars.Argument;
                    var items_ = ACTIVE_JOB_ITEMS = objs_[0] as IMPORT_ITEM[];
                    var prmts_ = PARAMETERS = objs_[1] as Dictionary<string, string>;

                    foreach (IMPORT_ITEM item_ in items_)
                    {
                        ACTIVE_JOB_ITEM = item_;

                        string sufix_ = GET_CMD_SUFIX(item_);
                        if (sufix_ == "") //add
                            UPDATE_CARD = true;
                        else
                            UPDATE_CARD = false;


                        if (PRM_FORCE_UPDATE_CARD)
                            UPDATE_CARD = true;

                        if (MY_IS_UPDATE())
                            UPDATE_CARD = true;

                        PLUGIN.EXEADPCMD(new string[] { item_.getAdapterCmd() + sufix_ }, new DoWorkEventHandler[] { MY_IMPORT_ITEM }, true);
                    }

                    PLUGIN.EXEADPCMD(new string[] { docAdp }, new DoWorkEventHandler[] { MY_IMPORT_DOC }, true);


                }
                finally
                {
                    ACTIVE_JOB_ITEM = null;
                    ACTIVE_JOB_ITEMS = null;
                    PARAMETERS = null;
                }
            }

            IMPORT_ITEM ACTIVE_JOB_ITEM;
            bool UPDATE_CARD = true;
            IMPORT_ITEM[] ACTIVE_JOB_ITEMS;
            Dictionary<string, string> PARAMETERS;


            DateTime MY_GET_DATE()
            {
                if (PARAMETERS.ContainsKey("DATE_") && PARAMETERS["DATE_"] != "")
                    return PARSEDATETIME(CHECKDATE(PARAMETERS["DATE_"])).Date;
                return DateTime.Now;
            }


            bool MY_IS_UPDATE()
            {
                if (PARAMETERS.ContainsKey("UPDATE") && PARAMETERS["UPDATE"] != "")
                    return PARAMETERS["UPDATE"] == "1";
                return false;
            }

            public void MY_IMPORT_DOC(object t, DoWorkEventArgs args)
            {

                args.Result = false;


                var DATE_ = MY_GET_DATE();

                DataSet doc_ = ((DataSet)args.Argument);

                DataTable tabHeader_ = TAB_GETTAB(doc_, "CLFICHE");

                DataTable tabLine_ = TAB_GETTAB(doc_, "CLFLINE");




                TAB_SETROW(tabHeader_, "SPECODE", "CLIMPORT");
                //opening doc 14
                // TAB_SETROW(tabHeader_,"DUMMY_____DATETIME", DATE_);

                foreach (IMPORT_ITEM item_ in ACTIVE_JOB_ITEMS)
                {
                    DataTable MAIN = item_.tables.ContainsKey(mainTable) ? item_.tables[mainTable] : null;
                    DataTable CLFLINE = item_.tables.ContainsKey("CLFLINE") ? item_.tables["CLFLINE"] : null;

                    if (CLFLINE != null)
                    {

                        var code_ = CASTASSTRING(TAB_GETROW(MAIN, "CODE"));
                        var debit = -1.0;
                        var credit = -1.0;

                        if (CLFLINE != null)
                        {

                            var colAmount_ = CLFLINE.Columns["AMOUNT"];
                            var colSign_ = CLFLINE.Columns["SIGN"];

                            if (colAmount_ != null && colSign_ != null)
                            {
                                {
                                    var rec = TAB_SEARCH(CLFLINE, colSign_.ColumnName, "0");//SIGN = 0 debit, string value
                                    if (rec != null)
                                        debit = CASTASDOUBLE(TAB_GETROW(rec, colAmount_.ColumnName));
                                }

                                {
                                    var rec = TAB_SEARCH(CLFLINE, colSign_.ColumnName, "1");//SIGN = 0 debit, string value
                                    if (rec != null)
                                        credit = CASTASDOUBLE(TAB_GETROW(rec, colAmount_.ColumnName));
                                }

                            }

                        }

                        if (debit > 0 || credit > 0)
                        {

                            var cref_ = ((PLUGIN.SQLSCALAR("SELECT LOGICALREF FROM LG_$FIRM$_CLCARD WITH(NOLOCK) WHERE CODE = @P1", new object[] { code_ })));

                            if (ISEMPTYLREF(cref_))
                                throw new Exception("Personal card not exists [" + code_ + "]");

                            DataRow rowLine_ = TAB_ADDROW(tabLine_);
                            //TAB_FILLNULL(tabLine_);
                            TAB_SETROW(rowLine_, "CLIENTREF", cref_);

                            if (debit >= 0.001)
                                TAB_SETROW(rowLine_, "DUMMY_____DEBIT", debit);
                            if (credit >= 0.001)
                                TAB_SETROW(rowLine_, "DUMMY_____CREDIT", credit);

                        }

                    }
                }



                args.Result = (tabLine_.Rows.Count > 0); //save if has data


            }
            public void MY_IMPORT_ITEM(object t, DoWorkEventArgs args)
            {


                args.Result = false;

                DataSet dataSet = ((DataSet)args.Argument);

                var DATE_ = MY_GET_DATE();

                DataTable MAIN = ACTIVE_JOB_ITEM.tables.ContainsKey(mainTable) ? ACTIVE_JOB_ITEM.tables[mainTable] : null;
                DataTable RECORDFIRM = ACTIVE_JOB_ITEM.tables.ContainsKey("RECORDFIRM") ? ACTIVE_JOB_ITEM.tables["RECORDFIRM"] : null;

                if (MAIN == null)
                    return;

                DataTable MAIND_ = dataSet.Tables[MAIN.TableName];
                if (MAIND_ == null)
                    throw new Exception("Cant find table [" + MAIN.TableName + "]");

                if (UPDATE_CARD)
                    for (int rIndx_ = 0; rIndx_ < MAIN.Rows.Count; ++rIndx_)
                    {
                        DataRow rowS_ = MAIN.Rows[rIndx_];

                        DataRow rowD_ = null;

                        if (rIndx_ < MAIND_.Rows.Count)
                            rowD_ = MAIND_.Rows[rIndx_];
                        else
                            MAIND_.Rows.Add(rowD_ = MAIND_.NewRow());


                        foreach (DataColumn colS_ in MAIN.Columns)
                        {
                            DataColumn colD_ = MAIND_.Columns[colS_.ColumnName];
                            if (colD_ == null)
                                throw new Exception("Cant find column in target[" + colS_.ColumnName + "]");

                            // TAB_SETROW(rowD_, colD_.ColumnName, PARSE(ISNULL(rowS_[colS_], "0").ToString(), colD_.DataType));


                            object valObj = null;
                            var rawVal = ISNULL(rowS_[colS_], "0").ToString();
                            try
                            {

                                valObj = PARSE(rawVal, colD_.DataType);
                            }
                            catch (Exception exc)
                            {
                                throw new Exception("Table [" + rowD_.Table.TableName + "]  column [" + colD_.ColumnName + "] value parse error [" + rawVal + "]", exc);
                            }

                            TAB_SETROW(rowD_, colD_.ColumnName, valObj);

                        }
                    }

                if (RECORDFIRM != null)
                {

                    DataTable RECORDFIRMD_ = dataSet.Tables[RECORDFIRM.TableName];
                    if (RECORDFIRMD_ == null)
                        throw new Exception("Cant find table [" + RECORDFIRM.TableName + "]");




                    var rowD_ = RECORDFIRMD_.NewRow();
                    RECORDFIRMD_.Rows.Add(rowD_);


                    foreach (DataRow rowS_ in RECORDFIRM.Rows)
                        foreach (DataColumn colS_ in RECORDFIRM.Columns)
                        {
                            DataColumn colD_ = RECORDFIRMD_.Columns[colS_.ColumnName];
                            if (colD_ == null)
                                throw new Exception("Cant find column in target[" + colS_.ColumnName + "]");

                            // TAB_SETROW(rowD_, colD_.ColumnName, PARSE(ISNULL(rowS_[colS_], "0").ToString(), colD_.DataType));



                            object valObj = null;
                            var rawVal = ISNULL(rowS_[colS_], "0").ToString();
                            try
                            {

                                valObj = PARSE(rawVal, colD_.DataType);
                            }
                            catch (Exception exc)
                            {
                                throw new Exception("Table [" + rowD_.Table.TableName + "]  column [" + colD_.ColumnName + "] value parse error [" + rawVal + "]", exc);
                            }

                            TAB_SETROW(rowD_, colD_.ColumnName, valObj);

                        }



                }



                args.Result = true;


            }

            Dictionary<string, string> MY_GET_PARAMETERS(DataTable pData)
            {

                Dictionary<string, string> dic_ = new Dictionary<string, string>();

                if (pData == null)
                    return dic_;

                if (pData == null || pData.Rows.Count == 0)
                    throw new Exception("T_MSG_OPERATION_STOPPING" + " (T_MSG_DATA_NO, T_HEADER)");

                object[] arr_ = pData.Rows[0].ItemArray;

                for (int indx_ = 0; indx_ + 1 < arr_.Length; indx_ += 2)
                {
                    string key_ = ISNULL(arr_[indx_], "").ToString().Trim();
                    string val_ = ISNULL(arr_[indx_ + 1], "").ToString().Trim();

                    if (key_ != "")
                    {
                        if (dic_.ContainsKey(key_))
                            throw new Exception("T_MSG_ERROR_DUPLICATE_RECORD (" + key_ + ")");
                        else
                            dic_.Add(key_, val_);
                    }
                }

                MY_CHECK_PRM(dic_);
                return dic_;
            }

            DataTable MY_GET_TABLE(DataTable pData, Dictionary<string, string> pParameters)
            {

                if (pData == null)
                    return null;

                DataTable table_ = pData.Copy();
                if (table_.Rows.Count > 0) //parameters
                    table_.Rows.RemoveAt(0);
                if (table_.Rows.Count > 0) //col desc
                    table_.Rows.RemoveAt(0);

                string tableName_ = pParameters[parmTable];

                List<string> listDelCols_ = new List<string>();
                foreach (DataColumn col_ in table_.Columns)
                {
                    if (MY_IS_DUMMY(col_.ColumnName))
                        listDelCols_.Add(col_.ColumnName);
                }
                foreach (string col_ in listDelCols_)
                    table_.Columns.Remove(col_);
                //
                foreach (DataColumn col_ in table_.Columns)
                {
                    if (!MY_IS_VALID_DS_COL(tableName_, col_.ColumnName))
                        throw new Exception("T_MSG_INVALID_PARAMETER, T_TABLE/T_COLUMN, " + tableName_ + "/" + col_.ColumnName);
                }


                MY_CHECK_TABLE(table_, pParameters);
                //
                return table_;
            }
            void MY_CHECK_PRM(Dictionary<string, string> pPrm)
            {

                pPrm[parmTable] = mainTable;

            }
            void MY_CHECK_TABLE(DataTable pTable, Dictionary<string, string> pParameters)
            {
                string tableName_ = pParameters[parmTable];

                foreach (DataColumn col_ in pTable.Columns)
                    foreach (DataRow row_ in pTable.Rows)
                        row_[col_] = row_[col_].ToString().Trim();

                foreach (DataColumn col_ in pTable.Columns)
                    if (MY_IS_FLOAT(tableName_, col_.ColumnName))
                        foreach (DataRow row_ in pTable.Rows)
                        {
                            string value_ = row_[col_].ToString();
                            if (value_ == string.Empty)
                                value_ = "0";

                            try
                            {
                                PARSEDOUBLE(value_);
                            }
                            catch
                            {
                                throw new Exception(
                                string.Format("T_MSG_INVALID_RECODR, T_TABLE/T_COLUMN/T_VALUE, {0}/{1},{2}", tableName_, col_.ColumnName, value_)
                                );
                            }

                            row_[col_] = value_;
                        }

                foreach (DataColumn col_ in pTable.Columns)
                    if (MY_IS_DATE(tableName_, col_.ColumnName))
                        foreach (DataRow row_ in pTable.Rows)
                        {
                            string value_ = row_[col_].ToString();
                            try
                            {
                                if (value_.Length < 10) throw new Exception();
                                value_ = CHECKDATE(value_);
                                PARSEDATETIME(value_);
                            }
                            catch
                            {
                                throw new Exception(
                                string.Format("T_MSG_INVALID_RECODR, T_TABLE/T_COLUMN/T_VALUE, {0}/{1},{2}", tableName_, col_.ColumnName, value_)
                                );
                            }

                            row_[col_] = value_;
                        }

                MY_CHECK_DATA_VALID(pTable, pParameters);

                MY_CHECK_DS_HAS_REQUIRED_DATA(pTable, pParameters);


            }
            void MY_CHECK_DATA_VALID(DataTable pTable, Dictionary<string, string> pParameters)
            {
                foreach (DataColumn col in pTable.Columns)
                {
                    if (col.ColumnName == "CODE")
                    {

                        int indx = 0;
                        foreach (DataRow row in pTable.Rows)
                        {
                            ++indx;
                            var val_ = row[col].ToString();

                            if (val_ == "")
                                throw new Exception("Qeyd ücun kod buraxılıb [Nr=" + indx + "]");
                        }


                    }


                }




            }
            void MY_CHECK_DS_HAS_REQUIRED_DATA(DataTable pTable, Dictionary<string, string> pParameters)
            {


            }
            void MY_CHECK_REPEAT(DataTable pTable, Dictionary<string, string> pParameters)
            {

            }



            bool MY_IS_DUMMY(string pCol)
            {
                try
                {
                    return (int.Parse(pCol) >= 500);
                }
                catch { }
                return false;
            }

            bool MY_IS_FLOAT(string pTab, string pCol)
            {
                Type t_ = MY_GET_COL_TYPE(pTab, pCol);
                return ((t_ == typeof(double)) || (t_ == typeof(int)) || (t_ == typeof(short)) || (t_ == typeof(decimal)) || (t_ == typeof(float)));
            }

            bool MY_IS_DATE(string pTab, string pCol)
            {
                Type t_ = MY_GET_COL_TYPE(pTab, pCol);
                return (t_ == typeof(DateTime));

            }
            bool MY_IS_STRING(string pTab, string pCol)
            {
                Type t_ = MY_GET_COL_TYPE(pTab, pCol);
                return (t_ == typeof(string));
            }
            Type MY_GET_COL_TYPE(string pTab, string pCol)
            {
                string[] arrCol_ = EXPLODELISTPATH(pCol);
                if (arrCol_.Length > 1)
                    pCol = arrCol_[0] + "_____" + arrCol_[1];

                return PLUGIN.GETCOLUMNTYPE(pTab, pCol);
            }
            bool MY_IS_VALID_DS_COL(string pTab, string pCol)
            {



                try
                {
                    return MY_GET_COL_TYPE(pTab, pCol) != null;
                }
                catch { }

                return false;
            }
            bool MY_IS_EMPTY(object pVal)
            {
                if (pVal == null)
                    return true;

                if (pVal.ToString().Trim() == "")
                    return true;

                if (pVal.ToString().Trim() == "0")
                    return true;

                return false;
            }
        }


        class IMPORT_ACCOUNT
        {

            //text 70-89
            //date 90-99
            //float 1-69
            //dummy >=500
            //

            //vers 5


            const string parmAdp = "ADP";
            const string parmTable = "TABLE";
            const string dataTableData = "IMPORT_ACCOUNT";

            const string mainTable = "EMUHACC";
            const string mainAdp = "adp.gl.rec.acc";
            string docAdp = "adp.gl.doc.slip.1";



            bool PRM_FORCE_UPDATE_CARD = true;

            const int specialTopRowsCount = 2;


            _PLUGIN PLUGIN;

            public IMPORT_ACCOUNT(_PLUGIN pPLUGIN)
            {
                PLUGIN = pPLUGIN;
            }

            public void IMPORT()
            {



                if (!PLUGIN.MSGUSERASK("T_MSG_COMMIT_BEGIN (T_GL - T_ACCOUNT, T_IMPORT)"))
                    return;

                string xlsFile_ = ASKFILE("Excel|*.xls;*.xlsx");

                if (xlsFile_ == null || xlsFile_ == string.Empty)
                    return;

                DataSet ds_ = XLSREAD(xlsFile_);
                if (!ds_.Tables.Contains(dataTableData))
                    throw new Exception("T_MSG_OPERATION_STOPPING" + " (T_MSG_DATA_NO, " + dataTableData + ")");

                MY_CLEAN(ds_.Tables[dataTableData]);


                Dictionary<string, string> dicParametres_ = MY_GET_PARAMETERS(ds_.Tables[dataTableData]); //get header info



                if (dicParametres_.ContainsKey("ADPDOC"))
                    docAdp = dicParametres_["ADPDOC"];


                DataTable dataTable_ = MY_GET_TABLE(ds_.Tables[dataTableData], dicParametres_);


                MY_IMPORT(dicParametres_, dataTable_);



                PLUGIN.MSGUSERINFO("T_MSG_OPERATION_FINISHED (T_GL - T_ACCOUNT, T_IMPORT)");
            }
            void MY_CLEAN(DataTable pData)
            {
                if (pData == null)
                    return;
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
            string CHECKDATE(string pDate)
            {

                string ptrn_ = "1900-01-01 00-00-00";
                string res_ = pDate;
                if (pDate.Length < ptrn_.Length)
                    res_ = pDate + ptrn_.Substring(pDate.Length, ptrn_.Length - pDate.Length);
                return res_;

            }




            class IMPORT_ITEM
            {

                public DataTable first = null;

                public Dictionary<string, DataTable> tables = new Dictionary<string, DataTable>();

                public Dictionary<string, string> parameters;

                public string getAdapterCmd()
                {

                    var type_ = "2";
                    if (parameters.ContainsKey("CARDTYPE") && parameters["CARDTYPE"] != "")
                        type_ = parameters["CARDTYPE"];


                    return mainAdp + "/" + type_;//  parameters[parmAdp];
                }

            }
            IMPORT_ITEM[] MY_CONVERTTOITEMS(Dictionary<string, string> pParameters, DataTable pTable)
            {
                List<IMPORT_ITEM> list_ = new List<IMPORT_ITEM>();


                foreach (DataRow row_ in pTable.Rows)
                {

                    IMPORT_ITEM item_ = new IMPORT_ITEM();

                    item_.parameters = pParameters;


                    DataTable tab_ = pTable.Clone();
                    DataRow newParentRow_ = tab_.Rows.Add(row_.ItemArray);
                    item_.first = item_.tables[tab_.TableName] = (tab_);




                    //split TABLE_____COLUMN to one-to-many

                    List<DataTable> listTabs_ = new List<DataTable>();

                    for (int c = 0; c < tab_.Columns.Count; ++c)
                    {

                        DataColumn col_ = tab_.Columns[c];
                        //
                        // if column like PRCLIST/PTYPE
                        //
                        string[] arrCol_ = EXPLODELISTPATH(col_.ColumnName);
                        if (arrCol_.Length >= 2) //TAB_ISCOLFULLNAME(col_.ColumnName))
                        {

                            string tabName_ = arrCol_[0];// TAB_GETTABFROMFULLNAME(col_.ColumnName);
                            string colName_ = arrCol_[1];//TAB_GETCOLFROMFULLNAME(col_.ColumnName);


                            DataTable newChild_ = null;
                            DataRow newChildLastRow_ = null;

                            //search tab
                            foreach (DataTable t in listTabs_)
                                if (t.TableName == tabName_)
                                    newChild_ = t;

                            if (newChild_ == null) newChild_ = new DataTable(tabName_);

                            //if has col then add new row
                            if (newChild_.Columns[colName_] == null)
                                newChild_.Columns.Add(colName_, typeof(string)); //add col
                            //


                            string colFilterName_ = arrCol_.Length >= 4 ? arrCol_[2] : null;
                            string colFilterVal_ = arrCol_.Length >= 3 ? arrCol_[3] : null; ;


                            var filterByColVal = (colFilterVal_ != null && colFilterName_ != null);
                            var filterByIndex = (colFilterVal_ != null && colFilterName_ == null);

                            int rowIndx_ = filterByIndex ? (PARSEINT(arrCol_[2]) - 1) : 0;

                            if (rowIndx_ < 0)
                                throw new Exception("Row index of [" + col_.ColumnName + "] is incorrect");

                            //

                            if (filterByIndex)
                            {

                                while (newChild_.Rows.Count - 1 < rowIndx_)
                                    newChild_.Rows.Add(newChild_.NewRow()); //new row


                                newChildLastRow_ = newChild_.Rows[rowIndx_];
                            }
                            else
                                if (filterByColVal)
                                {
                                    var type_ = MY_GET_COL_TYPE(tabName_, colFilterName_);
                                    //  var colObj = newChild_.Columns[colFilterName_];

                                    if (type_ == null)
                                        throw new Exception("Table [" + tabName_ + "] hasnt column [" + colFilterName_ + "]");

                                    //if has col then add new row
                                    if (newChild_.Columns[colFilterName_] == null)
                                        newChild_.Columns.Add(colFilterName_, typeof(string)); //add col
                                    //


                                    newChildLastRow_ = TAB_SEARCH(newChild_, colFilterName_, colFilterVal_);
                                    if (newChildLastRow_ == null)
                                    {
                                        newChild_.Rows.Add(newChildLastRow_ = newChild_.NewRow());
                                        TAB_SETROW(newChildLastRow_, colFilterName_, colFilterVal_);
                                    }
                                }
                                else
                                {

                                    //first record
                                    if (newChild_.Rows.Count == 0)
                                        newChild_.Rows.Add(newChild_.NewRow()); //new row
                                    newChildLastRow_ = newChild_.Rows[0];

                                }

                            newChildLastRow_[colName_] = newParentRow_[col_];

                            //remove row
                            tab_.Columns.Remove(col_);
                            --c;
                            //
                            listTabs_.Add(newChild_);
                        }



                    }

                    foreach (DataTable t in listTabs_)
                        TAB_FILLNULL(t);

                    foreach (var t in listTabs_)
                        item_.tables[t.TableName] = (t);

                    list_.Add(item_);

                }

                return list_.ToArray();
            }
            void MY_IMPORT(Dictionary<string, string> pParameters, DataTable pTable)
            {
                MY_CHECK_REPEAT(pTable, pParameters);


                //change to real names
                pTable.TableName = pParameters[parmTable];
                //

                //explode tables by TABNAME_____COLUMN
                //

                IMPORT_ITEM[] items_ = MY_CONVERTTOITEMS(pParameters, pTable);

                PLUGIN.INVOKEINBATCH(MY_IMPORT_EXT, new object[] { items_, pParameters });


            }



            string GET_CMD_SUFIX(IMPORT_ITEM pItem)
            {

                if (pItem.first == null)
                    return "";


                var code_ = CASTASSTRING(TAB_GETROW(pItem.first, "CODE"));

                var ref_ = ((PLUGIN.SQLSCALAR("SELECT LOGICALREF FROM LG_$FIRM$_EMUHACC WITH(NOLOCK) WHERE CODE = @P1", new object[] { code_ })));

                if (!ISEMPTYLREF(ref_))
                    return " cmd::edit lref::" + FORMAT(ref_);


                return "";
            }

            void MY_IMPORT_EXT(object t, DoWorkEventArgs vars)
            {

                try
                {
                    object[] objs_ = (object[])vars.Argument;
                    var items_ = ACTIVE_JOB_ITEMS = objs_[0] as IMPORT_ITEM[];
                    var prmts_ = PARAMETERS = objs_[1] as Dictionary<string, string>;

                    foreach (IMPORT_ITEM item_ in items_)
                    {
                        ACTIVE_JOB_ITEM = item_;

                        string sufix_ = GET_CMD_SUFIX(item_);
                        if (sufix_ == "") //add
                            UPDATE_CARD = true;
                        else
                            UPDATE_CARD = false;


                        if (PRM_FORCE_UPDATE_CARD)
                            UPDATE_CARD = true;

                        if (MY_IS_UPDATE())
                            UPDATE_CARD = true;

                        PLUGIN.EXEADPCMD(new string[] { item_.getAdapterCmd() + sufix_ }, new DoWorkEventHandler[] { MY_IMPORT_ITEM }, true);
                    }

                    PLUGIN.EXEADPCMD(new string[] { docAdp }, new DoWorkEventHandler[] { MY_IMPORT_DOC }, true);


                }
                finally
                {
                    ACTIVE_JOB_ITEM = null;
                    ACTIVE_JOB_ITEMS = null;
                    PARAMETERS = null;
                }
            }

            IMPORT_ITEM ACTIVE_JOB_ITEM;
            bool UPDATE_CARD = true;
            IMPORT_ITEM[] ACTIVE_JOB_ITEMS;
            Dictionary<string, string> PARAMETERS;


            DateTime MY_GET_DATE()
            {
                if (PARAMETERS.ContainsKey("DATE_") && PARAMETERS["DATE_"] != "")
                    return PARSEDATETIME(CHECKDATE(PARAMETERS["DATE_"])).Date;
                return DateTime.Now;
            }


            bool MY_IS_UPDATE()
            {
                if (PARAMETERS.ContainsKey("UPDATE") && PARAMETERS["UPDATE"] != "")
                    return PARAMETERS["UPDATE"] == "1";
                return false;
            }

            public void MY_IMPORT_DOC(object t, DoWorkEventArgs args)
            {

                args.Result = false;


                var DATE_ = MY_GET_DATE();

                DataSet doc_ = ((DataSet)args.Argument);

                DataTable tabHeader_ = TAB_GETTAB(doc_, "EMFICHE");

                DataTable tabLine_ = TAB_GETTAB(doc_, "EMFLINE");




                TAB_SETROW(tabHeader_, "SPECODE", "ACCIMPORT");
                //opening doc 14
                // TAB_SETROW(tabHeader_,"DUMMY_____DATETIME", DATE_);

                foreach (IMPORT_ITEM item_ in ACTIVE_JOB_ITEMS)
                {
                    DataTable MAIN = item_.tables.ContainsKey(mainTable) ? item_.tables[mainTable] : null;
                    DataTable EMFLINE = item_.tables.ContainsKey("EMFLINE") ? item_.tables["EMFLINE"] : null;

                    if (EMFLINE != null)
                    {

                        var code_ = CASTASSTRING(TAB_GETROW(MAIN, "CODE"));
                        var debit = -1.0;
                        var credit = -1.0;

                        if (EMFLINE != null)
                        {

                            var colAmount_ = EMFLINE.Columns["AMOUNT"];
                            var colSign_ = EMFLINE.Columns["SIGN"];

                            if (colAmount_ != null && colSign_ != null)
                            {
                                {
                                    var rec = TAB_SEARCH(EMFLINE, colSign_.ColumnName, "0");//SIGN = 0 debit, string value
                                    if (rec != null)
                                        debit = CASTASDOUBLE(TAB_GETROW(rec, colAmount_.ColumnName));
                                }

                                {
                                    var rec = TAB_SEARCH(EMFLINE, colSign_.ColumnName, "1");//SIGN = 0 debit, string value
                                    if (rec != null)
                                        credit = CASTASDOUBLE(TAB_GETROW(rec, colAmount_.ColumnName));
                                }

                            }

                        }

                        if (debit > 0 || credit > 0)
                        {

                            var cref_ = ((PLUGIN.SQLSCALAR("SELECT LOGICALREF FROM LG_$FIRM$_EMUHACC WITH(NOLOCK) WHERE CODE = @P1", new object[] { code_ })));

                            if (ISEMPTYLREF(cref_))
                                throw new Exception("Account card not exists [" + code_ + "]");

                            DataRow rowLine_ = TAB_ADDROW(tabLine_);
                            //TAB_FILLNULL(tabLine_);
                            TAB_SETROW(rowLine_, "ACCOUNTREF", cref_);

                            if (debit >= 0.001)
                                TAB_SETROW(rowLine_, "DEBIT", debit);
                            if (credit >= 0.001)
                                TAB_SETROW(rowLine_, "CREDIT", credit);

                        }

                    }
                }



                args.Result = (tabLine_.Rows.Count > 0); //save if has data


            }
            public void MY_IMPORT_ITEM(object t, DoWorkEventArgs args)
            {


                args.Result = false;

                DataSet dataSet = ((DataSet)args.Argument);

                var DATE_ = MY_GET_DATE();

                DataTable MAIN = ACTIVE_JOB_ITEM.tables.ContainsKey(mainTable) ? ACTIVE_JOB_ITEM.tables[mainTable] : null;
                DataTable RECORDFIRM = ACTIVE_JOB_ITEM.tables.ContainsKey("RECORDFIRM") ? ACTIVE_JOB_ITEM.tables["RECORDFIRM"] : null;

                if (MAIN == null)
                    return;

                DataTable MAIND_ = dataSet.Tables[MAIN.TableName];
                if (MAIND_ == null)
                    throw new Exception("Cant find table [" + MAIN.TableName + "]");

                if (UPDATE_CARD)
                    for (int rIndx_ = 0; rIndx_ < MAIN.Rows.Count; ++rIndx_)
                    {
                        DataRow rowS_ = MAIN.Rows[rIndx_];

                        DataRow rowD_ = null;

                        if (rIndx_ < MAIND_.Rows.Count)
                            rowD_ = MAIND_.Rows[rIndx_];
                        else
                            MAIND_.Rows.Add(rowD_ = MAIND_.NewRow());


                        foreach (DataColumn colS_ in MAIN.Columns)
                        {
                            DataColumn colD_ = MAIND_.Columns[colS_.ColumnName];
                            if (colD_ == null)
                                throw new Exception("Cant find column in target[" + colS_.ColumnName + "]");

                            // TAB_SETROW(rowD_, colD_.ColumnName, PARSE(ISNULL(rowS_[colS_], "0").ToString(), colD_.DataType));


                            object valObj = null;
                            var rawVal = ISNULL(rowS_[colS_], "0").ToString();
                            try
                            {

                                valObj = PARSE(rawVal, colD_.DataType);
                            }
                            catch (Exception exc)
                            {
                                throw new Exception("Table [" + rowD_.Table.TableName + "]  column [" + colD_.ColumnName + "] value parse error [" + rawVal + "]", exc);
                            }

                            TAB_SETROW(rowD_, colD_.ColumnName, valObj);

                        }
                    }

                if (RECORDFIRM != null)
                {

                    DataTable RECORDFIRMD_ = dataSet.Tables[RECORDFIRM.TableName];
                    if (RECORDFIRMD_ == null)
                        throw new Exception("Cant find table [" + RECORDFIRM.TableName + "]");




                    var rowD_ = RECORDFIRMD_.NewRow();
                    RECORDFIRMD_.Rows.Add(rowD_);


                    foreach (DataRow rowS_ in RECORDFIRM.Rows)
                        foreach (DataColumn colS_ in RECORDFIRM.Columns)
                        {
                            DataColumn colD_ = RECORDFIRMD_.Columns[colS_.ColumnName];
                            if (colD_ == null)
                                throw new Exception("Cant find column in target[" + colS_.ColumnName + "]");

                            // TAB_SETROW(rowD_, colD_.ColumnName, PARSE(ISNULL(rowS_[colS_], "0").ToString(), colD_.DataType));



                            object valObj = null;
                            var rawVal = ISNULL(rowS_[colS_], "0").ToString();
                            try
                            {

                                valObj = PARSE(rawVal, colD_.DataType);
                            }
                            catch (Exception exc)
                            {
                                throw new Exception("Table [" + rowD_.Table.TableName + "]  column [" + colD_.ColumnName + "] value parse error [" + rawVal + "]", exc);
                            }

                            TAB_SETROW(rowD_, colD_.ColumnName, valObj);

                        }



                }



                args.Result = true;


            }

            Dictionary<string, string> MY_GET_PARAMETERS(DataTable pData)
            {

                Dictionary<string, string> dic_ = new Dictionary<string, string>();

                if (pData == null)
                    return dic_;

                if (pData == null || pData.Rows.Count == 0)
                    throw new Exception("T_MSG_OPERATION_STOPPING" + " (T_MSG_DATA_NO, T_HEADER)");

                object[] arr_ = pData.Rows[0].ItemArray;

                for (int indx_ = 0; indx_ + 1 < arr_.Length; indx_ += 2)
                {
                    string key_ = ISNULL(arr_[indx_], "").ToString().Trim();
                    string val_ = ISNULL(arr_[indx_ + 1], "").ToString().Trim();

                    if (key_ != "")
                    {
                        if (dic_.ContainsKey(key_))
                            throw new Exception("T_MSG_ERROR_DUPLICATE_RECORD (" + key_ + ")");
                        else
                            dic_.Add(key_, val_);
                    }
                }

                MY_CHECK_PRM(dic_);
                return dic_;
            }

            DataTable MY_GET_TABLE(DataTable pData, Dictionary<string, string> pParameters)
            {

                if (pData == null)
                    return null;

                DataTable table_ = pData.Copy();
                if (table_.Rows.Count > 0) //parameters
                    table_.Rows.RemoveAt(0);
                if (table_.Rows.Count > 0) //col desc
                    table_.Rows.RemoveAt(0);

                string tableName_ = pParameters[parmTable];

                List<string> listDelCols_ = new List<string>();
                foreach (DataColumn col_ in table_.Columns)
                {
                    if (MY_IS_DUMMY(col_.ColumnName))
                        listDelCols_.Add(col_.ColumnName);
                }
                foreach (string col_ in listDelCols_)
                    table_.Columns.Remove(col_);
                //
                foreach (DataColumn col_ in table_.Columns)
                {
                    if (!MY_IS_VALID_DS_COL(tableName_, col_.ColumnName))
                        throw new Exception("T_MSG_INVALID_PARAMETER, T_TABLE/T_COLUMN, " + tableName_ + "/" + col_.ColumnName);
                }


                MY_CHECK_TABLE(table_, pParameters);
                //
                return table_;
            }
            void MY_CHECK_PRM(Dictionary<string, string> pPrm)
            {

                pPrm[parmTable] = mainTable;

            }
            void MY_CHECK_TABLE(DataTable pTable, Dictionary<string, string> pParameters)
            {
                string tableName_ = pParameters[parmTable];

                foreach (DataColumn col_ in pTable.Columns)
                    foreach (DataRow row_ in pTable.Rows)
                        row_[col_] = row_[col_].ToString().Trim();

                foreach (DataColumn col_ in pTable.Columns)
                    if (MY_IS_FLOAT(tableName_, col_.ColumnName))
                        foreach (DataRow row_ in pTable.Rows)
                        {
                            string value_ = row_[col_].ToString();
                            if (value_ == string.Empty)
                                value_ = "0";

                            try
                            {
                                PARSEDOUBLE(value_);
                            }
                            catch
                            {
                                throw new Exception(
                                string.Format("T_MSG_INVALID_RECODR, T_TABLE/T_COLUMN/T_VALUE, {0}/{1},{2}", tableName_, col_.ColumnName, value_)
                                );
                            }

                            row_[col_] = value_;
                        }

                foreach (DataColumn col_ in pTable.Columns)
                    if (MY_IS_DATE(tableName_, col_.ColumnName))
                        foreach (DataRow row_ in pTable.Rows)
                        {
                            string value_ = row_[col_].ToString();
                            try
                            {
                                if (value_.Length < 10) throw new Exception();
                                value_ = CHECKDATE(value_);
                                PARSEDATETIME(value_);
                            }
                            catch
                            {
                                throw new Exception(
                                string.Format("T_MSG_INVALID_RECODR, T_TABLE/T_COLUMN/T_VALUE, {0}/{1},{2}", tableName_, col_.ColumnName, value_)
                                );
                            }

                            row_[col_] = value_;
                        }

                MY_CHECK_DATA_VALID(pTable, pParameters);

                MY_CHECK_DS_HAS_REQUIRED_DATA(pTable, pParameters);


            }
            void MY_CHECK_DATA_VALID(DataTable pTable, Dictionary<string, string> pParameters)
            {
                foreach (DataColumn col in pTable.Columns)
                {
                    if (col.ColumnName == "CODE")
                    {

                        int indx = 0;
                        foreach (DataRow row in pTable.Rows)
                        {
                            ++indx;
                            var val_ = row[col].ToString();

                            if (val_ == "")
                                throw new Exception("Qeyd ücun kod buraxılıb [Nr=" + indx + "]");
                        }


                    }


                }




            }
            void MY_CHECK_DS_HAS_REQUIRED_DATA(DataTable pTable, Dictionary<string, string> pParameters)
            {


            }
            void MY_CHECK_REPEAT(DataTable pTable, Dictionary<string, string> pParameters)
            {

            }



            bool MY_IS_DUMMY(string pCol)
            {
                try
                {
                    return (int.Parse(pCol) >= 500);
                }
                catch { }
                return false;
            }

            bool MY_IS_FLOAT(string pTab, string pCol)
            {
                Type t_ = MY_GET_COL_TYPE(pTab, pCol);
                return ((t_ == typeof(double)) || (t_ == typeof(int)) || (t_ == typeof(short)) || (t_ == typeof(decimal)) || (t_ == typeof(float)));
            }

            bool MY_IS_DATE(string pTab, string pCol)
            {
                Type t_ = MY_GET_COL_TYPE(pTab, pCol);
                return (t_ == typeof(DateTime));

            }
            bool MY_IS_STRING(string pTab, string pCol)
            {
                Type t_ = MY_GET_COL_TYPE(pTab, pCol);
                return (t_ == typeof(string));
            }
            Type MY_GET_COL_TYPE(string pTab, string pCol)
            {
                string[] arrCol_ = EXPLODELISTPATH(pCol);
                if (arrCol_.Length > 1)
                    pCol = arrCol_[0] + "_____" + arrCol_[1];

                return PLUGIN.GETCOLUMNTYPE(pTab, pCol);
            }
            bool MY_IS_VALID_DS_COL(string pTab, string pCol)
            {



                try
                {
                    return MY_GET_COL_TYPE(pTab, pCol) != null;
                }
                catch { }

                return false;
            }
            bool MY_IS_EMPTY(object pVal)
            {
                if (pVal == null)
                    return true;

                if (pVal.ToString().Trim() == "")
                    return true;

                if (pVal.ToString().Trim() == "0")
                    return true;

                return false;
            }
        }


        class IMPORT_GENERAL
        {

            //text 70-89
            //date 90-99
            //float 1-69
            //dummy >=500
            //


            const string parmAdp = "ADP";
            const string parmTable = "TABLE";
            const string dataTableData = "IMPORT_GENERAL_HEDER";
            const string dataTableLine = "IMPORT_GENERAL_LINES";
            const int specialTopRowsCount = 2;

            _PLUGIN PLUGIN;

            public IMPORT_GENERAL(_PLUGIN pPLUGIN)
            {
                PLUGIN = pPLUGIN;
            }

            public void IMPORT()
            {

                if (!PLUGIN.MSGUSERASK("T_MSG_COMMIT_BEGIN (T_GENERAL, T_IMPORT)"))
                    return;

                string xlsFile_ = ASKFILE("Excel|*.xls;*.xlsx");

                if (xlsFile_ == null || xlsFile_ == string.Empty)
                    return;

                DataSet ds_ = XLSREAD(xlsFile_);
                if (!ds_.Tables.Contains(dataTableData))
                    throw new Exception("T_MSG_OPERATION_STOPPING" + " (T_MSG_DATA_NO, " + dataTableData + ")");


                MY_CLEAN(ds_.Tables[dataTableData]);
                Dictionary<string, string> dicParametres_ = MY_GET_PARAMETERS(ds_.Tables[dataTableData], false); //get header info
                Dictionary<string, string> dicParametresLine_ = null;
                DataTable dataTable_ = MY_GET_TABLE(ds_.Tables[dataTableData], dicParametres_, false);
                DataTable dataTableLine_ = null;

                if (ds_.Tables.Contains(dataTableLine))
                {


                    MY_CLEAN(ds_.Tables[dataTableLine]);
                    dicParametresLine_ = MY_GET_PARAMETERS(ds_.Tables[dataTableLine], true); //get line info
                    dataTableLine_ = MY_GET_TABLE(ds_.Tables[dataTableLine], dicParametresLine_, true);

                    MY_CHECK_DATA_AND_LINE(dicParametres_, dataTable_, dicParametresLine_, dataTableLine_);
                }
                else
                {
                    //  throw new Exception("T_MSG_OPERATION_STOPPING" + " (T_MSG_DATA_NO, " + dataTableLine + ")");
                }


                MY_IMPORT(dicParametres_, dataTable_, dicParametresLine_, dataTableLine_);

                PLUGIN.MSGUSERINFO("T_MSG_OPERATION_FINISHED (T_GENERAL, T_IMPORT)");
            }
            void MY_CLEAN(DataTable pData)
            {

                if (pData == null)
                    return;

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
            string CHECKDATE(string pDate)
            {

                string ptrn_ = "1900-01-01 00-00-00";
                string res_ = pDate;
                if (pDate.Length < ptrn_.Length)
                    res_ = pDate + ptrn_.Substring(pDate.Length, ptrn_.Length - pDate.Length);
                return res_;

            }
            void MY_IMPORT(Dictionary<string, string> pParameters, DataTable pTable, Dictionary<string, string> pParametersLine, DataTable pTableLine)
            {
                MY_CHECK_REPEAT(pTable, pParameters, false);
                MY_CHECK_REPEAT(pTableLine, pParametersLine, true);
                bool hasLineInfo = (pTableLine != null && pTableLine.Rows.Count > 0);
                var adapterCode = pParameters[parmAdp];

                var currIndex = -1;

               // PLUGIN.EXEADPCMD(new string[] { adapterCode }, new DoWorkEventHandler[] { (sender,args)=>{
               //     args.Result = false;
               //     ++currIndex;
               //     if (currIndex >= pTable.Rows.Count)
               //         return;
               //     var recHeader = pTable.Rows[currIndex];
               
                
                
               //  DataSet dataSet = ((DataSet)args.Argument);



               //args.Result = true;
                
                
                
                
                
               // } }, false);
                
                List<string> listCols_ = new List<string>();
                listCols_.Add("$t");
                listCols_.Add(pParameters[parmTable]);
                foreach (DataColumn col_ in pTable.Columns)
                    listCols_.Add(col_.ColumnName);

                string impLineParmsList_ = JOINLISTTAB(new string[] { "$p", "adp", pParameters[parmAdp] });
                string impLineColumnsList_ = JOINLISTTAB(listCols_.ToArray());
                string impLineColumnsListLine_ = null;

                if (hasLineInfo)
                {
                    List<string> listColsLine_ = new List<string>();
                    listColsLine_.Add("$t");
                    listColsLine_.Add(pParametersLine[parmTable]);
                    foreach (DataColumn col_ in pTableLine.Columns)
                        listColsLine_.Add(col_.ColumnName);

                    impLineColumnsListLine_ = JOINLISTTAB(listColsLine_.ToArray());
                }

                var importNr = 0;
                foreach (DataRow row_ in pTable.Rows)
                {
                    ++importNr;
                    StringBuilder bu_ = new StringBuilder();
                    //
                    //
                    bu_.AppendLine("$b");
                    bu_.AppendLine(impLineParmsList_);
                    ////////////////
                    bu_.AppendLine(impLineColumnsList_);

                    List<string> listValues_ = new List<string>();
                    listValues_.Add("$");
                    foreach (DataColumn col_ in pTable.Columns)
                        listValues_.Add(row_[col_].ToString());

                    bu_.AppendLine(JOINLISTTAB(listValues_.ToArray()));
                    ////////////////
                    if (hasLineInfo)
                    {
                        ////////////////
                        bu_.AppendLine(impLineColumnsListLine_);
                        foreach (DataRow rowLine_ in pTableLine.Rows)
                        {
                            List<string> listValuesLine_ = new List<string>();
                            listValuesLine_.Add("$");
                            foreach (DataColumn colLine_ in pTableLine.Columns)
                                listValuesLine_.Add(rowLine_[colLine_].ToString());

                            bu_.AppendLine(JOINLISTTAB(listValuesLine_.ToArray()));
                        }
                        ////////////////

                    }
                    bu_.AppendLine("$e");

                   var ok = PLUGIN.IMPORT(bu_.ToString());

                   if (!ok)
                       EXCEPTIONFORUSER("T_INDEX: " + importNr);
                }



            }

            //public void MY_IMPORT_ITEM(object t, DoWorkEventArgs args)
            //{
            //    args.Result = false;

            //    DataSet dataSet = ((DataSet)args.Argument);



            //    args.Result = true;


            //}

            Dictionary<string, string> MY_GET_PARAMETERS(DataTable pData, bool pIsLine)
            {

                Dictionary<string, string> dic_ = new Dictionary<string, string>();

                if (pData == null || pData.Rows.Count == 0)
                    throw new Exception("T_MSG_OPERATION_STOPPING" + " (T_MSG_DATA_NO, T_HEADER)");

                object[] arr_ = pData.Rows[0].ItemArray;

                for (int indx_ = 0; indx_ + 1 < arr_.Length; indx_ += 2)
                {
                    string key_ = ISNULL(arr_[indx_], "").ToString().Trim();
                    string val_ = ISNULL(arr_[indx_ + 1], "").ToString().Trim();

                    if (key_ != "")
                    {
                        if (dic_.ContainsKey(key_))
                            throw new Exception("T_MSG_ERROR_DUPLICATE_RECORD (" + key_ + ")");
                        else
                            dic_.Add(key_, val_);
                    }
                }

                MY_CHECK_PRM(dic_, pIsLine);
                return dic_;
            }

            DataTable MY_GET_TABLE(DataTable pData, Dictionary<string, string> pParameters, bool pIsLine)
            {
                DataTable table_ = pData.Copy();
                if (table_.Rows.Count > 0) //parameters
                    table_.Rows.RemoveAt(0);
                if (table_.Rows.Count > 0) //col desc
                    table_.Rows.RemoveAt(0);

                string tableName_ = pParameters[parmTable];

                List<string> listDelCols_ = new List<string>();
                foreach (DataColumn col_ in table_.Columns)
                {
                    if (MY_IS_DUMMY(col_.ColumnName))
                        listDelCols_.Add(col_.ColumnName);
                }
                foreach (string col_ in listDelCols_)
                    table_.Columns.Remove(col_);
                //
                foreach (DataColumn col_ in table_.Columns)
                {
                    if (!MY_IS_VALID_DS_COL(tableName_, col_.ColumnName))
                        throw new Exception("T_MSG_INVALID_PARAMETER, T_TABLE/T_COLUMN, " + tableName_ + "/" + col_.ColumnName);
                }


                MY_CHECK_TABLE(table_, pParameters, pIsLine);
                //
                return table_;
            }
            void MY_CHECK_PRM(Dictionary<string, string> pPrm, bool pIsLine)
            {

                if (!pPrm.ContainsKey(parmAdp) && !pIsLine)
                    throw new Exception("T_MSG_SET_REQFIELDS" + " (T_HEADER/" + parmAdp + ")");

                if (!pPrm.ContainsKey(parmTable))
                    throw new Exception("T_MSG_SET_REQFIELDS" + " (T_HEADER/" + parmTable + ")");

            }
            void MY_CHECK_TABLE(DataTable pTable, Dictionary<string, string> pParameters, bool pIsLine)
            {
                string tableName_ = pParameters[parmTable];

                foreach (DataColumn col_ in pTable.Columns)
                    foreach (DataRow row_ in pTable.Rows)
                        row_[col_] = row_[col_].ToString().Trim();

                foreach (DataColumn col_ in pTable.Columns)
                    if (MY_IS_FLOAT(tableName_, col_.ColumnName))
                        foreach (DataRow row_ in pTable.Rows)
                        {
                            string value_ = row_[col_].ToString();
                            if (value_ == string.Empty)
                                value_ = "0";

                            try
                            {
                                PARSEDOUBLE(value_);
                            }
                            catch
                            {
                                throw new Exception(
                                string.Format("T_MSG_INVALID_RECODR, T_TABLE/T_COLUMN/T_VALUE, {0}/{1},{2}", tableName_, col_.ColumnName, value_)
                                );
                            }

                            row_[col_] = value_;
                        }

                foreach (DataColumn col_ in pTable.Columns)
                    if (MY_IS_DATE(tableName_, col_.ColumnName))
                        foreach (DataRow row_ in pTable.Rows)
                        {
                            string value_ = row_[col_].ToString();
                            try
                            {
                                if (value_.Length < 4) throw new Exception();
                                value_ = CHECKDATE(value_);
                                PARSEDATETIME(value_);
                            }
                            catch
                            {
                                throw new Exception(
                                string.Format("T_MSG_INVALID_RECODR, T_TABLE/T_COLUMN/T_VALUE, {0}/{1},{2}", tableName_, col_.ColumnName, value_)
                                );
                            }

                            row_[col_] = value_;
                        }

                MY_CHECK_DATA_VALID(pTable, pParameters, pIsLine);

                MY_CHECK_DS_HAS_REQUIRED_DATA(pTable, pParameters, pIsLine);


            }
            void MY_CHECK_DATA_VALID(DataTable pTable, Dictionary<string, string> pParameters, bool pIsLine)
            {


            }
            void MY_CHECK_DS_HAS_REQUIRED_DATA(DataTable pTable, Dictionary<string, string> pParameters, bool pIsLine)
            {


            }
            void MY_CHECK_REPEAT(DataTable pTable, Dictionary<string, string> pParameters, bool pIsLine)
            {


            }

            void MY_CHECK_DATA_AND_LINE(Dictionary<string, string> pParameters, DataTable pTable, Dictionary<string, string> pParametersLine, DataTable pTableLine)
            {
                if (pTableLine != null)
                    if (pTable.Rows.Count > 1 && pTableLine.Rows.Count > 0)
                        throw new Exception(string.Format("T_MSG_ERROR_INVALID_ARGS_COUNT - {0}[{1}] T_AND {2}[{3}]", pTable.TableName, pTable.Rows.Count, pTableLine.TableName, pTableLine.Rows.Count));
            }

            bool MY_IS_DUMMY(string pCol)
            {
                try
                {
                    return (int.Parse(pCol) >= 500);
                }
                catch { }
                return false;
            }

            bool MY_IS_FLOAT(string pTab, string pCol)
            {
                Type t_ = MY_GET_COL_TYPE(pTab, pCol);
                return ((t_ == typeof(double)) || (t_ == typeof(int)) || (t_ == typeof(short)) || (t_ == typeof(decimal)) || (t_ == typeof(float)));
            }

            bool MY_IS_DATE(string pTab, string pCol)
            {
                Type t_ = MY_GET_COL_TYPE(pTab, pCol);
                return (t_ == typeof(DateTime));

            }
            bool MY_IS_STRING(string pTab, string pCol)
            {
                Type t_ = MY_GET_COL_TYPE(pTab, pCol);
                return (t_ == typeof(string));
            }
            Type MY_GET_COL_TYPE(string pTab, string pCol)
            {
                return PLUGIN.GETCOLUMNTYPE(pTab, pCol);
            }
            bool MY_IS_VALID_DS_COL(string pTab, string pCol)
            {
                try
                {
                    return PLUGIN.GETCOLUMNTYPE(pTab, pCol) != null;
                }
                catch { }

                return false;
            }
            bool MY_IS_EMPTY(object pVal)
            {
                if (pVal == null)
                    return true;

                if (pVal.ToString().Trim() == "")
                    return true;

                if (pVal.ToString().Trim() == "0")
                    return true;

                return false;
            }
        }


        #endregion

        #endregion