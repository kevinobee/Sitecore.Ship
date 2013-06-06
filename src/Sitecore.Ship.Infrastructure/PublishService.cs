using System;
using System.Collections.Generic;
using System.Linq;

using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.Globalization;

using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;

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

        public void Run(PublishParameters publishParameters)
        {
            var publishingMode = publishParameters.Mode.ToLower();

            if (!_publishingActions.ContainsKey(publishingMode))
            {
                throw new InvalidOperationException(string.Format("Invalid publishing mode ({0})", publishingMode));
            }

            PublishingTask(_publishingActions[publishingMode], publishParameters);
        }

        private static void PublishingTask(Func<Database, Database[], Language[], Handle> publishType, PublishParameters publishParameters)
        {
            using (new SecurityModel.SecurityDisabler())
            {
                var master = Sitecore.Configuration.Factory.GetDatabase(publishParameters.Source);
                var targetDBs = publishParameters.Targets.Select(Sitecore.Configuration.Factory.GetDatabase).ToArray();
                var languages = publishParameters.Languages.Select(LanguageManager.GetLanguage).ToArray();

                publishType(master, targetDBs, languages);
            }
        }
    }
}