﻿using System;
using System.Collections.Generic;
using System.Linq;
using TVProgViewer.Core;
using TVProgViewer.Core.Infrastructure;

namespace TVProgViewer.Services.Common
{
    /// <summary>
    ///  Maintenance service
    /// </summary>
    public partial class MaintenanceService : IMaintenanceService
    {
        #region Fields

        private readonly ITvProgFileProvider _fileProvider;

        #endregion

        #region Ctor

        public MaintenanceService(ITvProgFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get directory path for backs
        /// </summary>
        /// <param name="ensureFolderCreated">A value indicating whether a directory should be created if it doesn't exist</param>
        /// <returns></returns>
        protected virtual string GetBackupDirectoryPath(bool ensureFolderCreated = true)
        {
            var path = _fileProvider.GetAbsolutePath(TvProgCommonDefaults.DbBackupsPath);
            if (ensureFolderCreated)
                _fileProvider.CreateDirectory(path);
            return path;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all backup files
        /// </summary>
        /// <returns>Backup file collection</returns>
        public virtual IList<string> GetAllBackupFiles()
        {
            var path = GetBackupDirectoryPath();

            if (!_fileProvider.DirectoryExists(path))
                throw new TvProgException("Backup directory not exists");

            return _fileProvider.GetFiles(path, $"*.{TvProgCommonDefaults.DbBackupFileExtension}")
                .OrderByDescending(p => _fileProvider.GetLastWriteTime(p)).ToList();
        }

        /// <summary>
        /// Returns the path to the backup file
        /// </summary>
        /// <param name="backupFileName">The name of the backup file</param>
        /// <returns>The path to the backup file</returns>
        public virtual string GetBackupPath(string backupFileName)
        {
            return _fileProvider.Combine(GetBackupDirectoryPath(), backupFileName);
        }

        /// <summary>
        /// Creates a path to a new database backup file
        /// </summary>
        /// <returns>Path to a new database backup file</returns>
        public virtual string CreateNewBackupFilePath()
        {
            return _fileProvider.Combine(GetBackupDirectoryPath(), $"database_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{CommonHelper.GenerateRandomDigitCode(10)}.{TvProgCommonDefaults.DbBackupFileExtension}");
        }

        #endregion
    }
}