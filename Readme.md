#Okta User Export Tool
This is a sample C# tool that generates a CSV file of all the Okta user profiles in your Okta organization's User Directory, including custom UD attributes. 
###How to use this tool
Compile this tool in Visual Studio (preferably Visual Studio 2015) or grab the Okta.Tools.UserExporter.exe executable from the bin sub-folder.   
Edit the app.config file (if you plan to compile it) or the Okta.Tools.UserExporter.exe.config file (if you only plan to run it) and adjust the following parameters in the `<appSettings>` section:

- **OktaUrl**: base url of your Okta organization, including https://. Examples: https://acme.okta.com, https://acme.okta-emea.com, https://acmedev.oktapreview.com
- **OktaApiKey**: an API Token you must generate from your organization on the Security --> API page in the Admin section of your Okta organization. 
- **OutputFileName**: Optional parameter that specifies the name of the CSV file where the list of users will be generated. Note that you can only enter a file name, not a file path and that the file will be generated in the same folder where the tool is executed.
If you leave it blank, the default format of the file will be *OktaUsers_yyyyMMdd-hhmmss.csv*
