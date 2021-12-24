using LsonLib;//https://github.com/rstarkov/LsonLib
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;//not quite sure if this is correct

/* Welcome to DiCE - DCS Integrated Countermeasure Editor
 * 
 * Release Checklist:
 * -Correct all typos
 * -Clean up the spacing in the code a bit
 * -Increase the version number in the main probram and the entry.lua files
 * -add update notes
 * -Run the build and make sure it works in DCS itself
 * -Go to the DiCE\Release folder, copy the previous release, and paste over the luas and make sure to add the new stuff too
 * -make sure to re-Build the project and put the new exe in the Release folder
 * -Update the readme
 * -delete the old files form thje readme exe
 * -Drag all new files and readme to the release zip
 * -Put the zip in the google drive
 * -test the new version on a different device
 * -upload new pics to imgur
 * -on ED userfiles
 *  -Edit the DiCE entry
 *  -adjust title name (version number)
 *  -edit descroption
 *  -edit version notes
 *  -download the file to make sure the correct one was uploaded
 * -on ED forums
 *  -take relevent screenshoort of new feature
 *  -update first post
 *  -make normal post
 * -on discord
 *  -@everyone about the new version
 * -Yay!
 * 
 * 
 * Vision:
 * -Player Launches DCS
 * -Player confirms CMS editing via the DCS Special Options Menu
 * -This program automatically catches the change.
 * -This program then changes each aircraft's CMS .lua file to match what the user set.
 * -Player enjoys an easier and seamless way to change aircraft countermeasure profiles
 * 
 * 
 * Programing objectives
 * -Run on DCS launch (should be taken care of during the hook)
 * -Listen to the users Saved Games/DCS/Config/options.lua file for changes
 * -If the file changes, read the file and make the appropiate exports to the .lua file
 * -Close this program when DCS closes
 * 
 * ------------------------------------
 * 
 * TODO: 
 * Clean up comments
 * Make AH64 art, resources, and menu GUIs
 * 
 * --------------------------------------
 * 
 * TODO: Later
 * 
 * -if the program is launched and does not detect dcs, use the CMS Editor version
 * -have the users info saved via .json (use DCS-weatherman as an example
 * -Make a backup system (requested once via discord by Animal)
 *  -make the system via the exe
 *  -have a "backup countermeasures" button and a "restore backup" button
 *  -idk where to actually save the info yet
 *  -i image i have to go line by line to save and then line by line to load
 *  -maybe this may help: https://docs.microsoft.com/en-us/previous-versions/aa730869(v=vs.80)?redirectedfrom=MSDN
 * -Make logic for the export a10c and a10c2 so that the files are not exported twice for both aircraft
 * -try to scroll to end after init
 * -Make sure that the program never ever blocks DCS from writing to any file, ever(has never been blocked, yet...)
 * -Make better logic for the detection of DCS and the options.lua and to show the buttons when they are not detected
 * -select dcs.exe and  select options.lua backups arent exactly working...
 * -Decide if you want to do individual .lua backups and/or options.lua backups(not right now. The CMS files are only 4kb, so space wont be an issue)
 * -Create the options for DiCE.exe to launch with or without DCS (like SRS's special option)
 * -See if you can get DiCE.exe to launch in a way to at does not "take focus" from the DCS launch
 * -See if you can make logic that will check for changes before exporting the CMS file
 * -revamp the old aircraft with the new lua system
 * 
 * 
 * Version Targets:
 * -target v1 as f18 and f16 complete (done)
 * -target v2 as a10 and a102 complete (done)
 * -target v3 as av8b complete (done)
 * -target v4 as m2000c complete (done)
 * -tarvet v5 as AH-64D complete
 * -target v6 as f16harms as complete
 * 
 * 
 * Deliverables:
 * 
 * (v1 Start)
 * -DCS Integrated Countermeasure Editor
 * --mods
 * ---Services
 * ----DiCE F-16C
 * -----options
 * ------options.dlg
 * ------optionsData.lua
 * ------optionsDb.lua
 * -----theme
 * ------icon.png
 * ------icon_active.png
 * ------icon_select.png
 * ------icon-38x38.png
 * -----entry.lua
 * ----DiCE F-18C
 * -----options
 * ------options.dlg
 * ------optionsData.lua
 * ------optionsDb.lua
 * -----theme
 * ------icon.png
 * ------icon_active.png
 * ------icon_select.png
 * ------icon-38x38.png
 * -----entry.lua
 * ----DiCE-EXE
 * -----bin
 * ------DiCE.exe
 * --Logs
 * ---DiCE.log (this file will be created due to the hooks lua)
 * --Scripts
 * ---Hooks
 * ----DiCE-DCS-Integrated-Countermeasure-Editor-hook.lua
 * (v1 END) 
 * 
 *
 *
 * (v2 Start)
 * -DCS Integrated Countermeasure Editor
 * --mods
 * ---Services
 * ----DiCE A-10C
 * -----options
 * ------options.dlg
 * ------optionsData.lua
 * ------optionsDb.lua
 * -----theme
 * ------icon.png
 * ------icon_active.png
 * ------icon_select.png
 * ------icon-38x38.png
 * -----entry.lua
 * ----DiCE F-16C
 * -----options
 * ------options.dlg
 * ------optionsData.lua
 * ------optionsDb.lua
 * -----theme
 * ------icon.png
 * ------icon_active.png
 * ------icon_select.png
 * ------icon-38x38.png
 * -----entry.lua
 * ----DiCE F-18C
 * -----options
 * ------options.dlg
 * ------optionsData.lua
 * ------optionsDb.lua
 * -----theme
 * ------icon.png
 * ------icon_active.png
 * ------icon_select.png
 * ------icon-38x38.png
 * -----entry.lua
 * ----DiCE-EXE
 * -----bin
 * ------DiCE.exe
 * --Logs
 * ---DiCE.log (this file will be created due to the hooks lua)
 * --Scripts
 * ---Hooks
 * ----DiCE-DCS-Integrated-Countermeasure-Editor-hook.lua
 * (v2 END) 
 * 
 * 
 * (v3 Start)
 * -DCS Integrated Countermeasure Editor
 * --mods
 * ---Services
 * ----DiCE A-10C
 * -----options
 * ------options.dlg
 * ------optionsData.lua
 * ------optionsDb.lua
 * -----theme
 * ------icon.png
 * ------icon_active.png
 * ------icon_select.png
 * ------icon-38x38.png
 * ------icon-A10C.png
 * -----entry.lua
 * ----DiCE F16C
 * -----options
 * ------options.dlg
 * ------optionsData.lua
 * ------optionsDb.lua
 * -----theme
 * ------icon.png
 * ------icon_active.png
 * ------icon_select.png
 * ------icon-38x38.png
 * ------icon-F-16C.png
 * -----entry.lua
 * ----DiCE F-18C
 * -----options
 * ------options.dlg
 * ------optionsData.lua
 * ------optionsDb.lua
 * -----theme
 * ------icon.png
 * ------icon_active.png
 * ------icon_select.png
 * ------icon-38x38.png
 * ------icon-F18C.png
 * -----entry.lua
 * ----DiCE AV-8B
 * -----options
 * ------options.dlg
 * ------optionsData.lua
 * ------optionsDb.lua
 * -----theme
 * ------icon.png
 * ------icon_active.png
 * ------icon_select.png
 * ------icon-38x38.png
 * ------icon-AV8B.png
 * -----entry.lua
 * ----DiCE-EXE
 * -----bin
 * ------DiCE.exe
 * --Logs
 * ---DiCE.log (this file will be created due to the hooks lua)
 * --Scripts
 * ---Hooks
 * ----DiCE-DCS-Integrated-Countermeasure-Editor-hook.lua
 * (v3 END) 
 * 
 * 
 * Version Notes:
 * v1
 * -Initial Release
 * -DiCE F-18C enabled
 * -DiCE F-16C enabled
 * -about 1508 lines of code in this file
 * 
 * v2
 * -DiCE A-10C and A-10C2 enabled (one Special Menu for both)
 * -about 2002 lines of code in this file
 * 
 * v2.1
 *-Corrected some F-16C values
 *
 * v2.2
 * -DiCE now opens minimized (requested feature)
 * -Fixed a F-16 special options menu showing all zeros (added two parenthesis)
 * 
 * v3
 * -DiCE AV-8B enabled 
 * 
 * v4
 * -DiCE M2000C enabled
 * 
 * v4.1
 * -Updated readme (read readme if you have DCS installed in Program Files
 * -Took away “DiCE is already running” popup
 * 
 * v4.1.1
 * -Fixed DiCE errors in dcs.log file
 *
 * v4.2
 * -DiCE now makes CMS changes on startup too (thanks Bear21)
 *
 * v4.3
 * -Updated F-16C for DCS v2.7.7.15038
 *
 * vFuture
 * -DiCE AH-64D
 * -DiCE F-16C Harm Tables
 * 
 * 
 * 
 * Bugs:
 * -None reported
 * -If your DCS normally hangs up when you Alt+Tab from the loading screen, DCS may hang when loading DiCE. If
 *  you experence this problem, you may have to launch DiCE manually after DCS starts. Delete the DiCE hook file
 *  (located here) and make a shortcut to DiCE.exe (located here). (Noted in Readme)
 * 
 * 
 * Notes:
 * 
 * https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-read-text-from-a-file
 * 
 * DCS World OpenBeta\dxgui\skins\skinME\images\manager_modules\warning.png (can be used for something)
 * DCS World OpenBeta\dxgui\skins\skinME\images\Source (Good source pics)
 * DCS World OpenBeta\FUI\Common (DCS splash screen)
 * DCS World OpenBeta\dxgui\skins\skinME\images\Buttons\combobox //possible use for minimise icon
 * DCS World OpenBeta\dxgui\skins\skinME\images\Buttons\server_settings\minus //possible use for minimise icon
 * DCS World OpenBeta\dxgui\skins\skinME\images\m1\buttons\btnminus
 * DCS World OpenBeta\dxgui\skins\skinME\images\manual //good icons for a redo
 * DCS World OpenBeta\dxgui\skins\skinME\images\window\mp\create_server \\good icons too
 * 
 * DCS World OpenBeta\dxgui\skins\skinME\images\window\logbook_icon //sihlouettes of dcs aircraft
 * 
 * 
 * Thanks:
 * Thank you to everyone that helped me during this project and all projects I have done before.
 * Special thank to CiriBob, the creator of DCS-SRS. Witout their help, you would have had do a few more things by hand.
 * Special thanks to the Hoggit Discord for answering my questions and here, there, and everywhere.
 * Special thanks to all who voluntered to demo and test DiCE. Your bravery will never be forgotten.
 * 
 * 
 * Adding new aircraft to this code instructions:
 * add detection_AIRPLANE_DiCE
 * add detection_AIRPLANE_vanilla
 * add cmdsLua_AIRPLANE_fullPath
 * add cmdsLua_AIRPLANE_FolderPath
 * parse the the options lua
 * build the module CMDS file
 * 
 * 
 * Wishlist: 
 * Have the hidden icons populate based on the detected installed modules. And have the DiCE Log say what modules are detected.
 * End of Comments------//
 */



