#line 2


    #region PLUGIN_BODY
        const int VERSION = 15;


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

                    _SETTINGS.BUF = x;


                }


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
            public const string text_DESC = "Doc to Doc Converter";

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

                case SysEvent.SYS_D2D:
                    MY_SYS_D2D(arg1 as string, arg2 as DataSet, arg3 as DataSet);
                    break;

            }



        }





        public void MY_SYS_D2D(string CMD, DataSet DATASETS, DataSet DATASETD)
        {

            var isCmdInv = ("invoice" == CMD);
            var isCmdSlip = ("slip" == CMD);
            var isCmdReturn = ("return" == CMD);

            if (!(isCmdInv || isCmdSlip || isCmdReturn))
                return;

            var INVOICE = DATASETS.Tables["INVOICE"];
            var STFICHE = DATASETS.Tables["STFICHE"];
            var STLINE = DATASETS.Tables["STLINE"];
            var TRCODE = CASTASSHORT(TAB_GETROW(STLINE, "TRCODE"));
            var isOrder = ((TRCODE == 1 || TRCODE == 2) && STFICHE != null && STLINE.Columns["ORDTRANSREF"] == null);
            var isInv = (INVOICE != null);

            var isSaleOrder = (isOrder && TRCODE == 1);

            if (isCmdInv && isOrder)
            {
                TOOL_STOCK.CREATE_FROM_ORDER(DATASETS, DATASETD);
                var STLINED = DATASETD.Tables["STLINE"];
                foreach (DataRow ROW in STLINED.Rows)
                    if (!TAB_ROWDELETED(ROW))
                    {
                        //1 cut by appruve
                        var LINETYPE = CASTASSHORT(TAB_GETROW(ROW, "LINETYPE"));
                        var SOURCEINDEX = CASTASSHORT(TAB_GETROW(ROW, "SOURCEINDEX"));
                        double AMOUNT = CASTASDOUBLE(TAB_GETROW(ROW, "AMOUNT"));
                        var UINFO1 = CASTASDOUBLE(TAB_GETROW(ROW, "UINFO1"));
                        var UINFO2 = CASTASDOUBLE(TAB_GETROW(ROW, "UINFO2"));
                        var ORDTRANSREF = (TAB_GETROW(ROW, "ORDTRANSREF"));
                        var STOCKREF = (TAB_GETROW(ROW, "STOCKREF"));
                        if (LINETYPE == 0 || LINETYPE == 1)
                        {
                            var appruve = CASTASDOUBLE(ISNULL(SQLSCALAR(@"
SELECT SUM(AMOUNT* UINFO2 / UINFO1) AMOUNT 
FROM LG_$FIRM$_$PERIOD$_STLINE WITH(NOLOCK) WHERE ORDTRANSREF = @P1 AND @P1 > 0 AND STOCKREF = @P2
", new object[] { ORDTRANSREF, STOCKREF }), 0.0));
                            double unappruveRem = Math.Max(AMOUNT - appruve * UINFO1 / UINFO2, 0.0);

                            //cut by onhand
                            double onhand = 0.0;

                            if (isSaleOrder)
                            {
                                onhand = CASTASDOUBLE(
                               ISNULL(SQLSCALAR(@"

UPDATE LG_$FIRM$_$PERIOD$_GNTOTST SET LOGICALREF = LOGICALREF WHERE (STOCKREF =@P1) AND (INVENNO IN (@P2,-1));
SELECT ONHAND FROM LG_$FIRM$_$PERIOD$_GNTOTST WHERE (STOCKREF =@P1) AND (INVENNO = (@P2 )); 
 

-- declare @onhand float 
--  select @onhand  = 0
 
-- UPDATE LG_$FIRM$_$PERIOD$_STINVTOT
-- SET STOCKREF = STOCKREF ,@onhand = @onhand + ONHAND
-- WHERE  LOGICALREF IN (SELECT top 100 PERCENT LOGICALREF FROM LG_$FIRM$_$PERIOD$_STINVTOT WHERE  (STOCKREF =@P1)
-- 	AND (INVENNO IN (@P2)) ORDER BY STOCKREF,INVENNO,DATE_)

-- SELECT ISNULL(@onhand,0)


", new object[] { STOCKREF, SOURCEINDEX }), 0.0)
                               );

                            }

                            unappruveRem = Math.Max(unappruveRem, 0.0);

                            if (isSaleOrder)
                            {
                                double realAppruvable = (onhand > unappruveRem ? unappruveRem : onhand);
                                TAB_SETROW(ROW, "AMOUNT", realAppruvable);

                            }
                            else
                            {
                                //prch
                                double realAppruvable = unappruveRem;
                                TAB_SETROW(ROW, "AMOUNT", realAppruvable);
                            }
                          
                        }

                    }

                return;
            }




        }



        #endregion





        #endregion
