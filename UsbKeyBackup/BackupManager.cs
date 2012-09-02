using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace KlerksSoft.UsbKeyBackup
{
    internal class BackupManager
    {
        private KlerksSoft.EasyProgressDialog.ProgressDialog _progressDialog;
        private SingleAssemblyResourceManager _generalResourceManager;
        private string _password;
        private string _passwordCheckPhrase;
        private BackupSource _source;
        private string _backupLocation;
        private long? _FreeSpace;
        private DateTime? _latestBackupDate;
        private bool _useDatabase;
        private BackupCleaningOptions _cleaningOptions;
        private List<string> _expiredFolders;

        private BackupDatabase _backupDB;
        private List<int> _expiredDBBackups;

        private BackupLocationDatabase _backupLocationDB;

        public BackupManager(BackupSource source, string destinationPath, string password, string passwordCheckPhrase, SingleAssemblyResourceManager generalResourceManager, BackupCleaningOptions cleaningOptions, bool useDatabase)
            : this(source, destinationPath, password, passwordCheckPhrase, generalResourceManager, cleaningOptions, useDatabase, null) { }

        public BackupManager(BackupSource source, string destinationPath, string password, string passwordCheckPhrase, SingleAssemblyResourceManager generalResourceManager, BackupCleaningOptions cleaningOptions, bool useDatabase, KlerksSoft.EasyProgressDialog.ProgressDialog progressDialog)
        {
            if (!useDatabase && !string.IsNullOrEmpty(password))
                throw new ArgumentException("password must be empty if database is not used.", "password");

            _source = source;
            _backupLocation = destinationPath;
            _password = password;
            _passwordCheckPhrase = passwordCheckPhrase;
            _progressDialog = progressDialog;
            _generalResourceManager = generalResourceManager;
            _cleaningOptions = cleaningOptions;
            _useDatabase = useDatabase;
        }

        public BackupSource BackupSource
        {
            get
            {
                return _source;
            }
        }

        public string BackupLocation
        {
            get
            {
                return _backupLocation;
            }
        }

        private int? _existingBackupLocationEntryID;
        private int? ExistingBackupLocationEntryID
        {
            get
            {
                if (_existingBackupLocationEntryID == null)
                {
                    if (LoadedBackupLocationDB != null)
                    {
                        foreach (BackupLocationInfo locationInfo in LoadedBackupLocationDB.GetLocationsBySourceLabel(_source.BackupLabel))
                        {
                            if (string.IsNullOrEmpty(_password))
                            {
                                if (string.IsNullOrEmpty(locationInfo.EncryptionCheckValue))
                                {
                                    _existingBackupLocationEntryID = locationInfo.LocationEntryID;
                                }
                            }
                            else
                            {
                                try
                                {
                                    string previousCheckValue = Utils.DecryptString(locationInfo.EncryptionCheckValue, _password);
                                    if (previousCheckValue.Equals(_passwordCheckPhrase))
                                    {
                                        _existingBackupLocationEntryID = locationInfo.LocationEntryID;
                                    }
                                }
                                catch (Exception e)
                                {
                                    //decryption failed, so we didn't find what we wanted - that's fine, no further handling.
                                }
                            }

                        }
                    }
                }

                return _existingBackupLocationEntryID;
            }
        }

        public string FullBackupLocation
        {
            get
            {
                if (ExistingBackupLocationEntryID != null)
                    return Path.Combine(_backupLocation, _backupLocationDB.GetLocation(ExistingBackupLocationEntryID.Value).BackupPath);
                else
                    return String.Empty;
            }
        }

        public bool FullBackupLocationExists
        {
            get
            {
                return Directory.Exists(FullBackupLocation);
            }
        }

        public string FullBackupLocationDBDataPath
        {
            get
            {
                return System.IO.Path.Combine(FullBackupLocation, "FileData");
            }
        }

        public long? FreeSpace
        {
            get
            {
                if (_FreeSpace == null)
                {
                    string root = System.IO.Directory.GetDirectoryRoot(_backupLocation);
                    System.IO.DriveInfo destDrive = new System.IO.DriveInfo(root);
                    _FreeSpace = destDrive.AvailableFreeSpace;
                }
                return _FreeSpace;
            }
        }

        public DateTime? LatestBackupDate
        {
            get
            {
                if (_latestBackupDate == null)
                {
                    if (FullBackupLocationExists)
                    {
                        if (_useDatabase)
                        {
                            BackupInfo latestBackup = LoadedBackupDB.GetLatestBackup();
                            if (latestBackup != null)
                                _latestBackupDate = latestBackup.BackupDate;
                        }
                        else
                        {
                            System.IO.DirectoryInfo destinationFolder = new System.IO.DirectoryInfo(FullBackupLocation);
                            foreach (System.IO.DirectoryInfo possibleBackupDateDir in destinationFolder.GetDirectories())
                            {
                                DateTime thisDate;
                                if (DateTime.TryParse(possibleBackupDateDir.Name, out thisDate))
                                    if (_latestBackupDate == null || thisDate > _latestBackupDate)
                                        _latestBackupDate = thisDate;
                            }
                        }
                    }

                }
                return _latestBackupDate;
            }
        }

        private BackupDatabase LoadedBackupDB
        {
            get
            {
                if (_backupDB == null)
                {
                    if (Directory.Exists(FullBackupLocation))
                    {
                        _backupDB = new BackupDatabase(FullBackupLocation);

                        if (_backupDB.DatabaseFileExists)
                            _backupDB.ReadDatabaseFromFile();
                    }
                }
                return _backupDB;
            }
        }

        private BackupLocationDatabase LoadedBackupLocationDB
        {
            get
            {
                if (_backupLocationDB == null)
                {
                    if (Directory.Exists(_backupLocation))
                    {
                        _backupLocationDB = new BackupLocationDatabase(_backupLocation);

                        if (_backupLocationDB.DatabaseFileExists)
                            _backupLocationDB.ReadDatabaseFromFile();
                    }
                }
                return _backupLocationDB;
            }
        }

        public bool BackUp()
        {
            DirectoryInfo mainBackupFolder;
            if (!Directory.Exists(_backupLocation))
                mainBackupFolder = Directory.CreateDirectory(_backupLocation);
            else
                mainBackupFolder = new DirectoryInfo(_backupLocation);

            if (string.IsNullOrEmpty(FullBackupLocation))
            {
                LoadedBackupLocationDB.AddLocation(_source.BackupLabel, Utils.EncryptString(_passwordCheckPhrase, _password));
                LoadedBackupLocationDB.WriteDatabaseToFile();
            }

            if (_useDatabase)
                return BackUpWithDatabase();
            else
                return BackUpWithoutDatabase();
        }


        private bool BackUpWithDatabase()
        {
            if (!Directory.Exists(FullBackupLocationDBDataPath))
                Directory.CreateDirectory(FullBackupLocationDBDataPath);

            int backupID = LoadedBackupDB.StartBackup();

            DialogResult backupResult = _progressDialog.StartProgressDialog(_generalResourceManager.GetString("BackupProgressTitle"), _generalResourceManager.GetString("BackupProgressText"), _source.SourceSize.Value, bg_BackUpWithDatabase_DoWork, backupID);

            if (backupResult != DialogResult.Cancel)
            {
                LoadedBackupDB.CompleteBackup(backupID);
                LoadedBackupDB.WriteDatabaseToFile();
                return true;
            }
            else
                return false;
        }

        private void bg_BackUpWithDatabase_DoWork(object WorkArg)
        {
            int backupID = (int)WorkArg;
            DirectoryInfo originalFolder = new DirectoryInfo(_source.SourcePath);
            RecursiveDatabaseBasedCopy(originalFolder, backupID);
        }

        private bool RecursiveDatabaseBasedCopy(DirectoryInfo fromFolder, int backupID)
        {
            foreach (FileInfo sourceFile in fromFolder.GetFiles())
            {
                bool physicalCopy;
                string physicalFileName;

                int? existingFileID = LoadedBackupDB.GetFileRecordIDByProperties(_source.StripSourceFromPath(sourceFile.FullName), sourceFile.LastWriteTime, sourceFile.Length);

                if (existingFileID == null)
                {
                    existingFileID = LoadedBackupDB.AddFileRecord(_source.StripSourceFromPath(sourceFile.FullName), sourceFile.LastWriteTime, sourceFile.Length);
                    physicalFileName = "";
                    physicalCopy = true;
                }
                else
                {
                    physicalFileName = LoadedBackupDB.GetBackupFilePathByEntryID(existingFileID.Value);
                    
                    if (!string.IsNullOrEmpty(physicalFileName) && File.Exists(Path.Combine(FullBackupLocationDBDataPath, physicalFileName)))
                        physicalCopy = false;
                    else
                        physicalCopy = true;
                }

                if (physicalCopy)
                {
                    physicalFileName = DefineDBDataFileName(existingFileID.Value, _source.StripSourceFromPath(sourceFile.FullName), sourceFile.LastWriteTime, sourceFile.Length, _password);

                    string SHA1 = "";
                    if (string.IsNullOrEmpty(_password))
                    {
                        sourceFile.CopyTo(Path.Combine(FullBackupLocationDBDataPath, physicalFileName), true);
                        //No SHA1 - would be expensive to calculate given that we are using simple file copy.
                    }
                    else
                    {
                        SHA1 = Utils.EncryptFile(sourceFile.FullName, Path.Combine(FullBackupLocationDBDataPath, physicalFileName), _password);
                    }

                    LoadedBackupDB.UpdateDBDataFileName(existingFileID.Value, physicalFileName, SHA1);
                }

                LoadedBackupDB.AddBackupFileRecord(backupID, existingFileID.Value);

                //update progress (exit if cancelled)
                if (_progressDialog != null && !_progressDialog.Worker_SetSpecificProgress(null, _progressDialog.CurrentCount + sourceFile.Length, null)) 
                    return false;
            }

            foreach (DirectoryInfo sourceFolder in fromFolder.GetDirectories())
            {
                if (!RecursiveDatabaseBasedCopy(sourceFolder, backupID))
                    return false; //if cancelled at any point, return/abort. (recursive...)
            }

            return true; //if we got to this point, so far so good! (recursive...)
        }

        private static string DefineDBDataFileName(int fileEntryID, string fileName, DateTime dateTime, long fileSize, string password)
        {
            string extension = Path.GetExtension(fileName);
            string beforeFilename = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName));

            string outName = "";
            if (beforeFilename.Length > 100)
                outName += beforeFilename.Replace(Path.DirectorySeparatorChar, '_').Replace('.', '_').Substring(0, 100);
            else
                outName += beforeFilename.Replace(Path.DirectorySeparatorChar, '_').Replace('.', '_');

            if (extension.Contains("."))
                outName += extension;
            else
                outName += "."; //just a dot.

            outName += ".";
            outName += fileEntryID.ToString();
            outName += ".";
            outName += Utils.SortableStringFromDate(dateTime);

            if (string.IsNullOrEmpty(password))
                outName += ".backup";
            else
                outName += ".encrypted";

            return outName;
        }

        internal static string GetFilenameFromDBDataFileName(string dbDataFileName)
        {
            string[] filenamePieces = Path.GetFileName(dbDataFileName).Split(new char[] {'.'});
            if (filenamePieces.Length >= 2)
            {
                return filenamePieces[0] + "." + filenamePieces[1];
            }
            else
            {
                return dbDataFileName;
            }
        }

        private bool BackUpWithoutDatabase()
        {
            string ISODateString = Utils.SortableStringFromDate(DateTime.Today);
            string finalFolderString = System.IO.Path.Combine(FullBackupLocation, ISODateString);
            System.IO.DirectoryInfo finalFolder;
            if (System.IO.Directory.Exists(finalFolderString))
                finalFolder = new System.IO.DirectoryInfo(finalFolderString);
            else
                finalFolder = System.IO.Directory.CreateDirectory(finalFolderString);

            System.Windows.Forms.DialogResult backupResult = _progressDialog.StartProgressDialog(_generalResourceManager.GetString("BackupProgressTitle"), _generalResourceManager.GetString("BackupProgressText"), _source.SourceSize.Value, bg_BackUpWithoutDatabase_DoWork, finalFolder);

            if (backupResult != System.Windows.Forms.DialogResult.Cancel)
                return true;
            else
                return false;
        }

        private void bg_BackUpWithoutDatabase_DoWork(object WorkArg)
        {
            System.IO.DirectoryInfo finalFolder = (System.IO.DirectoryInfo)WorkArg;
            System.IO.DirectoryInfo originalFolder = new System.IO.DirectoryInfo(_source.SourcePath);
            Utils.RecursivelyCopyFolders(originalFolder, finalFolder, _progressDialog);
        }

        private List<string> ExpiredFolders
        {
            get
            {
                if (_expiredFolders == null && !_useDatabase)
                {
                    List<string> expiredFolders = new List<string>();

                    Dictionary<DateTime, string> candidateFolders = GetDateFolders(new System.IO.DirectoryInfo(FullBackupLocation));

                    if (_cleaningOptions.FinalCutOffDays > 0)
                    {
                        TrackPathsToDeleteByExpiry(ref candidateFolders,
                            _cleaningOptions.FinalCutOffDays,
                            ref expiredFolders);
                    }

                    if (_cleaningOptions.Period1Interval > 0 && _cleaningOptions.Period1Start > 0)
                    {
                        TrackPathsToDeleteByPeriod(ref candidateFolders,
                            _cleaningOptions.Period1Interval,
                            _cleaningOptions.Period1Start,
                            ref expiredFolders);
                    }

                    if (_cleaningOptions.Period2Interval > 0 && _cleaningOptions.Period2Start > 0)
                    {
                        TrackPathsToDeleteByPeriod(ref candidateFolders,
                            _cleaningOptions.Period2Interval,
                            _cleaningOptions.Period2Start,
                            ref expiredFolders);
                    }

                    if (_cleaningOptions.Period3Interval > 0 && _cleaningOptions.Period3Start > 0)
                    {
                        TrackPathsToDeleteByPeriod(ref candidateFolders,
                            _cleaningOptions.Period3Interval,
                            _cleaningOptions.Period3Start,
                            ref expiredFolders);
                    }

                    _expiredFolders = expiredFolders;
                }
                return _expiredFolders;
            }
        }

        private List<int> ExpiredDBBackups
        {
            get
            {
                if (_expiredDBBackups == null)
                {
                    List<int> expiredDBBackups = new List<int>();
                    List<BackupInfo> candidateDBBackups = LoadedBackupDB.GetBackups();

                    if (_cleaningOptions.FinalCutOffDays > 0)
                    {
                        TrackDBBackupsToDeleteByExpiry(ref candidateDBBackups,
                            _cleaningOptions.FinalCutOffDays,
                            ref expiredDBBackups);
                    }

                    if (_cleaningOptions.Period1Interval > 0 && _cleaningOptions.Period1Start > 0)
                    {
                        TrackDBBackupsToDeleteByPeriod(ref candidateDBBackups,
                            _cleaningOptions.Period1Interval,
                            _cleaningOptions.Period1Start,
                            ref expiredDBBackups);
                    }

                    if (_cleaningOptions.Period2Interval > 0 && _cleaningOptions.Period2Start > 0)
                    {
                        TrackDBBackupsToDeleteByPeriod(ref candidateDBBackups,
                            _cleaningOptions.Period2Interval,
                            _cleaningOptions.Period2Start,
                            ref expiredDBBackups);
                    }

                    if (_cleaningOptions.Period3Interval > 0 && _cleaningOptions.Period3Start > 0)
                    {
                        TrackDBBackupsToDeleteByPeriod(ref candidateDBBackups,
                            _cleaningOptions.Period3Interval,
                            _cleaningOptions.Period3Start,
                            ref expiredDBBackups);
                    }

                    _expiredDBBackups = expiredDBBackups;
                }
                return _expiredDBBackups;
            }
        }

        public int ExpiredBackupCount
        {
            get
            {
                if (_useDatabase)
                {
                    return ExpiredDBBackups.Count;
                }
                else
                    return ExpiredFolders.Count;
            }
        }

        private Dictionary<DateTime, string> GetDateFolders(System.IO.DirectoryInfo directoryInfo)
        {
            Dictionary<DateTime, string> outList = new Dictionary<DateTime, string>();
            foreach (System.IO.DirectoryInfo possibleDateFolder in directoryInfo.GetDirectories())
            {
                DateTime possibleDate;
                if (DateTime.TryParse(possibleDateFolder.Name, out possibleDate))
                    outList.Add(possibleDate, possibleDateFolder.FullName);

            }
            return outList;
        }

        private static void TrackPathsToDeleteByExpiry(ref Dictionary<DateTime, string> candidateFolders, int finalCutOffDays, ref List<string> foldersToDelete)
        {
            DateTime[] keyDates = new DateTime[candidateFolders.Keys.Count];
            candidateFolders.Keys.CopyTo(keyDates, 0);
            Array.Sort(keyDates);

            foreach (DateTime folderDate in keyDates)
            {
                if ((DateTime.Now - folderDate).TotalDays > finalCutOffDays)
                {
                    foldersToDelete.Add(candidateFolders[folderDate]);
                    candidateFolders.Remove(folderDate);
                }
                else
                {
                    break;
                }
            }
        }

        private static void TrackPathsToDeleteByPeriod(ref Dictionary<DateTime, string> candidateFolders, int periodInterval, int periodStart, ref List<string> foldersToDelete)
        {

            DateTime[] keyDates = new DateTime[candidateFolders.Keys.Count];
            candidateFolders.Keys.CopyTo(keyDates, 0);
            Array.Sort(keyDates);

            double? previousRetainedDaysAge = null;

            foreach (DateTime folderDate in keyDates)
            {
                double daysAge = (DateTime.Now - folderDate).TotalDays;
                if (daysAge > periodStart)
                {
                    if (previousRetainedDaysAge != null && daysAge + periodInterval > previousRetainedDaysAge)
                    {
                        foldersToDelete.Add(candidateFolders[folderDate]);
                        candidateFolders.Remove(folderDate);
                    }
                    else
                    {
                        previousRetainedDaysAge = daysAge;
                    }
                }
                else
                {
                    break;
                }
            }
        }


        private static void TrackDBBackupsToDeleteByExpiry(ref List<BackupInfo> candidateDBBackups, int finalCutOffDays, ref List<int> backupsToDelete)
        {
            candidateDBBackups.Sort(delegate(BackupInfo bi1, BackupInfo bi2) { return bi1.BackupDate.CompareTo(bi2.BackupDate); });

            BackupInfo[] dupeList = candidateDBBackups.ToArray(); //have to use dupe array to be able to change the List
            foreach(BackupInfo backup in dupeList)
            {
                if ((DateTime.Now - backup.BackupDate).TotalDays > finalCutOffDays)
                {
                    backupsToDelete.Add(backup.EntryID);
                    candidateDBBackups.Remove(backup);
                }
                else
                {
                    break;
                }
            }
        }

        private static void TrackDBBackupsToDeleteByPeriod(ref List<BackupInfo> candidateDBBackups, int periodInterval, int periodStart, ref List<int> backupsToDelete)
        {
            double? previousRetainedDaysAge = null;

            candidateDBBackups.Sort(delegate(BackupInfo bi1, BackupInfo bi2) { return bi1.BackupDate.CompareTo(bi2.BackupDate); });

            BackupInfo[] dupeList = candidateDBBackups.ToArray(); //have to use dupe array to be able to change the List
            foreach(BackupInfo backup in dupeList)
            {
                double daysAge = (DateTime.Now - backup.BackupDate).TotalDays;
                if (daysAge > periodStart)
                {
                    if (previousRetainedDaysAge != null && daysAge + periodInterval > previousRetainedDaysAge)
                    {
                        backupsToDelete.Add(backup.EntryID);
                        candidateDBBackups.Remove(backup);
                    }
                    else
                    {
                        previousRetainedDaysAge = daysAge;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        public bool DeleteExpiredBackups()
        {
            if (_useDatabase)
            {
                foreach (int backupToDelete in _expiredDBBackups)
                {
                    LoadedBackupDB.DeleteBackup(backupToDelete);
                }
                foreach (int orphanFileEntry in LoadedBackupDB.GetOrphanFiles())
                {
                    string dbBackupFilePath = LoadedBackupDB.GetBackupFilePathByEntryID(orphanFileEntry);
                    
                    if (File.Exists(Path.Combine(FullBackupLocationDBDataPath, dbBackupFilePath)))
                        File.Delete(Path.Combine(FullBackupLocationDBDataPath, dbBackupFilePath));

                    LoadedBackupDB.DeleteFile(orphanFileEntry);
                }
                LoadedBackupDB.WriteDatabaseToFile();
                _expiredDBBackups = null;
                return true; //would come in handy if we ever added a progress dialog.
            }
            else
            {
                foreach (string folderToDelete in _expiredFolders)
                {
                    Directory.Delete(folderToDelete, true);
                }
                _expiredFolders = null;
                return true; //would come in handy if we ever added a progress dialog.
            }
        }

        internal RestoreManager GetRestoreManager()
        {
            return GetRestoreManager(ExistingBackupLocationEntryID.Value);
        }

        internal RestoreManager GetRestoreManager(int BackupLocationID)
        {
            return new RestoreManager(_backupLocationDB.GetLocation(BackupLocationID).BackupPath, _backupLocation, _password, _generalResourceManager, _progressDialog);
        }

        internal IEnumerable<BackupLocationInfo> GetMatchingBackupLocations()
        {
            List<BackupLocationInfo> matchingLocations = new List<BackupLocationInfo>();

            foreach (BackupLocationInfo locationInfo in LoadedBackupLocationDB.GetAllLocations())
            {
                if (string.IsNullOrEmpty(_password))
                {
                    if (string.IsNullOrEmpty(locationInfo.EncryptionCheckValue))
                    {
                        matchingLocations.Add(locationInfo);
                    }
                }
                else
                {
                    try
                    {
                        string previousCheckValue = Utils.DecryptString(locationInfo.EncryptionCheckValue, _password);
                        if (previousCheckValue.Equals(_passwordCheckPhrase))
                        {
                            matchingLocations.Add(locationInfo);
                        }
                    }
                    catch (Exception e)
                    {
                        //decryption failed, so we didn't find what we wanted - that's fine, no further handling.
                    }
                }

            }

            return matchingLocations;
        }
    }



    public struct BackupCleaningOptions
    {
        public int FinalCutOffDays { get; set; }
        public int Period1Interval { get; set; }
        public int Period1Start { get; set; }
        public int Period2Interval { get; set; }
        public int Period2Start { get; set; }
        public int Period3Interval { get; set; }
        public int Period3Start { get; set; }
    }
}
