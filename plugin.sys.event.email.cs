#line 2




   #region PLUGIN_BODY
        const int VERSION = 6;

 
 
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

                    x.MY_EMAIL_USER = s.MY_EMAIL_USER;
                    x.MY_EMAIL_DESC = s.MY_EMAIL_DESC;
					x.MY_EMAIL_SUBJECT = s.MY_EMAIL_SUBJECT;
					x.MY_EMAIL_PASSWORD = s.MY_EMAIL_PASSWORD;
					x.MY_EMAIL_SERVER = s.MY_EMAIL_SERVER;
					x.MY_EMAIL_ONUSERNR = s.MY_EMAIL_ONUSERNR;
					x.MY_EMAIL_USERTO = s.MY_EMAIL_USERTO;
                    //

                    _SETTINGS.BUF = x;

                }

                public string MY_EMAIL_USER;
				public string MY_EMAIL_DESC;
				public string MY_EMAIL_SUBJECT;
                public string MY_EMAIL_PASSWORD;
				public string MY_EMAIL_SERVER;
				public string MY_EMAIL_USERTO;
				public string MY_EMAIL_ONUSERNR;
			 
				
            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC) //, "ava.email.config")
            {

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("User From Login")]
            public string MY_EMAIL_USER
            {
                get
                {
                    return (_GET("MY_EMAIL_USER", "test@testtesttest.com"));
                }
                set
                {
                    _SET("MY_EMAIL_USER", value);
                }

            }
            [ECategory(TEXT.text_DESC)]
            [EDisplayName("User From Password")]
            public string MY_EMAIL_PASSWORD
            {
                get
                {
                    return (_GET("MY_EMAIL_PASSWORD", "*****"));
                }
                set
                {
                    _SET("MY_EMAIL_PASSWORD", value);
                }

            }
            [ECategory(TEXT.text_DESC)]
            [EDisplayName("User From Name")]
            public string MY_EMAIL_DESC
            {
                get
                {
                    return (_GET("MY_EMAIL_DESC", "Ava Mail"));
                }
                set
                {
                    _SET("MY_EMAIL_DESC", value);
                }

            }
             [ECategory(TEXT.text_DESC)]
            [EDisplayName("Mail Subject")]
            public string MY_EMAIL_SUBJECT
            {
                get
                {
                    return (_GET("MY_EMAIL_SUBJECT", "AVA REPORTING"));
                }
                set
                {
                    _SET("MY_EMAIL_SUBJECT", value);
                }

            }
            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Mail Server")]
            public string MY_EMAIL_SERVER
            {
                get
                {
                    return (_GET("MY_EMAIL_SERVER", "smtp.testtesttest.com"));
                }
                set
                {
                    _SET("MY_EMAIL_SERVER", value);
                }

            }
            [ECategory(TEXT.text_DESC)]
            [EDisplayName("User To")]
            public string MY_EMAIL_USERTO
            {
                get
                {
                    return (_GET("MY_EMAIL_USERTO", "test@testtesttest.com"));
                }
                set
                {
                    _SET("MY_EMAIL_USERTO", value);
                }

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Send Mail If User Nr")]
            public string MY_EMAIL_ONUSERNR
            {
                get
                {
                    return (_GET("MY_EMAIL_ONUSERNR", "0"));
                }
                set
                {
                    _SET("MY_EMAIL_ONUSERNR", value);
                }

            }
			
			
 
        }

        #endregion
        #region TEXT

        public class TEXT
        {
            public const string text_DESC = "E-Mail Reporting";

        }
        #endregion

        #region MAIN



        public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
        {
	
            if (ISWEB())
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
                case SysEvent.SYS_LOGIN:
                    MY_SYS_EMAIL();
                    break;
                case SysEvent.SYS_USEREVENT:
                    if (EVENTCODE.EndsWith("/email_report"))
                        MY_SYS_EMAIL();
                    break;
            }



        }



		
		
		public void MY_EMAIL(string BODY, string SUBJECT = null, string TO = null){

		 	//MessageBox.Show("email start");
			
			
		RUNTIMELOG(string.Format("Mail: Server= {0} User = {1} Pw = {2} To = {3} Subject = {4}",
				_SETTINGS.BUF.MY_EMAIL_SERVER,
				_SETTINGS.BUF.MY_EMAIL_USER,
				"", //_SETTINGS.BUF.MY_EMAIL_PASSWORD,
				_SETTINGS.BUF.MY_EMAIL_USERTO,
				_SETTINGS.BUF.MY_EMAIL_SUBJECT
				));
				
				
				using(var client = new System.Net.Mail.SmtpClient(_SETTINGS.BUF.MY_EMAIL_SERVER)){
                client.EnableSsl = true;
                client.Credentials = new System.Net.NetworkCredential(_SETTINGS.BUF.MY_EMAIL_USER, _SETTINGS.BUF.MY_EMAIL_PASSWORD);
                var from = new System.Net.Mail.MailAddress(_SETTINGS.BUF.MY_EMAIL_USER, _SETTINGS.BUF.MY_EMAIL_DESC, System.Text.Encoding.UTF8);
                var to = new System.Net.Mail.MailAddress(TO??_SETTINGS.BUF.MY_EMAIL_USERTO);
                var message = new System.Net.Mail.MailMessage(from, to);
                message.Body = BODY;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.IsBodyHtml = true;
                message.Subject = SUBJECT??_SETTINGS.BUF.MY_EMAIL_SUBJECT;
                message.SubjectEncoding = System.Text.Encoding.UTF8;

                try
                {
                    client.Send(message);
                }
                catch (System.Net.Mail.SmtpException exc)
                {
					
					RUNTIMELOG("Mail Send Error");
					RUNTIMELOG(exc.ToString());
                   
                }
				}
			
		}
      

        public void MY_SYS_EMAIL() //adapter start
        {
            try
            {
				_PLUGIN PLUGIN = this;
				
                var user = GETSYSPRM_USER();
                var firm = GETSYSPRM_FIRM();
				var firmname = GETSYSPRM_FIRMNAME();
				var period = GETSYSPRM_PERIOD();
                if (firm == 0 || period == 0)
                    return;

                _SETTINGS._BUF.LOAD_SETTINGS(this);
 
				 var arr = _SETTINGS.BUF.MY_EMAIL_ONUSERNR.Split(',');
 
				  //MessageBox.Show("Condition User: "+_SETTINGS.BUF.MY_EMAIL_ONUSERNR);
				  //MessageBox.Show("Me:"+user.ToString());
				  //MessageBox.Show("Is Ok:"+Array.IndexOf<string>(arr, FORMAT(user)));
				  
                if (Array.IndexOf<string>(arr, FORMAT(user)) < 0)
				return;
			
				var date = CASTASDATE(XSQLSCALAR("SELECT getdate()", null));
				
				
				var map = new Dictionary<string,object>();

				map["user"]=user;
				map["firm"]=firm;
				map["period"]=period;
				map["date"]=date;
				map["firmname"]=firmname;
				
				
             //_SETTINGS.BUF.MY_EMAIL_USER
//MessageBox.Show("email actin");
                 var act = new Action(
                        () =>
                        {
                            try
                            {
								
							//MessageBox.Show("db test");
                            var BODY = MY_REPORT.REPORT("001", new object[]{1},map);

							MY_EMAIL(BODY);
 
//MessageBox.Show("email end");
 
                                
                            }
                            catch (Exception exc)
                            {
								RUNTIMELOG("Email send error");
                                RUNTIMELOG(exc.ToString());
                            }
                        });


                    RUNWITHTIMEOUTNOWAIT(act, 40);


            }
            catch (Exception exc)
            {
                RUNTIMELOG(exc.ToString());
            }


        }




		
		
		class MY_REPORT{
			
			
		
		public static string REPORT( string CODE, object[] ARG, Dictionary<string,object> pMap){

 
			switch(CODE){
				
				case "001":
					return REPORT001(  ARG, pMap); 
				
			}
 
			
			throw new Exception("Incorrect report code "+CODE);
		}
			public static string REPORT001( object[] ARG, Dictionary<string,object> pMap){
			
			var data = _PLUGIN.XSQL(SQL001, ARG);
			
			string res="";
			string nl = "\n";
			
			
res += @"

	<div style='width:100%;text-align:center;display: block'>  
	<H1>"+ FORMAT(pMap["firmname"])+ " "+ FORMAT(pMap["date"])+@" </H1> 
	</div>
	<br/>
	<div style='width:100%;text-align:center;display: block'>
	    <style>	
		tr:nth-child(even) {background: #CCC}
tr:nth-child(odd) {background: #FFF}

 
	   </style>	
      <table border='1' style='border-spacing: 0px' align='center'> 
    
 
" ;
			foreach(DataRow ROW in data.Rows){
				
				var code = CASTASSTRING(TAB_GETROW(ROW,"CODE"));
				var desc = CASTASSTRING(TAB_GETROW(ROW,"DESC_"));
				var val = FORMAT(CASTASDOUBLE(TAB_GETROW(ROW,"VALUE_")),"#,##0.00");
				
				var sep = code == "SEP";
				
				res += string.Format("<tr><td align='left'>{0}</td><td align='right'>{1}</td></tr>",
				System.Web.HttpUtility.HtmlEncode(desc),
				sep?"":val
				);
				
				
				
			}
			
			res += "</table></div>";
			return res;
		}
			
#region reg			
		public static string SQL001=
		@"
		
 
declare @df datetime=null,@dt datetime=null
	 
declare @RES TABLE
(
	CODE NVARCHAR(40) ,
	DESC_ NVARCHAR(40)  ,
	VALUE_ FLOAT  
)
 
 

--DECLARE @df datetime,@dt datetime
if @df is null or @dt is null
 select 
 
 @df =   DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE())),@df = dateadd(day,0,@df),
 @dt =   DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE())),@dt = dateadd(day,0,@dt)

 --select @df ='20150625', @dt =  '20150625'

 declare @ITEMS table(LOGICALREF INT PRIMARY KEY,FLOATF1 FLOAT,PRICE_PURCHASE FLOAT,PRICE_SALE FLOAT)
 
