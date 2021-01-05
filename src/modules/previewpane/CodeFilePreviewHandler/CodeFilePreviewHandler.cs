// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using Common;
using Microsoft.PowerToys.Telemetry;

namespace Microsoft.PowerToys.PreviewHandler.CodeFile
{
    /// <summary>
    /// Implementation of preview handler for markdown files.
    /// </summary>
    [Guid("3DB1B3B5-9C0C-460C-AA0A-787150D490F9")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public class CodeFilePreviewHandler : FileBasedPreviewHandler, IDisposable
    {
        private CodeFilePreviewHandlerControl _codeFilePreviewHandlerControl;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFilePreviewHandler"/> class.
        /// </summary>
        public CodeFilePreviewHandler()
        {
            Initialize();
        }

        /// <inheritdoc />
        public override void DoPreview()
        {
            _codeFilePreviewHandlerControl.DoPreview(FilePath);
        }

        /// <inheritdoc />
        protected override IPreviewHandlerControl CreatePreviewHandlerControl()
        {
            // PowerToysTelemetry.Log.WriteEvent(new Telemetry.Events.CodeFileFileHandlerLoaded());
            _codeFilePreviewHandlerControl = new CodeFilePreviewHandlerControl();

            return _codeFilePreviewHandlerControl;
        }

        /// <summary>
        /// Disposes objects
        /// </summary>
        /// <param name="disposing">Is Disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _codeFilePreviewHandlerControl.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
