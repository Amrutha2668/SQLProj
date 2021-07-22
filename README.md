# SQL-Project
Steps to open *.sln file from github in Visual Studio 2019
 
1. Open Visual studio 2019 
2. Open the terminal in the vs 2019
3. Copy the Git Repository link which has to be cloned
4. use "git clone [paste-the-coped-link]"
6. Open the downloaded folder 
7. Open SQLProj.sln
8. Check for the dependencies:
       
         SQLProject's dependencies
           1. Program.cs file's dependencies:
             1. Path of the CSV-file should be mentioned in StreamReader.
             2. Connection string of SQL-Server.

           2. Loader.cs file's dependencies:
             1. Connection string passed from the program.cs.
             2. Conncetion string from the newely created Database.
       
           3. Analyzer.cs file's dependencies:
             1. Database name from progrom.cs.

9. Run the project