INSERT INTO @ITEMS 
SELECT 
LOGICALREF,
FLOATF1 ,

		ISNULL((
				SELECT TOP 1 PRICE
				FROM LG_001_PRCLIST PRC WITH (NOLOCK)
				WHERE PRC.CARDREF = ITEMS.LOGICALREF
					AND PRC.PTYPE = 1
					AND PRC.PAYPLANREF = 0
				ORDER BY ENDDATE DESC
				), 0) PRICE_PURCHASE,
		ISNULL((
				SELECT TOP 1 PRICE
				FROM LG_001_PRCLIST PRC WITH (NOLOCK)
				WHERE PRC.CARDREF = ITEMS.LOGICALREF
					AND PRC.PTYPE = 2
					AND PRC.PAYPLANREF = 0
				ORDER BY ENDDATE DESC
				), 0) PRICE_SALE


FROM LG_001_ITEMS ITEMS WHERE 

EXISTS (
			SELECT TOP (1) 1
			FROM LG_001_01_STLINE WITH (
					NOLOCK,
					INDEX = I001_01_STLINE_I2
					)
			WHERE STOCKREF = ITEMS.LOGICALREF
				AND DATE_ > '19000101'
				AND FTIME >= 0
				AND TRCODE IN (1, 50, 14, 13)
				AND CANCELLED = 0
			)


