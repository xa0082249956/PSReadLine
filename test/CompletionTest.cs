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

        internal static CommandCompletion MockedCompleteInput(string input, int cursor, Hashtable options, PowerShell powerShell)
        {
            var ctor = typeof (CommandCompletion).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null,
                new [] {typeof (Collection<CompletionResult>), typeof (int), typeof (int), typeof (int)}, null);

            var completions = new Collection<CompletionResult>();
            const int currentMatchIndex = -1;
            var replacementIndex = 0;
            var replacementLength = 0;
            switch (input)
            {
            case "$t":
                replacementIndex = 0;
                replacementLength = 2;
                completions.Add(new CompletionResult("$thing"));
                completions.Add(new CompletionResult("$this"));
                completions.Add(new CompletionResult("$true"));
                break;
            case "$tr":
                replacementIndex = 0;
                replacementLength = 3;
                completions.Add(new CompletionResult("$true"));
                break;
            case "psvar":
                replacementIndex = 0;
                replacementLength = 5;
                completions.Add(new CompletionResult("$pssomething"));
                break;
            case "ambig":
                replacementIndex = 0;
                replacementLength = 5;
                completions.Add(new CompletionResult("ambiguous1"));
                completions.Add(new CompletionResult("ambiguous2"));
                completions.Add(new CompletionResult("ambiguous3"));
                break;
            case "Get-Many":
                replacementIndex = 0;
                replacementLength = 8;
                for (int i = 0; i < 15; i++)
                {
                    completions.Add(new CompletionResult("Get-Many" + i));
                }
                break;
            case "Get-Tooltips":
                replacementIndex = 0;
                replacementLength = 12;
                completions.Add(new CompletionResult("something really long", "item1", CompletionResultType.Command, "useful description goes here"));
                break;
            case "Get-Directory":
                replacementIndex = 0;
                replacementLength = 13;
                completions.Add(new CompletionResult("abc", "abc", CompletionResultType.ProviderContainer, "abc"));
                completions.Add(new CompletionResult("'e f'", "'e f'", CompletionResultType.ProviderContainer, "'e f'"));
                completions.Add(new CompletionResult("a", "a", CompletionResultType.ProviderContainer, "a"));
                completions.Add(new CompletionResult("'a b" + Path.DirectorySeparatorChar + "'", "a b" + Path.DirectorySeparatorChar + "'", CompletionResultType.ProviderContainer, "a b" + Path.DirectorySeparatorChar + "'"));
                completions.Add(new CompletionResult("\"a b" + Path.DirectorySeparatorChar + "\"", "\"a b" + Path.DirectorySeparatorChar + "\"", CompletionResultType.ProviderContainer, "\"a b" + Path.DirectorySeparatorChar + "\""));
                break;
            case "invalid result 1":
                replacementIndex = -1;
                replacementLength = 1;
                completions.Add(new CompletionResult("result"));
                break;
            case "invalid result 2":
                replacementIndex = 0;
                replacementLength = -1;
                completions.Add(new CompletionResult("result"));
                break;
            case "invalid result 3":
                replacementIndex = int.MaxValue;
                replacementLength = 1;
                completions.Add(new CompletionResult("result"));
                break;
            case "invalid result 4":
                replacementIndex = 0;
                replacementLength = int.MaxValue;
                completions.Add(new CompletionResult("result"));
                break;
            case "ls -H":
                replacementIndex = cursor;
                replacementLength = 0;
                completions.Add(new CompletionResult("idden"));
                break;
            case "none":
                break;
            }

            return (CommandCompletion)ctor.Invoke(
                new object[] {completions, currentMatchIndex, replacementIndex, replacementLength});
        }
    }
}
