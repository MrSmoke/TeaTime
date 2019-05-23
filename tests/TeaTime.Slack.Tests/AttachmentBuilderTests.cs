namespace TeaTime.Slack.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common.Models;
    using Common.Models.Data;
    using Xunit;

    public class AttachmentBuilderTests
    {
        [Fact]
        public void BuildOptions_SixOptions_TwoAttachments()
        {
            var options = new List<Option>
            {
                new Option
                {
                    Id = 1,
                    Name = "test1"
                },
                new Option
                {
                    Id = 2,
                    Name = "test2"
                },
                new Option
                {
                    Id = 3,
                    Name = "test3"
                },
                new Option
                {
                    Id = 4,
                    Name = "test4"
                },
                new Option
                {
                    Id = 5,
                    Name = "test5"
                },
                new Option
                {
                    Id = 6,
                    Name = "test6"
                },
            };

            var attachments = AttachmentBuilder.BuildOptions(options).ToList();

            Assert.Equal(2, attachments.Count);

            var attachment1 = attachments[0];
            var attachment2 = attachments[1];

            Assert.Equal(5, attachment1.Actions.Count);
            Assert.Single(attachment2.Actions);

            Assert.Equal(options[5].Id.ToString(), attachment2.Actions[0].Value);
        }

    }
}
