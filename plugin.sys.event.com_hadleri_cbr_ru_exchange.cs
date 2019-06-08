#line 2
 

   #region BODY




        const int VERSION = 2;

        #region SETTINGS



        #endregion


        const string BANK_RATES_TODOC_EVENT_CODE = "com_hadleri_cbr_ru_to_doc";
        const string BANK_RATES_TODB_EVENT_CODE = "com_hadleri_cbr_ru_to_db";

        public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
        {

            object arg1 = ARGS.Length > 0 ? ARGS[0] : null;
            object arg2 = ARGS.Length > 1 ? ARGS[1] : null;
            object arg3 = ARGS.Length > 2 ? ARGS[2] : null;

            string[] list_ = EXPLODELISTPATH(EVENTCODE);

            switch (list_.Length > 0 ? list_[0] : "")
            {

                case SysEvent.SYS_NEWFORM:
                    MY_SYS_NEWFORM_HANDLER(arg1 as Form);
                    break;
                case SysEvent.SYS_USEREVENT:
                    MY_SYS_USEREVENT_HANDLER(EVENTCODE, ARGS);
                    break;

            }



        }




        void MY_SYS_NEWFORM_HANDLER(Form FORM) //user init new form//only for customize able forms
        {
            if (FORM == null) return;

            var fn = GETFORMNAME(FORM);

            var isStockAdpForm = false;
            var isExchRefForm = false;
            //
            if ((fn.StartsWith("adp.mm.doc.slip") || fn.StartsWith("adp.sls.doc.inv") || fn.StartsWith("adp.prch.doc.inv")))
                isStockAdpForm = true;

            if ((fn.StartsWith("adp.sls.doc.order") || fn.StartsWith("adp.prch.doc.order")))
                isStockAdpForm = true;

            if (fn.StartsWith("ref.gen.rec.exchange"))
                isExchRefForm = true;

            if (isStockAdpForm)
            {

                var cPanelBtnSub = CONTROL_SEARCH(FORM, "cPanelBtnSub");

                if (cPanelBtnSub == null)
                    return;


                MY_SYS_NEWFORM_INTEGRATE(cPanelBtnSub, BANK_RATES_TODOC_EVENT_CODE);


            }

            if (isExchRefForm)
            {


                var cPanelBtnSub = CONTROL_SEARCH(FORM, "cPanelBtnSub");

                if (cPanelBtnSub == null)
                    return;


                MY_SYS_NEWFORM_INTEGRATE(cPanelBtnSub, BANK_RATES_TODB_EVENT_CODE);

            }



        }


        void MY_SYS_NEWFORM_INTEGRATE(Control pCtrl, string pEvent)
        {
            if (pCtrl == null)
                return;
            try
            {


                var args = new Dictionary<string, object>() { 
            
			{ "_cmd" ,""},
            { "_type" ,""},
			{ "CmdText" ,"event name::"+pEvent},
			{ "Text" ,"CBR"},
			{ "ImageName" ,"money_16x16"},
			
            };

                RUNUIINTEGRATION(pCtrl, args);


            }
            catch (Exception exc)
            {
                MSGUSERERROR("Cant add button");
            }

        }




        public void MY_SYS_USEREVENT_HANDLER(string EVENTCODE, object[] ARGS) //adapter start
        {
            //"SYS_USEREVENT/exchange_rates_cbr_ru"


            try
            {


                object arg1 = ARGS.Length > 0 ? ARGS[0] : null;
                object arg2 = ARGS.Length > 1 ? ARGS[1] : null;

                string[] list_ = EXPLODELISTPATH(EVENTCODE);

                switch (list_.Length > 1 ? list_[1].ToLowerInvariant() : "")
                {

                    case BANK_RATES_TODOC_EVENT_CODE:
                        {

                            var f = arg1 as Form;
                            //var b = arg2 as Button;
                            if (f != null)
                            {
                                var ds_ = GETDATASETFROMADPFORM(f);
                                if (ds_ != null)
                                {

                                    var tab = ds_.Tables["INVOICE"];
                                    if (tab == null)
                                        tab = ds_.Tables["STFICHE"];

                                    // has plugin and allowed ceheck
                                    //
                                    var r1 = MY_GET_RATE(CASTASSHORT(TAB_GETROW(tab, "TRCURR")), null);
                                    var r2 = MY_GET_RATE(GETSYSPRM_CURRENCYREP(), null);
                                    //
                                    TAB_SETROW(tab, "TRRATE", r1);
                                    TAB_SETROW(tab, "REPORTRATE", r2);

                                }
                            }
                        }
                        break;
                    case BANK_RATES_TODB_EVENT_CODE:
                        {

                            MY_GET_RATE(1, "USD", true);
                            MY_GET_RATE(20, "EUR", true);


                            RUNUIINTEGRATION(arg1 as Form, "_cmd", "last");//refresh, first

                            MSGUSERINFO("T_MSG_OPERATION_FINISHED");







                        }
                        break;
                }


            }
            catch (Exception exc)
            {
                LOG(exc);
                MSGUSERERROR("T_MSG_OPERATION_FAILED");
                MSGUSERERROR(exc.Message);
            }
        }









        double MY_GET_RATE(int pCurrId, string pCurrCode)
        {
            return MY_GET_RATE(pCurrId, pCurrCode, false);
        }


        double MY_GET_RATE(int pCurrId, string pCurrCode, bool pUpdate)
        {

            if (pCurrId == 0)
                return 1.0;

            if ((pCurrCode == "" || pCurrCode == null))
            {
                switch (pCurrId)
                {
                    case 1: pCurrCode = "USD"; break;
                    case 20: pCurrCode = "EUR"; break;
                }

            }

            // Last values
            string currUrl = "http://www.cbr.ru/scripts/XML_daily.asp";// "http://www.cbr.ru/scripts/XML_daily.asp";//?date_req=02/03/2002


            var xmlDoc = new XmlDocument();
            xmlDoc.Load(currUrl);

            // Xml içinden tarihi alma - gerekli olabilir
            DateTime exchangeDate = Convert.ToDateTime(xmlDoc.SelectSingleNode("//ValCurs").Attributes["Date"].Value);

            var nodeCurr = xmlDoc.SelectSingleNode("ValCurs/Valute/CharCode[.='" + pCurrCode + "']");
            if (nodeCurr == null || nodeCurr.ParentNode == null)
                throw new Exception("Node for " + pCurrCode + " not exists");

            nodeCurr = nodeCurr.ParentNode;

            var _Nominal = MAX(1.0, CASTASDOUBLE(CASTASSTRING(nodeCurr["Nominal"].InnerText).Replace(',', '.')));
            var _Value = CASTASDOUBLE(CASTASSTRING(nodeCurr["Value"].InnerText).Replace(',', '.'));

            var dateCurr = exchangeDate;
            var currRate = _Value;


            if (pUpdate)
            {

                var now = DateTime.Now;
                var nowInt = GETDATETODATEINT(DateTime.Now.Date);
                var currId = pCurrId;
                // var currRate = currRate; 

                if (ISNULL(SQLSCALAR("select 1 from L_DAILYEXCHANGES where DATE_ = @P1 and CRTYPE = @P2", new object[] { nowInt, pCurrId })))
                {

                    SQLSCALAR(@"
  INSERT INTO L_DAILYEXCHANGES (LREF,DATE_,CRTYPE,RATES1,RATES2,RATES3,RATES4,EDATE)  
VALUES (NEXTVAL('LREF_L_DAILYEXCHANGES'),@P1,@P2,@P3,0,0,0,@P4)
",

                        new object[] { nowInt, currId, currRate, now });

                }
                else
                {
                    SQLSCALAR(" update L_DAILYEXCHANGES set RATES1 = @P1,EDATE = @P2 where DATE_ = @P3 and CRTYPE = @P4",
                        new object[] { currRate, now, nowInt, currId });


                }




            }

            return currRate;

        }


        #endregion