DECLARE @CASHBEG FLOAT,
	@CASHIN FLOAT,
	@CASHOUT FLOAT,
	@CASHEND FLOAT,
	@CLDEBIT FLOAT,
	@CLCREDIT FLOAT,
	@CLVDEBIT FLOAT,
	@CLVCREDIT FLOAT,
	@CLIN FLOAT,
	@CLOUT FLOAT,
	@CLVIN FLOAT,
	@CLVOUT FLOAT,
	@MATREM_SLSPRC FLOAT,
	@MATREM_PRCHPRC FLOAT,
	@MATTOT_PRCH FLOAT,
	@MATTOT_PRCHRET FLOAT,
	@MATTOT_SLS FLOAT,
	@MATTOT_SLSRET FLOAT,
	@MATTOT_COUNT50 FLOAT,
	@MATTOT_COUNT51 FLOAT,
	@ONHAND_COST FLOAT,
	@MATTOT_PRCH_TOT FLOAT,
	@MATTOT_PRCH_PROMO_COST FLOAT,
	@MATTOT_SLS_TOT FLOAT,
	@MATTOT_SLS_COST FLOAT,
	@MATTOT_SLS_COST_BY_PERC FLOAT,
	@MATTOT_SLS_COST_BY_PRCH FLOAT,
	@ACC411 FLOAT,
	--Долгосрочные займы	
	@ACC521 FLOAT,
	--Расчеты с поставщиками
	@ACC534	FLOAT,		
	--Расчеты с учредителями
	@ACC642 FLOAT,
	--Доходы от текущей аренды
	@ACC655 FLOAT,
	--Проценты полученные
	@ACC728 FLOAT,
	--Прочие коммерческие расходы
	@ACC733 FLOAT,
	--Зарплата адмистративного и хозяйственного персонала
	@ACC735 FLOAT,
	--Налоги, сборы и платежи
	@ACC738 FLOAT,
	--Офисные расходы, расходы на связь
	@ACC742 FLOAT,
	--Расходы по текущей аренде
	@ACC747 FLOAT 
	--Недостачи и потери

	

	
	

