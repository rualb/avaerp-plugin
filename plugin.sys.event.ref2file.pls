#line 2


  #region BODY
        //BEGIN

        const int VERSION = 7;
        const string FILE = "plugin.sys.event.ref2file.pls";





        #region TEXT


        const string event_REF2FILE_ = "hadlericom_ref2file_";
        const string event_REF2FILE_EXPORT_EXCEL = "hadlericom_ref2file_export_excel";


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


                    x.MY_REF2FILE_USER = s.MY_REF2FILE_USER;

                    x.GETSYSPRM_USER = PLUGIN.GETSYSPRM_USER();

                    _SETTINGS.BUF = x;

                }


                public string MY_REF2FILE_USER;


                public short GETSYSPRM_USER;
            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC)
            {

            }


            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active On User")]
            public string MY_REF2FILE_USER
            {
                get
                {
                    return (_GET("MY_REF2FILE_USER", ""));
                }
                set
                {
                    _SET("MY_REF2FILE_USER", value);
                }

            }


            public static bool ISUSEROK(_PLUGIN pPLUGIN)
            {
                return BUF.MY_REF2FILE_USER == ""
                || Array.IndexOf<string>(
                     EXPLODELIST(BUF.MY_REF2FILE_USER),
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

            var isList = fn.StartsWith("ref.");

            if (isList)
            {
                //cMenuMainInfo


                foreach (var ctrl in CONTROL_DESTRUCT(FORM))
                {
                    {
                        var menuItem = ctrl as ToolStripItem;
                        if (menuItem != null && menuItem.Name == "cMenuMainInfo")
                        {
                            {
                                var args = new Dictionary<string, object>() { 
 
            { "_cmd" ,"add"},
            { "_type" ,"item"},
            { "_name" , event_REF2FILE_EXPORT_EXCEL},

            { "_infoLocation" ,"event"},
            { "_infoArg" ,event_REF2FILE_EXPORT_EXCEL},

            { "Text" ,LANG("T_EXPORT (Excel)")},
            { "ImageName" ,"excel_16x16"},
             { "Name" ,event_REF2FILE_EXPORT_EXCEL},
            };

                                RUNUIINTEGRATION(menuItem, args);

                            }











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


                    case event_REF2FILE_EXPORT_EXCEL:
                        {
                            var form = arg1 as Form;
                            if (form == null)
                                return;
                            if (ISADAPTERFORM(form))
                                return;
                            var grid = CONTROL_SEARCH(form, "cGrid") as DataGridView;
                            if (grid == null)
                                return;

                            MY_REF2FILE(form, grid);

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

        private void MY_REF2FILE(Form pForm, DataGridView pGrid)
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

            APPEND_HEADER(pGrid, sb);

            var dicSum = new Dictionary<int, double>();

            var globCount = 0;
            var subcount = 0;

            while (true)
            {
                if (subcount > 5000)
                {
                    if (MSGUSERASK("T_MSG_COMMIT_OPERATION_STOP (T_COUNT_ROWS = " + globCount + ")"))
                    {
                        break;
                    }
                    else
                    {
                        subcount = 0;
                    }

                }


                var count = 0;
                if (first)
                    count = CASTASINT(RUNUIINTEGRATION(pForm, "_cmd", "first"));
                else
                    count = CASTASINT(RUNUIINTEGRATION(pForm, "_cmd", "next"));

                if (count <= 0)
                    break;

                globCount += count;
                subcount += count;

                var pos = pGrid.Rows.Count - count;

                APPEND_ROWS(pGrid, pos, sb, dicSum);


                first = false;
            }

            APPEND_SUM(pGrid, sb, dicSum);


            sb.AppendLine(@"
</Table>
</Worksheet>
</Workbook>");

            var name = MAKENAME(pForm.Text, "");

            var file = MY_DIR.SAVE(sb, name, ".xls");

            PROCESS(file, null);
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




        //END



        #region CLAZZ

        class TEXT
        {

            public const string text_DESC = "List 2 CSV";

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
     Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "REF2FILE");

            static string filePrefix = "exp";

            public static void CHECK_DIR()
            {

                if (!System.IO.Directory.Exists(PRM_DIR_ROOT))
                    System.IO.Directory.CreateDirectory(PRM_DIR_ROOT);


            }




            public static string SAVE(StringBuilder pSb, string pSufix, string pExt = ".csv")
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


        #region TOOLS
        public static string MY_CHOOSE_SQL(string pSqlMs, string pSqlPg)
        {

            if (ISMSSQL())
                return pSqlMs;

            if (ISPOSTGRESQL())
                return pSqlPg;


            throw new Exception("Undefined datasource");
        }


        #endregion

        #endregion
        #endregion