using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace KlerksSoft.UsbKeyBackup
{
    internal class RestoreManager
    {
        private KlerksSoft.EasyProgressDialog.ProgressDialog _progressDialog;
        private SingleAssemblyResourceManager _generalResourceManager;
        private string _password;
        private string _backupLocation;
        private BackupDatabase _backupDB;
        private string _selectedBackupSourcePath;


        public RestoreManager(string sourceBackupPath, string destinationPath, string password, SingleAssemblyResourceManager generalResourceManager)
            : this(sourceBackupPath, destinationPath, password, generalResourceManager, null) { }

        public RestoreManager(string sourceBackupPath, string destinationPath, string password, SingleAssemblyResourceManager generalResourceManager, KlerksSoft.EasyProgressDialog.ProgressDialog progressDialog)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("password must not be empty", "password");

            _selectedBackupSourcePath = sourceBackupPath;
            _backupLocation = destinationPath;
            _password = password;
            _progressDialog = progressDialog;
            _generalResourceManager = generalResourceManager;
        }

        public string FullBackupLocation
        {
            get
            {
                return Path.Combine(_backupLocation, _selectedBackupSourcePath);
                //TODO: clean up...
            }
        }

        public string FullBackupLocationDBDataPath
        {
            get
            {
                return System.IO.Path.Combine(FullBackupLocation, "FileData");
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

        internal IEnumerable<BackupInfo> GetBackups()
        {
            if (LoadedBackupDB != null)
            {
                foreach (BackupInfo backupInfo in LoadedBackupDB.GetBackups())
                    yield return backupInfo;
            }
        }

        internal List<BackupPathEntry> GetBackupPathEntries(int backupID, string selectedBackupPath)
        {
            List<BackupPathEntry> relevantEntries = new List<BackupPathEntry>();
            foreach (BackupFileInfo fileEntry in _backupDB.GetBackupFiles(backupID))
            {
                if (string.IsNullOrEmpty(selectedBackupPath) || fileEntry.FileFullPath.StartsWith(selectedBackupPath + Path.DirectorySeparatorChar))
                {
                    string remainingPath;
                    if (string.IsNullOrEmpty(selectedBackupPath))
                        remainingPath = fileEntry.FileFullPath;
                    else
                        remainingPath = fileEntry.FileFullPath.Replace(selectedBackupPath + Path.DirectorySeparatorChar, "");

                    string[] remainingPathParts = remainingPath.Split(Path.DirectorySeparatorChar);

                    BackupPathEntry pathEntry = new BackupPathEntry();

                    if (remainingPathParts.Length > 1)
                        pathEntry.IsDirectory = true;
                    else
                        pathEntry.IsDirectory = false;

                    pathEntry.Name = remainingPathParts[0];

                    bool alreadyExists = false;
                    foreach (BackupPathEntry otherEntry in relevantEntries)
                        if (otherEntry.Name.Equals(pathEntry.Name))
                            alreadyExists = true;

                    if (!alreadyExists)
                        relevantEntries.Add(pathEntry);
                }
            }
            return relevantEntries;
        }


        internal bool RestoreFilesToDestination(Dictionary<string, string> restoreList, string destinationPath)
        {
            KeyValuePair<string, Dictionary<string, string>> args = new KeyValuePair<string, Dictionary<string, string>>(destinationPath, restoreList);

            DialogResult whatHappened = _progressDialog.StartProgressDialog(_generalResourceManager.GetString("RestoringFilesProgressTitle"), _generalResourceManager.GetString("RestoringFilesProgressText"), restoreList.Count, bg_RestoreFilesToDestination_DoWork, args);

            if (whatHappened != DialogResult.Cancel)
                return true;
            else
                return false;
        }

        private void bg_RestoreFilesToDestination_DoWork(object WorkArg)
        {
            KeyValuePair<string, Dictionary<string, string>> args = (KeyValuePair<string, Dictionary<string, string>>)WorkArg;
            string destinationPath = args.Key;
            Dictionary<string, string> restoreList = args.Value;

            foreach (string restoreFile in restoreList.Keys)
            {
                if (restoreFile.EndsWith(".encrypted"))
                {
                    Utils.DecryptFile(restoreFile, Path.Combine(destinationPath, restoreList[restoreFile]), _password);
                }
                else
                    File.Copy(restoreFile, Path.Combine(destinationPath, restoreList[restoreFile]));

                if (_progressDialog != null && !_progressDialog.Worker_IncrementProgress()) return;
            }
        }


        internal Dictionary<string, string> GetRestoreFilesByBackupID(int backupID)
        {
            BackupInfo backup = _backupDB.GetBackup(backupID);
            Dictionary<string, string> restoreFileList = new Dictionary<string, string>();
            foreach (BackupFileInfo fileInfo in _backupDB.GetBackupFiles(backupID))
            {
                restoreFileList.Add(Path.Combine(FullBackupLocationDBDataPath, fileInfo.BackupFilePath), Path.Combine(Utils.SortableStringFromDate(backup.BackupDate), fileInfo.FileFullPath));
            }
            return restoreFileList;
        }

        internal Dictionary<string, string> GetRestoreFilesByBackupIDAndPartialPath(int backupID, string partialPath)
        {
            Dictionary<string, string> restoreFileList = new Dictionary<string, string>();
            foreach (BackupFileInfo fileInfo in _backupDB.GetBackupFiles(backupID))
            {
                if (fileInfo.FileFullPath.StartsWith(partialPath))
                {
                    string outFileName;

                    if (fileInfo.FileFullPath.Equals(partialPath))
                    {
                        outFileName = Path.GetFileName(partialPath);
                    }
                    else
                    {
                        string[] pathParts = partialPath.Split(Path.DirectorySeparatorChar);
                        if (pathParts.Length > 1)
                        {
                            string PartsBeforeLastPartOfPartialString = string.Join(Path.DirectorySeparatorChar.ToString(), pathParts, 0, pathParts.Length - 1);
                            outFileName = fileInfo.FileFullPath.Substring(PartsBeforeLastPartOfPartialString.Length + 1);
                        }
                        else
                        {
                            outFileName = fileInfo.FileFullPath;
                        }
                    }

                    restoreFileList.Add(Path.Combine(FullBackupLocationDBDataPath, fileInfo.BackupFilePath), outFileName);
                }
            }
            return restoreFileList;
        }

    }
}