-- cash IO
SELECT @CASHBEG = sum((CASE WHEN CH.DATE_ < @df THEN 1 ELSE 0 END) * (CH.DEBIT - CH.CREDIT)),
	@CASHIN = sum((
			CASE WHEN CH.DATE_ BETWEEN @df
						AND @dt THEN 1 ELSE 0 END
			) * (CH.DEBIT)),
	@CASHOUT = sum((
			CASE WHEN CH.DATE_ BETWEEN @df
						AND @dt THEN 1 ELSE 0 END
			) * (CH.CREDIT)),
	@CASHEND = sum((CASE WHEN CH.DATE_ <= @dt THEN 1 ELSE 0 END) * (CH.DEBIT - CH.CREDIT))
FROM LG_001_01_CSHTOTS CH WITH (NOLOCK)
LEFT JOIN LG_001_KSCARD KS WITH (NOLOCK) ON CH.CARDREF = KS.LOGICALREF
WHERE CH.TOTTYPE = 1
	AND KS.CODE = 'AZN'
	AND CH.DATE_ <= @dt

-- debit credit
DECLARE @CL_BALANCE TABLE(LOGICALREF INT,BALANCE FLOAT,IN_ FLOAT,OUT_ FLOAT)
INSERT INTO @CL_BALANCE
SELECT C.LOGICALREF,
	sum(isnull((
				CASE WHEN (
							CASE WHEN CLFLINE.MODULENR = 5
									AND CLFLINE.TRCODE = 14 THEN (CLFLINE.DATE_ - 1) ELSE CLFLINE.DATE_ END
							) <= @dt THEN 1 ELSE 0 END
				) * (CASE WHEN CLFLINE.SIGN = 0 THEN 1 WHEN CLFLINE.SIGN = 1 THEN - 1 ELSE 0 END) * (CLFLINE.AMOUNT), 0)) BALANCE,
				
	sum(isnull((
				CASE WHEN ( CLFLINE.DATE_ ) BETWEEN @df AND @dt THEN 1 ELSE 0 END
				) * (CASE WHEN CLFLINE.SIGN = 0 THEN 1 ELSE 0 END) * (CLFLINE.AMOUNT), 0)) IN_,
	sum(isnull((
				CASE WHEN ( CLFLINE.DATE_ ) BETWEEN @df AND @dt THEN 1 ELSE 0 END
				) * (CASE WHEN CLFLINE.SIGN = 1 THEN 1 ELSE 0 END) * (CLFLINE.AMOUNT), 0)) OUT_			
	
 
