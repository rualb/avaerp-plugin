#line 2

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

                    x.TYPE = s.MY_COST_TYPE;
 
                    //

                    _SETTINGS.BUF = x;

                }
 
                public string TYPE;

            }




            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC)
            {

            }

             


            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Type")]
            [TypeConverter(typeof(EListConverterCostType))]
            public string MY_COST_TYPE
            {
                get { return _GET("MY_COST_TYPE", "FIFO"); }
                set { _SET("MY_COST_TYPE", value); }

            }


            class EListConverterCostType : EListConverter
            {
                public EListConverterCostType()
                    : base(",,FIFO,FIFO,AVCO,AVCO") { 
					
					}
 
            }

        }

#endregion
#region TEXT
 
        public class TEXT
        {
            public const string text_DESC = "COST";
		}
#endregion

public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
{
  
object arg1 = ARGS.Length > 0 ? ARGS[0]: null;
object arg2 = ARGS.Length > 1 ? ARGS[1]: null;
object arg3 = ARGS.Length > 2 ? ARGS[2]: null;

 string[] list_ = EXPLODELISTPATH(EVENTCODE);
 
 switch( list_.Length >0 ? list_[0]:""){
case SysEvent.SYS_PLUGINSETTINGS:
(arg1 as List<object>).Add(new _SETTINGS(this));
break;
 case SysEvent.SYS_NEWFORM:
  _MY_SYS_NEWFORM_INTEGRATE_MAINFORM(  arg1 as Form);
 break;
  case SysEvent.SYS_USEREVENT:
 
 break;
 
 }
 

 
}



 
 
 void _MY_SYS_NEWFORM_INTEGRATE_MAINFORM(Form FORM)
        {
if(FORM == null)
return;

            var fn = GETFORMNAME(FORM);
            if (fn != "form.app")
                return;


				
				
            var tree = CONTROL_SEARCH(FORM, "cTreeTools");
            if (tree == null)
                return;

				
			_SETTINGS._BUF.LOAD_SETTINGS(this);
            string nodeCode_ = "cTreeTools_comhadleri_cost";

			if(_SETTINGS.BUF.TYPE == "")
			return;
			
            var args = new Dictionary<string, object>() {            
			        { "_cmd" ,""},
                    { "_type" ,""},
			        { "CmdText" ,""},
			        { "Text" ,TEXT.text_DESC},
			        { "ImageName" ,"struct_32x32"},
			        { "Name" ,nodeCode_},
                };

            RUNUIINTEGRATION(tree, args);
            var arr1 = new string[] { "rep loc::act001040001","rep loc::act001040002" };
            var arr2 = new string[] { "FIFO","AVCO" };
            var arr3 = new string[] { "cost_32x32","cost_32x32" };

            for (int i = 0; i < arr1.Length; ++i)
			if(_SETTINGS.BUF.TYPE == arr2[i])
            {
                args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
			{ "_root" ,nodeCode_},
			{ "CmdText" ,arr1[i]},
			{ "Text" ,arr2[i]},
			{ "ImageName" ,arr3[i]},
			{ "Name" ,nodeCode_+ "_"+arr1[i]},
            };

                RUNUIINTEGRATION(tree, args);
            }
        }
 




