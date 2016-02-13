# Missing Projects Analysis for Visual Studio

If you have multiple .sln files that reference multiple .csproj projects and those projects reference each other then you might get yourself into trouble.
Solutions with missing projects will not build. It is easy to add a new reference to a project in one solution and forget about other solutions.

This repo consists:
 - msbuild target file
 - batch to run the target
 - batch to build validator as executable file
 - 3 sample solutions
 - 3 sample projects

Only one solution contains all required projects.

MSBuild 14.0 is required to run the target. Executable version of the validator can be used with systems that do not have MSBuild 14.0 installed as long as the relevant dll's copied over.

## Sample output
```
Build started 2016-02-06 01:27:36.
Project "C:\tooltest\validator.targets" on node 1 (default targets).
Check:
  Using MSBuildToolsPath [C:\Program Files (x86)\MSBuild\14.0\bin]
  Checking solution files:
  > C:\tooltest\SolutionAlfa\SolutionAlfa.sln
  > C:\tooltest\SolutionBeta.sln
  > C:\tooltest\SolutionGamma.sln
  Looking for missing projects in solution C:\tooltest\SolutionAlfa\SolutionAlfa.sln
  OK!
  Looking for missing projects in solution C:\tooltest\SolutionBeta.sln
C:\tooltest\validator.targets(84,5): error : Solution [C:\tooltest\SolutionBeta.sln] is missing referenced project [{D947B053-EB35-460F-9517-68AE452C8D33}]=[ProjectC]
  Looking for missing projects in solution C:\tooltest\SolutionGamma.sln
C:\tooltest\validator.targets(84,5): error : Solution [C:\tooltest\SolutionGamma.sln] is missing referenced project [{DFFE8C93-2C4F-4BBF-8F2C-F525516E759D}]=[ProjectB]
C:\tooltest\validator.targets(84,5): error : Solution [C:\tooltest\SolutionGamma.sln] is missing referenced project [{D947B053-EB35-460F-9517-68AE452C8D33}]=[ProjectC]
  Build continuing because "ContinueOnError" on the task "CheckSolutionForMissingProjects" is set to "ErrorAndContinue".
Done Building Project "C:\tooltest\validator.targets" (default targets) -- FAILED.


Build FAILED.

"C:\tooltest\validator.targets" (default target) (1) ->
(Check target) ->
  C:\tooltest\validator.targets(84,5): error : Solution [C:\tooltest\SolutionBeta.sln] is missing referenced project [{D947B053-EB35-460F-9517-68AE452C8D33}]=[ProjectC]
  C:\tooltest\validator.targets(84,5): error : Solution [C:\tooltest\SolutionGamma.sln] is missing referenced project [{DFFE8C93-2C4F-4BBF-8F2C-F525516E759D}]=[ProjectB]
  C:\tooltest\validator.targets(84,5): error : Solution [C:\tooltest\SolutionGamma.sln] is missing referenced project [{D947B053-EB35-460F-9517-68AE452C8D33}]=[ProjectC]

    0 Warning(s)
    3 Error(s)

Time Elapsed 00:00:00.62
```
