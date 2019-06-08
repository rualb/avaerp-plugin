 #line 2
 

 #region PLUGIN_BODY
        const int VERSION = 17;
        const string FILE = "plugin.sys.event.stockdocpromo.pls";


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


                    x.MY_STOCKDOCPROMO_USER = s.MY_STOCKDOCPROMO_USER;

                    //
                    x._USER = PLUGIN.GETSYSPRM_USER();
                    x._FIRM = PLUGIN.GETSYSPRM_FIRM();
                    x._FIRMNAME = PLUGIN.GETSYSPRM_FIRMNAME();
                    x._PERIOD = PLUGIN.GETSYSPRM_PERIOD();




                    _SETTINGS.BUF = x;


                }

                public string MY_STOCKDOCPROMO_USER;


                public short _FIRM;
                public string _FIRMNAME;
                public short _PERIOD;
                public short _USER;


            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC)//, "ava.dbbackup.config")
            {

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active On User")]
            public string MY_STOCKDOCPROMO_USER
            {
                get
                {
                    return (_GET("MY_STOCKDOCPROMO_USER", "1,2"));
                }
                set
                {
                    _SET("MY_STOCKDOCPROMO_USER", value);
                }

            }

            public static bool ISUSEROK(_PLUGIN pPLUGIN)
            {
                return Array.IndexOf<string>(
                     EXPLODELIST(BUF.MY_STOCKDOCPROMO_USER),
                     FORMAT(pPLUGIN.GETSYSPRM_USER())
                     ) >= 0;
            }

        }

        #endregion
        #region TEXT


        const string event_STOCKDOCPROMO_ = "event_stockdocpromo_";
        

        const string event_STOCKDOCPROMO_APPLY = "event_stockdocpromo_apply";


        const string event_STOCKDOCPROMO_ROBOT_ = "event_stockdocpromo_robot_";
        const string event_STOCKDOCPROMO_ROBOT_APPLY = "event_stockdocpromo_robot_apply";

        public class TEXT
        {
            public const string text_DESC = "Stock Doc Promo";

        }
        #endregion

        #region MAIN






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



                var isSls = fn.StartsWith("adp.sls.doc.inv");

                var isSlsOrder = fn.StartsWith("adp.sls.doc.order");


                if (isSls || isSlsOrder)
                {

                    var cPanelBtnSub = CONTROL_SEARCH(FORM, "cPanelBtnSub");

                    if (cPanelBtnSub == null)
                        return;



                    if (isSls)
                        _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_STOCKDOCPROMO_APPLY, LANG("T_PROMOTION"), "promo_16x16");

                    if (isSlsOrder)
                        _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_STOCKDOCPROMO_APPLY, LANG("T_PROMOTION"), "promo_16x16");

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
            //event_STOCKDOCPROMO_APPLY_ROBOT

            object arg1 = null;
            object arg2 = null;
            object arg3 = null;

            string cmd = null;

            try
            {
                arg1 = ARGS.Length > 0 ? ARGS[0] : null;
                arg2 = ARGS.Length > 1 ? ARGS[1] : null;
                arg3 = ARGS.Length > 2 ? ARGS[2] : null;

                string[] list_ = EXPLODELISTPATH(EVENTCODE);
                cmd = list_.Length > 1 ? list_[1].ToLowerInvariant() : "";

                if (!cmd.StartsWith(event_STOCKDOCPROMO_))
                    return;

            }
            catch (Exception exc)
            {
                RUNTIMELOG(exc.ToString());
            }

            if (ISEMPTY(cmd))
                return;

            if (cmd.StartsWith(event_STOCKDOCPROMO_ROBOT_))
            {
                //
                try
                {
                    switch (cmd)
                    {
                        case event_STOCKDOCPROMO_ROBOT_APPLY:
                            {
                                MY_PROMO_APPLY(arg1 as DataSet);
                            }
                            break;
                    }
                }
                catch (Exception exc)
                {
                    LOG(exc);
                }
            }
            else
            {
                //
                try
                {


                    switch (cmd)
                    {

                        case event_STOCKDOCPROMO_APPLY:
                            {
                                DataSet ds = arg1 as DataSet;
                                if (ds == null)
                                {
                                    var form = arg1 as Form;
                                    if (form != null && ISADAPTERFORM(form))
                                    {
                                        ds = GETDATASETFROMADPFORM(form);

                                    }

                                }

                                MY_PROMO_APPLY(ds);

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
        }



        void MY_PROMO_APPLY(DataSet pDs)
        {
            if (pDs == null)
                return;

            var LINES = pDs.Tables["STLINE"];
            var HEADER = pDs.Tables["INVOICE"] ?? pDs.Tables["STFICHE"];

            if (LINES == null || HEADER == null)
                return;


            var PRM = new Dictionary<string, object>();


            var pt = new HANDLER.PluginTool();

            pt._activity = (object pArgs) =>
            {
                object res = null;
                object[] args = pArgs as object[];
                if (args != null && args.Length > 0)
                {
                    string cmd = args[0] as string;
                    switch (cmd)
                    {
                        case "sql":
                            res = SQL(args[1].ToString(), args[2] as object[]);
                            break;
                        case "translate":
                            res = RESOLVESTR("[lang::" + (string)args[1] + "]");
                            break;
                        default:
                            throw new Exception("Not implemented " + cmd);
                    }
                }
                pt._return = res;

            };
            pt._dataSet = pDs;
            //
            //pt.initPrm(PRM);

            //
            pt.changeStline();





        }



        #endregion



        #region TOOLS

        public class HANDLER
        {

            public class CONSTVAL
            {
                public const string PROMO = "#promo";


            }


            public class PluginTool
            {
                #region RequiredFields
                //public bool getDocChanging

                public System.Threading.WaitCallback _activity = null;

                public object _return = null;

                public DataSet _dataSet = null;
                public Exception _exception = null;
                public string _desc = null;
                #endregion


                //public bool isMobile()
                //{
                //    DataColumn col_ = (_dataSet != null && _dataSet.Tables[Tables.ColsStfiche.TABLE] != null) ? _dataSet.Tables[Tables.ColsStfiche.TABLE].Columns[Tables.ColsStfiche.LOGICALREF] : null;

                //    if (col_ != null && col_.DataType == typeof(string))
                //        return true;

                //    if (col_ != null && col_.DataType == typeof(int))
                //        return false;

                //    throw new Exception("Init DataSet for ID column");
                //}

                //public void beginDoc()
                //{




                //    _desc = string.Empty;


                //    if (_dataSet != null)
                //        if (_dataSet.Tables.Contains(Tables.ColsStfiche.TABLE))
                //        {
                //            DataTable tab = _dataSet.Tables[Tables.ColsStfiche.TABLE];
                //            tab.ColumnChanged += new DataColumnChangeEventHandler(tab_ColumnChanged);
                //        }





                //}

                //void tab_ColumnChanged(object sender, DataColumnChangeEventArgs e)
                //{
                //    if (e.Column.ColumnName == Tables.ColsStfiche.GENEXP1)
                //        switch ((string)e.Row[Tables.ColsStfiche.GENEXP1])
                //        {
                //            //case "__cmdrefcl":
                //            //    //check
                //            //    DataRow row1 = callReference(References.client);
                //            //    if (row1 != null)
                //            //        ToolMsg.Show(row1["DEFINITION_"].ToString());
                //            //    break;
                //            //case "__cmdrefma":
                //            //    //check
                //            //    DataRow row2 = callReference(References.material);
                //            //    if (row2 != null)
                //            //        ToolMsg.Show(row2["NAME"].ToString());
                //            //    break;
                //            //case "__cmdrefsl":
                //            //    //check
                //            //    DataRow row3 = callReference(References.docSales);
                //            //    if (row3 != null)
                //            //        ToolMsg.Show(row3[Tables.ColsStfiche.GENEXP1].ToString());
                //            //    break;
                //            //case "__cmdtrs":
                //            //    ToolMsg.Show(translate("T_CODE"));
                //            //    break;
                //            //case "__cmdall":
                //            //    DataTable table = _dataSet.Tables[Tables.ColsStline.TABLE];
                //            //    DataTable tabItems = getTable("LG_$FIRM$_ITEMS");
                //            //    foreach (DataRow row in tabItems.Rows)
                //            //    {
                //            //        DataRow _newRow = table.NewRow();
                //            //        table.Rows.Add(_newRow);
                //            //        _newRow[Tables.ColsStline.STOCKREF] = row[Tables.ColsItems.LOGICALREF];
                //            //        _newRow[Tables.ColsStline.AMOUNT] = 1;
                //            //    }
                //            //    break;

                //            case DocTools.FillDocOrderWh.FILLCMD:
                //                if (isStockOrderWarehouse())
                //                {
                //                    DataTable _table = _dataSet.Tables[Tables.ColsStline.TABLE];
                //                    DataTable _tableHeader = _dataSet.Tables[Tables.ColsStfiche.TABLE];
                //                    DocTools.FillDocOrderWh.done(this, _table, _tableHeader);
                //                }
                //                break;
                //        }
                //}


                //public void changeDoc()
                //{

                //    if (isCashCollection())
                //        changeCash();

                //    if (isStockSlipSale() || isStockSlipReturn() || isStockOrderSale() || isStockOrderReturn())
                //        changeStline();



                //}

                void resetAll()
                {

                    _desc = string.Empty;


                }

                bool isStockOrder()
                {
                    if (_dataSet.Tables.Contains(Tables.ColsStline.TABLE) &&
                        (!_dataSet.Tables[Tables.ColsStline.TABLE].Columns.Contains(Tables.ColsStline.ORFLINEREF)))
                        return true;
                    return false;
                }
                bool isStockOrderSale()
                {
                    if (isStockOrder() && CASTASINT(_dataSet.Tables[Tables.ColsStfiche.TABLE].Rows[0][Tables.ColsStfiche.TRCODE]) == 8)
                        return true;
                    return false;
                }
                bool isStockOrderReturn()
                {
                    if (isStockOrder() && CASTASINT(_dataSet.Tables[Tables.ColsStfiche.TABLE].Rows[0][Tables.ColsStfiche.TRCODE]) == 3)
                        return true;
                    return false;
                }
                bool isStockOrderWarehouse()
                {
                    if (isStockOrder() && CASTASINT(_dataSet.Tables[Tables.ColsStfiche.TABLE].Rows[0][Tables.ColsStfiche.TRCODE]) == 15)
                        return true;
                    return false;
                }
                bool isStockSlip()
                {
                    if (_dataSet.Tables.Contains(Tables.ColsStline.TABLE) &&
                        (_dataSet.Tables[Tables.ColsStline.TABLE].Columns.Contains(Tables.ColsStline.ORFLINEREF)))
                        return true;
                    return false;
                }
                bool isStockSlipSale()
                {
                    if (isStockSlip() && CASTASINT(_dataSet.Tables[Tables.ColsStfiche.TABLE].Rows[0][Tables.ColsStfiche.TRCODE]) == 8)
                        return true;
                    return false;
                }
                bool isStockSlipReturn()
                {
                    if (isStockSlip() && CASTASINT(_dataSet.Tables[Tables.ColsStfiche.TABLE].Rows[0][Tables.ColsStfiche.TRCODE]) == 3)
                        return true;
                    return false;
                }
                bool isCash()
                {
                    if (_dataSet.Tables.Contains(Tables.ColsKslines.TABLE))
                        return true;
                    return false;
                }
                bool isCashCollection()
                {
                    if (isCash() && CASTASINT(_dataSet.Tables[Tables.ColsKslines.TABLE].Rows[0][Tables.ColsKslines.TRCODE]) == 11)
                        return true;
                    return false;
                }

                public static void stockDocClearPromoScripted(DataTable _table)
                {
                    List<DataRow> list = new List<DataRow>();
                    foreach (DataRow _row in _table.Rows)
                    {
                        //delete only disc or promo, real mat row can be scripted to
                        if (
                            _row.RowState != DataRowState.Deleted &&
                            (DocTools.ToolStline.isLinePromo(_row) || DocTools.ToolStline.isLineDiscount(_row)) &&
                            DocTools.ToolStline.isLineScripted(_row))
                            list.Add(_row);
                    }

                    foreach (DataRow _row in list)
                        _row.Delete();
                }


                public void changeStline()
                {


                    resetAll();

                    DataTable _table = _dataSet.Tables[Tables.ColsStline.TABLE];
                    DataTable _tableHeader = _dataSet.Tables[Tables.ColsStfiche.TABLE];

                    stockDocClearPromoScripted(_table);

                    bool ok = false;
                    try
                    {

                        //

                        DocTools.ClassDiscByClientDiscPerc.done(this, _tableHeader, _table);
                        // DocTools.ClassPriceLine.done(this, _tableHeader, _table);
                        // DocTools.ClassDiscLine.done(this, _tableHeader, _table);

                        //DocTools.ClassDiscHeaderByCell.done(this, _tableHeader, _table);

                        //
                        DocTools.ClassPromoLine.done(this, _tableHeader, _table);
                        //DocTools.ClassPromoGroup.done(this, _table, _tableHeader);
                        //DocTools.ClassPromoGroupMinAmount.done(this, _table, _tableHeader);
                        //DocTools.ClassPromoGroupCoif.done(this, _table, _tableHeader);
                        //DocTools.ClassPromoGroupSets.done(this, _table, _tableHeader);



                        //delete promo
                        //DocTools.ClassPromoDelete.done(this, _table, _tableHeader);
                        //


                        ////
                        //if (isMobile())
                        //{
                        //    if (isStockSlipSale() || isStockOrderSale())
                        //        DocTools.ClassBalanceLimitCtrl.done(this, _tableHeader);
                        //}

                        if (_tableHeader.Columns["GENEXP4"] != null)
                        {
                            var genexp4 = CASTASSTRING(TAB_GETROW(_tableHeader, "GENEXP4"));
                            if (!genexp4.Contains(CONSTVAL.PROMO))
                                genexp4 = genexp4 + (genexp4.Length > 0 ? " " : "") + CONSTVAL.PROMO;
                            TAB_SETROW(_tableHeader, "GENEXP4", genexp4);
                        }

                        ok = true;
                    }
                    finally
                    {

                        if (!ok)
                            stockDocClearPromoScripted(_table);
                    }
                }

                public void changeCash()
                {

                    resetAll();
                    //
                    DataTable _table = _dataSet.Tables[Tables.ColsKslines.TABLE];
                    DataRow _row = null;
                    if (_table.Rows.Count > 0)
                        _row = _table.Rows[0];
                    if (_row != null)
                    {

                        string clRef = (string)_row[Tables.ColsKslines.CLIENTREF];
                        DateTime date = (DateTime)_row[Tables.ColsKslines.DATE_];
                        DateTime dateFrom = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                        DateTime dateTo = new DateTime(date.Year, date.Month, date.Day, 23, 23, 59);

                        double amount = CASTASDOUBLE(_row[Tables.ColsKslines.AMOUNT]);
                        if (amount < 0.0000001)
                            throw new Exception("T_MSG_INVALID_AMOUNT");

                        {


                        }


                        object val_ = SQLSCALAR(
                             "select 1 from LG_$FIRM$_$PERIOD$_KSLINES where CLIENTREF = @P1 and DATE_ between @P2 and @P3 and abs(AMOUNT - @P4) < 0.001",
                             new object[] { clRef, dateFrom, dateTo, amount });

                        if (val_ != null)
                            throw new Exception("T_MSG_DOC_REPEAT");

                        //DataTable tabKslines = getTable("LG_$FIRM$_$PERIOD$_KSLINES");
                        //if (tabKslines != null)
                        //{


                        //    foreach (DataRow docRow in tabKslines.Rows)
                        //    {
                        //        short cancelled = (short)docRow[Tables.ColsKslines.CANCELLED];
                        //        if (cancelled == 0)
                        //        {
                        //            string clRefDoc = (string)docRow[Tables.ColsKslines.CLIENTREF];
                        //            DateTime dateDoc = (DateTime)docRow[Tables.ColsKslines.DATE_];
                        //            double amountDoc = (double)docRow[Tables.ColsKslines.AMOUNT];

                        //            if ((clRefDoc == clRef) && (dateDoc == date) && (Math.Abs(amountDoc - amount) < 0.0000001))
                        //                throw new Exception("T_MSG_DOC_REPEAT");

                        //        }
                        //    }
                        //}
                    }


                }






                public object SQLSCALAR(string sql, object[] parameters)
                {
                    DataTable table = SQL(sql, parameters);
                    if (table.Rows.Count > 0 && table.Columns.Count > 0)
                        return table.Rows[0][0];
                    return null;
                }

                public DataTable SQL(string sql, object[] parameters)
                {
                    object[] _args = new object[] { "sql", sql, parameters };
                    _activity.Invoke(_args);
                    return (DataTable)_return;

                }
                //check
                public DataRow callReference(string code)
                {
                    object[] _args = new object[] { "ref", code };
                    _activity.Invoke(_args);
                    return (DataRow)_return;
                }
                public string translate(string txt)
                {
                    object[] _args = new object[] { "translate", txt };
                    _activity.Invoke(_args);
                    return (string)_return;
                }



                //IDictionary<object, DataTable> _tablesCache = new Dictionary<object, DataTable>();

                //public DataTable getTable(string pObject)
                //{
                //    DataTable objData = null;
                //    if (_tablesCache.ContainsKey(pObject))
                //        objData = _tablesCache[pObject];
                //    else
                //        _tablesCache.Add(
                //            pObject,
                //            objData = SQL("SELECT * FROM " + pObject, new object[] { })
                //            );
                //    return objData;
                //}





                public CACHE_DOCSAVE cacheDocSave = new CACHE_DOCSAVE();
                public CACHE_ITEMS cacheMaterial = new CACHE_ITEMS();
                public CACHE_CLIENT cacheClient = new CACHE_CLIENT();

                //public void initPrm(Dictionary<string, object> PRM)
                //{
                //    foreach (string key in PRM.Keys)
                //    {

                //        switch (key.ToLowerInvariant())
                //        {
                //            case "promocardtable":
                //                _DocSavingTable = PRM[key] as DataTable;
                //                break;
                //            case "promocardcyphcode":
                //                _DocSavingTableCyphCode = PRM[key] as string;
                //                break;
                //            case "promocardcode":
                //                _DocSavingTableCode = PRM[key] as string;
                //                break;
                //        }


                //    }
                //}
            }

            public class ToolClient
            {
                public static int getSign(DataRow pRow, DataRowVersion pVers)
                {
                    if (pRow.Table.TableName == Tables.ColsStfiche.TABLE)
                    {
                        int trcode = CASTASINT(pRow[Tables.ColsStfiche.TRCODE, pVers]);
                        int cancelled = CASTASINT(pRow[Tables.ColsStfiche.CANCELLED, pVers]);
                        if (cancelled == 0)
                        {
                            switch (CASTASINT(trcode))
                            {
                                case 8:
                                    return +1;
                                case 3:
                                    return -1;
                            }
                        }
                    }
                    else
                        if (pRow.Table.TableName == Tables.ColsKslines.TABLE)
                        {
                            int trcode = CASTASINT(pRow[Tables.ColsKslines.TRCODE, pVers]);
                            int cancelled = CASTASINT(pRow[Tables.ColsKslines.CANCELLED, pVers]);
                            if (cancelled == 0)
                            {
                                switch (CASTASINT(trcode))
                                {
                                    case 11:
                                        return -1;
                                    case 12:
                                        return +1;
                                }
                            }
                        }

                    return 0;
                }
                public static double getTot(DataRow pRow, DataRowVersion pVers)
                {
                    if (pRow.Table.TableName == Tables.ColsStfiche.TABLE)
                        return CASTASDOUBLE(pRow[Tables.ColsStfiche.NETTOTAL, pVers]);
                    else
                        if (pRow.Table.TableName == Tables.ColsKslines.TABLE)
                            return CASTASDOUBLE(pRow[Tables.ColsKslines.AMOUNT, pVers]);

                    return 0;
                }
                public static object getClRef(DataRow pRow, DataRowVersion pVers)
                {
                    if (pRow.Table.TableName == Tables.ColsStfiche.TABLE)
                        return pRow[Tables.ColsStfiche.CLIENTREF, pVers];
                    else
                        if (pRow.Table.TableName == Tables.ColsKslines.TABLE)
                            return pRow[Tables.ColsKslines.CLIENTREF, pVers];
                    return 0;
                }
                public static double getTotalDebit(PluginTool pRoot, object pRefCl)
                {
                    return getTotalDebitCash(pRoot, pRefCl) + getTotalDebitSale(pRoot, pRefCl) + getTotalDebitOrder(pRoot, pRefCl);
                }
                public static double getTotalCredit(PluginTool pRoot, object pRefCl)
                {
                    return getTotalCreditCash(pRoot, pRefCl) + getTotalCreditSale(pRoot, pRefCl) + getTotalCreditOrder(pRoot, pRefCl);
                }


                public static double getTotalDebitCash(PluginTool pRoot, object pRefCl)
                {
                    return checkDbReturn(pRoot.SQLSCALAR("select sum(DOC.AMOUNT) TOTAL from LG_$FIRM$_$PERIOD$_KSLINES DOC	where	DOC.TRCODE = 12 and	DOC.CANCELLED = 0 and	DOC.CLIENTREF = @P1 ", new object[] { pRefCl }));
                }
                public static double getTotalDebitSale(PluginTool pRoot, object pRefCl)
                {
                    return checkDbReturn(pRoot.SQLSCALAR("select	sum(DOC.NETTOTAL) TOTAL	from LG_$FIRM$_$PERIOD$_INVOICE DOC	where	DOC.TRCODE = 8 and	DOC.CANCELLED = 0 and	DOC.CLIENTREF = @P1", new object[] { pRefCl }));
                }
                public static double getTotalDebitOrder(PluginTool pRoot, object pRefCl)
                {
                    return checkDbReturn(pRoot.SQLSCALAR("select	sum(DOC.NETTOTAL) TOTAL	from LG_$FIRM$_$PERIOD$_ORFICHE DOC	where	DOC.TRCODE = 8 and	DOC.CANCELLED = 0 and	DOC.CLIENTREF = @P1", new object[] { pRefCl }));
                }
                public static double getTotalCreditCash(PluginTool pRoot, object pRefCl)
                {
                    return checkDbReturn(pRoot.SQLSCALAR("select sum(DOC.AMOUNT) TOTAL from	LG_$FIRM$_$PERIOD$_KSLINES DOC	where	DOC.TRCODE = 11 and	DOC.CANCELLED = 0 and	DOC.CLIENTREF = @P1 ", new object[] { pRefCl }));
                }
                public static double getTotalCreditSale(PluginTool pRoot, object pRefCl)
                {
                    return checkDbReturn(pRoot.SQLSCALAR("select	sum(DOC.NETTOTAL) TOTAL	from LG_$FIRM$_$PERIOD$_INVOICE DOC	where	DOC.TRCODE = 3 and	DOC.CANCELLED = 0 and	DOC.CLIENTREF = @P1", new object[] { pRefCl }));
                }
                public static double getTotalCreditOrder(PluginTool pRoot, object pRefCl)
                {
                    return checkDbReturn(pRoot.SQLSCALAR("select	sum(DOC.NETTOTAL) TOTAL	from LG_$FIRM$_$PERIOD$_ORFICHE DOC	where	DOC.TRCODE = 3 and	DOC.CANCELLED = 0 and	DOC.CLIENTREF = @P1", new object[] { pRefCl }));
                }

                static double checkDbReturn(object pVal)
                {
                    if (
                        pVal == null ||
                        pVal.GetType() == DBNull.Value.GetType()
                        )
                        return 0;
                    return CASTASDOUBLE(pVal);
                }
            }

            public class Tables
            {
                public class ColsClient
                {
                    public const string LOGICALREF = "LOGICALREF";
                    public const string FILTERPROMOCL = "FILTERPROMOCL";
                    public const string BALANCELIMIT = "BALANCELIMIT";
                    public const string BALANCE = "BALANCE";
                    public const string DEFINITION_ = "DEFINITION_";
                    public const string CODE = "CODE";
                    public const string DISCPER = "DISCPER";
                }
                public class ColsDocSaving
                {
                    public const string LOGICALREF = "LOGICALREF";
                    public const string CODE = "CODE";
                    public const string TYPE = "TYPE";
                    public const string PROMOMATCODE = "PROMOMATCODE";
                    public const string CF1 = "CF1";
                    public const string CF2 = "CF2";
                    public const string CLCODE = "CLCODE";
                    public const string TEXT1 = "TEXT1";
                    public const string TEXT2 = "TEXT2";
                    public const string FILTERPROMOCL = "FILTERPROMOCL";

                }
                public class ColsItems
                {
                    public const string LOGICALREF = "LOGICALREF";
                    public const string STGRPCODETOP = "STGRPCODETOP";
                    public const string STGRPCODESUB = "STGRPCODESUB";
                    public const string CODE = "CODE";
                    public const string NAME = "NAME";
                    public const string ONHAND = "ONHAND";
                    public const string ONMAIN = "ONMAIN";
                    public const string PROMO = "PROMO";
                }
                public class ColsKslines
                {
                    public const string TABLE = "KSLINES";
                    public const string TRCODE = "TRCODE";
                    public const string AMOUNT = "AMOUNT";
                    public const string DATE_ = "DATE_";
                    public const string CLIENTREF = "CLIENTREF";
                    public const string CANCELLED = "CANCELLED";
                }
                public class ColsStfiche
                {
                    public const string LOGICALREF = "LOGICALREF";
                    public const string TABLE = "INVOICE";
                    public const string CLIENTREF = "CLIENTREF";
                    public const string TRCODE = "TRCODE";
                    public const string ORFLINEREF = "ORFLINEREF";
                    public const string GENEXP1 = "GENEXP1";
                    public const string GENEXP2 = "GENEXP2";
                    public const string GENEXP3 = "GENEXP3";
                    public const string GENEXP4 = "GENEXP4";
                    public const string DATE_ = "DATE_";
                    public const string DISCPER = "DISCPER";
                    public const string NETTOTAL = "NETTOTAL";
                    public const string CANCELLED = "CANCELLED";
                    public const string FICHENO = "FICHENO";
                }
                public class ColsStline
                {
                    public const string TABLE = "STLINE";
                    public const string STOCKREF = "STOCKREF";
                    public const string AMOUNT = "AMOUNT";
                    public const string PRICE = "PRICE";
                    public const string LINETYPE = "LINETYPE";
                    public const string GLOBTRANS = "GLOBTRANS";
                    public const string TRCODE = "TRCODE";
                    public const string ORFLINEREF = "ORFLINEREF";
                    public const string DISCPER = "DISCPER";

                    // public const string SCRIPTFLG = "SCRIPTFLG";
                    public const string SCRIPTEXP = "SCRIPTEXP";
                    public const string SPECODE = "SPECODE";

                    public const string UOMREF = "UOMREF";
                    public const string LINEEXP = "LINEEXP";

                }
            }

            public class DocTools
            {
                public class ClassDiscByClientDiscPerc
                {
                    const string _code = "cl_direct_disc";
                    public static void done(PluginTool pRoot, DataTable pStfiche, DataTable pLines)
                    {
                        //if (pRoot.isMobile())
                        //    return;

                        DataRow header_ = TAB_GETLASTROW(pStfiche);

                        if (!ToolStline.ignoreDoc(pRoot, header_))
                        {
                            applyItemPromo(pRoot, pStfiche, pLines, header_);
                        }
                    }

                    static void applyItemPromo(PluginTool pRoot, DataTable pHeader, DataTable pLines, DataRow pHeaderRow)
                    {
                        //if doc has manual added doc disc then dont apply cl disc

                        var bottomDisc = CASTASDOUBLE(TAB_GETROW(pHeaderRow, "ADDDISCOUNTS"));
                        if (bottomDisc > 0.001)
                            return;

                        object clientref_ = pHeaderRow[Tables.ColsStfiche.CLIENTREF];
                        DataRow row_ = pRoot.cacheClient.getClientRecord(pRoot, clientref_);
                        if (row_ == null)
                            return;

                        double pDiscount = CASTASDOUBLE(row_[Tables.ColsClient.DISCPER]);


                        if (

                           (pDiscount > 0.0000001 && pDiscount <= 100)
                        )
                        {
                            addDiscount(pRoot, pHeaderRow, pLines, pDiscount);
                        }

                    }


                    static void addDiscount(PluginTool pRoot, DataRow pHeaderRow, DataTable pLines, double pDiscount)
                    {


                        int indx_ = ToolStline.globalFirstPosition(pLines);

                        DataRow row_ = pLines.NewRow();
                        row_[Tables.ColsStline.LINETYPE] = 2; //discount
                        row_[Tables.ColsStline.GLOBTRANS] = 1; //global

                        pLines.Rows.InsertAt(row_, indx_);

                        row_[Tables.ColsStline.DISCPER] = pDiscount;
                        ToolStline.setLineScripted(row_, _code);


                    }
                    static DataRow[] getItemPromoInfo(DataTable promoInfo)
                    {
                        List<DataRow> list_ = new List<DataRow>();

                        for (int i = 0; i < promoInfo.Rows.Count; ++i)
                        {
                            DataRow row = promoInfo.Rows[i];
                            if (CASTASSTRING(row[Tables.ColsDocSaving.TYPE]) == _code)
                                list_.Add(row);
                        }

                        return list_.ToArray();
                    }
                }

                public class ClassDiscGlobalByColumnValue
                {
                    const string _code = "disc_by_col_val";
                    public static void done(PluginTool pRoot, DataTable pStfiche, DataTable pLines)
                    {
                        var tableDocSaving = pRoot.cacheDocSave.getDocSavingTable(pRoot);
                        DataRow header_ = TAB_GETLASTROW(pStfiche);

                        if (!ToolStline.ignoreDoc(pRoot, header_))
                        {
                            foreach (DataRow promoInfo_ in getItemPromoInfo(tableDocSaving))
                                if (ClassFilterPromoByClient.allowed(pRoot, header_, promoInfo_))
                                    applyItemPromo(pRoot, pStfiche, pLines, header_, promoInfo_);
                        }
                    }

                    static void applyItemPromo(PluginTool pRoot, DataTable pHeader, DataTable pLines, DataRow pHeaderRow, DataRow pPromoInfo)
                    {
                        if (pPromoInfo == null)
                            return;

                        double cf1 = CASTASDOUBLE(pPromoInfo[Tables.ColsDocSaving.CF1]);

                        string column_ = CASTASSTRING(pPromoInfo[Tables.ColsDocSaving.TEXT1]);
                        string value_ = CASTASSTRING(pPromoInfo[Tables.ColsDocSaving.TEXT2]);

                        if (
                            pHeader.Columns.Contains(column_) &&
                           (pHeaderRow[column_].ToString().ToLower() == value_.ToLower()) &&
                           (cf1 > 0.0000001 && cf1 <= 100)
                        )
                        {
                            addDiscount(pRoot, pHeaderRow, pLines, cf1, pPromoInfo);
                            return; //ignore next discounts
                        }

                    }


                    static void addDiscount(PluginTool pRoot, DataRow pHeaderRow, DataTable pLines, double pDiscount, DataRow pPromoInfo)
                    {
                        //if (pRoot.isMobile())
                        //{
                        //    pHeaderRow[Tables.ColsStfiche.DISCPER] = pDiscount;
                        //    ToolStline.setLineScripted(pHeaderRow, _code);
                        //}
                        //else
                        {


                            int indx_ = ToolStline.globalFirstPosition(pLines);

                            DataRow row_ = pLines.NewRow();
                            row_[Tables.ColsStline.LINETYPE] = 2; //discount
                            row_[Tables.ColsStline.GLOBTRANS] = 1; //global

                            pLines.Rows.InsertAt(row_, indx_);

                            row_[Tables.ColsStline.DISCPER] = pDiscount;
                            ToolStline.setLineScripted(row_, pPromoInfo);
                        }
                    }
                    static DataRow[] getItemPromoInfo(DataTable promoInfo)
                    {
                        List<DataRow> list_ = new List<DataRow>();

                        for (int i = 0; i < promoInfo.Rows.Count; ++i)
                        {
                            DataRow row = promoInfo.Rows[i];
                            if (CASTASSTRING(row[Tables.ColsDocSaving.TYPE]) == _code)
                                list_.Add(row);
                        }

                        return list_.ToArray();
                    }
                }

                public class ClassBalanceLimitCtrl
                {
                    public static void done(PluginTool pRoot, DataTable pTabHeader)
                    {
                        Dictionary<object, double> dic_ = new Dictionary<object, double>();
                        object defLRef = 0;
                        //
                        foreach (DataRow row_ in pTabHeader.Rows)
                        {
                            double newTotal = 0;
                            object newRefCl = defLRef;
                            double oldTotal = 0;
                            object oldRefCl = defLRef;

                            switch (row_.RowState)
                            {
                                case DataRowState.Added:
                                    newTotal = +1 * ToolClient.getSign(row_, DataRowVersion.Current) * ToolClient.getTot(row_, DataRowVersion.Current);
                                    newRefCl = ToolClient.getClRef(row_, DataRowVersion.Current);
                                    break;
                                case DataRowState.Deleted:
                                    oldTotal = -1 * ToolClient.getSign(row_, DataRowVersion.Original) * ToolClient.getTot(row_, DataRowVersion.Original);
                                    oldRefCl = ToolClient.getClRef(row_, DataRowVersion.Original);
                                    break;
                                case DataRowState.Modified:
                                    newTotal = +1 * ToolClient.getSign(row_, DataRowVersion.Current) * ToolClient.getTot(row_, DataRowVersion.Current);
                                    newRefCl = ToolClient.getClRef(row_, DataRowVersion.Current);
                                    //
                                    oldTotal = -1 * ToolClient.getSign(row_, DataRowVersion.Original) * ToolClient.getTot(row_, DataRowVersion.Original);
                                    oldRefCl = ToolClient.getClRef(row_, DataRowVersion.Original);
                                    break;

                            }
                            addToDic(dic_, newRefCl, newTotal);
                            addToDic(dic_, oldRefCl, oldTotal);
                        }
                        dic_.Remove(defLRef);

                        foreach (object key_ in dic_.Keys)
                        {
                            double total_ = dic_[key_];
                            object refCl_ = key_;

                            if (total_ > 0.0000001)
                            {

                                DataRow clRecord_ = pRoot.cacheClient.getClientRecord(pRoot, refCl_);

                                if (clRecord_ != null && clRecord_.Table.Columns.Contains(Tables.ColsClient.BALANCELIMIT))
                                {
                                    string strDesc = clRecord_[Tables.ColsClient.DEFINITION_].ToString();
                                    double limitValue_ = CASTASDOUBLE(clRecord_[Tables.ColsClient.BALANCELIMIT]);

                                    if (limitValue_ > 0.0000001)
                                    {
                                        double newBalance =
                                            CASTASDOUBLE(clRecord_[Tables.ColsClient.BALANCE])
                                            + total_
                                            + (ToolClient.getTotalDebit(pRoot, refCl_) - ToolClient.getTotalCredit(pRoot, refCl_));

                                        if (newBalance - limitValue_ > 0.001)
                                            throw new Exception(strDesc + " (T_BALANCE ^ " + (newBalance - limitValue_) + ")");
                                    }

                                }

                            }

                        }

                    }
                    static void addToDic(Dictionary<object, double> pDic, object pLref, double pVal)
                    {
                        if (!pDic.ContainsKey(pLref))
                            pDic.Add(pLref, 0);

                        pDic[pLref] = pDic[pLref] + pVal;
                    }




                }

                //public class ClassBlockPromoAsMaterial
                //{



                //    public static void done(PluginTool pRoot, DataTable pStline)
                //    {

                //        for (int i = 0; i < pStline.Rows.Count; ++i)
                //        {
                //            DataRow _row = pStline.Rows[i];
                //            if (_row.RowState != DataRowState.Deleted)
                //            {
                //                if (ToolStline.isLineMat(_row) && pRoot.stockIsPromo(pRoot, _row[Tables.ColsStline.STOCKREF]))
                //                {
                //                    throw new Exception("T_MSG_INVALID_MATERIAL");
                //                }
                //            }
                //        }
                //    }


                //}

                public class ClassDiscLine
                {


                    const string _code = "mat_disc";
                    public static void done(PluginTool pRoot, DataTable pStfiche, DataTable pStline)
                    {
                        DataRow header_ = TAB_GETLASTROW(pStfiche);

                        if (!ToolStline.ignoreDoc(pRoot, header_))
                        {

                            object clientRef_ = header_[Tables.ColsStfiche.CLIENTREF];

                            var tableDocSaving = pRoot.cacheDocSave.getDocSavingTable(pRoot);

                            foreach (DataRow _row in ToolTable.getFixedCollection(pStline)) //collection will be modifaed
                            {
                                if (_row.RowState != DataRowState.Deleted && _row.RowState != DataRowState.Detached)
                                {
                                    if (ToolStline.isLineLocal(_row) && ToolStline.isLineMat(_row))
                                    {
                                        foreach (DataRow promoInfo_ in getItemPromoInfo(pRoot, _row[Tables.ColsStline.STOCKREF], clientRef_, tableDocSaving))
                                        {
                                            if (ClassFilterPromoByClient.allowed(pRoot, header_, promoInfo_))
                                                if (applyItemPromo(pRoot, pStline, _row, promoInfo_))
                                                    break; //move to stline next line
                                        }
                                    }
                                }
                            }
                        }
                    }

                    static bool applyItemPromo(PluginTool pRoot, DataTable pLines, DataRow pLine, DataRow promoInfo)
                    {
                        if (promoInfo != null)
                        {

                            double discount_ = CASTASDOUBLE(promoInfo[Tables.ColsDocSaving.CF1]);
                            if (discount_ > 0.0000001 && discount_ <= 100)
                            {

                                //if (pRoot.isMobile())
                                //{
                                //    pLine[Tables.ColsStline.DISCPER] = discount_;
                                //    ToolStline.setLineScripted(pLine, promoInfo);
                                //    return true;
                                //}
                                //else
                                {
                                    DataRow _rowDisc = pLines.NewRow();
                                    _rowDisc[Tables.ColsStline.LINETYPE] = 2; //discount

                                    int indx_ = pLines.Rows.IndexOf(pLine);
                                    if (indx_ < 0)
                                        throw new Exception("Row not in table");
                                    pLines.Rows.InsertAt(_rowDisc, indx_ + 1);

                                    _rowDisc[Tables.ColsStline.DISCPER] = discount_;
                                    ToolStline.setLineScripted(_rowDisc, promoInfo);
                                    return true;
                                }




                            }
                        }

                        return false;


                    }
                    static DataRow[] getItemPromoInfo(PluginTool pRoot, object itemRef, object clRef, DataTable promoInfo)
                    {
                        List<DataRow> list = new List<DataRow>();

                        string filterItem_ = pRoot.cacheMaterial.getItemCode(pRoot, itemRef);
                        filterItem_ = filterItem_.Trim().ToLowerInvariant();

                        for (int i = 0; i < promoInfo.Rows.Count; ++i)
                        {
                            DataRow row = promoInfo.Rows[i];
                            if (row[Tables.ColsDocSaving.TYPE].ToString() == _code)
                            {
                                string promoCode_ = row[Tables.ColsDocSaving.CODE].ToString().Trim().ToLowerInvariant();

                                if (promoCode_ != "" && filterItem_.StartsWith(promoCode_))
                                    // if ((string)row[Tables.ColsDocSaving.CLCODE] == clRef)// by FILTERPROMOCL
                                    list.Add(row);
                            }
                        }
                        return list.ToArray();
                    }
                }


                public class ClassPriceLine
                {


                    const string _code = "mat_price";
                    public static void done(PluginTool pRoot, DataTable pStfiche, DataTable pStline)
                    {
                        DataRow header_ = TAB_GETLASTROW(pStfiche);

                        if (!ToolStline.ignoreDoc(pRoot, header_))
                        {

                            object clientRef_ = header_[Tables.ColsStfiche.CLIENTREF];

                            var tableDocSaving = pRoot.cacheDocSave.getDocSavingTable(pRoot);

                            foreach (DataRow _row in ToolTable.getFixedCollection(pStline)) //collection will be modifaed
                            {
                                if (_row.RowState != DataRowState.Deleted && _row.RowState != DataRowState.Detached)
                                {
                                    if (ToolStline.isLineLocal(_row) && ToolStline.isLineMat(_row))
                                    {
                                        foreach (DataRow promoInfo_ in getItemPromoInfo(pRoot, _row[Tables.ColsStline.STOCKREF], clientRef_, tableDocSaving))
                                        {
                                            if (ClassFilterPromoByClient.allowed(pRoot, header_, promoInfo_))
                                                if (applyItemPromo(pRoot, pStline, _row, promoInfo_))
                                                    break; //move to stline next line
                                        }
                                    }
                                }
                            }
                        }
                    }

                    static bool applyItemPromo(PluginTool pRoot, DataTable pLines, DataRow pLine, DataRow promoInfo)
                    {
                        if (promoInfo != null)
                        {
                            double price_ = CASTASDOUBLE(promoInfo[Tables.ColsDocSaving.CF1]);
                            if (price_ > 0.0000001)
                            {

                                pLine[Tables.ColsStline.PRICE] = price_;
                                ToolStline.setLineScripted(pLine, promoInfo);
                                return true;
                            }
                        }

                        return false;


                    }
                    static DataRow[] getItemPromoInfo(PluginTool pRoot, object itemRef, object clRef, DataTable promoInfo)
                    {
                        List<DataRow> list = new List<DataRow>();

                        string filterItem_ = pRoot.cacheMaterial.getItemCode(pRoot, itemRef);
                        filterItem_ = filterItem_.Trim().ToLowerInvariant();

                        for (int i = 0; i < promoInfo.Rows.Count; ++i)
                        {
                            DataRow row = promoInfo.Rows[i];
                            if (row[Tables.ColsDocSaving.TYPE].ToString() == _code)
                            {
                                string promoCode_ = row[Tables.ColsDocSaving.CODE].ToString().Trim().ToLowerInvariant();

                                if (promoCode_ != "" && filterItem_.StartsWith(promoCode_))
                                    // if ((string)row[Tables.ColsDocSaving.CLCODE] == clRef)// by FILTERPROMOCL
                                    list.Add(row);
                            }
                        }
                        return list.ToArray();
                    }
                }


                public class ClassFilterPromoByClient
                {


                    const string _COL = "FILTERPROMOCL";

                    public static bool allowed(PluginTool pRoot, DataRow pDocHeader, DataRow promoInfo)
                    {
                        if (promoInfo == null || pDocHeader == null || pRoot == null || pDocHeader.RowState == DataRowState.Deleted)
                            return false;

                        string promoFilterStr_ = string.Empty;
                        string clientFilterStr_ = string.Empty;
                        //
                        if (promoInfo != null && promoInfo.Table.Columns.Contains(_COL))
                            promoFilterStr_ = promoInfo[Tables.ColsDocSaving.FILTERPROMOCL].ToString().Trim();
                        //

                        if (promoFilterStr_ == string.Empty) // allow all id empty
                            return true;

                        ///////////////////////////////////////////////////////////////////////////////////
                        object clientRef = pDocHeader[Tables.ColsStfiche.CLIENTREF];

                        DataRow rowClient_ = pRoot.cacheClient.getClientRecord(pRoot, clientRef);

                        if (rowClient_ == null)//client not selected but comp with filted
                            return false;

                        if (rowClient_ != null && rowClient_.Table.Columns.Contains(_COL))
                            clientFilterStr_ = rowClient_[Tables.ColsClient.FILTERPROMOCL].ToString().Trim();

                        if (clientFilterStr_ == string.Empty) // company has filter but client hsnt
                            return false;

                        //////////////////////////////////////////////////////////////////////////////////

                        // clientFilterStr_ = clientFilterStr_.Replace(" ", "").Replace("\t", "");
                        //clientFilterStr_ = "," + clientFilterStr_ + ",";
                        // clientFilterStr_ = clientFilterStr_.ToLower();

                        // promoFilterStr_ = promoFilterStr_.Replace(" ", "").Replace("\t", "");
                        //promoFilterStr_ = "," + promoFilterStr_ + ",";
                        //  promoFilterStr_ = promoFilterStr_.ToLower();

                        /////////////////////////////////////////////////////////////////////////////////

                        //return (clientFilterStr_.IndexOf(promoFilterStr_) >= 0);

                        //return clientFilterStr_ == promoFilterStr_;


                        clientFilterStr_ = clientFilterStr_.Replace(" ", "").Replace("\t", "");
                        clientFilterStr_ = clientFilterStr_.ToLower();

                        promoFilterStr_ = promoFilterStr_.Replace(" ", "").Replace("\t", "");
                        promoFilterStr_ = promoFilterStr_.ToLower();

                        List<string> list1_ = new List<string>(clientFilterStr_.Split(','));
                        List<string> list2_ = new List<string>(promoFilterStr_.Split(','));

                        foreach (string s1 in list1_)
                            foreach (string s2 in list2_)
                            {
                                if (s1 != "" && s2 != "" && s1 == s2)
                                    return true;
                            }

                        return false;

                    }



                }

                public class ClassPromoDelete
                {
                    const string _code = "del";
                    public static void done(PluginTool pRoot, DataTable pStline, DataTable pStfiche)
                    {
                        List<DataRow> list_ = new List<DataRow>();
                        var tableDocSaving = pRoot.cacheDocSave.getDocSavingTable(pRoot);

                        DataRow header = TAB_GETLASTROW(pStfiche);

                        for (int i = 0; i < pStline.Rows.Count; ++i)
                        {
                            DataRow _row = pStline.Rows[i];
                            if (_row.RowState != DataRowState.Deleted)
                            {
                                if (ToolStline.isLinePromo(_row))
                                {
                                    double amount = CASTASDOUBLE(_row[Tables.ColsStline.AMOUNT]);
                                    if (amount > 0.0000001)
                                    {
                                        list_.Add(_row);
                                    }
                                }
                            }
                        }

                        foreach (DataRow promoInfo_ in getPromoInfo(tableDocSaving))
                            if (ClassFilterPromoByClient.allowed(pRoot, header, promoInfo_))
                                applyPromo(pStline, list_, promoInfo_);

                    }

                    static void applyPromo(DataTable table, List<DataRow> list, DataRow promoInfo)
                    {

                        if (promoInfo == null)
                            return;



                        double cf1 = CASTASDOUBLE(promoInfo[Tables.ColsDocSaving.CF1]);
                        int minAllowedPromoLines = (int)cf1;
                        if (list.Count < minAllowedPromoLines)
                            delete(list);

                    }
                    static void delete(List<DataRow> list)
                    {

                        foreach (DataRow row_ in list)
                            row_.Delete();

                    }
                    static DataRow[] getPromoInfo(DataTable promoInfo)
                    {
                        List<DataRow> list_ = new List<DataRow>();
                        for (int i = 0; i < promoInfo.Rows.Count; ++i)
                        {
                            DataRow row = promoInfo.Rows[i];
                            if ((string)row[Tables.ColsDocSaving.TYPE] == _code)
                                list_.Add(row);
                        }
                        return list_.ToArray();
                    }
                }

                public class ClassPromoGroup
                {
                    const string _code = "grp";
                    public static void done(PluginTool pRoot, DataTable pStline, DataTable pStfiche)
                    {
                        Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
                        var tableDocSaving = pRoot.cacheDocSave.getDocSavingTable(pRoot);
                        for (int i = 0; i < pStline.Rows.Count; ++i)
                        {
                            DataRow _row = pStline.Rows[i];
                            if (_row.RowState != DataRowState.Deleted)
                            {
                                if (ToolStline.isLineLocal(_row) && ToolStline.isLineMat(_row))
                                {
                                    double amount = CASTASDOUBLE(_row[Tables.ColsStline.AMOUNT]);
                                    if (amount > 0.0000001)
                                    {
                                        string itemRef = (string)_row[Tables.ColsStline.STOCKREF];
                                        DataRow itemRec = pRoot.cacheMaterial.getItemRecord(pRoot, itemRef);
                                        string itemGrp = (string)itemRec[Tables.ColsItems.STGRPCODESUB];
                                        if (!dic.ContainsKey(itemGrp))
                                            dic.Add(itemGrp, new List<string>());

                                        List<string> items = dic[itemGrp];
                                        if (!items.Contains(itemRef))
                                            items.Add(itemRef);
                                    }
                                }
                            }
                        }
                        DataRow header_ = TAB_GETLASTROW(pStfiche);

                        foreach (string grp in dic.Keys)
                        {
                            DataRow promoInfo_ = getGroupPromoInfo(grp, tableDocSaving);
                            if (ClassFilterPromoByClient.allowed(pRoot, header_, promoInfo_))
                                applyGroupPromo(pStline, dic[grp].Count, promoInfo_);
                        }

                    }

                    static void applyGroupPromo(DataTable table, int count, DataRow promoInfo)
                    {

                        if (promoInfo == null)
                            return;



                        double cf1 = CASTASDOUBLE(promoInfo[Tables.ColsDocSaving.CF1]);
                        double cf2 = CASTASDOUBLE(promoInfo[Tables.ColsDocSaving.CF2]);
                        string promoItem = (string)promoInfo[Tables.ColsDocSaving.PROMOMATCODE];

                        double amountPromo = cf2;

                        if (count > 0)
                            if (count >= cf1)
                            {
                                if (amountPromo > 0.0000001)
                                {
                                    DataRow _rowPromo = table.NewRow();
                                    table.Rows.Add(_rowPromo);
                                    _rowPromo[Tables.ColsStline.GLOBTRANS] = 1;
                                    _rowPromo[Tables.ColsStline.LINETYPE] = 1;
                                    _rowPromo[Tables.ColsStline.STOCKREF] = promoItem;
                                    _rowPromo[Tables.ColsStline.AMOUNT] = amountPromo;


                                    ToolStline.setLineScripted(_rowPromo, _code);
                                }
                            }

                    }

                    static DataRow getGroupPromoInfo(string grp, DataTable promoInfo)
                    {
                        for (int i = 0; i < promoInfo.Rows.Count; ++i)
                        {
                            DataRow row = promoInfo.Rows[i];
                            if ((string)row[Tables.ColsDocSaving.TYPE] == _code)
                                if ((string)row[Tables.ColsDocSaving.CODE] == grp)
                                    return row;
                        }
                        return null;
                    }
                }


                public class ClassPromoGroupCoif
                {
                    const string _code = "grp_coif";
                    public static void done(PluginTool pRoot, DataTable pStline, DataTable pStfiche)
                    {
                        Dictionary<string, double> dic = new Dictionary<string, double>();
                        var tableDocSaving = pRoot.cacheDocSave.getDocSavingTable(pRoot);
                        for (int i = 0; i < pStline.Rows.Count; ++i)
                        {
                            DataRow _row = pStline.Rows[i];
                            if (_row.RowState != DataRowState.Deleted)
                            {
                                if (ToolStline.isLineLocal(_row) && ToolStline.isLineMat(_row))
                                {
                                    double amount = CASTASDOUBLE(_row[Tables.ColsStline.AMOUNT]);
                                    if (amount > 0.0000001)
                                    {
                                        string itemRef = (string)_row[Tables.ColsStline.STOCKREF];
                                        DataRow itemRec = pRoot.cacheMaterial.getItemRecord(pRoot, itemRef);
                                        string itemGrp = (string)itemRec[Tables.ColsItems.STGRPCODESUB];
                                        if (!dic.ContainsKey(itemGrp))
                                            dic.Add(itemGrp, 0.0);

                                        dic[itemGrp] = dic[itemGrp] + amount;
                                    }
                                }
                            }
                        }

                        DataRow header_ = TAB_GETLASTROW(pStfiche);

                        foreach (string grp in dic.Keys)
                        {
                            foreach (DataRow promoInfo_ in getGroupPromoInfo(grp, tableDocSaving))
                                if (ClassFilterPromoByClient.allowed(pRoot, header_, promoInfo_))
                                    applyGroupPromo(pStline, dic[grp], promoInfo_);
                        }

                    }

                    static void applyGroupPromo(DataTable table, double amount, DataRow promoInfo)
                    {
                        if (promoInfo == null)
                            return;

                        double cf1 = CASTASDOUBLE(promoInfo[Tables.ColsDocSaving.CF1]);
                        double cf2 = CASTASDOUBLE(promoInfo[Tables.ColsDocSaving.CF2]);
                        string promoItem = (string)promoInfo[Tables.ColsDocSaving.PROMOMATCODE];

                        double amountPromo = Math.Floor((cf1 > 0.0000001 ? amount * cf2 / cf1 : 0));

                        if (amountPromo > 0.0000001)
                        {
                            DataRow _rowPromo = table.NewRow();
                            table.Rows.Add(_rowPromo);
                            _rowPromo[Tables.ColsStline.GLOBTRANS] = 1;
                            _rowPromo[Tables.ColsStline.LINETYPE] = 1;
                            _rowPromo[Tables.ColsStline.STOCKREF] = promoItem;
                            _rowPromo[Tables.ColsStline.AMOUNT] = amountPromo;

                            ToolStline.setLineScripted(_rowPromo, _code);

                        }

                    }

                    static DataRow[] getGroupPromoInfo(string grp, DataTable promoInfo)
                    {
                        List<DataRow> list_ = new List<DataRow>();
                        for (int i = 0; i < promoInfo.Rows.Count; ++i)
                        {
                            DataRow row = promoInfo.Rows[i];
                            if ((string)row[Tables.ColsDocSaving.TYPE] == _code)
                                if ((string)row[Tables.ColsDocSaving.CODE] == grp)
                                    list_.Add(row);
                        }
                        return list_.ToArray();
                    }
                }



                public class ClassPromoGroupMinAmount
                {
                    const string _code = "grp_min";
                    public static void done(PluginTool pRoot, DataTable pStline, DataTable pStfiche)
                    {
                        Dictionary<string, List<string>> dicGrps = new Dictionary<string, List<string>>();
                        Dictionary<string, double> dicGrpsMinAmount = new Dictionary<string, double>();
                        var tableDocSaving = pRoot.cacheDocSave.getDocSavingTable(pRoot);
                        for (int i = 0; i < pStline.Rows.Count; ++i)
                        {
                            DataRow _row = pStline.Rows[i];
                            if (_row.RowState != DataRowState.Deleted)
                            {
                                if (ToolStline.isLineLocal(_row) && ToolStline.isLineMat(_row))
                                {
                                    double amount = CASTASDOUBLE(_row[Tables.ColsStline.AMOUNT]);
                                    if (amount > 0.0000001)
                                    {
                                        string itemRef = (string)_row[Tables.ColsStline.STOCKREF];
                                        DataRow itemRec = pRoot.cacheMaterial.getItemRecord(pRoot, itemRef);
                                        string itemGrp = (string)itemRec[Tables.ColsItems.STGRPCODESUB];
                                        if (!dicGrps.ContainsKey(itemGrp))
                                            dicGrps.Add(itemGrp, new List<string>());

                                        List<string> items = dicGrps[itemGrp];
                                        if (!items.Contains(itemRef))
                                            items.Add(itemRef);

                                        {
                                            if (!dicGrpsMinAmount.ContainsKey(itemGrp))
                                                dicGrpsMinAmount.Add(itemGrp, amount);

                                            dicGrpsMinAmount[itemGrp] = Math.Min(amount, dicGrpsMinAmount[itemGrp]);

                                        }
                                    }
                                }
                            }
                        }

                        DataRow header_ = TAB_GETLASTROW(pStfiche);

                        foreach (string grp in dicGrps.Keys)
                        {
                            if (dicGrpsMinAmount.ContainsKey(grp))
                                if (dicGrpsMinAmount[grp] > 0.0000001)
                                {
                                    foreach (DataRow promoInfo_ in getGroupPromoInfo(grp, tableDocSaving))
                                        if (ClassFilterPromoByClient.allowed(pRoot, header_, promoInfo_))
                                            applyGroupPromo(pStline, dicGrps[grp].Count, dicGrpsMinAmount[grp], promoInfo_);

                                }
                        }

                    }

                    static void applyGroupPromo(DataTable table, int count, double grpMinAmount, DataRow promoInfo)
                    {

                        if (promoInfo == null)
                            return;



                        double cf1 = CASTASDOUBLE(promoInfo[Tables.ColsDocSaving.CF1]);

                        string promoItem = (string)promoInfo[Tables.ColsDocSaving.PROMOMATCODE];

                        double amountPromo = grpMinAmount;
                        double minCount = (int)Math.Floor(cf1);
                        if (count > 0)
                            if (count >= minCount)
                            {
                                if (amountPromo > 0.0000001)
                                {
                                    DataRow _rowPromo = table.NewRow();
                                    table.Rows.Add(_rowPromo);
                                    _rowPromo[Tables.ColsStline.GLOBTRANS] = 1;
                                    _rowPromo[Tables.ColsStline.LINETYPE] = 1;
                                    _rowPromo[Tables.ColsStline.STOCKREF] = promoItem;
                                    _rowPromo[Tables.ColsStline.AMOUNT] = amountPromo;

                                    ToolStline.setLineScripted(_rowPromo, _code);

                                }
                            }

                    }

                    static DataRow[] getGroupPromoInfo(string grp, DataTable promoInfo)
                    {
                        List<DataRow> list_ = new List<DataRow>();
                        for (int i = 0; i < promoInfo.Rows.Count; ++i)
                        {
                            DataRow row = promoInfo.Rows[i];
                            if ((string)row[Tables.ColsDocSaving.TYPE] == _code)
                                if ((string)row[Tables.ColsDocSaving.CODE] == grp)
                                    list_.Add(row);
                        }
                        return list_.ToArray();
                    }
                }



                public class ClassPromoGroupSets
                {
                    class GroupInfo
                    {
                        public int countMat = 0;
                        public double amountTot = 0;

                    }
                    const string _code = "grp_sets";
                    public static void done(PluginTool pRoot, DataTable pStline, DataTable pStfiche)
                    {
                        Dictionary<string, List<string>> dicGrpsMats = new Dictionary<string, List<string>>();
                        Dictionary<string, GroupInfo> dicGrpsInfo = new Dictionary<string, GroupInfo>();

                        var tableDocSaving = pRoot.cacheDocSave.getDocSavingTable(pRoot);
                        for (int i = 0; i < pStline.Rows.Count; ++i)
                        {
                            DataRow _row = pStline.Rows[i];
                            if (_row.RowState != DataRowState.Deleted)
                            {
                                if (ToolStline.isLineLocal(_row) && ToolStline.isLineMat(_row))
                                {
                                    double amount = CASTASDOUBLE(_row[Tables.ColsStline.AMOUNT]);
                                    if (amount > 0.0000001)
                                    {
                                        string itemRef = (string)_row[Tables.ColsStline.STOCKREF];
                                        DataRow itemRec = pRoot.cacheMaterial.getItemRecord(pRoot, itemRef);
                                        string itemGrp = (string)itemRec[Tables.ColsItems.STGRPCODESUB];
                                        //
                                        if (!dicGrpsMats.ContainsKey(itemGrp))
                                            dicGrpsMats.Add(itemGrp, new List<string>());
                                        if (!dicGrpsInfo.ContainsKey(itemGrp))
                                            dicGrpsInfo.Add(itemGrp, new GroupInfo());
                                        //
                                        List<string> matList_ = dicGrpsMats[itemGrp];
                                        GroupInfo info_ = dicGrpsInfo[itemGrp];
                                        //
                                        if (!matList_.Contains(itemRef))
                                        {
                                            info_.countMat += 1; //count distinct
                                            matList_.Add(itemRef); //then add
                                        }

                                        info_.amountTot += amount;


                                    }
                                }
                            }
                        }



                        //#if DEBUG
                        //            StreamWriter sw_ = File.CreateText("grp.txt");
                        //            foreach (string g_ in dicGrpsInfo.Keys)
                        //            {
                        //                sw_.WriteLine(g_ + "," + dicGrpsInfo[g_].countMat + "," + dicGrpsInfo[g_].amountTot);
                        //            }
                        //            sw_.Close();
                        //#endif

                        DataRow header_ = TAB_GETLASTROW(pStfiche);

                        foreach (DataRow promoInfo_ in getGroupPromoInfo(tableDocSaving))
                            if (ClassFilterPromoByClient.allowed(pRoot, header_, promoInfo_))
                                applyGroupPromo(pStline, dicGrpsInfo, promoInfo_);



                    }

                    class PromoList : List<PromoDesc>
                    {
                        public int apply(Dictionary<string, GroupInfo> dicStat)
                        {
                            int count_ = -1;

                            foreach (PromoDesc desc_ in this)
                            {
                                if (!dicStat.ContainsKey(desc_.groupName))
                                    return 0;
                                GroupInfo intoGrp_ = dicStat[desc_.groupName];
                                //
                                int countTmp_ = 0;

                                if (intoGrp_.countMat >= desc_.matCountMin && desc_.matCountMin > 0)
                                    countTmp_ = (int)(intoGrp_.amountTot / desc_.matQuantityTotMin);
                                //
                                if (countTmp_ == 0)
                                    return 0;

                                if (count_ < 0)
                                    count_ = countTmp_;

                                count_ = Math.Min(count_, countTmp_);
                            }

                            return Math.Max(0, count_);
                        }
                        public void parse(string pVal)
                        {
                            try
                            {
                                pVal = pVal.Trim();
                                Clear();
                                foreach (string itm_ in pVal.Split(';'))
                                    if (itm_ != string.Empty)
                                    {
                                        PromoDesc desc_ = new PromoDesc();
                                        desc_.parse(itm_);
                                        this.Add(desc_);
                                    }
                            }
                            catch (Exception exc)
                            {
                                throw new Exception("T_MSG_INVALID_PARAMETER " + pVal, exc);
                            }

                            //#if DEBUG
                            //                StreamWriter sw_ = File.CreateText("desc.txt");
                            //                foreach (PromoDesc d_ in this)
                            //                {
                            //                    sw_.WriteLine(d_.groupName + "," + d_.matCountMin + "," + d_.matQuantityTotMin);
                            //                }
                            //                sw_.Close();
                            //#endif
                        }
                    }
                    class PromoDesc
                    {
                        public string groupName = string.Empty;
                        public int matCountMin = 0;
                        public double matQuantityTotMin = 0;

                        public void parse(string pVal)
                        {
                            try
                            {
                                pVal = pVal.Trim();
                                string[] arr_ = pVal.Split(',');
                                if (arr_.Length > 0)
                                    groupName = arr_[0].Trim();
                                if (arr_.Length > 1)
                                    matCountMin = int.Parse(arr_[1].Trim());
                                if (arr_.Length > 2)
                                    matQuantityTotMin = double.Parse(arr_[2].Trim());
                            }
                            catch (Exception exc)
                            {
                                throw new Exception("T_MSG_INVALID_PARAMETER " + pVal, exc);
                            }
                        }

                    }
                    static void applyGroupPromo(DataTable table, Dictionary<string, GroupInfo> dicStat, DataRow promoInfo)
                    {
                        // GropuName,MinMatCount,MinMatAmountTot;
                        //Prizma,5,8;Sok,1,2

                        if (promoInfo == null)
                            return;



                        double cf1 = CASTASDOUBLE(promoInfo[Tables.ColsDocSaving.CF1]);
                        string promoItem = (string)promoInfo[Tables.ColsDocSaving.PROMOMATCODE];
                        string promoString = (string)promoInfo[Tables.ColsDocSaving.TEXT1] + ";" + (string)promoInfo[Tables.ColsDocSaving.TEXT2];
                        double amountPromo = cf1;

                        if (amountPromo > 0.000001)
                        {
                            PromoList listPromo_ = new PromoList();
                            listPromo_.parse(promoString);
                            int count_ = listPromo_.apply(dicStat);

                            if (count_ > 0)
                            {
                                DataRow _rowPromo = table.NewRow();
                                table.Rows.Add(_rowPromo);
                                _rowPromo[Tables.ColsStline.GLOBTRANS] = 1;
                                _rowPromo[Tables.ColsStline.LINETYPE] = 1;
                                _rowPromo[Tables.ColsStline.STOCKREF] = promoItem;
                                _rowPromo[Tables.ColsStline.AMOUNT] = amountPromo * count_;

                                ToolStline.setLineScripted(_rowPromo, _code);


                            }

                        }
                    }

                    static DataRow[] getGroupPromoInfo(DataTable promoInfo)
                    {
                        List<DataRow> list_ = new List<DataRow>();
                        for (int i = 0; i < promoInfo.Rows.Count; ++i)
                        {
                            DataRow row = promoInfo.Rows[i];
                            if ((string)row[Tables.ColsDocSaving.TYPE] == _code)
                                list_.Add(row);
                        }
                        return list_.ToArray();
                    }


                }


                public class ClassPromoLine
                {


                    const string _code = "mat_promo";
                    public static void done(PluginTool pRoot, DataTable pStfiche, DataTable pStline)
                    {
                        var tableDocSaving = pRoot.cacheDocSave.getDocSavingTable(pRoot);
                        DataRow header_ = TAB_GETLASTROW(pStfiche);

                        if (!ToolStline.ignoreDoc(pRoot, header_))
                        {

                            foreach (DataRow _row in ToolTable.getFixedCollection(pStline)) //collection will be modifaed
                            {
                                if (_row.RowState != DataRowState.Deleted && _row.RowState != DataRowState.Detached)
                                {
                                    if (ToolStline.isLineLocal(_row) && ToolStline.isLineMat(_row))
                                    {
                                        var promoInfo = getItemPromoInfo(pRoot, _row[Tables.ColsStline.STOCKREF], tableDocSaving);
                                        foreach (DataRow promoInfo_ in promoInfo)
                                            if (ClassFilterPromoByClient.allowed(pRoot, header_, promoInfo_))
                                                applyItemPromo(pRoot, pStline, _row, promoInfo_);
                                    }
                                }
                            }
                        }
                    }

                    static void applyItemPromo(PluginTool pRoot, DataTable pStline, DataRow pLine, DataRow promoInfo)
                    {
                        if (promoInfo == null)
                            return;

                        double cf1 = CASTASDOUBLE(promoInfo[Tables.ColsDocSaving.CF1]);
                        double cf2 = CASTASDOUBLE(promoInfo[Tables.ColsDocSaving.CF2]);

                        if (cf1 < 0.001 || cf2 < 0.001)
                            return;

                        string promoItemCode = CASTASSTRING(promoInfo[Tables.ColsDocSaving.PROMOMATCODE]);

                        object itemRef_ = pRoot.cacheMaterial.getItemRef(pRoot, promoItemCode);

                        double amount = CASTASDOUBLE(pLine[Tables.ColsStline.AMOUNT]);
                        double amountPromo = Math.Floor(Math.Floor(amount / cf1) * cf2);

                        if (amountPromo > 0.0000001)
                        {
                            DataRow _rowPromo = pStline.NewRow();
                            _rowPromo[Tables.ColsStline.LINETYPE] = 1;

                            int indx_ = pStline.Rows.IndexOf(pLine);
                            if (indx_ < 0)
                                throw new Exception("Row not in table");

                            pStline.Rows.InsertAt(_rowPromo, indx_ + 1);

                            _rowPromo[Tables.ColsStline.STOCKREF] = itemRef_;

                            //if (!pRoot.isMobile())
                            //    ToolTable.setRow(_rowPromo, Tables.ColsStline.UOMREF, _rowPromo[Tables.ColsStline.UOMREF]);

                            _rowPromo[Tables.ColsStline.AMOUNT] = amountPromo;

                            ToolStline.setLineScripted(_rowPromo, _code);

                        }
                    }
                    static DataRow[] getItemPromoInfo(PluginTool pRoot, object itemRef, DataTable promoInfo)
                    {
                        string filter_ = null;

                        //if (pRoot.isMobile())
                        //    filter_ = itemRef.ToString();
                        //else
                        filter_ = pRoot.cacheMaterial.getItemCode(pRoot, itemRef);


                        List<DataRow> list_ = new List<DataRow>();
                        for (int i = 0; i < promoInfo.Rows.Count; ++i)
                        {
                            DataRow row = promoInfo.Rows[i];
                            if (CASTASSTRING(row[Tables.ColsDocSaving.TYPE]) == _code)
                                if (MY_IS_MATH(filter_, CASTASSTRING(row[Tables.ColsDocSaving.CODE])))
                                    list_.Add(row);
                        }
                        return list_.ToArray();
                    }
                }



                public class FillDocOrderWh
                {
                    public const string FILLCMD = "__FILL";

                    const string __SQL_TEXT_GETDATE = "select max(DATE_) from LG_$FIRM$_$PERIOD$_ORFICHE where TRCODE = @P1 and CANCELLED = 0";

                    const string __SQL_TEXT_GETDATA = "select STOCKREF,AMOUNT from LG_$FIRM$_$PERIOD$_ORFLINE where TRCODE = @P1 and DATE_ > @P2 and DATE_ < @P3 and CANCELLED = 0";
                    public static void done(PluginTool pRoot, DataTable pStline, DataTable pStfiche)
                    {

                        ToolTable.deleteContent(pStline);
                        DateTime lastOrdWhDate = (DateTime)ToolType.isNull(
                            pRoot.SQLSCALAR(__SQL_TEXT_GETDATE, new object[] { (int)15 }),
                            DateTime.Now.Date);

                        DateTime newOrdWhDate = DateTime.Now;


                        DataTable tableFillData = pRoot.SQL(__SQL_TEXT_GETDATA, new object[] { (int)8, lastOrdWhDate, newOrdWhDate });

                        ToolTable.setRow(TAB_GETLASTROW(pStfiche), Tables.ColsStfiche.DATE_, newOrdWhDate);

                        Dictionary<object, double> dicFillData = new Dictionary<object, double>();
                        foreach (DataRow row_ in tableFillData.Rows)
                        {
                            object stockref = ToolType.isNull(row_[Tables.ColsStline.STOCKREF], string.Empty).ToString();
                            double quantity = CASTASDOUBLE(ToolType.isNull(row_[Tables.ColsStline.AMOUNT], 0.0));
                            if (!dicFillData.ContainsKey(stockref))
                                dicFillData.Add(stockref, quantity);
                            else
                                dicFillData[stockref] += quantity;
                        }

                        foreach (KeyValuePair<object, double> pair in dicFillData)
                        {
                            DataRow newRow_ = pStline.NewRow();
                            pStline.Rows.Add(newRow_);

                            newRow_[Tables.ColsStline.STOCKREF] = pair.Key;
                            newRow_[Tables.ColsStline.AMOUNT] = pair.Value;

                        }
                    }


                }

                public class ToolTable
                {

                    public static void setRow(DataRow pRow, string pCol, object pVal)
                    {
                        if (pRow != null)
                        {

                            if (pVal != null)
                            {
                                object val2_ = pRow[pCol];

                                Type t1_ = pVal.GetType();
                                Type t2_ = val2_.GetType();

                                if (t1_ == typeof(string) && t2_ == typeof(string))
                                {
                                    if ((string)pVal == (string)val2_)
                                        return;
                                }
                                else

                                    if (
                                        (t1_ == typeof(int) || t1_ == typeof(double) || t1_ == typeof(short)) &&
                                        (t2_ == typeof(int) || t2_ == typeof(double) || t2_ == typeof(short)))
                                    {
                                        if (Math.Abs(CASTASDOUBLE(pVal) - CASTASDOUBLE(val2_)) < 0.0000001)
                                            return;
                                    }
                                    else
                                        if (t1_ == typeof(DateTime) && t2_ == typeof(DateTime))
                                        {
                                            if ((DateTime)pVal == (DateTime)val2_)
                                                return;
                                        }

                            }


                            pRow[pCol] = pVal;
                        }


                    }

                    public static void deleteContent(DataTable _table)
                    {
                        List<DataRow> list = new List<DataRow>();
                        for (int i = 0; i < _table.Rows.Count; ++i)
                        {
                            DataRow _row = _table.Rows[i];
                            if (_row.RowState != DataRowState.Deleted)
                                list.Add(_row);

                        }

                        for (int i = 0; i < list.Count; ++i)
                            list[i].Delete();
                    }




                    public static DataRow[] getFixedCollection(DataTable pTab)
                    {
                        List<DataRow> l = new List<DataRow>();

                        for (int i = 0; i < pTab.Rows.Count; ++i)
                        {
                            l.Add(pTab.Rows[i]);
                        }

                        return l.ToArray();
                    }
                }





                public class ToolStline
                {
                    public static bool ignoreDoc(PluginTool pRoot, DataRow pHeaderRow)
                    {
                        //Dont aply discount for some docs

                        if (pHeaderRow == null || pHeaderRow.RowState == DataRowState.Deleted)
                            return true;

                        //if (!pRoot.isMobile() && pHeaderRow[Tables.ColsStfiche.FICHENO].ToString().StartsWith("M"))
                        //    return true;

                        return false;
                    }

                    public static bool isLineLocal(DataRow row)
                    {
                        return (CASTASINT(row[Tables.ColsStline.GLOBTRANS]) == 0);
                    }
                    public static bool isLineMat(DataRow row)
                    {
                        return (CASTASINT(row[Tables.ColsStline.LINETYPE]) == 0);
                    }
                    public static bool isLinePromo(DataRow row)
                    {
                        return (CASTASINT(row[Tables.ColsStline.LINETYPE]) == 1);
                    }
                    public static bool isLineDiscount(DataRow row)
                    {
                        return (CASTASINT(row[Tables.ColsStline.LINETYPE]) == 2);
                    }
                    public static bool isLineScripted(DataRow row)
                    {
                        DataColumn col_ = null;

                        //if (col_ == null)
                        //    col_ = row.Table.Columns[Tables.ColsStline.SCRIPTFLG];

                        if (col_ != null)
                        {
                            return (CASTASINT(row[col_]) == 1);
                        }
                        if (col_ == null)
                            col_ = row.Table.Columns[Tables.ColsStline.LINEEXP];

                        if (col_ != null)
                        {
                            return CASTASSTRING(row[col_]).StartsWith(CONSTVAL.PROMO);
                        }

                        throw new Exception("Cant detect row type");
                    }

                    public static void setLineScripted(DataRow row, string pCode)
                    {
                        string desc_ = CONSTVAL.PROMO + "." + pCode;

                        //{
                        //    var col1_ = row.Table.Columns[Tables.ColsStline.SCRIPTFLG];

                        //    if (col1_ != null)
                        //    {
                        //        row[col1_] = 1;
                        //    }
                        //}

                        {
                            var col3_ = row.Table.Columns[Tables.ColsStline.LINEEXP];

                            if (col3_ != null)
                            {
                                row[col3_] = desc_;
                            }
                        }
                    }

                    public static void setLineScripted(DataRow row, DataRow pByPromo)
                    {
                        string type_ = pByPromo[Tables.ColsDocSaving.TYPE].ToString();
                        string lref_ = pByPromo[Tables.ColsDocSaving.LOGICALREF].ToString();

                        if (lref_.Length > 20)
                            lref_ = lref_.Substring(0, 20);


                        string code_ = type_ + "." + lref_;


                        setLineScripted(row, code_);
                    }




                    public static int globalFirstPosition(DataTable pLines)
                    {
                        int indx_ = 0;
                        for (; indx_ < pLines.Rows.Count; ++indx_)
                            if (pLines.Rows[indx_].RowState != DataRowState.Deleted)
                            {
                                if (!isLineLocal(pLines.Rows[indx_]))
                                    return indx_;
                            }

                        return indx_;
                    }
                }


            }

            public class ToolType
            {

                public static object isNull(object val, object defVal)
                {
                    return isNull(val) ? defVal : val;
                }
                public static bool isNull(object val)
                {
                    return ((val == null) || (val.GetType() == typeof(DBNull)));
                }

                public static double strToDouble(string str)
                {
                    if (str == null || str.Trim() == string.Empty)
                        return 0;
                    str = str.Trim();
                    return double.Parse(str);
                }
                public static int strToInt(string str)
                {
                    if (str == null || str.Trim() == string.Empty)
                        return 0;
                    str = str.Trim();
                    return int.Parse(str);
                }
            }

            public class ToolTable
            {

                public static void setColumnValue(DataTable table, string col, object val)
                {
                    for (int i = 0; i < table.Rows.Count; ++i)
                        if (table.Rows[i].RowState != DataRowState.Deleted)
                            table.Rows[i][col] = val;
                }


            }


            public class CACHE_DOCSAVE
            {

                DataTable _DocSavingTable;
                DateTime version = new DateTime(1900, 1, 1);
                bool needUpdate()
                {
                    return ABS((DateTime.Now - version).TotalSeconds) > 10;
                }

                public DataTable getDocSavingTable(PluginTool pRoot)
                {
                    var res = _DocSavingTable;

                    if (res == null || needUpdate())
                    {


                        //if (pRoot.isMobile())
                        //    res = pRoot.SQL("select * from LG_$FIRM$_$PERIOD$_INFODOCSAVE order by LOGICALREF desc", null);
                        //else
                        {

                            var sql = @"

SELECT * FROM (

SELECT 
CODE AS LOGICALREF,SPECODE AS TYPE,SPECODE2 CODE,FLOATF1 AS CF1,FLOATF2 AS CF2,SPECODE3 AS PROMOMATCODE,SPECODE4 AS FILTERPROMOCL,TEXTF1 AS TEXT1,TEXTF2 AS TEXT2 
FROM 
LG_$FIRM$_CAMPAIGN AS H WITH(NOLOCK)
WHERE 
H.CARDTYPE = 2 AND 
H.ACTIVE = 0 AND 
@P1 BETWEEN H.BEGDATE AND H.ENDDATE
 
UNION ALL

SELECT   
CONCAT(H.CODE , ' [' , (CAST((L.LINENR) AS VARCHAR)) ,']') AS LOGICALREF,
H.SPECODE AS TYPE,
L.CONDITEMCODE AS CODE,
--(CASE WHEN L.FORMULA = '' THEN H.FLOATF1 ELSE (CAST (L.FORMULA AS FLOAT)) END ) AS CF1,
H.FLOATF1 AS CF1,
H.FLOATF2 AS CF2,
COALESCE(CASE WHEN L.LINETYPE=3 THEN (SELECT CODE FROM LG_$FIRM$_ITEMS WITH(NOLOCK) WHERE LOGICALREF=L.ITEMREF) ELSE '' END,'') AS PROMOMATCODE,
L.CONDITION AS FILTERPROMOCL,
L.FORMULA AS TEXT1,
L.LINEEXP AS TEXT2 
FROM LG_$FIRM$_CAMPAIGN AS H  WITH(NOLOCK) 
INNER JOIN 
LG_$FIRM$_CMPGNLINE L  WITH(NOLOCK)
ON H.LOGICALREF = L.CAMPCARDREF
WHERE
H.CARDTYPE = 2 AND 
H.ACTIVE = 0 AND 
@P1 BETWEEN H.BEGDATE AND H.ENDDATE  
) T
ORDER BY T.CODE DESC

";


                            res = pRoot.SQL(
                                sql
                                , new object[] { DateTime.Now.Date });
                        }


                        _DocSavingTable = res;
                        version = DateTime.Now;

                    }
                    return res;
                }



            }

            public class CACHE_ITEMS
            {


                IDictionary<string, DataRow> _itemsCache = new Dictionary<string, DataRow>();



                DateTime version = new DateTime(1900, 1, 1);
                bool needUpdate()
                {
                    return ABS((DateTime.Now - version).TotalSeconds) > 10;
                }

                public DataRow getItemRecord(PluginTool pRoot, object pMatLref)
                {
                    return _getItemRecord(pRoot, pMatLref, "");
                }

                DataRow _getItemRecord(PluginTool pRoot, object pMatLref, string pMatCode)
                {
                    var hasLref = !ISEMPTYLREF(pMatLref);
                    var hasCode = !ISEMPTY(pMatCode);

                    if (!hasLref && !hasCode)
                        return null;

                    var key = hasLref ? "lref::" + pMatLref : "code::" + pMatCode;

                    lock (_itemsCache)
                    {

                        if (needUpdate())
                        {
                            _itemsCache.Clear();
                            version = DateTime.Now;
                        }

                        DataRow tmpRow = null;
                        if (_itemsCache.ContainsKey(key))
                            tmpRow = _itemsCache[key];
                        else
                            if (hasLref)
                            {
                                _itemsCache[key] =
                                    tmpRow = TAB_GETLASTROW(pRoot.SQL("SELECT *,SPECODE AS STGRPCODETOP,SPECODE2 AS STGRPCODESUB FROM LG_$FIRM$_ITEMS WHERE LOGICALREF = @P1", new object[] { pMatLref }));
                            }
                            else
                                if (hasCode)
                                {
                                    _itemsCache[key] =
                                 tmpRow = TAB_GETLASTROW(pRoot.SQL("SELECT *,SPECODE AS STGRPCODETOP,SPECODE2 AS STGRPCODESUB FROM LG_$FIRM$_ITEMS WHERE CODE = @P1", new object[] { pMatCode }));
                                }


                        return tmpRow;

                    }
                }



                public object getItemRef(PluginTool pRoot, string itemCode)
                {
                    DataRow rec_ = _getItemRecord(pRoot, 0, itemCode);
                    if (rec_ != null)
                        return rec_[Tables.ColsItems.LOGICALREF];

                    return 0;

                }
                public string getItemCode(PluginTool pRoot, object itemId)
                {
                    DataRow rec_ = getItemRecord(pRoot, itemId);
                    if (rec_ != null)
                        return rec_[Tables.ColsItems.CODE].ToString();

                    return string.Empty;
                }



            }

            public class CACHE_CLIENT
            {


                IDictionary<object, DataRow> _clientCache = new Dictionary<object, DataRow>();


                DateTime version = new DateTime(1900, 1, 1);
                bool needUpdate()
                {
                    return ABS((DateTime.Now - version).TotalSeconds) > 10;
                }



                public string getClientCode(PluginTool pRoot, object clientId)
                {
                    DataRow rec_ = getClientRecord(pRoot, clientId);
                    if (rec_ != null)
                        return rec_[Tables.ColsClient.CODE].ToString();

                    return string.Empty;
                }

                public DataRow getClientRecord(PluginTool pRoot, object clientId)
                {
                    lock (_clientCache)
                    {
                        if (needUpdate())
                        {
                            _clientCache.Clear();
                            version = DateTime.Now;
                        }

                        DataRow tmpRow = null;
                        if (_clientCache.ContainsKey(clientId))
                            tmpRow = _clientCache[clientId];
                        else
                            _clientCache.Add(
                                clientId,
                                tmpRow = TAB_GETLASTROW(pRoot.SQL(
@"SELECT *, 
SPECODE4 AS FILTERPROMOCL,
DISCRATE AS DISCPER
FROM LG_$FIRM$_CLCARD WHERE LOGICALREF = @P1", new object[] { clientId }))
                                );



                        return tmpRow;
                    }
                }


            }



            public static bool MY_IS_MATH(string pOrg, string pPat)
            {
                return pOrg == pPat;
            }
        }


        #endregion

        #endregion