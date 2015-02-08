﻿

namespace CloudNotes.DesktopClient.Extensions.ImportFromWeb
{
    using CloudNotes.DesktopClient.Extensibility;
    using CloudNotes.DesktopClient.Extensibility.Exceptions;
    using CloudNotes.DesktopClient.Extensions.Properties;
    using CloudNotes.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    [Extension("D3C4C8BB-38E0-4EEE-9263-C83F3F4C39E0", "ImportFromWeb", typeof(ImportFromWebSettingProvider))]
    public sealed class ImportFromWebExtension : ToolExtension
    {
        public ImportFromWebExtension()
            : base(Resources.ImportFromWeb)
        { }

        protected async override void DoExecute(IShell shell)
        {
            try
            {
                var setting = this.SettingProvider.GetExtensionSetting<ImportFromWebSetting>();
                var dialog = new ImportFromWebDialog(setting);
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var title = HtmlUtilities.ExtractTitle(dialog.HtmlContent);
                    if (string.IsNullOrEmpty(title) || shell.ExistingNotesTitle.Contains(title))
                    {
                        var titleInputDialog = new TextInputBox(Resources.ImportFromWebInputTitlePrompt, title, 
                            new Tuple<Func<string, bool>, string>[]
                            {
                                new Tuple<Func<string, bool>, string>(txt => string.IsNullOrEmpty(txt), Resources.ImportFromWebEmptyTitleErrorMsg),
                                new Tuple<Func<string, bool>, string>(txt => shell.ExistingNotesTitle.Contains(txt), Resources.ImportFromWebDuplicatedTitleErrorMsg)
                            });
                        if (titleInputDialog.ShowDialog() == DialogResult.OK)
                        {
                            title = titleInputDialog.InputText;
                        }
                        else
                        {
                            return;
                        }
                    }
                    await shell.ImportNote(new Extensibility.Data.Note
                        {
                            Title = HtmlUtilities.ExtractTitle(dialog.HtmlContent),
                            Content = dialog.HtmlContent
                        });
                }
            }
            catch (Exception ex)
            {
                FrmExceptionDialog.ShowException(ex);
            }
        }

        public override string Manufacture
        {
            get
            {
                return "daxnet";
            }
        }

        public override string Description
        {
            get
            {
                return Resources.ImportFromWebDescription;
            }
        }
    }
}