FROM LG_001_CLCARD C WITH (NOLOCK)
INNER JOIN LG_001_01_CLFLINE CLFLINE WITH (NOLOCK) ON CLFLINE.CLIENTREF = C.LOGICALREF
WHERE (CLFLINE.DATE_ <= @dt)
	AND (CLFLINE.CANCELLED = 0)
	AND C.CARDTYPE IN (1,2)
GROUP BY C.LOGICALREF

SELECT @CLVDEBIT = isnull(sum(CASE WHEN BALANCE > 0 THEN B.BALANCE ELSE 0 END),0),
	@CLVCREDIT = isnull(sum(CASE WHEN BALANCE < 0 THEN abs(B.BALANCE) ELSE 0 END),0),
	@CLVIN = isnull(sum(B.IN_),0),
	@CLVOUT = isnull(sum(B.OUT_),0)
FROM @CL_BALANCE B INNER JOIN LG_001_CLCARD C WITH (NOLOCK) ON B.LOGICALREF = C.LOGICALREF
WHERE C.CARDTYPE IN (1)

SELECT @CLDEBIT = isnull(sum(CASE WHEN BALANCE > 0 THEN B.BALANCE ELSE 0 END),0),
	@CLCREDIT = isnull(sum(CASE WHEN BALANCE < 0 THEN abs(B.BALANCE) ELSE 0 END),0),
	@CLIN = isnull(sum(B.IN_),0),
	@CLOUT = isnull(sum(B.OUT_),0)
FROM @CL_BALANCE B INNER JOIN LG_001_CLCARD C WITH (NOLOCK) ON B.LOGICALREF = C.LOGICALREF
WHERE C.CARDTYPE IN (2)



-- mat rest simple
SELECT @MATREM_SLSPRC = isnull((sum(ONHAND * PRICE_SALE)),0),
	@MATREM_PRCHPRC = isnull((sum(ONHAND * PRICE_PURCHASE)),0)
FROM (
	SELECT T.STOCKREF,
		SUM(ONHAND) ONHAND,
		ISNULL((
				SELECT TOP 1 PRICE_PURCHASE
				FROM @ITEMS PRC
				WHERE PRC.LOGICALREF = T.STOCKREF
				), 0) PRICE_PURCHASE,
		ISNULL((
				SELECT TOP 1 PRICE_SALE
				FROM @ITEMS PRC
				WHERE PRC.LOGICALREF = T.STOCKREF
				), 0) PRICE_SALE
	FROM LG_001_01_STINVTOT T WITH (NOLOCK)
	WHERE T.INVENNO = -1
		AND T.DATE_ <= @dt
	GROUP BY T.STOCKREF
	) D
WHERE D.ONHAND > 0



