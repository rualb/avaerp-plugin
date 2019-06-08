#line 2


     #region BODY
        //BEGIN

        public const int VERSION = 11;
        public const string FILE = "plugin.sys.event.cloudmagent.pls";



        const string event_CLOUDMAGENT_ = "_cloudmagent_";
        const string event_CLOUDMAGENT_FROMMAIN = "_cloudmagent_frommain";
        const string event_CLOUDMAGENT_FROMMAIN_QUICK = "_cloudmagent_frommain_quick";
        const string event_CLOUDMAGENT_TOMAIN = "_cloudmagent_tomain";

        const string event_CLOUDMAGENT_MNG_MAT = "_cloudmagent_mng_mat";
        const string event_CLOUDMAGENT_MNG_CLIENT = "_cloudmagent_mng_client";

        const string event_CLOUDMAGENT_REPORT = "_cloudmagent_report";

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


                    //x.MY_CLOUDMAGENT_AUTH_IDENT = s.MY_CLOUDMAGENT_AUTH_IDENT;
                    //x.MY_CLOUDMAGENT_AUTH_PW = s.MY_CLOUDMAGENT_AUTH_PW;
                    //x.MY_CLOUDMAGENT_SERVER_URL = s.MY_CLOUDMAGENT_SERVER_URL;

                    //

                    _SETTINGS.BUF = x;

                }

                //public string MY_CLOUDMAGENT_AUTH_IDENT;
                // public string MY_CLOUDMAGENT_AUTH_PW;
                // public string MY_CLOUDMAGENT_SERVER_URL;

            }

            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC)
            {

            }

            //[ECategory(TEXT.text_DESC)]
            //[EDisplayName("User Ident (Env:User)")]
            //public string MY_CLOUDMAGENT_AUTH_IDENT
            //{
            //    get
            //    {
            //        return (_GET("MY_CLOUDMAGENT_AUTH_IDENT", ""));
            //    }
            //    set
            //    {
            //        _SET("MY_CLOUDMAGENT_AUTH_IDENT", value);
            //    }

            //}

            //[ECategory(TEXT.text_DESC)]
            //[EDisplayName("User Password (Password)")]
            //public string MY_CLOUDMAGENT_AUTH_PW
            //{
            //    get
            //    {
            //        return (_GET("MY_CLOUDMAGENT_AUTH_PW", ""));
            //    }
            //    set
            //    {
            //        _SET("MY_CLOUDMAGENT_AUTH_PW", value);
            //    }

            //}

            //[ECategory(TEXT.text_DESC)]
            //[EDisplayName("Server URL Base (http://localhost:8080)")]
            //public string MY_CLOUDMAGENT_SERVER_URL
            //{
            //    get
            //    {
            //        return (_GET("MY_CLOUDMAGENT_SERVER_URL", "http://localhost:8080"));
            //    }
            //    set
            //    {
            //        _SET("MY_CLOUDMAGENT_SERVER_URL", value);
            //    }

            //}






        }



        public class TEXT
        {
            public const string text_DESC = "Desktop M-Agent";

        }



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
                case SysEvent.SYS_ISALLOWEDAUTH:
                    MY_SYS_ISALLOWEDAUTH(arg1 as string, arg2 as DataRow, arg3 as List<bool>);
                    break;
                case SysEvent.SYS_NEXTNUM:
                    MY_SYS_NEXTNUM(arg1 as Dictionary<string, object>, arg2 as List<string>);
                    break;
                case SysEvent.SYS_LOGIN:
                    TOOL_FS.CLEAN();
                    break;

                case SysEvent.SYS_ADPBEGIN:
                    MY_SYS_ADPBEGIN(arg1 as DataSet);
                    break;
                case SysEvent.SYS_ADPEND:
                    MY_SYS_ADPEND(arg1 as DataSet);
                    break;
            }



        }

        void MY_SYS_ADPEND(DataSet DATASET)
        {
            if (DATASET == null)
                return;

            var cmd = DATASET.ExtendedProperties["_SYS_PRM_DO_"] as string; //edit

            if (cmd == "delete")
                return;

            foreach (DataTable t in DATASET.Tables)
            {
                //CLFLINE 
                if (t.TableName == "INVOICE" || t.TableName == "KSLINES" )
                {
                    if (t.Rows.Count == 1 && ISEMPTYLREF(t.Rows[0]["CLIENTREF"]))
                        throw new Exception("T_MSG_SET_PERSONAL");
                }
            }

        }

        void MY_SYS_ADPBEGIN(DataSet DATASET)
        {

            if (DATASET == null)
                return;


            var cmdLine = DATASET.ExtendedProperties["_SYS_PRM_CMD_"] as string; //full str
            var code = DATASET.ExtendedProperties["_SYS_PRM_CODE_"] as string; //code
            var cmd = DATASET.ExtendedProperties["_SYS_PRM_DO_"] as string; //edit

            if (cmd == "copy")
            {
                TAB_SETCOL(DATASET.Tables[0], "GLOBID", "");
            }

        }

        public void MY_SYS_NEXTNUM(Dictionary<string, object> dictionary, List<string> list)
        {
            list.Add(FORMAT(DateTime.Now).Replace("-", "").Replace(":", "").Replace(" ", ""));
        }

        public void MY_SYS_ISALLOWEDAUTH(string pCmd, DataRow pRow, List<bool> pArg)
        {

            if (ISEMPTY(pCmd))
                return;

            var objCode = CMDLINEGETNAME(pCmd);
            var cmd = CMDLINEGETARG(pCmd, "cmd");

            var allowedTranList = GETPRM("prm_magent_allow_tran", "");



            var list = new List<string>(TOOL_FROM_MAGENT.CONVERT_LIST_ALLOWED_FROM_CLOUD(allowedTranList));


            list.Add("ref.mm.rec.mat");
            list.Add("ref.fin.rec.client");


            if (cmd == "access")
            {
                if (objCode == "_admin" || objCode == "_sys")
                {
                    pArg.Add(false);
                    return;
                }
            }

            if (objCode.StartsWith("ref."))
            {
                pArg.Add(list.Contains(objCode));
            }

            //if (objCode.StartsWith("tool."))
            //{

            //}

            if (objCode == "rep")
            {
                //pArg.Add(list.Contains(objCode));
                pArg.Add(true);
            }
            if (objCode.StartsWith("adp."))
            {
                if (pArg.Count == 0)
                {
                    pArg.Add(false);

                    var allowed = list.Contains(objCode);
                    var globId = pRow != null ? CASTASSTRING(pRow.Table.Columns.Contains("GLOBID") ? TAB_GETROW(pRow, "GLOBID") : null) : null;
                    var hasGlobId = !ISEMPTY(globId);

                    switch (cmd)
                    {
                        case "cancel":
                        case "uncancel":
                            pArg[0] = false;
                            break;
                        case "status":
                        case "edit":
                            pArg[0] = allowed && !hasGlobId;
                            break;
                        case "view":
                            pArg[0] = true;
                            break;
                        case "":
                        case "add":
                        case "copy":
                            pArg[0] = allowed;
                            break;

                    }


                }
            }




        }

        void MY_SYS_NEWFORM_INTEGRATE(Form FORM)
        {
            if (FORM == null)
                return;

            var fn = GETFORMNAME(FORM);

            if (fn == null)
                return;

            var rep_codes = new string[] { "materialinfo" };
            var rep_titles = new string[] { "T_MATERIAL - T_INFO" };
            var rep_icons32 = new string[] { "mm_32x32" };
            var rep_icons16 = new string[] { "mm_16x16" };

            var isMatList = fn.StartsWith("ref.mm.rec.mat");
            var isFormMain = fn.StartsWith("form.app");

            if (isFormMain)
            {
                {
                    var tree = CONTROL_SEARCH(FORM, "cTreeTools");
                    if (tree != null)
                    {
                        {
                            var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
            // { "_root" ,event_EMAILREP_},
			//{ "CmdText" ,"event name::"+event_STOCKMARKETTOTS_},
			{ "Text" ,TEXT.text_DESC},
			{ "ImageName" ,"folder_32x32"},
			{ "Name" ,event_CLOUDMAGENT_},
            };

                            RUNUIINTEGRATION(tree, args);

                        }

                        {
                            var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
            { "_root" ,event_CLOUDMAGENT_},
			{ "CmdText" ,"event name::"+event_CLOUDMAGENT_FROMMAIN},
			{ "Text" ,LANG("T_IMPORT")},
			{ "ImageName" ,"import_32x32"},
			{ "Name" ,event_CLOUDMAGENT_FROMMAIN},
            };

                            RUNUIINTEGRATION(tree, args);

                        }

                        {
                            var args = new Dictionary<string, object>() { 
 
                    { "_cmd" ,""},
                    { "_type" ,""},
                    { "_root" ,event_CLOUDMAGENT_},
                    { "CmdText" ,"event name::"+event_CLOUDMAGENT_FROMMAIN_QUICK},
                    { "Text" ,LANG("T_IMPORT (T_SIMPLE)")},
                    { "ImageName" ,"import_32x32"},
                    { "Name" ,event_CLOUDMAGENT_FROMMAIN_QUICK},
                    };

                            RUNUIINTEGRATION(tree, args);

                        }








                        {
                            var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_CLOUDMAGENT_},
			{ "CmdText" ,"event name::"+event_CLOUDMAGENT_TOMAIN},
			{ "Text" ,LANG("T_EXPORT")},
			{ "ImageName" ,"export_32x32"},
			{ "Name" ,event_CLOUDMAGENT_TOMAIN},
            };

                            RUNUIINTEGRATION(tree, args);

                        }




                        if (ISTRUE(GETPRM("prm_magent_mng_mat", "0")))
                        {

                            var args = new Dictionary<string, object>() { 
 
                    { "_cmd" ,""},
                    { "_type" ,""},
                    { "_root" ,event_CLOUDMAGENT_},
                    { "CmdText" ,"event name::"+event_CLOUDMAGENT_MNG_MAT},
                    { "Text" ,LANG("T_MATERIAL")},
                    { "ImageName" ,"mm_32x32"},
                    { "Name" ,event_CLOUDMAGENT_MNG_MAT},
                    };

                            RUNUIINTEGRATION(tree, args);

                        }

                        //if (ISTRUE(GETPRM("prm_magent_mng_client", "0")))
                        //{

                        //    var args = new Dictionary<string, object>() { 

                        //{ "_cmd" ,""},
                        //{ "_type" ,""},
                        //{ "_root" ,event_CLOUDMAGENT_},
                        //{ "CmdText" ,"event name::"+event_CLOUDMAGENT_MNG_CLIENT},
                        //{ "Text" ,LANG("T_CONTRACTOR")},
                        //{ "ImageName" ,"client_32x32"},
                        //{ "Name" ,event_CLOUDMAGENT_MNG_CLIENT},
                        //};

                        //    RUNUIINTEGRATION(tree, args);

                        //}


                    }

                }
                {
                    var tree = CONTROL_SEARCH(FORM, "cTreeReports");
                    if (tree != null)
                    {

                        {
                            var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
            // { "_root" ,event_EMAILREP_},
			//{ "CmdText" ,"event name::"+event_STOCKMARKETTOTS_},
			{ "Text" ,TEXT.text_DESC},
			{ "ImageName" ,"folder_32x32"},
			{ "Name" ,event_CLOUDMAGENT_},
            };

                            RUNUIINTEGRATION(tree, args);

                        }



                        for (var i = 0; i < rep_codes.Length; ++i)
                        {
                            if (ISTRUE(GETPRM("prm_magent_user_report_" + rep_codes[i], "0")))
                            {
                                var args = new Dictionary<string, object>() { 

			                                { "_cmd" ,""},
                                            { "_type" ,""},
                                            { "_root" ,event_CLOUDMAGENT_},
			                                { "CmdText" ,"event name::"+event_CLOUDMAGENT_REPORT+" loc::"+rep_codes[i]},
			                                { "Text" ,LANG(rep_titles[i])},
			                                { "ImageName" ,rep_icons32[i]},
			                                { "Name" ,event_CLOUDMAGENT_REPORT+rep_codes[i]},
                                    };

                                RUNUIINTEGRATION(tree, args);

                            }
                        }
                    }
                }

            }


            if (isMatList)
            {
                var cPanelBtnSub = CONTROL_SEARCH(FORM, "cPanelBtnSub");
                if (cPanelBtnSub != null)
                {

                    for (var i = 0; i < rep_codes.Length; ++i)
                    {
                        if (ISTRUE(GETPRM("prm_magent_user_report_" + rep_codes[i], "0")))
                        {
                            _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, "" + event_CLOUDMAGENT_REPORT + " loc::" + rep_codes[i], LANG(rep_titles[i]), rep_icons16[i]);
                        }
                    }


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
			{ "CmdText" , "event name::"+pEvent },
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
                TOOL_UI.ERROR(this, "Cant add button: " + exc.Message);
            }

        }

        public void MY_SYS_USEREVENT_HANDLER(string EVENTCODE, object[] ARGS) //adapter start
        {


            //
            try
            {


                object arg1 = ARGS.Length > 0 ? ARGS[0] : null;
                object arg2 = ARGS.Length > 1 ? ARGS[1] : null;
                var cmdLine = (((ARGS.Length > 2 ? ARGS[2] : null) as string) ?? "");

                string[] list_ = EXPLODELISTPATH(EVENTCODE);
                var cmd = list_.Length > 1 ? list_[1].ToLowerInvariant() : "";

                switch (cmd)
                {


                    case event_CLOUDMAGENT_FROMMAIN:
                        {
                            MY_FROMMAIN(false);
                        }
                        break;
                    case event_CLOUDMAGENT_FROMMAIN_QUICK:
                        {
                            MY_FROMMAIN(true);
                        }
                        break;
                    case event_CLOUDMAGENT_TOMAIN:
                        {

                            MY_TOMAIN();

                        }
                        break;
                    case event_CLOUDMAGENT_MNG_MAT:
                        {

                            MY_MNG_MAT();

                        }
                        break;
                    case event_CLOUDMAGENT_MNG_CLIENT:
                        {

                            MY_MNG_CLIENT();

                        }
                        break;
                    case event_CLOUDMAGENT_REPORT:
                        {

                            DataRow rec = null;

                            var f = arg1 as Form;
                            if (f != null) // if (ISADAPTERFORM(f))
                            {
                                var grid_ = CONTROL_SEARCH(f, "cGrid") as DataGridView;
                                if (grid_ != null)
                                    rec = TOOL_GRID.GET_GRID_ROW_DATA(grid_);

                            }
                            MY_REPORT(cmdLine, rec);

                        }
                        break;


                }


            }

            catch (Exception exc)
            {

                TOOL_UI.ERROR(this, exc);
                LOG(exc);
            }
        }
        void MY_REPORT(string pCmdLine, DataRow pRec)
        {
            try
            {

                var repCode = CMDLINEGETARG(pCmdLine, "loc");
                var filter = new Dictionary<string, string>();
                var doReport = false;
                switch (repCode)
                {
                    case "materialinfo":
                        {
                            /*
                             * {
                             * title:material info
                             * filters:{
                             *  mat: lref of material  
                             *  }
                             * }
                             */
                            var lref = "";
                            if (pRec == null)
                            {
                                var recs = REF("ref.mm.rec.mat");
                                if (recs != null && recs.Length > 0)
                                    pRec = recs[0];

                            }

                            if (pRec != null)
                            {
                                switch (pRec.Table.TableName)
                                {
                                    case "ITEMS":
                                        lref = FORMAT(pRec["LOGICALREF"]);
                                        break;
                                }

                                filter["mat"] = lref;
                                doReport = true;
                            }

                        }
                        //filters:material<required>
                        break;
                    case "materialtran":
                        //filters:material<required>,client
                        break;
                    case "clienttran":
                        //filters:client<required>
                        break;
                    default:
                        throw new Exception("Not implemented report: " + repCode);
                }

                if (!doReport)
                    return;

                var data = TOOL_WEB.connect(null, "cloud_erp_magent.agent.get", "report", repCode, JSONFORMAT(filter), false);

                if (data.data == null || data.data.Length == 0)
                    throw new Exception("T_MSG_ERROR_NO_DATA");


                if (data.isText)
                {
                    TOOL_FS.BACKUP(data.data, "report", ".html");
                    var text = Encoding.UTF8.GetString(data.data);
                    MSGUSERINFO(text);
                }
                else
                    if (data.isJson)
                    {
                        //error

                        var res = data.getAsJson<TOOL_WEB.RESPONSE>();
                        //!!!
                        res.CHECKFORERROR();

                    }
            }
            catch (Exception exc)
            {

                TOOL_UI.ERROR(this, exc.Message);
                LOG(exc);

            }
        }
        void MY_MNG_CLIENT()
        {

        }
        void MY_MNG_MAT()
        {
            try
            {

                foreach (var f in Application.OpenForms)
                {
                    var z = f as TOOL_UI.FormMngMat;
                    if (z != null)
                    {
                        z.WindowState = FormWindowState.Maximized;
                        z.Activate();
                        return;
                    }
                }

                {
                    var f = new TOOL_UI.FormMngMat(this);
                    f.Show();
                }
            }
            catch (Exception exc)
            {

                TOOL_UI.ERROR(this, exc.Message);
                LOG(exc);

            }
        }

        void MY_FROMMAIN(bool pQuick)
        {

            if (!MSGUSERASK("T_MSG_OPERATION_STARTING - T_IMPORT " + (pQuick ? "(T_SIMPLE)" : "") + " ?"))
                return;

            var dtFrom = DateTime.Now;

            MY_FROMMAIN(this, pQuick);

            var dtTo = DateTime.Now;

            MSGUSERINFO("T_MSG_OPERATION_OK - T_IMPORT " + (pQuick ? "(T_SIMPLE)" : "") + (" (" + (int)((dtTo - dtFrom).TotalSeconds) + " sec.)"));

        }


        static void MY_FROMMAIN(_PLUGIN pPLUGIN, bool pQuick)
        {



            _SETTINGS._BUF.LOAD_SETTINGS(pPLUGIN);
            //
            TOOL_DS.CHECK(pPLUGIN);
            //all data should be commeted with +guid

            if (!pQuick)
                TOOL_FROM_MAGENT.CHECK_IF_DATA_NOT_EXPORTED(pPLUGIN);
            //TOOL_FROM_MY.CHECKINMAIN(this);//!!!
            //
            var data = TOOL_WEB.connect(null, "cloud_erp_magent.agent.get", "fromcloud", "", "", false);

            if (data.data == null || data.data.Length == 0)
                throw new Exception("T_MSG_ERROR_NO_DATA");


            if (data.isText)
            {

                TOOL_FS.BACKUP(data.data, "fromcloud");

                var ds = TOOL_WEB.DATASETTSV(data.data);

                TOOL_TO_MAGNET.MY_SAVE(pPLUGIN, ds, pQuick);

            }
            else
                if (data.isJson)
                {
                    //error

                    var res = data.getAsJson<TOOL_WEB.RESPONSE>();
                    //!!!
                    res.CHECKFORERROR();

                }

        }


        void MY_TOMAIN()
        {
            if (!MSGUSERASK("T_MSG_OPERATION_STARTING - T_EXPORT ?"))
                return;

            //TOOL_DIR.INIT();
            _SETTINGS._BUF.LOAD_SETTINGS(this);
            TOOL_DS.CHECK(this);


            while (true)
            {


                var ds = new TOOL_WEB.DATASET();
                var lisGuids = new List<string>();
                TOOL_FROM_MAGENT.EXPORT_DATA(this, lisGuids, ds, 50);
                //
                if (lisGuids.Count == 0)
                    break;
                //
                MY_TOMAIN(this, ds);
                //
                foreach (var guid in lisGuids)
                    TOOL_FROM_MAGENT.MARK_GUID_AS_EXPORTED(this, guid);

                //TODO mark exported by lisGuids
                //lisGuids
            }


            //Pack
            //Send
            MSGUSERINFO("T_MSG_OPERATION_OK - T_EXPORT");
        }


        static TOOL_WEB.RESPONSE MY_TOMAIN(_PLUGIN pPLUGIN, TOOL_WEB.DATASET pDs)
        {
            _SETTINGS._BUF.LOAD_SETTINGS(pPLUGIN);
            TOOL_DS.CHECK(pPLUGIN);
            //
            var dataStr = JSONFORMAT(pDs);
            var dataRaw = Encoding.UTF8.GetBytes(dataStr);
            //
            TOOL_FS.BACKUP(dataRaw, "tocloud");
            var webRes = TOOL_WEB.connect(dataRaw, "cloud_erp_magent.agent.set", "tocloud", "", "", true);
            //
            var res = webRes.getAsJson<TOOL_WEB.RESPONSE>();
            //!!!
            res.CHECKFORERROR();
            //
            return res;

        }




        #region CLAZZ
        class TOOL_FS
        {
            public static string newfileName(string pExt = null)
            {
                if (pExt == null)
                    pExt = ".backup";

                return "magent." + FORMAT(DateTime.Now.Ticks) + pExt;
            }
            public static string dirName(string pFolder, string pFileName)
            {
                pFolder = DIR(pFolder);

                return PATHCOMBINE(pFolder, pFileName);
            }

            //file names
            public static string[] getBackups(string pFolder)
            {
                pFolder = DIR(pFolder);

                var arr = System.IO.Directory.GetFiles(pFolder, "*.backup", System.IO.SearchOption.TopDirectoryOnly);

                for (var i = 0; i < arr.Length; ++i)
                    arr[i] = System.IO.Path.GetFileName(arr[i]);

                Array.Sort<string>(arr);

                return arr;
            }

            public static void BACKUP(byte[] pData, string pFolder, string pExt = null)
            {
                var path = dirName(pFolder, newfileName(pExt));

                FILEWRITE(path, pData);

            }

            public static void CLEAN()
            {
                try
                {
                    var dir = PATHCOMBINE(GETDYNDIR(), "magent");
                    if (!System.IO.Directory.Exists(dir))
                        System.IO.Directory.CreateDirectory(dir);

                    foreach (var file in System.IO.Directory.GetFiles(dir, "*", System.IO.SearchOption.AllDirectories))
                    {
                        var info = new System.IO.FileInfo(file);

                        if ((DateTime.Now - info.CreationTime).TotalDays > 30)
                        {
                            info.Delete();
                        }
                    }
                }
                catch (Exception exc)
                {
                    RUNTIMELOG(exc.ToString());
                }
            }

            static string DIR(string pFolder)
            {
                var dir = PATHCOMBINE(GETDYNDIR(), "magent/" + pFolder);

                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);

                return dir;

            }

        }
        class TOOL_WEB
        {




            static public WebData connect(byte[] pPostData, string pController, string pObjName, string pObjCmd, string pObjVal, bool pUsePOST = false)
            {




                var url = GET_ENV("API_SERVER_URL", "");// _SETTINGS.BUF.MY_CLOUDMAGENT_SERVER_URL;

                if (ISEMPTY(url))
                    throw new Exception("Server url parameters is empty");

                pPostData = pPostData ?? new byte[] { };

                var webData = new WebData();

                using (var client = new _WebClient())
                {

                    var ident = GET_ENV("API_AUTH_IDENT", "");//_SETTINGS.BUF.MY_CLOUDMAGENT_AUTH_IDENT;
                    var token = GET_ENV("API_AUTH_PW", "");//_SETTINGS.BUF.MY_CLOUDMAGENT_AUTH_PW;

                    if (ISEMPTY(ident) || ISEMPTY(token))
                        throw new Exception("Auth parameters is empty");

                    //s1.env:user
                    var envPrefix = EXPLODELISTSEP(ident, '.')[0];
                    // client.Headers.Add("x-sys-ident", ident);
                    // client.Headers.Add("x-sys-token", "pw " + token);

                    webData.headers["x-sys-ident"] = ident;
                    webData.headers["x-sys-token"] = "pw " + token;

                    // pPostData = ZIPDEFLATE(pPostData);
                    webData.data = pPostData;

                    url = url + "/api/" + envPrefix + "?__ctrl=" + pController + "&_objname=" + STRENCODE(pObjName) + "&_objcmd=" + STRENCODE(pObjCmd) + "&_objval=" + STRENCODE(pObjVal);


                    if (pUsePOST)
                    {
                        webData.postData(client, url);

                        //byte[] pageData = client.UploadData(url, "POST", pPostData);

                        //pageData = UNZIPDEFLATE(pageData);

                        //return pageData;
                    }
                    else
                    {
                        webData.downloadData(client, url);

                        //byte[] pageData = client.DownloadData(url);//GET

                        //pageData = UNZIPDEFLATE(pageData);
                        //return pageData;
                    }


                }

                return webData;

            }


            public class WebData
            {

                public byte[] data;
                public bool isJson = false;
                public bool isText = false;


                public Dictionary<string, string> headers = new Dictionary<string, string>();


                public void before(System.Net.WebClient client)
                {
                    foreach (var k in headers.Keys)
                        client.Headers.Add(k, headers[k]);

                    if (data != null && data.Length > 0)
                        data = ZIPDEFLATE(data);
                }

                public void after(System.Net.WebClient client)
                {
                    headers.Clear();

                    var cencoding = client.ResponseHeaders["content-encoding"];
                    if (cencoding != null && cencoding == "deflate")
                    {
                        //if "application/octet-stream"
                        if (data != null && data.Length > 0)
                            data = UNZIPDEFLATE(data);

                    }

                    var ctype = client.ResponseHeaders["content-type"];

                    isJson = ctype != null && ctype.StartsWith("application/json");
                    isText = ctype != null && ctype.StartsWith("text/plain");

                    foreach (var k in client.ResponseHeaders.AllKeys)
                        headers[k] = client.ResponseHeaders[k];


                }

                public void downloadData(System.Net.WebClient client, string pUrl)
                {
                    before(client);
                    data = client.DownloadData(pUrl);//GET
                    after(client);
                }

                public void postData(System.Net.WebClient client, string pUrl)
                {
                    before(client);
                    data = client.UploadData(pUrl, "POST", data);
                    after(client);

                }


                public T getAsJson<T>()
                {
                    if (this.isJson)
                    {
                        //error
                        var jsonTExt = Encoding.UTF8.GetString(this.data);

                        return JSONPARSE<T>(jsonTExt);

                    }

                    throw new Exception("Result not JSON");

                }

            }

            private class _WebClient : System.Net.WebClient
            {

                protected override System.Net.WebRequest GetWebRequest(Uri uri)
                {
                    var request = base.GetWebRequest(uri);
                    request.Timeout = 5 * 1000;
                    ((System.Net.HttpWebRequest)request).ReadWriteTimeout = 20 * 1000;
                    return request;
                }
            }


            public static DATASET DATASETTSV(byte[] pData)
            {
                DATASET ds = new DATASET();
                var sr = new System.IO.StreamReader(new System.IO.MemoryStream(pData), Encoding.UTF8);
                string line = null;
                TABLE tab = null;
                string[] colsLine = null;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "")
                        continue;
                    string[] arr = line.Split('\t');

                    switch (arr[0])
                    {
                        case "$begin":
                            colsLine = null;
                            tab = new TABLE();
                            if (arr.Length > 1)
                                tab.TABLENAME = arr[1];
                            ds.TABLES.Add(tab);
                            break;
                        case "$end":
                            //
                            break;
                        case "$cols":
                            colsLine = arr;
                            for (var i = 1; i < colsLine.Length; ++i)
                                tab.COLS[colsLine[i]] = "string";
                            break;
                        case "$types":
                            if (colsLine != null)
                                for (var i = 1; i < arr.Length && i < colsLine.Length; ++i)
                                    tab.COLS[colsLine[i]] = arr[i];
                            break;
                        case "$prop":
                            for (var i = 1; i < arr.Length - 1; i += 2)
                                tab.PROP[arr[i + 0]] = arr[i + 1];
                            break;
                        case "$":
                            if (colsLine != null)
                            {
                                var row = new TOOL_WEB.ROW();
                                for (var i = 1; i < arr.Length && i < colsLine.Length; ++i)
                                    row[colsLine[i]] = arr[i];
                                tab.ROWS.Add(row);
                            }
                            break;
                    }

                }



                return ds;

            }

            public class ROW : Dictionary<string, object>
            {


            }

            public class TABLE
            {

                public string TABLENAME;

                public Dictionary<string, string> PROP = new Dictionary<string, string>();

                public List<ROW> ROWS = new List<ROW>();

                public Dictionary<string, string> COLS = new Dictionary<string, string>();

                public static TABLE CREATE(DataTable pTab)
                {

                    var res_ = new TABLE();

                    res_.TABLENAME = pTab.TableName;

                    foreach (DataColumn col_ in pTab.Columns)
                        res_.COLS.Add(col_.ColumnName, FORMAT(col_.DataType));

                    foreach (DataRow row_ in pTab.Rows)
                        if (!TAB_ROWDELETED(row_))
                        {
                            var row = new ROW();
                            foreach (string col_ in res_.COLS.Keys)// pTab.Columns)
                                row[col_] = FORMAT(row_[col_]);

                            res_.ROWS.Add(row);
                        }

                    foreach (string key_ in pTab.ExtendedProperties.Keys)
                    {
                        string val_ = pTab.ExtendedProperties[key_] as string;
                        if (val_ != null)
                            res_.PROP[key_] = val_;
                    }


                    return res_;
                }

                public static DataTable CREATE(TABLE pTab)
                {

                    var res_ = new DataTable();


                    res_.TableName = pTab.TABLENAME;

                    foreach (string key_ in pTab.COLS.Keys)
                        res_.Columns.Add(key_, PARSETYPE(pTab.COLS[key_]));


                    foreach (var row_ in pTab.ROWS)
                    {
                        DataRow rowNew_ = res_.NewRow();

                        foreach (DataColumn col_ in res_.Columns)
                        {
                            var val_ = row_[col_.ColumnName];

                            if (val_ == null)
                                rowNew_[col_] = GETTYPEDEFAULVALUE(col_.DataType);
                            else
                                rowNew_[col_] = PARSE(FORMAT(val_), col_.DataType);

                        }
                        res_.Rows.Add(rowNew_);

                    }


                    foreach (string key_ in pTab.PROP.Keys)
                    {
                        res_.ExtendedProperties[key_] = pTab.PROP[key_];
                    }





                    return res_;
                }

            }
            public class DATASET
            {

                public string DATASETNAME;

                public Dictionary<string, string> PROP = new Dictionary<string, string>();

                public List<TABLE> TABLES = new List<TABLE>();

                public static DataSet CREATE(DATASET pDs)
                {

                    DataSet res_ = new DataSet();

                    res_.DataSetName = pDs.DATASETNAME;


                    foreach (var tab_ in pDs.TABLES)
                    {
                        res_.Tables.Add(TABLE.CREATE(tab_));
                    }


                    foreach (var key_ in pDs.PROP.Keys)
                    {
                        res_.ExtendedProperties[key_] = pDs.PROP[key_];
                    }


                    return res_;
                }


                public static DATASET CREATE(DataSet pDs)
                {

                    var res_ = new DATASET();

                    res_.DATASETNAME = pDs.DataSetName;


                    foreach (DataTable tab_ in pDs.Tables)
                    {
                        res_.TABLES.Add(TABLE.CREATE(tab_));
                    }

                    foreach (string key_ in pDs.ExtendedProperties.Keys)
                    {
                        string val_ = pDs.ExtendedProperties[key_] as string;
                        if (val_ != null)
                            res_.PROP[key_] = val_;
                    }


                    return res_;
                }
            }

            public class RESPONSE
            {
                public Dictionary<string, string> PROP = new Dictionary<string, string>();

                public void CHECKFORERROR()
                {
                    if (PROP == null)
                        throw new Exception("PROP is null");

                    if (PROP.ContainsKey("ERROR") && !ISEMPTY(PROP["ERROR"]))
                        throw new Exception(PROP["ERROR"]);

                    if (PROP.ContainsKey("OK") && ISTRUE(PROP["OK"]))
                        return;
                    else
                        throw new Exception("Result in not OK");


                }

            }


        }


        //TODO
        class TOOL_DS
        {

            public static void CHECK(_PLUGIN pPLUGIN)
            {

                var currVersionNum = 8;
                var currVersionCode = "CLOUDMAGENT_$FIRM$_$PERIOD$";


                var dbVers = pPLUGIN.GETVERSION(currVersionCode);


                if (dbVers >= currVersionNum)
                    return;






                pPLUGIN.SETVERSION(currVersionCode, currVersionNum);



            }






            public static Dictionary<string, string> GET_PRMS(_PLUGIN pPLUGIN, bool pSys = true)
            {
                var filter = pSys ? "prm_magent_%" : "%";
                var res = new Dictionary<string, string>();
                var t = pPLUGIN.SQL("select * from L_FIRMPARAMS where CODE like @P1", new object[] { filter });
                foreach (DataRow r in t.Rows)
                    res[CASTASSTRING(TAB_GETROW(r, "CODE"))] = CASTASSTRING(TAB_GETROW(r, "VALUE"));


                return res;
            }





        }

        //class TOOL_DIR
        //{
        //    public static string FILE_FROMMAIN_ZIP;
        //    public static string FILE_TOMAIN_ZIP;

        //    public static string WORK_DIR;
        //    public static string WORK_DIR_FROMMAIN;
        //    public static string WORK_DIR_TOMAIN;

        //    public static void INIT()
        //    {
        //        if (WORK_DIR == null)
        //        {
        //            WORK_DIR = System.IO.Path.Combine(_PLUGIN.GETHOMEDIR(), "../ava.cloudmagent");
        //            WORK_DIR = System.IO.Path.GetFullPath(WORK_DIR);

        //            WORK_DIR_FROMMAIN = System.IO.Path.Combine(WORK_DIR, "frommain");
        //            WORK_DIR_TOMAIN = System.IO.Path.Combine(WORK_DIR, "tomain");


        //            FILE_FROMMAIN_ZIP = System.IO.Path.Combine(WORK_DIR_FROMMAIN, "frommain.zip");
        //            FILE_TOMAIN_ZIP = System.IO.Path.Combine(WORK_DIR_TOMAIN, "tomain.zip");
        //        }

        //        foreach (var d in new string[] { WORK_DIR, WORK_DIR_FROMMAIN, WORK_DIR_TOMAIN })
        //            if (!System.IO.Directory.Exists(d))
        //                System.IO.Directory.CreateDirectory(d);
        //    }

        //    public static void CLEAN_FROMMAIN()
        //    {

        //        System.IO.Directory.Delete(WORK_DIR_FROMMAIN, true);

        //        INIT();
        //    }

        //    public static void CLEAN_TOMAIN()
        //    {

        //        System.IO.Directory.Delete(WORK_DIR_FROMMAIN);

        //        INIT();
        //    }
        //}

        class TOOL_FROM_MAGENT
        {

            public static void CHECK_IF_DATA_NOT_EXPORTED(_PLUGIN pPLUGIN)
            {
                var data = ALLDOCS(pPLUGIN, false);
                //
                if (data.Rows.Count > 0)
                {
                    throw new Exception("Some of data not exported: count : " + data.Rows.Count);
                }
            }

            public static void MARK_GUID_AS_EXPORTED(_PLUGIN pPLUGIN, string pGuid)
            {
                var data = pPLUGIN.SQL(@"

UPDATE LG_$FIRM$_$PERIOD$_INVOICE SET GLOBID=@P1 WHERE GLOBID = @P2;
UPDATE LG_$FIRM$_$PERIOD$_STFICHE SET GLOBID=@P1 WHERE GLOBID = @P2;
UPDATE LG_$FIRM$_$PERIOD$_ORFICHE SET GLOBID=@P1 WHERE GLOBID = @P2;
UPDATE LG_$FIRM$_$PERIOD$_KSLINES SET GLOBID=@P1 WHERE GLOBID = @P2;
 
", new object[] { "+" + pGuid, pGuid });
            }

            //public static void CHECKINMAIN(_PLUGIN pPLUGIN)
            //{
            //    var xml = GET_CHECKINMAIN(pPLUGIN);

            //    var xmlRaw = Encoding.UTF8.GetBytes(xml);

            //    var res = TOOL_WEB.connect(xmlRaw, "cloud_erp_magent.agent.get", "data", false);

            //    var resXml = Encoding.UTF8.GetString(res.data);

            //    APPLY_CHECKINMAIN(pPLUGIN, resXml);

            //}

            //static void APPLY_CHECKINMAIN(_PLUGIN pPLUGIN, string pXml)
            //{
            //    var doc = new System.Xml.XmlDocument();
            //    doc.LoadXml(pXml);

            //    var root = doc["settings"];
            //    var list = new List<string>();

            //    foreach (System.Xml.XmlNode node in root.ChildNodes)
            //    {
            //        var cmd = XMLNODEATTR(node, "cmd");
            //        switch (cmd)
            //        {
            //            case "IMPHIS":
            //                {
            //                    var tab_ = CASTASSTRING(XMLNODEATTR(node, "type"));
            //                    var value_ = CASTASSTRING(XMLNODEATTR(node, "value"));
            //                    var ficheno_ = CASTASSTRING(XMLNODEATTR(node, "ficheno"));
            //                    var lref_ = PARSEINT(CASTASSTRING(XMLNODEATTR(node, "indx")));
            //                    var guid_ = CASTASSTRING(XMLNODEATTR(node, "lref"));

            //                    if (value_ == "1")
            //                    {

            //                        pPLUGIN.SQL(string.Format(
            //     "update LG_$FIRM$_$PERIOD$_{0} set GLOBID = @P2 where LOGICALREF = @P1", tab_),
            //     new object[] { lref_, "+" + guid_ });


            //                    }
            //                    else
            //                    {
            //                        var modDesc = "";

            //                        switch (tab_)
            //                        {
            //                            case "INVOICE": modDesc = "T_INVOICE"; break;
            //                            case "STFICHE": modDesc = "T_SLIP"; break;
            //                            case "CASH": modDesc = "T_CASH"; break;
            //                            case "ORFICHE": modDesc = "T_ORDER"; break;
            //                        }

            //                        list.Add(string.Format("{0}:{1}", modDesc, ficheno_));

            //                    }
            //                }
            //                break;
            //        }

            //    }

            //    if (list.Count > 0)
            //    {
            //        if (list.Count > 10)
            //            list.RemoveRange(10, list.Count - 10 - 1);

            //        list.Insert(0, "T_MSG_ERROR_CHECK_FAILED");
            //        var errMsg = string.Join("\n", list.ToArray());
            //        throw new Exception(errMsg);
            //    }


            //}
            //static string GET_CHECKINMAIN(_PLUGIN pPLUGIN)
            //{

            //    var doc = new System.Xml.XmlDocument();

            //    var root = doc.CreateElement("settings");

            //    doc.AppendChild(root);


            //    {
            //        var tmp = doc.CreateElement("arr");
            //        XMLNODEATTR(tmp, "cmd", "GUID");
            //        XMLNODEATTR(tmp, "value", pPLUGIN.GETPRM("MOB_USR_DATA_ID", ""));
            //        root.AppendChild(tmp);
            //    }


            //    {


            //        var data = ALLDOCS(pPLUGIN);

            //        var hasUnTouched = false;

            //        foreach (DataRow row in data.Rows)
            //        {
            //            var tab_ = CASTASSTRING(TAB_GETROW(row, "TYPE_"));
            //            var lref_ = (TAB_GETROW(row, "LOGICALREF"));
            //            var guid_ = CASTASSTRING(TAB_GETROW(row, "GLOBID"));
            //            var ficheno_ = CASTASSTRING(TAB_GETROW(row, "FICHENO"));

            //            if (guid_ == "")
            //            {
            //                hasUnTouched = true;
            //                break;
            //            }

            //            var tmp = doc.CreateElement("arr");

            //            XMLNODEATTR(tmp, "cmd", "IMPHIS");
            //            XMLNODEATTR(tmp, "type", tab_);
            //            XMLNODEATTR(tmp, "lref", guid_);
            //            XMLNODEATTR(tmp, "ficheno", ficheno_);
            //            XMLNODEATTR(tmp, "indx", lref_);

            //            root.AppendChild(tmp);

            //        }

            //        if (hasUnTouched)
            //            throw new Exception("T_MSG_ERROR_NOT_ALLOWED (Has Un-Exported Doc)");

            //    }

            //    var res = XMLDOCFORMAT(doc);

            //    return res;
            //}


            static DataTable ALLDOCS(_PLUGIN pPLUGIN, bool pSetReadonly)
            {


                var data = pPLUGIN.SQL(@"
SELECT 
    TYPE_,LOGICALREF,DATE_,FICHENO,TRCODE,GLOBID 
FROM (
SELECT 'INVOICE' TYPE_,LOGICALREF,DATE_,FICHENO,TRCODE,GLOBID FROM LG_$FIRM$_$PERIOD$_INVOICE WHERE  CANCELLED = 0 AND (GRPCODE IN (1,2) AND TRCODE IN (1,6,3,8))
UNION
SELECT 'STFICHE' TYPE_,LOGICALREF,DATE_,FICHENO,TRCODE,GLOBID FROM LG_$FIRM$_$PERIOD$_STFICHE WHERE CANCELLED = 0 AND (GRPCODE = 3 AND TRCODE IN (50,51,11,12,25))
UNION
--SELECT 'ORFICHE' TYPE_,LOGICALREF,DATE_,FICHENO,TRCODE,GLOBID FROM LG_$FIRM$_$PERIOD$_ORFICHE WHERE CANCELLED = 0 AND (TRCODE IN (1,2))
--UNION
SELECT 'KSLINES' TYPE_,LOGICALREF,DATE_,FICHENO,TRCODE,GLOBID FROM LG_$FIRM$_$PERIOD$_KSLINES WHERE CANCELLED = 0 AND (TRCODE IN (11,12))
) T WHERE T.GLOBID NOT LIKE '+%' ORDER BY DATE_ ASC, TYPE_ ASC,LOGICALREF ASC
");


                //mark docs as read only

                if (pSetReadonly)
                {
                    var agentId = pPLUGIN.GETPRM("prm_magent_agent_id", "");


                    foreach (DataRow row in data.Rows)
                    {
                        var tab_ = CASTASSTRING(TAB_GETROW(row, "TYPE_"));
                        var lref_ = (TAB_GETROW(row, "LOGICALREF"));
                        var guid_ = CASTASSTRING(TAB_GETROW(row, "GLOBID"));
                        var dateStr_ = LEFT(FORMAT(TAB_GETROW(row, "DATE_")), 10);
                        var typeStr_ = FORMAT(TAB_GETROW(row, "TRCODE"));

                        if (guid_ == "")
                        {
                            //unix time
                            var sec = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                            guid_ = "madesk/" + FORMAT(agentId) + "/" + typeStr_ + "/" + FORMAT(sec) + "/" + FORMAT(lref_); // GUID();

                            pPLUGIN.SQL(string.Format(
                                "update LG_$FIRM$_$PERIOD$_{0} set GLOBID = @P2 where LOGICALREF = @P1", tab_),
                                new object[] { lref_, guid_ });

                            TAB_SETROW(row, "GLOBID", guid_);
                        }


                    }

                }

                return data;



            }

            public static string[] CONVERT_LIST_ALLOWED_FROM_CLOUD(string pList)
            {

                var res = new List<string>();

                var arr = pList.Split(',');

                var dic = new Dictionary<int, string[]>() { 

                    {111,new string[]{"adp.mm.doc.slip.11","ref.mm.doc.slip"}},
                    {112,new string[]{"adp.mm.doc.slip.12","ref.mm.doc.slip"}},
                    {113,new string[]{"adp.mm.doc.slip.13","ref.mm.doc.slip"}},
                    {114,new string[]{"adp.mm.doc.slip.14","ref.mm.doc.slip"}},
                    {125,new string[]{"adp.mm.doc.slip.25","ref.mm.doc.slip"}},
                    {150,new string[]{"adp.mm.doc.slip.50","ref.mm.doc.slip"}},
                    {151,new string[]{"adp.mm.doc.slip.51","ref.mm.doc.slip"}},

                    {201,new string[]{"adp.prch.doc.inv.1","ref.prch.doc.inv"}},
                    {206,new string[]{"adp.prch.doc.inv.6","ref.prch.doc.inv"}},

                    {303,new string[]{"adp.sls.doc.inv.3","ref.sls.doc.inv"}},
                    {308,new string[]{"adp.sls.doc.inv.8","ref.sls.doc.inv"}},

                    {411,new string[]{"adp.fin.doc.cash.11","ref.fin.doc.cash"}},
                    {412,new string[]{"adp.fin.doc.cash.12","ref.fin.doc.cash"}},

                };



                foreach (var itm in arr)
                {

                    int rectype = 0;

                    int.TryParse(itm, out rectype);

                    if (dic.ContainsKey(rectype))
                    {
                        res.AddRange(dic[rectype]);
                    }

                }

                return res.ToArray();
            }

            static int GET_CLOUD_REC_TYPE(string pTab, short pTrcode)
            {
                short moduleMM = 100;
                short modulePRCH = 200;
                short moduleSALE = 300;
                short moduleCASH = 400;
                short moduleBANK = 500;
                short moduleCLIENT = 600;

                switch (pTab)
                {

                    case "INVOICE":
                    case "STFICHE":
                        switch (pTrcode)
                        {
                            //
                            case 1:
                            case 6:
                                return modulePRCH + pTrcode;
                            case 3:
                            case 8:
                                return moduleSALE + pTrcode;
                            //
                            case 11:
                            case 12:
                            case 13:
                            case 25:
                            case 50:
                            case 51:
                                return moduleMM + pTrcode;

                        }
                        break;
                    case "KSLINES":
                        switch (pTrcode)
                        {
                            case 11:
                            case 12:
                                return moduleCASH + pTrcode;
                        }
                        break;
                }

                throw new Exception("Undefined combination: " + pTab + " and " + pTrcode);


            }

            static string _CORRECT(object obj)
            {
                //Upper

                return FORMAT(obj).ToUpperInvariant();
                // return txt;
            }
            public static void EXPORT_DATA(_PLUGIN pPLUGIN, List<string> pGUID, TOOL_WEB.DATASET pDATASET, int pCount)
            {

                var agentId = PARSESHORT(pPLUGIN.GETPRM("prm_magent_agent_id", ""));

                //var doc = new System.Xml.XmlDocument();

                // var root = doc.CreateElement("DATA");
                //doc.AppendChild(root);

                // var hasDocs = false;

                var ds = pDATASET;
                ds.DATASETNAME = "tran";

                {
                    //prop

                    var dic = TOOL_DS.GET_PRMS(pPLUGIN);

                    foreach (var k in dic.Keys)
                    {
                        ds.PROP[k] = dic[k];// XMLNODEATTR(root, k, dic[k]);
                    }

                }

                {
                    var docs = ALLDOCS(pPLUGIN, true);

                    // var cards = new MY_CARDS_EXPORT(agentId);

                    int _count = 0;
                    foreach (DataRow row in docs.Rows)
                    {
                        ++_count;
                        //
                        if (_count > pCount)
                            break;
                        //
                        //TYPE_,LOGICALREF,DATE_,FICHENO,TRCODE,GLOBID
                        //  hasDocs = true;

                        var tab_ = CASTASSTRING(TAB_GETROW(row, "TYPE_"));
                        var lref_ = (TAB_GETROW(row, "LOGICALREF"));
                        var guid_ = CASTASSTRING(TAB_GETROW(row, "GLOBID"));

                        switch (tab_)
                        {

                            case "INVOICE":
                            case "STFICHE":
                                {
                                    var isInv = (tab_ == "INVOICE");


                                    var recH = TAB_GETLASTROW(pPLUGIN.SQL(@"
select 
LOGICALREF,TRCODE,FICHENO,DATE_," + (isInv ? "TIME_" : "FTIME") + @",
DOCODE,DOCTRACKINGNR,SPECODE,SPECODE2,SPECODE3,
CLIENTREF,DEPARTMENT,SOURCEINDEX,PROJECTREF,
GLOBID,GENEXP1

from " + (isInv ? "LG_$FIRM$_$PERIOD$_INVOICE" : "LG_$FIRM$_$PERIOD$_STFICHE") + @" T where LOGICALREF = @P1
", new object[] { lref_ }));
                                    if (recH == null)
                                        throw new Exception("Invoice doc not found: " + lref_);

                                    var recsL = pPLUGIN.SQL(@"
select
LOGICALREF,GLOBTRANS,LINETYPE,CLIENTREF,STOCKREF,AMOUNT,VATMATRAH,DISTDISC,DISTEXP,VATINC,VAT,LINEEXP,SOURCEINDEX,IOCODE
                    from LG_$FIRM$_$PERIOD$_STLINE T where " + (isInv ? "INVOICEREF" : "STFICHEREF") + @" = @P1 order by " + (isInv ? "INVOICELNNO" : "STFICHELNNO") + @" asc
", new object[] { lref_ });

                                    {
                                        var docBody = new TOOL_WEB.TABLE();
                                        docBody.TABLENAME = "tran";
                                        docBody.COLS = null;
                                        //
                                        pGUID.Add(FORMAT(TAB_GETROW(recH, "GLOBID")));
                                        //
                                        var trcode = CASTASSHORT(TAB_GETROW(recH, "TRCODE"));
                                        //


                                        //rectype shoul be in header
                                        docBody.PROP["rectype"] = FORMAT(GET_CLOUD_REC_TYPE(tab_, trcode));
                                        docBody.PROP["guid"] = FORMAT(TAB_GETROW(recH, "GLOBID"));
                                        docBody.PROP["recdate"] = FORMAT(TAB_GETROW(recH, "DATE_"));
                                        var time_ = GETINTTIMETOTIME(CASTASINT(TAB_GETROW(recH, (isInv ? "TIME_" : "FTIME"))));
                                        docBody.PROP["rectime"] = FORMAT(time_.Hour * 100 + time_.Minute);

                                        docBody.PROP["tracknr"] = _CORRECT(TAB_GETROW(recH, "DOCTRACKINGNR"));
                                        docBody.PROP["textf1"] = _CORRECT(TAB_GETROW(recH, "GENEXP1"));

                                        docBody.PROP["markcode1"] = _CORRECT(TAB_GETROW(recH, "SPECODE"));
                                        docBody.PROP["markcode2"] = _CORRECT(TAB_GETROW(recH, "SPECODE2"));
                                        docBody.PROP["markcode3"] = _CORRECT(TAB_GETROW(recH, "SPECODE3"));

                                        docBody.PROP["prjref"] = FORMAT(TAB_GETROW(recH, "PROJECTREF"));


                                        //docBody.PROP["dep"] = FORMAT(TAB_GETROW(recH, "DEPARTMENT"));
                                        //docBody.PROP["wh"] = FORMAT(TAB_GETROW(recH, "SOURCEINDEX"));

                                        // docBody.PROP["srclref"] = FORMAT(0);

                                        foreach (DataRow recL in recsL.Rows)
                                        {
                                            //and (TRCODE != 25 OR (TRCODE = 25 AND IOCODE = 2)) 

                                            var iocode = CASTASSHORT(TAB_GETROW(recL, "IOCODE"));

                                            if (trcode == 25 && iocode != 2)
                                                continue;

                                            var glob = CASTASSHORT(TAB_GETROW(recL, "GLOBTRANS"));
                                            var ltype = CASTASSHORT(TAB_GETROW(recL, "LINETYPE"));

                                            if (glob == 0 && (ltype == 0 || ltype == 1))
                                            {
                                                var isPromo = (ltype == 1);
                                                //                         
                                                var amnt = CASTASDOUBLE(TAB_GETROW(recL, "AMOUNT"));
                                                var price = 0.0;
                                                var vat = CASTASDOUBLE(TAB_GETROW(recL, "VAT"));
                                                //
                                                var totVatBase = CASTASDOUBLE(TAB_GETROW(recL, "VATMATRAH"));
                                                var discTot = CASTASDOUBLE(TAB_GETROW(recL, "DISTDISC"));
                                                var expTot = CASTASDOUBLE(TAB_GETROW(recL, "DISTEXP"));
                                                price = DIV(totVatBase - ((discTot - expTot)), amnt);
                                                //negative disc is surch
                                                var discPerc = DIV(100.0 * (discTot - expTot), price * amnt);

                                                var bodyLine = new TOOL_WEB.ROW(){
                                                {"card1ref",FORMAT((TAB_GETROW(recL, "STOCKREF")))},
                                                {"card2ref",FORMAT( (TAB_GETROW(recL, "CLIENTREF")))},
                                                {"qty",FORMAT(amnt)},
                                                {"price",FORMAT(price)},
                                                {"discperc",FORMAT(isPromo?100:discPerc)},
                                                {"vat",FORMAT(vat)},
                                                {"textf2",_CORRECT(TAB_GETROW(recL, "LINEEXP"))},
                                                //
                                                {"dep",FORMAT(TAB_GETROW(recH, "DEPARTMENT"))},//header
                                                {"wh",FORMAT(TAB_GETROW(recL, "SOURCEINDEX"))},//line
                                                //
                                                };

                                                //
                                                docBody.ROWS.Add(bodyLine);

                                            }
                                            //
                                        }

                                        ds.TABLES.Add(docBody);

                                    }
                                }
                                break;


                            case "ORFICHE":
                                {

                                }
                                break;

                            case "KSLINES":
                                {
                                    var recH = TAB_GETLASTROW(pPLUGIN.SQL(@"
                                 
                                SELECT
                                LOGICALREF,FICHENO,
                                DATE_,HOUR_,MINUTE_,TRCODE,DEPARTMENT,PROJECTREF,
                                GLOBID,
                                SPECODE,SPECODE2,SPECODE3,CUSTTITLE,LINEEXP,AMOUNT,DOCODE,
                                COALESCE((CASE WHEN T.TRCODE  IN (11,12) THEN 
                                (
                                    (SELECT CLIENTREF FROM LG_$FIRM$_$PERIOD$_CLFLINE WHERE SOURCEFREF = T.LOGICALREF AND MODULENR = 10 )
                                ) ELSE 0 END) ,0) CLIENTREF 
                                FROM LG_$FIRM$_$PERIOD$_KSLINES T WHERE LOGICALREF = @P1
                                
                                ", new object[] { lref_ }));


                                    {
                                        var docBody = new TOOL_WEB.TABLE();
                                        docBody.TABLENAME = "tran";
                                        docBody.COLS = null;
                                        //
                                        pGUID.Add(FORMAT(TAB_GETROW(recH, "GLOBID")));
                                        //
                                        var trcode = CASTASSHORT(TAB_GETROW(recH, "TRCODE"));
                                        var amnt = CASTASDOUBLE(TAB_GETROW(recH, "AMOUNT"));

                                        //rectype shoul be in header
                                        docBody.PROP["rectype"] = FORMAT(GET_CLOUD_REC_TYPE(tab_, trcode));

                                        docBody.PROP["guid"] = FORMAT(TAB_GETROW(recH, "GLOBID"));
                                        //docBody.PROP["dep"] = FORMAT(TAB_GETROW(recH, "DEPARTMENT"));
                                        //docBody.PROP["wh"] = FORMAT(0);


                                        docBody.PROP["recdate"] = FORMAT(TAB_GETROW(recH, "DATE_"));
                                        docBody.PROP["rectime"] = FORMAT(CASTASSHORT(TAB_GETROW(recH, "HOUR_")) * 100 + CASTASSHORT(TAB_GETROW(recH, "MINUTE_")));

                                        docBody.PROP["markcode1"] = _CORRECT(TAB_GETROW(recH, "SPECODE"));
                                        docBody.PROP["markcode2"] = _CORRECT(TAB_GETROW(recH, "SPECODE2"));
                                        docBody.PROP["markcode3"] = _CORRECT(TAB_GETROW(recH, "SPECODE3"));

                                        docBody.PROP["prjref"] = FORMAT(TAB_GETROW(recH, "PROJECTREF"));

                                        var bodyLine = new TOOL_WEB.ROW(){
                                                //{"card1ref",FORMAT(0)},
                                                {"card2ref",FORMAT( (TAB_GETROW(recH, "CLIENTREF")))},
                                                //{"qty",FORMAT(1.0)},
                                                {"price",FORMAT(amnt)},
                                                // {"discperc",FORMAT(0.0)},
                                                // {"vat",FORMAT(0.0)},
                                                {"textf1",_CORRECT(TAB_GETROW(recH, "CUSTTITLE"))},
                                                {"textf2",_CORRECT(TAB_GETROW(recH, "LINEEXP"))},

                                                //{"rectype", FORMAT(TAB_GETROW(recH, "TRCODE"))},
                                                // {"guid",FORMAT(TAB_GETROW(recH, "GLOBID"))},
                                               // {"recdate", FORMAT(TAB_GETROW(recH, "DATE_"))},
                                               // {"rectime", FORMAT(CASTASSHORT(TAB_GETROW(recH, "HOUR_")) * 100 + CASTASSHORT(TAB_GETROW(recH, "MINUTE_")))},                                        
                                              //  {"markcode1", FORMAT(TAB_GETROW(recH, "SPECODE"))},
                                               // {"markcode2", FORMAT(TAB_GETROW(recH, "SPECODE2"))},
                                                {"dep", FORMAT(TAB_GETROW(recH, "DEPARTMENT"))},
                                                {"wh", FORMAT(0)},
                                                //group
                                               // {"srclref", FORMAT(0)},
                                                };

                                        //
                                        docBody.ROWS.Add(bodyLine);

                                        ds.TABLES.Add(docBody);

                                    }


                                    //{


                                    //    var lref = CASTASSTRING(TAB_GETROW(recH, "GLOBID"));
                                    //    var trcode = CASTASSHORT(TAB_GETROW(recH, "TRCODE"));
                                    //    var clref = (TAB_GETROW(recH, "CLIENTREF"));
                                    //    var date = CASTASDATE(TAB_GETROW(recH, "DATE_"));
                                    //    var hours = (CASTASINT(TAB_GETROW(recH, "HOUR_")));
                                    //    var minute = (CASTASINT(TAB_GETROW(recH, "MINUTE_")));
                                    //    var amount = CASTASDOUBLE(TAB_GETROW(recH, "AMOUNT"));
                                    //    var text1 = CASTASSTRING(TAB_GETROW(recH, "LINEEXP"));

                                    //    //
                                    //    date = date.AddHours(hours).AddMinutes(minute);
                                    //    //
                                    //    rootDocHeader.InnerText = MY_TOOLS.JOINLISTTAB(new object[] { 
                                    //                                             ( lref),
                                    //                                             ( trcode),
                                    //                                             ( clref),
                                    //                                             ( date),

                                    //                                             ( amount),
                                    //                                             ( text1) 
                                    //                                        });



                                    //}





                                }
                                break;




                        }

                    }


                    //cards.WRITE_CARDS(pPLUGIN, root);
                }

                //  if (!hasDocs)
                //      return null;




                // var res = XMLDOCFORMAT(doc);

                //return res;
            }
        }

        class TOOL_TO_MAGNET
        {


            static string[] dataTables = new string[] { 
"LG_$FIRM$_CLCARD",
//
"LG_$FIRM$_ITEMS",
"LG_$FIRM$_PRCLIST",
"LG_$FIRM$_ITMUNITA",
"LG_$FIRM$_UNITBARCODE",
"LG_$FIRM$_INVDEF",
"LG_$FIRM$_STCOMPLN",
//
"LG_$FIRM$_$PERIOD$_ORFICHE",
"LG_$FIRM$_$PERIOD$_ORFLINE",
//
"LG_$FIRM$_$PERIOD$_INVOICE",
"LG_$FIRM$_$PERIOD$_STFICHE",
"LG_$FIRM$_$PERIOD$_STLINE",
"LG_$FIRM$_$PERIOD$_CLFLINE",
"LG_$FIRM$_$PERIOD$_KSLINES",
"LG_$FIRM$_$PERIOD$_BNFICHE",
"LG_$FIRM$_$PERIOD$_BNFLINE",
//regs
"LG_$FIRM$_$PERIOD$_GNTOTBN",
"LG_$FIRM$_$PERIOD$_BNTOTFIL",
"LG_$FIRM$_$PERIOD$_GNTOTCL",
"LG_$FIRM$_$PERIOD$_GNTOTCSH",
"LG_$FIRM$_$PERIOD$_GNTOTST",
"LG_$FIRM$_$PERIOD$_CSHTOTS",
"LG_$FIRM$_$PERIOD$_CLTOTFIL",
"LG_$FIRM$_$PERIOD$_STINVENS",
"LG_$FIRM$_$PERIOD$_STINVTOT",
"LG_$FIRM$_$PERIOD$_SRVTOT",
"LG_$FIRM$_$PERIOD$_SRVNUMS",

            };



            public static void MY_SAVE(_PLUGIN pPLUGIN, TOOL_WEB.DATASET pDATASET, bool pQuick)
            {

                //  MY_ADP.DO_CACHE(pPLUGIN);

                //TODO if MOB_USR_DATA_ID same stop import
                //var doc = new System.Xml.XmlDocument();
                //doc.LoadXml(pXml);

                //var root = doc["DATA"];

                pPLUGIN.INVOKEINBATCH(() =>
                {
                    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    if (!pQuick)
                        MY_CLEAN(pPLUGIN);//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                    var tables = new string[] { "params", "clcard", "mat", "wh", "dep", "prj", "mcode" };

                    foreach (var tabName in tables)
                    {
                        foreach (var tab in pDATASET.TABLES)
                        {
                            if (tab.TABLENAME == tabName)
                            {

                                MY_SAVE_ADP(pPLUGIN, tab, pQuick);

                            }
                        }
                    }


                    if (!pQuick)
                    {

                        var curr1 = CASTASSHORT(pPLUGIN.GETPRM("prm_magent_firm_curr1nr", ""));
                        var curr2 = CASTASSHORT(pPLUGIN.GETPRM("prm_magent_firm_curr2nr", ""));
                        var firmName = CASTASSTRING(pPLUGIN.GETPRM("prm_magent_firm_name", ""));

                        if (curr1 > 0 || curr2 > 0)
                        {

                            pPLUGIN.SQL(" UPDATE L_CAPIFIRM SET LOCALCTYP=@P1,FIRMREPCURR=@P2 WHERE NR=@P3",
                                new object[] { curr1, curr2, pPLUGIN.GETSYSPRM_FIRM() });
                            pPLUGIN.SQL("UPDATE L_CAPIPERIOD SET PERLOCALCTYPE=@P1,PERREPCURR=@P2 WHERE FIRMNR = @P3 AND NR = @P4",
                                new object[] { curr1, curr2, pPLUGIN.GETSYSPRM_FIRM(), pPLUGIN.GETSYSPRM_PERIOD() });

                        }

                        try
                        {
                            if (curr1 > 0)
                            {
                                string cur1Desc_ = pPLUGIN.RESOLVESTR("[list::LIST_GEN_CURRENCY/" + FORMAT(curr1) + "]");
                                pPLUGIN.SQL("UPDATE LG_$FIRM$_KSCARD SET NAME = @P1 WHERE CODE = @P2 AND NAME = @P3",
                                   new object[] { cur1Desc_, "01", "DEFAULT" });
                            }
                        }
                        catch (Exception exc)
                        {

                        }

                        if (firmName != "")
                            pPLUGIN.SQL("UPDATE L_CAPIFIRM SET NAME=@P1 WHERE NR=@P2", new object[] { firmName, pPLUGIN.GETSYSPRM_FIRM() });

                    }


                    //  MY_FIX_SEQ(pPLUGIN);

                    //  MY_FINISH_IMPORT(pPLUGIN);

                }
                );

            }


            //public static void MY_SAVE_QUICK(_PLUGIN pPLUGIN, TOOL_WEB.DATASET pDATASET)
            //{

            //    MY_ADP.DO_CACHE(pPLUGIN);

            //    //TODO if MOB_USR_DATA_ID same stop import
            //    // var doc = new System.Xml.XmlDocument();
            //    // doc.LoadXml(pXml);

            //    // var root = doc["DATA"];

            //    pPLUGIN.INVOKEINBATCH(() =>
            //    {

            //        foreach (var tabName in new string[] { "mat" })
            //        {
            //            foreach (var tab in pDATASET.TABLES)
            //            {
            //                if (tab.TABLENAME == tabName)
            //                {

            //                    MY_SAVE_ADP_QUICK(pPLUGIN, tab);

            //                }
            //            }
            //        }



            //    });

            //}

            //static void MY_FINISH_IMPORT(_PLUGIN pPLUGIN)
            //{
            //    //mark as unchanged
            //    foreach (var tab in new string[]{
            //        "LG_$FIRM$_ITEMS",
            //        "LG_$FIRM$_PRCLIST",
            //        "LG_$FIRM$_CLCARD"
            //    })
            //    {
            //        // pPLUGIN.SQL("UPDATE " + tab + " SET CAPIBLOCK_CREATEDBY = -1, CAPIBLOCK_MODIFIEDBY = -1");
            //        pPLUGIN.SQL("UPDATE " + tab + " SET RECSTATUS = 0 WHERE RECSTATUS != 0");

            //    }
            //}


            static void MY_CLEAN_SEQ(_PLUGIN pPLUGIN)
            {

                var sql = "";

                foreach (var table in dataTables)
                {
                    sql = sql + "\n" + "delete from " + table + ";";

                }

                pPLUGIN.SQL(sql);
            }

            static void MY_CLEAN(_PLUGIN pPLUGIN)
            {

                var sql = " ";

                foreach (var table in dataTables)
                {
                    sql = sql + "\n" + "delete from " + table + ";";

                }

                pPLUGIN.SQL(@" 
delete from L_FIRMPARAMS where CODE like 'prm_magent_%' and FIRMNR = $FIRM$; 
delete from L_CAPIDEPT where FIRMNR = $FIRM$;
delete from L_CAPIWHOUSE where FIRMNR = $FIRM$;
");
                pPLUGIN.SQL(sql);
            }

            //static void MY_SAVE_NODE(_PLUGIN pPLUGIN, System.Xml.XmlNode pNode, string pNodeCode)
            //{
            //    var item = pNode[pNodeCode];

            //    if (item == null)
            //        return;

            //    var dataText = item.InnerText;
            //    var cols = EXPLODELISTTAB(XMLNODEATTR(item, "ei_cols"));

            //    var dataTab = TAB_EXPLODE(dataText, cols);
            //    dataTab.TableName = pNodeCode;

            //    MY_SAVE_ADP(pPLUGIN, dataTab);
            //}

            static string generateSql(string pTable, string[] pCols, int pParamOffset = 0)
            {

                var cols = pCols;
                var listCols = string.Join(",", cols);
                var listExcluded = string.Join(",", new List<string>(cols).ConvertAll<string>((x) => { return x + " = excluded." + x; }));


                var listParam = string.Join(",", new List<int>(RANGE(pParamOffset + 1, cols.Length)).ConvertAll<string>((x) => { return "@P" + x; }));


                if (ISSQLITE())
                {
                    return @"
insert or replace  
into " + pTable + @"(" + listCols + @") values (" + listParam + @")
;
";


                }

                if (ISPOSTGRESQL())
                {
                    return @"
insert 
into " + pTable + @" as x (" + listCols + @") values (" + listParam + @")
on conflict (LOGICALREF) do update set " + listExcluded + @"
 
;
";

                }

                throw new Exception("Not supported sql");

                //                return @"
                //insert 
                //--$SL$--or replace 
                //into " + pTable + @"(" + listCols + @") values (" + listParam + @")
                //--$PG$--ON CONFLICT DO UPDATE SET " + listExcluded + @";
                //";

            }


            static void MY_SAVE_ADP(_PLUGIN pPLUGIN, TOOL_WEB.TABLE pTABLE, bool pQuick)
            {

                switch (pTABLE.TABLENAME)
                {
                    case "params":
                        {
                            //Note: always replace prm

                            //if (pQuick)
                            //    return;

                            // var cols = new string[] {  "LOGICALREF", "FIRMNR", "CODE", "VALUE" };
                            // var sql = generateSql("L_FIRMPARAMS", cols);

                            foreach (var ROW in pTABLE.ROWS)
                            {
                                //var lref = CASTASINT(ROW["lref"]);
                                // var firmnr = pPLUGIN.GETSYSPRM_FIRM();
                                var code = CASTASSTRING(ROW["code"]);
                                var val = CASTASSTRING(ROW["value"]);

                                pPLUGIN.SETPRM(code, val);

                                // pPLUGIN.SQL(sql, new object[] { /*lref,*/ firmnr, code, val });

                            }
                        }
                        break;

                    case "clcard":
                        {
                            var _data = new TOOL_WEB.TABLE();


                            if (pQuick)
                            {
                                foreach (var ROW in pTABLE.ROWS)
                                {
                                    var lref = CASTASINT(ROW["lref"]);
                                    var regbalance = CASTASDOUBLE(ROW["regbalance"]);
                                    var debit = regbalance > 0 ? regbalance : 0;
                                    var credit = regbalance < 0 ? -regbalance : 0;


                                    var hasRec = ISTRUE(pPLUGIN.SQLSCALAR(@"
select 1 where 
exists(select 1 from LG_$FIRM$_$PERIOD$_GNTOTCL where CARDREF = @P1 and TOTTYP = @P2 and (DEBIT BETWEEN (@P3-0.01) and (@P3+0.01)) and (CREDIT BETWEEN (@P4-0.01) and (@P4+0.01)))
", new object[] { lref, 1, debit, credit }));

                                    if (!hasRec)
                                        _data.ROWS.Add(ROW);
                                }
                            }
                            else
                                _data = pTABLE;





                            foreach (var ROW in _data.ROWS)
                            {

                                var lref = CASTASINT(ROW["lref"]);
                                var cardtype = 3;
                                var code = CASTASSTRING(ROW["code"]);
                                var title = CASTASSTRING(ROW["title"]);
                                var markcode1 = CASTASSTRING(ROW["markcode1"]);
                                var markcode2 = CASTASSTRING(ROW["markcode2"]);
                                var regbalance = CASTASDOUBLE(ROW["regbalance"]);
                                var debit = regbalance > 0 ? regbalance : 0;
                                var credit = regbalance < 0 ? -regbalance : 0;



                                var cols = new string[] { "LOGICALREF", "CARDTYPE", "CODE", "DEFINITION_", "SPECODE", "SPECODE2" };
                                var sql = generateSql("LG_$FIRM$_CLCARD", cols);

                                var cols_balance = new string[] { "LOGICALREF", "CARDREF", "TOTTYP", "DEBIT", "CREDIT" };
                                var sql_balance = generateSql("LG_$FIRM$_$PERIOD$_GNTOTCL", cols_balance, cols.Length);





                                pPLUGIN.SQL(
                                    sql + sql_balance,
                                    new object[] {
                                        //
                                        lref, cardtype, code, title, markcode1, markcode2 ,
                                        //
                                        lref,lref, 1, debit, credit
                                    });





                            }
                        }
                        break;

                    case "mat":
                        {


                            var mainUnitSet = pPLUGIN.SQLSCALAR(
                           @"
select 
--$MS$--TOP(1) 
LOGICALREF from LG_$FIRM$_UNITSETF WITH(NOLOCK) where CARDTYPE = 5 order by LOGICALREF asc
--$PG$--LIMIT 1
--$SL$--LIMIT 1
", null);

                            var mainUnitSetItem = pPLUGIN.SQLSCALAR(
                                   @"
select 
--$MS$--TOP(1) 
LOGICALREF from LG_$FIRM$_UNITSETL WITH(NOLOCK) where UNITSETREF = @P1 order by LOGICALREF asc
--$PG$--LIMIT 1
--$SL$--LIMIT 1
", new object[] { mainUnitSet });

                            //mainUnitSet = 0;
                            //mainUnitSetItem = 0;

                            var _data = new TOOL_WEB.TABLE();

                            //string _sqlText_cached = null;

                            if (pQuick)
                            {
                                var perOnceRecsCheck = 13;//optimal
                                var cols = new string[] { "LREF", "PRC0", "PRC1", 
                                    //"ONHALL", "ONH0", 
                                    "INDX" };

                                for (int i = 0; i < pTABLE.ROWS.Count; ++i)
                                {
                                    var _dataTmp = new TOOL_WEB.TABLE();
                                    var args = new List<object>();

                                    for (var count = 0; i < pTABLE.ROWS.Count && count < perOnceRecsCheck; ++count, ++i)
                                    {


                                        var ROW = pTABLE.ROWS[i];

                                        //if price exists
                                        var lref = CASTASINT(ROW["lref"]);
                                        var regonhandall = CASTASDOUBLE(ROW["regonhandall"]);
                                        var regonhandagent = CASTASDOUBLE(ROW["regonhandagent"]);
                                        var price0 = CASTASDOUBLE(ROW["price0"]);
                                        var price1 = CASTASDOUBLE(ROW["price1"]);
                                        args.AddRange(new object[] { 
                                                lref, 
                                                 price0,price1, 
                                                 i,
                                                //regonhandall, regonhandagent 
                                                });

                                    }

                                    string sql_ = "";

                                    //var mostUsedSql = (args.Count == perOnceRecsCheck * cols.Length);


                                    //if (mostUsedSql && _sqlText_cached != null)
                                    //{
                                    //    sql_ = _sqlText_cached;
                                    //}
                                    //else
                                    //{

                                    var info = new StringBuilder();
                                    var shift = 0;

                                    while (shift < args.Count)
                                    {

                                        if (shift > 0)
                                            info.Append("\n union all \n");

                                        info.Append("select ");

                                        for (int col = 0; col < cols.Length; ++col)
                                        {
                                            info.Append((col > 0 ? "," : "") + "@P" + (++shift) + " as " + (cols[col]));
                                        }

                                    }


                                    sql_ = @"
with INFO as 
(" +
info.ToString() +
@"
)
select I.INDX from INFO as I where
(
   (not exists(select 1 from LG_$FIRM$_ITEMS as I where I.LOGICALREF = I.LREF))
or (I.PRC0 > 0.001 and not exists(select 1 from LG_$FIRM$_PRCLIST as L where L.CARDREF = I.LREF and L.PTYPE = 1 and (L.PRICE between I.PRC0-0.001 and I.PRC0+0.001) ))
or (I.PRC1 > 0.001 and not exists(select 1 from LG_$FIRM$_PRCLIST as L where L.CARDREF = I.LREF and L.PTYPE = 2 and (L.PRICE between I.PRC1-0.001 and I.PRC1+0.001) ))
);";


                                    //    if (_sqlText_cached == null && mostUsedSql)
                                    //        _sqlText_cached = sql_;

                                    //}


                                    var _res = pPLUGIN.SQL(sql_, args.ToArray());

                                    //or (I.ONHALL > 0.001 and not exists(select 1 from LG_$FIRM$_$PERIOD$_GNTOTST as G where G.STOCKREF = I.LREF and G.INVENNO = -1 and (G.ONHAND between I.ONHALL-0.001 and I.ONHALL+0.001) ))
                                    //or (I.ONH0 > 0.001 and not exists(select 1 from LG_$FIRM$_$PERIOD$_GNTOTST as G where G.STOCKREF = I.LREF and G.INVENNO = 0 and (G.ONHAND between I.ONH0-0.001 and I.ONH0+0.001) )) 


                                    if (_res.Rows.Count > 0)
                                        foreach (DataRow itm in _res.Rows)
                                            _data.ROWS.Add(pTABLE.ROWS[CASTASINT(itm["INDX"])]);





                                }


                            }
                            else
                                _data = pTABLE;




                            foreach (var ROW in _data.ROWS)
                            {
                                var lref = CASTASINT(ROW["lref"]);
                                var code = CASTASSTRING(ROW["code"]);
                                var title = CASTASSTRING(ROW["title"]);
                                var markcode1 = CASTASSTRING(ROW["markcode1"]);
                                var markcode2 = CASTASSTRING(ROW["markcode2"]);

                                //var regonhand = CASTASDOUBLE(ROW["regonhand"]);
                                var regonhandall = CASTASDOUBLE(ROW["regonhandall"]);
                                var regonhandagent = CASTASDOUBLE(ROW["regonhandagent"]);
                                var price0 = CASTASDOUBLE(ROW["price0"]);
                                var price1 = CASTASDOUBLE(ROW["price1"]);
                                var cardtype = 1;
                                //

                                //
                                var args = new List<object>();
                                var offset = 0;
                                var sql = "";



                                //
                                var cols_mat = new string[] { "LOGICALREF", "CARDTYPE", "CODE", "NAME", "SPECODE", "SPECODE2", "UNITSETREF" };
                                var sql_mat = generateSql("LG_$FIRM$_ITEMS", cols_mat, offset);
                                offset += cols_mat.Length;
                                args.AddRange(new object[] { lref, cardtype, code, title, markcode1, markcode2, mainUnitSet });
                                sql += sql_mat;
                                //
                                var cols_itmunita = new string[] { "LOGICALREF", "ITEMREF", "LINENR", "UNITLINEREF" };//mainUnitSetItem
                                var sql_itmunita = generateSql("LG_$FIRM$_ITMUNITA", cols_itmunita, offset);
                                offset += cols_itmunita.Length;
                                args.AddRange(new object[] { lref, lref, 1, mainUnitSetItem, });
                                sql += sql_itmunita;
                                //
                                if (price0 > 0.001)
                                {
                                    var cols_price0 = new string[] { "LOGICALREF", "CARDREF", "PTYPE", "PRICE", "UOMREF" };//mainUnitSetItem
                                    var sql_price0 = generateSql("LG_$FIRM$_PRCLIST", cols_price0, offset);
                                    offset += cols_price0.Length;
                                    args.AddRange(new object[] { -lref, lref, 1, price0, mainUnitSetItem, });
                                    sql += sql_price0;
                                }
                                //
                                if (price1 > 0.001)
                                {
                                    var cols_price1 = new string[] { "LOGICALREF", "CARDREF", "PTYPE", "PRICE", "UOMREF" };
                                    var sql_price1 = generateSql("LG_$FIRM$_PRCLIST", cols_price1, offset);
                                    offset += cols_price1.Length;
                                    args.AddRange(new object[] { lref, lref, 2, price1, mainUnitSetItem, });
                                    sql += sql_price1;
                                }
                                //
                                if (ABS(regonhandall) > 0.01)
                                {
                                    var cols_regonhandall = new string[] { "LOGICALREF", "STOCKREF", "INVENNO", "ONHAND" };
                                    var sql_regonhandall = generateSql("LG_$FIRM$_$PERIOD$_GNTOTST", cols_regonhandall, offset);
                                    offset += cols_regonhandall.Length;
                                    args.AddRange(new object[] { -lref, lref, -1, regonhandall, });
                                    sql += sql_regonhandall;
                                }
                                //
                                if (ABS(regonhandagent) > 0.01)
                                {
                                    var cols_regonhandagent = new string[] { "LOGICALREF", "STOCKREF", "INVENNO", "ONHAND" };
                                    var sql_regonhandagent = generateSql("LG_$FIRM$_$PERIOD$_GNTOTST", cols_regonhandagent, offset);
                                    offset += cols_regonhandagent.Length;
                                    args.AddRange(new object[] { lref, lref, 0, regonhandagent, });
                                    sql += sql_regonhandagent;

                                }


                                pPLUGIN.SQL(sql, args.ToArray());




                            }



                        }
                        break;



                    case "wh":
                        {
                            // if (pQuick)
                            //     return;
                            var cols = new string[] { "LOGICALREF", "FIRMNR", "NR", "NAME" };
                            var sql = generateSql("L_CAPIWHOUSE", cols);
                            foreach (var ROW in pTABLE.ROWS)
                            {
                                var lref = CASTASINT(ROW["lref"]);
                                var firmnr = pPLUGIN.GETSYSPRM_FIRM();
                                var nr = CASTASSHORT(ROW["nr"]);
                                var title = CASTASSTRING(ROW["title"]);
                                pPLUGIN.SQL(sql, new object[] { lref, firmnr, nr, title });
                            }
                        }
                        break;
                    case "dep":
                        {
                            // if (pQuick)
                            //     return;
                            var cols = new string[] { "LOGICALREF", "FIRMNR", "NR", "NAME" };
                            var sql = generateSql("L_CAPIDEPT", cols);
                            foreach (var ROW in pTABLE.ROWS)
                            {
                                var lref = CASTASINT(ROW["lref"]);
                                var firmnr = pPLUGIN.GETSYSPRM_FIRM();
                                var nr = CASTASSHORT(ROW["nr"]);
                                var title = CASTASSTRING(ROW["title"]);
                                pPLUGIN.SQL(sql, new object[] { lref, firmnr, nr, title });
                            }
                        }
                        break;
                    case "prj":
                        {
                            // if (pQuick)
                            //     return;
                            var cols = new string[] { "LOGICALREF", "CODE", "NAME" };
                            var sql = generateSql("LG_$FIRM$_PROJECT", cols);
                            foreach (var ROW in pTABLE.ROWS)
                            {
                                var lref = CASTASINT(ROW["lref"]);

                                var code = CASTASSTRING(ROW["code"]);
                                var title = CASTASSTRING(ROW["title"]);

                                pPLUGIN.SQL(sql, new object[] { lref, code, title });
                            }
                        }
                        break;
                    case "mcode":
                        {

                            var map = new Dictionary<string, short>() { 

                            {"adp.erp.mm.rec.mat",1},
                            {"adp.erp.mm.doc.slip.",2},
                            {"adp.erp.prch.doc.inv.",22},
                            {"adp.erp.sls.doc.inv.",23},

                            {"adp.erp.fin.rec.client",26},
                            {"adp.erp.fin.doc.bankacc.",43},
                            {"adp.erp.fin.doc.cash.11",44}, 
                            {"adp.erp.fin.doc.cash.12",44}, 
                            {"adp.erp.fin.doc.cash.",40},//pure cash
                            {"adp.erp.fin.doc.client.",44},
 
                            };
                            // if (pQuick)
                            //    return;
                            var cols = new string[] { "LOGICALREF", "CODETYPE", "SPECODETYPE", "SPECODE", "DEFINITION_" };
                            var sql = generateSql("LG_$FIRM$_SPECODES", cols);
                            foreach (var ROW in pTABLE.ROWS)
                            {
                                var lref = CASTASINT(ROW["lref"]);

                                var code = CASTASSTRING(ROW["code"]);
                                var title = CASTASSTRING(ROW["title"]);

                                var rectype = CASTASSTRING(ROW["rectype"]);
                                short codeType = 0;
                                short speCodeType = 0;

                                if (rectype.EndsWith("markcode1"))
                                    codeType = 1;
                                else
                                    if (rectype.EndsWith("markcode2"))
                                        codeType = 502;
                                    else
                                        if (rectype.EndsWith("markcode3"))
                                            codeType = 503;

                                foreach (var k in map.Keys)
                                    if (rectype.StartsWith(k))
                                    {
                                        speCodeType = map[k];
                                        break;
                                    }

                                if (codeType > 0 && speCodeType > 0)
                                    pPLUGIN.SQL(sql, new object[] { lref, codeType, speCodeType, code, title });
                            }
                        }
                        break;



                }

            }


            //            static void MY_SAVE_ADP_QUICK(_PLUGIN pPLUGIN, TOOL_WEB.TABLE pTABLE)
            //            {

            //                switch (pTABLE.TABLENAME)
            //                {

            //                    case "ITEMS":
            //                        {

            //                            var hasColOnhand = pTABLE.COLS.ContainsKey("regonhand");
            //                            //var hasColOnmain = pTABLE.COLS.ContainsKey("ONMAIN");
            //                            var hasColPrice0 = pTABLE.COLS.ContainsKey("price0");
            //                            var hasColPrice1 = pTABLE.COLS.ContainsKey("price1");
            //                            // var hasColPrice2 = pTABLE.COLS.ContainsKey("PRICE2");

            //                            foreach (var ROW in pTABLE.ROWS)
            //                            {

            //                                var lref = PARSEINT(CASTASSTRING(ROW["lref"]));

            //                                if (ISTRUE(pPLUGIN.SQLSCALAR("SELECT 1 FROM LG_$FIRM$_ITEMS WHERE LOGICALREF = @P1", new object[] { lref })))
            //                                {
            //                                    var valOnhand = (hasColOnhand ? CASTASDOUBLE(ROW["regonhand"]) : 0.0);
            //                                    // var valOnmain = (hasColOnmain ? CASTASDOUBLE(TAB_GETROW(row, "ONMAIN")) : 0.0);
            //                                    var valPrice0 = (hasColPrice0 ? CASTASDOUBLE(ROW["price0"]) : 0.0);
            //                                    var valPrice1 = (hasColPrice1 ? CASTASDOUBLE(ROW["price1"]) : 0.0);
            //                                    //var valPrice2 = (hasColPrice2 ? CASTASDOUBLE(ROW[ "PRICE2"]) : 0.0);

            //                                    // var name = CASTASSTRING(TAB_GETROW(row, "NAME"));

            //                                    pPLUGIN.SQL(@"UPDATE LG_$FIRM$_ITEMS SET 
            //
            //ONHAND = @P1,
            //--ONMAIN = @P2,
            //PRICE0 = @P3,
            //PRICE1 = @P4--,
            //--PRICE2 = @P5
            //
            // WHERE LOGICALREF = @P6 AND (
            //
            //ONHAND != @P7 OR
            //--ONMAIN != @P8 OR
            //PRICE0 != @P9 OR
            //PRICE1 != @P10 --OR
            //--PRICE2 != @P11 
            //)",

            //                                        new object[] {
            //                                            valOnhand, 0.0/*valOnmain*/, valPrice0, valPrice1,0.0, /*valPrice2,*/
            //                                            lref, 
            //                                            valOnhand, 0.0/*valOnmain*/, valPrice0, valPrice1,0.0 /*valPrice2*/
            //                                        });

            //                                    //  pPLUGIN.SQL(@"UPDATE LG_$FIRM$_PRCLIST SET PRICE = @P1 WHERE (CARDREF = @P2 AND PTYPE = @P3) AND (PRICE != @P1)", new object[] { valPrice1, lref, 2, valPrice1 });


            //                                }
            //                                else
            //                                {

            //                                    var tryMatRef = MY_ADP.SET_RECORD_ITEMS(pPLUGIN, new Dictionary<string, object>() { 
            //                                    {"LOGICALREF",CASTASINT(ROW[ "lref"])},
            //                                    {"CODE",CASTASSTRING(ROW[ "code"])},
            //                                    {"NAME",CASTASSTRING(ROW[ "title"])},
            //                                    {"SPECODE",CASTASSTRING(ROW[ "markcode1"])},
            //                                    {"SPECODE2",CASTASSTRING(ROW["markcode2"])},

            //                                    {"ONHAND",(hasColOnhand?CASTASDOUBLE(ROW[ "regonhand"]):0.0)},
            //                                  //  {"ONMAIN",(hasColOnmain?CASTASDOUBLE(ROW[ "ONMAIN"]):0.0)},
            //                                    {"PRICE0",(hasColPrice0?CASTASDOUBLE(ROW[ "price0"]):0.0)},
            //                                    {"PRICE1",(hasColPrice1?CASTASDOUBLE(ROW[ "price1"]):0.0)},
            //                                   // {"PRICE2",(hasColPrice2?CASTASDOUBLE(ROW[ "PRICE2"]):0.0)},


            //                                    {"CARDTYPE",(short)1} 

            //                                }, true);


            //                                    var price1 = PARSEDOUBLE(CASTASSTRING(ROW["price1"]));

            //                                    //if (!ISNUMZERO(price1))
            //                                    {
            //                                        MY_ADP.SET_RECORD_PRICE(pPLUGIN, new Dictionary<string, object>() { 

            //                                     {"CARDREF",tryMatRef},
            //                                     {"PRICE",price1},
            //                                     {"PTYPE",(short)2},

            //                                }, true);

            //                                    }

            //                                }

            //                            }
            //                        }
            //                        break;









            //                }

            //            }


        }


        class TOOL_UI
        {

            public static void ERROR(_PLUGIN pPLUGIN, string pMsg)
            {
                pPLUGIN.MSGUSERERROR(pMsg);
            }

            public static void ERROR(_PLUGIN pPLUGIN, Exception pExc)
            {

                {
                    var err = pExc as System.Net.WebException;
                    if (err != null)
                    {
                        switch (err.Status)
                        {
                            case System.Net.WebExceptionStatus.Timeout:
                            case System.Net.WebExceptionStatus.ConnectFailure:
                                ERROR(pPLUGIN, "T_MSG_ERROR_CONNECTION");
                                return;
                            case System.Net.WebExceptionStatus.ProtocolError:

                                var respHttp = err.Response as System.Net.HttpWebResponse;
                                if (respHttp != null)
                                {
                                    switch (respHttp.StatusCode)
                                    {

                                        case System.Net.HttpStatusCode.Forbidden:
                                        case System.Net.HttpStatusCode.Unauthorized:
                                            ERROR(pPLUGIN, "T_MSG_ERROR_NOT_ALLOWED (" + err.Message + ")");
                                            return;

                                    }


                                }
                                break;
                        }
                    }
                }

                ERROR(pPLUGIN, pExc.Message);
            }




            public class FormMngMat : Form
            {
                _PLUGIN PLUGIN;

                Filter filter;

                public FormMngMat(_PLUGIN pPLUGIN, Filter pFilter = null)
                {
                    PLUGIN = pPLUGIN;
                    filter = pFilter;

                    this.Font = new Font(this.Font.FontFamily, (float)(this.Font.Size * 1.08));
                    this.Text = string.Format("{0}", PLUGIN.LANG("T_MATERIAL"));

                    init();


                    bindFilterDo();


                    setData((string)null);

                    saveData();
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

                    this.WindowState = FormWindowState.Maximized;

                    var btnClose = new Button() { Text = PLUGIN.LANG("T_CLOSE"), Image = RES_IMAGE("close_16x16"), ImageAlign = ContentAlignment.MiddleLeft, Width = 160, Dock = DockStyle.Right };
                    btnClose.Click += (s, arg) => { this.Close(); };


                    var panelRecs = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Top, AutoSize = true };
                    var panelRecsExt = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Top, AutoSize = true };
                    var panelDoc = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Bottom, AutoSize = true };



                    var panelData = new Panel() { Dock = DockStyle.Fill };
                    var panelBtn = new Panel() { Dock = DockStyle.Bottom, Height = 25 };


                    //
                    mainPanel.Controls.Add(panelData);
                    mainPanel.Controls.Add(panelRecsExt);
                    mainPanel.Controls.Add(panelRecs);

                    mainPanel.Controls.Add(panelDoc);






                    mainPanel.Controls.Add(new Panel() { Tag = "Dummmy Padding", Height = 10, Dock = DockStyle.Bottom });
                    //

                    mainPanel.Controls.Add(panelBtn);





                    panelBtn.Controls.Add(btnClose);
                    //

                    panelRecs.Controls.AddRange(
new Control[]{
                    new Label(){Text = PLUGIN.LANG("T_BARCODE"), Width = 100, TextAlign = ContentAlignment.MiddleLeft,Dock = DockStyle.Left},
                    new TextBox() { Text = "", Name = "filter_barcode",Width=300 },


                    new Button(){Text=PLUGIN.LANG("T_UP"),Image = RES_IMAGE("up_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_record_cmd/moveup",Width=140 }, 
                    new Button(){Text=PLUGIN.LANG("T_DOWN"),Image = RES_IMAGE("down_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_record_cmd/movedown",Width=140 }, 
                    new Button(){Text=PLUGIN.LANG("T_DELETE"),Image = RES_IMAGE("delete_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_record_cmd/delete",Width=140 },
                    
                     
}
                    );


                    panelRecsExt.Controls.AddRange(
new Control[]{
                    new Button(){Text=PLUGIN.LANG("T_NEW"),Image = RES_IMAGE("new_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_record_cmd/new",Width=140 },
                    new Button(){Text=PLUGIN.LANG("T_SUM"),Image = RES_IMAGE("sum_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_record_cmd/sum",Width=140 },
                    new Button(){Text=PLUGIN.LANG("T_MATERIAL"),Image = RES_IMAGE("mm_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_record_cmd/mat",Width=140 },
}
                    );




                    panelDoc.Controls.AddRange(
                  new Control[]{
                    new Button(){Text=PLUGIN.LANG("T_DOC - T_PREV"),Image = RES_IMAGE("left_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_doc_cmd/prev",Width=140 },
                    new Button(){Text=PLUGIN.LANG("T_DOC - T_NEXT"),Image = RES_IMAGE("right_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_doc_cmd/next",Width=140 },
                    new Button(){Text=PLUGIN.LANG("T_DOC - T_NEW"),Image = RES_IMAGE("new_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_doc_cmd/new",Width=140 },
                   new Label(){Width = 30},
                   new Button(){Text=PLUGIN.LANG("T_CHANGE %"),Image = RES_IMAGE("perc_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_doc_cmd/perc",Width=140 },
                   new Button(){Text=PLUGIN.LANG("T_CREATE"),Image = RES_IMAGE("mm_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_doc_cmd/update",Width=140 },
                    new Button(){Text=PLUGIN.LANG("T_INVOICE"),Image = RES_IMAGE("doc_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_doc_cmd/invoice",Width=140 },
                  }
                    );

                    //        var grid = new DataGridView();


                    var grid = createGrid("grid_recs");
                    // var gridAgr = createGrid("grid_agr");

                    grid.RowHeadersVisible = true;
                    grid.ReadOnly = false;
                    grid.EditMode = DataGridViewEditMode.EditOnEnter;
                    grid.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;

                    grid.Columns.AddRange(
                                       new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_INDEX"), DataPropertyName = "LREF", Width = 80, ReadOnly = true, Frozen = true },
                                       new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_CODE"), DataPropertyName = "CODE", Width = 120, ReadOnly = true, Frozen = true },
                                       new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_TITLE"), DataPropertyName = "TITLE", Width = 300, Frozen = true },
                                       new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_QTY"), DataPropertyName = "QTY", Width = 100, Frozen = false },
                                       new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_PRICE\nT_PURCHASE"), DataPropertyName = "PRICE0", Width = 100, Frozen = false },
                                       new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("p0+%=p1"), DataPropertyName = "CALCPERC", Width = 60, Frozen = false },
                                       new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_PRICE\nT_SALES"), DataPropertyName = "PRICE1", Width = 100, Frozen = false },

                                       new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_MARKCODE 1"), DataPropertyName = "SPECODE", Width = 120 },
                                       new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_MARKCODE 2"), DataPropertyName = "SPECODE2", Width = 120 }//,
                        //  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_INFO 1"), DataPropertyName = "TEXTF1", Width = 240 },
                        //  new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_INFO 2"), DataPropertyName = "TEXTF2", Width = 240 }

                                       );

                    foreach (var x in new DataGridView[] { grid })
                        foreach (DataGridViewColumn colObj in x.Columns)
                        {
                            colObj.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            colObj.SortMode = DataGridViewColumnSortMode.Programmatic;

                            switch (colObj.DataPropertyName)
                            {
                                case "QTY":
                                    colObj.DefaultCellStyle.Format = "#,0.##;-#,0.##;''";
                                    colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                                    colObj.DefaultCellStyle.ForeColor = Color.Green;
                                    break;
                                case "CALCPERC":
                                    colObj.DefaultCellStyle.Format = "#,0.##;-#,0.##;''";
                                    colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                                    colObj.DefaultCellStyle.ForeColor = Color.Green;
                                    break;
                                case "PRICE0":
                                case "PRICE1":
                                    colObj.DefaultCellStyle.Format = "#,0.00;-#,0.00;''";
                                    colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                                    break;
                                case "TITLE":
                                case "TEXTF1":
                                case "TEXTF2":
                                case "SPECODE":
                                case "SPECODE2":
                                    colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                                    break;
                                case "LREF":
                                    colObj.ReadOnly = true;
                                    break;
                            }
                        }






                    {
                        //grid.PreviewKeyDown += (sender, e) =>
                        //{

                        //    if (!e.Control && !e.Alt && e.Shift && e.KeyCode == Keys.Enter)
                        //    {

                        //        doGridRec("down");
                        //    }


                        //};
                        //grid.KeyDown += (sender, e) =>
                        // {

                        //     if (!e.Control && !e.Alt && e.Shift && e.KeyCode == Keys.Enter)
                        //     {
                        //         e.Handled = true;
                        //         e.SuppressKeyPress=true;

                        //         doGridRec("down");
                        //     }


                        // };

                        grid.CellParsing += (sender, e) =>
                         {

                             try
                             {
                                 if (e.ColumnIndex < 0)
                                     return;
                                 var grid_ = sender as DataGridView;
                                 if (grid_ == null)
                                     return;

                                 var type_ = e.DesiredType;

                                 if (type_ == typeof(int) || type_ == typeof(short) || type_ == typeof(double) || type_ == typeof(float))
                                 {
                                     var val = 0.0;
                                     var valRaw = (e.Value as string) ?? "";

                                     valRaw = valRaw.Replace(" ", "");
                                     var dec = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
                                     var hasDesc = false;
                                     var newRaw = "";
                                     for (var i = valRaw.Length - 1; i >= 0; --i)
                                     {
                                         var ch = valRaw[i];
                                         if (ch >= '0' && ch <= '9')
                                             newRaw = ch + newRaw;
                                         else
                                             if ((ch == '+' || ch == '-') && i == 0)
                                                 newRaw = ch + newRaw;
                                             else
                                                 if (!hasDesc && ch == dec)
                                                     newRaw = ch + newRaw;
                                     }

                                     if (newRaw == "")
                                         val = 0;
                                     else
                                         double.TryParse(newRaw, out val);

                                     e.Value = Convert.ChangeType(val, type_);
                                     e.ParsingApplied = true;

                                 }
                             }
                             catch (Exception exc)
                             {

                                 TOOL_UI.ERROR(PLUGIN, exc);
                                 PLUGIN.LOG(exc);
                             }
                         };

                        //  grid.CellValidating += (sender, e) =>
                        //{
                        //    try
                        //    {

                        //        if (e.ColumnIndex < 0)
                        //            return;

                        //        var grid_ = sender as DataGridView;
                        //        if (grid_ == null)
                        //            return;

                        //        var type_ = grid_.Columns[e.ColumnIndex].ValueType;

                        //        if (type_ == typeof(int) || type_ == typeof(short) || type_ == typeof(double) || type_ == typeof(float))
                        //        {
                        //            e.Cancel = true;
                        //            var val = 0.0;
                        //            var valRaw = e.FormattedValue as string;
                        //            if (string.IsNullOrEmpty(valRaw) || double.TryParse(valRaw, out val))
                        //            {
                        //                e.Cancel = false;
                        //            }


                        //        }



                        //    }
                        //    catch (Exception exc)
                        //    {
                        //        TOOL_UI.ERROR(PLUGIN, exc);
                        //        PLUGIN.LOG(exc);
                        //    }



                        //};


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


                            }
                            catch (Exception exc)
                            {
                                TOOL_UI.ERROR(PLUGIN, exc);
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



                            }
                            catch (Exception exc)
                            {
                                TOOL_UI.ERROR(PLUGIN, exc);
                                PLUGIN.LOG(exc);
                            }

                        };


                        grid.CellFormatting += (sender, e) =>
                        {
                            try
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


                                //if (colObj_.DataPropertyName == "CALCPERC")
                                //{
                                //    e.Value = ISNUMZERO(e.Value)?"": FORMAT(e.Value, e.CellStyle.Format) + " %";
                                //    e.FormattingApplied = true;
                                //}
                            }
                            catch (Exception exc)
                            {
                                RUNTIMELOG(exc.ToString());
                            }
                        };

                        grid.RowPostPaint += (s, e) =>
                        {
                            try
                            {
                                if (e.RowIndex >= 0)
                                {
                                    var grid_ = s as DataGridView;
                                    var c = grid_.Rows[e.RowIndex].HeaderCell;
                                    var v = c.Value as string;
                                    var valNew_ = FORMAT(e.RowIndex + 1);

                                    if (v != valNew_)
                                        c.Value = valNew_;

                                }
                            }
                            catch (Exception exc)
                            {
                                RUNTIMELOG(exc.ToString());
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

                                var lref = CASTASINT(rowData_["LREF"]);


                                var font = e.CellStyle.Font;
                                if (font == null)
                                    font = colObj_.InheritedStyle.Font;

                                var isBold = (lref <= 0);


                                if (isBold != font.Bold)
                                {

                                    e.CellStyle.Font = new Font(font, FontStyle.Bold | font.Style);

                                }

                                if (bgColor != e.CellStyle.BackColor)
                                    e.CellStyle.BackColor = bgColor;
                            }
                        };

                        /////////
                    }

                    panelData.Controls.Add(grid);



                }

                void setData(string pFileName)
                {
                    var ds = loadData(pFileName);

                    setData(ds);


                }

                void refreshInfo()
                {
                    try
                    {
                        var grid = CONTROL_SEARCH(this, "grid_recs") as DataGridView;
                        var tab = grid.DataSource as DataTable;
                        // var prop = tab.DataSet.Tables["PROP"];

                        var date = tab.DataSet.ExtendedProperties["date"];// MY_PROP(prop, "date");
                        var filename = tab.DataSet.ExtendedProperties["filename"];// MY_PROP(prop, "filename");
                        var baks = TOOL_FS.getBackups("materials");
                        var indx = Array.IndexOf(baks, filename) + 1;

                        this.Text = string.Format("{0} [{1}, {2}]", date, indx, baks.Length);
                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        TOOL_UI.ERROR(PLUGIN, exc);
                    }

                }
                void setData(DataSet pDs)
                {
                    try
                    {


                        TAB_FILLNULL(pDs);

                        saveData();

                        var grid = CONTROL_SEARCH(this, "grid_recs") as DataGridView;
                        var tab = pDs.Tables["MATS"];
                        tab.ColumnChanged += (s, e) =>
                        {
                            recalc(s as DataTable, e.Column, e.Row);
                        };
                        tab.RowChanged += (s, e) =>
                        {
                            if (e.Action == DataRowAction.Add || e.Action == DataRowAction.Delete)
                                saveData();
                        };
                        grid.DataSource = tab;
                        //



                        refreshInfo();

                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        TOOL_UI.ERROR(PLUGIN, exc);
                    }
                }

                private void recalc(DataTable pTab, DataColumn pCol, DataRow pRow)
                {
                    if (pRow == null || pRow.RowState == DataRowState.Deleted || pRow.RowState == DataRowState.Detached)
                        return;

                    try
                    {
                        var doPrice1 = (pCol.ColumnName == "CALCPERC" || pCol.ColumnName == "PRICE0");

                        if (doPrice1)
                        {
                            var p0 = CASTASDOUBLE(pRow["PRICE0"]);
                            var perc = CASTASDOUBLE(pRow["CALCPERC"]);

                            TAB_SETROW(pRow, "PRICE1", ROUND(p0 * (100.0 + perc) / 100.0, 2));

                        }



                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        //TOOL_UI.ERROR(PLUGIN, exc);
                    }
                }

                void fillMatTableCols(DataTable tab)
                {
                    TAB_ADDCOL(tab, "CODE", typeof(string));
                    TAB_ADDCOL(tab, "TITLE", typeof(string));
                    TAB_ADDCOL(tab, "SPECODE", typeof(string));
                    TAB_ADDCOL(tab, "SPECODE2", typeof(string));
                    //TAB_ADDCOL(tab, "TEXTF1", typeof(string));
                    // TAB_ADDCOL(tab, "TEXTF2", typeof(string));

                    TAB_ADDCOL(tab, "QTY", typeof(double));
                    TAB_ADDCOL(tab, "PRICE0", typeof(double));
                    TAB_ADDCOL(tab, "PRICE1", typeof(double));
                    TAB_ADDCOL(tab, "CALCPERC", typeof(double));
                    TAB_ADDCOL(tab, "LREF", typeof(int));


                }
                DataSet newData()
                {


                    var ds = new DataSet("MAGENT");
                    {
                        var tab = new DataTable("MATS");

                        fillMatTableCols(tab);

                        ds.Tables.Add(tab);
                    }
                    //{

                    //    var tab = new DataTable("PROP");

                    //    TAB_ADDCOL(tab, "CODE", typeof(string));
                    //    TAB_ADDCOL(tab, "VALUE", typeof(string));
                    //    tab.Rows.Add("filename", TOOL_FS.newfileName());
                    //    tab.Rows.Add("date", FORMAT(DateTime.Now));
                    //    ds.Tables.Add(tab);

                    //}

                    ds.ExtendedProperties["filename"] = TOOL_FS.newfileName();
                    ds.ExtendedProperties["date"] = FORMAT(DateTime.Now);

                    return ds;


                }


                protected override void OnClosing(CancelEventArgs e)
                {
                    saveData();
                }


                DataSet loadData(string pFileName)
                {
                    try
                    {

                        if ("new" == pFileName)
                            return newData();

                        if (pFileName == null)
                        {
                            //load last
                            var files = TOOL_FS.getBackups("materials");

                            if (files.Length > 0)
                                pFileName = files[files.Length - 1];
                            else
                            {
                                return newData();
                            }

                        }
                        var path = TOOL_FS.dirName("materials", pFileName);
                        var text = FILEREADTEXT(path);
                        var wrap = JSONPARSE<TOOL_WEB.DATASET>(text);
                        var ds = TOOL_WEB.DATASET.CREATE(wrap);
                        fillMatTableCols(ds.Tables["MATS"]);
                        //
                        TAB_FILLNULL(ds);
                        return ds;

                        //   var ds = DSXMLREAD(TOOL_FS.dirName("materials", pFileName));
                        //   return ds;

                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        TOOL_UI.ERROR(PLUGIN, exc);
                    }

                    return newData();
                }
                void saveData()
                {
                    try
                    {

                        var grid = CONTROL_SEARCH(this, "grid_recs") as DataGridView;
                        var data = grid.DataSource as DataTable;

                        if (data != null)
                        {
                            //var rec = data.DataSet.ExtendedProperties["filename"];// TAB_SEARCH(data.DataSet.Tables["PROP"], "CODE", "filename");
                            var file = CASTASSTRING(data.DataSet.ExtendedProperties["filename"]);// rec["VALUE"] as string;

                            var path = TOOL_FS.dirName("materials", file);

                            var wrap = TOOL_WEB.DATASET.CREATE(data.DataSet);

                            FILEWRITE(path, JSONFORMAT(wrap));

                            //  DSXMLWRITE(path, data.DataSet);
                        }

                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        TOOL_UI.ERROR(PLUGIN, exc);
                    }
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
                            case "filter_barcode":
                                {
                                    this.ActiveControl = ctrl;
                                    ctrl.Focus();
                                    //
                                    var _txtBox = ctrl as TextBox;
                                    _txtBox.KeyDown += (s, e) =>
                                    {

                                        if (!e.Control && !e.Alt && !e.Shift)
                                        {

                                            if (e.KeyCode == Keys.Enter)
                                            {
                                                e.Handled = true;
                                                e.SuppressKeyPress = true;
                                                //
                                                var _txtBox2 = s as TextBox;
                                                if (doBarcode(_txtBox2.Text))
                                                    _txtBox2.Text = "";
                                            }

                                        }
                                    };

                                }
                                break;
                            case "do_record_cmd/moveup":
                            case "do_record_cmd/movedown":
                            case "do_record_cmd/delete":
                            case "do_record_cmd/new":
                            case "do_record_cmd/sum":
                            case "do_record_cmd/mat":
                                {
                                    var btn = ctrl as Button;
                                    btn.Click += (s, e) =>
                                    {
                                        doGridRec((s as Control).Name.Split('/')[1]);
                                    };
                                }
                                break;

                            case "do_doc_cmd/prev":
                            case "do_doc_cmd/next":
                            case "do_doc_cmd/new":
                            case "do_doc_cmd/update":
                            case "do_doc_cmd/invoice":
                            case "do_doc_cmd/perc":
                                {
                                    var btn = ctrl as Button;
                                    btn.Click += (s, e) =>
                                    {
                                        doDoc((s as Control).Name.Split('/')[1]);
                                    };
                                }
                                break;



                        }


                    }
                }

                //static string MY_PROP(DataTable pTab, string pCode, string pDef = "")
                //{

                //    var rec = TAB_SEARCH(pTab, "CODE", pCode);
                //    if (rec != null)
                //        return CASTASSTRING(rec["VALUE"]);

                //    return pDef;

                //}


                void checkDataIsCommited()
                {
                    var grid = CONTROL_SEARCH(this, "grid_recs") as DataGridView;
                    var data = grid.DataSource as DataTable;


                    foreach (DataRow row in data.Rows)
                    {
                        if (ISEMPTYLREF(CASTASINT(row["LREF"])))
                        {
                            var indx = data.Rows.IndexOf(row) + 1;

                            throw new Exception("T_MSG_ERROR_NO_DATA, T_MATERIAL: T_NR = " + indx);
                        }
                    }
                }


                private void doDoc(string pCmd)
                {
                    try
                    {
                        var grid = CONTROL_SEARCH(this, "grid_recs") as DataGridView;
                        var data = grid.DataSource as DataTable;
                        var dataRec = TOOL_GRID.GET_GRID_ROW_DATA(grid);
                        var filename = CASTASSTRING(data.DataSet.ExtendedProperties["filename"]); // MY_PROP(data.DataSet.Tables["PROP"], "filename");

                        switch (pCmd)
                        {
                            case "prev":
                                {
                                    var files = TOOL_FS.getBackups("materials");
                                    var indx = Array.IndexOf<string>(files, filename);
                                    --indx;
                                    if (indx >= 0)
                                        setData(files[indx]);


                                }
                                break;
                            case "next":
                                {
                                    var files = TOOL_FS.getBackups("materials");
                                    var indx = Array.IndexOf<string>(files, filename);
                                    ++indx;
                                    if (indx > 0 && indx < files.Length)
                                        setData(files[indx]);


                                }
                                break;
                            case "new":
                                //if not empty
                                if (data.Rows.Count > 0)
                                {
                                    setData("new");
                                    saveData();

                                    refreshInfo();
                                }
                                break;
                            case "perc":
                                if (data.Rows.Count > 0)
                                {
                                    DataRow[] rows_ = PLUGIN.REF("ref.gen.double desc::" + _PLUGIN.STRENCODE("%") + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(0));
                                    if (rows_ != null && rows_.Length > 0)
                                    {
                                        var perc = CASTASDOUBLE(rows_[0]["VALUE"]);

                                        foreach (DataRow row in data.Rows)
                                        {
                                            var percOrg = CASTASDOUBLE(TAB_GETROW(row, "CALCPERC"));
                                            if (ISNUMZERO(percOrg))
                                            {
                                                TAB_SETROW(row, "CALCPERC", perc);
                                            }
                                        }
                                    }


                                }
                                break;
                            case "invoice":
                                if (data.Rows.Count > 0)
                                {


                                    checkDataIsCommited();

                                    var cmd_ = "adp.prch.doc.inv.1";
                                    if (!PLUGIN.EXEADPCMDALLOWED(cmd_, null))
                                        return;

                                    var guid_ = GUID();
                                    PLUGIN.EXECMDTEXT(cmd_ + " guid::" + guid_);

                                    foreach (Form f in Application.OpenForms)
                                    {
                                        if (ISADAPTERFORM(f))
                                        {
                                            var ds_ = GETDATASETFROMADPFORM(f);
                                            if (guid_ == CMDLINEGETARG(ds_.ExtendedProperties["_SYS_PRM_CMD_"] as string, "guid"))
                                            {
                                                var header = ds_.Tables["INVOICE"];
                                                var lines = ds_.Tables["STLINE"];

                                                var insPos = 0;
                                                foreach (DataRow row in data.Rows)
                                                {
                                                    var recNew = lines.NewRow();
                                                    lines.Rows.InsertAt(recNew, insPos);

                                                    TAB_SETROW(recNew, "STOCKREF", TAB_GETROW(row, "LREF"));
                                                    TAB_SETROW(recNew, "AMOUNT", TAB_GETROW(row, "QTY"));
                                                    TAB_SETROW(recNew, "PRICE", TAB_GETROW(row, "PRICE0"));


                                                }

                                                Close();

                                                break;
                                            }
                                        }
                                    }



                                }
                                break;
                            case "update":
                                //TODO chenge mat last edit date, for print last
                                if (data.Rows.Count > 0)
                                {

                                    //if code and lref empty incorect rec

                                    //if has lref, check if changed
                                    //if lref empty create new one by code
                                    //      add by code  if code not exists in main db then add else update
                                    //run quick import
                                    //get new lref by code


                                    var tab_ = new DataTable("mat");
                                    TAB_ADDCOL(tab_, "lref", typeof(int));
                                    TAB_ADDCOL(tab_, "code", typeof(string));
                                    TAB_ADDCOL(tab_, "title", typeof(string));
                                    TAB_ADDCOL(tab_, "markcode1", typeof(string));
                                    TAB_ADDCOL(tab_, "markcode2", typeof(string));
                                    TAB_ADDCOL(tab_, "texf1", typeof(string));
                                    TAB_ADDCOL(tab_, "texf2", typeof(string));
                                    TAB_ADDCOL(tab_, "price0", typeof(double));
                                    TAB_ADDCOL(tab_, "price1", typeof(double));

                                    var ds_ = new DataSet("mat");
                                    ds_.Tables.Add(tab_);
                                    //

                                    var indx = 0;

                                    foreach (DataRow row in data.Rows)
                                    {
                                        ++indx;

                                        var lref = CASTASINT(TAB_GETROW(row, "LREF"));
                                        var code = CASTASSTRING(TAB_GETROW(row, "CODE"));
                                        var title = CASTASSTRING(TAB_GETROW(row, "TITLE")).Trim();
                                        var markcode1 = CASTASSTRING(TAB_GETROW(row, "SPECODE"));
                                        var markcode2 = CASTASSTRING(TAB_GETROW(row, "SPECODE2"));
                                        //var textf1 = CASTASSTRING(TAB_GETROW(row, "TEXTF1"));
                                        //var textf2 = CASTASSTRING(TAB_GETROW(row, "TEXTF2"));
                                        var price0 = ROUND(CASTASDOUBLE(TAB_GETROW(row, "PRICE0")), 3);
                                        var price1 = ROUND(CASTASDOUBLE(TAB_GETROW(row, "PRICE1")), 3);


                                        var sendToCloud = false;

                                        if (ISEMPTY(code))
                                        {
                                            TOOL_GRID.SET_GRID_POSITION(grid, row, "CODE");
                                            throw new Exception("T_MSG_INVALID_MATERIAL, T_CODE : " + indx);
                                        }
                                        if (ISNUMZERO(price1))
                                        {
                                            TOOL_GRID.SET_GRID_POSITION(grid, row, "PRICE1");
                                            throw new Exception("T_MSG_INVALID_MATERIAL, T_PRICE - T_SALE : " + indx);
                                        }
                                        if (ISEMPTY(title))
                                        {
                                            TOOL_GRID.SET_GRID_POSITION(grid, row, "TITLE");
                                            throw new Exception("T_MSG_INVALID_MATERIAL, T_TITLE : " + indx);
                                        }

                                        if (ISEMPTYLREF(lref))
                                            sendToCloud = true;
                                        else
                                        {
                                            var hasMat = ISTRUE(PLUGIN.SQLSCALAR(@"
select 1 from LG_$FIRM$_ITEMS where  CODE = @P1 and NAME = @P2 and SPECODE = @P3 and SPECODE2 = @P4  
", new object[] { code, title, markcode1, markcode2 }));


                                            var dbPrice0 = CASTASDOUBLE(PLUGIN.SQLSCALAR(@"
 select L.PRICE from LG_$FIRM$_PRCLIST as L where L.CARDREF = @P1 and L.PTYPE = @P2 
", new object[] { lref, 1 }));

                                            var dbPrice1 = CASTASDOUBLE(PLUGIN.SQLSCALAR(@"
 select L.PRICE from LG_$FIRM$_PRCLIST as L where L.CARDREF = @P1 and L.PTYPE = @P2 
", new object[] { lref, 2 }));

                                            if (!hasMat || (!ISNUMEQUAL(dbPrice0, price0)) || (!ISNUMEQUAL(dbPrice1, price1)))
                                                sendToCloud = true;

                                        }


                                        if (sendToCloud)
                                        {

                                            var newRec = tab_.NewRow();

                                            TAB_SETROW(newRec, "lref", lref);
                                            TAB_SETROW(newRec, "code", LEFT(code, 100));
                                            TAB_SETROW(newRec, "title", LEFT(title, 100));
                                            TAB_SETROW(newRec, "markcode1", LEFT(markcode1, 100));
                                            TAB_SETROW(newRec, "markcode2", LEFT(markcode2, 100));

                                            TAB_SETROW(newRec, "price0", price0);
                                            TAB_SETROW(newRec, "price1", price1);

                                            tab_.Rows.Add(newRec);

                                        }
                                    }

                                    if (tab_.Rows.Count > 0)
                                    {
                                        //send data
                                        MY_TOMAIN(PLUGIN, TOOL_WEB.DATASET.CREATE(ds_));
                                        //import data
                                        MY_FROMMAIN(PLUGIN, true);
                                        //
                                        foreach (DataRow row in data.Rows)
                                        {
                                            var lref = CASTASINT(TAB_GETROW(row, "LREF"));
                                            var code = CASTASSTRING(TAB_GETROW(row, "CODE"));

                                            if (ISEMPTYLREF(lref))
                                            {
                                                lref = CASTASINT(PLUGIN.SQLSCALAR("select LOGICALREF from LG_$FIRM$_ITEMS where CODE = @P1", new object[] { code }));
                                                TAB_SETROW(row, "LREF", lref);
                                            }
                                        }

                                        checkDataIsCommited();

                                    }

                                    PLUGIN.MSGUSERINFO("T_MSG_OPERATION_OK");
                                }
                                break;
                        }

                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        TOOL_UI.ERROR(PLUGIN, exc);
                    }
                }


                private void doGridRec(string pCmd)
                {
                    try
                    {

                        var grid = CONTROL_SEARCH(this, "grid_recs") as DataGridView;
                        var tab = grid.DataSource as DataTable;
                        var dataRec = TOOL_GRID.GET_GRID_ROW_DATA(grid);
                        var activeRecLref = (dataRec != null ? CASTASINT(dataRec["lref"]) : 0);
                        var activeRecCode = (dataRec != null ? CASTASSTRING(dataRec["code"]) : "");

                        switch (pCmd)
                        {
                            case "mat":
                                {
                                    PLUGIN.REF("ref.mm.rec.mat", "CODE", activeRecCode);
                                }
                                break;
                            case "sum":
                                {
                                    var totQty = 0.0;
                                    var totPrch = 0.0;
                                    var totSls = 0.0;

                                    foreach (DataRow ROW in tab.Rows)
                                    {

                                        var qty = CASTASDOUBLE(ROW["qty"]);
                                        var price0 = CASTASDOUBLE(ROW["price0"]);
                                        var price1 = CASTASDOUBLE(ROW["price1"]);

                                        totQty += qty;
                                        totPrch += qty * price0;
                                        totSls += qty + price1;
                                    }

                                    PLUGIN.MSGUSERINFO(


 "T_QTY: " + FORMAT(totQty, "#,0.##") + "\n\n" +
 "T_PURCHASE: " + FORMAT(totPrch, "#,0.00") + "\n" +
 "T_SALE: " + FORMAT(totSls, "#,0.00") + "\n\n" +
 "T_AVG %: " + FORMAT(DIV(totSls - totPrch, totPrch) * 100, "#,0.00") + "\n" + ""
);

                                }
                                break;
                            case "new":
                                {
                                    var newCodePrefix = CASTASSTRING(PLUGIN.GETPRM("prm_magent_new_code_prefix", ""));
                                    var newCodeMethod = CASTASSTRING(PLUGIN.GETPRM("prm_magent_new_code_method", ""));

                                    switch (newCodeMethod)
                                    {
                                        case "":
                                        case "diff"://minus 20170101
                                            {
                                                DataRow[] rows_ = PLUGIN.REF("ref.gen.int desc::" + _PLUGIN.STRENCODE("T_COUNT (Max. 50)") + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(0));
                                                if (rows_ != null && rows_.Length > 0)
                                                {
                                                    var count = CASTASINT(rows_[0]["VALUE"]);
                                                    count = Math.Min(count, 50);
                                                    if (count > 0)
                                                    {
                                                        var now = DateTime.Now;


                                                        var diffMin_ = (int)(DateTime.Now - new DateTime(2000, 1, 1, 0, 0, 0)).TotalMinutes;

                                                        if (diffMin_ < 10043328)//~ 17 years
                                                            throw new Exception("Correct computer time");

                                                        var list = new List<string>();

                                                        for (var i = 1; i <= count; ++i)
                                                        {
                                                            var newCode = newCodePrefix + FORMAT(diffMin_) + FORMAT(i).PadLeft(2, '0');
                                                            //
                                                            if (barcodeExistsInDb(newCode) || barcodeExistsInGrid(newCode))
                                                                throw new Exception("Generated code exists in db or in grid: " + newCode);
                                                            else
                                                                list.Add(newCode);
                                                        }

                                                        foreach (var code in list)
                                                            doBarcode(code);


                                                    }
                                                }
                                            }
                                            break;
                                        default:
                                            throw new Exception("Undefined method [" + newCodeMethod + "] for [new_code_method]");

                                    }

                                }
                                break;
                            case "delete":
                                if (dataRec != null)
                                {
                                    if (PLUGIN.MSGUSERASK("T_MSG_COMMIT_DELETE"))
                                        tab.Rows.Remove(dataRec);

                                    dataRec = TOOL_GRID.GET_GRID_ROW_DATA(grid);
                                    TOOL_GRID.SET_GRID_POSITION(grid, dataRec, null);
                                }
                                break;
                            case "moveup":
                                if (dataRec != null)
                                {
                                    var pos = tab.Rows.IndexOf(dataRec);
                                    if (pos > 0)
                                    {
                                        var recNew = tab.NewRow();
                                        recNew.ItemArray = dataRec.ItemArray;
                                        tab.Rows.InsertAt(recNew, pos - 1);
                                        tab.Rows.Remove(dataRec);
                                        TOOL_GRID.SET_GRID_POSITION(grid, recNew, null);
                                    }

                                }
                                break;
                            case "movedown":
                                if (dataRec != null)
                                {
                                    var pos = tab.Rows.IndexOf(dataRec);
                                    if (pos < tab.Rows.Count - 1)
                                    {
                                        var recNew = tab.NewRow();
                                        recNew.ItemArray = dataRec.ItemArray;
                                        tab.Rows.InsertAt(recNew, pos + 2);
                                        tab.Rows.Remove(dataRec);
                                        TOOL_GRID.SET_GRID_POSITION(grid, recNew, null);
                                    }
                                }
                                break;
                        }

                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        TOOL_UI.ERROR(PLUGIN, exc);
                    }
                }


                private bool barcodeExistsInDb(string pBarcode)
                {
                    return ISTRUE(PLUGIN.SQLSCALAR(@"
                    select 1 from LG_$FIRM$_ITEMS as I where CODE = @P1
                    ", new object[] { pBarcode }));

                }
                private bool barcodeExistsInGrid(string pBarcode)
                {
                    var grid = CONTROL_SEARCH(this, "grid_recs") as DataGridView;
                    var tab = grid.DataSource as DataTable;
                    var exists = TAB_SEARCH(tab, "CODE", pBarcode);
                    return exists != null;
                }
                private bool doBarcode(string pBarcode)
                {
                    try
                    {

                        //1 search by barcode, if exists +1
                        //2 fill line
                        //3 add to end


                        pBarcode = pBarcode ?? "";
                        pBarcode = pBarcode.Trim();

                        if (ISEMPTY(pBarcode))
                            return true;

                        var grid = CONTROL_SEARCH(this, "grid_recs") as DataGridView;
                        var tab = grid.DataSource as DataTable;



                        //search db
                        //1
                        var resTab = PLUGIN.SQL(@"
select 
LOGICALREF,CODE,NAME,SPECODE,SPECODE2,
(select
--$MS$--TOP(1) 
L.PRICE from LG_$FIRM$_PRCLIST as L where L.CARDREF = I.LOGICALREF and L.PTYPE = 1
--$PG$--LIMIT 1
--$SL$--LIMIT 1
) PRICE0,
(select
--$MS$--TOP(1) 
L.PRICE from LG_$FIRM$_PRCLIST as L where L.CARDREF = I.LOGICALREF and L.PTYPE = 2
--$PG$--LIMIT 1
--$SL$--LIMIT 1
) PRICE1
 
 
from LG_$FIRM$_ITEMS as I where CODE = @P1
", new object[] { pBarcode });
                        var resRec = TAB_GETLASTROW(resTab);

                        if (resRec == null)
                        {
                            //add new rec as empty
                            resRec = resTab.NewRow();
                            TAB_FILLNULL(resRec);
                            TAB_SETROW(resRec, "CODE", pBarcode);
                            //
                            beepErr();

                        }
                        else
                            beepOk();

                        //2 sound
                        //if exists +1
                        {
                            var exists = TAB_SEARCH(tab, "CODE", pBarcode);
                            if (exists != null)
                            {
                                TAB_SETROW(exists, "QTY", CASTASDOUBLE(TAB_GETROW(exists, "QTY")) + 1);
                                TOOL_GRID.SET_GRID_POSITION(grid, exists, null);
                                return true;
                            }

                        }

                        //create rec
                        {

                            var newRec = tab.NewRow();
                            TAB_FILLNULL(newRec);

                            TAB_SETROW(newRec, "LREF", TAB_GETROW(resRec, "LOGICALREF"));
                            TAB_SETROW(newRec, "CODE", TAB_GETROW(resRec, "CODE"));
                            TAB_SETROW(newRec, "TITLE", TAB_GETROW(resRec, "NAME"));
                            TAB_SETROW(newRec, "SPECODE", TAB_GETROW(resRec, "SPECODE"));
                            TAB_SETROW(newRec, "SPECODE2", TAB_GETROW(resRec, "SPECODE2"));

                            // TAB_SETROW(newRec, "TEXTF1", TAB_GETROW(resRec, "TEXTF1"));
                            // TAB_SETROW(newRec, "TEXTF2", TAB_GETROW(resRec, "TEXTF2"));

                            var prc0 = CASTASDOUBLE(TAB_GETROW(resRec, "PRICE0"));
                            var prc1 = CASTASDOUBLE(TAB_GETROW(resRec, "PRICE1"));

                            //1
                            TAB_SETROW(newRec, "CALCPERC", ROUND(DIV(prc1 - prc0, prc0) * 100.0, 3));
                            //2
                            TAB_SETROW(newRec, "PRICE0", prc0);
                            //3
                            TAB_SETROW(newRec, "PRICE1", prc1);


                            TAB_SETROW(newRec, "QTY", 1);
                            //
                            tab.Rows.Add(newRec);

                            TOOL_GRID.SET_GRID_POSITION(grid, newRec, null);
                        }




                        return true;
                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        TOOL_UI.ERROR(PLUGIN, exc);
                    }
                    finally
                    {

                        //   saveData();
                    }
                    return false;
                }

                static void beepErr()
                {
                    new System.Threading.Tasks.Task(() =>
                    {
                        // if (PRM.BEEP)
                        for (var i = 0; i < 3; ++i)
                        {
                            System.Media.SystemSounds.Asterisk.Play();
                            System.Threading.Thread.Sleep(600);

                        }

                    }).Start();



                }

                static void beepOk()
                {
                    new System.Threading.Tasks.Task(() =>
                    {

                        System.Media.SystemSounds.Beep.Play();

                    }).Start();
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

                    grid.RowHeadersWidth = 60;
                    return grid;


                }





                public class Filter
                {


                }


            }




        }
        //class MY_TOOLS
        //{


        //    public static string JOINLISTTAB(object[][] pArrs)
        //    {
        //        var sb = new StringBuilder();

        //        for (int r = 0; r < pArrs.Length; ++r)
        //        {
        //            if (r != 0)
        //                sb.Append('\n');
        //            sb.Append(JOINLISTTAB(pArrs[r]));
        //        }

        //        return sb.ToString();
        //    }
        //    public static string JOINLISTTAB(object[] pArr)
        //    {
        //        var sb = new StringBuilder();

        //        for (int i = 0; i < pArr.Length; ++i)
        //        {

        //            if (i != 0)
        //                sb.Append('\t');

        //            sb.Append(FORMAT(pArr[i]).Replace('\t', '_'));

        //        }

        //        return sb.ToString();
        //    }

        //}

        //        class MY_ADP
        //        {
        //            /*



        //             */

        //            static Dictionary<string, DataTable> modelsMap = new Dictionary<string, DataTable>();

        //            public static object mainUnitSet = null;
        //            public static object mainUnitSetItem = null;


        //            public static void DO_CACHE(_PLUGIN pPLUGIN)
        //            {

        //                modelsMap.Clear();

        //                mainUnitSet = pPLUGIN.SQLSCALAR(
        //                          @"
        //select 
        //--$MS$--TOP(1) 
        //LOGICALREF from LG_$FIRM$_UNITSETF WITH(NOLOCK) where CARDTYPE = 5 order by LOGICALREF asc
        //--$PG$--LIMIT 1
        //--$SL$--LIMIT 1
        //", null);

        //                mainUnitSetItem = pPLUGIN.SQLSCALAR(
        //                     @"
        //select 
        //--$MS$--TOP(1) 
        //LOGICALREF from LG_$FIRM$_UNITSETL WITH(NOLOCK) where UNITSETREF = @P1 order by LOGICALREF asc
        //--$PG$--LIMIT 1
        //--$SL$--LIMIT 1
        //", new object[] { mainUnitSet });


        //            }

        //            static DataTable GET_MODEL(_PLUGIN pPLUGIN, string pTABLE_SHORTNAME)
        //            {


        //                if (modelsMap.ContainsKey(pTABLE_SHORTNAME))
        //                    return modelsMap[pTABLE_SHORTNAME];

        //                var tab = pPLUGIN.GETTABLEMODEL(pTABLE_SHORTNAME);

        //                modelsMap[pTABLE_SHORTNAME] = tab;

        //                return tab;
        //            }



        //            public static object SET_RECORD(_PLUGIN pPLUGIN, string pTABLE, string pTABLE_SHORTNAME, Dictionary<string, object> pROW, bool pADD)
        //            {

        //                if (pADD)
        //                {
        //                    if (!pROW.ContainsKey("LOGICALREF"))
        //                    {
        //                        pROW["LOGICALREF"] = pPLUGIN.GETSEQ(pTABLE); ;
        //                    }





        //                    var MODEL = GET_MODEL(pPLUGIN, pTABLE_SHORTNAME);

        //                    var newROW = MODEL.NewRow();
        //                    TAB_FILLNULL(newROW);

        //                    foreach (var k in pROW.Keys)
        //                        TAB_SETROW(newROW, k, pROW[k]);

        //                    var colsList = new List<string>();
        //                    var argsNamesList = new List<string>();
        //                    var argsValsList = new List<object>();

        //                    for (int i = 0; i < MODEL.Columns.Count; ++i)
        //                    {
        //                        DataColumn c = MODEL.Columns[i];

        //                        colsList.Add(c.ColumnName);
        //                        argsNamesList.Add("@P" + FORMAT(i + 1));
        //                        var val = newROW[c];

        //                        if (c.ColumnName.StartsWith("SPECODE"))
        //                            val = LEFT(val.ToString(), 35);

        //                        if (c.ColumnName == ("NAME") || c.ColumnName == ("DEFINITION_"))
        //                            val = LEFT(val.ToString(), 50);


        //                        argsValsList.Add(val);

        //                    }
        //                    var sql = "insert into " + pTABLE + "(\n" + JOINLIST(colsList.ToArray()) + "\n) values(\n" + JOINLIST(argsNamesList.ToArray()) + "\n)";

        //                    pPLUGIN.SQL(sql, argsValsList.ToArray());

        //                }
        //                else //update
        //                {


        //                    if (!pROW.ContainsKey("LOGICALREF"))
        //                        throw new Exception("Cant update record without LOGICALREF value");

        //                    var colsList = new List<string>(pROW.Keys);
        //                    var argsNamesList = new List<string>();
        //                    var argsValsList = new List<object>();

        //                    for (var i = 0; i < colsList.Count; ++i)
        //                    {

        //                        if (TAB_ISCOLFULLNAME(colsList[i]))
        //                        {
        //                            colsList.RemoveAt(i);
        //                            --i;
        //                        }

        //                    }

        //                    colsList.Remove("LOGICALREF");


        //                    for (int i = 0; i < colsList.Count; ++i)
        //                    {

        //                        var colName = colsList[i];

        //                        var val = pROW[colName];

        //                        if (colName.StartsWith("SPECODE"))
        //                            val = LEFT(val.ToString(), 35);

        //                        if (colName == ("NAME"))
        //                            val = LEFT(val.ToString(), 50);

        //                        if (colName == ("DEFINITION_"))
        //                            val = LEFT(val.ToString(), 50);

        //                        argsNamesList.Add(colName + " = @P" + FORMAT(i + 1));
        //                        argsValsList.Add(val);

        //                    }


        //                    argsValsList.Add(pROW["LOGICALREF"]);
        //                    var lrefArgName = "@P" + FORMAT(argsValsList.Count);


        //                    var sql = "update " + pTABLE + " set " + JOINLIST(argsNamesList.ToArray()) + " where LOGICALREF = " + lrefArgName;


        //                    pPLUGIN.SQL(sql, argsValsList.ToArray());


        //                }

        //                return pROW["LOGICALREF"];
        //            }

        //            public static object SET_RECORD_CLCARD(_PLUGIN pPLUGIN, Dictionary<string, object> pROW, bool pADD)
        //            {

        //                if (pADD)
        //                {

        //                    if (!pROW.ContainsKey("CARDTYPE"))
        //                        pROW["CARDTYPE"] = 3;

        //                    //pROW["SALESBRWS"] = (short)1;
        //                    //pROW["PURCHBRWS"] = (short)1;
        //                    //pROW["IMPBRWS"] = (short)1;
        //                    //pROW["EXPBRWS"] = (short)1;
        //                    //pROW["FINBRWS"] = (short)1;
        //                }

        //                //pPLUGIN.INVOKEINBATCH
        //                return SET_RECORD(pPLUGIN, "LG_$FIRM$_CLCARD", "CLCARD", pROW, pADD);
        //            }

        //            public static object SET_RECORD_WH(_PLUGIN pPLUGIN, Dictionary<string, object> pROW, bool pADD)
        //            {
        //                if (pADD)
        //                {
        //                    pROW["FIRMNR"] = pPLUGIN.GETSYSPRM_FIRM();
        //                }
        //                return SET_RECORD(pPLUGIN, "L_CAPIWHOUSE", "WHOUSE", pROW, pADD);
        //            }

        //            public static object SET_RECORD_DEP(_PLUGIN pPLUGIN, Dictionary<string, object> pROW, bool pADD)
        //            {
        //                if (pADD)
        //                {
        //                    pROW["FIRMNR"] = pPLUGIN.GETSYSPRM_FIRM();
        //                }
        //                return SET_RECORD(pPLUGIN, "L_CAPIDEPT", "DEPT", pROW, pADD);
        //            }
        //            public static object SET_RECORD_ITEMS(_PLUGIN pPLUGIN, Dictionary<string, object> pROW, bool pADD)
        //            {
        //                if (pADD)
        //                {
        //                    if (!pROW.ContainsKey("CARDTYPE"))
        //                        pROW["CARDTYPE"] = 1;

        //                    var unitsetRef = pPLUGIN.SQLSCALAR(
        //                        @"
        //select 
        //--$MS$--TOP(1) 
        //LOGICALREF from LG_$FIRM$_UNITSETF WITH(NOLOCK) where CARDTYPE = 5 order by LOGICALREF asc
        //--$PG$--LIMIT 1
        //--$SL$--LIMIT 1
        //
        //", null);

        //                    var unitsetLineRef = pPLUGIN.SQLSCALAR(
        //                        @"
        //select 
        //--$MS$--TOP(1) 
        //LOGICALREF from LG_$FIRM$_UNITSETL WITH(NOLOCK) where UNITSETREF = @P1 order by LOGICALREF asc
        //--$PG$--LIMIT 1
        //--$SL$--LIMIT 1
        //
        //", new object[] { unitsetRef });

        //                    //
        //                    pROW["UNITSETREF"] = unitsetRef;
        //                    //
        //                    //pROW["MTRLBRWS"] = (short)1;
        //                    //pROW["PURCHBRWS"] = (short)1;
        //                    //pROW["SALESBRWS"] = (short)1;
        //                    //pROW["AUTOINCSL"] = (short)1;
        //                    //pROW["DIVLOTSIZE"] = (short)1;
        //                    //pROW["DEPRTYPE"] = (short)1;
        //                    //pROW["REVALFLAG"] = (short)1;
        //                    //pROW["REVDEPRFLAG"] = (short)1;
        //                    //pROW["DEPRTYPE2"] = (short)1;
        //                    //pROW["DISTLOTUNITS"] = (short)1;
        //                    //pROW["COMBLOTUNITS"] = (short)1;
        //                    //pROW["EXTACCESSFLAGS"] = (short)3;
        //                    //pROW["EXTCARDFLAGS"] = (short)63;
        //                    //


        //                    //pPLUGIN.INVOKEINBATCH((s, a) =>
        //                    //{

        //                    SET_RECORD(pPLUGIN, "LG_$FIRM$_ITEMS", "ITEMS", pROW, pADD);

        //                    SET_RECORD(pPLUGIN, "LG_$FIRM$_ITMUNITA", "ITMUNITA", new Dictionary<string, object>() { 

        //                            {"ITEMREF",pROW["LOGICALREF"]},
        //                             {"LINENR",(short)1},
        //                              {"UNITLINEREF",unitsetLineRef},
        //                         {"CONVFACT1",(short)1},
        //                         {"CONVFACT2",(short)1},
        //                       // {"PURCHCLAS",(short)1},
        //                        //{"MTRLCLAS",(short)1},
        //                       // {"SALESCLAS",(short)1},

        //                        }, pADD);

        //                    //}, null);


        //                }
        //                else
        //                {

        //                    pROW.Remove("UNITSETREF");

        //                    //pPLUGIN.INVOKEINBATCH
        //                    SET_RECORD(pPLUGIN, "LG_$FIRM$_ITEMS", "ITEMS", pROW, pADD);
        //                }

        //                return pROW["LOGICALREF"];
        //            }

        //            public static object SET_RECORD_PRICE(_PLUGIN pPLUGIN, Dictionary<string, object> pROW, bool pADD)
        //            {
        //                if (pADD)
        //                {
        //                    //CARDREF,PRICE

        //                    if (!pROW.ContainsKey("PTYPE"))
        //                        pROW["PTYPE"] = 1;

        //                    if (!pROW.ContainsKey("UOMREF"))
        //                        pROW["UOMREF"] = mainUnitSetItem;

        //                    if (!pROW.ContainsKey("BEGDATE"))
        //                        pROW["BEGDATE"] = pPLUGIN.GETSYSPRM_PERIODBEG();

        //                    if (!pROW.ContainsKey("ENDDATE"))
        //                        pROW["ENDDATE"] = pPLUGIN.GETSYSPRM_PERIODEND();

        //                    if (!pROW.ContainsKey("LOGICALREF"))
        //                        pROW["LOGICALREF"] = pPLUGIN.GETSEQ("LG_$FIRM$_PRCLIST");

        //                    if (!pROW.ContainsKey("CODE"))
        //                        pROW["CODE"] = FORMAT(pROW["LOGICALREF"]).PadLeft(10, '0');

        //                }

        //                return SET_RECORD(pPLUGIN, "LG_$FIRM$_PRCLIST", "PRCLIST", pROW, pADD);
        //            }

        //        }

        //        class MY_CARDS_EXPORT
        //        {

        //            class CARDS
        //            {

        //                public CARDS(string pCode)
        //                {
        //                    CODE = pCode;
        //                }

        //                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        //                List<object> lrefs = new List<object>();


        //                string CODE;


        //                public bool HASLREF(object pLref)
        //                {
        //                    return (lrefs.Contains(pLref));
        //                }


        //                public bool ADD(object pLref, Dictionary<string, object> pRec)
        //                {
        //                    if (HASLREF(pLref))
        //                        return false;

        //                    lrefs.Add(pLref);
        //                    rows.Add(pRec);

        //                    return true;
        //                }

        //                //public void WRITE_CARDS(System.Xml.XmlElement pRoot)
        //                //{
        //                //    var doc = pRoot.OwnerDocument;
        //                //    var root = pRoot;


        //                //    //cl
        //                //    foreach (var dic in rows)
        //                //    {
        //                //        //var cardRoot = doc.CreateElement("CARD");
        //                //        //XMLNODEATTR(cardRoot, "ei_code", "ADP_" + CODE);

        //                //        var cardRoot = doc.CreateElement(CODE);
        //                //        foreach (var key in dic.Keys)
        //                //            XMLNODEATTR(cardRoot, key, FORMAT(dic[key]));

        //                //        root.AppendChild(cardRoot);

        //                //    }




        //                //}
        //            }

        //            short agentNr;


        //            CARDS CLCARD = new CARDS("CLCARD");
        //            CARDS PRCLIST = new CARDS("PRCLIST");
        //            CARDS ITEMS = new CARDS("ITEMS");



        //            public MY_CARDS_EXPORT(short pAgentNr)
        //            {
        //                agentNr = pAgentNr;
        //            }


        //            public void ADD_PRICE(_PLUGIN pPLUGIN, object pMatRef)
        //            {

        //                var priceTab =
        //                   pPLUGIN.SQL("select LOGICALREF,PRICE,PTYPE from LG_$FIRM$_PRCLIST WHERE CARDREF=@P1 AND PTYPE IN (1,2) ORDER BY LOGICALREF ASC", new object[] { pMatRef });

        //                foreach (DataRow priceRec in priceTab.Rows)
        //                {
        //                    var lref = (TAB_GETROW(priceRec, "LOGICALREF"));//for dublicate check
        //                    var cardref = pMatRef;
        //                    var price = ROUND(CASTASDOUBLE(TAB_GETROW(priceRec, "PRICE")), 6);
        //                    var ptype = CASTASSHORT(TAB_GETROW(priceRec, "PTYPE"));

        //                    var added = PRCLIST.ADD(lref,
        //                 new Dictionary<string, object>() {
        //                {"CARDREF",cardref},
        //                {"PRICE",price},
        //                {"PTYPE",ptype},

        //                });
        //                }
        //            }
        //            public void ADD_MAT(_PLUGIN pPLUGIN, object pLref)
        //            {

        //                if (!isUserCard(pLref))
        //                    return;

        //                if (ITEMS.HASLREF(pLref))
        //                    return;

        //                var matRec_ = TAB_GETLASTROW(pPLUGIN.SQL(@"
        //SELECT
        //CODE,  
        //NAME,  
        //SPECODE ,
        //SPECODE2 ,
        //--TEXTF1 ,
        //CARDTYPE 
        //from LG_$FIRM$_ITEMS WITH(NOLOCK) WHERE LOGICALREF = @P1
        //", new object[] { pLref }));


        //                if (matRec_ == null)
        //                    return;


        //                var lref = pLref;
        //                var code = CASTASSTRING(TAB_GETROW(matRec_, "CODE"));
        //                var name = CASTASSTRING(TAB_GETROW(matRec_, "NAME"));
        //                var sp = CASTASSTRING(TAB_GETROW(matRec_, "SPECODE"));
        //                var sp2 = CASTASSTRING(TAB_GETROW(matRec_, "SPECODE2"));
        //                // var text1 = CASTASSTRING(TAB_GETROW(matRec_, "TEXTF1"));
        //                var type = CASTASSHORT(TAB_GETROW(matRec_, "CARDTYPE"));

        //                if (!isUserCard(lref))
        //                    return;

        //                var added = ITEMS.ADD(lref,
        //               new Dictionary<string, object>() {
        //                {"LOGICALREF",lref},
        //                {"CODE",code},
        //                {"NAME",name},
        //                {"SPECODE",sp},
        //                {"SPECODE2",sp2},
        //               // {"TEXTF1",text1},
        //                {"CARDTYPE",type},
        //               // {"PRCLIST_____PRICE1",0},
        //               // {"PRCLIST_____PRICE2",0},//TODO
        //               // {"ei_code","ADP_ITEMS"},
        //                });

        //                if (added)
        //                {
        //                    ADD_PRICE(pPLUGIN, lref);
        //                }
        //            }

        //            public void ADD_CLIENT(_PLUGIN pPLUGIN, object pLref)
        //            {

        //                if (!isUserCard(pLref))
        //                    return;


        //                if (CLCARD.HASLREF(pLref))
        //                    return;


        //                var clRec_ = TAB_GETLASTROW(pPLUGIN.SQL(@"
        //SELECT
        //CODE,  
        //DEFINITION_,  
        //SPECODE ,
        //SPECODE2 ,
        //--TEXTF1 ,
        //CARDTYPE 
        //from LG_$FIRM$_CLCARD WITH(NOLOCK) WHERE LOGICALREF = @P1
        //", new object[] { pLref }));


        //                if (clRec_ == null)
        //                    return;

        //                var lref = pLref;
        //                var code = CASTASSTRING(TAB_GETROW(clRec_, "CODE"));
        //                var name = CASTASSTRING(TAB_GETROW(clRec_, "DEFINITION_"));
        //                var sp = CASTASSTRING(TAB_GETROW(clRec_, "SPECODE"));
        //                var sp2 = CASTASSTRING(TAB_GETROW(clRec_, "SPECODE2"));
        //                // var text1 = CASTASSTRING(TAB_GETROW(clRec_, "TEXTF1"));
        //                var type = CASTASSHORT(TAB_GETROW(clRec_, "CARDTYPE"));


        //                CLCARD.ADD(lref,

        //              new Dictionary<string, object>() {
        //                {"LOGICALREF",lref},
        //                {"CODE",code},
        //                {"DEFINITION_",name},
        //                {"SPECODE",sp},
        //                {"SPECODE2",sp2},
        //               // {"TEXTF1",text1},
        //                {"CARDTYPE",type},

        //               // {"ei_code","ADP_CLCARD"},
        //                });


        //            }




        //            bool isUserCard(object pLref)
        //            {
        //                return IS_USER_CARD(pLref, agentNr);
        //            }


        //            public static bool IS_USER_CARD(object pLref, short pAgentNr)
        //            {
        //                if (ISEMPTYLREF(pLref))
        //                    return false;
        //                var _lref = CASTASINT(pLref);

        //                var arr = AGENT_LREF_MIN_MAX(pAgentNr);

        //                return arr[0] <= _lref && _lref <= arr[1];
        //            }


        //            public static int[] AGENT_LREF_MIN_MAX(short pAgent)
        //            {

        //                var _agent = (int)pAgent;

        //                _agent = _agent << (4 + 8 + 8);

        //                var min = _agent;
        //                var max = _agent | 0x000FFFFF;



        //                return new int[] { min, max };
        //            }




        ////            void ADD_CHANGED_CARDS_MAT(_PLUGIN pPLUGIN)
        ////            {
        ////                var clients = pPLUGIN.SQL(@"
        ////SELECT LOGICALREF
        ////from LG_$FIRM$_ITEMS WITH(NOLOCK) WHERE RECSTATUS > 0 --CAPIBLOCK_CREATEDBY > -1 OR CAPIBLOCK_MODIFIEDBY > -1
        ////UNION
        ////SELECT CARDREF
        ////from LG_$FIRM$_PRCLIST WITH(NOLOCK) WHERE PTYPE IN (1,2) AND RECSTATUS > 0 --CAPIBLOCK_CREATEDBY > -1 OR CAPIBLOCK_MODIFIEDBY > -1
        ////", new object[] { });


        ////                foreach (DataRow r in clients.Rows)
        ////                {
        ////                    ADD_MAT(pPLUGIN, TAB_GETROW(r, "LOGICALREF"));
        ////                }

        ////            }
        ////            void ADD_CHANGED_CARDS_CL(_PLUGIN pPLUGIN)
        ////            {

        ////                var clients = pPLUGIN.SQL(@"
        ////SELECT LOGICALREF
        ////from LG_$FIRM$_CLCARD WITH(NOLOCK) WHERE  RECSTATUS > 0 -- CAPIBLOCK_CREATEDBY > -1 OR CAPIBLOCK_MODIFIEDBY > -1
        ////", new object[] { });


        ////                foreach (DataRow r in clients.Rows)
        ////                {
        ////                    ADD_CLIENT(pPLUGIN, TAB_GETROW(r, "LOGICALREF"));
        ////                }


        ////            }


        //            //public void WRITE_CARDS(_PLUGIN pPLUGIN, System.Xml.XmlElement pRoot)
        //            //{

        //            //  //  ADD_CHANGED_CARDS_CL(pPLUGIN);
        //            //   // ADD_CHANGED_CARDS_MAT(pPLUGIN);

        //            //    var doc = pRoot.OwnerDocument;
        //            //    var root = pRoot;


        //            //    //cl
        //            //    CLCARD.WRITE_CARDS(root);
        //            //    ITEMS.WRITE_CARDS(root);
        //            //    PRCLIST.WRITE_CARDS(root);
        //            //    //mat


        //            //}

        //        }

        #endregion



        #endregion
