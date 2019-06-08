#line 2


 #region PLUGIN_BODY
        const int VERSION = 11;

        /*
         Send Allto GL
         */
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

                    x.MY_TAXINTEGAZ_USER = s.MY_TAXINTEGAZ_USER;

                    _SETTINGS.BUF = x;

                }

                public string MY_TAXINTEGAZ_USER;

            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC)
            {

            }



            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active On User")]
            public string MY_TAXINTEGAZ_USER
            {
                get
                {
                    return (_GET("MY_TAXINTEGAZ_USER", "1,2"));
                }
                set
                {
                    _SET("MY_TAXINTEGAZ_USER", value);
                }

            }



            public static bool ISUSEROK(_PLUGIN pPLUGIN)
            {
                return Array.IndexOf<string>(
                     EXPLODELIST(BUF.MY_TAXINTEGAZ_USER),
                     FORMAT(pPLUGIN.GETSYSPRM_USER())
                     ) >= 0;
            }
        }

        #endregion
        #region TEXT

        public class TEXT
        {
            public const string text_DESC = "T_GL";

        }
        #endregion

        #region MAIN

        const string event_GLDO_ = "com_hadleri_gldo_";
        const string event_GLDO_SEND_ = "com_hadleri_gldo_send_";

        const string event_GLDO_MULTI_ = "com_hadleri_gldo_multi_";
        const string event_GLDO_MULTI_DONE = "com_hadleri_gldo_multi_done";
        const string event_GLDO_MULTI_UNDONE = "com_hadleri_gldo_multi_undone";

        //const string event_GLDO_SEND_ALL = "com_hadleri_gldo_send_all";
        //const string event_GLDO_SEND_MM = "com_hadleri_gldo_send_mm";
        //const string event_GLDO_SEND_PRCH = "com_hadleri_gldo_send_prch";
        //const string event_GLDO_SEND_SLS = "com_hadleri_gldo_send_sls";
        //const string event_GLDO_SEND_CASH = "com_hadleri_gldo_send_cash";
        //const string event_GLDO_SEND_BANK = "com_hadleri_gldo_send_bank";
        //const string event_GLDO_SEND_PERS = "com_hadleri_gldo_send_pers";

        const string event_GLDO_DOC_FIND_ = "com_hadleri_gldo_doc_find_";
        const string event_GLDO_DOC_FIND_DOC2GL = "com_hadleri_gldo_doc_find_doc2gl";
        const string event_GLDO_DOC_FIND_GL2DOC = "com_hadleri_gldo_doc_find_gl2doc";



        readonly static Dictionary<string, string> docs_modules_map = new Dictionary<string, string>(){
        {"mm","T_MENU_001020004"},
        {"prch","T_MENU_002020012"},
        {"sls","T_MENU_003020013"},
        {"cash","T_MENU_004020012"},
        {"bank","T_MENU_004020011"},
        {"pers","T_MENU_004020009"},
   
        };


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
                    _MY_SYS_NEWFORM_INTEGRATE_MAINFORM(arg1 as Form);
                    break;
                case SysEvent.SYS_USEREVENT:
                    MY_SYS_USEREVENT_HANDLER(EVENTCODE, ARGS);
                    break;




            }



        }



        void _MY_SYS_NEWFORM_INTEGRATE_MAINFORM(Form FORM)
        {
            if (FORM == null)
                return;

            if (!EXEADPCMDALLOWED("adp.gl.doc.slip.4 cmd::add", null))
                return;


            var fn = GETFORMNAME(FORM);
            if (fn == "form.app")
            {





                var tree = CONTROL_SEARCH(FORM, "cTreeTools");
                if (tree == null)
                    return;





                var args = new Dictionary<string, object>() {            
			        { "_cmd" ,""},
                    { "_type" ,""},
			       // { "CmdText" ,""},
			        { "Text" ,"T_GL"},
			        { "ImageName" ,"gl_32x32"},
			        { "Name" ,event_GLDO_},
                };

                RUNUIINTEGRATION(tree, args);


                {
                    args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
			{ "_root" ,event_GLDO_},
			{ "CmdText" ,"event name::" + event_GLDO_MULTI_DONE},
			{ "Text" ,"T_GL_DO"},
			{ "ImageName" ,"gl_32x32"},
			{ "Name" , event_GLDO_MULTI_DONE},
            };

                    RUNUIINTEGRATION(tree, args);
                }

                {
                    args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
			{ "_root" ,event_GLDO_},
			{ "CmdText" ,"event name::" + event_GLDO_MULTI_UNDONE},
			{ "Text" ,"T_GL_UNDO"},
			{ "ImageName" ,"ungl_32x32"},
			{ "Name" , event_GLDO_MULTI_UNDONE},
            };

                    RUNUIINTEGRATION(tree, args);
                }


        //        var arr1 = new string[] { 
        //event_GLDO_SEND_ALL ,
        //event_GLDO_SEND_MM ,
        //event_GLDO_SEND_PRCH ,
        // event_GLDO_SEND_SLS,
        // event_GLDO_SEND_CASH ,
        // event_GLDO_SEND_BANK ,
        //event_GLDO_SEND_PERS  
        //    };
        //        var arr2 = new string[] { 
        //        "T_ALL",
        //         "T_MENU_001020004",
        //         "T_MENU_002020012",
        //         "T_MENU_003020013",
        //         "T_MENU_004020012",
        //         "T_MENU_004020011",
        //         "T_MENU_004020009",
            
        //    };
        //        var arr3 = new string[] { 
        //        "all_32x32",
        //        "mm_32x32",
        //        "prch_32x32",
        //        "sls_32x32",
        //        "money_32x32",
        //        "bank_32x32",
        //        "client_32x32",
       
 
        //    };

            //    for (int i = 0; i < arr1.Length; ++i)
            //    {
            //        args = new Dictionary<string, object>() { 
 
            //{ "_cmd" ,""},
            //{ "_type" ,""},
            //{ "_root" ,event_GLDO_},
            //{ "CmdText" ,"event name::" + arr1[i]},
            //{ "Text" ,arr2[i]},
            //{ "ImageName" ,arr3[i]},
            //{ "Name" , arr1[i]},
            //};

            //        RUNUIINTEGRATION(tree, args);
            //    }

                return;
            }



            if (
                (fn == "ref.mm.doc.slip") ||
                (fn == "ref.prch.doc.inv") ||
                (fn == "ref.sls.doc.inv") ||
                (fn == "ref.fin.doc.cash") ||
                (fn == "ref.fin.doc.bank") ||
                (fn == "ref.fin.doc.client") ||
                //
                (fn == "ref.gl.doc.slip")
                )
            {
                 var isGlList = (fn == "ref.gl.doc.slip");


                 var cPanelBtnSub = CONTROL_SEARCH(FORM, "cPanelBtnSub");
                 if (cPanelBtnSub != null)
                 {

                     if (isGlList)
                     {
                         _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_GLDO_DOC_FIND_GL2DOC, LANG("T_DOC"), "doc_16x16");
                     }
                     else
                     {
                         _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_GLDO_DOC_FIND_DOC2GL, LANG("T_DOC (T_GL)"), "gl_16x16");
                     }
                 }


               



                return;
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
            //"SYS_USEREVENT/exchange_rates_tcmb"

            if (!EVENTCODE.StartsWith("SYS_USEREVENT/" + event_GLDO_))
                return;

            try
            {

                object arg1 = ARGS.Length > 0 ? ARGS[0] : null;
                object arg2 = ARGS.Length > 1 ? ARGS[1] : null;

                string[] list_ = EXPLODELISTPATH(EVENTCODE);

                var cmd_ = list_.Length > 1 ? list_[1].ToLowerInvariant() : "";


                if (cmd_.StartsWith(event_GLDO_DOC_FIND_))
                {
                    switch (cmd_)
                    {
                        case event_GLDO_DOC_FIND_DOC2GL:
                        case event_GLDO_DOC_FIND_GL2DOC:
                            {
                                 DataRow row = MY_GET_REF_EVENT_DATA_REC(arg1);
                                 if (row != null)
                                 {
                                     MY_FIND_DOC(row);
                                 }
 
                            }
                            break;
                    }
                }

                //if (cmd_.StartsWith(event_GLDO_SEND_))
                //{
                //    if (!MSGUSERASK("T_MSG_COMMIT_BEGIN - T_GL_DO"))
                //        return;


                //    var docsCount = 0;
                //    var errCount = 0;
                //    switch (cmd_)
                //    {

                //        case event_GLDO_SEND_ALL:
                //            var range = ASK_DATE_RANGE(null);
                //            if (range != null)
                //            {
                //                MY_DO_GL("mm", range, ref docsCount, ref errCount);
                //                MY_DO_GL("prch", range, ref docsCount, ref errCount);
                //                MY_DO_GL("sls", range, ref docsCount, ref errCount);
                //                MY_DO_GL("cash", range, ref docsCount, ref errCount);
                //                MY_DO_GL("bank", range, ref docsCount, ref errCount);
                //                MY_DO_GL("pers", range, ref docsCount, ref errCount);
                //            }
                //            break;
                //        case event_GLDO_SEND_MM:
                //            MY_DO_GL("mm", null, ref docsCount, ref errCount);
                //            break;
                //        case event_GLDO_SEND_PRCH:
                //            MY_DO_GL("prch", null, ref docsCount, ref errCount);
                //            break;
                //        case event_GLDO_SEND_SLS:
                //            MY_DO_GL("sls", null, ref docsCount, ref errCount);
                //            break;
                //        case event_GLDO_SEND_CASH:
                //            MY_DO_GL("cash", null, ref docsCount, ref errCount);
                //            break;
                //        case event_GLDO_SEND_BANK:
                //            MY_DO_GL("bank", null, ref docsCount, ref errCount);
                //            break;
                //        case event_GLDO_SEND_PERS:
                //            MY_DO_GL("pers", null, ref docsCount, ref errCount);
                //            break;

                //        default:
                //            return;
                //    }

                //    var sufix = string.Format(" [T_QUANTITY = {0}/{1}]", docsCount, errCount);
                //    if (docsCount > 0 && errCount == 0)
                //        MSGUSERINFO("T_MSG_OPERATION_OK" + sufix);
                //    else
                //        if (errCount > 0)
                //            MSGUSERERROR("T_MSG_OPERATION_FAILED" + sufix);
                //        else
                //            MSGUSERINFO("T_MSG_OPERATION_FINISHED" + sufix);

                //}

                if (cmd_.StartsWith(event_GLDO_MULTI_))
                {

                    var docsCount = 0;
                    var errCount = 0;
                    switch (cmd_)
                    {
                        case event_GLDO_MULTI_DONE:
                            if (MSGUSERASK("T_MSG_COMMIT_BEGIN - T_GL_DO"))
                                if (!MY_DONE_GL(ref docsCount, ref errCount))
                                    return;
                            break;
                        case event_GLDO_MULTI_UNDONE:
                            if (MSGUSERASK("T_MSG_COMMIT_BEGIN - T_GL_UNDO") && MY_ASK_STRING(this, "T_MSG_COMMIT_DELETE", "") == "ok")
                                if (!MY_UNDONE_GL(ref docsCount, ref errCount))
                                    return;
                            break;

                        default:
                            return;
                    }

                    var sufix = string.Format(" [T_QUANTITY = {0}/{1}]", docsCount, errCount);
                    if (docsCount > 0 && errCount == 0)
                        MSGUSERINFO("T_MSG_OPERATION_OK" + sufix);
                    else
                        if (errCount > 0)
                            MSGUSERERROR("T_MSG_OPERATION_FAILED" + sufix);
                        else
                            MSGUSERINFO("T_MSG_OPERATION_FINISHED" + sufix);

                }
            }
            catch (Exception exc)
            {
                LOG(exc);
                MSGUSERERROR("T_MSG_OPERATION_FAILED");
                MSGUSERERROR(exc.Message);
            }
        }


        void MY_FIND_DOC(DataRow pRow)
        {
            if (pRow == null)
                return;

            var data = pRow.Table;

            var hasAccounted = data.Columns.Contains("ACCOUNTED");
            var hasGlDocRef = data.Columns.Contains("ACCFICHEREF");
            var hasLRef = data.Columns.Contains("LOGICALREF");

            if (data.Columns.Contains("ACCOUNTED") && data.Columns.Contains("ACCFICHEREF"))
            {
                var accounted = ISTRUE(TAB_GETROW(pRow, "ACCOUNTED"));
                var lref = CASTASINT(TAB_GETROW(pRow, "ACCFICHEREF"));
                if (accounted && !ISEMPTYLREF(lref))
                {
                    EXECMDTEXT("adp.gl.doc.slip.4 cmd::edit lref::" + lref);
                }
            }
            else
                if (data.Columns.Contains("LOGICALREF") && data.TableName == "EMFICHE")
                {
                    var lref = CASTASINT(TAB_GETROW(pRow, "LOGICALREF"));

                    if (!ISEMPTYLREF(lref))
                    {

                        var rec = TAB_GETLASTROW(SQL(@"
    SELECT 'INVOICE' AS RECTYPE,TRCODE,LOGICALREF FROM LG_$FIRM$_$PERIOD$_INVOICE WHERE ACCOUNTED = 1 AND ACCFICHEREF = @P1
    UNION ALL
    SELECT 'BNFICHE' AS RECTYPE,TRCODE,LOGICALREF FROM LG_$FIRM$_$PERIOD$_BNFICHE WHERE ACCOUNTED = 1 AND ACCFICHEREF = @P1
    UNION ALL
    SELECT 'KSLINES' AS RECTYPE,TRCODE,LOGICALREF FROM LG_$FIRM$_$PERIOD$_KSLINES WHERE ACCOUNTED = 1 AND ACCFICHEREF = @P1
    UNION ALL
    SELECT 'CLFICHE' AS RECTYPE,TRCODE,LOGICALREF FROM LG_$FIRM$_$PERIOD$_CLFICHE WHERE ACCOUNTED = 1 AND ACCFICHEREF = @P1
 

    ", new object[] { lref }));


                        if (rec != null)
                        {
                            var RECTYPE = CASTASSTRING(TAB_GETROW(rec, "RECTYPE"));
                            var TRCODE = CASTASSHORT(TAB_GETROW(rec, "TRCODE"));
                            var LOGICALREF = CASTASINT(TAB_GETROW(rec, "LOGICALREF"));

                            rec.Table.TableName = RECTYPE;

                            var adp = RECORDADP(rec);

                            if (!ISEMPTY(adp))
                            {
                                EXECMDTEXT(adp + " cmd::view lref::" + LOGICALREF);
                            }

                        }


                        //  EXECMDTEXT("adp.gl.doc.slip.4 cmd::view lref::" + lref);
                    }


                }


            // pRow.Table.Columns.Contains( "ACCOUNT" )
            //   TAB_GETROW(pRow,"")
        }


        public static string MY_ASK_STRING(_PLUGIN pPLUGIN, string pMsg, string pDef, string pCmdAppend = null)
        {

            DataRow[] rows_ = pPLUGIN.REF("ref.gen.string " + (ISEMPTY(pCmdAppend) ? "" : pCmdAppend) + "desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDef));
            if (rows_ != null && rows_.Length > 0)
            {
                return _PLUGIN.CASTASSTRING(ISNULL(rows_[0]["VALUE"], ""));
            }
            return null;

        }
        public DateTime[] ASK_DATE_RANGE(string pMsg)//, DateTime pDfDef, DateTime pDtDef)
        {
            pMsg = pMsg ?? "T_DATE_RANGE";
            //
            DataRow[] rows_ = REF("ref.gen.daterange desc::" + STRENCODE(pMsg)
                // + (pDfDef.Year != 1900 && pDtDef.Year != 1900 ? "" : " filter::filter_DATE1," + FORMATSERIALIZE(pDfDef) + "filter_DATE2," + FORMATSERIALIZE(pDtDef))
                );
            if (rows_ != null && rows_.Length > 0)
            {
                var d1 = CASTASDATE(rows_[0]["DATE1"]);
                var d2 = CASTASDATE(rows_[0]["DATE2"]);
                if (d1 > d2)
                    return null;

                return new DateTime[] { d1, d2 };
            }
            return null;

        }
        void MY_DO_GL(string pCode, DateTime[] pRange, ref int pTotalDocs, ref int pTotalErr)
        {

            var r = (pRange == null ? ASK_DATE_RANGE("T_DATE_RANGE") : pRange);
            if (r == null)
                return;
            pRange = r;

            string docTypeDesc = null;
            DataTable tableDataDocs = MY_GET_GL_DOCS(pCode, pRange, ref docTypeDesc);
            if (tableDataDocs == null)
                return;

            foreach (DataRow doc in tableDataDocs.Rows)
            {

                var trcode = CASTASSHORT(ISNULL(TAB_GETROW(doc, "TRCODE"), 0));
                var date = CASTASDATE((TAB_GETROW(doc, "DATE_")));
                var lref = ((TAB_GETROW(doc, "LOGICALREF")));
                var nr = CASTASSTRING((TAB_GETROW(doc, "FICHENO")));

                var adp = RECORDADP(doc) + " lref::" + FORMAT(lref);


                try
                {
                    ++pTotalDocs;

                    INVOKEINBATCH((s, a) =>
                    {
                        EXEADPCMD(adp + " cmd::gl", true);
                    }, null);

                }
                catch (Exception exc)
                {
                    ++pTotalErr;


                    JOURNAL("gl",
                        string.Format("{0}:{1}:{2}:{3}:{4}",
                        docTypeDesc,
                        trcode,
                        nr,
                        LEFT(FORMAT(date), 10),
                        UNWRAPEXEPTION(exc).Message),
                        string.Format("[CMD::{0}] [FIRMINDEX::{1}]", adp + " cmd::edit", GETSYSPRM_FIRMINDEX()));
                }




            }
            //



        }




        bool MY_DONE_GL(ref int pTotalDocs, ref int pTotalErr)
        {
            DateTime[] pRange = null;

            var r = (pRange == null ? ASK_DATE_RANGE("T_DATE_RANGE") : pRange);
            if (r == null)
                return false;
            pRange = r;

            var dateFrom = pRange[0].Date;
            var dateTo = pRange[1].Date;

            if ((dateTo - dateFrom).TotalDays > 31)
                EXCEPTIONFORUSER("T_MSG_INVALID_DATETIME [T_DAYS > 31]");


            var list = new List<string>();
            var filter_args_all = "filter::";
            foreach (var key in docs_modules_map.Keys)
            {
                list.AddRange(new string[] { key, docs_modules_map[key] });
                filter_args_all = filter_args_all + "filter_VALUE," + FORMATSERIALIZE(key) + ";";
            }
            //
            var res_ = REF(
                "ref.gen.definedlist [obj::" + JOINLIST(list.ToArray()) + "] [desc::T_DOCS] " +
                "[" + filter_args_all + "] " +
                "[multi::1] [checkbox::1] [type::string] ");

            if (res_ == null || res_.Length == 0)
                return false;


            while (dateFrom <= dateTo)
            {

                foreach (var rec in res_)
                {
                    var docCode = CASTASSTRING(TAB_GETROW(rec, "VALUE"));
                    string docTypeDesc = null;
                    short accounted = 0;
                    DataTable tableDataDocs = MY_GET_GL_DOCS(docCode, new DateTime[] { dateFrom, dateFrom }, ref docTypeDesc, accounted);
                    if (tableDataDocs == null)
                        continue;


                    foreach (DataRow doc in tableDataDocs.Rows)
                    {

                        var trcode = CASTASSHORT(ISNULL(TAB_GETROW(doc, "TRCODE"), 0));
                        var date = CASTASDATE((TAB_GETROW(doc, "DATE_")));
                        var lref = ((TAB_GETROW(doc, "LOGICALREF")));
                        var nr = CASTASSTRING((TAB_GETROW(doc, "FICHENO")));

                        var adp = RECORDADP(doc) + " lref::" + FORMAT(lref);



                        try
                        {
                            ++pTotalDocs;

                            INVOKEINBATCH((s, a) =>
                            {
                                EXEADPCMD(adp + " cmd::gl", true);
                            }, null);

                        }
                        catch (Exception exc)
                        {
                            ++pTotalErr;


                            JOURNAL("gl",
                                string.Format("{0}:{1}:{2}:{3}:{4}",
                                docTypeDesc,
                                trcode,
                                nr,
                                LEFT(FORMAT(date), 10),
                                UNWRAPEXEPTION(exc).Message),
                                string.Format("[CMD::{0}] [FIRMINDEX::{1}]", adp + " cmd::edit", GETSYSPRM_FIRMINDEX()));
                        }



                    }


                }

                dateFrom = dateFrom.AddDays(+1);

            }

            return true;


        }
        bool MY_UNDONE_GL(ref int pTotalDocs, ref int pTotalErr)
        {
            DateTime[] pRange = null;

            var r = (pRange == null ? ASK_DATE_RANGE("T_DATE_RANGE") : pRange);
            if (r == null)
                return false;
            pRange = r;

            var dateFrom = pRange[0].Date;
            var dateTo = pRange[1].Date;

            if ((dateTo - dateFrom).TotalDays > 31)
                EXCEPTIONFORUSER("T_MSG_INVALID_DATETIME [T_DAYS > 31]");


            var list = new List<string>();
            var filter_args_all = "filter::";
            foreach (var key in docs_modules_map.Keys)
            {
                list.AddRange(new string[] { key, docs_modules_map[key] });
                filter_args_all = filter_args_all + "filter_VALUE," + FORMATSERIALIZE(key) + ";";
            }
            //
            var res_ = REF(
                "ref.gen.definedlist [obj::" + JOINLIST(list.ToArray()) + "] [desc::T_DOCS] " +
                "[" + filter_args_all + "] " +
                "[multi::1] [checkbox::1] [type::string] ");

            if (res_ == null || res_.Length == 0)
                return false;


            while (dateFrom <= dateTo)
            {

                foreach (var rec in res_)
                {
                    var docCode = CASTASSTRING(TAB_GETROW(rec, "VALUE"));
                    string docTypeDesc = null;
                    short accounted = 1;
                    DataTable tableDataDocs = MY_GET_GL_DOCS(docCode, new DateTime[] { dateFrom, dateFrom }, ref docTypeDesc, accounted);
                    if (tableDataDocs == null)
                        continue;


                    foreach (DataRow doc in tableDataDocs.Rows)
                    {

                        var trcode = CASTASSHORT(ISNULL(TAB_GETROW(doc, "TRCODE"), 0));
                        var date = CASTASDATE((TAB_GETROW(doc, "DATE_")));
                        var lref = ((TAB_GETROW(doc, "LOGICALREF")));
                        var nr = CASTASSTRING((TAB_GETROW(doc, "FICHENO")));

                        var adp = RECORDADP(doc) + " lref::" + FORMAT(lref);



                        try
                        {
                            ++pTotalDocs;

                            INVOKEINBATCH((s, a) =>
                            {
                                EXEADPCMD(adp + " cmd::ungl", true);
                            }, null);

                        }
                        catch (Exception exc)
                        {
                            ++pTotalErr;


                            JOURNAL("gl",
                                string.Format("{0}:{1}:{2}:{3}:{4}",
                                docTypeDesc,
                                trcode,
                                nr,
                                LEFT(FORMAT(date), 10),
                                UNWRAPEXEPTION(exc).Message),
                                string.Format("[CMD::{0}] [FIRMINDEX::{1}]", adp + " cmd::edit", GETSYSPRM_FIRMINDEX()));
                        }



                    }


                }

                dateFrom = dateFrom.AddDays(+1);

            }

            return true;
        }

        DataTable MY_GET_GL_DOCS(string pCode, DateTime[] pRange, ref string pDesc, short pAccounted=0)
        {
            var r = pRange == null ? ASK_DATE_RANGE("T_DATE_RANGE") : pRange;
            if (r == null)
                return null;


            DataTable tableDataDocs = null;

            short accounted = pAccounted;

            //
            switch (pCode)
            {
                case "mm":
                    {
                        pDesc = docs_modules_map[pCode];

                        tableDataDocs = SQL(@"
SELECT 
LOGICALREF,
TRCODE,
DATE_,
FICHENO 
FROM 
LG_$FIRM$_$PERIOD$_STFICHE --$MS$--WITH(NOLOCK,INDEX=I$FIRM$_$PERIOD$_STFICHE_I3) 
WHERE 
GRPCODE = 3 AND DATE_ BETWEEN @P1 AND @P2 AND FTIME >=0 AND IOCODE IN (1,2,3,4) AND 
TRCODE IN (
11,12,13,
25,50,51,
15,16,17,18,19,
20,21,22,23,24
) 
AND ACCOUNTED = @P3 AND CANCELLED = 0
ORDER BY GRPCODE,DATE_,FTIME,IOCODE,TRCODE
", new object[] { pRange[0], pRange[1], accounted });
                        tableDataDocs.TableName = "STFICHE";
                    }
                    break;
                case "sls":
                    {
                        pDesc = docs_modules_map[pCode];
                        tableDataDocs = SQL(@"
SELECT 
LOGICALREF,
TRCODE,
DATE_,
FICHENO
FROM 
LG_$FIRM$_$PERIOD$_INVOICE --$MS$--WITH(NOLOCK,INDEX=I$FIRM$_$PERIOD$_INVOICE_I3) 
WHERE 
GRPCODE = 2 AND DATE_ BETWEEN @P1 AND @P2 AND TIME_ >=0 AND  
TRCODE IN (
2,3,7,8,9
)
AND ACCOUNTED = @P3 AND CANCELLED = 0
ORDER BY GRPCODE,DATE_,TIME_,TRCODE
", new object[] { pRange[0], pRange[1], accounted });
                        tableDataDocs.TableName = "INVOICE";
                    }
                    break;
                case "prch":
                    {
                        pDesc = docs_modules_map[pCode];
                        tableDataDocs = SQL(@"
SELECT 
LOGICALREF,
TRCODE,
DATE_,
FICHENO 
FROM 
LG_$FIRM$_$PERIOD$_INVOICE --$MS$--WITH(NOLOCK,INDEX=I$FIRM$_$PERIOD$_INVOICE_I3) 
WHERE 
GRPCODE = 1 AND DATE_ BETWEEN @P1 AND @P2 AND TIME_ >=0 AND  
TRCODE IN (
1,4,6
)
AND ACCOUNTED = @P3 AND CANCELLED = 0
ORDER BY GRPCODE,DATE_,TIME_,TRCODE
", new object[] { pRange[0], pRange[1], accounted });
                        tableDataDocs.TableName = "INVOICE";
                    }
                    break;
                case "cash":
                    {
                        pDesc = docs_modules_map[pCode];
                        tableDataDocs = SQL(@"
SELECT 
LOGICALREF,
TRCODE,
DATE_,
FICHENO 
FROM 
LG_$FIRM$_$PERIOD$_KSLINES --$MS$--WITH(NOLOCK,INDEX=I$FIRM$_$PERIOD$_KSLINES_I2) 
WHERE 
CARDREF > 0 AND DATE_ BETWEEN @P1 AND @P2 AND 
TRCODE IN (
11,12,21,22,73,74,79,80
)
AND ACCOUNTED = @P3 AND CANCELLED = 0
ORDER BY CARDREF,DATE_,TRCODE
", new object[] { pRange[0], pRange[1], accounted });
                        tableDataDocs.TableName = "KSLINES";
                    }
                    break;
                case "bank":
                    {
                        pDesc = docs_modules_map[pCode];
                        tableDataDocs = SQL(@"
SELECT 
LOGICALREF,
TRCODE,
DATE_,
FICHENO 
FROM 
LG_$FIRM$_$PERIOD$_BNFICHE --$MS$--WITH(NOLOCK,INDEX=I$FIRM$_$PERIOD$_BNFICHE_I3) 
WHERE 
DATE_ BETWEEN @P1 AND @P2 AND 
TRCODE IN (
1,2,3,4,6
)
AND ACCOUNTED = @P3 AND CANCELLED = 0
ORDER BY DATE_,TRCODE
", new object[] { pRange[0], pRange[1], accounted });
                        tableDataDocs.TableName = "BNFICHE";
                    }
                    break;
                case "pers":
                    {
                        pDesc = docs_modules_map[pCode];
                        tableDataDocs = SQL(@"
SELECT 
LOGICALREF,
TRCODE,
DATE_,
FICHENO 
FROM 
LG_$FIRM$_$PERIOD$_CLFICHE --$MS$--WITH(NOLOCK,INDEX=I$FIRM$_$PERIOD$_CLFICHE_I3) 
WHERE 
DATE_ BETWEEN @P1 AND @P2 AND
TRCODE IN (
1,2,3,4,5,6,41,42
)
AND ACCOUNTED = @P3 AND CANCELLED = 0
ORDER BY DATE_,TRCODE
", new object[] { pRange[0], pRange[1], accounted });
                        tableDataDocs.TableName = "CLFICHE";
                    }
                    break;


            }
            return tableDataDocs;
        }
        #endregion



        #region CLAZZ


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


        #endregion