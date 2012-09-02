using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;

namespace KlerksSoft.UsbKeyBackup
{
    public static class Utils
    {
        public static long? GetFolderSizeRecursive(DirectoryInfo startDirectory, KlerksSoft.EasyProgressDialog.ProgressDialog progressDialog)
        {
            if (progressDialog != null && !progressDialog.Worker_IncrementProgress()) return null;
            long? fileSizes = 0;
            foreach (FileInfo nextFile in startDirectory.GetFiles())
                fileSizes += nextFile.Length;
            foreach (DirectoryInfo nextDirectory in startDirectory.GetDirectories())
                fileSizes += GetFolderSizeRecursive(nextDirectory, progressDialog);
            return fileSizes;
        }

        [DllImport("kernel32.dll")]
        private static extern long GetVolumeInformation(string PathName, StringBuilder VolumeNameBuffer, UInt32 VolumeNameSize, ref UInt32 VolumeSerialNumber, ref UInt32 MaximumComponentLength, ref UInt32 FileSystemFlags, StringBuilder FileSystemNameBuffer, UInt32 FileSystemNameSize);
        // Aapted from Peter A. Bromberg's article: http://www.eggheadcafe.com/articles/20021019.asp
        /// <summary>
        /// Get Volume Serial Number as string
        /// </summary>
        /// <param name="strDriveName">Drive Name (e.g., "C:\")</param>
        /// <returns>string representation of Volume Serial Number</returns>
        public static string GetVolumeSerial(string strDriveName)
        {
            uint serNum = 0;
            uint maxCompLen = 0;
            StringBuilder VolLabel = new StringBuilder(256);	// Label
            UInt32 VolFlags = new UInt32();
            StringBuilder FSName = new StringBuilder(256);	// File System Name
            long Ret = GetVolumeInformation(strDriveName, VolLabel, (UInt32)VolLabel.Capacity, ref serNum, ref maxCompLen, ref VolFlags, FSName, (UInt32)FSName.Capacity);
            return Convert.ToString(serNum);
        }

        public static string FormatSize(long lSize)
        {
            System.Globalization.NumberFormatInfo myNfi = new System.Globalization.NumberFormatInfo();

            if (lSize < 1024)
            {
                return lSize.ToString() + " B";
            }
            else if (lSize < (1024 * 1024))
            {
                return Math.Round(lSize / 1024.0, 2).ToString() + " KB";
            }
            else if (lSize < (1024 * 1024 * 1024))
            {
                lSize = lSize / 1024;
                return Math.Round(lSize / 1024.0, 2).ToString() + " MB";
            }
            else
            {
                lSize = lSize / (1024 * 1024);
                return Math.Round(lSize / 1024.0, 2).ToString() + " GB";
            }
        }

        public static string SortableStringFromDate(DateTime d)
        {
            string ds;
            ds = d.Year.ToString() + "-" + PadIntegerDigits(d.Month, 2) + "-" + PadIntegerDigits(d.Day, 2);
            if (d.TimeOfDay.TotalMilliseconds > 0)
            {
                ds += " " + PadIntegerDigits(d.Hour, 2) + "-" + PadIntegerDigits(d.Minute, 2) + "-" + PadIntegerDigits(d.Second, 2);
            }
            return ds;
        }

        public static string PadIntegerDigits(int number, int digits)
        {
            string ds;
            ds = number.ToString();
            if (ds.Length < digits) ds = ds.PadLeft(digits, '0');
            return ds;
        }