-- mat IO simple
SELECT @MATTOT_PRCH = isnull(sum((CASE WHEN F.TRCODE IN (1) THEN 1 ELSE 0 END) * F.NETTOTAL),0),
	@MATTOT_PRCHRET = isnull(sum((CASE WHEN F.TRCODE IN (6) THEN 1 ELSE 0 END) * F.NETTOTAL),0),
	@MATTOT_SLS = isnull(sum((CASE WHEN F.TRCODE IN (8) THEN 1 ELSE 0 END) * F.NETTOTAL),0),
	@MATTOT_SLSRET = isnull(sum((CASE WHEN F.TRCODE IN (3) THEN 1 ELSE 0 END) * F.NETTOTAL),0),
	@MATTOT_COUNT50 = isnull(sum((CASE WHEN F.TRCODE IN (50) THEN 1 ELSE 0 END) * F.NETTOTAL),0),
	@MATTOT_COUNT51 = isnull(sum((CASE WHEN F.TRCODE IN (51) THEN 1 ELSE 0 END) * F.NETTOTAL),0)
FROM LG_001_01_STFICHE F
WHERE F.DATE_ BETWEEN @df
		AND @dt
	AND F.CANCELLED = 0
	--AND F.SOURCEINDEX = 0

	




SELECT 
	@MATTOT_PRCH_TOT = isnull(SUM(ISNULL((CASE WHEN STLINE.LINETYPE IN (0) THEN 1 ELSE 0 END) * (CASE WHEN STLINE.TRCODE IN (1) THEN + 1 WHEN STLINE.TRCODE IN (6) THEN - 1 ELSE 0 END) * (STLINE.VATMATRAH + STLINE.VATAMNT + STLINE.DISTEXP),0)),0),
	@MATTOT_PRCH_PROMO_COST =isnull( SUM(ISNULL((CASE WHEN STLINE.LINETYPE IN (1) THEN 1 ELSE 0 END) * (CASE WHEN STLINE.TRCODE IN (1) THEN + 1 WHEN STLINE.TRCODE IN (6) THEN - 1 ELSE 0 END) * (((STLINE.AMOUNT * STLINE.UINFO2 / STLINE.UINFO1) * STLINE.OUTCOST)),0)) ,0),
    @MATTOT_SLS_TOT = isnull(SUM(ISNULL((CASE WHEN STLINE.LINETYPE IN (0) THEN 1 ELSE 0 END) * (CASE WHEN STLINE.TRCODE IN (7, 8) THEN + 1 WHEN STLINE.TRCODE IN (2, 3) THEN - 1 ELSE 0 END) * (STLINE.VATMATRAH + STLINE.VATAMNT + STLINE.DISTEXP),0)),0),
	@MATTOT_SLS_COST = isnull(SUM(ISNULL((CASE WHEN STLINE.LINETYPE IN (0, 1) THEN 1 ELSE 0 END) * (CASE WHEN STLINE.TRCODE IN (7, 8) THEN + 1 WHEN STLINE.TRCODE IN (2, 3) THEN - 1 ELSE 0 END) * (((STLINE.AMOUNT * STLINE.UINFO2 / STLINE.UINFO1) * STLINE.OUTCOST)),0)),0),
	@MATTOT_SLS_COST_BY_PERC = isnull(SUM(ISNULL(((100/(100+I.FLOATF1))) * (CASE WHEN STLINE.LINETYPE IN (0) THEN 1 ELSE 0 END) * (CASE WHEN STLINE.TRCODE IN (7, 8) THEN + 1 WHEN STLINE.TRCODE IN (2, 3) THEN - 1 ELSE 0 END) * (STLINE.VATMATRAH + STLINE.VATAMNT + STLINE.DISTEXP),0)),0),
	@MATTOT_SLS_COST_BY_PRCH = isnull(SUM(ISNULL((CASE WHEN STLINE.LINETYPE IN (0, 1) THEN 1 ELSE 0 END) * (CASE WHEN STLINE.TRCODE IN (7, 8) THEN + 1 WHEN STLINE.TRCODE IN (2, 3) THEN - 1 ELSE 0 END) * ( ( (STLINE.AMOUNT * STLINE.UINFO2 / STLINE.UINFO1) * I.PRICE_PURCHASE)),0)),0)
FROM @ITEMS I INNER JOIN LG_001_01_STLINE STLINE WITH (
		NOLOCK,
		INDEX = I001_01_STLINE_I19
		) ON I.LOGICALREF = STLINE.STOCKREF