namespace DiCE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //globals
        string appPath = System.AppDomain.CurrentDomain.BaseDirectory;//path of this program exe

        string userDcs_Full_pathWithExtention;
        string userOptionsLua_Full_pathWithExtention;

        string dcs_topFolderPath;
        string optionsLua_topFolderPath;

        string exportPathBackup;//backup has not yet been implemented

        bool isDcsLocationSet;
        bool isOptionsLuaLocationSet;

        string cmdsLua_F18C_fullPath;
        string cmdsLua_F18C_FolderPath;

        string cmdsLua_F16C_fullPath;
        string cmdsLua_F16C_FolderPath;

        string cmdsLua_A10C_fullPath;
        string cmdsLua_A10C_FolderPath;

        string cmdsLua_A10C2_fullPath;//will be combined with the A-10C
        string cmdsLua_A10C2_FolderPath;//will be combined with the A-10C

        string cmdsLua_M2000C_fullPath;
        string cmdsLua_M2000C_FolderPath;

        string cmdsLua_AV8B_fullPath;
        string cmdsLua_AV8B_FolderPath;

        string cmdsLua_AH64D_fullPath;
        string cmdsLua_AH64D_FolderPath;

        bool isDCSrunning;

        FileSystemWatcher fileSystemWatcher1;
        DateTime fsLastRaised = DateTime.Now;//this is going to be used for making sure that 
        //there isnt an overreaction to the options lua being changed

        //these are the names of the identifiers in the options.lua file
        string detection_F18C_DiCE = "DiCE F-18C";
        string detection_F16C_DiCE = "DiCE F-16C";
        string detection_A10C_DiCE = "DiCE A-10C";
        string detection_A10C2_DiCE = "DiCE A-10C";//this will be the same as the A-10C
        string detection_AV8B_DiCE = "DiCE AV-8B";
        string detection_M2000C_DiCE = "DiCE M2000C";
        string detection_AH64D_DiCE = "DiCE AH-64D";

        //these make sure that DiCE exports CMS profiles that the user actually has
        string detection_F18C_vanilla = "[\"F/A-18C\"]";
        string detection_F16C_vanilla = "[\"F-16C\"]";
        string detection_A10C_vanilla = "[\"A-10C\"]";
        string detection_A10C2_vanilla = "[\"A-10C_2\"]";
        string detection_AV8B_vanilla = "[\"AV8BNA\"]";
        string detection_M2000C_vanilla = "[\"M-2000C\"]";
        string detection_AH64D_vanilla = "[\"AH-64D\"]";//TODO: Verify this is the correct module name

        int mainPageButtonLogo = 0;

        int secondsToCheckIfDcsIsAlive = 1;//DiCE will check if DCS.exe is running at this rate. 2 was fine

        string optionsLuaText;

        //this inits the timer. Putting here allows it to be used in the whole program
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        public MainWindow()
        {

            //-----------------------------------------------------------------------------------------
            //-----Is DCS DiCE Already Running Check
            //-----------------------------------------------------------------------------------------

            //simple check to make sure that an instance of dice is not launched while an instance is already running
            //https://stackoverflow.com/questions/7182949/how-to-check-if-a-wpf-application-is-already-running
            string procName = Process.GetCurrentProcess().ProcessName;

            // get the list of all processes by the "procName"       
            Process[] processes = Process.GetProcessesByName(procName);

            if (processes.Length > 1)
            {
                //MessageBox.Show(procName + " already running.");
                System.Windows.Application.Current.Shutdown();
                return;
            }
            else
            {
                // Application.Run(...);
            }

            InitializeComponent();

            //hide the buttons until they actually work in WPF and have use
            button_selectDcsExe.Visibility = Visibility.Hidden;
            button_selectOptionsLua.Visibility = Visibility.Hidden;

            //-----------------------------------------------------------------------------------------
            //------------Timer Init
            //-----------------------------------------------------------------------------------------

            //https://stackoverflow.com/questions/5410430/wpf-timer-like-c-sharp-timer

            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, secondsToCheckIfDcsIsAlive);//set the time for the DCS process check here (seconds, minutes, hours)


            //-----------------------------------------------------------------------------------------
            //--------First Log Message
            //-----------------------------------------------------------------------------------------

            //MessageBox.Show(appPath);//shows a text box of the folder path of the exe. does not include the exe name
            //https://stackoverflow.com/questions/6485156/adding-strings-to-a-richtextbox-in-c-sharp
            //Just a log entry to show that the program is running
            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "DiCE is Rolling...");
            richTextBox_log.ScrollToEnd();

            //-----------------------------------------------------------------------------------------
            //--------Get The DCS.exe Location
            //-----------------------------------------------------------------------------------------

            //https://stackoverflow.com/questions/51148/how-do-i-find-out-if-a-process-is-already-running-using-c
            //https://stackoverflow.com/questions/5497064/how-to-get-the-full-path-of-running-process
            //This gets the full path of DCS when DCS.exe is running as long as the process is called 'DCS'
            //this actually works!

            foreach (Process clsProcess in Process.GetProcesses())
            {
                //now we're going to see if any of the running processes
                //match the currently running processes. Be sure to not
                //add the .exe to the name you provide, i.e: NOTEPAD,
                //not NOTEPAD.EXE or false is always returned even if
                //notepad is running.
                //Remember, if you have the process running more than once, 
                //say IE open 4 times the loop thr way it is now will close all 4,
                //if you want it to just close the first one it finds
                //then add a return; after the Kill
                if (clsProcess.ProcessName.Equals("DCS"))
                {
                    //if the process is found to be running then we
                    //return a true
                    var process = Process.GetCurrentProcess(); // Or whatever method you are using (I dont think this does anything as is)
                    string userDcs_Full_pathWithExtention = clsProcess.MainModule.FileName;
                    //MessageBox.Show(userDcs_Full_pathWithExtention);//shows the path od dcss to include the exe when dcs is running
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "DCS.exe path: '" + userDcs_Full_pathWithExtention + "'");
                    richTextBox_log.ScrollToEnd();
                    dcs_topFolderPath = userDcs_Full_pathWithExtention.Remove(userDcs_Full_pathWithExtention.Length - 12);//remove the bin and exe so that you get to the top folder
                    isDcsLocationSet = true;
                    isDCSrunning = true;//using this instead of the else ofr reasons explained below
                    dispatcherTimer.Start();//starts the timer
                }
                else
                {
                    //richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "DCS is not detected. Close DiCE, make sure you have installed DiCE correctly, and then re-start DCS.");
                    //for some reason this repeats, a lot, on launch.OOOOOOooooh, thats because its in the 'foreach', so it sends the message for every process that isn't DCS.
                }
            }

            //https://stackoverflow.com/questions/14899422/how-to-navigate-a-few-folders-up

            userOptionsLua_Full_pathWithExtention = Path.GetFullPath(Path.Combine(appPath, @"..\..\..\..\"));
            userOptionsLua_Full_pathWithExtention = userOptionsLua_Full_pathWithExtention + "Config\\options.lua";
            //richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "DiCE.exe path : '" + appPath + "'");
            //richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Predicted options.lua path : '" + userOptionsLua_Full_pathWithExtention + "'");



            if (File.Exists(userOptionsLua_Full_pathWithExtention))
            {

                optionsLua_topFolderPath = userOptionsLua_Full_pathWithExtention.Remove(userOptionsLua_Full_pathWithExtention.Length - 12);//remove the bin and exe so that you get to the top folder
                exportPathBackup = (appPath + "\\DiCE-Backup\\");//string the location for the backup
                isOptionsLuaLocationSet = true;
                button_selectOptionsLua.Visibility = Visibility.Hidden;//dont show the button because the stuff is set
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Options.lua path: '" + userOptionsLua_Full_pathWithExtention + "'");
                richTextBox_log.ScrollToEnd();
            }


            else if (File.Exists(appPath + "\\DiCE-Backup\\DiCE-optionsLua-Location.txt"))//if there is already a backup file
            //if (!File.Exists(userOptionsLua_Full_pathWithExtention))//if the program couldn't find the options.lua by itself. (this actually triggers a weird condition that
            //goes straight tothe catch even if the location of the options.lua was set automatically
            {
                try
                {
                    userOptionsLua_Full_pathWithExtention = System.IO.File.ReadAllText(appPath + "\\DiCE-Backup\\DiCE-optionsLua-Location.txt");//set the location of the DCS.exe as a string
                    optionsLua_topFolderPath = userOptionsLua_Full_pathWithExtention.Remove(userOptionsLua_Full_pathWithExtention.Length - 12);//remove the bin and exe so that you get to the top folder
                    exportPathBackup = (appPath + "\\DiCE-Backup\\");//string the location for the backup
                    isOptionsLuaLocationSet = true;
                    button_selectOptionsLua.Visibility = Visibility.Hidden;//dont show the button because the stuff is set
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Options.lua path: '" + userOptionsLua_Full_pathWithExtention + "'");
                    richTextBox_log.ScrollToEnd();
                }
                catch (IOException e)
                {
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "DiCE could not read it's backup file");
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + e.Message);
                    richTextBox_log.ScrollToEnd();
                }
            }
            else
            {
                //just load the buttons and have the user choose the dcs.exe location
                if (button_selectOptionsLua.Visibility == Visibility.Hidden)
                {
                    //the options lua was already automatically gotten
                }
                else
                {
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "No Options.lua path found at: '" + userOptionsLua_Full_pathWithExtention + "'");
                    richTextBox_log.ScrollToEnd();
                    //the button is showing and the user should select the options.lua
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Please select your Options.lua file.");
                    richTextBox_log.ScrollToEnd();
                }
            }
            if (isDcsLocationSet == true && isOptionsLuaLocationSet == true)//if both locations are set, then run the program
            {
                //here are the locations where the countermeasure files should be located
                cmdsLua_F18C_fullPath = dcs_topFolderPath + @"\Mods\aircraft\FA-18C\Cockpit\Scripts\TEWS\device\CMDS_ALE47.lua";
                cmdsLua_F18C_FolderPath = dcs_topFolderPath + @"\Mods\aircraft\FA-18C\Cockpit\Scripts\TEWS\device";

                cmdsLua_F16C_fullPath = dcs_topFolderPath + @"\Mods\aircraft\F-16C\Cockpit\Scripts\EWS\CMDS\device\CMDS_ALE47.lua";
                cmdsLua_F16C_FolderPath = dcs_topFolderPath + @"\Mods\aircraft\F-16C\Cockpit\Scripts\EWS\CMDS\device";

                cmdsLua_A10C_fullPath = dcs_topFolderPath + @"\Mods\aircraft\A-10C\Cockpit\Scripts\AN_ALE40V\device\AN_ALE40V_params.lua";
                cmdsLua_A10C_FolderPath = dcs_topFolderPath + @"\Mods\aircraft\A-10C\Cockpit\Scripts\AN_ALE40V\device";

                cmdsLua_A10C2_fullPath = dcs_topFolderPath + @"\Mods\aircraft\A-10C_2\Cockpit\Scripts\AN_ALE40V\device\AN_ALE40V_params.lua";
                cmdsLua_A10C2_FolderPath = dcs_topFolderPath + @"\Mods\aircraft\A-10C_2\Cockpit\Scripts\AN_ALE40V\device";

                cmdsLua_M2000C_fullPath = dcs_topFolderPath + @"\Mods\aircraft\M-2000C\Cockpit\Scripts\SPIRALE.lua";
                cmdsLua_M2000C_FolderPath = dcs_topFolderPath + @"\Mods\aircraft\M-2000C\Cockpit\Scripts";

                cmdsLua_AV8B_fullPath = dcs_topFolderPath + @"\Mods\aircraft\AV8BNA\Cockpit\Scripts\EWS\EW_Dispensers_init.lua";
                cmdsLua_AV8B_FolderPath = dcs_topFolderPath + @"\Mods\aircraft\AV8BNA\Cockpit\Scripts\EWS\";

                cmdsLua_AV8B_fullPath = dcs_topFolderPath + @"\Mods\aircraft\AH-64D\Cockpit\Scripts\EWS\EW_Dispensers_init.lua";//TODO: adjust this to the module
                cmdsLua_AV8B_FolderPath = dcs_topFolderPath + @"\Mods\aircraft\AH-64D\Cockpit\Scripts\EWS\";

                //read the lua file so that we can do some init stuff
                var optionsLuaText = LsonVars.Parse(File.ReadAllText(userOptionsLua_Full_pathWithExtention));

                
                bool closeDiceAfterLaunch;//init the variable
                //get the bool from the options.lua. it is a checkbox so it will already provide true or false
                closeDiceAfterLaunch = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["closeDiceAfterLaunch"].GetBool();
                //if it is true, close DiCE
                if (closeDiceAfterLaunch)
                {
                    System.Windows.Application.Current.Shutdown();
                }

                //lua version

                //this just an example of the format for getting values.
                //if the value is not present, the program crashes. TODO: test this crash thing

                //string howManyBirds = optionsLuaText["options"]["difficulty"]["birds"].GetString();

                //richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + howManyBirds + " birds detected.");

                //richTextBox_log.ScrollToEnd();

                //MessageBox.Show(optionsLua_topFolderPath);
                
                listenToUsersOptionsFile();
                richTextBox_log.ScrollToEnd();
                Thread.Sleep(1100);//this is to make sure that the next line does not get cauth in the programs own rapid options.lua change check
                makeTheCmsChanges();//updates the values when launched

            }
            else
            {
                //dont do anything
            }

            if (!isDCSrunning == true)
            {//if DCS.exe is not detected as running
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "DCS is not detected. Close DCS, close DiCE, make sure you have installed DiCE correctly, and then re-start DCS.");
                richTextBox_log.ScrollToEnd();
            }
            richTextBox_log.ScrollToEnd();

            //if dcs is running is true, minimise
            if (isDCSrunning)
            {
                WindowState = WindowState.Minimized;//minimise the window. maybe try to make this a launch option?
            }

            //load this file (the options.lua) into a lua parser


        }

        private void listenToUsersOptionsFile()
        {//this makes the timer
            //https://www.c-sharpcorner.com/UploadFile/ad8d1c/watch-a-folder-for-updation-in-wpf-C-Sharp/
            fileSystemWatcher1 = new FileSystemWatcher(optionsLua_topFolderPath, "options.lua");
            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Watching for changes in: " + optionsLua_topFolderPath + "\\options.lua");
            
            richTextBox_log.ScrollToEnd();

            fileSystemWatcher1.EnableRaisingEvents = true;
            fileSystemWatcher1.IncludeSubdirectories = true;
            //This event will check for  new files added to the watching folder
            //fileSystemWatcher1.Created += new FileSystemEventHandler(newfile);
            //This event will check for any changes in the existing files in the watching folder
            fileSystemWatcher1.Changed += new FileSystemEventHandler(fs_Changed);
            //this event will check for any rename of file in the watching folder
            //fileSystemWatcher1.Renamed += new RenamedEventHandler(fs_Renamed);
            //this event will check for any deletion of file in the watching folder
            //fileSystemWatcher1.Deleted += new FileSystemEventHandler(fs_Deleted);

            //var pathWithEnv = @"%USERPROFILE%\Saved Games"; //this does not work because options.lua is too deep
            //var filePath = Environment.ExpandEnvironmentVariables(pathWithEnv);
            //fileSystemWatcher1.Path = (filePath);
            fileSystemWatcher1.Path = (optionsLua_topFolderPath);
            fileSystemWatcher1.Filter = "options.lua";
            //MessageBox.Show("listening via filesystemwatcher");
        }

        public void fs_Changed(object fschanged, FileSystemEventArgs changeEvent)
        {
            makeTheCmsChanges();
            
        }


        public void makeTheCmsChanges()
        {
            if (DateTime.Now.Subtract(fsLastRaised).TotalMilliseconds > 1000)//this hopefully prevents the options.lua to be read multiple times within 1 second
            {
                fsLastRaised = DateTime.Now;
                //MessageBox.Show("The file was chaged");//this works, trust me. Try to get it to watch, specifically, the options.lua file.
                //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-read-from-a-text-file
                //these are the names of the identifiers in the options.lua file

                Thread.Sleep(500);//is hopefully prevents the read of the below file during a DCS write. has not failed at 1000, yet.
                                  //2000 kinda takes too long, personally
                                  //500 seems to be working fine with the multi-change check
                                  //https://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
                                  //where the invoke thing comes from

                this.Dispatcher.Invoke(() =>
                {
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "A change in Options.lua was detected.");
                    try
                    {
                        optionsLuaText = System.IO.File.ReadAllText(userOptionsLua_Full_pathWithExtention);
                        //the blow '&&' statements should make sure that the player has the module installed before 
                        //the utility tries to export it. Just another check to prevent wasted utility effort
                        if (optionsLuaText.Contains(detection_F18C_DiCE) && optionsLuaText.Contains(detection_F18C_vanilla))
                        {
                            //richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "F-18C CMS file located.");
                            //richTextBox_log.ScrollToEnd();
                            readAndExportF18Data();
                        }

                        if (optionsLuaText.Contains(detection_F16C_DiCE) && optionsLuaText.Contains(detection_F16C_vanilla))
                        {
                            //richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "F-16C CMS file located.");
                            //richTextBox_log.ScrollToEnd();
                            readAndExportF16Data();
                        }

                        if (optionsLuaText.Contains(detection_A10C_DiCE) && optionsLuaText.Contains(detection_A10C_vanilla))
                        {
                            //richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "A-10C CMS file located.");
                            //richTextBox_log.ScrollToEnd();
                            readAndExportA10CData();
                        }

                        if (optionsLuaText.Contains(detection_A10C2_DiCE) && optionsLuaText.Contains(detection_A10C2_vanilla))
                        {
                            // richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "A-10C2 CMS file located.");
                            //richTextBox_log.ScrollToEnd();
                            readAndExportA10CData();
                        }
                        if (optionsLuaText.Contains(detection_AV8B_DiCE) && optionsLuaText.Contains(detection_AV8B_vanilla))
                        {
                            //richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "AV-8B CMS file located.");
                            //richTextBox_log.ScrollToEnd();
                            readAndExportAV8BData();
                        }
                        if (optionsLuaText.Contains(detection_M2000C_DiCE) && optionsLuaText.Contains(detection_M2000C_vanilla))
                        {
                            //richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "M2000C CMS file located.");
                            //richTextBox_log.ScrollToEnd();
                            readAndExportM2000CData();
                        }
                        if (optionsLuaText.Contains(detection_AH64D_DiCE) && optionsLuaText.Contains(detection_AH64D_vanilla))
                        {
                            //richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "AG-64D CMS file located.");
                            //richTextBox_log.ScrollToEnd();
                            //readAndExportAH64DData();TODO: Enable after module release
                        }

                    }
                    catch (IOException f)
                    {
                        richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "DiCE could not read the options.lua");
                        richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + f.Message);
                        richTextBox_log.ScrollToEnd();
                    }
                    //MessageBox.Show(optionsLuaText);

                });
            }
        }



        private void playCompleteSound()
        {
            //this is the sound that plays on successful export of a profile
            //this plays way too many times. maybe run a check to see if any of the written values actually changed
            //System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"C:\Music\Good job, Mobius 1!.wav");
            //System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"Assets\Play02.oog");
            //SystemSounds.Beep.Play();
            //SystemSounds.Asterisk.Play();
            //SystemSounds.Exclamation.Play();
            //the three above are all the same
            //SystemSounds.Hand.Play();//three tone tatutunnn
            //SystemSounds.Question.Play();//uh, no sound....

            //player.Play();
        }



        private void dispatcherTimer_Tick(object sender, EventArgs e)//fires every 1 seconds
        {
            //https://stackoverflow.com/questions/262280/how-can-i-know-if-a-process-is-running
            //if DCS is not running, this program will close
            Process[] pname = Process.GetProcessesByName("DCS");
            if (pname.Length == 0)
            {
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "DiCE Closing...");
                dispatcherTimer.Stop(); //https://stackoverflow.com/questions/5410430/wpf-timer-like-c-sharp-timer
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Closing...");
                richTextBox_log.ScrollToEnd();
                System.Windows.Application.Current.Shutdown();
            }
            else//if DCS is running, the program will continue
            { }
        }



        private void mainPageDiceLogo_click(object sender, RoutedEventArgs e) //EASTER EGG SPOILER DO NOT READ//-----------------
        {//EASTER EGG SPOILER DO NOT READ//-----------------
            mainPageButtonLogo++;//EASTER EGG SPOILER DO NOT READ//-----------------
            if (mainPageButtonLogo == 50)//EASTER EGG SPOILER DO NOT READ//-----------------
            {//EASTER EGG SPOILER DO NOT READ//-----------------
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + //EASTER EGG SPOILER DO NOT READ//-----------------
                    "You can add red wine while you scramble eggs to make them green. FYI.");//EASTER EGG SPOILER DO NOT READ//-----------------
                playCompleteSound();
            }//EASTER EGG SPOILER DO NOT READ//-----------------
        }//EASTER EGG SPOILER DO NOT READ//-----------------


        private void titleBar_leftButtonDown(object sender, MouseButtonEventArgs e)
        {//this moves the window when the titlebar is clicked and held down
            //I made the custom title bar and removed the default one to match the feel of DCS itself
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {//this closes the program when the close button is clicked
            //created to match the feel of DCS

            dispatcherTimer.Stop(); //https://stackoverflow.com/questions/5410430/wpf-timer-like-c-sharp-timer
            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Closing...");
            richTextBox_log.ScrollToEnd();
            System.Windows.Application.Current.Shutdown();
        }

        private void buttonOnBottomDonate_Click(object sender, RoutedEventArgs e)
        {//this contains the link to the donation site
            //MessageBox.Show("You pressed the donate button");
            //https://stackoverflow.com/questions/502199/how-to-open-a-web-page-from-my-application
            System.Diagnostics.Process.Start("https://www.paypal.com/paypalme/asherao");
            //Thank you for donating <3
        }

        private void richTextBox_log_TextChanged(object sender, TextChangedEventArgs e)
        {
            // scroll it automatically when the text is changed
            richTextBox_log.ScrollToEnd();
        }


        private void button_selectDcsExe_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = @"C:\Program Files\Eagle Dynamics\DCS World OpenBeta\bin\DCS.exe";
            openFileDialog1.Filter = "DCS.exe|*.exe";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Title = "Select DCS.exe (Example: C:\\Program Files\\Eagle Dynamics\\DCS World OpenBeta\\bin\\DCS.exe)";

            if (openFileDialog1.ShowDialog() == DialogResult.HasValue)
            {

                userDcs_Full_pathWithExtention = openFileDialog1.FileName;
                if (userDcs_Full_pathWithExtention.Contains(@"bin\DCS.exe") == true)
                {
                    try
                    {
                        // Determine whether the directory exists.
                        if (Directory.Exists(appPath + "\\DiCE-Backup"))
                        {
                            try
                            {
                                System.IO.File.WriteAllText(appPath + "\\DiCE-Backup\\DiCE-DCS-Location.txt", userDcs_Full_pathWithExtention);//this saves the DCS location in the backup location
                                                                                                                                              //MessageBox.Show("You have selected: " + userDcs_Full_pathWithExtention);
                                button_selectDcsExe.Visibility = Visibility.Hidden;
                                isDcsLocationSet = true;
                                checkIfBothDcsExeAndOptionsLuaAreSet();
                                return;//idk why this is here
                            }
                            catch (IOException i)
                            {
                                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "DiCE could not write to the backup file.");
                                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + i.Message);
                                richTextBox_log.ScrollToEnd();
                            }
                        }

                        try
                        {
                            // Try to create the directory.
                            DirectoryInfo di = Directory.CreateDirectory(appPath + "\\DiCE-Backup");
                            System.IO.File.WriteAllText(appPath + "\\DiCE-Backup\\DiCE-DCS-Location.txt", userDcs_Full_pathWithExtention);//this saves the DCS location in the backup location
                                                                                                                                          //MessageBox.Show("You have selected: " + userDcs_Full_pathWithExtention);
                            button_selectDcsExe.Visibility = Visibility.Hidden;
                            isDcsLocationSet = true;
                            checkIfBothDcsExeAndOptionsLuaAreSet();
                        }
                        catch (IOException j)
                        {
                            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "DiCE could not write to the backup file.");
                            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + j.Message);
                            richTextBox_log.ScrollToEnd();
                        }
                    }
                    catch (Exception)
                    {
                    }
                    finally { }
                }
                else
                {
                    MessageBox.Show("You have selected: " + userDcs_Full_pathWithExtention + "\nThat does not look like the correct file. Please try again.");
                    //...
                }
            }
        }

        private void button_selectOptionsLua_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = @"C:\%USERPROFILE%\Saved Games\DCS\Config\options.lua";
            openFileDialog1.Filter = "options.lua|*.lua";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Title = "Select options.lua (Example: C:\\ProfileName\\Saved Games\\DCS\\Config\\options.lua)";

            if (openFileDialog1.ShowDialog() == DialogResult.HasValue)
            {

                userOptionsLua_Full_pathWithExtention = openFileDialog1.FileName;
                if (userOptionsLua_Full_pathWithExtention.Contains(@"Config\options.lua") == true)
                {
                    try
                    {
                        // Determine whether the directory exists.
                        if (Directory.Exists(appPath + "\\DiCE-Backup"))
                        {
                            try
                            {
                                System.IO.File.WriteAllText(appPath + "\\DiCE-Backup\\DiCE-optionsLua-Location.txt", userOptionsLua_Full_pathWithExtention);//this saves the DCS location in the backup location
                                                                                                                                                            //MessageBox.Show("You have selected: " + userOptionsLua_Full_pathWithExtention);
                                button_selectOptionsLua.Visibility = Visibility.Hidden;
                                isOptionsLuaLocationSet = true;
                                checkIfBothDcsExeAndOptionsLuaAreSet();
                                return;//idk why this is here
                            }
                            catch (IOException g)
                            {
                                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "DiCE could not write to the bakcup file.");
                                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + g.Message);
                                richTextBox_log.ScrollToEnd();
                            }
                        }
                        try
                        {
                            // Try to create the directory.
                            DirectoryInfo di = Directory.CreateDirectory(appPath + "\\DiCE-Backup");
                            System.IO.File.WriteAllText(appPath + "\\DiCE-Backup\\DiCE-optionsLua-Location.txt", userOptionsLua_Full_pathWithExtention);//this saves the DCS location in the backup location
                                                                                                                                                        //MessageBox.Show("You have selected: " + userOptionsLua_Full_pathWithExtention);
                            button_selectOptionsLua.Visibility = Visibility.Hidden;
                            isOptionsLuaLocationSet = true;
                            checkIfBothDcsExeAndOptionsLuaAreSet();
                        }
                        catch (IOException g)
                        {
                            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "DiCE could not write to the bakcup file.");
                            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + g.Message);
                            richTextBox_log.ScrollToEnd();
                        }
                    }
                    catch (Exception)
                    {
                    }
                    finally { }
                }
                else
                {
                    MessageBox.Show("You have selected: " + userOptionsLua_Full_pathWithExtention + "\nThat does not look like the correct file. Please try again.");
                    //...
                }
            }
        }

        public void checkIfBothDcsExeAndOptionsLuaAreSet()
        {
            if (isDcsLocationSet == true && isOptionsLuaLocationSet == true)
            {
                MessageBox.Show("One-time setup is complete! DiCE will now close. Please restart DCS. Remember, you can edit your CMS profiles in the Settings > Special menu in DCS. Have fun!");
                dispatcherTimer.Stop(); //https://stackoverflow.com/questions/5410430/wpf-timer-like-c-sharp-timer
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Closing...");
                richTextBox_log.ScrollToEnd();
                System.Windows.Application.Current.Shutdown();
            }
            else if (isDcsLocationSet == true && isOptionsLuaLocationSet == false)
            {
                MessageBox.Show("You have selected: '" + userDcs_Full_pathWithExtention + "'.\nNext, please select your 'options.lua' file.");
            }
            else if (isDcsLocationSet == false && isOptionsLuaLocationSet == true)
            {
                MessageBox.Show("You have selected: '" + userOptionsLua_Full_pathWithExtention + "'.\nNext, please select your 'DCS.exe' file.");
            }
            else
            {
                //do nothing because both are false or not set
            }
        }

        private void richTextBox_log_TextInput(object sender, TextCompositionEventArgs e)
        {
            richTextBox_log.ScrollToEnd();
        }

        private void button_exit_Click(object sender, RoutedEventArgs e)
        {
            //this closes the program when the close button is clicked
            //created to match the feel of DCS

            dispatcherTimer.Stop(); //https://stackoverflow.com/questions/5410430/wpf-timer-like-c-sharp-timer
            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Closing...");
            richTextBox_log.ScrollToEnd();
            System.Windows.Application.Current.Shutdown();
        }

        private void minimiseButton_Click(object sender, RoutedEventArgs e)
        {
            //https://stackoverflow.com/questions/2841258/minimize-a-window-in-wpf
            WindowState = WindowState.Minimized;
        }

        private void readAndExportF18Data()
        {
            //this is where you read the data from the options.lua and put them into variables

            int F18C_Manual1Chaff_indexStart = optionsLuaText.IndexOf("[\"F18CManual1Chaff\"] =") + 22;
            int F18C_Manual1Chaff_indexEnd = optionsLuaText.IndexOf(",", F18C_Manual1Chaff_indexStart);
            string F18C_Manual1Chaff_string = optionsLuaText.Substring(F18C_Manual1Chaff_indexStart, F18C_Manual1Chaff_indexEnd - F18C_Manual1Chaff_indexStart);

            int F18C_Manual1Flare_indexStart = optionsLuaText.IndexOf("[\"F18CManual1Flare\"] =") + 22;
            int F18C_Manual1Flare_indexEnd = optionsLuaText.IndexOf(",", F18C_Manual1Flare_indexStart);
            string F18C_Manual1Flare_string = optionsLuaText.Substring(F18C_Manual1Flare_indexStart, F18C_Manual1Flare_indexEnd - F18C_Manual1Flare_indexStart);

            int F18C_Manual1Interval_indexStart = optionsLuaText.IndexOf("[\"F18CManual1Interval\"] =") + 25;
            int F18C_Manual1Interval_indexEnd = optionsLuaText.IndexOf(",", F18C_Manual1Interval_indexStart);
            string F18C_Manual1Interval_string = optionsLuaText.Substring(F18C_Manual1Interval_indexStart, F18C_Manual1Interval_indexEnd - F18C_Manual1Interval_indexStart);

            int F18C_Manual1Cycle_indexStart = optionsLuaText.IndexOf("[\"F18CManual1Cycle\"] =") + 22;
            int F18C_Manual1Cycle_indexEnd = optionsLuaText.IndexOf(",", F18C_Manual1Cycle_indexStart);
            string F18C_Manual1Cycle_string = optionsLuaText.Substring(F18C_Manual1Cycle_indexStart, F18C_Manual1Cycle_indexEnd - F18C_Manual1Cycle_indexStart);


            int F18C_Manual2Chaff_indexStart = optionsLuaText.IndexOf("[\"F18CManual2Chaff\"] =") + 22;
            int F18C_Manual2Chaff_indexEnd = optionsLuaText.IndexOf(",", F18C_Manual2Chaff_indexStart);
            string F18C_Manual2Chaff_string = optionsLuaText.Substring(F18C_Manual2Chaff_indexStart, F18C_Manual2Chaff_indexEnd - F18C_Manual2Chaff_indexStart);

            int F18C_Manual2Flare_indexStart = optionsLuaText.IndexOf("[\"F18CManual2Flare\"] =") + 22;
            int F18C_Manual2Flare_indexEnd = optionsLuaText.IndexOf(",", F18C_Manual2Flare_indexStart);
            string F18C_Manual2Flare_string = optionsLuaText.Substring(F18C_Manual2Flare_indexStart, F18C_Manual2Flare_indexEnd - F18C_Manual2Flare_indexStart);

            int F18C_Manual2Interval_indexStart = optionsLuaText.IndexOf("[\"F18CManual2Interval\"] =") + 25;
            int F18C_Manual2Interval_indexEnd = optionsLuaText.IndexOf(",", F18C_Manual2Interval_indexStart);
            string F18C_Manual2Interval_string = optionsLuaText.Substring(F18C_Manual2Interval_indexStart, F18C_Manual2Interval_indexEnd - F18C_Manual2Interval_indexStart);

            int F18C_Manual2Cycle_indexStart = optionsLuaText.IndexOf("[\"F18CManual2Cycle\"] =") + 22;
            int F18C_Manual2Cycle_indexEnd = optionsLuaText.IndexOf(",", F18C_Manual2Cycle_indexStart);
            string F18C_Manual2Cycle_string = optionsLuaText.Substring(F18C_Manual2Cycle_indexStart, F18C_Manual2Cycle_indexEnd - F18C_Manual2Cycle_indexStart);


            int F18C_Manual3Chaff_indexStart = optionsLuaText.IndexOf("[\"F18CManual3Chaff\"] =") + 22;
            int F18C_Manual3Chaff_indexEnd = optionsLuaText.IndexOf(",", F18C_Manual3Chaff_indexStart);
            string F18C_Manual3Chaff_string = optionsLuaText.Substring(F18C_Manual3Chaff_indexStart, F18C_Manual3Chaff_indexEnd - F18C_Manual3Chaff_indexStart);

            int F18C_Manual3Flare_indexStart = optionsLuaText.IndexOf("[\"F18CManual3Flare\"] =") + 22;
            int F18C_Manual3Flare_indexEnd = optionsLuaText.IndexOf(",", F18C_Manual3Flare_indexStart);
            string F18C_Manual3Flare_string = optionsLuaText.Substring(F18C_Manual3Flare_indexStart, F18C_Manual3Flare_indexEnd - F18C_Manual3Flare_indexStart);

            int F18C_Manual3Interval_indexStart = optionsLuaText.IndexOf("[\"F18CManual3Interval\"] =") + 25;
            int F18C_Manual3Interval_indexEnd = optionsLuaText.IndexOf(",", F18C_Manual3Interval_indexStart);
            string F18C_Manual3Interval_string = optionsLuaText.Substring(F18C_Manual3Interval_indexStart, F18C_Manual3Interval_indexEnd - F18C_Manual3Interval_indexStart);

            int F18C_Manual3Cycle_indexStart = optionsLuaText.IndexOf("[\"F18CManual3Cycle\"] =") + 22;
            int F18C_Manual3Cycle_indexEnd = optionsLuaText.IndexOf(",", F18C_Manual3Cycle_indexStart);
            string F18C_Manual3Cycle_string = optionsLuaText.Substring(F18C_Manual3Cycle_indexStart, F18C_Manual3Cycle_indexEnd - F18C_Manual3Cycle_indexStart);


            int F18C_Manual4Chaff_indexStart = optionsLuaText.IndexOf("[\"F18CManual4Chaff\"] =") + 22;
            int F18C_Manual4Chaff_indexEnd = optionsLuaText.IndexOf(",", F18C_Manual4Chaff_indexStart);
            string F18C_Manual4Chaff_string = optionsLuaText.Substring(F18C_Manual4Chaff_indexStart, F18C_Manual4Chaff_indexEnd - F18C_Manual4Chaff_indexStart);

            int F18C_Manual4Flare_indexStart = optionsLuaText.IndexOf("[\"F18CManual4Flare\"] =") + 22;
            int F18C_Manual4Flare_indexEnd = optionsLuaText.IndexOf(",", F18C_Manual4Flare_indexStart);
            string F18C_Manual4Flare_string = optionsLuaText.Substring(F18C_Manual4Flare_indexStart, F18C_Manual4Flare_indexEnd - F18C_Manual4Flare_indexStart);

            int F18C_Manual4Interval_indexStart = optionsLuaText.IndexOf("[\"F18CManual4Interval\"] =") + 25;
            int F18C_Manual4Interval_indexEnd = optionsLuaText.IndexOf(",", F18C_Manual4Interval_indexStart);
            string F18C_Manual4Interval_string = optionsLuaText.Substring(F18C_Manual4Interval_indexStart, F18C_Manual4Interval_indexEnd - F18C_Manual4Interval_indexStart);

            int F18C_Manual4Cycle_indexStart = optionsLuaText.IndexOf("[\"F18CManual4Cycle\"] =") + 22;
            int F18C_Manual4Cycle_indexEnd = optionsLuaText.IndexOf(",", F18C_Manual4Cycle_indexStart);
            string F18C_Manual4Cycle_string = optionsLuaText.Substring(F18C_Manual4Cycle_indexStart, F18C_Manual4Cycle_indexEnd - F18C_Manual4Cycle_indexStart);


            int F18C_Manual5Chaff_indexStart = optionsLuaText.IndexOf("[\"F18CManual5Chaff\"] =") + 22;
            int F18C_Manual5Chaff_indexEnd = optionsLuaText.IndexOf(",", F18C_Manual5Chaff_indexStart);
            string F18C_Manual5Chaff_string = optionsLuaText.Substring(F18C_Manual5Chaff_indexStart, F18C_Manual5Chaff_indexEnd - F18C_Manual5Chaff_indexStart);

            int F18C_Manual5Flare_indexStart = optionsLuaText.IndexOf("[\"F18CManual5Flare\"] =") + 22;
            int F18C_Manual5Flare_indexEnd = optionsLuaText.IndexOf(",", F18C_Manual5Flare_indexStart);
            string F18C_Manual5Flare_string = optionsLuaText.Substring(F18C_Manual5Flare_indexStart, F18C_Manual5Flare_indexEnd - F18C_Manual5Flare_indexStart);

            int F18C_Manual5Interval_indexStart = optionsLuaText.IndexOf("[\"F18CManual5Interval\"] =") + 25;
            int F18C_Manual5Interval_indexEnd = optionsLuaText.IndexOf(",", F18C_Manual5Interval_indexStart);
            string F18C_Manual5Interval_string = optionsLuaText.Substring(F18C_Manual5Interval_indexStart, F18C_Manual5Interval_indexEnd - F18C_Manual5Interval_indexStart);

            int F18C_Manual5Cycle_indexStart = optionsLuaText.IndexOf("[\"F18CManual5Cycle\"] =") + 22;
            int F18C_Manual5Cycle_indexEnd = optionsLuaText.IndexOf(",", F18C_Manual5Cycle_indexStart);
            string F18C_Manual5Cycle_string = optionsLuaText.Substring(F18C_Manual5Cycle_indexStart, F18C_Manual5Cycle_indexEnd - F18C_Manual5Cycle_indexStart);


            int F18C_Manual6Chaff_indexStart = optionsLuaText.IndexOf("[\"F18CManual6Chaff\"] =") + 22;
            int F18C_Manual6Chaff_indexEnd = optionsLuaText.IndexOf(",", F18C_Manual6Chaff_indexStart);
            string F18C_Manual6Chaff_string = optionsLuaText.Substring(F18C_Manual6Chaff_indexStart, F18C_Manual6Chaff_indexEnd - F18C_Manual6Chaff_indexStart);

            int F18C_Manual6Flare_indexStart = optionsLuaText.IndexOf("[\"F18CManual6Flare\"] =") + 22;
            int F18C_Manual6Flare_indexEnd = optionsLuaText.IndexOf(",", F18C_Manual6Flare_indexStart);
            string F18C_Manual6Flare_string = optionsLuaText.Substring(F18C_Manual6Flare_indexStart, F18C_Manual6Flare_indexEnd - F18C_Manual6Flare_indexStart);

            int F18C_Manual6Interval_indexStart = optionsLuaText.IndexOf("[\"F18CManual6Interval\"] =") + 25;
            int F18C_Manual6Interval_indexEnd = optionsLuaText.IndexOf(",", F18C_Manual6Interval_indexStart);
            string F18C_Manual6Interval_string = optionsLuaText.Substring(F18C_Manual6Interval_indexStart, F18C_Manual6Interval_indexEnd - F18C_Manual6Interval_indexStart);

            int F18C_Manual6Cycle_indexStart = optionsLuaText.IndexOf("[\"F18CManual6Cycle\"] =") + 22;
            int F18C_Manual6Cycle_indexEnd = optionsLuaText.IndexOf(",", F18C_Manual6Cycle_indexStart);
            string F18C_Manual6Cycle_string = optionsLuaText.Substring(F18C_Manual6Cycle_indexStart, F18C_Manual6Cycle_indexEnd - F18C_Manual6Cycle_indexStart);


            int F18C_AutoOldGenSamChaff_indexStart = optionsLuaText.IndexOf("[\"F18COldGenSamChaff\"] =") + 24;
            int F18C_AutoOldGenSamChaff_indexEnd = optionsLuaText.IndexOf(",", F18C_AutoOldGenSamChaff_indexStart);
            string F18C_AutoOldGenSamChaff_string = optionsLuaText.Substring(F18C_AutoOldGenSamChaff_indexStart, F18C_AutoOldGenSamChaff_indexEnd - F18C_AutoOldGenSamChaff_indexStart);

            int F18C_AutoOldGenSamFlare_indexStart = optionsLuaText.IndexOf("[\"F18COldGenSamFlare\"] =") + 24;
            int F18C_AutoOldGenSamFlare_indexEnd = optionsLuaText.IndexOf(",", F18C_AutoOldGenSamFlare_indexStart);
            string F18C_AutoOldGenSamFlare_string = optionsLuaText.Substring(F18C_AutoOldGenSamFlare_indexStart, F18C_AutoOldGenSamFlare_indexEnd - F18C_AutoOldGenSamFlare_indexStart);

            int F18C_AutoOldGenSamInterval_indexStart = optionsLuaText.IndexOf("[\"F18COldGenSamInterval\"] =") + 27;
            int F18C_AutoOldGenSamInterval_indexEnd = optionsLuaText.IndexOf(",", F18C_AutoOldGenSamInterval_indexStart);
            string F18C_AutoOldGenSamInterval_string = optionsLuaText.Substring(F18C_AutoOldGenSamInterval_indexStart, F18C_AutoOldGenSamInterval_indexEnd - F18C_AutoOldGenSamInterval_indexStart);

            int F18C_AutoOldGenSamCycle_indexStart = optionsLuaText.IndexOf("[\"F18COldGenSamCycle\"] =") + 24;
            int F18C_AutoOldGenSamCycle_indexEnd = optionsLuaText.IndexOf(",", F18C_AutoOldGenSamCycle_indexStart);
            string F18C_AutoOldGenSamCycle_string = optionsLuaText.Substring(F18C_AutoOldGenSamCycle_indexStart, F18C_AutoOldGenSamCycle_indexEnd - F18C_AutoOldGenSamCycle_indexStart);


            int F18C_AutoCurrentGenSamChaff_indexStart = optionsLuaText.IndexOf("[\"F18CCurrentGenSamChaff\"] =") + 28;
            int F18C_AutoCurrentGenSamChaff_indexEnd = optionsLuaText.IndexOf(",", F18C_AutoCurrentGenSamChaff_indexStart);
            string F18C_AutoCurrentGenSamChaff_string = optionsLuaText.Substring(F18C_AutoCurrentGenSamChaff_indexStart, F18C_AutoCurrentGenSamChaff_indexEnd - F18C_AutoCurrentGenSamChaff_indexStart);

            int F18C_AutoCurrentGenSamFlare_indexStart = optionsLuaText.IndexOf("[\"F18CCurrentGenSamFlare\"] =") + 28;
            int F18C_AutoCurrentGenSamFlare_indexEnd = optionsLuaText.IndexOf(",", F18C_AutoCurrentGenSamFlare_indexStart);
            string F18C_AutoCurrentGenSamFlare_string = optionsLuaText.Substring(F18C_AutoCurrentGenSamFlare_indexStart, F18C_AutoCurrentGenSamFlare_indexEnd - F18C_AutoCurrentGenSamFlare_indexStart);

            int F18C_AutoCurrentGenSamInterval_indexStart = optionsLuaText.IndexOf("[\"F18CCurrentGenSamInterval\"] =") + 31;
            int F18C_AutoCurrentGenSamInterval_indexEnd = optionsLuaText.IndexOf(",", F18C_AutoCurrentGenSamInterval_indexStart);
            string F18C_AutoCurrentGenSamInterval_string = optionsLuaText.Substring(F18C_AutoCurrentGenSamInterval_indexStart, F18C_AutoCurrentGenSamInterval_indexEnd - F18C_AutoCurrentGenSamInterval_indexStart);

            int F18C_AutoCurrentGenSamCycle_indexStart = optionsLuaText.IndexOf("[\"F18CCurrentGenSamCycle\"] =") + 28;
            int F18C_AutoCurrentGenSamCycle_indexEnd = optionsLuaText.IndexOf(",", F18C_AutoCurrentGenSamCycle_indexStart);
            string F18C_AutoCurrentGenSamCycle_string = optionsLuaText.Substring(F18C_AutoCurrentGenSamCycle_indexStart, F18C_AutoCurrentGenSamCycle_indexEnd - F18C_AutoCurrentGenSamCycle_indexStart);


            int F18C_AutoIrSamChaff_indexStart = optionsLuaText.IndexOf("[\"F18CIrSamChaff\"] =") + 20;
            int F18C_AutoIrSamChaff_indexEnd = optionsLuaText.IndexOf(",", F18C_AutoIrSamChaff_indexStart);
            string F18C_AutoIrSamChaff_string = optionsLuaText.Substring(F18C_AutoIrSamChaff_indexStart, F18C_AutoIrSamChaff_indexEnd - F18C_AutoIrSamChaff_indexStart);

            int F18C_AutoIrSamFlare_indexStart = optionsLuaText.IndexOf("[\"F18CIrSamFlare\"] =") + 20;
            int F18C_AutoIrSamFlare_indexEnd = optionsLuaText.IndexOf(",", F18C_AutoIrSamFlare_indexStart);
            string F18C_AutoIrSamFlare_string = optionsLuaText.Substring(F18C_AutoIrSamFlare_indexStart, F18C_AutoIrSamFlare_indexEnd - F18C_AutoIrSamFlare_indexStart);

            int F18C_AutoIrSamInterval_indexStart = optionsLuaText.IndexOf("[\"F18CIrSamInterval\"] =") + 23;
            int F18C_AutoIrSamInterval_indexEnd = optionsLuaText.IndexOf(",", F18C_AutoIrSamInterval_indexStart);
            string F18C_AutoIrSamInterval_string = optionsLuaText.Substring(F18C_AutoIrSamInterval_indexStart, F18C_AutoIrSamInterval_indexEnd - F18C_AutoIrSamInterval_indexStart);

            int F18C_AutoIrSamCycle_indexStart = optionsLuaText.IndexOf("[\"F18CIrSamCycle\"] =") + 20;
            int F18C_AutoIrSamCycle_indexEnd = optionsLuaText.IndexOf(",", F18C_AutoIrSamCycle_indexStart);
            string F18C_AutoIrSamCycle_string = optionsLuaText.Substring(F18C_AutoIrSamCycle_indexStart, F18C_AutoIrSamCycle_indexEnd - F18C_AutoIrSamCycle_indexStart);


            //MessageBox.Show("|" + F18C_Manual1Chaff_string + "|" + F18C_Manual1Flare_string + "|" + F18C_Manual1Interval_string + "|" + F18C_Manual1Cycle_string + "|");



            //then you merge that info with the formated cms.lua and export it to the proper folder location

            string[] luaExportString = { "local count = 0",
                "local function counter()",
                "    count = count + 1",
                "    return count",
                "end",
                "",
                "ProgramNames =",
                "{",
                "    MAN_1 = counter(),",
                "    MAN_2 = counter(),",
                "    MAN_3 = counter(),",
                "    MAN_4 = counter(),",
                "    MAN_5 = counter(),",
                "    MAN_6 = counter(),",
                "    AUTO_1 = counter(),",
                "    AUTO_2 = counter(),",
                "    AUTO_3 = counter(),",
                "    AUTO_4 = counter(),",
                "    AUTO_5 = counter(),",
                "    AUTO_6 = counter()",
                "}",
                "",
                "",
                "programs = {}",
                "",
                "-- Default manual presets",
                "-- MAN 1",
                "programs[ProgramNames.MAN_1] = {}",
                "programs[ProgramNames.MAN_1][\"chaff\"] = " + F18C_Manual1Chaff_string,
                "programs[ProgramNames.MAN_1][\"flare\"] = " + F18C_Manual1Flare_string,
                "programs[ProgramNames.MAN_1][\"intv\"]  = " + F18C_Manual1Interval_string,
                "programs[ProgramNames.MAN_1][\"cycle\"] = " + F18C_Manual1Cycle_string,
                "",
                "-- MAN 2",
                "programs[ProgramNames.MAN_2] = {}",
                "programs[ProgramNames.MAN_2][\"chaff\"] = " + F18C_Manual2Chaff_string,
                "programs[ProgramNames.MAN_2][\"flare\"] = " + F18C_Manual2Flare_string,
                "programs[ProgramNames.MAN_2][\"intv\"]  = " + F18C_Manual2Interval_string,
                "programs[ProgramNames.MAN_2][\"cycle\"] = " + F18C_Manual2Cycle_string,
                "",
                "-- MAN 3",
                "programs[ProgramNames.MAN_3] = {}",
                "programs[ProgramNames.MAN_3][\"chaff\"] = " + F18C_Manual3Chaff_string,
                "programs[ProgramNames.MAN_3][\"flare\"] = " + F18C_Manual3Flare_string,
                "programs[ProgramNames.MAN_3][\"intv\"]  = " + F18C_Manual3Interval_string,
                "programs[ProgramNames.MAN_3][\"cycle\"] = " + F18C_Manual3Cycle_string,
                "",
                "-- MAN 4",
                "programs[ProgramNames.MAN_4] = {}",
                "programs[ProgramNames.MAN_4][\"chaff\"] = " + F18C_Manual4Chaff_string,
                "programs[ProgramNames.MAN_4][\"flare\"] = " + F18C_Manual4Flare_string,
                "programs[ProgramNames.MAN_4][\"intv\"]  = " + F18C_Manual4Interval_string,
                "programs[ProgramNames.MAN_4][\"cycle\"] = " + F18C_Manual4Cycle_string,
                "",
                "-- MAN 5 - Chaff single",
                "programs[ProgramNames.MAN_5] = {}",
                "programs[ProgramNames.MAN_5][\"chaff\"] = " + F18C_Manual5Chaff_string,
                "programs[ProgramNames.MAN_5][\"flare\"] = " + F18C_Manual5Flare_string,
                "programs[ProgramNames.MAN_5][\"intv\"]  = " + F18C_Manual5Interval_string,
                "programs[ProgramNames.MAN_5][\"cycle\"] = " + F18C_Manual5Cycle_string,
                "",
                "-- MAN 6 - Wall Dispense button, Panic",
                "programs[ProgramNames.MAN_6] = {}",
                "programs[ProgramNames.MAN_6][\"chaff\"] = " + F18C_Manual6Chaff_string,
                "programs[ProgramNames.MAN_6][\"flare\"] = " + F18C_Manual6Flare_string,
                "programs[ProgramNames.MAN_6][\"intv\"]  = " + F18C_Manual6Interval_string,
                "programs[ProgramNames.MAN_6][\"cycle\"] = " + F18C_Manual6Cycle_string,
                "",
                "-- Auto presets",
                "-- Old generation radar SAM",
                "programs[ProgramNames.AUTO_1] = {}",
                "programs[ProgramNames.AUTO_1][\"chaff\"] = " + F18C_AutoOldGenSamChaff_string,
                "programs[ProgramNames.AUTO_1][\"flare\"] = " + F18C_AutoOldGenSamFlare_string,
                "programs[ProgramNames.AUTO_1][\"intv\"]  = " + F18C_AutoOldGenSamInterval_string,
                "programs[ProgramNames.AUTO_1][\"cycle\"] = " + F18C_AutoOldGenSamCycle_string,
                "",
                "-- Current generation radar SAM",
                "programs[ProgramNames.AUTO_2] = {}",
                "programs[ProgramNames.AUTO_2][\"chaff\"] = " + F18C_AutoCurrentGenSamChaff_string,
                "programs[ProgramNames.AUTO_2][\"flare\"] = " + F18C_AutoCurrentGenSamFlare_string,
                "programs[ProgramNames.AUTO_2][\"intv\"]  = " + F18C_AutoCurrentGenSamInterval_string,
                "programs[ProgramNames.AUTO_2][\"cycle\"] = " + F18C_AutoCurrentGenSamCycle_string,
                "",
                "-- IR SAM",
                "programs[ProgramNames.AUTO_3] = {}",
                "programs[ProgramNames.AUTO_3][\"chaff\"] = " + F18C_AutoIrSamChaff_string,
                "programs[ProgramNames.AUTO_3][\"flare\"] = " + F18C_AutoIrSamFlare_string,
                "programs[ProgramNames.AUTO_3][\"intv\"]  = " + F18C_AutoIrSamInterval_string,
                "programs[ProgramNames.AUTO_3][\"cycle\"] = " + F18C_AutoIrSamCycle_string,
                "",
                "",
                "need_to_be_closed = true -- lua_state  will be closed in post_initialize()" };
                //"--Exported via DiCE by Bailey " + System.DateTime.Now};

            System.IO.Directory.CreateDirectory(cmdsLua_F18C_FolderPath);

            try
            {
                System.IO.File.WriteAllLines(cmdsLua_F18C_fullPath, luaExportString);
                //https://stackoverflow.com/questions/5920882/file-move-does-not-work-file-already-exists
                //playCompleteSound();

                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "F-18C CMS file exported.");
                richTextBox_log.ScrollToEnd();
            }
            catch (IOException g)
            {
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "DiCE could not write the F-18C CMS lua.");
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + g.Message);
                richTextBox_log.ScrollToEnd();
            }
        }


        //TODO: Update this because DCS update 2.7.9.17830 on DEC 23rd added a BYPASS section
        //Apparently the CMDS luas fail on trying to parse them because they are not parseable bc they are not fully the correct format
        //if it was parseablee, the data transfer would look something like this
        //cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_1]"]["chaff"]["burstQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual1ChaffBurstQty"].GetDoubleLenient();

        private void readAndExportF16Data()//similar to 'readAndExportF18Data()'
        {
           
            var optionsLuaText = LsonVars.Parse(File.ReadAllText(userOptionsLua_Full_pathWithExtention));//Lson the options lua

            Double F16CManual1ChaffBurstQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual1ChaffBurstQty"].GetDoubleLenient();
            Double F16CManual1ChaffBurstIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual1ChaffBurstIntv"].GetDoubleLenient();
            Double F16CManual1ChaffSalvoQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual1ChaffSalvoQty"].GetDoubleLenient();
            Double F16CManual1ChaffSalvoIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual1ChaffSalvoIntv"].GetDoubleLenient();

            Double F16CManual2ChaffBurstQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual2ChaffBurstQty"].GetDoubleLenient();
            Double F16CManual2ChaffBurstIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual2ChaffBurstIntv"].GetDoubleLenient();
            Double F16CManual2ChaffSalvoQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual2ChaffSalvoQty"].GetDoubleLenient();
            Double F16CManual2ChaffSalvoIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual2ChaffSalvoIntv"].GetDoubleLenient();

            Double F16CManual3ChaffBurstQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual3ChaffBurstQty"].GetDoubleLenient();
            Double F16CManual3ChaffBurstIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual3ChaffBurstIntv"].GetDoubleLenient();
            Double F16CManual3ChaffSalvoQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual3ChaffSalvoQty"].GetDoubleLenient();
            Double F16CManual3ChaffSalvoIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual3ChaffSalvoIntv"].GetDoubleLenient();

            Double F16CManual4ChaffBurstQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual4ChaffBurstQty"].GetDoubleLenient();
            Double F16CManual4ChaffBurstIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual4ChaffBurstIntv"].GetDoubleLenient();
            Double F16CManual4ChaffSalvoQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual4ChaffSalvoQty"].GetDoubleLenient();
            Double F16CManual4ChaffSalvoIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual4ChaffSalvoIntv"].GetDoubleLenient();

            Double F16CManual5ChaffBurstQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual5ChaffBurstQty"].GetDoubleLenient();
            Double F16CManual5ChaffBurstIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual5ChaffBurstIntv"].GetDoubleLenient();
            Double F16CManual5ChaffSalvoQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual5ChaffSalvoQty"].GetDoubleLenient();
            Double F16CManual5ChaffSalvoIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual5ChaffSalvoIntv"].GetDoubleLenient();

            Double F16CManual6ChaffBurstQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual6ChaffBurstQty"].GetDoubleLenient();
            Double F16CManual6ChaffBurstIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual6ChaffBurstIntv"].GetDoubleLenient();
            Double F16CManual6ChaffSalvoQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual6ChaffSalvoQty"].GetDoubleLenient();
            Double F16CManual6ChaffSalvoIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual6ChaffSalvoIntv"].GetDoubleLenient();


            Double F16CAuto1ChaffBurstQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto1ChaffBurstQty"].GetDoubleLenient();
            Double F16CAuto1ChaffBurstIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto1ChaffBurstIntv"].GetDoubleLenient();
            Double F16CAuto1ChaffSalvoQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto1ChaffSalvoQty"].GetDoubleLenient();
            Double F16CAuto1ChaffSalvoIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto1ChaffSalvoIntv"].GetDoubleLenient();

            Double F16CAuto2ChaffBurstQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto2ChaffBurstQty"].GetDoubleLenient();
            Double F16CAuto2ChaffBurstIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto2ChaffBurstIntv"].GetDoubleLenient();
            Double F16CAuto2ChaffSalvoQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto2ChaffSalvoQty"].GetDoubleLenient();
            Double F16CAuto2ChaffSalvoIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto2ChaffSalvoIntv"].GetDoubleLenient();

            Double F16CAuto3ChaffBurstQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto3ChaffBurstQty"].GetDoubleLenient();
            Double F16CAuto3ChaffBurstIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto3ChaffBurstIntv"].GetDoubleLenient();
            Double F16CAuto3ChaffSalvoQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto3ChaffSalvoQty"].GetDoubleLenient();
            Double F16CAuto3ChaffSalvoIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto3ChaffSalvoIntv"].GetDoubleLenient();

            //TODO: Integrate this if requested
            /*
            Double F16CBypassChaffBurstQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CBypassChaffBurstQty"].GetDoubleLenient();
            Double F16CBypassChaffBurstIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CBypassChaffBurstIntv"].GetDoubleLenient();
            Double F16CBypassChaffSalvoQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CBypassChaffSalvoQty"].GetDoubleLenient();
            Double F16CBypassChaffSalvoIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CBypassChaffSalvoIntv"].GetDoubleLenient();
            */

            Double F16CBypassChaffBurstQty = 1;
            Double F16CBypassChaffBurstIntv = 0.02;
            Double F16CBypassChaffSalvoQty = 1;
            Double F16CBypassChaffSalvoIntv = 0.5;


            //flares
            Double F16CManual1FlareBurstQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual1FlareBurstQty"].GetDoubleLenient();
            Double F16CManual1FlareBurstIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual1FlareBurstIntv"].GetDoubleLenient();
            Double F16CManual1FlareSalvoQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual1FlareSalvoQty"].GetDoubleLenient();
            Double F16CManual1FlareSalvoIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual1FlareSalvoIntv"].GetDoubleLenient();

            Double F16CManual2FlareBurstQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual2FlareBurstQty"].GetDoubleLenient();
            Double F16CManual2FlareBurstIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual2FlareBurstIntv"].GetDoubleLenient();
            Double F16CManual2FlareSalvoQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual2FlareSalvoQty"].GetDoubleLenient();
            Double F16CManual2FlareSalvoIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual2FlareSalvoIntv"].GetDoubleLenient();

            Double F16CManual3FlareBurstQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual3FlareBurstQty"].GetDoubleLenient();
            Double F16CManual3FlareBurstIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual3FlareBurstIntv"].GetDoubleLenient();
            Double F16CManual3FlareSalvoQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual3FlareSalvoQty"].GetDoubleLenient();
            Double F16CManual3FlareSalvoIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual3FlareSalvoIntv"].GetDoubleLenient();

            Double F16CManual4FlareBurstQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual4FlareBurstQty"].GetDoubleLenient();
            Double F16CManual4FlareBurstIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual4FlareBurstIntv"].GetDoubleLenient();
            Double F16CManual4FlareSalvoQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual4FlareSalvoQty"].GetDoubleLenient();
            Double F16CManual4FlareSalvoIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual4FlareSalvoIntv"].GetDoubleLenient();

            Double F16CManual5FlareBurstQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual5FlareBurstQty"].GetDoubleLenient();
            Double F16CManual5FlareBurstIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual5FlareBurstIntv"].GetDoubleLenient();
            Double F16CManual5FlareSalvoQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual5FlareSalvoQty"].GetDoubleLenient();
            Double F16CManual5FlareSalvoIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual5FlareSalvoIntv"].GetDoubleLenient();

            Double F16CManual6FlareBurstQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual6FlareBurstQty"].GetDoubleLenient();
            Double F16CManual6FlareBurstIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual6FlareBurstIntv"].GetDoubleLenient();
            Double F16CManual6FlareSalvoQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual6FlareSalvoQty"].GetDoubleLenient();
            Double F16CManual6FlareSalvoIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual6FlareSalvoIntv"].GetDoubleLenient();


            Double F16CAuto1FlareBurstQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto1FlareBurstQty"].GetDoubleLenient();
            Double F16CAuto1FlareBurstIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto1FlareBurstIntv"].GetDoubleLenient();
            Double F16CAuto1FlareSalvoQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto1FlareSalvoQty"].GetDoubleLenient();
            Double F16CAuto1FlareSalvoIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto1FlareSalvoIntv"].GetDoubleLenient();

            Double F16CAuto2FlareBurstQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto2FlareBurstQty"].GetDoubleLenient();
            Double F16CAuto2FlareBurstIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto2FlareBurstIntv"].GetDoubleLenient();
            Double F16CAuto2FlareSalvoQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto2FlareSalvoQty"].GetDoubleLenient();
            Double F16CAuto2FlareSalvoIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto2FlareSalvoIntv"].GetDoubleLenient();

            Double F16CAuto3FlareBurstQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto3FlareBurstQty"].GetDoubleLenient();
            Double F16CAuto3FlareBurstIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto3FlareBurstIntv"].GetDoubleLenient();
            Double F16CAuto3FlareSalvoQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto3FlareSalvoQty"].GetDoubleLenient();
            Double F16CAuto3FlareSalvoIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto3FlareSalvoIntv"].GetDoubleLenient();

            //TODO: Integrate this if requested
            /*
            Double F16CBypassFlareBurstQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CBypassFlareBurstQty"].GetDoubleLenient();
            Double F16CBypassFlareBurstIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CBypassFlareBurstIntv"].GetDoubleLenient();
            Double F16CBypassFlareSalvoQty = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CBypassFlareSalvoQty"].GetDoubleLenient();
            Double F16CBypassFlareSalvoIntv = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CBypassFlareSalvoIntv"].GetDoubleLenient();
            */

            Double F16CBypassFlareBurstQty = 1;
            Double F16CBypassFlareBurstIntv = 0.02;
            Double F16CBypassFlareSalvoQty = 1;
            Double F16CBypassFlareSalvoIntv = 0.5;

            //future code if you figgure out how to parse the CMDS lua fie with LSON
            //try

            //{

            /*
            //now we are going to take the falues from the options Lua and put them into the f16CMDS.lua, directly

            // example: Double M2000CManual01Chaff = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual01Chaff"].GetDoubleLenient();

            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_1]"]["chaff"]["burstQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual1ChaffBurstQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_1]"]["chaff"]["burstIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual1ChaffBurstIntv"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_1]"]["chaff"]["salvoQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual1ChaffSalvoQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_1]"]["chaff"]["salvoIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual1ChaffSalvoIntv"].GetDoubleLenient();

            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_1]"]["flare"]["burstQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual1FlareBurstQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_1]"]["flare"]["burstIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual1FlareBurstIntv"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_1]"]["flare"]["salvoQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual1FlareSalvoQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_1]"]["flare"]["salvoIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual1FlareSalvoIntv"].GetDoubleLenient();


            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_2]"]["chaff"]["burstQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual2ChaffBurstQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_2]"]["chaff"]["burstIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual2ChaffBurstIntv"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_2]"]["chaff"]["salvoQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual2ChaffSalvoQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_2]"]["chaff"]["salvoIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual2ChaffSalvoIntv"].GetDoubleLenient();

            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_2]"]["flare"]["burstQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual2FlareBurstQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_2]"]["flare"]["burstIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual2FlareBurstIntv"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_2]"]["flare"]["salvoQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual2FlareSalvoQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_2]"]["flare"]["salvoIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual2FlareSalvoIntv"].GetDoubleLenient();


            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_3]"]["chaff"]["burstQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual3ChaffBurstQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_3]"]["chaff"]["burstIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual3ChaffBurstIntv"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_3]"]["chaff"]["salvoQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual3ChaffSalvoQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_3]"]["chaff"]["salvoIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual3ChaffSalvoIntv"].GetDoubleLenient();

            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_3]"]["flare"]["burstQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual3FlareBurstQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_3]"]["flare"]["burstIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual3FlareBurstIntv"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_3]"]["flare"]["salvoQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual3FlareSalvoQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_3]"]["flare"]["salvoIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual3FlareSalvoIntv"].GetDoubleLenient();


            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_4]"]["chaff"]["burstQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual4ChaffBurstQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_4]"]["chaff"]["burstIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual4ChaffBurstIntv"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_4]"]["chaff"]["salvoQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual4ChaffSalvoQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_4]"]["chaff"]["salvoIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual4ChaffSalvoIntv"].GetDoubleLenient();

            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_4]"]["flare"]["burstQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual4FlareBurstQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_4]"]["flare"]["burstIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual4FlareBurstIntv"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_4]"]["flare"]["salvoQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual4FlareSalvoQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_4]"]["flare"]["salvoIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual4FlareSalvoIntv"].GetDoubleLenient();


            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_5]"]["chaff"]["burstQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual5ChaffBurstQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_5]"]["chaff"]["burstIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual5ChaffBurstIntv"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_5]"]["chaff"]["salvoQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual5ChaffSalvoQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_5]"]["chaff"]["salvoIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual5ChaffSalvoIntv"].GetDoubleLenient();

            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_5]"]["flare"]["burstQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual5FlareBurstQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_5]"]["flare"]["burstIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual5FlareBurstIntv"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_5]"]["flare"]["salvoQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual5FlareSalvoQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_5]"]["flare"]["salvoIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual5FlareSalvoIntv"].GetDoubleLenient();


            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_6]"]["chaff"]["burstQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual6ChaffBurstQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_6]"]["chaff"]["burstIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual6ChaffBurstIntv"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_6]"]["chaff"]["salvoQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual6ChaffSalvoQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_6]"]["chaff"]["salvoIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual6ChaffSalvoIntv"].GetDoubleLenient();

            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_6]"]["flare"]["burstQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual6FlareBurstQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_6]"]["flare"]["burstIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual6FlareBurstIntv"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_6]"]["flare"]["salvoQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual6FlareSalvoQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.MAN_6]"]["flare"]["salvoIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CManual6FlareSalvoIntv"].GetDoubleLenient();


            //TODO: Remeber to rename these in the optionsDb.lua from Old/NewSam to Auto123 and Bypass
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.AUTO_1]"]["chaff"]["burstQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto1ChaffBurstQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.AUTO_1]"]["chaff"]["burstIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto1ChaffBurstIntv"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.AUTO_1]"]["chaff"]["salvoQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto1ChaffSalvoQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.AUTO_1]"]["chaff"]["salvoIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto1ChaffSalvoIntv"].GetDoubleLenient();

            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.AUTO_1]"]["flare"]["burstQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto1FlareBurstQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.AUTO_1]"]["flare"]["burstIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto1FlareBurstIntv"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.AUTO_1]"]["flare"]["salvoQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto1FlareSalvoQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.AUTO_1]"]["flare"]["salvoIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto1FlareSalvoIntv"].GetDoubleLenient();


            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.AUTO_2]"]["chaff"]["burstQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto2ChaffBurstQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.AUTO_2]"]["chaff"]["burstIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto2ChaffBurstIntv"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.AUTO_2]"]["chaff"]["salvoQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto2ChaffSalvoQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.AUTO_2]"]["chaff"]["salvoIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto2ChaffSalvoIntv"].GetDoubleLenient();

            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.AUTO_2]"]["flare"]["burstQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto2FlareBurstQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.AUTO_2]"]["flare"]["burstIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto2FlareBurstIntv"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.AUTO_2]"]["flare"]["salvoQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto2FlareSalvoQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.AUTO_2]"]["flare"]["salvoIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto2FlareSalvoIntv"].GetDoubleLenient();


            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.AUTO_3]"]["chaff"]["burstQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto3ChaffBurstQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.AUTO_3]"]["chaff"]["burstIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto3ChaffBurstIntv"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.AUTO_3]"]["chaff"]["salvoQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto3ChaffSalvoQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.AUTO_3]"]["chaff"]["salvoIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto3ChaffSalvoIntv"].GetDoubleLenient();

            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.AUTO_3]"]["flare"]["burstQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto3FlareBurstQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.AUTO_3]"]["flare"]["burstIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto3FlareBurstIntv"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.AUTO_3]"]["flare"]["salvoQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto3FlareSalvoQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.AUTO_3]"]["flare"]["salvoIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CAuto3FlareSalvoIntv"].GetDoubleLenient();


            */

            //TODO:Implement this in the Db and GUI in DCS
            /*
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.BYP]"]["chaff"]["burstQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CBypassChaffBurstQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.BYP]"]["chaff"]["burstIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CBypassChaffBurstIntv"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.BYP]"]["chaff"]["salvoQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CBypassChaffSalvoQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.BYP]"]["chaff"]["salvoIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CBypassChaffSalvoIntv"].GetDoubleLenient();

            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.BYP]"]["flare"]["burstQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CBypassFlareBurstQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.BYP]"]["flare"]["burstIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CBypassFlareBurstIntv"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.BYP]"]["flare"]["salvoQty"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CBypassFlareSalvoQty"].GetDoubleLenient();
            cmdsLua_F16C_fullPath_LsonParse["programs[ProgramNames.BYP]"]["flare"]["salvoIntv"] = optionsLuaText["options"]["plugins"]["DiCE F-16C"]["F16CBypassFlareSalvoIntv"].GetDoubleLenient();
            */

            //close close the F16CMDS.lua file to finalize the write

            //File.WriteAllText(cmdsLua_F16C_fullPath, LsonVars.ToString(cmdsLua_F16C_fullPath_LsonParse)); // serialize back to a file


            //https://stackoverflow.com/questions/5920882/file-move-does-not-work-file-already-exists
            //playCompleteSound();

            //richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "F-16C CMS file exported.");
            //    richTextBox_log.ScrollToEnd();
            //}
            //catch (IOException h)
            //{
            //    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "DiCE could not write the F-16C CMS lua.");
            //    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + h.Message);
            //    richTextBox_log.ScrollToEnd();
            //}

            
            string[] luaExportString = {"local gettext = require(\"i_18n\")",
                "_ = gettext.translate",
                "",
                "local count = 0",
                "local function counter()",
                "	count = count + 1",
                "	return count",
                "end",
                "",
                "ProgramNames =",
                "{",
                "	MAN_1 = counter(),",
                "	MAN_2 = counter(),",
                "	MAN_3 = counter(),",
                "	MAN_4 = counter(),",
                "	MAN_5 = counter(),",
                "	MAN_6 = counter(),",
                "	AUTO_1 = counter(),",
                "	AUTO_2 = counter(),",
                "	AUTO_3 = counter(),",
                "	BYP = 27,",
                "}",
                "",
                "programs = {}",
                "",
                "-- Default manual presets",
                "-- MAN 1",
                "programs[ProgramNames.MAN_1] = {",
                "	chaff = {",
                "		burstQty 	= " + F16CManual1ChaffBurstQty + ",",
                "		burstIntv	= " + F16CManual1ChaffBurstIntv + ",",
                "		salvoQty	= " + F16CManual1ChaffSalvoQty + ",",
                "		salvoIntv	= " + F16CManual1ChaffSalvoIntv + ",",
                "	},",
                "	flare = {",
                "		burstQty	= " + F16CManual1FlareBurstQty + ",",
                "		burstIntv	= " + F16CManual1FlareBurstIntv + ",",
                "		salvoQty	= " + F16CManual1FlareSalvoQty + ",",
                "		salvoIntv	= " + F16CManual1FlareSalvoIntv + ",",
                "	},",
                "}",
                "",
                "-- MAN 2",
                "programs[ProgramNames.MAN_2] = {",
                "	chaff = {",
                "		burstQty 	= " + F16CManual2ChaffBurstQty + ",",
                "		burstIntv	= " + F16CManual2ChaffBurstIntv + ",",
                "		salvoQty	= " + F16CManual2ChaffSalvoQty + ",",
                "		salvoIntv	= " + F16CManual2ChaffSalvoIntv + ",",
                "	},",
                "	flare = {",
                "		burstQty	= " + F16CManual2FlareBurstQty + ",",
                "		burstIntv	= " + F16CManual2FlareBurstIntv + ",",
                "		salvoQty	= " + F16CManual2FlareSalvoQty + ",",
                "		salvoIntv	= " + F16CManual2FlareSalvoIntv + ",",
                "	},",
                "}",
                "",
                "-- MAN 3",
                "programs[ProgramNames.MAN_3] = {",
                "	chaff = {",
                "		burstQty 	= " + F16CManual3ChaffBurstQty + ",",
                "		burstIntv	= " + F16CManual3ChaffBurstIntv + ",",
                "		salvoQty	= " + F16CManual3ChaffSalvoQty + ",",
                "		salvoIntv	= " + F16CManual3ChaffSalvoIntv + ",",
                "	},",
                "	flare = {",
                "		burstQty	= " + F16CManual3FlareBurstQty + ",",
                "		burstIntv	= " + F16CManual3FlareBurstIntv + ",",
                "		salvoQty	= " + F16CManual3FlareSalvoQty + ",",
                "		salvoIntv	= " + F16CManual3FlareSalvoIntv + ",",
                "	},",
                "}",
                "",
                "-- MAN 4",
                "programs[ProgramNames.MAN_4] = {",
                "	chaff = {",
                "		burstQty 	= " + F16CManual4ChaffBurstQty + ",",
                "		burstIntv	= " + F16CManual4ChaffBurstIntv + ",",
                "		salvoQty	= " + F16CManual4ChaffSalvoQty + ",",
                "		salvoIntv	= " + F16CManual4ChaffSalvoIntv + ",",
                "	},",
                "	flare = {",
                "		burstQty	= " + F16CManual4FlareBurstQty + ",",
                "		burstIntv	= " + F16CManual4FlareBurstIntv + ",",
                "		salvoQty	= " + F16CManual4FlareSalvoQty + ",",
                "		salvoIntv	= " + F16CManual4FlareSalvoIntv + ",",
                "	},",
                "}",
                "",
                "-- MAN 5 - Wall Dispense button, Panic",
                "programs[ProgramNames.MAN_5] = {",
                "	chaff = {",
                "		burstQty 	= " + F16CManual5ChaffBurstQty + ",",
                "		burstIntv	= " + F16CManual5ChaffBurstIntv + ",",
                "		salvoQty	= " + F16CManual5ChaffSalvoQty + ",",
                "		salvoIntv	= " + F16CManual5ChaffSalvoIntv + ",",
                "	},",
                "	flare = {",
                "		burstQty	= " + F16CManual5FlareBurstQty + ",",
                "		burstIntv	= " + F16CManual5FlareBurstIntv + ",",
                "		salvoQty	= " + F16CManual5FlareSalvoQty + ",",
                "		salvoIntv	= " + F16CManual5FlareSalvoIntv + ",",
                "	},",
                "}",
                "",
                "-- MAN 6 - CMS LEFT",
                "programs[ProgramNames.MAN_6] = {",
                "	chaff = {",
                "		burstQty 	= " + F16CManual6ChaffBurstQty + ",",
                "		burstIntv	= " + F16CManual6ChaffBurstIntv + ",",
                "		salvoQty	= " + F16CManual6ChaffSalvoQty + ",",
                "		salvoIntv	= " + F16CManual6ChaffSalvoIntv + ",",
                "	},",
                "	flare = {",
                "		burstQty	= " + F16CManual6FlareBurstQty + ",",
                "		burstIntv	= " + F16CManual6FlareBurstIntv + ",",
                "		salvoQty	= " + F16CManual6FlareSalvoQty + ",",
                "		salvoIntv	= " + F16CManual6FlareSalvoIntv + ",",
                "	},",
                "}",
                "",
                "-- Auto presets",
                "-- Threat type 1",
                "programs[ProgramNames.AUTO_1] = {",
                "	chaff = {",
                "		burstQty 	= " + F16CAuto1ChaffBurstQty + ",",
                "		burstIntv	= " + F16CAuto1ChaffBurstIntv + ",",
                "		salvoQty	= " + F16CAuto1ChaffSalvoQty + ",",
                "		salvoIntv	= " + F16CAuto1ChaffSalvoIntv + ",",
                "	},",
                "	flare = {",
                "		burstQty	= " + F16CAuto1FlareBurstQty + ",",
                "		burstIntv	= " + F16CAuto1FlareBurstIntv + ",",
                "		salvoQty	= " + F16CAuto1FlareSalvoQty + ",",
                "		salvoIntv	= " + F16CAuto1FlareSalvoIntv + ",",
                "	},",
                "}",
                "",
                "-- Threat type 2",
                "programs[ProgramNames.AUTO_2] = {",
                "	chaff = {",
                "		burstQty 	= " + F16CAuto2ChaffBurstQty + ",",
                "		burstIntv	= " + F16CAuto2ChaffBurstIntv + ",",
                "		salvoQty	= " + F16CAuto2ChaffSalvoQty + ",",
                "		salvoIntv	= " + F16CAuto2ChaffSalvoIntv + ",",
                "	},",
                "	flare = {",
                "		burstQty	= " + F16CAuto2FlareBurstQty + ",",
                "		burstIntv	= " + F16CAuto2FlareBurstIntv + ",",
                "		salvoQty	= " + F16CAuto2FlareSalvoQty + ",",
                "		salvoIntv	= " + F16CAuto2FlareSalvoIntv + ",",
                "	},",
                "}",
                "",
                "-- Threat type 3",
                "programs[ProgramNames.AUTO_3] = {",
                "	chaff = {",
                "		burstQty 	= " + F16CAuto3ChaffBurstQty + ",",
                "		burstIntv	= " + F16CAuto3ChaffBurstIntv + ",",
                "		salvoQty	= " + F16CAuto3ChaffSalvoQty + ",",
                "		salvoIntv	= " + F16CAuto3ChaffSalvoIntv + ",",
                "	},",
                "	flare = {",
                "		burstQty	= " + F16CAuto3FlareBurstQty + ",",
                "		burstIntv	= " + F16CAuto3FlareBurstIntv + ",",
                "		salvoQty	= " + F16CAuto3FlareSalvoQty + ",",
                "		salvoIntv	= " + F16CAuto3FlareSalvoIntv + ",",
                "	},",
                "}",
                "",
                "-- BYPASS",
                "programs[ProgramNames.BYP] = {",
                "	chaff = {",
                "		burstQty 	= " + F16CBypassChaffBurstQty + ",",
                "		burstIntv	= " + F16CBypassChaffBurstIntv + ",",
                "		salvoQty	= " + F16CBypassChaffSalvoQty + ",",
                "		salvoIntv	= " + F16CBypassChaffSalvoIntv + ",",
                "	},",
                "	flare = {",
                "		burstQty	= " + F16CBypassFlareBurstQty + ",",
                "		burstIntv	= " + F16CBypassFlareBurstIntv + ",",
                "		salvoQty	= " + F16CBypassFlareSalvoQty + ",",
                "		salvoIntv	= " + F16CBypassFlareSalvoIntv + ",",
                "	},",
                "}",
                "",
                "AN_ALE_47_FAILURE_TOTAL = 0",
                "AN_ALE_47_FAILURE_CONTAINER	= 1",
                "",
                "Damage = {	{Failure = AN_ALE_47_FAILURE_TOTAL, Failure_name = \"AN_ALE_47_FAILURE_TOTAL\", Failure_editor_name = _(\"AN/ALE-47 total failure\"),  Element = 10, Integrity_Treshold = 0.5, work_time_to_fail_probability = 0.5, work_time_to_fail = 3600*300},",
                "			{Failure = AN_ALE_47_FAILURE_CONTAINER, Failure_name = \"AN_ALE_47_FAILURE_CONTAINER\", Failure_editor_name = _(\"AN/ALE-47 container failure\"),  Element = 23, Integrity_Treshold = 0.75, work_time_to_fail_probability = 0.5, work_time_to_fail = 3600*300},",
                "}",
                "",
                "need_to_be_closed = true -- lua_state  will be closed in post_initialize()" };
                //"--Exported via DiCE by Bailey " + System.DateTime.Now};

            System.IO.Directory.CreateDirectory(cmdsLua_F16C_FolderPath);

            try
            {
                System.IO.File.WriteAllLines(cmdsLua_F16C_fullPath, luaExportString);
                //https://stackoverflow.com/questions/5920882/file-move-does-not-work-file-already-exists
                //playCompleteSound();

                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "F-16C CMS file exported.");
                richTextBox_log.ScrollToEnd();
            }
            catch (IOException h)
            {
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "DiCE could not write the F-16C CMS lua.");
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + h.Message);
                richTextBox_log.ScrollToEnd();
            }
            
        }


        private void readAndExportA10CData()
        {
            //this is where you read the data from the options.lua and put them into variables

            int A10C_ProgramAChaff_indexStart = optionsLuaText.IndexOf("[\"A10CProgramAChaff\"] =") + 23;
            int A10C_ProgramAChaff_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramAChaff_indexStart);
            string A10C_ProgramAChaff_string = optionsLuaText.Substring(A10C_ProgramAChaff_indexStart, A10C_ProgramAChaff_indexEnd - A10C_ProgramAChaff_indexStart);

            int A10C_ProgramAFlare_indexStart = optionsLuaText.IndexOf("[\"A10CProgramAFlare\"] =") + 23;
            int A10C_ProgramAFlare_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramAFlare_indexStart);
            string A10C_ProgramAFlare_string = optionsLuaText.Substring(A10C_ProgramAFlare_indexStart, A10C_ProgramAFlare_indexEnd - A10C_ProgramAFlare_indexStart);

            int A10C_ProgramAInterval_indexStart = optionsLuaText.IndexOf("[\"A10CProgramAInterval\"] =") + 26;
            int A10C_ProgramAInterval_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramAInterval_indexStart);
            string A10C_ProgramAInterval_string = optionsLuaText.Substring(A10C_ProgramAInterval_indexStart, A10C_ProgramAInterval_indexEnd - A10C_ProgramAInterval_indexStart);

            int A10C_ProgramACycle_indexStart = optionsLuaText.IndexOf("[\"A10CProgramACycle\"] =") + 23;
            int A10C_ProgramACycle_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramACycle_indexStart);
            string A10C_ProgramACycle_string = optionsLuaText.Substring(A10C_ProgramACycle_indexStart, A10C_ProgramACycle_indexEnd - A10C_ProgramACycle_indexStart);


            int A10C_ProgramBChaff_indexStart = optionsLuaText.IndexOf("[\"A10CProgramBChaff\"] =") + 23;
            int A10C_ProgramBChaff_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramBChaff_indexStart);
            string A10C_ProgramBChaff_string = optionsLuaText.Substring(A10C_ProgramBChaff_indexStart, A10C_ProgramBChaff_indexEnd - A10C_ProgramBChaff_indexStart);

            int A10C_ProgramBFlare_indexStart = optionsLuaText.IndexOf("[\"A10CProgramBFlare\"] =") + 23;
            int A10C_ProgramBFlare_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramBFlare_indexStart);
            string A10C_ProgramBFlare_string = optionsLuaText.Substring(A10C_ProgramBFlare_indexStart, A10C_ProgramBFlare_indexEnd - A10C_ProgramBFlare_indexStart);

            int A10C_ProgramBInterval_indexStart = optionsLuaText.IndexOf("[\"A10CProgramBInterval\"] =") + 26;
            int A10C_ProgramBInterval_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramBInterval_indexStart);
            string A10C_ProgramBInterval_string = optionsLuaText.Substring(A10C_ProgramBInterval_indexStart, A10C_ProgramBInterval_indexEnd - A10C_ProgramBInterval_indexStart);

            int A10C_ProgramBCycle_indexStart = optionsLuaText.IndexOf("[\"A10CProgramBCycle\"] =") + 23;
            int A10C_ProgramBCycle_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramBCycle_indexStart);
            string A10C_ProgramBCycle_string = optionsLuaText.Substring(A10C_ProgramBCycle_indexStart, A10C_ProgramBCycle_indexEnd - A10C_ProgramBCycle_indexStart);


            int A10C_ProgramCChaff_indexStart = optionsLuaText.IndexOf("[\"A10CProgramCChaff\"] =") + 23;
            int A10C_ProgramCChaff_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramCChaff_indexStart);
            string A10C_ProgramCChaff_string = optionsLuaText.Substring(A10C_ProgramCChaff_indexStart, A10C_ProgramCChaff_indexEnd - A10C_ProgramCChaff_indexStart);

            int A10C_ProgramCFlare_indexStart = optionsLuaText.IndexOf("[\"A10CProgramCFlare\"] =") + 23;
            int A10C_ProgramCFlare_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramCFlare_indexStart);
            string A10C_ProgramCFlare_string = optionsLuaText.Substring(A10C_ProgramCFlare_indexStart, A10C_ProgramCFlare_indexEnd - A10C_ProgramCFlare_indexStart);

            int A10C_ProgramCInterval_indexStart = optionsLuaText.IndexOf("[\"A10CProgramCInterval\"] =") + 26;
            int A10C_ProgramCInterval_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramCInterval_indexStart);
            string A10C_ProgramCInterval_string = optionsLuaText.Substring(A10C_ProgramCInterval_indexStart, A10C_ProgramCInterval_indexEnd - A10C_ProgramCInterval_indexStart);

            int A10C_ProgramCCycle_indexStart = optionsLuaText.IndexOf("[\"A10CProgramCCycle\"] =") + 23;
            int A10C_ProgramCCycle_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramCCycle_indexStart);
            string A10C_ProgramCCycle_string = optionsLuaText.Substring(A10C_ProgramCCycle_indexStart, A10C_ProgramCCycle_indexEnd - A10C_ProgramCCycle_indexStart);


            int A10C_ProgramDChaff_indexStart = optionsLuaText.IndexOf("[\"A10CProgramDChaff\"] =") + 23;
            int A10C_ProgramDChaff_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramDChaff_indexStart);
            string A10C_ProgramDChaff_string = optionsLuaText.Substring(A10C_ProgramDChaff_indexStart, A10C_ProgramDChaff_indexEnd - A10C_ProgramDChaff_indexStart);

            int A10C_ProgramDFlare_indexStart = optionsLuaText.IndexOf("[\"A10CProgramDFlare\"] =") + 23;
            int A10C_ProgramDFlare_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramDFlare_indexStart);
            string A10C_ProgramDFlare_string = optionsLuaText.Substring(A10C_ProgramDFlare_indexStart, A10C_ProgramDFlare_indexEnd - A10C_ProgramDFlare_indexStart);

            int A10C_ProgramDInterval_indexStart = optionsLuaText.IndexOf("[\"A10CProgramDInterval\"] =") + 26;
            int A10C_ProgramDInterval_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramDInterval_indexStart);
            string A10C_ProgramDInterval_string = optionsLuaText.Substring(A10C_ProgramDInterval_indexStart, A10C_ProgramDInterval_indexEnd - A10C_ProgramDInterval_indexStart);

            int A10C_ProgramDCycle_indexStart = optionsLuaText.IndexOf("[\"A10CProgramDCycle\"] =") + 23;
            int A10C_ProgramDCycle_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramDCycle_indexStart);
            string A10C_ProgramDCycle_string = optionsLuaText.Substring(A10C_ProgramDCycle_indexStart, A10C_ProgramDCycle_indexEnd - A10C_ProgramDCycle_indexStart);


            int A10C_ProgramEChaff_indexStart = optionsLuaText.IndexOf("[\"A10CProgramEChaff\"] =") + 23;
            int A10C_ProgramEChaff_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramEChaff_indexStart);
            string A10C_ProgramEChaff_string = optionsLuaText.Substring(A10C_ProgramEChaff_indexStart, A10C_ProgramEChaff_indexEnd - A10C_ProgramEChaff_indexStart);

            int A10C_ProgramEFlare_indexStart = optionsLuaText.IndexOf("[\"A10CProgramEFlare\"] =") + 23;
            int A10C_ProgramEFlare_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramEFlare_indexStart);
            string A10C_ProgramEFlare_string = optionsLuaText.Substring(A10C_ProgramEFlare_indexStart, A10C_ProgramEFlare_indexEnd - A10C_ProgramEFlare_indexStart);

            int A10C_ProgramEInterval_indexStart = optionsLuaText.IndexOf("[\"A10CProgramEInterval\"] =") + 26;
            int A10C_ProgramEInterval_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramEInterval_indexStart);
            string A10C_ProgramEInterval_string = optionsLuaText.Substring(A10C_ProgramEInterval_indexStart, A10C_ProgramEInterval_indexEnd - A10C_ProgramEInterval_indexStart);

            int A10C_ProgramECycle_indexStart = optionsLuaText.IndexOf("[\"A10CProgramECycle\"] =") + 23;
            int A10C_ProgramECycle_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramECycle_indexStart);
            string A10C_ProgramECycle_string = optionsLuaText.Substring(A10C_ProgramECycle_indexStart, A10C_ProgramECycle_indexEnd - A10C_ProgramECycle_indexStart);


            int A10C_ProgramFChaff_indexStart = optionsLuaText.IndexOf("[\"A10CProgramFChaff\"] =") + 23;
            int A10C_ProgramFChaff_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramFChaff_indexStart);
            string A10C_ProgramFChaff_string = optionsLuaText.Substring(A10C_ProgramFChaff_indexStart, A10C_ProgramFChaff_indexEnd - A10C_ProgramFChaff_indexStart);

            int A10C_ProgramFFlare_indexStart = optionsLuaText.IndexOf("[\"A10CProgramFFlare\"] =") + 23;
            int A10C_ProgramFFlare_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramFFlare_indexStart);
            string A10C_ProgramFFlare_string = optionsLuaText.Substring(A10C_ProgramFFlare_indexStart, A10C_ProgramFFlare_indexEnd - A10C_ProgramFFlare_indexStart);

            int A10C_ProgramFInterval_indexStart = optionsLuaText.IndexOf("[\"A10CProgramFInterval\"] =") + 26;
            int A10C_ProgramFInterval_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramFInterval_indexStart);
            string A10C_ProgramFInterval_string = optionsLuaText.Substring(A10C_ProgramFInterval_indexStart, A10C_ProgramFInterval_indexEnd - A10C_ProgramFInterval_indexStart);

            int A10C_ProgramFCycle_indexStart = optionsLuaText.IndexOf("[\"A10CProgramFCycle\"] =") + 23;
            int A10C_ProgramFCycle_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramFCycle_indexStart);
            string A10C_ProgramFCycle_string = optionsLuaText.Substring(A10C_ProgramFCycle_indexStart, A10C_ProgramFCycle_indexEnd - A10C_ProgramFCycle_indexStart);


            int A10C_ProgramGChaff_indexStart = optionsLuaText.IndexOf("[\"A10CProgramGChaff\"] =") + 23;
            int A10C_ProgramGChaff_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramGChaff_indexStart);
            string A10C_ProgramGChaff_string = optionsLuaText.Substring(A10C_ProgramGChaff_indexStart, A10C_ProgramGChaff_indexEnd - A10C_ProgramGChaff_indexStart);

            int A10C_ProgramGFlare_indexStart = optionsLuaText.IndexOf("[\"A10CProgramGFlare\"] =") + 23;
            int A10C_ProgramGFlare_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramGFlare_indexStart);
            string A10C_ProgramGFlare_string = optionsLuaText.Substring(A10C_ProgramGFlare_indexStart, A10C_ProgramGFlare_indexEnd - A10C_ProgramGFlare_indexStart);

            int A10C_ProgramGInterval_indexStart = optionsLuaText.IndexOf("[\"A10CProgramGInterval\"] =") + 26;
            int A10C_ProgramGInterval_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramGInterval_indexStart);
            string A10C_ProgramGInterval_string = optionsLuaText.Substring(A10C_ProgramGInterval_indexStart, A10C_ProgramGInterval_indexEnd - A10C_ProgramGInterval_indexStart);

            int A10C_ProgramGCycle_indexStart = optionsLuaText.IndexOf("[\"A10CProgramGCycle\"] =") + 23;
            int A10C_ProgramGCycle_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramGCycle_indexStart);
            string A10C_ProgramGCycle_string = optionsLuaText.Substring(A10C_ProgramGCycle_indexStart, A10C_ProgramGCycle_indexEnd - A10C_ProgramGCycle_indexStart);


            int A10C_ProgramHChaff_indexStart = optionsLuaText.IndexOf("[\"A10CProgramHChaff\"] =") + 23;
            int A10C_ProgramHChaff_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramHChaff_indexStart);
            string A10C_ProgramHChaff_string = optionsLuaText.Substring(A10C_ProgramHChaff_indexStart, A10C_ProgramHChaff_indexEnd - A10C_ProgramHChaff_indexStart);

            int A10C_ProgramHFlare_indexStart = optionsLuaText.IndexOf("[\"A10CProgramHFlare\"] =") + 23;
            int A10C_ProgramHFlare_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramHFlare_indexStart);
            string A10C_ProgramHFlare_string = optionsLuaText.Substring(A10C_ProgramHFlare_indexStart, A10C_ProgramHFlare_indexEnd - A10C_ProgramHFlare_indexStart);

            int A10C_ProgramHInterval_indexStart = optionsLuaText.IndexOf("[\"A10CProgramHInterval\"] =") + 26;
            int A10C_ProgramHInterval_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramHInterval_indexStart);
            string A10C_ProgramHInterval_string = optionsLuaText.Substring(A10C_ProgramHInterval_indexStart, A10C_ProgramHInterval_indexEnd - A10C_ProgramHInterval_indexStart);

            int A10C_ProgramHCycle_indexStart = optionsLuaText.IndexOf("[\"A10CProgramHCycle\"] =") + 23;
            int A10C_ProgramHCycle_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramHCycle_indexStart);
            string A10C_ProgramHCycle_string = optionsLuaText.Substring(A10C_ProgramHCycle_indexStart, A10C_ProgramHCycle_indexEnd - A10C_ProgramHCycle_indexStart);


            int A10C_ProgramIChaff_indexStart = optionsLuaText.IndexOf("[\"A10CProgramIChaff\"] =") + 23;
            int A10C_ProgramIChaff_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramIChaff_indexStart);
            string A10C_ProgramIChaff_string = optionsLuaText.Substring(A10C_ProgramIChaff_indexStart, A10C_ProgramIChaff_indexEnd - A10C_ProgramIChaff_indexStart);

            int A10C_ProgramIFlare_indexStart = optionsLuaText.IndexOf("[\"A10CProgramIFlare\"] =") + 23;
            int A10C_ProgramIFlare_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramIFlare_indexStart);
            string A10C_ProgramIFlare_string = optionsLuaText.Substring(A10C_ProgramIFlare_indexStart, A10C_ProgramIFlare_indexEnd - A10C_ProgramIFlare_indexStart);

            int A10C_ProgramIInterval_indexStart = optionsLuaText.IndexOf("[\"A10CProgramIInterval\"] =") + 26;
            int A10C_ProgramIInterval_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramIInterval_indexStart);
            string A10C_ProgramIInterval_string = optionsLuaText.Substring(A10C_ProgramIInterval_indexStart, A10C_ProgramIInterval_indexEnd - A10C_ProgramIInterval_indexStart);

            int A10C_ProgramICycle_indexStart = optionsLuaText.IndexOf("[\"A10CProgramICycle\"] =") + 23;
            int A10C_ProgramICycle_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramICycle_indexStart);
            string A10C_ProgramICycle_string = optionsLuaText.Substring(A10C_ProgramICycle_indexStart, A10C_ProgramICycle_indexEnd - A10C_ProgramICycle_indexStart);


            int A10C_ProgramJChaff_indexStart = optionsLuaText.IndexOf("[\"A10CProgramJChaff\"] =") + 23;
            int A10C_ProgramJChaff_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramJChaff_indexStart);
            string A10C_ProgramJChaff_string = optionsLuaText.Substring(A10C_ProgramJChaff_indexStart, A10C_ProgramJChaff_indexEnd - A10C_ProgramJChaff_indexStart);

            int A10C_ProgramJFlare_indexStart = optionsLuaText.IndexOf("[\"A10CProgramJFlare\"] =") + 23;
            int A10C_ProgramJFlare_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramJFlare_indexStart);
            string A10C_ProgramJFlare_string = optionsLuaText.Substring(A10C_ProgramJFlare_indexStart, A10C_ProgramJFlare_indexEnd - A10C_ProgramJFlare_indexStart);

            int A10C_ProgramJInterval_indexStart = optionsLuaText.IndexOf("[\"A10CProgramJInterval\"] =") + 26;
            int A10C_ProgramJInterval_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramJInterval_indexStart);
            string A10C_ProgramJInterval_string = optionsLuaText.Substring(A10C_ProgramJInterval_indexStart, A10C_ProgramJInterval_indexEnd - A10C_ProgramJInterval_indexStart);

            int A10C_ProgramJCycle_indexStart = optionsLuaText.IndexOf("[\"A10CProgramJCycle\"] =") + 23;
            int A10C_ProgramJCycle_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramJCycle_indexStart);
            string A10C_ProgramJCycle_string = optionsLuaText.Substring(A10C_ProgramJCycle_indexStart, A10C_ProgramJCycle_indexEnd - A10C_ProgramJCycle_indexStart);


            int A10C_ProgramKChaff_indexStart = optionsLuaText.IndexOf("[\"A10CProgramKChaff\"] =") + 23;
            int A10C_ProgramKChaff_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramKChaff_indexStart);
            string A10C_ProgramKChaff_string = optionsLuaText.Substring(A10C_ProgramKChaff_indexStart, A10C_ProgramKChaff_indexEnd - A10C_ProgramKChaff_indexStart);

            int A10C_ProgramKFlare_indexStart = optionsLuaText.IndexOf("[\"A10CProgramKFlare\"] =") + 23;
            int A10C_ProgramKFlare_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramKFlare_indexStart);
            string A10C_ProgramKFlare_string = optionsLuaText.Substring(A10C_ProgramKFlare_indexStart, A10C_ProgramKFlare_indexEnd - A10C_ProgramKFlare_indexStart);

            int A10C_ProgramKInterval_indexStart = optionsLuaText.IndexOf("[\"A10CProgramKInterval\"] =") + 26;
            int A10C_ProgramKInterval_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramKInterval_indexStart);
            string A10C_ProgramKInterval_string = optionsLuaText.Substring(A10C_ProgramKInterval_indexStart, A10C_ProgramKInterval_indexEnd - A10C_ProgramKInterval_indexStart);

            int A10C_ProgramKCycle_indexStart = optionsLuaText.IndexOf("[\"A10CProgramKCycle\"] =") + 23;
            int A10C_ProgramKCycle_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramKCycle_indexStart);
            string A10C_ProgramKCycle_string = optionsLuaText.Substring(A10C_ProgramKCycle_indexStart, A10C_ProgramKCycle_indexEnd - A10C_ProgramKCycle_indexStart);


            int A10C_ProgramLChaff_indexStart = optionsLuaText.IndexOf("[\"A10CProgramLChaff\"] =") + 23;
            int A10C_ProgramLChaff_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramLChaff_indexStart);
            string A10C_ProgramLChaff_string = optionsLuaText.Substring(A10C_ProgramLChaff_indexStart, A10C_ProgramLChaff_indexEnd - A10C_ProgramLChaff_indexStart);

            int A10C_ProgramLFlare_indexStart = optionsLuaText.IndexOf("[\"A10CProgramLFlare\"] =") + 23;
            int A10C_ProgramLFlare_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramLFlare_indexStart);
            string A10C_ProgramLFlare_string = optionsLuaText.Substring(A10C_ProgramLFlare_indexStart, A10C_ProgramLFlare_indexEnd - A10C_ProgramLFlare_indexStart);

            int A10C_ProgramLInterval_indexStart = optionsLuaText.IndexOf("[\"A10CProgramLInterval\"] =") + 26;
            int A10C_ProgramLInterval_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramLInterval_indexStart);
            string A10C_ProgramLInterval_string = optionsLuaText.Substring(A10C_ProgramLInterval_indexStart, A10C_ProgramLInterval_indexEnd - A10C_ProgramLInterval_indexStart);

            int A10C_ProgramLCycle_indexStart = optionsLuaText.IndexOf("[\"A10CProgramLCycle\"] =") + 23;
            int A10C_ProgramLCycle_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramLCycle_indexStart);
            string A10C_ProgramLCycle_string = optionsLuaText.Substring(A10C_ProgramLCycle_indexStart, A10C_ProgramLCycle_indexEnd - A10C_ProgramLCycle_indexStart);


            int A10C_ProgramMChaff_indexStart = optionsLuaText.IndexOf("[\"A10CProgramMChaff\"] =") + 23;
            int A10C_ProgramMChaff_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramMChaff_indexStart);
            string A10C_ProgramMChaff_string = optionsLuaText.Substring(A10C_ProgramMChaff_indexStart, A10C_ProgramMChaff_indexEnd - A10C_ProgramMChaff_indexStart);

            int A10C_ProgramMFlare_indexStart = optionsLuaText.IndexOf("[\"A10CProgramMFlare\"] =") + 23;
            int A10C_ProgramMFlare_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramMFlare_indexStart);
            string A10C_ProgramMFlare_string = optionsLuaText.Substring(A10C_ProgramMFlare_indexStart, A10C_ProgramMFlare_indexEnd - A10C_ProgramMFlare_indexStart);

            int A10C_ProgramMInterval_indexStart = optionsLuaText.IndexOf("[\"A10CProgramMInterval\"] =") + 26;
            int A10C_ProgramMInterval_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramMInterval_indexStart);
            string A10C_ProgramMInterval_string = optionsLuaText.Substring(A10C_ProgramMInterval_indexStart, A10C_ProgramMInterval_indexEnd - A10C_ProgramMInterval_indexStart);

            int A10C_ProgramMCycle_indexStart = optionsLuaText.IndexOf("[\"A10CProgramMCycle\"] =") + 23;
            int A10C_ProgramMCycle_indexEnd = optionsLuaText.IndexOf(",", A10C_ProgramMCycle_indexStart);
            string A10C_ProgramMCycle_string = optionsLuaText.Substring(A10C_ProgramMCycle_indexStart, A10C_ProgramMCycle_indexEnd - A10C_ProgramMCycle_indexStart);


            //MessageBox.Show("|" + A10C_Manual1Chaff_string + "|" + A10C_Manual1Flare_string + "|" + A10C_Manual1Interval_string + "|" + A10C_Manual1Cycle_string + "|");



            //then you merge that info with the formated cms.lua and export it to the proper folder location

            string[] luaExportString = {
                                "local gettext = require(\"i_18n\")",
                "_ = gettext.translate",
                "",
                "programs = {}",
                "",
                "-- Old generation radar SAM",
                "programs['A'] = {}",
                "programs['A'][\"chaff\"] =" + A10C_ProgramAChaff_string,
                "programs['A'][\"flare\"] =" + A10C_ProgramAFlare_string,
                "programs['A'][\"intv\"]  =" + A10C_ProgramAInterval_string,
                "programs['A'][\"cycle\"] =" + A10C_ProgramACycle_string,
                "",
                "-- Current generation radar SAM",
                "programs['B'] = {}",
                "programs['B'][\"chaff\"] =" + A10C_ProgramBChaff_string,
                "programs['B'][\"flare\"] =" + A10C_ProgramBFlare_string,
                "programs['B'][\"intv\"]  =" + A10C_ProgramBInterval_string,
                "programs['B'][\"cycle\"] =" + A10C_ProgramBCycle_string,
                "",
                "-- IR SAM",
                "programs['C'] = {}",
                "programs['C'][\"chaff\"] =" + A10C_ProgramCChaff_string,
                "programs['C'][\"flare\"] =" + A10C_ProgramCFlare_string,
                "programs['C'][\"intv\"]  =" + A10C_ProgramCInterval_string,
                "programs['C'][\"cycle\"] =" + A10C_ProgramCCycle_string,
                "",
                "-- Default manual presets",
                "-- Mix 1",
                "programs['D'] = {}",
                "programs['D'][\"chaff\"] =" + A10C_ProgramDChaff_string,
                "programs['D'][\"flare\"] =" + A10C_ProgramDFlare_string,
                "programs['D'][\"intv\"]  =" + A10C_ProgramDInterval_string,
                "programs['D'][\"cycle\"] =" + A10C_ProgramDCycle_string,
                "",
                "-- Mix 2",
                "programs['E'] = {}",
                "programs['E'][\"chaff\"] =" + A10C_ProgramEChaff_string,
                "programs['E'][\"flare\"] =" + A10C_ProgramEFlare_string,
                "programs['E'][\"intv\"]  =" + A10C_ProgramEInterval_string,
                "programs['E'][\"cycle\"] =" + A10C_ProgramECycle_string,
                "",
                "-- Mix 3",
                "programs['F'] = {}",
                "programs['F'][\"chaff\"] =" + A10C_ProgramFChaff_string,
                "programs['F'][\"flare\"] =" + A10C_ProgramFFlare_string,
                "programs['F'][\"intv\"]  =" + A10C_ProgramFInterval_string,
                "programs['F'][\"cycle\"] =" + A10C_ProgramFCycle_string,
                "",
                "-- Mix 4",
                "programs['G'] = {}",
                "programs['G'][\"chaff\"] =" + A10C_ProgramGChaff_string,
                "programs['G'][\"flare\"] =" + A10C_ProgramGFlare_string,
                "programs['G'][\"intv\"]  =" + A10C_ProgramGInterval_string,
                "programs['G'][\"cycle\"] =" + A10C_ProgramGCycle_string,
                "",
                "-- Chaff single",
                "programs['H'] = {}",
                "programs['H'][\"chaff\"] =" + A10C_ProgramHChaff_string,
                "programs['H'][\"flare\"] =" + A10C_ProgramHFlare_string,
                "programs['H'][\"intv\"]  =" + A10C_ProgramHInterval_string,
                "programs['H'][\"cycle\"] =" + A10C_ProgramHCycle_string,
                "",
                "-- Chaff pair",
                "programs['I'] = {}",
                "programs['I'][\"chaff\"] =" + A10C_ProgramIChaff_string,
                "programs['I'][\"flare\"] =" + A10C_ProgramIFlare_string,
                "programs['I'][\"intv\"]  =" + A10C_ProgramIInterval_string,
                "programs['I'][\"cycle\"] =" + A10C_ProgramICycle_string,
                "",
                "-- Flare single",
                "programs['J'] = {}",
                "programs['J'][\"chaff\"] =" + A10C_ProgramJChaff_string,
                "programs['J'][\"flare\"] =" + A10C_ProgramJFlare_string,
                "programs['J'][\"intv\"]  =" + A10C_ProgramJInterval_string,
                "programs['J'][\"cycle\"] =" + A10C_ProgramJCycle_string,
                "",
                "-- Flare pair",
                "programs['K'] = {}",
                "programs['K'][\"chaff\"] =" + A10C_ProgramKChaff_string,
                "programs['K'][\"flare\"] =" + A10C_ProgramKFlare_string,
                "programs['K'][\"intv\"]  =" + A10C_ProgramKInterval_string,
                "programs['K'][\"cycle\"] =" + A10C_ProgramKCycle_string,
                "",
                "-- Chaff pre-empt",
                "programs['L'] = {}",
                "programs['L'][\"chaff\"] =" + A10C_ProgramAChaff_string,
                "programs['L'][\"flare\"] =" + A10C_ProgramLFlare_string,
                "programs['L'][\"intv\"]  =" + A10C_ProgramLInterval_string,
                "programs['L'][\"cycle\"] =" + A10C_ProgramLCycle_string,
                "",
                "-- Flare pre-empt",
                "programs['M'] = {}",
                "programs['M'][\"chaff\"] =" + A10C_ProgramMChaff_string,
                "programs['M'][\"flare\"] =" + A10C_ProgramMFlare_string,
                "programs['M'][\"intv\"]  =" + A10C_ProgramMInterval_string,
                "programs['M'][\"cycle\"] =" + A10C_ProgramMCycle_string,
                "",
                "",
                "",
                "ContainerChaffCapacity = 120",
                "",
                "ContainerFlareCapacity = 60",
                "",
                "NumberOfContiners      = 4",
                "",
                "AN_ALE_40V_FAILURE_TOTAL = 0",
                "AN_ALE_40V_FAILURE_CONTAINER_LEFT_WING	= 1",
                "AN_ALE_40V_FAILURE_CONTAINER_LEFT_GEAR	= 2",
                "AN_ALE_40V_FAILURE_CONTAINER_RIGHT_GEAR	= 3",
                "AN_ALE_40V_FAILURE_CONTAINER_RIGHT_WING	= 4",
                "",
                "Damage = {	{Failure = AN_ALE_40V_FAILURE_TOTAL, Failure_name = \"AN_ALE_40V_FAILURE_TOTAL\", Failure_editor_name = _(\"AN/ALE-40(V) total failure\"),  Element = 10, Integrity_Treshold = 0.5, work_time_to_fail_probability = 0.5, work_time_to_fail = 3600*300},",
                "		{Failure = AN_ALE_40V_FAILURE_CONTAINER_LEFT_WING, Failure_name = \"AN_ALE_40V_FAILURE_CONTAINER_LEFT_WING\", Failure_editor_name = _(\"AN/ALE-40(V) left wing container failure\"),  Element = 23, Integrity_Treshold = 0.75, work_time_to_fail_probability = 0.5, work_time_to_fail = 3600*300},",
                "		{Failure = AN_ALE_40V_FAILURE_CONTAINER_LEFT_GEAR, Failure_name = \"AN_ALE_40V_FAILURE_CONTAINER_LEFT_GEAR\", Failure_editor_name = _(\"AN/ALE-40(V) left gear container failure\"),  Element = 15, Integrity_Treshold = 0.75, work_time_to_fail_probability = 0.5, work_time_to_fail = 3600*300},",
                "		{Failure = AN_ALE_40V_FAILURE_CONTAINER_RIGHT_GEAR, Failure_name = \"AN_ALE_40V_FAILURE_CONTAINER_RIGHT_GEAR\", Failure_editor_name = _(\"AN/ALE-40(V) right gear container failure\"),  Element = 16, Integrity_Treshold = 0.75, work_time_to_fail_probability = 0.5, work_time_to_fail = 3600*300},",
                "		{Failure = AN_ALE_40V_FAILURE_CONTAINER_RIGHT_WING, Failure_name = \"AN_ALE_40V_FAILURE_CONTAINER_RIGHT_WING\", Failure_editor_name = _(\"AN/ALE-40(V) right wing container failure\"),  Element = 24, Integrity_Treshold = 0.75, work_time_to_fail_probability = 0.5, work_time_to_fail = 3600*300},",
                "}",
                "",
                "need_to_be_closed = true -- lua_state  will be closed in post_initialize()" };
                //"--Exported via DiCE by Bailey " + System.DateTime.Now};

            //if DiCE detects that the A-10C is installed
            if (optionsLuaText.Contains(detection_A10C_DiCE) && optionsLuaText.Contains(detection_A10C_vanilla))
            {//create the directory
                System.IO.Directory.CreateDirectory(cmdsLua_A10C_FolderPath);

                try
                {//then try to write the file
                    System.IO.File.WriteAllLines(cmdsLua_A10C_fullPath, luaExportString);
                    //https://stackoverflow.com/questions/5920882/file-move-does-not-work-file-already-exists
                    //playCompleteSound();

                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "A-10C CMS file exported.");
                    richTextBox_log.ScrollToEnd();
                }
                catch (IOException g)
                {
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "DiCE could not write the A-10C CMS lua.");
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + g.Message);
                    richTextBox_log.ScrollToEnd();
                }
            }

            //if DiCE detects that the A-10C2 is installed
            if (optionsLuaText.Contains(detection_A10C2_DiCE) && optionsLuaText.Contains(detection_A10C2_vanilla))
            {//create the directory
                System.IO.Directory.CreateDirectory(cmdsLua_A10C2_FolderPath);

                try
                {//then try to write the file
                    System.IO.File.WriteAllLines(cmdsLua_A10C2_fullPath, luaExportString);
                    //https://stackoverflow.com/questions/5920882/file-move-does-not-work-file-already-exists
                    //playCompleteSound();

                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "A-10C2 CMS file exported.");
                    richTextBox_log.ScrollToEnd();
                }
                catch (IOException g)
                {
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "DiCE could not write the A-10C2 CMS lua.");
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + g.Message);
                    richTextBox_log.ScrollToEnd();
                }
            }
        }

        private void readAndExportAV8BData()
        {
            //All Expendables
            string tempLength = ("[\"AV8BAllExpendablesChaffBurstQuantity\"] =");
            int AV8BAllExpendablesChaffBurstQuantity_indexStart = optionsLuaText.IndexOf("[\"AV8BAllExpendablesChaffBurstQuantity\"] =") + 42;
            int AV8BAllExpendablesChaffBurstQuantity_indexEnd = optionsLuaText.IndexOf(",", AV8BAllExpendablesChaffBurstQuantity_indexStart);
            string AV8BAllExpendablesChaffBurstQuantity_string = optionsLuaText.Substring(AV8BAllExpendablesChaffBurstQuantity_indexStart, AV8BAllExpendablesChaffBurstQuantity_indexEnd - AV8BAllExpendablesChaffBurstQuantity_indexStart);

            tempLength = ("[\"AV8BAllExpendablesChaffBurstInterval\"] =");//used to have a little more automation and a little less manual error
            int AV8BAllExpendablesChaffBurstInterval_indexStart = optionsLuaText.IndexOf(tempLength) + tempLength.Length;
            int AV8BAllExpendablesChaffBurstInterval_indexEnd = optionsLuaText.IndexOf(",", AV8BAllExpendablesChaffBurstInterval_indexStart);
            string AV8BAllExpendablesChaffBurstInterval_string = optionsLuaText.Substring(AV8BAllExpendablesChaffBurstInterval_indexStart, AV8BAllExpendablesChaffBurstInterval_indexEnd - AV8BAllExpendablesChaffBurstInterval_indexStart);

            tempLength = ("[\"AV8BAllExpendablesChaffSalvoQuantity\"] =");
            int AV8BAllExpendablesChaffSalvoQuantity_indexStart = optionsLuaText.IndexOf(tempLength) + tempLength.Length;
            int AV8BAllExpendablesChaffSalvoQuantity_indexEnd = optionsLuaText.IndexOf(",", AV8BAllExpendablesChaffSalvoQuantity_indexStart);
            string AV8BAllExpendablesChaffSalvoQuantity_string = optionsLuaText.Substring(AV8BAllExpendablesChaffSalvoQuantity_indexStart, AV8BAllExpendablesChaffSalvoQuantity_indexEnd - AV8BAllExpendablesChaffSalvoQuantity_indexStart);

            tempLength = ("[\"AV8BAllExpendablesChaffSalvoInterval\"] =");
            int AV8BAllExpendablesChaffSalvoInterval_indexStart = optionsLuaText.IndexOf(tempLength) + tempLength.Length;
            int AV8BAllExpendablesChaffSalvoInterval_indexEnd = optionsLuaText.IndexOf(",", AV8BAllExpendablesChaffSalvoInterval_indexStart);
            string AV8BAllExpendablesChaffSalvoInterval_string = optionsLuaText.Substring(AV8BAllExpendablesChaffSalvoInterval_indexStart, AV8BAllExpendablesChaffSalvoInterval_indexEnd - AV8BAllExpendablesChaffSalvoInterval_indexStart);

            tempLength = ("[\"AV8BAllExpendablesFlaresSalvoQuantity\"] =");
            int AV8BAllExpendablesFlaresSalvoQuantity_indexStart = optionsLuaText.IndexOf(tempLength) + tempLength.Length;
            int AV8BAllExpendablesFlaresSalvoQuantity_indexEnd = optionsLuaText.IndexOf(",", AV8BAllExpendablesFlaresSalvoQuantity_indexStart);
            string AV8BAllExpendablesFlaresSalvoQuantity_string = optionsLuaText.Substring(AV8BAllExpendablesFlaresSalvoQuantity_indexStart, AV8BAllExpendablesFlaresSalvoQuantity_indexEnd - AV8BAllExpendablesFlaresSalvoQuantity_indexStart);

            tempLength = ("[\"AV8BAllExpendablesFlaresSalvoInterval\"] =");
            int AV8BAllExpendablesFlaresSalvoInterval_indexStart = optionsLuaText.IndexOf(tempLength) + tempLength.Length;
            int AV8BAllExpendablesFlaresSalvoInterval_indexEnd = optionsLuaText.IndexOf(",", AV8BAllExpendablesFlaresSalvoInterval_indexStart);
            string AV8BAllExpendablesFlaresSalvoInterval_string = optionsLuaText.Substring(AV8BAllExpendablesFlaresSalvoInterval_indexStart, AV8BAllExpendablesFlaresSalvoInterval_indexEnd - AV8BAllExpendablesFlaresSalvoInterval_indexStart);

            //Chaff Only

            tempLength = ("[\"AV8BChaffOnlyChaffBurstQuantity\"] =");
            int AV8BChaffOnlyChaffBurstQuantity_indexStart = optionsLuaText.IndexOf(tempLength) + tempLength.Length;
            int AV8BChaffOnlyChaffBurstQuantity_indexEnd = optionsLuaText.IndexOf(",", AV8BChaffOnlyChaffBurstQuantity_indexStart);
            string AV8BChaffOnlyChaffBurstQuantity_string = optionsLuaText.Substring(AV8BChaffOnlyChaffBurstQuantity_indexStart, AV8BChaffOnlyChaffBurstQuantity_indexEnd - AV8BChaffOnlyChaffBurstQuantity_indexStart);

            tempLength = ("[\"AV8BChaffOnlyChaffBurstInterval\"] =");
            int AV8BChaffOnlyChaffBurstInterval_indexStart = optionsLuaText.IndexOf(tempLength) + tempLength.Length;
            int AV8BChaffOnlyChaffBurstInterval_indexEnd = optionsLuaText.IndexOf(",", AV8BChaffOnlyChaffBurstInterval_indexStart);
            string AV8BChaffOnlyChaffBurstInterval_string = optionsLuaText.Substring(AV8BChaffOnlyChaffBurstInterval_indexStart, AV8BChaffOnlyChaffBurstInterval_indexEnd - AV8BChaffOnlyChaffBurstInterval_indexStart);

            tempLength = ("[\"AV8BChaffOnlyChaffSalvoQuantity\"] =");
            int AV8BChaffOnlyChaffSalvoQuantity_indexStart = optionsLuaText.IndexOf(tempLength) + tempLength.Length;
            int AV8BChaffOnlyChaffSalvoQuantity_indexEnd = optionsLuaText.IndexOf(",", AV8BChaffOnlyChaffSalvoQuantity_indexStart);
            string AV8BChaffOnlyChaffSalvoQuantity_string = optionsLuaText.Substring(AV8BChaffOnlyChaffSalvoQuantity_indexStart, AV8BChaffOnlyChaffSalvoQuantity_indexEnd - AV8BChaffOnlyChaffSalvoQuantity_indexStart);

            tempLength = ("[\"AV8BChaffOnlyChaffSalvoInterval\"] =");
            int AV8BChaffOnlyChaffSalvoInterval_indexStart = optionsLuaText.IndexOf(tempLength) + tempLength.Length;
            int AV8BChaffOnlyChaffSalvoInterval_indexEnd = optionsLuaText.IndexOf(",", AV8BChaffOnlyChaffSalvoInterval_indexStart);
            string AV8BChaffOnlyChaffSalvoInterval_string = optionsLuaText.Substring(AV8BChaffOnlyChaffSalvoInterval_indexStart, AV8BChaffOnlyChaffSalvoInterval_indexEnd - AV8BChaffOnlyChaffSalvoInterval_indexStart);

            //Flares Only

            tempLength = ("[\"AV8BFlaresOnlyFlaresSalvoQuantity\"] =");
            int AV8BFlaresOnlyFlaresSalvoQuantity_indexStart = optionsLuaText.IndexOf(tempLength) + tempLength.Length;
            int AV8BFlaresOnlyFlaresSalvoQuantity_indexEnd = optionsLuaText.IndexOf(",", AV8BFlaresOnlyFlaresSalvoQuantity_indexStart);
            string AV8BFlaresOnlyFlaresSalvoQuantity_string = optionsLuaText.Substring(AV8BFlaresOnlyFlaresSalvoQuantity_indexStart, AV8BFlaresOnlyFlaresSalvoQuantity_indexEnd - AV8BFlaresOnlyFlaresSalvoQuantity_indexStart);

            tempLength = ("[\"AV8BFlaresOnlyChaffSalvoInterval\"] =");
            int AV8BFlaresOnlyChaffSalvoInterval_indexStart = optionsLuaText.IndexOf(tempLength) + tempLength.Length;
            int AV8BFlaresOnlyChaffSalvoInterval_indexEnd = optionsLuaText.IndexOf(",", AV8BFlaresOnlyChaffSalvoInterval_indexStart);
            string AV8BFlaresOnlyChaffSalvoInterval_string = optionsLuaText.Substring(AV8BFlaresOnlyChaffSalvoInterval_indexStart, AV8BFlaresOnlyChaffSalvoInterval_indexEnd - AV8BFlaresOnlyChaffSalvoInterval_indexStart);


            string[] luaExportString = {
                "local gettext = require(\"i_18n\")",
                "_ = gettext.translate",
                "",
                "-- Chaff Burst Values",
                "-- BQTY: 1 to 15. Special values: -1 = Continuous (will use ALL chaff); -2 = Random (will dispense between 1 to 6 chaff)",
                "-- BINT: 0.1 to 1.5 seconds. Special values: -2 = Random (will set an interval between 0.1 and 0.4 seconds)",
                "",
                "-- Chaff Salvo Values",
                "-- SQTY: 1 to 15.",
                "-- SINT: 1 to 15 seconds.",
                "",
                "-- Flare Salvo Values",
                "-- SQTY: 1 to 15.",
                "-- SINT: 1 to 15 seconds.",
                "",
                "--All Expendables",
                "EW_ALL_CHAFF_BQTY =" + AV8BAllExpendablesChaffBurstQuantity_string + ";",
                "EW_ALL_CHAFF_BINT =" + AV8BAllExpendablesChaffBurstInterval_string + ";",
                "EW_ALL_CHAFF_SQTY =" + AV8BAllExpendablesChaffSalvoQuantity_string + ";",
                "EW_ALL_CHAFF_SINT =" + AV8BAllExpendablesChaffSalvoInterval_string + ";",
                "EW_ALL_FLARES_SQTY =" + AV8BAllExpendablesFlaresSalvoQuantity_string + ";",
                "EW_ALL_FLARES_SINT =" + AV8BAllExpendablesFlaresSalvoInterval_string + ";",
                "",
                "--Chaff Only",
                "EW_CHAFF_BQTY =" + AV8BChaffOnlyChaffBurstQuantity_string + ";",
                "EW_CHAFF_BINT =" + AV8BChaffOnlyChaffBurstInterval_string + ";",
                "EW_CHAFF_SQTY =" + AV8BChaffOnlyChaffSalvoQuantity_string + ";",
                "EW_CHAFF_SINT =" + AV8BChaffOnlyChaffSalvoInterval_string + ";",
                "",
                "--Flares Only",
                "EW_FLARES_SQTY =" + AV8BFlaresOnlyFlaresSalvoQuantity_string + ";",
                "EW_FLARES_SINT =" + AV8BFlaresOnlyChaffSalvoInterval_string + ";",
                "",
                "need_to_be_closed = true",
                "" };
                //"--Exported via DiCE by Bailey " + System.DateTime.Now};
            System.IO.Directory.CreateDirectory(cmdsLua_AV8B_FolderPath);

            try
            {
                System.IO.File.WriteAllLines(cmdsLua_AV8B_fullPath, luaExportString);
                //https://stackoverflow.com/questions/5920882/file-move-does-not-work-file-already-exists
                //playCompleteSound();

                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "AV-8B CMS file exported.");
                richTextBox_log.ScrollToEnd();
            }
            catch (IOException h)
            {
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "DiCE could not write the AV-8B CMS lua.");
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + h.Message);
                richTextBox_log.ScrollToEnd();
            }
        }

        private void readAndExportM2000CData()//TODO: this is the new way of how to get the data from the file
        {
            //
            var optionsLuaText = LsonVars.Parse(File.ReadAllText(userOptionsLua_Full_pathWithExtention));
            //var M2000CManual01Chaff = optionsLuaText["options"]["plugins"]["DiCE M2000C"].GetStringLenient();//you dont even need this!!!!
            Double M2000CManual01Chaff = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual01Chaff"].GetDoubleLenient();
            Double M2000CManual01Flare = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual01Flare"].GetDoubleLenient();
            Double M2000CManual01Cycle = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual01Cycle"].GetDoubleLenient();
            Double M2000CManual01CycleInterval = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual01CycleInterval"].GetDoubleLenient();
            Double M2000CManual01Interval = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual01Interval"].GetDoubleLenient();
            Double M2000CManual02Chaff = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual02Chaff"].GetDoubleLenient();
            Double M2000CManual02Cycle = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual02Cycle"].GetDoubleLenient();
            Double M2000CManual02CycleInterval = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual02CycleInterval"].GetDoubleLenient();
            Double M2000CManual02Flare = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual02Flare"].GetDoubleLenient();
            Double M2000CManual02Interval = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual02Interval"].GetDoubleLenient();
            Double M2000CManual03Chaff = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual03Chaff"].GetDoubleLenient();
            Double M2000CManual03Cycle = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual03Cycle"].GetDoubleLenient();
            Double M2000CManual03CycleInterval = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual03CycleInterval"].GetDoubleLenient();
            Double M2000CManual03Flare = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual03Flare"].GetDoubleLenient();
            Double M2000CManual03Interval = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual03Interval"].GetDoubleLenient();
            Double M2000CManual04Chaff = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual04Chaff"].GetDoubleLenient();
            Double M2000CManual04Cycle = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual04Cycle"].GetDoubleLenient();
            Double M2000CManual04CycleInterval = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual04CycleInterval"].GetDoubleLenient();
            Double M2000CManual04Flare = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual04Flare"].GetDoubleLenient();
            Double M2000CManual04Interval = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual04Interval"].GetDoubleLenient();
            Double M2000CManual05Chaff = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual05Chaff"].GetDoubleLenient();
            Double M2000CManual05Cycle = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual05Cycle"].GetDoubleLenient();
            Double M2000CManual05CycleInterval = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual05CycleInterval"].GetDoubleLenient();
            Double M2000CManual05Flare = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual05Flare"].GetDoubleLenient();
            Double M2000CManual05Interval = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual05Interval"].GetDoubleLenient();
            Double M2000CManual06Chaff = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual06Chaff"].GetDoubleLenient();
            Double M2000CManual06Cycle = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual06Cycle"].GetDoubleLenient();
            Double M2000CManual06CycleInterval = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual06CycleInterval"].GetDoubleLenient();
            Double M2000CManual06Flare = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual06Flare"].GetDoubleLenient();
            Double M2000CManual06Interval = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual06Interval"].GetDoubleLenient();
            Double M2000CManual07Chaff = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual07Chaff"].GetDoubleLenient();
            Double M2000CManual07Cycle = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual07Cycle"].GetDoubleLenient();
            Double M2000CManual07CycleInterval = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual07CycleInterval"].GetDoubleLenient();
            Double M2000CManual07Flare = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual07Flare"].GetDoubleLenient();
            Double M2000CManual07Interval = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual07Interval"].GetDoubleLenient();
            Double M2000CManual08Chaff = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual08Chaff"].GetDoubleLenient();
            Double M2000CManual08Cycle = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual08Cycle"].GetDoubleLenient();
            Double M2000CManual08CycleInterval = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual08CycleInterval"].GetDoubleLenient();
            Double M2000CManual08Flare = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual08Flare"].GetDoubleLenient();
            Double M2000CManual08Interval = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual08Interval"].GetDoubleLenient();
            Double M2000CManual09Chaff = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual09Chaff"].GetDoubleLenient();
            Double M2000CManual09Cycle = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual09Cycle"].GetDoubleLenient();
            Double M2000CManual09CycleInterval = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual09CycleInterval"].GetDoubleLenient();
            Double M2000CManual09Flare = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual09Flare"].GetDoubleLenient();
            Double M2000CManual09Interval = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual09Interval"].GetDoubleLenient();
            Double M2000CManual10Chaff = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual10Chaff"].GetDoubleLenient();
            Double M2000CManual10Cycle = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual10Cycle"].GetDoubleLenient();
            Double M2000CManual10CycleInterval = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual10CycleInterval"].GetDoubleLenient();
            Double M2000CManual10Flare = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual10Flare"].GetDoubleLenient();
            Double M2000CManual10Interval = optionsLuaText["options"]["plugins"]["DiCE M2000C"]["M2000CManual10Interval"].GetDoubleLenient();


            string[] luaExportString = {
                "local gettext = require(\"i_18n\")",
                "_ = gettext.translate",
                "",
                "programs = {}",
                "",
                "-- User Modifiable program",
                "programs[1] = {}",
                "programs[1][\"chaff\"]  = "+ M2000CManual01Chaff,
                "programs[1][\"flare\"]  = "+ M2000CManual01Flare,
                "programs[1][\"intv\"]   = "+ M2000CManual01Interval*100,
                "programs[1][\"cycle\"]  = "+ M2000CManual01Cycle,
                "programs[1][\"c_intv\"] = "+ M2000CManual01CycleInterval*100,
                "programs[1][\"panic\"]  = 0",
                "",
                "programs[2] = {}",
                "programs[2][\"chaff\"]  = "+ M2000CManual02Chaff,
                "programs[2][\"flare\"]  = "+ M2000CManual02Flare,
                "programs[2][\"intv\"]   = "+ M2000CManual02Interval*100,
                "programs[2][\"cycle\"]  = "+ M2000CManual02Cycle,
                "programs[2][\"c_intv\"] = "+ M2000CManual02CycleInterval*100,
                "programs[2][\"panic\"]  = 0",
                "",
                "programs[3] = {}",
                "programs[3][\"chaff\"]  = "+ M2000CManual03Chaff,
                "programs[3][\"flare\"]  = "+ M2000CManual03Flare,
                "programs[3][\"intv\"]   = "+ M2000CManual03Interval*100,
                "programs[3][\"cycle\"]  = "+ M2000CManual03Cycle,
                "programs[3][\"c_intv\"] = "+ M2000CManual03CycleInterval*100,
                "programs[3][\"panic\"]  = 0",
                "",
                "programs[4] = {}",
                "programs[4][\"chaff\"]  = "+ M2000CManual04Chaff,
                "programs[4][\"flare\"]  = "+ M2000CManual04Flare,
                "programs[4][\"intv\"]   = "+ M2000CManual04Interval*100,
                "programs[4][\"cycle\"]  = "+ M2000CManual04Cycle,
                "programs[4][\"c_intv\"] = "+ M2000CManual04CycleInterval*100,
                "programs[4][\"panic\"]  = 0",
                "",
                "programs[5] = {}",
                "programs[5][\"chaff\"]  = "+ M2000CManual05Chaff,
                "programs[5][\"flare\"]  = "+ M2000CManual05Flare,
                "programs[5][\"intv\"]   = "+ M2000CManual05Interval*100,
                "programs[5][\"cycle\"]  = "+ M2000CManual05Cycle,
                "programs[5][\"c_intv\"] = "+ M2000CManual05CycleInterval*100,
                "programs[5][\"panic\"]  = 0",
                "",
                "programs[6] = {}",
                "programs[6][\"chaff\"]  = "+ M2000CManual06Chaff,
                "programs[6][\"flare\"]  = "+ M2000CManual06Flare,
                "programs[6][\"intv\"]   = "+ M2000CManual06Interval*100,
                "programs[6][\"cycle\"]  = "+ M2000CManual06Cycle,
                "programs[6][\"c_intv\"] = "+ M2000CManual06CycleInterval*100,
                "programs[6][\"panic\"]  = 0",
                "",
                "programs[7] = {}",
                "programs[7][\"chaff\"]  = "+ M2000CManual07Chaff,
                "programs[7][\"flare\"]  = "+ M2000CManual07Flare,
                "programs[7][\"intv\"]   = "+ M2000CManual07Interval*100,
                "programs[7][\"cycle\"]  = "+ M2000CManual07Cycle,
                "programs[7][\"c_intv\"] = "+ M2000CManual07CycleInterval*100,
                "programs[7][\"panic\"]  = 0",
                "",
                "programs[8] = {}",
                "programs[8][\"chaff\"]  = "+ M2000CManual08Chaff,
                "programs[8][\"flare\"]  = "+ M2000CManual08Flare,
                "programs[8][\"intv\"]   = "+ M2000CManual08Interval*100,
                "programs[8][\"cycle\"]  = "+ M2000CManual08Cycle,
                "programs[8][\"c_intv\"] = "+ M2000CManual08CycleInterval*100,
                "programs[8][\"panic\"]  = 0",
                "",
                "programs[9] = {}",
                "programs[9][\"chaff\"]  = "+ M2000CManual09Chaff,
                "programs[9][\"flare\"]  = "+ M2000CManual09Flare,
                "programs[9][\"intv\"]   = "+ M2000CManual09Interval*100,
                "programs[9][\"cycle\"]  = "+ M2000CManual09Cycle,
                "programs[9][\"c_intv\"] = "+ M2000CManual09CycleInterval*100,
                "programs[9][\"panic\"]  = 0",
                "",
                "programs[10] = {}",
                "programs[10][\"chaff\"]  = "+ M2000CManual10Chaff,
                "programs[10][\"flare\"]  = "+ M2000CManual10Flare,
                "programs[10][\"intv\"]   = "+ M2000CManual10Interval*100,
                "programs[10][\"cycle\"]  = "+ M2000CManual10Cycle,
                "programs[10][\"c_intv\"] = "+ M2000CManual10CycleInterval*100,
                "programs[10][\"panic\"]  = 0",
                "",
                "need_to_be_closed = true" };
                //"--Exported via DiCE by Bailey " + System.DateTime.Now};


            //start of default settings for some future purpose. make sure to double check the numbers with a fresh file
            string[] luaExportStringDefaultSettings = {
                "local gettext = require(\"i_18n\")",
                "_ = gettext.translate",
                "",
                "programs = {}",
                "",
                "-- User Modifiable program",
                "programs[1] = {}",
                "programs[1][\"chaff\"]  = 6",
                "programs[1][\"flare\"]  = 0",
                "programs[1][\"intv\"]   = 50",
                "programs[1][\"cycle\"]  = 1",
                "programs[1][\"c_intv\"] = 0",
                "programs[1][\"panic\"]  = 0",
                "",
                "programs[2] = {}",
                "programs[2][\"chaff\"]  = 6",
                "programs[2][\"flare\"]  = 0",
                "programs[2][\"intv\"]   = 50",
                "programs[2][\"cycle\"]  = 2",
                "programs[2][\"c_intv\"] = 200",
                "programs[2][\"panic\"]  = 0",
                "",
                "programs[3] = {}",
                "programs[3][\"chaff\"]  = 6",
                "programs[3][\"flare\"]  = 0",
                "programs[3][\"intv\"]   = 50",
                "programs[3][\"cycle\"]  = 3",
                "programs[3][\"c_intv\"] = 200",
                "programs[3][\"panic\"]  = 0",
                "",
                "programs[4] = {}",
                "programs[4][\"chaff\"]  = 0",
                "programs[4][\"flare\"]  = 2",
                "programs[4][\"intv\"]   = 0",
                "programs[4][\"cycle\"]  = 1",
                "programs[4][\"c_intv\"] = 0",
                "programs[4][\"panic\"]  = 0",
                "",
                "programs[5] = {}",
                "programs[5][\"chaff\"]  = 1",
                "programs[5][\"flare\"]  = 1",
                "programs[5][\"intv\"]   = 0",
                "programs[5][\"cycle\"]  = 1",
                "programs[5][\"c_intv\"] = 0",
                "programs[5][\"panic\"]  = 0",
                "",
                "programs[6] = {}",
                "programs[6][\"chaff\"]  = 12",
                "programs[6][\"flare\"]  = 0",
                "programs[6][\"intv\"]   = 75",
                "programs[6][\"cycle\"]  = 1",
                "programs[6][\"c_intv\"] = 0",
                "programs[6][\"panic\"]  = 0",
                "",
                "programs[7] = {}",
                "programs[7][\"chaff\"]  = 20",
                "programs[7][\"flare\"]  = 0",
                "programs[7][\"intv\"]   = 25",
                "programs[7][\"cycle\"]  = 1",
                "programs[7][\"c_intv\"] = 0",
                "programs[7][\"panic\"]  = 0",
                "",
                "programs[8] = {}",
                "programs[8][\"chaff\"]  = 0",
                "programs[8][\"flare\"]  = 6",
                "programs[8][\"intv\"]   = 25",
                "programs[8][\"cycle\"]  = 1",
                "programs[8][\"c_intv\"] = 0",
                "programs[8][\"panic\"]  = 0",
                "",
                "programs[9] = {}",
                "programs[9][\"chaff\"]  = 20",
                "programs[9][\"flare\"]  = 6",
                "programs[9][\"intv\"]   = 25",
                "programs[9][\"cycle\"]  = 1",
                "programs[9][\"c_intv\"] = 0",
                "programs[9][\"panic\"]  = 0",
                "",
                "programs[10] = {}",
                "programs[10][\"chaff\"]  = 0",
                "programs[10][\"flare\"]  = 32",
                "programs[10][\"intv\"]   = 25",
                "programs[10][\"cycle\"]  = 1",
                "programs[10][\"c_intv\"] = 0",
                "programs[10][\"panic\"]  = 0",
                "",
                "need_to_be_closed = true" };
                //"--Exported via DiCE by Bailey " + System.DateTime.Now};
            //end of the default settings

            System.IO.Directory.CreateDirectory(cmdsLua_M2000C_FolderPath);

            try
            {
                System.IO.File.WriteAllLines(cmdsLua_M2000C_fullPath, luaExportString);
                //https://stackoverflow.com/questions/5920882/file-move-does-not-work-file-already-exists
                //playCompleteSound();

                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "M2000C CMS file exported.");
                richTextBox_log.ScrollToEnd();
            }
            catch (IOException h)
            {
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "DiCE could not write the M2000C CMS lua.");
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + h.Message);
                richTextBox_log.ScrollToEnd();
            }
        }

        /*
        private void readAndExportAH64DData()//TODO: do this after the CMS for the module is understood
        {
            //
            var optionsLuaText = LsonVars.Parse(File.ReadAllText(userOptionsLua_Full_pathWithExtention));
            Double AH64DManual01Chaff = optionsLuaText["options"]["plugins"]["DiCE AH64D"]["AH64DManual01Chaff"].GetDoubleLenient();
            //continue...

            string[] luaExportString = {
                "local gettext = require(\"i_18n\")",
                "_ = gettext.translate",
                "",
                "programs = {}",
                "",
                "-- User Modifiable program",
                "programs[1] = {}",
                "programs[1][\"chaff\"]  = "+ AH64DManual01Chaff,
                "programs[1][\"flare\"]  = "+ M2000CManual01Flare,
                "programs[1][\"intv\"]   = "+ M2000CManual01Interval*100,
                "programs[1][\"cycle\"]  = "+ M2000CManual01Cycle,
                "programs[1][\"c_intv\"] = "+ M2000CManual01CycleInterval*100,
                "programs[1][\"panic\"]  = 0",
                "",
                "programs[2] = {}",
                "programs[2][\"chaff\"]  = "+ M2000CManual02Chaff,
                "programs[2][\"flare\"]  = "+ M2000CManual02Flare,
                "programs[2][\"intv\"]   = "+ M2000CManual02Interval*100,
                "programs[2][\"cycle\"]  = "+ M2000CManual02Cycle,
                "programs[2][\"c_intv\"] = "+ M2000CManual02CycleInterval*100,
                "programs[2][\"panic\"]  = 0",
                "",
                "programs[3] = {}",
                "programs[3][\"chaff\"]  = "+ M2000CManual03Chaff,
                "programs[3][\"flare\"]  = "+ M2000CManual03Flare,
                "programs[3][\"intv\"]   = "+ M2000CManual03Interval*100,
                "programs[3][\"cycle\"]  = "+ M2000CManual03Cycle,
                "programs[3][\"c_intv\"] = "+ M2000CManual03CycleInterval*100,
                "programs[3][\"panic\"]  = 0",
                "",
                "programs[4] = {}",
                "programs[4][\"chaff\"]  = "+ M2000CManual04Chaff,
                "programs[4][\"flare\"]  = "+ M2000CManual04Flare,
                "programs[4][\"intv\"]   = "+ M2000CManual04Interval*100,
                "programs[4][\"cycle\"]  = "+ M2000CManual04Cycle,
                "programs[4][\"c_intv\"] = "+ M2000CManual04CycleInterval*100,
                "programs[4][\"panic\"]  = 0",
                "",
                "programs[5] = {}",
                "programs[5][\"chaff\"]  = "+ M2000CManual05Chaff,
                "programs[5][\"flare\"]  = "+ M2000CManual05Flare,
                "programs[5][\"intv\"]   = "+ M2000CManual05Interval*100,
                "programs[5][\"cycle\"]  = "+ M2000CManual05Cycle,
                "programs[5][\"c_intv\"] = "+ M2000CManual05CycleInterval*100,
                "programs[5][\"panic\"]  = 0",
                "",
                "programs[6] = {}",
                "programs[6][\"chaff\"]  = "+ M2000CManual06Chaff,
                "programs[6][\"flare\"]  = "+ M2000CManual06Flare,
                "programs[6][\"intv\"]   = "+ M2000CManual06Interval*100,
                "programs[6][\"cycle\"]  = "+ M2000CManual06Cycle,
                "programs[6][\"c_intv\"] = "+ M2000CManual06CycleInterval*100,
                "programs[6][\"panic\"]  = 0",
                "",
                "programs[7] = {}",
                "programs[7][\"chaff\"]  = "+ M2000CManual07Chaff,
                "programs[7][\"flare\"]  = "+ M2000CManual07Flare,
                "programs[7][\"intv\"]   = "+ M2000CManual07Interval*100,
                "programs[7][\"cycle\"]  = "+ M2000CManual07Cycle,
                "programs[7][\"c_intv\"] = "+ M2000CManual07CycleInterval*100,
                "programs[7][\"panic\"]  = 0",
                "",
                "programs[8] = {}",
                "programs[8][\"chaff\"]  = "+ M2000CManual08Chaff,
                "programs[8][\"flare\"]  = "+ M2000CManual08Flare,
                "programs[8][\"intv\"]   = "+ M2000CManual08Interval*100,
                "programs[8][\"cycle\"]  = "+ M2000CManual08Cycle,
                "programs[8][\"c_intv\"] = "+ M2000CManual08CycleInterval*100,
                "programs[8][\"panic\"]  = 0",
                "",
                "programs[9] = {}",
                "programs[9][\"chaff\"]  = "+ M2000CManual09Chaff,
                "programs[9][\"flare\"]  = "+ M2000CManual09Flare,
                "programs[9][\"intv\"]   = "+ M2000CManual09Interval*100,
                "programs[9][\"cycle\"]  = "+ M2000CManual09Cycle,
                "programs[9][\"c_intv\"] = "+ M2000CManual09CycleInterval*100,
                "programs[9][\"panic\"]  = 0",
                "",
                "programs[10] = {}",
                "programs[10][\"chaff\"]  = "+ M2000CManual10Chaff,
                "programs[10][\"flare\"]  = "+ M2000CManual10Flare,
                "programs[10][\"intv\"]   = "+ M2000CManual10Interval*100,
                "programs[10][\"cycle\"]  = "+ M2000CManual10Cycle,
                "programs[10][\"c_intv\"] = "+ M2000CManual10CycleInterval*100,
                "programs[10][\"panic\"]  = 0",
                "",
                "need_to_be_closed = true" };
            //"--Exported via DiCE by Bailey " + System.DateTime.Now};


            //start of default settings for some future purpose. make sure to double check the numbers with a fresh file
            string[] luaExportStringDefaultSettings = {
                "local gettext = require(\"i_18n\")",
                "_ = gettext.translate",
                "",
                "programs = {}",
                "",
                "-- User Modifiable program",
                "programs[1] = {}",
                "programs[1][\"chaff\"]  = 6",
                "programs[1][\"flare\"]  = 0",
                "programs[1][\"intv\"]   = 50",
                "programs[1][\"cycle\"]  = 1",
                "programs[1][\"c_intv\"] = 0",
                "programs[1][\"panic\"]  = 0",
                "",
                "programs[2] = {}",
                "programs[2][\"chaff\"]  = 6",
                "programs[2][\"flare\"]  = 0",
                "programs[2][\"intv\"]   = 50",
                "programs[2][\"cycle\"]  = 2",
                "programs[2][\"c_intv\"] = 200",
                "programs[2][\"panic\"]  = 0",
                "",
                "programs[3] = {}",
                "programs[3][\"chaff\"]  = 6",
                "programs[3][\"flare\"]  = 0",
                "programs[3][\"intv\"]   = 50",
                "programs[3][\"cycle\"]  = 3",
                "programs[3][\"c_intv\"] = 200",
                "programs[3][\"panic\"]  = 0",
                "",
                "programs[4] = {}",
                "programs[4][\"chaff\"]  = 0",
                "programs[4][\"flare\"]  = 2",
                "programs[4][\"intv\"]   = 0",
                "programs[4][\"cycle\"]  = 1",
                "programs[4][\"c_intv\"] = 0",
                "programs[4][\"panic\"]  = 0",
                "",
                "programs[5] = {}",
                "programs[5][\"chaff\"]  = 1",
                "programs[5][\"flare\"]  = 1",
                "programs[5][\"intv\"]   = 0",
                "programs[5][\"cycle\"]  = 1",
                "programs[5][\"c_intv\"] = 0",
                "programs[5][\"panic\"]  = 0",
                "",
                "programs[6] = {}",
                "programs[6][\"chaff\"]  = 12",
                "programs[6][\"flare\"]  = 0",
                "programs[6][\"intv\"]   = 75",
                "programs[6][\"cycle\"]  = 1",
                "programs[6][\"c_intv\"] = 0",
                "programs[6][\"panic\"]  = 0",
                "",
                "programs[7] = {}",
                "programs[7][\"chaff\"]  = 20",
                "programs[7][\"flare\"]  = 0",
                "programs[7][\"intv\"]   = 25",
                "programs[7][\"cycle\"]  = 1",
                "programs[7][\"c_intv\"] = 0",
                "programs[7][\"panic\"]  = 0",
                "",
                "programs[8] = {}",
                "programs[8][\"chaff\"]  = 0",
                "programs[8][\"flare\"]  = 6",
                "programs[8][\"intv\"]   = 25",
                "programs[8][\"cycle\"]  = 1",
                "programs[8][\"c_intv\"] = 0",
                "programs[8][\"panic\"]  = 0",
                "",
                "programs[9] = {}",
                "programs[9][\"chaff\"]  = 20",
                "programs[9][\"flare\"]  = 6",
                "programs[9][\"intv\"]   = 25",
                "programs[9][\"cycle\"]  = 1",
                "programs[9][\"c_intv\"] = 0",
                "programs[9][\"panic\"]  = 0",
                "",
                "programs[10] = {}",
                "programs[10][\"chaff\"]  = 0",
                "programs[10][\"flare\"]  = 32",
                "programs[10][\"intv\"]   = 25",
                "programs[10][\"cycle\"]  = 1",
                "programs[10][\"c_intv\"] = 0",
                "programs[10][\"panic\"]  = 0",
                "",
                "need_to_be_closed = true" };
            //"--Exported via DiCE by Bailey " + System.DateTime.Now};
            //end of the default settings

            System.IO.Directory.CreateDirectory(cmdsLua_AH64D_FolderPath);

            try
            {
                System.IO.File.WriteAllLines(cmdsLua_AH64D_fullPath, luaExportString);
                //https://stackoverflow.com/questions/5920882/file-move-does-not-work-file-already-exists
                //playCompleteSound();

                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "AH64D CMS file exported.");
                richTextBox_log.ScrollToEnd();
            }
            catch (IOException h)
            {
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "DiCE could not write the AH64D CMS lua.");
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + h.Message);
                richTextBox_log.ScrollToEnd();
            }
        }
        */
    }
}