        public static void RecursivelyCopyFolders(DirectoryInfo FromFolder, DirectoryInfo ToFolder, KlerksSoft.EasyProgressDialog.ProgressDialog progressDialog)
        {
            //Simple recursive copy. 
            // - Does not delete additional files from destination
            // - Does not copy files that already exist with similar modified date (within 5 secs)

            foreach (FileInfo sourceFile in FromFolder.GetFiles())
            {
                bool skipFile = false;

                if (File.Exists(Path.Combine(ToFolder.FullName, sourceFile.Name)))
                {
                    FileInfo targetFile = new FileInfo(Path.Combine(ToFolder.FullName, sourceFile.Name));
                    if (targetFile.LastWriteTime.AddSeconds(5) > sourceFile.LastWriteTime
                        && targetFile.LastWriteTime.AddSeconds(-5) < sourceFile.LastWriteTime
                        && targetFile.Length == sourceFile.Length
                        )
                    {
                        skipFile = true;
                    }
                }

                progressDialog.Worker_SetSpecificProgress(null, progressDialog.CurrentCount + sourceFile.Length, null);

                if (!skipFile)
                {
                    sourceFile.CopyTo(Path.Combine(ToFolder.FullName, sourceFile.Name), true);
                }
            }

            foreach (DirectoryInfo sourceFolder in FromFolder.GetDirectories())
            {
                DirectoryInfo targetFolder;

                if (Directory.Exists(Path.Combine(ToFolder.FullName, sourceFolder.Name)))
                    targetFolder = new DirectoryInfo(Path.Combine(ToFolder.FullName, sourceFolder.Name));
                else
                    targetFolder = Directory.CreateDirectory(Path.Combine(ToFolder.FullName, sourceFolder.Name));

                RecursivelyCopyFolders(sourceFolder, targetFolder, progressDialog);
            }
        }

        private static byte[] CreateKeyFromPassword(string password, int bitLength, string salt)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes((password + salt).ToCharArray());

            SHA512Managed SHA512 = new SHA512Managed();
            byte[] passwordHashedBytes = SHA512.ComputeHash(passwordBytes);

            //there MUST be a cleaner way to copy the 1st N values from one array to another...
            byte[] key = new byte[bitLength / 8];
            for (int i = 0; i < (bitLength / 8); i++)
                key[i] = passwordHashedBytes[i];

