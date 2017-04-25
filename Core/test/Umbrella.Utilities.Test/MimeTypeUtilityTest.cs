using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Umbrella.Utilities.Test
{
    public class MimeTypeUtilityTest
    {
        [Fact]
        public void GetMimeType_Empty()
        {
            Assert.Throws<ArgumentException>(() => MimeTypeUtility.GetMimeType(""));
        }

        [Fact]
        public void GetMimeType_Whitespace()
        {
            Assert.Throws<ArgumentException>(() => MimeTypeUtility.GetMimeType("    "));
        }

        [Fact]
        public void GetMimeType_Extension()
        {
            string mimeType = MimeTypeUtility.GetMimeType(".png");
            Assert.Equal("image/png", mimeType);
        }

        [Fact]
        public void GetMimeType_Extension_NoLeadingPeriod()
        {
            string mimeType = MimeTypeUtility.GetMimeType("png");
            Assert.Equal("image/png", mimeType);
        }

        [Fact]
        public void GetMimeType_Filename()
        {
            string mimeType = MimeTypeUtility.GetMimeType("test.png");
            Assert.Equal("image/png", mimeType);
        }

        [Fact]
        public void GetMimeType_Extension_Uppercase()
        {
            string mimeType = MimeTypeUtility.GetMimeType("PNG");
            Assert.Equal("image/png", mimeType);
        }

        [Fact]
        public void GetMimeType_Filepath()
        {
            string mimeType = MimeTypeUtility.GetMimeType("/path/test.png");
            Assert.Equal("image/png", mimeType);
        }

        [Fact]
        public void GetMimeType_FilenameWithPeriods()
        {
            string mimeType = MimeTypeUtility.GetMimeType("test.image.with.periods.png");
            Assert.Equal("image/png", mimeType);
        }
    }
}