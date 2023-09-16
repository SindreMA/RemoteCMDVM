using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteCMDVM
{
    public partial class Service1 : ServiceBase
    {
        private System.Timers.Timer _timer;
        public int hj;
        public string lastCmd;
        public Service1()
        {
            InitializeComponent();


            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += _timer_Elapsed;
            

        }
        public void all()
        {
            if (hj == 1)
            {
                hj = 0;
                RegistryKey localKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
                try
                {
                    localKey.DeleteSubKeyTree(@"SOFTWARE\Microsoft\Virtual Machine\Guest");
                }
                catch  { }
                
                localKey = localKey.OpenSubKey(@"SOFTWARE\Microsoft\Virtual Machine\External");
               
                if (localKey != null)
                {
                    SendCmdValue = localKey.GetValue("SendCmd").ToString();
                }
                ;
                if (SendCmdValue != lastCmd)
                {
                    File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\Log_cmd.txt", DateTime.Now.ToLongTimeString() + "Running command " + SendCmdValue + Environment.NewLine);
                    try
                    {

                        //var powershell = PowerShell.Create();
                        //powershell.Commands.AddScript(SendCmdValue);
                        //powershell.Invoke();
                        string strCmdText = "/c " + SendCmdValue;
                        System.Diagnostics.Process.Start("Powershell.exe", strCmdText);

                    }
                    catch
                    {
                        File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\Log_cmd.txt", DateTime.Now.ToLongTimeString() + "ERROR running command " + SendCmdValue + Environment.NewLine);
                    }
                    localKey.Close();
                    RegistryKey localKey1 = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
                    localKey1 = localKey1.OpenSubKey(@"SOFTWARE\Microsoft\Virtual Machine\Guest", true);
                    localKey1.SetValue("ReturnCmd", SendCmdValue);
                    lastCmd = SendCmdValue;


                }
                hj = 1;
            }
        }
        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            all();
        }

      
        public void ondebug()
        {
            OnStart(null);
        }


        public string SendCmdValue;
        public string  s;
    
        protected override void OnStart(string[] args)
        {
            
            hj = 1;
            _timer.Start();
        }

        protected override void OnStop()
        {
            _timer.Stop();
        }
    }
}
