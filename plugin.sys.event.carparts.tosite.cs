#line 2



      #region PLUGIN_BODY
        const int VERSION = 11;

        const string FILE = "plugin.sys.event.carparts.tosite.pls";



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

                    x.MY_CARPARTS_TOSITE_MYSQL = s.MY_CARPARTS_TOSITE_MYSQL;

                    x.MY_CARPARTS_TOSITE_USER = s.MY_CARPARTS_TOSITE_USER;

                    //

                    _SETTINGS.BUF = x;

                }

                public string MY_CARPARTS_TOSITE_MYSQL;
                public string MY_CARPARTS_TOSITE_USER;



            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC) //, "ava.production.config")
            {

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active On User")]
            public string MY_CARPARTS_TOSITE_USER
            {
                get
                {
                    return (_GET("MY_CARPARTS_TOSITE_USER", "1"));
                }
                set
                {
                    _SET("MY_CARPARTS_TOSITE_USER", value);
                }

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("MySql Connection")]
            public string MY_CARPARTS_TOSITE_MYSQL
            {
                get
                {
                    return (_GET("MY_CARPARTS_TOSITE_MYSQL", "MYSQL,Server=127.0.0.1;Port=3306;Database=database;Uid=username;Password=pw;"));
                }
                set
                {
                    _SET("MY_CARPARTS_TOSITE_MYSQL", value);
                }

            }

            //

            public static bool ISUSEROK(_PLUGIN pPLUGIN)
            {
                return Array.IndexOf<string>(
                     EXPLODELIST(BUF.MY_CARPARTS_TOSITE_USER),
                     FORMAT(pPLUGIN.GETSYSPRM_USER())
                     ) >= 0;
            }

        }

        #endregion



        #region TEXT

        public class TEXT
        {
            public const string text_DESC = "Car Parts To Site";


            public class L
            {

            }
        }


        const string event_CAR_PARTS_TOSITE_ = "_car_parts_tosite_";
        const string event_CAR_PARTS_TOSITE_UPLOAD = "_car_parts_tosite_upload";

        #endregion

        #region MAIN



        public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
        {

            if (ISWEB())
                return;


            _SETTINGS._BUF.LOAD_SETTINGS(this);


            if (!_SETTINGS.ISUSEROK(this))
                return;



            object arg1 = ARGS.Length > 0 ? ARGS[0] : null;
            object arg2 = ARGS.Length > 1 ? ARGS[1] : null;
            object arg3 = ARGS.Length > 2 ? ARGS[2] : null;

            string[] list_ = EXPLODELISTPATH(EVENTCODE);

            var cmdType = list_.Length > 0 ? list_[0] : "";
            var cmdExt = list_.Length > 1 ? list_[1] : "";



            switch (cmdType)
            {
                case SysEvent.SYS_PLUGINSETTINGS:
                    (arg1 as List<object>).Add(new _SETTINGS(this));
                    break;
                case SysEvent.SYS_LOGIN:
                    {

                    }
                    break;
                case SysEvent.SYS_USEREVENT:
                    MY_SYS_USEREVENT_HANDLER(EVENTCODE, ARGS);
                    break;
                case SysEvent.SYS_NEWFORM:
                    MY_SYS_NEWFORM_INTEGRATE(arg1 as Form);
                    break;


            }



        }



        void MY_SYS_NEWFORM_INTEGRATE(Form FORM)
        {


            var fn = GETFORMNAME(FORM);



            if (fn == "form.app")
            {
                var tree = CONTROL_SEARCH(FORM, "cTreeTools");
                if (tree != null)
                {

                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
			{ "Text" ,"Car Parts To Site"},
			{ "ImageName" ,"folder_32x32"},
			{ "Name" ,event_CAR_PARTS_TOSITE_},
            };

                        RUNUIINTEGRATION(tree, args);

                    }


                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_CAR_PARTS_TOSITE_},
			{ "CmdText" ,"event name::"+event_CAR_PARTS_TOSITE_UPLOAD},
			{ "Text" ,"Upload To Site"},
			{ "ImageName" ,"srv_32x32"},
			//{ "Name" ,event_CAR_PARTS_TOSITE_LOADPARTCODESTECDOC},
            };

                        RUNUIINTEGRATION(tree, args);

                    }












                }
                return;

            }
            var cPanelBtnSub = CONTROL_SEARCH(FORM, "cPanelBtnSub");




            if (cPanelBtnSub == null)
                return;


            if (fn.StartsWith("ref.mm.rec.mat"))
            {

                //_MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(
                //           cPanelBtnSub,
                //           event_WF_PLAN_LOAD,
                //           LANG("T_TEST_1"),
                //           "test_16x16"
                //           );




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

                switch (list_.Length > 1 ? list_[1].ToLowerInvariant() : "")
                {


                    case event_CAR_PARTS_TOSITE_UPLOAD:

                        MY_TOSITE_UPLOAD();

                        break;




                }


            }
            catch (Exception exc)
            {
                LOG(exc);
                MSGUSERERROR(exc.Message);
            }
        }





        #region HANDLERS




        void MY_TOSITE_UPLOAD()
        {


            if (!MSGUSERASK("T_MSG_COMMIT_BEGIN - Car Parts To Site Upload"))
                return;







            var sql_generate = @"

   
 

declare @rate float
select top 1 @rate=RATES1 from L_DAILYEXCHANGES where CRTYPE = 162 order by DATE_ desc
select  @rate = ISNULL( @rate,1)


declare @SQL_TEXT TABLE( 
INDX INT IDENTITY(1,1),
VALUE varchar(MAX)
)

declare @ITEMS TABLE(  
INDX INT IDENTITY(1,1),  
LOGICALREF int,
CODE varchar(100),
NAME varchar(100),
SPECODE varchar(100),
SPECODE2 varchar(100),
CROSSES varchar(MAX),
ONHAND decimal(10,2),
ONHAND2 decimal(10,2),
PRICESLS decimal(10,2))  
declare @INDX_FROM INT,@INDX_TO INT,@LIM INT
select @INDX_FROM =0,@LIM = 200,@INDX_TO = @INDX_FROM+@LIM

declare @SQL varchar(max)
 
insert into @ITEMS(LOGICALREF,CODE,NAME,SPECODE,SPECODE2,CROSSES,ONHAND,ONHAND2,PRICESLS) select -- top(@LIM*1) 
LOGICALREF,
dbo.[p_MARKET_CLEAN_STR] (UPPER(LEFT(CODE,50))) CODE,
dbo.[p_MARKET_CLEAN_STR] (UPPER(LEFT(REPLACE(NAME,' ',''),50))) NAME,
dbo.[p_MARKET_CLEAN_STR] (UPPER(LEFT(ltrim(rtrim(upper(SPECODE))),50))) SPECODE,
dbo.[p_MARKET_CLEAN_STR] (UPPER(LEFT(ltrim(rtrim(upper(SPECODE2))),50))) SPECODE2,
----
dbo.[p_MARKET_CLEAN_STR] ( ISNULL((SELECT X.CROSSESCODE FROM CAR_PARTS_001_CROSSES X WHERE X.ITEMREF = ITEMS.LOGICALREF),'') ) 

CROSSES,
----
round(ISNULL((
		SELECT (ONHAND)
		FROM LG_001_02_GNTOTST WITH (NOLOCK)
		WHERE STOCKREF = ITEMS.LOGICALREF AND 
			INVENNO = 0 
),0.0),2) ONHAND,
----
round(ISNULL((
		SELECT (ONHAND)
		FROM LG_001_02_GNTOTST WITH (NOLOCK)
		WHERE STOCKREF = ITEMS.LOGICALREF AND 
			INVENNO = 5 
),0.0),2) ONHAND2,
----
round(ISNULL((
		SELECT TOP 1 PRICE/@rate
		FROM LG_001_PRCLIST PRC WITH (NOLOCK)
		WHERE PRC.CARDREF = ITEMS.LOGICALREF AND PRC.PTYPE = 2 
		ORDER BY ENDDATE DESC
),0.0),2) PRICESLS from LG_001_ITEMS ITEMS WITH (NOLOCK) order by LOGICALREF desc

 
while exists(select top 1 '1' from @ITEMS where INDX between @INDX_FROM and @INDX_TO )
begin
-- insert into SERVER_SITE...LG_001_ITEMS 
-- select LOGICALREF,CODE,NAME,SPECODE,SPECODE2,ONHAND,PRICESLS from @ITEMS where INDX between @INDX_FROM and @INDX_TO
select @SQL = null
select @SQL = isnull(@SQL+';'+char(13),'') + 
'insert into LG_001_ITEMS (LOGICALREF,CODE,NAME,SPECODE,SPECODE2,CROSSES,ONHAND,ONHAND2,PRICESLS) VALUES ('
+ '' +cast(LOGICALREF as varchar(50)) + ',' +
+ '''' +cast(CODE as varchar(50)) + ''',' +
+ '''' +cast(NAME as varchar(50)) + ''',' +
+ '''' +cast(SPECODE as varchar(50)) + ''',' +
+ '''' +cast(SPECODE2 as varchar(50)) + ''',' +
+ '''' +cast(CROSSES as varchar(max)) + ''',' +
+ '' +cast(ONHAND as varchar(50))+ ',' +
+ '' +cast(ONHAND2 as varchar(50))+ ',' +
+ '' +cast(PRICESLS as varchar(50)) +
')' from @ITEMS where INDX between @INDX_FROM and @INDX_TO


 insert into @SQL_TEXT(VALUE) values(@SQL);

 select @INDX_FROM =@INDX_TO+1,@INDX_TO = @INDX_FROM+@LIM;

end
 
 select VALUE from @SQL_TEXT order by INDX ASC;


  

";



            var sql_commit = @"


/*default_structures_brands*/
delete from default_structures_brands;
insert into default_structures_brands(`id`,`author_id`,`date`,`update`,`order`,`title`)
select MIN(LOGICALREF) `id`,1 `author_id`,NOW() `date`,NOW() `update`,0 `order`,  SPECODE `title` from LG_001_ITEMS group by SPECODE;

/*default_structures_makes*/

delete from default_structures_makes;
insert into default_structures_makes(`id`,`author_id`,`date`,`update`,`order`,`title`)
select MIN(LOGICALREF) `id`,1 `author_id`,NOW() `date`,NOW() `update`,0 `order`,  SPECODE2 `title` from LG_001_ITEMS group by SPECODE2;

/*default_structures_types*/

/*
delete from default_structures_types;
insert into default_structures_types(`id`,`author_id`,`date`,`update`,`order`,`title`, `image_id`)
select 1 `id`,1 `author_id`,NOW() `date`,NOW() `update`,0 `order`,  ''avto'' `title`, 0 `image_id` ;
*/

/*default_structures_catalog*/

 

/* delete from default_structures_catalog;*/

insert into default_structures_catalog(
`id`,
`author_id`,
`date`,
`update`,
`order`,
`type_id`,
`brand_code`,
`original_code`,
`price1`,
`qty`,
`qty2`,
`brand_id`,
`make_id`,
`price2`)
SELECT LOGICALREF `id`,
   1 `author_id`,
   NOW() `date`,
   NOW() `update`,
   0 `order`,
   14 `type_id`,
   CODE `brand_code`,
   ###NAME `original_code`,
   CROSSES `original_code`,
   PRICESLS `price1`,
   ONHAND `qty`,
   ONHAND2 `qty2`,
   B.id `brand_id`,
   M.id `make_id`,
   PRICESLS `price2`
FROM LG_001_ITEMS ITEMS 
 LEFT JOIN default_structures_brands B on B.title = ITEMS.SPECODE
LEFT JOIN default_structures_makes M on M.title = ITEMS.SPECODE2
WHERE ITEMS.LOGICALREF NOT IN (select id from default_structures_catalog);


 UPDATE default_structures_catalog C
inner join LG_001_ITEMS ITEMS on  C.id =  ITEMS.LOGICALREF 
 LEFT JOIN default_structures_brands B on B.title = ITEMS.SPECODE
LEFT JOIN default_structures_makes M on M.title = ITEMS.SPECODE2
 set
  C.`date` = NOW() ,
   C.`update` = NOW() ,
   
     C.`brand_code`= ITEMS.CODE,
    ###C.`original_code`=ITEMS.NAME,
	C.`original_code`=ITEMS.CROSSES,
	
   C.`price1`=ITEMS. PRICESLS,
    C.`price2`=ITEMS.PRICESLS,
    C.`qty`=ITEMS.ONHAND,
	C.`qty2`=ITEMS.ONHAND2,
   C.`brand_id`=IFNULL(B.id,0),
    C.`make_id`= IFNULL(M.id,0)

;
";



            var tabSql = XSQL(sql_generate, new object[] { }, null);

            XSQL("delete from LG_001_ITEMS ", new object[] { }, _SETTINGS.BUF.MY_CARPARTS_TOSITE_MYSQL);


            foreach (DataRow row in tabSql.Rows)
            {
                var _sql_tmp = TAB_GETROW(row, "VALUE").ToString();

                XSQL(_sql_tmp, new object[] { }, _SETTINGS.BUF.MY_CARPARTS_TOSITE_MYSQL);
            }

            XSQL(sql_commit, new object[] { }, _SETTINGS.BUF.MY_CARPARTS_TOSITE_MYSQL);

            MSGUSERINFO("T_MSG_OPERATION_OK");
        }



        #region SQL


        #endregion
        #endregion












        #region Help


        public static string MY_ASK_STRING(_PLUGIN pPLUGIN, string pMsg, string pDef)
        {

            DataRow[] rows_ = pPLUGIN.REF("ref.gen.string desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDef));
            if (rows_ != null && rows_.Length > 0)
            {
                return _PLUGIN.CASTASSTRING(ISNULL(rows_[0]["VALUE"], ""));
            }
            return null;

        }


        public static double MY_ASKNUM(_PLUGIN pPLUGIN, string pMsg, double pDef, int pDecimals)
        {
            //  
            DataRow[] rows_ = pPLUGIN.REF("ref.gen.double decimals::" + FORMAT(pDecimals) + " calc::1 desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDef));
            if (rows_ != null && rows_.Length > 0)
            {
                return _PLUGIN.CASTASDOUBLE(ISNULL(rows_[0]["VALUE"], 0.0));
            }
            return -1;
        }
        public static double MY_ASKNUM(_PLUGIN pPLUGIN, string pMsg, double pDef)
        {

            return MY_ASKNUM(pPLUGIN, pMsg, pDef, 2);
        }


        static DateTime MONTH_BEG(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }
        static DateTime MONTH_END(DateTime dt)
        {
            return MONTH_BEG(dt).AddMonths(+1).AddDays(-1);
        }

        static DateTime GET_SERVER_DATE(_PLUGIN pPLUGIN)
        {
            return CASTASDATE(pPLUGIN.SQLSCALAR("select getdate()", null));
        }





        #endregion




        #endregion




        #region CLASS




        #endregion


        #endregion
