using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Phoenix.Core;
using Phoenix.Core.Services;
using Phoenix.Medicaid.Models.OptForms;
using Phoenix.Models.Constants;

namespace Phoenix.Medicaid.Service.Services
{
    public class FileDiscoveryService : BaseTaskService
    {
        private readonly DirectoryInfo _fileDirectoryInfo = new DirectoryInfo(@"X:\Documents\PCBSSPhoenix\Test Files");
        private const int FileDiscoveryWaitTime = 5000;// 300000;

        public override void TaskToRun()
        {
            if (isRunning)
                throw new Exception("File Discovery Task already running!");

            isRunning = true;
            Task.Run(() => FileDiscovery(CancellationTokenSource.Token));
        }

        private void FileDiscovery(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!_fileDirectoryInfo.Exists || !_fileDirectoryInfo.GetFiles().Any()) continue;
                Console.WriteLine("Found {0} files to process.", _fileDirectoryInfo.GetFiles("*.csv").Count());
                foreach (var file in _fileDirectoryInfo.GetFiles("*.csv"))
                {
                    Console.WriteLine("Processing file: {0}", file.FullName);
                    if(file.Name.Contains("61"))
                        ProcessFile(file);                    
                }
                Thread.Sleep(FileDiscoveryWaitTime);
            }
            Console.WriteLine("Cancelled FileDiscoveryTask.");
        }

        private void ProcessFile(FileInfo file)
        {
            var stream = new StreamReader(file.FullName, Encoding.Default);
            if (stream.Peek() != -1) stream.ReadLine();
            var i = 0;
            while (stream.Peek() != -1 && i < 4)
            {
                var record = stream.ReadLine();
                var opt = new Opt61Form(MedicaidFormFieldService.Current.GetMedicaidFields().Where(f => f.MedicaidFormId == FormConstants.MedicaidForms.Opt61).ToList());
                opt.PopulateFromCsv(record);
                Console.WriteLine("***************************");
                i++;
            }
            stream.Close();
        }
    }
}