WHERE 
	 STLINE.DATE_ BETWEEN @df
		AND @dt
		AND STLINE.FTIME >= 0 
	AND STLINE.TRCODE IN (7, 8, 2, 3,1,6)
	AND STLINE.CANCELLED = 0
	AND STLINE.LINETYPE IN (0,1)
	

SELECT @ACC642 = sum(ISNULL((
				CASE WHEN TRCODE = 11
						AND SPECODE LIKE '642%' THEN AMOUNT ELSE 0 END
				), 0)),
	@ACC655 = sum(ISNULL((
				CASE WHEN TRCODE = 11
						AND SPECODE LIKE '655%' THEN AMOUNT ELSE 0 END
				), 0)),
	@ACC411 = sum(ISNULL((
				CASE WHEN TRCODE = 11
						AND SPECODE LIKE '411%' THEN AMOUNT ELSE 0 END
				), 0)),
	@ACC521 = sum(ISNULL((
				CASE WHEN TRCODE = 12
						AND SPECODE LIKE '521%' THEN AMOUNT ELSE 0 END
				), 0)),
	@ACC534 = sum(ISNULL((
				CASE WHEN TRCODE = 12
						AND SPECODE LIKE '534%' THEN AMOUNT ELSE 0 END
				), 0)),
	@ACC728 = sum(ISNULL((
				CASE WHEN TRCODE = 12
						AND SPECODE LIKE '728%' THEN AMOUNT ELSE 0 END
				), 0)),
	@ACC733 = sum(ISNULL((
				CASE WHEN TRCODE = 12
						AND SPECODE LIKE '733%' THEN AMOUNT ELSE 0 END
				), 0)),
	@ACC735 = sum(ISNULL((
				CASE WHEN TRCODE = 12
						AND SPECODE LIKE '735%' THEN AMOUNT ELSE 0 END
				), 0)),
	@ACC738 = sum(ISNULL((
				CASE WHEN TRCODE = 12
						AND SPECODE LIKE '738%' THEN AMOUNT ELSE 0 END
				), 0)),
	@ACC742 = sum(ISNULL((
				CASE WHEN TRCODE = 12
						AND SPECODE LIKE '742%' THEN AMOUNT ELSE 0 END
				), 0)),
	@ACC747 = sum(ISNULL((
				CASE WHEN TRCODE = 12
						AND SPECODE LIKE '747%' THEN AMOUNT ELSE 0 END
				), 0))
FROM LG_001_01_KSLINES WITH (NOLOCK)
WHERE CANCELLED = 0
	AND DATE_ BETWEEN @df
		AND @dt
	AND CANCELLED = 0
	AND TRCODE IN (11, 12)
	
