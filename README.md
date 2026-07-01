Install Microsoft Visual Studio Community 2022 (64-bit) and PostgreSQL 18.3 on your machine.
Restore the database using the WorkMonitor360DB.backup file available in the Database folder of this repository:Database/WorkMonitor360DB.backup
Open the App.config file and update the following settings: Connection String: Replace the PostgreSQL username/password and database details with your local PostgreSQL configuration. OpenAI API Key: Add your paid licensed OpenAI API key under the appSettings section.
 Build and run the application from Visual Studio.
