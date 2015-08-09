﻿// =======================================================================================================
//
//    ,uEZGZX  LG                             Eu       iJ       vi                                              
//   BB7.  .:  uM                             8F       0BN      Bq             S:                               
//  @X         LO    rJLYi    :     i    iYLL XJ       Xu7@     Mu    7LjL;   rBOii   7LJ7    .vYUi             
// ,@          LG  7Br...SB  vB     B   B1...7BL       0S i@,   OU  :@7. ,u@   @u.. :@:  ;B  LB. ::             
// v@          LO  B      Z0 i@     @  BU     @Y       qq  .@L  Oj  @      5@  Oi   @.    MB U@                 
// .@          JZ :@      :@ rB     B  @      5U       Eq    @0 Xj ,B      .B  Br  ,B:rv777i  :0ZU              
//  @M         LO  @      Mk :@    .@  BL     @J       EZ     GZML  @      XM  B;   @            Y@             
//   ZBFi::vu  1B  ;B7..:qO   BS..iGB   BJ..:vB2       BM      rBj  :@7,.:5B   qM.. i@r..i5. ir. UB             
//     iuU1vi   ,    ;LLv,     iYvi ,    ;LLr  .       .,       .     rvY7:     rLi   7LLr,  ,uvv:  
//
//
// Copyright 2014-2015 daxnet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// =======================================================================================================

namespace CloudNotes.DesktopClient.Extensibility.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;
    using CloudNotes.Infrastructure;

    /// <summary>
    /// Represents the Extension Manager that registers and manages the extensions.
    /// </summary>
    internal sealed class ExtensionManager : ExternalResourceManager<Extension>
    {

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionManager"/> class.
        /// </summary>
        /// <param name="path">The path which contains the extension assemblies.</param>
        public ExtensionManager(string path)
            : base(path, Constants.ExtensionFileSearchPattern)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionManager"/> class.
        /// </summary>
        public ExtensionManager()
            : this(Path.Combine(Application.StartupPath, Constants.ExtensionFolderName))
        {

        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets all the tool extensions.
        /// </summary>
        /// <value>
        /// The tool extensions.
        /// </value>
        public IEnumerable<ToolExtension> ToolExtensions
        {
            get
            {
                return this.Resources.Values.Where(p => p.GetType().IsSubclassOf(typeof(ToolExtension))).Select(p => (ToolExtension)p);
            }
        }

        /// <summary>
        /// Gets all the export extensions.
        /// </summary>
        /// <value>
        /// The export extensions.
        /// </value>
        public IEnumerable<ExportExtension> ExportExtensions
        {
            get
            {
                return this.Resources.Values.Where(p => p.GetType().IsSubclassOf(typeof(ExportExtension))).Select(p => (ExportExtension)p);
            }
        }

        /// <summary>
        /// Gets all of the registered extensions.
        /// </summary>
        /// <value>
        /// All extensions.
        /// </value>
        public IEnumerable<KeyValuePair<Guid, Extension>> AllExtensions
        {
            get
            {
                return this.Resources;
            }
        }

        #endregion

        protected override IEnumerable<Extension> LoadResources(string fileName)
        {
            var assembly = Assembly.LoadFrom(fileName);
            var result = new List<Extension>();
            foreach (var type in assembly.GetExportedTypes())
            {
                if (type.IsDefined(typeof (ExtensionAttribute)) &&
                    type.IsSubclassOf(typeof (Extension)))
                {
                    try
                    {
                        var extensionLoaded = (Extension) Activator.CreateInstance(type);
                        this.OnResourceLoaded(extensionLoaded);
                        result.Add(extensionLoaded);
                    }
                    catch
                    {
                    }
                }
            }
            return result;
        }
    }
}