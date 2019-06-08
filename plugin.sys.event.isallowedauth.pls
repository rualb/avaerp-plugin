#line 2
 
 
  #region PLUGIN_BODY
        const int VERSION = 11;

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

                    x.DAYS = s.MY_ISALLOWEDAUTH_DAYSOUT;

                    //

                    _SETTINGS.BUF = x;

                }

                public int DAYS;

            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC)
            {

            }




            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Readonly After Days")]
            public int MY_ISALLOWEDAUTH_DAYSOUT
            {
                get { return PARSEINT(_GET("MY_ISALLOWEDAUTH_DAYSOUT", "-1")); }
                set { _SET("MY_ISALLOWEDAUTH_DAYSOUT", value); }

            }



        }

        #endregion
        #region TEXT

        public class TEXT
        {
            public const string text_DESC = "Is Allowed";

        }
        #endregion

        #region MAIN



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
                case SysEvent.SYS_ISALLOWEDAUTH:
                    MY_SYS_ISALLOWEDAUTH(arg1 as string, arg2 as DataRow, arg3 as List<bool>);
                    break;
            }



        }


        //todo
        DateTime? serverDate;
        int countChecked = 0;
        public void MY_SYS_ISALLOWEDAUTH(string pCmd, DataRow pRow, List<bool> pArg)
        {
            _SETTINGS._BUF.LOAD_SETTINGS(this);
            if (serverDate == null || countChecked > 100)
            {
                serverDate = CASTASDATE(SQLSCALAR(@"
--$MS$--SELECT GETDATE()
--$PG$--SELECT NOW()
				", null));
                countChecked = 0;
            }
            //if has DATE_ out of date

            if (pRow == null)
                return;

            if (pCmd == "view" || pCmd == "copy")
                return;

             if (GETSYSPRM_USER() == 1)
                 return;

            {

                ++countChecked;

                var col_ = pRow.Table.Columns["DATE_"];
                if (col_ == null || col_.DataType != typeof(DateTime))
                    return;

                var date_ = CASTASDATE(ISNULL(pRow[col_], new DateTime(1900, 1, 1)));

                var diff_ = serverDate.Value - date_;

                if (diff_.Days > 0 && diff_.Days > _SETTINGS.BUF.DAYS)
                    pArg.Add(false);


            }



        }


		/*
		class SQLTEXT
		{
		
			static SQLTEXT(){
			var t=GET_ENV("DATASOURCETYPE","");
				if(t == "POSTGRESQL"){
					GETDATE = "SELECT NOW()";
				} else {
					GETDATE = "SELECT getdate()";
				}
			}
		public static string GETDATE;
		}
		*/
		
 
        #endregion



        #endregion