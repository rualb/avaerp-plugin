#line 2

 

     #region ITS
        //BEGIN

        const int VERSION = 23;

        #region SETTINGS

        static bool ITS_PRM_OK = false;
        static string ITS_USER_CODE;
        static string ITS_USER_PASSWD;
        static string ITS_WH_CODE;
        static string ITS_WH_DESC;
        static TOOL_ITS.WH_TYPE ITS_WH_TYPE;
        static int MY_ITS_GOOD_ITEM_MIN_LIFE;

        static void LOAD_SETTINGS(_PLUGIN PLUGIN)
        {
            if (ITS_PRM_OK)
                return;
            var s = new _SETTINGS(PLUGIN);

            ITS_USER_CODE = s.MY_ITS_USER_CODE;
            ITS_USER_PASSWD = s.MY_ITS_USER_PASSWD;
            ITS_WH_CODE = s.MY_ITS_WH_CODE;
            ITS_WH_DESC = s.MY_ITS_WH_NAME;
            ITS_WH_TYPE = (TOOL_ITS.WH_TYPE)Convert.ToInt32(s.MY_ITS_WH_TYPE);
            MY_ITS_GOOD_ITEM_MIN_LIFE = s.MY_ITS_GOOD_ITEM_MIN_LIFE;
            //
            ITS_PRM_OK = true;
        }


        class _SETTINGS : TOOL_SETTINGS
        {
            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, "ITS")
            {

            }

            [ECategory(TEXT.text_ITS)]
            [EDisplayName(TEXT.text_USER_CODE)]
            public string MY_ITS_USER_CODE
            {
                get { return _GET("MY_ITS_USER_CODE", ""); }
                set { _SET("MY_ITS_USER_CODE", value); }

            }
            [ECategory(TEXT.text_ITS)]
            [EDisplayName(TEXT.text_USER_PASSWD)]
            public string MY_ITS_USER_PASSWD
            {
                get { return _GET("MY_ITS_USER_PASSWD", ""); }
                set { _SET("MY_ITS_USER_PASSWD", value); }

            }
            [ECategory(TEXT.text_ITS)]
            [EDisplayName(TEXT.text_WH_CODE)]
            public string MY_ITS_WH_CODE
            {
                get { return _GET("MY_ITS_WH_CODE", ""); }
                set { _SET("MY_ITS_WH_CODE", value); }

            }

            [ECategory(TEXT.text_ITS)]
            [EDisplayName(TEXT.text_WH_NAME)]
            public string MY_ITS_WH_NAME
            {
                get { return _GET("MY_ITS_WH_NAME", ""); }
                set { _SET("MY_ITS_WH_NAME", value); }

            }

            [ECategory(TEXT.text_ITS)]
            [EDisplayName(TEXT.text_GOOD_ITEM_MIN_LIFE)]
            public byte MY_ITS_GOOD_ITEM_MIN_LIFE
            {
                get { return (byte)PARSESHORT(_GET("MY_ITS_GOOD_ITEM_MIN_LIFE", "12")); }
                set { _SET("MY_ITS_GOOD_ITEM_MIN_LIFE", value); }

            }


            [ECategory(TEXT.text_ITS)]
            [EDisplayName(TEXT.text_WH_TYPE)]
            [TypeConverter(typeof(EListConverterITSType))]
            public string MY_ITS_WH_TYPE
            {
                get { return _GET("MY_ITS_WH_TYPE", "1"); }
                set { _SET("MY_ITS_WH_TYPE", value); }

            }


            class EListConverterITSType : EListConverter
            {
                public EListConverterITSType()
                    : base(list()) { }


                static string list()
                {
                    var r = new StringBuilder();

                    r.Append((int)TOOL_ITS.WH_TYPE.depo).Append(",Ecza Depo,");
                    r.Append((int)TOOL_ITS.WH_TYPE.expdepo).Append(",Ihracatçı Depo,");
                    r.Append((int)TOOL_ITS.WH_TYPE.eczane).Append(",Eczane,");
                    r.Append((int)TOOL_ITS.WH_TYPE.uretici).Append(",Üretici,");
                    r.Append((int)TOOL_ITS.WH_TYPE.hasatane).Append(",Hastane");

                    return r.ToString();
                }
            }

        }

        #endregion


        public class TEXT
        {
            public const string text_ITS = "ITS";
            public const string text_USER_CODE = "(ITS) Kullanıcı Kodu";
            public const string text_USER_PASSWD = "(ITS) Şifre";
            public const string text_WH_NAME = "Depo Adi";
            public const string text_WH_CODE = "Depo Kodu";
            public const string text_WH_TYPE = "İTS İş Akışı";
            public const string text_GOOD_ITEM_MIN_LIFE = "Asgari Raf Ömür (Ay)";

            public const string text_NEW_MAT_NAME = "Yeni Ürün";
            //
            public const string text_MSG_DOC1TOITS = "Alış Bildirim İşlemi Yapmak ?";
            public const string text_MSG_DOC3TOITS = "Satış İade Bildirim İşlemi Yapmak ?";
            public const string text_MSG_DOC6TOITS = "Alış İade Bildirim İşlemi Yapmak ?";
            public const string text_MSG_DOC8TOITS = "Satış Bildirim İşlemi Yapmak ?";
            public const string text_MSG_DOC8TOITS_EXP = "İhraç (Satış) Bildirim İşlemi Yapmak ?";

            public const string text_MSG_DOC11TOITS = "Deaktivasyon ve Sarf Bildirim İşlemi Yapmak ?";
            public const string text_MSG_DOC13TOITS = "Üretim Bildirim İşlemi Yapmak ?";



            public const string text_SEND_TO_ITS = "Send doc to ITS ?";
            public const string text_GET_FROM_ITS = "Read doc from ITS ?";

            public const string text_SEND_TO_PTS = "Send doc to PTS ?";
            public const string text_GET_FROM_PTS = "Read doc from PTS ?";

            public const string text_UNDEFINED_DOC_FOR_ITS = "This doc type is not for ITS";
            public const string text_ASK_ADD_NEW_MAT = "Create material with code";
            public const string text_ERR_CLIENT_GLN_INVALID = "Client valid GLN(Code) required";
            public const string text_ERR_DATE_RANGE = "Date range not correct";
            public const string text_NO_REC_SELECTED = "No record selected";
            public const string text_DOC_HAS_ITS_RECS = "Document has ITS record";
            public const string text_ERR_NO_MAT = "Cant find material";
        }

        const string event_ITS_ = "hadlericom_its_";
        const string event_ITS_DRUGLIST = "hadlericom_its_druglist";
        const string event_ITS_EXPORT = "hadlericom_its_export";

        const string event_ITS_INFO = "hadlericom_its_urundogrulama";
        const string event_ITS_SERIALNR = "hadlericom_its_serialnr";
        const string event_ITS_SENDITS = "hadlericom_its_sendits";
        const string event_PTS_IMP = "hadlericom_pts_importpacket";
        const string event_PTS_EXP = "hadlericom_pts_exportpacket";

        const string event_ITS_DOCINFO = "hadlericom_its_docbasicinfo";

        public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
        {



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
                case SysEvent.SYS_ADPDONE:
                    MY_SYS_ADPDONE_HANDLER(EVENTCODE, ARGS);
                    break;
                case SysEvent.SYS_DYNOBJECT:
                    (arg1 as List<IDictionary<string, object>>).AddRange(  TOOL_ITS.DYN.DYNOBJECT(this));
                    break;
            }



        }


        void MY_SYS_NEWFORM_INTEGRATE(Form FORM)
        {
            if (FORM == null)
                return;
            try
            {
                _MY_SYS_NEWFORM_INTEGRATE_MAINFORM(FORM);
                _MY_SYS_NEWFORM_INTEGRATE_STOCKREF(FORM);
                _MY_SYS_NEWFORM_INTEGRATE_STOCKADP(FORM);
            }
            catch (Exception exc)
            {
                MSGUSERERROR("Cant add button: " + exc.Message);
            }

        }

        void _MY_SYS_NEWFORM_INTEGRATE_MAINFORM(Form FORM)
        {

            var fn = GETFORMNAME(FORM);
            if (fn != "form.app")
                return;


            var tree = CONTROL_SEARCH(FORM, "cTreeTools");
            if (tree == null)
                return;
            string nodeCode_ = "cTreeTools_comhadleri_its";

            {

                var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
		 
		 
			{ "Text" , TEXT.text_ITS},
			{ "ImageName" ,"helth_32x32"},
			{ "Name" ,nodeCode_},
            };

                RUNUIINTEGRATION(tree, args);

            }

            {





                var arr1 = new string[] { event_ITS_INFO, event_ITS_DRUGLIST, event_ITS_EXPORT };
                var arr2 = new string[] { "Urun Dogrulama", "İTS Ürün Listesi", LANG("T_EXPORT") };
                var arr3 = new string[] { "info_32x32", "mm_32x32", "export_32x32" };

                for (int i = 0; i < arr1.Length; ++i)
                {

                    var code_ = arr1[i];

                    switch (code_)
                    {
                        case event_ITS_DRUGLIST:
                            if (GETSYSPRM_USER() != 1)
                                continue;
                            break;
                    }



                    RUNUIINTEGRATION(tree,

                        new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
			{ "_root" ,nodeCode_},
			{ "CmdText" ,  "event name::"+arr1[i]},
			{ "Text" ,arr2[i]},
			{ "ImageName" ,arr3[i]},
			{ "Name" ,nodeCode_+ "_"+arr1[i]},
            }
                        );
                }

                ////////////////////////////

                RUNUIINTEGRATION(tree,

                      new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
			{ "_root" ,nodeCode_},
			{ "CmdText" ,  "ref.dyn.its.serials" },
			{ "Text" ,"Serials"},
			{ "ImageName" ,"records_32x32"},
			{ "Name" ,nodeCode_+ "_"+"ref.dyn.its.serials"},
            }
                      );
                ////////////////////////////
                 

                
            }

        }

        void _MY_SYS_NEWFORM_INTEGRATE_STOCKREF(Form FORM)
        {
            var fn = GETFORMNAME(FORM);
            if (!(fn.StartsWith("ref.mm.doc.slip") || fn.StartsWith("ref.sls.doc.inv") || fn.StartsWith("ref.prch.doc.inv")))
                return;


            var cPanelBtnSub = CONTROL_SEARCH(FORM, "cPanelBtnSub");

            if (cPanelBtnSub == null)
                return;


            _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_ITS_SERIALNR, "Seri No.");//
            _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_ITS_SENDITS, "İTS");
            _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_PTS_EXP, "PTS");
            _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_ITS_DOCINFO, "Bilgi (İTS)");


        }
        void _MY_SYS_NEWFORM_INTEGRATE_STOCKADP(Form FORM)
        {

            var fn = GETFORMNAME(FORM);
            if (!(fn.StartsWith("adp.mm.doc.slip") || fn.StartsWith("adp.sls.doc.inv") || fn.StartsWith("adp.prch.doc.inv")))
                return;


            switch (fn)
            {
                case "adp.sls.doc.inv.8":
                case "adp.prch.doc.inv.1":
                    break;


                default:
                    return;
            }


            var taskcmd = RUNUIINTEGRATION(FORM, "_cmd", "taskcmd") as string;

            if (taskcmd == null)
                return;

            var taskcmd_cmd = CMDLINEGETARG(taskcmd, "cmd");

            if (!(taskcmd_cmd == "" || taskcmd_cmd == "add" || taskcmd_cmd == "copy" || taskcmd_cmd == "edit"))
                return;



            var cPanelBtnSub = CONTROL_SEARCH(FORM, "cPanelBtnSub");

            if (cPanelBtnSub == null)
                return;


            _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_PTS_IMP, "PTS Al");
            // _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_ITS_SERIALNR, "Seri No.");
        }

        void _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(Control pCtrl, string pEvent, string pText)
        {
            if (pCtrl == null)
                return;
            try
            {


                var args = new Dictionary<string, object>() { 
            
			{ "_cmd" ,""},
            { "_type" ,""},
			{ "CmdText" ,"event name::"+pEvent},
			{ "Text" ,pText},
			{ "ImageName" ,"right_16x16"},
			{ "Width" ,80},
            };

                //   RUNUIINTEGRATION(pCtrl, args);

                var b = RUNUIINTEGRATION(pCtrl, args) as Button;
                if (b != null)
                {
                    b.AutoSize = true;
                    var w = (Math.Max(80, b.Width + (12 + 16 + 12)) / 20) * 20;
                    b.AutoSize = false;
                    b.Width = w;

                }
            }
            catch (Exception exc)
            {
                MSGUSERERROR("Cant add button: " + exc.Message);
            }

        }


        public void MY_SYS_ADPDONE_HANDLER(string EVENTCODE, object[] ARGS) //adapter start
        {
            object arg1 = ARGS.Length > 0 ? ARGS[0] : null;
            var ds = arg1 as DataSet;

            if (ds == null)
                return;


            TOOL_ITS.UTIL_ITSTRACK.SAVE(this, ds);

        }


        public void MY_SYS_USEREVENT_HANDLER(string EVENTCODE, object[] ARGS) //adapter start
        {
            // TOOL_ITS.DB_SUPPORT.VALIDATE(this);

            MY_DATASOURCE.UPDATE(this);
            //
            //"SYS_USEREVENT/event_code"
            LOAD_SETTINGS(this);

            //
            try
            {


                object arg1 = ARGS.Length > 0 ? ARGS[0] : null;
                object arg2 = ARGS.Length > 1 ? ARGS[1] : null;

                string[] list_ = EXPLODELISTPATH(EVENTCODE);

                switch (list_.Length > 1 ? list_[1].ToLowerInvariant() : "")
                {

                    case event_ITS_INFO:
                        {
                            MY_RUN_EVENT_STOCK_INFO();
                        }
                        break;
                    case event_ITS_DRUGLIST:
                        {
                            MY_RUN_EVENT_STOCK_IMPORT();
                        }
                        break;
                    case event_ITS_EXPORT:
                        {
                            MY_RUN_EVENT_EXPORT();
                        }
                        break;

                    case event_ITS_SERIALNR:
                        {
                            if (ISREFERENCEFORM(arg1 as Form))
                            {
                                var rec = RUNUIINTEGRATION(arg1, "_cmd", "record") as DataRow;
                                MY_RUN_EVENT_STOCK_SERIALNR(rec, null);
                            }
                            else
                                if (ISADAPTERFORM(arg1 as Form))
                                {
                                    var ds = RUNUIINTEGRATION(arg1, "_cmd", "dataset") as DataSet;
                                    MY_RUN_EVENT_STOCK_SERIALNR(null, ds);
                                }


                        }
                        break;
                    case event_ITS_SENDITS:
                        {
                            var rec = RUNUIINTEGRATION(arg1, "_cmd", "record") as DataRow;
                            MY_RUN_EVENT_STOCK_SENDITS(rec, arg1);
                        }
                        break;

                    case event_PTS_IMP:
                        {

                            if (ISADAPTERFORM(arg1 as Form))
                            {
                                var ds = RUNUIINTEGRATION(arg1, "_cmd", "dataset") as DataSet;

                                MY_RUN_EVENT_STOCK_IMPPTS(ds);
                            }
                        }
                        break;
                    case event_PTS_EXP:
                        {
                            if (ISREFERENCEFORM(arg1 as Form))
                            {
                                var rec = RUNUIINTEGRATION(arg1, "_cmd", "record") as DataRow;
                                MY_RUN_EVENT_STOCK_EXPPTS(rec, arg1);
                            }
                        }
                        break;
                    case event_ITS_DOCINFO:
                        {

                            if (ISREFERENCEFORM(arg1 as Form))
                            {
                                var rec = RUNUIINTEGRATION(arg1, "_cmd", "record") as DataRow;
                                MY_RUN_EVENT_ITSDOCINFO(rec, arg1);
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


        void MY_RUN_EVENT_STOCK_INFO()
        {
            try
            {
                var x = MY_ASKSTRING(this, "T_BARCODE", "");
                if (x == "" || x == null)
                    return;

                var msg = TOOL_ITS.ImplService.CheckItemsByBarcode(x, this);
                MSGUSERINFO(msg);
            }
            catch (Exception exc)
            {
                MSGUSERERROR(exc.Message);
            }
        }



        void MY_RUN_EVENT_EXPORT()
        {
            try
            {


                MY_RUN_EVENT_EXPORT(this);

            }
            catch (Exception exc)
            {
                MSGUSERERROR(exc.Message);
            }
        }


        static void MY_RUN_EVENT_EXPORT(_PLUGIN pPLUGIN)
        {



            var list = new List<string>();





           // list.AddRange(new string[] { "all", pPLUGIN.LANG("T_ALL") });
            list.AddRange(new string[] { "onhand", pPLUGIN.LANG("T_ONHAND") });

            var res_ = pPLUGIN.REF("ref.gen.definedlist [obj::" + JOINLIST(list.ToArray()) + "] [desc::T_EXPORT] type::string");

            string exportCode_ = (res_ != null && res_.Length > 0 ? CASTASSTRING(res_[0]["VALUE"]) : null);
            if (exportCode_ == null)
                return;

            DataTable serials = null;

            switch (exportCode_)
            {
                 
                case "onhand":
                    serials = pPLUGIN.SQL(@"
WITH __DATA AS (SELECT 
MAX(T.GROUP_) AS GROUP_,
T.TRACKNO,
T.GTIN,
T.STOCKREF ,
(SELECT NAME FROM LG_$FIRM$_ITEMS WHERE LOGICALREF = T.STOCKREF) AS NAME,
SUM(COALESCE((SELECT COUNT(*) FROM LG_$FIRM$_$PERIOD$_STFICHE D WHERE D.LOGICALREF = T.STFICHEREF AND D.TRCODE IN (8,7,6,11,12,51,20)),0)) AS COUNT_OUT,
SUM(COALESCE((SELECT COUNT(*) FROM LG_$FIRM$_$PERIOD$_STFICHE D WHERE D.LOGICALREF = T.STFICHEREF AND D.TRCODE IN (1,3,2,13,14,50,15)),0)) AS COUNT_INT

FROM LG_$FIRM$_$PERIOD$_ITSTRACK T 

GROUP BY T.TRACKNO,T.GTIN,T.STOCKREF
)

SELECT * FROM __DATA WHERE COUNT_INT>COUNT_OUT
ORDER BY  GROUP_ ASC, TRACKNO ASC
", null);
                    break;



            }
            //GROUP_ ASC,TRACKNO ASC

            if (serials == null)
                return;

            string lastGroup_ = null;
            string lastGtin_ = null;

            MY_DIR.CLEAN_SN();

 
            if (serials.Rows.Count == 0)
                throw new Exception("T_MSG_ERROR_NO_DATA");

            var sb = new StringBuilder();

            var listAllData = new List<string>();

            foreach (DataRow row in serials.Rows)
            {


                var GROUP_ = CASTASSTRING(TAB_GETROW(row, "GROUP_"));
                var TRACKNO = CASTASSTRING(TAB_GETROW(row, "TRACKNO"));
                var GTIN = TAB_GETROW(row, "GTIN").ToString();
                var DESC_ = TAB_GETROW(row, "NAME").ToString();


                var changed = (lastGroup_ != GROUP_ || lastGtin_ != GTIN);

                if (changed)
                {

                    //dump
                    if (lastGroup_ != null && sb.Length > 0)//first
                    {
                        listAllData.Add(sb.ToString());
                        MY_DIR.DUMP_MED_INFO(lastGroup_, lastGtin_, sb.ToString());
                    }
                    //reset
                    sb.Clear();

                    lastGroup_ = GROUP_;
                    lastGtin_ = GTIN;
                }


                sb.AppendLine(string.Format("{0}\t{1}\t{2}\t{3}",
                   TRACKNO, GROUP_, GTIN, DESC_
                   ));
            }

            //dump last
            if (lastGroup_ != null && sb.Length > 0)//first
            {
                listAllData.Add(sb.ToString());
                MY_DIR.DUMP_MED_INFO(lastGroup_, lastGtin_, sb.ToString());
            }


            MY_DIR.DUMP_MED_INFO("__", "__", string.Join("", listAllData.ToArray()));

 

        }


        void MY_RUN_EVENT_STOCK_IMPORT()
        {
            try
            {
                if (GETSYSPRM_USER() != 1)
                    return;


                if (!MSGUSERASK("Import drugs from ITS service ?"))
                    return;

                var res = TOOL_ITS.ImplService.ImportMedicine(false, this);
                MSGUSERINFO("Imported items count: " + res);
            }
            catch (Exception exc)
            {
                MSGUSERERROR(exc.Message);
            }
        }




        void MY_RUN_EVENT_STOCK_SERIALNR(DataRow pRefRec, DataSet pAdpDs)
        {
            if (pRefRec == null && pAdpDs == null)
                return;

            try
            {

                object docRef = null;
                bool isInvoice = true;

                //
                if (pRefRec != null)
                {
                    //
                    if (pRefRec.Table.TableName == "INVOICE")
                    {
                        isInvoice = true;
                        docRef = TAB_GETROW(pRefRec, "LOGICALREF");
                    }
                    else
                        if (pRefRec.Table.TableName == "STFICHE")
                        {
                            isInvoice = false;
                            docRef = TAB_GETROW(pRefRec, "LOGICALREF");
                        }
                        else
                            return;
                    //
                    var trcode = CASTASSHORT(TAB_GETROW(pRefRec, "TRCODE"));

                    //switch (trcode)
                    //{
                    //    case 1:
                    //    case 8:
                    //        break;


                    //    default:
                    //        return;
                    //}
                    ////

                    switch (trcode)
                    {
                        case 1:
                        case 6:
                        case 2:
                        case 3:
                        case 7:
                        case 8:
                            break;

                        case 11://fire
                        case 13: //uretim
                            break;

                        default:
                            return;
                    }

                    var f = new TOOL_ITS.ITS_FORMS.FormITSSerialNo(this, docRef, isInvoice);
                    f.ShowDialog();

                }

                //if (pAdpDs != null)
                //{

                //    var f = new TOOL_ITS.ITS_FORMS.FormITSSerialNo(this, pAdpDs);
                //    f.ShowDialog();

                //}


            }
            catch (Exception exc)
            {
                MSGUSERERROR(exc.Message);
            }
        }



        void MY_RUN_EVENT_STOCK_SENDITS(DataRow rec, object refObj)
        {
            if (rec == null)
                return;

            object docRef = null;
            bool isInvoice = true;

            if (!MSGUSERASK(TEXT.text_SEND_TO_ITS))
                return;

            //
            if (rec.Table.TableName == "INVOICE")
            {
                isInvoice = true;
                docRef = TAB_GETROW(rec, "LOGICALREF");
            }
            else
                if (rec.Table.TableName == "STFICHE")
                {
                    isInvoice = false;
                    docRef = TAB_GETROW(rec, "LOGICALREF");
                }
                else
                    return;
            //
            var trcode = CASTASSHORT(TAB_GETROW(rec, "TRCODE"));

            switch (trcode)
            {
                case 1:
                case 6:
                case 2:
                case 3:
                case 7:
                case 8:
                    break;

                case 11://fire
                case 13: //uretim
                    break;

                default:
                    return;
            }
            //

            try
            {

                if (TOOL_ITS.ImplService.sendDocToITS(docRef, isInvoice, this))
                    RUNUIINTEGRATION(refObj, "_cmd", "refresh");

            }
            catch (Exception exc)
            {
                MSGUSERERROR(exc.Message);
            }
        }
        void MY_RUN_EVENT_STOCK_IMPPTS(DataSet pDs)
        {
            if (pDs == null)
                return;


            try
            {



                if (TOOL_ITS.ImplService.getDocFromPTS(pDs, this, 0))
                {
                    MSGUSERINFO("T_MSG_OPERATION_OK");
                    //exported
                }


            }
            catch (Exception exc)
            {
                MSGUSERERROR(exc.Message);
            }
        }

        void MY_RUN_EVENT_STOCK_EXPPTS(DataRow rec, object refObj)
        {
            if (rec == null)
                return;


            if (!MSGUSERASK(TEXT.text_SEND_TO_PTS))
                return;

            object docRef = null;
            bool isInvoice = true;



            //
            if (rec.Table.TableName == "INVOICE")
            {
                isInvoice = true;
                docRef = TAB_GETROW(rec, "LOGICALREF");
            }
            else
                if (rec.Table.TableName == "STFICHE")
                {
                    isInvoice = false;
                    docRef = TAB_GETROW(rec, "LOGICALREF");
                }
                else
                    return;
            //
            var trcode = CASTASSHORT(TAB_GETROW(rec, "TRCODE"));

            //switch (trcode)
            //{
            //    case 1:
            //    case 8:
            //        break;


            //    default:
            //        return;
            //}
            //
            switch (trcode)
            {
                case 1:
                case 6:
                case 2:
                case 3:
                case 7:
                case 8:
                    break;

                case 11://fire
                case 13: //uretim
                    break;

                default:
                    return;
            }
            try
            {



                if (TOOL_ITS.ImplService.postDocToPTS(docRef, isInvoice, this))
                {

                    MSGUSERINFO("T_MSG_OPERATION_OK");
                    //exported
                }


            }
            catch (Exception exc)
            {
                MSGUSERERROR(exc.Message);
            }
        }

        void MY_RUN_EVENT_ITSDOCINFO(DataRow rec, object refObj)
        {
            if (rec == null)
                return;

            try
            {
                object docRef = null;
                bool isInvoice = true;

                if (rec.Table.TableName == "INVOICE")
                {
                    isInvoice = true;
                    docRef = TAB_GETROW(rec, "LOGICALREF");
                }
                else
                    if (rec.Table.TableName == "STFICHE")
                    {
                        isInvoice = false;
                        docRef = TAB_GETROW(rec, "LOGICALREF");
                    }
                    else
                        return;

                var doc = new TOOL_ITS.ITS_STOCKDOC(this, docRef, isInvoice);
                if (!doc.VALID)
                    return;

                var res = new StringBuilder();
                res.AppendLine("<html>");
                res.AppendLine(@" 

<style>
          table, th, td {
                border: 1px solid #dbdbdb;
                border-collapse: collapse;
                white-space: nowrap;
                padding: 2px;
                font-family:Segoe UI, Verdana, Aral;
                padding: 2px 0 0 2px;
 mso-number-format:""\@"";
            }

 
th{
font-weight:bold

}
</style>
 
");


                res.AppendLine("<table style='border-color: #c0c0c0;' border='1' cellspacing='0'>");
                res.AppendLine("<tr><th>SERİ NUMARA</th><th>GRUP</th></tr>");
                var list = new List<TOOL_ITS.ITS_BARCODE>();

                list.AddRange(doc.SERIALS_0);
                list.AddRange(doc.SERIALS_1);

                foreach (var s in list)
                {
                    var t = TOOL_TPL.FORMAT("<tr><td>{RAW}</td><td>{GROUP_}</td></tr>", s);
                    res.AppendLine(t);

                }
                res.AppendLine("</table>");
                res.AppendLine("</html>");
                MSGUSERINFO(res.ToString());


            }
            catch (Exception exc)
            {
                MSGUSERERROR(exc.Message);
            }

        }
        static double MY_ASKNUMBER(_PLUGIN pPLUGIN, string pMsg, double pDef)
        {

            var rows_ = pPLUGIN.REF("ref.gen.double desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDef));
            if (rows_ != null && rows_.Length > 0)
            {
                return CASTASDOUBLE(ISNULL(rows_[0]["VALUE"], 0.0));
            }
            return 0;

        }

        static string MY_ASKSTRING(_PLUGIN pPLUGIN, string pMsg, string pDef)
        {

            var rows_ = pPLUGIN.REF("ref.gen.string desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDef));
            if (rows_ != null && rows_.Length > 0)
            {
                return CASTASSTRING(ISNULL(rows_[0]["VALUE"], 0.0));
            }
            return "";

        }
        static string MY_ASKTEXT(_PLUGIN pPLUGIN, string pMsg, string pDef)
        {

            var rows_ = pPLUGIN.REF("ref.gen.text desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDef));
            if (rows_ != null && rows_.Length > 0)
            {
                return CASTASSTRING(ISNULL(rows_[0]["VALUE"], 0.0));
            }
            return "";

        }
        #region ITS_API


        #region UTILS

        public static string MY_CHOOSE_SQL(string pSqlMs, string pSqlPg)
        {

            if (ISMSSQL())
                return pSqlMs;

            if (ISPOSTGRESQL())
                return pSqlPg;


            throw new Exception("Undefined datasource");
        }

        public static void __INIT_SRV(System.Web.Services.Protocols.SoapHttpClientProtocol pSrv)
        {
            pSrv.Credentials = new System.Net.NetworkCredential(ITS_USER_CODE, ITS_USER_PASSWD);
        }


        public static string DATE_XML_FORMAT(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }
        public static DateTime DATE_XML_PARSE(string dt)
        {
            return DateTime.ParseExact(dt, "yyyy-MM-dd", null);
        }
        public static string StokPartiNo(string kareKod)
        {
            return new TOOL_ITS.ITS_BARCODE(kareKod).BN;
        }

        public static string StokSiraNo(string kareKod)
        {
            return new TOOL_ITS.ITS_BARCODE(kareKod).SN;
        }

        public static DateTime StokSonKullanmaTarihi(string kareKod)
        {


            var x = new TOOL_ITS.ITS_BARCODE(kareKod);
            return x.XD_DATE;

        }

        public static string StokGTINNo(string kareKod)
        {
            return new TOOL_ITS.ITS_BARCODE(kareKod).GTIN;
        }



        public static string GetGLNToCariUnvani(_PLUGIN plugin, string code)
        {
            if (string.IsNullOrEmpty(code))
                return null;

            if (code == ITS_WH_CODE)
                return ITS_WH_DESC;

            var r = plugin.SQLSCALAR(
                MY_CHOOSE_SQL(
                @"SELECT DEFINITION_ FROM LG_$FIRM$_CLCARD WITH(NOLOCK) WHERE CODE = @P1",
                @"SELECT DEFINITION_ FROM LG_$FIRM$_CLCARD WHERE CODE = @P1"),
                new object[] { code }) as string;
            return r ?? "";
        }

        public static string GetGTINToStokAdi(_PLUGIN plugin, string code)
        {
            if (string.IsNullOrEmpty(code))
                return null;
            var r = plugin.SQLSCALAR(
MY_CHOOSE_SQL(
@"SELECT NAME FROM LG_$FIRM$_ITEMS WITH(NOLOCK) WHERE CODE in (@P1,@P2)",
@"SELECT NAME FROM LG_$FIRM$_ITEMS WHERE CODE in (@P1,@P2)"),
new object[] { code, code.TrimStart('0') }) as string;
            return r ?? "";
        }

        public static void DOCITSFINISHED(_PLUGIN pPLUGIN, object pDocRef, bool pIsInv)
        {
            var newValue = "İTS_GÖNDERİLDİ";
            if (pIsInv)
            {
                pPLUGIN.SQL("UPDATE LG_$FIRM$_$PERIOD$_INVOICE SET CYPHCODE = @P2 WHERE LOGICALREF = @P1", new object[] { pDocRef, newValue });
            }
            else
            {
                pPLUGIN.SQL("UPDATE LG_$FIRM$_$PERIOD$_STFICHE SET CYPHCODE = @P2' WHERE LOGICALREF = @P1", new object[] { pDocRef, newValue });

            }
        }


        //return true if Ok else if error false
        public static bool ITEMGOTMSG(_PLUGIN plugin, string gtin, string sn, object sourceRef, string msgNr, TOOL_ITS.WebServices.SrvExt pSrv, short ioStatus = 0)
        {
            var res = true;

            if (msgNr != "")
            {
                var errNr = PARSEINT(msgNr);
                var f = "GTIN " + gtin + " SN " + sn + "";




                var msg = "";

                if (errNr == 0 || Array.IndexOf<int>(pSrv.successfulOperCodes, errNr) >= 0) //valid
                {
                    plugin.SQL(
                        MY_CHOOSE_SQL(
@"UPDATE LG_$FIRM$_$PERIOD$_ITSTRACK SET STATUS_ = 1,EDATE_=getdate(),IOSTATUS = @P3 WHERE STFICHEREF = @P1 AND STOCKREF > 0 AND TRACKNO LIKE @P2 ",
@"UPDATE LG_$FIRM$_$PERIOD$_ITSTRACK SET STATUS_ = 1,EDATE_=NOW()::TIMESTAMP(0),IOSTATUS = @P3 WHERE STFICHEREF = @P1 AND STOCKREF > 0 AND TRACKNO LIKE @P2 "),


                        new object[] { sourceRef, f, ioStatus });

                    msg = (pSrv.srvDesc + ": Ürün [" + f + "] Bildirimi Başarı Yapılmıştır" + (errNr == 0 ? "" : ": " + getErrorCodeMsg(errNr)));

                }
                else
                {
                    var mdesc = LEFT(CASTASSTRING(
                        plugin.SQLSCALAR(@"
SELECT
--$MS$--TOP(1)
NAME FROM LG_$FIRM$_ITEMS WITH(NOLOCK) WHERE CODE IN (@P1,@P2)
--$PG$--LIMIT 1
", new object[] { gtin, gtin.TrimStart('0') })
                        ), 30);
                    msg = (pSrv.srvDesc + ": Ürün [" + f + "] [" + mdesc + "]Hatasi: " + getErrorCodeMsg(errNr));
                    res = false;
                }


                LOG_JOURNAL(plugin, msg);
            }

            return res;
        }


        static void LOG_JOURNAL(_PLUGIN pl, string msg)
        {
            pl.JOURNAL("ITS", msg);
        }
        static void beepErr()
        {

            {
                System.Media.SystemSounds.Asterisk.Play();
                System.Threading.Thread.Sleep(600);
                System.Media.SystemSounds.Asterisk.Play();
            }
        }

        public static string getErrorCodeMsg(object pNr)
        {
            var nr = FORMAT(pNr);
            Dictionary<string, string> dic = new Dictionary<string, string>();


            {

                dic.Add("60000", "Ürün Deaktif.");
                dic.Add("60004", "Ürün Eczane Tarafından Satılmış.");
                dic.Add("60005", "Ürün İhraç Edilmiş.");
                dic.Add("60006", "Ürün Sarf Edilmiş.");
                dic.Add("60007", "Ürün Geri Çekilmiş.");
                dic.Add("60001", "Ürün Üreticide.");
                dic.Add("60002", "Ürün Depoda.");
                dic.Add("60003", "Ürün Eczanede.");
                dic.Add("60008", "Ürün Sarf Merkezinde.");
                dic.Add("00000", "Ürün üzerinde gerçeklestirilen islem basarilidir.");
                dic.Add("20001", "Kaynak GLN Bilgisi Yok/Geçersiz");
                dic.Add("10231", "Ürün tarafinizdan sarf edilmistir.");
                dic.Add("20006", "Birden Fazla Dosya Gönderilemez");
                dic.Add("20007", "Dosya Formati Desteklenmiyor");
                dic.Add("20008", "Girilen Transfer ID Sistemde Bulunamadi");
                dic.Add("20009", "Dosya boyutu en fazla 10MB olabilir.");
                dic.Add("20100", "Kullanici bu firma için paket bilgilerini alamaz.");
                dic.Add("20101", "Tarafiniza gönderilmis paketler için 'Teslim alinmamis transfer bilgilerini getir' secenegi seçilebilinir.");
                dic.Add("20201", "Geçersiz paydas tipi");
                dic.Add("40004", "Ürün tarafinizdan satilmistir.");
                dic.Add("11002", "Veri Tabani Hatasi");
                dic.Add("21005", "Tek seferde sadece bir paket gönderilebilir!");
                dic.Add("21014", "Alicisi veya göndericisi olmadigini paketler hakkinda bilgi alamazsiniz!");
                dic.Add("21015", "Paket Detay Servisinde sorgu baslangiç tarihi, bitis tarihinden sonra olamaz!");
                dic.Add("20005", "Servis Ekinde Dosya Bulunamadi");
                dic.Add("10306", "Belirtilen ürün baska bir üretici firma üzerine kayitlidir.");
                dic.Add("10309", "Belirtilen ürün baska bir hastane üzerine kayitlidir.");
                dic.Add("21009", "Paketi göndermeye çalistiginiz paydas deaktif!");
                dic.Add("40009", "Üzerinize kayitli ürün baska bir paydas seviyesinde geri çekilmistir.");
                dic.Add("21012", "Alinmak istenen paket # defa alinmaya çalisilmis. Bir paket en fazla 100 defa alinabilir, Lütfen süreçlerinizi kontrol ediniz.");
                dic.Add("20002", "Hedef GLN Bilgisi Yok/Geçersiz");
                dic.Add("20004", "Kullanici Bu Firma için Paket Alamaz");
                dic.Add("40006", "Ürünün son kullanma tarihi geçmistir.");
                dic.Add("10308", "Belirtilen ürün baska bir eczane üzerine kayitlidir.");
                dic.Add("21001", "Transfer id formati düzgün degildir! Transfer id sadece rakamlardan olusabilir.");
                dic.Add("40008", "Ürün baska bir paydas seviyesinde geri çekilmistir.");
                dic.Add("21016", "Paket Detay Servisindeki maksimum sorgu süresi 1 ay(31 gün) olmalidir. Lütfen baslangiç ve bitis tarihlerinizi düzeltiniz!");
                dic.Add("11001", "Kullanici Bu Firma Için Bildirim Yapamaz.");
                dic.Add("11003", "Bildirilen GLN e kayitlarimizda rastlanamamistir.");
                dic.Add("11004", "Yanlis GTIN Numarasi. Bu Numarada Kayitli Ürün Yok.");
                dic.Add("11005", "Bu Ürüne ait Üretim Bildirimi Yapma Yetkiniz Yok.");
                dic.Add("11006", "Bu Ürün Üretim Bildirimine Kapalidir!");
                dic.Add("11009", "Bildirilen Alici Bilgisine Kayitlarimizda Rastlanilamamistir.");
                dic.Add("11011", "Ürün Tipi Alani Yok ya da Geçersiz Ürün Tipi...Sadece Ilaç (PP) kabul edilir.");
                dic.Add("11012", "Parti Numarasi Alani Yok ya da Geçersizdir.");
                dic.Add("11013", "Ürün Barkod Bilgisi Yok ya da Geçersizdir.");
                dic.Add("11014", "Üretici GLN Bilgisi Yok ya da Geçersiz.");
                dic.Add("11015", "Üretim Zamani Bilgisi Yok ya da Yanlis");
                dic.Add("11016", "Son Kullanma Tarihi Bilgisi Yok ya da Yanlis.");
                dic.Add("11017", "Içerisinde Ürünlerin Sira Numarasi Bilgileri Girilmemis.");
                dic.Add("11018", "Gönderici GLN Bilgisi Yok ya da Geçersiz.");
                dic.Add("11019", "Alici Bilgisi Yok ya da Geçersiz.");
                dic.Add("11020", "Içerisinde Bilgileri Girilmemis.");
                dic.Add("11022", "Deaktivasyon Kodu Yanlis.");
                dic.Add("11023", "Bu Servisi Kullanmaya Yetkili Degilsiniz.");
                dic.Add("11024", "Bilgisi Bos ya da Geçersiz.");
                dic.Add("11025", "alaninda bir Eczane Kodu varsa , ve alanlari bos olmalidir.");
                dic.Add("11026", "alaninda SGK nin Kodu varsa ve Alanlari dolu olmalidir.");
                dic.Add("11027", "Bu Reçete Kayit Numarasi Zaten Kayitli");
                dic.Add("11028", "Bilgisi Yok ya da Geçersiz!");
                dic.Add("11029", "Bilgisi Yok ya da Geçersiz!");
                dic.Add("11030", "Belirtilen Reçete Kayit Numarasina Satis Bildirimi Yapilmamistir.");
                dic.Add("11031", "Gönderilen XML Veri Yapisi WSDL Dokümanindaki Sema ile Uyumlu Degildir");
                dic.Add("11032", "Sira Numarasi Formati Geçersiz");
                dic.Add("11033", "Alici Bilgisi Geri Ödeme Kurumu Oldugunda alani bos olmalidir");
                dic.Add("11034", "Elden Satis Kodu Oldugunda ve alanlari bos olmalidir.");
                dic.Add("11035", "Ürüne Ait Son Kullanim Tarihi (XD) Formati Uyumsuz");
                dic.Add("11036", "Ürüne Ait Parti Numarasi (BN) Formati Uyumsuz");
                dic.Add("11037", "Ürüne ait GTIN numarasi Formati Uyumsuz");
                dic.Add("11038", "(XD) Ürün Son Kullanma Tarihi, Üretim Tarihinden Itibaren 7 Yili Geçemez");
                dic.Add("11039", "Kullanici adinin ilk 13 hanesi GLN ile ayni olmalidir");
                dic.Add("11208", "Geçersiz Alici Bilgisi");
                dic.Add("11213", "Belirtilen Reçete Numarasi Zaten Kayitli");
                dic.Add("11215", "Bildirilen Satici (Depo) Bilgisi Yanlistir");
                dic.Add("11216", "Belirtilen Reçete Kayit Numarasi Sistemimizde Kayitli Degildir");
                dic.Add("11217", "Bu Reçete Geri Ödeme Kurumu tarafindan Sorgulanmistir. Iptal Edilemez");
                dic.Add("11901", "Kullanici bu servisi kullanmaya yetkili degildir. Lütfen firma yetkilinizle görüsünüz");
                dic.Add("11902", "Belirtilen alici sistemde kayitli degildir");
                dic.Add("11903", "Belirtilen satici sistemde kayitli degildir");
                dic.Add("11904", "Belirtilen alici deaktif durumdadir");
                dic.Add("11905", "Belirtilen satici deaktif durumdadir");
                dic.Add("11906", "Belirtilen alici bu isleme uygun paydas türünde degildir");
                dic.Add("11907", "Belirtilen satici bu isleme uygun paydas türünde degildir");
                dic.Add("10007", "Bu Sira Numarasi Zaten Kayitli!");
                dic.Add("10008", "Tanimlanmamis Kayit Hatasi.");
                dic.Add("10201", "Belirtilen Ürün Sistemimizde Kayitli Degildir.");
                dic.Add("10202", "Ürünün Son Kullanma Tarihi Geçmistir. (Hastaya verilemez.)");
                dic.Add("10203", "Ürün Bilgileri Tutarsiz.");
                dic.Add("10204", "Belirtilen Ürün Önceden Satilmistir.");
                dic.Add("10205", "Bu Ürünün Satisi Yasaklanmistir.");
                dic.Add("10206", "Veritabani Kayit Hatasi.");
                dic.Add("10207", "Bu Ürün Önceden Ihraç Edilmistir.");
                dic.Add("10209", "Ürün Su Anda Baska Bir Eczane Stokunda Görünüyor.");
                dic.Add("10210", "Ürün Stokunuzda Görünüyor.");
                dic.Add("10211", "Ürün Stokunuzda Görünmüyor!");
                dic.Add("10219", "Belirtilen Ürün Tarafinizdan Satilmamistir");
                dic.Add("10220", "Ürün Geri Ödeme Kurumuna Satilmistir. Satisin Reçete Bazli Iptal Edilmesi Gerekir");
                dic.Add("10221", "Ürünün Satisi Iptal Edilemez.");
                dic.Add("10222", "Ürün Üzerinize Kayitli Degil");
                dic.Add("10223", "Ürün Üzerinize Kayitli Görünüyor");
                dic.Add("10224", "Ürün Eczane Tarafindan Satilmistir.");
                dic.Add("10225", "Ürün su anda baska bir birimde görünüyor.");
                dic.Add("10227", "Girilen GLN, eczane GLNsi degildir. GLNnin size ait oldugundan emin olunuz.");
                dic.Add("10301", "Girilen satici GLNsi yanlistir. Ürünü aldiginiz paydasin GLNsini belirttiginizden emin olunuz.");
                dic.Add("10302", "Girilen alici GLNsi yanlistir. Ürünü sattiginiz paydasin GLNsini belirttiginizden emin olunuz.");
                dic.Add("10303", "Belirtilen ürün üzerinize kayitlidir.");
                dic.Add("10304", "Belirtilen ürün üzerinize kayitli degildir.");
                dic.Add("10305", "Belirtilen ürün baska bir paydas üzerine kayitlidir.");
                dic.Add("11045", "Belirtilen parti numarasina geri çekme uygulanmistir.");
                dic.Add("11021", "Bildirim Türü Yanlis.");
                dic.Add("10230", "Ürün sarf edilmistir.");
                dic.Add("20003", "Kullanici Bu Firma için Paket Gönderemez");
                dic.Add("40001", "Ürün üzerinize kayitlidir.");
                dic.Add("40002", "Ürün üzerinize kayitli degildir.");
                dic.Add("40003", "Ürün üzerinize kayitli degildir.");
                dic.Add("40005", "Ürün geri çekilmistir.");
                dic.Add("10307", "Belirtilen ürün baska bir ecza deposu üzerine kayitlidir.");
                dic.Add("11040", "(PRODUCTS) içerisinde (PRODUCT) Bilgileri Girilmemis");
                dic.Add("21006", "Transfer id sistemde yoktur ve/veya tarafiniza böyle bir paket gönderilmemistir!");
                dic.Add("21002", "Alici GLN formati uygun degildir! GLN 13 haneli olmali ve rakamlardan olusmalidir.");
                dic.Add("21003", "Gönderilmek istenen paket formati desteklenmemektedir! Paketin ZIP formatinda oldugundan emin olunuz.");
                dic.Add("21004", "Gönderilecek paketin boyutu azami 10MB olabilir!");
                dic.Add("21007", "Gönderilmek istenen paket bulunamadi! Lütfen paketi gönderdiginizden emin olun.");
                dic.Add("21008", "Paketi göndermeye çalistiginiz paydas sistemde bulunamadi! Lütfen karsi paydasin GLN'sini kontrol ediniz.");
                dic.Add("21010", "Alinmak istenen paket sistem üzerinde bulunamadi! Lütfen ITS Yardim Masasi Hata Kodu ve Transfer id ile basvurunuz.");
                dic.Add("21011", "Gönderici kendisine paket gönderemez!");
                dic.Add("40007", "Üzerinize kayitli ürün geri çekilmistir.");
                dic.Add("21013", "Gönderici GLN formati uygun degildir! GLN 13 haneli olmali ve rakamlardan olusmalidir!");


            }

            if (dic.ContainsKey(nr.ToString()))
                return nr + ": " + dic[nr.ToString()];

            return "Error " + nr;

        }



        public static TOOL_ITS.WebServices.WebServiceErrorCode.errorCode[] GetHataKodlari()
        {

            var webSrv = new TOOL_ITS.WebServices.WebServiceErrorCode.ErrorCode();
            __INIT_SRV(webSrv);
            var errorCodes = webSrv.getErrorCodes(new TOOL_ITS.WebServices.WebServiceErrorCode.errorCodeRequest()).errorCodes;

            return errorCodes;

        }
        #endregion

        public class TOOL_ITS
        {


            static string URL_ServerIts = "http://its.saglik.gov.tr:80";
            static string URL_ErrorCode =
             URL_ServerIts + "/ReferenceServices/ErrorCode";
            static string URL_Drug =
           URL_ServerIts + "/ReferenceServices/Drug";

            static string URL_UrunDogrulamaReceiverService =
           URL_ServerIts + "/UrunDogrulama/UrunDogrulamaReceiverService";

            static string URL_CheckStatusNotification =
         URL_ServerIts + "/ITSServices/CheckStatusNotification";

            static string URL_DeaktivasyonBildirimReceiverService =
          URL_ServerIts + "/DeaktivasyonBildirim/DeaktivasyonBildirimReceiverService";
            static string URL_UretimBildirimReceiverService =
         URL_ServerIts + "/UretimBildirim/UretimBildirimReceiverService";
            static string URL_ReceiptNotification = //purch
         URL_ServerIts + "/ITSServices/ReceiptNotification";
            static string URL_ReturnNotification =
         URL_ServerIts + "/ITSServices/ReturnNotification";
            static string URL_DispatchCancellation = //sale canccel
         URL_ServerIts + "/ITSServices/DispatchCancellation";

            static string URL_DispatchNotification = //sale
        URL_ServerIts + "/ITSServices/DispatchNotification";
            static string URL_IhracatReceiverService =
          URL_ServerIts + "/IhracatBildirim/IhracatReceiverService";


            static string URL_ServerPts = "http://pts.saglik.gov.tr:80";
            static string URL_PackageReceiverWebService =
            URL_ServerPts + "/PTS/PackageReceiverWebService";
            static string URL_PackageSenderWebService =
             URL_ServerPts + "/PTS/PackageSenderWebService";
            static string URL_PackageTransferHelperWebService =
            URL_ServerPts + "/PTSHelper/PackageTransferHelperWebService";




            public class ImplService
            {
                public static string getPtsFileName(ITS_STOCKDOC pDoc)
                {

                    var dir = "../ava.work/PTS/OUT/";

                    if (!System.IO.Directory.Exists(dir))
                        System.IO.Directory.CreateDirectory(dir);

                    var fileName = string.Format(dir + "{0}_{1}.xml", pDoc.GET_TRCODE(), pDoc.GET_FICHENO());

                    return fileName;

                }


                public static string unzipPts(byte[] pData)
                {
                    var dir = "../ava.work/PTS/IN/";

                    if (!System.IO.Directory.Exists(dir))
                        System.IO.Directory.CreateDirectory(dir);


                    var dirTmp = GETTMPDIR();

                    var xmlFile = dir + "pts.xml"; //TODO as transfer id may be to name
                    var xmlFileZip = xmlFile + ".zip";


                    FILEWRITE(xmlFileZip, pData);
                    UNZIP(xmlFileZip, dirTmp);
                    var arr = System.IO.Directory.GetFiles(dirTmp, "*.xml");
                    if (arr.Length > 0)
                    {
                        System.IO.File.Delete(xmlFile);
                        System.IO.File.Move(arr[0], xmlFile);
                        return System.IO.File.ReadAllText(xmlFile);
                    }

                    throw new Exception("Cant unpack PTS packet");



                }


                public static string saveForPTSOut(_PLUGIN PLUGIN, object docRef, bool isInvoice, Dictionary<string, object> pRetData)
                {

                    var doc = new ITS_STOCKDOC(PLUGIN, docRef, isInvoice);
                    if (doc.VALID)
                    {

                        //ask or not



                        var operType = "";
                        if (doc.SERIALS_0.Length > 0 || doc.SERIALS_1.Length > 0)
                        {
                            switch (doc.GET_TRCODE())
                            {
                                case 1://prch
                                    {
                                        operType = "P";
                                    }
                                    break;

                                case 6://prch ret
                                    {
                                        operType = "R";
                                    }
                                    break;
                                case 7://sls
                                case 8://sls
                                    {
                                        operType = "S";
                                    }
                                    break;
                                case 2://sls ret
                                case 3://sls ret
                                    {
                                        operType = "C";
                                    }
                                    break;
                                case 11://fire
                                    {
                                        operType = "O";
                                    }
                                    break;
                                case 13://uretim
                                    {
                                        operType = "U";
                                    }
                                    break;
                                default:
                                    throw new Exception(TEXT.text_UNDEFINED_DOC_FOR_ITS);
                            }



                            /*
                                         
P=Mal Alim (product Purchase)
S=Satis (diSpatch)
C=Cancel Sale (dispatch Cancellation)
R=Iade (Return)
D=Deaktivasyon (Deactivation)
M=Uretim (production/Manufacture)
I=Ithalat (Importation)
X=Ihrac (eXportation)
O=Sarf (cOnsume)
N=Bilgi (iNformation)
T=Devir (Turnover)
L=Devir Iptal (turnover canceLlation)
F=Aktarim (non-its transFer)
K=Aktarim Iptal (non-its cancel transfer)

                                         
                             */

                            var docXml = wrapDocToPtsXml(PLUGIN, doc, operType, pRetData);

                            var fileName = getPtsFileName(doc);

                            FILEWRITE(fileName, docXml.OuterXml, true);

                            var fileNameZip = fileName + ".zip";

                            ZIP(new string[] { fileName }, fileName + ".zip");

                            return fileNameZip;

                        }

                        throw new Exception("T_MSG_DATA_NO");

                    }

                    throw new Exception("T_MSG_INVALID_DOC");

                }
                public static int ImportMedicine(bool pAll, _PLUGIN plugin)
                {


                    var webSrv = new WebServices.WebServiceDrug.Drug();


                    var body = new WebServices.WebServiceDrug.drugRequest()
                    {
                        getAll = false
                    };



                    var response = webSrv.CallDrug(body);
                    var count = 0;
                    foreach (var drugItem in response.drugs)
                    {

                        // addManufacturer(drugItem.gtin, drugItem.manufacturerName, plugin);
                        var d = addDrug(drugItem.gtin, drugItem.drugName, drugItem.manufacturerName, drugItem.isImported, plugin);

                        if (d)
                            ++count;
                    }


                    return count;
                }
                static bool addManufacturer(string code, string name, _PLUGIN plugin)
                {

                    name = name ?? ""; //can be null

                    var x = plugin.SQLSCALAR("SELECT 'OK' FROM LG_$FIRM$_CLCARD WHERE CODE = @P1", new object[] { code }) as string;

                    if (x == "OK")
                        return false;


                    if (name.Length > 100)
                        name = name.Substring(0, 100);


                    plugin.EXEADPCMD(new string[] { "adp.fin.rec.client/3" }, new DoWorkEventHandler[] { (s,args)=>{

                        args.Result = false;

                        var DS = ((DataSet)args.Argument);

                        var ITEMS = TAB_GETTAB(DS, "CLCARD");
 
                        TAB_SETCOL(ITEMS, "CODE", code);
                        TAB_SETCOL(ITEMS, "DEFINITION_", name);
                      //  TAB_SETCOL(ITEMS, "ACTIVE", 1);
                         
                        args.Result = true;
                    } }, false);

                    return true;
                }
                static bool addDrug(string code, string name, string nameManuf, bool import, _PLUGIN plugin)
                {

                    name = name ?? ""; //can be null
                    nameManuf = nameManuf ?? ""; //can be null


                    var code1 = code;
                    var code2 = code.TrimStart('0');
                    var x = plugin.SQLSCALAR("SELECT 'OK' FROM LG_$FIRM$_ITEMS WHERE CODE IN (@P1,@P2) ", new object[] { code1, code2 }) as string;

                    if (x == "OK")
                        return false;

                    if (name.Length > 100)
                        name = name.Substring(0, 100);

                    {

                        nameManuf = nameManuf.
                              Replace(" İLAÇ SAN. VE TİC. A.Ş.", "").
                              Replace(" İLAÇ VE KİMYA SANAYİİ A.Ş.", "").
                              Replace(" İLAÇ PAZARLAMA A.Ş.", "").
                              Replace(" İLAÇ SAN. VE TİC. LTD. ŞTİ.", "").
                              Replace(" DIŞ TİC. VE PAZ. A.Ş.", "").
                              Replace(" SAN. TİC. A.Ş.", "").
                              Replace(" TİC.LTD.ŞTİ.", "").
                              Replace(" TİC. A.Ş.", "").
                              Replace(" A.S.", "").
                              Replace(" A.Ş.", "")

                              ;

                    }
                    if (nameManuf.Length > 35)
                        nameManuf = nameManuf.Substring(0, 35);

                    plugin.EXEADPCMD(new string[] { "adp.mm.rec.mat/1" }, new DoWorkEventHandler[] { (s,args)=>{

                        args.Result = false;

                        var DS = ((DataSet)args.Argument);

                        var ITEMS = TAB_GETTAB(DS, "ITEMS");


                        TAB_SETCOL(ITEMS, "CODE", code);
                        TAB_SETCOL(ITEMS, "NAME", name);
                        TAB_SETCOL(ITEMS, "SPECODE", nameManuf);
                      //  TAB_SETCOL(ITEMS, "ACTIVE", 1);
                         
                        args.Result = true;
                    } }, false);

                    return true;
                }

                public static string CheckItemsByBarcode(string kareKod, _PLUGIN plugin)
                {

                    var barcodeObj = new TOOL_ITS.ITS_BARCODE(kareKod);
                    var stockDesc = GetGTINToStokAdi(plugin, barcodeObj.GTIN);

                    var list = new List<string>();
                    var title = "";



                    {
                        var webSrv2 = new WebServices.WebServiceUrunDogrulamaReceiverService.UrunDogrulamaReceiverService();
                        __INIT_SRV(webSrv2);

                        var body = new WebServices.WebServiceUrunDogrulamaReceiverService.UrunDogrulamaBildirimType()
                        {
                            DT = "V",
                            FR = ITS_WH_CODE
                        };
                        body.URUNLER = new WebServices.WebServiceUrunDogrulamaReceiverService.UrunDogrulamaBildirimTypeURUN[] {
                            new WebServices.WebServiceUrunDogrulamaReceiverService.UrunDogrulamaBildirimTypeURUN() {                           
                                BN = barcodeObj.BN,
                                GTIN = barcodeObj.GTIN,
                                SN = barcodeObj.SN,
                                XD = barcodeObj.XD_DATE
                            }
                        
                        };


                        var response = webSrv2.UrunDogrulamaBildir(body);
                        var urun = response.URUNLER[0];
                        title = getErrorCodeMsg(urun.UC);
                    }



                    //
                    list.Add("Kod");
                    list.Add(barcodeObj.GTIN);
                    list.Add("İsim");
                    list.Add(LEFT(stockDesc, 30));
                    list.Add("SKT");//"Son Kullanma Tarih");
                    list.Add(barcodeObj.XD_DATE.ToShortDateString());
                    list.Add("Parti");
                    list.Add(barcodeObj.BN);
                    list.Add("Seri");
                    list.Add(barcodeObj.SN);


                    //
                    {
                        //
                        var webSrv = new WebServices.WebServiceCheckStatusNotification.CheckStatusNotification();
                        __INIT_SRV(webSrv);

                        var request = new WebServices.WebServiceCheckStatusNotification.ItsPlainRequest();

                        request.PRODUCTS = new WebServices.WebServiceCheckStatusNotification.ItsPlainRequestPRODUCT[] { 
                            new WebServices.WebServiceCheckStatusNotification.ItsPlainRequestPRODUCT()
                            {
                                BN = barcodeObj.BN,
                                GTIN = barcodeObj.GTIN,
                                SN = barcodeObj.SN,
                                XD = barcodeObj.XD_DATE
                            }
                        };

                        var response = webSrv.sendCheckStatusNotification(request);

                        var PRODUCT = response.PRODUCTS[0];


                        string descGLN1 = GetGLNToCariUnvani(plugin, PRODUCT.GLN1);
                        string descGLN2 = GetGLNToCariUnvani(plugin, PRODUCT.GLN2);

                        if (ISEMPTY(descGLN1) && !ISEMPTY(PRODUCT.GLN1))
                            descGLN1 = PRODUCT.GLN1;

                        if (ISEMPTY(descGLN2) && !ISEMPTY(PRODUCT.GLN2))
                            descGLN2 = PRODUCT.GLN2;


                        if (!ISEMPTY(descGLN1))
                        {

                            list.Add("Kayıtlı/GLN");
                            list.Add("" + LEFT(descGLN1, 30) + "/" + PRODUCT.GLN1 + "");
                        }


                        if (!ISEMPTY(descGLN2))
                        {

                            list.Add("Kaynak/GLN");
                            list.Add("" + LEFT(descGLN2, 30) + "/" + PRODUCT.GLN2 + "");
                        }





                    }

                    var resMsg = new StringBuilder();
                    resMsg.AppendLine("<html>");
                    resMsg.AppendLine(@"
<style>
          table, th, td {
                border: 1px solid #dbdbdb;
                border-collapse: collapse;
                white-space: nowrap;
                padding: 2px;
                font-family:Segoe UI, Verdana, Aral;
                padding: 2px 0 0 2px;
            }
</style>

");
                    resMsg.Append("<h3><center>").Append(title).AppendLine("</center></h3>");
                    resMsg.AppendLine("<table>");
                    for (int i = 0; i < list.Count; i += 2)
                    {
                        resMsg.Append("<tr><td><b> ").Append(list[i]).Append(" </b></td><td> ").Append(list[i + 1]).AppendLine(" </td></tr>");
                    }
                    resMsg.AppendLine("</table>");
                    resMsg.AppendLine("</html>");


                    return resMsg.ToString();
                }
                //true if PTS Ok
                public static bool postDocToPTS(object docRef, bool isInvoice, _PLUGIN PLUGIN)
                {

                    try
                    {
                        string tabName = "LG_$FIRM$_$PERIOD$_" + (isInvoice ? "INVOICE" : "STFICHE");
                        string packTransferId = PLUGIN.SQLSCALAR("select DOCTRACKINGNR from " + tabName + " where LOGICALREF = @P1",
                                   new object[] { docRef }).ToString();

                        if (!string.IsNullOrEmpty(packTransferId))
                        {
                            PLUGIN.MSGUSERERROR("Document is marked as PTD done with TransferId: " + packTransferId);
                            return false;
                        }

                        var dic = new Dictionary<string, object>();

                        var file = saveForPTSOut(PLUGIN, docRef, isInvoice, dic);

                        if (file != null)
                        {
                            var srv = new WebServices.WebServicePacketGonder.PackageSenderWebService();

                            var res = srv._sendFileStream(PLUGIN, file, dic["destinationGLN"].ToString());

                            if (res.transferId > 0)
                                PLUGIN.SQL("update " + tabName + " set DOCTRACKINGNR = @P2 where LOGICALREF = @P1",
                                  new object[] { docRef, "PTS" + FORMAT(res.transferId) });


                            PLUGIN.MSGUSERINFO("T_MSG_OPERATION_FINISHED :" + (res != null ? res.transferId.ToString() : ""));
                        }


                    }
                    catch (Exception exc2)
                    {
                        PLUGIN.MSGUSERERROR(exc2.Message);
                    }

                    return false;
                }
                public static bool getDocFromPTS(DataSet pDs, _PLUGIN pPLUGIN, long pTransferId)
                {

                    try
                    {
                        var inv = TAB_GETTAB(pDs, "INVOICE");
                        var slip = TAB_GETTAB(pDs, "STFICHE");
                        var lines = TAB_GETTAB(pDs, "STLINE");
                        var tracks = UTIL_ITSTRACK.GET_TAB(pPLUGIN, pDs);

                        if (inv == null && slip == null)
                            throw new Exception("Doc dataset hasnt Inv on Slip table");
                        //
                        bool isInvoice = (inv != null);
                        var recHeader = isInvoice ? TAB_GETLASTROW(inv) : TAB_GETLASTROW(slip);

                        //
                        var trcode = CASTASSHORT(TAB_GETROW(recHeader, "TRCODE"));
                        var transferIdText = CASTASSTRING(TAB_GETROW(recHeader, "DOCTRACKINGNR"));

                        switch (trcode)
                        {
                            case 1:
                            case 8:
                                break;


                            default:
                                return false;
                        }
                        //

                        var isAdpCmdEditing = (recHeader.RowState != DataRowState.Added);


                        if (isAdpCmdEditing && tracks.Rows.Count > 0)
                            throw new Exception(TEXT.text_DOC_HAS_ITS_RECS);

                        //

                        var headerXml = new Dictionary<string, string>();
                        var linesXml = new List<ITS_SERIALS>();

                        if (pTransferId <= 0)
                        {
                            if (transferIdText.ToUpperInvariant().StartsWith("PTS"))
                                transferIdText = transferIdText.Substring(3);

                            long.TryParse(transferIdText, out pTransferId);

                            if (!pPLUGIN.MSGUSERASK(TEXT.text_GET_FROM_PTS))
                                return false;

                        }
                        if (pTransferId <= 0)
                        {

                            var f = new ITS_FORMS.FormPTSPacks(pPLUGIN)
                            {
                                defaultSourceGLN = TAB_GETROW(recHeader, "CLCARD_____CODE").ToString()


                            };

                            f.ShowDialog();

                            pTransferId = f.resultTransferId;


                        }
                        if (pTransferId <= 0)
                        {
                            return false;
                        }
                        var srv = new WebServices.WebServicePacketAl.PackageReceiverWebService();

                        var docXml = srv._receiveFileStream(pPLUGIN, pTransferId);




                        unWrapPtsXmlToDoc(docXml, headerXml, linesXml);

                        if (isInvoice)
                        {
                            if (headerXml.ContainsKey("sourceGLN"))
                            { }

                        }

                        //validate material
                        foreach (var x in linesXml)
                        {

                            while (!x.ROOT.LOAD_REC(pPLUGIN))
                            {

                                pPLUGIN.MSGUSERERROR(TEXT.text_ERR_NO_MAT + " [" + x.ROOT.GTIN + "] ?");
                                return false;

                                if (pPLUGIN.MSGUSERASK(TEXT.text_ASK_ADD_NEW_MAT + " [" + x.ROOT.GTIN + "] ?"))
                                {




                                    //var desc = MY_ASKSTRING(pPLUGIN, "T_NAME", null);
                                    // if (desc != null || desc != "")
                                    {
                                        //create mat
                                        pPLUGIN.EXEADPCMD("adp.mm.rec.mat/1 cmd::add", ((s, a) =>
                                        {
                                            var ds = a.Argument as DataSet;
                                            if (ds == null)
                                                return;
                                            var itm = TAB_GETTAB(ds, "ITEMS");

                                            TAB_SETCOL(itm, "CODE", x.ROOT.GTIN);
                                            TAB_SETCOL(itm, "NAME", TEXT.text_NEW_MAT_NAME + " " + x.ROOT.GTIN);

                                            a.Result = true;
                                        }), false);


                                    }
                                    //  else
                                    //     return false;
                                }
                                else
                                    return false;






                            }
                        }





                        //
                        TAB_SETROW(recHeader, "DOCTRACKINGNR", "PTS" + FORMAT(pTransferId));
                        TAB_SETROW(recHeader, "GENEXP1", headerXml.ContainsKey("note") ? headerXml["note"] : "");
                        TAB_SETROW(recHeader, "GENEXP2", headerXml.ContainsKey("documentDate") ? headerXml["documentDate"] : "");

                        if (!isAdpCmdEditing)
                            TAB_DELETE(lines);


                        if (!isAdpCmdEditing)
                            TAB_DELETE(tracks);


                        {

                            //1 find material
                            foreach (var x in linesXml)
                            {
                                if (x.ROOT.LOAD_REC(pPLUGIN))
                                {
                                    //
                                }
                                else
                                {
                                    LOG_JOURNAL(pPLUGIN, "Cant find material [" + x.ROOT.RAW + "]");
                                    pPLUGIN.MSGUSERERROR("Cant find material [" + x.ROOT.GTIN + "]");
                                }
                            }



                            //3 add serials
                            foreach (var x in linesXml)
                            {

                                //load serial nr to table
                                UTIL_ITSTRACK.ITSTRACK_ADD(pPLUGIN, pDs, x);

                            }

                            if (!isAdpCmdEditing)
                            {

                                //2 join items
                                Dictionary<object, double> items = new Dictionary<object, double>();
                                foreach (var x in linesXml)
                                {
                                    var key = TAB_GETROW(x.ROOT.MAT_REC, "LOGICALREF");
                                    var qty = x.SERIALS.Length;
                                    //
                                    items[key] = (items.ContainsKey(key) ? items[key] + qty : qty);

                                }


                                //4
                                var indx = 0;
                                foreach (var x in items)
                                {

                                    var r = lines.NewRow();
                                    lines.Rows.InsertAt(r, indx);
                                    ++indx;
                                    //

                                    TAB_SETROW(r, "STOCKREF", x.Key);
                                    TAB_SETROW(r, "AMOUNT", x.Value);
                                    //TAB_SETROW(r, "LINEEXP", x.ROOT.RAW);
                                    //

                                }


                            }


                            //var indx = 0;
                            //foreach (var x in linesXml)
                            //{
                            //    if (!isEditing)
                            //    {
                            //        var r = lines.NewRow();
                            //        lines.Rows.InsertAt(r, indx);
                            //        ++indx;
                            //        //

                            //        TAB_SETROW(r, "STOCKREF", TAB_GETROW(x.ROOT.MAT_REC, "LOGICALREF"));
                            //        TAB_SETROW(r, "AMOUNT", x.SERIALS.Length);
                            //        //TAB_SETROW(r, "LINEEXP", x.ROOT.RAW);
                            //        //

                            //    }

                            //    //load serial nr to table
                            //    UTIL_ITSTRACK.ITSTRACK_ADD(pPLUGIN, pDs, x);

                            //}

                        }

                        return true;

                    }
                    catch (Exception exc2)
                    {
                        pPLUGIN.MSGUSERERROR(exc2.Message);
                    }

                    return false;
                }


                public static System.Xml.XmlDocument wrapDocToPtsXml(_PLUGIN pPLUGIN, ITS_STOCKDOC pDoc, string pOperType, Dictionary<string, object> pRetData)
                {
                    pRetData["destinationGLN"] = LEFT(pDoc.CLIENT.GET_CODE(), 13);

                    var dic = new Dictionary<string, string>() { 
                                        {"sourceGLN",ITS_WH_CODE},
                                        {"destinationGLN",LEFT(pDoc.CLIENT.GET_CODE(),13)},
                                        {"actionType",pOperType},
                                        {"shipTo",LEFT(pDoc.CLIENT.GET_CODE(),13)},
                                        {"documentNumber",pDoc.GET_STR("FICHENO","")},
                                        {"documentDate",DATE_XML_FORMAT((DateTime) pDoc._GET("DATE_",null) )},
                                        {"note",""},
                                        {"version","1.4"},
                                      //  {"carrier",""} //last !!!
                                        };

                    var xmlDoc = new System.Xml.XmlDocument();

                    xmlDoc.AppendChild(xmlDoc.CreateProcessingInstruction("xml", "version='1.0' encoding='UTF-8'"));

                    var rootNode = xmlDoc.CreateElement("transfer");
                    // rootNode.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                    // rootNode.SetAttribute("xsi:noNamespaceSchemaLocation", "http://pts.saglik.gov.tr/pts_xml_schema_v_1_4.xsd");

                    //  var lastNode = rootNode;

                    xmlDoc.AppendChild(rootNode);

                    foreach (var k in dic.Keys)
                    {
                        //  lastNode = xmlDoc.CreateElement(k);
                        // lastNode.InnerText = dic[k];
                        // rootNode.AppendChild(lastNode);

                        var newElm = xmlDoc.CreateElement(k);
                        newElm.InnerText = dic[k];
                        rootNode.AppendChild(newElm);

                    }
                    //


                    var _serials = new List<ITS_BARCODE>();

                    _serials.AddRange(pDoc.SERIALS_0);
                    _serials.AddRange(pDoc.SERIALS_1);


                    var serials = ITS_SERIALS.create(_serials.ToArray());

                    foreach (var ser in serials)
                    {

                        var pack = ser.ROOT.GROUP_ ?? "";
                        var parts = pack.Split('.');
                        if (parts.Length == 0)
                            parts = new string[] { "" };

                        var lastRoot = rootNode;

                        foreach (var p in parts)
                        {
                            var newNode = xmlDoc.CreateElement("carrier");
                            newNode.SetAttribute("carrierLabel", p); //or "000..." max len 20
                            newNode.SetAttribute("containerType", "C");

                            lastRoot.AppendChild(newNode);

                            lastRoot = newNode;
                        }

                        var productList = xmlDoc.CreateElement("productList");
                        productList.SetAttribute("GTIN", ser.ROOT.GTIN);
                        productList.SetAttribute("lotNumber", ser.ROOT.BN);
                        productList.SetAttribute("expirationDate", DATE_XML_FORMAT(ser.ROOT.XD_DATE));

                        foreach (var str in ser.SERIALS)
                        {
                            var serialNumber = xmlDoc.CreateElement("serialNumber");
                            serialNumber.InnerText = str;
                            productList.AppendChild(serialNumber);

                        }

                        lastRoot.AppendChild(productList);
                        //



                    }

                    return xmlDoc;
                }

                public static void unWrapPtsXmlToDoc(string pDocXml, Dictionary<string, string> pHeader, List<ITS_SERIALS> pLines)
                {
                    //TODO sub carriers



                    var docXml = new System.Xml.XmlDocument();
                    docXml.LoadXml(pDocXml);


                    var node = docXml.FirstChild as System.Xml.XmlElement;


                    var tmpH_ = docXml.SelectNodes("transfer/*");
                    foreach (System.Xml.XmlNode x in tmpH_)
                    {

                        if (!string.IsNullOrEmpty(x.Name) && (x.FirstChild as System.Xml.XmlElement == null))
                            pHeader[x.Name] = x.InnerText;
                    }

                    //<carrier> can be multi level included and array
                    var tmpL_ = docXml.SelectNodes("transfer/carrier//productList");
                    foreach (System.Xml.XmlNode x in tmpL_)
                    {
                        if (x.Name == "productList")
                        {

                            string group_ = "";

                            {
                                var level = 0;
                                var parent = x.ParentNode;
                                while (parent != null)
                                {
                                    var elm = parent as System.Xml.XmlElement;


                                    if (elm != null && elm.Name == "carrier")
                                    {
                                        var label_ = elm.GetAttribute("carrierLabel") ?? "";
                                        //  var type_ = elm.GetAttribute("containerType")??"";

                                        if (!string.IsNullOrEmpty(label_))
                                        {
                                            group_ = label_ + (group_.Length == 0 ? "" : ".") + group_;
                                            ++level;

                                            if (level == 2)
                                                break;
                                        }

                                        parent = elm.ParentNode;


                                    }
                                    else
                                        break;
                                }
                            }
                            //

                            var e = x as System.Xml.XmlElement;

                            var ser = new ITS_SERIALS();

                            ser.ROOT = new ITS_BARCODE(
                             e.GetAttribute("GTIN"),
                             "",
                            DATE_XML_PARSE(e.GetAttribute("expirationDate")),
                             e.GetAttribute("lotNumber"),
                             group_
                             );

                            var list = new List<string>();
                            foreach (System.Xml.XmlNode l in x.ChildNodes)
                                if (l.Name == "serialNumber")
                                {
                                    list.Add(l.InnerText);
                                }

                            ser.SERIALS = list.ToArray();
                            if (ser.SERIALS.Length > 0)
                                pLines.Add(ser);
                        }
                    }


                }


                //true if ITS Ok
                public static bool sendDocToITS(object docRef, bool isInvoice, _PLUGIN PLUGIN)
                {



                    try
                    {

                        var slipRef = isInvoice ? PLUGIN.SQLSCALAR("SELECT LOGICALREF FROM LG_$FIRM$_$PERIOD$_STFICHE WHERE INVOICEREF = @P1", new object[] { docRef }) : docRef;
                        var docHedaerRec = TAB_GETLASTROW(PLUGIN.SQL(string.Format("SELECT FICHENO,DATE_,SPECODE,TRCODE,CLIENTREF FROM LG_$FIRM$_$PERIOD$_{0} WHERE LOGICALREF = @P1", (isInvoice ? "INVOICE" : "STFICHE")), new object[] { docRef }));

                        if (docHedaerRec == null)
                            throw new Exception("Incorrect doc,has no header rec");

                        var fishNo = CASTASSTRING(TAB_GETROW(docHedaerRec, "FICHENO"));
                        var fishDate = CASTASDATE(TAB_GETROW(docHedaerRec, "DATE_"));
                        var speCode = CASTASSTRING(TAB_GETROW(docHedaerRec, "SPECODE"));
                        var trcode = CASTASSHORT(TAB_GETROW(docHedaerRec, "TRCODE"));
                        var clRef = (TAB_GETROW(docHedaerRec, "CLIENTREF"));

                        var clObj = new ITS_CLIENT(PLUGIN, clRef);


                        {
                            try
                            {
                                //ask or not
                                {

                                    var input = (short)1;
                                    var output = (short)4;

                                    var ok = true;
                                    var subItems = getTrackNoForITS(slipRef, PLUGIN, 0);
                                    if (subItems.Length > 0)
                                    {
                                        switch (trcode)
                                        {
                                            case 1://prch (input)
                                                {

                                                    if (!PLUGIN.MSGUSERASK(TEXT.text_MSG_DOC1TOITS))
                                                        return false;

                                                    var srv = new TOOL_ITS.WebServices.WebServiceReceiptNotification.ReceiptNotification();
                                                    var response = srv._sendReceiptNotification(PLUGIN, clObj, subItems, fishNo);
                                                    foreach (var x in response.PRODUCTS)
                                                    {
                                                        var valid = ITEMGOTMSG(PLUGIN, x.GTIN, x.SN, slipRef, x.UC, srv, input);
                                                        if (!valid)
                                                        {
                                                            ok = false;
                                                            //
                                                        }
                                                    }
                                                }
                                                break;

                                            case 6://prch ret (output)
                                                {

                                                    if (!PLUGIN.MSGUSERASK(TEXT.text_MSG_DOC6TOITS))
                                                        return false;

                                                    var srv = new TOOL_ITS.WebServices.WebServiceReturnNotification.ReturnNotification();
                                                    var response = srv._sendReturnNotification(PLUGIN, clObj, subItems, fishNo);
                                                    foreach (var x in response.PRODUCTS)
                                                    {
                                                        var valid = ITEMGOTMSG(PLUGIN, x.GTIN, x.SN, slipRef, x.UC, srv, output);
                                                        if (!valid)
                                                        {
                                                            ok = false;
                                                            //
                                                        }
                                                    }
                                                }
                                                break;
                                            case 7://sls (output)
                                            case 8://sls (output)
                                                {
                                                    var isExport = (
                                                        (speCode.Trim().StartsWith("EXPORT"))//EXPORT.632
                                                        );

                                                    if (!isExport)
                                                    {
                                                        if (!PLUGIN.MSGUSERASK(TEXT.text_MSG_DOC8TOITS))
                                                            return false;

                                                        var srv = new TOOL_ITS.WebServices.WebServiceDispatchNotification.DispatchNotification();
                                                        var response = srv._sendDispatchNotification(PLUGIN, clObj, subItems, fishNo);
                                                        foreach (var x in response.PRODUCTS)
                                                        {
                                                            var valid = ITEMGOTMSG(PLUGIN, x.GTIN, x.SN, slipRef, x.UC, srv, output);
                                                            if (!valid)
                                                            {
                                                                ok = false;
                                                                //
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (!PLUGIN.MSGUSERASK(TEXT.text_MSG_DOC8TOITS_EXP))
                                                            return false;


                                                        var arr = speCode.Split('.');

                                                        if (arr.Length != 2)
                                                            throw new Exception("Export code shuld be like 'EXPORT.632'");

                                                        var srv = new TOOL_ITS.WebServices.WebServiceIhracat.IhracatReceiverService();
                                                        var response = srv._Ihracat(PLUGIN, clObj, subItems, fishNo, arr[1], fishDate);
                                                        foreach (var x in response.URUNLER)
                                                        {
                                                            var valid = ITEMGOTMSG(PLUGIN, x.GTIN, x.SN, slipRef, x.UC, srv, output);
                                                            if (!valid)
                                                            {
                                                                ok = false;
                                                                //
                                                            }
                                                        }

                                                    }
                                                }
                                                break;
                                            case 2://sls ret (input)
                                            case 3://sls ret (input)
                                                {

                                                    if (!PLUGIN.MSGUSERASK(TEXT.text_MSG_DOC3TOITS))
                                                        return false;

                                                    var srv = new TOOL_ITS.WebServices.WebServiceReturnDispatchNotification.DispatchCancellation();
                                                    var response = srv._sendDispatchCancellation(PLUGIN, clObj, subItems, fishNo);
                                                    foreach (var x in response.PRODUCTS)
                                                    {
                                                        var valid = ITEMGOTMSG(PLUGIN, x.GTIN, x.SN, slipRef, x.UC, srv, input);
                                                        if (!valid)
                                                        {
                                                            ok = false;
                                                            //
                                                        }
                                                    }
                                                }
                                                break;
                                            case 11://fire (output)
                                                {
                                                    if (!PLUGIN.MSGUSERASK(TEXT.text_MSG_DOC11TOITS))
                                                        return false;


                                                    var srv = new TOOL_ITS.WebServices.WebServiceDeaktivasyonNotification.DeaktivasyonBildirimReceiverService();
                                                    var response = srv._DeaktivasyonBildir(PLUGIN, clObj, subItems, fishNo, fishDate);
                                                    foreach (var x in response.URUNLER)
                                                    {
                                                        var valid = ITEMGOTMSG(PLUGIN, x.GTIN, x.SN, slipRef, x.UC, srv, output);
                                                        if (!valid)
                                                        {
                                                            ok = false;
                                                            //
                                                        }
                                                    }


                                                }
                                                break;
                                            case 13://uretim (input)
                                                {
                                                    if (!PLUGIN.MSGUSERASK(TEXT.text_MSG_DOC13TOITS))
                                                        return false;

                                                    var arrTmp = ITS_SERIALS.create(subItems);

                                                    foreach (var serTmp in arrTmp)
                                                    {

                                                        var srv = new TOOL_ITS.WebServices.WebServiceProduction.UretimBildirimReceiverService();
                                                        var response = srv._UretimBildir(PLUGIN, clObj, serTmp, fishNo, fishDate);
                                                        foreach (var x in response.URUNLER)
                                                        {
                                                            var valid = ITEMGOTMSG(PLUGIN, response.GTIN, x.SN, slipRef, x.UC, srv, input);
                                                            if (!valid)
                                                            {
                                                                ok = ok && false;
                                                                //
                                                            }
                                                        }

                                                    }
                                                }
                                                break;
                                            default:
                                                throw new Exception(TEXT.text_UNDEFINED_DOC_FOR_ITS);
                                        }
                                    }
                                    else
                                    {
                                        PLUGIN.MSGUSERINFO("T_MSG_DATA_NO");
                                        return false;
                                    }


                                    if (!ok)
                                    {
                                        PLUGIN.MSGUSERERROR("T_MSG_OPERATION_FAILED");
                                    }
                                    else
                                    {
                                        DOCITSFINISHED(PLUGIN, docRef, isInvoice);//mark doc for user
                                        PLUGIN.MSGUSERINFO("T_MSG_OPERATION_FINISHED");
                                        return true;
                                    }

                                }
                            }
                            catch (Exception exc)
                            {
                                PLUGIN.MSGUSERERROR(exc.Message);
                            }
                        }


                    }
                    catch (Exception exc2)
                    {
                        PLUGIN.MSGUSERERROR(exc2.Message);
                    }

                    return false;
                }


                public static ITS_BARCODE[] getTrackNoForITS(object pSlipRef, _PLUGIN PLUGIN, short pStatus)
                {

                    var t = PLUGIN.SQL(@"SELECT TRACKNO,GROUP_ FROM LG_$FIRM$_$PERIOD$_ITSTRACK WHERE STFICHEREF = @P1 AND STATUS_ = @P2 ORDER BY GROUP_ ASC,TRACKNO ASC", new object[] { pSlipRef, pStatus });

                    List<ITS_BARCODE> list = new List<ITS_BARCODE>();
                    foreach (DataRow r in t.Rows)
                    {
                        var b = new ITS_BARCODE(TAB_GETROW(r, "TRACKNO").ToString());
                        b.GROUP_ = TAB_GETROW(r, "GROUP_").ToString();
                        list.Add(b);
                    }

                    return list.ToArray();

                }


            }


            public class WebServices
            {

                public interface SrvExt
                {
                    string srvDesc { get; }
                    int[] successfulOperCodes { get; }
                }

                public class WebServiceDrug
                {


                    [XmlInclude(typeof(drug[]))]
                    [System.Web.Services.WebServiceBinding(Name = "DrugPortBinding", Namespace = "http://services.reference.its/")]
                    public class Drug : System.Web.Services.Protocols.SoapHttpClientProtocol, SrvExt
                    {

                        public Drug()
                        {
                            this.Url = URL_Drug;

                            __INIT_SRV(this);
                        }

                        [System.Web.Services.Protocols.SoapDocumentMethod("", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
                        [return: XmlElement("response", Namespace = "http://services.reference.its/", IsNullable = true)]
                        public drugResponse CallDrug([XmlElement(Namespace = "http://services.reference.its/", IsNullable = true)] drugRequest request)
                        {
                            object[] objArray = new object[] { request };
                            return (drugResponse)base.Invoke("CallDrug", objArray)[0];
                        }



                        public string srvDesc
                        {
                            get { return "Drug List Service"; }
                        }

                        public int[] successfulOperCodes
                        {
                            get
                            {
                                return new int[] { 
   
                            };
                            }
                        }


                    }







                    [XmlType(Namespace = "http://services.reference.its/")]
                    [Serializable]
                    public class drug
                    {


                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string drugName;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string gtin;
                        [XmlAttribute]
                        public bool isActive;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public bool isImported;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string manufacturerGLN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string manufacturerName;


                    }

                    [XmlType(Namespace = "http://services.reference.its/")]
                    [Serializable]
                    public class drugRequest
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public bool getAll;
                    }

                    [XmlType(Namespace = "http://services.reference.its/")]
                    [Serializable]
                    public class drugResponse
                    {


                        [XmlArray(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        [XmlArrayItem(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public drug[] drugs;

                    }




                }

                public class WebServiceErrorCode
                {

                    [XmlInclude(typeof(errorCode[]))]
                    [System.Web.Services.WebServiceBinding(Name = "ErrorCodePortBinding", Namespace = "http://services.reference.its/")]
                    public class ErrorCode : System.Web.Services.Protocols.SoapHttpClientProtocol
                    {


                        public ErrorCode()
                        {
                            this.Url = URL_ErrorCode;

                        }



                        [System.Web.Services.Protocols.SoapDocumentMethod("", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
                        [return: XmlElement("response", Namespace = "http://services.reference.its/", IsNullable = true)]
                        public errorCodeResponse getErrorCodes([XmlElement(Namespace = "http://services.reference.its/", IsNullable = true)] errorCodeRequest request)
                        {
                            object[] objArray = new object[] { request };
                            return (errorCodeResponse)base.Invoke("getErrorCodes", objArray)[0];
                        }

                    }



                    [XmlType(Namespace = "http://services.reference.its/")]
                    [Serializable]
                    public class errorCode
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string code;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string description;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string message;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string type;

                    }


                    [XmlType(Namespace = "http://services.reference.its/")]
                    [Serializable]
                    public class errorCodeRequest
                    {

                    }

                    [XmlType(Namespace = "http://services.reference.its/")]
                    [Serializable]
                    public class errorCodeResponse
                    {


                        [XmlArray(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        [XmlArrayItem(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public errorCode[] errorCodes;


                    }




                }

                public class WebServiceCheckStatusNotification
                {

                    [System.Web.Services.WebServiceBinding(Name = "CheckStatusNotificationPortBinding", Namespace = "http://its.iegm.gov.tr/notification/queryStatus")]
                    public class CheckStatusNotification : System.Web.Services.Protocols.SoapHttpClientProtocol
                    {


                        public CheckStatusNotification()
                        {
                            this.Url = URL_CheckStatusNotification;

                        }

                        [System.Web.Services.Protocols.SoapDocumentMethod("http://its.iegm.gov.tr/notification/queryStatus/ItsRequest", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
                        [return: XmlElement("QueryResponse", Namespace = "http://its.iegm.gov.tr/notification/queryStatus", IsNullable = true)]
                        public ItsResponse sendCheckStatusNotification([XmlElement(Namespace = "http://its.iegm.gov.tr/notification/queryStatus", IsNullable = true)] ItsPlainRequest QueryRequest)
                        {
                            object[] queryRequest = new object[] { QueryRequest };
                            return (ItsResponse)base.Invoke("sendCheckStatusNotification", queryRequest)[0];
                        }

                    }



                    [XmlType(Namespace = "http://its.iegm.gov.tr/notification/queryStatus")]
                    [Serializable]
                    public class ItsPlainRequest
                    {

                        [XmlArray(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        [XmlArrayItem("PRODUCT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
                        public ItsPlainRequestPRODUCT[] PRODUCTS;


                    }


                    [XmlType(AnonymousType = true, Namespace = "http://its.iegm.gov.tr/notification/queryStatus")]
                    [Serializable]
                    public class ItsPlainRequestPRODUCT
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string BN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string GTIN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
                        public DateTime XD;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string SN;


                    }


                    [XmlType(Namespace = "http://its.iegm.gov.tr/notification/queryStatus")]
                    [Serializable]
                    public class ItsResponse
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string NOTIFICATIONID;
                        [XmlArray(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        [XmlArrayItem("PRODUCT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
                        public ItsResponsePRODUCT[] PRODUCTS;


                    }


                    [XmlType(AnonymousType = true, Namespace = "http://its.iegm.gov.tr/notification/queryStatus")]
                    [Serializable]
                    public class ItsResponsePRODUCT
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string GLN1;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string GLN2;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string GTIN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string SN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string UC;

                    }

                }


                public class WebServiceUrunDogrulamaReceiverService
                {


                    [System.Web.Services.WebServiceBinding(Name = "UrunDogrulamaReceiverBinding", Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/UrunDogrulama/Genel")]
                    public class UrunDogrulamaReceiverService : System.Web.Services.Protocols.SoapHttpClientProtocol
                    {

                        public UrunDogrulamaReceiverService()
                        {
                            this.Url = URL_UrunDogrulamaReceiverService;
                        }

                        [System.Web.Services.Protocols.SoapDocumentMethod("http://its.iegm.gov.tr/bildirim/BR/v1/UrunDogrulama/UrunDogrulamaBildirimRequest", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
                        [return: XmlElement("UrunDogrulamaBildirimCevap", Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/UrunDogrulama/Genel")]
                        public UrunDogrulamaBildirimCevapType UrunDogrulamaBildir([XmlElement(Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/UrunDogrulama/Genel")] UrunDogrulamaBildirimType UrunDogrulamaBildirim)
                        {
                            object[] urunDogrulamaBildirim = new object[] { UrunDogrulamaBildirim };
                            return (UrunDogrulamaBildirimCevapType)base.Invoke("UrunDogrulamaBildir", urunDogrulamaBildirim)[0];
                        }


                    }





                    [XmlType(Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/UrunDogrulama/Genel")]
                    [Serializable]
                    public class UrunDogrulamaBildirimCevapType
                    {
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string BILDIRIMID;
                        [XmlArray(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        [XmlArrayItem("URUNDURUM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
                        public UrunDogrulamaBildirimCevapTypeURUNDURUM[] URUNLER;
                    }

                    [XmlType(AnonymousType = true, Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/UrunDogrulama/Genel")]
                    [Serializable]
                    public class UrunDogrulamaBildirimCevapTypeURUNDURUM
                    {
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string GTIN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string SN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string UC;
                    }


                    [XmlType(Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/UrunDogrulama/Genel")]
                    [Serializable]
                    public class UrunDogrulamaBildirimType
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string DT;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string FR;
                        [XmlArray(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        [XmlArrayItem("URUN", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
                        public UrunDogrulamaBildirimTypeURUN[] URUNLER;


                    }


                    [XmlType(AnonymousType = true, Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/UrunDogrulama/Genel")]
                    [Serializable]
                    public class UrunDogrulamaBildirimTypeURUN
                    {
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string BN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string GTIN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
                        public DateTime XD;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string SN;
                    }


                }

                public class WebServiceReceiptNotification
                {

                    [System.Web.Services.WebServiceBinding(Name = "ReceiptNotificationPortBinding", Namespace = "http://its.iegm.gov.tr/notification/receipt")]
                    public class ReceiptNotification : System.Web.Services.Protocols.SoapHttpClientProtocol, SrvExt
                    {

                        public ReceiptNotification()
                        {
                            this.Url = URL_ReceiptNotification;


                            __INIT_SRV(this);
                        }

                        [System.Web.Services.Protocols.SoapDocumentMethod("http://its.iegm.gov.tr/notification/receipt/ItsRequest", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
                        [return: XmlElement("ReceiptResponse", Namespace = "http://its.iegm.gov.tr/notification/receipt", IsNullable = true)]
                        public ItsResponse sendReceiptNotification([XmlElement(Namespace = "http://its.iegm.gov.tr/notification/receipt", IsNullable = true)] ItsPlainRequest ReceiptRequest)
                        {
                            object[] receiptRequest = new object[] { ReceiptRequest };
                            return (ItsResponse)base.Invoke("sendReceiptNotification", receiptRequest)[0];
                        }


                        public ItsResponse _sendReceiptNotification(_PLUGIN pPLUGIN, ITS_CLIENT pCl, ITS_BARCODE[] pArr, string pExtInfo)
                        {

                            //if (!pCl.VALID || pCl.GET_CODE() == "")
                            //    throw new Exception(TEXT.text_ERR_CLIENT_GLN_INVALID);

                            LOG_JOURNAL(pPLUGIN, srvDesc + ": " + pCl.GET_CODE() + ": " + pCl.GET_DESC() + ": " + (pExtInfo ?? ""));

                            var itemsList = new List<ItsPlainRequestPRODUCT>();
                            //

                            foreach (var b in pArr)
                            {

                                var x = new ItsPlainRequestPRODUCT()
                                {
                                    GTIN = b.GTIN,
                                    BN = b.BN,
                                    SN = b.SN,
                                    XD = b.XD_DATE
                                };

                                itemsList.Add(x);
                            }


                            var itemsObj = new ItsPlainRequest()
                                        {
                                            PRODUCTS = itemsList.ToArray()
                                        };

                            return sendReceiptNotification(itemsObj);
                        }


                        public string srvDesc
                        {
                            get { return "Alış Servis"; }
                        }

                        public int[] successfulOperCodes
                        {
                            get
                            {
                                return new int[] { 
                                10303,
                                10223 
                            };
                            }
                        }
                    }


                    [XmlType(AnonymousType = true, Namespace = "http://its.iegm.gov.tr/notification/receipt")]
                    [Serializable]
                    public class ItsResponsePRODUCT
                    {


                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string GTIN;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string SN;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string UC;

                        public ItsResponsePRODUCT()
                        {
                        }
                    }

                    [XmlType(Namespace = "http://its.iegm.gov.tr/notification/receipt")]
                    [Serializable]
                    public class ItsResponse
                    {


                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string NOTIFICATIONID;

                        [XmlArray(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        [XmlArrayItem("PRODUCT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
                        public ItsResponsePRODUCT[] PRODUCTS;

                        public ItsResponse()
                        {
                        }
                    }

                    [XmlType(AnonymousType = true, Namespace = "http://its.iegm.gov.tr/notification/receipt")]
                    [Serializable]
                    public class ItsPlainRequestPRODUCT
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string BN;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string GTIN;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
                        public DateTime XD;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string SN;

                        public ItsPlainRequestPRODUCT()
                        {
                        }
                    }

                    [XmlType(Namespace = "http://its.iegm.gov.tr/notification/receipt")]
                    [Serializable]
                    public class ItsPlainRequest
                    {


                        [XmlArray(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        [XmlArrayItem("PRODUCT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
                        public ItsPlainRequestPRODUCT[] PRODUCTS;
                        public ItsPlainRequest()
                        {
                        }
                    }
                }

                public class WebServiceReturnNotification
                {

                    [System.Web.Services.WebServiceBinding(Name = "ReturnNotificationPortBinding", Namespace = "http://its.iegm.gov.tr/p2/notification/return")]
                    public class ReturnNotification : System.Web.Services.Protocols.SoapHttpClientProtocol, SrvExt
                    {

                        public ReturnNotification()
                        {
                            this.Url = URL_ReturnNotification;

                            __INIT_SRV(this);

                        }


                        [System.Web.Services.Protocols.SoapDocumentMethod("http://its.iegm.gov.tr/p2/notification/return/ItsRequest", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
                        [return: XmlElement("ReturnResponse", Namespace = "http://its.iegm.gov.tr/p2/notification/return", IsNullable = true)]
                        public ItsResponse sendReturnNotification([XmlElement(Namespace = "http://its.iegm.gov.tr/p2/notification/return", IsNullable = true)] ItsRequest ReturnRequest)
                        {
                            object[] returnRequest = new object[] { ReturnRequest };
                            return (ItsResponse)base.Invoke("sendReturnNotification", returnRequest)[0];
                        }



                        public ItsResponse _sendReturnNotification(_PLUGIN pPLUGIN, ITS_CLIENT pCl, ITS_BARCODE[] pArr, string pExtInfo)
                        {

                            if (!pCl.VALID || pCl.GET_CODE() == "")
                                throw new Exception(TEXT.text_ERR_CLIENT_GLN_INVALID);

                            LOG_JOURNAL(pPLUGIN, srvDesc + ": " + pCl.GET_CODE() + ": " + pCl.GET_DESC() + ": " + (pExtInfo ?? ""));


                            var itemsList = new List<ItsRequestPRODUCT>();
                            //

                            foreach (var b in pArr)
                            {

                                var x = new ItsRequestPRODUCT()
                                {
                                    GTIN = b.GTIN,
                                    BN = b.BN,
                                    SN = b.SN,
                                    XD = b.XD_DATE
                                };

                                itemsList.Add(x);
                            }


                            var itemsObj = new ItsRequest()
                            {
                                PRODUCTS = itemsList.ToArray(),
                                TOGLN = pCl.GET_CODE()
                            };

                            return sendReturnNotification(itemsObj);
                        }


                        public string srvDesc
                        {
                            get { return "Alış İade Servis"; }
                        }

                        public int[] successfulOperCodes
                        {
                            get
                            {
                                return new int[] { 
                              
                            };
                            }
                        }
                    }



                    [XmlType(AnonymousType = true, Namespace = "http://its.iegm.gov.tr/p2/notification/return")]
                    [Serializable]
                    public class ItsResponsePRODUCT
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string GTIN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string SN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string UC;


                    }

                    [XmlType(Namespace = "http://its.iegm.gov.tr/p2/notification/return")]
                    [Serializable]
                    public class ItsResponse
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string NOTIFICATIONID;

                        [XmlArray(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        [XmlArrayItem("PRODUCT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
                        public ItsResponsePRODUCT[] PRODUCTS;


                    }

                    [XmlType(AnonymousType = true, Namespace = "http://its.iegm.gov.tr/p2/notification/return")]
                    [Serializable]
                    public class ItsRequestPRODUCT
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string BN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string GTIN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
                        public DateTime XD;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string SN;

                    }

                    [XmlType(Namespace = "http://its.iegm.gov.tr/p2/notification/return")]
                    [Serializable]
                    public class ItsRequest
                    {

                        [XmlArray(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        [XmlArrayItem("PRODUCT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
                        public ItsRequestPRODUCT[] PRODUCTS;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string TOGLN;

                    }


                }

                public class WebServiceDispatchNotification
                {
                    [System.Web.Services.WebServiceBinding(Name = "DispatchNotificationPortBinding", Namespace = "http://its.iegm.gov.tr/p2/notification/dispatch")]
                    public class DispatchNotification : System.Web.Services.Protocols.SoapHttpClientProtocol, SrvExt
                    {


                        public DispatchNotification()
                        {
                            this.Url = URL_DispatchNotification;

                            __INIT_SRV(this);
                        }


                        [System.Web.Services.Protocols.SoapDocumentMethod("http://its.iegm.gov.tr/p2/notification/dispatch/ItsRequest", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
                        [return: XmlElement("DispatchResponse", Namespace = "http://its.iegm.gov.tr/p2/notification/dispatch", IsNullable = true)]
                        public ItsResponse sendDispatchNotification([XmlElement(Namespace = "http://its.iegm.gov.tr/p2/notification/dispatch", IsNullable = true)] ItsRequest DispatchRequest)
                        {
                            object[] dispatchRequest = new object[] { DispatchRequest };
                            return (ItsResponse)base.Invoke("sendDispatchNotification", dispatchRequest)[0];
                        }



                        public ItsResponse _sendDispatchNotification(_PLUGIN pPLUGIN, ITS_CLIENT pCl, ITS_BARCODE[] pArr, string pExtInfo)
                        {

                            if (!pCl.VALID || pCl.GET_CODE() == "")
                                throw new Exception(TEXT.text_ERR_CLIENT_GLN_INVALID);

                            LOG_JOURNAL(pPLUGIN, srvDesc + ": " + pCl.GET_CODE() + ": " + pCl.GET_DESC() + ": " + (pExtInfo ?? ""));


                            var itemsList = new List<ItsRequestPRODUCT>();
                            //

                            foreach (var b in pArr)
                            {

                                var x = new ItsRequestPRODUCT()
                                {
                                    GTIN = b.GTIN,
                                    BN = b.BN,
                                    SN = b.SN,
                                    XD = b.XD_DATE
                                };

                                itemsList.Add(x);
                            }


                            var itemsObj = new ItsRequest()
                            {
                                PRODUCTS = itemsList.ToArray(),
                                TOGLN = pCl.GET_CODE()
                            };

                            return sendDispatchNotification(itemsObj);
                        }


                        public string srvDesc
                        {
                            get { return "Satış Servis"; }
                        }

                        public int[] successfulOperCodes
                        {
                            get
                            {
                                return new int[] { 
                              40004
                            };
                            }
                        }

                    }


                    [XmlType(Namespace = "http://its.iegm.gov.tr/p2/notification/dispatch")]
                    [Serializable]
                    public class ItsRequest
                    {

                        [XmlArray(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        [XmlArrayItem("PRODUCT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
                        public ItsRequestPRODUCT[] PRODUCTS;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string TOGLN;

                    }


                    [XmlType(AnonymousType = true, Namespace = "http://its.iegm.gov.tr/p2/notification/dispatch")]
                    [Serializable]
                    public class ItsRequestPRODUCT
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string BN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string GTIN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
                        public DateTime XD;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string SN;

                    }

                    [XmlType(Namespace = "http://its.iegm.gov.tr/p2/notification/dispatch")]
                    [Serializable]
                    public class ItsResponse
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string NOTIFICATIONID;
                        [XmlArray(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        [XmlArrayItem("PRODUCT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
                        public ItsResponsePRODUCT[] PRODUCTS;


                    }


                    [XmlType(AnonymousType = true, Namespace = "http://its.iegm.gov.tr/p2/notification/dispatch")]
                    [Serializable]
                    public class ItsResponsePRODUCT
                    {
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string GTIN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string SN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string UC;

                    }


                }

                public class WebServiceReturnDispatchNotification
                {
                    [System.Web.Services.WebServiceBinding(Name = "DispatchCancellationPortBinding", Namespace = "http://its.iegm.gov.tr/p2/cancellation/dispatch")]
                    public class DispatchCancellation : System.Web.Services.Protocols.SoapHttpClientProtocol, SrvExt
                    {


                        public DispatchCancellation()
                        {
                            this.Url = URL_DispatchCancellation;

                            __INIT_SRV(this);

                        }


                        [System.Web.Services.Protocols.SoapDocumentMethod("http://its.iegm.gov.tr/p2/cancellation/dispatch/ItsRequest", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
                        [return: XmlElement("DispatchCancellationResponse", Namespace = "http://its.iegm.gov.tr/p2/cancellation/dispatch", IsNullable = true)]
                        public ItsResponse sendDispatchCancellation([XmlElement(Namespace = "http://its.iegm.gov.tr/p2/cancellation/dispatch", IsNullable = true)] ItsPlainRequest DispatchCancellationRequest)
                        {
                            object[] dispatchCancellationRequest = new object[] { DispatchCancellationRequest };
                            return (ItsResponse)base.Invoke("sendDispatchCancellation", dispatchCancellationRequest)[0];
                        }



                        public ItsResponse _sendDispatchCancellation(_PLUGIN pPLUGIN, ITS_CLIENT pCl, ITS_BARCODE[] pArr, string pExtInfo)
                        {

                            if (!pCl.VALID || pCl.GET_CODE() == "")
                                throw new Exception(TEXT.text_ERR_CLIENT_GLN_INVALID);

                            LOG_JOURNAL(pPLUGIN, srvDesc + ": " + pCl.GET_CODE() + ": " + pCl.GET_DESC() + ": " + (pExtInfo ?? ""));


                            var itemsList = new List<ItsPlainRequestPRODUCT>();
                            //

                            foreach (var b in pArr)
                            {

                                var x = new ItsPlainRequestPRODUCT()
                                {
                                    GTIN = b.GTIN,
                                    BN = b.BN,
                                    SN = b.SN,
                                    XD = b.XD_DATE
                                };

                                itemsList.Add(x);
                            }


                            var itemsObj = new ItsPlainRequest()
                            {
                                PRODUCTS = itemsList.ToArray()
                            };

                            return sendDispatchCancellation(itemsObj);
                        }


                        public string srvDesc
                        {
                            get { return "Satış İade Servis"; }
                        }

                        public int[] successfulOperCodes
                        {
                            get
                            {
                                return new int[] { 
                              10223
                            };
                            }
                        }
                    }


                    [XmlType(AnonymousType = true, Namespace = "http://its.iegm.gov.tr/p2/cancellation/dispatch")]
                    [Serializable]
                    public class ItsResponsePRODUCT
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string GTIN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string SN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string UC;
                    }



                    [XmlType(Namespace = "http://its.iegm.gov.tr/p2/cancellation/dispatch")]
                    [Serializable]
                    public class ItsResponse
                    {
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string NOTIFICATIONID;
                        [XmlArray(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        [XmlArrayItem("PRODUCT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
                        public ItsResponsePRODUCT[] PRODUCTS;
                    }



                    [XmlType(AnonymousType = true, Namespace = "http://its.iegm.gov.tr/p2/cancellation/dispatch")]
                    [Serializable]
                    public class ItsPlainRequestPRODUCT
                    {


                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string BN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string GTIN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
                        public DateTime XD;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string SN;

                    }


                    [XmlType(Namespace = "http://its.iegm.gov.tr/p2/cancellation/dispatch")]
                    [Serializable]
                    public class ItsPlainRequest
                    {

                        [XmlArray(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        [XmlArrayItem("PRODUCT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
                        public ItsPlainRequestPRODUCT[] PRODUCTS;


                    }



                }

                public class WebServiceDeaktivasyonNotification
                {



                    [System.Web.Services.WebServiceBinding(Name = "DeaktivasyonBildirimReceiverBinding", Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/Deaktivasyon")]
                    public class DeaktivasyonBildirimReceiverService : System.Web.Services.Protocols.SoapHttpClientProtocol, SrvExt
                    {



                        public DeaktivasyonBildirimReceiverService()
                        {
                            this.Url = URL_DeaktivasyonBildirimReceiverService;
                            __INIT_SRV(this);
                        }



                        [System.Web.Services.Protocols.SoapDocumentMethod("http://its.iegm.gov.tr/bildirim/BildirimReceiver/v1/DeaktivasyonBildirRequest", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
                        [return: XmlElement("DeaktivasyonBildirimCevap", Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/Deaktivasyon")]
                        public DeaktivasyonBildirimCevapType DeaktivasyonBildir([XmlElement(Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/Deaktivasyon")] DeaktivasyonBildirimType DeaktivasyonBildirim)
                        {
                            object[] deaktivasyonBildirim = new object[] { DeaktivasyonBildirim };
                            return (DeaktivasyonBildirimCevapType)base.Invoke("DeaktivasyonBildir", deaktivasyonBildirim)[0];
                        }



                        public DeaktivasyonBildirimCevapType _DeaktivasyonBildir(_PLUGIN pPLUGIN, ITS_CLIENT pCl, ITS_BARCODE[] pArr, string pDocNo, DateTime pDocDate)
                        {

                            //  if (!pCl.VALID || pCl.GET_CODE() == "")
                            //      throw new Exception(TEXT.text_ERR_CLIENT_GLN_INVALID);

                            LOG_JOURNAL(pPLUGIN, srvDesc + ": " + pCl.GET_CODE() + ": " + pCl.GET_DESC() + ": " + (pDocNo ?? ""));



                            var itemsObj = new DeaktivasyonBildirimType()
                            {
                                BELGE = new DeaktivasyonBildirimTypeBELGE()
                                {
                                    DD = pDocDate,
                                    DN = pDocNo

                                },
                                DT = "D",
                                /*
                                 Üretim M
İthalat I
Satış & Mal Devir S
Deaktivasyon & Sarf D
İhracat X
Mal İadesi F
Mal Alım A
Satış & Mal Devir İptal C
                                 */
                                FR = ITS_WH_CODE,
                                DS = "10"
                                /*
                                 KOD DEAKTİVASYON SEBEBİ
10 “SİSTEMDEN ÇIKARMA”
20 “ÜRETİM FİRELERİ”
30 “GERİ ÇEKME SEBEBİYLE İMHA”
40 “MİAT SEBEBİYLE İMHA”
50 “REVİZYON”
                                 
                                 */
                            };


                            var itemsList = new List<DeaktivasyonBildirimTypeURUN>();
                            foreach (var b in pArr)
                            {

                                var x = new DeaktivasyonBildirimTypeURUN()
                                {
                                    GTIN = b.GTIN,
                                    BN = b.BN,
                                    SN = b.SN,
                                    XD = b.XD_DATE
                                };

                                itemsList.Add(x);
                            }

                            //Optional
                            //itemsObj.BELGE = new DeaktivasyonBildirimTypeBELGE()
                            //{
                            //    DD = new DateTime?(pDate),
                            //    DN = pSlipNo
                            //};
                            itemsObj.URUNLER = itemsList.ToArray();





                            //





                            return DeaktivasyonBildir(itemsObj);
                        }


                        public string srvDesc
                        {
                            get { return "Deaktivasyon Servis"; }
                        }

                        public int[] successfulOperCodes
                        {
                            get
                            {
                                return new int[] { 
                               
                            };
                            }
                        }

                    }


                    [XmlType(Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/Deaktivasyon")]
                    [Serializable]
                    public class DeaktivasyonBildirimType
                    {


                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
                        public DeaktivasyonBildirimTypeBELGE BELGE;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string DS;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string DT;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string FR;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string ISACIKLAMA;

                        [XmlArray(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        [XmlArrayItem("URUN", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
                        public DeaktivasyonBildirimTypeURUN[] URUNLER;


                    }

                    [XmlType(AnonymousType = true, Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/Deaktivasyon")]
                    [Serializable]
                    public class DeaktivasyonBildirimTypeURUN
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string BN;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string GTIN;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
                        public DateTime XD;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string SN;


                    }


                    [XmlType(AnonymousType = true, Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/Deaktivasyon")]
                    [Serializable]
                    public class DeaktivasyonBildirimTypeBELGE
                    {


                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date", IsNullable = true)]
                        public DateTime? DD;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
                        public string DN;


                    }


                    [XmlType(AnonymousType = true, Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/Deaktivasyon")]
                    [Serializable]
                    public class DeaktivasyonBildirimCevapTypeURUNDURUM
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string GTIN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string SN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string UC;

                    }


                    [XmlType(Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/Deaktivasyon")]
                    [Serializable]
                    public class DeaktivasyonBildirimCevapType
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string BILDIRIMID;
                        [XmlArray(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        [XmlArrayItem("URUNDURUM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
                        public DeaktivasyonBildirimCevapTypeURUNDURUM[] URUNLER;

                    }


                }


                public class WebServiceIhracat
                {


                    [System.Web.Services.WebServiceBinding(Name = "IhracatBildirimReceiverBinding", Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/Ihracat")]
                    public class IhracatReceiverService : System.Web.Services.Protocols.SoapHttpClientProtocol, SrvExt
                    {




                        public IhracatReceiverService()
                        {
                            this.Url = URL_IhracatReceiverService;
                            __INIT_SRV(this);
                        }


                        [System.Web.Services.Protocols.SoapDocumentMethod("http://its.iegm.gov.tr/bildirim/BildirimReceiver/v1/Ihracat/IhracatBildirRequest", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
                        [return: XmlElement("IhracatBildirimCevap", Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/Ihracat")]
                        public IhracatBildirimCevapType IhracatBildir([XmlElement(Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/Ihracat")] IhracatBildirimType IhracatBildirim)
                        {
                            object[] ıhracatBildirim = new object[] { IhracatBildirim };
                            return (IhracatBildirimCevapType)base.Invoke("IhracatBildir", ıhracatBildirim)[0];
                        }




                        public IhracatBildirimCevapType _Ihracat(_PLUGIN pPLUGIN, ITS_CLIENT pCl, ITS_BARCODE[] pArr, string pDocNo, string pExportDesc, DateTime pDocDate)
                        {

                            //  if (!pCl.VALID || pCl.GET_CODE() == "")
                            //      throw new Exception(TEXT.text_ERR_CLIENT_GLN_INVALID);

                            LOG_JOURNAL(pPLUGIN, srvDesc + ": " + pCl.GET_CODE() + ": " + pCl.GET_DESC() + ": " + (pDocNo ?? ""));



                            var itemsObj = new IhracatBildirimType()
                            {

                                BELGE = new IhracatBildirimTypeBELGE()
                                 {
                                     DD = pDocDate,
                                     DN = pDocNo
                                 },
                                DT = "X",
                                /*
                                 Üretim M
İthalat I
Satış & Mal Devir S
Deaktivasyon & Sarf D
İhracat X
Mal İadesi F
Mal Alım A
Satış & Mal Devir İptal C
                                 */
                                FR = ITS_WH_CODE,
                                //<RT>Yunanistan ABC Şirketine İhraç DTPGTIP34223</RT>
                                RT = pExportDesc //export contry custom code //632
                                /*
                                 KOD DEAKTİVASYON SEBEBİ
10 “SİSTEMDEN ÇIKARMA”
20 “ÜRETİM FİRELERİ”
30 “GERİ ÇEKME SEBEBİYLE İMHA”
40 “MİAT SEBEBİYLE İMHA”
50 “REVİZYON”
                                 
                                 */
                            };


                            var itemsList = new List<IhracatBildirimTypeURUN>();
                            foreach (var b in pArr)
                            {

                                var x = new IhracatBildirimTypeURUN()
                                {
                                    GTIN = b.GTIN,
                                    BN = b.BN,
                                    SN = b.SN,
                                    XD = b.XD_DATE
                                };

                                itemsList.Add(x);
                            }

                            //Optional
                            //itemsObj.BELGE = new IhracatBildirimTypeBELGE()
                            //{
                            //    DD = new DateTime?(pDate),
                            //    DN = pSlipNo
                            //};
                            itemsObj.URUNLER = itemsList.ToArray();


                            return IhracatBildir(itemsObj);
                        }


                        public string srvDesc
                        {
                            get { return "Ihracat Servis"; }
                        }

                        public int[] successfulOperCodes
                        {
                            get
                            {
                                return new int[] { 
                               10207
                            };
                            }
                        }


                    }

                    [XmlType(AnonymousType = true, Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/Ihracat")]
                    [Serializable]
                    public class IhracatBildirimTypeURUN
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string BN;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string GTIN;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
                        public DateTime XD;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string SN;


                    }


                    [XmlType(AnonymousType = true, Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/Ihracat")]
                    [Serializable]
                    public class IhracatBildirimTypeBELGE
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
                        public DateTime DD;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
                        public string DN;



                    }
                    [XmlType(Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/Ihracat")]
                    [Serializable]
                    public class IhracatBildirimType
                    {


                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
                        public IhracatBildirimTypeBELGE BELGE;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string DT;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string FR;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string RT;

                        [XmlArray(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        [XmlArrayItem("URUN", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
                        public IhracatBildirimTypeURUN[] URUNLER;

                    }

                    [XmlType(AnonymousType = true, Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/Ihracat")]
                    [Serializable]
                    public class IhracatBildirimCevapTypeURUNDURUM
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string GTIN;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string SN;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string UC;


                    }


                    [XmlType(Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/Ihracat")]
                    [Serializable]
                    public class IhracatBildirimCevapType
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string BILDIRIMID;
                        [XmlArray(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        [XmlArrayItem("URUNDURUM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
                        public IhracatBildirimCevapTypeURUNDURUM[] URUNLER;


                    }


                }
                public class WebServiceProduction
                {

                    [System.Web.Services.WebServiceBinding(Name = "UretimBildirimReceiverBinding", Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/Uretim")]
                    public class UretimBildirimReceiverService : System.Web.Services.Protocols.SoapHttpClientProtocol, SrvExt
                    {

                        public UretimBildirimReceiverService()
                        {
                            this.Url = URL_UretimBildirimReceiverService;
                            __INIT_SRV(this);
                        }

                        [System.Web.Services.Protocols.SoapDocumentMethod("http://its.iegm.gov.tr/bildirim/BildirimReceiver/v1/UretimBildirim/UretimBildirRequest", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
                        [return: XmlElement("UretimResponse", Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/Uretim")]
                        public UretimBildirimCevapType UretimBildir([XmlElement(Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/Uretim")] UretimBildirimType Uretim)
                        {
                            object[] uretim = new object[] { Uretim };
                            return (UretimBildirimCevapType)base.Invoke("UretimBildir", uretim)[0];
                        }


                        public UretimBildirimCevapType _UretimBildir(_PLUGIN pPLUGIN, ITS_CLIENT pCl, ITS_SERIALS pSerials, string pDocNo, DateTime pDocDate)
                        {



                            LOG_JOURNAL(pPLUGIN, srvDesc + ": " + pCl.GET_CODE() + ": " + pCl.GET_DESC() + ": " + (pDocNo ?? ""));




                            var itemsObj = new UretimBildirimType()
                            {
                                DT = "M",
                                /*
                                 Üretim M
                                 */
                                MI = ITS_WH_CODE,
                                PT = UretimBildirimTypePT.PP,

                                BELGE = new UretimBildirimTypeBELGE()
                                {
                                    DD = pDocDate,
                                    DN = pDocNo
                                },


                                MD = pDocDate,
                                GTIN = pSerials.ROOT.GTIN,
                                XD = pSerials.ROOT.XD_DATE,
                                BN = pSerials.ROOT.BN,

                                URUNLER = pSerials.SERIALS

                            };


                            return UretimBildir(itemsObj);
                        }


                        public string srvDesc
                        {
                            get { return "Ihracat Servis"; }
                        }

                        public int[] successfulOperCodes
                        {
                            get
                            {
                                return new int[] { 
                                
                            };
                            }
                        }


                    }


                    [XmlType(AnonymousType = true, Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/Uretim")]
                    [Serializable]
                    public enum UretimBildirimTypePT
                    {
                        PP, //ilac
                        BP, //beslenme
                        FP //ara urun
                    }




                    [XmlType(AnonymousType = true, Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/Uretim")]
                    [Serializable]
                    public class UretimBildirimTypeBELGE
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
                        public DateTime DD;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string DN;


                    }

                    [XmlType(Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/Uretim")]
                    [Serializable]
                    public class UretimBildirimType
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
                        public UretimBildirimTypeBELGE BELGE;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string BN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string DT;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string GTIN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
                        public DateTime XD;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
                        public DateTime MD;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string MI;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public UretimBildirimTypePT PT;
                        [XmlArray(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        [XmlArrayItem("SN", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
                        public string[] URUNLER;


                    }


                    [XmlType(AnonymousType = true, Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/Uretim")]
                    [Serializable]
                    public class UretimBildirimCevapTypeSNDURUM
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string SN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string UC;


                    }


                    [XmlType(Namespace = "http://its.iegm.gov.tr/bildirim/BR/v1/Uretim")]
                    [Serializable]
                    public class UretimBildirimCevapType
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string BILDIRIMID;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string BN;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string GTIN;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
                        public DateTime XD;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
                        public DateTime MD;
                        [XmlArray(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        [XmlArrayItem("SNDURUM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
                        public UretimBildirimCevapTypeSNDURUM[] URUNLER;

                    }



                }

                public class WebServicePacketAl
                {
                    [System.Web.Services.WebServiceBinding(Name = "PackageReceiverWSPortBinding", Namespace = "http://its.iegm.gov.tr/pts/receivepackage")]
                    public class PackageReceiverWebService : System.Web.Services.Protocols.SoapHttpClientProtocol
                    {

                        public PackageReceiverWebService()
                        {
                            this.Url = URL_PackageReceiverWebService;

                            __INIT_SRV(this);
                        }

                        [System.Web.Services.Protocols.SoapDocumentMethod("", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
                        [return: XmlElement("responseFileStream", Namespace = "http://its.iegm.gov.tr/pts/receivepackage", DataType = "base64Binary")]
                        public byte[] receiveFileStream([XmlElement(Namespace = "http://its.iegm.gov.tr/pts/receivepackage")] receiveFileParametersType receiveFileStreamParameters)
                        {
                            object[] objArray = new object[] { receiveFileStreamParameters };
                            return (byte[])base.Invoke("receiveFileStream", objArray)[0];
                        }


                        public string _receiveFileStream(_PLUGIN pPLUGIN, long pTransferId)
                        {

                            LOG_JOURNAL(pPLUGIN, srvDesc + ": " + pTransferId);


                            var itemsObj = new receiveFileParametersType()
                            {
                                sourceGLN = ITS_WH_CODE,
                                transferId = pTransferId

                            };


                            var data = this.receiveFileStream(itemsObj);//on error service throw exception


                            if (data != null)
                            {
                                //data
                                return ImplService.unzipPts(data);


                            }

                            return null;
                        }


                        public string srvDesc
                        {
                            get { return "PTS Paket Al Servis"; }
                        }

                        public int[] successfulOperCodes
                        {
                            get
                            {
                                return new int[] { 
                              
                            };
                            }
                        }



                    }

                    [XmlType(Namespace = "http://its.iegm.gov.tr/pts/receivepackage")]
                    [Serializable]
                    public class receiveFileParametersType
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string sourceGLN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public long transferId;
                    }



                }

                public class WebServicePacketGonder
                {

                    [System.Web.Services.WebServiceBinding(Name = "PackageSenderWSPortBinding", Namespace = "http://its.iegm.gov.tr/pts/sendpackage")]
                    public class PackageSenderWebService : System.Web.Services.Protocols.SoapHttpClientProtocol
                    {


                        public PackageSenderWebService()
                        {
                            this.Url = URL_PackageSenderWebService;

                            __INIT_SRV(this);
                        }






                        [System.Web.Services.Protocols.SoapDocumentMethod("", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
                        [return: XmlElement("sendFileResponse", Namespace = "http://its.iegm.gov.tr/pts/sendpackage")]
                        public sendFileResponseType sendFileStream([XmlElement(Namespace = "http://its.iegm.gov.tr/pts/sendpackage")] sendFileStreamParametersType sendFileStreamParameters)
                        {
                            object[] objArray = new object[] { sendFileStreamParameters };
                            return (sendFileResponseType)base.Invoke("sendFileStream", objArray)[0];
                        }





                        public sendFileResponseType _sendFileStream(_PLUGIN pPLUGIN, string pFileName, string pDestinationGLN)
                        {

                            if (!System.IO.File.Exists(pFileName))
                                throw new Exception("File not exists");

                            LOG_JOURNAL(pPLUGIN, srvDesc + ": " + pFileName);


                            var itemsObj = new sendFileStreamParametersType()
                            {
                                fileStreamElement = System.IO.File.ReadAllBytes(pFileName),
                                sendFileParameters = new sendFileParametersType()
                            {
                                sourceGLN = ITS_WH_CODE,
                                destinationGLN = pDestinationGLN //destination
                            }
                            };


                            var response = this.sendFileStream(itemsObj);//on error service throw exception
                            if (response != null)
                            {
                                //  response.transferId
                                //ok
                            }

                            return response;

                        }


                        public string srvDesc
                        {
                            get { return "PTS Giden Paket Servis"; }
                        }

                        public int[] successfulOperCodes
                        {
                            get
                            {
                                return new int[] { 
                              
                            };
                            }
                        }

                    }

                    [XmlType(Namespace = "http://its.iegm.gov.tr/pts/sendpackage")]
                    [Serializable]
                    public class sendFileStreamParametersType
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "base64Binary")]
                        public byte[] fileStreamElement;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public sendFileParametersType sendFileParameters;


                    }

                    [XmlType(Namespace = "http://its.iegm.gov.tr/pts/sendpackage")]
                    [Serializable]
                    public class sendFileResponseType
                    {
                        private long transferIdField;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public long transferId;


                    }

                    [XmlType(Namespace = "http://its.iegm.gov.tr/pts/sendpackage")]
                    [Serializable]
                    public class sendFileParametersType
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string destinationGLN;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string sourceGLN;

                    }
                }

                public class WebServicePacketSearch
                {
                    [System.Web.Services.WebServiceBinding(Name = "PackageTransferHelperWSPortBinding", Namespace = "http://its.iegm.gov.tr/pts/helper/receiveTrasnferDetails")]
                    public class PackageTransferHelperWebService : System.Web.Services.Protocols.SoapHttpClientProtocol, SrvExt
                    {



                        public PackageTransferHelperWebService()
                        {
                            this.Url = URL_PackageTransferHelperWebService;
                            __INIT_SRV(this);

                        }





                        [System.Web.Services.Protocols.SoapDocumentMethod("", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
                        [return: XmlElement("receiveTransferDetailsResponse", Namespace = "http://its.iegm.gov.tr/pts/helper/receiveTrasnferDetails")]
                        public receiveTransferDetailsResponseType receiveTransferDetails([XmlElement(Namespace = "http://its.iegm.gov.tr/pts/helper/receiveTrasnferDetails")] receiveTransferDetailsParametersType receiveTransferDetailsParameters)
                        {
                            object[] objArray = new object[] { receiveTransferDetailsParameters };
                            return (receiveTransferDetailsResponseType)base.Invoke("receiveTransferDetails", objArray)[0];
                        }



                        public receiveTransferDetailsResponseTypeTransferDetailsTransferDetail[] _receiveTransferDetails(_PLUGIN pPLUGIN, string pSourceGLN, DateTime pDf, DateTime pDt, bool pUnhandledPacks)
                        {
                            pDf = pDf.Date;
                            pDt = pDt.Date.AddDays(+1);//!!!

                            if (pDf > pDt)
                                throw new Exception(TEXT.text_ERR_DATE_RANGE);

                            LOG_JOURNAL(pPLUGIN, srvDesc + ": " + string.Format("GLN {0} Tarihden {1} Tarihe {2}", pSourceGLN, pDf, pDt));

                            var itemsObj = new receiveTransferDetailsParametersType()
                            {
                                destinationGLN = ITS_WH_CODE,
                                startDate = pDf,
                                endDate = pDt,//TODO ? Why +1
                                sourceGLN = string.IsNullOrEmpty(pSourceGLN) ? null : pSourceGLN,
                                bringNotReceivedTransferInfo = pUnhandledPacks //false for all
                            };

                            var response = this.receiveTransferDetails(itemsObj);

                            return response.transferDetails.transferDetail;


                        }


                        public string srvDesc
                        {
                            get { return "PTS Giden Paket Servis"; }
                        }

                        public int[] successfulOperCodes
                        {
                            get
                            {
                                return new int[] { 
                              
                            };
                            }
                        }

                    }


                    [XmlType(AnonymousType = true, Namespace = "http://its.iegm.gov.tr/pts/helper/receiveTrasnferDetails")]
                    [Serializable]
                    public class receiveTransferDetailsResponseTypeTransferDetailsTransferDetail
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string destinationGLN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string sourceGLN;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public DateTime transferDate;
                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public long transferId;

                    }

                    [XmlType(AnonymousType = true, Namespace = "http://its.iegm.gov.tr/pts/helper/receiveTrasnferDetails")]
                    [Serializable]
                    public class receiveTransferDetailsResponseTypeTransferDetails
                    {

                        [XmlElement("transferDetail", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public receiveTransferDetailsResponseTypeTransferDetailsTransferDetail[] transferDetail;


                    }
                    [XmlType(Namespace = "http://its.iegm.gov.tr/pts/helper/receiveTrasnferDetails")]
                    [Serializable]
                    public class receiveTransferDetailsResponseType
                    {

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public receiveTransferDetailsResponseTypeTransferDetails transferDetails;


                    }

                    [XmlType(Namespace = "http://its.iegm.gov.tr/pts/helper/receiveTrasnferDetails")]
                    [Serializable]
                    public class receiveTransferDetailsParametersType
                    {

                        public receiveTransferDetailsParametersType()
                        {
                            this.bringNotReceivedTransferInfo = false;
                        }


                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public bool bringNotReceivedTransferInfo;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string destinationGLN;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
                        public DateTime endDate;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                        public string sourceGLN;

                        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
                        public DateTime startDate;


                    }





                }
            }
            public class ITS_STOCKDOC
            {
                public DataRow DOC_REC;
                public ITS_CLIENT CLIENT;
                public ITS_BARCODE[] SERIALS_0;
                public ITS_BARCODE[] SERIALS_1;
                public readonly bool VALID = false;
                public ITS_STOCKDOC(_PLUGIN PLUGIN, object pLRef, bool pIsInvoice)
                {

                    var slipRef = pIsInvoice ? PLUGIN.SQLSCALAR("SELECT LOGICALREF FROM LG_$FIRM$_$PERIOD$_STFICHE WHERE INVOICEREF = @P1", new object[] { pLRef }) : pLRef;
                    var docTab = (pIsInvoice ? "INVOICE" : "STFICHE");
                    this.DOC_REC = TAB_GETLASTROW(PLUGIN.SQL(string.Format("SELECT * FROM LG_$FIRM$_$PERIOD$_{0} WHERE LOGICALREF = @P1", docTab), new object[] { pLRef }));

                    this.VALID = (DOC_REC != null);

                    if (VALID)
                    {
                        this.DOC_REC.Table.TableName = docTab;

                        this.CLIENT = new ITS_CLIENT(PLUGIN, _GET("CLIENTREF", 0));

                        this.SERIALS_0 = ImplService.getTrackNoForITS(slipRef, PLUGIN, 0);
                        this.SERIALS_1 = ImplService.getTrackNoForITS(slipRef, PLUGIN, 1);
                    }
                }

                public object _GET(string pCol, object pDef)
                {

                    if (!VALID)
                        return pDef;

                    return ISNULL(TAB_GETROW(DOC_REC, pCol), pDef);

                }

                public string GET_STR(string pCol, object pDef)
                {
                    return CASTASSTRING(_GET(pCol, pDef));
                }


                public short GET_TRCODE()
                {
                    return CASTASSHORT(_GET("TRCODE", 0));
                }
                public string GET_FICHENO()
                {
                    return GET_STR("FICHENO", "");
                }

            }
            public class ITS_CLIENT
            {
                public DataRow CL_REC = null;
                public bool VALID = false;
                public ITS_CLIENT(_PLUGIN PLUGIN, object pLRef)
                {
                    if (!ISEMPTYLREF(pLRef))
                        CL_REC = TAB_GETLASTROW(PLUGIN.SQL("SELECT * FROM LG_$FIRM$_CLCARD WHERE LOGICALREF = @P1", new object[] { pLRef }));
                    VALID = (CL_REC != null);
                }

                public object _GET(string pCol, object pDef)
                {

                    if (!VALID)
                        return pDef;

                    return ISNULL(TAB_GETROW(CL_REC, pCol), pDef);

                }

                public object GET_STR(string pCol, object pDef)
                {
                    return CASTASSTRING(_GET(pCol, pDef));
                }
                public string GET_CODE()
                {
                    return CASTASSTRING(_GET("CODE", ""));
                }

                public string GET_DESC()
                {
                    return CASTASSTRING(_GET("DEFINITION_", ""));
                }
            }
            public class ITS_BARCODE
            {
                public static ITS_BARCODE parse(string barcode)
                {
                    return new ITS_BARCODE(barcode);
                }

                public static bool ISSAME(ITS_BARCODE x, ITS_BARCODE y)
                {
                    return x.GTIN == y.GTIN && x.XD == y.XD && x.BN == y.BN;
                }
                public static bool ISSAMEPACK(ITS_BARCODE x, ITS_BARCODE y)
                {
                    return x.GROUP_ == y.GROUP_;
                }
                public bool LOAD_REC(_PLUGIN PLUGIN)
                {
                    if (MAT_REC == null)
                    {
                        MAT_REC = TAB_GETLASTROW(PLUGIN.SQL("SELECT * FROM LG_$FIRM$_ITEMS WHERE CODE in (@P1,@P2)", new object[] { GTIN, GTIN.TrimStart('0') }));

                        if (MAT_REC != null)
                            MAT_REC.Table.TableName = "ITEMS";
                    }


                    return (MAT_REC != null);
                }
                public DataRow MAT_REC;
                public ITS_BARCODE(string gtin, string sn, DateTime xd, string bn)
                    : this("01" + (gtin ?? "") + "21" + (sn ?? "") + "17" + (xd.ToString("yyMMdd")) + "10" + (bn ?? ""))
                {


                }

                public ITS_BARCODE(string gtin, string sn, DateTime xd, string bn, string pGroup)
                    : this(gtin, sn, xd, bn)
                {
                    GROUP_ = pGroup ?? "";
                }
                public ITS_BARCODE(string barcode)
                {

                    if (barcode == null)
                        barcode = "";

                    barcode = barcode.
                    Trim().
                    Replace(" ", "").
                    Replace("\n", "").
                    Replace("\t", "").
                    Trim('+');

                    {
                        var sb = new StringBuilder();

                        foreach (var c in barcode.ToCharArray())
                        {
                            if (char.IsLetterOrDigit(c) || c == '_')
                                sb.Append(c);

                        }

                        barcode = sb.ToString();
                    }

                    const int maxLen = 20 * 4;
                    try
                    {

                        if (barcode == "" || barcode.Length > (maxLen))
                            throw new Exception();

                        //(01)(21)(17)(10)
                        RAW = barcode;


                        this.GTIN = barcode.Substring(2, 14);
                        string date = "";
                        int lastIndx = (2 + 14);

                        int dateIndx = -1;
                        int dateCount = 0;

                        var now = DateTime.Now;
                        while (true)
                        {
                            lastIndx = barcode.IndexOf("17", lastIndx + 2);
                            if (lastIndx < 0 || lastIndx > (barcode.Length - (2 + 2 + 2 + 2 + (2/*BN*/)))) //(10) DD MM YY (BN)
                                break;
                            //
                            string markerSN = barcode.Substring(lastIndx + 2 + 6, 2); //10
                            //
                            date = barcode.Substring(lastIndx + 2, 6);//160531
                            string year = "20" + date.Substring(0, 2);
                            string month = date.Substring(2, 2);
                            string day = date.Substring(4, 2);
                            //
                            if (

                                markerSN == "10" &&
                                Convert.ToInt32(year) < now.Year + 20 && //(max SKT)
                                Convert.ToInt32(year) >= 0 &&
                                Convert.ToInt32(day) <= 31 &&
                                Convert.ToInt32(day) >= 0 && //inc 00
                                Convert.ToInt32(month) <= 12 &&
                                Convert.ToInt32(month) >= 1

                                )
                            {
                                ++dateCount;



                                this.XD = date;
                                dateIndx = lastIndx;

                            }

                        }
                        //
                        if (dateCount == 0)
                            throw new Exception("No date");

                        if (dateCount > 1)
                            throw new Exception("Has two date");

                        this.SN = barcode.Substring(2 + 14 + 2, dateIndx - (2 + 14 + 2));
                        //
                        this.BN = barcode.Substring(dateIndx + 2 + 6 + 2);



                    }
                    catch (Exception exc)
                    {
                        throw new Exception("Cant parse 2D Barcode: " + barcode, exc);
                    }


                }




                public string GTIN = ""; //01 barcode
                public string SN = ""; //21 serial 
                public string XD = ""; //17 shelf life
                public string BN = ""; //10 batch


                public string GROUP_ = ""; //label of Box

                public string RAW = "";
                public DateTime XD_DATE
                {
                    get
                    {
                        try
                        {
                            var y = int.Parse("20" + XD.Substring(0, 2));
                            var m = int.Parse(XD.Substring(2, 2));
                            var d = int.Parse(XD.Substring(4, 2));
                            var res = new DateTime(
                               y,
                               m,
                               d == 0 ? 1 : d
                                );

                            if (d == 0) //correct to last day of month
                                res = res.AddMonths(1).AddDays(-1);

                            return res;
                        }
                        catch
                        {
                            return new DateTime(1900, 1, 1);
                        }
                    }
                }

                public override string ToString()
                {
                    return RAW;
                }

            }

            public class ITS_SERIALS
            {

                public static ITS_SERIALS[] create(ITS_BARCODE[] pArr)
                {
                    var res = new List<ITS_SERIALS>();

                    var tmp = new List<ITS_BARCODE>(pArr);

                    while (tmp.Count > 0)
                    {

                        var _root = tmp.Count > 0 ? new ITS_BARCODE(tmp[0].RAW) { GROUP_ = tmp[0].GROUP_ } : null;
                        var _ser = new List<string>();

                        for (int i = 0; i < tmp.Count; ++i)
                        {
                            var z = tmp[i];
                            if (ITS_BARCODE.ISSAME(z, _root) && ITS_BARCODE.ISSAMEPACK(z, _root))
                            {
                                _ser.Add(z.SN);

                                tmp.RemoveAt(i);

                                --i;

                            }
                        }

                        if (_root != null)
                        {
                            var itm = new ITS_SERIALS();
                            itm.ROOT = _root;
                            itm.SERIALS = _ser.ToArray();
                            res.Add(itm);
                        }

                    }



                    return res.ToArray();

                }

                public ITS_BARCODE ROOT;
                public string[] SERIALS;
            }

            public class ITS_FORMS
            {
                public class FormITSSerialNo : Form
                {

                    _PLUGIN PLUGIN;
                    //
                    object docRef;
                    bool isInvoice = true; //or slip
                    object slipRef;
                    DataTable dataTrackNoHeader;
                    DataTable dataTrackNo;

                    Dictionary<object, double> amountsInDoc = new Dictionary<object, double>();
                    //
                    string formExtDesc = "";
                    //
                    bool readOnly = false;

                    bool adpMode = false;

                    public FormITSSerialNo()
                    {
                        InitializeComponent();
                        //
                        cGridTop.AutoGenerateColumns = cGrid.AutoGenerateColumns = false;
                        cGridTop.SelectionMode = cGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;


                        ActiveControl = cBarcode;
                        cBarcode.Focus();

                        Icon = CTRL_FORM_ICON();
                        Text = TEXT.text_ITS;
                        //

                    }


                    public FormITSSerialNo(_PLUGIN pPLUGIN, object pDocRef, bool pIsInv)
                        : this()
                    {
                        PLUGIN = pPLUGIN;

                        docRef = pDocRef;


                        isInvoice = pIsInv;

                        if (pIsInv)
                        {
                            readOnly = !string.IsNullOrEmpty(CASTASSTRING(
                                  pPLUGIN.SQLSCALAR(
                                  "select DOCTRACKINGNR from LG_$FIRM$_$PERIOD$_INVOICE where LOGICALREF = @P1",
                                  new object[] { docRef })));
                        }
                    }

                    protected override void OnLoad(EventArgs e)
                    {
                        base.OnLoad(e);

                        try
                        {



                            _dbLoad();
                            //

                            initHeader();

                            //1
                            //buid header
                            foreach (var l in this.amountsInDoc.Keys)
                            {
                                var rec = trackRecordOperate(l, 0);
                                TAB_SETROW(rec, "AMOUNTINDOC", this.amountsInDoc[l]);
                            }

                            foreach (DataRow r in dataTrackNo.Rows)
                            {
                                //  converTrackTable(r);
                                trackRecordOperate(TAB_GETROW(r, "STOCKREF"), +1);
                            }
                            //load

                            dataTrackNo.DefaultView.Sort = "TRACKNO ASC";
                            cGrid.DataSource = dataTrackNo;

                            //2
                            dataTrackNoHeader.DefaultView.Sort = "ITEMS_NAME ASC";
                            cGridTop.DataSource = dataTrackNoHeader;
                            //3
                            TOOL_GRID.BIND_GRID_TO_GRID(cGridTop, "STOCKREF", cGrid, "STOCKREF");
                            //
                            cBarcode.ReadOnly = readOnly;
                            cBarcode.Enabled = !readOnly;

                            cGridTop.ReadOnly = cGrid.ReadOnly = true;

                            cBtnDel.Enabled = !readOnly;
                            cBtnDelAll.Enabled = !readOnly;
                            cBtnLoad.Enabled = !readOnly;
                            //
                            cBtnExport.ImageAlign = cBtnLoad.ImageAlign = cBtnClose.ImageAlign = ContentAlignment.MiddleLeft;


                            cBtnExport.Image = RES_IMAGE("export_16x16");
                            cBtnLoad.Image = RES_IMAGE("import_16x16");
                            cBtnClose.Image = RES_IMAGE("close_16x16");
                            //
                            cGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            cGrid.ColumnHeadersDefaultCellStyle.Font = new Font(cGrid.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);

                            cGridTop.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            cGridTop.ColumnHeadersDefaultCellStyle.Font = new Font(cGrid.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);

                            this.Text = this.Text + (" " + formExtDesc);
                        }
                        catch (Exception exc)
                        {
                            PLUGIN.LOG(exc);
                            PLUGIN.MSGUSERERROR(exc.Message);
                            this.Close();
                        }
                    }

                    private DataRow trackRecordOperate(object pStockRef, int pAmnt /*or delete*/)
                    {
                        var isAdd = pAmnt >= 0;

                        if (pAmnt > 1 || pAmnt < -1)
                            throw new Exception("Incorrect quantity");

                        var headerRec = TAB_SEARCH(dataTrackNoHeader, "STOCKREF", pStockRef);

                        if (isAdd)
                        {
                            if (headerRec == null)
                            {

                                var matDbRec = TAB_GETLASTROW(PLUGIN.SQL("SELECT NAME,CODE FROM LG_$FIRM$_ITEMS WHERE LOGICALREF = @P1", new object[] { pStockRef }));
                                //
                                var desc = matDbRec != null ? TAB_GETROW(matDbRec, "NAME") : "";
                                var code = matDbRec != null ? TAB_GETROW(matDbRec, "CODE") : "";
                                //
                                var x = dataTrackNoHeader.NewRow();
                                TAB_FILLNULL(x);
                                TAB_SETROW(x, "STOCKREF", pStockRef);
                                TAB_SETROW(x, "AMOUNT", 0.0);
                                TAB_SETROW(x, "ITEMS_NAME", desc);
                                TAB_SETROW(x, "GTIN", code);
                                dataTrackNoHeader.Rows.Add(headerRec = x);

                            }

                        }

                        if (headerRec != null)
                        {
                            var amt = CASTASDOUBLE(TAB_GETROW(headerRec, "AMOUNT"));

                            amt = Math.Max(amt + pAmnt, 0);

                            TAB_SETROW(headerRec, "AMOUNT", amt);

                            if (isAdd)
                                return headerRec;
                        }

                        return null;
                    }

                    private void initHeader()
                    {
                        dataTrackNoHeader = new DataTable();
                        TAB_ADDCOL(dataTrackNoHeader, "STOCKREF", typeof(int));
                        TAB_ADDCOL(dataTrackNoHeader, "ITEMS_NAME", typeof(string));
                        TAB_ADDCOL(dataTrackNoHeader, "GTIN", typeof(string));
                        TAB_ADDCOL(dataTrackNoHeader, "AMOUNT", typeof(double));
                        TAB_ADDCOL(dataTrackNoHeader, "AMOUNTINDOC", typeof(double));


                    }

                    private void _dbLoad()
                    {

                        if (ISEMPTYLREF(docRef))
                            return;

                        slipRef = isInvoice ? PLUGIN.SQLSCALAR("SELECT LOGICALREF FROM LG_$FIRM$_$PERIOD$_STFICHE WHERE INVOICEREF = @P1", new object[] { docRef }) : docRef;

                        {

                            var sqlTrackNo_ = (@"
select 
*
from LG_$FIRM$_$PERIOD$_ITSTRACK T WITH(NOLOCK) 
where T.STFICHEREF = @P1
");

                            dataTrackNo = PLUGIN.SQL(sqlTrackNo_, new object[] { slipRef });

                        }
                        {


                            var docLines = PLUGIN.SQL(@"
select L.STOCKREF,L.AMOUNT from 
LG_$FIRM$_$PERIOD$_STLINE L
where L.STFICHEREF = @P1 AND L.LINETYPE IN (0,1)
", new object[] { slipRef });


                            foreach (DataRow r in docLines.Rows)
                            {

                                var lref = TAB_GETROW(r, "STOCKREF");
                                var amt = CASTASDOUBLE(TAB_GETROW(r, "AMOUNT"));

                                if (!amountsInDoc.ContainsKey(lref))
                                    amountsInDoc[lref] = amt;
                                else
                                    amountsInDoc[lref] = amountsInDoc[lref] + amt;
                            }

                        }
                        {


                            var sql = isInvoice ?
(
MY_CHOOSE_SQL(
@"
SELECT 

'['+ISNULL((SELECT LEFT(DEFINITION_,15) FROM LG_$FIRM$_CLCARD WITH(NOLOCK) WHERE LOGICALREF = H.CLIENTREF),'') +']'+
'['+convert(varchar,DATE_,102 )+']'
 
FROM LG_$FIRM$_$PERIOD$_INVOICE H WITH(NOLOCK) WHERE LOGICALREF = @P1

",
 @"
SELECT 
'['||COALESCE((SELECT LEFT(DEFINITION_,15) FROM LG_$FIRM$_CLCARD WHERE LOGICALREF = H.CLIENTREF),'')||']'
||
'['||(left(DATE_::varchar,10))||']'
FROM LG_$FIRM$_$PERIOD$_INVOICE H WHERE LOGICALREF =  @P1

")
)
:
(
MY_CHOOSE_SQL(
@"

SELECT 

'['+convert(varchar,DATE_,102 )+']'
 
FROM LG_$FIRM$_$PERIOD$_STFICHE H WITH(NOLOCK) WHERE LOGICALREF = @P1

",
 @"

SELECT 
'['||(left(DATE_::varchar,10))||']'
FROM LG_$FIRM$_$PERIOD$_STFICHE H WHERE LOGICALREF = @P1

")
)
;

                            formExtDesc = CASTASSTRING(PLUGIN.SQLSCALAR(sql, new object[] { docRef }));



                        }


                    }


                    //void converTrackTable(DataRow pDataTrackNoRec)
                    //{
                    //    var t = TAB_GETROW(pDataTrackNoRec, "TRACKNO").ToString();
                    //    var b = new ITS_BARCODE(t);
                    //    TAB_SETROW(pDataTrackNoRec, "GTIN", b.GTIN);
                    //    TAB_SETROW(pDataTrackNoRec, "SN", b.SN);
                    //    TAB_SETROW(pDataTrackNoRec, "XD_DATE", b.XD_DATE);
                    //    TAB_SETROW(pDataTrackNoRec, "BN", b.BN);
                    //    // b.LOAD_REC(PLUGIN);
                    //    // TAB_SETROW(pDataTrackNoRec, "ITEMS_NAME", TAB_GETROW(b.MAT_REC, "NAME"));
                    //}


                    #region UI

                    void InitializeComponent()
                    {
                        this.cGrid = new System.Windows.Forms.DataGridView();
                        this.cGridTop = new System.Windows.Forms.DataGridView();
                        this.cGridTop_DESC = new System.Windows.Forms.DataGridViewTextBoxColumn();
                        this.cGridTop_AMOUNT = new System.Windows.Forms.DataGridViewTextBoxColumn();
                        this.cGridTop_AMOUNTINDOC = new System.Windows.Forms.DataGridViewTextBoxColumn();

                        this.cGridTop_GTIN = new System.Windows.Forms.DataGridViewTextBoxColumn();
                        this.cGrid_SN = new System.Windows.Forms.DataGridViewTextBoxColumn();
                        this.cGrid_TRACKNO = new System.Windows.Forms.DataGridViewTextBoxColumn();

                        this.cGrid_XD = new System.Windows.Forms.DataGridViewTextBoxColumn();
                        this.cGrid_BN = new System.Windows.Forms.DataGridViewTextBoxColumn();
                        this.cGrid_PACK = new System.Windows.Forms.DataGridViewTextBoxColumn();
                        this.cGrid_STATUS_ = new System.Windows.Forms.DataGridViewCheckBoxColumn();

                        this.cBarcode = new System.Windows.Forms.TextBox();
                        this.cPanelBtn = new System.Windows.Forms.Panel();
                        this.cBtnClose = new System.Windows.Forms.Button();
                        this.cBtnDel = new System.Windows.Forms.Button();
                        this.cBtnDelAll = new System.Windows.Forms.Button();
                        this.cBtnLoad = new System.Windows.Forms.Button();
                        this.cBtnExport = new System.Windows.Forms.Button();


                        this.cLabelBarcode = new System.Windows.Forms.Label();
                        ((System.ComponentModel.ISupportInitialize)(this.cGrid)).BeginInit();
                        ((System.ComponentModel.ISupportInitialize)(this.cGridTop)).BeginInit();
                        this.cPanelBtn.SuspendLayout();
                        this.SuspendLayout();
                        // 
                        // cGridTop
                        // 
                        this.cGridTop.AllowUserToAddRows = false;
                        this.cGridTop.AllowUserToDeleteRows = false;
                        this.cGridTop.AllowUserToResizeColumns = false;
                        this.cGridTop.AllowUserToResizeRows = false;
                        this.cGridTop.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
                        this.cGridTop.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
                        this.cGridTop.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                        this.cGridTop.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cGridTop_DESC,
            this.cGridTop_AMOUNT,
            this.cGridTop_AMOUNTINDOC,
            this.cGridTop_GTIN
                        });
                        this.cGridTop.Location = new System.Drawing.Point(12, 38);
                        this.cGridTop.MultiSelect = false;
                        this.cGridTop.Name = "cGridTop";
                        this.cGridTop.ReadOnly = true;
                        this.cGridTop.RowHeadersWidth = 60;
                        this.cGridTop.Size = new System.Drawing.Size(768, 138);
                        this.cGridTop.TabIndex = 0;
                        this.cGridTop.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.cGrid_RowPostPaint);

                        //     
                        // cGridTop_GTIN
                        // 
                        this.cGridTop_GTIN.HeaderText = "Barkod Numarası";
                        this.cGridTop_GTIN.Name = "cGridTop_GTIN";
                        this.cGridTop_GTIN.ReadOnly = true;
                        this.cGridTop_GTIN.Width = 140;
                        this.cGridTop_GTIN.DataPropertyName = "GTIN";
                        this.cGridTop_GTIN.SortMode = DataGridViewColumnSortMode.Programmatic;

                        // 
                        // cGridTop_DESC
                        // 
                        this.cGridTop_DESC.HeaderText = "İlaç";
                        this.cGridTop_DESC.Name = "cGridTop_DESC";
                        this.cGridTop_DESC.ReadOnly = true;
                        this.cGridTop_DESC.Width = 240;
                        this.cGridTop_DESC.DataPropertyName = "ITEMS_NAME";
                        this.cGridTop_DESC.SortMode = DataGridViewColumnSortMode.Programmatic;
                        this.cGridTop_DESC.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        // 
                        // cGridTop_AMOUNT
                        // 
                        this.cGridTop_AMOUNT.HeaderText = "Miktar";
                        this.cGridTop_AMOUNT.Name = "cGridTop_AMOUNT";
                        this.cGridTop_AMOUNT.ReadOnly = true;
                        this.cGridTop_AMOUNT.Width = 100;
                        this.cGridTop_AMOUNT.DataPropertyName = "AMOUNT";
                        this.cGridTop_AMOUNT.SortMode = DataGridViewColumnSortMode.Programmatic;
                        this.cGridTop_AMOUNT.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.cGridTop_AMOUNT.DefaultCellStyle.Format = "# ##0.##";
                        // 
                        // cGridTop_AMOUNTINDOC
                        // 
                        this.cGridTop_AMOUNTINDOC.HeaderText = "Fiş Miktar";
                        this.cGridTop_AMOUNTINDOC.Name = "cGridTop_AMOUNTINDOC";
                        this.cGridTop_AMOUNTINDOC.ReadOnly = true;
                        this.cGridTop_AMOUNTINDOC.Width = 100;
                        this.cGridTop_AMOUNTINDOC.DataPropertyName = "AMOUNTINDOC";
                        this.cGridTop_AMOUNTINDOC.SortMode = DataGridViewColumnSortMode.Programmatic;
                        this.cGridTop_AMOUNTINDOC.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.cGridTop_AMOUNTINDOC.DefaultCellStyle.Format = "# ##0.##";



                        // 
                        // cGrid
                        // 
                        this.cGrid.AllowUserToAddRows = false;
                        this.cGrid.AllowUserToDeleteRows = false;
                        this.cGrid.AllowUserToResizeColumns = false;
                        this.cGrid.AllowUserToResizeRows = false;
                        this.cGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
                        this.cGrid.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
                        this.cGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                        this.cGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            
        
            this.cGrid_SN,
            this.cGrid_XD,
            this.cGrid_BN,
            this.cGrid_STATUS_,
            this.cGrid_PACK,
            this.cGrid_TRACKNO

                        });
                        this.cGrid.Location = new System.Drawing.Point(12, 38 + 138 + 1);
                        this.cGrid.MultiSelect = false;
                        this.cGrid.Name = "cGrid";
                        this.cGrid.ReadOnly = true;
                        this.cGrid.RowHeadersWidth = 60;
                        this.cGrid.Size = new System.Drawing.Size(768, 138);
                        this.cGrid.TabIndex = 0;
                        this.cGrid.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.cGrid_RowPostPaint);
                        this.cGrid.CellDoubleClick += cGrid_CellDoubleClick;

                        // 
                        // cGrid_SN
                        // 
                        this.cGrid_SN.HeaderText = "Seri Numarası";
                        this.cGrid_SN.Name = "cGrid_SN";
                        this.cGrid_SN.ReadOnly = true;
                        this.cGrid_SN.Width = 140;
                        this.cGrid_SN.DataPropertyName = "SN";
                        this.cGrid_SN.SortMode = DataGridViewColumnSortMode.Programmatic;
                        // 
                        // cGrid_TRACKNO
                        // 
                        this.cGrid_TRACKNO.HeaderText = "Ürün Barkod";
                        this.cGrid_TRACKNO.Name = "cGrid_TRACKNO";
                        this.cGrid_TRACKNO.ReadOnly = true;
                        this.cGrid_TRACKNO.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        this.cGrid_TRACKNO.DataPropertyName = "TRACKNO";
                        this.cGrid_TRACKNO.SortMode = DataGridViewColumnSortMode.Programmatic;
                        // 
                        // cGrid_XD
                        // 
                        this.cGrid_XD.HeaderText = "S.K.T.";
                        this.cGrid_XD.Name = "cGrid_XD";
                        this.cGrid_XD.ReadOnly = true;
                        this.cGrid_XD.Width = 70;
                        this.cGrid_XD.DataPropertyName = "XD_DATE";
                        this.cGrid_XD.DefaultCellStyle.Format = "yyyy-MM-dd";
                        this.cGrid_XD.SortMode = DataGridViewColumnSortMode.Programmatic;
                        // 
                        // cGrid_BN
                        // 
                        this.cGrid_BN.HeaderText = "Parti Numarası";
                        this.cGrid_BN.Name = "cGrid_BN";
                        this.cGrid_BN.ReadOnly = true;
                        this.cGrid_BN.Width = 100;
                        this.cGrid_BN.DataPropertyName = "BN";
                        this.cGrid_BN.SortMode = DataGridViewColumnSortMode.Programmatic;
                        // 
                        // cGrid_PACK
                        // 
                        this.cGrid_PACK.HeaderText = "Koli";
                        this.cGrid_PACK.Name = "cGrid_PACK";
                        this.cGrid_PACK.ReadOnly = true;
                        this.cGrid_PACK.Width = 100;
                        this.cGrid_PACK.DataPropertyName = "GROUP_";
                        this.cGrid_PACK.SortMode = DataGridViewColumnSortMode.Programmatic;
                        this.cGrid_PACK.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;


                        // 
                        // cGrid_STATUS_
                        // 
                        this.cGrid_STATUS_.HeaderText = "İTS";
                        this.cGrid_STATUS_.Name = "cGrid_STATUS_";
                        this.cGrid_STATUS_.ReadOnly = true;
                        this.cGrid_STATUS_.Width = 30;
                        this.cGrid_STATUS_.DataPropertyName = "STATUS_";
                        this.cGrid_STATUS_.SortMode = DataGridViewColumnSortMode.Programmatic;
                        this.cGrid_STATUS_.TrueValue = 1;
                        this.cGrid_STATUS_.FalseValue = 0;
                        // 
                        // cBarcode
                        // 
                        this.cBarcode.Location = new System.Drawing.Point(79, 12);
                        this.cBarcode.Name = "cBarcode";
                        this.cBarcode.Size = new System.Drawing.Size(299, 20);
                        this.cBarcode.TabIndex = 2;
                        this.cBarcode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cBarcode_KeyDown);
                        // 
                        // cPanelBtn
                        // 
                        this.cPanelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
                        this.cPanelBtn.Controls.Add(this.cBtnClose);

                        this.cPanelBtn.Controls.Add(this.cBtnExport);
                        this.cPanelBtn.Controls.Add(this.cBtnLoad);
                        this.cPanelBtn.Controls.Add(this.cBtnDel);
                        this.cPanelBtn.Controls.Add(this.cBtnDelAll);

                        this.cPanelBtn.Location = new System.Drawing.Point(12, 321);
                        this.cPanelBtn.Name = "cPanelBtn";
                        this.cPanelBtn.Size = new System.Drawing.Size(768, 40);
                        this.cPanelBtn.TabIndex = 3;
                        // 
                        // cBtnClose
                        // 
                        this.cBtnClose.Dock = System.Windows.Forms.DockStyle.Right;
                        this.cBtnClose.Location = new System.Drawing.Point(693, 0);
                        this.cBtnClose.Name = "cBtnClose";
                        this.cBtnClose.Size = new System.Drawing.Size(75, 40);
                        this.cBtnClose.TabIndex = 1;
                        this.cBtnClose.Text = "Kapat";
                        this.cBtnClose.UseVisualStyleBackColor = true;
                        this.cBtnClose.Click += new System.EventHandler(this.cBtnClose_Click);
                        // 
                        // cBtnDel
                        // 
                        this.cBtnDel.Dock = System.Windows.Forms.DockStyle.Left;
                        this.cBtnDel.Location = new System.Drawing.Point(0, 0);
                        this.cBtnDel.Name = "cBtnDel";
                        this.cBtnDel.Size = new System.Drawing.Size(75, 40);
                        this.cBtnDel.TabIndex = 0;
                        this.cBtnDel.Text = "Sil";
                        this.cBtnDel.UseVisualStyleBackColor = true;
                        this.cBtnDel.Click += new System.EventHandler(this.cBtnDel_Click);
                        // 
                        // cBtnDelAll
                        // 
                        this.cBtnDelAll.Dock = System.Windows.Forms.DockStyle.Left;
                        this.cBtnDelAll.Location = new System.Drawing.Point(0, 0);
                        this.cBtnDelAll.Name = "cBtnDel";
                        this.cBtnDelAll.Size = new System.Drawing.Size(75, 40);
                        this.cBtnDelAll.TabIndex = 0;
                        this.cBtnDelAll.Text = "Sil (Mal.)";
                        this.cBtnDelAll.UseVisualStyleBackColor = true;
                        this.cBtnDelAll.Click += new System.EventHandler(this.cBtnDelAll_Click);
                        // 
                        // cBtnLoad
                        // 
                        this.cBtnLoad.Dock = System.Windows.Forms.DockStyle.Left;
                        this.cBtnLoad.Location = new System.Drawing.Point(0, 0);
                        this.cBtnLoad.Name = "cBtnLoad";
                        this.cBtnLoad.Size = new System.Drawing.Size(75, 40);
                        this.cBtnLoad.TabIndex = 0;
                        this.cBtnLoad.Text = "İçeri";
                        this.cBtnLoad.UseVisualStyleBackColor = true;
                        this.cBtnLoad.Click += new System.EventHandler(this.cBtnLoad_Click);
                        // 
                        // cBtnExport
                        // 
                        this.cBtnExport.Dock = System.Windows.Forms.DockStyle.Left;
                        this.cBtnExport.Location = new System.Drawing.Point(0, 0);
                        this.cBtnExport.Name = "cBtnExport";
                        this.cBtnExport.Size = new System.Drawing.Size(75, 40);
                        this.cBtnExport.TabIndex = 0;
                        this.cBtnExport.Text = "Dışarı";
                        this.cBtnExport.UseVisualStyleBackColor = true;
                        this.cBtnExport.Click += new System.EventHandler(this.cBtnExport_Click);


                        // 
                        // cLabelBarcode
                        // 
                        this.cLabelBarcode.AutoSize = true;
                        this.cLabelBarcode.Location = new System.Drawing.Point(12, 15);
                        this.cLabelBarcode.Name = "cLabelBarcode";
                        this.cLabelBarcode.Size = new System.Drawing.Size(47, 13);
                        this.cLabelBarcode.TabIndex = 4;
                        this.cLabelBarcode.Text = "Barkod";
                        // 
                        // FormITSSerialNo
                        // 
                        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                        this.ClientSize = new System.Drawing.Size(792, 373);
                        this.Controls.Add(this.cLabelBarcode);
                        this.Controls.Add(this.cPanelBtn);
                        this.Controls.Add(this.cBarcode);
                        this.Controls.Add(this.cGrid);
                        this.Controls.Add(this.cGridTop);
                        this.Name = "FormITSSerialNo";
                        this.Text = "FormITSSerialNo";
                        ((System.ComponentModel.ISupportInitialize)(this.cGrid)).EndInit();
                        ((System.ComponentModel.ISupportInitialize)(this.cGridTop)).EndInit();
                        this.cPanelBtn.ResumeLayout(false);
                        this.ResumeLayout(false);
                        this.PerformLayout();

                    }





                    private System.Windows.Forms.DataGridView cGrid;
                    private System.Windows.Forms.DataGridView cGridTop;

                    private System.Windows.Forms.TextBox cBarcode;
                    private System.Windows.Forms.Panel cPanelBtn;
                    private System.Windows.Forms.Button cBtnClose;
                    private System.Windows.Forms.Button cBtnDel;
                    private System.Windows.Forms.Button cBtnDelAll;
                    private System.Windows.Forms.Button cBtnLoad;
                    private System.Windows.Forms.Button cBtnExport;

                    private System.Windows.Forms.DataGridViewTextBoxColumn cGridTop_DESC;
                    private System.Windows.Forms.DataGridViewTextBoxColumn cGridTop_AMOUNT;
                    private System.Windows.Forms.DataGridViewTextBoxColumn cGridTop_AMOUNTINDOC;

                    private System.Windows.Forms.DataGridViewTextBoxColumn cGridTop_GTIN;
                    private System.Windows.Forms.DataGridViewTextBoxColumn cGrid_SN;
                    private System.Windows.Forms.DataGridViewTextBoxColumn cGrid_TRACKNO;

                    private System.Windows.Forms.DataGridViewTextBoxColumn cGrid_XD;
                    private Label cLabelBarcode;
                    private System.Windows.Forms.DataGridViewTextBoxColumn cGrid_BN;
                    private System.Windows.Forms.DataGridViewTextBoxColumn cGrid_PACK;
                    private System.Windows.Forms.DataGridViewCheckBoxColumn cGrid_STATUS_;
                    #endregion


                    #region CMD
                    void doClose()
                    {
                        try
                        {
                            doValidateData();
                            this.Close();
                        }
                        catch (Exception exc)
                        {
                            PLUGIN.LOG(exc);
                            PLUGIN.MSGUSERERROR(exc.Message);
                        }
                    }


                    void doValidateData()
                    {


                    }

                    void doExport()
                    {
                        //if (readOnly)
                        //    return;

                        try
                        {
                            var list = new List<string>();





                            list.AddRange(new string[] { "all", PLUGIN.LANG("T_ALL") });
                            list.AddRange(new string[] { "onhand", PLUGIN.LANG("T_ONHAND") });

                            var res_ = PLUGIN.REF("ref.gen.definedlist [obj::" + JOINLIST(list.ToArray()) + "] [desc::T_EXPORT] type::string");

                            string exportCode_ = (res_ != null && res_.Length > 0 ? CASTASSTRING(res_[0]["VALUE"]) : null);
                            if (exportCode_ == null)
                                return;

                            var onlyOnhand = false;

                            switch (exportCode_)
                            {
                                case "all":

                                    break;
                                case "onhand":
                                    onlyOnhand = true;
                                    break;



                            }

                            var dataExp = dataTrackNo.Copy();

                            dataExp.DefaultView.Sort = "GROUP_ ASC,TRACKNO ASC";
                            var serials = new List<DataRow>();
                            foreach (DataRowView rowView in dataExp.DefaultView)
                            {
                                DataRow row = rowView.Row;

                                if (onlyOnhand)
                                {
                                    var TRACKNO = CASTASSTRING(TAB_GETROW(row, "TRACKNO"));
                                    var STOCKREF = (TAB_GETROW(row, "STOCKREF"));


                                    var outDocRef = PLUGIN.SQLSCALAR(@"

select 
--$MS$--TOP 1
 STFICHEREF
from LG_$FIRM$_$PERIOD$_ITSTRACK T WITH(NOLOCK) where STFICHEREF>0 and STOCKREF = @P1 and TRACKNO = @P2
and exists(select 1 from LG_$FIRM$_$PERIOD$_STFICHE D where D.LOGICALREF = T.STFICHEREF AND D.TRCODE IN (8,7,6,11,12))
--$PG$--limit 1 

", new object[] { STOCKREF, TRACKNO });


                                    if (!ISEMPTYLREF(outDocRef))
                                        continue;
                                }


                                serials.Add(row);
                            }





                            string lastGroup_ = null;
                            string lastGtin_ = null;

                            MY_DIR.CLEAN_SN();


                            if (serials.Count == 0)
                                throw new Exception("T_MSG_ERROR_NO_DATA");

                            var sb = new StringBuilder();

                            var listAllData = new List<string>();

                            foreach (DataRow row in serials)
                            {


                                var GROUP_ = CASTASSTRING(TAB_GETROW(row, "GROUP_"));
                                var TRACKNO = CASTASSTRING(TAB_GETROW(row, "TRACKNO"));
                                var GTIN = TAB_GETROW(row, "GTIN").ToString();
                                var DESC_ = CASTASSTRING(
                                    PLUGIN.SQLSCALAR("select NAME from LG_$FIRM$_ITEMS where CODE = @P1", new object[] { GTIN, GTIN.TrimStart('0') })
                                    );


                                var changed = (lastGroup_ != GROUP_ || lastGtin_ != GTIN);

                                if (changed)
                                {

                                    //dump
                                    if (lastGroup_ != null && sb.Length > 0)//first
                                    {
                                        listAllData.Add(sb.ToString());
                                        MY_DIR.DUMP_MED_INFO(lastGroup_, lastGtin_, sb.ToString());
                                    }
                                    //reset
                                    sb.Clear();

                                    lastGroup_ = GROUP_;
                                    lastGtin_ = GTIN;
                                }


                                sb.AppendLine(string.Format("{0}\t{1}\t{2}\t{3}",
                                   TRACKNO, GROUP_, GTIN, DESC_
                                   ));
                            }

                            //dump last
                            if (lastGroup_ != null && sb.Length > 0)//first
                            {
                                listAllData.Add(sb.ToString());
                                MY_DIR.DUMP_MED_INFO(lastGroup_, lastGtin_, sb.ToString());
                            }


                            MY_DIR.DUMP_MED_INFO("__", "__", string.Join("", listAllData.ToArray()));


                            PLUGIN.MSGUSERINFO("T_MSG_OPERATION_OK");
                        }
                        catch (Exception exc)
                        {
                            PLUGIN.LOG(exc);
                            PLUGIN.MSGUSERERROR(exc.Message);
                        }
                    }


                    void doLoad()
                    {
                        if (readOnly)
                            return;

                        try
                        {

                            var file = ASKFILE("Text|*.txt");

                            if (string.IsNullOrEmpty(file))
                                return;

                            var res = System.IO.File.ReadAllText(file);// MY_ASKTEXT(PLUGIN, "", "");

                            if (string.IsNullOrEmpty(res))
                                return;

                            var arr = EXPLODELISTSEP(res, '\n');
                            var counterAll = 0;
                            var counterImp = 0;
                            var ok = true;
                            for (int i = 0; i < arr.Length; ++i)
                            {
                                try
                                {
                                    var itm = arr[i].Trim();

                                    if (string.IsNullOrEmpty(itm))
                                        continue;

                                    //some barcodes has ' ' inside
                                    var parts = itm.Split(new char[] { '\t' }, 3); //EXPLODELISTSEP(itm, '\t');

                                    itm = parts[0].Trim();

                                    var group = (parts.Length > 1 ? parts[1].Trim() : "");


                                    //0012345678123456789A.00123456781234567894
                                    //if (group.Length != 20)
                                    //    group = "";

                                    if (!string.IsNullOrEmpty(itm) && itm.StartsWith("01"))
                                    {

                                        {

                                            try
                                            {
                                                ++counterAll;

                                                if (doBarcode(itm, group, true))
                                                    ++counterImp;



                                            }
                                            catch (Exception exc)
                                            {
                                                PLUGIN.MSGUSERERROR("Serial import error: " + itm);
                                            }
                                        }


                                    }
                                }
                                catch (Exception exc)
                                {
                                    ok = false;
                                    PLUGIN.MSGUSERERROR("Line parse error: " + arr[i]);
                                }
                            }

                            if (ok)
                                PLUGIN.MSGUSERINFO("Imported " + counterImp + " of " + counterAll + "");





                        }
                        catch (Exception exc)
                        {
                            PLUGIN.LOG(exc);
                            PLUGIN.MSGUSERERROR(exc.Message);
                        }
                    }


                    void doRecDelete()
                    {
                        if (readOnly)
                            return;

                        try
                        {
                            var rec = _PLUGIN.TOOL_GRID.GET_GRID_ROW_DATA(cGrid);
                            if (rec != null)
                            {
                                var status = CASTASSHORT(TAB_GETROW(rec, "STATUS_"));
                                if (status == 0)
                                {
                                    if (PLUGIN.MSGUSERASK("T_MSG_COMMIT_DELETE"))
                                    {
                                        var metRef = TAB_GETROW(rec, "STOCKREF");
                                        var trackNo = TAB_GETROW(rec, "TRACKNO").ToString();

                                        var b = new ITS_BARCODE(trackNo);
                                        b.LOAD_REC(PLUGIN);

                                        _delFromDb(slipRef, b);
                                        //
                                        trackRecordOperate(metRef, -1);
                                        //
                                        rec.Table.Rows.Remove(rec);
                                    }
                                }
                                else
                                    PLUGIN.MSGUSERINFO("T_MSG_TRANS_READONLY");
                            }
                        }
                        catch (Exception exc)
                        {
                            PLUGIN.LOG(exc);
                            PLUGIN.MSGUSERERROR(exc.Message);
                        }
                    }


                    void doRecDeleteAll()
                    {
                        if (readOnly)
                            return;

                        try
                        {


                            var recSelected = _PLUGIN.TOOL_GRID.GET_GRID_ROW_DATA(cGrid);
                            if (recSelected == null)
                                return;



                            var dataTable = recSelected.Table;

                            var recDelList = new List<DataRow>();


                            var metRef = TAB_GETROW(recSelected, "STOCKREF");

                            foreach (DataRow row in dataTable.Rows)
                            {
                                var metRefCurr = TAB_GETROW(row, "STOCKREF");

                                if (COMPARE(metRefCurr, metRef))
                                {
                                    var status = CASTASSHORT(TAB_GETROW(row, "STATUS_"));
                                    var trackNo = CASTASSTRING(TAB_GETROW(row, "TRACKNO"));
                                    if (status == 0)
                                    {
                                        recDelList.Add(row);
                                    }
                                    else
                                    {
                                        PLUGIN.MSGUSERERROR("T_MSG_TRANS_READONLY\n" + trackNo);
                                        return;
                                    }
                                }

                            }


                            if (recDelList.Count > 0)
                            {
                                if (PLUGIN.MSGUSERASK("T_MSG_COMMIT_DELETE\nT_COUNT=" + recDelList.Count))
                                {

                                    foreach (var recDel in recDelList)
                                    {

                                        var trackNo = CASTASSTRING(TAB_GETROW(recDel, "TRACKNO"));

                                        var b = new ITS_BARCODE(trackNo);
                                        b.LOAD_REC(PLUGIN);

                                        _delFromDb(slipRef, b);
                                        //
                                        trackRecordOperate(metRef, -1);
                                        //
                                        dataTable.Rows.Remove(recDel);

                                    }
                                }

                            }





                        }
                        catch (Exception exc)
                        {
                            PLUGIN.LOG(exc);
                            PLUGIN.MSGUSERERROR(exc.Message);
                        }
                    }
                    bool doBarcode(string txt)
                    {

                        txt = txt.Trim().TrimStart('+');




                        if (txt.Length == 20 && txt.StartsWith("00"))
                        {


                            return doGroup(txt);



                        }


                        return doBarcode(txt, "", false);
                    }

                    bool doGroup(string pBarcodeGroupFilter)
                    {
                        if (readOnly)
                            return false;

                        try
                        {
                            //LG_$FIRM$_$PERIOD$_ITSTRACK
                            //STFICHEREF,STOCKREF,TRACKNO

                            var recSearch = TAB_GETLASTROW(PLUGIN.SQL(@"
SELECT 
--$MS$--TOP(1)
STFICHEREF,STOCKREF,GROUP_
FROM LG_$FIRM$_$PERIOD$_ITSTRACK T
WHERE 
GROUP_ LIKE @P1 --AND
--EXISTS(SELECT 1 FROM LG_$FIRM$_$PERIOD$_STFICHE WHERE LOGICALREF = T.STFICHEREF AND TRCODE IN (1,3))
ORDER BY 
STFICHEREF DESC,STOCKREF DESC
--$PG$--LIMIT 1
", new object[] { "%" + pBarcodeGroupFilter + "%" }));

                            if (recSearch == null)
                            {

                                PLUGIN.MSGUSERERROR("T_MSG_DATA_NO");
                                return false;
                            }
                            else
                            {
                                var docRef = TAB_GETROW(recSearch, "STFICHEREF");
                                var date = CASTASDATE(PLUGIN.SQLSCALAR("SELECT DATE_ FROM LG_$FIRM$_$PERIOD$_STFICHE WHERE LOGICALREF = @P1", new object[] { docRef }));
                                var trcode = CASTASSHORT(PLUGIN.SQLSCALAR("SELECT TRCODE FROM LG_$FIRM$_$PERIOD$_STFICHE WHERE LOGICALREF = @P1", new object[] { docRef }));
                                var docDesc = PLUGIN.LANG("T_DOC_STOCK_SLIP_" + FORMAT(trcode));
                                var matRef = TAB_GETROW(recSearch, "STOCKREF");
                                var matDesc = CASTASSTRING(PLUGIN.SQLSCALAR("SELECT NAME FROM LG_$FIRM$_ITEMS WHERE LOGICALREF = @P1", new object[] { matRef }));
                                var groupFull = CASTASSTRING(TAB_GETROW(recSearch, "GROUP_"));

                                var addRecs = (PLUGIN.SQL(@"
SELECT 
TRACKNO,
GROUP_
FROM LG_$FIRM$_$PERIOD$_ITSTRACK WHERE STFICHEREF = @P1 AND STOCKREF = @P2 AND GROUP_ LIKE @P3
ORDER BY STFICHEREF,STOCKREF,TRACKNO

", new object[] { docRef, matRef, "%" + pBarcodeGroupFilter + "%" }));

                                if (!PLUGIN.MSGUSERASK(string.Format(
                                    "T_LOAD\nT_DATE: {0:d}\nT_MATERIAL: {1}\nT_GROUP: {2}\nT_COUNT: {3}\nT_DOC: {4}",
                                    date,
                                    matDesc,
                                    groupFull,
                                    addRecs.Rows.Count,
                                    docDesc
                                    )))
                                    return false;




                                var counterImp = 0;

                                foreach (DataRow rowMat in addRecs.Rows)
                                {
                                    var trackNo = CASTASSTRING(TAB_GETROW(rowMat, "TRACKNO"));
                                    var groupNo = CASTASSTRING(TAB_GETROW(rowMat, "GROUP_"));
                                    //groupFull = groupFull;

                                    if (doBarcode(trackNo, groupNo, true))
                                        ++counterImp;

                                }


                                PLUGIN.MSGUSERINFO("Imported " + counterImp + " of " + addRecs.Rows.Count + "");

                                return true;

                            }



                            //
                        }
                        catch (Exception exc)
                        {
                            beepErr();
                            PLUGIN.LOG(exc);
                            PLUGIN.MSGUSERERROR(exc.Message);
                        }

                        return false;
                    }


                    bool doBarcode(string pBarcode, string pBarcodeGroup, bool pSkeepDubl)
                    {
                        if (readOnly)
                            return false;

                        try
                        {
                            var b = ITS_BARCODE.parse(pBarcode);
                            b.GROUP_ = pBarcodeGroup;

                            if (b.LOAD_REC(PLUGIN))
                            {
                                var metRef = TAB_GETROW(b.MAT_REC, "LOGICALREF");

                                var currRec = TAB_SEARCH(dataTrackNo, "STOCKREF", metRef, "TRACKNO", b.RAW);
                                if (currRec != null)
                                {
                                    TOOL_GRID.SET_GRID_POSITION(cGrid, currRec, null);
                                    if (!pSkeepDubl)
                                        throw new Exception("T_MSG_RECORD_REPEAT: " + pBarcode);

                                    return false;
                                }
                                else
                                {
                                    //
                                    _saveToDb(slipRef, b);

                                    var newRec = dataTrackNo.NewRow();
                                    TAB_SETROW(newRec, "STOCKREF", metRef);
                                    TAB_SETROW(newRec, "TRACKNO", b.RAW);
                                    //

                                    TAB_SETROW(newRec, "GTIN", b.GTIN);
                                    TAB_SETROW(newRec, "SN", b.SN);
                                    TAB_SETROW(newRec, "XD_DATE", b.XD_DATE);
                                    TAB_SETROW(newRec, "BN", b.BN);
                                    TAB_SETROW(newRec, "GROUP_", b.GROUP_);

                                    //converTrackTable(newRec);

                                    dataTrackNo.Rows.Add(newRec);

                                    var hederRec = trackRecordOperate(metRef, +1);

                                    TOOL_GRID.SET_GRID_POSITION(cGrid, newRec, null);
                                    TOOL_GRID.SET_GRID_POSITION(cGridTop, hederRec, null);

                                    return true;
                                }
                            }
                            else
                            {

                                throw new Exception("T_MSG_INVALID_RECODR: " + pBarcode);
                            }

                        }
                        catch (Exception exc)
                        {
                            beepErr();
                            PLUGIN.LOG(exc);
                            PLUGIN.MSGUSERERROR(exc.Message);
                        }

                        return false;
                    }

                    bool _saveToDb(object pDocRef, ITS_BARCODE pBarcode)
                    {
                        return UTIL_ITSTRACK.SET_TO_DB(PLUGIN, pDocRef, pBarcode);

                    }
                    void _delFromDb(object pDocRef, ITS_BARCODE pBarcode)
                    {
                        UTIL_ITSTRACK.DEL_FROM_DB(PLUGIN, pDocRef, pBarcode);
                    }
                    #endregion
                    private void cBtnDel_Click(object sender, EventArgs e)
                    {

                        doRecDelete();
                    }

                    private void cBtnDelAll_Click(object sender, EventArgs e)
                    {

                        doRecDeleteAll();
                    }

                    private void cBtnLoad_Click(object sender, EventArgs e)
                    {

                        doLoad();
                    }
                    private void cBtnExport_Click(object sender, EventArgs e)
                    {

                        doExport();
                    }


                    private void cBarcode_KeyDown(object sender, KeyEventArgs e)
                    {

                        if (e.KeyCode == Keys.Enter)
                        {
                            e.SuppressKeyPress = true;
                            //
                            var x = cBarcode.Text;
                            cBarcode.Text = "";
                            doBarcode(x);
                        }
                    }

                    private void cBtnClose_Click(object sender, EventArgs e)
                    {
                        doClose();
                    }


                    void cGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
                    {
                        try
                        {

                            //"" raise eeror
                            Clipboard.Clear();

                            var txt = TOOL_GRID.GET_GRID_CELL(sender as DataGridView).Value as string;

                            if (!string.IsNullOrEmpty(txt))
                                Clipboard.SetText(txt);



                        }
                        catch (Exception Exception)
                        {

                        }
                    }

                    private void cGrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
                    {
                        //row header rec indx
                        try
                        {
                            var grid = sender as DataGridView;

                            if (e.RowIndex >= 0)
                            {


                                var row = grid.Rows[e.RowIndex];
                                var head = row.HeaderCell;
                                var v = head.Value as string;

                                var valNew_ = (e.RowIndex + 1).ToString();

                                if (v != valNew_)
                                    head.Value = valNew_;

                            }


                            if (object.ReferenceEquals(grid, cGrid))
                            {
                                //serials

                                var row = grid.Rows[e.RowIndex];
                                var data = TOOL_GRID.GET_GRID_ROW_DATA(grid.Rows[e.RowIndex]);
                                var xd = CASTASDATE(TAB_GETROW(data, "XD_DATE"));

                                if ((xd - DateTime.Now.Date).Days < MY_ITS_GOOD_ITEM_MIN_LIFE * 30)
                                {

                                    TOOL_GRID.SETSTYLECOLORTEXT(row.DefaultCellStyle, Color.Red);
                                }


                            }
                        }
                        catch { }
                    }
                }

                public class FormPTSPacks : Form
                {

                    _PLUGIN PLUGIN;
                    //

                    DateTimePicker date1;
                    DateTimePicker date2;
                    TextBox sourceGLN;
                    //CheckBox getOnlyUnhandledPacket;
                    //

                    //
                    public long resultTransferId = 0;
                    public string defaultSourceGLN;

                    public FormPTSPacks()
                    {
                        InitializeComponent();
                        loadUI();
                        //
                        cGrid.AutoGenerateColumns = false;
                        ActiveControl = date1;
                        date1.Focus();
                        cGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                        loadData();

                        this.Load += Form_Load;
                    }

                    void Form_Load(object sender, EventArgs e)
                    {

                        { //defaults


                            sourceGLN.Text = defaultSourceGLN ?? "";

                        }
                    }



                    private void loadData()
                    {
                        var tab = new DataTable();
                        TAB_ADDCOL(tab, "CLCARD_DEFINITION_", typeof(string));
                        TAB_ADDCOL(tab, "SOURCEGLN", typeof(string));
                        TAB_ADDCOL(tab, "DATE_", typeof(DateTime));
                        TAB_ADDCOL(tab, "TRANSFERID", typeof(long));

                        tab.DefaultView.Sort = "DATE_ ASC";

                        cGrid.DataSource = tab;
                    }

                    void convertData(DataTable tab)
                    {

                        foreach (DataRow row in tab.Rows)
                            convertData(row);
                    }

                    void convertData(DataRow row)
                    {
                        if (row == null || TAB_ROWDELETED(row))
                            return;

                        var gln = TAB_GETROW(row, "SOURCEGLN").ToString();

                        var glnDesc = PLUGIN.SQLSCALAR("SELECT DEFINITION_ FROM LG_$FIRM$_CLCARD WHERE CODE = @P1", new object[] { LEFT(gln, 13) });

                        TAB_SETROW(row, "CLCARD_DEFINITION_", glnDesc);
                    }


                    void loadUI()
                    {
                        cGrid.ReadOnly = true;
                        Icon = _PLUGIN.CTRL_FORM_ICON();
                        { //grid

                            var arr = new System.Windows.Forms.DataGridViewColumn[] { 
                
                new System.Windows.Forms.DataGridViewTextBoxColumn(){
                    AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill,
                    DataPropertyName = "CLCARD_DEFINITION_",
                    HeaderText = "Cari İsim",
                },
                 new System.Windows.Forms.DataGridViewTextBoxColumn(){
                       DataPropertyName = "SOURCEGLN",
                        HeaderText = "Kaynak GLN",
                        Width = 140,
                },
                  new System.Windows.Forms.DataGridViewTextBoxColumn(){
                         DataPropertyName = "DATE_",
                        HeaderText = "Tarih",
                        Width = 100,
                },
                 new System.Windows.Forms.DataGridViewTextBoxColumn(){
                         DataPropertyName = "TRANSFERID",
                        HeaderText = "Transfer Id",
                        Width = 100,
                },
                
                };

                            this.cGrid.Columns.AddRange(arr);


                        }


                        {//btn


                            {//btn
                                var arr = new Button[] { 
                
                new Button(){
                Dock = System.Windows.Forms.DockStyle.Right,
                Width = 80,
                Tag = "close",
                Text = "Kapat",
                Image = _PLUGIN.RES_IMAGE(  "close_16x16"),
                ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
                },
 
                new Button(){
                Dock = System.Windows.Forms.DockStyle.Left,
                Width = 80,
                Tag = "import",
                Text = "Aktar",
                Image = _PLUGIN.RES_IMAGE(  "import_16x16") ,
                ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
                },
                new Button(){
                Dock = System.Windows.Forms.DockStyle.Left,
                Width = 80,
                Tag = "search",
                Text = "Ara",
                Image = _PLUGIN.RES_IMAGE(  "search_16x16") ,
                ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
                } ,
                };

                                foreach (var b in arr)
                                    b.Click += this.cBtn_Click;

                                this.cPanelBtn.Controls.AddRange(arr);
                            }




                        }
                        {//layout

                            var arrLabel = new Control[] { 
                new Label(){Text = "Tarihden"},
                new Label(){Text = "Tarihe"},
                new Label(){Text = "Kaynak GLN"},
              //  new Label(){Text = "Alınmayan Paketler"},
                };


                            var arrInput = new Control[] { 
                
               date1= new DateTimePicker(){Value = DateTime.Now,Format = DateTimePickerFormat.Short,Width = 140},
              date2 =  new DateTimePicker(){Value = DateTime.Now,Format = DateTimePickerFormat.Short,Width = 140},
               sourceGLN = new TextBox(){Width = 140},
             //  getOnlyUnhandledPacket = new CheckBox(){Text = "",Checked = false}

                };




                            //this.cLayoutTop.ColumnCount = 2;
                            this.cLayoutTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
                            this.cLayoutTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
                            //
                            //this.cLayoutTop.RowCount = arrLabel.Length;
                            for (int i = 0; i < arrLabel.Length; ++i)
                                this.cLayoutTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
                            //
                            for (int i = 0; i < arrLabel.Length; ++i)
                            {

                                this.cLayoutTop.Controls.Add(arrLabel[i], 0, i);
                                this.cLayoutTop.Controls.Add(arrInput[i], 1, i);


                            }


                            //

                        }


                    }

                    public FormPTSPacks(_PLUGIN pPLUGIN)
                        : this()
                    {
                        PLUGIN = pPLUGIN;
                    }



                    protected override void OnLoad(EventArgs e)
                    {
                        base.OnLoad(e);

                        try
                        {


                        }
                        catch (Exception exc)
                        {
                            PLUGIN.LOG(exc);
                            PLUGIN.MSGUSERERROR(exc.Message);
                            this.Close();
                        }
                    }






                    #region UI

                    void InitializeComponent()
                    {
                        this.cGrid = new System.Windows.Forms.DataGridView();
                        this.cPanelBtn = new System.Windows.Forms.Panel();
                        this.cLayoutTop = new System.Windows.Forms.TableLayoutPanel();
                        this.cLayoutContent = new System.Windows.Forms.TableLayoutPanel();
                        ((System.ComponentModel.ISupportInitialize)(this.cGrid)).BeginInit();
                        this.cLayoutContent.SuspendLayout();
                        this.SuspendLayout();
                        // 
                        // cGrid
                        // 
                        this.cGrid.AllowUserToAddRows = false;
                        this.cGrid.AllowUserToDeleteRows = false;
                        this.cGrid.AllowUserToResizeColumns = false;
                        this.cGrid.AllowUserToResizeRows = false;
                        this.cGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
                        this.cGrid.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
                        this.cGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                        this.cGrid.Location = new System.Drawing.Point(3, 3);
                        this.cGrid.MultiSelect = false;
                        this.cGrid.Name = "cGrid";
                        this.cGrid.ReadOnly = true;
                        this.cGrid.RowHeadersWidth = 60;
                        this.cGrid.Size = new System.Drawing.Size(586, 327);
                        this.cGrid.TabIndex = 0;
                        this.cGrid.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.cGrid_RowPostPaint);
                        // 
                        // cPanelBtn
                        // 
                        this.cPanelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
                        this.cPanelBtn.Location = new System.Drawing.Point(3, 336);
                        this.cPanelBtn.Name = "cPanelBtn";
                        this.cPanelBtn.Size = new System.Drawing.Size(586, 34);
                        this.cPanelBtn.TabIndex = 3;
                        // 
                        // cLayoutTop
                        // 
                        this.cLayoutTop.AutoSize = true;
                        this.cLayoutTop.Dock = System.Windows.Forms.DockStyle.Top;
                        this.cLayoutTop.Location = new System.Drawing.Point(0, 0);
                        this.cLayoutTop.Name = "cLayoutTop";
                        this.cLayoutTop.Size = new System.Drawing.Size(592, 0);
                        this.cLayoutTop.TabIndex = 5;
                        // 
                        // cLayoutContent
                        // 
                        this.cLayoutContent.ColumnCount = 1;
                        this.cLayoutContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
                        this.cLayoutContent.Controls.Add(this.cGrid, 0, 0);
                        this.cLayoutContent.Controls.Add(this.cPanelBtn, 0, 1);
                        this.cLayoutContent.Dock = System.Windows.Forms.DockStyle.Fill;
                        this.cLayoutContent.Location = new System.Drawing.Point(0, 0);
                        this.cLayoutContent.Name = "cLayoutContent";
                        this.cLayoutContent.RowCount = 2;
                        this.cLayoutContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
                        this.cLayoutContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
                        this.cLayoutContent.Size = new System.Drawing.Size(592, 373);
                        this.cLayoutContent.TabIndex = 6;
                        // 
                        // FormPTSPacks
                        // 
                        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                        this.ClientSize = new System.Drawing.Size(592, 373);
                        this.Controls.Add(this.cLayoutContent);
                        this.Controls.Add(this.cLayoutTop);
                        this.Name = "Form";
                        this.Text = "";
                        ((System.ComponentModel.ISupportInitialize)(this.cGrid)).EndInit();
                        this.cLayoutContent.ResumeLayout(false);
                        this.ResumeLayout(false);
                        this.PerformLayout();

                    }


                    private System.Windows.Forms.TableLayoutPanel cLayoutTop;
                    private System.Windows.Forms.TableLayoutPanel cLayoutContent;
                    private System.Windows.Forms.DataGridView cGrid;
                    private System.Windows.Forms.Panel cPanelBtn;


                    #endregion


                    #region CMD
                    void doClose()
                    {
                        try
                        {
                            this.Close();
                        }
                        catch (Exception exc)
                        {
                            PLUGIN.LOG(exc);
                            PLUGIN.MSGUSERERROR(exc.Message);
                        }
                    }
                    void doSearch()
                    {
                        try
                        {
                            var _df = date1.Value.Date;
                            var _dt = date2.Value.Date;
                            //
                            var _sourceGLN = sourceGLN.Text.Trim();




                            var srv = new WebServices.WebServicePacketSearch.PackageTransferHelperWebService();

                            var items_ = srv._receiveTransferDetails(PLUGIN, _sourceGLN, _df, _dt, false);// getOnlyUnhandledPacket.Checked);

                            var tab = cGrid.DataSource as DataTable;

                            if (tab == null)
                                throw new Exception("Grid datasource is not DataTable");
                            tab.Clear();

                            if (items_ != null)
                            {
                                foreach (var x in items_)
                                {
                                    var r = tab.NewRow();

                                    TAB_SETROW(r, "SOURCEGLN", x.sourceGLN);
                                    TAB_SETROW(r, "DATE_", x.transferDate);
                                    TAB_SETROW(r, "TRANSFERID", x.transferId);

                                    tab.Rows.Add(r);
                                }

                                convertData(tab);
                            }
                        }
                        catch (Exception exc)
                        {
                            PLUGIN.LOG(exc);
                            PLUGIN.MSGUSERERROR(exc.Message);
                        }
                    }

                    void doImport()
                    {
                        try
                        {
                            var rec = TOOL_GRID.GET_GRID_ROW_DATA(cGrid);
                            if (rec != null)
                            {
                                if (PLUGIN.MSGUSERASK("Paketi Almak ?"))
                                {
                                    resultTransferId = CASTASLONG(TAB_GETROW(rec, "TRANSFERID"));
                                    this.Close();
                                }
                            }
                            else
                                PLUGIN.MSGUSERINFO(TEXT.text_NO_REC_SELECTED);
                        }
                        catch (Exception exc)
                        {
                            PLUGIN.LOG(exc);
                            PLUGIN.MSGUSERERROR(exc.Message);
                        }
                    }



                    #endregion


                    private void cBtn_Click(object sender, EventArgs e)
                    {
                        var b = sender as Button;
                        if (b == null)
                            return;

                        switch (b.Tag as string)
                        {
                            case "close":
                                doClose();
                                break;
                            case "import":
                                doImport();
                                break;
                            case "search":
                                doSearch();
                                break;



                        }
                    }


                    private void cGrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
                    {
                        //row header rec indx
                        try
                        {
                            if (e.RowIndex >= 0)
                            {

                                var grid = sender as DataGridView;

                                var row = grid.Rows[e.RowIndex];
                                var head = row.HeaderCell;
                                var v = head.Value as string;

                                var valNew_ = (e.RowIndex + 1).ToString();

                                if (v != valNew_)
                                    head.Value = valNew_;

                            }
                        }
                        catch { }
                    }
                }

            }

            public class UTIL_ITSTRACK
            {

                public static DataTable GET_TAB(_PLUGIN pPLUGIN, DataSet pStockDoc)
                {
                    var res = pStockDoc.Tables["ITSTRACK"];


                    if (res == null)
                    {
                        var tSlip = pStockDoc.Tables["STFICHE"];
                        if (tSlip != null)
                        {
                            var rec = TAB_GETLASTROW(tSlip);
                            if (rec != null) //may be deleted
                            {

                                if (res == null)
                                {
                                    var lref = TAB_GETROW(rec, "LOGICALREF");

                                    res = pPLUGIN.SQL("SELECT * FROM LG_$FIRM$_$PERIOD$_ITSTRACK WHERE STFICHEREF = @P1", new object[] { lref });
                                    res.TableName = "ITSTRACK";

                                    pStockDoc.Tables.Add(res);
                                }
                            }
                        }
                    }

                    return res;
                }
                public static void ITSTRACK_ADD(_PLUGIN pPLUGIN, DataSet pStockDoc, TOOL_ITS.ITS_SERIALS pSerials)
                {
                    var tab = GET_TAB(pPLUGIN, pStockDoc);


                    foreach (var s in pSerials.SERIALS)
                    {
                        var b = new ITS_BARCODE(pSerials.ROOT.GTIN, s, pSerials.ROOT.XD_DATE, pSerials.ROOT.BN, pSerials.ROOT.GROUP_);
                        ITSTRACK_ADD(pPLUGIN, tab, b);
                    }

                }


                static void ITSTRACK_ADD(_PLUGIN pPLUGIN, DataTable pTab, string txt)
                {
                    ITSTRACK_ADD(pPLUGIN, pTab, new ITS_BARCODE(txt));
                }
                static DataRow ITSTRACK_ADD(_PLUGIN pPLUGIN, DataTable pTab, ITS_BARCODE pBarcode)
                {

                    if (pBarcode.LOAD_REC(pPLUGIN))
                    {
                        var metRef = TAB_GETROW(pBarcode.MAT_REC, "LOGICALREF");

                        var tmp_ = TAB_SEARCH(pTab, "STOCKREF", metRef, "TRACKNO", pBarcode.RAW);
                        if (tmp_ == null)
                        {
                            //
                            var newRec = pTab.NewRow();
                            TAB_SETROW(newRec, "STOCKREF", metRef);
                            TAB_SETROW(newRec, "TRACKNO", pBarcode.RAW);
                            TAB_SETROW(newRec, "GROUP_", pBarcode.GROUP_);
                            TAB_FILLNULL(newRec);
                            pTab.Rows.Add(newRec);
                            return newRec;
                        }
                        else
                            return tmp_;
                    }

                    return null;

                }




                public static void SAVE(_PLUGIN pPLUGIN, DataSet ds)
                {
                    //inside trunsaction

                    var tabItsTrack = ds.Tables["ITSTRACK"];
                    if (tabItsTrack == null)
                        return;

                    var tSlip = ds.Tables["STFICHE"];
                    if (tSlip == null)
                        return;

                    var rec = TAB_GETLASTROW(tSlip);
                    if (rec == null) //may be deleted
                        return;

                    var lref = TAB_GETROW(rec, "LOGICALREF");
                    foreach (DataRow r in tabItsTrack.Rows)
                        if (!TAB_ROWDELETED(rec) &&
                            (TAB_ROWADDED(rec) || TAB_ROWMODIFIED(rec))
                            )
                        {
                            TAB_SETROW(r, "STFICHEREF", lref);
                        }


                    foreach (DataRow r in tabItsTrack.Rows)
                        if (!TAB_ROWDELETED(rec))
                        {

                            var b = new ITS_BARCODE(TAB_GETROW(r, "TRACKNO").ToString())
                            {
                                GROUP_ = TAB_GETROW(r, "GROUP_").ToString()
                            };

                            b.LOAD_REC(pPLUGIN);

                            SET_TO_DB(
                                pPLUGIN,
                                TAB_GETROW(r, "STFICHEREF"),
                                b
                                );
                        }

                }

                public static void DEL_FROM_DB(_PLUGIN pPLUGIN, object pDocRef, ITS_BARCODE pBarcode)
                {
                    var r = pPLUGIN.SQLSCALAR(@"
DELETE FROM LG_$FIRM$_$PERIOD$_ITSTRACK WHERE STFICHEREF = @P1 AND STOCKREF = @P2 AND TRACKNO = @P3
                        ", new object[] { pDocRef, TAB_GETROW(pBarcode.MAT_REC, "LOGICALREF"), pBarcode.RAW });
                }

                public static bool SET_TO_DB(_PLUGIN pPLUGIN, object pDocRef, ITS_BARCODE pBarcode)
                {
                    var r = pPLUGIN.SQLSCALAR(
                        MY_CHOOSE_SQL(
@"
IF NOT EXISTS(SELECT 1 FROM LG_$FIRM$_$PERIOD$_ITSTRACK WHERE STFICHEREF = @P1 AND STOCKREF = @P2 AND TRACKNO = @P3)
BEGIN
INSERT INTO LG_$FIRM$_$PERIOD$_ITSTRACK
           (STFICHEREF
           ,STOCKREF
           ,TRACKNO
           ,STATUS_
           ,EDATE_
           ,GTIN
           ,SN
           ,XD_DATE
           ,BN
           ,GROUP_)
     VALUES
           (@P1
           ,@P2
           ,@P3
           ,0
,getdate() --edate
           ,@P4          
           ,@P5
           ,@P6
           ,@P7
           ,@P8

           )
SELECT 'OK'
END
ELSE
BEGIN
SELECT ''
END
                        ",


 @"

INSERT INTO LG_$FIRM$_$PERIOD$_ITSTRACK
           (STFICHEREF
           ,STOCKREF
           ,TRACKNO
           ,STATUS_
           ,EDATE_
           ,GTIN
           ,SN
           ,XD_DATE
           ,BN
           ,GROUP_)
     SELECT
            @P1
           ,@P2
           ,@P3
           ,0
           ,NOW()::TIMESTAMP(0)
           ,@P4          
           ,@P5
           ,@P6
           ,@P7
           ,@P8
            
WHERE NOT EXISTS(SELECT 1 FROM LG_$FIRM$_$PERIOD$_ITSTRACK WHERE STFICHEREF = @P1 AND STOCKREF = @P2 AND TRACKNO = @P3)

RETURNING 'OK';
 
                        "),
                         new object[]  { 
                             pDocRef, //1
                             TAB_GETROW(pBarcode.MAT_REC, "LOGICALREF"), //2
                             pBarcode.RAW, //3
                             pBarcode.GTIN, //4
                             pBarcode.SN, //5
                             pBarcode.XD_DATE, //6
                             pBarcode.BN, //7
                             pBarcode.GROUP_ //8
                         });
                    return r.ToString() == "OK";
                }

            }



            public class DYN
            {


                public static IDictionary<string, object>[] DYNOBJECT(_PLUGIN pPLUGIN)
                {
                    var res = new List<IDictionary<string, object>>();


                    //

                    res.Add(new Dictionary<string, object>() { 
                
                    {"code","ref.dyn.its.serials"},
                    {"table","ITSTRACK"},
                    {"title","ITS Serials"},
                    {"ds.sql",@"
SELECT $LIMIT$ 

ITSTRACK.*,
ITEMS.NAME AS ITEMS_____NAME,
STFICHE.TRCODE AS DUMMY_____TRCODE,COALESCE(INVOICE.LOGICALREF,STFICHE.LOGICALREF) AS DUMMY_____STOCKDOCREF 

from LG_001_01_ITSTRACK AS ITSTRACK 
left join
LG_001_01_STFICHE STFICHE on ITSTRACK.STFICHEREF = STFICHE.LOGICALREF
left join
LG_001_01_INVOICE INVOICE on STFICHE.INVOICEREF = INVOICE.LOGICALREF
left join
LG_001_ITEMS ITEMS on ITSTRACK.STOCKREF = ITEMS.LOGICALREF
$WHERE$
$ORDER$
$LIMIT2$

 
"},
                {"info.xml",
@"<?xml version='1.0' encoding='UTF-8'?>
<settings>
	<indexes>
		<arr name=''  cols='TRACKNO' sortCol='TRACKNO' /> 
	</indexes> 
	<filters>
	</filters>
</settings>
"},
            {"form.xml",
@"<?xml version='1.0' encoding='UTF-8'?>
<settings>

  <cGrid ColumnsShow='TRACKNO,ITEMS_____NAME,DUMMY_____TRCODE,DUMMY_____STOCKDOCREF,STATUS_' ColumnsExp='' ColumnsSql='' />
  <cGrid_TRACKNO Text='Serial' Width='400' />
  <cGrid_ITEMS_____NAME Text='Material' Width='200' />
  <cGrid_DUMMY_____STOCKDOCREF Text='Doc Index' Width='100' />
  <cGrid_DUMMY_____TRCODE Text='Type' FormatExt='list::LIST_STOCK_SLIP_DOC_TYPE' Width='140' />
  <cGrid_STATUS_ Text='ITS' FormatExt='list::LIST_GEN_YESNO' Width='60' />
 
</settings>
"}
                
                });



                

                    return res.ToArray();
                }

            }


            public enum WH_TYPE
            {
                depo = 1,
                expdepo = 2,
                eczane = 3,
                uretici = 4,
                hasatane = 5
            }
        }

        #endregion




        //END





        #region CLASS


        class MY_DIR
        {
            public static string PRM_DIR_ROOT = PATHCOMBINE(
     Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ITS");

            public static string PRM_DIR_SN = PATHCOMBINE(PRM_DIR_ROOT, "SN");


            public static void CHECK_DIR()
            {

                if (!Directory.Exists(PRM_DIR_SN))
                    Directory.CreateDirectory(PRM_DIR_SN);

            }


            public static void CLEAN_SN()
            {
                if (Directory.Exists(PRM_DIR_SN))
                    Directory.Delete(PRM_DIR_SN, true);





                CHECK_DIR();
            }
            public static void DUMP_MED_INFO(string pGroup, string pGtin, string pData)
            {


                var file = PRM_DIR_SN + "/" + pGroup + "_" + pGtin + ".txt";

                FILEWRITE(file, pData, true);



            }


        }


        class MY_DATASOURCE
        {

            static string VERSION_CODE = "its_api_period_$FIRM$_$PERIOD$";
            static int VERSION_INDX = 6;


            public static void UPDATE(_PLUGIN pPLUGIN)
            {
                if (ISVERSONOK(pPLUGIN))
                    return;


                UPDATE(pPLUGIN,
                @"
 EXEC('
IF NOT EXISTS (
		SELECT *
		FROM sysobjects
		WHERE id = OBJECT_ID(''LG_$FIRM$_$PERIOD$_ITSTRACK'')
		)
	CREATE TABLE LG_$FIRM$_$PERIOD$_ITSTRACK (
		STFICHEREF INT NOT NULL,
        STOCKREF INT NOT NULL,
		TRACKNO VARCHAR(60) NOT NULL,
		STATUS_ SMALLINT NULL,
		EDATE_ DATETIME NULL,
        GTIN VARCHAR(20) NOT NULL,
        SN VARCHAR(20) NOT NULL,
        XD_DATE DATETIME NOT NULL,
        BN VARCHAR(20) NOT NULL,
		CONSTRAINT I_LG_$FIRM$_$PERIOD$_ITSTRACK_I1 PRIMARY KEY (STFICHEREF,STOCKREF,TRACKNO)
		) ON [PRIMARY]
')

",
                 @"
 
	CREATE TABLE IF NOT EXISTS LG_$FIRM$_$PERIOD$_ITSTRACK (
		STFICHEREF INT NOT NULL,
        STOCKREF INT NOT NULL,
		TRACKNO VARCHAR(60) NOT NULL,
		STATUS_ SMALLINT NULL,
		EDATE_ TIMESTAMP(0) NULL,
        GTIN VARCHAR(20) NOT NULL,
        SN VARCHAR(20) NOT NULL,
        XD_DATE TIMESTAMP(0) NOT NULL,
        BN VARCHAR(20) NOT NULL,
		CONSTRAINT I_LG_$FIRM$_$PERIOD$_ITSTRACK_I1 PRIMARY KEY (STFICHEREF,STOCKREF,TRACKNO)
		) 
 
");
                UPDATE(pPLUGIN,
                @"
 EXEC('
IF NOT EXISTS(
select 1 from  [INFORMATION_SCHEMA].[COLUMNS] where TABLE_NAME = ''LG_$FIRM$_$PERIOD$_ITSTRACK'' and COLUMN_NAME = ''GROUP_''
 )
BEGIN
	 alter table LG_$FIRM$_$PERIOD$_ITSTRACK add GROUP_ VARCHAR(50) DEFAULT '''' 
END
 ') 

",
                 @"
 
 alter table LG_$FIRM$_$PERIOD$_ITSTRACK add COLUMN  IF NOT EXISTS GROUP_ VARCHAR(50) DEFAULT ''
");
                UPDATE(pPLUGIN,
                @"
 UPDATE LG_$FIRM$_$PERIOD$_ITSTRACK SET GROUP_ = '' WHERE GROUP_ IS NULL;

",
                 @"
  UPDATE LG_$FIRM$_$PERIOD$_ITSTRACK SET GROUP_ = '' WHERE GROUP_ IS NULL;
 
");
                UPDATE(pPLUGIN,
                @"
 EXEC('
IF NOT EXISTS(
select 1 from  [INFORMATION_SCHEMA].[COLUMNS] where TABLE_NAME = ''LG_$FIRM$_$PERIOD$_ITSTRACK'' and COLUMN_NAME = ''IOSTATUS''
 )
BEGIN
	alter table LG_$FIRM$_$PERIOD$_ITSTRACK add IOSTATUS SMALLINT DEFAULT 0  
END
 ') 
",
                 @"
 alter table LG_$FIRM$_$PERIOD$_ITSTRACK add COLUMN  IF NOT EXISTS IOSTATUS SMALLINT DEFAULT 0  
 
");
                UPDATE(pPLUGIN,
                @"
UPDATE LG_$FIRM$_$PERIOD$_ITSTRACK SET IOSTATUS = 0 WHERE IOSTATUS IS NULL; 

",
                 @"
 UPDATE LG_$FIRM$_$PERIOD$_ITSTRACK SET IOSTATUS = 0 WHERE IOSTATUS IS NULL; 
 
");
                UPDATE(pPLUGIN,
                @"
EXEC('
IF NOT EXISTS(

SELECT 1 FROM sys.indexes  
          WHERE Name = N''I_LG_$FIRM$_$PERIOD$_ITSTRACK_I2''
          AND Object_ID = Object_ID(N''LG_$FIRM$_$PERIOD$_ITSTRACK''))
BEGIN
	create index I_LG_$FIRM$_$PERIOD$_ITSTRACK_I2 on LG_$FIRM$_$PERIOD$_ITSTRACK(GROUP_) 
END
 ') 

",
                 @"
 create index IF NOT EXISTS I_LG_$FIRM$_$PERIOD$_ITSTRACK_I2 on LG_$FIRM$_$PERIOD$_ITSTRACK(GROUP_) 
 
");


                SETVERSION(pPLUGIN);
            }

            static bool ISVERSONOK(_PLUGIN pPLUGIN)
            {
                var currVersionNum = VERSION_INDX;
                var currVersionCode = VERSION_CODE;
                var dbVers = CASTASINT(pPLUGIN.SQLSCALAR(
                     MY_CHOOSE_SQL(
                    "select dbo.f_GETOBJVERS('" + currVersionCode + "')", //has pattern
                    "select f_GETOBJVERS('" + currVersionCode + "')"
                    ),
                    new object[] { }));


                return (dbVers >= currVersionNum);

            }

            static void SETVERSION(_PLUGIN pPLUGIN)
            {
                pPLUGIN.SQL(
                  MY_CHOOSE_SQL(
                  ("exec dbo.p_SETOBJVERS '" + VERSION_CODE + "', @P1"),
                  ("select p_SETOBJVERS ('" + VERSION_CODE + "', @P1)")
                  ), //has pattern
              new object[] { VERSION_INDX });

            }


            static void UPDATE(_PLUGIN pPLUGIN, string pMsSql, string pPgSql)
            {

                pPLUGIN.SQL(MY_CHOOSE_SQL(pMsSql, pPgSql));

            }

        }


        #endregion




        #endregion