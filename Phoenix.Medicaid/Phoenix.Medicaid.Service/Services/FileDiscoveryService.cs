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
using Phoenix.Models.NHibernate;
using Phoenix.Medicaid.Service.Logging;
using Phoenix.Core.Constants;
using Phoenix.Core.Extensions;

namespace Phoenix.Medicaid.Service.Services
{
    public class FileDiscoveryService : BaseTaskService
    {
        private ILoggingService LoggingService { get; set; }
        private readonly DirectoryInfo _fileDirectoryInfo = new DirectoryInfo(@"D:\Social Services\PCBSSPhoenix\Test Files");
        private const int FileDiscoveryWaitTime = 5000;// 300000;

        public FileDiscoveryService(ILoggingService loggingService)
        {
            LoggingService = loggingService;
        }

        public override void TaskToRun()
        {
            if (isRunning)
                throw new Exception("File Discovery Task already running!");

            isRunning = true;
            Task.Run(() => FileDiscovery(CancellationTokenSource.Token));
        }

        public void CancelTask()
        {
            LoggingService.LogEvent("File Discovery cancelled.", EventTypes.MedicaidEvents.FileDiscoveryCancelled.ToInt(), false);
        }

        private void FileDiscovery(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!_fileDirectoryInfo.Exists || !_fileDirectoryInfo.GetFiles().Any()) continue;
                LoggingService.LogEvent(string.Format("Found {0} files to process.", _fileDirectoryInfo.GetFiles("*.csv").Count()), EventTypes.MedicaidEvents.FileDiscoveryFilesFound.ToInt(), false);
                foreach (var file in _fileDirectoryInfo.GetFiles("*.csv"))
                {
                    LoggingService.LogEvent(string.Format("Processing file: {0}", file.FullName), EventTypes.MedicaidEvents.FileDiscoveryStopped.ToInt(), false);
                    if(file.Name.Contains("61"))
                        ProcessFile(file); 
                    base.CancelTask();
                }
                Thread.Sleep(FileDiscoveryWaitTime);
            }
            LoggingService.LogEvent("Cancelled FileDiscoveryTask.", EventTypes.MedicaidEvents.FileDiscoveryStopped.ToInt(), false);
        }

        private void ProcessFile(FileInfo file)
        {
            var stream = new StreamReader(file.FullName, Encoding.Default);
            if (stream.Peek() != -1) stream.ReadLine();
            var i = 0;
            while (stream.Peek() != -1)
            {
                var record = stream.ReadLine();
                var opt = new Opt61Form(MedicaidFormFieldService.Current.GetMedicaidFields().Where(f => f.MedicaidFormId == FormConstants.MedicaidForms.Opt61).ToList());
                opt.PopulateFromCsv(record);
                if (opt != null && !opt.CaseNumber.IsFieldEmpty())
                {
                    LoggingService.LogEvent(string.Format("Adding Opt 61 Case {0} to the queue.", opt.CaseNumber.Data), EventTypes.MedicaidEvents.AddCaseToQueue.ToInt(), false);
                    var opt61Queue = opt.ToOpt61Queue();
                    NHibernateHelper.CurrentSession.SaveOrUpdate(opt61Queue);
                    NHibernateHelper.FlushAndCommit();
                }                
                i++;
            }
            stream.Close();
        }
    }
}
