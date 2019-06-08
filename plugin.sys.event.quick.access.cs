#line 2


       #region BODY
        //BEGIN

        const int VERSION = 23;
        const string FILE = "plugin.sys.event.quick.access.pls";


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

                    x.MY_QUICKACCESS_USER = s.MY_QUICKACCESS_USER;

                    //

                    _SETTINGS.BUF = x;

                }

                public string MY_QUICKACCESS_USER;



            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC) //, "ava.production.config")
            {

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active On User")]
            public string MY_QUICKACCESS_USER
            {
                get
                {
                    return (_GET("MY_QUICKACCESS_USER", ""));
                }
                set
                {
                    _SET("MY_QUICKACCESS_USER", value);
                }

            }



            //

            public static bool ISUSEROK(_PLUGIN pPLUGIN)
            {
                return ISEMPTY(BUF.MY_QUICKACCESS_USER) || Array.IndexOf<string>(
                     EXPLODELIST(BUF.MY_QUICKACCESS_USER),
                     FORMAT(pPLUGIN.GETSYSPRM_USER())
                     ) >= 0;
            }

        }

        #endregion

        #region TEXT

        public class TEXT
        {
            public const string text_DESC = "Quick Access";


            public class L
            {

            }
        }

        const string event_QUICKACCESS_ = "hadlericom_quickaccess_";
        const string event_QUICKACCESS_CLIENT_TRAN_REP = "hadlericom_quickaccess_cl_tran_rep";
        const string event_QUICKACCESS_BANKACC_TRAN_REP = "hadlericom_quickaccess_bankacc_tran_rep";
        const string event_QUICKACCESS_CASH_TRAN_REP = "hadlericom_quickaccess_cash_tran_rep";
        const string event_QUICKACCESS_CASH_DOC_REP = "hadlericom_quickaccess_cash_doc_rep";
        const string event_QUICKACCESS_CASH_OPER = "hadlericom_quickaccess_cash_oper";
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



            //  public string A = "A";


            public void lang_az()
            {
            }

            public void lang_ru()
            {

            }

            public void lang_tr()
            {
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
            try
            {

                var fn = GETFORMNAME(FORM);
                var cPanelBtnSub = CONTROL_SEARCH(FORM, "cPanelBtnSub");

                if (cPanelBtnSub == null)
                    return;


                if (fn == "ref.fin.rec.client")
                {

                    if (EXECMDTEXTALLOWED("rep loc::rep004030003"))
                        _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_QUICKACCESS_CLIENT_TRAN_REP, LANG("T_MENU_004030003_EXTRACT"), "report_16x16");

                    return;
                }

                if (fn == "ref.fin.rec.cash")
                {
                    if (EXECMDTEXTALLOWED("rep loc::rep004030004"))
                        _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_QUICKACCESS_CASH_TRAN_REP, LANG("T_MENU_004030004_EXTRACT"), "report_16x16");

                    if (EXECMDTEXTALLOWED("ref.fin.doc.cash"))
                        _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_QUICKACCESS_CASH_OPER, LANG("T_REF_FIN_DOC_CASH"), "run_16x16");

                    return;
                }

                if (fn == "ref.fin.doc.cash")
                {

                    if (EXECMDTEXTALLOWED("rep loc::rep004030015"))
                        _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_QUICKACCESS_CASH_DOC_REP, LANG("T_MENU_004030015"), "doc_16x16");


                    return;
                }

                if (fn == "ref.fin.rec.bankacc")
                {
                    if (EXECMDTEXTALLOWED("rep loc::rep004030006"))
                        _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_QUICKACCESS_BANKACC_TRAN_REP, LANG("T_MENU_004030006_EXTRACT"), "report_16x16");

                    return;
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
			{ "Text" ,( pText)},
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


                    //case event_QUICKACCESS_MONTH:
                    //    {
                    //        QUICKACCESS_MONTH(arg1 as DataRow);
                    //    }
                    //    break;

                    //case event_QUICKACCESS_RANGE:
                    //    {
                    //        QUICKACCESS_RANGE(arg1 as DataRow);
                    //    }
                    //    break;

                    case event_QUICKACCESS_CASH_OPER:
                        {
                            DataRow row = MY_GET_REF_EVENT_DATA_REC(arg1);
                            if (row != null)
                            {
                                switch (row.Table.TableName)
                                {
                                    case "KSCARD":
                                        {
                                            string cmd_ = "ref.fin.doc.cash " +
                    "filter::" +
                    "filter_CARDREF," + FORMATSERIALIZE(TAB_GETROW(row, "LOGICALREF")) + ";" +
                      "";
                                            REFNORES(cmd_);
                                        }
                                        break;
                                }
                            }
                        }
                        break;


                    case event_QUICKACCESS_CASH_DOC_REP:
                    case event_QUICKACCESS_CLIENT_TRAN_REP:
                    case event_QUICKACCESS_CASH_TRAN_REP:
                    case event_QUICKACCESS_BANKACC_TRAN_REP:
                        {

                            DataRow row = MY_GET_REF_EVENT_DATA_REC(arg1);
                            if (row != null)
                            {
                                switch (row.Table.TableName)
                                {
                                    case "CLCARD":
                                        {
                                            string cmd_ = "rep loc::rep004030003 " +
                    "filter::" +
                                                // "filter_CLFLINE_DATE_," + FORMATSERIALIZE(GETSYSPRM_PERIODBEG().Date) + "," + FORMATSERIALIZE(DateTime.Now.Date) + ";" +
                    "filter_CLFLINE_CLIENTREF," + FORMATSERIALIZE(TAB_GETROW(row, "LOGICALREF")) + ";" +
                      "";// " REP_QUICK_MODE_B::1 REP_DEF_FILTER_B::1";
                                            //  if (EXECMDTEXTALLOWED(cmd_))
                                            EXECMDTEXT(cmd_);
                                        }
                                        break;
                                    case "KSCARD":
                                        {
                                            string cmd_ = "rep loc::rep004030004 " +
                    "filter::" +
                                                // "filter_CLFLINE_DATE_," + FORMATSERIALIZE(GETSYSPRM_PERIODBEG().Date) + "," + FORMATSERIALIZE(DateTime.Now.Date) + ";" +
                    "filter_KSLINES_CARDREF," + FORMATSERIALIZE(TAB_GETROW(row, "LOGICALREF")) + ";" +
                      "";// " REP_QUICK_MODE_B::1 REP_DEF_FILTER_B::1";
                                            //  if (EXECMDTEXTALLOWED(cmd_))
                                            EXECMDTEXT(cmd_);
                                        }
                                        break;
                                    case "BANKACC":
                                        {
                                            string cmd_ = "rep loc::rep004030006 " +
                    "filter::" +
                                                // "filter_CLFLINE_DATE_," + FORMATSERIALIZE(GETSYSPRM_PERIODBEG().Date) + "," + FORMATSERIALIZE(DateTime.Now.Date) + ";" +
                    "filter_BNFLINE_BNACCREF," + FORMATSERIALIZE(TAB_GETROW(row, "LOGICALREF")) + ";" +
                      "";// " REP_QUICK_MODE_B::1 REP_DEF_FILTER_B::1";
                                            //  if (EXECMDTEXTALLOWED(cmd_))
                                            EXECMDTEXT(cmd_);
                                        }
                                        break;


                                    case "KSLINES":
                                        {

                                            string cmd_ = "rep loc::rep004030015 " +
                    "filter::" +

                    "filter_KSLINES_LOGICALREF," + FORMATSERIALIZE(TAB_GETROW(row, "LOGICALREF")) + ";" +
                      "";// " REP_QUICK_MODE_B::1 REP_DEF_FILTER_B::1";
                                            //  if (EXECMDTEXTALLOWED(cmd_))
                                            EXECMDTEXT(cmd_);
                                        }
                                        break;
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



        #region TOOLS

        static DataRow MY_GET_REF_EVENT_DATA_REC(object pArg)
        {

            DataRow row = pArg as DataRow;

            if (row == null)
            {
                var grid_ = CONTROL_SEARCH(pArg as Form, "cGrid") as DataGridView;
                if (grid_ != null)
                    row = TOOL_GRID.GET_GRID_ROW_DATA(grid_);
            }
            return row;

        }

        #endregion



        #endregion
