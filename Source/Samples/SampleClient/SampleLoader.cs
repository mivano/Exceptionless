﻿#region Copyright 2014 Exceptionless

// This program is free software: you can redistribute it and/or modify it 
// under the terms of the GNU Affero General Public License as published 
// by the Free Software Foundation, either version 3 of the License, or 
// (at your option) any later version.
// 
//     http://www.gnu.org/licenses/agpl-3.0.html

#endregion

using System;
using System.IO;
using System.Xml;
using Exceptionless.Models;

namespace Exceptionless.SampleClient {
    public class SampleLoader {
        public SampleLoader(string sampleFolder) {
            SampleFolder = sampleFolder;
        }

        public string SampleFolder { get; private set; }

        public static string FindSamples() {
            string folder = Path.GetFullPath(@".\Samples");
            if (Directory.Exists(folder))
                return folder;

            folder = Path.GetFullPath(@"..\Samples");
            if (Directory.Exists(folder))
                return folder;

            folder = Path.GetFullPath(@"..\..\Samples");
            if (Directory.Exists(folder))
                return folder;

            return null;
        }

        public int Submitted { get; set; }

        public void Load() {
            var sampleRoot = new DirectoryInfo(SampleFolder);
            if (!sampleRoot.Exists)
                return;

            foreach (DirectoryInfo directory in sampleRoot.GetDirectories()) {
                FileInfo[] caseReports = directory.GetFiles("*.Error.xml", SearchOption.TopDirectoryOnly);
                if (caseReports.Length == 0)
                    continue;

                FileInfo caseReport = caseReports[0];
                LoadSample(caseReport);
            }
        }

        private void LoadSample(FileInfo caseReport) {
            if (!caseReport.Exists)
                return;

            Error report = null;

            using (FileStream fs = caseReport.OpenRead()) {
                using (XmlReader xr = new XmlTextReader(fs)) {
                    //report = Error.Load(xr);
                }
            }

            if (report == null)
                return;

            InitializeReport(report);

            ExceptionlessClient.Current.SubmitError(report);
            Submitted++;
        }

        private void InitializeReport(Error report) {
            if (report.ExtendedData == null)
                report.ExtendedData = new ExtendedDataDictionary();
            if (report.Tags == null)
                report.Tags = new TagSet();

            // reset values
            report.Id = String.Empty;
            report.OccurrenceDate = DateTimeOffset.Now;
            //report.ProjectId = ExceptionlessClient.Current.Configuration.ProjectId;
        }
    }
}