#line 2



 
 
       #region PLUGIN_BODY
        const int VERSION = 11;

        const string FILE = "plugin.sys.event.carparts.serials.pls";



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

                    x.MY_CARPARTS_SERIALS_USER = s.MY_CARPARTS_SERIALS_USER;

                    //

                    _SETTINGS.BUF = x;

                }

                public string MY_CARPARTS_SERIALS_USER;



            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC) //, "ava.production.config")
            {

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active On User")]
            public string MY_CARPARTS_SERIALS_USER
            {
                get
                {
                    return (_GET("MY_CARPARTS_SERIALS_USER", "1"));
                }
                set
                {
                    _SET("MY_CARPARTS_SERIALS_USER", value);
                }

            }



            //

            public static bool ISUSEROK(_PLUGIN pPLUGIN)
            {
                return Array.IndexOf<string>(
                     EXPLODELIST(BUF.MY_CARPARTS_SERIALS_USER),
                     FORMAT(pPLUGIN.GETSYSPRM_USER())
                     ) >= 0;
            }

        }

        #endregion



        #region TEXT

        public class TEXT
        {
            public const string text_DESC = "Car Parts Serials";


            public class L
            {

            }
        }

        const string event_CAR_PARTS_  = "_car_parts_";
        const string event_CAR_PARTS_SERIALS_ = "_car_parts_serials_";
        const string event_CAR_PARTS_SERIALS_LOADPARTCODESTECDOC = "_car_parts_serials_loadpartcodestecdoc";
        const string event_CAR_PARTS_SERIALS_LOADPARTCODES2DB = "_car_parts_serials_loadpartcodes2db";
        const string event_CAR_PARTS_SERIALS_DELETEOEMINFO = "_car_parts_serials_deleteoeminfo";
        const string event_CAR_PARTS_SERIALS_STATISTIC = "_car_parts_serials_statstic";
        const string event_CAR_PARTS_SERIALS_CREATECROSSES = "_car_parts_serials_createcrosses";
        #endregion

        #region MAIN



        public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
        {

            if (ISWEB())
                return;


            _SETTINGS._BUF.LOAD_SETTINGS(this);


            if (!_SETTINGS.ISUSEROK(this))
                return;

            CHECK_LOC_DS(this);

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
			{ "Text" ,"Car Parts Serials"},
			{ "ImageName" ,"folder_32x32"},
			{ "Name" ,event_CAR_PARTS_SERIALS_},
            };

                        RUNUIINTEGRATION(tree, args);

                    }


                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_CAR_PARTS_SERIALS_},
			{ "CmdText" ,"event name::"+event_CAR_PARTS_SERIALS_LOADPARTCODESTECDOC},
			{ "Text" ,"Create info from TEC DOC folder"},
			{ "ImageName" ,"srv_32x32"},
			//{ "Name" ,event_CAR_PARTS_SERIALS_LOADPARTCODESTECDOC},
            };

                        RUNUIINTEGRATION(tree, args);

                    }
                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_CAR_PARTS_SERIALS_},
			{ "CmdText" ,"event name::"+event_CAR_PARTS_SERIALS_LOADPARTCODES2DB},
			{ "Text" ,"Load to database OEM info"},
			{ "ImageName" ,"storage_32x32"},
			//{ "Name" ,event_CAR_PARTS_SERIALS_LOADPARTCODES2DB},
            };

                        RUNUIINTEGRATION(tree, args);

                    }
					
					
{
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_CAR_PARTS_SERIALS_},
			{ "CmdText" ,"event name::"+event_CAR_PARTS_SERIALS_CREATECROSSES},
			{ "Text" ,"Update Crosses"},
			{ "ImageName" ,"exim_32x32"},
			//{ "Name" ,event_CAR_PARTS_SERIALS_LOADPARTCODES2DB},
            };

                        RUNUIINTEGRATION(tree, args);

                    }
					
                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_CAR_PARTS_SERIALS_},
			{ "CmdText" ,"event name::"+event_CAR_PARTS_SERIALS_DELETEOEMINFO},
			{ "Text" ,"Delete OEM info"},
			{ "ImageName" ,"delete_32x32"},
			//{ "Name" ,event_CAR_PARTS_SERIALS_LOADPARTCODES2DB},
            };

                        RUNUIINTEGRATION(tree, args);

                    }
                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_CAR_PARTS_SERIALS_},
			{ "CmdText" ,"event name::"+event_CAR_PARTS_SERIALS_STATISTIC},
			{ "Text" ,"Info"},
			{ "ImageName" ,"info_32x32"},
			//{ "Name" ,event_CAR_PARTS_SERIALS_LOADPARTCODES2DB},
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


                    case event_CAR_PARTS_SERIALS_LOADPARTCODESTECDOC:

                        MY_SYS_LOADPARTCODESTECDOC();

                        break;
                    case event_CAR_PARTS_SERIALS_LOADPARTCODES2DB:

                        MY_SYS_LOADPARTCODES2DB();

                        break;
                    case event_CAR_PARTS_SERIALS_DELETEOEMINFO:

                        MY_SYS_DELETEOEMINFO();

                        break;
                    case event_CAR_PARTS_SERIALS_STATISTIC:

                        MY_SYS_STATISTIC();

                        break;

                    case event_CAR_PARTS_SERIALS_CREATECROSSES:

                        MY_SYS_CREATECROSSES();

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




        void MY_SYS_LOADPARTCODESTECDOC()
        {


            if (!MSGUSERASK("T_MSG_COMMIT_BEGIN - Convert TEC DOC files"))
                return;


            MY_DIR.CHECK_DIR();

            MY_DIR.CLEAN_OEM_INFO();


            var oemFiles = Directory.GetFiles(MY_DIR.PRM_DIR_TECDOC, "crosses*OEM.csv");


            foreach (var file in oemFiles)
            {
                var fileOut = Path.Combine(MY_DIR.PRM_DIR_OEM_INFO, Path.GetFileName(file));
                var listOutInfo = new List<string[]>();
                var lineIndx = 0;
                var lastLine = "";
                try
                {
                    //read file
                    using (var fs = new FileStream(file, FileMode.Open))
                    {
                        using (var bs = new BufferedStream(fs, 1000000))
                        {
                            using (var sr = new StreamReader(bs))
                            {


                                while (true)
                                {
                                    ++lineIndx;

                                    var line = lastLine = sr.ReadLine();
                                    if (line == null)
                                        break;

                                    line = line.Trim();
                                    if (line == "")
                                        continue;

                                    if (lineIndx == 1)
                                    {
                                        //skeep header
                                        continue;
                                    }

                                    //expmle  "Пружина";"SWAG";="10902104";2362187;"CHRYSLER";="05098006AA";="05098006AA"

                                    var lineParts = line.Split(';');
                                    if (lineParts.Length != 7)
                                    {
                                        throw new Exception("Waiting line with 7 parts");

                                    }


                                    var info = new string[] { 
                                  MY_TOOL.UNWRAP_CSV_ITEM(  lineParts[2]),
                            MY_TOOL.REPLACE_BRAND_NAME(MY_TOOL.UNWRAP_CSV_ITEM(  lineParts[1])),
                                  MY_TOOL.UNWRAP_CSV_ITEM(  lineParts[5]),
                             MY_TOOL.REPLACE_BRAND_NAME(MY_TOOL.UNWRAP_CSV_ITEM(  lineParts[4]))
                                    };

                                    listOutInfo.Add(info);

                                }


                            }

                        }

                    }
                    //write file

                    using (var fs = new FileStream(fileOut, FileMode.Create))
                    {
                        using (var bs = new BufferedStream(fs, 1000000))
                        {
                            using (var sw = new StreamWriter(bs))
                            {

                                foreach (var line in listOutInfo)
                                {
                                    sw.WriteLine(JOINLISTTAB(line));
                                }

                                sw.Flush();
                            }


                        }

                    }
                }
                catch (Exception exc)
                {

                    MY_TOOL.LOG("Error: " + file);
                    MY_TOOL.LOG("Error Line Index: " + lineIndx);
                    MY_TOOL.LOG("Error Line Info: " + lastLine);
                    MY_TOOL.LOG(exc.ToString());

                    throw new Exception(exc.Message);
                }
            }



            MSGUSERINFO("T_MSG_OPERATION_OK");
        }


        void MY_SYS_CREATECROSSES()
        {

            if (!MSGUSERASK("T_MSG_COMMIT_BEGIN - Update Crosses"))
                return;



            //INVOKEINBATCH((s, a) => {


                var sql = @"




   DECLARE  @CODES TABLE(CODE VARCHAR(50) PRIMARY KEY)
 
 INSERT INTO @CODES 
 SELECT CODE FROM LG_$FIRM$_ITEMS
 UNION
 SELECT  REPLACE(REPLACE(CODE,' ',''),'.','') FROM LG_$FIRM$_ITEMS
  

DECLARE @LREF INT,
	@LREFMIN INT,
	@LREFMAX INT,
	@CODE VARCHAR(100),
	@CODE2 VARCHAR(100),
	@NAME VARCHAR(100),
	@CROSSESCODE VARCHAR(7000)


declare @PART_OEM table(CODE varchar(100),NAME varchar(100))
declare @PART_BRAND table(CODE varchar(100),NAME varchar(100))
declare @PART_ALL table(CODE varchar(100),NAME varchar(100))


SELECT @LREFMIN = MIN(LOGICALREF)
FROM LG_$FIRM$_ITEMS  

SELECT @LREFMAX = MAX(LOGICALREF)
FROM LG_$FIRM$_ITEMS     

SELECT @LREF = @LREFMIN

WHILE @LREF <= @LREFMAX
BEGIN
	SELECT @CODE = NULL

	SELECT @CODE = CODE,@NAME = NAME
	FROM LG_$FIRM$_ITEMS WITH(NOLOCK)
	WHERE LOGICALREF = @LREF  --AND CODE IN ('12400 01' )

	IF @CODE IS NOT NULL
	BEGIN

	SELECT @CODE2 = REPLACE(REPLACE(@CODE,' ',''),'.','')
	 

delete from @PART_OEM  
delete from @PART_BRAND  
delete from @PART_ALL 
	 
--BRAND
insert into @PART_BRAND(CODE,NAME)
select top(400) INFO.BRAND_ART_CODE,INFO.BRAND_NAME from CAR_PARTS_OEM_INFO INFO with(nolock) where INFO.OEM_ART_CODE IN (@CODE,@CODE2)

--OEM
 insert into @PART_OEM(CODE,NAME)
select distinct top(400) INFO.OEM_ART_CODE,INFO.OEM_NAME from 
@PART_BRAND B inner join CAR_PARTS_OEM_INFO INFO with(nolock) 
on B.CODE = INFO.BRAND_ART_CODE and B.NAME = INFO.BRAND_NAME


--BRAND + OEM
insert into @PART_ALL(CODE,NAME)
select top(200)  CODE,NAME from @PART_BRAND

insert into @PART_ALL(CODE,NAME)
select top(200) O.CODE,O.NAME from @PART_OEM O inner join @CODES I on I.CODE = O.CODE    
 
 insert into @PART_ALL(CODE,NAME) values(@NAME,'')

 if @CODE != @CODE2
  insert into @PART_ALL(CODE,NAME) values(@CODE,'')

 select @CROSSESCODE = NULL

 select @CROSSESCODE = ISNULL(@CROSSESCODE +' ','')+LEFT(NAME,5)+'('+CODE+')' from @PART_ALL

  
 --select  @CROSSESCODE  = LEFT(@CROSSESCODE,8000)

-- print @CROSSESCODE

 if @CROSSESCODE is not null
 begin
	 if exists (select 1 from CAR_PARTS_$FIRM$_CROSSES where ITEMREF = @LREF)
	 update CAR_PARTS_$FIRM$_CROSSES 
	 set CROSSESCODE = @CROSSESCODE where 
	 ITEMREF = @LREF AND (CROSSESCODE != @CROSSESCODE)
	 else
	 insert into CAR_PARTS_$FIRM$_CROSSES(ITEMREF,CROSSESCODE) 
	 values(@LREF,@CROSSESCODE)
 end


 
	END

	SELECT @LREF = @LREF + 1
END



";


                SQL(sql);
            
            
           // },null);



            MSGUSERINFO("T_MSG_OPERATION_OK");
        }

        void MY_SYS_STATISTIC()
        {
            MY_DIR.CHECK_DIR();

            {
                var data = SQL("select OEM_NAME,COUNT(*) COUNT_ from  CAR_PARTS_OEM_INFO with(nolock) group by OEM_NAME order by OEM_NAME");

                var sb = new StringBuilder();

                foreach (DataRow r in data.Rows)
                {
                    sb.AppendLine(
                         FORMAT(TAB_GETROW(r, "OEM_NAME"))
                        + "\t" +
                          FORMAT(TAB_GETROW(r, "COUNT_"))
                          );

                }

                File.WriteAllText(Path.Combine(MY_DIR.PRM_DIR_ROOT, "STATISTIC-OEM.txt"), sb.ToString());
            }

            {
                var data = SQL("select BRAND_NAME,COUNT(*) COUNT_ from  CAR_PARTS_OEM_INFO with(nolock) group by BRAND_NAME order by BRAND_NAME");

                var sb = new StringBuilder();

                foreach (DataRow r in data.Rows)
                {
                    sb.AppendLine(
                        FORMAT(TAB_GETROW(r, "BRAND_NAME"))
                        + "\t" +
                          FORMAT(TAB_GETROW(r, "COUNT_"))
                        );

                }

                File.WriteAllText(Path.Combine(MY_DIR.PRM_DIR_ROOT, "STATISTIC-BRAND.txt"), sb.ToString());
            }

            {
                var data = SQL(@"
				select 
				(SELECT CODE FROM LG_$FIRM$_ITEMS WITH(NOLOCK) WHERE LOGICALREF = C.ITEMREF) CODE,
				CROSSESCODE 
				from  CAR_PARTS_$FIRM$_CROSSES C with(nolock) order by CODE
				");

                var sb = new StringBuilder();

                foreach (DataRow r in data.Rows)
                {
                    sb.AppendLine(
                        FORMAT(TAB_GETROW(r, "CODE"))
                        + "\t" +
                          FORMAT(TAB_GETROW(r, "CROSSESCODE"))
                        );

                }

                File.WriteAllText(Path.Combine(MY_DIR.PRM_DIR_ROOT, "CROSSES.txt"), sb.ToString());
            }

            MSGUSERINFO("T_MSG_OPERATION_OK");
        }

        void MY_SYS_DELETEOEMINFO()
        {

            if (!MSGUSERASK("T_MSG_COMMIT_BEGIN - Delete OEM info"))
                return;

            var OEM = MY_ASK_STRING(this, "OEM Name", "");

            if (string.IsNullOrEmpty(OEM))
                return;

            var count = CASTASINT(
                 SQLSCALAR("select COUNT(*) from CAR_PARTS_OEM_INFO with(nolock) where OEM_NAME = @P1", new object[] { OEM }));


            if (count == 0)
            {
                MSGUSERERROR("T_MSG_ERROR_NO_DATA OEM:" + OEM);
                return;
            }
            else
            {
                if (MSGUSERASK("T_MSG_COMMIT_DELETE \n OEM: " + OEM + ": T_COUNT " + count))
                {
                    SQL("delete from CAR_PARTS_OEM_INFO where OEM_NAME = @P1", new object[] { OEM });

                    MSGUSERINFO("T_MSG_OPERATION_OK");
                }


            }

        }

        void MY_SYS_LOADPARTCODES2DB()
        {


            if (!MSGUSERASK("T_MSG_COMMIT_BEGIN - Update database"))
                return;


            MY_DIR.CHECK_DIR();

            //  MY_DIR.CLEAN_OEM_INFO();


            var oemFiles = Directory.GetFiles(MY_DIR.PRM_DIR_OEM_INFO, "crosses*OEM.csv");

           // SQL("truncate  table CAR_PARTS_OEM_INFO", null);

            foreach (var file in oemFiles)
            {

                var listOutInfo = new List<string[]>();
                var lineIndx = 0;
                var lastLine = "";
                try
                {
                    //read file
                    using (var fs = new FileStream(file, FileMode.Open))
                    {
                        using (var bs = new BufferedStream(fs, 1000000))
                        {
                            using (var sr = new StreamReader(bs))
                            {


                                while (true)
                                {
                                    ++lineIndx;

                                    var line = lastLine = sr.ReadLine();
                                    if (line == null)
                                        break;

                                    line = line.Trim();
                                    if (line == "")
                                        continue;


                                    //expmle  2362187  SWAG   05098006AA  CHRYSLER

                                    var lineParts = EXPLODELISTTAB(line);
                                    if (lineParts.Length != 4)
                                        throw new Exception("Waiting line with 4 parts");

                                    listOutInfo.Add(lineParts);



                                }


                            }

                        }

                    }

                    //write to db

                    foreach (var item in listOutInfo)
                    {

                        var oem_code = item[0];
                        var oem_name = item[1];
                        var brand_code = item[2];
                        var brand_name = item[3];

                        var res = SQLSCALAR(@"

if not exists(
select 1 from CAR_PARTS_OEM_INFO with(nolock,index=INDX_CAR_PARTS_OEM_INFO_I2) where 
 OEM_ART_CODE = @P1 and
 OEM_NAME= @P2  and
 BRAND_ART_CODE= @P3  and
 BRAND_NAME= @P4
)
insert into  CAR_PARTS_OEM_INFO values (@P1,@P2,@P3,@P4)
else
select 'DUBL'
", new object[] { oem_code, oem_name, brand_code, brand_name });

                        if (res == "DUBL")
                        {

                        }



                    }

                }
                catch (Exception exc)
                {

                    MY_TOOL.LOG("Error: " + file);
                    MY_TOOL.LOG("Error Line Index: " + lineIndx);
                    MY_TOOL.LOG("Error Line Info: " + lastLine);
                    MY_TOOL.LOG(exc.ToString());

                    throw new Exception(exc.Message);
                }
            }



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

        static bool CHECK_DATA_IS_OLD(_PLUGIN pPLUGIN)
        {
            var dateLastValid_ = GET_SERVER_DATE(pPLUGIN).Date.AddDays(-1);
            var dateLastDb_ = CASTASDATE(pPLUGIN.SQLSCALAR("select DATE_ from OLAP_$FIRM$_LAST_DATE where CODE = 'ALL'", null)).Date;

            return (dateLastDb_ < dateLastValid_);
        }
        static void CHECK_LOC_DS(_PLUGIN pPLUGIN)
        {

            /*
             
           var date_=   CASTASDATE(pPLUGIN.SQLSCALAR("select getdate()",  null));
 
             
             
             */


            //10


            var currVersionNum = 11;
            var currVersionCode = "CARPARTS_$FIRM$_$PERIOD$";
            var dbVers = CASTASINT(pPLUGIN.SQLSCALAR(
                "select [dbo].[f_GETOBJVERS]('" + currVersionCode + "')", //has pattern
                new object[] { }));


            if (dbVers >= currVersionNum)
                return;


            pPLUGIN.SQL(@"





IF ISNULL (
		  OBJECT_ID('CAR_PARTS_OEM_INFO'),0
		) = 0
CREATE   TABLE CAR_PARTS_OEM_INFO(

	 OEM_ART_CODE varchar(25) NOT NULL,
	 OEM_NAME varchar(25) NOT NULL,
	 BRAND_ART_CODE varchar(25) NOT NULL,
	 BRAND_NAME varchar(25) NOT NULL 
 ) 



IF not exists (
		  select 1 from sys.indexes where name = 'INDX_CAR_PARTS_OEM_INFO_I2'
		)  
create index INDX_CAR_PARTS_OEM_INFO_I2 on CAR_PARTS_OEM_INFO 
(
	 OEM_ART_CODE  ,
	 BRAND_ART_CODE  
)


IF not exists (
		  select 1 from sys.indexes where name = 'INDX_CAR_PARTS_OEM_INFO_I3'
		)  
create index INDX_CAR_PARTS_OEM_INFO_I3 on CAR_PARTS_OEM_INFO 
(
	 BRAND_ART_CODE  ,
	 OEM_ART_CODE  
)



IF ISNULL (
		  OBJECT_ID('CAR_PARTS_$FIRM$_CROSSES'),0
		) = 0
CREATE TABLE CAR_PARTS_$FIRM$_CROSSES(
	 ITEMREF INT NOT NULL PRIMARY KEY,
	 CROSSESCODE varchar(MAX) NOT NULL   
 ) 

 

", null);



           



            //fix
            pPLUGIN.SQL("exec [dbo].[p_SETOBJVERS] '" + currVersionCode + "', @P1", //has pattern
                new object[] { currVersionNum });



        }






        void _ADD_COL(_PLUGIN plg, string tab, string col, string type)
        {
            var sql = string.Format(
                @"

 
IF NOT EXISTS(
select 1 from  [INFORMATION_SCHEMA].[COLUMNS] where TABLE_NAME = '{0}' and COLUMN_NAME = '{1}'
 )
BEGIN
	 alter table {0} add {1} {2}
END
 

", tab, col, type
                );

            plg.SQL(sql, null);

        }


        #endregion




        #endregion




        #region CLASS

        class MY_TOOL
        {

            static Dictionary<string, string> replace = new Dictionary<string, string>() { 
            {"MERCEDES-BENZ","MERCEDES"},
            // {"FEBIBILSTEIN","FEBI"},
            //{"FEBI BILSTEIN","FEBI"},
            };

            public static string UNWRAP_CSV_ITEM(string pItem)
            {

                pItem = pItem.Trim().Trim('=').Trim('"').Trim().Replace(" ", "");

                return pItem;

            }
            public static string REPLACE_BRAND_NAME(string pItem)
            {

                if (replace.ContainsKey(pItem))
                    return replace[pItem];

                return pItem;

            }
            public static void LOG(string pMsg)
            {

                File.AppendAllText(MY_DIR.PRM_FILE_LOG, "[" + FORMAT(DateTime.Now) + "]" + "\n" + pMsg + "\n");

            }

        }

        class MY_DIR
        {
            public static string PRM_DIR_ROOT = PATHCOMBINE(
     Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CAR_PARTS");

            public static string PRM_DIR_TECDOC = PATHCOMBINE(PRM_DIR_ROOT, "TECDOC");
            public static string PRM_DIR_OEM_INFO = PATHCOMBINE(PRM_DIR_ROOT, "OEM_INFO");
            public static string PRM_FILE_LOG = PATHCOMBINE(PRM_DIR_ROOT, "LOG.txt");

            public static void CHECK_DIR()
            {

                if (!Directory.Exists(PRM_DIR_TECDOC))
                    Directory.CreateDirectory(PRM_DIR_TECDOC);
                if (!Directory.Exists(PRM_DIR_OEM_INFO))
                    Directory.CreateDirectory(PRM_DIR_OEM_INFO);


            }


            public static void CLEAN_OEM_INFO()
            {
                if (Directory.Exists(PRM_DIR_OEM_INFO))
                    Directory.Delete(PRM_DIR_OEM_INFO, true);

                if (!Directory.Exists(PRM_DIR_OEM_INFO))
                    Directory.CreateDirectory(PRM_DIR_OEM_INFO);

                if (File.Exists(PRM_FILE_LOG))
                    File.Delete(PRM_FILE_LOG);
            }


        }


        #endregion


        #endregion
