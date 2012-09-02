using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;

namespace KlerksSoft.UsbKeyBackup
{
    internal class BackupDatabase
    {
        private const string _backupDBFileName = "DBFile.xml";
        private string _backupDBPath;
        private DataSet _DB;
        private DataTable _Files;
        private DataTable _Backups;
        private DataTable _BackupFiles;
        private DataView _FilesByFileNameAndSize;
        private DataRelation _filesRelation;
        private DataRelation _backupsRelation;

        internal BackupDatabase(string backupDBPath)
        {
            if (!Directory.Exists(backupDBPath))
                throw new ArgumentException("Path does not exist!", "backupDBPath");

            _backupDBPath = backupDBPath;

            _DB = new DataSet();
            _DB.DataSetName = "BackupDatabase";
            _DB.Tables.Add("Files");
            _DB.Tables.Add("Backups");
            _DB.Tables.Add("BackupFiles");

            _Files = _DB.Tables["Files"];

            DataColumn autoFileNumberer = new DataColumn("FileEntryID");
            autoFileNumberer.DataType = Type.GetType("System.Int32");
            autoFileNumberer.AutoIncrement = true;
            autoFileNumberer.AutoIncrementSeed = 1;
            autoFileNumberer.AutoIncrementStep = 1;
            autoFileNumberer.ReadOnly = true;
            autoFileNumberer.AllowDBNull = false;
            _Files.Columns.Add(autoFileNumberer);

            _Files.Columns.Add(new DataColumn("FileFullPath"));
            _Files.Columns.Add(new DataColumn("FileDate", Type.GetType("System.DateTime")));
            _Files.Columns.Add(new DataColumn("FileSizeBytes", Type.GetType("System.Int64")));
            _Files.Columns.Add(new DataColumn("FileSHA1"));
            _Files.Columns.Add(new DataColumn("BackupFilePath"));

            _Files.Constraints.Add("PK_FileEntryID", autoFileNumberer, true);


            _Backups = _DB.Tables["Backups"];

            DataColumn autoBackupNumberer = new DataColumn("BackupEntryID");
            autoBackupNumberer.DataType = Type.GetType("System.Int32");
            autoBackupNumberer.AutoIncrement = true;
            autoBackupNumberer.AutoIncrementSeed = 1;
            autoBackupNumberer.AutoIncrementStep = 1;
            autoBackupNumberer.ReadOnly = true;
            autoBackupNumberer.AllowDBNull = false;
            _Backups.Columns.Add(autoBackupNumberer);

            _Backups.Columns.Add(new DataColumn("BackupDate", Type.GetType("System.DateTime")));
            _Backups.Columns.Add(new DataColumn("BackupComplete", Type.GetType("System.Boolean")));

            _Backups.Constraints.Add("PK_BackupEntryID", autoBackupNumberer, true);


            _BackupFiles = _DB.Tables["BackupFiles"];

            DataColumn fileNumbererChild = new DataColumn("FileEntryID", Type.GetType("System.Int32"));
            _BackupFiles.Columns.Add(fileNumbererChild);

            DataColumn backupNumbererChild = new DataColumn("BackupEntryID", Type.GetType("System.Int32"));
            _BackupFiles.Columns.Add(backupNumbererChild);


            _filesRelation = new DataRelation("FilesToBackupFiles", autoFileNumberer, fileNumbererChild, true);
            _filesRelation.Nested = false; //it can only be nested once, better the other way.
            _DB.Relations.Add(_filesRelation);
            _filesRelation.ChildKeyConstraint.UpdateRule = Rule.Cascade;

            _backupsRelation = new DataRelation("BackupsToBackupFiles", autoBackupNumberer, backupNumbererChild, true);
            _backupsRelation.Nested = true;
            _DB.Relations.Add(_backupsRelation);
            _backupsRelation.ChildKeyConstraint.UpdateRule = Rule.Cascade;

            _FilesByFileNameAndSize = new DataView(_Files, "FileFullPath <> ''", "FileFullPath, FileSizeBytes", DataViewRowState.CurrentRows);
        }

        internal bool DatabaseFileExists
        {
            get
            {
                return File.Exists(Path.Combine(_backupDBPath, _backupDBFileName));
            }
        }

        internal void ReadDatabaseFromFile()
        {
            _BackupFiles.Clear();
            _Files.Clear();
            _Backups.Clear();
            _DB.ReadXml(Path.Combine(_backupDBPath, _backupDBFileName));
        }

        internal void WriteDatabaseToFile()
        {
            if (File.Exists(Path.Combine(_backupDBPath, _backupDBFileName)))
                File.SetAttributes(Path.Combine(_backupDBPath, _backupDBFileName), System.IO.FileAttributes.Normal);
            _DB.WriteXml(Path.Combine(_backupDBPath, _backupDBFileName));
            File.SetAttributes(Path.Combine(_backupDBPath, _backupDBFileName), FileAttributes.Hidden);
        }

        internal int StartBackup()
        {
            DataRow newBackupRow = _Backups.NewRow();
            newBackupRow["BackupDate"] = DateTime.Now;
            newBackupRow["BackupComplete"] = false;
            _Backups.Rows.Add(newBackupRow);
            return (int)newBackupRow["BackupEntryID"];
        }

        internal void CompleteBackup(int backupID)
        {
            DataRow matchingRow = _Backups.Rows.Find(backupID);
            matchingRow["BackupComplete"] = true;
            matchingRow.AcceptChanges();
        }

