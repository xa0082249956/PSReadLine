using Microsoft.PowerShell;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;

namespace Test
{
    public partial class ReadLine
    {
        [SkippableFact]
        public void AddLine()
        {
            TestSetup(KeyMode.Cmd);

            Test("1\n2", Keys('1', _.Shift_Enter, '2'));
        }
    }
}