            return key;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="outputStream"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static string EncryptStream(Stream inputStream, Stream outputStream, string Password)
        {
            int keySize = 128;
            int blockSize = 256;
            int streamBlockSize = 0x1000;
            string salt = "*hw@o1$9kj}az|y83j'nsl_=";

            System.Security.Cryptography.RijndaelManaged rijndaelProvider = new RijndaelManaged();
            rijndaelProvider.KeySize = keySize;
            rijndaelProvider.BlockSize = blockSize;
            rijndaelProvider.Key = CreateKeyFromPassword(Password, keySize, salt);
            rijndaelProvider.GenerateIV();
            rijndaelProvider.Mode = CipherMode.CBC;

            System.Security.Cryptography.SHA1Managed sha1Provider = new SHA1Managed();
            byte[] hashValueBytes = new byte[160 / 8];

            //Write out encryption algorithm
            outputStream.Write(Encoding.ASCII.GetBytes("AESV1     "), 0, 10);
            //key size
            outputStream.Write(System.BitConverter.GetBytes((short)keySize), 0, 2);
            //block size
            outputStream.Write(System.BitConverter.GetBytes((short)blockSize), 0, 2);
            //salt (questionable?)
            outputStream.Write(Encoding.ASCII.GetBytes(salt), 0, 24);

            //IV
            outputStream.Write(rijndaelProvider.IV, 0, blockSize / 8);

            //Original File Size
            long inputLength = inputStream.Length;
            byte[] fileLengthBytes = System.BitConverter.GetBytes(inputLength);
            outputStream.Write(fileLengthBytes, 0, 8);

            //Hash Algorithm
            outputStream.Write(Encoding.ASCII.GetBytes("SHA1V1    "), 0, 10);

            System.Security.Cryptography.CryptoStream outStream = new CryptoStream(outputStream, rijndaelProvider.CreateEncryptor(), CryptoStreamMode.Write);

            byte[] inputBytes = new byte[streamBlockSize];

            long inputPosition = 0;

            while (inputPosition < inputLength)
            {
                if (inputPosition + streamBlockSize < inputLength)
                {
                    inputStream.Read(inputBytes, 0, streamBlockSize);
                    outStream.Write(inputBytes, 0, streamBlockSize);
                    sha1Provider.TransformBlock(inputBytes, 0, streamBlockSize, null, 0); //the "transformed" output is irrelevant for SHA1
                }
                else
                {
                    inputStream.Read(inputBytes, 0, (int)(inputLength - inputPosition));
                    outStream.Write(inputBytes, 0, (int)(inputLength - inputPosition));
                    sha1Provider.TransformBlock(inputBytes, 0, (int)(inputLength - inputPosition), null, 0);  //the "transformed" output is irrelevant for SHA1
                }

                inputPosition = inputPosition + streamBlockSize;
            }

            //Hash value - in the encrypted content (so no information leakage from hash)
            sha1Provider.TransformFinalBlock(inputBytes, 0, 0);
            hashValueBytes = sha1Provider.Hash;
            outStream.Write(hashValueBytes, 0, 160 / 8);
            outStream.FlushFinalBlock();

            outStream.Close();
            outStream.Dispose();

            return BitConverter.ToString(hashValueBytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FromFile"></param>
        /// <param name="ToFile"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static string EncryptFile(string FromFile, string ToFile, string Password)
        {
            FileStream inputData = File.OpenRead(FromFile);
            FileStream outputStreamEnc = File.Create(ToFile);
            string outHash;

            //TODO: ultimately write the temp file to a different location and then copy, like ZIP programs - but would need to consider temp space!!

            try
            {
                outHash = EncryptStream(inputData, outputStreamEnc, Password);
                inputData.Close();
                outputStreamEnc.Close();
            }
            finally
            {
                inputData.Dispose();
                outputStreamEnc.Dispose();
            }

            return outHash;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static string EncryptString(string Data, string Password)
        {
            MemoryStream inputData = new MemoryStream(Encoding.UTF8.GetBytes(Data));
            MemoryStream outputData = new MemoryStream();
            EncryptStream(inputData, outputData, Password);
            inputData.Close();
            outputData.Close();
            return System.Convert.ToBase64String(outputData.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="outputStream"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        /// <exception></exception>
        public static string DecryptStream(Stream inputStream, Stream outputStream, string Password)
        {
            byte[] encryptionAlgorithmBytes = new byte[10];
            inputStream.Read(encryptionAlgorithmBytes, 0, 10);
            if (!"AESV1     ".Equals(Encoding.ASCII.GetString(encryptionAlgorithmBytes)))
            {
                throw new Exception("The encryption algorithm used to encrypt this file is not supported.");
            }

            byte[] keySizeBytes = new byte[2];
            inputStream.Read(keySizeBytes, 0, 2);
            int keySize = System.BitConverter.ToInt16(keySizeBytes, 0);

            byte[] blockSizeBytes = new byte[2];
            inputStream.Read(blockSizeBytes, 0, 2);
            int blockSize = System.BitConverter.ToInt16(blockSizeBytes, 0);

            byte[] saltBytes = new byte[24];
            inputStream.Read(saltBytes, 0, 24);
            string salt = Encoding.ASCII.GetString(saltBytes);

            byte[] initVectorBytes = new byte[blockSize / 8];
            inputStream.Read(initVectorBytes, 0, blockSize / 8);

            byte[] fileLengthReportedBytes = new byte[8];
            inputStream.Read(fileLengthReportedBytes, 0, 8);
            long fileLengthReported = System.BitConverter.ToInt64(fileLengthReportedBytes, 0);

            byte[] hashAlgorithmBytes = new byte[10];
            inputStream.Read(hashAlgorithmBytes, 0, 10);
            if (!"SHA1V1    ".Equals(Encoding.ASCII.GetString(hashAlgorithmBytes)))
            {
                throw new Exception("The hash algorithm used to sign this file is not supported.");
            }

            long dataFileLength = inputStream.Length - 56 - (blockSize / 8);

            int streamBlockSize = 0x1000;

            System.Security.Cryptography.RijndaelManaged rijndaelProvider = new RijndaelManaged();
            rijndaelProvider.KeySize = keySize;
            rijndaelProvider.BlockSize = blockSize;
            rijndaelProvider.Key = CreateKeyFromPassword(Password, keySize, salt);
            rijndaelProvider.IV = initVectorBytes;
            rijndaelProvider.Mode = CipherMode.CBC;

            System.Security.Cryptography.SHA1Managed sha1Provider = new SHA1Managed();
            byte[] hashValueBytes = new byte[160 / 8];
            byte[] reportedHashValueBytes = new byte[160 / 8];

            System.Security.Cryptography.CryptoStream cryptoStream = new CryptoStream(inputStream, rijndaelProvider.CreateDecryptor(), CryptoStreamMode.Read);

            byte[] outputBytes = new byte[streamBlockSize];
            long inputPosition = 0;

            while (inputPosition < dataFileLength) //use data file length rather than self-reported length - in case someone messed with it and tries to get some sort of overflow error going.
            {
                cryptoStream.Read(outputBytes, 0, streamBlockSize);

                if (inputPosition + streamBlockSize < fileLengthReported)
                {
                    outputStream.Write(outputBytes, 0, streamBlockSize);
                    sha1Provider.TransformBlock(outputBytes, 0, streamBlockSize, null, 0); //the "transformed" output is irrelevant for SHA1
                }
                else
                {
                    long remainingDataSize = (fileLengthReported - inputPosition);

                    if (remainingDataSize > 0)
                    {
                        outputStream.Write(outputBytes, 0, (int)remainingDataSize);
                        sha1Provider.TransformBlock(outputBytes, 0, (int)remainingDataSize, null, 0); //the "transformed" output is irrelevant for SHA1

                        //get what we can of the original/reported hash, out of that last file data block.
                        for (int i = 0; (i + remainingDataSize < streamBlockSize) && (i < reportedHashValueBytes.Length); i++)
                        {
                            reportedHashValueBytes[i] = outputBytes[i + remainingDataSize];
                        }
                    }
                    else if (remainingDataSize + reportedHashValueBytes.Length > 0)
                    {
                        //if spillover into the next block, get the remainder of the original/reported hash.
                        for (int i = 0; (i < streamBlockSize) && (i - remainingDataSize < reportedHashValueBytes.Length); i++)
                        {
                            reportedHashValueBytes[i - remainingDataSize] = outputBytes[i];
                        }
                    }
                    //otherwise it is unexpected extra file padding that we ignore.
                }

                inputPosition += streamBlockSize;
            }

            cryptoStream.Close();
            cryptoStream.Dispose();
            bool hashFailure = false;
            sha1Provider.TransformFinalBlock(outputBytes, 0, 0);
            hashValueBytes = sha1Provider.Hash;
            if (reportedHashValueBytes != null
                && hashValueBytes != null
                && reportedHashValueBytes.Length == hashValueBytes.Length)
            {
                for (int i = 0; i < hashValueBytes.Length; i++)
                {
                    if (hashValueBytes[i] != reportedHashValueBytes[i])
                        hashFailure = true;
                }
            }
            else
                hashFailure = true;

            if (hashFailure)
            {
                throw new DecryptionHashCheckException();
            }

            return BitConverter.ToString(hashValueBytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FromFile"></param>
        /// <param name="ToFile"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        /// <exception></exception>
        public static string DecryptFile(string FromFile, string ToFile, string Password)
        {
            if (!Directory.Exists(Path.GetDirectoryName(ToFile)))
                Directory.CreateDirectory(Path.GetDirectoryName(ToFile));

            FileStream outFile = File.Create(ToFile);
            FileStream inputData = File.OpenRead(FromFile);

            //TODO: ultimately write the temp file to a different location and then copy, like ZIP programs - but would need to consider temp space!!

            string hashBytes;
            try
            {
                hashBytes = DecryptStream(inputData, outFile, Password);
                inputData.Close();
                outFile.Close();
            }
            catch (DecryptionHashCheckException e)
            {
                inputData.Close();
                outFile.Close();
                File.Delete(ToFile);
                throw;
            }
            finally
            {
                inputData.Dispose();
                outFile.Dispose();
            }

            return hashBytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EncryptedData"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static string DecryptString(string EncryptedData, string Password)
        {
            MemoryStream inputData = new MemoryStream(System.Convert.FromBase64String(EncryptedData));
            MemoryStream outputData = new MemoryStream();
            DecryptStream(inputData, outputData, Password);
            inputData.Close();
            outputData.Close();
            return Encoding.UTF8.GetString(outputData.ToArray());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DecryptionHashCheckException : Exception
    {
        public DecryptionHashCheckException()
            : base("Hash verification failure during decryption. (Invalid Password? File corruption?)")
        {
        }
    }
}
