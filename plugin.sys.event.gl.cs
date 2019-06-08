#line 2
  
  
        #region PLUGIN_BODY

        const int VERSION = 11;



        const bool _useJournal = true;
        const string JOURNAL_NS = "gl";

        public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
        {

            object arg0 = ARGS.Length > 0 ? ARGS[0] : null;
            object arg1 = ARGS.Length > 1 ? ARGS[1] : null;
            object arg2 = ARGS.Length > 2 ? ARGS[2] : null;

            string[] list_ = EXPLODELISTPATH(EVENTCODE);

            switch (list_.Length > 0 ? list_[0] : "")
            {

                case SysEvent.SYS_GL:
                    MY_SYS_GL(arg0 as DataSet, arg1 as DataSet);
                    break;

            }



        }


        public void MY_SYS_GL(DataSet DATASETS, DataSet DATASETD)
        {



            MY_GLREL.CLEAN_CACHE();

            //1
            {
                var t = DATASETS.Tables["INVOICE"];
                if (t != null)
                {
                    new GLDO_STOCK(this).MY_GL(DATASETS, DATASETD);
                    return;
                }
            }
            //2
            {
                var t = DATASETS.Tables["STFICHE"];
                if (t != null)
                {
                    new GLDO_STOCK(this).MY_GL(DATASETS, DATASETD);
                    return;
                }
            }
            //3
            {
                var t = DATASETS.Tables["KSLINES"];
                if (t != null)
                {
                    new GLDO_CASH(this).MY_GL(DATASETS, DATASETD);
                    return;
                }
            }
            //4
            {
                var t = DATASETS.Tables["BNFLINE"];
                if (t != null)
                {
                    new GLDO_BANK(this).MY_GL(DATASETS, DATASETD);
                    return;
                }
            }
            //5
            {
                var t = DATASETS.Tables["CLFICHE"];
                if (t != null)
                {
                    new GLDO_CLIENT(this).MY_GL(DATASETS, DATASETD);
                    return;
                }
            }
            //6
            {
                var t = DATASETS.Tables["COSTDISTFC"];
                if (t != null)
                {
                    new GLDO_COSTDIST(this).MY_GL(DATASETS, DATASETD);
                    return;
                }
            }
        }


        #region CLASS

        #region STOCK
        class GLDO_STOCK
        {
            public GLDO_STOCK(_PLUGIN pPLUGIN)
            {
                PLUGIN = pPLUGIN;
            }
            _PLUGIN PLUGIN;


            class GL_RECORD : Dictionary<string, object>
            {

            }
            class STOCK_GL_TRANS_DESC
            {
                public STOCK_GL_TRANS_DESC(DataRow pRecord, DataRow pParent, STOCK_GL_TRANS_TYPE pType)
                {
                    record = pRecord;
                    parent = pParent;
                    type = pType;
                }
                public STOCK_GL_TRANS_TYPE type;
                public DataRow record;
                public DataRow parent;
            }
            enum STOCK_GL_TRANS_TYPE
            {
                material = 1,
                service = 2,
                VATMaterial = 3,
                VATMaterialSurcharge = 4,
                VATService = 5,
                VATServiceSurcharge = 6,
                discountGlobal = 7,
                discountMaterial = 8,
                discountService = 9,
                discountPromotion = 10,
                surchargeGlobal = 11,
                surchargeMaterial = 12,
                surchargeService = 13,
                promoGlobal = 14,
                promoMaterial = 15,
                promoService = 16,
                VATSurchargeGlobal = 17,
                materialPair = 18,
                VATMaterialPair = 19
            }
            enum STOCK_TRANS_TYPE
            {
                material = 0,
                promotion = 1,
                discount = 2,
                surcharge = 3,
                service = 4,
                deposit = 5,
                fixedAsset = 8,
                subcontracting = 11
            }
            enum GLREL_TRCODE
            {
                undef = 0,
                material = 1,
                personalCarad = 2,
                service = 3,
                VAT = 4,
                cash = 5,
                bank = 6,
                discount = 7,
                surcharge = 8,
                promo = 9

            }
            class GLREL_RELCODE1
            {
                public const string EMPTY = "";
                public const string MATSURCH = "MATSURCH";
                public const string SRVSURCH = "SRVSURCH";
                public const string SURCH = "SURCH";
                public const string PROMO = "PROMO";
                public const string MAT = "MAT";
                public const string SRV = "SRV";
                public const string PAIR = "PAIR";
            }
            public void MY_GL(DataSet DATASETS, DataSet DATASETD)
            {
                DataTable dataHeader_;
                DataTable dataLines_;

                DataTable glHeader_;
                DataTable glLines_;

                dataHeader_ = MY_GETTAB(DATASETS, TABLE_INVOICE.TABLE) != null ? MY_GETTAB(DATASETS, TABLE_INVOICE.TABLE) : MY_GETTAB(DATASETS, TABLE_STFICHE.TABLE);
                dataLines_ = MY_GETTAB(DATASETS, TABLE_STLINE.TABLE);

                glHeader_ = MY_GETTAB(DATASETD, TABLE_EMFICHE.TABLE);
                glLines_ = MY_GETTAB(DATASETD, TABLE_EMFLINE.TABLE);

                MY_CHECK_TABLES(dataHeader_, dataLines_, glHeader_, glLines_);
                MY_SYNC_HEADERS(dataHeader_, glHeader_);
                MY_PARSE_DATA(glHeader_, glLines_, MY_FORMAT_DATA(dataHeader_, dataLines_));

            }
            void MY_JOURNAL_WRITE(string[] pTables, object[] pArr)
            {
                if (!_useJournal)
                    return;
                PLUGIN.JOURNAL(JOURNAL_NS,
                string.Format(PLUGIN.RESOLVESTR("$search\t{0}/{1}"),
                JOINLIST(pTables),
                JOINLIST(FORMAT(pArr))));
            }
            void MY_JOURNAL_WRITE(DataTable pDocHeader)
            {
                if (!_useJournal)
                    return;
                DataRow headerRow_ = TAB_GETLASTROW(pDocHeader);
                PLUGIN.JOURNAL(JOURNAL_NS,
                    string.Format(PLUGIN.RESOLVESTR(Environment.NewLine + "$doc\t({0}/{1}/{2})\t[lang::T_TYPE],{3};[lang::T_NR],{4}"),
                    TABLE_STOCKHEADER.TABLE,
                    TABLE_STOCKHEADER.LOGICALREF,
                    TAB_GETROW(headerRow_, TABLE_STOCKHEADER.LOGICALREF),
                    MY_GLREL_DOCCODE(headerRow_),
                    TAB_GETROW(headerRow_, TABLE_STOCKHEADER.FICHENO)));
            }
            void MY_JOURNAL_WRITE(DataRow pTrans)
            {
                if (!_useJournal)
                    return;

                PLUGIN.JOURNAL(JOURNAL_NS,
                    string.Format(PLUGIN.RESOLVESTR("$line\t({0}/{1}/{2})"),
                    pTrans.Table.TableName,
                    TABLE_STOCKHEADER.LOGICALREF,
                    TAB_GETROW(pTrans, TABLE_STOCKHEADER.LOGICALREF)));
            }

            void MY_PARSE_DATA(DataTable pGlHeader, DataTable pGlLines, GL_RECORD[] pRecords)
            {
                foreach (GL_RECORD rec_ in pRecords)
                {
                    TAB_ADDROW(pGlLines);
                    foreach (KeyValuePair<string, object> pair_ in rec_)
                        TAB_SETROW(pGlLines, pair_.Key, pair_.Value);
                }

            }

            GL_RECORD[] MY_FORMAT_DATA(DataTable pDataHeader, DataTable pDataLines)
            {
                List<GL_RECORD> list_ = new List<GL_RECORD>();
                //Heaer as trans
                MY_FORMAT_DOC_HEADER(list_, pDataHeader, pDataLines);
                //Lines as trans
                MY_FORMAT_DOC_LINES(list_, pDataHeader, pDataLines);
                return list_.ToArray();
            }

            //string MY_GET_ACC_SEARCH_SQL(string pDoc, string pTrans, string pCard)
            //{

            //    return MY_GLREL.MY_GET_ACC_SEARCH_SQL(  pDoc,   pTrans,   pCard);


            //    string sql_ =
            //    "DECLARE" + "\n" +
            //    "@TRCODE smallint," + "\n" +
            //    "@DOCCODE nvarchar(25)," + "\n" +
            //    "@RELCODE1 nvarchar(25)," + "\n" +
            //    "@RELCODE2 nvarchar(25)," + "\n" +
            //    "@RELCODE3 nvarchar(25)," + "\n" +
            //    "@RELCODE4 nvarchar(25)," + "\n" +
            //    "@RELCODE5 nvarchar(25)," + "\n" +
            //    "@RELCODE6 nvarchar(25)," + "\n" +
            //    "@RELCODE7 nvarchar(25)" + "\n" +
            //    "SELECT" + "\n" +
            //    "@TRCODE = @P1," + "\n" +
            //    "@DOCCODE = @P2," + "\n" +
            //    "@RELCODE1 = @P3," + "\n" +
            //    "@RELCODE2 = [dbo].[f_GLREL_$FIRM$_$PERIOD$_DOC_{0}] (@P4,1,0)," + "\n" +
            //    "@RELCODE3 = [dbo].[f_GLREL_$FIRM$_$PERIOD$_DOC_{0}] (@P5,2,0)," + "\n" +
            //    "@RELCODE4 = [dbo].[f_GLREL_$FIRM$_$PERIOD$_TRANS_{1}] (@P6,1,0)," + "\n" +
            //    "@RELCODE5 = [dbo].[f_GLREL_$FIRM$_$PERIOD$_TRANS_{1}] (@P7,2,0)," + "\n" +
            //    "@RELCODE6 = [dbo].[f_GLREL_$FIRM$_CARD_{2}] (@P8,1,0)," + "\n" +
            //    "@RELCODE7 = [dbo].[f_GLREL_$FIRM$_CARD_{2}] (@P9,2,0)" + "\n" +
            //    "EXEC [dbo].[p_GLREL_$FIRM$_GET_ACC_FROM_REL] " + "\n" +
            //    "@TRCODE," + "\n" +
            //    "@DOCCODE," + "\n" +
            //    "@RELCODE1," + "\n" +
            //    "@RELCODE2," + "\n" +
            //    "@RELCODE3," + "\n" +
            //    "@RELCODE4," + "\n" +
            //    "@RELCODE5," + "\n" +
            //    "@RELCODE6," + "\n" +
            //    "@RELCODE7" + "\n";

            //    return string.Format(sql_, pDoc, pTrans, pCard);
            //}
            object MY_GET_CLIENT_ACC(DataRow pDocRow, DataRow pTransRow)
            {


                string[] tablesArr_ = new string[] { pDocRow.Table.TableName, pTransRow.Table.TableName, TABLE_CLCARD.TABLE };

                string sql_ = MY_GLREL.MY_GET_ACC_SEARCH_SQL(tablesArr_[0], tablesArr_[1], tablesArr_[2]);

                object docLRef_ = TAB_GETROW(pDocRow, TABLE_STOCKHEADER.LOGICALREF);
                object transLRef_ = TAB_GETROW(pTransRow, TABLE_STOCKHEADER.LOGICALREF);
                object cardLRef_ = TAB_GETROW(pTransRow, TABLE_STOCKHEADER.CLIENTREF);



                //    object[] var_ = new object[]{
                //    (short)GLREL_TRCODE.personalCarad,
                //    MY_GLREL_DOCCODE(pDocRow),
                //    "",
                //    docLRef_,
                //    docLRef_,
                //    transLRef_,
                //    transLRef_,
                //    cardLRef_,
                //    cardLRef_
                //};

                //    MY_JOURNAL_WRITE(tablesArr_, var_);

                //    object acc_ = PLUGIN.SQLSCALAR(sql_, var_);

                //    MY_CHECK_ACC(acc_);
                //    return acc_;


                return MY_GLREL.MY_GET_ACC(PLUGIN, sql_,
                        (short)GLREL_TRCODE.personalCarad,
                  MY_GLREL_DOCCODE(pDocRow),
                  "",
                  docLRef_,
                  docLRef_,
                  transLRef_,
                  transLRef_,
                  cardLRef_,
                  cardLRef_

                      );



            }

            void MY_CHECK_ACC(object pLRef)
            {
                if (CASTASINT(ISNULL(pLRef, 0)) == 0)
                    throw MY_EXCEPTION_INVALID_LREF(TABLE_EMUHACC.TABLE, TABLE_EMUHACC.LOGICALREF, pLRef);
            }

            object MY_GET_STOCK_ACC(DataRow pDocRow, DataRow pTransRow, STOCK_GL_TRANS_TYPE pTransType, GLREL_TRCODE pGLRelType, string pGLRelFilterCode)
            {
                string cardTable_ = "";
                switch (pTransType)
                {

                    case STOCK_GL_TRANS_TYPE.material:
                    case STOCK_GL_TRANS_TYPE.promoGlobal:
                    case STOCK_GL_TRANS_TYPE.promoMaterial:
                    case STOCK_GL_TRANS_TYPE.promoService:
                    case STOCK_GL_TRANS_TYPE.VATMaterial:
                    case STOCK_GL_TRANS_TYPE.discountPromotion:
                    case STOCK_GL_TRANS_TYPE.materialPair:
                    case STOCK_GL_TRANS_TYPE.VATMaterialPair:
                        cardTable_ = TABLE_ITEMS.TABLE;
                        break;
                    case STOCK_GL_TRANS_TYPE.service:
                    case STOCK_GL_TRANS_TYPE.VATService:
                        cardTable_ = TABLE_SRVCARD.TABLE;
                        break;
                    case STOCK_GL_TRANS_TYPE.VATSurchargeGlobal:
                    case STOCK_GL_TRANS_TYPE.VATMaterialSurcharge:
                    case STOCK_GL_TRANS_TYPE.VATServiceSurcharge:
                    case STOCK_GL_TRANS_TYPE.discountGlobal:
                    case STOCK_GL_TRANS_TYPE.discountMaterial:
                    case STOCK_GL_TRANS_TYPE.discountService:
                    case STOCK_GL_TRANS_TYPE.surchargeGlobal:
                    case STOCK_GL_TRANS_TYPE.surchargeMaterial:
                    case STOCK_GL_TRANS_TYPE.surchargeService:
                        cardTable_ = TABLE_DECARDS.TABLE;
                        break;
                    default:
                        throw MY_EXCEPTION_INVALID_VAR(typeof(STOCK_GL_TRANS_TYPE).Name, pTransType);
                }

                string[] tablesArr_ = new string[] { pDocRow.Table.TableName, pTransRow.Table.TableName, cardTable_ };
                string sql_ = MY_GLREL.MY_GET_ACC_SEARCH_SQL(tablesArr_[0], tablesArr_[1], tablesArr_[2]);

                object docLRef_ = TAB_GETROW(pDocRow, TABLE_STOCKHEADER.LOGICALREF);
                object transLRef_ = TAB_GETROW(pTransRow, TABLE_STOCKHEADER.LOGICALREF);
                object cardLRef_ = TAB_GETROW(pTransRow, TABLE_STLINE.STOCKREF);


                //    object[] var_ = new object[]{
                //    (short)pGLRelType,
                //    MY_GLREL_DOCCODE(pDocRow),
                //    pGLRelFilterCode,
                //    docLRef_,
                //    docLRef_,
                //    transLRef_,
                //    transLRef_,
                //    cardLRef_,
                //    cardLRef_
                //};
                //    //
                //    MY_JOURNAL_WRITE(tablesArr_, var_);
                //    //
                //    object acc_ = PLUGIN.SQLSCALAR(sql_, var_);
                //    MY_CHECK_ACC(acc_);
                //    return acc_;


                return MY_GLREL.MY_GET_ACC(PLUGIN, sql_,

                     (short)pGLRelType,
             MY_GLREL_DOCCODE(pDocRow),
             pGLRelFilterCode,
             docLRef_,
             docLRef_,
             transLRef_,
             transLRef_,
             cardLRef_,
             cardLRef_
                 );



            }
            string MY_GET_CLIENT_ACC_COL(DataRow pTransRow)
            {
                short trcode_ = CASTASSHORT(TAB_GETROW(pTransRow, TABLE_STOCKHEADER.TRCODE));
                bool res_ = false;
                switch (trcode_)
                {
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        res_ = true;
                        break;
                    case 1:
                    case 4:
                    case 2:
                    case 3:
                        res_ = false;
                        break;
                    default:
                        throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_INVOICE.TABLE, TABLE_STOCKHEADER.TRCODE, trcode_);
                }
                return MY_ACC_SIDE_TO_COL(res_);
            }
            string MY_GET_STOCK_ACC_COL(DataRow pTransRow, STOCK_GL_TRANS_TYPE pTransType)
            {
                short trcode_ = CASTASSHORT(TAB_GETROW(pTransRow, TABLE_STLINE.TRCODE));
                bool res_ = MY_GET_STOCK_TRANS_MAT_SIDE(pTransRow);

                switch (pTransType)
                {
                    case STOCK_GL_TRANS_TYPE.material:
                    case STOCK_GL_TRANS_TYPE.promoGlobal:
                    case STOCK_GL_TRANS_TYPE.promoMaterial:
                    case STOCK_GL_TRANS_TYPE.promoService:
                    case STOCK_GL_TRANS_TYPE.VATMaterial:
                    case STOCK_GL_TRANS_TYPE.service:
                    case STOCK_GL_TRANS_TYPE.VATService:
                    case STOCK_GL_TRANS_TYPE.VATSurchargeGlobal:
                    case STOCK_GL_TRANS_TYPE.VATMaterialSurcharge:
                    case STOCK_GL_TRANS_TYPE.VATServiceSurcharge:
                    case STOCK_GL_TRANS_TYPE.surchargeGlobal:
                    case STOCK_GL_TRANS_TYPE.surchargeMaterial:
                    case STOCK_GL_TRANS_TYPE.surchargeService:
                        // res_ = res_;
                        break;
                    case STOCK_GL_TRANS_TYPE.discountPromotion:
                    case STOCK_GL_TRANS_TYPE.discountGlobal:
                    case STOCK_GL_TRANS_TYPE.discountMaterial:
                    case STOCK_GL_TRANS_TYPE.discountService:
                    case STOCK_GL_TRANS_TYPE.materialPair:
                    case STOCK_GL_TRANS_TYPE.VATMaterialPair:
                        res_ = !res_;
                        break;
                    default:
                        throw MY_EXCEPTION_INVALID_VAR(typeof(STOCK_GL_TRANS_TYPE).Name, pTransType);
                }


                return MY_ACC_SIDE_TO_COL(res_);
            }
            bool MY_GET_STOCK_TRANS_MAT_SIDE(DataRow pTransRow)
            {
                short trcode_ = CASTASSHORT(TAB_GETROW(pTransRow, TABLE_STLINE.TRCODE));
                short iocode_ = CASTASSHORT(TAB_GETROW(pTransRow, TABLE_STLINE.IOCODE));
                bool res_ = false;


                switch (trcode_)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 13:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 50:
                        res_ = true;
                        break;
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 11:
                    case 12:
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 51:
                        res_ = false;
                        break;
                    case 25:
                        if (MY_TRANS_IS_STOCK_IN(pTransRow))
                            res_ = true;
                        else
                            res_ = false;

                        break;
                    default:
                        throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_STLINE.TABLE, TABLE_STLINE.TRCODE, trcode_);

                }
                return res_;


            }
            string MY_ACC_SIDE_TO_COL(bool pAccSide)
            {
                return pAccSide ? TABLE_EMFLINE.DEBIT : TABLE_EMFLINE.CREDIT;
            }
            string MY_GLREL_DOCCODE(DataRow pDocRow)
            {
                string prefix_ = (pDocRow.Table.TableName == TABLE_INVOICE.TABLE ? "INV" : "SLIP");
                string nr_ = FORMAT(TAB_GETROW(pDocRow, TABLE_STOCKHEADER.TRCODE));
                return prefix_ + "." + nr_;
            }
            void MY_FORMAT_DOC_HEADER(List<GL_RECORD> pList, DataTable pDataHeader, DataTable pDataLines)
            {
                if (pDataHeader.TableName != TABLE_INVOICE.TABLE)
                    return;
                DataRow headerRecord_ = TAB_GETLASTROW(pDataHeader);
                //
                //
                GL_RECORD records_ = new GL_RECORD();
                DataRow transRecord_ = TAB_GETLASTROW(pDataHeader);
                //
                MY_JOURNAL_WRITE(transRecord_);
                //
                records_.Add(TABLE_EMFLINE.ACCOUNTREF, MY_GET_CLIENT_ACC(headerRecord_, transRecord_));
                records_.Add(MY_GET_CLIENT_ACC_COL(transRecord_), TAB_GETROW(transRecord_, TABLE_STOCKHEADER.NETTOTAL));
                records_.Add(TABLE_EMFLINE.REPORTRATE, TAB_GETROW(transRecord_, TABLE_STOCKHEADER.REPORTRATE));
                records_.Add(TABLE_EMFLINE.TRCURR, TAB_GETROW(transRecord_, TABLE_STOCKHEADER.TRCURR));
                records_.Add(TABLE_EMFLINE.TRRATE, TAB_GETROW(transRecord_, TABLE_STOCKHEADER.TRRATE));
                //
                //records_.Add(TABLE_EMFLINE.SPECODE, TAB_GETROW(headerRecord_, TABLE_STOCKHEADER.SPECODE));
                //records_.Add(TABLE_EMFLINE.INVOICENO, TAB_GETROW(headerRecord_, TABLE_STOCKHEADER.FICHENO));
                //records_.Add(TABLE_EMFLINE.CLDEF, TAB_GETROW(transRecord_, TABLE_STOCKHEADER.E_CLCARD__DEFINITION_));
                // records_.Add(TABLE_EMFLINE.LINEEXP, JOINLIST(FORMAT(new object[] { MY_GLREL_DOCCODE(headerRecord_), TAB_GETROW(transRecord_, TABLE_STOCKHEADER.E_CLCARD__CODE) })));
                //
                pList.Add(records_);
            }
            void MY_FORMAT_DOC_LINES(List<GL_RECORD> pList, DataTable pDataHeader, DataTable pDataLines)
            {
                DataRow headerRecord_ = TAB_GETLASTROW(pDataHeader);
                //
                bool distribDiscToAcc_ = MY_DISTRIB_DISC_TO_ACC(headerRecord_);
                bool distribSurchToAcc_ = MY_DISTRIB_SURCH_TO_ACC(headerRecord_);
                bool distribPromoToItem_ = MY_DISTRIB_PROMO_TO_MAT(headerRecord_);
                //


                //
                foreach (STOCK_GL_TRANS_DESC desc_ in MY_STOCK_LINES_TO_GL_LINES(pDataHeader, pDataLines))
                {
                    GL_RECORD records_ = new GL_RECORD();
                    DataRow transRecord_ = desc_.record;
                    //
                    MY_JOURNAL_WRITE(transRecord_);
                    //
                    double glValue_ = 0;
                    GLREL_TRCODE glRelTrcode_ = GLREL_TRCODE.undef;
                    string glRelFilter_ = GLREL_RELCODE1.EMPTY;

                    switch (desc_.type)
                    {

                        case STOCK_GL_TRANS_TYPE.material:
                            glRelTrcode_ = GLREL_TRCODE.material;
                            glValue_ = MY_GET_LINE_TOTAL(transRecord_, distribDiscToAcc_, distribSurchToAcc_, distribPromoToItem_);
                            glRelFilter_ = GLREL_RELCODE1.EMPTY;
                            break;
                        case STOCK_GL_TRANS_TYPE.materialPair:
                            glRelTrcode_ = GLREL_TRCODE.material;
                            glValue_ = MY_GET_LINE_TOTAL(transRecord_, distribDiscToAcc_, distribSurchToAcc_, distribPromoToItem_);
                            glRelFilter_ = GLREL_RELCODE1.PAIR;
                            break;
                        case STOCK_GL_TRANS_TYPE.service:
                            glRelTrcode_ = GLREL_TRCODE.service;
                            glValue_ = MY_GET_LINE_TOTAL(transRecord_, distribDiscToAcc_, distribSurchToAcc_, distribPromoToItem_);
                            glRelFilter_ = GLREL_RELCODE1.EMPTY;
                            break;
                        case STOCK_GL_TRANS_TYPE.VATSurchargeGlobal:
                            glRelTrcode_ = GLREL_TRCODE.VAT;
                            glValue_ = MY_GET_LINE_VAT(transRecord_);
                            glRelFilter_ = GLREL_RELCODE1.SURCH;
                            break;
                        case STOCK_GL_TRANS_TYPE.VATMaterial:
                            glRelTrcode_ = GLREL_TRCODE.VAT;
                            glValue_ = MY_GET_LINE_VAT(transRecord_);
                            glRelFilter_ = GLREL_RELCODE1.MAT;
                            break;
                        case STOCK_GL_TRANS_TYPE.VATMaterialSurcharge:
                            glRelTrcode_ = GLREL_TRCODE.VAT;
                            glValue_ = MY_GET_LINE_VAT(transRecord_);
                            glRelFilter_ = GLREL_RELCODE1.MATSURCH;
                            break;
                        case STOCK_GL_TRANS_TYPE.VATService:
                            glRelTrcode_ = GLREL_TRCODE.VAT;
                            glValue_ = MY_GET_LINE_VAT(transRecord_);
                            glRelFilter_ = GLREL_RELCODE1.SRV;
                            break;
                        case STOCK_GL_TRANS_TYPE.VATServiceSurcharge:
                            glRelTrcode_ = GLREL_TRCODE.VAT;
                            glValue_ = MY_GET_LINE_VAT(transRecord_);
                            glRelFilter_ = GLREL_RELCODE1.SRVSURCH;
                            break;
                        case STOCK_GL_TRANS_TYPE.VATMaterialPair:
                            glRelTrcode_ = GLREL_TRCODE.VAT;
                            glValue_ = MY_GET_LINE_VAT(transRecord_);
                            glRelFilter_ = GLREL_RELCODE1.PAIR;
                            break;
                        case STOCK_GL_TRANS_TYPE.discountGlobal:
                            if (distribDiscToAcc_)
                            {
                                glRelTrcode_ = GLREL_TRCODE.discount;
                                glValue_ = MY_GET_LINE_TOTAL(transRecord_);
                                glRelFilter_ = GLREL_RELCODE1.EMPTY;
                            }
                            break;
                        case STOCK_GL_TRANS_TYPE.discountMaterial:
                            if (distribDiscToAcc_)
                            {
                                glRelTrcode_ = GLREL_TRCODE.discount;
                                glValue_ = MY_GET_LINE_TOTAL(transRecord_);
                                glRelFilter_ = GLREL_RELCODE1.MAT;
                            }
                            break;
                        case STOCK_GL_TRANS_TYPE.discountService:
                            if (distribDiscToAcc_)
                            {
                                glRelTrcode_ = GLREL_TRCODE.discount;
                                glValue_ = MY_GET_LINE_TOTAL(transRecord_);
                                glRelFilter_ = GLREL_RELCODE1.SRV;
                            }
                            break;
                        case STOCK_GL_TRANS_TYPE.discountPromotion:
                            if (!distribPromoToItem_)
                            {
                                glRelTrcode_ = GLREL_TRCODE.discount;
                                glValue_ = MY_GET_LINE_TOTAL(transRecord_);
                                glRelFilter_ = GLREL_RELCODE1.PROMO;
                            }
                            break;
                        case STOCK_GL_TRANS_TYPE.surchargeGlobal:
                            if (distribSurchToAcc_)
                            {
                                glRelTrcode_ = GLREL_TRCODE.surcharge;
                                glValue_ = MY_GET_LINE_TOTAL(transRecord_);
                                glRelFilter_ = GLREL_RELCODE1.EMPTY;
                            }
                            break;
                        case STOCK_GL_TRANS_TYPE.surchargeMaterial:
                            if (distribSurchToAcc_)
                            {
                                glRelTrcode_ = GLREL_TRCODE.surcharge;
                                glValue_ = MY_GET_LINE_TOTAL(transRecord_);
                                glRelFilter_ = GLREL_RELCODE1.MAT;
                            }
                            break;
                        case STOCK_GL_TRANS_TYPE.surchargeService:
                            if (distribSurchToAcc_)
                            {
                                glRelTrcode_ = GLREL_TRCODE.surcharge;
                                glValue_ = MY_GET_LINE_TOTAL(transRecord_);
                                glRelFilter_ = GLREL_RELCODE1.SRV;
                            }
                            break;
                        case STOCK_GL_TRANS_TYPE.promoGlobal:
                            if (!distribPromoToItem_)
                            {
                                glRelTrcode_ = GLREL_TRCODE.promo;
                                glValue_ = MY_GET_LINE_TOTAL(transRecord_);
                                glRelFilter_ = GLREL_RELCODE1.EMPTY;
                            }
                            break;
                        case STOCK_GL_TRANS_TYPE.promoMaterial:
                            if (!distribPromoToItem_)
                            {
                                glRelTrcode_ = GLREL_TRCODE.promo;
                                glValue_ = MY_GET_LINE_TOTAL(transRecord_);
                                glRelFilter_ = GLREL_RELCODE1.MAT;
                            }
                            break;
                        case STOCK_GL_TRANS_TYPE.promoService:
                            if (!distribPromoToItem_)
                            {
                                glRelTrcode_ = GLREL_TRCODE.promo;
                                glValue_ = MY_GET_LINE_TOTAL(transRecord_);
                                glRelFilter_ = GLREL_RELCODE1.SRV;
                            }
                            break;
                    }

                    if (!ISNUMZERO(glValue_))
                    {
                        records_.Add(TABLE_EMFLINE.ACCOUNTREF, MY_GET_STOCK_ACC(headerRecord_, transRecord_, desc_.type, glRelTrcode_, glRelFilter_));
                        records_.Add(MY_GET_STOCK_ACC_COL(transRecord_, desc_.type), glValue_);
                        records_.Add(TABLE_EMFLINE.REPORTRATE, TAB_GETROW(transRecord_, TABLE_STLINE.REPORTRATE));
                        records_.Add(TABLE_EMFLINE.TRCURR, TAB_GETROW(transRecord_, TABLE_STLINE.TRCURR));
                        records_.Add(TABLE_EMFLINE.TRRATE, TAB_GETROW(transRecord_, TABLE_STLINE.TRRATE));
                        //
                        //records_.Add(TABLE_EMFLINE.SPECODE, TAB_GETROW(headerRecord_, TABLE_STOCKHEADER.SPECODE));
                        //records_.Add(TABLE_EMFLINE.INVOICENO, TAB_GETROW(headerRecord_, TABLE_STOCKHEADER.FICHENO));
                        //records_.Add(TABLE_EMFLINE.CLDEF, TAB_GETROW(transRecord_, TABLE_STLINE.E_ITEM__NAME));
                        // records_.Add(TABLE_EMFLINE.LINEEXP, JOINLIST(FORMAT(new object[] { MY_GLREL_DOCCODE(headerRecord_), TAB_GETROW(transRecord_, TABLE_STLINE.E_ITEM__CODE), (short)glRelTrcode_, glRelFilter_ })));
                        //
                        records_.Add(TABLE_EMFLINE.AMNT, MY_GET_LINE_AMNT(transRecord_));
                        pList.Add(records_);
                    }
                    //


                }
            }



            double MY_GET_LINE_TOTAL(DataRow pTransRow, bool pFlagDiscToAcc, bool pFlagSurchToAcc, bool pFlagPromoToItem)
            {
                double res_ = MY_GET_LINE_TOTAL(pTransRow);
                if (!pFlagDiscToAcc)
                    res_ = res_ - CASTASDOUBLE(TAB_GETROW(pTransRow, TABLE_STLINE.DISTDISC));
                if (!pFlagSurchToAcc)
                    res_ = res_ + CASTASDOUBLE(TAB_GETROW(pTransRow, TABLE_STLINE.DISTEXP));
                return res_;
            }
            double MY_GET_LINE_TOTAL(DataRow pTransRow)
            {
                double total_ = CASTASDOUBLE(TAB_GETROW(pTransRow, TABLE_STLINE.TOTAL));
                double vat_ = MY_GET_LINE_VAT(pTransRow);
                short flagVatInc_ = CASTASSHORT(TAB_GETROW(pTransRow, TABLE_STLINE.VATINC));
                return (flagVatInc_ == 0 ? total_ : total_ - vat_);
            }
            double MY_GET_LINE_AMNT(DataRow pTransRow)
            {
                double amnt_ = CASTASDOUBLE(TAB_GETROW(pTransRow, TABLE_STLINE.AMOUNT));
                double info1_ = CASTASDOUBLE(TAB_GETROW(pTransRow, TABLE_STLINE.UINFO1));
                double info2_ = CASTASDOUBLE(TAB_GETROW(pTransRow, TABLE_STLINE.UINFO2));
                info1_ = ISNUMZERO(info1_) ? 1 : info1_;
                info2_ = ISNUMZERO(info2_) ? 1 : info2_;
                return amnt_ * info2_ / info1_;
            }
            double MY_GET_LINE_VAT(DataRow pTransRow)
            {
                return CASTASDOUBLE(TAB_GETROW(pTransRow, TABLE_STLINE.VATAMNT));
            }
            STOCK_GL_TRANS_DESC[] MY_STOCK_LINES_TO_GL_LINES(DataTable pDataHeader, DataTable pDataLines)
            {
                List<STOCK_GL_TRANS_DESC> list_ = new List<STOCK_GL_TRANS_DESC>();


                DataRow lastParent_ = null;

                foreach (DataRow transRecord_ in pDataLines.Rows)
                {

                    bool isGlobal_ = MY_STOCK_TRANS_IS_GLOBAL(transRecord_);
                    bool isParent_ = MY_STOCK_TRANS_IS_PARENT(transRecord_);

                    switch (CASTASSHORT(TAB_GETROW(transRecord_, TABLE_STLINE.LINETYPE)))
                    {

                        case (short)STOCK_TRANS_TYPE.material:
                        case (short)STOCK_TRANS_TYPE.fixedAsset:
                            lastParent_ = transRecord_;
                            list_.Add(new STOCK_GL_TRANS_DESC(transRecord_, null, STOCK_GL_TRANS_TYPE.material));
                            list_.Add(new STOCK_GL_TRANS_DESC(transRecord_, null, STOCK_GL_TRANS_TYPE.VATMaterial));
                            if (MY_STOCK_TRANS_HAS_GL_PAIR(transRecord_))
                            {
                                list_.Add(new STOCK_GL_TRANS_DESC(transRecord_, null, STOCK_GL_TRANS_TYPE.materialPair));
                                list_.Add(new STOCK_GL_TRANS_DESC(transRecord_, null, STOCK_GL_TRANS_TYPE.VATMaterialPair));
                            }
                            break;
                        case (short)STOCK_TRANS_TYPE.promotion:
                            if (isGlobal_)
                                list_.Add(new STOCK_GL_TRANS_DESC(transRecord_, lastParent_, STOCK_GL_TRANS_TYPE.promoGlobal));
                            else
                                if (lastParent_ != null && MY_STOCK_TRANS_IS_MATERIAL(lastParent_))
                                    list_.Add(new STOCK_GL_TRANS_DESC(transRecord_, lastParent_, STOCK_GL_TRANS_TYPE.promoMaterial));
                                else
                                    if (lastParent_ != null && MY_STOCK_TRANS_IS_SERVICE(lastParent_))
                                        list_.Add(new STOCK_GL_TRANS_DESC(transRecord_, lastParent_, STOCK_GL_TRANS_TYPE.promoService));
                                    else
                                        throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_STLINE.TABLE, TABLE_STLINE.LOGICALREF, TAB_GETROW(transRecord_, TABLE_STLINE.LOGICALREF));

                            list_.Add(new STOCK_GL_TRANS_DESC(transRecord_, lastParent_, STOCK_GL_TRANS_TYPE.discountPromotion));
                            break;
                        case (short)STOCK_TRANS_TYPE.discount:
                            if (isGlobal_)
                                list_.Add(new STOCK_GL_TRANS_DESC(transRecord_, lastParent_, STOCK_GL_TRANS_TYPE.discountGlobal));
                            else
                                if (lastParent_ != null && MY_STOCK_TRANS_IS_MATERIAL(lastParent_))
                                    list_.Add(new STOCK_GL_TRANS_DESC(transRecord_, lastParent_, STOCK_GL_TRANS_TYPE.discountMaterial));
                                else
                                    if (lastParent_ != null && MY_STOCK_TRANS_IS_SERVICE(lastParent_))
                                        list_.Add(new STOCK_GL_TRANS_DESC(transRecord_, lastParent_, STOCK_GL_TRANS_TYPE.discountService));
                                    else
                                        throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_STLINE.TABLE, TABLE_STLINE.LOGICALREF, TAB_GETROW(transRecord_, TABLE_STLINE.LOGICALREF));
                            break;
                        case (short)STOCK_TRANS_TYPE.surcharge:
                            if (isGlobal_)
                            {
                                list_.Add(new STOCK_GL_TRANS_DESC(transRecord_, lastParent_, STOCK_GL_TRANS_TYPE.surchargeGlobal));
                                list_.Add(new STOCK_GL_TRANS_DESC(transRecord_, lastParent_, STOCK_GL_TRANS_TYPE.VATSurchargeGlobal));
                            }
                            else
                                if (lastParent_ != null && MY_STOCK_TRANS_IS_MATERIAL(lastParent_))
                                {
                                    list_.Add(new STOCK_GL_TRANS_DESC(transRecord_, lastParent_, STOCK_GL_TRANS_TYPE.surchargeMaterial));
                                    list_.Add(new STOCK_GL_TRANS_DESC(transRecord_, lastParent_, STOCK_GL_TRANS_TYPE.VATMaterialSurcharge));
                                }
                                else
                                    if (lastParent_ != null && MY_STOCK_TRANS_IS_SERVICE(lastParent_))
                                    {
                                        list_.Add(new STOCK_GL_TRANS_DESC(transRecord_, lastParent_, STOCK_GL_TRANS_TYPE.surchargeService));
                                        list_.Add(new STOCK_GL_TRANS_DESC(transRecord_, lastParent_, STOCK_GL_TRANS_TYPE.VATServiceSurcharge));
                                    }
                                    else
                                        throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_STLINE.TABLE, TABLE_STLINE.LOGICALREF, TAB_GETROW(transRecord_, TABLE_STLINE.LOGICALREF));
                            break;
                        case (short)STOCK_TRANS_TYPE.service:
                        case (short)STOCK_TRANS_TYPE.subcontracting:
                            lastParent_ = transRecord_;
                            list_.Add(new STOCK_GL_TRANS_DESC(transRecord_, null, STOCK_GL_TRANS_TYPE.service));
                            list_.Add(new STOCK_GL_TRANS_DESC(transRecord_, null, STOCK_GL_TRANS_TYPE.VATService));
                            break;
                        case (short)STOCK_TRANS_TYPE.deposit:
                            list_.Add(new STOCK_GL_TRANS_DESC(transRecord_, null, STOCK_GL_TRANS_TYPE.material));
                            break;
                        default:
                            throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_STLINE.TABLE, TABLE_STLINE.LOGICALREF, TAB_GETROW(transRecord_, TABLE_STLINE.LOGICALREF));
                    }
                }
                return list_.ToArray();
            }
            bool MY_STOCK_TRANS_IS_GLOBAL(DataRow pLine)
            {
                short isGlobal_ = CASTASSHORT(TAB_GETROW(pLine, TABLE_STLINE.GLOBTRANS));
                return (isGlobal_ == 1);
            }
            bool MY_STOCK_TRANS_IS_PARENT(DataRow pLine)
            {
                short lineType_ = CASTASSHORT(TAB_GETROW(pLine, TABLE_STLINE.LINETYPE));
                return (
                    (lineType_ == (short)STOCK_TRANS_TYPE.material) ||
                    (lineType_ == (short)STOCK_TRANS_TYPE.service) ||
                    (lineType_ == (short)STOCK_TRANS_TYPE.fixedAsset) ||
                    (lineType_ == (short)STOCK_TRANS_TYPE.subcontracting)
                    );
            }
            bool MY_STOCK_TRANS_IS_MATERIAL(DataRow pLine)
            {
                short lineType_ = CASTASSHORT(TAB_GETROW(pLine, TABLE_STLINE.LINETYPE));
                return (
                    (lineType_ == (short)STOCK_TRANS_TYPE.material) ||
                    (lineType_ == (short)STOCK_TRANS_TYPE.fixedAsset)
                    );
            }

            bool MY_STOCK_TRANS_HAS_GL_PAIR(DataRow pLine)
            {
                short lineTrCode_ = CASTASSHORT(TAB_GETROW(pLine, TABLE_STLINE.TRCODE));
                return (
                     (lineTrCode_ == 11) ||
                     (lineTrCode_ == 12) ||
                     (lineTrCode_ == 13) ||
                     (lineTrCode_ == 50) ||
                     (lineTrCode_ == 51) ||
                     (lineTrCode_ >= 15 && lineTrCode_ <= 19) ||
                     (lineTrCode_ >= 20 && lineTrCode_ <= 24)
                    );
            }
            bool MY_STOCK_TRANS_IS_SERVICE(DataRow pLine)
            {
                short lineType_ = CASTASSHORT(TAB_GETROW(pLine, TABLE_STLINE.LINETYPE));
                return (
                    (lineType_ == (short)STOCK_TRANS_TYPE.service) ||
                    (lineType_ == (short)STOCK_TRANS_TYPE.subcontracting)
                    );
            }
            bool MY_DISTRIB_DISC_TO_ACC(DataRow pDocRow)
            {
                if (pDocRow.Table.TableName != TABLE_INVOICE.TABLE)
                    return false;
                short distFlags_ = CASTASSHORT(TAB_GETROW(pDocRow, TABLE_INVOICE.ENTEGSET));
                return (!((distFlags_ & 1) == 1));
            }
            bool MY_DISTRIB_SURCH_TO_ACC(DataRow pDocRow)
            {
                if (pDocRow.Table.TableName != TABLE_INVOICE.TABLE)
                    return false;
                short distFlags_ = CASTASSHORT(TAB_GETROW(pDocRow, TABLE_INVOICE.ENTEGSET));
                return (!((distFlags_ & 2) == 2));

            }
            bool MY_DISTRIB_PROMO_TO_MAT(DataRow pDocRow)
            {
                if (pDocRow.Table.TableName != TABLE_INVOICE.TABLE)
                    return false;
                short distFlags_ = CASTASSHORT(TAB_GETROW(pDocRow, TABLE_INVOICE.ENTEGSET));
                return (!((distFlags_ & 4) == 4));

            }
            void MY_SYNC_HEADERS(DataTable pDataHeader, DataTable pGlHeader)
            {
                GL_RECORD records_ = new GL_RECORD();

                records_.Add(TABLE_EMFICHE.DATE_, TAB_GETROW(pDataHeader, TABLE_STOCKHEADER.DATE_));
                records_.Add(TABLE_EMFICHE.CANCELLED, TAB_GETROW(pDataHeader, TABLE_STOCKHEADER.CANCELLED));
                records_.Add(TABLE_EMFICHE.BRANCH, TAB_GETROW(pDataHeader, TABLE_STOCKHEADER.BRANCH));
                records_.Add(TABLE_EMFICHE.DEPARTMENT, TAB_GETROW(pDataHeader, TABLE_STOCKHEADER.DEPARTMENT));
                records_.Add(TABLE_EMFICHE.GENEXP1, TAB_GETROW(pDataHeader, TABLE_STOCKHEADER.GENEXP1));
                records_.Add(TABLE_EMFICHE.GENEXP2, TAB_GETROW(pDataHeader, TABLE_STOCKHEADER.GENEXP2));
                records_.Add(TABLE_EMFICHE.GENEXP3, TAB_GETROW(pDataHeader, TABLE_STOCKHEADER.GENEXP3));
                records_.Add(TABLE_EMFICHE.GENEXP4, TAB_GETROW(pDataHeader, TABLE_STOCKHEADER.GENEXP4));
                records_.Add(TABLE_EMFICHE.SPECODE, TAB_GETROW(pDataHeader, TABLE_STOCKHEADER.SPECODE));
                records_.Add(TABLE_EMFICHE.SPECODE2, TAB_GETROW(pDataHeader, TABLE_STOCKHEADER.SPECODE2));
                records_.Add(TABLE_EMFICHE.SPECODE3, TAB_GETROW(pDataHeader, TABLE_STOCKHEADER.SPECODE3));
                records_.Add(TABLE_EMFICHE.CYPHCODE, TAB_GETROW(pDataHeader, TABLE_STOCKHEADER.CYPHCODE));

                records_.Add(TABLE_EMFICHE.FICHENO, MY_TOOLS.CRETAGLFICHENO(pDataHeader));

                foreach (KeyValuePair<string, object> pair_ in records_)
                    TAB_SETCOL(pGlHeader, pair_.Key, pair_.Value);

            }
            Exception MY_EXCEPTION_INVALID_COL_VALUE(string pTab, string pCol, object pVal)
            {
                return new Exception(string.Format("T_MSG_ERROR_INVALID_VAR, {0}/{1}/{2}", pTab, pCol, pVal));
            }
            Exception MY_EXCEPTION_INVALID_TAB(string pTab)
            {
                return new Exception(string.Format("T_MSG_ERROR_INVALID_VAR, T_TABLE '{0}'", pTab));
            }
            Exception MY_EXCEPTION_INVALID_VAR(string pDesc, object pVal)
            {
                return new Exception(string.Format("T_MSG_ERROR_INVALID_VAR, {0}/{1}", pDesc, pVal));
            }
            Exception MY_EXCEPTION_INVALID_LREF(string pTab, string pCol, object pVal)
            {
                return new Exception(string.Format("T_MSG_ERROR_INVALID_LREF_TYPE, {0}/{1}/{2}", pTab, pCol, pVal));
            }
            bool MY_TRANS_IS_STOCK_IN(DataRow pLine)
            {
                if (pLine.Table.TableName == TABLE_STLINE.TABLE)
                {
                    short iocode_ = CASTASSHORT(TAB_GETROW(pLine, TABLE_STLINE.IOCODE));
                    if (iocode_ == 1 || iocode_ == 2)
                        return true;
                    else
                        if (iocode_ == 3 || iocode_ == 4)
                            return false;
                        else
                            throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_STLINE.TABLE, TABLE_STLINE.IOCODE, iocode_);
                }
                throw MY_EXCEPTION_INVALID_TAB(pLine.Table.TableName);
            }
            void MY_CHECK_IS_WELL_KNOWN_TRANS(DataRow pLine)
            {
                switch (pLine.Table.TableName)
                {
                    case TABLE_STLINE.TABLE:
                        {
                            short trcode_ = CASTASSHORT(TAB_GETROW(pLine, TABLE_STLINE.TRCODE));
                            short iocode_ = CASTASSHORT(TAB_GETROW(pLine, TABLE_STLINE.IOCODE));
                            switch (trcode_)
                            {
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                case 6:
                                case 7:
                                case 8:
                                case 9:
                                case 11:
                                case 12:
                                case 13:
                                case 15:
                                case 16:
                                case 17:
                                case 18:
                                case 19:
                                case 20:
                                case 21:
                                case 22:
                                case 23:
                                case 24:
                                case 50:
                                case 51:
                                    return;
                                case 25:
                                    if (iocode_ == 1 || iocode_ == 2)
                                        return;
                                    else
                                        if (iocode_ == 3 || iocode_ == 4)
                                            return;

                                    throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_STLINE.TABLE, TABLE_STLINE.IOCODE, iocode_);
                                default:
                                    throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_STLINE.TABLE, TABLE_STLINE.TRCODE, trcode_);

                            }

                        }
                    case TABLE_INVOICE.TABLE:
                        {
                            short trcode_ = CASTASSHORT(TAB_GETROW(pLine, TABLE_INVOICE.TRCODE));
                            switch (trcode_)
                            {
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                case 6:
                                case 7:
                                case 8:
                                case 9:
                                    return;
                                default:
                                    throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_INVOICE.TABLE, TABLE_INVOICE.TRCODE, trcode_);
                            }
                        }
                    default:
                        throw MY_EXCEPTION_INVALID_TAB(pLine.Table.TableName);

                }
            }
            void MY_CHECK_TABLES(DataTable pDataHeader, DataTable pDataLines, DataTable pGlHeader, DataTable pGlLines)
            {
                if (pDataHeader == null || TAB_GETLASTROW(pDataHeader) == null)
                    EXCEPTIONFORUSER(string.Format("T_MSG_DATA_NO (T_TABLE '{0}' T_OR '{1}')", TABLE_INVOICE.TABLE, TABLE_STFICHE.TABLE));

                MY_JOURNAL_WRITE(pDataHeader);

                if (pDataLines == null)
                    EXCEPTIONFORUSER(string.Format("T_MSG_DATA_NO (T_TABLE '{0}')", TABLE_STLINE.TABLE));

                foreach (DataRow row_ in pDataLines.Rows)
                    MY_CHECK_IS_WELL_KNOWN_TRANS(row_);

                if (pGlHeader == null || TAB_GETLASTROW(pGlHeader) == null)
                    EXCEPTIONFORUSER(string.Format("T_MSG_DATA_NO (T_TABLE '{0}')", TABLE_EMFICHE.TABLE));
                if (pGlLines == null)
                    EXCEPTIONFORUSER(string.Format("T_MSG_DATA_NO (T_TABLE '{0}')", TABLE_EMFLINE.TABLE));


            }






        }

        #endregion
        #region CASH

        class GLDO_CASH
        {

            public GLDO_CASH(_PLUGIN pPLUGIN)
            {
                PLUGIN = pPLUGIN;
            }
            _PLUGIN PLUGIN;


            class GL_RECORD : Dictionary<string, object>
            {

            }
            class GL_TRANS_DESC
            {
                public GL_TRANS_DESC(DataRow pRecord, DataRow pParent, GL_TRANS_TYPE pType)
                {
                    record = pRecord;
                    parent = pParent;
                    type = pType;
                }
                public GL_TRANS_TYPE type;
                public DataRow record;
                public DataRow parent;
            }
            enum GL_TRANS_TYPE
            {
                cash = 1,
                cashPair = 2,
                client = 3,
                bank = 4,
                cash2 = 5 // cash remittanse trans
            }
            enum FIN_SIGN
            {
                debit = 0,
                credit = 1
            }
            enum GLREL_TRCODE
            {
                undef = 0,
                material = 1,
                personalCarad = 2,
                service = 3,
                VAT = 4,
                cash = 5,
                bank = 6,
                discount = 7,
                surcharge = 8,
                promo = 9

            }
            class GLREL_RELCODE1
            {
                public const string EMPTY = "";
                public const string PAIR = "PAIR";
            }
            public void MY_GL(DataSet DATASETS, DataSet DATASETD)
            {
                DataTable dataHeader_;
                DataTable dataLines_;

                DataTable glHeader_;
                DataTable glLines_;

                dataHeader_ = MY_GETTAB(DATASETS, TABLE_KSLINES.TABLE);
                dataLines_ = MY_GETTAB(DATASETS, TABLE_KSLINES.TABLE);

                glHeader_ = MY_GETTAB(DATASETD, TABLE_EMFICHE.TABLE);
                glLines_ = MY_GETTAB(DATASETD, TABLE_EMFLINE.TABLE);

                MY_CHECK_TABLES(dataHeader_, dataLines_, glHeader_, glLines_);
                MY_SYNC_HEADERS(dataHeader_, glHeader_);
                MY_PARSE_DATA(glHeader_, glLines_, MY_FORMAT_DATA(dataHeader_, dataLines_));

            }
            void MY_JOURNAL_WRITE(string[] pTables, object[] pArr)
            {
                if (!_useJournal)
                    return;
                PLUGIN.JOURNAL(JOURNAL_NS,
                string.Format(PLUGIN.RESOLVESTR("$search\t{0}/{1}"),
                JOINLIST(pTables),
                JOINLIST(FORMAT(pArr))));
            }
            void MY_JOURNAL_WRITE(DataTable pDocHeader)
            {
                if (!_useJournal)
                    return;
                DataRow headerRow_ = TAB_GETLASTROW(pDocHeader);
                PLUGIN.JOURNAL(JOURNAL_NS,
                    string.Format(PLUGIN.RESOLVESTR(Environment.NewLine + "$doc\t({0}/{1}/{2})\t[lang::T_TYPE],{3};[lang::T_NR],{4}"),
                    TABLE_KSLINES.TABLE,
                    TABLE_KSLINES.LOGICALREF,
                    TAB_GETROW(headerRow_, TABLE_KSLINES.LOGICALREF),
                    MY_GLREL_DOCCODE(headerRow_),
                    TAB_GETROW(headerRow_, TABLE_KSLINES.FICHENO)));
            }
            void MY_JOURNAL_WRITE(DataRow pTrans)
            {
                if (!_useJournal)
                    return;

                PLUGIN.JOURNAL(JOURNAL_NS,
                    string.Format(PLUGIN.RESOLVESTR("$line\t({0}/{1}/{2})"),
                    pTrans.Table.TableName,
                    TABLE_KSLINES.LOGICALREF,
                    TAB_GETROW(pTrans, TABLE_KSLINES.LOGICALREF)));
            }

            void MY_PARSE_DATA(DataTable pGlHeader, DataTable pGlLines, GL_RECORD[] pRecords)
            {
                foreach (GL_RECORD rec_ in pRecords)
                {
                    TAB_ADDROW(pGlLines);
                    foreach (KeyValuePair<string, object> pair_ in rec_)
                        TAB_SETROW(pGlLines, pair_.Key, pair_.Value);
                }

            }

            GL_RECORD[] MY_FORMAT_DATA(DataTable pDataHeader, DataTable pDataLines)
            {
                List<GL_RECORD> list_ = new List<GL_RECORD>();
                //Lines as trans
                MY_FORMAT_DATA_LINES(list_, pDataHeader, pDataLines);
                return list_.ToArray();
            }



            object MY_GET_TRANS_ACC(DataRow pDocRow, DataRow pTransRow, GL_TRANS_TYPE pTransType, GLREL_TRCODE pGLRelType, string pGLRelFilterCode)
            {
                string cardTable_ = string.Empty;
                string cardColumn_ = string.Empty;


                var TRCODE = CASTASSHORT(TAB_GETROW(pDocRow, TABLE_KSLINES.TRCODE));
                var ACCREF = (TAB_GETROW(pDocRow, TABLE_KSLINES.ACCREF));
                var CSACCREF = (TAB_GETROW(pDocRow, TABLE_KSLINES.CSACCREF));


                switch (TRCODE)
                {
                    //case 11:
                    //case 12:
                    //case 21:
                    //case 22:
                    case 41:
                    case 42:
                        switch (pTransType)
                        {
                            case GL_TRANS_TYPE.cash:
                                if (!ISEMPTYLREF(CSACCREF))
                                    return CSACCREF;
                                break;
                            case GL_TRANS_TYPE.cashPair:
                            //case GL_TRANS_TYPE.bank:
                            //case GL_TRANS_TYPE.client:
                                if (!ISEMPTYLREF(ACCREF))
                                    return ACCREF;
                                break;
                        }
                        break;

                }

                switch (pTransType)
                {
                    case GL_TRANS_TYPE.cash:
                        cardTable_ = TABLE_KSCARD.TABLE;
                        cardColumn_ = TABLE_KSLINES.CARDREF;
                        break;
                    case GL_TRANS_TYPE.cashPair:
                        cardTable_ = TABLE_KSCARD.TABLE;
                        cardColumn_ = TABLE_KSLINES.CARDREF;
                        break;
                    case GL_TRANS_TYPE.cash2:
                        cardTable_ = TABLE_KSCARD.TABLE;
                        cardColumn_ = TABLE_KSLINES.VCARDREF;
                        break;
                    case GL_TRANS_TYPE.bank:
                        cardTable_ = TABLE_BANKACC.TABLE;
                        cardColumn_ = TABLE_KSLINES.BNACCREF;
                        break;
                    case GL_TRANS_TYPE.client:
                        cardTable_ = TABLE_CLCARD.TABLE;
                        cardColumn_ = TABLE_KSLINES.CLIENTREF;
                        break;
                    default:
                        throw MY_EXCEPTION_INVALID_VAR(typeof(GL_TRANS_TYPE).Name, pTransType);


                }

                string[] tablesArr_ = new string[] { pDocRow.Table.TableName, pTransRow.Table.TableName, cardTable_ };

                string sql_ = MY_GLREL.MY_GET_ACC_SEARCH_SQL(tablesArr_[0], tablesArr_[1], tablesArr_[2]);

                object docLRef_ = TAB_GETROW(pDocRow, TABLE_KSLINES.LOGICALREF);
                object transLRef_ = TAB_GETROW(pTransRow, TABLE_KSLINES.LOGICALREF);
                object cardLRef_ = TAB_GETROW(pTransRow, cardColumn_);

                return MY_GLREL.MY_GET_ACC(PLUGIN, sql_,
                      (short)pGLRelType,
              MY_GLREL_DOCCODE(pDocRow),
              pGLRelFilterCode,
              docLRef_,
              docLRef_,
              transLRef_,
              transLRef_,
              cardLRef_,
              cardLRef_

                  );


            }

            void MY_CHECK_ACC(object pLRef)
            {
                if (CASTASINT(ISNULL(pLRef, 0)) == 0)
                    throw MY_EXCEPTION_INVALID_LREF(TABLE_EMUHACC.TABLE, TABLE_EMUHACC.LOGICALREF, pLRef);
            }

            bool MY_GET_TRANS_CASH_SIDE(DataRow pTransRow, GL_TRANS_TYPE pTransType)
            {
                short trcode_ = -1;
                bool res_ = false;



                trcode_ = CASTASSHORT(TAB_GETROW(pTransRow, TABLE_KSLINES.TRCODE));

                switch (trcode_)
                {
                    case 11:
                    case 41:
                    case 22:
                    case 79:
                        res_ = true;
                        break;
                    case 12:
                    case 42:
                    case 21:
                    case 80:
                        res_ = false;
                        break;
                    case 73:
                        if (pTransType == GL_TRANS_TYPE.cash)
                            res_ = true;
                        else
                            if (pTransType == GL_TRANS_TYPE.cash2)
                                res_ = false;
                            else
                                throw MY_EXCEPTION_INVALID_VAR(typeof(GL_TRANS_TYPE).Name, pTransType);
                        break;
                    case 74:
                        if (pTransType == GL_TRANS_TYPE.cash)
                            res_ = false;
                        else
                            if (pTransType == GL_TRANS_TYPE.cash2)
                                res_ = true;
                            else
                                throw MY_EXCEPTION_INVALID_VAR(typeof(GL_TRANS_TYPE).Name, pTransType);
                        break;
                    default:
                        throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_CLFLINE.TABLE, TABLE_CLFLINE.TRCODE, trcode_);
                }



                return res_;
            }
            string MY_GET_TRANS_COL(DataRow pTransRow, GL_TRANS_TYPE pTransType)
            {

                bool res_ = MY_GET_TRANS_CASH_SIDE(pTransRow, pTransType);

                switch (pTransType)
                {
                    case GL_TRANS_TYPE.cash:
                    case GL_TRANS_TYPE.cash2:
                        // res_ = res_;
                        break;
                    case GL_TRANS_TYPE.cashPair:
                    case GL_TRANS_TYPE.client:
                    case GL_TRANS_TYPE.bank:
                        res_ = !res_;
                        break;
                    default:
                        throw MY_EXCEPTION_INVALID_VAR(typeof(GL_TRANS_TYPE).Name, pTransType);

                }


                return MY_ACC_SIDE_TO_COL(res_);
            }

            string MY_ACC_SIDE_TO_COL(bool pAccSide)
            {
                return pAccSide ? TABLE_EMFLINE.DEBIT : TABLE_EMFLINE.CREDIT;
            }
            string MY_GLREL_DOCCODE(DataRow pDocRow)
            {
                string nr_ = FORMAT(TAB_GETROW(pDocRow, TABLE_KSLINES.TRCODE));
                return "CASH" + "." + nr_;
            }


            void MY_FORMAT_DATA_LINES(List<GL_RECORD> pList, DataTable pDataHeader, DataTable pDataLines)
            {
                DataRow headerRecord_ = TAB_GETLASTROW(pDataHeader);
                //
                var toGlLines = MY_LINES_TO_GL_LINES(pDataHeader, pDataLines);
                //
                foreach (GL_TRANS_DESC desc_ in toGlLines)
                {
                    GL_RECORD records_ = new GL_RECORD();
                    DataRow transRecord_ = desc_.record;
                    //
                    MY_JOURNAL_WRITE(transRecord_);
                    //
                    double glValue_ = 0;
                    GLREL_TRCODE glRelTrcode_ = GLREL_TRCODE.undef;
                    string glRelFilter_ = GLREL_RELCODE1.EMPTY;

                    switch (desc_.type)
                    {

                        case GL_TRANS_TYPE.cash:
                            glRelTrcode_ = GLREL_TRCODE.cash;
                            glValue_ = MY_GET_LINE_TOTAL(transRecord_);
                            glRelFilter_ = GLREL_RELCODE1.EMPTY;
                            break;
                        case GL_TRANS_TYPE.cash2:
                            glRelTrcode_ = GLREL_TRCODE.cash;
                            glValue_ = MY_GET_LINE_TOTAL(transRecord_);
                            glRelFilter_ = GLREL_RELCODE1.EMPTY;
                            break;
                        case GL_TRANS_TYPE.cashPair:
                            glRelTrcode_ = GLREL_TRCODE.cash;
                            glValue_ = MY_GET_LINE_TOTAL(transRecord_);
                            glRelFilter_ = GLREL_RELCODE1.PAIR;
                            break;
                        case GL_TRANS_TYPE.client:
                            glRelTrcode_ = GLREL_TRCODE.personalCarad;
                            glValue_ = MY_GET_LINE_TOTAL(transRecord_);
                            glRelFilter_ = GLREL_RELCODE1.EMPTY;
                            break;
                        case GL_TRANS_TYPE.bank:
                            glRelTrcode_ = GLREL_TRCODE.bank;
                            glValue_ = MY_GET_LINE_TOTAL(transRecord_);
                            glRelFilter_ = GLREL_RELCODE1.EMPTY;
                            break;

                    }

                    if (!ISNUMZERO(glValue_))
                    {


                        var accRef = MY_GET_TRANS_ACC(headerRecord_, transRecord_, desc_.type, glRelTrcode_, glRelFilter_);

                        records_.Add(TABLE_EMFLINE.ACCOUNTREF, accRef);
                        records_.Add(MY_GET_TRANS_COL(transRecord_, desc_.type), glValue_);
                        records_.Add(TABLE_EMFLINE.REPORTRATE, TAB_GETROW(transRecord_, TABLE_KSLINES.REPORTRATE));
                        records_.Add(TABLE_EMFLINE.TRCURR, TAB_GETROW(transRecord_, TABLE_KSLINES.TRCURR));
                        records_.Add(TABLE_EMFLINE.TRRATE, TAB_GETROW(transRecord_, TABLE_KSLINES.TRRATE));
                        //
                        //records_.Add(TABLE_EMFLINE.SPECODE, TAB_GETROW(headerRecord_, TABLE_KSLINES.SPECODE));
                        // records_.Add(TABLE_EMFLINE.INVOICENO, TAB_GETROW(headerRecord_, TABLE_KSLINES.FICHENO));
                        // records_.Add(TABLE_EMFLINE.CLDEF, TAB_GETROW(transRecord_, MY_GET_TRANS_CARD_INFO(transRecord_, desc_.type, false)));
                        //records_.Add(TABLE_EMFLINE.LINEEXP, JOINLIST(FORMAT(new object[] { 
                        //MY_GLREL_DOCCODE(headerRecord_), 
                        //TAB_GETROW(transRecord_, MY_GET_TRANS_CARD_INFO(transRecord_, desc_.type, true)), 
                        //(short)glRelTrcode_, 
                        //glRelFilter_ })));
                        //
                        pList.Add(records_);
                    }
                    //


                }
            }
            string MY_GET_TRANS_CARD_INFO(DataRow pTransRow, GL_TRANS_TYPE pTransType, bool pCode)
            {
                switch (pTransType)
                {
                    case GL_TRANS_TYPE.cash:
                    case GL_TRANS_TYPE.cashPair:
                        return (pCode ? TABLE_KSLINES.E_KSCARD__CODE : TABLE_KSLINES.E_KSCARD__NAME);
                    case GL_TRANS_TYPE.cash2:
                        return (pCode ? TABLE_KSLINES.E_KSCARD__CODE2 : TABLE_KSLINES.E_KSCARD__NAME2);
                    case GL_TRANS_TYPE.bank:
                        return (pCode ? TABLE_KSLINES.E_BANKACC__CODE : TABLE_KSLINES.E_BANKACC__DEFINITION_);
                    case GL_TRANS_TYPE.client:
                        return (pCode ? TABLE_KSLINES.E_CLCARD__CODE : TABLE_KSLINES.E_CLCARD__DEFINITION_);
                    default:
                        throw MY_EXCEPTION_INVALID_VAR(typeof(GL_TRANS_TYPE).Name, pTransType);
                }
            }


            double MY_GET_LINE_TOTAL(DataRow pTransRow)
            {
                double total_ = 0;
                if (pTransRow.Table.TableName == TABLE_KSLINES.TABLE)
                    total_ = CASTASDOUBLE(TAB_GETROW(pTransRow, TABLE_KSLINES.AMOUNT));
                else
                    throw MY_EXCEPTION_INVALID_TAB(pTransRow.Table.TableName);
                return total_;
            }


            GL_TRANS_DESC[] MY_LINES_TO_GL_LINES(DataTable pDataHeader, DataTable pDataLines)
            {
                List<GL_TRANS_DESC> list_ = new List<GL_TRANS_DESC>();


                foreach (DataRow transRecord_ in pDataLines.Rows)
                {
                    switch (transRecord_.Table.TableName)
                    {
                        case TABLE_KSLINES.TABLE:
                            short trcode_ = CASTASSHORT(TAB_GETROW(transRecord_, TABLE_KSLINES.TRCODE));
                            switch (trcode_)
                            {
                                case 11:
                                case 12:
                                    list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.cash));
                                    list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.client));
                                    break;
                                case 21:
                                case 22:
                                    list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.cash));
                                    list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.bank));
                                    break;
                                case 41:
                                case 42:
                                    list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.cash));
                                    list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.cashPair));
                                    break;
                                case 73:
                                case 74:
                                    list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.cash));
                                    list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.cash2));
                                    break;
                                case 79:
                                case 80:
                                    list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.cash));
                                    list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.cashPair));
                                    break;
                                default:
                                    throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_KSLINES.TABLE, TABLE_KSLINES.TRCODE, trcode_);
                            }
                            break;
                        default:
                            throw MY_EXCEPTION_INVALID_TAB(transRecord_.Table.TableName);

                    }
                }

                return list_.ToArray();
            }

            Exception MY_EXCEPTION_INVALID_COL_VALUE(string pTab, string pCol, object pVal)
            {
                return new Exception(string.Format("T_MSG_ERROR_INVALID_VAR, {0}/{1}/{2}", pTab, pCol, pVal));
            }
            Exception MY_EXCEPTION_INVALID_TAB(string pTab)
            {
                return new Exception(string.Format("T_MSG_ERROR_INVALID_VAR, T_TABLE '{0}'", pTab));
            }
            Exception MY_EXCEPTION_INVALID_VAR(string pDesc, object pVal)
            {
                return new Exception(string.Format("T_MSG_ERROR_INVALID_VAR, {0}/{1}", pDesc, pVal));
            }
            Exception MY_EXCEPTION_INVALID_LREF(string pTab, string pCol, object pVal)
            {
                return new Exception(string.Format("T_MSG_ERROR_INVALID_LREF_TYPE, {0}/{1}/{2}", pTab, pCol, pVal));
            }
            void MY_CHECK_IS_WELL_KNOWN_TRANS(DataRow pLine)
            {
                switch (pLine.Table.TableName)
                {
                    case TABLE_KSLINES.TABLE:
                        {
                            short trcode_ = CASTASSHORT(TAB_GETROW(pLine, TABLE_KSLINES.TRCODE));
                            short sign_ = CASTASSHORT(TAB_GETROW(pLine, TABLE_KSLINES.SIGN));
                            switch (trcode_)
                            {
                                case 11:
                                case 12:
                                case 21:
                                case 22:
                                case 41:
                                case 42:
                                case 73:
                                case 74:
                                case 79:
                                case 80:
                                    switch (sign_)
                                    {
                                        case (short)FIN_SIGN.debit:
                                        case (short)FIN_SIGN.credit:
                                            return;
                                        default:
                                            throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_KSLINES.TABLE, TABLE_KSLINES.SIGN, sign_);
                                    }
                                default:
                                    throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_KSLINES.TABLE, TABLE_KSLINES.TRCODE, trcode_);

                            }

                        }
                    default:
                        throw MY_EXCEPTION_INVALID_TAB(pLine.Table.TableName);

                }
            }


            bool MY_TRANS_IS_FIN_DEBIT(DataRow pLine)
            {
                if (pLine.Table.TableName == TABLE_KSLINES.TABLE)
                {
                    short sign_ = CASTASSHORT(TAB_GETROW(pLine, TABLE_KSLINES.SIGN));
                    if (sign_ == (short)FIN_SIGN.debit)
                        return true;
                    else
                        if (sign_ == (short)FIN_SIGN.credit)
                            return false;

                    throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_KSLINES.TABLE, TABLE_KSLINES.SIGN, sign_);
                }
                throw MY_EXCEPTION_INVALID_TAB(pLine.Table.TableName);

            }
            void MY_SYNC_HEADERS(DataTable pDataHeader, DataTable pGlHeader)
            {
                GL_RECORD records_ = new GL_RECORD();

                records_.Add(TABLE_EMFICHE.DATE_, TAB_GETROW(pDataHeader, TABLE_KSLINES.DATE_));
                records_.Add(TABLE_EMFICHE.CANCELLED, TAB_GETROW(pDataHeader, TABLE_KSLINES.CANCELLED));
                records_.Add(TABLE_EMFICHE.BRANCH, TAB_GETROW(pDataHeader, TABLE_KSLINES.BRANCH));
                records_.Add(TABLE_EMFICHE.DEPARTMENT, TAB_GETROW(pDataHeader, TABLE_KSLINES.DEPARTMENT));
                records_.Add(TABLE_EMFICHE.GENEXP1, TAB_GETROW(pDataHeader, TABLE_KSLINES.LINEEXP));
                records_.Add(TABLE_EMFICHE.GENEXP2, TAB_GETROW(pDataHeader, TABLE_KSLINES.CUSTTITLE));
                records_.Add(TABLE_EMFICHE.SPECODE, TAB_GETROW(pDataHeader, TABLE_KSLINES.SPECODE));
                records_.Add(TABLE_EMFICHE.SPECODE2, TAB_GETROW(pDataHeader, TABLE_KSLINES.SPECODE2));
                records_.Add(TABLE_EMFICHE.SPECODE3, TAB_GETROW(pDataHeader, TABLE_KSLINES.SPECODE3));
                records_.Add(TABLE_EMFICHE.CYPHCODE, TAB_GETROW(pDataHeader, TABLE_KSLINES.CYPHCODE));
                records_.Add(TABLE_EMFICHE.FICHENO, MY_TOOLS.CRETAGLFICHENO(pDataHeader));



                foreach (KeyValuePair<string, object> pair_ in records_)
                    TAB_SETCOL(pGlHeader, pair_.Key, pair_.Value);

            }

            void MY_CHECK_TABLES(DataTable pDataHeader, DataTable pDataLines, DataTable pGlHeader, DataTable pGlLines)
            {
                if (pDataHeader == null || TAB_GETLASTROW(pDataHeader) == null)
                    EXCEPTIONFORUSER(string.Format("T_MSG_DATA_NO (T_TABLE '{0}')", TABLE_KSLINES.TABLE));

                MY_JOURNAL_WRITE(pDataHeader);

                if (pDataLines == null)
                    EXCEPTIONFORUSER(string.Format("T_MSG_DATA_NO (T_TABLE '{0}')", TABLE_KSLINES.TABLE));

                foreach (DataRow row_ in pDataLines.Rows)
                    MY_CHECK_IS_WELL_KNOWN_TRANS(row_);

                if (pGlHeader == null || TAB_GETLASTROW(pGlHeader) == null)
                    EXCEPTIONFORUSER(string.Format("T_MSG_DATA_NO (T_TABLE {0})", TABLE_EMFICHE.TABLE));

                if (pGlLines == null)
                    EXCEPTIONFORUSER(string.Format("T_MSG_DATA_NO (T_TABLE {0})", TABLE_EMFLINE.TABLE));


            }






        }

        #endregion
        #region BANK

        class GLDO_BANK
        {

            public GLDO_BANK(_PLUGIN pPLUGIN)
            {
                PLUGIN = pPLUGIN;
            }
            _PLUGIN PLUGIN;


            class GL_RECORD : Dictionary<string, object>
            {

            }
            class GL_TRANS_DESC
            {
                public GL_TRANS_DESC(DataRow pRecord, DataRow pParent, GL_TRANS_TYPE pType)
                {
                    record = pRecord;
                    parent = pParent;
                    type = pType;
                }
                public GL_TRANS_TYPE type;
                public DataRow record;
                public DataRow parent;
            }
            enum GL_TRANS_TYPE
            {
                bank = 1,
                bankDebitPair = 2,
                bankCreditPair = 3,
                client = 4
            }
            enum FIN_SIGN
            {
                debit = 0,
                credit = 1
            }
            enum GLREL_TRCODE
            {
                undef = 0,
                material = 1,
                personalCarad = 2,
                service = 3,
                VAT = 4,
                cash = 5,
                bank = 6,
                discount = 7,
                surcharge = 8,
                promo = 9

            }
            class GLREL_RELCODE1
            {
                public const string EMPTY = "";
                public const string CPAIR = "CPAIR";
                public const string DPAIR = "DPAIR";
            }
            public void MY_GL(DataSet DATASETS, DataSet DATASETD)
            {
                DataTable dataHeader_;
                DataTable dataLines_;

                DataTable glHeader_;
                DataTable glLines_;

                dataHeader_ = MY_GETTAB(DATASETS, TABLE_BNFICHE.TABLE);
                dataLines_ = MY_GETTAB(DATASETS, TABLE_BNFLINE.TABLE);

                glHeader_ = MY_GETTAB(DATASETD, TABLE_EMFICHE.TABLE);
                glLines_ = MY_GETTAB(DATASETD, TABLE_EMFLINE.TABLE);

                MY_CHECK_TABLES(dataHeader_, dataLines_, glHeader_, glLines_);
                MY_SYNC_HEADERS(dataHeader_, glHeader_);
                MY_PARSE_DATA(glHeader_, glLines_, MY_FORMAT_DATA(dataHeader_, dataLines_));

            }
            void MY_JOURNAL_WRITE(string[] pTables, object[] pArr)
            {
                if (!_useJournal)
                    return;
                PLUGIN.JOURNAL(JOURNAL_NS,
                string.Format(PLUGIN.RESOLVESTR("$search\t{0}/{1}"),
                JOINLIST(pTables),
                JOINLIST(FORMAT(pArr))));
            }
            void MY_JOURNAL_WRITE(DataTable pDocHeader)
            {
                if (!_useJournal)
                    return;
                DataRow headerRow_ = TAB_GETLASTROW(pDocHeader);
                PLUGIN.JOURNAL(JOURNAL_NS,
                    string.Format(PLUGIN.RESOLVESTR(Environment.NewLine + "$doc\t({0}/{1}/{2})\t[lang::T_TYPE],{3};[lang::T_NR],{4}"),
                    TABLE_BNFICHE.TABLE,
                    TABLE_BNFICHE.LOGICALREF,
                    TAB_GETROW(headerRow_, TABLE_BNFICHE.LOGICALREF),
                    MY_GLREL_DOCCODE(headerRow_),
                    TAB_GETROW(headerRow_, TABLE_BNFICHE.FICHENO)));
            }
            void MY_JOURNAL_WRITE(DataRow pTrans)
            {
                if (!_useJournal)
                    return;

                PLUGIN.JOURNAL(JOURNAL_NS,
                    string.Format(PLUGIN.RESOLVESTR("$line\t({0}/{1}/{2})"),
                    pTrans.Table.TableName,
                    TABLE_BNFICHE.LOGICALREF,
                    TAB_GETROW(pTrans, TABLE_BNFICHE.LOGICALREF)));
            }

            void MY_PARSE_DATA(DataTable pGlHeader, DataTable pGlLines, GL_RECORD[] pRecords)
            {
                foreach (GL_RECORD rec_ in pRecords)
                {
                    TAB_ADDROW(pGlLines);
                    foreach (KeyValuePair<string, object> pair_ in rec_)
                        TAB_SETROW(pGlLines, pair_.Key, pair_.Value);
                }

            }

            GL_RECORD[] MY_FORMAT_DATA(DataTable pDataHeader, DataTable pDataLines)
            {
                List<GL_RECORD> list_ = new List<GL_RECORD>();
                //Lines as trans
                MY_FORMAT_DATA_LINES(list_, pDataHeader, pDataLines);
                return list_.ToArray();
            }

            //string MY_GET_ACC_SEARCH_SQL(string pDoc, string pTrans, string pCard)
            //{

            //    return MY_GLREL.MY_GET_ACC_SEARCH_SQL(pDoc, pTrans, pCard);


            //    string sql_ =
            //    "DECLARE" + "\n" +
            //    "@TRCODE smallint," + "\n" +
            //    "@DOCCODE nvarchar(25)," + "\n" +
            //    "@RELCODE1 nvarchar(25)," + "\n" +
            //    "@RELCODE2 nvarchar(25)," + "\n" +
            //    "@RELCODE3 nvarchar(25)," + "\n" +
            //    "@RELCODE4 nvarchar(25)," + "\n" +
            //    "@RELCODE5 nvarchar(25)," + "\n" +
            //    "@RELCODE6 nvarchar(25)," + "\n" +
            //    "@RELCODE7 nvarchar(25)" + "\n" +
            //    "SELECT" + "\n" +
            //    "@TRCODE = @P1," + "\n" +
            //    "@DOCCODE = @P2," + "\n" +
            //    "@RELCODE1 = @P3," + "\n" +
            //    "@RELCODE2 = [dbo].[f_GLREL_$FIRM$_$PERIOD$_DOC_{0}] (@P4,1,0)," + "\n" +
            //    "@RELCODE3 = [dbo].[f_GLREL_$FIRM$_$PERIOD$_DOC_{0}] (@P5,2,0)," + "\n" +
            //    "@RELCODE4 = [dbo].[f_GLREL_$FIRM$_$PERIOD$_TRANS_{1}] (@P6,1,0)," + "\n" +
            //    "@RELCODE5 = [dbo].[f_GLREL_$FIRM$_$PERIOD$_TRANS_{1}] (@P7,2,0)," + "\n" +
            //    "@RELCODE6 = [dbo].[f_GLREL_$FIRM$_CARD_{2}] (@P8,1,0)," + "\n" +
            //    "@RELCODE7 = [dbo].[f_GLREL_$FIRM$_CARD_{2}] (@P9,2,0)" + "\n" +
            //    "EXEC [dbo].[p_GLREL_$FIRM$_GET_ACC_FROM_REL] " + "\n" +
            //    "@TRCODE," + "\n" +
            //    "@DOCCODE," + "\n" +
            //    "@RELCODE1," + "\n" +
            //    "@RELCODE2," + "\n" +
            //    "@RELCODE3," + "\n" +
            //    "@RELCODE4," + "\n" +
            //    "@RELCODE5," + "\n" +
            //    "@RELCODE6," + "\n" +
            //    "@RELCODE7" + "\n";

            //    return string.Format(sql_, pDoc, pTrans, pCard);
            //}

            object MY_GET_TRANS_ACC(DataRow pDocRow, DataRow pTransRow, GL_TRANS_TYPE pTransType, GLREL_TRCODE pGLRelType, string pGLRelFilterCode)
            {
                string cardTable_ = string.Empty;
                string cardColumn_ = string.Empty;
                switch (pTransType)
                {
                    case GL_TRANS_TYPE.bank:
                    case GL_TRANS_TYPE.bankCreditPair:
                    case GL_TRANS_TYPE.bankDebitPair:
                        cardTable_ = TABLE_BANKACC.TABLE;
                        cardColumn_ = TABLE_BNFLINE.BNACCREF;
                        break;
                    case GL_TRANS_TYPE.client:
                        cardTable_ = TABLE_CLCARD.TABLE;
                        cardColumn_ = TABLE_BNFLINE.CLIENTREF;
                        break;
                    default:
                        throw MY_EXCEPTION_INVALID_VAR(typeof(GL_TRANS_TYPE).Name, pTransType);
                }
                string[] tablesArr_ = new string[] { pDocRow.Table.TableName, pTransRow.Table.TableName, cardTable_ };

                string sql_ = MY_GLREL.MY_GET_ACC_SEARCH_SQL(tablesArr_[0], tablesArr_[1], tablesArr_[2]);

                object docLRef_ = TAB_GETROW(pDocRow, TABLE_BNFICHE.LOGICALREF);
                object transLRef_ = TAB_GETROW(pTransRow, TABLE_BNFLINE.LOGICALREF);
                object cardLRef_ = TAB_GETROW(pTransRow, cardColumn_);

                //    object[] var_ = new object[]{
                //    (short)pGLRelType,
                //    MY_GLREL_DOCCODE(pDocRow),
                //    pGLRelFilterCode,
                //    docLRef_,
                //    docLRef_,
                //    transLRef_,
                //    transLRef_,
                //    cardLRef_,
                //    cardLRef_
                //};

                //    MY_JOURNAL_WRITE(tablesArr_, var_);

                //    object acc_ = PLUGIN.SQLSCALAR(sql_, var_);
                //    MY_CHECK_ACC(acc_);
                //    return acc_;



                return MY_GLREL.MY_GET_ACC(PLUGIN, sql_,

                     (short)pGLRelType,
              MY_GLREL_DOCCODE(pDocRow),
              pGLRelFilterCode,
              docLRef_,
              docLRef_,
              transLRef_,
              transLRef_,
              cardLRef_,
              cardLRef_
                  );


            }

            void MY_CHECK_ACC(object pLRef)
            {
                if (CASTASINT(ISNULL(pLRef, 0)) == 0)
                    throw MY_EXCEPTION_INVALID_LREF(TABLE_EMUHACC.TABLE, TABLE_EMUHACC.LOGICALREF, pLRef);
            }

            bool MY_GET_TRANS_BANK_SIDE(DataRow pTransRow)
            {
                short trcode_ = -1;
                short sign_ = -1;
                bool res_ = false;


                if (pTransRow.Table.TableName == TABLE_BNFLINE.TABLE)
                {
                    trcode_ = CASTASSHORT(TAB_GETROW(pTransRow, TABLE_BNFLINE.TRCODE));
                    sign_ = CASTASSHORT(TAB_GETROW(pTransRow, TABLE_BNFLINE.SIGN));


                    switch (trcode_)
                    {
                        case 3:
                            res_ = true;
                            break;
                        case 4:
                            res_ = false;
                            break;
                        case 1:
                        case 2:
                        case 6:
                            if (sign_ == (short)FIN_SIGN.debit)
                                res_ = true;
                            else
                                if (sign_ == (short)FIN_SIGN.credit)
                                    res_ = false;
                                else
                                    throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_BNFLINE.TABLE, TABLE_BNFLINE.SIGN, sign_);
                            break;

                        default:
                            throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_BNFLINE.TABLE, TABLE_BNFLINE.TRCODE, trcode_);
                    }
                }
                else
                    throw MY_EXCEPTION_INVALID_TAB(pTransRow.Table.TableName);


                return res_;
            }
            string MY_GET_TRANS_COL(DataRow pTransRow, GL_TRANS_TYPE pTransType)
            {

                bool res_ = MY_GET_TRANS_BANK_SIDE(pTransRow);

                switch (pTransType)
                {
                    case GL_TRANS_TYPE.bank:
                        // res_ = res_;
                        break;
                    case GL_TRANS_TYPE.client:
                    case GL_TRANS_TYPE.bankDebitPair:
                    case GL_TRANS_TYPE.bankCreditPair:
                        res_ = !res_;
                        break;
                    default:
                        throw MY_EXCEPTION_INVALID_VAR(typeof(GL_TRANS_TYPE).Name, pTransType);

                }


                return MY_ACC_SIDE_TO_COL(res_);
            }

            string MY_ACC_SIDE_TO_COL(bool pAccSide)
            {
                return pAccSide ? TABLE_EMFLINE.DEBIT : TABLE_EMFLINE.CREDIT;
            }
            string MY_GLREL_DOCCODE(DataRow pDocRow)
            {
                string nr_ = FORMAT(TAB_GETROW(pDocRow, TABLE_BNFICHE.TRCODE));
                return "BANK" + "." + nr_;
            }


            void MY_FORMAT_DATA_LINES(List<GL_RECORD> pList, DataTable pDataHeader, DataTable pDataLines)
            {
                DataRow headerRecord_ = TAB_GETLASTROW(pDataHeader);
                //

                var toGlLines = MY_LINES_TO_GL_LINES(pDataHeader, pDataLines);
                //
                foreach (GL_TRANS_DESC desc_ in toGlLines)
                {
                    GL_RECORD records_ = new GL_RECORD();
                    DataRow transRecord_ = desc_.record;
                    //
                    MY_JOURNAL_WRITE(transRecord_);
                    //
                    double glValue_ = 0;
                    GLREL_TRCODE glRelTrcode_ = GLREL_TRCODE.undef;
                    string glRelFilter_ = GLREL_RELCODE1.EMPTY;

                    switch (desc_.type)
                    {

                        case GL_TRANS_TYPE.client:
                            glRelTrcode_ = GLREL_TRCODE.personalCarad;
                            glValue_ = MY_GET_LINE_TOTAL(transRecord_);
                            glRelFilter_ = GLREL_RELCODE1.EMPTY;
                            break;
                        case GL_TRANS_TYPE.bank:
                            glRelTrcode_ = GLREL_TRCODE.bank;
                            glValue_ = MY_GET_LINE_TOTAL(transRecord_);
                            glRelFilter_ = GLREL_RELCODE1.EMPTY;
                            break;
                        case GL_TRANS_TYPE.bankDebitPair:
                            glRelTrcode_ = GLREL_TRCODE.bank;
                            glValue_ = MY_GET_LINE_TOTAL(transRecord_);
                            glRelFilter_ = GLREL_RELCODE1.DPAIR;
                            break;
                        case GL_TRANS_TYPE.bankCreditPair:
                            glRelTrcode_ = GLREL_TRCODE.bank;
                            glValue_ = MY_GET_LINE_TOTAL(transRecord_);
                            glRelFilter_ = GLREL_RELCODE1.CPAIR;
                            break;


                    }

                    if (!ISNUMZERO(glValue_))
                    {
                        records_.Add(TABLE_EMFLINE.ACCOUNTREF, MY_GET_TRANS_ACC(headerRecord_, transRecord_, desc_.type, glRelTrcode_, glRelFilter_));
                        records_.Add(MY_GET_TRANS_COL(transRecord_, desc_.type), glValue_);
                        records_.Add(TABLE_EMFLINE.REPORTRATE, TAB_GETROW(transRecord_, TABLE_BNFLINE.REPORTRATE));
                        records_.Add(TABLE_EMFLINE.TRCURR, TAB_GETROW(transRecord_, TABLE_BNFLINE.TRCURR));
                        records_.Add(TABLE_EMFLINE.TRRATE, TAB_GETROW(transRecord_, TABLE_BNFLINE.TRRATE));
                        //
                        //records_.Add(TABLE_EMFLINE.SPECODE, TAB_GETROW(headerRecord_, TABLE_BNFICHE.SPECODE));
                        // records_.Add(TABLE_EMFLINE.INVOICENO, TAB_GETROW(headerRecord_, TABLE_BNFICHE.FICHENO));

                        records_.Add(TABLE_EMFLINE.SPECODE, TAB_GETROW(transRecord_, TABLE_BNFLINE.SPECODE));
                        records_.Add(TABLE_EMFLINE.LINEEXP, TAB_GETROW(transRecord_, TABLE_BNFLINE.LINEEXP));

                        //records_.Add(TABLE_EMFLINE.CLDEF, TAB_GETROW(transRecord_, MY_GET_TRANS_CARD_INFO(transRecord_, desc_.type, false)));

                        //records_.Add(TABLE_EMFLINE.LINEEXP, JOINLIST(FORMAT(new object[] { 
                        //MY_GLREL_DOCCODE(headerRecord_), 
                        //TAB_GETROW(transRecord_,  MY_GET_TRANS_CARD_INFO(transRecord_, desc_.type, true  )), 
                        //(short)glRelTrcode_, 

                        //glRelFilter_ })));
                        //
                        pList.Add(records_);
                    }
                    //


                }
            }
            string MY_GET_TRANS_CARD_INFO(DataRow pTransRow, GL_TRANS_TYPE pTransType, bool pCode)
            {
                switch (pTransType)
                {
                    case GL_TRANS_TYPE.bank:
                    case GL_TRANS_TYPE.bankCreditPair:
                    case GL_TRANS_TYPE.bankDebitPair:
                        return (pCode ? TABLE_BNFLINE.E_BANKACC__CODE : TABLE_BNFLINE.E_BANKACC__DEFINITION_);
                    case GL_TRANS_TYPE.client:
                        return (pCode ? TABLE_BNFLINE.E_CLCARD__CODE : TABLE_BNFLINE.E_CLCARD__DEFINITION_);
                    default:
                        throw MY_EXCEPTION_INVALID_VAR(typeof(GL_TRANS_TYPE).Name, pTransType);
                }
            }


            double MY_GET_LINE_TOTAL(DataRow pTransRow)
            {
                double total_ = 0;
                if (pTransRow.Table.TableName == TABLE_BNFLINE.TABLE)
                    total_ = CASTASDOUBLE(TAB_GETROW(pTransRow, TABLE_BNFLINE.AMOUNT));
                else
                    throw MY_EXCEPTION_INVALID_TAB(pTransRow.Table.TableName);
                return total_;
            }


            GL_TRANS_DESC[] MY_LINES_TO_GL_LINES(DataTable pDataHeader, DataTable pDataLines)
            {
                List<GL_TRANS_DESC> list_ = new List<GL_TRANS_DESC>();


                foreach (DataRow transRecord_ in pDataLines.Rows)
                {
                    switch (transRecord_.Table.TableName)
                    {
                        case TABLE_BNFLINE.TABLE:
                            short trcode_ = CASTASSHORT(TAB_GETROW(transRecord_, TABLE_BNFLINE.TRCODE));
                            short sign_ = CASTASSHORT(TAB_GETROW(transRecord_, TABLE_BNFLINE.SIGN));
                            switch (trcode_)
                            {
                                case 1:
                                    list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.bank));
                                    if (MY_TRANS_IS_FIN_DEBIT(transRecord_))
                                        list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.bankDebitPair));
                                    else
                                        list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.bankCreditPair));
                                    break;
                                case 2:
                                    list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.bank));
                                    break;
                                case 3:
                                case 4:
                                    list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.bank));
                                    list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.client));
                                    break;
                                case 6:
                                    list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.bank));
                                    if (MY_TRANS_IS_FIN_DEBIT(transRecord_))
                                        list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.bankDebitPair));
                                    else
                                        list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.bankCreditPair));
                                    break;
                                default:
                                    throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_BNFLINE.TABLE, TABLE_BNFLINE.TRCODE, trcode_);
                            }
                            break;
                        default:
                            throw MY_EXCEPTION_INVALID_TAB(transRecord_.Table.TableName);

                    }
                }

                return list_.ToArray();
            }

            Exception MY_EXCEPTION_INVALID_COL_VALUE(string pTab, string pCol, object pVal)
            {
                return new Exception(string.Format("T_MSG_ERROR_INVALID_VAR, {0}/{1}/{2}", pTab, pCol, pVal));
            }
            Exception MY_EXCEPTION_INVALID_TAB(string pTab)
            {
                return new Exception(string.Format("T_MSG_ERROR_INVALID_VAR, T_TABLE '{0}'", pTab));
            }
            Exception MY_EXCEPTION_INVALID_VAR(string pDesc, object pVal)
            {
                return new Exception(string.Format("T_MSG_ERROR_INVALID_VAR, {0}/{1}", pDesc, pVal));
            }
            Exception MY_EXCEPTION_INVALID_LREF(string pTab, string pCol, object pVal)
            {
                return new Exception(string.Format("T_MSG_ERROR_INVALID_LREF_TYPE, {0}/{1}/{2}", pTab, pCol, pVal));
            }
            void MY_CHECK_IS_WELL_KNOWN_TRANS(DataRow pLine)
            {
                switch (pLine.Table.TableName)
                {
                    case TABLE_BNFLINE.TABLE:
                        {
                            short trcode_ = CASTASSHORT(TAB_GETROW(pLine, TABLE_BNFLINE.TRCODE));
                            short sign_ = CASTASSHORT(TAB_GETROW(pLine, TABLE_BNFLINE.SIGN));
                            switch (trcode_)
                            {
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                case 6:
                                    switch (sign_)
                                    {
                                        case (short)FIN_SIGN.debit:
                                        case (short)FIN_SIGN.credit:
                                            return;
                                        default:
                                            throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_BNFLINE.TABLE, TABLE_BNFLINE.SIGN, sign_);
                                    }
                                default:
                                    throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_BNFLINE.TABLE, TABLE_BNFLINE.TRCODE, trcode_);

                            }

                        }
                    default:
                        throw MY_EXCEPTION_INVALID_TAB(pLine.Table.TableName);

                }
            }


            bool MY_TRANS_IS_FIN_DEBIT(DataRow pLine)
            {
                if (pLine.Table.TableName == TABLE_BNFLINE.TABLE)
                {
                    short sign_ = CASTASSHORT(TAB_GETROW(pLine, TABLE_BNFLINE.SIGN));
                    if (sign_ == (short)FIN_SIGN.debit)
                        return true;
                    else
                        if (sign_ == (short)FIN_SIGN.credit)
                            return false;

                    throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_BNFLINE.TABLE, TABLE_BNFLINE.SIGN, sign_);
                }
                throw MY_EXCEPTION_INVALID_TAB(pLine.Table.TableName);

            }
            void MY_SYNC_HEADERS(DataTable pDataHeader, DataTable pGlHeader)
            {
                GL_RECORD records_ = new GL_RECORD();

                records_.Add(TABLE_EMFICHE.DATE_, TAB_GETROW(pDataHeader, TABLE_BNFICHE.DATE_));
                records_.Add(TABLE_EMFICHE.CANCELLED, TAB_GETROW(pDataHeader, TABLE_BNFICHE.CANCELLED));
                records_.Add(TABLE_EMFICHE.BRANCH, TAB_GETROW(pDataHeader, TABLE_BNFICHE.BRANCH));
                records_.Add(TABLE_EMFICHE.DEPARTMENT, TAB_GETROW(pDataHeader, TABLE_BNFICHE.DEPARMENT));
                records_.Add(TABLE_EMFICHE.GENEXP1, TAB_GETROW(pDataHeader, TABLE_BNFICHE.GENEXP1));
                records_.Add(TABLE_EMFICHE.GENEXP2, TAB_GETROW(pDataHeader, TABLE_BNFICHE.GENEXP2));
                records_.Add(TABLE_EMFICHE.GENEXP3, TAB_GETROW(pDataHeader, TABLE_BNFICHE.GENEXP3));
                records_.Add(TABLE_EMFICHE.GENEXP4, TAB_GETROW(pDataHeader, TABLE_BNFICHE.GENEXP4));
                records_.Add(TABLE_EMFICHE.SPECODE, TAB_GETROW(pDataHeader, TABLE_BNFICHE.SPECODE));
                records_.Add(TABLE_EMFICHE.SPECODE2, TAB_GETROW(pDataHeader, TABLE_BNFICHE.SPECODE2));
                records_.Add(TABLE_EMFICHE.SPECODE3, TAB_GETROW(pDataHeader, TABLE_BNFICHE.SPECODE3));
                records_.Add(TABLE_EMFICHE.CYPHCODE, TAB_GETROW(pDataHeader, TABLE_BNFICHE.CYPHCODE));
                records_.Add(TABLE_EMFICHE.FICHENO, MY_TOOLS.CRETAGLFICHENO(pDataHeader));

                foreach (KeyValuePair<string, object> pair_ in records_)
                    TAB_SETCOL(pGlHeader, pair_.Key, pair_.Value);

            }

            void MY_CHECK_TABLES(DataTable pDataHeader, DataTable pDataLines, DataTable pGlHeader, DataTable pGlLines)
            {
                if (pDataHeader == null || TAB_GETLASTROW(pDataHeader) == null)
                    EXCEPTIONFORUSER(string.Format("T_MSG_DATA_NO (T_TABLE '{0}')", TABLE_BNFICHE.TABLE));

                MY_JOURNAL_WRITE(pDataHeader);

                if (pDataLines == null)
                    EXCEPTIONFORUSER(string.Format("T_MSG_DATA_NO (T_TABLE '{0}')", TABLE_BNFLINE.TABLE));

                foreach (DataRow row_ in pDataLines.Rows)
                    MY_CHECK_IS_WELL_KNOWN_TRANS(row_);

                if (pGlHeader == null || TAB_GETLASTROW(pGlHeader) == null)
                    EXCEPTIONFORUSER(string.Format("T_MSG_DATA_NO (T_TABLE '{0}')", TABLE_EMFICHE.TABLE));

                if (pGlLines == null)
                    EXCEPTIONFORUSER(string.Format("T_MSG_DATA_NO (T_TABLE '{0}')", TABLE_EMFLINE.TABLE));


            }





        }

        #endregion
        #region CLIENT

        class GLDO_CLIENT
        {


            public GLDO_CLIENT(_PLUGIN pPLUGIN)
            {
                PLUGIN = pPLUGIN;
            }
            _PLUGIN PLUGIN;


            class GL_RECORD : Dictionary<string, object>
            {

            }
            class GL_TRANS_DESC
            {
                public GL_TRANS_DESC(DataRow pRecord, DataRow pParent, GL_TRANS_TYPE pType)
                {
                    record = pRecord;
                    parent = pParent;
                    type = pType;
                }
                public GL_TRANS_TYPE type;
                public DataRow record;
                public DataRow parent;
            }
            enum GL_TRANS_TYPE
            {
                client = 1,
                clientPair = 2,
                clientDebitPair = 3,
                clientCreditPair = 4,
                VAT = 5
            }
            enum FIN_SIGN
            {
                debit = 0,
                credit = 1
            }
            enum GLREL_TRCODE
            {
                undef = 0,
                material = 1,
                personalCarad = 2,
                service = 3,
                VAT = 4,
                cash = 5,
                bank = 6,
                discount = 7,
                surcharge = 8,
                promo = 9

            }
            class GLREL_RELCODE1
            {
                public const string EMPTY = "";
                public const string PAIR = "PAIR";
                public const string CPAIR = "CPAIR";
                public const string DPAIR = "DPAIR";
            }
            public void MY_GL(DataSet DATASETS, DataSet DATASETD)
            {
                DataTable dataHeader_;
                DataTable dataLines_;

                DataTable glHeader_;
                DataTable glLines_;

                dataHeader_ = MY_GETTAB(DATASETS, TABLE_CLFICHE.TABLE);
                dataLines_ = MY_GETTAB(DATASETS, TABLE_INVOICE.TABLE) != null ? MY_GETTAB(DATASETS, TABLE_INVOICE.TABLE) : MY_GETTAB(DATASETS, TABLE_CLFLINE.TABLE);

                glHeader_ = MY_GETTAB(DATASETD, TABLE_EMFICHE.TABLE);
                glLines_ = MY_GETTAB(DATASETD, TABLE_EMFLINE.TABLE);

                MY_CHECK_TABLES(dataHeader_, dataLines_, glHeader_, glLines_);
                MY_SYNC_HEADERS(dataHeader_, glHeader_);
                MY_PARSE_DATA(glHeader_, glLines_, MY_FORMAT_DATA(dataHeader_, dataLines_));

            }
            void MY_JOURNAL_WRITE(string[] pTables, object[] pArr)
            {
                if (!_useJournal)
                    return;
                PLUGIN.JOURNAL(JOURNAL_NS,
                string.Format(PLUGIN.RESOLVESTR("$search\t{0}/{1}"),
                JOINLIST(pTables),
                JOINLIST(FORMAT(pArr))));
            }
            void MY_JOURNAL_WRITE(DataTable pDocHeader)
            {
                if (!_useJournal)
                    return;
                DataRow headerRow_ = TAB_GETLASTROW(pDocHeader);
                PLUGIN.JOURNAL(JOURNAL_NS,
                    string.Format(PLUGIN.RESOLVESTR(Environment.NewLine + "$doc\t({0}/{1}/{2})\t[lang::T_TYPE],{3};[lang::T_NR],{4}"),
                    TABLE_CLFICHE.TABLE,
                    TABLE_CLFICHE.LOGICALREF,
                    TAB_GETROW(headerRow_, TABLE_CLFICHE.LOGICALREF),
                    MY_GLREL_DOCCODE(headerRow_),
                    TAB_GETROW(headerRow_, TABLE_CLFICHE.FICHENO)));
            }
            void MY_JOURNAL_WRITE(DataRow pTrans)
            {
                if (!_useJournal)
                    return;

                PLUGIN.JOURNAL(JOURNAL_NS,
                    string.Format(PLUGIN.RESOLVESTR("$line\t({0}/{1}/{2})"),
                    pTrans.Table.TableName,
                    TABLE_CLFICHE.LOGICALREF,
                    TAB_GETROW(pTrans, TABLE_CLFICHE.LOGICALREF)));
            }

            void MY_PARSE_DATA(DataTable pGlHeader, DataTable pGlLines, GL_RECORD[] pRecords)
            {
                foreach (GL_RECORD rec_ in pRecords)
                {
                    TAB_ADDROW(pGlLines);
                    foreach (KeyValuePair<string, object> pair_ in rec_)
                        TAB_SETROW(pGlLines, pair_.Key, pair_.Value);
                }

            }

            GL_RECORD[] MY_FORMAT_DATA(DataTable pDataHeader, DataTable pDataLines)
            {
                List<GL_RECORD> list_ = new List<GL_RECORD>();
                //Lines as trans
                MY_FORMAT_DATA_LINES(list_, pDataHeader, pDataLines);
                return list_.ToArray();
            }

            //string MY_GET_ACC_SEARCH_SQL(string pDoc, string pTrans, string pCard)
            //{

            //    return MY_GLREL.MY_GET_ACC_SEARCH_SQL(pDoc, pTrans, pCard);


            //    string sql_ =
            //    "DECLARE" + "\n" +
            //    "@TRCODE smallint," + "\n" +
            //    "@DOCCODE nvarchar(25)," + "\n" +
            //    "@RELCODE1 nvarchar(25)," + "\n" +
            //    "@RELCODE2 nvarchar(25)," + "\n" +
            //    "@RELCODE3 nvarchar(25)," + "\n" +
            //    "@RELCODE4 nvarchar(25)," + "\n" +
            //    "@RELCODE5 nvarchar(25)," + "\n" +
            //    "@RELCODE6 nvarchar(25)," + "\n" +
            //    "@RELCODE7 nvarchar(25)" + "\n" +
            //    "SELECT" + "\n" +
            //    "@TRCODE = @P1," + "\n" +
            //    "@DOCCODE = @P2," + "\n" +
            //    "@RELCODE1 = @P3," + "\n" +
            //    "@RELCODE2 = [dbo].[f_GLREL_$FIRM$_$PERIOD$_DOC_{0}] (@P4,1,0)," + "\n" +
            //    "@RELCODE3 = [dbo].[f_GLREL_$FIRM$_$PERIOD$_DOC_{0}] (@P5,2,0)," + "\n" +
            //    "@RELCODE4 = [dbo].[f_GLREL_$FIRM$_$PERIOD$_TRANS_{1}] (@P6,1,0)," + "\n" +
            //    "@RELCODE5 = [dbo].[f_GLREL_$FIRM$_$PERIOD$_TRANS_{1}] (@P7,2,0)," + "\n" +
            //    "@RELCODE6 = [dbo].[f_GLREL_$FIRM$_CARD_{2}] (@P8,1,0)," + "\n" +
            //    "@RELCODE7 = [dbo].[f_GLREL_$FIRM$_CARD_{2}] (@P9,2,0)" + "\n" +
            //    "EXEC [dbo].[p_GLREL_$FIRM$_GET_ACC_FROM_REL] " + "\n" +
            //    "@TRCODE," + "\n" +
            //    "@DOCCODE," + "\n" +
            //    "@RELCODE1," + "\n" +
            //    "@RELCODE2," + "\n" +
            //    "@RELCODE3," + "\n" +
            //    "@RELCODE4," + "\n" +
            //    "@RELCODE5," + "\n" +
            //    "@RELCODE6," + "\n" +
            //    "@RELCODE7" + "\n";

            //    return string.Format(sql_, pDoc, pTrans, pCard);
            //}

            object MY_GET_TRANS_ACC(DataRow pDocRow, DataRow pTransRow, GL_TRANS_TYPE pTransType, GLREL_TRCODE pGLRelType, string pGLRelFilterCode)
            {
                string[] tablesArr_ = new string[] { pDocRow.Table.TableName, pTransRow.Table.TableName, TABLE_CLCARD.TABLE };

                string sql_ = MY_GLREL.MY_GET_ACC_SEARCH_SQL(tablesArr_[0], tablesArr_[1], tablesArr_[2]);

                object docLRef_ = TAB_GETROW(pDocRow, TABLE_CLFICHE.LOGICALREF);
                object transLRef_ = TAB_GETROW(pTransRow, pTransRow.Table.TableName == TABLE_CLFLINE.TABLE ? TABLE_CLFLINE.LOGICALREF : TABLE_INVOICE.LOGICALREF);
                object cardLRef_ = TAB_GETROW(pTransRow, pTransRow.Table.TableName == TABLE_CLFLINE.TABLE ? TABLE_CLFLINE.CLIENTREF : TABLE_INVOICE.CLIENTREF);

                //    object[] var_ = new object[]{
                //    (short)pGLRelType,
                //    MY_GLREL_DOCCODE(pDocRow),
                //    pGLRelFilterCode,
                //    docLRef_,
                //    docLRef_,
                //    transLRef_,
                //    transLRef_,
                //    cardLRef_,
                //    cardLRef_
                //};

                //    MY_JOURNAL_WRITE(tablesArr_, var_);

                //    object acc_ = PLUGIN.SQLSCALAR(sql_, var_);
                //    MY_CHECK_ACC(acc_);
                //    return acc_;



                return MY_GLREL.MY_GET_ACC(PLUGIN, sql_,

                     (short)pGLRelType,
             MY_GLREL_DOCCODE(pDocRow),
             pGLRelFilterCode,
             docLRef_,
             docLRef_,
             transLRef_,
             transLRef_,
             cardLRef_,
             cardLRef_
                 );

            }

            void MY_CHECK_ACC(object pLRef)
            {
                if (CASTASINT(ISNULL(pLRef, 0)) == 0)
                    throw MY_EXCEPTION_INVALID_LREF(TABLE_EMUHACC.TABLE, TABLE_EMUHACC.LOGICALREF, pLRef);
            }

            bool MY_GET_TRANS_CLIENT_SIDE(DataRow pTransRow)
            {
                short trcode_ = -1;
                short sign_ = -1;
                bool res_ = false;


                if (pTransRow.Table.TableName == TABLE_CLFLINE.TABLE)
                {
                    trcode_ = CASTASSHORT(TAB_GETROW(pTransRow, TABLE_CLFLINE.TRCODE));
                    sign_ = CASTASSHORT(TAB_GETROW(pTransRow, TABLE_CLFLINE.SIGN));


                    switch (trcode_)
                    {
                        case 2:
                        case 3:
                            res_ = true;
                            break;
                        case 1:
                        case 4:
                            res_ = false;
                            break;
                        case 5:
                        case 6:

                            if (sign_ == (short)FIN_SIGN.debit)
                                res_ = true;
                            else
                                if (sign_ == (short)FIN_SIGN.credit)
                                    res_ = false;
                                else
                                    throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_CLFLINE.TABLE, TABLE_CLFLINE.SIGN, sign_);
                            break;

                        default:
                            throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_CLFLINE.TABLE, TABLE_CLFLINE.TRCODE, trcode_);
                    }
                }
                else
                    if (pTransRow.Table.TableName == TABLE_INVOICE.TABLE)
                    {
                        trcode_ = CASTASSHORT(TAB_GETROW(pTransRow, TABLE_INVOICE.TRCODE));
                        switch (trcode_)
                        {
                            case 11:
                                res_ = true;
                                break;
                            case 12:
                                res_ = false;
                                break;
                            default:
                                throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_INVOICE.TABLE, TABLE_INVOICE.TRCODE, trcode_);
                        }
                    }
                    else
                        throw MY_EXCEPTION_INVALID_TAB(pTransRow.Table.TableName);


                return res_;
            }
            string MY_GET_TRANS_COL(DataRow pTransRow, GL_TRANS_TYPE pTransType)
            {

                bool res_ = MY_GET_TRANS_CLIENT_SIDE(pTransRow);

                switch (pTransType)
                {
                    case GL_TRANS_TYPE.client:
                        // res_ = res_;
                        break;
                    case GL_TRANS_TYPE.clientPair:
                    case GL_TRANS_TYPE.clientDebitPair:
                    case GL_TRANS_TYPE.clientCreditPair:
                    case GL_TRANS_TYPE.VAT:
                        res_ = !res_;
                        break;
                    default:
                        throw MY_EXCEPTION_INVALID_VAR(typeof(GL_TRANS_TYPE).Name, pTransType);

                }


                return MY_ACC_SIDE_TO_COL(res_);
            }

            string MY_ACC_SIDE_TO_COL(bool pAccSide)
            {
                return pAccSide ? TABLE_EMFLINE.DEBIT : TABLE_EMFLINE.CREDIT;
            }
            string MY_GLREL_DOCCODE(DataRow pDocRow)
            {
                string nr_ = FORMAT(TAB_GETROW(pDocRow, TABLE_CLFICHE.TRCODE));
                return "CLIENT" + "." + nr_;
            }


            void MY_FORMAT_DATA_LINES(List<GL_RECORD> pList, DataTable pDataHeader, DataTable pDataLines)
            {
                DataRow headerRecord_ = TAB_GETLASTROW(pDataHeader);
                //

                var toGlLines = MY_LINES_TO_GL_LINES(pDataHeader, pDataLines);
                //
                foreach (GL_TRANS_DESC desc_ in toGlLines)
                {
                    GL_RECORD records_ = new GL_RECORD();
                    DataRow transRecord_ = desc_.record;
                    //
                    MY_JOURNAL_WRITE(transRecord_);
                    //
                    double glValue_ = 0;
                    GLREL_TRCODE glRelTrcode_ = GLREL_TRCODE.undef;
                    string glRelFilter_ = GLREL_RELCODE1.EMPTY;

                    switch (desc_.type)
                    {

                        case GL_TRANS_TYPE.client:
                            glRelTrcode_ = GLREL_TRCODE.personalCarad;
                            glValue_ = MY_GET_LINE_TOTAL(transRecord_);
                            glRelFilter_ = GLREL_RELCODE1.EMPTY;
                            break;
                        case GL_TRANS_TYPE.clientPair:
                            glRelTrcode_ = GLREL_TRCODE.personalCarad;
                            glValue_ = MY_GET_LINE_TOTAL_NET(transRecord_);
                            glRelFilter_ = GLREL_RELCODE1.PAIR;
                            break;
                        case GL_TRANS_TYPE.clientDebitPair:
                            glRelTrcode_ = GLREL_TRCODE.personalCarad;
                            glValue_ = MY_GET_LINE_TOTAL_NET(transRecord_);
                            glRelFilter_ = GLREL_RELCODE1.DPAIR;
                            break;
                        case GL_TRANS_TYPE.clientCreditPair:
                            glRelTrcode_ = GLREL_TRCODE.personalCarad;
                            glValue_ = MY_GET_LINE_TOTAL_NET(transRecord_);
                            glRelFilter_ = GLREL_RELCODE1.CPAIR;
                            break;
                        case GL_TRANS_TYPE.VAT:
                            glRelTrcode_ = GLREL_TRCODE.VAT;
                            glValue_ = MY_GET_LINE_VAT(transRecord_);
                            glRelFilter_ = GLREL_RELCODE1.EMPTY;
                            break;

                    }

                    if (!ISNUMZERO(glValue_))
                    {
                        records_.Add(TABLE_EMFLINE.ACCOUNTREF, MY_GET_TRANS_ACC(headerRecord_, transRecord_, desc_.type, glRelTrcode_, glRelFilter_));
                        records_.Add(MY_GET_TRANS_COL(transRecord_, desc_.type), glValue_);
                        records_.Add(TABLE_EMFLINE.REPORTRATE, TAB_GETROW(transRecord_, (transRecord_.Table.TableName == TABLE_CLFLINE.TABLE ? TABLE_CLFLINE.REPORTRATE : TABLE_INVOICE.REPORTRATE)));
                        records_.Add(TABLE_EMFLINE.TRCURR, TAB_GETROW(transRecord_, (transRecord_.Table.TableName == TABLE_CLFLINE.TABLE ? TABLE_CLFLINE.TRCURR : TABLE_INVOICE.TRCURR)));
                        records_.Add(TABLE_EMFLINE.TRRATE, TAB_GETROW(transRecord_, (transRecord_.Table.TableName == TABLE_CLFLINE.TABLE ? TABLE_CLFLINE.TRRATE : TABLE_INVOICE.TRRATE)));
                        //
                        //records_.Add(TABLE_EMFLINE.SPECODE, TAB_GETROW(headerRecord_, TABLE_CLFICHE.SPECODE));

                        //records_.Add(TABLE_EMFLINE.SPECODE, TAB_GETROW(transRecord_, TABLE_CLFLINE.SPECODE));

                        //  records_.Add(TABLE_EMFLINE.INVOICENO, TAB_GETROW(headerRecord_, TABLE_CLFICHE.FICHENO));
                        //  records_.Add(TABLE_EMFLINE.CLDEF, TAB_GETROW(transRecord_, (transRecord_.Table.TableName == TABLE_CLFLINE.TABLE ? TABLE_CLFLINE.E_CLCARD__DEFINITION_ : TABLE_INVOICE.E_CLCARD__DEFINITION_)));

                        records_.Add(TABLE_EMFLINE.SPECODE, TAB_GETROW(transRecord_, TABLE_CLFLINE.SPECODE));
                        records_.Add(TABLE_EMFLINE.LINEEXP, TAB_GETROW(transRecord_, TABLE_CLFLINE.LINEEXP));
                        //Add => [key] = value
                        //records_.Add(TABLE_EMFLINE.LINEEXP, JOINLIST(FORMAT(new object[] { 
                        //MY_GLREL_DOCCODE(headerRecord_), 
                        //TAB_GETROW(transRecord_, (transRecord_.Table.TableName == TABLE_CLFLINE.TABLE ? TABLE_CLFLINE.E_CLCARD__CODE : TABLE_INVOICE.E_CLCARD__CODE)), 
                        //(short)glRelTrcode_, 
                        //glRelFilter_ })));
                        ////

                        pList.Add(records_);
                    }
                    //


                }
            }

            double MY_GET_LINE_TOTAL_NET(DataRow pTransRow)
            {
                double total_ = 0;
                if (pTransRow.Table.TableName == TABLE_CLFLINE.TABLE)
                    total_ = MY_GET_LINE_TOTAL(pTransRow);
                else
                    if (pTransRow.Table.TableName == TABLE_INVOICE.TABLE)
                        total_ = MY_GET_LINE_TOTAL(pTransRow) - MY_GET_LINE_VAT(pTransRow);
                return total_;
            }

            double MY_GET_LINE_TOTAL(DataRow pTransRow)
            {
                double total_ = 0;
                if (pTransRow.Table.TableName == TABLE_CLFLINE.TABLE)
                    total_ = CASTASDOUBLE(TAB_GETROW(pTransRow, TABLE_CLFLINE.AMOUNT));
                else
                    if (pTransRow.Table.TableName == TABLE_INVOICE.TABLE)
                        total_ = CASTASDOUBLE(TAB_GETROW(pTransRow, TABLE_INVOICE.NETTOTAL));
                    else
                        throw MY_EXCEPTION_INVALID_TAB(pTransRow.Table.TableName);
                return total_;
            }

            double MY_GET_LINE_VAT(DataRow pTransRow)
            {
                double total_ = 0;
                if (pTransRow.Table.TableName == TABLE_INVOICE.TABLE)
                    total_ = CASTASDOUBLE(TAB_GETROW(pTransRow, TABLE_INVOICE.TOTALVAT));
                else
                    throw MY_EXCEPTION_INVALID_TAB(pTransRow.Table.TableName);
                return total_;
            }
            GL_TRANS_DESC[] MY_LINES_TO_GL_LINES(DataTable pDataHeader, DataTable pDataLines)
            {
                List<GL_TRANS_DESC> list_ = new List<GL_TRANS_DESC>();


                foreach (DataRow transRecord_ in pDataLines.Rows)
                {
                    switch (transRecord_.Table.TableName)
                    {
                        case TABLE_CLFLINE.TABLE:
                            {
                                short trcode_ = CASTASSHORT(TAB_GETROW(transRecord_, TABLE_CLFLINE.TRCODE));
                                short sign_ = CASTASSHORT(TAB_GETROW(transRecord_, TABLE_CLFLINE.SIGN));

                                switch (trcode_)
                                {
                                    case 1:
                                    case 2:
                                    case 3:
                                    case 4:
                                    case 41:
                                    case 42:
                                        list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.client));
                                        list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.clientPair));
                                        break;
                                    case 5:
                                        list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.client));
                                        break;
                                    case 6:
                                        list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.client));
                                        if (MY_TRANS_IS_FIN_DEBIT(transRecord_))
                                            list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.clientDebitPair));
                                        else
                                            list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.clientCreditPair));
                                        break;
                                    default:
                                        throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_CLFLINE.TABLE, TABLE_CLFLINE.TRCODE, trcode_);

                                }



                            }
                            break;
                        case TABLE_INVOICE.TABLE:
                            list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.client));
                            list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.clientPair));
                            list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.VAT));
                            break;
                        default:
                            throw MY_EXCEPTION_INVALID_TAB(transRecord_.Table.TableName);

                    }
                }

                return list_.ToArray();
            }

            Exception MY_EXCEPTION_INVALID_COL_VALUE(string pTab, string pCol, object pVal)
            {
                return new Exception(string.Format("T_MSG_ERROR_INVALID_VAR, {0}/{1}/{2}", pTab, pCol, pVal));
            }
            Exception MY_EXCEPTION_INVALID_TAB(string pTab)
            {
                return new Exception(string.Format("T_MSG_ERROR_INVALID_VAR, T_TABLE '{0}'", pTab));
            }
            Exception MY_EXCEPTION_INVALID_VAR(string pDesc, object pVal)
            {
                return new Exception(string.Format("T_MSG_ERROR_INVALID_VAR, {0}/{1}", pDesc, pVal));
            }
            Exception MY_EXCEPTION_INVALID_LREF(string pTab, string pCol, object pVal)
            {
                return new Exception(string.Format("T_MSG_ERROR_INVALID_LREF_TYPE, {0}/{1}/{2}", pTab, pCol, pVal));
            }
            void MY_CHECK_IS_WELL_KNOWN_TRANS(DataRow pLine)
            {
                switch (pLine.Table.TableName)
                {
                    case TABLE_CLFLINE.TABLE:
                        {
                            short trcode_ = CASTASSHORT(TAB_GETROW(pLine, TABLE_CLFLINE.TRCODE));
                            short sign_ = CASTASSHORT(TAB_GETROW(pLine, TABLE_CLFLINE.SIGN));
                            switch (trcode_)
                            {
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                case 5:
                                case 6:
                                    switch (sign_)
                                    {
                                        case (short)FIN_SIGN.debit:
                                        case (short)FIN_SIGN.credit:
                                            return;
                                        default:
                                            throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_CLFLINE.TABLE, TABLE_CLFLINE.SIGN, sign_);
                                    }
                                default:
                                    throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_CLFLINE.TABLE, TABLE_CLFLINE.TRCODE, trcode_);

                            }

                        }
                    case TABLE_INVOICE.TABLE:
                        {
                            short trcode_ = CASTASSHORT(TAB_GETROW(pLine, TABLE_INVOICE.TRCODE));
                            switch (trcode_)
                            {
                                case 11:
                                case 12:
                                    return;
                                default:
                                    throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_INVOICE.TABLE, TABLE_INVOICE.TRCODE, trcode_);

                            }
                        }
                    default:
                        throw MY_EXCEPTION_INVALID_TAB(pLine.Table.TableName);

                }
            }

            bool MY_TRANS_IS_FIN_DEBIT(DataRow pLine)
            {
                if (pLine.Table.TableName == TABLE_CLFLINE.TABLE)
                {
                    short sign_ = CASTASSHORT(TAB_GETROW(pLine, TABLE_CLFLINE.SIGN));
                    if (sign_ == (short)FIN_SIGN.debit)
                        return true;
                    else
                        if (sign_ == (short)FIN_SIGN.credit)
                            return false;

                    throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_CLFLINE.TABLE, TABLE_CLFLINE.SIGN, sign_);
                }
                throw MY_EXCEPTION_INVALID_TAB(pLine.Table.TableName);

            }
            void MY_SYNC_HEADERS(DataTable pDataHeader, DataTable pGlHeader)
            {
                GL_RECORD records_ = new GL_RECORD();

                records_.Add(TABLE_EMFICHE.DATE_, TAB_GETROW(pDataHeader, TABLE_CLFICHE.DATE_));
                records_.Add(TABLE_EMFICHE.CANCELLED, TAB_GETROW(pDataHeader, TABLE_CLFICHE.CANCELLED));
                records_.Add(TABLE_EMFICHE.BRANCH, TAB_GETROW(pDataHeader, TABLE_CLFICHE.BRANCH));
                records_.Add(TABLE_EMFICHE.DEPARTMENT, TAB_GETROW(pDataHeader, TABLE_CLFICHE.DEPARTMENT));
                records_.Add(TABLE_EMFICHE.GENEXP1, TAB_GETROW(pDataHeader, TABLE_CLFICHE.GENEXP1));
                records_.Add(TABLE_EMFICHE.GENEXP2, TAB_GETROW(pDataHeader, TABLE_CLFICHE.GENEXP2));
                records_.Add(TABLE_EMFICHE.GENEXP3, TAB_GETROW(pDataHeader, TABLE_CLFICHE.GENEXP3));
                records_.Add(TABLE_EMFICHE.GENEXP4, TAB_GETROW(pDataHeader, TABLE_CLFICHE.GENEXP4));
                records_.Add(TABLE_EMFICHE.SPECODE, TAB_GETROW(pDataHeader, TABLE_CLFICHE.SPECCODE));
                records_.Add(TABLE_EMFICHE.SPECODE2, TAB_GETROW(pDataHeader, TABLE_CLFICHE.SPECODE2));
                records_.Add(TABLE_EMFICHE.SPECODE3, TAB_GETROW(pDataHeader, TABLE_CLFICHE.SPECODE3));
                records_.Add(TABLE_EMFICHE.CYPHCODE, TAB_GETROW(pDataHeader, TABLE_CLFICHE.CYPHCODE));
                records_.Add(TABLE_EMFICHE.FICHENO, MY_TOOLS.CRETAGLFICHENO(pDataHeader));

                foreach (KeyValuePair<string, object> pair_ in records_)
                    TAB_SETCOL(pGlHeader, pair_.Key, pair_.Value);

            }

            void MY_CHECK_TABLES(DataTable pDataHeader, DataTable pDataLines, DataTable pGlHeader, DataTable pGlLines)
            {
                if (pDataHeader == null || TAB_GETLASTROW(pDataHeader) == null)
                    EXCEPTIONFORUSER(string.Format("T_MSG_DATA_NO (T_TABLE '{0}')", TABLE_CLFICHE.TABLE));

                MY_JOURNAL_WRITE(pDataHeader);

                if (pDataLines == null)
                    EXCEPTIONFORUSER(string.Format("T_MSG_DATA_NO (T_TABLE '{0}' T_OR '{1}')", TABLE_CLFLINE.TABLE, TABLE_INVOICE.TABLE));

                foreach (DataRow row_ in pDataLines.Rows)
                    MY_CHECK_IS_WELL_KNOWN_TRANS(row_);

                if (pGlHeader == null || TAB_GETLASTROW(pGlHeader) == null)
                    EXCEPTIONFORUSER(string.Format("T_MSG_DATA_NO (T_TABLE '{0}')", TABLE_EMFICHE.TABLE));

                if (pGlLines == null)
                    EXCEPTIONFORUSER(string.Format("T_MSG_DATA_NO (T_TABLE '{0}')", TABLE_EMFLINE.TABLE));


            }







        }

        #endregion
        #region COSTDIST

        class GLDO_COSTDIST
        {


            public GLDO_COSTDIST(_PLUGIN pPLUGIN)
            {
                PLUGIN = pPLUGIN;
            }
            _PLUGIN PLUGIN;




            class GL_RECORD : Dictionary<string, object>
            {

            }
            class GL_TRANS_DESC
            {
                public GL_TRANS_DESC(DataRow pRecord, DataRow pParent, GL_TRANS_TYPE pType)
                {
                    record = pRecord;
                    parent = pParent;
                    type = pType;
                }
                public GL_TRANS_TYPE type;
                public DataRow record;
                public DataRow parent;
            }
            enum GL_TRANS_TYPE
            {
                service = 1,
                material = 2
            }
            enum BOOL_VAL
            {
                no = 0,
                yes = 1
            }
            enum GLREL_TRCODE
            {
                undef = 0,
                material = 1,
                personalCarad = 2,
                service = 3,
                VAT = 4,
                cash = 5,
                bank = 6,
                discount = 7,
                surcharge = 8,
                promo = 9

            }
            class GLREL_RELCODE1
            {
                public const string EMPTY = "";
            }
            public void MY_GL(DataSet DATASETS, DataSet DATASETD)
            {
                DataTable dataHeader_;
                DataTable dataLines_;
                DataTable dataValues_;

                DataTable glHeader_;
                DataTable glLines_;

                dataHeader_ = MY_GETTAB(DATASETS, TABLE_COSTDISTFC.TABLE);
                dataLines_ = MY_GETTAB(DATASETS, TABLE_COSTDISTLN.TABLE);
                dataValues_ = MY_GETTAB(DATASETS, TABLE_COSTDISTPEG.TABLE);

                glHeader_ = MY_GETTAB(DATASETD, TABLE_EMFICHE.TABLE);
                glLines_ = MY_GETTAB(DATASETD, TABLE_EMFLINE.TABLE);

                MY_CHECK_TABLES(dataHeader_, dataLines_, dataValues_, glHeader_, glLines_);
                MY_SYNC_HEADERS(dataHeader_, glHeader_);
                MY_PARSE_DATA(glHeader_, glLines_, MY_FORMAT_DATA(dataHeader_, dataLines_, dataValues_));

            }
            void MY_JOURNAL_WRITE(string[] pTables, object[] pArr)
            {
                if (!_useJournal)
                    return;
                PLUGIN.JOURNAL(JOURNAL_NS,
                string.Format(PLUGIN.RESOLVESTR("$search\t{0}/{1}"),
                JOINLIST(pTables),
                JOINLIST(FORMAT(pArr))));
            }
            void MY_JOURNAL_WRITE(DataTable pDocHeader)
            {
                if (!_useJournal)
                    return;
                DataRow headerRow_ = TAB_GETLASTROW(pDocHeader);
                PLUGIN.JOURNAL(JOURNAL_NS,
                    string.Format(PLUGIN.RESOLVESTR(Environment.NewLine + "$doc\t({0}/{1}/{2})\t[lang::T_TYPE],{3};[lang::T_NR],{4}"),
                    TABLE_COSTDISTFC.TABLE,
                    TABLE_COSTDISTFC.LOGICALREF,
                    TAB_GETROW(headerRow_, TABLE_COSTDISTFC.LOGICALREF),
                    MY_GLREL_DOCCODE(headerRow_),
                    TAB_GETROW(headerRow_, TABLE_COSTDISTFC.FICHENO)));
            }
            void MY_JOURNAL_WRITE(DataRow pTrans)
            {
                if (!_useJournal)
                    return;

                PLUGIN.JOURNAL(JOURNAL_NS,
                    string.Format(PLUGIN.RESOLVESTR("$line\t({0}/{1}/{2})"),
                    pTrans.Table.TableName,
                    TABLE_COSTDISTFC.LOGICALREF,
                    TAB_GETROW(pTrans, TABLE_COSTDISTFC.LOGICALREF)));
            }

            void MY_PARSE_DATA(DataTable pGlHeader, DataTable pGlLines, GL_RECORD[] pRecords)
            {
                foreach (GL_RECORD rec_ in pRecords)
                {
                    TAB_ADDROW(pGlLines);
                    foreach (KeyValuePair<string, object> pair_ in rec_)
                        TAB_SETROW(pGlLines, pair_.Key, pair_.Value);
                }

            }

            GL_RECORD[] MY_FORMAT_DATA(DataTable pDataHeader, DataTable pDataLines, DataTable pDataValues)
            {
                List<GL_RECORD> list_ = new List<GL_RECORD>();
                //Lines as trans
                MY_FORMAT_DATA_LINES(list_, pDataHeader, pDataLines, pDataValues);
                return list_.ToArray();
            }

            //string MY_GET_ACC_SEARCH_SQL(string pDoc, string pTrans, string pCard)
            //{
            //    return MY_GLREL.MY_GET_ACC_SEARCH_SQL(pDoc, pTrans, pCard);


            //    string sql_ =
            //    "DECLARE" + "\n" +
            //    "@TRCODE smallint," + "\n" +
            //    "@DOCCODE nvarchar(25)," + "\n" +
            //    "@RELCODE1 nvarchar(25)," + "\n" +
            //    "@RELCODE2 nvarchar(25)," + "\n" +
            //    "@RELCODE3 nvarchar(25)," + "\n" +
            //    "@RELCODE4 nvarchar(25)," + "\n" +
            //    "@RELCODE5 nvarchar(25)," + "\n" +
            //    "@RELCODE6 nvarchar(25)," + "\n" +
            //    "@RELCODE7 nvarchar(25)" + "\n" +
            //    "SELECT" + "\n" +
            //    "@TRCODE = @P1," + "\n" +
            //    "@DOCCODE = @P2," + "\n" +
            //    "@RELCODE1 = @P3," + "\n" +
            //    "@RELCODE2 = [dbo].[f_GLREL_$FIRM$_$PERIOD$_DOC_{0}] (@P4,1,0)," + "\n" +
            //    "@RELCODE3 = [dbo].[f_GLREL_$FIRM$_$PERIOD$_DOC_{0}] (@P5,2,0)," + "\n" +
            //    "@RELCODE4 = [dbo].[f_GLREL_$FIRM$_$PERIOD$_TRANS_{1}] (@P6,1,0)," + "\n" +
            //    "@RELCODE5 = [dbo].[f_GLREL_$FIRM$_$PERIOD$_TRANS_{1}] (@P7,2,0)," + "\n" +
            //    "@RELCODE6 = [dbo].[f_GLREL_$FIRM$_CARD_{2}] (@P8,1,0)," + "\n" +
            //    "@RELCODE7 = [dbo].[f_GLREL_$FIRM$_CARD_{2}] (@P9,2,0)" + "\n" +
            //    "EXEC [dbo].[p_GLREL_$FIRM$_GET_ACC_FROM_REL] " + "\n" +
            //    "@TRCODE," + "\n" +
            //    "@DOCCODE," + "\n" +
            //    "@RELCODE1," + "\n" +
            //    "@RELCODE2," + "\n" +
            //    "@RELCODE3," + "\n" +
            //    "@RELCODE4," + "\n" +
            //    "@RELCODE5," + "\n" +
            //    "@RELCODE6," + "\n" +
            //    "@RELCODE7" + "\n";

            //    return string.Format(sql_, pDoc, pTrans, pCard);
            //}


            object MY_GET_TRANS_ACC(DataRow pDocRow, DataRow pTransRow, GL_TRANS_TYPE pTransType, GLREL_TRCODE pGLRelType, string pGLRelFilterCode)
            {
                string tranLRef_ = string.Empty;
                string cardTable_ = string.Empty;
                string cardColumn_ = string.Empty;

                switch (pTransType)
                {
                    case GL_TRANS_TYPE.service:
                        tranLRef_ = TABLE_COSTDISTLN.LOGICALREF;
                        cardColumn_ = TABLE_COSTDISTLN.SRVREF;
                        cardTable_ = TABLE_SRVCARD.TABLE;
                        break;
                    case GL_TRANS_TYPE.material:
                        tranLRef_ = TABLE_COSTDISTPEG.LOGICALREF;
                        cardColumn_ = TABLE_COSTDISTPEG.ITEMREF;
                        cardTable_ = TABLE_ITEMS.TABLE;
                        break;
                    default:
                        throw MY_EXCEPTION_INVALID_VAR(typeof(GL_TRANS_TYPE).Name, pTransType);
                }


                string[] tablesArr_ = new string[] { pDocRow.Table.TableName, pTransRow.Table.TableName, cardTable_ };

                string sql_ = MY_GLREL.MY_GET_ACC_SEARCH_SQL(tablesArr_[0], tablesArr_[1], tablesArr_[2]);

                object docLRef_ = TAB_GETROW(pDocRow, TABLE_COSTDISTFC.LOGICALREF);
                object transLRef_ = TAB_GETROW(pTransRow, tranLRef_);
                object cardLRef_ = TAB_GETROW(pTransRow, cardColumn_);

                //    object[] var_ = new object[]{
                //    (short)pGLRelType,
                //    MY_GLREL_DOCCODE(pDocRow),
                //    pGLRelFilterCode,
                //    docLRef_,
                //    docLRef_,
                //    transLRef_,
                //    transLRef_,
                //    cardLRef_,
                //    cardLRef_
                //};

                //    MY_JOURNAL_WRITE(tablesArr_, var_);

                //    object acc_ = PLUGIN.SQLSCALAR(sql_, var_);
                //    MY_CHECK_ACC(acc_);
                //    return acc_;


                return MY_GLREL.MY_GET_ACC(PLUGIN, sql_,

                    (short)pGLRelType,
             MY_GLREL_DOCCODE(pDocRow),
             pGLRelFilterCode,
             docLRef_,
             docLRef_,
             transLRef_,
             transLRef_,
             cardLRef_,
             cardLRef_
                 );




            }

            void MY_CHECK_ACC(object pLRef)
            {
                if (CASTASINT(ISNULL(pLRef, 0)) == 0)
                    throw MY_EXCEPTION_INVALID_LREF(TABLE_EMUHACC.TABLE, TABLE_EMUHACC.LOGICALREF, pLRef);
            }

            bool MY_GET_TRANS_SRV_SIDE(DataRow pTransRow, GL_TRANS_TYPE pTransType)
            {
                return false;
            }
            string MY_GET_TRANS_COL(DataRow pTransRow, GL_TRANS_TYPE pTransType)
            {
                bool res_ = MY_GET_TRANS_SRV_SIDE(pTransRow, pTransType);
                switch (pTransType)
                {
                    case GL_TRANS_TYPE.service:
                        // res_ = res_;
                        break;
                    case GL_TRANS_TYPE.material:
                        res_ = !res_;
                        break;
                    default:
                        throw MY_EXCEPTION_INVALID_VAR(typeof(GL_TRANS_TYPE).Name, pTransType);

                }


                return MY_ACC_SIDE_TO_COL(res_);
            }

            string MY_ACC_SIDE_TO_COL(bool pAccSide)
            {
                return pAccSide ? TABLE_EMFLINE.DEBIT : TABLE_EMFLINE.CREDIT;
            }
            string MY_GLREL_DOCCODE(DataRow pDocRow)
            {
                return "COSTDIST";
            }


            void MY_FORMAT_DATA_LINES(List<GL_RECORD> pList, DataTable pDataHeader, DataTable pDataLines, DataTable pDataValues)
            {
                DataRow headerRecord_ = TAB_GETLASTROW(pDataHeader);
                //

                //
                var toGlLines = MY_LINES_TO_GL_LINES(pDataHeader, pDataLines, pDataValues);
                //
                foreach (GL_TRANS_DESC desc_ in toGlLines)
                // foreach (GL_TRANS_DESC desc_ in MY_LINES_TO_GL_LINES(pDataHeader, pDataLines, pDataValues))
                {
                    GL_RECORD records_ = new GL_RECORD();
                    DataRow transRecord_ = desc_.record;
                    //
                    MY_JOURNAL_WRITE(transRecord_);
                    //
                    double glValue_ = 0;

                    GLREL_TRCODE glRelTrcode_ = GLREL_TRCODE.undef;
                    string glRelFilter_ = GLREL_RELCODE1.EMPTY;

                    switch (desc_.type)
                    {

                        case GL_TRANS_TYPE.service:
                            glRelTrcode_ = GLREL_TRCODE.service;
                            glValue_ = MY_GET_LINE_TOTAL(transRecord_);
                            glRelFilter_ = GLREL_RELCODE1.EMPTY;
                            break;
                        case GL_TRANS_TYPE.material:
                            glRelTrcode_ = GLREL_TRCODE.material;
                            glValue_ = MY_GET_LINE_TOTAL(transRecord_);
                            glRelFilter_ = GLREL_RELCODE1.EMPTY;
                            break;


                    }

                    if (!ISNUMZERO(glValue_))
                    {
                        records_.Add(TABLE_EMFLINE.ACCOUNTREF, MY_GET_TRANS_ACC(headerRecord_, transRecord_, desc_.type, glRelTrcode_, glRelFilter_));
                        records_.Add(MY_GET_TRANS_COL(transRecord_, desc_.type), glValue_);
                        records_.Add(TABLE_EMFLINE.REPORTRATE, MY_GET_LINE_REPRATE(transRecord_));

                        //
                        //records_.Add(TABLE_EMFLINE.SPECODE, TAB_GETROW(headerRecord_, TABLE_COSTDISTFC.SPECODE));
                        //  records_.Add(TABLE_EMFLINE.INVOICENO, TAB_GETROW(headerRecord_, TABLE_COSTDISTFC.FICHENO));
                        //  records_.Add(TABLE_EMFLINE.CLDEF, TAB_GETROW(transRecord_, MY_GET_TRANS_CARD_INFO(transRecord_, desc_.type, false)));
                        //records_.Add(TABLE_EMFLINE.LINEEXP, JOINLIST(FORMAT(new object[] { 
                        //MY_GLREL_DOCCODE(headerRecord_), 
                        //TAB_GETROW(transRecord_, MY_GET_TRANS_CARD_INFO(transRecord_, desc_.type, true)), 
                        //(short)glRelTrcode_, 

                        //glRelFilter_ })));
                        //
                        pList.Add(records_);
                    }
                    //


                }
            }
            string MY_GET_TRANS_CARD_INFO(DataRow pTransRow, GL_TRANS_TYPE pTransType, bool pCode)
            {
                switch (pTransType)
                {

                    case GL_TRANS_TYPE.service:
                        return (pCode ? TABLE_COSTDISTLN.E_SRVCARD__CODE : TABLE_COSTDISTLN.E_SRVCARD__DEFINITION_);
                    case GL_TRANS_TYPE.material:
                        return (pCode ? TABLE_COSTDISTPEG.E_ITEMS__CODE : TABLE_COSTDISTPEG.E_ITEMS__DEFINITION_);

                    default:
                        throw MY_EXCEPTION_INVALID_VAR(typeof(GL_TRANS_TYPE).Name, pTransType);
                }
            }


            double MY_GET_LINE_TOTAL(DataRow pTransRow)
            {
                double value_ = 0;
                switch (pTransRow.Table.TableName)
                {
                    case TABLE_COSTDISTLN.TABLE:
                        value_ = CASTASDOUBLE(TAB_GETROW(pTransRow, TABLE_COSTDISTLN.DISTTOTAL));
                        break;
                    case TABLE_COSTDISTPEG.TABLE:
                        value_ = CASTASDOUBLE(TAB_GETROW(pTransRow, TABLE_COSTDISTPEG.ADDEXPENSE));
                        break;
                    default:
                        throw MY_EXCEPTION_INVALID_TAB(pTransRow.Table.TableName);
                }
                return value_;
            }
            double MY_GET_LINE_REPRATE(DataRow pTransRow)
            {
                double value_ = 0;
                switch (pTransRow.Table.TableName)
                {
                    case TABLE_COSTDISTLN.TABLE:
                        value_ = CASTASDOUBLE(TAB_GETROW(pTransRow, TABLE_COSTDISTLN.E_DUMMY_REPORTRATE));
                        break;
                    case TABLE_COSTDISTPEG.TABLE:

                        value_ = DIV(TAB_GETROW(pTransRow, TABLE_COSTDISTPEG.ADDEXPENSE), TAB_GETROW(pTransRow, TABLE_COSTDISTPEG.ADDRPEXPENSE));
                        break;
                    default:
                        throw MY_EXCEPTION_INVALID_TAB(pTransRow.Table.TableName);
                }
                return value_;
            }

            GL_TRANS_DESC[] MY_LINES_TO_GL_LINES(DataTable pDataHeader, DataTable pDataLines, DataTable pDataValues)
            {
                List<GL_TRANS_DESC> list_ = new List<GL_TRANS_DESC>();
                foreach (DataRow transRecord_ in pDataLines.Rows)
                {
                    switch (transRecord_.Table.TableName)
                    {
                        case TABLE_COSTDISTLN.TABLE:
                            DataRow[] transValueArr_ = TAB_SEARCHALL(pDataValues, TABLE_COSTDISTPEG.COSTDISTLNREF, TAB_GETROW(transRecord_, TABLE_COSTDISTLN.LOGICALREF), TABLE_COSTDISTPEG.ISDISTRIBUTED, (short)BOOL_VAL.yes);
                            if (transValueArr_.Length > 0)
                            {
                                list_.Add(new GL_TRANS_DESC(transRecord_, null, GL_TRANS_TYPE.service));
                                foreach (DataRow transValue_ in transValueArr_)
                                    list_.Add(new GL_TRANS_DESC(transValue_, null, GL_TRANS_TYPE.material));
                            }
                            break;
                        default:
                            throw MY_EXCEPTION_INVALID_TAB(transRecord_.Table.TableName);

                    }
                }

                return list_.ToArray();
            }

            Exception MY_EXCEPTION_INVALID_COL_VALUE(string pTab, string pCol, object pVal)
            {
                return new Exception(string.Format("T_MSG_ERROR_INVALID_VAR, {0}/{1}/{2}", pTab, pCol, pVal));
            }
            Exception MY_EXCEPTION_INVALID_TAB(string pTab)
            {
                return new Exception(string.Format("T_MSG_ERROR_INVALID_VAR, T_TABLE '{0}'", pTab));
            }
            Exception MY_EXCEPTION_INVALID_VAR(string pDesc, object pVal)
            {
                return new Exception(string.Format("T_MSG_ERROR_INVALID_VAR, {0}/{1}", pDesc, pVal));
            }
            Exception MY_EXCEPTION_INVALID_LREF(string pTab, string pCol, object pVal)
            {
                return new Exception(string.Format("T_MSG_ERROR_INVALID_LREF_TYPE, {0}/{1}/{2}", pTab, pCol, pVal));
            }
            void MY_CHECK_IS_WELL_KNOWN_TRANS(DataRow pLine)
            {
                switch (pLine.Table.TableName)
                {

                    case TABLE_COSTDISTLN.TABLE:
                        return;
                    case TABLE_COSTDISTPEG.TABLE:
                        {
                            short sign_ = CASTASSHORT(TAB_GETROW(pLine, TABLE_COSTDISTPEG.ISDISTRIBUTED));
                            switch (sign_)
                            {
                                case (short)BOOL_VAL.no:
                                case (short)BOOL_VAL.yes:
                                    return;
                                default:
                                    throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_COSTDISTPEG.TABLE, TABLE_COSTDISTPEG.ISDISTRIBUTED, sign_);
                            }
                        }
                    default:
                        throw MY_EXCEPTION_INVALID_TAB(pLine.Table.TableName);

                }
            }


            bool MY_TRANS_IS_ACTIVE(DataRow pLine)
            {
                if (pLine.Table.TableName == TABLE_COSTDISTPEG.TABLE)
                {
                    short sign_ = CASTASSHORT(TAB_GETROW(pLine, TABLE_COSTDISTPEG.ISDISTRIBUTED));
                    if (sign_ == (short)BOOL_VAL.yes)
                        return true;
                    else
                        if (sign_ == (short)BOOL_VAL.no)
                            return false;

                    throw MY_EXCEPTION_INVALID_COL_VALUE(TABLE_COSTDISTPEG.TABLE, TABLE_COSTDISTPEG.ISDISTRIBUTED, sign_);
                }
                throw MY_EXCEPTION_INVALID_TAB(pLine.Table.TableName);

            }
            void MY_SYNC_HEADERS(DataTable pDataHeader, DataTable pGlHeader)
            {
                GL_RECORD records_ = new GL_RECORD();

                records_.Add(TABLE_EMFICHE.DATE_, TAB_GETROW(pDataHeader, TABLE_COSTDISTFC.DATE_));
                //records_.Add(TABLE_EMFICHE.SPECODE, TAB_GETROW(pDataHeader, TABLE_COSTDISTFC.SPECODE));
                //records_.Add(TABLE_EMFICHE.CYPHCODE, TAB_GETROW(pDataHeader, TABLE_COSTDISTFC.CYPHCODE));
                foreach (KeyValuePair<string, object> pair_ in records_)
                    TAB_SETCOL(pGlHeader, pair_.Key, pair_.Value);

            }

            void MY_CHECK_TABLES(DataTable pDataHeader, DataTable pDataLines, DataTable pDataValues, DataTable pGlHeader, DataTable pGlLines)
            {
                if (pDataHeader == null || TAB_GETLASTROW(pDataHeader) == null)
                    EXCEPTIONFORUSER(string.Format("T_MSG_DATA_NO (T_TABLE '{0}')", TABLE_COSTDISTFC.TABLE));

                MY_JOURNAL_WRITE(pDataHeader);

                if (pDataLines == null)
                    EXCEPTIONFORUSER(string.Format("T_MSG_DATA_NO (T_TABLE '{0}')", TABLE_COSTDISTLN.TABLE));

                foreach (DataRow row_ in pDataLines.Rows)
                    MY_CHECK_IS_WELL_KNOWN_TRANS(row_);

                if (pDataValues == null)
                    EXCEPTIONFORUSER(string.Format("T_MSG_DATA_NO (T_TABLE '{0}')", TABLE_COSTDISTPEG.TABLE));

                foreach (DataRow row_ in pDataValues.Rows)
                    MY_CHECK_IS_WELL_KNOWN_TRANS(row_);

                if (pGlHeader == null || TAB_GETLASTROW(pGlHeader) == null)
                    EXCEPTIONFORUSER(string.Format("T_MSG_DATA_NO (T_TABLE {0})", TABLE_EMFICHE.TABLE));

                if (pGlLines == null)
                    EXCEPTIONFORUSER(string.Format("T_MSG_DATA_NO (T_TABLE {0})", TABLE_EMFLINE.TABLE));


            }





        }

        #endregion



        #endregion



        #region TOOLS


        class MY_TOOLS
        {

            public static string CRETAGLFICHENO(DataTable pDocHeader)
            {

                var glDocCode = "";
                var orgDocCode = CASTASSTRING(TAB_GETROW(pDocHeader, "FICHENO"));

                if (ISEMPTY(orgDocCode))
                    return glDocCode;

                switch (pDocHeader.TableName)
                {
                    case "STFICHE":
                        {
                            var module = 100;
                            var trcode = CASTASSHORT(TAB_GETROW(pDocHeader, "TRCODE"));
                            glDocCode = FORMAT(module + trcode) + "-" + orgDocCode;
                        }
                        break;
                    case "INVOICE":
                        {
                            var grpcode = CASTASSHORT(TAB_GETROW(pDocHeader, "GRPCODE"));
                            var module = (grpcode == 1 ? 200 : 300);
                            var trcode = CASTASSHORT(TAB_GETROW(pDocHeader, "TRCODE"));
                            glDocCode = FORMAT(module + trcode) + "-" + orgDocCode;

                        }
                        break;
                    case "KSLINES":
                        {
                            var module = 400;
                            var trcode = CASTASSHORT(TAB_GETROW(pDocHeader, "TRCODE"));
                            glDocCode = FORMAT(module + trcode) + "-" + orgDocCode;
                        }
                        break;
                    case "BNFICHE":
                        {
                            var module = 500;
                            var trcode = CASTASSHORT(TAB_GETROW(pDocHeader, "TRCODE"));
                            glDocCode = FORMAT(module + trcode) + "-" + orgDocCode;
                        }
                        break;
                    case "CLFICHE":
                        {
                            var module = 600;
                            var trcode = CASTASSHORT(TAB_GETROW(pDocHeader, "TRCODE"));
                            glDocCode = FORMAT(module + trcode) + "-" + orgDocCode;
                        }
                        break;

                }

                return glDocCode;
            }

        }

        class MY_GLREL
        {



            static DateTime glRelLastUsed = new DateTime(1900, 1, 1);
            static DataTable glRel;





            static string GETRELSQL_CARD(string pRecName, short pIndx)
            {
                string sql = null;
                switch (pRecName)
                {
                    case "ITEMS":
                    case "CLCARD":
                    case "BANKACC":
                    case "KSCARD":
                    case "SRVCARD":
                    case "DECARDS":

                        if (pIndx == 1)
                        {
                            sql = string.Format(@"
    SELECT SPECODE9 FROM LG_$FIRM$_{0}
--$MS$--WITH(NOLOCK)
	WHERE LOGICALREF = @P8
", pRecName);

                        }
                        else
                        {
                            sql = string.Format(@"
    SELECT SPECODE10 FROM LG_$FIRM$_{0}
--$MS$--WITH(NOLOCK)
	WHERE LOGICALREF = @P9
", pRecName);

                        }

                        break;

                    default:
                        EXCEPTIONFORUSER("Card type undefined:" + pRecName);
                        break;
                }

                if (sql == null)
                    EXCEPTIONFORUSER("Cant generate sql for:" + pRecName);

                return sql;

            }

            static string GETRELSQL_DOC(string pRecName, short pIndx)
            {
                string sql = null;
                switch (pRecName)
                {


                    case "INVOICE":
                    case "STFICHE":
                    case "KSLINES":

                        if (pIndx == 1)
                        {
                            sql = string.Format(@"
    SELECT SPECODE FROM LG_$FIRM$_$PERIOD$_{0}
--$MS$--WITH(NOLOCK)
	WHERE LOGICALREF = @P4
", pRecName);

                        }
                        else
                        {
                            sql = string.Format(@"
    SELECT cast(DEPARTMENT AS VARCHAR) FROM LG_$FIRM$_$PERIOD$_{0}
--$MS$--WITH(NOLOCK)
	WHERE LOGICALREF = @P5
", pRecName);

                        }

                        break;
                    case "BNFICHE":

                        if (pIndx == 1)
                        {
                            sql = string.Format(@"
    SELECT SPECODE FROM LG_$FIRM$_$PERIOD$_{0}
--$MS$--WITH(NOLOCK)
	WHERE LOGICALREF = @P4
", pRecName);

                        }
                        else
                        {
                            sql = string.Format(@"
    SELECT cast(DEPARMENT AS VARCHAR) FROM LG_$FIRM$_$PERIOD$_{0}
--$MS$--WITH(NOLOCK)
	WHERE LOGICALREF = @P5
", pRecName);

                        }

                        break;
                    case "CLFICHE":
                        if (pIndx == 1)
                        {
                            sql = string.Format(@"
    SELECT SPECCODE FROM LG_$FIRM$_$PERIOD$_{0}
--$MS$--WITH(NOLOCK)
	WHERE LOGICALREF = @P4
", pRecName);

                        }
                        else
                        {
                            sql = string.Format(@"
    SELECT cast(DEPARTMENT AS VARCHAR) FROM LG_$FIRM$_$PERIOD$_{0}
--$MS$--WITH(NOLOCK)
	WHERE LOGICALREF = @P5
", pRecName);

                        }
                        break;
                    case "COSTDISTFC":
                        if (pIndx == 1)
                        {
                            sql = string.Format(@"
    SELECT SPECODE FROM LG_$FIRM$_$PERIOD$_{0}
--$MS$--WITH(NOLOCK)
	WHERE LOGICALREF = @P4
", pRecName);

                        }
                        else
                        {
                            sql = string.Format(@"
    SELECT '' FROM LG_$FIRM$_$PERIOD$_{0}
--$MS$--WITH(NOLOCK)
	WHERE LOGICALREF = @P5
", pRecName);

                        }
                        break;

                    default:
                        EXCEPTIONFORUSER("Doc type undefined:" + pRecName);
                        break;
                }

                if (sql == null)
                    EXCEPTIONFORUSER("Doc generate sql for:" + pRecName);

                return sql;

            }

            static string GETRELSQL_TRAN(string pRecName, short pIndx)
            {
                string sql = null;
                switch (pRecName)
                {


                    case "CLFLINE":
                    case "BNFLINE":
                    case "KSLINES":

                        if (pIndx == 1)
                        {
                            sql = string.Format(@"
    SELECT SPECODE FROM LG_$FIRM$_$PERIOD$_{0}
--$MS$--WITH(NOLOCK)
	WHERE LOGICALREF = @P6
", pRecName);

                        }
                        else
                        {
                            sql = string.Format(@"
    SELECT TRADINGGRP FROM LG_$FIRM$_$PERIOD$_{0}
--$MS$--WITH(NOLOCK)
	WHERE LOGICALREF = @P7
", pRecName);

                        }

                        break;
                    case "STLINE":

                        if (pIndx == 1)
                        {
                            sql = string.Format(@"
    SELECT SPECODE FROM LG_$FIRM$_$PERIOD$_{0}
--$MS$--WITH(NOLOCK)
	WHERE LOGICALREF = @P6
", pRecName);

                        }
                        else
                        {
                            sql = string.Format(@"
    SELECT cast(SOURCEINDEX AS VARCHAR) FROM LG_$FIRM$_$PERIOD$_{0}
--$MS$--WITH(NOLOCK)
	WHERE LOGICALREF = @P7
", pRecName);

                        }

                        break;
                    case "COSTDISTLN":

                        if (pIndx == 1)
                        {
                            sql = string.Format(@"
	SELECT  SPECODE 
	FROM LG_$FIRM$_$PERIOD$_STLINE
    --$MS$--WITH(NOLOCK)
	WHERE LOGICALREF = (
						SELECT SRVTRANSREF
						FROM LG_$FIRM$_$PERIOD$_{0}
                        --$MS$--WITH(NOLOCK)
						WHERE LOGICALREF = @P6
						)
", pRecName);

                        }
                        else
                        {
                            sql = string.Format(@"
	SELECT cast(SOURCEINDEX AS VARCHAR)
	FROM LG_$FIRM$_$PERIOD$_STLINE
    --$MS$--WITH(NOLOCK)
	WHERE LOGICALREF = (
						SELECT SRVTRANSREF
						FROM LG_$FIRM$_$PERIOD$_{0}
                        --$MS$--WITH(NOLOCK)
						WHERE LOGICALREF = @P7
						)
", pRecName);

                        }

                        break;
                    case "COSTDISTPEG":

                        if (pIndx == 1)
                        {
                            sql = string.Format(@"
	SELECT  SPECODE 
	FROM LG_$FIRM$_$PERIOD$_STLINE
    --$MS$--WITH(NOLOCK)
	WHERE LOGICALREF = (
						SELECT STTRANSREF
						FROM LG_$FIRM$_$PERIOD$_{0}
                        --$MS$--WITH(NOLOCK)
						WHERE LOGICALREF = @P6
						)
", pRecName);

                        }
                        else
                        {
                            sql = string.Format(@"
	SELECT cast(SOURCEINDEX AS VARCHAR)
	FROM LG_$FIRM$_$PERIOD$_STLINE
    --$MS$--WITH(NOLOCK)
	WHERE LOGICALREF = (
						SELECT STTRANSREF
						FROM LG_$FIRM$_$PERIOD$_{0}
                        --$MS$--WITH(NOLOCK)
						WHERE LOGICALREF = @P7
						)
", pRecName);

                        }

                        break;
                    case "INVOICE":

                        if (pIndx == 1)
                        {
                            sql = string.Format(@"

	
		SELECT  
		(CASE WHEN TRCODE IN (11,12) THEN SPECODE ELSE TRADINGGRP END)
	FROM LG_$FIRM$_$PERIOD$_{0}
                        --$MS$--WITH(NOLOCK)
	WHERE LOGICALREF = @P6

", pRecName);

                        }
                        else
                        {
                            sql = string.Format(@"
		SELECT  
		(CASE WHEN TRCODE IN (11,12) THEN TRADINGGRP ELSE cast(SOURCEINDEX AS VARCHAR) END)
	FROM LG_$FIRM$_$PERIOD$_{0}
                        --$MS$--WITH(NOLOCK)
	WHERE LOGICALREF = @P7
", pRecName);

                        }

                        break;
                    default:
                        EXCEPTIONFORUSER("Tran type undefined:" + pRecName);
                        break;
                }

                if (sql == null)
                    EXCEPTIONFORUSER("Tran generate sql for:" + pRecName);

                return sql;

            }

            public static void CLEAN_CACHE()
            {
                var diff = DateTime.Now - glRelLastUsed;
                if (diff.TotalSeconds > 5)
                    glRel = null;
            }


            public static object MY_GET_ACC(
                    _PLUGIN PLUGIN,

                    string SQL,

                    short pType,
                    string pDocCode,
                    string pFilter,
                    object pDocRef1,
                    object pDocRef2,
                    object pTranRef1,
                    object pTranRef2,
                    object pCardRef1,
                    object pCardRef2

                )
            {

                var args = new object[] { 
                         pType,
                      pDocCode,
                      pFilter,
                      pDocRef1,
                      pDocRef2,
                      pTranRef1,
                      pTranRef2,
                      pCardRef1,
                      pCardRef2
                };

                glRelLastUsed = DateTime.Now;

                var data = glRel;


                if (data == null)
                {

                    //DESC sort for empty to be last
                    glRel = data = PLUGIN.SQL(@"SELECT * FROM LG_$FIRM$_GLREL 
ORDER BY 
        TRCODE DESC,
		DOCCODE DESC,
		RELCODE1 DESC,
		RELCODE2 DESC,
		RELCODE3 DESC,
		RELCODE4 DESC,
		RELCODE5 DESC,
		RELCODE6 DESC,
		RELCODE7 DESC

");

                }




                var glAccSearchInfo = TAB_GETLASTROW(PLUGIN.SQL(SQL, args));
                TAB_FILLNULL(glAccSearchInfo);


                var TRCODE = CASTASSHORT(TAB_GETROW(glAccSearchInfo, "TRCODE"));
                var DOCCODE = CASTASSTRING(TAB_GETROW(glAccSearchInfo, "DOCCODE")).Trim().ToUpperInvariant();
                var DOCCODE_TOP = DOCCODE.Contains(".") ? DOCCODE.Split('.')[0] : DOCCODE;
                var RELCODE1 = CASTASSTRING(TAB_GETROW(glAccSearchInfo, "RELCODE1")).Trim().ToUpperInvariant();
                var RELCODE2 = CASTASSTRING(TAB_GETROW(glAccSearchInfo, "RELCODE2")).Trim().ToUpperInvariant();
                var RELCODE3 = CASTASSTRING(TAB_GETROW(glAccSearchInfo, "RELCODE3")).Trim().ToUpperInvariant();
                var RELCODE4 = CASTASSTRING(TAB_GETROW(glAccSearchInfo, "RELCODE4")).Trim().ToUpperInvariant();
                var RELCODE5 = CASTASSTRING(TAB_GETROW(glAccSearchInfo, "RELCODE5")).Trim().ToUpperInvariant();
                var RELCODE6 = CASTASSTRING(TAB_GETROW(glAccSearchInfo, "RELCODE6")).Trim().ToUpperInvariant();
                var RELCODE7 = CASTASSTRING(TAB_GETROW(glAccSearchInfo, "RELCODE7")).Trim().ToUpperInvariant();

                object glAccRef = null;


                foreach (DataRow row in data.Rows)
                {
                    var _tmp_TRCODE = CASTASSHORT(TAB_GETROW(row, "TRCODE"));
                    var _tmp_DOCCODE = CASTASSTRING(TAB_GETROW(row, "DOCCODE")).Trim().ToUpperInvariant();
                    var _tmp_RELCODE1 = CASTASSTRING(TAB_GETROW(row, "RELCODE1")).Trim().ToUpperInvariant();
                    var _tmp_RELCODE2 = CASTASSTRING(TAB_GETROW(row, "RELCODE2")).Trim().ToUpperInvariant();
                    var _tmp_RELCODE3 = CASTASSTRING(TAB_GETROW(row, "RELCODE3")).Trim().ToUpperInvariant();
                    var _tmp_RELCODE4 = CASTASSTRING(TAB_GETROW(row, "RELCODE4")).Trim().ToUpperInvariant();
                    var _tmp_RELCODE5 = CASTASSTRING(TAB_GETROW(row, "RELCODE5")).Trim().ToUpperInvariant();
                    var _tmp_RELCODE6 = CASTASSTRING(TAB_GETROW(row, "RELCODE6")).Trim().ToUpperInvariant();
                    var _tmp_RELCODE7 = CASTASSTRING(TAB_GETROW(row, "RELCODE7")).Trim().ToUpperInvariant();

                    if (
                        (_tmp_TRCODE == TRCODE) && //Allways shuld equal
                        (_tmp_DOCCODE == "" || _tmp_DOCCODE == DOCCODE || _tmp_DOCCODE == DOCCODE_TOP) && // CLIENT.3, CLIENT, ''
                        (_tmp_RELCODE1 == RELCODE1) && //PAIR and etc. //Allways shuld equal
                        (_tmp_RELCODE2 == "" || _tmp_RELCODE2 == RELCODE2) &&
                        (_tmp_RELCODE3 == "" || _tmp_RELCODE3 == RELCODE3) &&
                        (_tmp_RELCODE4 == "" || _tmp_RELCODE4 == RELCODE4) &&
                        (_tmp_RELCODE5 == "" || _tmp_RELCODE5 == RELCODE5) &&
                        (_tmp_RELCODE6 == "" || _tmp_RELCODE6 == RELCODE6) &&
                        (_tmp_RELCODE7 == "" || _tmp_RELCODE7 == RELCODE7)

                        )
                    {
                        glAccRef = TAB_GETROW(row, "ACCOUNTREF");
                    }

                }

                if (ISEMPTYLREF(glAccRef))
                    EXCEPTIONFORUSER("GL Account not found: Type [" + pType + "] Code [" + pDocCode + "]");


                return glAccRef;


            }



            public static string MY_GET_ACC_SEARCH_SQL(string pDoc, string pTrans, string pCard)
            {

                string sql_ = @"
                

SELECT 
               (cast(@P1 as smallint)) TRCODE ,
               (cast(@P2 as varchar)) DOCCODE ,
               (cast(@P3 as varchar)) RELCODE1 ,
               (
{0}
) RELCODE2 ,
               (
{1}
) RELCODE3 ,
               (
{2}
) RELCODE4 ,
               (
{3}
) RELCODE5 ,
               (
{4}
) RELCODE6 ,
               (
{5}
) RELCODE7 

";



                sql_ = string.Format(sql_,

                    GETRELSQL_DOC(pDoc, 1),
                    GETRELSQL_DOC(pDoc, 2),
                    GETRELSQL_TRAN(pTrans, 1),
                    GETRELSQL_TRAN(pTrans, 2),
                    GETRELSQL_CARD(pCard, 1),
                    GETRELSQL_CARD(pCard, 2)

                    );

                return sql_;
            }



        }



        static DataTable MY_GETTAB(DataSet pDs, string pTabName)
        {
            return pDs.Tables[pTabName];

        }
        #endregion



        #region TABLES


        class TABLE_CLCARD
        {
            public const string LOGICALREF = "LOGICALREF"; // int  
            public const string TABLE = "CLCARD";
            public const string CODE = "CODE";
            public const string DEFINITION_ = "DEFINITION_";
        }

        class TABLE_EMUHACC
        {
            public const string LOGICALREF = "LOGICALREF"; // int  
            public const string TABLE = "EMUHACC";
            public const string CODE = "CODE";
            public const string DEFINITION_ = "DEFINITION_";
        }
        class TABLE_BANKACC
        {

            public const string TABLE = "BANKACC";
            public const string LOGICALREF = "LOGICALREF"; // int 4
            public const string CODE = "CODE"; // varchar 17
            public const string DEFINITION_ = "DEFINITION_"; // varchar 51


        }
        class TABLE_KSCARD
        {

            public const string TABLE = "KSCARD";


            public const string LOGICALREF = "LOGICALREF"; // int 4
            public const string CODE = "CODE"; // varchar 17
            public const string NAME = "NAME"; // varchar 51

        }

        class TABLE_KSLINES
        {
            public const String TABLE = "KSLINES";

            public const string LOGICALREF = "LOGICALREF"; // int 4
            public const string CARDREF = "CARDREF"; // int 4
            public const string VCARDREF = "VCARDREF"; // int 4

            public const string DATE_ = "DATE_"; // datetime 8
            public const string HOUR_ = "HOUR_"; // smallint 2
            public const string MINUTE_ = "MINUTE_"; // smallint 2
            public const string TRCODE = "TRCODE"; // smallint 2
            public const string BRANCH = "BRANCH"; // smallint 2
            public const string DEPARTMENT = "DEPARTMENT"; // smallint 2
            public const string DESTBRANCH = "DESTBRANCH"; // smallint 2
            public const string DESTDEPARTMENT = "DESTDEPARTMENT"; // smallint 2
            public const string SPECODE = "SPECODE"; // varchar 11
            public const string SPECODE2 = "SPECODE2"; // varchar 11
            public const string SPECODE3 = "SPECODE3"; // varchar 11
            public const string CYPHCODE = "CYPHCODE"; // varchar 11
            public const string FICHENO = "FICHENO"; // varchar 9
            public const string CUSTTITLE = "CUSTTITLE"; // varchar 51
            public const string LINEEXP = "LINEEXP"; // varchar 51
            public const string AMOUNT = "AMOUNT"; // float 8
            public const string REPORTRATE = "REPORTRATE"; // float 8

            public const string TRRATE = "TRRATE"; // float 8
            public const string TRCURR = "TRCURR"; // smallint 2
            public const string SIGN = "SIGN"; // smallint 2

            public const string CANCELLED = "CANCELLED"; // smallint 2


            public const string TRADINGGRP = "TRADINGGRP"; // varchar 17

            public const string PROJECTREF = "PROJECTREF"; // int 4


            public const string DOCODE = "DOCODE"; // varchar 17
            public const string TRANNO = "TRANNO"; // varchar 9

            public const string CLIENTREF = TABLE_CLFLINE.CLIENTREF;
            public const string BANKREF = TABLE_BNFLINE.BANKREF;
            public const string BNACCREF = TABLE_BNFLINE.BNACCREF;
            //
            public const string ACCREF = "ACCREF";
            public const string CSACCREF = "CSACCREF";

            public static readonly string E_CLCARD__CODE = TAB_GETCOLFULLNAME(TABLE_CLCARD.TABLE, TABLE_CLCARD.CODE);
            public static readonly string E_CLCARD__DEFINITION_ = TAB_GETCOLFULLNAME(TABLE_CLCARD.TABLE, TABLE_CLCARD.DEFINITION_);

            public static readonly string E_BANKACC__CODE = TAB_GETCOLFULLNAME(TABLE_BANKACC.TABLE, TABLE_BANKACC.CODE);
            public static readonly string E_BANKACC__DEFINITION_ = TAB_GETCOLFULLNAME(TABLE_BANKACC.TABLE, TABLE_BANKACC.DEFINITION_);

            public static readonly string E_KSCARD__CODE = TAB_GETCOLFULLNAME(TABLE_KSCARD.TABLE, TABLE_KSCARD.CODE);
            public static readonly string E_KSCARD__NAME = TAB_GETCOLFULLNAME(TABLE_KSCARD.TABLE, TABLE_KSCARD.NAME);

            public static readonly string E_KSCARD__CODE2 = TAB_GETCOLFULLNAME(TABLE_KSCARD.TABLE, TABLE_KSCARD.CODE, 2);
            public static readonly string E_KSCARD__NAME2 = TAB_GETCOLFULLNAME(TABLE_KSCARD.TABLE, TABLE_KSCARD.NAME, 2);

        }
        class TABLE_CLFLINE
        {
            public const string TABLE = "CLFLINE";

            public const string LOGICALREF = "LOGICALREF"; // int 4
            public const string CLIENTREF = "CLIENTREF"; // int 4
            public const string DATE_ = "DATE_"; // datetime 8
            public const string DEPARTMENT = "DEPARTMENT"; // smallint 2
            public const string BRANCH = "BRANCH"; // smallint 2
            public const string TRCODE = "TRCODE"; // smallint 2
            public const string SPECODE = "SPECODE"; // varchar 11 // varchar 11
            public const string SPECODE2 = "SPECODE2"; // varchar 11
            public const string SPECODE3 = "SPECODE3"; // varchar 11
            public const string CYPHCODE = "CYPHCODE"; // varchar 11
            public const string TRANNO = "TRANNO"; // varchar 9
            public const string DOCODE = "DOCODE"; // varchar 17
            public const string LINEEXP = "LINEEXP"; // varchar 51
            public const string SIGN = "SIGN"; // smallint 2
            public const string AMOUNT = "AMOUNT"; // float 8
            public const string TRCURR = "TRCURR"; // smallint 2
            public const string TRRATE = "TRRATE"; // float 8
            public const string TRNET = "TRNET"; // float 8
            public const string REPORTRATE = "REPORTRATE"; // float 8
            public const string REPORTNET = "REPORTNET"; // float 8
            public const string CANCELLED = "CANCELLED"; // smallint 2
            public const string TRADINGGRP = "TRADINGGRP"; // varchar 17

            public static readonly string E_CLCARD__CODE = TAB_GETCOLFULLNAME(TABLE_CLCARD.TABLE, TABLE_CLCARD.CODE);
            public static readonly string E_CLCARD__DEFINITION_ = TAB_GETCOLFULLNAME(TABLE_CLCARD.TABLE, TABLE_CLCARD.DEFINITION_);


        }
        class TABLE_BNFICHE
        {

            public const string TABLE = "BNFICHE";

            public const string LOGICALREF = "LOGICALREF"; // int 4
            public const string DATE_ = "DATE_"; // datetime 8
            public const string FICHENO = "FICHENO"; // varchar 9
            public const string SPECODE = "SPECODE"; // varchar 11 // varchar 11
            public const string SPECODE2 = "SPECODE2"; // varchar 11
            public const string SPECODE3 = "SPECODE3"; // varchar 11
            public const string CYPHCODE = "CYPHCODE"; // varchar 11
            public const string BRANCH = "BRANCH"; // smallint 2
            public const string DEPARMENT = "DEPARMENT"; // smallint 2
            public const string TRCODE = "TRCODE"; // smallint 2
            public const string CANCELLED = "CANCELLED"; // smallint 2
            public const string GENEXP1 = "GENEXP1"; // varchar 51
            public const string GENEXP2 = "GENEXP2"; // varchar 51
            public const string GENEXP3 = "GENEXP3"; // varchar 51
            public const string GENEXP4 = "GENEXP4"; // varchar 51
            public const string PROJECTREF = "PROJECTREF"; // int 4

        }
        class TABLE_BNFLINE
        {

            public const String TABLE = "BNFLINE";

            public const string LOGICALREF = "LOGICALREF"; // int 4
            public const string BANKREF = "BANKREF"; // int 4
            public const string BNACCREF = "BNACCREF"; // int 4
            public const string CLIENTREF = "CLIENTREF"; // int 4
            public const string DATE_ = "DATE_"; // datetime 8
            public const string DEPARTMENT = "DEPARTMENT"; // smallint 2
            public const string BRANCH = "BRANCH"; // smallint 2
            public const string SIGN = "SIGN"; // smallint 2
            public const string TRCODE = "TRCODE"; // smallint 2
            public const string SPECODE = "SPECODE"; // varchar 11 // varchar 11
            public const string SPECODE2 = "SPECODE2"; // varchar 11
            public const string SPECODE3 = "SPECODE3"; // varchar 11
            public const string CYPHCODE = "CYPHCODE"; // varchar 11
            public const string DOCODE = "DOCODE"; // varchar 17
            public const string LINEEXP = "LINEEXP"; // varchar 51
            public const string TRCURR = "TRCURR"; // smallint 2
            public const string AMOUNT = "AMOUNT"; // float 8
            public const string TRRATE = "TRRATE"; // float 8
            public const string REPORTRATE = "REPORTRATE"; // float 8
            public const string TRADINGGRP = "TRADINGGRP"; // varchar 17
            public const string PROJECTREF = "PROJECTREF"; // int 4

            public static readonly string E_CLCARD__CODE = TAB_GETCOLFULLNAME(TABLE_CLCARD.TABLE, TABLE_CLCARD.CODE);
            public static readonly string E_CLCARD__DEFINITION_ = TAB_GETCOLFULLNAME(TABLE_CLCARD.TABLE, TABLE_CLCARD.DEFINITION_);

            public static readonly string E_BANKACC__CODE = TAB_GETCOLFULLNAME(TABLE_BANKACC.TABLE, TABLE_BANKACC.CODE);
            public static readonly string E_BANKACC__DEFINITION_ = TAB_GETCOLFULLNAME(TABLE_BANKACC.TABLE, TABLE_BANKACC.DEFINITION_);

        }
        class TABLE_EMFICHE
        {

            public const string TABLE = "EMFICHE";
            public const string DATE_ = "DATE_";
            public const string CANCELLED = "CANCELLED";
            public const string BRANCH = "BRANCH"; // smallint 2
            public const string DEPARTMENT = "DEPARTMENT"; // smallint 2
            public const string GENEXP1 = "GENEXP1"; // varchar 51
            public const string GENEXP2 = "GENEXP2"; // varchar 51
            public const string GENEXP3 = "GENEXP3"; // varchar 51
            public const string GENEXP4 = "GENEXP4"; // varchar 51
            public const string SPECODE = "SPECODE"; // varchar 11
            public const string SPECODE2 = "SPECODE2"; // varchar 11
            public const string SPECODE3 = "SPECODE3"; // varchar 11

            public const string CYPHCODE = "CYPHCODE"; // varchar 11
            public const string FICHENO = "FICHENO"; // varchar 11

        }
        class TABLE_EMFLINE
        {
            public const string TABLE = "EMFLINE";
            public const string ACCOUNTREF = "ACCOUNTREF"; // int 4
            public const string DEBIT = "DEBIT"; // float 8
            public const string CREDIT = "CREDIT"; // float 8
            public const string LINEEXP = "LINEEXP"; // varchar 51
            public const string REPORTRATE = "REPORTRATE"; // float 8
            public const string TRCURR = "TRCURR"; // smallint 2
            public const string TRRATE = "TRRATE"; // float 8
            public const string AMNT = "AMNT"; // float 8
            //public const string INVOICENO = "INVOICENO"; // varchar 17
            //   public const string CLDEF = "CLDEF"; // varchar 51
            public const string SPECODE = "SPECODE"; // varchar 11
        }
        class TABLE_GLREL
        {
            public const string TABLE = "GLREL";
            public const string LOGICALREF = "LOGICALREF"; // int 4
            public const string ACCOUNTREF = "ACCOUNTREF"; // int 4
            public const string CYPHCODE = "CYPHCODE"; // nvarchar 50
            public const string TRCODE = "TRCODE"; // smallint 2
            public const string DOCCODE = "DOCCODE"; // nvarchar 50
            public const string RELCODE1 = "RELCODE1"; // nvarchar 50
            public const string RELCODE2 = "RELCODE2"; // nvarchar 50
            public const string RELCODE3 = "RELCODE3"; // nvarchar 50
            public const string RELCODE4 = "RELCODE4"; // nvarchar 50
            public const string RELCODE5 = "RELCODE5"; // nvarchar 50
            public const string RELCODE6 = "RELCODE6"; // nvarchar 50
            public const string RELCODE7 = "RELCODE7"; // nvarchar 50
        }



        class TABLE_INVOICE
        {
            public const string TRCODE = "TRCODE"; // smallint  
            public const string LOGICALREF = "LOGICALREF"; // int  
            public const string FICHENO = "FICHENO"; // varchar  
            public const string TABLE = "INVOICE";
            public const string DATE_ = "DATE_";
            public const string CANCELLED = "CANCELLED";
            public const string BRANCH = "BRANCH"; // smallint   
            public const string DEPARTMENT = "DEPARTMENT"; // smallint  
            public const string GENEXP1 = "GENEXP1"; // varchar  
            public const string GENEXP2 = "GENEXP2"; // varchar  
            public const string GENEXP3 = "GENEXP3"; // varchar  
            public const string GENEXP4 = "GENEXP4"; // varchar  
            public const string NETTOTAL = "NETTOTAL"; // float  
            public const string REPORTRATE = "REPORTRATE"; // float  
            public const string TRCURR = "TRCURR"; // smallint  
            public const string TRRATE = "TRRATE"; // float  
            public const string ENTEGSET = "ENTEGSET"; // smallint  
            public const string CLIENTREF = "CLIENTREF"; // int  
            public const string SPECODE = "SPECODE"; // varchar  
            public const string CYPHCODE = "CYPHCODE"; // varchar  
            public const string VAT = "VAT"; // float 8
            public const string TOTALVAT = "TOTALVAT"; // float 8
            public static readonly string E_CLCARD__CODE = TAB_GETCOLFULLNAME(TABLE_CLCARD.TABLE, TABLE_CLCARD.CODE);
            public static readonly string E_CLCARD__DEFINITION_ = TAB_GETCOLFULLNAME(TABLE_CLCARD.TABLE, TABLE_CLCARD.DEFINITION_);
        }
        class TABLE_CLFICHE
        {
            public const String TABLE = "CLFICHE";

            public const string LOGICALREF = "LOGICALREF"; // int 
            public const string FICHENO = "FICHENO"; // varchar 
            public const string DATE_ = "DATE_"; // datetime 
            public const string DOCODE = "DOCODE"; // varchar 
            public const string TRCODE = "TRCODE"; // smallint 2
            public const string SPECCODE = "SPECCODE"; // varchar 
            public const string SPECODE2 = "SPECODE2"; // varchar 
            public const string SPECODE3 = "SPECODE3"; // varchar 
            public const string CYPHCODE = "CYPHCODE"; // varchar 
            public const string BRANCH = "BRANCH"; // smallint 
            public const string DEPARTMENT = "DEPARTMENT"; // smallint 
            public const string GENEXP1 = "GENEXP1"; // varchar 
            public const string GENEXP2 = "GENEXP2"; // varchar 
            public const string GENEXP3 = "GENEXP3"; // varchar 
            public const string GENEXP4 = "GENEXP4"; // varchar 
            public const string CANCELLED = "CANCELLED"; // smallint 
            public const string TRADINGGRP = "TRADINGGRP"; // varchar 


        }

        class TABLE_ITEMS
        {
            public const string LOGICALREF = "LOGICALREF"; // int 4
            public const string TABLE = "ITEMS";
            public const string CODE = "CODE";
            public const string NAME = "NAME";
        }
        class TABLE_SRVCARD
        {
            public const string LOGICALREF = "LOGICALREF"; // int 4
            public const string TABLE = "SRVCARD";
            public const string CODE = "CODE";
            public const string DEFINITION_ = "DEFINITION_";
        }
        class TABLE_STLINE
        {
            public const string TABLE = "STLINE";
            public const string AMOUNT = "AMOUNT"; // float 8
            public const string TOTAL = "TOTAL"; // float 8
            public const string TRCURR = "TRCURR"; // smallint 2
            public const string TRRATE = "TRRATE"; // float 8
            public const string REPORTRATE = "REPORTRATE"; // float 8
            public const string DISTDISC = "DISTDISC"; // float 8
            public const string DISTEXP = "DISTEXP"; // float 8
            public const string DISTPROM = "DISTPROM"; // float 8
            public const string LINEEXP = "LINEEXP"; // varchar 31
            public const string UINFO1 = "UINFO1"; // float 8
            public const string UINFO2 = "UINFO2"; // float 8
            public const string VATAMNT = "VATAMNT"; // float 8
            public const string LINETYPE = "LINETYPE"; // smallint 2
            public const string GLOBTRANS = "GLOBTRANS"; // smallint 2
            public const string LOGICALREF = "LOGICALREF"; // int 4
            public const string STOCKREF = "STOCKREF"; // int 4
            public const string VATINC = "VATINC"; // smallint 2
            public const string TRCODE = "TRCODE"; // smallint 2
            public const string IOCODE = "IOCODE"; // smallint 2
            public const string SPECODE = "SPECODE"; // varchar 11

            public static readonly string E_ITEM__CODE = TAB_GETCOLFULLNAME(TABLE_ITEMS.TABLE, TABLE_ITEMS.CODE);
            public static readonly string E_ITEM__NAME = TAB_GETCOLFULLNAME(TABLE_ITEMS.TABLE, TABLE_ITEMS.NAME);

        }
        class TABLE_COSTDISTFC
        {

            public const string TABLE = "COSTDISTFC";
            public const string LOGICALREF = "LOGICALREF"; // int 4
            public const string FICHENO = "FICHENO"; // varchar 17
            public const string DATE_ = "DATE_"; // datetime 8
            public const string DOCODE = "DOCODE"; // varchar 17
            public const string SPECODE = "SPECODE"; // varchar 11
            public const string CYPHCODE = "CYPHCODE"; // varchar 11

        }

        class TABLE_DUMMY
        {
            public const string LOGICALREF = "LOGICALREF"; // int 4
            public const string TABLE = "DUMMY";
            public const string TOT = "TOT";
            public const string REPRATE = "REPRATE";
        }



        class TABLE_COSTDISTLN
        {

            public const string TABLE = "COSTDISTLN";


            public const string LOGICALREF = "LOGICALREF"; // int 4
            public const string SRVREF = "SRVREF"; // int 4
            public const string DATE_ = "DATE_"; // datetime 8

            public const string COSTDISTFCREF = "COSTDISTFCREF"; // int 4
            public const string SRVFICHEREF = "SRVFICHEREF"; // int 4
            public const string SRVTRANSREF = "SRVTRANSREF"; // int 4


            public const string DISTTOTAL = "DISTTOTAL"; // float 8


            public static readonly string E_DUMMY_REPORTRATE = TAB_GETCOLFULLNAME(TABLE_DUMMY.TABLE, TABLE_DUMMY.REPRATE);

            public static readonly string E_SRVCARD__CODE = TAB_GETCOLFULLNAME(TABLE_SRVCARD.TABLE, TABLE_SRVCARD.CODE);
            public static readonly string E_SRVCARD__DEFINITION_ = TAB_GETCOLFULLNAME(TABLE_SRVCARD.TABLE, TABLE_SRVCARD.DEFINITION_);


        }
        class TABLE_COSTDISTPEG
        {

            public const string TABLE = "COSTDISTPEG";


            public const string LOGICALREF = "LOGICALREF"; // int 4
            public const string COSTDISTFCREF = "COSTDISTFCREF"; // int 4
            public const string COSTDISTLNREF = "COSTDISTLNREF"; // int 4
            public const string SRVFICHEREF = "SRVFICHEREF"; // int 4
            public const string SRVTRANSREF = "SRVTRANSREF"; // int 4
            public const string INVOICEREF = "INVOICEREF"; // int 4
            public const string STFICHEREF = "STFICHEREF"; // int 4
            public const string STTRANSREF = "STTRANSREF"; // int 4
            public const string PARENTSTTRREF = "PARENTSTTRREF"; // int 4
            public const string ISDISTRIBUTED = "ISDISTRIBUTED"; // smallint 2
            public const string ITEMREF = "ITEMREF"; // int 4

            public const string ADDEXPENSE = "ADDEXPENSE"; // float 8
            public const string ADDRPEXPENSE = "ADDRPEXPENSE"; // float 8

            public static readonly string E_ITEMS__CODE = TAB_GETCOLFULLNAME(TABLE_ITEMS.TABLE, TABLE_ITEMS.CODE);
            public static readonly string E_ITEMS__DEFINITION_ = TAB_GETCOLFULLNAME(TABLE_ITEMS.TABLE, TABLE_ITEMS.NAME);
        }

        class TABLE_STOCKHEADER : TABLE_STFICHE
        {

        }

        class TABLE_DECARDS
        {
            public const string LOGICALREF = "LOGICALREF"; // int 4
            public const string TABLE = "DECARDS";
        }

        class TABLE_STFICHE
        {
            public const string TRCODE = "TRCODE"; // smallint 2
            public const string LOGICALREF = "LOGICALREF"; // int 4
            public const string FICHENO = "FICHENO"; // varchar 17
            public const string TABLE = "STFICHE";
            public const string DATE_ = "DATE_";
            public const string CANCELLED = "CANCELLED";
            public const string BRANCH = "BRANCH"; // smallint 2
            public const string DEPARTMENT = "DEPARTMENT"; // smallint 2
            public const string GENEXP1 = "GENEXP1"; // varchar 51
            public const string GENEXP2 = "GENEXP2"; // varchar 51
            public const string GENEXP3 = "GENEXP3"; // varchar 51
            public const string GENEXP4 = "GENEXP4"; // varchar 51
            public const string NETTOTAL = "NETTOTAL"; // float 8
            public const string REPORTRATE = "REPORTRATE"; // float 8
            public const string TRCURR = "TRCURR"; // smallint 2
            public const string TRRATE = "TRRATE"; // float 8
            public const string CLIENTREF = "CLIENTREF"; // int 4
            public const string SPECODE = "SPECODE"; // varchar 11
            public const string SPECODE2 = "SPECODE2"; // varchar 11
            public const string SPECODE3 = "SPECODE3"; // varchar 11
            public const string CYPHCODE = "CYPHCODE"; // varchar 11
            public static readonly string E_CLCARD__CODE = TAB_GETCOLFULLNAME(TABLE_CLCARD.TABLE, TABLE_CLCARD.CODE);
            public static readonly string E_CLCARD__DEFINITION_ = TAB_GETCOLFULLNAME(TABLE_CLCARD.TABLE, TABLE_CLCARD.DEFINITION_);
        }









        #endregion

        #endregion
