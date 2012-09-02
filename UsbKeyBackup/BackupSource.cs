using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace KlerksSoft.UsbKeyBackup
{
    public class BackupSource
    {
        private KlerksSoft.EasyProgressDialog.ProgressDialog _progressDialog;
        private SingleAssemblyResourceManager _generalResourceManager;
        private string _sourcePath;
        private string _backupLabel;
        private long? _sourceSize;

        public BackupSource(string sourcePath, SingleAssemblyResourceManager resourceManager)
            : this(sourcePath, resourceManager, null) { }

        public BackupSource(string sourcePath, SingleAssemblyResourceManager resourceManager, KlerksSoft.EasyProgressDialog.ProgressDialog progressDialog)
        {
            string expandedSourcePath = Path.GetFullPath(sourcePath);
            if (!Directory.Exists(expandedSourcePath))
                throw new ArgumentException("Path to back up must exist!", "sourcePath");

            _sourcePath = expandedSourcePath;
            _generalResourceManager = resourceManager;
            _progressDialog = progressDialog;
        }

        public string SourcePath
        {
            get
            {
                return _sourcePath;
            }
        }

        public string BackupLabel
        {
            get
            {
                if (_backupLabel == null)
                {
                    string sourceRoot = System.IO.Directory.GetDirectoryRoot(_sourcePath);
                    System.IO.DriveInfo backupDrive = new System.IO.DriveInfo(sourceRoot);
                    string partialLabel = backupDrive.VolumeLabel + "_" + Utils.GetVolumeSerial(backupDrive.Name);
                    if (!String.Equals(_sourcePath, sourceRoot))
                        partialLabel += "_" + _sourcePath.Replace(sourceRoot, "").Replace(System.IO.Path.DirectorySeparatorChar, '_');

                    _backupLabel = partialLabel;
                }
                return _backupLabel;
            }
        }

        public string StripSourceFromPath(string path)
        {
            string matchString;
            if (SourcePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                matchString = SourcePath;
            }
            else
            {
                matchString = SourcePath + Path.DirectorySeparatorChar.ToString();
            }

            if (path.StartsWith(matchString))
            {
                return path.Substring(matchString.Length);
            }
            else
            {
                throw new Exception("The provided path does not belong in this source!");
            }
        }

        public long? SourceSize
        {
            get
            {
                if (_sourceSize == null)
                {
                    //ignore the case where the user cancels - this will result in the value "null", which will be handled by the calling code.
                    _progressDialog.StartProgressDialog(_generalResourceManager.GetString("SizeEstimateProgressTitle"), _generalResourceManager.GetString("SizeEstimateProgressText"), 100, bg_CalcBAckupFolderSize_DoWork, null);
                }
                return _sourceSize;
            }
        }

        private void bg_CalcBAckupFolderSize_DoWork(object WorkArg)
        {
            _sourceSize = Utils.GetFolderSizeRecursive(new System.IO.DirectoryInfo(_sourcePath), _progressDialog);
        }

    }
}