        internal int? GetFileRecordIDByProperties(string FileFullPath, DateTime FileDate, long FileSizeBytes)
        {
            foreach (DataRowView thisRowView in _FilesByFileNameAndSize.FindRows(new object[] {FileFullPath, FileSizeBytes}))
            {
                DateTime dbFileDate = (DateTime)thisRowView.Row["FileDate"];
                if ((dbFileDate - FileDate).TotalSeconds < 5
                    && (FileDate - dbFileDate).TotalSeconds < 5)
                    return (int)thisRowView.Row["FileEntryID"];
            }
            return null;
        }

        internal int AddFileRecord(string filePath, DateTime fileDate, long fileSize)
        {
            DataRow newFileDataRow = _Files.NewRow();
            newFileDataRow["FileFullPath"] = filePath;
            newFileDataRow["FileDate"] = fileDate;
            newFileDataRow["FileSizeBytes"] = fileSize;
            _Files.Rows.Add(newFileDataRow);
            return (int)newFileDataRow["FileEntryID"];
        }

        internal string GetBackupFilePathByEntryID(int fileEntryID)
        {
            DataRow matchingRow = _Files.Rows.Find(fileEntryID);
            return matchingRow["BackupFilePath"].ToString();
        }

        internal void UpdateDBDataFileName(int fileEntryID, string physicalFileName, string SHA1)
        {
            DataRow matchingRow = _Files.Rows.Find(fileEntryID);
            matchingRow["BackupFilePath"] = physicalFileName;
            matchingRow["FileSHA1"] = SHA1;
            matchingRow.AcceptChanges();
        }

        internal void AddBackupFileRecord(int backupID, int existingFileID)
        {
            DataRow newBackupFileRecord = _BackupFiles.NewRow();
            newBackupFileRecord["BackupEntryID"] = backupID;
            newBackupFileRecord["FileEntryID"] = existingFileID;
            _BackupFiles.Rows.Add(newBackupFileRecord);
        }

        internal List<BackupInfo> GetBackups()
        {
            List<BackupInfo> outList = new List<BackupInfo>();
            foreach (DataRow outRow in _Backups.Select("BackupComplete = 1"))
            {
                BackupInfo outBackup = new BackupInfo();
                outBackup.EntryID = (int)outRow["BackupEntryID"];
                outBackup.BackupDate = (DateTime)outRow["BackupDate"];
                outBackup.BackupComplete = (bool)outRow["BackupComplete"];
                outList.Add(outBackup);
            }
            return outList;
        }

        internal void DeleteBackup(int backupID)
        {
            DataRow matchingRow = _Backups.Rows.Find(backupID);
            _Backups.Rows.Remove(matchingRow);
        }

        internal void DeleteFile(int orphanFileEntryID)
        {
            DataRow matchingRow = _Files.Rows.Find(orphanFileEntryID);
            _Files.Rows.Remove(matchingRow);
        }

        internal List<int> GetOrphanFiles()
        {
            List<int> outList = new List<int>();
            foreach (DataRow fileEntry in _Files.Rows)
            {
                if (fileEntry.GetChildRows(_filesRelation).Length == 0)
                    outList.Add((int)fileEntry["FileEntryID"]);
            }
            return outList;
        }

        internal IEnumerable<BackupFileInfo> GetBackupFiles(int backupID)
        {
            DataRow backupRow = _Backups.Rows.Find(backupID);

            foreach (DataRow backupFileRow in backupRow.GetChildRows(_backupsRelation))
            {
                DataRow fileRow = backupFileRow.GetParentRow(_filesRelation);
                BackupFileInfo backupFile = new BackupFileInfo();
                backupFile.FileEntryID = (int)fileRow["FileEntryID"];
                backupFile.FileFullPath = (string)fileRow["FileFullPath"];
                backupFile.FileDate = (DateTime)fileRow["FileDate"];
                backupFile.FileSizeBytes = (long)fileRow["FileSizeBytes"];
                backupFile.FileSHA1 = (string)fileRow["FileSHA1"];
                backupFile.BackupFilePath = (string)fileRow["BackupFilePath"];
                yield return backupFile;
            }
        }

        internal BackupInfo GetBackup(int backupID)
        {
            DataRow outRow = _Backups.Rows.Find(backupID);
            BackupInfo outBackup = new BackupInfo();
            outBackup.EntryID = (int)outRow["BackupEntryID"];
            outBackup.BackupDate = (DateTime)outRow["BackupDate"];
            outBackup.BackupComplete = (bool)outRow["BackupComplete"];
            return outBackup;
        }

        internal BackupInfo GetLatestBackup()
        {
            DateTime? latestSoFarDate = null;
            DataRow latestSoFarRow = null;

            foreach (DataRow outRow in _Backups.Rows)
            {
                if (latestSoFarDate == null || (DateTime)outRow["BackupDate"] > latestSoFarDate.Value)
                {
                    latestSoFarDate = (DateTime)outRow["BackupDate"];
                    latestSoFarRow = outRow;
                }
            }

            BackupInfo outBackup = null;
            if (latestSoFarRow != null)
            {
                outBackup = new BackupInfo();
                outBackup.EntryID = (int)latestSoFarRow["BackupEntryID"];
                outBackup.BackupDate = (DateTime)latestSoFarRow["BackupDate"];
                outBackup.BackupComplete = (bool)latestSoFarRow["BackupComplete"];
            }
            return outBackup;
        }
    }
}
