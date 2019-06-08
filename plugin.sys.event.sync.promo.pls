 #line 2
 
     #region PLUGIN_BODY
        const int VERSION = 9;


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

                    x._USER = PLUGIN.GETSYSPRM_USER();
                    x._FIRM = PLUGIN.GETSYSPRM_FIRM();
                    x._FIRMNAME = PLUGIN.GETSYSPRM_FIRMNAME();
                    x._PERIOD = PLUGIN.GETSYSPRM_PERIOD();



                    _SETTINGS.BUF = x;


                }


                public short _FIRM;
                public string _FIRMNAME;
                public short _PERIOD;
                public short _USER;



            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC)
            {

            }



       
             
        }

        #endregion


        #region TEXT

        public class TEXT
        {
            public const string text_DESC = "Sync Promo";
        }
        #endregion




        #region MAIN


        bool PRINTINV = false;
        bool OFFLINE = true;
        List<object> PRINTINVLREF = new List<object>();
        public object SYS_DONE(string CMD, object[] ARGS)
        {

            SYS_BEGIN(CMD, ARGS);

            return null;
        }

        public void SYS_BEGIN(string CMD, object[] ARGS)
        {
            try
            {

                _SETTINGS._BUF.LOAD_SETTINGS(this);


                ARGS = (ARGS == null ? new object[0] : ARGS);

                object arg0 = ARGS.Length > 0 ? ARGS[0] : null;
                object arg1 = ARGS.Length > 1 ? ARGS[1] : null;
                object arg2 = ARGS.Length > 2 ? ARGS[2] : null;

                switch (CMD)
                {
 
                    case SysEvent.SYS_PLUGINSETTINGS:
                        (arg0 as List<object>).Add(new _SETTINGS(this));
                        break;

                    //case "STARTED": //system start
                    //    MY_STARTED();
                    //    break;
                    //case "DATASTARTING"://start docS import//1
                    //    MY_DATASTARTING(arg0 as _PLUGIN.TOOL_WEB.DATASET);
                    //    break;
                    case "DATABEFORESAVE"://before .update() (put infor to db)//2
                        MY_DATABEFORESAVE(arg0 as DataSet);
                        break;
                    //case "DATAIMPORTED":// doc in db, pack not commited //3
                    //    MY_DATAIMPORTED(arg0 as object, arg1 as _PLUGIN.TOOL_WEB.DATASET, arg2 as _PLUGIN.TOOL_WEB.DATASET);
                    //    break;
                    //case "DATAFINISHING":// docs imported, not commited //4
                    //    MY_DATAFINISHING(arg0 as _PLUGIN.TOOL_WEB.DATASET);
                    //    break;
                    //case "DATACOMMIT":// docs imported, and commited //5
                    //    MY_DATACOMMIT(arg0 as _PLUGIN.TOOL_WEB.DATASET);
                    //    break;
           
                    
                }
            }
            catch (Exception exc)
            {
                RUNTIMELOG(exc.ToString());

                EXCEPTIONFORUSER("T_MSG_ERROR_INNER_SERVER");
            }

        }






        void MY_STARTED()
        {



        }


        void MY_DATASTARTING(_PLUGIN.TOOL_WEB.DATASET pData)
        {
            //PRINTINVLREF.Clear();
        }

        void MY_DATAFINISHING(_PLUGIN.TOOL_WEB.DATASET pData)
        {

            

        }
        void MY_DATACOMMIT(_PLUGIN.TOOL_WEB.DATASET pData)
        {

            //if (PRINTINV)
            //{
            //    foreach (object l in PRINTINVLREF)
            //    {
            //        string cmd_ = "rep loc::config/report/mm.000029 filter::{0} REP_QUICK_MODE_B::1 REP_DEF_FILTER_B::1 REP_DSG_KEY_WORDS_S::printsync DSGN_OUTPUT_B::1 DSGN_OUTPUT_DEF_DEV_B::1";
            //        string filter_ = string.Format("filter_INVOICE_LOGICALREF2,{0}", FORMATSERIALIZE(l));
            //        cmd_ = string.Format(cmd_, filter_);
            //        EXECMDTEXT(cmd_);
            //    }
            //}
            ////FILEWRITE("log.txt","\n"+FORMAT(l),false);


            //PRINTINVLREF.Clear();
        }



        void MY_DATABEFORESAVE(DataSet pDATASET)
        {
            if (pDATASET == null)
                return;


            try
            {

                var HEADER = pDATASET.Tables["INVOICE"];
                if (HEADER == null)
                    return;
                var TRCODE = CASTASSHORT(TAB_GETROW(HEADER, "TRCODE"));
                if (TRCODE != 8)
                    return;
                var CANCELLED = CASTASSHORT(TAB_GETROW(HEADER, "CANCELLED"));
                if (CANCELLED != 0)
                    return;

                var LINES = pDATASET.Tables["STLINE"];
                if (LINES == null)
                    return;


                SYSUSEREVENT("event_stockdocpromo_robot_apply", new object[] { pDATASET });

            }
            catch (Exception exc)
            {

                RUNTIMELOG(exc.ToString());
            }

        }

        void MY_DATAIMPORTED(object lref, _PLUGIN.TOOL_WEB.DATASET nodeRoot, _PLUGIN.TOOL_WEB.DATASET nodeDoc)
        {
            //if (PRINTINV)
            //{

            //    if (XMLNODEATTR(nodeDoc, "ei_code") == "ADP_INVOICE")
            //    {

            //        PRINTINVLREF.Add(lref);

            //    }
            //}


        }
       

 




        #endregion



        #region TOOLS

         


        #endregion

        #region CLASS

    
        #endregion


        #region TOOLS


        static string SQLTEXT(string pSqlText)
        {
            var dic = new Dictionary<string, string>(){
              {"$FIRM$",_SETTINGS.BUF._FIRM.ToString().PadLeft(3,'0')},
              {"$PERIOD$",_SETTINGS.BUF._PERIOD.ToString().PadLeft(2,'0')},
          };

            foreach (var itm in dic)
                pSqlText = pSqlText.Replace(itm.Key, itm.Value);

            return pSqlText;
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
        #endregion
