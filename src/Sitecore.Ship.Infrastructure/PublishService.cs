using System;
using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.Ship.Core;

namespace Sitecore.Ship.Infrastructure
{
    public class PublishService : IPublishService
    {
        private readonly Dictionary<string, Func<Database, Database[], Language[], Handle>> _publishingActions;

        public PublishService()
        {
            _publishingActions = new Dictionary<string, Func<Database, Database[], Language[], Handle>>
                {
                    { "full",           Publishing.PublishManager.Republish },
                    { "smart",          Publishing.PublishManager.PublishSmart },
                    { "incremental",    Publishing.PublishManager.PublishIncremental }
                };
        }

        public void Run(string mode)
        {
            var publishingMode = mode.ToLower();

            if (!_publishingActions.ContainsKey(publishingMode))
            {
                throw new InvalidOperationException(string.Format("Invalid publishing mode ({0})", mode));
            }

            PublishingTask(_publishingActions[publishingMode]);
        }

        private static void PublishingTask(Func<Database, Database[], Language[], Handle> publishType)
        {
            using (new SecurityModel.SecurityDisabler())
            {
                var master = Sitecore.Configuration.Factory.GetDatabase("master");
                var targetDBs = new[] { Sitecore.Configuration.Factory.GetDatabase("web") };
                var languages = new[] { LanguageManager.GetLanguage("en") };

                publishType(master, targetDBs, languages);
            }
        }
    }
}