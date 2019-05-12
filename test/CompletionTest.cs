using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using Microsoft.PowerShell;
using Xunit;

namespace Test
{
    public partial class ReadLine
    {
        [SkippableFact]
        public void TabComplete()
        {
            TestSetup(KeyMode.Cmd);

            Test("$true", Keys(
                "$tr",
                _.Tab,
                CheckThat(() => AssertCursorLeftIs(5))));

            // Validate no change on no match
            Test("$zz", Keys(
                "$zz",
                _.Tab,
                CheckThat(() => AssertCursorLeftIs(3))));

            Test("$this", Keys(
                "$t",
                _.Tab,
                CheckThat(() => AssertLineIs("$thing")),
                _.Tab,
                CheckThat(() => AssertLineIs("$this")),
                _.Tab,
                CheckThat(() => AssertLineIs("$true")),
                _.Shift_Tab,
                CheckThat(() => AssertLineIs("$this"))));
        }
    }
}