INSERT INTO @RES
VALUES
 
	('CASHBEG',N'Kassa İlk Qalıq', @CASHBEG),
	('CASHIN',N'Kassaya Qələn Pul', @CASHIN),
	('CASHOUT',N'Kassadan Qedən Pun', @CASHOUT),
	('CASHEND',N'Kassa Son Qalıq', @CASHEND),
	('SEP',N' ', 0),
	('CLDEBIT',N'Tedarukculərin Borcu', @CLDEBIT),
	('CLCREDIT',N'Tedarukculərə Borc', @CLCREDIT),
	('CLIN',N'Tedarukculərə Borc Azalması', @CLIN),
	('CLOUT',N'Tedarukculərə Borc Coxalması', @CLOUT),
	('SEP',N' ', 0),
	('CLVDEBIT',N'Alıcı Borcu', @CLVDEBIT),
	('CLVCREDIT',N'Alıcılara Borc', @CLVCREDIT),
	('CLVIN',N'Alıcıların Borc Coxalması', @CLVIN),
	('CLVOUT',N'Alıcıların Borc Azalması', @CLVOUT),
	('SEP',N' ', 0),
	('MATREM_SLSPRC',N'Mal Qalıqı Satış Qiymətnən', @MATREM_SLSPRC),
	('MATREM_PRCHPRC',N'Mal Qalıqı Alış Qiymətnən', @MATREM_PRCHPRC),
	('MATREM_PROFIT',N'Mal Qalıqında (Satış-Alış) Fərq', @MATREM_SLSPRC- @MATREM_PRCHPRC),
	('SEP',N' ', 0),
	('MATTOT_PRCH',N'Mal Alışı Qros', @MATTOT_PRCH),
	('MATTOT_PRCHRET',N'Mal Alışın Qaytarılması', @MATTOT_PRCHRET),
	('MATTOT_PRCHNET',N'Mal Alışın Net', @MATTOT_PRCH-@MATTOT_PRCHRET),
	('SEP',N'', 0),
	('MATTOT_SLS',N'Mal Satışı Qros', @MATTOT_SLS),
	('MATTOT_SLSRET',N'Mal Satışı Qaytarılması', @MATTOT_SLSRET),
	('MATTOT_SLSNET',N'Mal Satışı Net', @MATTOT_SLS - @MATTOT_SLSRET),
	('SEP',N' ', 0),
	('MATTOT_COUNT50',N'Mal Sayım Artığı', @MATTOT_COUNT50),
	('MATTOT_COUNT51',N'Mal Sayım Əksiyi', @MATTOT_COUNT51) ,
	('SEP',N' ', 0),
	--('ONHAND_COST',N'ONHAND_COST', @ONHAND_COST),
	  ('MATTOT_SLS_TOT',N'Mal Satışı', @MATTOT_SLS_TOT),
	--('MATTOT_SLS_COST',N'MATTOT_SLS_COST', @MATTOT_SLS_COST),
	--('MATTOT_SLS_COST_BY_PERC',N'MATTOT_SLS_COST_BY_PERC', @MATTOT_SLS_COST_BY_PERC),
	--('MATTOT_PRCH_TOT',N'MATTOT_PRCH_TOT', @MATTOT_PRCH_TOT),
	--('MATTOT_PRCH_PROMO_COST',N'MATTOT_PRCH_PROMO_COST', @MATTOT_PRCH_PROMO_COST),
	  ('MATTOT_SLS_COST_BY_PRCH',N'Mal Satışı Alış Qiymətnən', @MATTOT_SLS_COST_BY_PRCH),
	  ('MATTOT_SLS_COST_BY_PRCH',N'Qiymət Fərqi Əsasında Qazanc', @MATTOT_SLS_TOT - @MATTOT_SLS_COST_BY_PRCH)
	
	--('ACC411',N'ACC411', @ACC411),
	--Долгосрочные займы
	--('ACC521',N'ACC521', @ACC521),
	--Расчеты с поставщиками
	--('ACC534',N'ACC534', @ACC534),
	--Расчеты с учредителями
	--('ACC642',N'ACC642', @ACC642),
	--Доходы от текущей аренды
	--('ACC655',N'ACC655', @ACC655),
	--Проценты полученные
	--('ACC728',N'ACC728', @ACC728),
	--Прочие коммерческие расходы
	--('ACC733',N'ACC733', @ACC733),
	--Зарплата адмистративного и хозяйственного персонала
	--('ACC735',N'ACC735', @ACC735),
	--Налоги, сборы и платежи
	--('ACC738',N'ACC738', @ACC738),
	--Офисные расходы, расходы на связь
	--('ACC742',N'ACC742', @ACC742),
	--Расходы по текущей аренде
	--('ACC747',N'ACC747', @ACC747)
	--Недостачи и потери
	

select CODE,DESC_,ISNULL(VALUE_,0) VALUE_ from @RES

		";
		
#endregion
			
			
			
			
			
			
			
		}
 
        #endregion



        #endregion
