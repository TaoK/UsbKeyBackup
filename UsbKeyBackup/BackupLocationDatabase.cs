using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;

namespace KlerksSoft.UsbKeyBackup
{
    internal class BackupLocationDatabase
    {
        private const string _DBFileName = "LocationsDBFile.xml";
        private string _DBPath;
        private DataSet _DB;
        private DataTable _BackupLocations;

        internal BackupLocationDatabase(string DBPath)
        {
            if (!Directory.Exists(DBPath))
                throw new ArgumentException("Path does not exist!", "DBPath");

            _DBPath = DBPath;

            _DB = new DataSet();
            _DB.DataSetName = "BackupLocationDatabase";
            _DB.Tables.Add("BackupLocations");
            _BackupLocations = _DB.Tables["BackupLocations"];

            DataColumn autoLocationNumberer = new DataColumn("LocationEntryID");
            autoLocationNumberer.DataType = Type.GetType("System.Int32");
            autoLocationNumberer.AutoIncrement = true;
            autoLocationNumberer.AutoIncrementSeed = 1;
            autoLocationNumberer.AutoIncrementStep = 1;
            autoLocationNumberer.ReadOnly = true;
            autoLocationNumberer.AllowDBNull = false;
            _BackupLocations.Columns.Add(autoLocationNumberer);

            _BackupLocations.Columns.Add(new DataColumn("SourceLabel"));
            _BackupLocations.Columns.Add(new DataColumn("EncryptionCheckValue"));
            _BackupLocations.Columns.Add(new DataColumn("BackupPath"));

            _BackupLocations.Constraints.Add("PK_LocationEntryID", autoLocationNumberer, true);


        }

        internal bool DatabaseFileExists
        {
            get
            {
                return File.Exists(Path.Combine(_DBPath, _DBFileName));
            }
        }

        internal void ReadDatabaseFromFile()
        {
            _BackupLocations.Clear();
            _DB.ReadXml(Path.Combine(_DBPath, _DBFileName));
        }

        internal void WriteDatabaseToFile()
        {
            if (File.Exists(Path.Combine(_DBPath, _DBFileName)))
                File.SetAttributes(Path.Combine(_DBPath, _DBFileName), System.IO.FileAttributes.Normal);
            _DB.WriteXml(Path.Combine(_DBPath, _DBFileName));
            File.SetAttributes(Path.Combine(_DBPath, _DBFileName), FileAttributes.Hidden);
        }

        internal int AddLocation(string SourceLabel, string EncryptionCheckValue)
        {
            //first figure out what the Path should be.
            string candidatePath = SourceLabel;
            int counter = 0;
            while (_BackupLocations.Select(String.Format("BackupPath = '{0}'", candidatePath.Replace("'", "''"))).Length > 0)
            {
                counter++;
                candidatePath = SourceLabel + "_" + counter.ToString();
            }

            DataRow newLocationRow = _BackupLocations.NewRow();
            newLocationRow["SourceLabel"] = SourceLabel;
            newLocationRow["EncryptionCheckValue"] = EncryptionCheckValue;
            newLocationRow["BackupPath"] = candidatePath;
            _BackupLocations.Rows.Add(newLocationRow);
            return (int)newLocationRow["LocationEntryID"];
        }

        private BackupLocationInfo RowToLocation (DataRow locationRow)
        {
            BackupLocationInfo thisLocation = new BackupLocationInfo();
            thisLocation.LocationEntryID = (int)locationRow["LocationEntryID"];
            thisLocation.SourceLabel = locationRow["SourceLabel"].ToString();
            thisLocation.EncryptionCheckValue = locationRow["EncryptionCheckValue"].ToString();
            thisLocation.BackupPath = locationRow["BackupPath"].ToString();
            return thisLocation;
        }

        internal List<BackupLocationInfo> GetLocationsBySourceLabel(string SourceLabel)
        {
            DataRow[] foundRows = _BackupLocations.Select(String.Format("SourceLabel = '{0}'", SourceLabel.Replace("'", "''")));
            List<BackupLocationInfo> locations = new List<BackupLocationInfo>();
            foreach (DataRow thisRow in foundRows)
                locations.Add(RowToLocation (thisRow));
            return locations;
        }

        internal BackupLocationInfo GetLocation(int LocationEntryID)
        {
            DataRow[] foundRows = _BackupLocations.Select(String.Format("LocationEntryID = {0}", LocationEntryID));
            foreach (DataRow thisRow in foundRows)
                return RowToLocation(thisRow);
            return null;
        }

        internal IEnumerable<BackupLocationInfo> GetAllLocations()
        {
            DataRow[] foundRows = _BackupLocations.Select();
            foreach (DataRow thisRow in foundRows)
                yield return RowToLocation(thisRow);
        }
    }
}
