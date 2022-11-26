using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budget;


//
// NOTE: Learning to read/write blobs taken from the following web site
// https://www.akadia.com/services/dotnet_read_write_blob.html
// https://www.codeproject.com/Articles/66271/Working-with-SQL-Server-BLOB-Data-in-NET
//

namespace EnterpriseBudget.Model
{
    /// <summary>
    /// Handles the sqlite files stored in the sql server
    /// </summary>
    public class DepartmentBudgets
    {
        private String sqliteFileName = "deptBudget.db";
        private String appName = "EnterpriseBudget";
        private String sPath;
        private HomeBudget homeBudget;
        private int currentUserDepartmentId;

        private String FullPath { get { return $"{sPath}\\{appName}\\{sqliteFileName}"; } }

        public HomeBudget HomeBudget { get { return homeBudget; } }

        public int CurrentUserDepartmentId { get { return currentUserDepartmentId; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public DepartmentBudgets()
        {
            sPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Directory.CreateDirectory($"{sPath}\\{appName}");
        }

        /// <summary>
        /// close all required connections
        /// </summary>
        public void Close()
        {
            if (homeBudget != null)
            {
                homeBudget.CloseDB();
            }
        }

        /// <summary>
        /// Get the sqlite file from sqlserver, and open budget file
        /// </summary>
        /// <param name="departmentID">The ID of the department whose budget we want to open</param>
        /// <returns>true if successful, false otherwise</returns>
        public bool DownLoadAndOpenDepartmentBudgetFile(int departmentID) 
        {
            try {
                currentUserDepartmentId = departmentID;
                ReadAndSaveBlobFromSQLServer(Connection.cnn, "deptBudgets", "sqlitefile", $"deptId={departmentID}",FullPath);

                homeBudget = new HomeBudget(FullPath, false);
                
                return true;
            }
            catch(Exception e) { 
                return false; 
            }
        }

        public bool SaveToDatabase()
        {
            bool saved = WriteBlobToSQLServer(Connection.cnn, FullPath, "deptBudgets", "sqlitefile", "deptId = @departmentId;");
            DownLoadAndOpenDepartmentBudgetFile(currentUserDepartmentId);
            return saved;
        }

        // write binary data to SQLServer
        // Returns true if the file was saved, false if there was an error
        private bool WriteBlobToSQLServer(SqlConnection cnn, string fileName, string tableName, string columnName, string whereCondition)
        {
            try
            {
                homeBudget.CloseDB();
                SqlCommand writeBlob = cnn.CreateCommand();
                writeBlob.CommandText = $"UPDATE {tableName} set {columnName} = @data where {whereCondition} ";
                writeBlob.Parameters.AddWithValue("@data", File.ReadAllBytes(fileName));
                writeBlob.Parameters.AddWithValue("@departmentId", currentUserDepartmentId);
                writeBlob.ExecuteNonQuery();
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        // **** Read BLOB from the Database and save it on the Filesystem
        // NOTE: columnName should specify 1 column only.  Do NOT try to fool it by setting columnName to "col1, col2". 
        //       Chaos will ensue :(
        // NOTE: the whereCondition should be sufficient to ensure that only 1 row will be returned. Failing that...
        //       Chaos will ensue :(
        private void ReadAndSaveBlobFromSQLServer(SqlConnection cnn, string tableName, string columnName, string whereCondition, string outputFileName)
        {
            SqlCommand getBinaryData = new SqlCommand(
                $"SELECT {columnName} " +
                $"FROM {tableName} " +
                $"WHERE {whereCondition} ", cnn);

            FileStream fs;                          // Writes the BLOB to a file.
            BinaryWriter bw;                        // Streams the BLOB to the FileStream object.
            int bufferSize = 512;                   // Size of the BLOB buffer.
            byte[] outbyte = new byte[bufferSize];  // The BLOB byte[] buffer to be filled by GetBytes.
            long retval;                            // The bytes returned from GetBytes.
            long startIndex = 0;                    // The starting position in the BLOB output.

            SqlDataReader myReader = getBinaryData.ExecuteReader(CommandBehavior.SequentialAccess);

            while (myReader.Read())
            {
                fs = new FileStream(outputFileName, FileMode.OpenOrCreate, FileAccess.Write);
                bw = new BinaryWriter(fs);

                // Reset the starting byte for the new BLOB.
                startIndex = 0;

                // Read the bytes into outbyte[] and retain the number of bytes returned.
                retval = myReader.GetBytes(0, startIndex, outbyte, 0, bufferSize);

                // Continue reading and writing while there are bytes beyond the size of the buffer.
                while (retval == bufferSize)
                {
                    bw.Write(outbyte);
                    bw.Flush();

                    // Reposition the start index to the end of the last buffer and fill the buffer.
                    startIndex += bufferSize;
                    retval = myReader.GetBytes(0, startIndex, outbyte, 0, bufferSize);
                }

                // Write the remaining buffer.
                bw.Write(outbyte, 0, (int)retval);
                bw.Flush();

                // Close the output file.
                bw.Close();
                fs.Close();
            }

            // Close the reader and the connection.
            myReader.Close();
        }

    }
}
