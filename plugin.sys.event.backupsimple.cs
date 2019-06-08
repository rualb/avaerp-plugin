#line 2


   #region PLUGIN_BODY
        const int VERSION = 21;


        /*
         * try use defined bacup folder
         * try use [C-Z]:/(AppName)/Backup
         * try use ../backup
         *      
         */
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



                    x.MY_DBBACKUP_SIMPLE_DB_LIST = s.MY_DBBACKUP_SIMPLE_DB_LIST;
                    x.MY_DBBACKUP_SIMPLE_PATH = s.MY_DBBACKUP_SIMPLE_PATH;
                    x.MY_DBBACKUP_SIMPLE_PATH_COPY = s.MY_DBBACKUP_SIMPLE_PATH_COPY;
                    x.MY_DBBACKUP_SIMPLE_PATH_COPY2 = s.MY_DBBACKUP_SIMPLE_PATH_COPY2;

                    x.MY_DBBACKUP_SIMPLE_USER = s.MY_DBBACKUP_SIMPLE_USER;
                    x.MY_DBBACKUP_TIMEOUT = s.MY_DBBACKUP_TIMEOUT;
                    x.MY_DBBACKUP_COUNT = s.MY_DBBACKUP_COUNT;


                    x.MY_DBBACKUP_TIME_1 = s.MY_DBBACKUP_TIME_1;
                    x.MY_DBBACKUP_TIME_2 = s.MY_DBBACKUP_TIME_2;
                    x.MY_DBBACKUP_TIME_3 = s.MY_DBBACKUP_TIME_3;
                    //
                    x.MY_DBBACKUP_ONLOGIN = s.MY_DBBACKUP_ONLOGIN;
                    //
                    x.MY_DBBACKUP_SIMPLE_BINDIR = s.MY_DBBACKUP_SIMPLE_BINDIR;

                    x._USER = PLUGIN.GETSYSPRM_USER();
                    x._FIRM = PLUGIN.GETSYSPRM_FIRM();
                    x._FIRMNAME = PLUGIN.GETSYSPRM_FIRMNAME();
                    x._PERIOD = PLUGIN.GETSYSPRM_PERIOD();


                    x._ISUSEROK = ("[" + x.MY_DBBACKUP_SIMPLE_USER + "]").Contains(FORMAT(x._USER));



                    _SETTINGS.BUF = x;


                }


                public string MY_DBBACKUP_SIMPLE_DB_LIST;
                public string MY_DBBACKUP_SIMPLE_PATH;
                public string MY_DBBACKUP_SIMPLE_PATH_COPY;
                public string MY_DBBACKUP_SIMPLE_PATH_COPY2;
                public string MY_DBBACKUP_SIMPLE_USER;
                public int MY_DBBACKUP_TIMEOUT;
                public int MY_DBBACKUP_COUNT;
                public bool MY_DBBACKUP_ONLOGIN;

                public int MY_DBBACKUP_TIME_1;
                public int MY_DBBACKUP_TIME_2;
                public int MY_DBBACKUP_TIME_3;


                public string MY_DBBACKUP_SIMPLE_BINDIR;
                //

                public bool _ISUSEROK;
                public short _FIRM;
                public string _FIRMNAME;
                public short _PERIOD;
                public short _USER;



            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC)//, "ava.dbbackup.config")
            {

            }




            [ECategory(TEXT.text_DESC)]
            [EDisplayName("List of Database's")]
            public string MY_DBBACKUP_SIMPLE_DB_LIST
            {
                get
                {
                    return _GET("MY_DBBACKUP_SIMPLE_DB_LIST", "");

                }
                set
                {
                    _SET("MY_DBBACKUP_SIMPLE_DB_LIST", value);
                }

            }


            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Backup Path")]
            public string MY_DBBACKUP_SIMPLE_PATH
            {
                get
                {
                    var p = _GET("MY_DBBACKUP_SIMPLE_PATH", "");
                    if (ISEMPTY(p))
                    {
                        p= MY_GET_BACKUP_DIR_INTERNAL();

                        //var arr = MY_BACKUPDIR_SEARCH();
                        //if (arr != null && arr.Length > 0)
                        //    p = MY_DBBACKUP_SIMPLE_PATH = arr[0];

                    }

                    return p;
                }
                set
                {
                    _SET("MY_DBBACKUP_SIMPLE_PATH", value);
                }

            }
            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Backup Path Copy")]
            public string MY_DBBACKUP_SIMPLE_PATH_COPY
            {
                get
                {
                    var p = _GET("MY_DBBACKUP_SIMPLE_PATH_COPY", "");

                    if (ISEMPTY(p))
                        p= MY_GET_BACKUP_DIR_EXTERNAL();

                    return p;
                }
                set
                {
                    _SET("MY_DBBACKUP_SIMPLE_PATH_COPY", value);
                }

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Backup Path Copy 2")]
            public string MY_DBBACKUP_SIMPLE_PATH_COPY2
            {
                get
                {
                    return _GET("MY_DBBACKUP_SIMPLE_PATH_COPY2", "");
                }
                set
                {
                    _SET("MY_DBBACKUP_SIMPLE_PATH_COPY2", value);
                }

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Backup On User (Nr)")]
            public string MY_DBBACKUP_SIMPLE_USER
            {
                get
                {
                    return (_GET("MY_DBBACKUP_SIMPLE_USER", "1,2"));
                }
                set
                {
                    _SET("MY_DBBACKUP_SIMPLE_USER", value);
                }

            }


            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Backup On User Login")]
            public bool MY_DBBACKUP_ONLOGIN
            {
                get
                {
                    return _GET("MY_DBBACKUP_ONLOGIN", "1") == "1";
                }
                set
                {
                    _SET("MY_DBBACKUP_ONLOGIN", value ? "1" : "0");
                }

            }



            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Backup TimeOut (Seconds)")]
            public int MY_DBBACKUP_TIMEOUT
            {
                get
                {
                    return (int)MAX(5 * 60, PARSEINT(_GET("MY_DBBACKUP_TIMEOUT", "300")));
                }
                set
                {
                    _SET("MY_DBBACKUP_TIMEOUT", value);
                }

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Backup Count")]
            public int MY_DBBACKUP_COUNT
            {
                get
                {
                    return (int)MAX(5, PARSEINT(_GET("MY_DBBACKUP_COUNT", "30")));
                }
                set
                {
                    _SET("MY_DBBACKUP_COUNT", value);
                }

            }



            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Backup Time 1 (0 - 23 and -1)")]
            public int MY_DBBACKUP_TIME_1
            {
                get
                {
                    return (int)MIN(23, MAX(-1, PARSEINT(_GET("MY_DBBACKUP_TIME_1", "-1"))));
                }
                set
                {
                    _SET("MY_DBBACKUP_TIME_1", value);
                }

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Backup Time 2 (0 - 23 and -1)")]
            public int MY_DBBACKUP_TIME_2
            {
                get
                {
                    return (int)MIN(23, MAX(-1, PARSEINT(_GET("MY_DBBACKUP_TIME_2", "-1"))));
                }
                set
                {
                    _SET("MY_DBBACKUP_TIME_2", value);
                }

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Backup Time 3 (0 - 23 and -1)")]
            public int MY_DBBACKUP_TIME_3
            {
                get
                {
                    return (int)MIN(23, MAX(-1, PARSEINT(_GET("MY_DBBACKUP_TIME_3", "-1"))));
                }
                set
                {
                    _SET("MY_DBBACKUP_TIME_3", value);
                }

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Bin's location dir")]
            public string MY_DBBACKUP_SIMPLE_BINDIR
            {
                get
                {
                    return (_GET("MY_DBBACKUP_SIMPLE_BINDIR", ""));
                }
                set
                {
                    _SET("MY_DBBACKUP_SIMPLE_BINDIR", value);
                }

            }

            protected override string _GET(string name, string def)
            {
                return base._GET(name, def);
            }

            protected override void _SET(string name, object val)
            {
                base._SET(name, val);
            }


        }

        #endregion
        #region TEXT

        public class TEXT
        {
            public const string text_DESC = "Buckup On User Login";

        }
        #endregion

        #region MAIN





        public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
        {
            if (ISWEB())
                return;

            // if (!ISMSSQL())
            //   return;

            _SETTINGS._BUF.LOAD_SETTINGS(this);

            object arg0 = ARGS.Length > 0 ? ARGS[0] : null;
            object arg1 = ARGS.Length > 1 ? ARGS[1] : null;
            object arg2 = ARGS.Length > 2 ? ARGS[2] : null;

            string[] list_ = EXPLODELISTPATH(EVENTCODE);

            switch (list_.Length > 0 ? list_[0] : "")
            {
                case SysEvent.SYS_PLUGINSETTINGS:
                    (arg0 as List<object>).Add(new _SETTINGS(this));
                    break;
                case SysEvent.SYS_LOGIN:
                    if (_SETTINGS.BUF._ISUSEROK)
                    {

                        foreach (var time in new int[] {
                        _SETTINGS.BUF.MY_DBBACKUP_TIME_1,
                        _SETTINGS.BUF.MY_DBBACKUP_TIME_2,
                        _SETTINGS.BUF.MY_DBBACKUP_TIME_3,
                        })
                            if (time > 0)
                            {
                                RUNUIINTEGRATION(this, new Dictionary<string, object>() { 
                                    {"_cmd","jobpool"},
                                   {"time",FORMAT(time)},
                                //  {"time","20.29"},
                                 
                                  {"action",new Action(MY_SYS_BACKUP_DB)},
                                    });
                            }


                        if (_SETTINGS.BUF.MY_DBBACKUP_ONLOGIN)
                        {
                            MY_SYS_BACKUP_DB();
                            MY_SYS_BACKUP_APP(GETHOMEDIR());
                        }

                    }
                    break;
                case SysEvent.SYS_USEREVENT:
                    //   if (EVENTCODE.EndsWith("/dbbackup"))
                    //     MY_SYS_BACKUP();
                    break;

            }



        }



        static int isDbOk()
        {
            var conn = false;
            var db = false;
            try
            {
                XSQLSCALAR("SELECT 'OK'", null, null, true);// false);
                conn = true;

                XSQLSCALAR("SELECT 'OK'", null, null, true);
                db = true;

            }
            catch (Exception exc)
            {

            }

            if (conn && db)
                return 1;

            if (conn && !db)
                return 2;

            return 0;
        }


        static string MY_CLEAN_OLD(string pDir, string pFileNameTemplate, int pCount)
        {
            try
            {//clean

                pCount = Math.Max(pCount, 5);

                if (string.IsNullOrEmpty(pDir))
                    return null;

                var bakCount = pCount; //one will be created

                var baks = Directory.GetFiles(pDir, pFileNameTemplate);
                Array.Sort<string>(baks);

                for (int i = 0; i < (baks.Length - bakCount); ++i)
                {
                    File.Delete(baks[i]);
                }


                if (baks.Length > 0)
                    return baks[baks.Length - 1];

            }
            catch (Exception exc)
            {
                RUNTIMELOG(exc.ToString());

            }

            return null;
        }

        static void MY_SYS_BACKUP_APP(string pDir) //adapter start
        {
            try
            {

                if (_SETTINGS.BUF._FIRM == 0)
                    return;


                {
                    /*
                     TODO
                     * 
                     * use Google Cloud
                     * 
                     */

                    if (isDbOk() != 1)
                        return;

                    var pathBackup = _SETTINGS.BUF.MY_DBBACKUP_SIMPLE_PATH;

                    var pathBackupCopy = _SETTINGS.BUF.MY_DBBACKUP_SIMPLE_PATH_COPY;
                    var pathBackupCopy2 = _SETTINGS.BUF.MY_DBBACKUP_SIMPLE_PATH_COPY2;

                    var appName = Path.GetFileName(pDir).ToLowerInvariant();

                    var date = CASTASDATE(XSQLSCALAR(@"
--$MS$-- SELECT getdate()
--$PG$-- SELECT now()
", null));
                    // var dbname = CASTASSTRING(XSQLSCALAR("SELECT DB_NAME()", null)); //use as app name
                    var fileTemplate = "appbackup#{0}#{1}#" + FORMAT(_SETTINGS.BUF._USER).PadLeft(4, '0') + ".backup.zip";
                    var fileTemplateSearch = string.Format(fileTemplate, appName, "*");

                    if (string.IsNullOrEmpty(pathBackup))
                        return;

                    var lastBak = MY_CLEAN_OLD(pathBackup, fileTemplateSearch, _SETTINGS.BUF.MY_DBBACKUP_COUNT);
                    MY_CLEAN_OLD(pathBackupCopy, fileTemplateSearch, _SETTINGS.BUF.MY_DBBACKUP_COUNT);
                    MY_CLEAN_OLD(pathBackupCopy2, fileTemplateSearch, _SETTINGS.BUF.MY_DBBACKUP_COUNT);

                    if (lastBak != null)
                    {

                        var lastBackDate = PARSEDATETIME(Path.GetFileName(lastBak).Split('#')[2]);

                        if ((date - lastBackDate).Days < 10)
                            return;

                    }

                    var file = string.Format(fileTemplate, appName, FORMAT(date).Replace(' ', '-').Replace(':', '-'));

                    var bakPath = Path.Combine(pathBackup, file);

                    RUNWITHTIMEOUTNOWAIT((
                        () =>
                        {
                            //1
                            MY_DO_BAK_APP(pDir, bakPath);
                            //2
                            MY_COPY_BAK(bakPath, pathBackupCopy);
                            MY_COPY_BAK(bakPath, pathBackupCopy2);

                        }), _SETTINGS.BUF.MY_DBBACKUP_TIMEOUT);

                    //////////////////////////////////////////////////////////

                }

            }
            catch (Exception exc)
            {
                RUNTIMELOG(exc.ToString());
            }


        }



        static void MY_SYS_BACKUP_DB() //adapter start
        {
            try
            {

                if (_SETTINGS.BUF._FIRM == 0)
                    return;


                {
                    /*
                     TODO
                     * 
                     * use Google Cloud
                     * 
                     */

                    if (isDbOk() != 1)
                        return;

                    var list = new List<string>(EXPLODELIST(_SETTINGS.BUF.MY_DBBACKUP_SIMPLE_DB_LIST));

                    var dbname = CASTASSTRING(XSQLSCALAR(@"
--$MS$-- SELECT DB_NAME()
--$PG$-- SELECT CURRENT_DATABASE()
", null));

                    if (!list.Contains(dbname))
                        list.Add(dbname);

                    foreach (var x in list)
                    {

                        dbname = x.Trim();

                        if (string.IsNullOrEmpty(dbname))
                            continue;


                        var date = CASTASDATE(XSQLSCALAR(@"
--$MS$-- SELECT getdate()
--$PG$-- SELECT now()
", null));


                        var fileTemplate = (ISPOSTGRESQL() ? "pgbackup" : "msbackup") + "#{0}#{1}#" + FORMAT(_SETTINGS.BUF._USER).PadLeft(4, '0') + ".backup";
                        var fileTemplateSearch = string.Format(fileTemplate, dbname, '*');

                        // var path = Path.Combine(_SETTINGS.BUF.MY_DBBACKUP_SIMPLE_PATH, dbname);
                        var pathBackup = _SETTINGS.BUF.MY_DBBACKUP_SIMPLE_PATH;
                        var pathBackupCopy = _SETTINGS.BUF.MY_DBBACKUP_SIMPLE_PATH_COPY;
                        var pathBackupCopy2 = _SETTINGS.BUF.MY_DBBACKUP_SIMPLE_PATH_COPY2;

                        if (string.IsNullOrEmpty(pathBackup))
                            return;


                        var lastBak = MY_CLEAN_OLD(pathBackup, fileTemplateSearch, _SETTINGS.BUF.MY_DBBACKUP_COUNT);
                        MY_CLEAN_OLD(pathBackupCopy, fileTemplateSearch, _SETTINGS.BUF.MY_DBBACKUP_COUNT);
                        MY_CLEAN_OLD(pathBackupCopy2, fileTemplateSearch, _SETTINGS.BUF.MY_DBBACKUP_COUNT);

                        var file = string.Format(fileTemplate, dbname, FORMAT(date).Replace(' ', '-').Replace(':', '-'));

                        var bakPath = Path.Combine(pathBackup, file);


                        var dic = new Dictionary<string, string>()
                        {
                            {"Database",dbname}
                        };


                        {


                            var ds = GET_ENV("DATASOURCE", "");

                            var arr = ds.Split(';', ',');

                            foreach (var itm in arr)
                            {
                                var keyval = itm.Trim().Split('=');

                                if (keyval.Length == 2)
                                {
                                    var key = keyval[0].Trim();
                                    var val = keyval[1].Trim();
                                    switch (key)
                                    {
                                        case "User Id":
                                            dic["User Id"] = val;
                                            break;
                                        case "Password":
                                            dic["Password"] = val;
                                            break;
                                        //case "Database":
                                        //case "Initial Catalog":
                                        //    dic["Database"] = val;
                                        //    break;
                                        case "Server":
                                        case "Data Source":
                                            dic["Server"] = val;
                                            break;
                                    }

                                }
                            }


                        }

                        //1
                        RUNWITHTIMEOUTNOWAIT((
                          () =>
                          {
                              //bak from prev bak-run
                              MY_COPY_BAK(lastBak, pathBackupCopy);
                              MY_COPY_BAK(lastBak, pathBackupCopy2);

                          }), _SETTINGS.BUF.MY_DBBACKUP_TIMEOUT);

                        //2
                        RUNWITHTIMEOUTNOWAIT((
                            () =>
                            {
                                //run bak, may be in separated process 
                                MY_DO_BAK_DB(bakPath, dic);
                            }), _SETTINGS.BUF.MY_DBBACKUP_TIMEOUT);

                        //////////////////////////////////////////////////////////
                    }
                }

            }
            catch (Exception exc)
            {
                RUNTIMELOG(exc.ToString());
            }


        }

        static void MY_COPY_BAK(string pFile, string pDir)
        {

            try
            {

                if (!string.IsNullOrEmpty(pDir) &&
                                                  Directory.Exists(pDir) &&
                                                  File.Exists(pFile))
                {
                    RUNTIMELOG("Try backup copy [" + pFile + "] to dir [" + pDir + "]");

                    var file = Path.GetFileName(pFile);

                    var target = Path.Combine(pDir, file);
                    if (pFile.ToLowerInvariant() != target.ToLowerInvariant())
                    {
                        if (File.Exists(target))
                            RUNTIMELOG("Copy backup stoped, file exists: " + target);
                        else
                        {
                            RUNTIMELOG("Copy backup starting ["+pFile+"] to [" + target+"]");
                            File.Copy(pFile, target);
                        }

                    }


                }

            }
            catch (Exception exc)
            {
                RUNTIMELOG(exc.ToString());
            }
        }


        static void MY_DO_BAK_DB(string pFile, Dictionary<string, string> pArgs)
        {

            try
            {







                if (ISMSSQL())
                {
                    var sql = @"
                    declare @bakpath nvarchar(1000)=@P1,@dbname nvarchar(100) = DB_NAME()
                    BACKUP DATABASE @dbname TO DISK = @bakpath
                    WITH INIT,
	                    SKIP
";
                    XSQLSCALAR(sql, new object[] { pFile });
                }
                else
                    if (ISPOSTGRESQL())
                    {
                        var binDir = _SETTINGS.BUF.MY_DBBACKUP_SIMPLE_BINDIR;

                        if (ISEMPTY(binDir))
                        {
                            binDir = XSQLSCALAR(" select setting from  pg_config where name = 'BINDIR' ", null) as string;
                        }


                        var dumpBin = binDir + "" + "/pg_dump.exe";

                        //   var args = new string[]{

                        //    "--file",
                        //    "\""+pFile+"\"",

                        //   "--host",
                        //  pArgs["Server"],// "localhost",

                        //   "--port",
                        //   "5432",

                        //   "--username",
                        //pArgs["User Id"],//   "postgres",

                        //"--password",


                        // //  "--no-password",

                        //   //"--verbose",

                        //   "--format=c",
                        //  //  "--format=p",
                        //  //   "--format=t",


                        //   "--blobs",

                        //   pArgs["Database"]

                        //   };

                        var args = new string[]{
                        
                              "--file",
                         "\""+pFile+"\"",
                         "--dbname=postgresql://"+pArgs["User Id"]+":"+pArgs["Password"]+"@"+pArgs["Server"]+":5432/"+pArgs["Database"]+" ",
                               "--format=c",
                         "--blobs",
                        };



                        var argsList = string.Join(" ", args);

                        RUNTIMELOG(dumpBin + " " + argsList);

                        var process = PROCESS(
                               dumpBin,
                            argsList
                        );




                        /*
                 * //tar
                 --file "H:\\POSTGR~1\\AVA_TE~1.BAK" --host "localhost" --port "5432" --username "postgres" --no-password --verbose --format=t --blobs "ava_test"
                 //default(custom)
                 --file "H:\\POSTGR~1\\AVA_TE~2.BAK" --host "localhost" --port "5432" --username "postgres" --no-password --verbose --format=c --blobs "ava_test"
 
                 use  --verbose for details
                         
                 * */


                    }


            }
            catch (Exception exc)
            {
                RUNTIMELOG(exc.ToString());
            }
        }
        static void MY_DO_BAK_APP(string pSourceDir, string pFile)
        {

            try
            {

                //RUNTIMELOG("Backup App: [" + pSourceDir + "] to [" + pFile + "]");

                var root = pSourceDir;
                string[] files = System.IO.Directory.GetFiles(root, "*", System.IO.SearchOption.AllDirectories);

                var d = new Dictionary<string, byte[]>();

                foreach (var f in files)
                {
                    var key = f.Substring(root.Length);

                    key = key.TrimStart('/', '\\');

                    d[key] = System.IO.File.ReadAllBytes(f);
                }


                byte[] data = ZIP(d);

                System.IO.File.WriteAllBytes(pFile, data);


                //  ZIP(pSourceDir, pFile);

                //  XSQLSCALAR(sql, new object[] { pFile });
            }
            catch (Exception exc)
            {
                RUNTIMELOG(exc.ToString());
            }
        }
        static string[] MY_BACKUPDIR_SEARCH()
        {



            if (Application.ExecutablePath.StartsWith("\\\\")) //from shared folder
                return new string[] { };

            var arr = System.IO.DriveInfo.GetDrives();

            var appname = APPNAME();

            appname = appname + "/backup";
            //
            var list = new List<string>();

            var driveSys = System.IO.Path.GetPathRoot(Environment.SystemDirectory);
            foreach (var d in arr)
                if (
                    (
                    d.DriveType == System.IO.DriveType.Fixed ||
                    d.DriveType == System.IO.DriveType.Removable
                    )
                    && d.IsReady)
                {
                    var root = d.RootDirectory.FullName;
                    var dir = System.IO.Path.Combine(root, appname);
                    if (!root.StartsWith(dir))
                    {
                        if (Directory.Exists(dir))
                            list.Add(dir);
                    }

                }



            // if (list.Count == 0)
            {
                var d = Path.GetFullPath(Path.Combine(GETHOMEDIR(), "../backup"));
                if (Directory.Exists(d))
                    list.Add(d);
            }


            return list.ToArray();
        }


        #endregion


        #region TOOLS



        static string MY_GET_BACKUP_DIR_EXTERNAL()
        {

            var driveList = System.IO.DriveInfo.GetDrives();
            foreach (var drive in driveList)
            {
                if (drive.DriveType == System.IO.DriveType.Removable)
                {
                    var path = drive.Name + "/backup";
                    if (System.IO.Directory.Exists(path))
                    {
                        return path;
                    }
                }


            }

            return null;
        }
        static string MY_GET_BACKUP_DIR_INTERNAL()
        {

            var driveList = System.IO.DriveInfo.GetDrives();
            foreach (var drive in driveList)
            {
                if (drive.DriveType == System.IO.DriveType.Fixed)
                {
                    var path = drive.Name + "/backup";
                    if (System.IO.Directory.Exists(path))
                    {
                        return path;
                    }
                }

            }

            return null;
        }
        #endregion


        #endregion