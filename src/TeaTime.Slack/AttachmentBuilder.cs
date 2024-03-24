namespace TeaTime.Slack
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.Models.Data;
    using Models;

    internal static class AttachmentBuilder
    {
        internal static IEnumerable<Attachment> BuildOptions(IEnumerable<Option> options)
        {
            const int maxButtons = 5;

            var attachments = new List<Attachment>();
            var actions = options.Select(o => CreateButton(o)).ToList();

            //Add initial attachment
            var attachment = new Attachment
            {
                Text = "Select an option to join this round!",
                CallBackId = "teatime"
            };
            attachment.Actions.AddRange(actions.Take(maxButtons));
            attachments.Add(attachment);

            if (actions.Count < maxButtons)
                return attachments;

            //Add any additional attachments
            for (var i = 1; i <= (actions.Count) / maxButtons; i++)
            {
                var additionalAttachment = new Attachment
                {
                    CallBackId = "teatime"
                };
                additionalAttachment.Actions.AddRange(actions.Skip(i * maxButtons).Take(maxButtons));
                attachments.Add(additionalAttachment);
            }

            return attachments;
        }

        private static Button CreateButton(Option option, string name = "tea-option")
        {
            return new Button
            {
                Name = name,
                Text = option.Name,
                Value = option.Id.ToString()
            };
        }
    }
}
