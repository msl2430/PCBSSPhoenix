﻿using System;
using System.Diagnostics;
using Phoenix.Core.Constants;
using Phoenix.Core.Extensions;
using Phoenix.Medicaid.Service.Services;

namespace Phoenix.Medicaid.Service.Logging
{
    public interface ILoggingService
    {
        void LogEvent(string message, int eventType, bool savetoDatabase);
        void LogError(string message, string innerException);
    }

    public class LoggingService : MedicaidBaseService, ILoggingService
    {
        private bool SaveToSystem { get; set; }
        public LoggingService(EventLog medicaidEventLog)
        {
            SaveToSystem = true;
            MedicaidEventLog = medicaidEventLog;
        }
        public LoggingService()
        {
            SaveToSystem = false;
        }

        public void LogEvent(string message, int eventType, bool savetoDatabase)
        {
            if (savetoDatabase) 
                SaveEventToDatabase(message, eventType);
            if (SaveToSystem)
                SaveEventToSystem(message, eventType);
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsDebug"])) 
                SaveEventToConsole(message, eventType);
        }

        public void LogError(string message, string innerException)
        {
            LogEvent(string.Format("{0} Inner Exception: {1}", message, innerException), EventTypes.Events.ApplicationError.ToInt(), true);
        }

        /// <summary>
        /// Write event to console (Testing)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="eventType"></param>
        private void SaveEventToConsole(string message, int eventType)
        {
            Console.WriteLine("({0}) {1}: {2}", "Medicaid Service", DateTime.Now.ToString("HH:mm:ss"), message);
        }

        /// <summary>
        /// Save event to Database
        /// </summary>
        /// <param name="message"></param>
        /// <param name="eventType"></param>
        private void SaveEventToDatabase(string message, int eventType)
        {
            ApplicationEventService.SaveEvent(message, ApplicationConstants.Applications.PhoenixMedicaidService, eventType);
        }

        /// <summary>
        /// Save event to system event log
        /// </summary>
        /// <param name="message"></param>
        /// <param name="eventType"></param>
        private void SaveEventToSystem(string message, int eventType)
        {
            if (!EventLog.SourceExists("Phoenix Medicaid Service"))
            {
                EventLog.CreateEventSource("Phoenix Medicaid Service", "Phoenix Medicaid Service Log");
            }
            MedicaidEventLog.Source = "Phoenix Medicaid Service";
            MedicaidEventLog.Log = "Phoenix Medicaid Service Log";
            if(MedicaidEventLog != null) 
                MedicaidEventLog.WriteEntry(message);
        }
    }
}
