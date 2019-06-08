#line 2

const int VERSION = 9;
//Env. arguments
//ADPSTR - adapter string
//ADPCODE - adapter code
// !!! if pluging called externally (func PLUGIN) env is not inherited
////////////////////////////////////////////////////////////////////////////////////////////////
// Sys Event
//////////////////////////////////////////////////////////////////////////////////////////////////
//



 




public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
{

object arg1 = ARGS.Length > 0 ? ARGS[0]: null;
object arg2 = ARGS.Length > 1 ? ARGS[1]: null;
object arg3 = ARGS.Length > 2 ? ARGS[2]: null;

 string[] list_ = EXPLODELISTPATH(EVENTCODE);
 
 switch( list_.Length >0 ? list_[0]:""){
 
 case SysEvent.SYS_LOGIN:
MY_SYS_LOGIN() ;
 break;
 case SysEvent.SYS_NEWFORM:
 MY_SYS_NEWFORM(arg1 as Form) ;
 break;
 case SysEvent.SYS_EXITING:
 MY_SYS_EXITING() ;
 break;
  case SysEvent.SYS_ADPBEGIN:
 MY_SYS_ADPBEGIN(arg1 as DataSet) ;
 break;
  case SysEvent.SYS_ADPEND:
 MY_SYS_ADPEND(arg1 as DataSet) ;
 break;
  case SysEvent.SYS_ADPDONE:
 MY_SYS_ADPDONE(arg1 as DataSet) ;
 break;
  case SysEvent.SYS_ADPDSCHANGED:
 MY_SYS_ADPDSCHANGED(arg1 as DataSet) ;
 break;
  case SysEvent.SYS_USEREVENT:
 MY_SYS_USEREVENT(EVENTCODE,ARGS) ;
 break;
 case SysEvent.SYS_SYSEVENT:
 MY_SYS_SYSEVENT(EVENTCODE,ARGS) ;
 break;
 }
 

 
}
 
////////////////////////////////////////////////////////////////////////////////////////////////

public void MY_SYS_LOGIN() //user entered
{

 
}
public void MY_SYS_NEWFORM(Form FORM) //user init new form//only for customize able forms
{
if(FORM == null) return;
 
 
}
public void MY_SYS_EXITING() //user exiting
{
 

}
public void MY_SYS_ADPBEGIN(DataSet DATASET) //adapter start
{
//MSGUSERINFO((string)DATASET.ExtendedProperties[PRM_CONST.PRM_ADP_CMD]);

}
public void MY_SYS_ADPEND(DataSet DATASET) //adapter start
{
 
 
 
 
	
}
public void MY_SYS_ADPDONE(DataSet DATASET) //adapter start
{
 
  

}
public void MY_SYS_ADPDSCHANGED(DataSet DATASET) //adapter start
{
//MSGUSERINFO((string)DATASET.ExtendedProperties[PRM_CONST.PRM_ADP_CMD]);
 
}
public void MY_SYS_USEREVENT(string EVENTCODE, object[] ARGS) //adapter start
{
 
}

 


public void MY_SYS_SYSEVENT(string EVENTCODE, object[] ARGS) //adapter start
{
IDictionary<string,object> dic = ARGS.Length > 0 ? ARGS[0] as IDictionary<string,object>: null;
 if(dic == null)
 return;
 		
 string[] list_ = EXPLODELISTPATH(EVENTCODE);
  /*
 switch( list_.Length >1 ? list_[1].ToUpperInvariant():""){
 
  case UserEventType.SYS_STOCKLEVEL:
	 
 break;
 }
 
 */
}

 


 

///////////////////////////////////////////////////////////////////////////////////////////////

  
		
		 




  